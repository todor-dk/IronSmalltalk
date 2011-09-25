/*
 * **************************************************************************
 *
 * Copyright (c) The IronSmalltalk Project. 
 *
 * This source code is subject to terms and conditions of the 
 * license agreement found in the solution directory. 
 * See: $(SolutionDir)\License.htm ... in the root of this distribution.
 * By using this source code in any fashion, you are agreeing 
 * to be bound by the terms of the license agreement.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * **************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IronSmalltalk.AstJitCompiler.Internals;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Internal;
using IronSmalltalk.Runtime.CodeGeneration.Bindings;
using IronSmalltalk.Runtime.Execution.Internals;
using System.Dynamic;
using IronSmalltalk.Runtime.Execution.Internals.Primitives;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.Runtime.CodeGeneration.Visiting
{
    /// <summary>
    /// Encoder visitor for visiting and generating primitive calls.
    /// </summary>
    /// <remarks>
    /// This is the place where primitive calls are encoded to expressions and where most of the interop to the .Net framework happens.
    /// </remarks>
    public partial class PrimitiveCallVisitor : NestedEncoderVisitor<List<Expression>>, IPrimitiveClient
    {
        /// <summary>
        /// Indicates if there is Smalltalk fallback code after the primitive call.
        /// </summary>
        public bool HasFallbackCode { get; private set; }

        /// <summary>
        /// The enclosing method visitor that created this visitor and defines the context of this visitor.
        /// </summary>
        public MethodVisitor MethodVisitor { get; private set; }

        public PrimitiveCallVisitor(MethodVisitor enclosingVisitor, bool hasFallbackCode)
            : base(enclosingVisitor)
        {
            this.HasFallbackCode = hasFallbackCode;
            this.MethodVisitor = enclosingVisitor;
        }

        /// <summary>
        /// Visits the Primitive Call node.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <remarks>
        /// This is the place where primitive calls are encoded to expressions and where most of the interop to the .Net framework happens.
        /// </remarks>
        public override List<Expression> VisitPrimitiveCall(Compiler.SemanticNodes.PrimitiveCallNode node)
        {
            // Large case with the API conventions we support;
            // Each generates an Expression to perform the primitive call.
            Expression primitiveCall;
            try
            {
                primitiveCall = this.GeneratePrimitiveExpression(node);
            }
            catch (CodeGenerationException ex)
            {
                ex.SetNode(node);
                throw;
            }

            // We need to handle void returns, because Smalltalk always needs a valid receiver.
            NameBinding selfBinding = this.MethodVisitor.GetBinding(SemanticConstants.Self);
            if (primitiveCall.Type == typeof(void))
                primitiveCall = Expression.Block(primitiveCall, selfBinding.GenerateReadExpression(this));
            else if (primitiveCall.Type != typeof(object))
                primitiveCall = Expression.Convert(primitiveCall, typeof(object));
            // A successful primitive call must return directly without executing any other statements.
            primitiveCall = this.Return(primitiveCall);

            List<Expression> result = new List<Expression>();
            if (this.HasFallbackCode)
            {
                // This is the case, where some Smalltalk fall-back code follows the primitive call.
                // In this case, we encapsulate the primitive call in a try-catch block, similar to:
                //      object _exception;          // optional ... defined by the Smalltalk method
                //      try
                //      {
                //          return (object) primitiveCall();
                //      } catch (Exception exception) {
                //          _exception = exception;         // optional ... only if "_exception" variable is declared.
                //      };
                Expression handler;
                // This is the special hardcoded temp variable we are looking for.
                NameBinding exceptionVariable = this.MethodVisitor.GetLocalVariable("_exception");
                ParameterExpression expetionParam = Expression.Parameter(typeof(Exception), "exception");
                if (!exceptionVariable.IsErrorBinding && (exceptionVariable is IAssignableBinding))
                    // Case of handler block that sets the "_exception" temporary variable and has "self" as the last statement.
                    handler = Expression.Block(
                        ((IAssignableBinding)exceptionVariable).GenerateAssignExpression(expetionParam, this),
                        Expression.Convert(selfBinding.GenerateReadExpression(this), typeof(object)));
                else
                    // Case of handler block that has "self" as the one and only statement
                    handler = Expression.Convert(selfBinding.GenerateReadExpression(this), typeof(object));
                // Create the try/catch block.
                result.Add(Expression.TryCatch(primitiveCall,
                    Expression.Catch(typeof(BlockResult), Expression.Rethrow(typeof(object))),
                    Expression.Catch(expetionParam, handler)));
            }
            else
            {
                // This is the case, where none Smalltalk fall-back code follows the primitive call.
                // The API call is called directly without any encapsulation in a try-catch block, similar to:
                //      return (object) primitiveCall();
                // If it fails, it fails and an exception is thrown. The sender of the Smalltalk method 
                // is responsible for handling exceptions manually.
                result.Add(primitiveCall);
            }

            return result;
        }

        /// <summary>
        /// This generates the expression needed to do a primitive call. This is the main worker method!
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>
        /// BNF for primitives:
        ///     Primitive ::= BuiltInPrimitive | InvokeStaticMethod | InvokeInstanceMethod | 
        ///         InvokeConstructor | InvokeGetProperty | InvokeSetProperty | GetFieldValue | SetFieldValue
        ///         
        ///     BuiltInPrimitive        ::= '<' 'primitive'     Primitive_Name              Prim_Arg*               '>'
        ///     InvokeStaticMethod      ::= '<' 'static'    	Defining_Type	Method_Name Arg_Type*	            '>'
        ///     InvokeInstanceMethod    ::= '<' 'call'	        Defining_Type	Method_Name	This_Type	Arg_Type*   '>'
        ///     InvokeConstructor       ::= '<' 'ctor'	        Defining_Type		        Arg_Type*	            '>'
        ///     InvokeGetProperty       ::= '<' 'get_property'	Defining_Type	Prop_Name	Arg_Type*	Return_Type '>'
        ///     InvokeSetProperty       ::= '<' 'set_property'	Defining_Type	Prop_Name	Arg_Type*	Return_Type '>'
        ///     GetFieldValue           ::= '<' 'get_field'	    Defining_Type	Field_Name	Arg_Type*	            '>'
        ///     SetFieldValue           ::= '<' 'set_field'	    Defining_Type	Field_Name	Arg_Type*	            '>'
        ///
        ///     Primitive_Name  ::= "See BuiltInPrimitivesEnum"
        ///     Prim_Arg        ::= "Argument, depending on the value of Primitive_Name"
        ///     Defining_Type   ::= Type_Name           "The type where to look up the respective member"
        ///     Method_Name     ::= "Case-sensitive member (method) name. See Type.GetMethod()"
        ///     Prop_Name       ::= "Case-sensitive member (property) name. See Type.GetProperty()"
        ///     Field_Name      ::= "Case-sensitive member (property) name. See Type.GetField()"
        ///     Arg_Type        ::= Type_Name           "The type of the member argument"
        ///     This_Type       ::= 'this' | Type_Name  "The type of the this argument"
        ///     Return_Type     ::= Type_Name           "The type of the property return value"
        ///     
        ///     Type_Name       ::= CSharp_TypeName | CorLib_TypeName | IST_TypeName | Qualified_TypeName
        ///     CSharp_TypeName     ::= "One of the C# build-in type (aliases), e.g. 'bool', 'int', 'float' etc."
        ///     CorLib_TypeName     ::= "Name of a type in mscorlib. No need for assembly information"
        ///     IST_TypeName        ::= '_' Name    "Typename prefixed with _ (underscore), for cenvenience for not having to include assembly info etc."
        ///     Qualified_TypeName  ::= "Assembly quaified type name. Used for everything else than the above 3 categories."
        ///     
        /// Number of primitive parameters:
        ///     BuiltInPrimitive        1+  (Primitive_Name + Prim_Arg*)
        ///     InvokeStaticMethod      2+  (Defining_Type + Method_Name + Arg_Type*)
        ///     InvokeInstanceMethod    3+  (Defining_Type + Method_Name + This_Type + Arg_Type*)
        ///     InvokeConstructor       1+  (Defining_Type + Arg_Type*)
        ///     InvokeGetProperty       3+  (Defining_Type + Prop_Name + Return_Type + Arg_Type*)
        ///     InvokeSetProperty       3+  (Defining_Type + Prop_Name + Return_Type + Arg_Type*)
        ///     GetFieldValue           2   (Defining_Type + Field_Name)
        ///     SetFieldValue           2   (Defining_Type + Field_Name)
        /// </remarks>
        private Expression GeneratePrimitiveExpression(Compiler.SemanticNodes.PrimitiveCallNode node)
        {
            // What type of primitive do we have to do with?
            PrimitivesEnum primitive;
            if (node.ApiConvention.Value == "primitive:")
                primitive = PrimitivesEnum.BuiltInPrimitive;
            else if (node.ApiConvention.Value == "static:")
                primitive = PrimitivesEnum.InvokeStaticMethod;
            else if (node.ApiConvention.Value == "call:")
                primitive = PrimitivesEnum.InvokeInstanceMethod;
            else if (node.ApiConvention.Value == "ctor:")
                primitive = PrimitivesEnum.InvokeConstructor;
            else if (node.ApiConvention.Value == "get_property:")
                primitive = PrimitivesEnum.InvokeGetProperty;
            else if (node.ApiConvention.Value == "set_property:")
                primitive = PrimitivesEnum.InvokeSetProperty;
            else if (node.ApiConvention.Value == "get_field:")
                primitive = PrimitivesEnum.GetFieldValue;
            else if (node.ApiConvention.Value == "set_field:")
                primitive = PrimitivesEnum.SetFieldValue;
            else
                throw new PrimitiveSemanticException(CodeGenerationErrors.UnexpectedCallingconvention);

            // Where in the API declaration are the different parameters needed for the API?
            Type definingType;
            string memberName;
            int parametersStartIndex;
            if (primitive == PrimitivesEnum.BuiltInPrimitive)
            {
                definingType = null;
                memberName = node.ApiParameters[0].Value;
                parametersStartIndex = 1;
            }
            else
            {
                definingType = this.GetDefiningType(node);
                if (primitive == PrimitivesEnum.InvokeConstructor)
                {
                    memberName = null;
                    parametersStartIndex = 1;
                }
                else
                {
                    memberName = node.ApiParameters[1].Value;
                    parametersStartIndex = 2;
                }
            }
            IEnumerable<string> parameters = node.ApiParameters.Skip(parametersStartIndex).Select(token => token.Value);

            // Have the helper do the heavy wotk!
            BindingRestrictions restrictions = this.RootVisitor.BindingRestrictions;
            Expression primitiveCall = PrimitiveBuilder.GeneratePrimitive(this,
                primitive,
                definingType,
                memberName,
                parameters,
                this.MethodVisitor.SelfArgument,
                this.MethodVisitor.PassedArguments,
                ref restrictions);
            this.RootVisitor.BindingRestrictions = restrictions;
            if (primitiveCall == null)
                throw new PrimitiveSemanticException(CodeGenerationErrors.UnexpectedCallingconvention);
            return primitiveCall;
        }


        ObjectClassCallSiteBinder IPrimitiveClient.GetClassBinder()
        {
            ObjectClassCallSiteBinder binder = this.RootVisitor.BinderCache.CachedObjectClassCallSiteBinder;
            if (binder == null)
            {
                binder = new ObjectClassCallSiteBinder(this.RootVisitor.Runtime);
                this.RootVisitor.BinderCache.CachedObjectClassCallSiteBinder = binder;
            }
            return binder;
        }

        /// <summary>
        /// Get the Type that is supposed to define the member we are about to invoke.
        /// </summary>
        /// <param name="node">Parse node describing the primitive call.</param>
        /// <returns>Type that is supposed to define the member we are about to invoke.</returns>
        private Type GetDefiningType(Compiler.SemanticNodes.PrimitiveCallNode node)
        {
            String definingTypeName = node.ApiParameters[0].Value;
            // Get the type that is expected to implement the member we are looking for.
            Type definingType = NativeTypeClassMap.GetType(definingTypeName);
            if (definingType == null)
                throw new PrimitiveInvalidTypeException(String.Format(RuntimeCodeGenerationErrors.WrongTypeName, definingTypeName)).SetNode(node);
            return definingType;
        }
    }
}

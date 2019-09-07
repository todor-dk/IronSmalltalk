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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Primitives;
using IronSmalltalk.ExpressionCompiler.Primitives.Exceptions;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using IronSmalltalk.Runtime.Execution.Internals;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    /// <summary>
    /// Encoder visitor for visiting and generating primitive calls.
    /// </summary>
    /// <remarks>
    /// This is the place where primitive calls are encoded to expressions and where most of the interop to the .Net framework happens.
    /// </remarks>
    public partial class PrimitiveCallVisitor : NestedEncoderVisitor<List<Expression>>
    {
        /// <summary>
        /// Indicates if there is Smalltalk fallback code after the primitive call.
        /// </summary>
        public bool HasFallbackCode { get; private set; }

        /// <summary>
        /// The enclosing method visitor that created this visitor and defines the context of this visitor.
        /// </summary>
        public MethodVisitor MethodVisitor { get; private set; }

        public PrimitiveCallVisitor(MethodVisitor parentVisitor, bool hasFallbackCode)
            : base(parentVisitor)
        {
            this.HasFallbackCode = hasFallbackCode;
            this.MethodVisitor = parentVisitor;
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
                ex.SetErrorLocation(node);
                throw;
            }

            // We need to handle void returns, because Smalltalk always needs a valid receiver.
            NameBinding selfBinding = this.MethodVisitor.Context.GetBinding(SemanticConstants.Self);
            if (primitiveCall.Type == typeof(void))
                primitiveCall = Expression.Block(primitiveCall, selfBinding.GenerateReadExpression(this));
            else if (primitiveCall.Type != typeof(object))
                primitiveCall = Expression.Convert(primitiveCall, typeof(object));
            // A successful primitive call must return directly without executing any other statements.
            primitiveCall = this.Context.Return(primitiveCall);

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
        ///     BuiltInPrimitive        ::= '˂' 'primitive'     Primitive_Name              Prim_Arg*               '˃'
        ///     InvokeStaticMethod      ::= '˂' 'static'    	Defining_Type	Method_Name Arg_Type*	            '˃'
        ///     InvokeInstanceMethod    ::= '˂' 'call'	        Defining_Type	Method_Name	This_Type	Arg_Type*   '˃'
        ///     InvokeConstructor       ::= '˂' 'ctor'	        Defining_Type		        Arg_Type*	            '˃'
        ///     InvokeGetProperty       ::= '˂' 'get_property'	Defining_Type	Prop_Name	Arg_Type*	Return_Type '˃'
        ///     InvokeSetProperty       ::= '˂' 'set_property'	Defining_Type	Prop_Name	Arg_Type*	Return_Type '˃'
        ///     GetFieldValue           ::= '˂' 'get_field'	    Defining_Type	Field_Name	Arg_Type*	            '˃'
        ///     SetFieldValue           ::= '˂' 'set_field'	    Defining_Type	Field_Name	Arg_Type*	            '˃'
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
        ///     IST_TypeName        ::= '_' Name    "Type name prefixed with _ (underscore), for convenience for not having to include assembly info etc."
        ///     Qualified_TypeName  ::= "Assembly qualified type name. Used for everything else than the above 3 categories."
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
            IEnumerable<string> parameters = node.ApiParameters.Select(token => token.Value);

            // What type of primitive do we have to do with?
            if (node.ApiConvention.Value == "primitive:")
                return BuiltInPrimitiveEncoder.GeneratePrimitive(this, parameters.Skip(1), node.ApiParameters[0].Value);

            Type definingType = this.GetDefiningType(node);
            if (node.ApiConvention.Value == "ctor:")
                return InvokeConstructorPrimitiveEncoder.GeneratePrimitive(this, parameters.Skip(1), definingType);

            string memberName = node.ApiParameters[1].Value;
            parameters = parameters.Skip(2);
            if (node.ApiConvention.Value == "static:")
                return InvokeStaticMethodPrimitiveEncoder.GeneratePrimitive(this, parameters, definingType, memberName);
            if (node.ApiConvention.Value == "call:")
                return InvokeInstanceMethodPrimitiveEncoder.GeneratePrimitive(this, parameters, definingType, memberName);
            if (node.ApiConvention.Value == "get_property:")
                return GetPropertyPrimitiveEncoder.GeneratePrimitive(this, parameters, definingType, memberName);
            if (node.ApiConvention.Value == "set_property:")
                return SetPropertyPrimitiveEncoder.GeneratePrimitive(this, parameters, definingType, memberName);
            if (node.ApiConvention.Value == "get_field:")
                return GetFieldPrimitiveEncoder.GeneratePrimitive(this, parameters, definingType, memberName);
            if (node.ApiConvention.Value == "set_field:")
                return SetFieldPrimitiveEncoder.GeneratePrimitive(this, parameters, definingType, memberName);
            else
                throw new PrimitiveSemanticException(CodeGenerationErrors.UnexpectedCallingconvention).SetErrorLocation(node);
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
                throw new PrimitiveInvalidTypeException(String.Format(CodeGenerationErrors.WrongTypeName, definingTypeName)).SetErrorLocation(node);
            return definingType;
        }
    }
}

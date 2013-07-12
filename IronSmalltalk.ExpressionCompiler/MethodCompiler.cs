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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.ExpressionCompiler
{
    public abstract class MethodCompiler : ExpressionCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="compilerOptions">Options that control the workings of the compiler.</param>
        protected MethodCompiler(SmalltalkRuntime runtime, CompilerOptions compilerOptions)
            : base(runtime, compilerOptions)
        {
        }

        public MethodCompilationResult CompileMethod(MethodNode parseTree, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments)
        {
            if (parseTree == null)
                throw new ArgumentNullException("parseTree");
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (self == null)
                throw new ArgumentNullException("self");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            VisitingContext context = this.GetVisitingContext(cls, self, arguments);
            MethodVisitor visitor = new MethodVisitor(context, arguments.Skip(1).ToArray());    // Skip(1) is the ExecutionContext
            Expression code = parseTree.Accept(visitor);
            return new MethodCompilationResult(code, context.BindingRestrictions);
        }

        protected abstract VisitingContext GetVisitingContext(SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments);

        public LambdaExpression CompileBindDelegate(MethodNode parseTree, SmalltalkClass cls, string methodName)
        {
            if (parseTree == null)
                throw new ArgumentNullException("parseTree");

            // Declare parameter expression for each argument ....
            ParameterExpression[] parameters = MethodCompiler.CreateParametersForMethod(parseTree);

            // Create dynamic meta objects for those argumen
            DynamicMetaObject target = DynamicMetaObject.Create(null, parameters[1]);               // self
            DynamicMetaObject[] arguments = new DynamicMetaObject[parameters.Length - 2];           // the method arguments incl. the execution context
            for (int i = 0; i < arguments.Length; i++)
                arguments[i] = DynamicMetaObject.Create(null, parameters[i + 2]);

            // Compile the method !!!
            MethodCompilationResult compilationResult = this.CompileMethod(parseTree, cls, target, arguments);
            Expression body = compilationResult.ExecutableCode;
            BindingRestrictions restrictions = compilationResult.Restrictions; // BUG-BUG may need to add RemoteObjectRestrictions

            // BUG BUG BUG .... we must implement this in the process!!!!
if (restrictions == null)
    restrictions = BindingRestrictions.GetTypeRestriction(parameters[1], typeof(SmalltalkObject));

            // The tricky work ... compile everything into a CallSite delegate
            ParameterExpression site = parameters[0];
            List<Expression> expressions = new List<Expression>();
            Type methodSignature = MethodSignatures.GetMethodType(parseTree.Arguments.Count);
            Type siteType = typeof(CallSite<>).MakeGenericType(methodSignature);

            // Return lable
            LabelTarget returnLabel = Expression.Label(typeof(object)); // ST methods always return object

            // Fix the main logic to have a return. 
            if (body.NodeType != ExpressionType.Goto)
                body = Expression.Return(returnLabel, body);

            //  object Method(CallSite $site, ...)
            //  {
            //      if (TESTS)                                  // The binding restrictions
            //          return MAIN_LOGIC;                      // The logic of the method
            //
            //  Update:
            //      if (CallSiteOps.SetNotMatched($site))
            //          return null;
            //      else
            //          return ((CallSite<...>)$site).Update($site, ...);
            //  }



            // Add the restrictions
            if (restrictions == BindingRestrictions.Empty)
                expressions.Add(body);
            else
                expressions.Add(Expression.IfThen(restrictions.ToExpression(), body));

            // The update logic...
            expressions.Add(Expression.Label(CallSiteBinder.UpdateLabel));
            //expressions.Add(Expression.IfThenElse(
            //    // if (CallSiteOps.SetNotMatched($site))
            //    Expression.Call(typeof(CallSiteOps).GetMethod("SetNotMatched"), site),
            //    // return null;
            //    Expression.Return(returnLabel, Expression.Constant(null, typeof(object))),
            //    // return ((CallSite<...>)$site).Update($site, ...);
            //    Expression.Return(returnLabel, Expression.Invoke(
            //        Expression.Property(
            //            Expression.Convert(site, siteType),
            //            siteType.GetProperty("Update")),
            //        parameters))));
            expressions.Add(Expression.Label(
                returnLabel,
                Expression.Condition(
                    Expression.Call(
                        typeof(CallSiteOps).GetMethod("SetNotMatched"),
                        site),
                    Expression.Constant(null, typeof(object)),
                    Expression.Invoke(
                        Expression.Property(
                            Expression.Convert(site, siteType),
                            siteType.GetProperty("Update")),
                        parameters))));

            // Finally, create a lambda expression out of it.
            return Expression.Lambda(
                Expression.Block(expressions),
                methodName,
                true, // always compile the rules with tail call optimization,
                parameters);
        }

        private static ParameterExpression[] CreateParametersForMethod(MethodNode parseTree)
        {
            ParameterExpression[] args = new ParameterExpression[parseTree.Arguments.Count + 3];
            Dictionary<string, ParameterExpression> argsMap = new Dictionary<string, ParameterExpression>();

            string name = MethodCompiler.GetUniqueName(argsMap, "self");
            ParameterExpression param = Expression.Parameter(typeof(object), name); // All our args are Object
            argsMap.Add(name, param);
            args[1] = param;

            for (int i = 0; i < parseTree.Arguments.Count; i++)
            {
                MethodArgumentNode arg = parseTree.Arguments[i];
                name = MethodCompiler.GetUniqueName(argsMap, arg.Token.Value);
                param = Expression.Parameter(typeof(object), name); // All our args are Object
                argsMap.Add(name, param);
                args[i + 3] = param;
            }

            // Those are not used by code, and we define them last, just in case there are naming conflicts - the name of those is unimportant.
            name = MethodCompiler.GetUniqueName(argsMap, "$site");
            param = Expression.Parameter(typeof(CallSite), name);
            argsMap.Add(name, param);
            args[0] = param;
            name = MethodCompiler.GetUniqueName(argsMap, "$executionContext");
            param = Expression.Parameter(typeof(ExecutionContext), name);
            argsMap.Add(name, param);
            args[2] = param;

            return args;
        }

        private static string GetUniqueName<TItem>(IDictionary<string, TItem> map, string name)
        {
            string suggestion = name;
            int idx = 1;
            while (map.ContainsKey(suggestion))
            {
                suggestion = String.Format(CultureInfo.InvariantCulture, "{0}{1}", suggestion, idx);
                idx++;
            }
            return suggestion;
        }

        //public abstract BindingRestrictions GetBindingRestrictions(SmalltalkClass cls, Expression self, Expression executionContext)
            
        //public static BindingRestrictions GetBindingRestrictions(SmalltalkRuntime runtime, SmalltalkClass cls, Expression self, Expression executionContext, bool checkRuntimeInstance)
        //{
        //    if (runtime == null)
        //        throw new ArgumentNullException("runtime");
        //    if (cls == null)
        //        throw new ArgumentNullException("cls");
        //    if (self == null)
        //        throw new ArgumentNullException("self");

        //    )

        //    // Special case handling of null, so it acts like first-class-object.
        //    //if (receiver == null)
        //    //{
        //    //    cls = runtime.NativeTypeClassMap.UndefinedObject;
        //    //    // If not explicitely mapped to a ST Class, fallback to the generic .Net mapping class.
        //    //    if (cls == null)
        //    //        cls = runtime.NativeTypeClassMap.Native;
        //    //    if (cls == null)
        //    //        cls = runtime.NativeTypeClassMap.Object;
        //    //    restrictions = BindingRestrictions.GetInstanceRestriction(self.Expression, null);
        //    //}
        //    // Smalltalk objects ... almost every objects ends up here.
        //    //else if (receiver is SmalltalkObject)
        //    //{
        //    //    SmalltalkObject obj = (SmalltalkObject)receiver;
        //    //    cls = obj.Class;
        //    //    if (cls.Runtime == runtime)
        //    //    {
        //    //        FieldInfo field = typeof(SmalltalkObject).GetField("Class", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
        //    //        if (field == null)
        //    //            throw new InvalidOperationException();
        //    //        restrictions = BindingRestrictions.GetTypeRestriction(self.Expression, typeof(SmalltalkObject));
        //    //        restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
        //    //            Expression.ReferenceEqual(Expression.Field(Expression.Convert(self.Expression, typeof(SmalltalkObject)), field), Expression.Constant(cls))));
        //    //    }
        //    //    else
        //    //    {
        //    //        // A smalltalk object, but from different runtime
        //    //        cls = null; // Let block below handle this.
        //    //        restrictions = null;
        //    //    }
        //    //}
        //    if (cls == runtime.NativeTypeClassMap.Symbol)
        //    {
        //        BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Symbol));
        //        if (checkRuntimeInstance)
        //        {
        //            FieldInfo symManager = typeof(Symbol).GetField("Manager", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
        //            if (symManager == null)
        //                throw new InvalidOperationException();
        //            PropertyInfo symRuntime = typeof(SymbolTable).GetProperty("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetProperty,
        //                null, typeof(SmalltalkRuntime), new Type[0], null);
        //            if (symRuntime == null)
        //                throw new InvalidOperationException();
        //            FieldInfo ecRuntime = typeof(ExecutionContext).GetField("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetField);
        //            if (ecRuntime == null)
        //                throw new InvalidOperationException();

        //            // (self is ...) && (self.Manager.Runtime == executionContext.Runtime)
        //            restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
        //                Expression.ReferenceEqual(
        //                    Expression.Property(Expression.Field(Expression.Convert(self, typeof(Symbol)), symManager), symRuntime),
        //                    Expression.Field(executionContext, ecRuntime))));
        //        }
        //        return restrictions;
        //    }
            
        //    if (cls == runtime.NativeTypeClassMap.Pool)
        //    {
        //        BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Pool));
        //        if (checkRuntimeInstance)
        //        {
        //            PropertyInfo poolRuntime = typeof(Pool).GetProperty("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetProperty,
        //                null, typeof(SmalltalkRuntime), new Type[0], null);
        //            if (poolRuntime == null)
        //                throw new InvalidOperationException();
        //            FieldInfo ecRuntime = typeof(ExecutionContext).GetField("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetField);
        //            if (ecRuntime == null)
        //                throw new InvalidOperationException();

        //            // (self is ...) && (self.Runtime == executionContext.Runtime)
        //            restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
        //                Expression.ReferenceEqual(
        //                    Expression.Property(Expression.Convert(self, typeof(Pool)), poolRuntime)
        //                    Expression.Field(executionContext, ecRuntime))));
        //        }
        //        return restrictions;
        //    }

        //    // Common FCL type mapping (bool, int, string, etc) to first-class-object.
        //    if (cls == runtime.NativeTypeClassMap.True)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(bool)).Merge(
        //            BindingRestrictions.GetExpressionRestriction(Expression.IsTrue(Expression.Convert(self, typeof(bool))))));
        //    if (cls == runtime.NativeTypeClassMap.False)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(bool)).Merge(
        //            BindingRestrictions.GetExpressionRestriction(Expression.IsFalse(Expression.Convert(self, typeof(bool))))));
        //    if (cls == runtime.NativeTypeClassMap.SmallInteger)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(int));
        //    if (cls == runtime.NativeTypeClassMap.String)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(string));
        //    if (cls == runtime.NativeTypeClassMap.Character)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(char));
        //    if (cls == runtime.NativeTypeClassMap.Float)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(double));
        //    if (cls == runtime.NativeTypeClassMap.SmallDecimal)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(decimal));
        //    if (cls == runtime.NativeTypeClassMap.BigInteger)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(System.Numerics.BigInteger));
        //    if (cls == runtime.NativeTypeClassMap.BigDecimal)
        //        return BindingRestrictions.GetTypeRestriction(self, typeof(IronSmalltalk.Common.BigDecimal));

        //    // Special case for Smalltalk classes, because we want the class behavior ...
        //    else if (receiver is SmalltalkClass)
        //    {
        //        cls = (SmalltalkClass)receiver;
        //        if (cls.Runtime == runtime)
        //        {
        //            cls = runtime.NativeTypeClassMap.Class;
        //            if (cls == null)
        //                cls = runtime.NativeTypeClassMap.Object;

        //            if (cls == null)
        //                restrictions = null;
        //            else
        //                // NB: Restriction below are no good for For class behavior.
        //                restrictions = BindingRestrictions.GetTypeRestriction(self.Expression, typeof(SmalltalkClass)); 
        //        }
        //        else
        //        {
        //            // A smalltalk object, but from different runtime
        //            cls = null; // Let block below handle this.
        //            restrictions = null;
        //        }
        //    }
        //    // Some .Net type that's neither IronSmalltalk object nor any on the known (hardcoded) types.
        //    else
        //    {
        //        cls = null; // Let block below handle this.
        //        restrictions = null;
        //    }

        //    // In case of any of the known (hardcoded) types has no registered Smalltalk class, 
        //    // fallback to the generic .Net type to Smalltalk class mapping.
        //    if (cls != null)
        //    {
        //        return cls;
        //    }
        //    else
        //    {
        //        Debug.Assert(receiver != null, "receiver != null");
        //        Type type = receiver.GetType();
        //        cls = runtime.NativeTypeClassMap.GetSmalltalkClass(type);
        //        // If not explicitely mapped to a ST Class, fallback to the generic .Net mapping class.
        //        if (cls == null)
        //            cls = runtime.NativeTypeClassMap.Native;
        //        if (restrictions == null)
        //            restrictions = BindingRestrictions.GetTypeRestriction(self.Expression, type);
        //        return cls;
        //    }
        //}


    }
}

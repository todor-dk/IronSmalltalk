using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler
{
    public sealed class InstanceMethodCompiler : MethodCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="compilerOptions">Options that control the workings of the compiler.</param>
        public InstanceMethodCompiler(SmalltalkRuntime runtime, CompilerOptions compilerOptions)
            : base(runtime, compilerOptions)
        {
        }

        protected override VisitingContext GetVisitingContext(SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments)
        {
            SmalltalkNameScope globalNameScope = this.CompilerOptions.GlobalNameScope ?? this.Runtime.GlobalScope;

            BindingScope globalScope = BindingScope.ForInstanceMethod(cls, globalNameScope);
            BindingScope reservedScope = (cls.Superclass == null) ?
                ReservedScope.ForRootClassInstanceMethod() :
                ReservedScope.ForInstanceMethod();

            return new VisitingContext(this, globalScope, reservedScope, self, arguments[0], cls.Name);
        }

        //public override BindingRestrictions GetBindingRestrictions(SmalltalkClass cls, Expression self, Expression executionContext)
        //{
        //    if (cls == null)
        //        throw new ArgumentNullException("cls");
        //    if (self == null)
        //        throw new ArgumentNullException("self");
        //    if (executionContext == null)
        //        throw new ArgumentNullException("executionContext");
            
        //    if (cls.InstanceState != InstanceStateEnum.Native)
        //        // Special case handling of null, so it acts like first-class-object.
        //        //if (receiver == null)
        //        //{
        //        //    cls = runtime.NativeTypeClassMap.UndefinedObject;
        //        //    // If not explicitely mapped to a ST Class, fallback to the generic .Net mapping class.
        //        //    if (cls == null)
        //        //        cls = runtime.NativeTypeClassMap.Native;
        //        //    if (cls == null)
        //        //        cls = runtime.NativeTypeClassMap.Object;
        //        //    restrictions = BindingRestrictions.GetInstanceRestriction(self.Expression, null);
        //        //}
        //        // Smalltalk objects ... almost every objects ends up here.
        //        //else if (receiver is SmalltalkObject)
        //        //{
        //        //    SmalltalkObject obj = (SmalltalkObject)receiver;
        //        //    cls = obj.Class;
        //        //    if (cls.Runtime == runtime)
        //        //    {
        //        //        FieldInfo field = typeof(SmalltalkObject).GetField("Class", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
        //        //        if (field == null)
        //        //            throw new InvalidOperationException();
        //        //        restrictions = BindingRestrictions.GetTypeRestriction(self.Expression, typeof(SmalltalkObject));
        //        //        restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
        //        //            Expression.ReferenceEqual(Expression.Field(Expression.Convert(self.Expression, typeof(SmalltalkObject)), field), Expression.Constant(cls))));
        //        //    }
        //        //    else
        //        //    {
        //        //        // A smalltalk object, but from different runtime
        //        //        cls = null; // Let block below handle this.
        //        //        restrictions = null;
        //        //    }
        //        //}
        //        if (cls == runtime.NativeTypeClassMap.Symbol)
        //        {
        //            BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Symbol));
        //            if (checkRuntimeInstance)
        //            {
        //                FieldInfo symManager = typeof(Symbol).GetField("Manager", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
        //                if (symManager == null)
        //                    throw new InvalidOperationException();
        //                PropertyInfo symRuntime = typeof(SymbolTable).GetProperty("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetProperty,
        //                    null, typeof(SmalltalkRuntime), new Type[0], null);
        //                if (symRuntime == null)
        //                    throw new InvalidOperationException();
        //                FieldInfo ecRuntime = typeof(ExecutionContext).GetField("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetField);
        //                if (ecRuntime == null)
        //                    throw new InvalidOperationException();

        //                // (self is ...) && (self.Manager.Runtime == executionContext.Runtime)
        //                restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
        //                    Expression.ReferenceEqual(
        //                        Expression.Property(Expression.Field(Expression.Convert(self, typeof(Symbol)), symManager), symRuntime),
        //                        Expression.Field(executionContext, ecRuntime))));
        //            }
        //            return restrictions;
        //        }

        //        if (cls == runtime.NativeTypeClassMap.Pool)
        //        {
        //            BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Pool));
        //            if (checkRuntimeInstance)
        //            {
        //                PropertyInfo poolRuntime = typeof(Pool).GetProperty("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetProperty,
        //                    null, typeof(SmalltalkRuntime), new Type[0], null);
        //                if (poolRuntime == null)
        //                    throw new InvalidOperationException();
        //                FieldInfo ecRuntime = typeof(ExecutionContext).GetField("Runtime", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.GetField);
        //                if (ecRuntime == null)
        //                    throw new InvalidOperationException();

        //                // (self is ...) && (self.Runtime == executionContext.Runtime)
        //                restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
        //                    Expression.ReferenceEqual(
        //                        Expression.Property(Expression.Convert(self, typeof(Pool)), poolRuntime)
        //                        Expression.Field(executionContext, ecRuntime))));
        //            }
        //            return restrictions;
        //        }

        //        // Common FCL type mapping (bool, int, string, etc) to first-class-object.
        //        if (cls == runtime.NativeTypeClassMap.True)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(bool)).Merge(
        //                BindingRestrictions.GetExpressionRestriction(Expression.IsTrue(Expression.Convert(self, typeof(bool))))));
        //        if (cls == runtime.NativeTypeClassMap.False)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(bool)).Merge(
        //                BindingRestrictions.GetExpressionRestriction(Expression.IsFalse(Expression.Convert(self, typeof(bool))))));
        //        if (cls == runtime.NativeTypeClassMap.SmallInteger)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(int));
        //        if (cls == runtime.NativeTypeClassMap.String)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(string));
        //        if (cls == runtime.NativeTypeClassMap.Character)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(char));
        //        if (cls == runtime.NativeTypeClassMap.Float)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(double));
        //        if (cls == runtime.NativeTypeClassMap.SmallDecimal)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(decimal));
        //        if (cls == runtime.NativeTypeClassMap.BigInteger)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(System.Numerics.BigInteger));
        //        if (cls == runtime.NativeTypeClassMap.BigDecimal)
        //            return BindingRestrictions.GetTypeRestriction(self, typeof(IronSmalltalk.Common.BigDecimal));
            
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.NativeCompiler.Internals
{
    public class NativeDynamicCallStrategy : IDynamicCallStrategy
    {
        internal string CurrentMethodName;

        private readonly MethodGenerator MethodGenerator;

        internal NativeDynamicCallStrategy(MethodGenerator methodGenerator)
        {
            this.MethodGenerator = methodGenerator;
        }

        private TypeBuilder _CallSitesType = null;
        private TypeBuilder CallSitesType
        {
            get
            {
                if (this._CallSitesType == null)
                    this._CallSitesType = this.GetCallSitesType();
                return this._CallSitesType;
            }
        }

        private Type CallSitesTypeType;

        private TypeBuilder GetCallSitesType()
        {
            string name = string.Format("{0}.{1}", this.MethodGenerator.TypeBuilder.FullName, "$CallSites");
            name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalTypeName(name);
            return this.MethodGenerator.TypeBuilder.DefineNestedType(
                name,
                TypeAttributes.Class | TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Abstract,
                typeof(object));
        }

        internal void GenerateCallSitesType()
        {
            if (this._CallSitesType == null)
                return;

            // It would have been nice to use the Lambda Compiler, but it can't compile constructors,
            // so we have to generate the constructor by emitting IL code by hand :/
            ConstructorBuilder ctor = this._CallSitesType.DefineConstructor(
                MethodAttributes.Private | MethodAttributes.Static, CallingConventions.Standard, new Type[0]);
            ILGenerator ctorIL = ctor.GetILGenerator();
            // ... now assign to each literal field
            for (int i = 0; i < this.DefinedCallSites.Count; i++)
            {
                CallSiteDefinition def = this.DefinedCallSites[i];
                MethodInfo create = def.SiteType.GetMethod("Create");   // Get the CallSite<>.Create method
                def.Binder.GenerateBinderInitializer(ctorIL);           // Load the Binder
                ctorIL.Emit(OpCodes.Call, create);                      // Call CallSite<>.Create(Binder) 
                ctorIL.Emit(OpCodes.Stsfld, def.CallSiteField);         // Store the value in the static field for the call site
            }
            ctorIL.Emit(OpCodes.Ret);

            this.CallSitesTypeType = this._CallSitesType.CreateType();
        }


        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext)
        {
            Expression callSite = this.CreateCallSite(0, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { });
        }

        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, Expression argument)
        {
            Expression callSite = this.CreateCallSite(1, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { argument });
        }

        public Expression CompileDynamicCall(VisitingContext context, string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            Expression callSite = this.CreateCallSite(argumentCount, selector, nativeName, isSuperSend, isConstantReceiver, superLookupScope);
            return this.CompileDynamicCall(callSite, receiver, executionContext, arguments);
        }


        public Expression CompileGetClass(VisitingContext context, Expression receiver, Expression executionContext)
        {
            IBinderDefinition binder = new ClassBinderDefinition();

            Type delegateType = typeof(Func<CallSite, object, ExecutionContext, object>);
            Type siteType = typeof(CallSite<Func<CallSite, object, ExecutionContext, object>>);

            Expression callSite = this.CreateCallSite(binder, delegateType, siteType, "class");
            return this.CompileDynamicCall(callSite, receiver, executionContext, new Expression[] { });            
        }



        private Expression CompileDynamicCall(Expression callSite, Expression receiver, Expression executionContext, IEnumerable<Expression> arguments)
        {
            Type delegateType = NativeDynamicCallStrategy.GetCallSiteType(arguments.Count());
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);

            List<Expression> args = new List<Expression>();
            args.Add(callSite);
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);

            FieldInfo target = TypeUtilities.Field(siteType, "Target", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo invoke = delegateType.GetMethod("Invoke");
            ParameterInfo[] pis = invoke.GetParameters();

            // siteExpr.Target.Invoke(siteExpr, *args)
            return Expression.Call(
                Expression.Field(callSite, target),
                invoke,
                args);

            /* C# Style
            ParameterExpression site = Expression.Variable(siteType, "$site");
            List<Expression> args = new List<Expression>();
            args.Add(site);
            args.Add(receiver);
            args.Add(executionContext);
            args.AddRange(arguments);

            FieldInfo target = TypeUtilities.Field(siteType, "Target", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo invoke = delegateType.GetMethod("Invoke");
            ParameterInfo[] pis = invoke.GetParameters();
            // ($site = siteExpr).Target.Invoke($site, *args)
            return Expression.Block(
                new[] { site },
                Expression.Call(
                    Expression.Field(Expression.Assign(site, callSite), target),
                    invoke,
                    args));
             */
        }


        private static Type GetCallSiteType(int argumentCount)
        {
            if (argumentCount == 0)
                return typeof(Func<CallSite, object, ExecutionContext, object>);
            if (argumentCount == 1)
                return typeof(Func<CallSite, object, ExecutionContext, object, object>);
            if (argumentCount == 2)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object>);
            if (argumentCount == 3)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object>);
            if (argumentCount == 4)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object>);
            if (argumentCount == 5)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object>);
            if (argumentCount == 6)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object>);
            if (argumentCount == 7)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object>);
            if (argumentCount == 8)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 9)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 10)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 11)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 12)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object, object, object>);
            if (argumentCount == 13)
                return typeof(Func<CallSite, object, ExecutionContext, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
            throw new NotImplementedException();
        }

        private Expression CreateCallSite(int argumentCount, string selector, string nativeName, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
        {
            BinderDefinition binder = new BinderDefinition(selector, nativeName, argumentCount, isSuperSend, isConstantReceiver, superLookupScope);

            Type delegateType = NativeDynamicCallStrategy.GetCallSiteType(argumentCount);
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);

            return this.CreateCallSite(binder, delegateType, siteType, selector);
        }

        private Expression CreateCallSite(IBinderDefinition binder, Type delegateType, Type siteType, string nameSuggestion)
        {
            //string nameSuggestion = String.Format("{0}.{1}", this.CurrentMethodName, selector);
            string name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalMethodName(nameSuggestion);
            int idx = 0;
            while (this.DefinedCallSites.Any(def => def.Name == name))
                name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalMethodName(String.Format("{0}${1}", nameSuggestion, idx++));

            FieldBuilder field = this.CallSitesType.DefineField(name, siteType, FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Assembly);
            this.DefinedCallSites.Add(new CallSiteDefinition(name, delegateType, siteType, field, binder));

            return Expression.Field(null, field);
            //MethodInfo create = siteType.GetMethod("Create");
            //MethodInfo getCallSite = typeof(NativeDynamicCallStrategy).GetMethod("GetCallSite");

            //Expression binder = Expression.Call(null, getCallSite,
            //    Expression.Constant(selector, typeof(string)),
            //    Expression.Constant(nativeName, typeof(string)),
            //    Expression.Constant(isSuperSend, typeof(bool)),
            //    Expression.Constant(isConstantReceiver, typeof(bool)),
            //    Expression.Constant(superLookupScope, typeof(string)));
            //return Expression.Call(null, create, binder);
        }

        private List<CallSiteDefinition> DefinedCallSites = new List<CallSiteDefinition>();

        private struct CallSiteDefinition
        {
            public readonly string Name;
            public readonly FieldBuilder CallSiteField;
            public readonly Type DelegateType;
            public readonly Type SiteType;
            public readonly IBinderDefinition Binder;

            public CallSiteDefinition(string name, Type delegateType, Type siteType, FieldBuilder callSiteField, IBinderDefinition binder)
            {
                this.Name = name;
                this.DelegateType = delegateType;
                this.SiteType = siteType;
                this.CallSiteField = callSiteField;
                this.Binder = binder;
            }

        }

        internal interface IBinderDefinition
        {
            void GenerateBinderInitializer(ILGenerator ilgen);
        }

        private class BinderDefinition : IBinderDefinition
        {
            public readonly string Selector;
            public readonly string NativeName;
            public readonly int ArgumentCount;
            public readonly bool IsSuperSend;
            public readonly bool IsConstantReceiver;
            public readonly string SuperLookupScope;

            public BinderDefinition(string selector, string nativeName, int argumentCount, bool isSuperSend, bool isConstantReceiver, string superLookupScope)
            {
                if (selector == null)
                    throw new ArgumentNullException("selector");
                if (argumentCount < 0)
                    throw new ArgumentOutOfRangeException("argumentCount");
                this.Selector = selector;
                this.NativeName = nativeName;
                this.ArgumentCount = argumentCount;
                this.IsSuperSend = isSuperSend;
                this.IsConstantReceiver = isConstantReceiver;
                this.SuperLookupScope = superLookupScope;
            }

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                MethodInfo getBinder = typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache).GetMethod("GetMessageBinder");

                ilgen.Emit(OpCodes.Ldstr, this.Selector);
                if (this.NativeName == null)
                    ilgen.Emit(OpCodes.Ldnull);
                else
                    ilgen.Emit(OpCodes.Ldstr, this.NativeName);
                ilgen.PushInt(this.ArgumentCount);
                if (this.IsSuperSend)
                    ilgen.Emit(OpCodes.Ldc_I4_1);
                else
                    ilgen.Emit(OpCodes.Ldc_I4_0);
                if (this.IsConstantReceiver)
                    ilgen.Emit(OpCodes.Ldc_I4_1);
                else
                    ilgen.Emit(OpCodes.Ldc_I4_0);
                if (this.SuperLookupScope == null)
                    ilgen.Emit(OpCodes.Ldnull);
                else
                    ilgen.Emit(OpCodes.Ldstr, this.SuperLookupScope);
                ilgen.Emit(OpCodes.Call, getBinder);
            }

        }

        private class ClassBinderDefinition : IBinderDefinition
        {
            private static readonly FieldInfo ObjectClassCallSiteBinderField = TypeUtilities.Field(typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache), "ObjectClassCallSiteBinder");

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                ilgen.Emit(OpCodes.Ldsfld, ClassBinderDefinition.ObjectClassCallSiteBinderField);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common.Internal;

namespace IronSmalltalk.NativeCompiler.CompilationStrategies
{
    internal sealed class CallSiteGenerator
    {
        private readonly string CallSitesTypeName;

        private readonly INativeStrategyClient Client;

        private List<CallSiteDefinition> DefinedCallSites = new List<CallSiteDefinition>();

        public CallSiteGenerator(INativeStrategyClient client, string callSitesTypeName)
        {
            this.Client = client;
            this.CallSitesTypeName = callSitesTypeName;
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
            string name = String.Format("{0}.{1}", this.Client.ContainingType.FullName, this.CallSitesTypeName);
            name = this.Client.Compiler.NativeGenerator.AsLegalTypeName(name);
            return this.Client.ContainingType.DefineNestedType(
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
                MethodInfo create = TypeUtilities.Method(def.SiteType, "Create");   // Get the CallSite<>.Create method
                def.Binder.GenerateBinderInitializer(ctorIL);                       // Load the Binder
                ctorIL.Emit(OpCodes.Call, create);                                  // Call CallSite<>.Create(Binder) 
                ctorIL.Emit(OpCodes.Stsfld, def.CallSiteField);                     // Store the value in the static field for the call site
            }
            ctorIL.Emit(OpCodes.Ret);

            this.CallSitesTypeType = this._CallSitesType.CreateType();
        }

        internal Expression CreateCallSite(IBinderDefinition binder, Type delegateType, Type siteType, string nameSuggestion)
        {
            //string nameSuggestion = String.Format("{0}.{1}", this.CurrentMethodName, selector);
            string name = this.Client.Compiler.NativeGenerator.AsLegalMethodName(nameSuggestion);
            int idx = 0;
            while (this.DefinedCallSites.Any(def => def.Name == name))
                name = this.Client.Compiler.NativeGenerator.AsLegalMethodName(String.Format("{0}${1}", nameSuggestion, idx++));

            FieldBuilder field = this.CallSitesType.DefineField(name, siteType, FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Assembly);
            this.DefinedCallSites.Add(new CallSiteDefinition(name, delegateType, siteType, field, binder));

            return Expression.Field(null, field);

            //MethodInfo create = TypeUtilities.Method(siteType, "Create");
            //MethodInfo getCallSite = TypeUtilities.Method(typeof(NativeDynamicCallStrategy), "GetCallSite");

            //Expression binder = Expression.Call(null, getCallSite,
            //    Expression.Constant(selector, typeof(string)),
            //    Expression.Constant(nativeName, typeof(string)),
            //    Expression.Constant(isSuperSend, typeof(bool)),
            //    Expression.Constant(isConstantReceiver, typeof(bool)),
            //    Expression.Constant(superLookupScope, typeof(string)));
            //return Expression.Call(null, create, binder);
        }

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
    }
}

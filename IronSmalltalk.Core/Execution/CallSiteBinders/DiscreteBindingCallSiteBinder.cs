using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution.Internals;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    public abstract class DiscreteBindingCallSiteBinderBase : DynamicMetaObjectBinder, ICallSiteBinderCacheItem<string>
    {
        // IMPORTANT: Do not change those because existing compiled assemblies have compiled those values and depend on them!
        private const string GlobalPrefix = "global";
        private const string ClassVariablePrefix = "classvariable";
        private const string PoolItemPrefix = "poolitem";

        public static string GetMoniker(IDiscreteGlobalBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException();

            return DiscreteBindingCallSiteBinderBase.GetMoniker(
                DiscreteBindingCallSiteBinderBase.GlobalPrefix,
                binding.Name.Value);
        }

        public static string GetMoniker(SmalltalkClass cls, ClassVariableBinding binding)
        {
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (binding == null)
                throw new ArgumentNullException("binding");

            return DiscreteBindingCallSiteBinderBase.GetMoniker(
                DiscreteBindingCallSiteBinderBase.ClassVariablePrefix,
                cls.Name.Value,
                binding.Name.Value);
        }

        public static string GetMoniker(PoolBinding poolBinding, PoolVariableOrConstantBinding binding)
        {
            if (poolBinding == null)
                throw new ArgumentNullException("poolBinding");
            if (binding == null)
                throw new ArgumentNullException("binding");

            return DiscreteBindingCallSiteBinderBase.GetMoniker(
                DiscreteBindingCallSiteBinderBase.PoolItemPrefix,
                poolBinding.Name.Value,
                binding.Name.Value);
        }

        private static string GetMoniker(params string[] paths)
        {
            return String.Join(".", paths.Select(each => {
                if (each.Contains('.') || each.Contains('\''))
                    return "'" + each.Replace("'", "''") + "'";
                else
                    return each;
            }));
        }

        private static GetBindingStrategyBase ParseGetBindingStrategy(string moniker)
        {
            string[] monikerParts = DiscreteBindingCallSiteBinderBase.ParseMoniker(moniker);
            for (int i = 0; i < monikerParts.Length; i++)
			{
                if (String.IsNullOrWhiteSpace(monikerParts[i]))
                    throw new ImplementationException(String.Format("Invalid discrete binding moniker ({0})", moniker));
			}

            if (monikerParts.Length < 1)
                throw new ImplementationException(String.Format("Invalid discrete binding moniker ({0})", moniker));
            string type = monikerParts[0];

            if ((type == DiscreteBindingCallSiteBinderBase.GlobalPrefix) && (monikerParts.Length == 2))
                return new GetGlobalBindingStrategy(monikerParts[1]);
            if ((type == DiscreteBindingCallSiteBinderBase.ClassVariablePrefix) && (monikerParts.Length == 3))
                return new GetClassVariableBindingStrategy(monikerParts[1], monikerParts[2]);
            if ((type == DiscreteBindingCallSiteBinderBase.PoolItemPrefix) && (monikerParts.Length == 3))
                return new GetPoolItemBindingStrategy(monikerParts[1], monikerParts[2]);
            throw new ImplementationException(String.Format("Invalid discrete binding moniker ({0})", moniker));
        }

        private static string[] ParseMoniker(string moniker)
        {
            List<string> parts = new List<string>();
            
            bool insideQuote = false;
            StringBuilder part = new StringBuilder();
            for (int i = 0; i < moniker.Length; i++)
            {
                char ch = moniker[i];
                if (insideQuote)
                {
                    part.Append(ch);
                    if (ch == '\'')
                        insideQuote = false;
                }
                else
                {
                    if (ch == '\'')
                        insideQuote = true;
                    if (ch == '.')
                    {
                        parts.Add(part.ToString());
                        part = new StringBuilder();
                    }
                    else
                    {
                        part.Append(ch);
                    }
                }
            }
            parts.Add(part.ToString());

            for (int i = 0; i < parts.Count; i++)
            {
                string p = parts[i];
                if ((p.Length != 0) && (p[0] == '\''))
                    p = p.Substring(1, p.Length - 2).Replace("''", "'");
                parts[i] = p;
            }

            return parts.ToArray();
        }

        public string Moniker { get; private set; }

        protected readonly GetBindingStrategyBase GetBindingStrategy;

        public DiscreteBindingCallSiteBinderBase(string moniker)
        {
            if (moniker == null)
                throw new ArgumentNullException("moniker");
            this.Moniker = moniker;
            this.GetBindingStrategy = DiscreteBindingCallSiteBinderBase.ParseGetBindingStrategy(moniker);
        }

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ExecutionContext executionContext = null;
            if (target != null)
                executionContext = target.Value as ExecutionContext;
            if (executionContext == null)
                // If this is null, the binder was not used by a Smalltalk method. Or may be somebody passed null for the ExecutionContext, which is illegal too.
                throw new ImplementationException("The DiscreteBindingCallSiteBinderBase can only be used in methods where the signature is (ExecutionContext)");

            Expression expr = this.GetExpression(executionContext);

            BindingRestrictions restictions = BindingRestrictions.GetExpressionRestriction(
                Expression.Equal(
                    Expression.Field(target.Expression, ExecutionContext.RuntimeField),
                    Expression.Constant(executionContext.Runtime, typeof(SmalltalkRuntime))));

            return new DynamicMetaObject(expr, restictions);
        }

        protected abstract Expression GetExpression(ExecutionContext executionContext);

        protected abstract class GetBindingStrategyBase
        {
            public abstract IDiscreteBinding GetBinding(ExecutionContext executionContext);
        }

        private class GetGlobalBindingStrategy : GetBindingStrategyBase
        {
            public readonly string GlobalName;

            public GetGlobalBindingStrategy(string globalName)
            {
                this.GlobalName = globalName;
            }

            public override IDiscreteBinding GetBinding(ExecutionContext executionContext)
            {
                return executionContext.Runtime.GlobalScope.GetGlobalBinding(this.GlobalName);
            }
        }

        private class GetClassVariableBindingStrategy : GetBindingStrategyBase
        {
            public readonly string ClassName;
            public readonly string VariableName;

            public GetClassVariableBindingStrategy(string className, string variableName)
            {
                this.ClassName = className;
                this.VariableName = variableName;
            }

            public override IDiscreteBinding GetBinding(ExecutionContext executionContext)
            {
                SmalltalkClass cls = executionContext.Runtime.GetClass(this.ClassName);
                if (cls == null)
                    return null;
                ClassVariableBinding binding = null;
                cls.ClassVariableBindings.TryGetValue(this.VariableName, out binding);
                return binding;
            }
        }

        private class GetPoolItemBindingStrategy : GetBindingStrategyBase
        {
            public readonly string PoolName;
            public readonly string VariableName;

            public GetPoolItemBindingStrategy(string poolName, string variableName)
            {
                this.PoolName = poolName;
                this.VariableName = variableName;
            }

            public override IDiscreteBinding GetBinding(ExecutionContext executionContext)
            {
                Pool pool = executionContext.Runtime.GetPool(this.PoolName);
                if (pool == null)
                    return null;
                PoolVariableOrConstantBinding binding = null;
                pool.TryGetValue(this.VariableName, out binding);
                return binding;
            }
        }

        #region Call-Site-Binder Cache Support

        string ICallSiteBinderCacheItem<string>.CacheKey
        {
            get { return this.Moniker; }
        }

        ICallSiteBinderCacheFinalizationManager<string> ICallSiteBinderCacheItem<string>.FinalizationManager
        {
            get
            {
                return this._finalizationManager;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                this._finalizationManager = value;
            }
        }

        private ICallSiteBinderCacheFinalizationManager<string> _finalizationManager;

        ~DiscreteBindingCallSiteBinderBase()
        {
            if (this._finalizationManager != null)
                this._finalizationManager.InternalRemoveItem(this.Moniker);
        }

        #endregion


        /// <summary>
        /// Property info for the get_Value property of the discrete variable binding.
        /// </summary>
        protected static PropertyInfo GetPropertyInfo(Type type)
        {
            // NB: We can't use GetProperty("Value") due to AmbiguousMatchException, therefore do stuff by hand
            IEnumerable<PropertyInfo> properties = type.GetProperties(
                BindingFlags.ExactBinding | BindingFlags.FlattenHierarchy |
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            // Filter only properties named "Value"
            properties = properties.Where(pi => (pi.Name == "Value"));
            // First try for a property declared directly in the defining type.
            PropertyInfo result = properties.Where(pi => (pi.DeclaringType == type)).FirstOrDefault();
            // If not, it may be defined in the superclass, just take any property found (there should be just one)
            if (result == null)
                result = properties.FirstOrDefault();

            if (result == null)
                throw new InvalidOperationException("Expected type to have getter property named Value");
            return result;
        }
    }

    public class DiscreteBindingVariableValueCallSiteBinder : DiscreteBindingCallSiteBinderBase
    {
        public DiscreteBindingVariableValueCallSiteBinder(string moniker)
            : base(moniker)
        {
        }

        protected override Expression GetExpression(ExecutionContext executionContext)
        {
            IDiscreteBinding binding = this.GetBindingStrategy.GetBinding(executionContext);
            if (binding == null)
                throw new CodeGenerationException(String.Format("Could not find global binding {0}. This may indicate a bug in our code.", this.Moniker));
            return Expression.Property(
                Expression.Constant(binding, this.ReturnType),
                DiscreteBindingCallSiteBinderBase.GetPropertyInfo(binding.GetType())); 
        }
    }

    public class DiscreteBindingConstantValueCallSiteBinder : DiscreteBindingCallSiteBinderBase
    {
        public DiscreteBindingConstantValueCallSiteBinder(string moniker)
            : base(moniker)
        {
        }

        protected override Expression GetExpression(ExecutionContext executionContext)
        {
            IDiscreteBinding binding = this.GetBindingStrategy.GetBinding(executionContext);
            if (binding == null)
                throw new CodeGenerationException(String.Format("Could not find global binding {0}. This may indicate a bug in our code.", this.Moniker));
            return Expression.Constant(binding.Value, this.ReturnType);
        }
    }

    public class DiscreteBindingObjectCallSiteBinder : DiscreteBindingCallSiteBinderBase
    {
        public DiscreteBindingObjectCallSiteBinder(string moniker)
            : base(moniker)
        {
        }

        protected override Expression GetExpression(ExecutionContext executionContext)
        {
            IDiscreteBinding binding = this.GetBindingStrategy.GetBinding(executionContext);
            if (binding == null)
                throw new CodeGenerationException(String.Format("Could not find global binding {0}. This may indicate a bug in our code.", this.Moniker));
            return Expression.Constant(binding, this.ReturnType);
        }
    }
}

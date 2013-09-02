using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    public class SymbolCallSiteBinder : DynamicMetaObjectBinder, ICallSiteBinderCacheItem<string>
    {
        public string SymbolKey { get; private set; }

        public SymbolCallSiteBinder(string symbolKey)
        {
            if (symbolKey == null)
                throw new ArgumentNullException();
            this.SymbolKey = symbolKey;
        }

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ExecutionContext executionContext = null;
            if (target != null)
                executionContext = target.Value as ExecutionContext;
            if (executionContext == null)
                // If this is null, the binder was not used by a Smalltalk method. Or may be somebody passed null for the ExecutionContext, which is illegal too.
                throw new ImplementationException("The SymbolCallSiteBinder can only be used in methods where the signature is (object, ExecutionContext, ...)");

            Symbol symbol = executionContext.Runtime.GetSymbol(this.SymbolKey);

            Expression expr = Expression.Constant(symbol, typeof(object));
            BindingRestrictions restictions = BindingRestrictions.GetExpressionRestriction(
                Expression.Equal(
                    Expression.Field(args[0].Expression, typeof(ExecutionContext).GetField("Runtime")),
                    Expression.Constant(executionContext.Runtime, typeof(SmalltalkRuntime))));

            return new DynamicMetaObject(expr, restictions);
        }

        #region Call-Site-Binder Cache Support

        string ICallSiteBinderCacheItem<string>.CacheKey
        {
            get { return this.SymbolKey; }
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

        ~SymbolCallSiteBinder()
        {
            if (this._finalizationManager != null)
                this._finalizationManager.InternalRemoveItem(this.SymbolKey);
        }

        #endregion

    }
}

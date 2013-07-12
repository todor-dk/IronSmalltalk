using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.ExpressionCompiler.Bindings
{
    public sealed class SpecialBinding : NameBinding
    {
        public Func<IBindingClient, Expression> GetReadExpressionFunction { get; private set; }

        public bool IsConstantValue { get; private set; }

        public SpecialBinding(string name, Func<IBindingClient, Expression> getReadExpressionFunction, bool isConstantValue)
            : base(name)
        {
            if (getReadExpressionFunction == null)
                throw new ArgumentNullException("getReadExpressionFunction");

            this.GetReadExpressionFunction = getReadExpressionFunction;
            this.IsConstantValue = isConstantValue;
        }
        
        public override Expression GenerateReadExpression(IBindingClient client)
        {
            return this.GetReadExpressionFunction(client);
        }

        /// <summary>
        /// This returns true if the value of the binding will always be the same. 
        /// Some read-only bindings (e.g. self, super) are NOT constant-value-bindings.
        /// </summary>
        public override bool IsConstantValueBinding
        {
            get { return this.IsConstantValue; }
        }
    }

}

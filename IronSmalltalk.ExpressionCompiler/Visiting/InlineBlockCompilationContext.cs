using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public sealed class InlineBlockCompilationContext : CompilationContext
    {
        public CompilationContext OuterContext { get; private set; }

        internal InlineBlockCompilationContext(CompilationContext outerContex)
            : base(outerContex.Compiler, outerContex.GlobalScope, outerContex.ReservedScope, outerContex.Self, outerContex.ExecutionContextArgument, outerContex.SuperLookupScope)
        {
            this.OuterContext = outerContex;
        }

        public override RootCompilationContext RootContext
        {
            get { return this.OuterContext.RootContext; }
        }

        #region Bindings and Arguments

        internal override NameBinding GetBinding(string name)
        {
            NameBinding result;
            result = this.ReservedScope.GetBinding(name);
            if (result != null)
                return result;

            result = this.LocalScope.GetBinding(name);
            if (result != null)
                return result;

            return this.OuterContext.GetBinding(name);
        }

        #endregion

        #region Compilation

        internal override Expression Return(Expression value)
        {
            return this.OuterContext.Return(value);
        }

        internal override Expression CompileLightweightExceptionCheck(Visiting.EncoderVisitor visitor, Expression expression)
        {
            return this.OuterContext.CompileLightweightExceptionCheck(visitor, expression);
        }

        internal override Expression GeneratePrologAndEpilogue(List<Expression> expressions)
        {
            Expression result;
            if ((this.Temporaries.Count == 0) && (expressions.Count == 1))
                result = expressions[0];
            else
                result = Expression.Block(this.Temporaries.Select(binding => binding.Expression), expressions);

            // Somebody requested explicit return
            if (this._ReturnLabel != null)
                result = Expression.Label(this._ReturnLabel, result);

            return result;
        }

        #endregion
    }
}

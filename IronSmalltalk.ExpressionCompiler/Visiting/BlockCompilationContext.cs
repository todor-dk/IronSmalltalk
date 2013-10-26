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
    public sealed class BlockCompilationContext : CompilationContext
    {
        public CompilationContext OuterContext { get; private set; }

        internal BlockCompilationContext(CompilationContext outerContex)
            : base(outerContex.Compiler, outerContex.GlobalScope, outerContex.ReservedScope, outerContex.Self, Expression.Parameter(typeof(ExecutionContext), "executionContext"), outerContex.SuperLookupScope)
        {
            this.OuterContext = outerContex;
            this.BlockArguments = new List<ArgumentBinding>();
        }

        public override RootCompilationContext RootContext
        {
            get { return this.OuterContext.RootContext; }
        }

        #region Bindings and Arguments

        public IReadOnlyList<ArgumentBinding> BlockArguments { get; private set; }

        internal override void DefineArgument(ArgumentBinding argument)
        {
            base.DefineArgument(argument);
            ((List<ArgumentBinding>)this.BlockArguments).Add(argument);
        }

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

        internal IEnumerable<ParameterExpression> GetLambdaParameters()
        {
            return (new ParameterExpression[] { (ParameterExpression) this.ExecutionContextArgument }).Concat(this.BlockArguments.Select(binding => (ParameterExpression)binding.Expression));
        }

        #endregion

        #region Compilation

        internal override Expression Return(Expression value)
        {
            if (this.Compiler.CompilerOptions.LightweightExceptions)
                return this.ReturnLocal(Expression.New(BlockResult.ConstructorInfo, this.RootContext.HomeContext, value));
            else
                return Expression.Throw(Expression.New(BlockResult.ConstructorInfo, this.RootContext.HomeContext, value), typeof(object));
        }

        internal override Expression CompileLightweightExceptionCheck(Visiting.EncoderVisitor visitor, Expression expression)
        {
            // C# Semantics:
            // var temp = <expression>
            // if (temp is BlockResult)
            //        return temp;
            // temp
            return Expression.Block(
                Expression.Assign(this.TempValue, expression),
                Expression.IfThen(
                    Expression.TypeIs(this.TempValue, typeof(BlockResult)),
                    this.ReturnLocal(this.TempValue)),
                this.TempValue);
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

            if (this._TempValue != null)
                result = Expression.Block(new ParameterExpression[] { this._TempValue }, result);

            return result;
        }

        #endregion
    }
}

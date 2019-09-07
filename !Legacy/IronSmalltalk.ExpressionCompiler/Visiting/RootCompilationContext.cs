using IronSmalltalk.Common.Internal;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    public sealed class RootCompilationContext : CompilationContext
    {
        internal RootCompilationContext(ExpressionCompiler compiler, BindingScope globalScope, BindingScope reservedScope, Expression self, Expression executionContext, IEnumerable<Expression> arguments, string superLookupScope, string codeName)
            : base(compiler, globalScope, reservedScope, self, executionContext, superLookupScope)
        {
            if (arguments == null)
                throw new ArgumentNullException("Arguments");

            this.MethodArguments = arguments.ToList().AsReadOnly();
            this.CodeName = codeName;
            this._HomeContext = null; // Lazy init
        }

        public override RootCompilationContext RootContext
        {
            get { return this; }
        }

        #region Bindings and Arguments

        /// <summary>
        /// Arguments passed to the method. This excludes "self" and "executionContext"
        /// </summary>
        public IReadOnlyList<Expression> MethodArguments { get; private set; }

        internal override NameBinding GetBinding(string name)
        {
            NameBinding result = this.ReservedScope.GetBinding(name);
            if (result != null)
                return result;

            result = this.LocalScope.GetBinding(name);
            if (result != null)
                return result;

            result = this.GlobalScope.GetBinding(name);
            if (result != null)
                return result;

            return new ErrorBinding(name);
        }

        #endregion

        #region Compilation

        private static readonly ConstructorInfo HomeContextCtor = TypeUtilities.Constructor(typeof(HomeContext));

        private Expression _HomeContext;
        private ParameterExpression _HomeContextVariable;
        internal Expression HomeContext
        {
            get
            {
                if (this._HomeContext == null)
                {
                    // Semantics are as follows:
                    // .... = ( (_HomeContext == null) ? (_HomeContext = new HomeContext()) : _HomeContext );
                    ParameterExpression homeContextVariable = Expression.Variable(typeof(HomeContext), "_HomeContext");
                    this._HomeContext = Expression.Condition(
                        Expression.ReferenceEqual(homeContextVariable, Expression.Constant(null, typeof(HomeContext))),
                        Expression.Assign(homeContextVariable, Expression.New(RootCompilationContext.HomeContextCtor)),
                        homeContextVariable,
                        typeof(HomeContext));
                    this._HomeContextVariable = homeContextVariable;
                }
                return this._HomeContext;
            }
        }

        //internal override Expression CompileLightweightExceptionCheck(Visiting.EncoderVisitor visitor, Expression expression)
        //{
        //    var na = this.HomeContext; // Init needed stuff
                
        //    // C# Semantics:
        //    // var temp = <expression>
        //    // if (temp is BlockResult)
        //    // {
        //    //      if (((BlockResult) temp).HomeContext == _HomeContext)
        //    //          return ((BlockResult) temp).Value;
        //    //      else
        //    //          return temp;
        //    // }
        //    // temp
        //    return Expression.Block(
        //        Expression.Assign(this.TempValue, expression),
        //        Expression.IfThen(
        //            Expression.TypeIs(this.TempValue, typeof(BlockResult)),
        //            Expression.IfThenElse(
        //                    // Improve ... unnecessary test if the  methods doesn't declare blocks.
        //                Expression.Equal(Expression.Field(Expression.Convert(this.TempValue, typeof(BlockResult)), BlockResult.HomeContextField), this._HomeContextVariable),
        //                this.ReturnLocal(Expression.Field(Expression.Convert(this.TempValue, typeof(BlockResult)), BlockResult.ValueField)),
        //                this.ReturnLocal(this.TempValue))),
        //        this.TempValue);
        //}

        //internal override Expression GeneratePrologAndEpilogue(List<Expression> expressions)
        //{
        //    Expression result;
        //    if ((this.Temporaries.Count == 0) && (expressions.Count == 1))
        //        result = expressions[0];
        //    else
        //        result = Expression.Block(this.Temporaries.Select(binding => binding.Expression), expressions);

        //    // Somebody requested explicit return
        //    if (this._ReturnLabel != null)
        //        result = Expression.Label(this._ReturnLabel, result);

        //    // Somebody requested the home context used for block returns ... we must handle this correctly with try ... catch ...
        //    if (this._HomeContext != null)
        //    {


        //        if (this.Compiler.CompilerOptions.LightweightExceptions)
        //        {
        //            result = Expression.Block(new ParameterExpression[] { this._HomeContextVariable }, result);
        //        }
        //        else
        //        {
        //            // Semantics:
        //            // HomeContext homeContext = new HomeContext();     ... however, this is lazy init'ed
        //            // try
        //            // {
        //            //      return <<result>>;
        //            // } 
        //            // .... this is how we would like to have it ... if we could. CLR limitations do not allow filters in dynamic methods ....
        //            // catch (BlockResult blockResult) where (blockResult.HomeContext == homeContext)       ... the where semantics are not part of C#
        //            // {
        //            //      return blockResult.Result;
        //            // }
        //            // .... therefore the following implementation ....
        //            // catch (BlockResult blockResult)
        //            // {
        //            //      if (blockResult.HomeContext == homeContext)
        //            //          return blockResult.Result;
        //            //      else
        //            //          throw;
        //            // }
        //            ParameterExpression blockResult = Expression.Parameter(typeof(BlockResult), "blockResult");
        //            CatchBlock catchBlock = Expression.Catch(
        //                blockResult,
        //                Expression.Condition(
        //                    Expression.ReferenceEqual(Expression.Field(blockResult, BlockResult.HomeContextField), this._HomeContextVariable),
        //                    Expression.Field(blockResult, BlockResult.ValueField),
        //                    Expression.Rethrow(typeof(object))));

        //            result = Expression.Block(new ParameterExpression[] { this._HomeContextVariable }, Expression.TryCatch(result, catchBlock));
        //        }
        //    }

        //    if (this._TempValue != null)
        //        result = Expression.Block(new ParameterExpression[] { this._TempValue }, result);

        //    return result;
        //}

        internal override Expression CompileLightweightExceptionCheck(Visiting.EncoderVisitor visitor, Expression expression)
        {
            var na = this.HomeContext; // Init needed stuff

            // C# Semantics:
            // var temp = <expression>
            // if (temp is BlockResult)
            // {
            //      if (((BlockResult) temp).HomeContext == _HomeContext)
            //          return ((BlockResult) temp).Value;
            //      else
            //          return temp;
            // }
            // temp
            var x = this.BlockReturnLabel;
            return Expression.Block(
                Expression.Assign(this.TempValue, expression),
                Expression.IfThen(
                    Expression.TypeIs(this.TempValue, typeof(BlockResult)),
                    Expression.Return(this.BlockReturnLabel, this.TempValue)),
                this.TempValue);
        }

        internal override Expression GeneratePrologAndEpilogue(List<Expression> expressions)
        {
            Expression result;
            if ((this.Temporaries.Count == 0) && (expressions.Count == 1))
                result = expressions[0];
            else
                result = Expression.Block(this.Temporaries.Select(binding => binding.Expression), expressions);


            if (this._BlockReturnLabel != null)
            {
                result = Expression.Label(this._BlockReturnLabel, result);

                // BUG BUG ... must be re-written

                result = Expression.Condition(
                    Expression.Equal(
                        Expression.Field(Expression.Convert(result, typeof(BlockResult)), BlockResult.HomeContextField), 
                        this._HomeContextVariable),
                    Expression.Field(Expression.Convert(result, typeof(BlockResult)), BlockResult.ValueField),
                    result);
            }

            // Somebody requested explicit return
            if (this._ReturnLabel != null)
                result = Expression.Label(this._ReturnLabel, result);


            // Somebody requested the home context used for block returns ... we must handle this correctly with try ... catch ...
            if (this._HomeContext != null)
            {


                if (this.Compiler.CompilerOptions.LightweightExceptions)
                {
                    result = Expression.Block(new ParameterExpression[] { this._HomeContextVariable }, result);
                }
                else
                {
                    // Semantics:
                    // HomeContext homeContext = new HomeContext();     ... however, this is lazy init'ed
                    // try
                    // {
                    //      return <<result>>;
                    // } 
                    // .... this is how we would like to have it ... if we could. CLR limitations do not allow filters in dynamic methods ....
                    // catch (BlockResult blockResult) where (blockResult.HomeContext == homeContext)       ... the where semantics are not part of C#
                    // {
                    //      return blockResult.Result;
                    // }
                    // .... therefore the following implementation ....
                    // catch (BlockResult blockResult)
                    // {
                    //      if (blockResult.HomeContext == homeContext)
                    //          return blockResult.Result;
                    //      else
                    //          throw;
                    // }
                    ParameterExpression blockResult = Expression.Parameter(typeof(BlockResult), "blockResult");
                    CatchBlock catchBlock = Expression.Catch(
                        blockResult,
                        Expression.Condition(
                            Expression.ReferenceEqual(Expression.Field(blockResult, BlockResult.HomeContextField), this._HomeContextVariable),
                            Expression.Field(blockResult, BlockResult.ValueField),
                            Expression.Rethrow(typeof(object))));

                    result = Expression.Block(new ParameterExpression[] { this._HomeContextVariable }, Expression.TryCatch(result, catchBlock));
                }
            }

            if (this._TempValue != null)
                result = Expression.Block(new ParameterExpression[] { this._TempValue }, result);

            return result;
        }



        private LabelTarget _BlockReturnLabel;
        private LabelTarget BlockReturnLabel
        {
            get
            {
                if (this._BlockReturnLabel == null)
                    this._BlockReturnLabel = Expression.Label(typeof(object), "blockreturn");
                return this._BlockReturnLabel;
            }
        }

        internal override Expression Return(Expression value)
        {
            return this.ReturnLocal(value);
        }

        private readonly string CodeName;

        private int BlockNumber = 0;

        internal string GetLambdaName(BlockVisitor visitor, BlockNode block)
        {
            this.BlockNumber++;
            if (String.IsNullOrWhiteSpace(this.CodeName))
                return null;
            else
                return String.Format(CultureInfo.InvariantCulture, "[{0}] in {1}", this.BlockNumber, this.CodeName);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IronSmalltalk.LambdaCompiler.Utils;

namespace IronSmalltalk.LambdaCompiler.Hacks
{
    internal class BlockN : BlockExpression
    {
        private IList<Expression> _expressions;         // either the original IList<Expression> or a ReadOnlyCollection if the user has accessed it.

        internal BlockN(IList<Expression> expressions)
        {
            Debug.Assert(expressions.Count != 0);

            _expressions = expressions;
        }

        //internal override Expression GetExpression(int index)
        //{
        //    Debug.Assert(index >= 0 && index < _expressions.Count);

        //    return _expressions[index];
        //}

        //internal override int ExpressionCount
        //{
        //    get
        //    {
        //        return _expressions.Count;
        //    }
        //}

        //internal override ReadOnlyCollection<Expression> GetOrMakeExpressions()
        //{
        //    return ReturnReadOnly(ref _expressions);
        //}

        //internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
        //{
        //    Debug.Assert(variables == null || variables.Count == 0);

        //    return new BlockN(args);
        //}


        internal static ReadOnlyCollection<T> ReturnReadOnly<T>(ref IList<T> collection)
        {
            IList<T> comparand = collection;
            ReadOnlyCollection<T> onlys = comparand as ReadOnlyCollection<T>;
            if (onlys != null)
            {
                return onlys;
            }
            Interlocked.CompareExchange<IList<T>>(ref collection, comparand.ToReadOnly<T>(), comparand);
            return (ReadOnlyCollection<T>)collection;
        }

 

    }
}

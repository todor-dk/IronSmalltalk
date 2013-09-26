/*
 * **************************************************************************
 *
 * Copyright (c) The IronSmalltalk Project. 
 *
 * This source code is subject to terms and conditions of the 
 * license agreement found in the solution directory. 
 * See: $(SolutionDir)\License.htm ... in the root of this distribution.
 * By using this source code in any fashion, you are agreeing 
 * to be bound by the terms of the license agreement.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * **************************************************************************
*/

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
    public sealed class ArrayCallSiteBinder : DynamicMetaObjectBinder
    {
        private readonly object[] ArrayTemplate;

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public ArrayCallSiteBinder(object[] arrayTemplate)
        {
            if (arrayTemplate == null)
                throw new ArgumentNullException();
            this.ArrayTemplate = arrayTemplate;
        }

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ExecutionContext executionContext = null;
            if (target != null)
                executionContext = target.Value as ExecutionContext;
            if (executionContext == null)
                // If this is null, the binder was not used by a Smalltalk method. Or may be somebody passed null for the ExecutionContext, which is illegal too.
                throw new ImplementationException("The ArrayCallSiteBinder can only be used in methods where the signature is (ExecutionContext)");

            object[] array = this.GetArray(executionContext);

            Expression expr = Expression.Constant(array, typeof(object));
            BindingRestrictions restictions = BindingRestrictions.GetExpressionRestriction(
                Expression.Equal(
                    Expression.Field(target.Expression, ExecutionContext.RuntimeField),
                    Expression.Constant(executionContext.Runtime, typeof(SmalltalkRuntime))));

            return new DynamicMetaObject(expr, restictions);
        }

        private object[] GetArray(ExecutionContext executionContext)
        {
            return this.GetArray(executionContext, this.ArrayTemplate);
        }

        private object[] GetArray(ExecutionContext executionContext, object[] template)
        {
            if (template == null)
                return null;
            object[] result = new object[template.Length];
            for (int i = 0; i < template.Length; i++)
            {
                object item = template[i];
                if (item is object[])
                    result[i] = this.GetArray(executionContext, (object[])item);
                else if (item is SymbolPlaceholder)
                    result[i] = executionContext.Runtime.GetSymbol(((SymbolPlaceholder)item).Value);
                else
                    result[i] = item;
            }
            return result;
        }
    }
}

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
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.Runtime.Behavior
{
    public class InitializerCompilationResult : CompilationResult<Expression<Func<object, ExecutionContext, object>>>
    {
        /// <summary>
        /// Create a new InitializerCompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        public InitializerCompilationResult(Expression<Func<object, ExecutionContext, object>> executableCode)
            : base(executableCode, null)
        {
        }

        /// <summary>
        /// Create a new InitializerCompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        /// <param name="restrictions">Optional restrictions attached to the executable code expression.</param>
        public InitializerCompilationResult(Expression<Func<object, ExecutionContext, object>> executableCode, BindingRestrictions restrictions)
            : base(executableCode, restrictions)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Behavior
{
    public class InitializerCompilationResult : CompilationResult<Expression<Func<SmalltalkRuntime, object, object>>>
    {
        /// <summary>
        /// Create a new InitializerCompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        public InitializerCompilationResult(Expression<Func<SmalltalkRuntime, object, object>> executableCode)
            : base(executableCode, null)
        {
        }

        /// <summary>
        /// Create a new InitializerCompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        /// <param name="restrictions">Optional restrictions attached to the executable code expression.</param>
        public InitializerCompilationResult(Expression<Func<SmalltalkRuntime, object, object>> executableCode, BindingRestrictions restrictions)
            : base(executableCode, restrictions)
        {
        }
    }
}

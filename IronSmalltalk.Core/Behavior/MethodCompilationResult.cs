using System;
using System.Dynamic;
using System.Linq.Expressions;

namespace IronSmalltalk.Runtime.Behavior
{
    public class MethodCompilationResult : CompilationResult<Expression>
    {
        /// <summary>
        /// Create a new MethodCompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        public MethodCompilationResult(Expression executableCode)
            : base(executableCode, null)
        {
        }

        /// <summary>
        /// Create a new MethodCompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        /// <param name="restrictions">Optional restrictions attached to the executable code expression.</param>
        public MethodCompilationResult(Expression executableCode, BindingRestrictions restrictions)
            : base(executableCode, restrictions)
        {
        }
    }
}

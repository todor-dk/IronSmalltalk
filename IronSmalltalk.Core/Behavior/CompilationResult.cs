using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace IronSmalltalk.Runtime.Behavior
{
    /// <summary>
    /// Compilation result returned when compiling an IntermediateCode. 
    /// </summary>
    /// <typeparam name="TExpression">Type of the executable code expression.</typeparam>
    public abstract class CompilationResult<TExpression>
        where TExpression : Expression
    {
        /// <summary>
        /// The Expression for the executable code.
        /// </summary>
        public TExpression ExecutableCode { get; private set; }

        /// <summary>
        /// Optional restrictions attached to the executable code expression.
        /// </summary>
        public BindingRestrictions Restrictions { get; private set; }


        /// <summary>
        /// Create a new CompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        public CompilationResult(TExpression executableCode)
            : this(executableCode, null)
        {
        }

        /// <summary>
        /// Create a new CompilationResult.
        /// </summary>
        /// <param name="executableCode">The Expression for the executable code.</param>
        /// <param name="restrictions">Optional restrictions attached to the executable code expression.</param>
        public CompilationResult(TExpression executableCode, BindingRestrictions restrictions)
        {
            if (executableCode == null)
                throw new ArgumentNullException("executableCode");
            this.ExecutableCode = executableCode;
            this.Restrictions = restrictions;
        }

        /// <summary>
        /// Merge the restrictions of this compilation result with the given restrictions.
        /// </summary>
        /// <param name="restrictions">Restrictions to merge with.</param>
        /// <returns>The merged binding restrictions.</returns>
        public BindingRestrictions MergeRestrictions(BindingRestrictions restrictions)
        {
            if (restrictions == null)
                return this.Restrictions;
            if (this.Restrictions == null)
                return restrictions;
            return restrictions.Merge(this.Restrictions);
        }

        /// <summary>
        /// Merge the restrictions of the given compilation results.
        /// </summary>
        /// <param name="compilationResults">Compilation results whos restrictions are to be merge.</param>
        /// <returns>The merged binding restrictions.</returns>
        public static BindingRestrictions MergeRestrictions(IEnumerable<CompilationResult<TExpression>> compilationResults)
        {
            BindingRestrictions result = null;
            foreach (var compilationResult in compilationResults)
                result = compilationResult.MergeRestrictions(result);
            return result;
        }

        /// <summary>
        /// Create a DynamicMetaObject for the current compilation result containd 
        /// the merged restrictions of this compilation result with the given restrictions.
        /// </summary>
        /// <param name="restrictions">Optional. Restrictions to merge with.</param>
        /// <returns>DynamicMetaObject for the executable code and the merged restrictions.</returns>
        public DynamicMetaObject GetDynamicMetaObject(BindingRestrictions restrictions)
        {
            return new DynamicMetaObject(this.ExecutableCode, this.MergeRestrictions(restrictions));
        }
    }
}

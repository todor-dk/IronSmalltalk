using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IronSmalltalk.Common
{
    public static class Contract
    {
        /// <summary>
        /// Validate that the given value is not null.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to be validated.</typeparam>
        /// <param name="value">Value to validate for null.</param>
        /// <param name="name">Name of the argument (value) being validated.</param>
        /// <exception cref="ArgumentNullException"> is thrown if the given value is null.</exception>
        [System.Diagnostics.Contracts.ContractAbbreviator]
        [System.Diagnostics.Contracts.ContractArgumentValidator]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresNotNull<TValue>(TValue value, string name)
        {
#if DEBUG
            if (value == null)
                throw new ArgumentNullException(name);
#endif
        }
    }
}

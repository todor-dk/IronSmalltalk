using IronSmalltalk.Common.Internal;
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
        public static void RequiresNotNull<TValue>([ValidatedNotNull] TValue value, string name)
        {
#if DEBUG
            if (value == null)
                throw new ArgumentNullException(name);
#endif
        }

        /// <summary>
        /// Validate that the given string is not null, empty or contains only whitespaces.
        /// </summary>
        /// <param name="value">String to validate.</param>
        /// <param name="name">Name of the argument (string) being validated.</param>
        /// <exception cref="ArgumentNullException"> is thrown if the given string is null, empty or contains only whitespaces.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresNotEmptyOrWhiteSpace(string value, string name)
        {
#if DEBUG
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(name);
#endif
        }

        /// <summary>
        /// This methods can be used to trick code analyzers that a parameter is used.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="value">This value is not used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060, CA1801 // Remove unused parameter
        public static void ParameterNotUsed<TValue>(TValue value)
#pragma warning restore IDE0060, CA1801 // Remove unused parameter
        {
        }
    }
}

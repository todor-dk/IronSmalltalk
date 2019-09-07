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
using System.Globalization;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.LexicalAnalysis;

namespace IronSmalltalk.Compiler.LexicalTokens
{
    /// <summary>
    /// Quoted selector token as described in X3J20 "3.5.10 Quoted Selector".
    /// </summary>
    /// <remarks>
    /// The string value excludes the preceding '#' hash char.
    /// </remarks>
    public class QuotedSelectorToken : LiteralToken<string>
    {
        /// <summary>
        /// Create a new quoted selector token.
        /// </summary>
        /// <param name="value">String value of the selector, excluding the preceding '#' hash char.</param>
        public QuotedSelectorToken(string value)
            : base(value)
        {
			Contract.RequiresNotNull(value, nameof(value));
        }

        /// <summary>
        /// The (part of the) source code that the token represents.
        /// </summary>
        public override string SourceString
        {
            get { return LexicalConstants.QuotedSelectorDelimiter.ToString(CultureInfo.InvariantCulture) + this.Value; }
        }
    }
}

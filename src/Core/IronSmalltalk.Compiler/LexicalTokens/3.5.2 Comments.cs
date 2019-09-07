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
using IronSmalltalk.Compiler.LexicalAnalysis;

namespace IronSmalltalk.Compiler.LexicalTokens
{
    /// <summary>
    /// Comments token as described in X3J20 "3.5.2 Comments".
    /// The Value property contains the comment text. This excludes the delimiting " (quote) characters.
    /// </summary>
    public class CommentToken : WhitespaceToken
    {
        /// <summary>
        /// Create a new Comment Token.
        /// </summary>
        /// <param name="comment">Comment text. This excludes the delimiting " (quote) characters.</param>
        public CommentToken(string comment)
            : base(comment)
        {
#if DEBUG
            if (comment.IndexOf(LexicalConstants.CommentDelimiter) != -1)
                throw new ArgumentException("No delimiters inside comment text.", nameof(comment)); // No delimiters inside comment text!
#endif
        }

        /// <summary>
        /// The (part of the) source code that the token represents.
        /// </summary>
        public override string SourceString
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0}{1}{0}", LexicalConstants.CommentDelimiter, this.Value); }
        }
    }
}

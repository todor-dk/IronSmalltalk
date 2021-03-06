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

using IronSmalltalk.Compiler.LexicalTokens;

namespace IronSmalltalk.Compiler.SemanticNodes
{
    /// <summary>
    /// Parse node representing a symbol literal constant.
    /// </summary>
    public partial class SymbolLiteralNode : SingleValueLiteralNode<HashedStringToken>
    {
        /// <summary>
        /// Create and initialize a new symbol literal node.
        /// </summary>
        /// <param name="parent">Parent node that defines this literal node.</param>
        /// <param name="token">Token defining the value of the literal node.</param>
        protected internal SymbolLiteralNode(ILiteralNodeParent parent, HashedStringToken token)
            : base(parent, token)
        {
        }
    }
}

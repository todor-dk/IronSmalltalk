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

using System.Linq.Expressions;
using IronSmalltalk.Common;

namespace IronSmalltalk.ExpressionCompiler.Runtime
{
    public interface IDebugInfoService
    {
        /// <summary>
        /// Translate the locations of source references from relative to absolute positions.
        /// </summary>
        /// <param name="position">Relative source location.</param>
        /// <returns>Absolute source location.</returns>
        SourceLocation TranslateSourcePosition(SourceLocation position);

        /// <summary>
        /// Get or set the document containing the debug symbols.
        /// </summary>
        SymbolDocumentInfo SymbolDocument { get; }
    }
}

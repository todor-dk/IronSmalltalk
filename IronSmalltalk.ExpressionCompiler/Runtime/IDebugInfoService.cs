using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;

namespace IronSmalltalk.Runtime.Behavior
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

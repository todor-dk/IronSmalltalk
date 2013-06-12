using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Installer;

namespace IronSmalltalk.InterchangeInstaller.Compiler
{
    public class DebugInfoService : IDebugInfoService
    {
        /// <summary>
        /// Source code service for translating source code positions.
        /// </summary>
        public ISourceCodeReferenceService SourceCodeService { get; private set; }


        public DebugInfoService(ISourceCodeReferenceService sourceCodeService)
        {
            if (sourceCodeService == null)
                throw new ArgumentNullException();
            this.SourceCodeService = sourceCodeService;
        }

        public SourceLocation TranslateSourcePosition(SourceLocation position)
        {
            return this.SourceCodeService.TranslateSourcePosition(position);
        }

        public System.Linq.Expressions.SymbolDocumentInfo SymbolDocument
        {
            get
            {
                ISymbolDocumentProvider sdp = (ISymbolDocumentProvider)this.SourceCodeService.SourceObject;
                return sdp.SymbolDocument;
            }
        }
    }

    public interface ISymbolDocumentProvider
    {
        SymbolDocumentInfo SymbolDocument { get; }
    }
}

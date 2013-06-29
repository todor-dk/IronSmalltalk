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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.DefinitionInstaller;
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.ExpressionCompiler.Runtime;
using IronSmalltalk.Runtime.Behavior;

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

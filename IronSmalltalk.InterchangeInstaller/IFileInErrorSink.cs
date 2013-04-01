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
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;

namespace IronSmalltalk.Interchange
{
    public interface IFileInErrorSink : IInterchangeErrorSink
    {
        /// <summary>
        /// Report an error encountered during install (validation phase) of definitions from interchange format source code file.
        /// </summary>
        /// <param name="startPosition">Source code start position.</param>
        /// <param name="stopPosition">source code end position.</param>
        /// <param name="errorMessage">Error message describing an issue in the interchange format file.</param>
        void AddInstallError(SourceLocation startPosition, SourceLocation stopPosition, string errorMessage);
    }
}

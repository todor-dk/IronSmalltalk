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

namespace IronSmalltalk.AstJitCompiler.Runtime
{
    /// <summary>
    /// Error sink for reporting errors while validating intermediate code.
    /// </summary>
    public interface IRuntimeCodeValidationErrorSink
    {
        /// <summary>
        /// Report an intermediate code validation error.
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="start">Start location in the source code where the error occured.</param>
        /// <param name="stop">Stop location in the source code where the error occured.</param>
        // <returns></returns>
        void ReportError(string errorMessage, SourceLocation start, SourceLocation stop);
    }
}

﻿/*
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
using IronSmalltalk.Common;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;

namespace IronSmalltalk.DefinitionInstaller.Definitions
{
    public interface IMethodFactory
    {
        CompiledMethod CreateMethod(MethodDefinition definition, IDefinitionInstallerContext installer, SmalltalkClass cls, CompiledMethod.MethodType methodType);

        bool ValidateMethod(MethodDefinition definition, IDefinitionInstallerContext installer, SmalltalkClass cls, CompiledMethod.MethodType methodType, ICodeValidationErrorSink errorSink);
    }

    /// <summary>
    /// Error sink for reporting errors while validating intermediate code.
    /// </summary>
    public interface ICodeValidationErrorSink
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

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

namespace IronSmalltalk.Runtime.Installer
{
    /// <summary>
    /// Represents a reference to a source code.
    /// </summary>
    /// <remarks>
    /// Having reference to source location is optional. In other words, it is
    /// legal to let the source location attributes be empty and just set the Value.
    /// </remarks>
    public interface ISourceReference
    {
        /// <summary>
        /// Absolute start location in the source code that defines the value.
        /// </summary>
        SourceLocation StartPosition { get; }

        /// <summary>
        /// Absolute stop location in the source code that defines the value.
        /// </summary>
        SourceLocation StopPosition { get; }

        /// <summary>
        /// Return the source code service for this source code reference.
        /// The service can translate the locations of source references from relative to absolute positions.
        /// </summary>
        ISourceCodeReferenceService Service { get; }
    }
}

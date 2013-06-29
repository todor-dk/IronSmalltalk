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

using IronSmalltalk.Common;

namespace IronSmalltalk.DefinitionInstaller
{
    /// <summary>
    /// Service that can translate the locations of source references from relative to absolute positions.
    /// </summary>
    /// <remarks>
    /// This service is implemented by the reader/processor of source code files. 
    /// It is used by the SourceReference class to convert relative source code positions
    /// to absolute positions within the source code file. 
    /// 
    /// This functionality is optional but helps identifying to the end-user the exact source of an error.
    /// </remarks>
    public interface ISourceCodeReferenceService
    {
        /// <summary>
        /// Translate the locations of source references from relative to absolute positions.
        /// </summary>
        /// <param name="position">Relative source location.</param>
        /// <returns>Absolute source location.</returns>
        int TranslateSourcePosition(int position);

        /// <summary>
        /// Translate the locations of source references from relative to absolute positions.
        /// </summary>
        /// <param name="position">Relative source location.</param>
        /// <returns>Absolute source location.</returns>
        SourceLocation TranslateSourcePosition(SourceLocation position);

        /// <summary>
        /// Get the source (file-in object / source file) where the source code references reside.
        /// </summary>
        /// <remarks>
        /// Due to the assembly dependencies, it's not possible to have this property strongly types.
        /// This is implemented and used by the Interchange Installer.
        /// </remarks>
        object SourceObject { get; }
    }
}

/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Scripting.Utils;
using System.Runtime.CompilerServices;
using System.Collections;

namespace Microsoft.Scripting {


    /// <summary>
    /// Abstracts system operations that are used by DLR and could potentially be platform specific.
    /// The host can implement its PAL to adapt DLR to the platform it is running on.
    /// For example, the Silverlight host adapts some file operations to work against files on the server.
    /// </summary>
    [Serializable]
    public class PlatformAdaptationLayer {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly PlatformAdaptationLayer Default = new PlatformAdaptationLayer();

        public static readonly bool IsCompactFramework =
            Environment.OSVersion.Platform == PlatformID.WinCE ||
            Environment.OSVersion.Platform == PlatformID.Xbox;

        #region Assembly Loading

        public virtual Assembly LoadAssembly(string name) {
            return Assembly.Load(name);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
        public virtual Assembly LoadAssemblyFromPath(string path) {
            return Assembly.LoadFile(path);
        }

        public virtual void TerminateScriptExecution(int exitCode) {
            System.Environment.Exit(exitCode);
        }

        #endregion

        #region Virtual File System

        public virtual bool IsSingleRootFileSystem {
            get {
                return Environment.OSVersion.Platform == PlatformID.Unix
                    || Environment.OSVersion.Platform == PlatformID.MacOSX;
            }
        }

        public virtual StringComparer PathComparer {
            get {
                return Environment.OSVersion.Platform == PlatformID.Unix ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            }
        }

        public virtual bool FileExists(string path) {
            return File.Exists(path);
        }

        public virtual bool DirectoryExists(string path) {
            return Directory.Exists(path);
        }

        // TODO: better APIs
        public virtual Stream OpenFileStream(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read, int bufferSize = 8192) {
            if (string.Equals("nul", path, StringComparison.InvariantCultureIgnoreCase)) {
                return Stream.Null;
            }
            return new FileStream(path, mode, access, share, bufferSize);
        }

        // TODO: better APIs
        public virtual Stream OpenInputFileStream(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, int bufferSize = 8192) {
            return OpenFileStream(path, mode, access, share, bufferSize);
        }

        // TODO: better APIs
        public virtual Stream OpenOutputFileStream(string path) {
            return OpenFileStream(path, FileMode.Create, FileAccess.Write);
        }

        public virtual void DeleteFile(string path, bool deleteReadOnly) {
            FileInfo info = new FileInfo(path);
            if (deleteReadOnly && info.IsReadOnly) {
                info.IsReadOnly = false;
            }
            info.Delete();
        }

        public string[] GetFiles(string path, string searchPattern) {
            return GetFileSystemEntries(path, searchPattern, true, false);
        }

        public string[] GetDirectories(string path, string searchPattern) {
            return GetFileSystemEntries(path, searchPattern, false, true);
        }

        public string[] GetFileSystemEntries(string path, string searchPattern) {
            return GetFileSystemEntries(path, searchPattern, true, true);
        }

        public virtual string[] GetFileSystemEntries(string path, string searchPattern, bool includeFiles, bool includeDirectories) {
            if (includeFiles && includeDirectories) {
                return Directory.GetFileSystemEntries(path, searchPattern);
            }
            if (includeFiles) {
                return Directory.GetFiles(path, searchPattern);
            }
            if (includeDirectories) {
                return Directory.GetDirectories(path, searchPattern);
            }
            return ArrayUtils.EmptyStrings;
        }

        /// <exception cref="ArgumentException">Invalid path.</exception>
        public virtual string GetFullPath(string path) {
            try {
                return Path.GetFullPath(path);
            } catch (Exception) {
                throw Error.InvalidPath();
            }
        }

        public virtual string CombinePaths(string path1, string path2) {
            return Path.Combine(path1, path2);
        }

        public virtual string GetFileName(string path) {
            return Path.GetFileName(path);
        }

        public virtual string GetDirectoryName(string path) {
            return Path.GetDirectoryName(path);
        }

        public virtual string GetExtension(string path) {
            return Path.GetExtension(path);
        }

        public virtual string GetFileNameWithoutExtension(string path) {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <exception cref="ArgumentException">Invalid path.</exception>
        public virtual bool IsAbsolutePath(string path) {
            if (String.IsNullOrEmpty(path)) {
                return false;
            }

            // no drives, no UNC:
            if (IsSingleRootFileSystem) {
                return IsDirectorySeparator(path[0]);
            }

            if (IsDirectorySeparator(path[0])) {
                // UNC path
                return path.Length > 1 && IsDirectorySeparator(path[1]);
            }

            if (path.Length > 2 && path[1] == ':' && IsDirectorySeparator(path[2])) {
                return true;
            }

            return false;
        }

        private bool IsDirectorySeparator(char c) {
            return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
        }

        public virtual string CurrentDirectory {
            get {
                return Directory.GetCurrentDirectory();
            }
            set {
                Directory.SetCurrentDirectory(value);
            }
        }

        public virtual void CreateDirectory(string path) {
            Directory.CreateDirectory(path);
        }

        public virtual void DeleteDirectory(string path, bool recursive) {
            Directory.Delete(path, recursive);
        }

        public virtual void MoveFileSystemEntry(string sourcePath, string destinationPath) {
            Directory.Move(sourcePath, destinationPath);
        }

        #endregion

        #region Environmental Variables

        public virtual string GetEnvironmentVariable(string key) {
            return Environment.GetEnvironmentVariable(key);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        public virtual void SetEnvironmentVariable(string key, string value) {
            if (value != null && value.Length == 0) {
                SetEmptyEnvironmentVariable(key);
            } else {
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2149:TransparentMethodsMustNotCallNativeCodeFxCopRule")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetEmptyEnvironmentVariable(string key) {
            // System.Environment.SetEnvironmentVariable interprets an empty value string as 
            // deleting the environment variable. So we use the native SetEnvironmentVariable 
            // function here which allows setting of the value to an empty string.
            // This will require high trust and will fail in sandboxed environments
            if (!NativeMethods.SetEnvironmentVariable(key, String.Empty)) {
                throw new ExternalException("SetEnvironmentVariable failed", Marshal.GetLastWin32Error());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual Dictionary<string, string> GetEnvironmentVariables() {
            var result = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                result.Add((string)entry.Key, (string)entry.Value);
            }

            return result;
        }

        #endregion
    }
}

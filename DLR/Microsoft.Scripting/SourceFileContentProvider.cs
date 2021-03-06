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

using System.Runtime.Remoting;
using System;
using System.IO;
using Microsoft.Scripting.Utils;

namespace Microsoft.Scripting {

    /// <summary>
    /// Provides a StreamContentProvider for a stream of content backed by a file on disk.
    /// </summary>
    [Serializable]
    internal sealed class FileStreamContentProvider : StreamContentProvider {
        private readonly string _path;
        private readonly PALHolder _pal;

        internal string Path {
            get { return _path; }
        }

        #region Construction

        internal FileStreamContentProvider(PlatformAdaptationLayer pal, string path) {
            Assert.NotNull(pal, path);

            _path = path;
            _pal = new PALHolder(pal);
        }

        #endregion

        public override Stream GetStream() {
            return _pal.GetStream(Path);
        }

        [Serializable]
        private class PALHolder : MarshalByRefObject {
            [NonSerialized]
            private readonly PlatformAdaptationLayer _pal;

            internal PALHolder(PlatformAdaptationLayer pal) {
                _pal = pal;
            }

            internal Stream GetStream(string path) {
                return _pal.OpenInputFileStream(path);
            }

            // TODO: Figure out what is the right lifetime
            public override object InitializeLifetimeService() {
                return null;
            }
        }
    }
}

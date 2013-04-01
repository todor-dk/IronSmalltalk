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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Interchange
{
    public abstract class FileInInformation
    {
        /// <summary>
        /// Error sink for reporting errors encountered during processing of the source code contained in interchange file.
        /// </summary>
        public IFileInErrorSink ErrorSink { get; set; }

        /// <summary>
        /// Get the text reader that contains the source code.
        /// </summary>
        /// <returns></returns>
        public abstract TextReader GetTextReader();
    }

    public class StringFileInInformation : FileInInformation
    {
        /// <summary>
        /// The source code to be filed in.
        /// </summary>
        public string Code { get; set; }

        public StringFileInInformation(string code)
        {
            this.Code = code; // null is OK
        }

        public StringFileInInformation(string code, IFileInErrorSink errorSink)
            : this(code)
        {
            this.ErrorSink = errorSink;
        }

        public override TextReader GetTextReader()
        {
            if (this.Code == null)
                return new StringReader(String.Empty);
            else
                return new StringReader(this.Code);
        }
    }

    public class PathFileInInformation : FileInInformation
    {
        /// <summary>
        /// Path to file containing source code to be filed in.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Encoding representing the binary encoding of the source stream.
        /// </summary>
        public Encoding Encoding { get; private set; }

        public PathFileInInformation(string path, Encoding encoding)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            this.Path = path;
            this.Encoding = encoding;
        }

        public PathFileInInformation(string path, Encoding encoding, IFileInErrorSink errorSink)
            : this(path, encoding)
        {
            this.ErrorSink = errorSink;
        }

        public override TextReader GetTextReader()
        {
            return new StreamReader(this.Path, this.Encoding);
        }
    }

    public class DelegateFileInInformation : FileInInformation
    {
        /// <summary>
        /// Path to file containing source code to be filed in.
        /// </summary>
        public Func<TextReader> GetReaderFunction { get; private set; }

        public DelegateFileInInformation(Func<TextReader> getReaderFunction)
        {
            if (getReaderFunction == null)
                throw new ArgumentNullException("getReaderFunction");
            this.GetReaderFunction = getReaderFunction;
        }

        public DelegateFileInInformation(Func<TextReader> getReaderFunction, IFileInErrorSink errorSink)
            : this(getReaderFunction)
        {
            this.ErrorSink = errorSink;
        }

        public override TextReader GetTextReader()
        {
            TextReader reader = this.GetReaderFunction();
            if (reader == null)
                throw new InvalidOperationException("The GetReaderFunction should not have returned null.");
            return reader;
        }
    }
}

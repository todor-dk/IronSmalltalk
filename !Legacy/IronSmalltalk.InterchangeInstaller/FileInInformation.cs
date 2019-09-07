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
using System.IO;
using System.Linq.Expressions;
using System.Text;
using IronSmalltalk.InterchangeInstaller.Compiler;

namespace IronSmalltalk.InterchangeInstaller
{
    public abstract class FileInInformation : ISymbolDocumentProvider
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

        /// <summary>
        /// Get or set the document containing the debug symbols.
        /// </summary>
        public SymbolDocumentInfo SymbolDocument { get; protected set; }
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
            : this(path, encoding, null, null)
        {
        }

        public PathFileInInformation(string path, Encoding encoding, IFileInErrorSink errorSink, SymbolDocumentInfo symbolDocument)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            this.Path = path;
            this.Encoding = encoding;
            this.ErrorSink = errorSink; // null is OK
            this.SymbolDocument = symbolDocument; // null is OK
        }

        public PathFileInInformation(string path, Encoding encoding, IFileInErrorSink errorSink)
            : this(path, encoding, errorSink, null)
        {
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
            : this(getReaderFunction, errorSink, null)
        {
        }

        public DelegateFileInInformation(Func<TextReader> getReaderFunction, IFileInErrorSink errorSink, SymbolDocumentInfo symbolDocument)
            : this(getReaderFunction)
        {
            this.ErrorSink = errorSink;
            this.SymbolDocument = symbolDocument;
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

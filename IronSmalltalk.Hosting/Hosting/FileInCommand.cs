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
using IronSmalltalk.Hosting.Hosting;
using Microsoft.Scripting;

namespace IronSmalltalk.Runtime.Hosting
{
    public abstract class FileInCommand
    {
        public abstract SourceUnit GetSourceUnit(SmalltalkLanguageContext language);
    }

    public class FileInFile : FileInCommand
    {
        public string File { get; private set; }

        public FileInFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            this.File = path;
        }

        public override SourceUnit GetSourceUnit(SmalltalkLanguageContext language)
        {
            return language.CreateFileUnit(this.File, language.DefaultEncoding, SourceCodeKind.File);
        }
    }

    public class FileInCode : FileInCommand
    {
        public string Code { get; private set; }

        public FileInCode(string code)
        {
            if (code == null)
                this.Code = String.Empty;
            else
                this.Code = code;
        }

        public override SourceUnit GetSourceUnit(SmalltalkLanguageContext language)
        {
            return language.CreateSnippet(this.Code, SourceCodeKind.Statements);
        }
    }
}

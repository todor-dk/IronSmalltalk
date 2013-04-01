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
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace IronSmalltalk.Hosting.Host
{
    public abstract class SmalltalkCommandLineCommand
    {
        public abstract int Execute(SmalltalkCommandLine commandLine);
    }



    public class EvaluateFile : SmalltalkCommandLineCommand
    {
        public string File { get; private set; }

        public EvaluateFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            this.File = path;
        }

        public override int Execute(SmalltalkCommandLine commandLine)
        {
            ScriptSource source = commandLine.Engine.CreateScriptSourceFromFile(this.File, commandLine.SmalltalkLanguageContext.DefaultEncoding);
            return commandLine.RunFile(source);
        }
    }


    public class EvaluateCode : SmalltalkCommandLineCommand
    {
        public string Code { get; private set; }

        public EvaluateCode(string code)
        {
            if (code == null)
                this.Code = String.Empty;
            else
                this.Code = code;
        }

        public override int Execute(SmalltalkCommandLine commandLine)
        {
            ScriptSource source = commandLine.Engine.CreateScriptSourceFromString(this.Code, SourceCodeKind.Statements);
            return commandLine.RunFile(source);
        }
    }
}

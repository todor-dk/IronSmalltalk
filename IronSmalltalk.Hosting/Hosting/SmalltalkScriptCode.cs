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
using System.Linq.Expressions;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution;
using Microsoft.Scripting;

namespace IronSmalltalk.Runtime.Hosting
{   
    /// <summary>
    /// SmalltalkScriptCode is an instance of Smalltalk compiled code that is bound to a specific 
    /// SmalltalkLanguageContext but not a specific ScriptScope. The code can be re-executed multiple 
    /// times in different scopes. Hosting API counterpart for this class is <c>CompiledCode</c>.
    /// </summary>
    /// <typeparam name="TEnvironment">Type of the SmalltalkEnvironment passed to the target delegate.</typeparam>
    public class SmalltalkScriptCode : ScriptCode
    {
        /// <summary>
        /// Create a new SmalltalkScriptCode.
        /// </summary>
        /// <param name="code">Initializer code representing the code to be execution.</param>
        /// <param name="runtime">The IronSmalltalk Runtime this source is bound to.</param>
        /// <param name="sourceUnit">SourceUnit that resulted in the generated code.</param>
        public SmalltalkScriptCode(CompiledInitializer code, SmalltalkRuntime runtime, SourceUnit sourceUnit)
            : base(sourceUnit)
        {
            if (code == null)
                throw new ArgumentNullException("code");
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (sourceUnit == null)
                throw new ArgumentNullException("sourceUnit");
            this.Code = code;
            this.Runtime = runtime;
        }

        /// <summary>
        /// The IronSmalltalk Runtime this source is bound to.
        /// </summary>
        public SmalltalkRuntime Runtime { get; private set; }

        /// <summary>
        /// This is the initializer code representing the code to be execution.
        /// </summary>
        public CompiledInitializer Code { get; private set; }

        /// <summary>
        /// Run the code in the given scope.
        /// </summary>
        /// <param name="scope">ScriptScope to be used when running the code.</param>
        /// <returns>The value of the last evaluated expression.</returns>
        public override object Run(Microsoft.Scripting.Runtime.Scope scope)
        {
            // This is where the magic happens.
            // 1. The Target property compiled the code to an executable delegate.
            // 2. We execute it with:
            //      - The SmalltalkEnvironment (that is bound to the LanguageContext)
            //      - The receiver ... which is currently set to nil (null).
            return this.Code.Execute(null, new ExecutionContext(this.Runtime));
        }
    }
}

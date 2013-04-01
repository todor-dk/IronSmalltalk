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
using System.Threading;
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
        /// <param name="code">Expression/Expression Tree code representing the code to be execution.</param>
        /// <param name="runtime">The IronSmalltalk Runtime this source is bound to.</param>
        /// <param name="sourceUnit">SourceUnit that resulted in the generated code.</param>
        public SmalltalkScriptCode(Expression<Func<SmalltalkRuntime, object, object>> code, SmalltalkRuntime runtime, SourceUnit sourceUnit)
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
        /// This is the Expression/Expression Tree code representing the code to be execution.
        /// </summary>
        /// <remarks>
        /// This is equivelent to metadata for the actual IL code to be generated.
        /// </remarks>
        public Expression<Func<SmalltalkRuntime, object, object>> Code { get; private set; }

        /// <summary>
        /// Private variable being lazy-initialized and that holds the compiled executable code.
        /// </summary>
        private Func<SmalltalkRuntime, object, object> _target;

        /// <summary>
        /// This is the compiled code (delegate) that can be executed.
        /// </summary>
        public Func<SmalltalkRuntime, object, object> Target
        {
            get
            {
                // If not compiled, compile the metadata to IL code (delegate).
                if (this._target == null)
                {
                    Func<SmalltalkRuntime, object, object> compiled = this.Code.Compile();
                    Interlocked.CompareExchange(ref this._target, compiled, null);
                }
                return this._target;
            }
        }

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
            return this.Target(this.Runtime, null);
        }
    }
}

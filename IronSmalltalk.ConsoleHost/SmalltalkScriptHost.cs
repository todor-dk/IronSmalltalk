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
using Microsoft.Scripting.Hosting;

namespace IronSmalltalk.Hosting.Host
{
    /// <summary>
    /// ScriptHost is collocated with ScriptRuntime in the same app-domain. 
    /// The host can implement a derived class to consume some notifications and/or 
    /// customize operations like TryGetSourceUnit,ResolveSourceUnit, etc.
    ///
    /// The areguments to the the constructor of the derived class are specified in ScriptRuntimeSetup 
    /// instance that enters ScriptRuntime initialization.
    /// 
    /// If the host is remote with respect to DLR (i.e. also with respect to ScriptHost)
    /// and needs to access objects living in its app-domain it can pass MarshalByRefObject 
    /// as an argument to its ScriptHost subclass constructor.
    /// </summary>
    /// <remarks>
    /// IronSmalltalk doesn't do much with this class. Basically, 
    /// the runtime host can inform us about two things:
    /// - RuntimeAttached   : The runtime is being initialized (this is where we can add assemblies to scopes etc.)
    /// - EngineCreated     : A sepcific DLR language engine has been created
    /// </remarks>
    public class SmalltalkScriptHost : ScriptHost
    {
        protected override void RuntimeAttached()
        {
            base.RuntimeAttached();
        }

        protected override void EngineCreated(ScriptEngine engine)
        {
            base.EngineCreated(engine);
        }
    }
}

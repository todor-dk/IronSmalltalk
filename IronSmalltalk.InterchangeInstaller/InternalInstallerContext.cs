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

namespace IronSmalltalk.InterchangeInstaller
{
    /// <summary>
    /// Special installer context for handling the IronSmalltalk class library.
    /// This is responsible for installing definitions into the extension-scope,
    /// as opposite to normal user code that ends up in the global scope.
    /// </summary>

    public class InternalInstallerContext : InterchangeInstallerContext
    {
        public InternalInstallerContext(SmalltalkRuntime runtime)
            : base(runtime)
        {
        }

        protected override void CreateTemporaryNameSpace()
        {
            if (this.NameScope != null)
                throw new InvalidOperationException("Install phase has commenced.");
            this.NameScope = this.Runtime.ExtensionScope.Copy();
        }

        protected override void ReplaceSmalltalkContextNameSpace()
        {
            if (this.NameScope == null)
                throw new InvalidOperationException("Install phase has not commenced.");
            this.Runtime.SetExtensionScope(this.NameScope);
            this.Runtime.SetGlobalScope(this.Runtime.GlobalScope.Copy(this.NameScope));
        }
    }
}

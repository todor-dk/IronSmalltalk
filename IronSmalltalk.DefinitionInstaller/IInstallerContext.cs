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
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Installer
{
    public interface IInstallerContext
    {
        SmalltalkRuntime Runtime { get; }
        void RegisterNewClass(SmalltalkClass cls, ISourceReference sourceReference);
        void AddClassBinding(ClassBinding binding);
        void AddGlobalConstantBinding(GlobalConstantBinding binding);
        void AddGlobalVariableBinding(GlobalVariableBinding binding);
        void AddPoolBinding(PoolBinding binding);
        IDiscreteGlobalBinding GetLocalGlobalBinding(Symbol name);
        ClassBinding GetLocalClassBinding(Symbol name);
        ClassBinding GetClassBinding(Symbol name);
        ClassBinding GetClassBinding(string name);
        PoolBinding GetLocalPoolBinding(Symbol name);
        PoolBinding GetPoolBinding(Symbol name);
        PoolBinding GetPoolBinding(string name);
        GlobalVariableOrConstantBinding GetGlobalVariableOrConstantBinding(Symbol name);
        GlobalVariableOrConstantBinding GetGlobalVariableOrConstantBinding(string name);
        bool IsProtectedName(Symbol name);
        bool ReportError(ISourceReference sourceReference, string errorMessage);
        bool AnnotateObject(IAnnotetable annotetableObject, IEnumerable<KeyValuePair<string, string>> annotations);
        SmalltalkNameScope NameScope { get; }
    }
}

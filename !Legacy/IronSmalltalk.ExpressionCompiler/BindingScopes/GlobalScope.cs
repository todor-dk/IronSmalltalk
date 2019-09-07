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
using IronSmalltalk.ExpressionCompiler.Bindings;
using RTB = IronSmalltalk.Runtime.Bindings;
using ClassBinding = IronSmalltalk.ExpressionCompiler.Bindings.ClassBinding;
using GlobalConstantBinding = IronSmalltalk.ExpressionCompiler.Bindings.GlobalConstantBinding;
using GlobalVariableBinding = IronSmalltalk.ExpressionCompiler.Bindings.GlobalVariableBinding;
using PoolBinding = IronSmalltalk.ExpressionCompiler.Bindings.PoolBinding;

namespace IronSmalltalk.ExpressionCompiler.BindingScopes
{
    /*
    See full description in BindingScope.cs
      
    *** GLOBAL ***
    global_scope                : Everything global in Smalltalk, e.g. Smalltalk at: ...
    extansion_scope             : Everything global defined by IronSmalltalk
    global_definition_scope     : Everything global defined by the user program
      
    global_scope := extansion_scope + global_definition_scope       // X3J20:3.3.1.1
    */

    /// <summary>
    /// 
    /// </summary>
    public sealed class GlobalScope : ComposableBindingScope
    {
        public RTB.SmalltalkNameScope NameScope { get; private set; }

        public GlobalScope(RTB.SmalltalkNameScope nameScope)
            : base()
        {
            if (nameScope == null)
                throw new ArgumentNullException();
            this.NameScope = nameScope;
        }

        public GlobalScope(RTB.SmalltalkNameScope nameScope, BindingScope outerScope)
            : base(outerScope)
        {
            if (nameScope == null)
                throw new ArgumentNullException();
            this.NameScope = nameScope;
        }

        protected override NameBinding ResolveBinding(string name)
        {
            RTB.IDiscreteBinding binding = this.NameScope.GetGlobalBinding(name);
            if (binding == null)
                return null;
            if (binding is RTB.ClassBinding)
                return new ClassBinding(name, (RTB.ClassBinding)binding);
            if (binding is RTB.GlobalVariableBinding)
                return new GlobalVariableBinding(name, (RTB.GlobalVariableBinding)binding);
            if (binding is RTB.GlobalConstantBinding)
                return new GlobalConstantBinding(name, (RTB.GlobalConstantBinding)binding);
            if (binding is RTB.PoolBinding)
                return new PoolBinding(name, (RTB.PoolBinding)binding);
            throw new NotImplementedException("We don't know how to handle runtime bindings of type: " + binding.GetType().Name);
        }
    }
}

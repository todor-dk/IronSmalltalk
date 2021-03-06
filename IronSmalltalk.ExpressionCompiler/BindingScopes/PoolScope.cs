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
using IronSmalltalk.Runtime;
using RTB = IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler.BindingScopes
{
    /*
    See full description in BindingScope.cs     
   
    *** INITIALIZER ***
    
    pool_scope := global_scope + (pool_var_1 + pool_var_2 + ... + pool_var_n)
    */
    public sealed class PoolScope : ComposableBindingScope
    {
        public Pool Pool { get; private set; }

        public PoolScope(Pool pool, GlobalScope globalScope)
            : base(globalScope)
        {
            if (pool == null)
                throw new ArgumentNullException("pool");
            this.Pool = pool;
        }

        protected override NameBinding ResolveBinding(string name)
        {
            NameBinding result = null;
            RTB.PoolVariableOrConstantBinding binding;
            this.Pool.TryGetValue(name, out binding);
            if (binding != null)
            {
                RTB.PoolBinding poolBinding = this.Pool.Runtime.GlobalScope.GetPoolBinding(this.Pool.Name);
                System.Diagnostics.Debug.Assert(poolBinding != null, String.Format("Could not find pool binding named {0}.", this.Pool.Name.Value));
                if (binding is RTB.PoolConstantBinding)
                    result = new PoolConstantBinding(name, poolBinding, (RTB.PoolConstantBinding)binding);
                else
                    result = new PoolVariableBinding(name, poolBinding, (RTB.PoolVariableBinding)binding);
            }
            return result; // null means try outer scope.
        }
    }
}

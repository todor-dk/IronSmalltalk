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

using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.Runtime;
using RTB = IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler.BindingScopes
{
    /// <summary>
    /// 
    /// </summary>
    /*
    See full description in BindingScope.cs     
      
    *** CLASS ***
    class_variable_names            : All class variables defined in the class
    
    (class_variable_names & instance_variable_names) = Ø           // No duplicates X3J20:3.3.2.1
    (class_variable_names & class_instance_variable_names) = Ø     // No duplicates X3J20:3.3.2.2

    inheritable_class_variable_scope := superclass:inheritable_class_variable_scope 
                                        + class:class_variable_names               // X3J20:3.3.2.2
                                           
    *** BEHAVIOR  ***   
    class_scope := (global_scope + pool_variable_scope) + inheritable_class_variable_scope      // X3J20:3.3.2.3
     */

    public sealed class ClassScope : ClassRelatedBindingScope
    {
        public ClassScope(SmalltalkClass cls, PoolVariableScope outerScope)
            : base(cls, outerScope)
        {
        }

        protected override NameBinding ResolveBinding(string name)
        {
            // inheritable_class_variable_scope
            SmalltalkClass cls = this.Class;
            RTB.ClassVariableBinding binding;
            while (cls != null)
            {
                cls.ClassVariableBindings.TryGetValue(name, out binding);
                if (binding != null)
                    return new ClassVariableBinding(name, cls, binding);
                cls = cls.Superclass;
            }
            return null; // null means try outer scope.
        }
    }
}

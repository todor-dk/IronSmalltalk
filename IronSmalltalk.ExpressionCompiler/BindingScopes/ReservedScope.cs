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

using System.Linq.Expressions;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.ExpressionCompiler.BindingScopes
{
    public static class ReservedScope
    {
        private static BindingScope CreateBindings()
        {
            return new BindingScope(new NameBinding[] {
                new SpecialBinding(SemanticConstants.Nil, client => PreboxedConstants.Nil_Expression, true),
                new SpecialBinding(SemanticConstants.True, client => client.TrueExpression, true),
                new SpecialBinding(SemanticConstants.False, client => client.FalseExpression, true)
            });
        }

        public static BindingScope ForPoolInitializer()
        {
            return ReservedScope.ForGlobalInitializer(); // Same as for globals
        }

        public static BindingScope ForProgramInitializer()
        {
            return ReservedScope.ForGlobalInitializer(); // Same as for globals
        }

        public static BindingScope ForGlobalInitializer()
        {
            BindingScope result = ReservedScope.CreateBindings();
            result.DefineBinding(new ErrorBinding(SemanticConstants.Self));    // error binding unless <<class initializer>>. 
            result.DefineBinding(new ErrorBinding(SemanticConstants.Super));   // Within any type of initializer super has the error binding.
            return result;
        }

        public static BindingScope ForClassInitializer()
        {
            BindingScope result = ReservedScope.CreateBindings();
            result.DefineBinding(new ArgumentBinding(SemanticConstants.Self));
            result.DefineBinding(new ErrorBinding(SemanticConstants.Super));   // Within any type of initializer super has the error binding.
            return result;
        }

        public static BindingScope ForInstanceMethod()
        {
            BindingScope result = ReservedScope.CreateBindings();
            result.DefineBinding(new SpecialBinding(SemanticConstants.Self, client => client.SelfExpression, false));
            result.DefineBinding(new SpecialBinding(SemanticConstants.Super, client => client.SelfExpression, false));
            return result;
        }

        public static BindingScope ForClassMethod()
        {
            BindingScope result = ReservedScope.CreateBindings();
            result.DefineBinding(new SpecialBinding(SemanticConstants.Self, client => client.SelfExpression, false));
            result.DefineBinding(new SpecialBinding(SemanticConstants.Super, client => client.SelfExpression, false));
            return result;
        }

        public static BindingScope ForRootClassInstanceMethod()
        {
            BindingScope result = ReservedScope.CreateBindings();
            result.DefineBinding(new SpecialBinding(SemanticConstants.Self, client => client.SelfExpression, false));
            result.DefineBinding(new ErrorBinding(SemanticConstants.Super));   // Erroneous if instance method and no superclass.
            return result;
        }
    }
}

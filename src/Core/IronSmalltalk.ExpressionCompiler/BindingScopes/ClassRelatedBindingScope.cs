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
using IronSmalltalk.Runtime;

namespace IronSmalltalk.ExpressionCompiler.BindingScopes
{
    public abstract class ClassRelatedBindingScope : ComposableBindingScope
    {
        public SmalltalkClass Class { get; private set; }

        public ClassRelatedBindingScope(SmalltalkClass cls, BindingScope outerScope)
            : base(outerScope)
        {
            if (cls == null)
                throw new ArgumentNullException(nameof(cls));
            this.Class = cls;
        }
    }
}

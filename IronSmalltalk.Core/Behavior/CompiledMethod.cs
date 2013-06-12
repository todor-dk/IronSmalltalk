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
using System.Dynamic;
using System.Linq;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Behavior
{
    public abstract class CompiledMethod : CompiledCode
    {
        public Symbol Selector { get; private set; }

        protected CompiledMethod(Symbol selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            this.Selector = selector;
        }

        /// <summary>
        /// Return the name of the method when this method is called from the outside world, i.e. dynamically from .Net code.
        /// </summary>
        public string NativeName
        {
            get
            {
                string name = null;
                if (this._annotations == null)
                    return null;
                this._annotations.TryGetValue("ist.runtime.native-name", out name);
                if (String.IsNullOrWhiteSpace(name))
                    return null;
                return name;
            }
        }

        /// <summary>
        /// Return the number of arguments that this method expects.
        /// </summary>
        public int NumberOfArguments
        {
            get
            {
                int args = 0;
                bool binary = this.Selector.Value.Length != 0;
                foreach(char c in this.Selector.Value)
                {
                    if (binary)
                    {
                        if (!@"!%&*+,/<=>?@\~|-".Contains(c))
                        binary = false;
                    } else 
                    {
                        if (c == ':')
                            args++;
                    }
                }
                if (binary)
                    return 1;
                else
                    return args;
            }
        }


        public abstract MethodCompilationResult CompileInstanceMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope);
        public abstract MethodCompilationResult CompileClassMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope);
        public abstract bool ValidateInstanceMethod(SmalltalkClass cls, SmalltalkNameScope globalNameScope, IIntermediateCodeValidationErrorSink errorSink);
        public abstract bool ValidateClassMethod(SmalltalkClass cls, SmalltalkNameScope globalNameScope, IIntermediateCodeValidationErrorSink errorSink);
    }

    public sealed class NativeCompiledMethod : CompiledMethod
    {
        public NativeCompiledMethod(Symbol selector)
            : base(selector)
        {
        }

        public override MethodCompilationResult CompileClassMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope)
        {
            throw new NotImplementedException();
        }

        public override MethodCompilationResult CompileInstanceMethod(SmalltalkRuntime runtime, SmalltalkClass cls, DynamicMetaObject self, DynamicMetaObject[] arguments, Symbol superScope)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateInstanceMethod(SmalltalkClass cls, SmalltalkNameScope globalNameScope, IIntermediateCodeValidationErrorSink errorSink)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateClassMethod(SmalltalkClass cls, SmalltalkNameScope globalNameScope, IIntermediateCodeValidationErrorSink errorSink)
        {
            throw new NotImplementedException();
        }
    }
}

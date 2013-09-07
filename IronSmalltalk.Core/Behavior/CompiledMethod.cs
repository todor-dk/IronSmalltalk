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
using System.Linq.Expressions;
using IronSmalltalk.Common;

namespace IronSmalltalk.Runtime.Behavior
{
    /// <summary>
    /// Instances of the CompiledMethod class represent the code behind a Smalltalk method.
    /// This code is represented by an AST Expression and can be executed.
    /// </summary>
    public abstract class CompiledMethod : CompiledCode
    {
        /// <summary>
        /// The selector of the method.
        /// </summary>
        public Symbol Selector { get; private set; }

        /// <summary>
        /// The class that defines the method.
        /// </summary>
        public SmalltalkClass Class { get; private set; }

        /// <summary>
        /// The type of method - either a class or an instance method.
        /// </summary>
        public MethodType Type { get; private set; }

        /// <summary>
        /// Create a new CompiledMethod.
        /// </summary>
        /// <param name="cls">The class that defines the method.</param>
        /// <param name="selector">The selector of the method.</param>
        /// <param name="methodType">The type of method - either a class or an instance method.</param>
        protected CompiledMethod(SmalltalkClass cls, Symbol selector, MethodType methodType)
        {
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (selector == null)
                throw new ArgumentNullException("selector");

            this.Class = cls;
            this.Selector = selector;
            this.Type = methodType;
        }

        /// <summary>
        /// Return the name of the method when this method is called from the outside world, i.e. dynamically from .Net code.
        /// </summary>
        public string NativeName
        {
            get
            {
                string name = null;
                if (this._Annotations == null)
                    return null;
                this._Annotations.TryGetValue("ist.runtime.native-name", out name);
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
                        if (!GlobalConstants.BinaryCharacters.Contains(c))
                        binary = false;
                    } else 
                    {
                        if (c == GlobalConstants.KeywordPostfix)
                            args++;
                    }
                }
                if (binary)
                    return 1;
                else
                    return args;
            }
        }

        /// <summary>
        /// The type of method - either a class or an instance method.
        /// </summary>
        public enum MethodType
        {
            /// <summary>
            /// The method is part of the instance behavior of a Smalltalk class.
            /// </summary>
            Instance,
            /// <summary>
            /// The method is part of the class behavior of a Smalltalk class.
            /// </summary>
            Class
        }

        /// <summary>
        /// Get (generate) the AST Expression that is needed to execute this method.
        /// </summary>
        /// <param name="self">The expression representing the receiver.</param>
        /// <param name="executionContext">The expression that represents the Smalltalk ExecutionContext.</param>
        /// <param name="arguments">Arguments that are passed to the method.</param>
        /// <returns>An AST Expression that can execute the logic of this method.</returns>
        public abstract Expression GetExpression(Expression self, Expression executionContext, IEnumerable<Expression> arguments);
    }
}

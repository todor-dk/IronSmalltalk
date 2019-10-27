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

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    /// <summary>
    /// Call-Site-Binder for dynamic message super sends. 
    /// This binder is responsible for binding the operations for message super sends.
    /// </summary>
    /// <remarks>
    /// Super-sends apply only to Smalltalk objects and is exclusive part of the Smalltalk 
    /// semantics. For that reason it's ONLY the Smalltalk runtime that can bind a super
    /// send and the object (receiver) IS NEVER asked to bind that operation.
    /// </remarks>
    public partial class SuperSendCallSiteBinder : MessageSendCallSiteBinderBase
    {
        /// <summary>
        /// For super sends, return the name of the class ABOVE which to start the method lookup.
        /// </summary>
        public string SuperScope { get; protected set; }

        /// <summary>
        /// Create a new SuperSendCallSiteBinder.
        /// </summary>
        /// <param name="selector">Selector of the message being sent.</param>
        /// <param name="superScope">The name of the class ABOVE which to start the method lookup.</param>
        public SuperSendCallSiteBinder(string selector, string superScope)
            : base(selector)
        {
            if (String.IsNullOrWhiteSpace(superScope))
                throw new ArgumentNullException(nameof(superScope));
            this.SuperScope = superScope;
        }

        /// <summary>
        /// For super sends, return the name of the class ABOVE which to start the method lookup.
        /// </summary>
        /// <returns>Return a class name or null to start the method lookup immediately.</returns>
        /// <example>
        /// Let's say a method in class Integer sends "super test".
        /// If this method returns #Integer, the method look-up will skip 
        /// the Integer class (and all subclasses) and start the look-up 
        /// process from the superclass of the Integer class.
        /// </example>
        protected override string GetSuperLookupScope()
        {
            return this.SuperScope;
        }
    }
}

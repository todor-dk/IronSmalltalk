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


namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    /// <summary>
    /// Call-Site-Binder for nomal (non-super) dynamic message sends to a constant receiver.
    /// This call site binder may implement more optimization logic in the future knowing that the receiver
    /// will always be constant, i.e. it will never change.
    /// </summary>
    public class ConstantSendCallSiteBinder : MessageSendCallSiteBinder
    {
        /// <summary>
        /// Create a new ConstantSendCallSiteBinder.
        /// </summary>
        /// <param name="selector">Selector of the message being sent.</param>
        /// <param name="nativeName">Name of the method that the target is asked to bind.</param>
        /// <param name="argumentCount">Number of method arguments.</param>
        public ConstantSendCallSiteBinder(string selector, string nativeName, int argumentCount)
            : base(selector, nativeName, argumentCount)
        {
        }
    }
}

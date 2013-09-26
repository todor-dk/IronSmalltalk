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

namespace IronSmalltalk.Common.Internal
{
    /// <summary>
    /// The AccessedViaReflectionAttribute indicates that 
    /// the type or the member decorated by the attribute
    /// is referenced by name using reflection code. 
    /// 
    /// The development tools are usually unable to detect
    /// those references, e.g. via the "Find All References".
    /// Care must be taken when renaming or removing that
    /// type or member.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Constructor | 
        AttributeTargets.Delegate | 
        AttributeTargets.Enum | 
        AttributeTargets.Event | 
        AttributeTargets.Field | 
        AttributeTargets.Interface | 
        AttributeTargets.Method | 
        AttributeTargets.Property | 
        AttributeTargets.Struct)]
    public class AccessedViaReflectionAttribute : Attribute
    {
    }
}

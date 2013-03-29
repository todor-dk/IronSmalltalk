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

using System.Dynamic;
using System.Linq.Expressions;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using IronSmalltalk.Runtime.Execution.Dynamic;

namespace IronSmalltalk.Runtime
{
    partial class SmalltalkClass : ISmalltalkDynamicMetaObjectProvider
    {
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            // Create the restrictions, which in pseudo-C# is defines as:
            //  SmalltalkClass cls = this.Class;    // Constant value  
            //  (self == this) // Reference equals
            BindingRestrictions restrictions = BindingRestrictions.GetInstanceRestriction(parameter, this);

            return new SmalltalkDynamicMetaObject(parameter, restrictions, this);
        }

        DynamicMetaObject ISmalltalkDynamicMetaObjectProvider.PerformOperation(SmalltalkDynamicMetaObject target, string name, bool ignoreCase, int argumentCount, DynamicMetaObject[] args, out bool caseConflict)
        {
            SmalltalkClass cls = this;
            Symbol na = null;
            bool localCaseConflict = false;
            CompiledMethod method = MethodLookupHelper.LookupMethod(ref cls, ref na, delegate(SmalltalkClass c)
            {
                return c.ClassBehavior.GetMethodByNativeName(name, argumentCount, ignoreCase, out localCaseConflict);
            });

            caseConflict = localCaseConflict;
            if (localCaseConflict)
                return null;

            if (method != null)
            {
                var compilationResult = method.Code.CompileClassMethod(cls.Runtime, cls, target, args, null);
                return compilationResult.GetDynamicMetaObject(target.Restrictions);
            }

            // If no method found on the class side, we must do a lookup on the instance side.
            cls = this.Runtime.NativeTypeClassMap.Class;
            if (cls == null)
                cls = this.Runtime.NativeTypeClassMap.Object;
            method = MethodLookupHelper.LookupMethod(ref cls, ref na, delegate(SmalltalkClass c)
            {
                return c.InstanceBehavior.GetMethodByNativeName(name, argumentCount, ignoreCase, out localCaseConflict);
            });

            caseConflict = localCaseConflict;
            if (localCaseConflict)
                return null;

            if (method != null)
            {
                var compilationResult = method.Code.CompileInstanceMethod(cls.Runtime, cls, target, args, null);
                return compilationResult.GetDynamicMetaObject(target.Restrictions);
            }

            // No luck
            return null;
        }
    }
}

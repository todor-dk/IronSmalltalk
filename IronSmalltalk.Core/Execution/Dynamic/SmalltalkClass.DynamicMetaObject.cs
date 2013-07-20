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
using System.Dynamic;
using System.Linq;
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
            throw new NotImplementedException();
            //SmalltalkClass cls = this;
            //Symbol na = null;
            //bool localCaseConflict = false;
            //CompiledMethod method = MethodLookupHelper.LookupMethod(ref cls, ref na, 
            //    c => c.ClassBehavior.GetMethodByNativeName(name, argumentCount, ignoreCase, out localCaseConflict));

            //caseConflict = localCaseConflict;
            //if (localCaseConflict)
            //    return null;

            //if (method != null)
            //{
            //    var compilationResult = method.CompileClassMethod(cls.Runtime, cls, target.Expression, args.Select(dmo => dmo.Expression).ToArray(), null);
            //    return compilationResult.GetDynamicMetaObject(target.Restrictions);
            //}

            //// If no method found on the class side, we must do a lookup on the instance side.
            //cls = this.Runtime.NativeTypeClassMap.Class;
            //if (cls == null)
            //    cls = this.Runtime.NativeTypeClassMap.Object;
            //method = MethodLookupHelper.LookupMethod(ref cls, ref na, 
            //    c => c.InstanceBehavior.GetMethodByNativeName(name, argumentCount, ignoreCase, out localCaseConflict));

            //caseConflict = localCaseConflict;
            //if (localCaseConflict)
            //    return null;

            //if (method != null)
            //{
            //    var compilationResult = method.CompileInstanceMethod(cls.Runtime, cls, target.Expression, args.Select(dmo => dmo.Expression).ToArray(), null);
            //    return compilationResult.GetDynamicMetaObject(target.Restrictions);
            //}

            //// No luck
            //return null;
        }
    }
}

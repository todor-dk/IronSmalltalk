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
using System.Reflection;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using IronSmalltalk.Runtime.Execution.Dynamic;

namespace IronSmalltalk.Runtime
{
    partial class SmalltalkObject : ISmalltalkDynamicMetaObjectProvider
    {
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            // Create the restrictions, which in pseudo-C# is defines as:
            //  SmalltalkClass cls = this.Class;    // Constant value  
            //  (self is SmalltalkObject) && (((SmalltalkObject) self).Class == cls);
            FieldInfo field = typeof(SmalltalkObject).GetField("Class", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
            if (field == null)
                throw new InvalidOperationException();
            BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(parameter, typeof(SmalltalkObject));
            restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
                Expression.ReferenceEqual(Expression.Field(Expression.Convert(parameter, typeof(SmalltalkObject)), field), Expression.Constant(this.Class))));

            return new SmalltalkDynamicMetaObject(parameter, restrictions, this);
        }

        DynamicMetaObject ISmalltalkDynamicMetaObjectProvider.PerformOperation(SmalltalkDynamicMetaObject target, string name, bool ignoreCase, int argumentCount, DynamicMetaObject[] args, out bool caseConflict)
        {
            throw new NotImplementedException();
            //bool localCaseConflict = false;
            //SmalltalkClass cls = this.Class;
            //Symbol na = null;
            //CompiledMethod method = MethodLookupHelper.LookupMethod(ref cls, ref na, 
            //    c => c.InstanceBehavior.GetMethodByNativeName(name, argumentCount, ignoreCase, out localCaseConflict));

            //caseConflict = localCaseConflict;
            //if (localCaseConflict)
            //    return null;

            //if (method != null)
            //{
            //    MethodCompilationResult compilationResult = method.CompileInstanceMethod(cls.Runtime, cls, target.Expression, args.Select(dmo => dmo.Expression).ToArray(), null);
            //    return compilationResult.GetDynamicMetaObject(target.Restrictions);
            //}
            //return null;
        }
    }
}

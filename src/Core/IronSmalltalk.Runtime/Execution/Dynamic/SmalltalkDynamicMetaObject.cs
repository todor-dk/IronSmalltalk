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
using System.Linq.Expressions;
using System.Reflection;
using IronSmalltalk.Common.Internal;

namespace IronSmalltalk.Runtime.Execution.Dynamic
{
    public interface ISmalltalkDynamicMetaObjectProvider : IDynamicMetaObjectProvider
    {
        DynamicMetaObject PerformOperation(SmalltalkDynamicMetaObject target, string name, bool ignoreCase, int argumentCount, DynamicMetaObject[] args, out bool caseConflict);
    }

    public class SmalltalkDynamicMetaObject : DynamicMetaObject
    {
        private readonly ISmalltalkDynamicMetaObjectProvider Self;

        public SmalltalkDynamicMetaObject(Expression expression, BindingRestrictions restrictions, ISmalltalkDynamicMetaObjectProvider self)
            : base(expression, restrictions, self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            this.Self = self;
        }

        private static readonly ConstructorInfo InvalidOperationExceptionCtor = TypeUtilities.Constructor(typeof(InvalidOperationException), typeof(string));

        internal static Expression CreateCaseConflictException(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return Expression.Throw(
                Expression.New(SmalltalkDynamicMetaObject.InvalidOperationExceptionCtor, Expression.Constant(message, typeof(string))));
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            DynamicMetaObject result = this.PerformOperation(binder.Name, binder.IgnoreCase, binder.CallInfo.ArgumentCount, args);
            if (result != null)
                return result;
            return base.BindInvokeMember(binder, args);
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            DynamicMetaObject result = this.PerformOperation(binder.Name, binder.IgnoreCase, 0, null);
            if (result != null)
                return result;
            return base.BindGetMember(binder);
        }


        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            DynamicMetaObject result = this.PerformOperation(binder.Name, binder.IgnoreCase, 1, new DynamicMetaObject[] { value });
            if (result != null)
                return result;

            return base.BindSetMember(binder, value);
        }

        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            return base.BindGetIndex(binder, indexes);
        }

        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            return base.BindSetIndex(binder, indexes, value);
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        {
            return base.BindInvoke(binder, args);
        }

        public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
        {
            return base.BindUnaryOperation(binder);
        }

        public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
        {
            return base.BindBinaryOperation(binder, arg);
        }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            return base.BindConvert(binder);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            HashSet<string> names = new HashSet<string>();
            //foreach (var behavior in this.Behaviors)
            //{
            //    foreach (string name in behavior.GetNativeMethodNames())
            //        names.Add(name);
            //}
            return names;
        }


        private DynamicMetaObject PerformOperation(string name, bool ignoreCase, int argumentCount, DynamicMetaObject[] args)
        {
            if (args == null)
                args = Array.Empty<DynamicMetaObject>();
            bool caseConflict;
            DynamicMetaObject result = this.Self.PerformOperation(this, name, ignoreCase, argumentCount, args, out caseConflict);
            if (!caseConflict)
                return result;

            // The case-conflict exception
            return new DynamicMetaObject(SmalltalkDynamicMetaObject.CreateCaseConflictException(
                $"Several methods exist with the name '{name}' and only differ in case."), this.Restrictions);
        }
    }
}

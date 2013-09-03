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
using IronSmalltalk.Common.Internal;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using IronSmalltalk.Runtime.Execution.Dynamic;

namespace IronSmalltalk
{
    partial class SmalltalkRuntime : IDynamicMetaObjectProvider
    {
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            // Create the restrictions, which in pseudo-C# is defines as:
            //  (self == this);      // reference equals
            BindingRestrictions restrictions = BindingRestrictions.GetInstanceRestriction(parameter, this);

            return new SmalltalkRuntimeDynamicMetaObject(parameter, restrictions, this);
        }
    }

    public class SmalltalkRuntimeDynamicMetaObject : DynamicMetaObject
    {
        public SmalltalkRuntime Runtime { get; private set; }

        public SmalltalkRuntimeDynamicMetaObject(Expression expression, BindingRestrictions restrictions, SmalltalkRuntime runtime)
            : base(expression, restrictions, runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            this.Runtime = runtime;
        }

        static SmalltalkRuntimeDynamicMetaObject()
        {
            SmalltalkRuntimeDynamicMetaObject.GetValueProperty = typeof(IBinding).GetProperty("Value");
            if (SmalltalkRuntimeDynamicMetaObject.GetValueProperty == null)
                throw new InvalidOperationException("The property Value does not exists in IBinding");
            SmalltalkRuntimeDynamicMetaObject.SetValueProperty = typeof(IWritableBinding).GetProperty("Value");
            if (SmalltalkRuntimeDynamicMetaObject.SetValueProperty == null)
                throw new InvalidOperationException("The property Value does not exists in IWritableBinding");
        }

        private static readonly PropertyInfo GetValueProperty = typeof(IBinding).GetProperty("Value");
        private static readonly PropertyInfo SetValueProperty = typeof(IWritableBinding).GetProperty("Value");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
        /// <remarks>
        /// The code is highly optimized and it inlines direct access to the discrete binding instead of accessing it every time.
        /// 
        /// If we create binding versioning, we must add restrctions as well to take the version into account.
        /// </remarks>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            return this.GetSetMemberWorker(binder.Name, binder.IgnoreCase, binder.ReturnType, false,
                value => value,
                errorSuggestion => binder.FallbackGetMember(this, errorSuggestion));
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            // TO-DO
            return this.GetSetMemberWorker(binder.Name, binder.IgnoreCase, binder.ReturnType, true,
                val => Expression.Assign(val, value.Expression),
                errorSuggestion => binder.FallbackSetMember(this, errorSuggestion));
        }

        private static readonly ConstructorInfo InvalidOperationExceptionCtor = TypeUtilities.Constructor(typeof(InvalidOperationException), typeof(string));

        private DynamicMetaObject GetSetMemberWorker(string name, bool ignoreCase, Type returnType, bool isSetValue, Func<MemberExpression, Expression> action, Func<DynamicMetaObject, DynamicMetaObject> fallback)
        {
            bool caseConflict;
            IBinding binding = SmalltalkRuntimeDynamicMetaObject.GetMemberBinding(this.Runtime, name, ignoreCase, out caseConflict);

            if (caseConflict)
            {
                // The case-conflict exception
                return new DynamicMetaObject(SmalltalkDynamicMetaObject.CreateCaseConflictException(
                    String.Format("Several methods exist with the name '{0}' and only differ in case.", name)), this.Restrictions);
            }

            string msg = null;
            if (binding == null)
                msg = String.Format("The Smalltalk Runtime does not contain a class or global named '{0}'", name);
            else if (isSetValue && !(binding is IWritableBinding))
                msg = String.Format("The Smalltalk Runtime does not contain a global variable named '{0}'", name);

            if (msg != null)
            {
                Expression errorSuggestion = Expression.Throw(
                    Expression.New(SmalltalkRuntimeDynamicMetaObject.InvalidOperationExceptionCtor, Expression.Constant(msg, typeof(String))), returnType);

                return fallback(new DynamicMetaObject(errorSuggestion, this.Restrictions));
            }

            MemberExpression value = Expression.Property(Expression.Constant(binding),
                (isSetValue ? SmalltalkRuntimeDynamicMetaObject.SetValueProperty : SmalltalkRuntimeDynamicMetaObject.GetValueProperty));
            return new DynamicMetaObject(Expression.Convert(action(value), returnType), this.Restrictions);
        }

        private static IBinding GetMemberBinding(SmalltalkRuntime runtime, string name, bool ignoreCase, out bool caseConflict)
        {
            if (ignoreCase)
            {
                IBinding result = null;
                caseConflict = true;
                foreach (IDiscreteGlobalBinding binding in runtime.Globals.Values)
                {
                    if (binding.Name.Value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (result == null)
                            result = binding;
                        else
                            return null; // caseConflict
                    }
                }
                caseConflict = false;
                return result;
            }
            else
            {
                IDiscreteGlobalBinding binding;
                runtime.Globals.TryGetValue(name, out binding);
                caseConflict = false;
                return binding;
            }
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            DynamicMetaObject result = this.PerformOperation(binder.Name, binder.IgnoreCase, binder.CallInfo.ArgumentCount, args);
            if (result != null)
                return result;
            return base.BindInvokeMember(binder, args);
        }

        private DynamicMetaObject PerformOperation(string name, bool ignoreCase, int argumentCount, DynamicMetaObject[] args)
        {
            throw new NotImplementedException();
            //if (args == null)
            //    args = new DynamicMetaObject[0];
            //bool caseConflict = false;

            //SmalltalkClass cls = this.Runtime.NativeTypeClassMap.GetSmalltalkClass(this.Runtime.GetType());
            //if (cls == null)
            //    cls = this.Runtime.NativeTypeClassMap.Object;
            //Symbol na = null;
            //CompiledMethod method = MethodLookupHelper.LookupMethod(ref cls, ref na, 
            //    c => c.InstanceBehavior.GetMethodByNativeName(name, argumentCount, ignoreCase, out caseConflict));

            //if (!caseConflict)
            //{
            //    if (method != null)
            //    {
            //        var compilationResult = method.CompileInstanceMethod(cls.Runtime, cls, this.Expression, args.Select(dmo => dmo.Expression).ToArray(), null);
            //        return compilationResult.GetDynamicMetaObject(this.Restrictions);
            //    }
            //    return null;
            //}

            //// The case-conflict exception
            //return new DynamicMetaObject(SmalltalkDynamicMetaObject.CreateCaseConflictException(
            //    String.Format("Several methods exist with the name '{0}' and only differ in case.", name)), this.Restrictions);
        }
    }
}
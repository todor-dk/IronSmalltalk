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
using System.Linq.Expressions;
using System.Reflection;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;
using GlobalVariableBinding = IronSmalltalk.ExpressionCompiler.Bindings.GlobalVariableBinding;
using RTB = IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler.Bindings
{
    public sealed class ClassVariableBinding : DiscreteBinding<RTB.ClassVariableBinding>, IAssignableBinding
    {
        public SmalltalkClass Class { get; private set; }

        public ClassVariableBinding(string name, SmalltalkClass cls, RTB.ClassVariableBinding binding)
            : base(name, binding)
        {
            if (cls == null)
                throw new ArgumentNullException(nameof(cls));
            this.Class = cls;
        }

        public Expression GenerateAssignExpression(Expression value, IBindingClient client)
        {
            return Expression.Assign(
                Expression.Property(
                    client.DiscreteBindingEncodingStrategy.GetBindingExpression<ClassVariableBinding, RTB.ClassVariableBinding>(client, this),
                    GlobalVariableBinding.SetPropertyInfo),
                value);
        }

        public override string Moniker
        {
            get { return DiscreteBindingCallSiteBinderBase.GetMoniker(this.Class, this.Binding); }
        }
    }

    public abstract class ArrayBasedVariableBinding<TReceiver> : NameBinding, IAssignableBinding
    {
        public int VariableIndex { get; private set; }

        public ArrayBasedVariableBinding(string name, int index)
            : base(name)
        {
            this.VariableIndex = index;
        }

        protected abstract Expression InstanceVariablesAccess(IBindingClient client);

        public override Expression GenerateReadExpression(IBindingClient client)
        {
            Expression instVars = this.InstanceVariablesAccess(client);
            return Expression.ArrayAccess(instVars, Expression.Constant(this.VariableIndex));
        }

        public Expression GenerateAssignExpression(Expression value, IBindingClient client)
        {
            Expression instVars = this.InstanceVariablesAccess(client);
            return Expression.Assign(
                Expression.ArrayAccess(instVars, Expression.Constant(this.VariableIndex)),
                value);
        }
    }


    public sealed class InstanceVariableBinding : ArrayBasedVariableBinding<SmalltalkObject>
    {
        public InstanceVariableBinding(string name, int index)
            : base(name, index)
        {
        }

        protected override Expression InstanceVariablesAccess(IBindingClient client)
        {
            // return ((SmalltalkObject) self).InstanceVariables;
            Expression self = client.SelfExpression;
            return Expression.Field(
                    Expression.Convert(self, typeof(SmalltalkObject)),
                    SmalltalkObject.InstanceVariablesField);
        }
    }

    public sealed class ClassInstanceVariableBinding : ArrayBasedVariableBinding<SmalltalkClass>
    {
        public ClassInstanceVariableBinding(string name, int index)
            : base(name, index)
        {
        }

        protected override Expression InstanceVariablesAccess(IBindingClient client)
        {
            // return ((SmalltalkClass) self).ClassInstanceVariables;
            Expression self = client.SelfExpression;
            return Expression.Property(
                    Expression.Convert(self, typeof(SmalltalkClass)),
                    SmalltalkClass.ClassInstanceVariablesProperty);
        }
    }
}

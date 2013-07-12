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
using IronSmalltalk.Runtime;
using RTB = IronSmalltalk.Runtime.Bindings;
using GlobalVariableBinding = IronSmalltalk.ExpressionCompiler.Bindings.GlobalVariableBinding;


namespace IronSmalltalk.ExpressionCompiler.Bindings
{
    public sealed class ClassVariableBinding : DiscreteBinding<RTB.ClassVariableBinding>, IAssignableBinding
    {
        public ClassVariableBinding(string name, RTB.ClassVariableBinding binding)
            : base(name, binding)
        {
        }

        public Expression GenerateAssignExpression(Expression value, IBindingClient client)
        {
            return Expression.Assign(
                Expression.Property(
                    Expression.Constant(this.Binding, typeof(GlobalVariableBinding)),
                    GlobalVariableBinding.SetPropertyInfo),
                value);
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
        private static readonly FieldInfo InstanceVariablesField;

        static InstanceVariableBinding()
        {
            InstanceVariableBinding.InstanceVariablesField = InstanceVariableBinding.GetField("InstanceVariables");
        }

        private static FieldInfo GetField(string name)
        {
            FieldInfo field = typeof(SmalltalkObject).GetField(name, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
            if (field == null)
                throw new InvalidOperationException(String.Format("Could not find the SmalltalkObject.{0} field!", name));
            return field;
        }

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
                    InstanceVariableBinding.InstanceVariablesField);
        }
    }

    public sealed class ClassInstanceVariableBinding : ArrayBasedVariableBinding<SmalltalkClass>
    {
        private static readonly PropertyInfo InstanceVariablesProperty;

        static ClassInstanceVariableBinding()
        {
            ClassInstanceVariableBinding.InstanceVariablesProperty = ClassInstanceVariableBinding.GetProperty("ClassInstanceVariables");
        }

        private static PropertyInfo GetProperty(string name)
        {
            PropertyInfo prop = typeof(SmalltalkClass).GetProperty(name, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (prop == null)
                throw new InvalidOperationException(String.Format("Could not find the SmalltalkClass.{0} field!", name));
            return prop;
        }

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
                    ClassInstanceVariableBinding.InstanceVariablesProperty);
        }
    }
}
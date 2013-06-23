﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.CodeGeneration.BindingScopes;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.Runtime.Behavior
{
    public abstract class CompiledInitializer : CompiledCode
    {
        /// <summary>
        /// Type of the initializer.
        /// </summary>
        public InitializerType Type { get; private set; }

        /// <summary>
        /// Optional binding to the object that this initializer sets.
        /// This is set for GlobalInitializer, ClassInitializer and PoolVariableInitializer.
        /// It is null for ProgramInitializer.
        /// </summary>
        public IDiscreteBinding Binding { get; private set; }

        protected CompiledInitializer(InitializerType type, IDiscreteBinding binding)
        {
            if (type == InitializerType.ProgramInitializer)
            {
                if (binding != null)
                    throw new ArgumentException("ProgramInitializers must have null binding.");
            }
            else if (type == InitializerType.ClassInitializer)
            {
                if (binding == null)
                    throw new ArgumentNullException("binding");
                if (!(binding is ClassBinding))
                    throw new ArgumentException("ClassInitializers must have binding of type ClassBinding.");
            }
            else if (type == InitializerType.GlobalInitializer)
            {
                if (binding == null)
                    throw new ArgumentNullException("binding");
                if (!(binding is GlobalVariableOrConstantBinding))
                    throw new ArgumentException("ClassInitializers must have binding of type GlobalVariableOrConstantBinding.");
            }
            else if (type == InitializerType.PoolVariableInitializer)
            {
                if (binding == null)
                    throw new ArgumentNullException("binding");
                if (!(binding is PoolVariableOrConstantBinding))
                    throw new ArgumentException("ClassInitializers must have binding of type PoolVariableOrConstantBinding.");
            }
            else 
            {
                throw new ArgumentOutOfRangeException("type");
            }

            this.Type = type;
            this.Binding = binding;
        }

        public abstract object Execute(object self, ExecutionContext executionContext);

        public void ExecuteInitializer(ExecutionContext executionContext)
        {
            object value;
            switch (this.Type)
            {
                case InitializerType.ProgramInitializer:
                    this.Execute(null, executionContext);
                    break;
                case InitializerType.GlobalInitializer:
                    value = this.Execute(null, executionContext);
                    ((GlobalVariableOrConstantBinding)this.Binding).SetValue(value);
                    break;
                case InitializerType.ClassInitializer:
                    this.Execute(this.Binding.Value, executionContext);
                    break;
                case InitializerType.PoolVariableInitializer:
                    value = this.Execute(null, executionContext);
                    ((PoolVariableOrConstantBinding)this.Binding).SetValue(value);
                    break;
                default:
                    break;
            }
        }
    }

    public enum InitializerType
    {
        ProgramInitializer,
        GlobalInitializer,
        ClassInitializer,
        PoolVariableInitializer
    }
}

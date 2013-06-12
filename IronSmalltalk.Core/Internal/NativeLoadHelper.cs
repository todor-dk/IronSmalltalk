using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.Runtime.Internal
{
    public static class NativeLoadHelper
    {
        public static void AddProtectedName(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (name == null)
                throw new ArgumentNullException("name");
            Symbol symbol = runtime.GetSymbol(name);
            if (!scope.ProtectedNames.Contains(symbol))
                scope.ProtectedNames.Add(symbol);
        }

        public static ClassBinding AddClassBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (name == null)
                throw new ArgumentNullException("name");
            Symbol symbol = runtime.GetSymbol(name);
            ClassBinding binding = new ClassBinding(symbol);
            scope.Classes.Add(binding);
            return binding;
        }

        public static PoolBinding AddPoolBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (name == null)
                throw new ArgumentNullException("name");
            Symbol symbol = runtime.GetSymbol(name);

            PoolBinding binding = new PoolBinding(symbol);
            scope.Pools.Add(binding);
            return binding;
        }

        public static GlobalVariableBinding AddGlobalVariableBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (name == null)
                throw new ArgumentNullException("name");
            Symbol symbol = runtime.GetSymbol(name);

            GlobalVariableBinding binding = new GlobalVariableBinding(symbol);
            scope.GlobalVariables.Add(binding);
            return binding;
        }

        public static GlobalConstantBinding AddGlobalConstantBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (name == null)
                throw new ArgumentNullException("name");
            Symbol symbol = runtime.GetSymbol(name);

            GlobalConstantBinding binding = new GlobalConstantBinding(symbol);
            scope.GlobalConstants.Add(binding);
            return binding;
        }

        public static void CreateClass(SmalltalkRuntime runtime, SmalltalkNameScope scope, ClassBinding binding, string superclassName, 
            SmalltalkClass.InstanceStateEnum instanceState, string[] classVarNames, string[] instVarNames, string[] classInstVarNames, string[] importedPools,
            Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>> classMethodDicInitializer, Func<SmalltalkRuntime, Dictionary<Symbol, CompiledMethod>> instanceMethodDicInitializer )
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (binding == null)
                throw new ArgumentNullException("binding");
            if (classMethodDicInitializer == null)
                throw new ArgumentNullException("classMethodDicInitializer");
            if (instanceMethodDicInitializer == null)
                throw new ArgumentNullException("instanceMethodDicInitializer");
            // 3. Prepare stuff ....
            ClassBinding superclass;
            if (String.IsNullOrWhiteSpace(superclassName))
            {
                superclass = null; // Object has no superclass
            }
            else
            {
                superclass = scope.GetClassBinding(superclassName);
                if (superclass == null)
                    throw new InvalidOperationException("Should have found a binding for the superclass");
            }

            // Create the collection of class, class-instance, instance variables and imported pools
            BindingDictionary<InstanceVariableBinding> instVars = new BindingDictionary<InstanceVariableBinding>(runtime);
            DiscreteBindingDictionary<ClassVariableBinding> classVars = new DiscreteBindingDictionary<ClassVariableBinding>(runtime, ((classVarNames == null) ? 0 : classVarNames.Length));
            BindingDictionary<ClassInstanceVariableBinding> classInstVars = new BindingDictionary<ClassInstanceVariableBinding>(runtime);
            DiscreteBindingDictionary<PoolBinding> pools = new DiscreteBindingDictionary<PoolBinding>(runtime);
            // Add class variable names ...
            if (classVarNames != null)
            {
                foreach (string identifier in classVarNames)
                {
                    Symbol varName = runtime.GetSymbol(identifier);
                    classVars.Add(new ClassVariableBinding(varName));
                }
            }
            // Add instance variable names ...
            if (instVarNames != null)
            {
                foreach (string identifier in instVarNames)
                {
                    Symbol varName = runtime.GetSymbol(identifier);
                    instVars.Add(new InstanceVariableBinding(varName));
                }
            }
            // Add class instance variable names ...
            if (classInstVarNames != null)
            {
                foreach (string identifier in classInstVarNames)
                {
                    Symbol varName = runtime.GetSymbol(identifier);
                    classInstVars.Add(new ClassInstanceVariableBinding(varName));
                }
            }
            // Add imported pool names ...
            if (importedPools != null)
            {
                foreach (string identifier in importedPools)
                {
                    Symbol varName = runtime.GetSymbol(identifier);
                    PoolBinding pool = scope.GetPoolBinding(varName);
                    if (pool == null)
                        throw new InvalidOperationException(String.Format("Should have found a binding for pool {0}", identifier));
                    pools.Add(pool);
                }
            }
            // Create method dics
            ClassMethodDictionary classMethods = new ClassMethodDictionary(runtime, classMethodDicInitializer);
            InstanceMethodDictionary instanceMethods = new InstanceMethodDictionary(runtime, instanceMethodDicInitializer);

            // 4. Finally, create the behavior object
            binding.SetValue(new SmalltalkClass(runtime, binding.Name, superclass, instanceState, instVars, classVars, classInstVars, pools, instanceMethods, classMethods));
        }

        public static void CreatePool(SmalltalkRuntime runtime, PoolBinding binding)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (binding == null)
                throw new ArgumentNullException("binding");

            binding.SetValue(new Pool(runtime, binding.Name));
        }

        public static void AnnotateObject(IDiscreteBinding binding, string key, string value)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");
            binding.Annotate(key, value);
        }

        public static PoolVariableBinding CreatePoolVariableBinding(SmalltalkRuntime runtime, PoolBinding poolBinding, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (poolBinding == null)
                throw new ArgumentNullException("binding");

            Symbol varName = runtime.GetSymbol(name);
            PoolVariableBinding binding = new PoolVariableBinding(varName);
            poolBinding.Value.Add(binding);
            return binding;
        }

        public static PoolConstantBinding CreatePoolConstantBinding(SmalltalkRuntime runtime, PoolBinding poolBinding, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (poolBinding == null)
                throw new ArgumentNullException("binding");

            Symbol varName = runtime.GetSymbol(name);
            PoolConstantBinding binding = new PoolConstantBinding(varName);
            poolBinding.Value.Add(binding);
            return binding;
        }
    }
}

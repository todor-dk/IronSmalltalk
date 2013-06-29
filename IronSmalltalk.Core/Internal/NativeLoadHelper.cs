using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;

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

        public static void AnnotateObject(CompiledInitializer initializer, string key, string value)
        {
            if (initializer == null)
                throw new ArgumentNullException("initializer");
            initializer.Annotate(key, value);
        }

        public static PoolVariableBinding CreatePoolVariableBinding(SmalltalkRuntime runtime, PoolBinding poolBinding, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (poolBinding == null)
                throw new ArgumentNullException("poolBinding");

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
                throw new ArgumentNullException("poolBinding");

            Symbol varName = runtime.GetSymbol(name);
            PoolConstantBinding binding = new PoolConstantBinding(varName);
            poolBinding.Value.Add(binding);
            return binding;
        }

        public static SmalltalkRuntime CreateRuntime(bool initialize, Action<SmalltalkRuntime, SmalltalkNameScope> extensionScopeInitializer, Action<SmalltalkRuntime, SmalltalkNameScope> globalScopeInitializer)
        {
            if (extensionScopeInitializer == null)
                throw new ArgumentNullException("extensionScopeInitializer");
            if (globalScopeInitializer == null)
                throw new ArgumentNullException("globalScopeInitializer");

            SmalltalkRuntime runtime = new SmalltalkRuntime();

            ExecutionContext executionContext = new ExecutionContext(runtime);
    
            // Extension scope
            SmalltalkNameScope scope = runtime.ExtensionScope.Copy();
            extensionScopeInitializer(runtime, scope);
            runtime.SetExtensionScope(scope);
            NativeLoadHelper.RecompileClasses(scope);
            if (initialize)
                scope.ExecuteInitializers(executionContext);

            // Global scope
            scope = runtime.GlobalScope.Copy();
            globalScopeInitializer(runtime, scope);
            runtime.SetGlobalScope(scope);
            NativeLoadHelper.RecompileClasses(scope);
            if (initialize)
                scope.ExecuteInitializers(executionContext);

            return runtime;
        }

        private static void RecompileClasses(SmalltalkNameScope scope)
        {
            List<SmalltalkClass> toRecompile = new List<SmalltalkClass>();
            foreach (ClassBinding cls in scope.Classes)
            {
                // Do not recompile classes that we are going to recompile anyway
                bool subclassOfRecompiled = false;
                foreach (ClassBinding c in scope.Classes)
                {
                    if (NativeLoadHelper.InheritsFrom(cls.Value, c.Value))
                    {
                        subclassOfRecompiled = true;
                        break;
                    }
                }
                if (!subclassOfRecompiled)
                    toRecompile.Add(cls.Value);
            }

            foreach (SmalltalkClass cls in toRecompile)
            {
                cls.Recompile();
            }
        }

        private static bool InheritsFrom(SmalltalkClass self, SmalltalkClass cls)
        {
            while (self != null)
            {
                if (self.Superclass == cls)
                    return true;
                self = self.Superclass;
            }
            return false;
        }

        private static Type[] InitializerDelegateTypes = new Type[] { typeof(object), typeof(ExecutionContext) };

        public static CompiledInitializer AddProgramInitializer(SmalltalkRuntime runtime, SmalltalkNameScope scope, Type delegateType, string delegateName)
        {
            return NativeLoadHelper.AddInitializer(scope, InitializerType.ProgramInitializer, null, delegateType, delegateName);
        }

        public static CompiledInitializer AddClassInitializer(SmalltalkRuntime runtime, SmalltalkNameScope scope, Type delegateType, string delegateName, string className)
        {
            ClassBinding binding = scope.GetClassBinding(className);
            if (binding == null)
                throw new ArgumentException(String.Format("Class named {0} does not exist.", className));
            return NativeLoadHelper.AddInitializer(scope, InitializerType.ClassInitializer, binding, delegateType, delegateName);
        }

        public static CompiledInitializer AddGlobalInitializer(SmalltalkRuntime runtime, SmalltalkNameScope scope, Type delegateType, string delegateName, string globalName)
        {
            GlobalVariableOrConstantBinding binding = scope.GetGlobalVariableOrConstantBinding(globalName);
            if (binding == null)
                throw new ArgumentException(String.Format("Global variable or constant named {0} does not exist.", globalName));
            return NativeLoadHelper.AddInitializer(scope, InitializerType.GlobalInitializer, binding, delegateType, delegateName);
        }

        public static CompiledInitializer AddPoolInitializer(SmalltalkRuntime runtime, SmalltalkNameScope scope, Type delegateType, string delegateName, string poolName, string poolItemName)
        {
            PoolBinding poolBinding = scope.GetPoolBinding(poolName);
            if ((poolBinding == null) || (poolBinding.Value == null))
                throw new ArgumentException(String.Format("Pool named {0} does not exist.", poolName));
            PoolVariableOrConstantBinding binding = poolBinding.Value[poolItemName];
            if (binding == null)
                throw new ArgumentException(String.Format("Pool variable or constant named {0} does not exist in pool {1}.", poolItemName, poolName));
            return NativeLoadHelper.AddInitializer(scope, InitializerType.PoolVariableInitializer, binding, delegateType, delegateName);
        }

        private static CompiledInitializer AddInitializer(SmalltalkNameScope scope, InitializerType type, IDiscreteBinding binding, Type delegateType, string delegateName)
        {

            MethodInfo method = delegateType.GetMethod(delegateName, BindingFlags.Public | BindingFlags.Static, null, NativeLoadHelper.InitializerDelegateTypes, null);
            Func<object, ExecutionContext, object> functionDelegate = (Func<object, ExecutionContext, object>) method.CreateDelegate(typeof(Func<object, ExecutionContext, object>));

            NativeCompiledInitializer initializer = new NativeCompiledInitializer(type, binding, functionDelegate);
            scope.Initializers.Add(initializer);
            return initializer;
        }
    }
}

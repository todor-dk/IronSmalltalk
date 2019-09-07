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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common.Internal;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.Runtime.Internal
{
    public static class NativeLoadHelper
    {
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static void AddProtectedName(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Symbol symbol = runtime.GetSymbol(name);
            if (!scope.ProtectedNames.Contains(symbol))
                scope.ProtectedNames.Add(symbol);
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static ClassBinding AddClassBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Symbol symbol = runtime.GetSymbol(name);
            ClassBinding binding = new ClassBinding(symbol);
            scope.Classes.Add(binding);
            return binding;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static PoolBinding AddPoolBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Symbol symbol = runtime.GetSymbol(name);

            PoolBinding binding = new PoolBinding(symbol);
            scope.Pools.Add(binding);
            return binding;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static GlobalVariableBinding AddGlobalVariableBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Symbol symbol = runtime.GetSymbol(name);

            GlobalVariableBinding binding = new GlobalVariableBinding(symbol);
            scope.GlobalVariables.Add(binding);
            return binding;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static GlobalConstantBinding AddGlobalConstantBinding(SmalltalkRuntime runtime, SmalltalkNameScope scope, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Symbol symbol = runtime.GetSymbol(name);

            GlobalConstantBinding binding = new GlobalConstantBinding(symbol);
            scope.GlobalConstants.Add(binding);
            return binding;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static void CreateClass(SmalltalkRuntime runtime, SmalltalkNameScope scope, ClassBinding binding, string superclassName, 
            SmalltalkClass.InstanceStateEnum instanceState, string[] classVarNames, string[] instVarNames, string[] classInstVarNames, string[] importedPools,
            Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>> classMethodDicInitializer, Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>> instanceMethodDicInitializer)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));
            if (classMethodDicInitializer == null)
                throw new ArgumentNullException(nameof(classMethodDicInitializer));
            if (instanceMethodDicInitializer == null)
                throw new ArgumentNullException(nameof(instanceMethodDicInitializer));
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
            // Create method dictionaries
            MethodDictionaryInitializer clsMthInitializer = new MethodDictionaryInitializer(classMethodDicInitializer);
            MethodDictionaryInitializer instMthInitializer = new MethodDictionaryInitializer(instanceMethodDicInitializer);
            ClassMethodDictionary classMethods = new ClassMethodDictionary(runtime, clsMthInitializer.Initialize);
            InstanceMethodDictionary instanceMethods = new InstanceMethodDictionary(runtime, instMthInitializer.Initialize);

            // 4. Finally, create the behavior object
            SmalltalkClass cls = new SmalltalkClass(runtime, binding.Name, superclass, instanceState, instVars, classVars, classInstVars, pools, instanceMethods, classMethods);
            clsMthInitializer.Class = cls;
            instMthInitializer.Class = cls;
            binding.SetValue(cls);
        }

        private class MethodDictionaryInitializer
        {
            private readonly Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>> Initializer;
            internal SmalltalkClass Class { get; set; }

            public MethodDictionaryInitializer(Func<SmalltalkClass, Dictionary<Symbol, CompiledMethod>> initializer)
            {
                this.Initializer = initializer;
            }

            internal Dictionary<Symbol, CompiledMethod> Initialize(SmalltalkRuntime runtime)
            {
                return this.Initializer(this.Class);
            }
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static void CreatePool(SmalltalkRuntime runtime, PoolBinding binding)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));

            binding.SetValue(new Pool(runtime, binding.Name));
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static void AnnotateObject(IDiscreteBinding binding, string key, string value)
        {
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));
            binding.Annotate(key, value);
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static void AnnotateObject(CompiledCode initializer, string key, string value)
        {
            if (initializer == null)
                throw new ArgumentNullException(nameof(initializer));
            initializer.Annotate(key, value);
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static PoolVariableBinding CreatePoolVariableBinding(SmalltalkRuntime runtime, PoolBinding poolBinding, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (poolBinding == null)
                throw new ArgumentNullException(nameof(poolBinding));

            Symbol varName = runtime.GetSymbol(name);
            PoolVariableBinding binding = new PoolVariableBinding(varName);
            poolBinding.Value.Add(binding);
            return binding;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static PoolConstantBinding CreatePoolConstantBinding(SmalltalkRuntime runtime, PoolBinding poolBinding, string name)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));
            if (poolBinding == null)
                throw new ArgumentNullException(nameof(poolBinding));

            Symbol varName = runtime.GetSymbol(name);
            PoolConstantBinding binding = new PoolConstantBinding(varName);
            poolBinding.Value.Add(binding);
            return binding;
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static SmalltalkRuntime CreateRuntime(bool initialize, Action<SmalltalkRuntime, SmalltalkNameScope> extensionScopeInitializer, Action<SmalltalkRuntime, SmalltalkNameScope> globalScopeInitializer)
        {
            if (extensionScopeInitializer == null)
                throw new ArgumentNullException(nameof(extensionScopeInitializer));
            if (globalScopeInitializer == null)
                throw new ArgumentNullException(nameof(globalScopeInitializer));

            SmalltalkRuntime runtime = new SmalltalkRuntime();

            ExecutionContext executionContext = new ExecutionContext(runtime);
    
            // Extension scope
            SmalltalkNameScope scope = runtime.ExtensionScope.Copy();
            extensionScopeInitializer(runtime, scope);
            runtime.SetExtensionScope(scope);
            runtime.SetGlobalScope(runtime.GlobalScope.Copy(scope));
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

        private static readonly Type[] InitializerDelegateTypes = new Type[] { typeof(object), typeof(ExecutionContext) };

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static CompiledInitializer AddProgramInitializer(SmalltalkRuntime runtime, SmalltalkNameScope scope, Type delegateType, string delegateName)
        {
            return NativeLoadHelper.AddInitializer(scope, InitializerType.ProgramInitializer, null, delegateType, delegateName);
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static CompiledInitializer AddClassInitializer(SmalltalkRuntime runtime, SmalltalkNameScope scope, Type delegateType, string delegateName, string className)
        {
            ClassBinding binding = scope.GetClassBinding(className);
            if (binding == null)
                throw new ArgumentException(String.Format("Class named {0} does not exist.", className));
            return NativeLoadHelper.AddInitializer(scope, InitializerType.ClassInitializer, binding, delegateType, delegateName);
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static CompiledInitializer AddGlobalInitializer(SmalltalkRuntime runtime, SmalltalkNameScope scope, Type delegateType, string delegateName, string globalName)
        {
            GlobalVariableOrConstantBinding binding = scope.GetGlobalVariableOrConstantBinding(globalName);
            if (binding == null)
                throw new ArgumentException(String.Format("Global variable or constant named {0} does not exist.", globalName));
            return NativeLoadHelper.AddInitializer(scope, InitializerType.GlobalInitializer, binding, delegateType, delegateName);
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
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

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static NativeCompiledMethod AddClassMethod(Dictionary<Symbol, CompiledMethod> dictionary, SmalltalkClass cls, string selector, Type containingType, string nativeName)
        {
            return NativeLoadHelper.AddMethod(dictionary, cls, selector, containingType, nativeName, CompiledMethod.MethodType.Class);
        }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public static NativeCompiledMethod AddInstanceMethod(Dictionary<Symbol, CompiledMethod> dictionary, SmalltalkClass cls, string selector, Type containingType, string nativeName)
        {
            return NativeLoadHelper.AddMethod(dictionary, cls, selector, containingType, nativeName, CompiledMethod.MethodType.Instance);
        }

        private static NativeCompiledMethod AddMethod(Dictionary<Symbol, CompiledMethod> dictionary, SmalltalkClass cls, string selector, Type containingType, string nativeName, CompiledMethod.MethodType methodType)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (cls == null)
                throw new ArgumentNullException(nameof(cls));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (containingType == null)
                throw new ArgumentNullException(nameof(containingType));
            if (nativeName == null)
                throw new ArgumentNullException(nameof(nativeName));


            Symbol sel = cls.Runtime.GetSymbol(selector);
            MethodInfo nativeMethod = containingType.GetMethod(nativeName, BindingFlags.Public | BindingFlags.Static);
            if (nativeMethod == null)
                throw new MissingMethodException(containingType.FullName, nativeName);
            NativeCompiledMethod method = new NativeCompiledMethod(cls, sel, methodType, nativeMethod);
            dictionary.Add(sel, method);
            return method;
        }

        private static CompiledInitializer AddInitializer(SmalltalkNameScope scope, InitializerType type, IDiscreteBinding binding, Type delegateType, string delegateName)
        {

            MethodInfo method = TypeUtilities.Method(delegateType, delegateName, BindingFlags.Public | BindingFlags.Static, NativeLoadHelper.InitializerDelegateTypes);
            Func<object, ExecutionContext, object> functionDelegate = (Func<object, ExecutionContext, object>) method.CreateDelegate(typeof(Func<object, ExecutionContext, object>));

            NativeCompiledInitializer initializer = new NativeCompiledInitializer(type, binding, functionDelegate);
            scope.Initializers.Add(initializer);
            return initializer;
        }
    }
}

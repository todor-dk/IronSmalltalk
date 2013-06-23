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
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Bindings;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Installer.Definitions;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Installer
{
    /// <summary>
    /// Installer context encapsulates and represents the transaction that is
    /// associated with installing definitions (sources) into the smalltalk context.
    /// </summary>
    /// <remarks>
    /// The lifespan of definition installation follows the following pattern:
    /// 1. Create a new InstallerContext
    /// 2. Add definitions to the InstallerContext (read and process source files).
    ///     ... those are kept in memory as definition objects until next phase.
    /// 3. Create real runtime objects, but in the local context by calling the Install() method.
    /// 4. Modify the running SmalltalkContext with the newly created objects.
    /// 5. Run Initializers to initialize stuff (this is done outside the transaction).
    /// </remarks>
    public class InstallerContext : IInstallerContext
    {
        private List<GlobalBase> _globals = new List<GlobalBase>();
        private List<PoolValueDefinition> _poolVariables = new List<PoolValueDefinition>();
        private List<MethodDefinition> _methods = new List<MethodDefinition>();
        private List<InitializerDefinition> _initializers = new List<InitializerDefinition>();
        private List<Tuple<SmalltalkClass, ISourceReference>> _newClasses = new List<Tuple<SmalltalkClass, ISourceReference>>();
        
        /// <summary>
        /// Smalltalk context that this installation is part of.
        /// </summary>
        public SmalltalkRuntime Runtime { get; private set; }

        /// <summary>
        /// Optional error sink for reporting errors.
        /// </summary>
        public IInstallErrorSink ErrorSink { get; set; }

        /// <summary>
        /// Determines if meta-annotations (comments, documentation, etc.) 
        /// are installed (saved) in the corresponding runtime objects.
        /// </summary>
        public bool InstallMetaAnnotations { get; set; }

        public InstallerContext(SmalltalkRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.Runtime = runtime;
            this.InstallMetaAnnotations = false;
        }

        #region Preparation

        /// <summary>
        /// Add a class definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the class to be added.</param>
        public void AddClass(ClassDefinition definition)
        {
            this._globals.Add(definition);
        }

        /// <summary>
        /// Add a global (variable or constant) definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the global to be added.</param>        
        public void AddGlobal(GlobalDefinition definition)
        {
            this._globals.Add(definition);
        }

        /// <summary>
        /// Add a global initializer definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the global initializer to be added.</param>
        public void AddGlobalInitializer(GlobalInitializer initializer)
        {
            this._initializers.Add(initializer);
        }

        /// <summary>
        /// Add a (instance or class) method definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the method to be added.</param>
        public void AddMethod(MethodDefinition definition)
        {
            this._methods.Add(definition);
        }

        /// <summary>
        /// Add a pool definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the pool to be added.</param>
        public void AddPool(PoolDefinition definition)
        {
            this._globals.Add(definition);
        }

        /// <summary>
        /// Add a pool variable or pool constant definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the pool variable or pool constant to be added.</param>
        public void AddPoolVariable(PoolValueDefinition definition)
        {
            this._poolVariables.Add(definition);
        }

        /// <summary>
        /// Add a pool variable or pool constant initializer definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the initializer to be added.</param>
        public void AddPoolVariableInitializer(PoolVariableInitializer initializer)
        {
            this._initializers.Add(initializer);
        }

        /// <summary>
        /// Add a program initializer definition to the installation context.
        /// </summary>
        /// <param name="definition">Definition of the initializer to be added.</param>
        public void AddProgramInitializer(ProgramInitializer initializer)
        {
            this._initializers.Add(initializer);
        }

        #endregion

        #region Install

        public SmalltalkNameScope NameScope { get; protected set; }

        public bool Install()
        {
            this.CreateTemporaryNameSpace();
                
            if (!this.CreateGlobalBindings())
                return false;
            if (!this.CreateGlobalObjects())
                return false;
            if (!this.ValidateGlobalObjects())
                return false;
            if (!this.CreatePoolVariableBindings())
                return false;
            if (!this.ValidateMethods())
                return false;
            if (!this.ValidateInitializers())
                return false;
            if (!this.CreateMethods())
                return false;
            if (!this.CreateInitializers())
                return false;
            if (!this.AddAnnotation())
                return false;

            this.ReplaceSmalltalkContextNameSpace();
            return this.RecompileClasses(); // Must be after ReplaceSmalltalkContextNameSpace(), otherwise class cannot find subclasses.
        }

        protected virtual void CreateTemporaryNameSpace()
        {
            if (this.NameScope != null)
                throw new InvalidOperationException("Install phase has commenced.");
            this.NameScope = this.Runtime.GlobalScope.Copy();
        }

        private bool CreateGlobalBindings()
        {
            bool result = true;
            foreach(GlobalBase def in this._globals)
                result = (result & def.CreateGlobalBinding(this));
            return result;
        }

        private bool CreateGlobalObjects()
        {
            bool result = true;
            foreach (GlobalBase def in this._globals)
                result = (result & def.CreateGlobalObject(this));
            return result;
        }

        private bool ValidateGlobalObjects()
        {
            bool result = true;
            foreach (GlobalBase def in this._globals)
                result = (result & def.ValidateObject(this));
            return result;
        }

        private bool CreatePoolVariableBindings()
        {
            bool result = true;
            foreach (PoolValueDefinition def in this._poolVariables)
                result = (result & def.CreatePoolVariableBinding(this));
            return result;
        }

        private bool ValidateMethods()
        {
            bool result = true;
            foreach (MethodDefinition def in this._methods)
                result = (result & def.ValidateMethod(this));
            return result;
        }

        private bool ValidateInitializers()
        {
            bool result = true;
            foreach (InitializerDefinition def in this._initializers)
                result = (result & def.ValidateInitializer(this));
            return result;
        }

        private bool CreateMethods()
        {
            bool result = true;
            foreach (MethodDefinition def in this._methods)
                result = (result & def.CreateMethod(this));
            return result;
        }

        private bool CreateInitializers()
        {
            bool result = true;
            foreach (InitializerDefinition def in this._initializers)
                result = (result & def.CreateInitializer(this));
            return result;
        }

        private bool AddAnnotation()
        {
            bool result = true;
            foreach (GlobalBase def in this._globals)
                result = (result & def.AnnotateObject(this));
            foreach (PoolValueDefinition def in this._poolVariables)
                result = (result & def.AnnotateObject(this));
            foreach (MethodDefinition def in this._methods)
                result = (result & def.AnnotateObject(this));
            foreach (InitializerDefinition def in this._initializers)
                result = (result & def.AnnotateObject(this));
            return result;
        }

        protected virtual void ReplaceSmalltalkContextNameSpace()
        {
            if (this.NameScope == null)
                throw new InvalidOperationException("Install phase has not commenced.");
            this.Runtime.SetGlobalScope(this.NameScope);
        }

        private bool RecompileClasses()
        {
            List<Tuple<SmalltalkClass, ISourceReference>> toRecompile = new List<Tuple<SmalltalkClass, ISourceReference>>();
            foreach(Tuple<SmalltalkClass, ISourceReference> cls in this._newClasses)
            {
                // Do not recompile classes that we are going to recompile anyway
                bool subclassOfRecompiled = false;
                foreach (Tuple<SmalltalkClass, ISourceReference> c in this._newClasses)
                {
                    if (this.InheritsFrom(cls.Item1, c.Item1))
                    {
                        subclassOfRecompiled = true;
                        break;
                    }
                }
                if (!subclassOfRecompiled)
                    toRecompile.Add(cls);
            }

            bool success = true;
            foreach (Tuple<SmalltalkClass, ISourceReference> cls in toRecompile)
            {
                try
                {
                    cls.Item1.Recompile();
                }
                catch (SmalltalkDefinitionException ex)
                {
                    if (this.ErrorSink != null)
                        this.ErrorSink.AddInstallError(ex.Message, cls.Item2);
                    success = false;
                }
            }
            return success;
        }

        private bool InheritsFrom(SmalltalkClass self, SmalltalkClass cls)
        {
            while (self != null)
            {
                if (self.Superclass == cls)
                    return true;
                self = self.Superclass;
            }
            return false;
        }

        #endregion

        #region IInstallerContext interface implementation

        bool IInstallerContext.ReportError(ISourceReference sourceReference, string errorMessage)
        {
            
            if (this.ErrorSink != null)
                this.ErrorSink.AddInstallError(errorMessage, sourceReference);
            // This value has no mening to us, but makes it easier for senders to use us and return <false> directly.
            return false; 
        }

        void IInstallerContext.RegisterNewClass(SmalltalkClass cls, ISourceReference sourceReference)
        {
            if (cls == null)
                throw new ArgumentNullException("cls");
            if (sourceReference == null)
                throw new ArgumentNullException("sourceReference");
            this._newClasses.Add(new Tuple<SmalltalkClass, ISourceReference>(cls, sourceReference));
        }

        void IInstallerContext.AddClassBinding(ClassBinding binding)
        {
            this.NameScope.Classes.Add(binding);
        }

        void IInstallerContext.AddGlobalConstantBinding(GlobalConstantBinding binding)
        {
            this.NameScope.GlobalConstants.Add(binding);
        }

        void IInstallerContext.AddGlobalVariableBinding(GlobalVariableBinding binding)
        {
            this.NameScope.GlobalVariables.Add(binding);
        }

        void IInstallerContext.AddPoolBinding(PoolBinding binding)
        {
            this.NameScope.Pools.Add(binding);
        }

        void IInstallerContext.AddInitializer(CompiledInitializer initializer)
        {
            this.NameScope.Initializers.Add(initializer);
        }

        PoolBinding IInstallerContext.GetPoolBinding(Symbol name)
        {
            return this.NameScope.GetPoolBinding(name);
        }

        PoolBinding IInstallerContext.GetPoolBinding(string name)
        {
            return this.NameScope.GetPoolBinding(name);
        }

        GlobalVariableOrConstantBinding IInstallerContext.GetGlobalVariableOrConstantBinding(Symbol name)
        {
            return this.NameScope.GetGlobalVariableOrConstantBinding(name);
        }

        GlobalVariableOrConstantBinding IInstallerContext.GetGlobalVariableOrConstantBinding(string name)
        {
            return this.NameScope.GetGlobalVariableOrConstantBinding(name);
        }

        IDiscreteGlobalBinding IInstallerContext.GetLocalGlobalBinding(Symbol name)
        {
            return this.NameScope.GetLocalGlobalBinding(name);
        }

        PoolBinding IInstallerContext.GetLocalPoolBinding(Symbol name)
        {
            PoolBinding binding;
            this.NameScope.Pools.TryGetValue(name, out binding);
            return binding;
        }

        ClassBinding IInstallerContext.GetLocalClassBinding(Symbol name)
        {
            ClassBinding binding;
            this.NameScope.Classes.TryGetValue(name, out binding);
            return binding;
        }

        ClassBinding IInstallerContext.GetClassBinding(Symbol name)
        {
            return this.NameScope.GetClassBinding(name);
        }

        ClassBinding IInstallerContext.GetClassBinding(string name)
        {
            return this.NameScope.GetClassBinding(name);
        }

        bool IInstallerContext.IsProtectedName(Symbol name)
        {
            return this.NameScope.IsProtectedName(name);
        }

        bool IInstallerContext.AnnotateObject(IAnnotetable annotetableObject, IEnumerable<KeyValuePair<string, string>> annotations)
        {
            if (annotetableObject == null)
                return false;
            if (annotations == null)
                return false;

            foreach (var pair in annotations)
            {
                if (!String.IsNullOrEmpty(pair.Key))
                {
                    if (this.InstallMetaAnnotations || !pair.Key.StartsWith("ist.meta."))
                        annotetableObject.Annotate(pair.Key, pair.Value);
                }
            }

            return true;
        }

        #endregion
    }
}

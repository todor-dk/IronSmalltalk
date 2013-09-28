using IronSmalltalk.Common.Internal;
using IronSmalltalk.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.Generators
{
    internal sealed class StandardLibraryGenerator : GeneratorBase
    {
        private readonly MethodInfo ScopeInitializer;

        public StandardLibraryGenerator(NativeCompiler compiler, MethodInfo scopeInitializer)
            : base(compiler)
        {
            this.ScopeInitializer = scopeInitializer;
        }

        internal void GenerateEntryMethod()
        {
            TypeBuilder type = this.Compiler.NativeGenerator.DefineType(
                this.Compiler.GetTypeName("Smalltalk"),
                typeof(Object),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);


            // Create the initializer method ... internal static void InitializeScope(SmalltalkRuntime runtime, SmalltalkNameScope scope)
            MethodBuilder method = type.DefineMethod("InitializeScope", MethodAttributes.Static | MethodAttributes.Public);

            CustomAttributeBuilder cab = new CustomAttributeBuilder(StandardLibraryGenerator.ScopeInitializerAttributeCtor, new object[] {});
            

            // Define parameters
            ParameterExpression runtime = Expression.Parameter(typeof(SmalltalkRuntime), "runtime");
            ParameterExpression scope = Expression.Parameter(typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope), "scope");

            // IMPROVE: Why can't we use this.ScopeInitializer directly and need to do the extra lookup?
            Type initializerType = this.ScopeInitializer.DeclaringType;
            MethodInfo initializer = TypeUtilities.Method(initializerType, this.ScopeInitializer.Name, BindingFlags.Static | BindingFlags.NonPublic, typeof(SmalltalkRuntime), typeof(IronSmalltalk.Runtime.Bindings.SmalltalkNameScope));

            var lambda = Expression.Lambda<Action<SmalltalkRuntime, IronSmalltalk.Runtime.Bindings.SmalltalkNameScope>>(
                Expression.Call(initializer, runtime, scope), method.Name, new ParameterExpression[] { runtime, scope });

            // Compile the lambda into method. This is the name scope initializer method.
            if (this.Compiler.NativeGenerator.DebugInfoGenerator == null)
                lambda.CompileToMethod(method);
            else
                lambda.CompileToMethod(method, this.Compiler.NativeGenerator.DebugInfoGenerator);

            method.SetCustomAttribute(cab);
        }

        private static readonly ConstructorInfo ScopeInitializerAttributeCtor = TypeUtilities.Constructor(typeof(ScopeInitializerAttribute));
    }
}

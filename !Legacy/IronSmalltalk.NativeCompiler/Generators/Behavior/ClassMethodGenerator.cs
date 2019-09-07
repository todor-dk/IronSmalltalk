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
using IronSmalltalk.ExpressionCompiler;
using IronSmalltalk.NativeCompiler.Generators.Globals;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.NativeCompiler.Generators.Behavior
{
    internal sealed class ClassMethodGenerator : MethodGenerator
    {
        internal static ClassMethodGenerator CreateAndPrepareGenerator(ClassGenerator classGenerator)
        {
            ClassMethodGenerator generator = new ClassMethodGenerator(
                classGenerator.Compiler,
                classGenerator.Binding.Value,
                classGenerator.Binding.Value.ClassBehavior);
            generator.PrepareGenerator();
            return generator;
        }

        private ClassMethodGenerator(NativeCompiler compiler, SmalltalkClass cls, MethodDictionary methods)
            : base(compiler, cls, methods)
        {
        }

        protected override MethodCompiler GetMethodCompiler(MethodInformation method)
        {
            CompilerOptions options = new CompilerOptions();
            options.DebugInfoService = method.Method.GetDebugInfoService();
            options.LiteralEncodingStrategy = this.LiteralEncodingStrategy;
            options.DynamicCallStrategy = this.DynamicCallStrategy;
            options.DiscreteBindingEncodingStrategy = this.DiscreteBindingEncodingStrategy;
            return new ClassMethodCompiler(this.Compiler.Parameters.Runtime, options);
        }

        protected override string TypeName
        {
            get { return String.Format("{0} class", this.Class.Name.Value); }
        }

        protected override string InitMethodDictionariesMethodName
        {
            get { return String.Format("Init_{0}_ClassMethods", this.Class.Name.Value); }
        }

        private static readonly MethodInfo AddClassMethodMethod = TypeUtilities.Method(typeof(NativeLoadHelper), "AddClassMethod",
            typeof(Dictionary<Symbol, CompiledMethod>), typeof(SmalltalkClass), typeof(string), typeof(Type), typeof(string));

        protected override System.Reflection.MethodInfo AddMethodMethod
        {
            get { return ClassMethodGenerator.AddClassMethodMethod; }
        }
    }
}

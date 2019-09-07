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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.NativeCompiler.Internals;

namespace IronSmalltalk.NativeCompiler.CompilationStrategies
{
    internal sealed class LiteralGenerator
    {
        public readonly INativeStrategyClient Client;

        public readonly string LiteralTypeName;

        public LiteralGenerator(INativeStrategyClient client, string literalTypeName)
        {
            this.Client = client;
            this.LiteralTypeName = literalTypeName;
        }

        private TypeBuilder _LiteralsType = null;
        private TypeBuilder LiteralsType
        {
            get
            {
                if (this._LiteralsType == null)
                    this._LiteralsType = this.GetLiteralsType();
                return this._LiteralsType;
            }
        }

        private TypeBuilder GetLiteralsType()
        {
            string name = string.Format("{0}.{1}", this.Client.ContainingType.FullName, this.LiteralTypeName);
            name = this.Client.Compiler.NativeGenerator.AsLegalTypeName(name);
            return this.Client.ContainingType.DefineNestedType(
                name,
                TypeAttributes.Class | TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Abstract,
                typeof(object));
        }

        private Type LiteralTypeType;

        internal void GenerateLiteralType()
        {
            if (this._LiteralsType == null)
                return;

            // It would have been nice to use the Lambda Compiler, but it can't compile constructors,
            // ... so we compile a helper method that retunrns an array ...
            NewArrayExpression array = Expression.NewArrayInit(typeof(object), this.DefinedLiterals.Select(def => def.Initializer));
            LambdaExpression lambda = Expression.Lambda(array, true);
            MethodBuilder methodBuilder = this.LiteralsType.DefineMethod("InitializeLiterals", MethodAttributes.Private | MethodAttributes.Static);
            if (this.Client.Compiler.NativeGenerator.DebugInfoGenerator == null)
                lambda.CompileToMethod(methodBuilder);
            else
                lambda.CompileToMethod(methodBuilder, this.Client.Compiler.NativeGenerator.DebugInfoGenerator);
            // ... and fromt his array we generate the constructor by emitting IL code by hand :/
            ConstructorBuilder ctor = this._LiteralsType.DefineConstructor(
                MethodAttributes.Private | MethodAttributes.Static, CallingConventions.Standard, new Type[0]);
            ILGenerator ctorIL = ctor.GetILGenerator();
            ctorIL.DeclareLocal(typeof(object[]));      // Define a temp var for the array
            ctorIL.Emit(OpCodes.Call, methodBuilder);   // Call the InitializeLiterals method.
            ctorIL.Emit(OpCodes.Stloc_0);               // Store the result in the temp var
            // ... now assign to each literal field
            for (int i = 0; i < this.DefinedLiterals.Count; i++)
            {
                ctorIL.Emit(OpCodes.Ldloc_0);           // Load the temp array var
                ctorIL.PushInt(i);                      // Index in the array
                ctorIL.Emit(OpCodes.Ldelem_Ref);        // Load elem from the array at the given index
                ctorIL.Emit(OpCodes.Stsfld, this.DefinedLiterals[i].Field);  // Store the value in the static field
            }
            ctorIL.Emit(OpCodes.Ret);

            this.LiteralTypeType = this._LiteralsType.CreateType();
        }

        internal FieldBuilder DefineLiteralField(string prefix, Expression initializer)
        {
            string name = string.Format("{0}${1}", prefix, this.LiteralCounter++);

            name = this.Client.Compiler.NativeGenerator.AsLegalMethodName(name);

            FieldBuilder field = this.LiteralsType.DefineField(name, typeof(object), FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Assembly);
            this.DefinedLiterals.Add(new LiteralDefinition(name, field, initializer));

            return field;
        }

        private int LiteralCounter = 1;

        private readonly List<LiteralDefinition> DefinedLiterals = new List<LiteralDefinition>();

        private struct LiteralDefinition
        {
            public readonly string Name;
            public readonly Expression Initializer;
            public readonly FieldBuilder Field;

            public LiteralDefinition(string name, FieldBuilder field, Expression initializer)
                : this()
            {
                this.Name = name;
                this.Field = field;
                this.Initializer = initializer;
            }
        }
    }
}

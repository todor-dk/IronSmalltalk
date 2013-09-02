using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Execution;

namespace IronSmalltalk.NativeCompiler.Internals
{
    public class NativeLiteralEncodingStrategy : ILiteralEncodingStrategy
    {

        private readonly MethodGenerator MethodGenerator;

        internal NativeLiteralEncodingStrategy(MethodGenerator methodGenerator)
        {
            if (methodGenerator == null)
                throw new ArgumentNullException();
            this.MethodGenerator = methodGenerator;
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
            string name = string.Format("{0}.{1}", this.MethodGenerator.TypeBuilder.FullName, "$Literals");
            name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalTypeName(name);
            return this.MethodGenerator.TypeBuilder.DefineNestedType(
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
            lambda.CompileToMethod(methodBuilder, this.MethodGenerator.Compiler.NativeGenerator.DebugInfoGenerator);
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


        private TypeBuilder _LiteralCallSitesType = null;
        private TypeBuilder LiteralCallSitesType
        {
            get
            {
                if (this._LiteralCallSitesType == null)
                    this._LiteralCallSitesType = this.GetLiteralCallSitesType();
                return this._LiteralCallSitesType;
            }
        }

        private Type LiteralCallSitesTypeType;

        private TypeBuilder GetLiteralCallSitesType()
        {
            string name = string.Format("{0}.{1}", this.MethodGenerator.TypeBuilder.FullName, "$LiteralCallSites");
            name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalTypeName(name);
            return this.MethodGenerator.TypeBuilder.DefineNestedType(
                name,
                TypeAttributes.Class | TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Abstract,
                typeof(object));
        }

        internal void GenerateLiteralCallSitesType()
        {
            if (this._LiteralCallSitesType == null)
                return;

            // It would have been nice to use the Lambda Compiler, but it can't compile constructors,
            // so we have to generate the constructor by emitting IL code by hand :/
            ConstructorBuilder ctor = this._LiteralCallSitesType.DefineConstructor(
                MethodAttributes.Private | MethodAttributes.Static, CallingConventions.Standard, new Type[0]);
            ILGenerator ctorIL = ctor.GetILGenerator();
            // ... now assign to each literal field
            for (int i = 0; i < this.DefinedLiteralCallSites.Count; i++)
            {
                CallSiteDefinition def = this.DefinedLiteralCallSites[i];
                MethodInfo create = def.SiteType.GetMethod("Create");   // Get the CallSite<>.Create method
                def.Binder.GenerateBinderInitializer(ctorIL);           // Load the Binder
                ctorIL.Emit(OpCodes.Call, create);                      // Call CallSite<>.Create(Binder) 
                ctorIL.Emit(OpCodes.Stsfld, def.CallSiteField);         // Store the value in the static field for the call site
            }
            ctorIL.Emit(OpCodes.Ret);

            this.LiteralCallSitesTypeType = this._LiteralCallSitesType.CreateType();
        }

        private List<CallSiteDefinition> DefinedLiteralCallSites = new List<CallSiteDefinition>();

        private struct CallSiteDefinition
        {
            public readonly string Name;
            public readonly FieldBuilder CallSiteField;
            public readonly Type DelegateType;
            public readonly Type SiteType;
            public readonly IBinderDefinition Binder;

            public CallSiteDefinition(string name, Type delegateType, Type siteType, FieldBuilder callSiteField, IBinderDefinition binder)
            {
                this.Name = name;
                this.DelegateType = delegateType;
                this.SiteType = siteType;
                this.CallSiteField = callSiteField;
                this.Binder = binder;
            }
        }

        internal interface IBinderDefinition
        {
            void GenerateBinderInitializer(ILGenerator ilgen);
        }

        private class SymbolBinderDefinition : IBinderDefinition
        {
            public readonly string SymbolKey;

            public SymbolBinderDefinition(string symbolKey)
            {
                if (symbolKey == null)
                    throw new ArgumentNullException("symbolKey");
               this.SymbolKey = symbolKey;
            }

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                MethodInfo getBinder = typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache).GetMethod("GetSymbolBinder");

                ilgen.Emit(OpCodes.Ldstr, this.SymbolKey);
                ilgen.Emit(OpCodes.Call, getBinder);
            }

        }

        //private Expression DefineLiteralCallSite(string nameSuggestion)
        //{
        //    //string nameSuggestion = String.Format("{0}.{1}", this.CurrentMethodName, selector);
        //    string name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalMethodName(nameSuggestion);
        //    int idx = 0;
        //    while (this.DefinedLiteralCallSites.Any(def => def.Name == name))
        //        name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalMethodName(String.Format("{0}${1}", nameSuggestion, idx++));

        //    Type delegateType = typeof(Func<ExecutionContext, Symbol>);
        //    Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);

        //    FieldBuilder field = this.LiteralCallSitesType.DefineField(name, siteType, FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Assembly);
        //    this.DefinedCallSites.Add(new CallSiteDefinition(name, delegateType, siteType, field, binder));

        //    return Expression.Field(null, field);
        //}










        private int LiteralCounter = 1;

        private Expression DefineLiteral(string prefix, string valueText, Expression initializer)
        {
            return this.DefineLiteral(string.Format("{0}_{1}", prefix, valueText), initializer);
        }

        private Expression DefineLiteral(string prefix, Expression initializer)
        {
            string name = string.Format("{0}${1}", prefix, this.LiteralCounter++);

            name = this.MethodGenerator.Compiler.NativeGenerator.AsLegalMethodName(name);

            FieldBuilder field = this.LiteralsType.DefineField(name, typeof(object), FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Assembly);
            this.DefinedLiterals.Add(new LiteralDefinition(name, field, initializer));

            return Expression.Field(null, field);
        }

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


        public Expression Array(EncoderVisitor visitor, IList<LiteralNode> elements)
        {
            return Expression.Constant(null, typeof(object));
        }

        public Expression Array(LiteralVisitorExpressionValue visitor, IList<LiteralNode> elements)
        {
            return Expression.Constant(null, typeof(object));
        }

        public Expression Character(VisitingContext context, char value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(char)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("Char", string.Format("0x{0:X4}", (int)value, CultureInfo.InvariantCulture), initializer);
        }

        public Expression False(VisitingContext context)
        {
            return PreboxedConstants.False_Expression;
        }

        public Expression FloatD(VisitingContext context, double value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(double)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("FloatD", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        public Expression FloatE(VisitingContext context, float value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(float)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("FloatE", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        public Expression LargeInteger(VisitingContext context, BigInteger value)
        {
            Expression bytes = Expression.NewArrayInit(typeof(byte),
                value.ToByteArray().Select(b => Expression.Constant(b, typeof(byte))));
            ConstructorInfo ctor = typeof(BigInteger).GetConstructor(new Type[] { typeof(byte[]) });
            Expression initializer = Expression.Convert(Expression.New(ctor, bytes), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("BigInteger", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        public Expression Nil(VisitingContext context)
        {
            return PreboxedConstants.Nil_Expression;
        }

        public Expression ScaledDecimal(VisitingContext context, BigDecimal value)
        {
            // The numerator of the BigDecimal
            ConstructorInfo ctor = typeof(BigInteger).GetConstructor(new Type[] { typeof(byte[]) });
            Expression bytes = Expression.NewArrayInit(typeof(byte),
                value.Numerator.ToByteArray().Select(b => Expression.Constant(b, typeof(byte))));
            Expression numerator = Expression.New(ctor, bytes);
            // The scale
            Expression scale = Expression.Constant(value.Scale, typeof(int));
            // Constructing a new BigDecimal
            ctor = typeof(BigDecimal).GetConstructor(new Type[] { typeof(BigInteger), typeof(int) });
            Expression initializer = Expression.Convert(Expression.New(ctor, numerator, scale), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("BigDecimal", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        public Expression SmallInteger(VisitingContext context, int value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(int)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral("Int", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        public Expression String(VisitingContext context, string value)
        {
            return Expression.Convert(Expression.Constant(value, typeof(string)), typeof(object));
        }

        public Expression Symbol(VisitingContext context, string value)
        {
            return Expression.Constant(null, typeof(object));
        }

        public Expression True(VisitingContext context)
        {
            return PreboxedConstants.True_Expression;
        }

        public Expression GetZero(Type type)
        {
            if (type == typeof(System.Numerics.BigInteger))
                return Expression.Property(null, typeof(System.Numerics.BigInteger), "Zero");
            if (type == typeof(Int64))
                return Expression.Constant((Int64)0, type);
            if (type == typeof(Int32))
                return Expression.Constant((Int32)0, type);
            if (type == typeof(Int16))
                return Expression.Constant((Int16)0, type);
            if (type == typeof(SByte))
                return Expression.Constant((SByte)0, type);
            if (type == typeof(UInt64))
                return Expression.Constant((UInt64)0, type);
            if (type == typeof(UInt32))
                return Expression.Constant((UInt32)0, type);
            if (type == typeof(UInt16))
                return Expression.Constant((UInt16)0, type);
            if (type == typeof(Byte))
                return Expression.Constant((Byte)0, type);
            throw new NotImplementedException();
        }

        public Expression GetOne(Type type)
        {
            if (type == typeof(System.Numerics.BigInteger))
                return Expression.Property(null, typeof(System.Numerics.BigInteger), "One");
            if (type == typeof(Int64))
                return Expression.Constant((Int64)1, type);
            if (type == typeof(Int32))
                return Expression.Constant((Int32)1, type);
            if (type == typeof(Int16))
                return Expression.Constant((Int16)1, type);
            if (type == typeof(SByte))
                return Expression.Constant((SByte)1, type);
            if (type == typeof(UInt64))
                return Expression.Constant((UInt64)1, type);
            if (type == typeof(UInt32))
                return Expression.Constant((UInt32)1, type);
            if (type == typeof(UInt16))
                return Expression.Constant((UInt16)1, type);
            if (type == typeof(Byte))
                return Expression.Constant((Byte)1, type);
            throw new NotImplementedException();
        }

        public Expression GenericLiteral(VisitingContext context, string name, Expression value)
        {
            return this.DefineLiteral(name, Expression.Convert(value, typeof(object)));
        }
    }
}

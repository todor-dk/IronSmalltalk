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
using IronSmalltalk.Common.Internal;
using IronSmalltalk.Compiler.SemanticNodes;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.NativeCompiler.Internals;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.NativeCompiler.CompilationStrategies
{
    internal sealed class NativeLiteralEncodingStrategy : ILiteralEncodingStrategy
    {
        private readonly LiteralGenerator LiteralGenerator;

        private readonly CallSiteGenerator CallSiteGenerator;

        internal NativeLiteralEncodingStrategy(INativeStrategyClient client)
        {
            if (client == null)
                throw new ArgumentNullException();
            this.LiteralGenerator = new LiteralGenerator(client, "$Literals");
            this.CallSiteGenerator = new CallSiteGenerator(client, "$LiteralCallSites");
        }

        internal void GenerateTypes()
        {
            this.LiteralGenerator.GenerateLiteralType();
            this.CallSiteGenerator.GenerateCallSitesType();
        }

        private Expression DefineLiteral(EncoderVisitor visitor, string prefix, string valueText, Expression initializer)
        {
            return this.DefineLiteral(visitor, string.Format("{0}_{1}", prefix, valueText), initializer);
        }

        private Expression DefineLiteral(EncoderVisitor visitor, string prefix, Expression initializer)
        {
            // If encoding part of an array, return the initializer directly. The outer-most visitor will do the job.
            if (visitor.GetNativeLiteralArrayVisitor() != null)
                return initializer;
            else
                return Expression.Field(null, this.LiteralGenerator.DefineLiteralField(prefix, initializer));        
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

            private static readonly MethodInfo GetSymbolBinderMethod = TypeUtilities.Method(
                typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.CallSiteBinderCache), "GetSymbolBinder");

            public void GenerateBinderInitializer(ILGenerator ilgen)
            {
                ilgen.Emit(OpCodes.Ldstr, this.SymbolKey);
                ilgen.Emit(OpCodes.Call, SymbolBinderDefinition.GetSymbolBinderMethod);
            }
        }

        private class ArrayBinderDefinition : IBinderDefinition
        {
            public readonly FieldInfo ArrayField;

            public ArrayBinderDefinition(FieldInfo arrayField)
            {
                if (arrayField == null)
                    throw new ArgumentNullException("arrayField");
                this.ArrayField = arrayField;
            }

            private static readonly ConstructorInfo ArrayCallSiteBinderCtor = TypeUtilities.Constructor(
                typeof(IronSmalltalk.Runtime.Execution.CallSiteBinders.ArrayCallSiteBinder), typeof(object[]));

            void IBinderDefinition.GenerateBinderInitializer(ILGenerator ilgen)
            {
                ilgen.Emit(OpCodes.Ldsfld, this.ArrayField);
                ilgen.Emit(OpCodes.Castclass, typeof(object[]));
                ilgen.Emit(OpCodes.Newobj, ArrayBinderDefinition.ArrayCallSiteBinderCtor);
            }
        }

        internal class NativeLiteralArrayVisitor : LiteralVisitorExpressionValue
        {
            internal bool HadSymbols = false;

            public NativeLiteralArrayVisitor(EncoderVisitor enclosingVisitor)
                : base(enclosingVisitor)
            {

            }
        }

        Expression ILiteralEncodingStrategy.Array(EncoderVisitor visitor, IList<LiteralNode> elements)
        {
            NativeLiteralArrayVisitor arrayVisitor = new NativeLiteralArrayVisitor(visitor);
            Expression[] items = new Expression[elements.Count];
            for (int i = 0; i < items.Length; i++)
                items[i] = elements[i].Accept(arrayVisitor);

            Expression initializer = Expression.NewArrayInit(typeof(object), items);
            NativeLiteralArrayVisitor outerArrrayVisitor = visitor.GetNativeLiteralArrayVisitor();
            if (outerArrrayVisitor != null)
            {
                outerArrrayVisitor.HadSymbols = outerArrrayVisitor.HadSymbols | arrayVisitor.HadSymbols;
                return initializer; // Embed in outer array
            }

            // Root (outer-most) array visitor 
            if (!arrayVisitor.HadSymbols)
                return this.DefineLiteral(visitor, "Array", initializer);

            // Special handling if the array contains symbols.
            FieldInfo field = this.LiteralGenerator.DefineLiteralField("Array", initializer);
            ArrayBinderDefinition binder = new ArrayBinderDefinition(field);
            return this.GenerateLiteralCallSite(visitor, binder, "Array");
        }

        Expression ILiteralEncodingStrategy.Character(EncoderVisitor visitor, char value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(char)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral(visitor, "Char", string.Format("0x{0:X4}", (int)value, CultureInfo.InvariantCulture), initializer);
        }

        Expression ILiteralEncodingStrategy.False(EncoderVisitor visitor)
        {
            return PreboxedConstants.False_Expression;
        }

        Expression ILiteralEncodingStrategy.FloatD(EncoderVisitor visitor, double value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(double)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral(visitor, "FloatD", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        Expression ILiteralEncodingStrategy.FloatE(EncoderVisitor visitor, float value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(float)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral(visitor, "FloatE", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        private static readonly ConstructorInfo BigIntegerCtor = TypeUtilities.Constructor(typeof(BigInteger), typeof(byte[]));

        Expression ILiteralEncodingStrategy.LargeInteger(EncoderVisitor visitor, BigInteger value)
        {
            Expression bytes = Expression.NewArrayInit(typeof(byte),
                value.ToByteArray().Select(b => Expression.Constant(b, typeof(byte))));
            Expression initializer = Expression.Convert(Expression.New(NativeLiteralEncodingStrategy.BigIntegerCtor, bytes), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral(visitor, "BigInteger", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        Expression ILiteralEncodingStrategy.Nil(EncoderVisitor visitor)
        {
            return PreboxedConstants.Nil_Expression;
        }

        private static readonly ConstructorInfo BigDecimalCtor = TypeUtilities.Constructor(typeof(BigDecimal), typeof(BigInteger), typeof(int));

        Expression ILiteralEncodingStrategy.ScaledDecimal(EncoderVisitor visitor, BigDecimal value)
        {
            // The numerator of the BigDecimal
            Expression bytes = Expression.NewArrayInit(typeof(byte),
                value.Numerator.ToByteArray().Select(b => Expression.Constant(b, typeof(byte))));
            Expression numerator = Expression.New(NativeLiteralEncodingStrategy.BigIntegerCtor, bytes);
            // The scale
            Expression scale = Expression.Constant(value.Scale, typeof(int));
            // Constructing a new BigDecimal
            Expression initializer = Expression.Convert(Expression.New(NativeLiteralEncodingStrategy.BigDecimalCtor, numerator, scale), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral(visitor, "BigDecimal", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        Expression ILiteralEncodingStrategy.SmallInteger(EncoderVisitor visitor, int value)
        {
            Expression initializer = Expression.Convert(Expression.Constant(value, typeof(int)), typeof(object));
            return PreboxedConstants.GetConstant(value) ?? this.DefineLiteral(visitor, "Int", string.Format("{0}", value, CultureInfo.InvariantCulture), initializer);
        }

        Expression ILiteralEncodingStrategy.String(EncoderVisitor visitor, string value)
        {
            return Expression.Convert(Expression.Constant(value, typeof(string)), typeof(object));
        }

        private static readonly ConstructorInfo SymbolPlaceholderCtor = TypeUtilities.Constructor(typeof(SymbolPlaceholder), typeof(string));

        Expression ILiteralEncodingStrategy.Symbol(EncoderVisitor visitor, string value)
        {
            NativeLiteralArrayVisitor arrayVisitor = visitor.GetNativeLiteralArrayVisitor();
            if (arrayVisitor != null)
            {
                // If inside a literal array, special handling ... return a SymbolPlaceholder instead.
                arrayVisitor.HadSymbols = true;
                return Expression.Convert(Expression.New(NativeLiteralEncodingStrategy.SymbolPlaceholderCtor, Expression.Constant(value, typeof(string))), typeof(object));
            }

            SymbolBinderDefinition binder = new SymbolBinderDefinition(value);
            return this.GenerateLiteralCallSite(visitor, binder, value);
        }

        private Expression GenerateLiteralCallSite(EncoderVisitor visitor, IBinderDefinition binder, string nameSuggestion)
        {
            Type delegateType = typeof(Func<CallSite, ExecutionContext, object>);
            Type siteType = typeof(CallSite<>).MakeGenericType(delegateType);
            Expression callSite = this.CallSiteGenerator.CreateCallSite(binder, delegateType, siteType, nameSuggestion);

            List<Expression> args = new List<Expression>();
            args.Add(callSite);
            args.Add(visitor.Context.ExecutionContextArgument);

            FieldInfo target = TypeUtilities.Field(siteType, "Target", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo invoke = TypeUtilities.Method(delegateType, "Invoke");
            ParameterInfo[] pis = invoke.GetParameters();

            // siteExpr.Target.Invoke(siteExpr, *args)
            return Expression.Call(
                Expression.Field(callSite, target),
                invoke,
                args);

            /* C# Style
            ParameterExpression site = Expression.Variable(siteType, "$site");
            List<Expression> args = new List<Expression>();
            args.Add(site);
            args.Add(context.ExecutionContext);
            args.AddRange(arguments);

            FieldInfo target = TypeUtilities.Field(siteType, "Target", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo invoke = TypeUtilities.Method(delegateType, "Invoke");
            ParameterInfo[] pis = invoke.GetParameters();
            // ($site = siteExpr).Target.Invoke($site, *args)
            return Expression.Block(
                new[] { site },
                Expression.Call(
                    Expression.Field(Expression.Assign(site, callSite), target),
                    invoke,
                    args));
             */
        }

        Expression ILiteralEncodingStrategy.True(EncoderVisitor visitor)
        {
            return PreboxedConstants.True_Expression;
        }

        Expression ILiteralEncodingStrategy.GetZero(Type type)
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

        Expression ILiteralEncodingStrategy.GetOne(Type type)
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

        Expression ILiteralEncodingStrategy.GenericLiteral(EncoderVisitor visitor, string name, Expression value)
        {
            return this.DefineLiteral(visitor, name, Expression.Convert(value, typeof(object)));
        }
    }
}

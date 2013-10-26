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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Primitives.Exceptions;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    public abstract class PrimitiveEncoder
    {
        public PrimitiveCallVisitor Visitor { get; private set; }
        public IReadOnlyList<string> Parameters { get; private set; }

        public PrimitiveEncoder(PrimitiveCallVisitor visitor, IEnumerable<string> parameters)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            this.Visitor = visitor;
            this.Parameters = parameters.ToList().AsReadOnly();
        }

        public ExpressionCompiler Compiler
        {
            get { return this.Context.Compiler; }
        }

        public CompilationContext Context
        {
            get { return this.Visitor.Context; }
        }


        protected Expression UnaryOperation(Func<Expression, Expression> func)
        {
            return this.UnaryOperation((arg, type) => func(arg));
        }

        protected Expression UnaryOperation(Func<Expression, Type, Expression> func)
        {
            return this.UnaryOperation(Conversion.Checked, func);
        }

        protected Expression UnaryOperation(Conversion conversion, Func<Expression, Type, Expression> func)
        {
            if (this.Parameters == null)
                return null;
            if (this.Parameters.Count != 1)
                return null;
            Type type = (this.Parameters[0] == "self") ? null : NativeTypeClassMap.GetType(this.Parameters[0]);
            Type[] types = new Type[] { type };
            IList<Expression> args = this.GetArguments(types, conversion);
            return func(args[0], type);
        }

        protected Expression BinaryOperation(Func<Expression, Expression, Expression> func)
        {
            return this.BinaryOperation((arg1, arg2, type1, type2) => func(arg1, arg2));
        }

        protected Expression BinaryOperation(Func<Expression, Expression, Type, Type, Expression> func)
        {
            return this.BinaryOperation(Conversion.Checked, func);
        }

        protected Expression BinaryOperation(Conversion conversion, Func<Expression, Expression, Type, Type, Expression> func)
        {
            if (this.Parameters == null)
                return null;
            if (this.Parameters.Count != 2)
                return null;
            Type type0 = (this.Parameters[0] == "self") ? null : NativeTypeClassMap.GetType(this.Parameters[0]);
            Type type1 = (this.Parameters[1] == "self") ? null : NativeTypeClassMap.GetType(this.Parameters[1]);
            Type[] types = new Type[] { type0, type1 };
            IList<Expression> args = this.GetArguments(types, conversion);
            return func(args[0], args[1], type0, type1);
        }

        protected Expression Convert(Expression parameter, Type type, Conversion conversion)
        {
            // Special cases that are easy to handle
            if (type == null)
                return parameter;
            else if (type == typeof(object))
                return Expression.Convert(parameter, typeof(object));

            // This is the tricky part .... 
            //
            // Example that will NOT WORK:
            //      Int16 i16 = 123;
            //      Object o16 = i16;
            //      Int32 i32 = (Int32) o16     // *** FAILS *** ... even if object currently has an Int16, it's an object and no cast to Int32!
            //  
            // Example that works:
            //      Int16 i16 = 123;
            //      Object o16 = i16;
            //      Int32 i32 = (Int16) o16     // OK! First cast Object=>Int16 then IMPLICIT cast Int16=>Int32
            //
            // VERY IMPORTANT!!!!
            //      This should ONLY do implicit conversion AND NO EXPLICIT conversions.
            //      If we do explicit conversion, we are screwed because the value 
            //      will loose precision - and this is critical show stopper!
            //
            // C# Pseudocode Example ... cast to Int32:
            //      if (obj is Int32)
            //          return (Int32) obj;
            //      else
            //          return (Int32) ((dynamic) obj);     // This uses a CallSite and a C# CallSiteBinder to do the cast.
            //
            Expression castConvert;
            if ((conversion & Conversion.Checked) == Conversion.Checked)
                castConvert = Expression.ConvertChecked(parameter, type);
            else
                castConvert = Expression.Convert(parameter, type);
            Expression dynamicConvert = this.Visitor.Context.CompileDynamicConvert(parameter, type, conversion);

            return Expression.Condition(
                // Improve: The "is" test can be optimized for reference types, e.g.
                //      if ((obj is String) || (obj == null))   ... Null always casts to reference type
                Expression.TypeIs(parameter, type), 
                castConvert,
                dynamicConvert);
        }

        /// <summary>
        /// An array of Expression representing empty (no) arguments. For convenience.
        /// </summary>
        private static readonly Expression[] EmptyArguments = new Expression[0];

        /// <summary>
        /// This converts the arguments that were passed in to expressions with the correct types.
        /// </summary>
        /// <param name="argumentTypes">Collection of types to convert to.</param>
        /// <param name="conversion">Type of conversion to perform on each argument.</param>
        /// <returns>Collection of argument expressions that can be passed to the member call.</returns>
        protected IList<Expression> GetArguments(Type[] argumentTypes, Conversion conversion)
        {
            if (argumentTypes == null)
                throw new ArgumentNullException("argumentTypes"); 
            
            // Arguments exists only at the method / initializer level ... not in blocks
            Expression[] arguments = this.Context.RootContext.MethodArguments.Cast<Expression>().ToArray();
            if (arguments == null)
                arguments = PrimitiveEncoder.EmptyArguments;

            List<Expression> args = new List<Expression>(argumentTypes.Length);

            // There are two options:
            if (argumentTypes.Length == arguments.Length)
            {
                // 1. Defined exactly the same number of arguments as there were passed to the method,
                //      then simply convert and map each argument passed to us to an argument that we are passing to the method.
                for (int i = 0; i < argumentTypes.Length; i++)
                    args.Add(this.Convert(arguments[i], argumentTypes[i], conversion));
                return args;
            }
            if (argumentTypes.Length == (arguments.Length + 1))
            {
                // 2. Exactly one more argument was defined than passed to the method,
                //      implying that the first defined argument is mapped to the receiver (self),
                //      and the remaining arguments are mapped to the arguments passed to the method.
                args.Add(this.Convert(this.Context.Self, argumentTypes[0], conversion));

                for (int i = 1; i < argumentTypes.Length; i++)
                    args.Add(this.Convert(arguments[i - 1], argumentTypes[i], conversion));
                return args;
            }
            // Some mismatch :-/
            throw new PrimitiveInternalException(CodeGenerationErrors.WrongNumberOfParameters);
        }

        public static Expression EncodeReferenceEquals(Expression a, Expression b)
        {
            return PrimitiveEncoder.EncodeReferenceEquals(a, b, PreboxedConstants.True_Expression, PreboxedConstants.False_Expression);
        }

        public static Expression EncodeReferenceEquals(Expression a, Expression b, Expression trueValue, Expression falseValue)
        {
            return Expression.Condition(
                Expression.AndAlso(Expression.TypeIs(a, typeof(bool)), Expression.TypeIs(b, typeof(bool))),
                Expression.Condition(
                    Expression.Equal(a, b),
                    trueValue,
                    falseValue),
                Expression.Condition(
                    Expression.ReferenceEqual(a, b),
                    trueValue,
                    falseValue));
        }
    }


    [Flags]
    public enum Conversion
    {
        /// <summary>
        /// The conversion happens in a checked context. 
        /// The conversion throws an exception if the target type is overflowed.
        /// </summary>
        Checked = 1,

        /// <summary>
        /// The conversion is explicit, contrary to an implicit conversion.
        /// </summary>
        Explicit = 2
    }
}

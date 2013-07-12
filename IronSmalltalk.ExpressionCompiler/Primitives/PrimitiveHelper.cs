﻿/*
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
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.Runtime.Execution.Internals;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    internal static class PrimitiveHelper
    {
        /// <summary>
        /// An array of DynamicMetaObjects representing empty (no) arguments. For convenience.
        /// </summary>
        public static readonly DynamicMetaObject[] EmptyArguments = new DynamicMetaObject[0];

        /// <summary>
        /// This converts the arguments that were passed in to expressions with the correct types.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="argumentTypes">Collection of types to convert to.</param>
        /// <param name="conversion">Type of conversion to perform on each argument.</param>
        /// <returns>Collection of argument expressions that can be passed to the member call.</returns>
        public static IList<Expression> GetArguments(PrimitiveBuilder builder, Type[] argumentTypes, Conversion conversion)
        {
            if (builder == null)
                throw new ArgumentNullException();

            DynamicMetaObject[] arguments = builder.Arguments;
            if (arguments == null)
                arguments = PrimitiveHelper.EmptyArguments;
            if (argumentTypes == null)
                throw new ArgumentNullException("argumentTypes");

            List<Expression> args = new List<Expression>(argumentTypes.Length);

            // There are two options:
            if (argumentTypes.Length == arguments.Length)
            {
                // 1. Defined exactly the same number of arguments as there were passed to the method,
                //      then simply convert and map each argument passed to us to an argument that we are passing to the method.
                BindingRestrictions restrictions = builder.Restrictions;
                for (int i = 0; i < argumentTypes.Length; i++)
                    args.Add(PrimitiveHelper.Convert(arguments[i], argumentTypes[i], conversion, ref restrictions));
                builder.Restrictions = restrictions;
                return args;
            }
            if (argumentTypes.Length == (arguments.Length + 1))
            {
                // 2. Exactly one more argument was defined than passed to the method,
                //      implying that the first defined argument is mapped to the receiver (self),
                //      and the remaining arguments are mapped to the arguments passed to the method.
                BindingRestrictions restrictions = builder.Restrictions;
                args.Add(PrimitiveHelper.Convert(builder.Self, argumentTypes[0], conversion, ref restrictions));

                for (int i = 1; i < argumentTypes.Length; i++)
                    args.Add(PrimitiveHelper.Convert(arguments[i - 1], argumentTypes[i], conversion, ref restrictions));
                builder.Restrictions = restrictions;
                return args;
            }
            // Some mismatch :-/
            throw new PrimitiveInternalException(CodeGenerationErrors.WrongNumberOfParameters);
        }

        public static Expression Convert(DynamicMetaObject parameter, Type type, Conversion conversion, ref BindingRestrictions restrictions)
        {
            if (type == null)
                return parameter.Expression;
            else if (type == typeof(object))
                return Expression.Convert(parameter.Expression, typeof(object));

            Type limitingType = (parameter.Value != null) ? parameter.Value.GetType() : parameter.LimitType;
            if ((limitingType == null) || (limitingType == typeof(object)))
            {
                if ((conversion & Conversion.Checked) == Conversion.Checked)
                    return Expression.ConvertChecked(parameter.Expression, type);
                else
                    return Expression.Convert(parameter.Expression, type);
            } else {
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
                //      This should ONLY do implicit covertion AND NO EXPLICIT conversions.
                //      If we do explicit conversion, we are screwed because the value 
                //      will loose precision - and this is critical show stopper!

                // 1. Create an polymorphic inlined cache restrictions ... as long as the arguments are of the given type,
                //      we can "hardcode" the cast into the expression code (there is no way to do dynamic cast - cast is a static thing)
                if (restrictions == null)
                    restrictions = BindingRestrictions.Empty;
                if (parameter.Restrictions != null)
                    restrictions = restrictions.Merge(parameter.Restrictions);
                restrictions = restrictions.Merge(BindingRestrictions.GetTypeRestriction(parameter.Expression, limitingType));

                if (limitingType == type)
                {
                    // No need for double cast ... the argument is already in the given type.
                    if ((conversion & Conversion.Checked) == Conversion.Checked)
                        return Expression.ConvertChecked(parameter.Expression, type);
                    else
                        return Expression.Convert(parameter.Expression, type);
                }
                else
                {
                    // This is the place where we have to do the conversion. 

                    // The DLR does not provide an easy helper function. So we have two options:
                    //  1. Write the logic for what converts implicitely to what (incl. dynamic cast). Too much work currently.
                    //  2. Use the C# binder to do the work. We actually WANT the same semantics as C#, so that's OK. But we depend on them :-/

                    Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags = Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None;
                    if ((conversion & Conversion.Checked) == Conversion.Checked)
                        flags = flags | Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.CheckedContext;
                    if ((conversion & Conversion.Explicit) == Conversion.Explicit)
                        flags = flags | Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.ConvertExplicit;
                    // Create a C# convert binder. Currently, this is not cached, but we could do this in the future.
                    ConvertBinder bndr = (ConvertBinder)Microsoft.CSharp.RuntimeBinder.Binder.Convert(flags, type, typeof(PrimitiveHelper));
                    DynamicMetaObject conversionResult = bndr.Bind(parameter, null);

                    if (conversionResult == null)
                        throw new InvalidOperationException("We did not expect the C# ConvertBinder.Bind to return null");

                    // Must merge the restrictions returned by the C# ConvertBinder.Bind.
                    // We will most probably have 'the same' restriction already, but the DLR will remove the duplicate
                    if (restrictions == null)
                        restrictions = BindingRestrictions.Empty;
                    if (conversionResult.Restrictions != null)
                        restrictions = restrictions.Merge(conversionResult.Restrictions);
                    // The result is easy ... the Expression return by the C# ConvertBinder.Bind
                    return conversionResult.Expression;
                }
            }
        }

        public static Expression Convert(Expression expression, Type type)
        {
            if (type == null)
                return expression;
            else if (type == typeof(object))
                return Expression.Convert(expression, typeof(object));
            else
                return Expression.ConvertChecked(expression, type);
        }

        public static string GetDefaultMemberName(Type type, Type memberType)
        {
            foreach (var member in type.GetDefaultMembers())
            {
                if (memberType.IsInstanceOfType(member))
                    return member.Name;
            }
            return null;
        }

        public static Type[] GetArgumentTypes(IEnumerable<string> parameters, Type thisType)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            // Then get type definitions for each parameter we are to pass to the member.
            bool first = true;
            List<Type> argumentTypes = new List<Type>();
            foreach(string typeName in parameters)
            {
                // Special case, for lazy people, the first parameter type can be "this", meaning the same as the defining type.
                if (first && (typeName == "this"))
                {
                    if (thisType == null)
                        throw new ArgumentNullException("thisType");
                    argumentTypes.Add(thisType);
                }
                else
                {
                    // Get the parameter type, if we fail to find one, throw an exception now! 
                    Type type = NativeTypeClassMap.GetType(typeName);
                    if (type == null)
                        throw new PrimitiveInvalidTypeException(String.Format(CodeGenerationErrors.WrongTypeName, typeName));
                    argumentTypes.Add(type);
                }
                first = false;
            }

            return argumentTypes.ToArray();
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
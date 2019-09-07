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

namespace IronSmalltalk.Common.Internal
{
    public static class TypeUtilities
    {
        private static readonly Type[] EmptyTypeArray = new Type[0];

        public static ConstructorInfo Constructor(Type type, params Type[] argumentTypes)
        {
            return TypeUtilities.Constructor(type, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy, argumentTypes);
        }

        public static ConstructorInfo Constructor(Type type, BindingFlags bindingFlags, params Type[] argumentTypes)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (argumentTypes == null)
                argumentTypes = TypeUtilities.EmptyTypeArray;
            ConstructorInfo ctor = type.GetConstructor(bindingFlags, null, argumentTypes, null);
            if (ctor == null)
                throw new MissingMemberException(type.Name, String.Format(".ctor({0})", TypeUtilities.ConcatArgumentTypes(argumentTypes)));

            return ctor;
        }

        public static FieldInfo Field(Type type, string name)
        {
            return TypeUtilities.Field(type, name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        }

        public static FieldInfo Field(Type type, string name, BindingFlags bindingFlags)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            FieldInfo field = type.GetField(name, bindingFlags);
            if (field == null)
                throw new MissingFieldException(type.Name, name);

            return field;
        }

        public static MethodInfo Method(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            MethodInfo method = type.GetMethod(name);
            if (method == null)
                throw new MissingMethodException(type.Name, String.Format("{0}(...)", name));

            return method;
        }

        public static MethodInfo Method(Type type, string name, params Type[] argumentTypes)
        {
            return TypeUtilities.Method(type, name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, argumentTypes);
        }

        public static MethodInfo Method(Type type, string name, BindingFlags bindingFlags, params Type[] argumentTypes)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (argumentTypes == null)
                argumentTypes = TypeUtilities.EmptyTypeArray;
            MethodInfo method = type.GetMethod(name, bindingFlags, null, argumentTypes, null);
            if (method == null)
                throw new MissingMethodException(type.Name, String.Format("{0}({1})", name, TypeUtilities.ConcatArgumentTypes(argumentTypes)));

            return method;
        }

        public static PropertyInfo Property(Type type, string name, params Type[] argumentTypes)
        {
            return TypeUtilities.Property(type, name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, argumentTypes);
        }

        public static PropertyInfo Property(Type type, string name, BindingFlags bindingFlags, params Type[] argumentTypes)
        {
            return TypeUtilities.Property(type, null, name, bindingFlags, argumentTypes);
        }

        public static PropertyInfo Property(Type type, Type returnType, string name, params Type[] argumentTypes)
        {
            return TypeUtilities.Property(type, returnType, name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, argumentTypes);
        }

        public static PropertyInfo Property(Type type, Type returnType, string name, BindingFlags bindingFlags, params Type[] argumentTypes)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (argumentTypes == null)
                argumentTypes = TypeUtilities.EmptyTypeArray;
            PropertyInfo property = type.GetProperty(name, bindingFlags, null, returnType, argumentTypes, null);
            if (property == null)
                throw new MissingMemberException(type.Name, String.Format("{0} {1}({2})", 
                    (returnType == null) ? "" : returnType.FullName, name, TypeUtilities.ConcatArgumentTypes(argumentTypes)));

            return property;
        }



        private static string ConcatArgumentTypes(IEnumerable<Type> types)
        {
            if (types == null)
                return "";
            return String.Join(", ", types.Select(t => (t == null) ? "" : t.FullName));
        }

    }
}

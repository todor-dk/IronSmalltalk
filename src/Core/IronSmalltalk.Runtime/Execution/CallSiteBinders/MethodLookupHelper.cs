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
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IronSmalltalk.Runtime.Behavior;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    public static class MethodLookupHelper
    {
        /// <summary>
        /// This method is the core of the dynamic method lookup system.
        /// It determines the class of an object and looks-up the method implementation
        /// for a given method selector.
        /// </summary>
        /// <param name="runtime">Required.</param>
        /// <param name="selector">Required.</param>
        /// <param name="superLookupScope">Optional.</param>
        /// <param name="receiver">Optional.</param>
        /// <param name="self">Required.</param>
        /// <param name="executionContext">Required.</param>
        /// <param name="arguments">Required.</param>
        /// <param name="receiverClass">Must Return!</param>
        /// <param name="methodClass">Return null if missing.</param>
        /// <param name="restrictions">Must Return!</param>
        /// <param name="executableCode">Return null if missing.</param>
        public static void GetMethodInformation(SmalltalkRuntime runtime, 
            Symbol selector, 
            Symbol superLookupScope, 
            object receiver,
            Expression self,
            Expression executionContext,
            IEnumerable<Expression> arguments, 
            out SmalltalkClass receiverClass,
            out SmalltalkClass methodClass,
            out BindingRestrictions restrictions, 
            out Expression executableCode)
        {
            restrictions = null;
            SmalltalkClass cls = null;

            // Special case for Smalltalk classes, because we want the class behavior first ...
            if (receiver is SmalltalkClass)
            {
                cls = (SmalltalkClass)receiver;
                if (cls.Runtime == runtime)
                {
                    // The class of a SmalltalkClass object is a special class known by the runtime.
                    receiverClass = runtime.NativeTypeClassMap.Class;
                    if (receiverClass == null)
                        // And in case it's not set (not normal), we default to object
                        receiverClass = runtime.NativeTypeClassMap.Object;
                    // Lookup method in class behavior
                    CompiledMethod mth = MethodLookupHelper.LookupClassMethod(selector, ref cls, superLookupScope);
                    if (mth != null)
                    {
                        // A class method, special restrictions
                        methodClass = cls;
                        restrictions = BindingRestrictions.GetInstanceRestriction(self, receiver);
                        executableCode = mth.GetExpression(self, executionContext, arguments);
                        if (executableCode != null)
                            return;
                    }
                    // Not in class behavior ... fallback to instance / Object behavior
                    cls = receiverClass;
                    superLookupScope = null;
                    // IMPROVE: May need to add Runtime Restrictions too.
                    restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(SmalltalkClass));
                }
            }

            if ((cls == null) || (restrictions == null)) // Will not run for receivers of type SmalltalkClass
                cls = MethodLookupHelper.GetClassAndRestrictions(runtime, receiver, self, arguments, out restrictions);
            receiverClass = cls;

            // Look-up the method
            CompiledMethod method = MethodLookupHelper.LookupInstanceMethod(selector, ref cls, superLookupScope);
            methodClass = cls;
            if (method == null)
                executableCode = null;
            else
                executableCode = method.GetExpression(self, executionContext, arguments);
        }

        /// <summary>
        /// Look-up for an instance method implementation given a method selector and a class.
        /// </summary>
        /// <param name="selector">Method selector to look for.</param>
        /// <param name="cls">Class where to start searching for the method (unless superLookupScope) is set.</param>
        /// <param name="superLookupScope">If set, start the lookup from the superclass of this class.</param>
        /// <returns>Returns the compiled method for the given selector or null if none was found.</returns>
        public static CompiledMethod LookupInstanceMethod(Symbol selector, ref SmalltalkClass cls, Symbol superLookupScope)
        {
            return MethodLookupHelper.LookupMethod(ref cls, superLookupScope, c =>
            {
                CompiledMethod method;
                if (c.InstanceBehavior.TryGetValue(selector, out method))
                    return method;
                return null;
            });
        }

        /// <summary>
        /// Look-up for a class method implementation given a method selector and a class.
        /// </summary>
        /// <param name="selector">Method selector to look for.</param>
        /// <param name="cls">Class where to start searching for the method (unless superLookupScope) is set.</param>
        /// <param name="superLookupScope">If set, start the lookup from the superclass of this class.</param>
        /// <returns>Returns the compiled method for the given selector or null if none was found.</returns>
        /// <remarks>If the method is not found, this functions does not searches the instance side of the class.</remarks>
        public static CompiledMethod LookupClassMethod(Symbol selector, ref SmalltalkClass cls, Symbol superLookupScope)
        {
            return MethodLookupHelper.LookupMethod(ref cls, superLookupScope, c =>
            {
                CompiledMethod method;
                if (c.ClassBehavior.TryGetValue(selector, out method))
                    return method;
                return null;
            });
        }

        /// <summary>
        /// Look-up a method implementation starting with the given class.
        /// </summary>
        /// <param name="cls">Class where to start searching for the method (unless superLookupScope) is set.</param>
        /// <param name="superLookupScope">If set, start the lookup from the superclass of this class.</param>
        /// <param name="lookupFunction">Function to perform the method lookup.</param>
        /// <returns>Returns the compiled method for the given selector or null if none was found.</returns>
        public static CompiledMethod LookupMethod(ref SmalltalkClass cls, Symbol superLookupScope, Func<SmalltalkClass, CompiledMethod> lookupFunction)
        {
            if (lookupFunction == null)
                throw new ArgumentNullException(nameof(lookupFunction));

            while (cls != null)
            {
                if (superLookupScope == null)
                {
                    CompiledMethod method = lookupFunction(cls);
                    if (method != null)
                        return method;
                }
                else
                {
                    if (cls.Name == superLookupScope)
                        superLookupScope = null;
                }
                cls = cls.Superclass;
            }

            // No method ... no luck;
            return null;
        }

        /// <summary>
        /// This core method determines the class of an object.
        /// </summary>
        /// <param name="runtime">Required: SmalltalkRuntime containing the Smalltalk classes.</param>
        /// <param name="receiver">Optional: Object whose class is to be determined.</param>
        /// <param name="self">Required: Expression for the receiver.</param>
        /// <param name="arguments">Required: Currently not used.</param>
        /// <param name="restrictions">Restrictions for the given receiver.</param>
        /// <returns>The SmalltalkClass for the given receiver. This always returns an object (unless the given SmalltalkRuntime is inconsistent).</returns>
        public static SmalltalkClass GetClassAndRestrictions(SmalltalkRuntime runtime, 
            object receiver,
            Expression self,
            IEnumerable<Expression> arguments, 
            out BindingRestrictions restrictions)
        {
            SmalltalkClass cls;
            // Special case handling of null, so it acts like first-class-object.
            if (receiver == null)
            {
                cls = runtime.NativeTypeClassMap.UndefinedObject;
                // If not explicitly mapped to a ST Class, fallback to the generic .Net mapping class.
                if (cls == null)
                    cls = runtime.NativeTypeClassMap.Native;
                if (cls == null)
                    cls = runtime.NativeTypeClassMap.Object;
                restrictions = BindingRestrictions.GetInstanceRestriction(self, null);
            }
            // Smalltalk objects ... almost every objects ends up here.
            else if (receiver is SmalltalkObject)
            {
                SmalltalkObject obj = (SmalltalkObject)receiver;
                cls = obj.Class;
                if (cls.Runtime == runtime)
                {
                    // Restrictions look like:
                    // (self != null) && (self.GetType == typeof(SmalltalkObject) && (((SmalltalkObject)self).Class == cls)
                    restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(SmalltalkObject));
                    restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
                        Expression.ReferenceEqual(Expression.Field(Expression.Convert(self, typeof(SmalltalkObject)), SmalltalkObject.ClassField), Expression.Constant(cls))));
                }
                else
                {
                    // A smalltalk object, but from different runtime
                    cls = null; // Let block below handle this.
                    restrictions = null;
                }
            }
            // Special case for Symbol objects
            else if (receiver is Symbol)
            {
                Symbol symbol = (Symbol)receiver;
                SymbolTable manager = symbol.Manager;
                if (manager.Runtime == runtime)
                {
                    cls = runtime.NativeTypeClassMap.Symbol;
                    // Restrictions look like:
                    // (self != null) && (self.GetType == typeof(Symbol) && (((Symbol)self).Manager == manager)
                    restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Symbol));
                    restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
                        Expression.ReferenceEqual(Expression.Field(Expression.Convert(self, typeof(Symbol)), Symbol.ManagerField), Expression.Constant(manager))));
                }
                else
                {
                    // A smalltalk object, but from different runtime
                    cls = null; // Let block below handle this.
                    restrictions = null;
                }
            }
            // Special case for Pool objects
            else if (receiver is Pool)
            {
                Pool pool = (Pool)receiver;

                if (pool.Runtime == runtime)
                {
                    cls = runtime.NativeTypeClassMap.Pool;
                    // Restrictions look like:
                    // (self != null) && (self.GetType == typeof(Pool) && (((Pool)self).Runtime == runtime)
                    restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(Pool));
                    restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(
                        Expression.ReferenceEqual(Expression.Property(Expression.Convert(self, typeof(Pool)), Pool.RuntimeProperty), Expression.Constant(runtime))));
                }
                else
                {
                    // A smalltalk object, but from different runtime
                    cls = null; // Let block below handle this.
                    restrictions = null;
                }
            }
            // Common FCL type mapping (bool, int, string, etc) to first-class-object.
            else if (receiver is bool)
            {
                Expression restrictionTest;
                if ((bool)receiver)
                {
                    cls = runtime.NativeTypeClassMap.True;
                    restrictionTest = Expression.IsTrue(Expression.Convert(self, typeof(bool)));
                }
                else
                {
                    cls = runtime.NativeTypeClassMap.False;
                    restrictionTest = Expression.IsFalse(Expression.Convert(self, typeof(bool)));
                }
                // Restrictions look like:
                // (self != null) && (self.GetType == typeof(Boolean) && ((Boolean)self)
                // (self != null) && (self.GetType == typeof(Boolean) && !((Boolean)self)
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(bool))
                    .Merge(BindingRestrictions.GetExpressionRestriction(restrictionTest));
            }
            else if (receiver is int)
            {
                cls = runtime.NativeTypeClassMap.SmallInteger;
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(int));
            }
            else if (receiver is string)
            {
                cls = runtime.NativeTypeClassMap.String;
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(string));
            }
            else if (receiver is char)
            {
                cls = runtime.NativeTypeClassMap.Character;
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(char));
            }
            else if (receiver is double)
            {
                cls = runtime.NativeTypeClassMap.Float;
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(double));
            }
            else if (receiver is decimal)
            {
                cls = runtime.NativeTypeClassMap.SmallDecimal;
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(decimal));
            }
            else if (receiver is System.Numerics.BigInteger)
            {
                cls = runtime.NativeTypeClassMap.BigInteger;
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(System.Numerics.BigInteger));
            }
            else if (receiver is IronSmalltalk.Common.BigDecimal)
            {
                cls = runtime.NativeTypeClassMap.BigDecimal;
                restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(IronSmalltalk.Common.BigDecimal));
            }
            // Special case for Smalltalk classes, because we want the class behavior ...
            else if (receiver is SmalltalkClass)
            {
                cls = (SmalltalkClass)receiver;
                if (cls.Runtime == runtime)
                {
                    cls = runtime.NativeTypeClassMap.Class;
                    if (cls == null)
                        cls = runtime.NativeTypeClassMap.Object;

                    if (cls == null)
                        restrictions = null;
                    else
                        // NB: Restriction below are no good for For class behavior. Caller must create own restrictions if class behavior.
                        restrictions = BindingRestrictions.GetTypeRestriction(self, typeof(SmalltalkClass)); 
                }
                else
                {
                    // A smalltalk object, but from different runtime
                    cls = null; // Let block below handle this.
                    restrictions = null;
                }
            }
            // Some .Net type that's neither IronSmalltalk object nor any on the known (hardcoded) types.
            else
            {
                cls = null; // Let block below handle this.
                restrictions = null;
            }

            // In case of any of the known (hardcoded) types has no registered Smalltalk class, 
            // fallback to the generic .Net type to Smalltalk class mapping.
            if (cls != null)
            {
                return cls;
            }
            else
            {
                Debug.Assert(receiver != null, "receiver != null");
                Type type = receiver.GetType();
                cls = runtime.NativeTypeClassMap.GetSmalltalkClass(type);
                // If not explicitly mapped to a ST Class, fallback to the generic .Net mapping class.
                if (cls == null)
                    cls = runtime.NativeTypeClassMap.Native;
                if (restrictions == null)
                    restrictions = BindingRestrictions.GetTypeRestriction(self, type);
                return cls;
            }
        }
    }
}

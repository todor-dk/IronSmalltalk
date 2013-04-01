/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Utils;

namespace Microsoft.Scripting.Runtime {
    /// <summary>
    /// Used as the value for the ScriptingRuntimeHelpers.GetDelegate method caching system
    /// </summary>
    public sealed class DelegateInfo {
        private const int TargetIndex = 0;
        private const int CallSiteIndex = 1;
        private const int ConvertSiteIndex = 2;
        private static readonly object TargetPlaceHolder = new object();
        private static readonly object CallSitePlaceHolder = new object();
        private static readonly object ConvertSitePlaceHolder = new object();

        // to enable:
        // function x() { }
        // someClass.someEvent += delegateType(x) 
        // someClass.someEvent -= delegateType(x) 
        //
        // We need to avoid re-creating the closure because the delegates won't
        // compare equal when removing the delegate if they have different closure
        // instances.  Therefore we use a weak hashtable to get back the
        // original closure. The closures also need to be held via a weak refererence to avoid
        // creating a circular reference from the constants target back to the
        // target. This is fine because as long as the delegate is referenced
        // the object array will stay alive.  Once the delegate is gone it's not
        // wired up anywhere and -= will never be used again.
        //
        // Note that the closure content depends on the signature of the delegate. So a single dynamic object
        // might need multiple closures if it is converted to delegates of different signatures.
        private WeakDictionary<object, WeakReference> _closureMap = new WeakDictionary<object, WeakReference>();
        
        private readonly Type _returnType;
        private readonly Type[] _parameterTypes;
        private readonly MethodInfo _method;
        private readonly InvokeBinder _invokeBinder;
        private readonly ConvertBinder _convertBinder;

        public DelegateInfo(LanguageContext context, Type returnType, Type[] parameters) {
            Assert.NotNull(returnType);
            Assert.NotNullItems(parameters);

            _returnType = returnType;
            _parameterTypes = parameters;

            PerfTrack.NoteEvent(PerfTrack.Categories.DelegateCreate, ToString());

            if (_returnType != typeof(void)) {
                _convertBinder = context.CreateConvertBinder(_returnType, true);
            }

            _invokeBinder = context.CreateInvokeBinder(new CallInfo(_parameterTypes.Length));

            Type[] delegateParams = new Type[1 + _parameterTypes.Length];
            delegateParams[0] = typeof(object[]);
            for (int i = 0; i < _parameterTypes.Length; i++) {
                delegateParams[1 + i] = _parameterTypes[i];
            }

            EmitClrCallStub(returnType, delegateParams, out _method);
        }

        public Delegate CreateDelegate(Type delegateType, object dynamicObject) {
            Assert.NotNull(delegateType, dynamicObject);

            object[] closure;            
            lock (_closureMap) {
                WeakReference weakClosure;

                if (!_closureMap.TryGetValue(dynamicObject, out weakClosure) || (closure = (object[])weakClosure.Target) == null) {

                    closure = new[] { TargetPlaceHolder, CallSitePlaceHolder, ConvertSitePlaceHolder };
                    _closureMap[dynamicObject] = new WeakReference(closure);

                    Type[] siteTypes = MakeSiteSignature(_parameterTypes);

                    CallSite callSite = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(siteTypes), _invokeBinder);

                    CallSite convertSite = null;
                    if (_returnType != typeof(void)) {
                        convertSite = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(typeof(object), _returnType), _convertBinder);
                    }

                    closure[TargetIndex] = dynamicObject;
                    closure[CallSiteIndex] = callSite;
                    closure[ConvertSiteIndex] = convertSite;
                }
            }

            return _method.CreateDelegate(delegateType, closure);
        }

        private void EmitClrCallStub(Type returnType, Type[] parameterTypes, out MethodInfo method) {
            // Create the method with a special name so the langauge compiler knows that method's stack frame is not visible
            DynamicILGen cg = Snippets.Shared.CreateDynamicMethod("_Scripting_", returnType, parameterTypes, false);
            EmitClrCallStub(cg);
            method = cg.Finish();
        }

        /// <summary>
        /// Generates stub to receive the CLR call and then call the dynamic language code.
        /// </summary>
        private void EmitClrCallStub(ILGen cg) {

            List<ReturnFixer> fixers = new List<ReturnFixer>(0);
            // Create strongly typed return type from the site.
            // This will, among other things, generate tighter code.
            Type[] siteTypes = MakeSiteSignature(_parameterTypes);

            CallSite callSite = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(siteTypes), _invokeBinder);
            Type siteType = callSite.GetType();

            Type convertSiteType = null;
            CallSite convertSite = null;

            if (_returnType != typeof(void)) {
                convertSite = CallSite.Create(DynamicSiteHelpers.MakeCallSiteDelegate(typeof(object), _returnType), _convertBinder);
                convertSiteType = convertSite.GetType();
            }

            LocalBuilder convertSiteLocal = null;
            FieldInfo convertTarget = null;
            if (_returnType != typeof(void)) {
                // load up the conversion logic on the stack
                convertSiteLocal = cg.DeclareLocal(convertSiteType);
                EmitConstantGet(cg, ConvertSiteIndex, convertSiteType);

                cg.Emit(OpCodes.Dup);
                cg.Emit(OpCodes.Stloc, convertSiteLocal);

                convertTarget = convertSiteType.GetDeclaredField("Target");
                cg.EmitFieldGet(convertTarget);
                cg.Emit(OpCodes.Ldloc, convertSiteLocal);
            }

            // load up the invoke logic on the stack
            LocalBuilder site = cg.DeclareLocal(siteType);
            EmitConstantGet(cg, CallSiteIndex, siteType);
            cg.Emit(OpCodes.Dup);
            cg.Emit(OpCodes.Stloc, site);

            FieldInfo target = siteType.GetDeclaredField("Target");
            cg.EmitFieldGet(target);
            cg.Emit(OpCodes.Ldloc, site);

            EmitConstantGet(cg, TargetIndex, typeof(object));

            for (int i = 0; i < _parameterTypes.Length; i++) {
                if (_parameterTypes[i].IsByRef) {
                    ReturnFixer rf = ReturnFixer.EmitArgument(cg, i + 1, _parameterTypes[i]);
                    if (rf != null) fixers.Add(rf);
                } else {
                    cg.EmitLoadArg(i + 1);
                }
            }

            // emit the invoke for the call
            cg.EmitCall(target.FieldType, "Invoke");

            // emit the invoke for the convert
            if (_returnType == typeof(void)) {
                cg.Emit(OpCodes.Pop);
            } else {
                cg.EmitCall(convertTarget.FieldType, "Invoke");
            }

            // fixup any references
            foreach (ReturnFixer rf in fixers) {
                rf.FixReturn(cg);
            }

            cg.Emit(OpCodes.Ret);
        }

        private static void EmitConstantGet(ILGen il, int index, Type type) {
            il.Emit(OpCodes.Ldarg_0);
            il.EmitInt(index);
            il.Emit(OpCodes.Ldelem_Ref);
            if (type != typeof(object)) {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static Type[] MakeSiteSignature(Type[] parameterTypes) {
            Type[] sig = new Type[parameterTypes.Length + 2];

            // target object
            sig[0] = typeof(object);

            // arguments
            for (int i = 0; i < parameterTypes.Length; i++) {
                if (parameterTypes[i].IsByRef) {
                    sig[i + 1] = typeof(object);
                } else {
                    sig[i + 1] = parameterTypes[i];
                }
            }

            // return type
            sig[sig.Length - 1] = typeof(object);

            return sig;
        }
    }
}

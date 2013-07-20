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
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronSmalltalk.Runtime.Execution.Internals;

namespace IronSmalltalk.Runtime.Execution.CallSiteBinders
{
    /// <summary>
    /// Base class for the dynamic message send binder. 
    /// This binder is responsible for binding the operationgs for message sends.
    /// </summary>
    public abstract class MessageSendCallSiteBinderBase : SmalltalkDynamicMetaObjectBinder
    {
        /// <summary>
        /// Selector of the message being sent.
        /// </summary>
        /// <remarks>
        /// We keep the selector as string, because we want the binder to be Runtime neutral.
        /// </remarks>
        public string Selector { get; private set; }

        /// <summary>
        /// Create a new MessageSendCallSiteBinderBase.
        /// </summary>
        /// <param name="selector">Selector of the message being sent.</param>
        protected MessageSendCallSiteBinderBase(string selector)
            : base()
        {
            if (String.IsNullOrWhiteSpace(selector))
                throw new ArgumentNullException("selector");
            this.Selector = selector;
        }

        /// <summary>
        /// Performs the binding of the dynamic operation.
        /// </summary>
        /// <param name="target">The target of the dynamic operation.</param>
        /// <param name="args">An array of arguments of the dynamic operation.</param>
        /// <returns>The System.Dynamic.DynamicMetaObject representing the result of the binding.</returns>
        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ExecutionContext executionContext = this.GetExecutionContext(args);
            if (executionContext == null)
                // If this is null, the binder was not used by a Smalltalk method. Or may be somebody passed null for the ExecutionContext, which is illegal too.
                throw new ArgumentException("The MessageSendCallSiteBinderBase can only be used in methods where the signature is (object, ExecutionContext, ...)");
            SmalltalkRuntime runtime = executionContext.Runtime;

            // Look-up the method implementation for the given selector 
            // and generate an expression (runtime code) for that method.
            // This also returns the restriction needed for the PIC.
            BindingRestrictions restrictions;
            SmalltalkClass receiverClass;
            SmalltalkClass methodClass;
            Expression expression;
            string superScope = this.GetSuperLookupScope();
            MethodLookupHelper.GetMethodInformation(runtime,
                runtime.GetSymbol(this.Selector),
                (superScope == null) ? null : runtime.GetSymbol(superScope),
                target.Value,
                target.Expression,
                args[0].Expression,
                args.Skip(1).Select(dmo => dmo.Expression),
                out receiverClass,
                out methodClass,
                out restrictions,
                out expression);

            // If no code was generated, method is missing, and we must do #doesNotUnderstand. 
            if (expression == null)
            {
                // When the regular lookup experience a message send that the receiver does not understand, 
                // we auto-generate an expression to send the #_doesNotUnderstand:arguments: to the receiver.  
                // 
                // Every object in the system must implement #_doesNotUnderstand:arguments:!
                //
                // Example:
                //     tmp := anObject invalidMessageWith: arg1 with: arg2 with: arg3.
                //     
                // We auto-generate a code to:
                //     tmp := anObject _doesNotUnderstand: #invalidMessageWith:with:with:
                //         arguments: (Array with: arg1 with: arg2 with: arg3).
                // 
                Expression[] dnuArgs = new Expression[] 
                {
                    Expression.Constant(runtime.GetSymbol(this.Selector), typeof(object)), // The selecor
                    Expression.NewArrayInit(typeof(object), args.Skip(1).Select(d => Expression.Convert(d.Expression, typeof(object)))) // The remeaining args as an array
                };
                // Look-up the #_doesNotUnderstand:arguments: method.
                MethodLookupHelper.GetMethodInformation(runtime,
                    runtime.GetSymbol("_doesNotUnderstand:arguments:"),
                    null,
                    target.Value,
                    target.Expression,
                    args[0].Expression,
                    dnuArgs,
                    out receiverClass,
                    out methodClass,
                    out restrictions,
                    out expression);

                // Every class is supposed to implement the #_doesNotUnderstand:arguments:, if not, throw a runtime exception
                if (expression == null)
                    throw new CodeGenerationException(RuntimeErrors.DoesNotUnderstandMissing);
            }

            // Return a DynamicMetaObject with the generated code.
            // Important here are the restrictions, which ensure that as long as <self> is 
            // of the correct type, we can freely execute the code for the method we've just found.
            return new DynamicMetaObject(expression, target.Restrictions.Merge(restrictions));
        }

        protected ExecutionContext GetExecutionContext(DynamicMetaObject[] args)
        {
            if ((args == null) || (args.Length < 1))
                return null;
            return args[0].Value as ExecutionContext;
        }

        /// <summary>
        /// For super sends, return the name of the class ABOVE which to start the method lookup.
        /// </summary>
        /// <returns>Return a class name or null to start the method lookup immedeately.</returns>
        protected virtual string GetSuperLookupScope()
        {
            return null;
        }
    }
}

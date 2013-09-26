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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Execution.Internals
{

    public static class MethodSignatures
    {
        /// <summary>
        /// The maximum number of arguments that our implementation supports.
        /// </summary>
        public static readonly int MaxArgumentCount = 32;

        /// <summary>
        /// Returns the delegate type (method signature) for a Smalltalk method with the given argument count.
        /// </summary>
        /// <param name="argumentCount">Number of arguments for the method.</param>
        /// <returns>The delegate type (method signature) for a Smalltalk method with the given argument count.</returns>
        public static Type GetMethodType(int argumentCount)
        {
            switch (argumentCount)
            {
                case 0:
                    return typeof(SmalltalkMethod0);
                case 1:
                    return typeof(SmalltalkMethod1);
                case 2:
                    return typeof(SmalltalkMethod2);
                case 3:
                    return typeof(SmalltalkMethod3);
                case 4:
                    return typeof(SmalltalkMethod4);
                case 5:
                    return typeof(SmalltalkMethod5);
                case 6:
                    return typeof(SmalltalkMethod6);
                case 7:
                    return typeof(SmalltalkMethod7);
                case 8:
                    return typeof(SmalltalkMethod8);
                case 9:
                    return typeof(SmalltalkMethod9);
                case 10:
                    return typeof(SmalltalkMethod10);
                case 11:
                    return typeof(SmalltalkMethod11);
                case 12:
                    return typeof(SmalltalkMethod12);
                case 13:
                    return typeof(SmalltalkMethod13);
                case 14:
                    return typeof(SmalltalkMethod14);
                case 15:
                    return typeof(SmalltalkMethod15);
                case 16:
                    return typeof(SmalltalkMethod16);
                case 17:
                    return typeof(SmalltalkMethod17);
                case 18:
                    return typeof(SmalltalkMethod18);
                case 19:
                    return typeof(SmalltalkMethod19);
                case 20:
                    return typeof(SmalltalkMethod20);
                case 21:
                    return typeof(SmalltalkMethod21);
                case 22:
                    return typeof(SmalltalkMethod22);
                case 23:
                    return typeof(SmalltalkMethod23);
                case 24:
                    return typeof(SmalltalkMethod24);
                case 25:
                    return typeof(SmalltalkMethod25);
                case 26:
                    return typeof(SmalltalkMethod26);
                case 27:
                    return typeof(SmalltalkMethod27);
                case 28:
                    return typeof(SmalltalkMethod28);
                case 29:
                    return typeof(SmalltalkMethod29);
                case 30:
                    return typeof(SmalltalkMethod30);
                case 31:
                    return typeof(SmalltalkMethod31);
                case 32:
                    return typeof(SmalltalkMethod32);
                default:
                    throw new ImplementationLimitationException(
                        String.Format(RuntimeErrors.TooManyMethodArguments, argumentCount, MethodSignatures.MaxArgumentCount));
            }
        }

        /**** Smalltalk code for generating the method signatures below ****
         
        str := WriteStream on: String new.

        0 to: 32 do: [ :i |
            str nextPutAll: '/// <summary>'; cr.
            str nextPutAll: '/// Signature for a Smalltalk method with '; nextPutAll: i asString; nextPutAll: ' arguments.'; cr.
            str nextPutAll: '/// </summary>'; cr.
            str nextPutAll: '/// <param name="self">Receiver.</param>'; cr.
            str nextPutAll: '/// <param name="executionContext">Execution context.</param>'; cr.
            1 to: i do: [ :j |
                str nextPutAll: '/// <param name="arg1">Argument '; nextPutAll: j asString; nextPutAll: '.</param>'; cr.
            ].
            str nextPutAll: '/// <returns>Return value of the method.</returns>'; cr.

            str nextPutAll: 'public delegate object SmalltalkMethod'; nextPutAll: i asString; nextPutAll: '(object self, ExecutionContext executionContext'.
            1 to: i do: [ :j | str nextPutAll: ', object arg'; nextPutAll: j asString ].
            str nextPutAll: ');'; cr; cr.
        ].
	
        str contents.
        
        */

        /// <summary>
        /// Signature for a Smalltalk method with 0 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod0(object self, ExecutionContext executionContext);

        /// <summary>
        /// Signature for a Smalltalk method with 1 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod1(object self, ExecutionContext executionContext, object arg1);

        /// <summary>
        /// Signature for a Smalltalk method with 2 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod2(object self, ExecutionContext executionContext, object arg1, object arg2);

        /// <summary>
        /// Signature for a Smalltalk method with 3 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod3(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3);

        /// <summary>
        /// Signature for a Smalltalk method with 4 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod4(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4);

        /// <summary>
        /// Signature for a Smalltalk method with 5 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod5(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5);

        /// <summary>
        /// Signature for a Smalltalk method with 6 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod6(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6);

        /// <summary>
        /// Signature for a Smalltalk method with 7 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod7(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7);

        /// <summary>
        /// Signature for a Smalltalk method with 8 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod8(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8);

        /// <summary>
        /// Signature for a Smalltalk method with 9 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod9(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9);

        /// <summary>
        /// Signature for a Smalltalk method with 10 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod10(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10);

        /// <summary>
        /// Signature for a Smalltalk method with 11 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod11(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11);

        /// <summary>
        /// Signature for a Smalltalk method with 12 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod12(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12);

        /// <summary>
        /// Signature for a Smalltalk method with 13 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod13(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13);

        /// <summary>
        /// Signature for a Smalltalk method with 14 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod14(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14);

        /// <summary>
        /// Signature for a Smalltalk method with 15 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod15(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15);

        /// <summary>
        /// Signature for a Smalltalk method with 16 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod16(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16);

        /// <summary>
        /// Signature for a Smalltalk method with 17 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod17(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17);

        /// <summary>
        /// Signature for a Smalltalk method with 18 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod18(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18);

        /// <summary>
        /// Signature for a Smalltalk method with 19 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod19(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19);

        /// <summary>
        /// Signature for a Smalltalk method with 20 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod20(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20);

        /// <summary>
        /// Signature for a Smalltalk method with 21 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod21(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21);

        /// <summary>
        /// Signature for a Smalltalk method with 22 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod22(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22);

        /// <summary>
        /// Signature for a Smalltalk method with 23 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod23(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23);

        /// <summary>
        /// Signature for a Smalltalk method with 24 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod24(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24);

        /// <summary>
        /// Signature for a Smalltalk method with 25 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod25(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25);

        /// <summary>
        /// Signature for a Smalltalk method with 26 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <param name="arg1">Argument 26.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod26(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26);

        /// <summary>
        /// Signature for a Smalltalk method with 27 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <param name="arg1">Argument 26.</param>
        /// <param name="arg1">Argument 27.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod27(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27);

        /// <summary>
        /// Signature for a Smalltalk method with 28 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <param name="arg1">Argument 26.</param>
        /// <param name="arg1">Argument 27.</param>
        /// <param name="arg1">Argument 28.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod28(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28);

        /// <summary>
        /// Signature for a Smalltalk method with 29 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <param name="arg1">Argument 26.</param>
        /// <param name="arg1">Argument 27.</param>
        /// <param name="arg1">Argument 28.</param>
        /// <param name="arg1">Argument 29.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod29(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29);

        /// <summary>
        /// Signature for a Smalltalk method with 30 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <param name="arg1">Argument 26.</param>
        /// <param name="arg1">Argument 27.</param>
        /// <param name="arg1">Argument 28.</param>
        /// <param name="arg1">Argument 29.</param>
        /// <param name="arg1">Argument 30.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod30(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29, object arg30);

        /// <summary>
        /// Signature for a Smalltalk method with 31 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <param name="arg1">Argument 26.</param>
        /// <param name="arg1">Argument 27.</param>
        /// <param name="arg1">Argument 28.</param>
        /// <param name="arg1">Argument 29.</param>
        /// <param name="arg1">Argument 30.</param>
        /// <param name="arg1">Argument 31.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod31(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29, object arg30, object arg31);

        /// <summary>
        /// Signature for a Smalltalk method with 32 arguments.
        /// </summary>
        /// <param name="self">Receiver.</param>
        /// <param name="executionContext">Execution context.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg1">Argument 2.</param>
        /// <param name="arg1">Argument 3.</param>
        /// <param name="arg1">Argument 4.</param>
        /// <param name="arg1">Argument 5.</param>
        /// <param name="arg1">Argument 6.</param>
        /// <param name="arg1">Argument 7.</param>
        /// <param name="arg1">Argument 8.</param>
        /// <param name="arg1">Argument 9.</param>
        /// <param name="arg1">Argument 10.</param>
        /// <param name="arg1">Argument 11.</param>
        /// <param name="arg1">Argument 12.</param>
        /// <param name="arg1">Argument 13.</param>
        /// <param name="arg1">Argument 14.</param>
        /// <param name="arg1">Argument 15.</param>
        /// <param name="arg1">Argument 16.</param>
        /// <param name="arg1">Argument 17.</param>
        /// <param name="arg1">Argument 18.</param>
        /// <param name="arg1">Argument 19.</param>
        /// <param name="arg1">Argument 20.</param>
        /// <param name="arg1">Argument 21.</param>
        /// <param name="arg1">Argument 22.</param>
        /// <param name="arg1">Argument 23.</param>
        /// <param name="arg1">Argument 24.</param>
        /// <param name="arg1">Argument 25.</param>
        /// <param name="arg1">Argument 26.</param>
        /// <param name="arg1">Argument 27.</param>
        /// <param name="arg1">Argument 28.</param>
        /// <param name="arg1">Argument 29.</param>
        /// <param name="arg1">Argument 30.</param>
        /// <param name="arg1">Argument 31.</param>
        /// <param name="arg1">Argument 32.</param>
        /// <returns>Return value of the method.</returns>
        public delegate object SmalltalkMethod32(object self, ExecutionContext executionContext, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29, object arg30, object arg31, object arg32);

    }
}

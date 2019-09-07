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
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Primitives.Exceptions;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime.Internal;

namespace IronSmalltalk.ExpressionCompiler.Primitives
{
    public abstract class MemberPrimitiveEncoder : PrimitiveEncoder
    {
        public Type DefiningType { get; private set; }

        public MemberPrimitiveEncoder(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType)
            : base(visitor, parameters)
        {
            if (definingType == null)
                throw new ArgumentNullException("definingType");
            this.DefiningType = definingType;
        }

        protected Type[] GetArgumentTypes()
        {
            return this.GetArgumentTypes(this.Parameters);
        }

        protected Type[] GetArgumentTypes(IEnumerable<string> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            // Then get type definitions for each parameter we are to pass to the member.
            bool first = true;
            List<Type> argumentTypes = new List<Type>();
            foreach (string typeName in parameters)
            {
                // Special case, for lazy people, the first parameter type can be "this", meaning the same as the defining type.
                if (first && (typeName == "this"))
                {
                    if (this.DefiningType == null)
                        throw new ArgumentNullException("this.DefiningType");
                    argumentTypes.Add(this.DefiningType);
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

    public abstract class NamedMemberPrimitiveEncoder : MemberPrimitiveEncoder
    {
        public string MemberName { get; private set; }

        public NamedMemberPrimitiveEncoder(PrimitiveCallVisitor visitor, IEnumerable<string> parameters, Type definingType, string memberName)
            : base(visitor, parameters, definingType)
        {
            if (String.IsNullOrWhiteSpace(memberName))
                throw new ArgumentNullException("memberName");
            this.MemberName = memberName;
        }
    }
}

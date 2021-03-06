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
using System.Reflection;
using IronSmalltalk.Common.Internal;

namespace IronSmalltalk.Runtime.Internal
{
    /// <summary>
    /// Home context of a block closure.
    /// </summary>
    /// <remarks>
    /// The home context is an object used as a marker to identify the unique method activation.
    /// </remarks>
    public class HomeContext : Object
    {
        /// <summary>
        /// Create a new HomeContext.
        /// </summary>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public HomeContext()
        {

        }
    }

    /// <summary>
    /// Result of a non-local return of a block closure.
    /// </summary>
    /// <remarks>
    /// A block closure with explicit (non-local) return throws an instance of BlockResult.
    /// The BlockResult contains the actual result and the HomeContext of the block.
    /// </remarks>
    public class BlockResult //: Exception
    {
        /// <summary>
        /// HomeContext identifying which method activation created the block.
        /// </summary>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public readonly HomeContext HomeContext;

        /// <summary>
        /// Value being returned.
        /// </summary>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public object Value;

        /// <summary>
        /// Internal. The FieldInfo of the BlockResult.Value field.
        /// </summary>
        public static readonly FieldInfo ValueField = TypeUtilities.Field(typeof(BlockResult), "Value");

        /// <summary>
        /// Internal. The FieldInfo of the BlockResult.HomeContext field.
        /// </summary>
        public static readonly FieldInfo HomeContextField = TypeUtilities.Field(typeof(BlockResult), "HomeContext");

        /// <summary>
        /// Internal. The ConstructorInfo of the BlockResult.ctor() constructor.
        /// </summary>
        public static readonly ConstructorInfo ConstructorInfo = TypeUtilities.Constructor(typeof(BlockResult), typeof(HomeContext), typeof(object));

        /// <summary>
        /// Create a new block result.
        /// </summary>
        /// <param name="homeContext">HomeContext identifying which method activation created the block.</param>
        /// <param name="value">Value being returned.</param>
        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public BlockResult(HomeContext homeContext, object value)
            //: base("Method has already returned")
        {
#if DEBUG
            if (homeContext == null)
                throw new ArgumentNullException("homeContext");
#endif
            this.HomeContext = homeContext;
            this.Value = value;
        }
    }
}

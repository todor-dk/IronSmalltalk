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
using IronSmalltalk.Hosting.Hosting;
using IronSmalltalk.Runtime.Hosting;

namespace IronSmalltalk.Hosting
{
    /// <summary>
    /// This usility class contains information about the IronSmalltalk language.
    /// </summary>
    public static class SmalltalkLanguageSetup
    {
        /// <summary>
        /// The Type that implements the LanguageContext for the IronSmalltalk language.
        /// </summary>
        public static readonly Type LanguageContextType = typeof(SmalltalkLanguageContext);

        /// <summary>
        /// The display name for the IronSmalltalk language.
        /// </summary>
        public static readonly string DisplayName = "IronSmalltalk";

        /// <summary>
        /// Secondary display names for the IronSmalltalk language.
        /// </summary>
        public static readonly string[] SecondaryNames = new string[] { "IronSmalltalk", "Iron Smalltalk", "ist" };

        /// <summary>
        /// File extensions used by IronSmalltalk files. 
        /// </summary>
        public static readonly string[] FileExtensions = new string[] { "ist" };

        /// <summary>
        /// The version of this IronSmalltalk implementation
        /// </summary>
        /// <remarks>
        /// Currently, we use the assembly version.
        /// </remarks>
        public static readonly Version Version = typeof(SmalltalkLanguageContext).Assembly.GetName().Version;

        /// <summary>
        /// The Guid that uniqualy identifies the IronSmalltalk language
        /// </summary>
        public static readonly Guid LanguageGuid = new Guid("E9653E63-112C-4865-A7F4-CDB8800C0E6E");

        /// <summary>
        /// The Guid that uniqualy identifies the vendor of the IronSmalltalk language, 
        /// i.e. that is the IronSmalltalk Project (because we don't do other languages).
        /// </summary>
        public static readonly Guid VendorGuid = new Guid("621750CA-32CD-4FC2-AEE5-B148B0D853D2");

    }
}

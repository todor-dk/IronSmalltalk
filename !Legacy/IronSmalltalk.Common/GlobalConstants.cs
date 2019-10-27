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
namespace IronSmalltalk.Common
{
    /// <summary>
    /// Class containing global constants.
    /// </summary>
    public static class GlobalConstants
    {
        /// <summary>
        /// Returns the names of the reserved identifiers (nil, true, false, self and super).
        /// </summary>
        public static readonly string[] ReservedIdentifiers = new string[] { "nil", "true", "false", "self", "super" };

        /// <summary>
        /// 3.5.5 Operators - Binary Characters
        /// </summary>
        public const string BinaryCharacters = @"!%&*+,/<=>?@\~|-";

        /// <summary>
        /// 3.5.4 Keywords - Keyword Postfix
        /// </summary>
        public const char KeywordPostfix = ':';

        /// <summary>
        /// The Guid that uniquely identifies the IronSmalltalk language
        /// </summary>
        public static readonly Guid LanguageGuid = new Guid("E9653E63-112C-4865-A7F4-CDB8800C0E6E");

        /// <summary>
        /// The Guid that uniquely identifies the vendor of the IronSmalltalk language,
        /// i.e. that is the IronSmalltalk Project (because we don't do other languages).
        /// </summary>
        public static readonly Guid VendorGuid = new Guid("621750CA-32CD-4FC2-AEE5-B148B0D853D2");
    }
}

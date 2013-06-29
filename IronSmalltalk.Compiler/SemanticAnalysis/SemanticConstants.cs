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


namespace IronSmalltalk.Compiler.SemanticAnalysis
{
    /// <summary>
    /// Semantic constants, such as reserved identifiers, statement delimiters etc.,
    /// mostly defined in X3J20 "3.4 Method Grammar".
    /// </summary>
    public static class SemanticConstants
    {
        public const string Self = "self";
        public const string Super = "super";
        public const string Nil = "nil";
        public const string True = "true";
        public const string False = "false";

        public const char BlockStartDelimiter = '[';
        public const char BlockEndDelimiter = ']';
        public const char BlockArgumentPrefix = ':';
        public const char OpeningParenthesis = '(';
        public const char ClosingParenthesis = ')';
        public const char StatementDelimiter = '.';
        public const char CascadeDelimiter = ';';
        public const char LiteralArrayPrefix = '#';

        // IronSmalltalk extensions to the grammer.
        public const string PrimitiveOpeningDelimiter = "<";
        public const string PrimitiveClosingDelimiter = ">";
        // Those are alternative primitve delimiters, because we use the special unicode 
        // characters in the inline documentation (to avoid HTML encode '<' and '>')
        // and if somebody accidently copied some documentation, we want things to work.
        public const string AlternativePrimitiveOpeningDelimiter = "\u02C2"; // U+02C2 ˂ Modifier Letter Left Arrowhead 
        public const string AlternativePrimitiveClosingDelimiter = "\u02C3"; // U+02C3 ˃ Modifier Letter Right Arrowhead 
    }
}

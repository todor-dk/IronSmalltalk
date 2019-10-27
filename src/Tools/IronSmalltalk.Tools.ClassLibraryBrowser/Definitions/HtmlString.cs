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
using System.Text;
using System.Xml;

namespace IronSmalltalk.Tools.ClassLibraryBrowser.Definitions
{
    public struct HtmlString : IEquatable<HtmlString>
    {
        public string Html; // { get; set; }

        public string Text
        {
            get { return this.StripHtml(); } // Not perfect, but OK for now 
        }

        public HtmlString(XmlNode elem)
        {
            if (elem != null)
                this.Html = elem.InnerText;
            else
                this.Html = null;
        }

        public HtmlString(string html)
        {
            this.Html = html;
        }

        public void SaveXml(XmlWriter xml, string elementName)
        {
            this.SaveXml(xml, elementName, false);
        }

        public void SaveXml(XmlWriter xml, string elementName, bool alwaysEmit)
        {
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));
            if (String.IsNullOrWhiteSpace(elementName))
                throw new ArgumentNullException(nameof(elementName));

            if (alwaysEmit || !String.IsNullOrWhiteSpace(this.Html))
                xml.WriteElementString(elementName, this.Html);
        }

        private string StripHtml()
        {
            if (this.Html == null)
                return null;

            bool intag = false;
            StringBuilder txt = new StringBuilder();
            foreach (char ch in this.Html.Replace("</p>", "\r\n", StringComparison.OrdinalIgnoreCase))
            {
                if (intag)
                {
                    if (ch == '>')
                        intag = false;
                }
                else
                {
                    if (ch == '<')
                        intag = true;
                    else
                        txt.Append(ch);
                }
            }
            return System.Web.HttpUtility.HtmlDecode(txt.ToString());
        }


        #region Equality Members

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
#pragma warning disable CA1307 // Specify StringComparison
            return this.Html?.GetHashCode() ?? 0;
#pragma warning restore CA1307 // Specify StringComparison
        }

        /// <summary>
        /// Determines whether the specified HtmlString is equal to the current HtmlString.
        /// </summary>
        /// <param name="other">Another HtmlString to compare to this HtmlString.</param>
        /// <returns><c>true</c> if this HtmlString equals the given HtmlString, <c>false</c> otherwise.</returns>
        public bool Equals(HtmlString other)
        {
            return this.Html == other.Html;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HtmlString))
                return false;

            return this.Equals((HtmlString)obj);
        }

        /// <summary>
        /// Compares whether the left HtmlString operand is equal to the right HtmlString operand.
        /// </summary>
        /// <param name="left">The left HtmlString operand.</param>
        /// <param name="right">The right HtmlString operand.</param>
        /// <returns>The result of the equality operator.</returns>
        public static bool operator ==(HtmlString left, HtmlString right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares whether the left HtmlString operand is not equal to the right HtmlString operand.
        /// </summary>
        /// <param name="left">The left HtmlString operand.</param>
        /// <param name="right">The right HtmlString operand.</param>
        /// <returns>The result of the inequality operator.</returns>
        public static bool operator !=(HtmlString left, HtmlString right)
        {
            return !left.Equals(right);
        }

        #endregion

    }
}

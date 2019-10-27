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
using System.Xml;

namespace IronSmalltalk.Tools.ClassLibraryBrowser.Definitions.Implementation
{
#pragma warning disable CA1036 // Override methods on comparable types
    public abstract class GlobalItem : Definition<SystemImplementation>, IComparable<GlobalItem>, IComparable, IEquatable<GlobalItem>
#pragma warning restore CA1036 // Override methods on comparable types
    {
        public string Name { get; set; }

        public string DefiningProtocol { get; set; }

        public HtmlString Description { get; set; }

        public GlobalItem(SystemImplementation parent)
            : base(parent)
        {
        }

        public GlobalItem(SystemImplementation parent, XmlNode xml, XmlNamespaceManager nsm)
            : base(parent, xml, nsm)
        {
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));
            if (nsm == null)
                throw new ArgumentNullException(nameof(nsm));

            XmlAttribute attr = xml.SelectSingleNode("@name", nsm) as XmlAttribute;
            if (attr != null)
                this.Name = attr.Value.Trim();
            attr = xml.SelectSingleNode("@definingProtocol", nsm) as XmlAttribute;
            if (attr != null)
                this.DefiningProtocol = attr.Value.Trim();

            XmlNode elem = xml.SelectSingleNode("si:Description", nsm) as XmlElement;
            if (elem != null)
                this.Description = new HtmlString(elem);
        }

        public abstract void Save(XmlWriter xml);

        int IComparable<GlobalItem>.CompareTo(GlobalItem other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            return String.Compare(this.Name, other.Name, StringComparison.InvariantCulture);
        }

        int IComparable.CompareTo(object obj)
        {
            if (!(obj is GlobalItem))
                throw new ArgumentException("Invalid type", nameof(obj));
            return String.Compare(this.Name, ((GlobalItem)obj).Name, StringComparison.InvariantCulture);
        }


        #region Equality Members

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.Name?.GetHashCode(StringComparison.InvariantCulture) ?? 0;
        }

        /// <summary>
        /// Determines whether the specified GlobalItem is equal to the current GlobalItem.
        /// </summary>
        /// <param name="other">Another GlobalItem to compare to this GlobalItem.</param>
        /// <returns><c>true</c> if this GlobalItem equals the given GlobalItem, <c>false</c> otherwise.</returns>
        public bool Equals(GlobalItem other)
        {
            if (other is null)
                return false;
            if (other.GetType() != this.GetType())
                return false;
            return this.Name.Equals(other.Name, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is GlobalItem))
                return false;

            return this.Equals((GlobalItem)obj);
        }

        /// <summary>
        /// Compares whether the left GlobalItem operand is equal to the right GlobalItem operand.
        /// </summary>
        /// <param name="left">The left GlobalItem operand.</param>
        /// <param name="right">The right GlobalItem operand.</param>
        /// <returns>The result of the equality operator.</returns>
        public static bool operator ==(GlobalItem left, GlobalItem right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares whether the left GlobalItem operand is not equal to the right GlobalItem operand.
        /// </summary>
        /// <param name="left">The left GlobalItem operand.</param>
        /// <param name="right">The right GlobalItem operand.</param>
        /// <returns>The result of the inequality operator.</returns>
        public static bool operator !=(GlobalItem left, GlobalItem right)
        {
            return !left.Equals(right);
        }

        #endregion

    }
}

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
    public class Definition<TParent>
    {
        public TParent Parent { get; private set; }

        public Annotations Annotations { get; private set; }

        public Definition(TParent parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            this.Parent = parent;
            this.Annotations = new Annotations();
        }

        public Definition(TParent parent, XmlNode xml, XmlNamespaceManager nsm)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            this.Parent = parent;
            this.Annotations = new Annotations(xml, nsm);
        }
    }
}

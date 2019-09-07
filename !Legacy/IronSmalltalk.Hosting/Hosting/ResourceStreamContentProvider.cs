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
using Microsoft.Scripting;

namespace IronSmalltalk.Hosting.Hosting
{
    /// <summary>
    /// Provides a factory to create streams over resource based sources of binary content.  
    /// </summary>
    public class ResourceStreamContentProvider : StreamContentProvider
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }

        public ResourceStreamContentProvider(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");
            this.Type = type;
            this.Name = name;
        }

        private string GetStreamName()
        {
            StringBuilder builder = new StringBuilder();
            if (this.Type == null)
            {
                if (this.Name == null)
                {
                    throw new ArgumentNullException("type");
                }
            }
            else
            {
                string str = this.Type.Namespace;
                if (str != null)
                {
                    builder.Append(str);
                    if (this.Name != null)
                    {
                        builder.Append(Type.Delimiter);
                    }
                }
            }
            if (this.Name != null)
            {
                builder.Append(this.Name);
            }
            return builder.ToString();
        }

        public override System.IO.Stream GetStream()
        {
            string name = this.GetStreamName();
            System.IO.Stream stream = this.Type.Assembly.GetManifestResourceStream(name);
            return stream;
        }
    }
}

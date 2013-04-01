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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting;

namespace IronSmalltalk.Hosting.Hosting
{
    /// <summary>
    /// Provides a factory to create streams over sources using a callback delegate.  
    /// </summary>
    public class DelegateStreamContentProvider : StreamContentProvider
    {
        public Func<Stream> GetStreamFunction { get; private set; }

        public DelegateStreamContentProvider(Func<Stream> getStreamFunction)
        {
            if (getStreamFunction == null)
                throw new ArgumentNullException();
            this.GetStreamFunction = getStreamFunction;
        }

        public override Stream GetStream()
        {
            Stream result = this.GetStreamFunction();
            if (result == null)
                throw new InvalidOperationException("The GetStreamFunction was not expected to return null.");
            return result;
        }
    }
}

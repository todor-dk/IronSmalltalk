/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Scripting.Utils {
    public static class ExceptionUtils {
        public static ArgumentOutOfRangeException MakeArgumentOutOfRangeException(string paramName, object actualValue, string message) {
            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }

        public static ArgumentNullException MakeArgumentItemNullException(int index, string arrayName) {
            return new ArgumentNullException(String.Format("{0}[{1}]", arrayName, index));
        }

        public static object GetData(this Exception e, object key) {
            return e.Data[key];
        }

        public static void SetData(this Exception e, object key, object data) {
            e.Data[key] = data;
        }

        public static void RemoveData(this Exception e, object key) {
            e.Data.Remove(key);
        }
    }
}

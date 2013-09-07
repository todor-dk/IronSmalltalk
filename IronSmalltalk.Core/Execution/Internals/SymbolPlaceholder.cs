using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Execution.Internals
{
    public class SymbolPlaceholder
    {
        public string Value { get; private set; }

        [IronSmalltalk.Common.Internal.AccessedViaReflection]
        public SymbolPlaceholder(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            this.Value = value;
        }
    }
}

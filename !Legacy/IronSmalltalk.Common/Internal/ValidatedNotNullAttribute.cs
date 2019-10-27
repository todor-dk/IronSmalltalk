using System;
using System.Collections.Generic;
using System.Text;

namespace IronSmalltalk.Common.Internal
{
    /// <summary>
    /// The name ValidatedNotNullAttribute is magic. It doesn't matter what it does so long as it is named "ValidatedNotNullAttribute".
    /// See: https://esmithy.net/2011/03/15/suppressing-ca1062/.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Internal
{
    /// <summary>
    /// Indicates that the method is used to initialize the contents of a SmalltalkNameScope.
    /// The native compiler is decorates the initializer method with this attribute to indicate
    /// that this method is to be used to initialize a name scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ScopeInitializerAttribute : Attribute
    {
    }
}

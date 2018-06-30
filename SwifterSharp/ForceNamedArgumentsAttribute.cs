using System;

namespace SwifterSharp
{
    /// <summary>
    /// When this attribute is placed on a method callers
    /// must explicitly name all arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ForceNamedArgumentsAttribute : Attribute
    {
    }
}

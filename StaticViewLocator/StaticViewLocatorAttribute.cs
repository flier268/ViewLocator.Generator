using System;

namespace StaticViewLocator;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class StaticViewLocatorAttribute : Attribute
{
}

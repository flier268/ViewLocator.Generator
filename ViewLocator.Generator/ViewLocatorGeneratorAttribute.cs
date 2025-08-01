using System;

namespace ViewLocator.Generator;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ViewLocatorGeneratorAttribute : Attribute
{
    public string? ViewToViewModelNamespaceRule { get; set; }
    public string? ViewToViewModelSuffixRule { get; set; }
    public string[]? IncludeNamespaces { get; set; }
    public string[]? ExcludeNamespaces { get; set; }

    public ViewLocatorGeneratorAttribute() { }
}

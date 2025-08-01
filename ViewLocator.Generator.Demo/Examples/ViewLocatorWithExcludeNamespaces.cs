using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ViewLocator.Generator.Common;
using ViewLocator.Generator.Demo.ViewModels;

namespace ViewLocator.Generator.Demo.Examples;

/// <summary>
/// Example showing ExcludeNamespaces usage.
/// This excludes ViewModels from specific namespaces.
/// For example: Exclude all ViewModels from "Avalonia" namespaces (internal Avalonia ViewModels).
/// </summary>
[GenerateViewLocator(
    ExcludeNamespaces = new[] { "Avalonia", "System" }
)]
public partial class ViewLocatorWithExcludeNamespaces : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var type = data.GetType();

        if (s_views.TryGetValue(type, out var func))
        {
            return func.Invoke();
        }

        throw new Exception($"Unable to create view for type: {type}");
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
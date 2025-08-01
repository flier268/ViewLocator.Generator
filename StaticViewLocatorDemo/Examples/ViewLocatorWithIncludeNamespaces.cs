using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using StaticViewLocator;
using StaticViewLocatorDemo.ViewModels;

namespace StaticViewLocatorDemo.Examples;

/// <summary>
/// Example showing IncludeNamespaces usage.
/// This only includes ViewModels from specific namespaces.
/// For example: Only include ViewModels from "StaticViewLocatorDemo.ViewModels" namespace.
/// </summary>
[StaticViewLocator(
    IncludeNamespaces = new[] { "StaticViewLocatorDemo.ViewModels" }
)]
public partial class ViewLocatorWithIncludeNamespaces : IDataTemplate
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
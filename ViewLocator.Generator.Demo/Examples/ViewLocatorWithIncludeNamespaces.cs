using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ViewLocator.Generator.Common;
using ViewLocator.Generator.Demo.ViewModels;

namespace ViewLocator.Generator.Demo.Examples;

/// <summary>
/// Example showing IncludeNamespaces usage.
/// This only includes ViewModels from specific namespaces.
/// For example: Only include ViewModels from "ViewLocator.Generator.Demo.ViewModels" namespace.
/// </summary>
[GenerateViewLocator(
    IncludeNamespaces = new[] { "ViewLocator.Generator.Demo.ViewModels" }
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
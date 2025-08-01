using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ViewLocator.Generator.Common;
using ViewLocator.Generator.Demo.ViewModels;

namespace ViewLocator.Generator.Demo.Examples;

/// <summary>
/// Example showing combined usage of multiple properties.
/// This demonstrates using ViewToViewModelNamespaceRule, ViewToViewModelSuffixRule,
/// and ExcludeNamespaces together for complex view location scenarios.
/// </summary>
[GenerateViewLocator(
    ViewToViewModelNamespaceRule = "Views -> ViewModels",
    ViewToViewModelSuffixRule = "View -> ViewModel",
    ExcludeNamespaces = new[] { "Avalonia", "System" },
    IncludeNamespaces = new[] { "ViewLocator.Generator.Demo" }
)]
public partial class ViewLocatorCombinedExample : IDataTemplate
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
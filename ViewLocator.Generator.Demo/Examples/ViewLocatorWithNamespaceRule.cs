using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ViewLocator.Generator.Common;
using ViewLocator.Generator.Demo.ViewModels;

namespace ViewLocator.Generator.Demo.Examples;

/// <summary>
/// Example showing ViewToViewModelNamespaceRule usage.
/// This transforms "Views" namespace to "ViewModels" namespace.
/// For example: MyApp.Views.UserView -> MyApp.ViewModels.UserViewModel
/// </summary>
[GenerateViewLocator(
    ViewToViewModelNamespaceRule = "Views -> ViewModels",
    ExcludeNamespaces = new[] { "Avalonia" }
)]
public partial class ViewLocatorWithNamespaceRule : IDataTemplate
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
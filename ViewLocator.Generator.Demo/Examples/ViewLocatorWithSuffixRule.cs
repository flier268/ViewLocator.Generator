using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using StaticViewLocator;
using StaticViewLocatorDemo.ViewModels;

namespace StaticViewLocatorDemo.Examples;

/// <summary>
/// Example showing ViewToViewModelSuffixRule usage.
/// This transforms "Page" suffix to "PageViewModel" suffix.
/// For example: MyApp.Views.UserPage -> MyApp.ViewModels.UserPageViewModel
/// </summary>
[StaticViewLocator(
    ViewToViewModelSuffixRule = "Page -> PageViewModel",
    ExcludeNamespaces = new[] { "Avalonia" }
)]
public partial class ViewLocatorWithSuffixRule : IDataTemplate
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
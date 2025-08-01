using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ViewLocator.Generator.Common;
using ViewLocator.Generator.Demo.ViewModels;

namespace ViewLocator.Generator.Demo;

[GenerateViewLocator(ExcludeNamespaces = new[] { "Avalonia" })]
public partial class ViewLocator : IDataTemplate
{
#if true
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
#else
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }
#endif

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}

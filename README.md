# ViewLocator.Generator

[![CI](https://github.com/flier268/ViewLocator.Generator/actions/workflows/build.yml/badge.svg)](https://github.com/flier268/ViewLocator.Generator/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/ViewLocator.Generator.svg)](https://www.nuget.org/packages/ViewLocator.Generator)
[![NuGet](https://img.shields.io/nuget/dt/ViewLocator.Generator.svg)](https://www.nuget.org/packages/ViewLocator.Generator)

A C# source generator that automatically implements static view locator for Avalonia without using reflection.

*Originally from [wieslawsoltes/GenerateViewLocator](https://github.com/wieslawsoltes/GenerateViewLocator)*

## Usage

Add NuGet package references to your project:

```xml
<PackageReference Include="ViewLocator.Generator" Version="0.0.1">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
<PackageReference Include="ViewLocator.Generator.Common" Version="0.0.1" />
```

Annotate view locator class with `[GenerateViewLocator]` attribute, make class `partial` and implement `Build` using `s_views` dictionary to retrieve views for `data` objects.

```csharp
using ViewLocator.Generator.Common;

[GenerateViewLocator]
public partial class ViewLocator : IDataTemplate
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
```

Source generator will generate the `s_views` dictionary similar to below code using convention based on `ViewModel` suffix for view models substituted to `View` suffix.

```csharp
public partial class ViewLocator
{
	private static Dictionary<Type, Func<Control>> s_views = new()
	{
		[typeof(GenerateViewLocatorDemo.ViewModels.MainWindowViewModel)] = () => new GenerateViewLocatorDemo.Views.MainWindow(),
		[typeof(GenerateViewLocatorDemo.ViewModels.TestViewModel)] = () => new GenerateViewLocatorDemo.Views.TestView(),
	};
}
```

## Configuration Options

The `GenerateViewLocator` attribute supports several properties to customize the view location behavior:

### ViewToViewModelNamespaceRule

Transforms view namespaces to view model namespaces using a mapping rule.

**Format:** `"ViewNamespace -> ViewModelNamespace"`

```csharp
[GenerateViewLocator(ViewToViewModelNamespaceRule = "Views -> ViewModels")]
public partial class ViewLocator : IDataTemplate
{
    // Implementation...
}
```

**Example transformations:**
- `MyApp.Views.UserView` → `MyApp.ViewModels.UserViewModel`
- `MyApp.Views.Products.ProductView` → `MyApp.ViewModels.Products.ProductViewModel`

### ViewToViewModelSuffixRule

Transforms view suffixes to view model suffixes using a mapping rule.

**Format:** `"ViewSuffix -> ViewModelSuffix"`

```csharp
[GenerateViewLocator(ViewToViewModelSuffixRule = "Page -> PageViewModel")]
public partial class ViewLocator : IDataTemplate
{
    // Implementation...
}
```

**Example transformations:**
- `UserPage` → `UserPageViewModel`
- `ProductPage` → `ProductPageViewModel`

### IncludeNamespaces

Only includes ViewModels from the specified namespaces. This is useful when you want to limit view location to specific parts of your application.

```csharp
[GenerateViewLocator(IncludeNamespaces = new[] { "MyApp.ViewModels", "SharedLib.ViewModels" })]
public partial class ViewLocator : IDataTemplate
{
    // Implementation...
}
```

### ExcludeNamespaces

Excludes ViewModels from the specified namespaces. This is particularly useful for excluding framework or third-party ViewModels.

```csharp
[GenerateViewLocator(ExcludeNamespaces = new[] { "Avalonia", "System" })]
public partial class ViewLocator : IDataTemplate
{
    // Implementation...
}
```

### Combined Configuration

You can combine multiple properties for complex scenarios:

```csharp
[GenerateViewLocator(
    ViewToViewModelNamespaceRule = "Views -> ViewModels",
    ViewToViewModelSuffixRule = "View -> ViewModel",
    ExcludeNamespaces = new[] { "Avalonia", "System" },
    IncludeNamespaces = new[] { "MyApp" }
)]
public partial class ViewLocator : IDataTemplate
{
    // Implementation...
}
```

## Default Behavior

When no configuration is specified, the default behavior is:

- **Namespace transformation:** `ViewModels` → `Views`
- **Suffix transformation:** `ViewModel` → `View` (with special handling for `WindowViewModel` → `Window`)
- **Include all:** All accessible ViewModels are included (except when explicitly excluded)

## Examples

See the [Examples](GenerateViewLocatorDemo/Examples/) directory for comprehensive examples demonstrating each configuration option.

## Common Use Cases

### 1. Standard MVVM with Framework Exclusion
```csharp
[GenerateViewLocator(ExcludeNamespaces = new[] { "Avalonia" })]
```

### 2. Custom Naming Convention
```csharp
[GenerateViewLocator(
    ViewToViewModelNamespaceRule = "UI -> Business.ViewModels",
    ViewToViewModelSuffixRule = "Control -> ControlViewModel"
)]
```

### 3. Multi-Project Architecture
```csharp
[GenerateViewLocator(
    IncludeNamespaces = new[] { "MyApp.Core.ViewModels", "MyApp.Modules.ViewModels" },
    ExcludeNamespaces = new[] { "ThirdParty" }
)]
```

## License

ViewLocator.Generator is licensed under the MIT license. See [LICENSE](LICENSE.TXT) file for details.

# StaticViewLocator Examples

This directory contains examples demonstrating the various configuration options available with the StaticViewLocator.

## Available Configuration Properties

### ViewToViewModelNamespaceRule
Transforms view namespaces to view model namespaces using a mapping rule.

**Format:** `"ViewNamespace -> ViewModelNamespace"`

**Example:**
```csharp
[StaticViewLocator(ViewToViewModelNamespaceRule = "Views -> ViewModels")]
```

This would transform:
- `MyApp.Views.UserView` → `MyApp.ViewModels.UserViewModel`
- `MyApp.Views.Products.ProductView` → `MyApp.ViewModels.Products.ProductViewModel`

### ViewToViewModelSuffixRule
Transforms view suffixes to view model suffixes using a mapping rule.

**Format:** `"ViewSuffix -> ViewModelSuffix"`

**Example:**
```csharp
[StaticViewLocator(ViewToViewModelSuffixRule = "Page -> PageViewModel")]
```

This would transform:
- `UserPage` → `UserPageViewModel`
- `ProductPage` → `ProductPageViewModel`

### IncludeNamespaces
Only includes ViewModels from the specified namespaces.

**Example:**
```csharp
[StaticViewLocator(IncludeNamespaces = new[] { "MyApp.ViewModels", "SharedLib.ViewModels" })]
```

This would only include ViewModels from:
- `MyApp.ViewModels.*`
- `SharedLib.ViewModels.*`

### ExcludeNamespaces
Excludes ViewModels from the specified namespaces.

**Example:**
```csharp
[StaticViewLocator(ExcludeNamespaces = new[] { "Avalonia", "System" })]
```

This would exclude ViewModels from:
- `Avalonia.*` (internal Avalonia ViewModels)
- `System.*` (system ViewModels)

## Example Files

1. **ViewLocatorWithNamespaceRule.cs** - Demonstrates ViewToViewModelNamespaceRule
2. **ViewLocatorWithSuffixRule.cs** - Demonstrates ViewToViewModelSuffixRule
3. **ViewLocatorWithIncludeNamespaces.cs** - Demonstrates IncludeNamespaces
4. **ViewLocatorWithExcludeNamespaces.cs** - Demonstrates ExcludeNamespaces
5. **ViewLocatorCombinedExample.cs** - Demonstrates using multiple properties together

## Default Behavior

If no rules are specified, the default behavior is:
- **Namespace:** `ViewModels` → `Views`
- **Suffix:** `ViewModel` → `View` (with special handling for `WindowViewModel` → `Window`)

## Common Use Cases

### 1. Standard MVVM Pattern
```csharp
[StaticViewLocator(ExcludeNamespaces = new[] { "Avalonia" })]
```
Uses default transformations but excludes framework ViewModels.

### 2. Custom Naming Convention
```csharp
[StaticViewLocator(
    ViewToViewModelNamespaceRule = "UI -> Business.ViewModels",
    ViewToViewModelSuffixRule = "Control -> ControlViewModel"
)]
```
Transforms `UI.UserControl` → `Business.ViewModels.UserControlViewModel`.

### 3. Multiple Project Architecture
```csharp
[StaticViewLocator(
    IncludeNamespaces = new[] { "MyApp.Core.ViewModels", "MyApp.Modules.ViewModels" },
    ExcludeNamespaces = new[] { "ThirdParty" }
)]
```
Includes ViewModels from core and modules, but excludes third-party libraries.
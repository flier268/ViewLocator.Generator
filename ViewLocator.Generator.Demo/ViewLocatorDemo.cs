using System;
using ViewLocator.Generator.Demo.ViewModels;
using ViewLocator.Generator.Demo.Examples;

namespace ViewLocator.Generator.Demo;

/// <summary>
/// Demo class showing how to use different ViewLocator configurations
/// </summary>
public class ViewLocatorDemo
{
    public static void ShowViewLocatorExamples()
    {
        var testViewModel = new TestViewModel();
        var mainWindowViewModel = new MainWindowViewModel();

        Console.WriteLine("=== ViewLocator.Generator Configuration Examples ===");
        Console.WriteLine();

        // Example 1: Default ViewLocator with ExcludeNamespaces
        Console.WriteLine("1. Default ViewLocator (with ExcludeNamespaces):");
        var defaultLocator = new ViewLocator();
        try
        {
            var view1 = defaultLocator.Build(testViewModel);
            Console.WriteLine($"   TestViewModel -> {view1?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   TestViewModel -> Error: {ex.Message}");
        }

        // Example 2: ViewLocator with NamespaceRule
        Console.WriteLine();
        Console.WriteLine("2. ViewLocator with ViewToViewModelNamespaceRule:");
        var namespaceRuleLocator = new ViewLocatorWithNamespaceRule();
        try
        {
            var view2 = namespaceRuleLocator.Build(testViewModel);
            Console.WriteLine($"   TestViewModel -> {view2?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   TestViewModel -> Error: {ex.Message}");
        }

        // Example 3: ViewLocator with SuffixRule
        Console.WriteLine();
        Console.WriteLine("3. ViewLocator with ViewToViewModelSuffixRule:");
        var suffixRuleLocator = new ViewLocatorWithSuffixRule();
        try
        {
            var view3 = suffixRuleLocator.Build(testViewModel);
            Console.WriteLine($"   TestViewModel -> {view3?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   TestViewModel -> Error: {ex.Message}");
        }

        // Example 4: ViewLocator with IncludeNamespaces
        Console.WriteLine();
        Console.WriteLine("4. ViewLocator with IncludeNamespaces:");
        var includeNamespacesLocator = new ViewLocatorWithIncludeNamespaces();
        try
        {
            var view4 = includeNamespacesLocator.Build(testViewModel);
            Console.WriteLine($"   TestViewModel -> {view4?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   TestViewModel -> Error: {ex.Message}");
        }

        // Example 5: ViewLocator with ExcludeNamespaces
        Console.WriteLine();
        Console.WriteLine("5. ViewLocator with ExcludeNamespaces:");
        var excludeNamespacesLocator = new ViewLocatorWithExcludeNamespaces();
        try
        {
            var view5 = excludeNamespacesLocator.Build(testViewModel);
            Console.WriteLine($"   TestViewModel -> {view5?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   TestViewModel -> Error: {ex.Message}");
        }

        // Example 6: Combined configuration
        Console.WriteLine();
        Console.WriteLine("6. ViewLocator with Combined Configuration:");
        var combinedLocator = new ViewLocatorCombinedExample();
        try
        {
            var view6 = combinedLocator.Build(testViewModel);
            Console.WriteLine($"   TestViewModel -> {view6?.GetType().Name ?? "null"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   TestViewModel -> Error: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("=== Generated View Mappings ===");
        Console.WriteLine("Check the generated *_StaticViewLocator.cs files in obj/GeneratedFiles/ to see");
        Console.WriteLine("how each configuration affects the generated view mappings.");
        
        Console.WriteLine();
        ViewLocatorTests.RunBasicTests();
    }
}
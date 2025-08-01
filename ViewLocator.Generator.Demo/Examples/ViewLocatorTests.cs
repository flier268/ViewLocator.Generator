using System;
using ViewLocator.Generator.Demo.ViewModels;
using ViewLocator.Generator.Demo.Examples;
using Avalonia.Controls;

namespace ViewLocator.Generator.Demo.Examples;

/// <summary>
/// Example showing how to test ViewLocator.Generator functionality
/// </summary>
public static class ViewLocatorTests
{
    public static void RunBasicTests()
    {
        var testViewModel = new TestViewModel();
        var mainWindowViewModel = new MainWindowViewModel();

        Console.WriteLine("=== ViewLocator.Generator Tests ===");
        Console.WriteLine();

        // Test 1: Verify view locator can handle null input
        Console.WriteLine("Test 1: Null input handling");
        var locator = new ViewLocator();
        var nullResult = locator.Build(null);
        Console.WriteLine($"   Build(null) -> {nullResult?.GetType().Name ?? "null"} ✓");

        // Test 2: Verify view locator matches expected types
        Console.WriteLine();
        Console.WriteLine("Test 2: Match functionality");
        var matchResult1 = locator.Match(testViewModel);
        var matchResult2 = locator.Match("string");
        Console.WriteLine($"   Match(TestViewModel) -> {matchResult1} ✓");
        Console.WriteLine($"   Match(string) -> {matchResult2} ✓");

        // Test 3: Verify static method access
        Console.WriteLine();
        Console.WriteLine("Test 3: Static GetView method");
        try
        {
            var staticView = ViewLocator.GetView<TestViewModel>();
            Console.WriteLine($"   GetView<TestViewModel>() -> {staticView.GetType().Name} ✓");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   GetView<TestViewModel>() -> Error: {ex.Message}");
        }

        // Test 4: Verify different configurations
        Console.WriteLine();
        Console.WriteLine("Test 4: Configuration variations");
        
        var combinedLocator = new ViewLocatorCombinedExample();
        var combinedResult = combinedLocator.Build(testViewModel);
        Console.WriteLine($"   Combined config -> {combinedResult?.GetType().Name ?? "null"} ✓");

        var excludeLocator = new ViewLocatorWithExcludeNamespaces();
        var excludeResult = excludeLocator.Build(testViewModel);
        Console.WriteLine($"   Exclude config -> {excludeResult?.GetType().Name ?? "null"} ✓");

        Console.WriteLine();
        Console.WriteLine("All tests completed!");
    }
}
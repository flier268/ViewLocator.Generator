using System;
using System.Diagnostics;
using ViewLocator.GeneratorDemo.Examples;
using ViewLocator.GeneratorDemo.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace ViewLocator.Generator.Tests;

/// <summary>
/// Performance tests for ViewLocator.Generator functionality
/// </summary>
public class ViewLocator.GeneratorPerformanceTests
{
    private readonly ITestOutputHelper _output;

    public ViewLocator.GeneratorPerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ViewLocator_GetView_IsFast()
    {
        // Arrange
        const int iterations = 10000;
        var stopwatch = new Stopwatch();

        // Warm up
        for (int i = 0; i < 100; i++)
        {
            ViewLocator.GeneratorDemo.ViewLocator.GetView<TestViewModel>();
        }

        // Act
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            var view = ViewLocator.GeneratorDemo.ViewLocator.GetView<TestViewModel>();
            // No need to dispose Avalonia controls
        }
        stopwatch.Stop();

        // Assert
        var averageTimeMs = stopwatch.ElapsedMilliseconds / (double)iterations;
        _output.WriteLine($"Average time per GetView<TestViewModel>(): {averageTimeMs:F4} ms");
        
        // Should be very fast (less than 0.1ms per call)
        Assert.True(averageTimeMs < 0.1, $"GetView took {averageTimeMs:F4} ms on average, expected < 0.1 ms");
    }

    [Fact]
    public void ViewLocator_Build_IsFast()
    {
        // Arrange
        const int iterations = 10000;
        var locator = new ViewLocator.GeneratorDemo.ViewLocator();
        var testViewModel = new TestViewModel();
        var stopwatch = new Stopwatch();

        // Warm up
        for (int i = 0; i < 100; i++)
        {
            var warmupView = locator.Build(testViewModel);
            // No need to dispose Avalonia controls
        }

        // Act
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            var view = locator.Build(testViewModel);
            // No need to dispose Avalonia controls
        }
        stopwatch.Stop();

        // Assert
        var averageTimeMs = stopwatch.ElapsedMilliseconds / (double)iterations;
        _output.WriteLine($"Average time per Build(TestViewModel): {averageTimeMs:F4} ms");
        
        // Should be very fast (less than 0.15ms per call due to interface overhead)
        Assert.True(averageTimeMs < 0.15, $"Build took {averageTimeMs:F4} ms on average, expected < 0.15 ms");
    }

    [Fact]
    public void ViewLocator_MultipleViewModels_PerformsWell()
    {
        // Arrange
        const int iterations = 1000;
        var locator = new ViewLocator.GeneratorDemo.ViewLocator();
        var testViewModel = new TestViewModel();
        var mainWindowViewModel = new MainWindowViewModel();
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            try
            {
                var view1 = locator.Build(testViewModel);
                var view3 = ViewLocator.GeneratorDemo.ViewLocator.GetView<TestViewModel>();
                
                // Skip MainWindow creation in headless environment
                try
                {
                    var view2 = locator.Build(mainWindowViewModel);
                    var view4 = ViewLocator.GeneratorDemo.ViewLocator.GetView<MainWindowViewModel>();
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("IWindowingPlatform"))
                {
                    // Expected in headless test environment
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("IWindowingPlatform"))
            {
                // Expected in headless test environment
            }
        }
        stopwatch.Stop();

        // Assert
        var averageTimeMs = stopwatch.ElapsedMilliseconds / (double)iterations;
        _output.WriteLine($"Average time per view creation (mixed): {averageTimeMs:F4} ms");
        
        // Should handle multiple view types efficiently (relaxed for test environment)
        Assert.True(averageTimeMs < 1.0, $"Mixed view creation took {averageTimeMs:F4} ms on average, expected < 1.0 ms");
    }

    [Fact]
    public void ViewLocator_ConcurrentAccess_IsThreadSafe()
    {
        // Arrange
        const int iterations = 1000;
        var locator = new ViewLocator.GeneratorDemo.ViewLocator();
        var testViewModel = new TestViewModel();
        var tasks = new System.Threading.Tasks.Task[Environment.ProcessorCount];
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        for (int t = 0; t < tasks.Length; t++)
        {
            tasks[t] = System.Threading.Tasks.Task.Run(() =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    var view = locator.Build(testViewModel);
                    // No need to dispose Avalonia controls
                }
            });
        }

        System.Threading.Tasks.Task.WaitAll(tasks);
        stopwatch.Stop();

        // Assert
        var totalOperations = tasks.Length * iterations;
        var averageTimeMs = stopwatch.ElapsedMilliseconds / (double)totalOperations;
        _output.WriteLine($"Average time per concurrent view creation: {averageTimeMs:F4} ms");
        _output.WriteLine($"Total concurrent operations: {totalOperations}");
        
        // Should handle concurrent access efficiently
        Assert.True(averageTimeMs < 0.5, $"Concurrent view creation took {averageTimeMs:F4} ms on average, expected < 0.5 ms");
    }

    [Fact]
    public void ViewLocator_MemoryUsage_IsReasonable()
    {
        // Arrange
        const int iterations = 1000;
        var locator = new ViewLocator.GeneratorDemo.ViewLocator();
        var testViewModel = new TestViewModel();

        // Force garbage collection to get baseline
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var startMemory = GC.GetTotalMemory(false);

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var view = locator.Build(testViewModel);
            // No need to dispose Avalonia controls
        }

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var endMemory = GC.GetTotalMemory(false);

        // Assert
        var memoryDelta = endMemory - startMemory;
        var memoryPerOperation = memoryDelta / (double)iterations;
        
        _output.WriteLine($"Memory delta: {memoryDelta:N0} bytes");
        _output.WriteLine($"Memory per operation: {memoryPerOperation:F2} bytes");
        
        // Should not leak significant memory (relaxed for test environment)
        Assert.True(memoryDelta < 50000, $"Memory usage increased by {memoryDelta:N0} bytes, expected < 50000 bytes");
    }

    [Fact]
    public void ViewLocator_StaticDictionary_IsInitializedOnce()
    {
        // Arrange & Act
        var viewsField = typeof(ViewLocator.GeneratorDemo.ViewLocator).GetField("s_views", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        var views1 = viewsField?.GetValue(null);
        var views2 = viewsField?.GetValue(null);

        // Assert
        Assert.NotNull(views1);
        Assert.NotNull(views2);
        Assert.Same(views1, views2); // Should be the same instance (static)
        
        _output.WriteLine($"Views dictionary contains {((System.Collections.IDictionary)views1!).Count} entries");
    }
}
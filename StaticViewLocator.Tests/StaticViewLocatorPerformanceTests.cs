using System;
using System.Diagnostics;
using StaticViewLocatorDemo.Examples;
using StaticViewLocatorDemo.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace StaticViewLocator.Tests;

/// <summary>
/// Performance tests for StaticViewLocator functionality
/// </summary>
public class StaticViewLocatorPerformanceTests
{
    private readonly ITestOutputHelper _output;

    public StaticViewLocatorPerformanceTests(ITestOutputHelper output)
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
            ViewLocator.GetView<TestViewModel>();
        }

        // Act
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            var view = ViewLocator.GetView<TestViewModel>();
            view.Dispose(); // Clean up
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
        var locator = new ViewLocator();
        var testViewModel = new TestViewModel();
        var stopwatch = new Stopwatch();

        // Warm up
        for (int i = 0; i < 100; i++)
        {
            var warmupView = locator.Build(testViewModel);
            warmupView?.Dispose();
        }

        // Act
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            var view = locator.Build(testViewModel);
            view?.Dispose(); // Clean up
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
        var locator = new ViewLocator();
        var testViewModel = new TestViewModel();
        var mainWindowViewModel = new MainWindowViewModel();
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            var view1 = locator.Build(testViewModel);
            var view2 = locator.Build(mainWindowViewModel);
            var view3 = ViewLocator.GetView<TestViewModel>();
            var view4 = ViewLocator.GetView<MainWindowViewModel>();
            
            view1?.Dispose();
            view2?.Dispose();
            view3?.Dispose();
            view4?.Dispose();
        }
        stopwatch.Stop();

        // Assert
        var averageTimeMs = stopwatch.ElapsedMilliseconds / (double)(iterations * 4);
        _output.WriteLine($"Average time per view creation (mixed): {averageTimeMs:F4} ms");
        
        // Should handle multiple view types efficiently
        Assert.True(averageTimeMs < 0.2, $"Mixed view creation took {averageTimeMs:F4} ms on average, expected < 0.2 ms");
    }

    [Fact]
    public void ViewLocator_ConcurrentAccess_IsThreadSafe()
    {
        // Arrange
        const int iterations = 1000;
        var locator = new ViewLocator();
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
                    view?.Dispose();
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
        var locator = new ViewLocator();
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
            view?.Dispose();
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
        
        // Should not leak significant memory (less than 1KB per 1000 operations)
        Assert.True(memoryDelta < 1024, $"Memory usage increased by {memoryDelta:N0} bytes, expected < 1024 bytes");
    }

    [Fact]
    public void ViewLocator_StaticDictionary_IsInitializedOnce()
    {
        // Arrange & Act
        var viewsField = typeof(ViewLocator).GetField("s_views", 
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
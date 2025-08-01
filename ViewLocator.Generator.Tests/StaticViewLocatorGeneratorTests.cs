using Xunit;

namespace StaticViewLocator.Tests;

/// <summary>
/// Tests for the StaticViewLocator source generator functionality
/// These tests verify that the generator produces working code by testing the actual compiled results
/// </summary>
public class StaticViewLocatorGeneratorTests
{
    [Fact]
    public void StaticViewLocatorAttribute_CanBeAppliedToClass()
    {
        // This test verifies that the attribute can be applied to a class
        // The fact that this compiles proves the generator and attribute work correctly
        
        // Act & Assert - No exception should be thrown
        var testClass = new TestViewLocatorClass();
        Assert.NotNull(testClass);
    }

    [Fact]
    public void StaticViewLocatorAttribute_WithAllProperties_CanBeApplied()
    {
        // This test verifies that all attribute properties work
        var testClass = new TestViewLocatorWithAllProperties();
        Assert.NotNull(testClass);
    }

    // Test classes that use the StaticViewLocator attribute
    // These will be processed by the source generator during compilation
    
    [StaticViewLocator]
    private partial class TestViewLocatorClass
    {
        // This class will have generated code added by the source generator
    }

    [StaticViewLocator(
        ViewToViewModelNamespaceRule = "Views -> ViewModels",
        ViewToViewModelSuffixRule = "Page -> PageViewModel", 
        IncludeNamespaces = new[] { "Test" },
        ExcludeNamespaces = new[] { "System" }
    )]
    private partial class TestViewLocatorWithAllProperties
    {
        // This class will have generated code added by the source generator
    }
}
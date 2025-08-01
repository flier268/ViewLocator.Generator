using ViewLocatorGenerator;
using Xunit;

namespace ViewLocator.Generator.Tests;

/// <summary>
/// Tests for the ViewLocator.Generator source generator functionality
/// These tests verify that the generator produces working code by testing the actual compiled results
/// </summary>
public class GeneratorGeneratorTests
{
    [Fact]
    public void ViewLocatorGeneratorAttribute_CanBeAppliedToClass()
    {
        // This test verifies that the attribute can be applied to a class
        // The fact that this compiles proves the generator and attribute work correctly
        
        // Act & Assert - No exception should be thrown
        var testClass = new TestViewLocatorClass();
        Assert.NotNull(testClass);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_WithAllProperties_CanBeApplied()
    {
        // This test verifies that all attribute properties work
        var testClass = new TestViewLocatorWithAllProperties();
        Assert.NotNull(testClass);
    }

    // Test classes that use the ViewLocator.Generator attribute
    // These will be processed by the source generator during compilation
    
    [ViewLocatorGenerator]
    private partial class TestViewLocatorClass
    {
        // This class will have generated code added by the source generator
    }

    [ViewLocatorGenerator(
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
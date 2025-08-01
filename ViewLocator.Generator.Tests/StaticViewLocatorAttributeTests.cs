using System;
using ViewLocatorGenerator;
using Xunit;

namespace ViewLocator.Generator.Tests;

/// <summary>
/// Tests for the ViewLocatorGeneratorAttribute configuration
/// </summary>
public class ViewLocatorGeneratorAttributeTests
{
    [Fact]
    public void ViewLocatorGeneratorAttribute_DefaultConstructor_CreatesInstance()
    {
        // Act
        var attribute = new ViewLocatorGeneratorAttribute();

        // Assert
        Assert.NotNull(attribute);
        Assert.Null(attribute.ViewToViewModelNamespaceRule);
        Assert.Null(attribute.ViewToViewModelSuffixRule);
        Assert.Null(attribute.IncludeNamespaces);
        Assert.Null(attribute.ExcludeNamespaces);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_CanSetViewToViewModelNamespaceRule()
    {
        // Arrange
        var attribute = new ViewLocatorGeneratorAttribute();
        var rule = "Views -> ViewModels";

        // Act
        attribute.ViewToViewModelNamespaceRule = rule;

        // Assert
        Assert.Equal(rule, attribute.ViewToViewModelNamespaceRule);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_CanSetViewToViewModelSuffixRule()
    {
        // Arrange
        var attribute = new ViewLocatorGeneratorAttribute();
        var rule = "Page -> PageViewModel";

        // Act
        attribute.ViewToViewModelSuffixRule = rule;

        // Assert
        Assert.Equal(rule, attribute.ViewToViewModelSuffixRule);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_CanSetIncludeNamespaces()
    {
        // Arrange
        var attribute = new ViewLocatorGeneratorAttribute();
        var namespaces = new[] { "MyApp.ViewModels", "MyApp.Components" };

        // Act
        attribute.IncludeNamespaces = namespaces;

        // Assert
        Assert.Equal(namespaces, attribute.IncludeNamespaces);
        Assert.Equal(2, attribute.IncludeNamespaces.Length);
        Assert.Contains("MyApp.ViewModels", attribute.IncludeNamespaces);
        Assert.Contains("MyApp.Components", attribute.IncludeNamespaces);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_CanSetExcludeNamespaces()
    {
        // Arrange
        var attribute = new ViewLocatorGeneratorAttribute();
        var namespaces = new[] { "Avalonia", "System" };

        // Act
        attribute.ExcludeNamespaces = namespaces;

        // Assert
        Assert.Equal(namespaces, attribute.ExcludeNamespaces);
        Assert.Equal(2, attribute.ExcludeNamespaces.Length);
        Assert.Contains("Avalonia", attribute.ExcludeNamespaces);
        Assert.Contains("System", attribute.ExcludeNamespaces);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_HasCorrectAttributeUsage()
    {
        // Act
        var attributeUsage = typeof(ViewLocatorGeneratorAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)[0] as AttributeUsageAttribute;

        // Assert
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Class, attributeUsage.ValidOn);
        Assert.False(attributeUsage.Inherited);
        Assert.False(attributeUsage.AllowMultiple);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_CanSetAllPropertiesSimultaneously()
    {
        // Arrange
        var attribute = new ViewLocatorGeneratorAttribute();

        // Act
        attribute.ViewToViewModelNamespaceRule = "Views -> ViewModels";
        attribute.ViewToViewModelSuffixRule = "Page -> PageViewModel";
        attribute.IncludeNamespaces = new[] { "MyApp" };
        attribute.ExcludeNamespaces = new[] { "System" };

        // Assert
        Assert.Equal("Views -> ViewModels", attribute.ViewToViewModelNamespaceRule);
        Assert.Equal("Page -> PageViewModel", attribute.ViewToViewModelSuffixRule);
        Assert.Single(attribute.IncludeNamespaces);
        Assert.Equal("MyApp", attribute.IncludeNamespaces[0]);
        Assert.Single(attribute.ExcludeNamespaces);
        Assert.Equal("System", attribute.ExcludeNamespaces[0]);
    }

    [Fact]
    public void ViewLocatorGeneratorAttribute_CanBeAppliedToClass()
    {
        // This test verifies that the attribute can be applied to a class
        // The fact that the test class compiles proves the attribute usage is correct
        
        // Act & Assert - No exception should be thrown
        var testClass = new TestClassWithAttribute();
        Assert.NotNull(testClass);
    }

    [ViewLocatorGenerator(
        ViewToViewModelNamespaceRule = "Views -> ViewModels",
        ViewToViewModelSuffixRule = "Page -> PageViewModel",
        IncludeNamespaces = new[] { "Test" },
        ExcludeNamespaces = new[] { "System" }
    )]
    private class TestClassWithAttribute
    {
    }
}
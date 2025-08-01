using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ViewLocatorGenerator;

[Generator]
public class ViewLocatorGenerator : IIncrementalGenerator
{
    private const string ViewLocatorGeneratorAttributeDisplayString =
        "ViewLocatorGenerator.ViewLocatorGeneratorAttribute";
    private const string ViewModelSuffix = "ViewModel";
    private const string ViewSuffix = "View";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classSymbols = context
            .SyntaxProvider.CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) =>
                {
                    return ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node)
                        as INamedTypeSymbol;
                }
            )
            .Where(symbol => symbol is not null)
            .Collect();

        var compilationAndSymbols = context.CompilationProvider.Combine(classSymbols);

        context.RegisterSourceOutput(
            compilationAndSymbols,
            (spc, tuple) =>
            {
                var (compilation, allSymbols) = tuple;
                var locators = allSymbols
                    .Where(s =>
                    {
                        return s!
                            .GetAttributes()
                            .Any(a =>
                            {
                                return a.AttributeClass?.ToDisplayString()
                                    == ViewLocatorGeneratorAttributeDisplayString;
                            });
                    })
                    .ToList();

                // 获取所有引用的程序集中的类型
                var referencedViewModels = new List<INamedTypeSymbol>();
                foreach (var reference in compilation.References)
                {
                    if (
                        compilation.GetAssemblyOrModuleSymbol(reference)
                        is IAssemblySymbol assemblySymbol
                    )
                    {
                        GetViewModelsFromAssembly(
                            assemblySymbol.GlobalNamespace,
                            referencedViewModels
                        );
                    }
                }

                var viewModels = allSymbols
                    .Where(s =>
                    {
                        return s!.Name.EndsWith(ViewModelSuffix) && !s.IsAbstract && s.DeclaredAccessibility == Accessibility.Public;
                    })
                    .Concat(referencedViewModels.Where(s => s.DeclaredAccessibility == Accessibility.Public))
                    .ToList();

                foreach (var locator in locators)
                {
                    var classSource = ProcessClass(compilation, locator!, viewModels!);
                    if (!string.IsNullOrEmpty(classSource))
                    {
                        spc.AddSource(
                            $"{locator!.Name}_ViewLocatorGenerator.cs",
                            SourceText.From(classSource!, Encoding.UTF8)
                        );
                    }
                }
            }
        );
    }

    private static string? ProcessClass(
        Compilation compilation,
        INamedTypeSymbol namedTypeSymbolLocator,
        List<INamedTypeSymbol> namedTypeSymbolViewModels
    )
    {
        if (
            !namedTypeSymbolLocator.ContainingSymbol.Equals(
                namedTypeSymbolLocator.ContainingNamespace,
                SymbolEqualityComparer.Default
            )
        )
        {
            return null;
        }
        string namespaceNameLocator = namedTypeSymbolLocator.ContainingNamespace.ToDisplayString();
        var format = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
                | SymbolDisplayGenericsOptions.IncludeTypeConstraints
                | SymbolDisplayGenericsOptions.IncludeVariance
        );
        string classNameLocator = namedTypeSymbolLocator.ToDisplayString(format);
        var attr = namedTypeSymbolLocator
            .GetAttributes()
            .FirstOrDefault(a =>
                a.AttributeClass?.ToDisplayString() == ViewLocatorGeneratorAttributeDisplayString
            );
        string? nsRule = null;
        string? suffixRule = null;
        string[]? includeNamespaces = null;
        string[]? excludeNamespaces = null;
        if (attr != null)
        {
            foreach (var arg in attr.NamedArguments)
            {
                if (arg.Key == "ViewToViewModelNamespaceRule")
                    nsRule = arg.Value.Value as string;
                if (arg.Key == "ViewToViewModelSuffixRule")
                    suffixRule = arg.Value.Value as string;
                if (arg.Key == "IncludeNamespaces")
                {
                    if (!arg.Value.Values.IsDefault)
                    {
                        includeNamespaces =
                        [
                            .. arg
                                .Value.Values.Select(v => v.Value as string)
                                .Where(v => v != null)!,
                        ];
                    }
                }
                if (arg.Key == "ExcludeNamespaces")
                {
                    if (!arg.Value.Values.IsDefault)
                    {
                        excludeNamespaces =
                        [
                            .. arg
                                .Value.Values.Select(v => v.Value as string)
                                .Where(v => v != null)!,
                        ];
                    }
                }
            }
        }
        string? nsFrom = null;
        string? nsTo = null;
        string? suffixFrom = null;
        string? suffixTo = null;
        if (!string.IsNullOrEmpty(nsRule) && nsRule!.Contains("->"))
        {
            var parts = nsRule!.Split(new[] { "->" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                nsFrom = parts[0].Trim();
                nsTo = parts[1].Trim();
            }
        }
        if (!string.IsNullOrEmpty(suffixRule) && suffixRule!.Contains("->"))
        {
            var parts = suffixRule!.Split(new[] { "->" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                suffixFrom = parts[0].Trim();
                suffixTo = parts[1].Trim();
            }
        }
        var source = new StringBuilder();
        source.AppendLine("// <auto-generated />");
        source.AppendLine("#nullable enable");
        source.AppendLine("using System;");
        source.AppendLine("using System.Collections.Generic;");
        source.AppendLine("using Avalonia.Controls;");
        source.AppendLine();
        source.AppendLine($"namespace {namespaceNameLocator};");
        source.AppendLine();
        source.AppendLine($"public partial class {classNameLocator}");
        source.AppendLine("{");
        source.AppendLine("    private static Dictionary<Type, Func<Control>> s_views = new()");
        source.AppendLine("    {");
        var userControlViewSymbol = compilation.GetTypeByMetadataName(
            "Avalonia.Controls.UserControl"
        );
        var windowViewSymbol = compilation.GetTypeByMetadataName(
            "Avalonia.Controls.Window"
        );
        foreach (var namedTypeSymbolViewModel in namedTypeSymbolViewModels)
        {
            string namespaceNameViewModel =
                namedTypeSymbolViewModel.ContainingNamespace.ToDisplayString();
            string classNameViewModel = namedTypeSymbolViewModel.Name;

            // 应用命名空间筛选器
            if (includeNamespaces != null && includeNamespaces.Length > 0)
            {
                if (!includeNamespaces.Any(ns => namespaceNameViewModel.StartsWith(ns)))
                {
                    continue;
                }
            }

            if (excludeNamespaces != null && excludeNamespaces.Length > 0)
            {
                if (excludeNamespaces.Any(ns => namespaceNameViewModel.StartsWith(ns)))
                {
                    continue;
                }
            }

            // 处理嵌套类：使用完整的类型名称而不是仅仅Name
            var fullTypeName = namedTypeSymbolViewModel.ToDisplayString(
                new SymbolDisplayFormat(
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
                )
            );

            // 如果是嵌套类，跳过处理（因为通常嵌套的ViewModel不需要对应的View）
            if (namedTypeSymbolViewModel.ContainingSymbol is INamedTypeSymbol)
            {
                continue;
            }

            string viewNamespace = namespaceNameViewModel;
            string viewName = classNameViewModel;
            
            // Apply namespace rule (if specified) or default transformation
            if (!string.IsNullOrEmpty(nsFrom) && !string.IsNullOrEmpty(nsTo))
            {
                viewNamespace = namespaceNameViewModel.Replace(nsTo!, nsFrom!);
            }
            else
            {
                // Default: ViewModels -> Views
                viewNamespace = namespaceNameViewModel.Replace("ViewModels", "Views");
            }
            
            // Apply suffix rule (if specified) or default transformation
            if (!string.IsNullOrEmpty(suffixFrom) && !string.IsNullOrEmpty(suffixTo))
            {
                viewName = classNameViewModel.Replace(suffixTo!, suffixFrom!);
            }
            else
            {
                // Default: ViewModel -> View, but handle special cases
                if (classNameViewModel.EndsWith("ViewModel"))
                {
                    var baseName = classNameViewModel.Substring(0, classNameViewModel.Length - "ViewModel".Length);
                    // For MainWindow, remove ViewModel completely; for others, add View
                    if (baseName.EndsWith("Window"))
                    {
                        viewName = baseName;
                    }
                    else
                    {
                        viewName = baseName + "View";
                    }
                }
            }
            string classNameView = $"{viewNamespace}.{viewName}";
            var classNameViewSymbol = compilation.GetTypeByMetadataName(classNameView);
            bool isValidView = classNameViewSymbol is not null && (
                classNameViewSymbol.BaseType?.Equals(userControlViewSymbol, SymbolEqualityComparer.Default) == true ||
                classNameViewSymbol.BaseType?.Equals(windowViewSymbol, SymbolEqualityComparer.Default) == true
            );
            
            if (!isValidView)
            {
                source.AppendLine(
                    $"        [typeof({namespaceNameViewModel}.{classNameViewModel})] = () => new TextBlock() {{ Text = \"Not Found: {classNameView}\" }},"
                );
            }
            else
            {
                source.AppendLine(
                    $"        [typeof({namespaceNameViewModel}.{classNameViewModel})] = () => new {classNameView}(),"
                );
            }
        }
        source.AppendLine("    };");
        source.AppendLine();
        source.AppendLine("    public static Control GetView<T>()");
        source.AppendLine("        where T : notnull");
        source.AppendLine("    {");
        source.AppendLine("        var type = typeof(T);");
        source.AppendLine("        if (s_views.TryGetValue(type, out var factory))");
        source.AppendLine("            return factory();");
        source.AppendLine();
        source.AppendLine(
            "        throw new InvalidOperationException($\"No view registered for {typeof(T).FullName}\");"
        );
        source.AppendLine("    }");
        source.AppendLine("}");
        return source.ToString();
    }

    private static void GetViewModelsFromAssembly(
        INamespaceSymbol namespaceSymbol,
        List<INamedTypeSymbol> viewModels
    )
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            if (member is INamedTypeSymbol namedType)
            {
                if (namedType.Name.EndsWith(ViewModelSuffix) && !namedType.IsAbstract && namedType.DeclaredAccessibility == Accessibility.Public)
                {
                    viewModels.Add(namedType);
                }

                // Recursively process nested types
                foreach (var nestedType in namedType.GetTypeMembers())
                {
                    if (nestedType.Name.EndsWith(ViewModelSuffix) && !nestedType.IsAbstract && nestedType.DeclaredAccessibility == Accessibility.Public)
                    {
                        viewModels.Add(nestedType);
                    }
                }
            }
            else if (member is INamespaceSymbol nestedNamespace)
            {
                GetViewModelsFromAssembly(nestedNamespace, viewModels);
            }
        }
    }
}

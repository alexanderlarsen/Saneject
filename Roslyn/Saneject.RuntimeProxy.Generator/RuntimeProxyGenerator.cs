using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Saneject.RuntimeProxy.Generator;

[Generator]
public class RuntimeProxyGenerator : ISourceGenerator
{
    private const string NamespaceRoot = "Plugins.Saneject.Experimental.Runtime";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ForwardMethodReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not ForwardMethodReceiver receiver)
            return;

        Compilation compilation = context.Compilation;
        INamedTypeSymbol attrSymbol = compilation.GetTypeByMetadataName($"{NamespaceRoot}.Attributes.GenerateRuntimeProxyAttribute");
        INamedTypeSymbol proxyBaseSymbol = compilation.GetTypeByMetadataName($"{NamespaceRoot}.Proxy.RuntimeProxy`1");

        if (attrSymbol is null || proxyBaseSymbol is null)
            return;

        foreach (ClassDeclarationSyntax candidate in receiver.Candidates)
        {
            SemanticModel model = compilation.GetSemanticModel(candidate.SyntaxTree);

            if (model.GetDeclaredSymbol(candidate) is not INamedTypeSymbol classSymbol)
                continue;

            if (!classSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attrSymbol)))
                continue;

            if (classSymbol.BaseType is not { } baseType ||
                !SymbolEqualityComparer.Default.Equals(baseType.ConstructedFrom, proxyBaseSymbol))
                continue;

            if (baseType.TypeArguments.Length < 1)
                continue;

            if (baseType.TypeArguments[0] is not INamedTypeSymbol concreteType)
                continue;

            List<ISymbol> allInterfaces = concreteType.AllInterfaces
                .Where(i => i.DeclaredAccessibility == Accessibility.Public && !i.IsGenericType)
                .Where(i => i.ToDisplayString() != "UnityEngine.ISerializationCallbackReceiver")
                .Where(i => i.ToDisplayString() != "Plugins.Saneject.Experimental.Runtime.Proxy.IRuntimeProxySwapTarget")
                .Distinct(SymbolEqualityComparer.Default)
                .ToList();

            if (allInterfaces.Count == 0)
                continue;

            string ns = classSymbol.ContainingNamespace?.ToDisplayString();
            bool hasNamespace = !string.IsNullOrEmpty(ns) && !classSymbol.ContainingNamespace.IsGlobalNamespace;

            StringBuilder sb = new();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System;");
            sb.AppendLine();

            if (hasNamespace)
            {
                sb.AppendLine($"namespace {ns}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"    public partial class {classSymbol.Name} : {string.Join(", ", allInterfaces.Select(i => i.ToDisplayString()))}");
            sb.AppendLine("    {");

            HashSet<string> generatedMethods = [];
            HashSet<string> generatedProperties = [];
            HashSet<string> generatedEvents = [];

            List<INamedTypeSymbol> interfaces = allInterfaces.OfType<INamedTypeSymbol>().ToList();

            List<IEventSymbol> events = interfaces
                .SelectMany(i => i.GetMembers().OfType<IEventSymbol>())
                .Where(e => e.DeclaredAccessibility == Accessibility.Public)
                .Where(evt => generatedEvents.Add(evt.Name))
                .ToList();

            List<IPropertySymbol> properties = interfaces
                .SelectMany(i => i.GetMembers().OfType<IPropertySymbol>())
                .Where(p => p.DeclaredAccessibility == Accessibility.Public)
                .Where(p => generatedProperties.Add(p.Name))
                .ToList();

            List<IMethodSymbol> methods = interfaces
                .SelectMany(i => i.GetMembers().OfType<IMethodSymbol>())
                .Where(m => m.MethodKind == MethodKind.Ordinary)
                .Where(m => m.DeclaredAccessibility == Accessibility.Public)
                .Where(method => generatedMethods.Add($"{method.Name}({string.Join(",", method.Parameters.Select(p => p.Type.ToDisplayString()))})"))
                .ToList();

            string exceptionMessage =
                $@"Saneject: {classSymbol.Name} was accessed directly before being finalized.\n\n" +
                @"This indicates the object was not swapped by a Scope before use.\n\n" +
                @"Check that:\n" +
                @" • The component is under a Scope.\n" +
                @" • Injection has been executed.\n" +
                @" • Scope has proxy swap targets.\n" +
                @" • Script execution order has not been modified.\n\n" +
                "RuntimeProxy instances are serialized placeholders and should never be accessed directly.";

            sb.AppendLine("        private const string ProxyAccessExceptionMessage =");
            sb.AppendLine($"            \"{exceptionMessage}\";");

            foreach (IEventSymbol evt in events)
            {
                string eventType = evt.Type.ToDisplayString();
                string eventName = evt.Name;

                sb.AppendLine();
                sb.AppendLine($"        public event {eventType} {eventName}");
                sb.AppendLine("        {");
                sb.AppendLine("            add");
                sb.AppendLine("            {");
                sb.AppendLine("                throw new InvalidOperationException(ProxyAccessExceptionMessage);");
                sb.AppendLine("            }");
                sb.AppendLine("            remove");
                sb.AppendLine("            {");
                sb.AppendLine("                throw new InvalidOperationException(ProxyAccessExceptionMessage);");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
            }

            foreach (IPropertySymbol property in properties)
            {
                sb.AppendLine();
                sb.AppendLine($"        public {property.Type.ToDisplayString()} {property.Name}");
                sb.AppendLine("        {");

                if (property.GetMethod != null)
                {
                    sb.AppendLine("            get");
                    sb.AppendLine("            {");
                    sb.AppendLine("                throw new InvalidOperationException(ProxyAccessExceptionMessage);");
                    sb.AppendLine("            }");
                }

                if (property.SetMethod != null)
                {
                    sb.AppendLine("            set");
                    sb.AppendLine("            {");
                    sb.AppendLine("                throw new InvalidOperationException(ProxyAccessExceptionMessage);");
                    sb.AppendLine("            }");
                }

                sb.AppendLine("        }");
            }

            if (properties.Count > 0 && methods.Count > 0)
                sb.AppendLine();

            foreach (IMethodSymbol method in methods)
            {
                string returnType = method.ReturnType.ToDisplayString();
                string[] paramList = method.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}").ToArray();

                sb.AppendLine();
                sb.AppendLine($"        public {returnType} {method.Name}({string.Join(", ", paramList)})");
                sb.AppendLine("        {");
                sb.AppendLine("            throw new InvalidOperationException(ProxyAccessExceptionMessage);");
                sb.AppendLine("        }");
            }

            sb.AppendLine("    }"); // end class

            if (hasNamespace)
                sb.AppendLine("}"); // end namespace

            string safeName = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace("+", "_"); // for nested classes

            context.AddSource($"{safeName}_RuntimeProxy.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    private class ForwardMethodReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Candidates { get; } = [];

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is not ClassDeclarationSyntax cds)
                return;

            if (!cds.AttributeLists.Any(list => list.Attributes.Any(attribute => attribute.Name.ToString().Contains("GenerateRuntimeProxy"))))
                return;

            Candidates.Add(cds);
        }
    }
}
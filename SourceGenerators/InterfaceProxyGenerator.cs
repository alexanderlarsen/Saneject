using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SerializedDIGenerators;

[Generator]
public class InterfaceProxyGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ForwardMethodReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not ForwardMethodReceiver receiver)
            return;

        Compilation compilation = context.Compilation;
        INamedTypeSymbol attrSymbol = compilation.GetTypeByMetadataName("Plugins.Saneject.Runtime.Attributes.GenerateInterfaceProxyAttribute");
        INamedTypeSymbol proxyBaseSymbol = compilation.GetTypeByMetadataName("Plugins.Saneject.Runtime.InterfaceProxy.InterfaceProxyObject`1");

        if (attrSymbol is null || proxyBaseSymbol is null)
            return;

        foreach (ClassDeclarationSyntax candidate in receiver.Candidates)
        {
            SemanticModel model = compilation.GetSemanticModel(candidate.SyntaxTree);

            if (model.GetDeclaredSymbol(candidate) is not INamedTypeSymbol classSymbol)
                continue;

            if (!classSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attrSymbol)))
                continue;

            if (classSymbol.BaseType is not INamedTypeSymbol baseType ||
                baseType.ConstructedFrom is null ||
                !SymbolEqualityComparer.Default.Equals(baseType.ConstructedFrom, proxyBaseSymbol))
                continue;

            if (baseType.TypeArguments.Length < 1)
                continue;

            INamedTypeSymbol concreteType = baseType.TypeArguments[0] as INamedTypeSymbol;

            if (concreteType is null)
                continue;

            List<ISymbol> interfaces = concreteType.AllInterfaces
                .Where(i => i.DeclaredAccessibility == Accessibility.Public && !i.IsGenericType && i.ToDisplayString() != "UnityEngine.ISerializationCallbackReceiver")
                .OfType<INamedTypeSymbol>()
                .Distinct(SymbolEqualityComparer.Default)
                .ToList();

            if (interfaces.Count == 0)
                continue;

            string ns = classSymbol.ContainingNamespace?.ToDisplayString();
            bool hasNamespace = !string.IsNullOrEmpty(ns) && !classSymbol.ContainingNamespace.IsGlobalNamespace;

            string concreteName = concreteType.ToDisplayString();

            StringBuilder sb = new();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            if (hasNamespace)
            {
                sb.AppendLine($"namespace {ns}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"    [CreateAssetMenu(fileName = \"{classSymbol.Name}\", menuName = \"Saneject/Proxy/{classSymbol.Name}\")]");
            sb.AppendLine($"    public partial class {classSymbol.Name} : {string.Join(", ", interfaces.Select(i => i.ToDisplayString()))}");
            sb.AppendLine("    {");

            HashSet<string> generatedMethods = [];
            HashSet<string> generatedProperties = [];
            HashSet<string> generatedEvents = [];

            foreach (INamedTypeSymbol ifaceSymbol in interfaces.OfType<INamedTypeSymbol>())
            {
                foreach (IMethodSymbol method in ifaceSymbol.GetMembers().OfType<IMethodSymbol>())
                {
                    if (method.MethodKind != MethodKind.Ordinary || method.DeclaredAccessibility != Accessibility.Public)
                        continue;

                    string methodKey = $"{method.Name}({string.Join(",", method.Parameters.Select(p => p.Type.ToDisplayString()))})";

                    if (!generatedMethods.Add(methodKey))
                        continue;

                    string returnType = method.ReturnType.ToDisplayString();
                    string[] paramList = method.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}").ToArray();
                    string[] argList = method.Parameters.Select(p => p.Name).ToArray();

                    sb.AppendLine($"        public {returnType} {method.Name}({string.Join(", ", paramList)})");
                    sb.AppendLine("        {");
                    sb.AppendLine("                    if (!instance) { instance = ResolveInstance(); }");
                    string call = $"instance.{method.Name}({string.Join(", ", argList)})";
                    sb.AppendLine(method.ReturnsVoid ? $"            {call};" : $"            return {call};");
                    sb.AppendLine("        }");
                    sb.AppendLine();
                }

                foreach (IPropertySymbol prop in ifaceSymbol.GetMembers().OfType<IPropertySymbol>())
                {
                    if (prop.DeclaredAccessibility != Accessibility.Public)
                        continue;

                    if (!generatedProperties.Add(prop.Name))
                        continue;

                    sb.AppendLine($"        public {prop.Type.ToDisplayString()} {prop.Name}");
                    sb.AppendLine("        {");

                    if (prop.GetMethod != null)
                    {
                        sb.AppendLine("            get");
                        sb.AppendLine("            {");
                        sb.AppendLine("                if (!instance) { instance = ResolveInstance(); }");
                        sb.AppendLine($"                return instance.{prop.Name};");
                        sb.AppendLine("            }");
                    }

                    if (prop.SetMethod != null)
                    {
                        sb.AppendLine("            set");
                        sb.AppendLine("            {");
                        sb.AppendLine("                if (!instance) { instance = ResolveInstance(); }");
                        sb.AppendLine($"                instance.{prop.Name} = value;");
                        sb.AppendLine("            }");
                    }

                    sb.AppendLine("        }");
                    sb.AppendLine();
                }

                foreach (IEventSymbol evt in ifaceSymbol.GetMembers().OfType<IEventSymbol>())
                {
                    if (evt.DeclaredAccessibility != Accessibility.Public)
                        continue;

                    if (!generatedEvents.Add(evt.Name))
                        continue;

                    string eventType = evt.Type.ToDisplayString();
                    string eventName = evt.Name;
                    string subsField = $"__{eventName}Subscriptions";

                    sb.AppendLine($"        private readonly List<({concreteName} target, {eventType} handler)> {subsField} = new();");
                    sb.AppendLine();
                    sb.AppendLine($"        public event {eventType} {eventName}");
                    sb.AppendLine("        {");
                    sb.AppendLine("            add");
                    sb.AppendLine("            {");
                    sb.AppendLine("                 if (!instance) { instance = ResolveInstance(); }");
                    sb.AppendLine("                var target = instance;");
                    sb.AppendLine($"                target.{eventName} += value;");
                    sb.AppendLine($"                {subsField}.Add((target, value));");
                    sb.AppendLine("            }");
                    sb.AppendLine("            remove");
                    sb.AppendLine("            {");
                    sb.AppendLine($"                var sub = {subsField}.Find(x => x.handler == value);");
                    sb.AppendLine("                if (sub.target != null && !sub.target.Equals(null))");
                    sb.AppendLine($"                    sub.target.{eventName} -= value;");
                    sb.AppendLine($"                {subsField}.RemoveAll(x => x.handler == value);");
                    sb.AppendLine("            }");
                    sb.AppendLine("        }");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }"); // end class

            if (hasNamespace)
                sb.AppendLine("}"); // end namespace

            string safeName = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace("+", "_"); // for nested classes

            context.AddSource($"{safeName}_Forwarding.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    private class ForwardMethodReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> Candidates { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax cds &&
                cds.AttributeLists.Any(al =>
                    al.Attributes.Any(a => a.Name.ToString().Contains("GenerateInterfaceProxy"))))
                Candidates.Add(cds);
        }
    }
}
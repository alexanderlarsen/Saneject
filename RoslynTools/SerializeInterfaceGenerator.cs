using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Saneject.Roslyn.Generators;

[Generator]
public class SerializeInterfaceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new InterfaceMemberReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not InterfaceMemberReceiver receiver)
            return;

        Dictionary<INamedTypeSymbol, List<IFieldSymbol>> groupedFields = new(SymbolEqualityComparer.Default);
        Dictionary<INamedTypeSymbol, List<(IPropertySymbol prop, bool hasInject, string idValue)>> groupedProps = new(SymbolEqualityComparer.Default);

        //–– fields ––
        foreach (FieldDeclarationSyntax candidate in receiver.FieldCandidates)
        {
            SemanticModel model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);

            foreach (VariableDeclaratorSyntax variable in candidate.Declaration.Variables)
            {
                if (model.GetDeclaredSymbol(variable) is not IFieldSymbol fieldSym) continue;
                if (!fieldSym.GetAttributes().Any(a => a.AttributeClass?.Name == "SerializeInterfaceAttribute")) continue;
                if (fieldSym.Type.TypeKind != TypeKind.Interface) continue;

                INamedTypeSymbol cls = fieldSym.ContainingType;
                if (!groupedFields.TryGetValue(cls, out List<IFieldSymbol> list)) groupedFields[cls] = list = new List<IFieldSymbol>();
                list.Add(fieldSym);
            }
        }

        //–– props ––
        foreach (PropertyDeclarationSyntax candidate in receiver.PropertyCandidates)
        {
            // only [field: SerializeInterface]
            if (!candidate.AttributeLists
                    .Any(al => al.Target?.Identifier.Text == "field"
                               && al.Attributes.Any(a => a.Name.ToString().Contains("SerializeInterface"))))
                continue;

            SemanticModel model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);
            if (model.GetDeclaredSymbol(candidate) is not IPropertySymbol propSym) continue;
            if (propSym.Type.TypeKind != TypeKind.Interface) continue;

            // detect [field: Inject(...)]
            AttributeSyntax injectSyntax = candidate.AttributeLists
                .Where(al => al.Target?.Identifier.Text == "field")
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(a => a.Name.ToString().Contains("Inject"));

            bool hasInject = injectSyntax != null;
            string idValue = "null";

            if (hasInject && injectSyntax.ArgumentList?.Arguments.Count > 0)
            {
                ExpressionSyntax expr = injectSyntax.ArgumentList.Arguments[0].Expression;

                if (expr is LiteralExpressionSyntax lit && lit.Token.Value is string s)
                    idValue = $"\"{s}\"";
            }

            INamedTypeSymbol cls = propSym.ContainingType;
            if (!groupedProps.TryGetValue(cls, out List<(IPropertySymbol prop, bool hasInject, string idValue)> list)) groupedProps[cls] = list = new List<(IPropertySymbol prop, bool hasInject, string idValue)>();
            list.Add((propSym, hasInject, idValue));
        }

        //–– generate ––
        foreach (INamedTypeSymbol cls in groupedFields.Keys.Union(groupedProps.Keys))
        {
            if (!IsUnitySerializable(cls) || cls.IsSealed) continue;
            string ns = cls.ContainingNamespace?.ToDisplayString();
            bool hasNs = !string.IsNullOrEmpty(ns) && !cls.ContainingNamespace.IsGlobalNamespace;
            StringBuilder sb = new();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Plugins.Saneject.Runtime.Attributes;");
            sb.AppendLine();

            if (hasNs)
            {
                sb.AppendLine($"namespace {ns}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"    public partial class {cls.Name} : UnityEngine.ISerializationCallbackReceiver");
            sb.AppendLine("    {");

            if (groupedFields.TryGetValue(cls, out List<IFieldSymbol> fields))
                foreach (IFieldSymbol f in fields)
                {
                    string bn = "__" + f.Name;
                    AttributeData inj = f.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "InjectAttribute");
                    bool hi = inj != null;
                    string iv = "null";
                    if (hi && inj.ConstructorArguments.Length > 0 && inj.ConstructorArguments[0].Value is string id) iv = $"\"{id}\"";

                    sb.AppendLine(
                        $"        [SerializeField, InterfaceBackingField(typeof({f.Type.ToDisplayString()}), {hi.ToString().ToLowerInvariant()}, {iv})] private UnityEngine.Object {bn};"
                    );
                }

            if (groupedProps.TryGetValue(cls, out List<(IPropertySymbol prop, bool hasInject, string idValue)> props))
                foreach ((IPropertySymbol p, bool hi, string iv) in props)
                {
                    string bn = "__" + p.Name;

                    sb.AppendLine(
                        $"        [SerializeField, InterfaceBackingField(typeof({p.Type.ToDisplayString()}), {hi.ToString().ToLowerInvariant()}, {iv})] private UnityEngine.Object {bn};"
                    );
                }

            sb.AppendLine();
            sb.AppendLine("        public void OnBeforeSerialize() {}");
            sb.AppendLine();
            sb.AppendLine("        public void OnAfterDeserialize()");
            sb.AppendLine("        {");

            if (fields != null)
                foreach (IFieldSymbol f in fields)
                {
                    string bn = "__" + f.Name;
                    sb.AppendLine($"            {f.Name} = {bn} as {f.Type.ToDisplayString()};");
                }

            if (props != null)
                foreach ((IPropertySymbol p, bool _, string _) in props)
                {
                    string bn = "__" + p.Name;
                    sb.AppendLine($"            {p.Name} = {bn} as {p.Type.ToDisplayString()};");
                }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            if (hasNs) sb.AppendLine("}");

            string safeName = cls.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace("+", "_"); // for nested classes

            context.AddSource($"{safeName}_SerializeInterface.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    private static bool InheritsFromUnityObject(INamedTypeSymbol? t)
    {
        while (t != null)
        {
            if (t.ToDisplayString() == "UnityEngine.Object") return true;
            t = t.BaseType;
        }

        return false;
    }

    private static bool IsUnitySerializable(INamedTypeSymbol t)
    {
        return InheritsFromUnityObject(t) || t.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "System.SerializableAttribute");
    }

    private class InterfaceMemberReceiver : ISyntaxReceiver
    {
        public List<FieldDeclarationSyntax> FieldCandidates { get; } = new();
        public List<PropertyDeclarationSyntax> PropertyCandidates { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is FieldDeclarationSyntax f && f.AttributeLists
                    .Any(al => al.Attributes.Any(a => a.Name.ToString().Contains("SerializeInterface"))))
                FieldCandidates.Add(f);

            if (node is PropertyDeclarationSyntax p && p.AttributeLists
                    .Any(al => al.Target?.Identifier.Text == "field"
                               && al.Attributes.Any(a => a.Name.ToString().Contains("SerializeInterface"))))
                PropertyCandidates.Add(p);
        }
    }
}
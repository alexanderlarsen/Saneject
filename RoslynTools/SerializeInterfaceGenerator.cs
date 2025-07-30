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
        Dictionary<INamedTypeSymbol, List<(IPropertySymbol propertySymbol, bool hasInject, string idValue)>> groupedProps = new(SymbolEqualityComparer.Default);
        bool hasInterfaceCollection = false;

        //–– fields ––
        foreach (FieldDeclarationSyntax candidate in receiver.FieldCandidates)
        {
            SemanticModel model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);

            foreach (VariableDeclaratorSyntax variable in candidate.Declaration.Variables)
            {
                if (model.GetDeclaredSymbol(variable) is not IFieldSymbol fieldSymbol)
                    continue;

                if (!fieldSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "SerializeInterfaceAttribute"))
                    continue;

                ITypeSymbol t = fieldSymbol.Type;
                bool isSingle = t.TypeKind == TypeKind.Interface;
                bool isCollection = IsInterfaceCollection(t);
                hasInterfaceCollection |= isCollection;

                if (!isSingle && !isCollection)
                    continue;

                INamedTypeSymbol owner = fieldSymbol.ContainingType;

                if (!groupedFields.TryGetValue(owner, out List<IFieldSymbol> list))
                    groupedFields[owner] = list = new List<IFieldSymbol>();

                list.Add(fieldSymbol);
            }
        }

        //–– props ––
        foreach (PropertyDeclarationSyntax candidate in receiver.PropertyCandidates)
        {
            if (!candidate.AttributeLists.Any(al => al.Target?.Identifier.Text == "field"
                                                    && al.Attributes.Any(a => a.Name.ToString().Contains("SerializeInterface"))))
                continue;

            SemanticModel model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);

            if (model.GetDeclaredSymbol(candidate) is not IPropertySymbol prop)
                continue;

            ITypeSymbol t = prop.Type;
            bool isSingle = t.TypeKind == TypeKind.Interface;
            bool isCollection = IsInterfaceCollection(t);
            hasInterfaceCollection |= isCollection;

            if (!isSingle && !isCollection)
                continue;

            AttributeSyntax injectSyntax = candidate.AttributeLists
                .Where(al => al.Target?.Identifier.Text == "field")
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(a => a.Name.ToString().Contains("Inject"));

            bool hasInject = injectSyntax != null;
            string idValue = "null";

            if (hasInject && injectSyntax.ArgumentList?.Arguments.Count > 0
                          && injectSyntax.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax lit
                          && lit.Token.Value is string s)
                idValue = $"\"{s}\"";

            INamedTypeSymbol owner = prop.ContainingType;

            if (!groupedProps.TryGetValue(owner, out List<(IPropertySymbol propertySymbol, bool hasInject, string idValue)> list))
                groupedProps[owner] = list = new List<(IPropertySymbol, bool, string)>();

            list.Add((prop, hasInject, idValue));
        }

        //–– generate ––
        foreach (INamedTypeSymbol classSymbol in groupedFields.Keys.Union(groupedProps.Keys))
        {
            if (!IsUnitySerializable(classSymbol) || classSymbol.IsSealed)
                continue;

            INamespaceSymbol nsSymbol = classSymbol.ContainingNamespace;
            string ns = nsSymbol.ToDisplayString();
            bool hasNs = !string.IsNullOrEmpty(ns) && !nsSymbol.IsGlobalNamespace;
            StringBuilder sb = new();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Plugins.Saneject.Runtime.Attributes;");
            sb.AppendLine();

            if (hasNs)
            {
                sb.AppendLine($"namespace {ns}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"    public partial class {classSymbol.Name} : ISerializationCallbackReceiver");
            sb.AppendLine("    {");

            // backing fields for fields
            if (groupedFields.TryGetValue(classSymbol, out List<IFieldSymbol> fields))
                foreach (IFieldSymbol fs in fields)
                {
                    string backing = "__" + fs.Name;
                    ITypeSymbol t = fs.Type;
                    string typeStr = t.ToDisplayString();
                    AttributeData injAttr = fs.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "InjectAttribute");
                    string hasInj = injAttr != null ? "true" : "false";
                    string id = "null";

                    if (injAttr?.ConstructorArguments.Length > 0 && injAttr.ConstructorArguments[0].Value is string idv)
                        id = $"\"{idv}\"";

                    if (t is IArrayTypeSymbol arr && arr.ElementType.TypeKind == TypeKind.Interface)
                    {
                        string elementType = arr.ElementType.ToDisplayString();
                        sb.AppendLine($"        /// <summary> Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{fs.Name}\" />. You should not interact with the backing field directly.</summary>");
                        sb.AppendLine($"        [SerializeField, InterfaceBackingField(typeof({elementType}), {hasInj}, {id}), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] private UnityEngine.Object[] {backing};");
                    }
                    else if (t is INamedTypeSymbol named && named.IsGenericType
                                                         && named.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.List<>"
                                                         && named.TypeArguments[0].TypeKind == TypeKind.Interface)
                    {
                        string elementType = named.TypeArguments[0].ToDisplayString();
                        sb.AppendLine($"        /// <summary> Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{fs.Name}\" />. You should not interact with the backing field directly.</summary>");
                        sb.AppendLine($"        [SerializeField, InterfaceBackingField(typeof({elementType}), {hasInj}, {id}), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] private List<UnityEngine.Object> {backing};");
                    }
                    else
                    {
                        sb.AppendLine($"        /// <summary> Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{fs.Name}\" />. You should not interact with the backing field directly.</summary>");
                        sb.AppendLine($"        [SerializeField, InterfaceBackingField(typeof({typeStr}), {hasInj}, {id}), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] private UnityEngine.Object {backing};");
                    }
                }

            // backing fields for props
            if (groupedProps.TryGetValue(classSymbol, out List<(IPropertySymbol propertySymbol, bool hasInject, string idValue)> props))
                foreach ((IPropertySymbol ps, bool hasInjAttr, string idv) in props)
                {
                    string backing = "__" + ps.Name;
                    ITypeSymbol t = ps.Type;
                    string typeStr = t.ToDisplayString();
                    string hasInj = hasInjAttr ? "true" : "false";

                    if (t is IArrayTypeSymbol arr && arr.ElementType.TypeKind == TypeKind.Interface)
                    {
                        string elementType = arr.ElementType.ToDisplayString();
                        sb.AppendLine($"        /// <summary> Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{ps.Name}\" />. You should not interact with the backing field directly.</summary>");
                        sb.AppendLine($"        [SerializeField, InterfaceBackingField(typeof({elementType}), {hasInj}, {idv}), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] private UnityEngine.Object[] {backing};");
                    }
                    else if (t is INamedTypeSymbol named && named.IsGenericType
                                                         && named.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.List<>"
                                                         && named.TypeArguments[0].TypeKind == TypeKind.Interface)
                    {
                        string elementType = named.TypeArguments[0].ToDisplayString();
                        sb.AppendLine($"        /// <summary> Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{ps.Name}\" />. You should not interact with the backing field directly.</summary>");
                        sb.AppendLine($"        [SerializeField, InterfaceBackingField(typeof({elementType}), {hasInj}, {idv}), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] private List<UnityEngine.Object> {backing};");
                    }
                    else
                    {
                        sb.AppendLine($"        /// <summary> Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{ps.Name}\" />. You should not interact with the backing field directly.</summary>");
                        sb.AppendLine($"        [SerializeField, InterfaceBackingField(typeof({typeStr}), {hasInj}, {idv}), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] private UnityEngine.Object {backing};");
                    }
                }

            sb.AppendLine();
            sb.AppendLine("        /// <summary>Auto-generated method used by Saneject to sync interface -> backing field. You should not interact with the method directly.</summary>");
            sb.AppendLine("        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]");
            sb.AppendLine("        public void OnBeforeSerialize()");
            sb.AppendLine("        {");
            sb.AppendLine("        #if UNITY_EDITOR");

            if (groupedFields.TryGetValue(classSymbol, out fields))
                foreach (IFieldSymbol fs in fields)
                {
                    string name = fs.Name;
                    string backing = "__" + name;
                    ITypeSymbol t = fs.Type;

                    if (t.TypeKind == TypeKind.Interface)
                        sb.AppendLine($"            {backing} = {name} as UnityEngine.Object;");
                    else if (t is IArrayTypeSymbol arr && arr.ElementType.TypeKind == TypeKind.Interface)
                        sb.AppendLine($"            {backing} = {name}?.Cast<UnityEngine.Object>().ToArray();");
                    else if (t is INamedTypeSymbol named && named.IsGenericType &&
                             named.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.List<>" &&
                             named.TypeArguments[0].TypeKind == TypeKind.Interface)
                        sb.AppendLine($"            {backing} = {name}?.Cast<UnityEngine.Object>().ToList();");
                }

            if (groupedProps.TryGetValue(classSymbol, out props))
                foreach ((IPropertySymbol ps, _, _) in props)
                {
                    string name = ps.Name;
                    string backing = "__" + name;
                    ITypeSymbol t = ps.Type;

                    if (t.TypeKind == TypeKind.Interface)
                        sb.AppendLine($"            {backing} = {name} as UnityEngine.Object;");
                    else if (t is IArrayTypeSymbol arr && arr.ElementType.TypeKind == TypeKind.Interface)
                        sb.AppendLine($"            {backing} = {name}?.Cast<UnityEngine.Object>().ToArray();");
                    else if (t is INamedTypeSymbol named && named.IsGenericType &&
                             named.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.List<>" &&
                             named.TypeArguments[0].TypeKind == TypeKind.Interface)
                        sb.AppendLine($"            {backing} = {name}?.Cast<UnityEngine.Object>().ToList();");
                }

            sb.AppendLine("        #endif");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Auto-generated method used by Saneject to sync backing field -> interface. You should not interact with the method directly.</summary>");
            sb.AppendLine("        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]");
            sb.AppendLine("        public void OnAfterDeserialize()");
            sb.AppendLine("        {");

            // deserialize fields
            if (groupedFields.TryGetValue(classSymbol, out fields))
                foreach (IFieldSymbol fs in fields)
                {
                    string name = fs.Name;
                    ITypeSymbol t = fs.Type;
                    string backing = "__" + name;

                    if (t is IArrayTypeSymbol arr && arr.ElementType.TypeKind == TypeKind.Interface)
                    {
                        string elem = arr.ElementType.ToDisplayString();
                        sb.AppendLine($"            {name} = ({backing} ?? Array.Empty<UnityEngine.Object>()).Select(x => x as {elem}).ToArray();");
                    }
                    else if (t is INamedTypeSymbol named && named.IsGenericType
                                                         && named.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.List<>"
                                                         && named.TypeArguments[0].TypeKind == TypeKind.Interface)
                    {
                        string elem = named.TypeArguments[0].ToDisplayString();
                        sb.AppendLine($"            {name} = ({backing} ?? new List<UnityEngine.Object>()).Select(x => x as {elem}).ToList();");
                    }
                    else
                    {
                        sb.AppendLine($"            {name} = {backing} as {t.ToDisplayString()};");
                    }
                }

            // deserialize props
            if (groupedProps.TryGetValue(classSymbol, out props))
                foreach ((IPropertySymbol ps, bool _, string _) in props)
                {
                    string name = ps.Name;
                    ITypeSymbol t = ps.Type;
                    string backing = "__" + name;

                    if (t is IArrayTypeSymbol arr && arr.ElementType.TypeKind == TypeKind.Interface)
                    {
                        string elem = arr.ElementType.ToDisplayString();
                        sb.AppendLine($"            {name} = ({backing} ?? Array.Empty<UnityEngine.Object>()).Select(x => x as {elem}).ToArray();");
                    }
                    else if (t is INamedTypeSymbol named && named.IsGenericType
                                                         && named.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.List<>"
                                                         && named.TypeArguments[0].TypeKind == TypeKind.Interface)
                    {
                        string elem = named.TypeArguments[0].ToDisplayString();
                        sb.AppendLine($"            {name} = ({backing} ?? new List<UnityEngine.Object>()).Select(x => x as {elem}).ToList();");
                    }
                    else
                    {
                        sb.AppendLine($"            {name} = {backing} as {t.ToDisplayString()};");
                    }
                }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            if (hasNs) sb.AppendLine("}");

            string hint = classSymbol
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace("+", "_");

            context.AddSource($"{hint}_SerializeInterface.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    private static bool IsInterfaceCollection(ITypeSymbol t)
    {
        if (t is IArrayTypeSymbol arr && arr.ElementType.TypeKind == TypeKind.Interface) return true;

        if (t is INamedTypeSymbol named && named.IsGenericType
                                        && named.TypeArguments.Length == 1
                                        && named.TypeArguments[0].TypeKind == TypeKind.Interface) return true;

        return false;
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
        return InheritsFromUnityObject(t)
               || t.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "System.SerializableAttribute");
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
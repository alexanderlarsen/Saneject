using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Saneject.SerializeInterface.Generator.Legacy.Extensions;
using Saneject.SerializeInterface.Generator.Legacy.Utils;

namespace Saneject.SerializeInterface.Generator.Legacy;

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

        Dictionary<INamedTypeSymbol, List<IFieldSymbol>> fieldsByClass = new(SymbolEqualityComparer.Default);
        Dictionary<INamedTypeSymbol, List<IPropertySymbol>> propertiesByClass = new(SymbolEqualityComparer.Default);
        HashSet<INamedTypeSymbol> classesWithInterfaces = new(SymbolEqualityComparer.Default);

        CollectFields(context, receiver, fieldsByClass, classesWithInterfaces);
        CollectProperties(context, receiver, propertiesByClass, classesWithInterfaces);
        GenerateCode(context, fieldsByClass, propertiesByClass, classesWithInterfaces);
    }

    private static void CollectFields(
        GeneratorExecutionContext context,
        InterfaceMemberReceiver receiver,
        Dictionary<INamedTypeSymbol, List<IFieldSymbol>> fieldsByClass,
        HashSet<INamedTypeSymbol> classesWithInterfaces)
    {
        foreach (FieldDeclarationSyntax candidate in receiver.FieldCandidates)
        {
            SemanticModel model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);

            foreach (VariableDeclaratorSyntax variable in candidate.Declaration.Variables)
            {
                if (model.GetDeclaredSymbol(variable) is not IFieldSymbol fieldSymbol)
                    continue;

                if (!fieldSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "SerializeInterfaceAttribute"))
                    continue;

                if (!fieldSymbol.Type.IsInterface() && !fieldSymbol.Type.IsInterfaceCollection())
                    continue;

                INamedTypeSymbol owner = fieldSymbol.ContainingType;

                if (!fieldsByClass.ContainsKey(owner))
                    fieldsByClass[owner] = [];

                fieldsByClass[owner].Add(fieldSymbol);
                classesWithInterfaces.Add(owner);
            }
        }
    }

    private static void CollectProperties(
        GeneratorExecutionContext context,
        InterfaceMemberReceiver receiver,
        Dictionary<INamedTypeSymbol, List<IPropertySymbol>> propertiesByClass,
        HashSet<INamedTypeSymbol> classesWithInterfaces)
    {
        foreach (PropertyDeclarationSyntax candidate in receiver.PropertyCandidates)
        {
            if (!candidate.AttributeLists.Any(al => al.Target?.Identifier.Text == "field" && al.Attributes.Any(a => a.Name.ToString().Contains("SerializeInterface"))))
                continue;

            SemanticModel model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);

            if (model.GetDeclaredSymbol(candidate) is not IPropertySymbol propertySymbol)
                continue;

            if (!propertySymbol.Type.IsInterface() && !propertySymbol.Type.IsInterfaceCollection())
                continue;

            INamedTypeSymbol owner = propertySymbol.ContainingType;

            if (!propertiesByClass.ContainsKey(owner))
                propertiesByClass[owner] = [];

            propertiesByClass[owner].Add(propertySymbol);
            classesWithInterfaces.Add(owner);
        }
    }

    private static void GenerateCode(
        GeneratorExecutionContext context,
        Dictionary<INamedTypeSymbol, List<IFieldSymbol>> fieldsByClass,
        Dictionary<INamedTypeSymbol, List<IPropertySymbol>> propertiesByClass,
        HashSet<INamedTypeSymbol> classesWithInterfaces)
    {
        IEnumerable<INamedTypeSymbol> allClasses = fieldsByClass.Keys
            .Union(propertiesByClass.Keys, SymbolEqualityComparer.Default)
            .OfType<INamedTypeSymbol>();

        // Generate code
        foreach (INamedTypeSymbol classSymbol in allClasses)
        {
            if (!classSymbol.IsUnitySerializable() || classSymbol.IsSealed)
                continue;

            INamespaceSymbol namespaceSymbol = classSymbol.ContainingNamespace;
            string namespaceName = namespaceSymbol.ToDisplayString();
            bool hasNamespace = !string.IsNullOrEmpty(namespaceName) && !namespaceSymbol.IsGlobalNamespace;
            StringBuilder sb = new();

            sb.AppendLine("using System.Linq;");
            sb.AppendLine();

            if (hasNamespace)
            {
                sb.AppendLine($"namespace {namespaceName}");
                sb.AppendLine("{");
            }

            sb.AppendLine($"    public partial class {classSymbol.Name} : UnityEngine.ISerializationCallbackReceiver");
            sb.AppendLine("    {");

            // Generate backing fields for fields
            if (fieldsByClass.TryGetValue(classSymbol, out List<IFieldSymbol> fieldSymbols))
                for (int i = 0; i < fieldSymbols.Count; i++)
                {
                    IFieldSymbol fieldSymbol = fieldSymbols[i];
                    string name = fieldSymbol.Name;

                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine($"        /// Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{name}\" />. You should not interact with this backing field directly.");
                    sb.AppendLine("        /// </summary>");

                    if (fieldSymbol.Type.IsInterfaceArray(out IArrayTypeSymbol array))
                    {
                        sb.AppendLine($"        [{AttributeUtils.GetAttributes(fieldSymbol, array.ElementType)}]");
                        sb.AppendLine($"        private UnityEngine.Object[] __{name};");
                    }
                    else if (fieldSymbol.Type.IsInterfaceList(out INamedTypeSymbol list))
                    {
                        sb.AppendLine($"        [{AttributeUtils.GetAttributes(fieldSymbol, list.TypeArguments[0])}]");
                        sb.AppendLine($"        private System.Collections.Generic.List<UnityEngine.Object> __{name};");
                    }
                    else
                    {
                        sb.AppendLine($"        [{AttributeUtils.GetAttributes(fieldSymbol, fieldSymbol.Type)}]");
                        sb.AppendLine($"        private UnityEngine.Object __{name};");
                    }

                    if (i != fieldSymbols.Count - 1)
                        sb.AppendLine();
                }

            // Generate bcking fields for properties
            if (propertiesByClass.TryGetValue(classSymbol, out List<IPropertySymbol> propertySymbols))
            {
                if (fieldSymbols is { Count: > 0 })
                    sb.AppendLine();

                for (int i = 0; i < propertySymbols.Count; i++)
                {
                    IPropertySymbol propertySymbol = propertySymbols[i];
                    string name = propertySymbol.Name;

                    IFieldSymbol backingField = propertySymbol.ContainingType
                        .GetMembers()
                        .OfType<IFieldSymbol>()
                        .FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, propertySymbol));

                    ISymbol attributeSymbol = backingField != null ? backingField : propertySymbol;

                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine($"        /// Auto-generated backing field used by Saneject to hold the value for the interface field <see cref=\"{name}\" />. You should not interact with this backing field directly.");
                    sb.AppendLine("        /// </summary>");

                    if (propertySymbol.Type.IsInterfaceArray(out IArrayTypeSymbol array))
                    {
                        sb.AppendLine($"        [{AttributeUtils.GetAttributes(attributeSymbol, array.ElementType)}]");
                        sb.AppendLine($"        private UnityEngine.Object[] __{name};");
                    }
                    else if (propertySymbol.Type.IsInterfaceList(out INamedTypeSymbol list))
                    {
                        sb.AppendLine($"        [{AttributeUtils.GetAttributes(attributeSymbol, list.TypeArguments[0])}]");
                        sb.AppendLine($"        private System.Collections.Generic.List<UnityEngine.Object> __{name};");
                    }
                    else
                    {
                        sb.AppendLine($"        [{AttributeUtils.GetAttributes(attributeSymbol, propertySymbol.Type)}]");
                        sb.AppendLine($"        private UnityEngine.Object __{name};");
                    }

                    if (i != propertySymbols.Count - 1)
                        sb.AppendLine();
                }
            }

            bool baseClassesHaveInterfaces = classesWithInterfaces.ContainsAnyBaseClassOf(classSymbol);

            // Serialize interface objects
            sb.AppendLine();
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Auto-generated method used by Saneject to sync interface -> backing field. You should not interact with the method directly.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        [{AttributeUtils.GetEditorBrowsableAttribute(EditorBrowsableState.Never)}]");

            sb.AppendLine(
                baseClassesHaveInterfaces
                    ? "        public new void OnBeforeSerialize()"
                    : "        public virtual void OnBeforeSerialize()"
            );

            sb.AppendLine("        {");
            sb.AppendLine("#if UNITY_EDITOR");

            if (baseClassesHaveInterfaces)
                sb.AppendLine("            base.OnBeforeSerialize();");

            if (fieldsByClass.TryGetValue(classSymbol, out fieldSymbols))
                foreach (IFieldSymbol fieldSymbol in fieldSymbols)
                {
                    string name = fieldSymbol.Name;

                    if (fieldSymbol.Type.IsInterfaceArray(out _))
                        sb.AppendLine($"            __{name} = {name}?.Cast<UnityEngine.Object>().ToArray();");
                    else if (fieldSymbol.Type.IsInterfaceList(out _))
                        sb.AppendLine($"            __{name} = {name}?.Cast<UnityEngine.Object>().ToList();");
                    else
                        sb.AppendLine($"            __{name} = {name} as UnityEngine.Object;");
                }

            if (propertiesByClass.TryGetValue(classSymbol, out propertySymbols))
                foreach (IPropertySymbol propertySymbol in propertySymbols)
                {
                    string name = propertySymbol.Name;

                    if (propertySymbol.Type.IsInterfaceArray(out _))
                        sb.AppendLine($"            __{name} = {name}?.Cast<UnityEngine.Object>().ToArray();");
                    else if (propertySymbol.Type.IsInterfaceList(out _))
                        sb.AppendLine($"            __{name} = {name}?.Cast<UnityEngine.Object>().ToList();");
                    else
                        sb.AppendLine($"            __{name} = {name} as UnityEngine.Object;");
                }

            sb.AppendLine("#endif");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Auto-generated method used by Saneject to sync backing field -> interface. You should not interact with the method directly.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        [{AttributeUtils.GetEditorBrowsableAttribute(EditorBrowsableState.Never)}]");

            sb.AppendLine(
                baseClassesHaveInterfaces
                    ? "        public new void OnAfterDeserialize()"
                    : "        public virtual void OnAfterDeserialize()"
            );

            sb.AppendLine("        {");

            if (baseClassesHaveInterfaces)
                sb.AppendLine("            base.OnAfterDeserialize();");

            // Deserialize fields
            if (fieldsByClass.TryGetValue(classSymbol, out fieldSymbols))
                foreach (IFieldSymbol fieldSymbol in fieldSymbols)
                {
                    string name = fieldSymbol.Name;

                    if (fieldSymbol.Type.IsInterfaceArray(out IArrayTypeSymbol arr))
                    {
                        string type = arr.ElementType.ToDisplayString();
                        sb.AppendLine($"            {name} = (__{name} ?? System.Array.Empty<UnityEngine.Object>()).Select(x => x as {type}).ToArray();");
                    }
                    else if (fieldSymbol.Type.IsInterfaceList(out INamedTypeSymbol list))
                    {
                        string type = list.TypeArguments[0].ToDisplayString();
                        sb.AppendLine($"            {name} = (__{name} ?? new System.Collections.Generic.List<UnityEngine.Object>()).Select(x => x as {type}).ToList();");
                    }
                    else
                    {
                        string type = fieldSymbol.Type.ToDisplayString();
                        sb.AppendLine($"            {name} = __{name} as {type};");
                    }
                }

            // Deserialize properties
            if (propertiesByClass.TryGetValue(classSymbol, out propertySymbols))
                foreach (IPropertySymbol propertySymbol in propertySymbols)
                {
                    string name = propertySymbol.Name;

                    if (propertySymbol.Type.IsInterfaceArray(out IArrayTypeSymbol array))
                    {
                        string type = array.ElementType.ToDisplayString();
                        sb.AppendLine($"            {name} = (__{name} ?? System.Array.Empty<UnityEngine.Object>()).Select(x => x as {type}).ToArray();");
                    }
                    else if (propertySymbol.Type.IsInterfaceList(out INamedTypeSymbol list))
                    {
                        string type = list.TypeArguments[0].ToDisplayString();
                        sb.AppendLine($"            {name} = (__{name} ?? new System.Collections.Generic.List<UnityEngine.Object>()).Select(x => x as {type}).ToList();");
                    }
                    else
                    {
                        string type = propertySymbol.Type.ToDisplayString();
                        sb.AppendLine($"            {name} = __{name} as {type};");
                    }
                }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            if (hasNamespace) sb.AppendLine("}");

            string hint = classSymbol
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "")
                .Replace(".", "_")
                .Replace("+", "_");

            context.AddSource($"{hint}_SerializeInterface.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    private class InterfaceMemberReceiver : ISyntaxReceiver
    {
        public List<FieldDeclarationSyntax> FieldCandidates { get; } = [];
        public List<PropertyDeclarationSyntax> PropertyCandidates { get; } = [];

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is FieldDeclarationSyntax fieldDeclaration
                && fieldDeclaration.AttributeLists.Any(lists =>
                    lists.Attributes.Any(attribute =>
                        attribute.Name
                            .ToString()
                            .Contains("SerializeInterface"))))
                FieldCandidates.Add(fieldDeclaration);

            if (node is PropertyDeclarationSyntax propertyDeclaration
                && propertyDeclaration.AttributeLists.Any(lists =>
                    lists.Target?.Identifier.Text == "field"
                    && lists.Attributes.Any(attribute =>
                        attribute.Name
                            .ToString()
                            .Contains("SerializeInterface"))))
                PropertyCandidates.Add(propertyDeclaration);
        }
    }
}
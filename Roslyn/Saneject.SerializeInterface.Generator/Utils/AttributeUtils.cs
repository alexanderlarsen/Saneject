using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Saneject.SerializeInterface.Generator.Utils;

public static class AttributeUtils
{
    private const string AttributeNamespaceRoot =
        "Plugins.Saneject.Experimental.Runtime.Attributes";

    public static string GetAttributes(ISymbol symbol)
    {
        List<string> attributes =
        [
            "UnityEngine.SerializeField",
            "UnityEngine.HideInInspector",
            "System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)"
        ];

        foreach (AttributeData attributeData in GetAllRelevantAttributes(symbol))
        {
            string attribute = attributeData.ToString();

            // Strip generator-driving attributes
            if (IsSerializeInterfaceAttribute(attributeData) ||
                IsInjectAttribute(attributeData))
                continue;

            if (attributes.Contains(attribute))
                continue;

            attributes.Add(attribute);
        }

        return string.Join(", ", attributes);
    }

    public static string GetEditorBrowsableAttribute(EditorBrowsableState state)
    {
        return $"System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.{state})";
    }

    public static bool HasSerializeInterfaceAttribute(ISymbol symbol)
    {
        return GetAllRelevantAttributes(symbol).Any(IsSerializeInterfaceAttribute);
    }

    // ------------------------------
    // Internals
    // ------------------------------

    /// <summary>
    /// Returns attributes applied directly to the symbol,
    /// plus backing-field attributes for auto-properties.
    /// </summary>
    private static IEnumerable<AttributeData> GetAllRelevantAttributes(ISymbol symbol)
    {
        // Attributes directly on the symbol
        foreach (AttributeData attr in symbol.GetAttributes())
            yield return attr;

        // Attributes applied via [field: ...] on auto-properties
        if (symbol is IPropertySymbol property)
        {
            IFieldSymbol backingField = property.ContainingType
                .GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(f =>
                    SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, property));

            if (backingField != null)
                foreach (AttributeData attr in backingField.GetAttributes())
                    yield return attr;
        }
    }

    private static bool IsSerializeInterfaceAttribute(AttributeData attr)
    {
        INamedTypeSymbol attrClass = attr.AttributeClass;

        if (attrClass == null)
            return false;

        if (attrClass.Name != "SerializeInterfaceAttribute")
            return false;

        string ns = attrClass.ContainingNamespace?.ToDisplayString();
        return ns != null && ns.StartsWith(AttributeNamespaceRoot);
    }

    private static bool IsInjectAttribute(AttributeData attr)
    {
        INamedTypeSymbol attrClass = attr.AttributeClass;

        if (attrClass == null)
            return false;

        if (attrClass.Name != "InjectAttribute")
            return false;

        string ns = attrClass.ContainingNamespace?.ToDisplayString();
        return ns != null && ns.StartsWith(AttributeNamespaceRoot);
    }
}
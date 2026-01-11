using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Saneject.SerializeInterface.Generator.Legacy.Utils;

public static class AttributeUtils
{
    private const string AttributeNamespaceRoot = "Plugins.Saneject.Legacy.Runtime.Attributes";

    public static string GetAttributes(
        ISymbol symbol,
        ITypeSymbol typeSymbol)
    {
        List<string> attributes =
        [
            "UnityEngine.SerializeField",
            $"Plugins.Saneject.Legacy.Runtime.Attributes.InterfaceBackingFieldAttribute(typeof({typeSymbol.ToDisplayString()}))",
            "System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)"
        ];

        foreach (AttributeData attributeData in symbol.GetAttributes())
        {
            string attribute = attributeData.ToString();

            if (IsSerializeInterfaceAttribute(attributeData))
                continue;

            if (attributes.Contains(attribute))
                continue;

            attributes.Add(attribute);
        }

        return string.Join(", ", attributes);
    }

    public static string GetEditorBrowsableAttribute(EditorBrowsableState state)
    {
        return $"System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.{state.ToString()})";
    }

    public static bool HasSerializeInterfaceAttribute(ISymbol symbol)
    {
        return Enumerable.Any(symbol.GetAttributes(), IsSerializeInterfaceAttribute);
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
}
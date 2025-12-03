using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.CodeAnalysis;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace RoslynTools.Utils;

public static class AttributeUtils
{
    // TODO: Refactor with ISymbol base 
    public static string GetAttributes(
        IFieldSymbol fieldSymbol,
        ITypeSymbol typeSymbol)
    {
        List<string> attributes =
        [
            "UnityEngine.SerializeField",
            $"Plugins.Saneject.Runtime.Attributes.InterfaceBackingFieldAttribute(typeof({typeSymbol.ToDisplayString()}))",
            "System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)"
        ];

        foreach (AttributeData attributeData in fieldSymbol.GetAttributes())
        {
            string attribute = attributeData.ToString();

            if (attribute == "Plugins.Saneject.Runtime.Attributes.SerializeInterfaceAttribute")
                continue;

            if (attributes.Contains(attribute))
                continue;

            attributes.Add(attribute);
        }

        return string.Join(", ", attributes);
    }
    
    public static string GetAttributes(
        IPropertySymbol propertySymbol,
        ITypeSymbol typeSymbol)
    {
        List<string> attributes =
        [
            "UnityEngine.SerializeField",
            $"Plugins.Saneject.Runtime.Attributes.InterfaceBackingFieldAttribute(typeof({typeSymbol.ToDisplayString()}))",
            "System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)"
        ];

        foreach (AttributeData attributeData in propertySymbol.GetAttributes())
        {
            string attribute = attributeData.ToString();

            if (attribute == "Plugins.Saneject.Runtime.Attributes.SerializeInterfaceAttribute")
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
}
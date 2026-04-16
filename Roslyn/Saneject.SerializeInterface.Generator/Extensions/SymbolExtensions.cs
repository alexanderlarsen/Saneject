using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Saneject.SerializeInterface.Generator.Extensions;

public static class SymbolExtensions
{
    public static bool IsInterfaceCollection(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsInterfaceArray(out _) || typeSymbol.IsInterfaceList(out _);
    }

    public static bool IsInterfaceArray(
        this ITypeSymbol typeSymbol,
        out IArrayTypeSymbol array)
    {
        if (typeSymbol is IArrayTypeSymbol { ElementType.TypeKind: TypeKind.Interface } arr)
        {
            array = arr;
            return true;
        }

        array = null;
        return false;
    }

    public static bool IsInterfaceList(
        this ITypeSymbol typeSymbol,
        out INamedTypeSymbol list)
    {
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true } named
            && named.ConstructUnboundGenericType().ToDisplayString() == "System.Collections.Generic.List<>"
            && named.TypeArguments[0].TypeKind == TypeKind.Interface)
        {
            list = named;
            return true;
        }

        list = null;
        return false;
    }

    public static bool IsInterface(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind == TypeKind.Interface;
    }

    public static bool IsUnitySerializable(this INamedTypeSymbol t)
    {
        return t.InheritsFromUnityObject()
               || t.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "System.SerializableAttribute");
    }

    public static bool InheritsFromUnityObject(this INamedTypeSymbol t)
    {
        while (t != null)
        {
            if (t.ToDisplayString() == "UnityEngine.Object") return true;
            t = t.BaseType;
        }

        return false;
    }

    public static bool ContainsAnyBaseClassOf(
        this HashSet<INamedTypeSymbol> lookup,
        INamedTypeSymbol symbol)
    {
        INamedTypeSymbol current = symbol.BaseType;

        while (current != null)
        {
            if (lookup.Contains(current))
                return true;

            current = current.BaseType;
        }

        return false;
    }
}
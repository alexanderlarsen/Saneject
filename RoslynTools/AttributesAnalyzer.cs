using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Saneject.Roslyn.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AttributesAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        "INJ001",
        "[Inject] fields must have [SerializeField] or [SerializeInterface]",
        "Field '{0}' with [Inject] must also have [SerializeField] or [SerializeInterface]",
        "Injection",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor Rule2 = new(
        "INJ002",
        "[SerializeInterface] fields must be interfaces",
        "Field '{0}' is marked with [SerializeInterface] but its type '{1}' is not an interface or a List/Array of interfaces",
        "Injection",
        DiagnosticSeverity.Error,
        true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule, Rule2);

    public override void Initialize(AnalysisContext ctx)
    {
        ctx.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        ctx.EnableConcurrentExecution();
        ctx.RegisterSymbolAction(AnalyzeInjectUsage, SymbolKind.Field, SymbolKind.Property);
        ctx.RegisterSymbolAction(AnalyzeSerializeInterfaceUsage, SymbolKind.Field);
    }

    private void AnalyzeInjectUsage(SymbolAnalysisContext context)
    {
        ISymbol symbol = context.Symbol;
        if (!HasAttribute(symbol, "Inject")) return;
        if (HasAttribute(symbol, "SerializeField") || HasAttribute(symbol, "SerializeInterface")) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name));
    }

    private void AnalyzeSerializeInterfaceUsage(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IFieldSymbol field) return;
        if (!HasAttribute(field, "SerializeInterface")) return;
        
        ITypeSymbol typeSymbol = field.Type;

        // skip single‐interface
        if (typeSymbol.TypeKind == TypeKind.Interface)
            return;

        // skip interface[] 
        if (typeSymbol is IArrayTypeSymbol { ElementType.TypeKind: TypeKind.Interface })
            return;

        // skip List<IFoo> or any single-T generic of an interface
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedTypeSymbol
            && namedTypeSymbol.TypeArguments[0].TypeKind == TypeKind.Interface)
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule2, field.Locations[0], field.Name, field.Type.Name));
    }

    private bool HasAttribute(
        ISymbol symbol,
        string attrName)
    {
        return symbol.GetAttributes().Any(a =>
            a.AttributeClass?.Name == attrName || a.AttributeClass?.Name == attrName + "Attribute");
    }
}
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Saneject.Roslyn.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InjectAttributeCodeFixProvider))]
public class InjectAttributeCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("INJ001", "INJ002");

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var node = root.FindNode(diagnostic.Location.SourceSpan);

        if (node is not VariableDeclaratorSyntax varDecl ||
            varDecl.Parent?.Parent is not FieldDeclarationSyntax fieldDecl)
            return;

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        var symbol = semanticModel.GetDeclaredSymbol(varDecl, context.CancellationToken) as IFieldSymbol;
        bool isInterface = symbol?.Type.TypeKind == TypeKind.Interface;

        switch (diagnostic.Id)
        {
            case "INJ001":
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Add [SerializeField]",
                        c => AddAttributeAsync(context.Document, fieldDecl, "SerializeField", c),
                        "AddSerializeField"),
                    diagnostic);

                if (isInterface)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Add [SerializeInterface]",
                            c => AddAttributeAsync(context.Document, fieldDecl, "SerializeInterface", c),
                            "AddSerializeInterface"),
                        diagnostic);
                }
                break;

            case "INJ002":
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Remove [SerializeInterface]",
                        c => RemoveAttributeAsync(context.Document, fieldDecl, "SerializeInterface", c),
                        "RemoveSerializeInterface"),
                    diagnostic);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Replace [SerializeInterface] with [SerializeField]",
                        c => ReplaceAttributeAsync(context.Document, fieldDecl, "SerializeInterface", "SerializeField", c),
                        "ReplaceSerializeInterface"),
                    diagnostic);
                break;
        }
    }

    private async Task<Document> AddAttributeAsync(
        Document document,
        FieldDeclarationSyntax field,
        string attribute,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);

        var newAttribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attribute));
        var existingList = field.AttributeLists.FirstOrDefault();

        SyntaxList<AttributeListSyntax> updatedLists;

        if (existingList != null)
        {
            var mergedList = existingList.WithAttributes(
                existingList.Attributes.Add(newAttribute));

            updatedLists = field.AttributeLists.Replace(existingList, mergedList);
        }
        else
        {
            var newList = SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(newAttribute));

            updatedLists = field.AttributeLists.Add(newList);
        }

        var newField = field.WithAttributeLists(updatedLists);
        var newRoot = root.ReplaceNode(field, newField);
        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> RemoveAttributeAsync(
        Document document,
        FieldDeclarationSyntax field,
        string attributeName,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);

        var newAttrLists = SyntaxFactory.List(field.AttributeLists
            .Select(al => al.WithAttributes(
                SyntaxFactory.SeparatedList(
                    al.Attributes.Where(attr => !IsMatchingAttribute(attr, attributeName)))))
            .Where(al => al.Attributes.Count > 0));

        var newField = field.WithAttributeLists(newAttrLists);
        var newRoot = root.ReplaceNode(field, newField);
        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> ReplaceAttributeAsync(
        Document document,
        FieldDeclarationSyntax field,
        string oldAttribute,
        string newAttribute,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);

        var newAttrLists = SyntaxFactory.List(field.AttributeLists
            .Select(al => al.WithAttributes(
                SyntaxFactory.SeparatedList(
                    al.Attributes.Select(attr =>
                        IsMatchingAttribute(attr, oldAttribute)
                            ? attr.WithName(SyntaxFactory.IdentifierName(newAttribute))
                            : attr)))));

        var newField = field.WithAttributeLists(newAttrLists);
        var newRoot = root.ReplaceNode(field, newField);
        return document.WithSyntaxRoot(newRoot);
    }

    private bool IsMatchingAttribute(AttributeSyntax attr, string name)
    {
        var id = attr.Name.ToString();
        return id == name || id == name + "Attribute";
    }
}

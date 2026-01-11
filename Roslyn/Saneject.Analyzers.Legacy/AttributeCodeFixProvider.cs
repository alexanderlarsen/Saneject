using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Saneject.Analyzers.Legacy;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeCodeFixProvider))]
public class AttributeCodeFixProvider : CodeFixProvider
{
    private const string RuntimeAttributeRoot =
        "Plugins.Saneject.Legacy.Runtime.Attributes";

    private const string SerializeInterfaceFullName =
        RuntimeAttributeRoot + ".SerializeInterface";

    private const string SerializeFieldFullName =
        "UnityEngine.SerializeField";

    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("INJ001", "INJ002");

    public sealed override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics.First();

        SyntaxNode root = await context.Document
            .GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);

        if (node is not VariableDeclaratorSyntax varDecl ||
            varDecl.Parent?.Parent is not FieldDeclarationSyntax fieldDecl)
            return;

        SemanticModel semanticModel = await context.Document
            .GetSemanticModelAsync(context.CancellationToken)
            .ConfigureAwait(false);

        IFieldSymbol symbol =
            semanticModel.GetDeclaredSymbol(varDecl, context.CancellationToken) as IFieldSymbol;

        bool isInterface = symbol?.Type.TypeKind == TypeKind.Interface;

        switch (diagnostic.Id)
        {
            case "INJ001":
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Add [SerializeField]",
                        c => AddAttributeAsync(
                            context.Document,
                            fieldDecl,
                            SerializeFieldFullName,
                            c),
                        "AddSerializeField"),
                    diagnostic);

                if (isInterface)
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Add [SerializeInterface]",
                            c => AddAttributeAsync(
                                context.Document,
                                fieldDecl,
                                SerializeInterfaceFullName,
                                c),
                            "AddSerializeInterface"),
                        diagnostic);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Make field public",
                        c => MakeFieldPublicAsync(
                            context.Document,
                            fieldDecl,
                            c),
                        "MakePublic"),
                    diagnostic);

                break;

            case "INJ002":
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Remove [SerializeInterface]",
                        c => RemoveAttributeAsync(
                            context.Document,
                            fieldDecl,
                            "SerializeInterface",
                            c),
                        "RemoveSerializeInterface"),
                    diagnostic);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Replace [SerializeInterface] with [SerializeField]",
                        c => ReplaceAttributeAsync(
                            context.Document,
                            fieldDecl,
                            "SerializeInterface",
                            SerializeFieldFullName,
                            c),
                        "ReplaceSerializeInterface"),
                    diagnostic);

                break;
        }
    }

    private async Task<Document> AddAttributeAsync(
        Document document,
        FieldDeclarationSyntax field,
        string fullyQualifiedAttribute,
        CancellationToken cancellationToken)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

        AttributeSyntax newAttribute =
            SyntaxFactory.Attribute(SyntaxFactory.ParseName(fullyQualifiedAttribute));

        AttributeListSyntax existingList = field.AttributeLists.FirstOrDefault();

        SyntaxList<AttributeListSyntax> updatedLists;

        if (existingList != null)
        {
            AttributeListSyntax merged =
                existingList.WithAttributes(existingList.Attributes.Add(newAttribute));

            updatedLists = field.AttributeLists.Replace(existingList, merged);
        }
        else
        {
            AttributeListSyntax newList =
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(newAttribute));

            updatedLists = field.AttributeLists.Add(newList);
        }

        FieldDeclarationSyntax newField = field.WithAttributeLists(updatedLists);
        return document.WithSyntaxRoot(root.ReplaceNode(field, newField));
    }

    private async Task<Document> RemoveAttributeAsync(
        Document document,
        FieldDeclarationSyntax field,
        string attributeName,
        CancellationToken cancellationToken)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

        SyntaxList<AttributeListSyntax> newAttrLists =
            SyntaxFactory.List(
                field.AttributeLists
                    .Select(al =>
                        al.WithAttributes(
                            SyntaxFactory.SeparatedList(
                                al.Attributes.Where(attr =>
                                    !IsMatchingAttribute(attr, attributeName)))))
                    .Where(al => al.Attributes.Count > 0));

        FieldDeclarationSyntax newField = field.WithAttributeLists(newAttrLists);
        return document.WithSyntaxRoot(root.ReplaceNode(field, newField));
    }

    private async Task<Document> ReplaceAttributeAsync(
        Document document,
        FieldDeclarationSyntax field,
        string oldAttributeName,
        string newFullyQualifiedAttribute,
        CancellationToken cancellationToken)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

        SyntaxList<AttributeListSyntax> newAttrLists =
            SyntaxFactory.List(
                field.AttributeLists.Select(al =>
                    al.WithAttributes(
                        SyntaxFactory.SeparatedList(
                            al.Attributes.Select(attr =>
                                IsMatchingAttribute(attr, oldAttributeName)
                                    ? attr.WithName(
                                        SyntaxFactory.ParseName(newFullyQualifiedAttribute))
                                    : attr)))));

        FieldDeclarationSyntax newField = field.WithAttributeLists(newAttrLists);
        return document.WithSyntaxRoot(root.ReplaceNode(field, newField));
    }

    private static bool IsMatchingAttribute(
        AttributeSyntax attr,
        string name)
    {
        string id = attr.Name.ToString();
        return id == name || id == name + "Attribute";
    }

    private async Task<Document> MakeFieldPublicAsync(
        Document document,
        FieldDeclarationSyntax field,
        CancellationToken cancellationToken)
    {
        SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

        SyntaxTokenList newModifiers =
            SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        FieldDeclarationSyntax newField = field.WithModifiers(newModifiers);
        return document.WithSyntaxRoot(root.ReplaceNode(field, newField));
    }
}
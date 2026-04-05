#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FutureMUD_Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FutureProgStatementCodeFixProvider)), Shared]
public sealed class FutureProgStatementCodeFixProvider : CodeFixProvider
{
    private const string Title = "Implement missing method";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FutureProgStatementDiagnosticAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics.First();
        TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

        TypeDeclarationSyntax? declaration = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault();

        if (declaration is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: cancellationToken => ImplementFutureProgStatementCompilerRegisterAsync(context.Document, declaration, cancellationToken),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> ImplementFutureProgStatementCompilerRegisterAsync(
        Document document,
        TypeDeclarationSyntax declaration,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        Workspace workspace = document.Project.Solution.Workspace;
        SyntaxGenerator syntaxGenerator = SyntaxGenerator.GetGenerator(document);
        MethodDeclarationSyntax method = (MethodDeclarationSyntax)syntaxGenerator.MethodDeclaration(
                "RegisterCompiler",
                parameters: System.Array.Empty<SyntaxNode>(),
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Static)
            .WithAdditionalAnnotations(Formatter.Annotation);

        TypeDeclarationSyntax newDeclaration = declaration.AddMembers(method);
        SyntaxNode newRoot = root.ReplaceNode(declaration, newDeclaration);
        newRoot = Formatter.Format(newRoot, Formatter.Annotation, workspace);

        return document.WithSyntaxRoot(newRoot);
    }
}

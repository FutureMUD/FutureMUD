using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace FutureMUD_Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FutureProgStatementCodeFixProvider)), Shared]
    public class FutureProgStatementCodeFixProvider : CodeFixProvider
    {
        private const string title = "Implement missing method";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FutureProgStatementDiagnosticAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ImplementFutureProgStatementCompilerRegister(context, context.Diagnostics.First().Location.SourceSpan, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> ImplementFutureProgStatementCompilerRegister(CodeFixContext context, TextSpan sourceSpan, CancellationToken cancellationToken) {
            var document = context.Document;
            var workspace = document.Project.Solution.Workspace;
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var exprFromAnalyzer = root.FindNode(sourceSpan);
            var classDeclaration = (ClassDeclarationSyntax)exprFromAnalyzer;
            var syntaxGenerator = SyntaxGenerator.GetGenerator(document);
            var newMethod = (MethodDeclarationSyntax)syntaxGenerator.MethodDeclaration("RegisterCompiler", new List<SyntaxNode>(),
                                              accessibility: Accessibility.Public,
                                              modifiers: DeclarationModifiers.Static).WithAdditionalAnnotations(Formatter.Annotation);
            
            var newClassDeclaration = classDeclaration.AddMembers(newMethod);
            var newRoot = Formatter.Format(root.ReplaceNode(classDeclaration, newClassDeclaration), Formatter.Annotation, workspace);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
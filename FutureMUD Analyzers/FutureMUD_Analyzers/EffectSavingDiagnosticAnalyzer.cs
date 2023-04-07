using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualBasic;

namespace FutureMUD_Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class EffectSavingDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FutureMUD_Analyzers_Effect_Saving";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.EffectSavingAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.EffectSavingAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.EffectSavingAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "FutureMUD Effects";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Only examine concrete types
            if (namedTypeSymbol.IsAbstract)
            {
                return;
            }

            // Find all named type symbols that implement the IEffect interface in MudSharp.Effects
            if (namedTypeSymbol.AllInterfaces.Any(x => x.MetadataName.Equals("IEffect")))
            {
                foreach (var item in namedTypeSymbol.GetMembers("SavingEffect")) {
                    if (item.Kind != SymbolKind.Property || !(item is IPropertySymbol ps) || !ps.GetMethod.IsOverride) {
                        continue;
                    }

                    var syntaxTree = ps.GetMethod.DeclaringSyntaxReferences.First().SyntaxTree;
                    var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
                    var root = syntaxTree.GetRoot();
                    var getFunctionNode = (PropertyDeclarationSyntax) root.FindNode(ps.Locations.First().SourceSpan);

                    bool IsOrContainsLiteralTrueReturnExpression(SyntaxNode node) {
                        if (node is ReturnStatementSyntax rss && rss.Expression is LiteralExpressionSyntax les &&
                            semanticModel.GetConstantValue(les).Value is bool bv && bv) {
                            return true;
                        }

                        if (node is LiteralExpressionSyntax tles &&
                            semanticModel.GetConstantValue(tles).Value is bool tbv && tbv) {
                            return true;
                        }

                        return false;
                    }

                    if (getFunctionNode.ExpressionBody?.Expression?.DescendantNodesAndSelf().Any(IsOrContainsLiteralTrueReturnExpression) == true) {
                        if (namedTypeSymbol.GetMembers("InitialiseEffectType").Any(
                            x => x.IsStatic && x.Kind == SymbolKind.Method && x is IMethodSymbol ms && ms.ReturnsVoid &&
                                 !ms.Parameters.Any())) {
                            // Nothing to see here, go home
                            return;
                        }
                        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}

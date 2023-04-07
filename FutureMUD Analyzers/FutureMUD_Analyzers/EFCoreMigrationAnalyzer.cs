using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FutureMUD_Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class EFCoreMigrationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FutureMUD_Analyzers_EFCore_Migration";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.EFCoreMigrationTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.EFCoreMigrationMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.EFCoreMigrationDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "EF Core";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;
            var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpr?.Name.ToString() != "Clear")
            {
                return;
            }

            var memberSymbol = context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
            if (memberSymbol.ContainingType.MetadataName != "System.Collections.Generic.ICollection`1")
            {
                return;
            }

            var childAccessExpr = memberAccessExpr.Expression as MemberAccessExpressionSyntax;
            if (childAccessExpr == null)
            {
                return;
            }

            var childSymbol = context.SemanticModel.GetSymbolInfo(childAccessExpr).Symbol as IPropertySymbol;
            if (childSymbol == null)
            {
                return;
            }
            
            if (childSymbol.ContainingNamespace.ToString() != "MudSharp.Models")
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), memberSymbol.Name, childSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}

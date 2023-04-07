using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FutureMUD_Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GameItemComponentProtoDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FutureMUD_Analyzers_GameItemComponentProtoDiagnosticAnalyzer";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.GameItemComponentProtoAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.GameItemComponentProtoAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.GameItemComponentProtoAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        internal const string Category = "FutureMUD GameItems";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

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

            // Find all named type symbols that implement the IGameItemComponentProto interface in MudSharp.GameItems
            if (namedTypeSymbol.AllInterfaces.Any(x => x.MetadataName.Equals("IGameItemComponentProto")))
            {
                if (namedTypeSymbol.GetMembers("RegisterComponentInitialiser").Any(x => x.IsStatic && x.Kind == SymbolKind.Method && x is IMethodSymbol ms && ms.ReturnsVoid && ms.Parameters.Count() == 1 && ms.Parameters[0].Type.MetadataName.Equals("GameItemComponentManager")))
                {
                    // Nothing to see here, go home
                    return;
                }

                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}

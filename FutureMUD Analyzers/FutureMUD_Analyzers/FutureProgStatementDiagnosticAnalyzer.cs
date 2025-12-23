#nullable enable
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FutureMUD_Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FutureProgStatementDiagnosticAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "FutureMUD_Analyzers_FutureProg_Statement";

	private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.FutureProgStatementAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.FutureProgStatementAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.FutureProgStatementAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
	private const string Category = "FutureMUD Progs";

	private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
		{
			return;
		}

		if (namedTypeSymbol.TypeKind != TypeKind.Class || namedTypeSymbol.IsAbstract || namedTypeSymbol.IsImplicitlyDeclared)
		{
			return;
		}

		if (!ImplementsInterface(namedTypeSymbol, "IStatement", "MudSharp.FutureProg"))
		{
			return;
		}

		if (ImplementsInterface(namedTypeSymbol, "IFunction", "MudSharp.FutureProg"))
		{
			return;
		}

		if (namedTypeSymbol.GetMembers("RegisterCompiler")
			.OfType<IMethodSymbol>()
			.Any(IsRegisterCompilerMethod))
		{
			return;
		}

		var location = namedTypeSymbol.Locations.FirstOrDefault(l => l.IsInSource);
		if (location is null)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rule, location, namedTypeSymbol.Name));
	}

	private static bool ImplementsInterface(INamedTypeSymbol symbol, string interfaceName, string interfaceNamespace)
	{
		return symbol.AllInterfaces.Any(interfaceSymbol =>
			interfaceSymbol.Name == interfaceName &&
			interfaceSymbol.ContainingNamespace.ToDisplayString() == interfaceNamespace);
	}

	private static bool IsRegisterCompilerMethod(IMethodSymbol method)
	{
		return method.IsStatic &&
			method.DeclaredAccessibility == Accessibility.Public &&
			method.ReturnsVoid &&
			method.Parameters.Length == 0;
	}
}

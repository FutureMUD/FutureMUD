#nullable enable
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FutureMUD_Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class GameItemComponentProtoDiagnosticAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "FutureMUD_Analyzers_GameItemComponentProtoDiagnosticAnalyzer";
	private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.GameItemComponentProtoAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.GameItemComponentProtoAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.GameItemComponentProtoAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
	internal const string Category = "FutureMUD GameItems";

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

		if (!ImplementsInterface(namedTypeSymbol, "IGameItemComponentProto", "MudSharp.GameItems"))
		{
			return;
		}

		if (namedTypeSymbol.GetMembers("RegisterComponentInitialiser")
			.OfType<IMethodSymbol>()
			.Any(IsRegisterComponentInitialiserMethod))
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

	private static bool IsRegisterComponentInitialiserMethod(IMethodSymbol method)
	{
		return method.IsStatic &&
			method.DeclaredAccessibility == Accessibility.Public &&
			method.ReturnsVoid &&
			method.Parameters.Length == 1 &&
			IsType(method.Parameters[0].Type, "GameItemComponentManager", "MudSharp.GameItems");
	}

	private static bool IsType(ITypeSymbol typeSymbol, string typeName, string typeNamespace)
	{
		return typeSymbol.Name == typeName &&
			typeSymbol.ContainingNamespace.ToDisplayString() == typeNamespace;
	}
}

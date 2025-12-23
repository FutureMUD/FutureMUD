#nullable enable
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FutureMUD_Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EffectSavingDiagnosticAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "FutureMUD_Analyzers_Effect_Saving";

	private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.EffectSavingAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.EffectSavingAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
	private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.EffectSavingAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
	private const string Category = "FutureMUD Effects";

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

		if (!ImplementsInterface(namedTypeSymbol, "IEffect", "MudSharp.Effects"))
		{
			return;
		}

		var savingProperty = namedTypeSymbol.GetMembers("SavingEffect")
			.OfType<IPropertySymbol>()
			.FirstOrDefault(property => property.GetMethod is { IsOverride: true });

		if (savingProperty is null)
		{
			return;
		}

		if (!PropertyAlwaysReturnsTrue(savingProperty))
		{
			return;
		}

		if (namedTypeSymbol.GetMembers("InitialiseEffectType")
			.OfType<IMethodSymbol>()
			.Any(IsInitialiseEffectTypeMethod))
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

	private static bool IsInitialiseEffectTypeMethod(IMethodSymbol method)
	{
		return method.IsStatic &&
			method.DeclaredAccessibility == Accessibility.Public &&
			method.ReturnsVoid &&
			method.Parameters.Length == 0;
	}

	private static bool PropertyAlwaysReturnsTrue(IPropertySymbol propertySymbol)
	{
		foreach (var syntaxReference in propertySymbol.DeclaringSyntaxReferences)
		{
			if (syntaxReference.GetSyntax() is not PropertyDeclarationSyntax propertyDeclaration)
			{
				continue;
			}

			if (IsTrueLiteral(propertyDeclaration.ExpressionBody?.Expression))
			{
				return true;
			}

			if (IsTrueLiteral(propertyDeclaration.Initializer?.Value))
			{
				return true;
			}

			var getter = propertyDeclaration.AccessorList?.Accessors
				.FirstOrDefault(accessor => accessor.IsKind(SyntaxKind.GetAccessorDeclaration));

			if (IsTrueLiteral(getter?.ExpressionBody?.Expression))
			{
				return true;
			}

			if (getter?.Body is null)
			{
				continue;
			}

			var returnStatements = getter.Body.DescendantNodes()
				.OfType<ReturnStatementSyntax>()
				.ToList();

			if (returnStatements.Count == 0)
			{
				continue;
			}

			if (returnStatements.All(statement => IsTrueLiteral(statement.Expression)))
			{
				return true;
			}
		}

		return false;
	}

	private static bool IsTrueLiteral(ExpressionSyntax? expression)
	{
		if (expression is null)
		{
			return false;
		}

		while (expression is ParenthesizedExpressionSyntax parenthesized)
		{
			expression = parenthesized.Expression;
		}

		return expression.IsKind(SyntaxKind.TrueLiteralExpression);
	}
}

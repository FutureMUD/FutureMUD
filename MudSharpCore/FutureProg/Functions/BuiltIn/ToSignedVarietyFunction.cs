#nullable enable

using MudSharp.Communication.Language;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class ToSignedVarietyFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ToSignedVarietyFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.SignedVariety;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var identifier = ParameterFunctions[^1].Result?.GetObject;
		if (identifier is null)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		if (ParameterFunctions.Count == 1)
		{
			Result = _gameworld.SignedLanguages
				.SelectMany(x => x.Varieties)
				.Get((long)(decimal)identifier) ?? (IProgVariable)new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ISignedLanguage language)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var result = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? language.Varieties.FirstOrDefault(x => x.Name.EqualTo((string)identifier))
			: language.Varieties.Get((long)(decimal)identifier);
		Result = result is not null ? result : new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var name in new[] { "tosignedvariety", "tosignedlanguagevariety" })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name,
				[ProgVariableTypes.Number], (pars, world) => new ToSignedVarietyFunction(pars, world),
				["id"], ["The globally unique ID to find."],
				"Returns a signed variety by ID, or null if absent.", "Lookup", ProgVariableTypes.SignedVariety));

			foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
			{
				FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name,
					[ProgVariableTypes.SignedLanguage, type], (pars, world) => new ToSignedVarietyFunction(pars, world),
					["language", "identifier"], ["The signed language to search within.", "The ID or case-insensitive name to find within the owner."],
					"Returns a signed variety within the supplied signed language, or null if either is absent. Numeric arguments are IDs.",
					"Lookup", ProgVariableTypes.SignedVariety));
			}
		}
	}
}

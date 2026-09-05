#nullable enable

using MudSharp.RPG.Law;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class ToLawFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ToLawFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Law;
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
			Result = _gameworld.LegalAuthorities
				.SelectMany(x => x.Laws)
				.Get((long)(decimal)identifier) ?? (IProgVariable)new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ILegalAuthority authority)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var result = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? authority.Laws.FirstOrDefault(x => x.Name.EqualTo((string)identifier))
			: authority.Laws.Get((long)(decimal)identifier);
		Result = result is not null ? result : new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("tolaw",
			[ProgVariableTypes.Number], (pars, world) => new ToLawFunction(pars, world),
			["id"], ["The globally unique ID to find."],
			"Returns a law by ID, or null if absent.", "Lookup", ProgVariableTypes.Law));

		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("tolaw",
				[ProgVariableTypes.LegalAuthority, type], (pars, world) => new ToLawFunction(pars, world),
				["authority", "identifier"], ["The legal authority to search within.", "The ID or case-insensitive name to find within the owner."],
				"Returns a law within the supplied legal authority, or null if either is absent. Numeric arguments are IDs.",
				"Lookup", ProgVariableTypes.Law));
		}
	}
}

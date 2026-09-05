#nullable enable

using MudSharp.Economy;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class ToMerchandiseFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ToMerchandiseFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Merchandise;
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
			Result = _gameworld.Shops
				.SelectMany(x => x.Merchandises)
				.Get((long)(decimal)identifier) ?? (IProgVariable)new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IShop shop)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var result = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? shop.Merchandises.FirstOrDefault(x => x.Name.EqualTo((string)identifier))
			: shop.Merchandises.Get((long)(decimal)identifier);
		Result = result is not null ? result : new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("tomerchandise",
			[ProgVariableTypes.Number], (pars, world) => new ToMerchandiseFunction(pars, world),
			["id"], ["The globally unique ID to find."],
			"Returns a merchandise by ID, or null if absent.", "Lookup", ProgVariableTypes.Merchandise));

		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("tomerchandise",
				[ProgVariableTypes.Shop, type], (pars, world) => new ToMerchandiseFunction(pars, world),
				["shop", "identifier"], ["The shop to search within.", "The ID or case-insensitive name to find within the owner."],
				"Returns a merchandise within the supplied shop, or null if either is absent. Numeric arguments are IDs.",
				"Lookup", ProgVariableTypes.Merchandise));
		}
	}
}

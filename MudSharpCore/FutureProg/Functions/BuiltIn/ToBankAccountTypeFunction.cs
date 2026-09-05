#nullable enable

using MudSharp.Economy;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class ToBankAccountTypeFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ToBankAccountTypeFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.BankAccountType;
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
			Result = _gameworld.Banks
				.SelectMany(x => x.BankAccountTypes)
				.Get((long)(decimal)identifier) ?? (IProgVariable)new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IBank bank)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var result = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo((string)identifier))
			: bank.BankAccountTypes.Get((long)(decimal)identifier);
		Result = result is not null ? result : new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("tobankaccounttype",
			[ProgVariableTypes.Number], (pars, world) => new ToBankAccountTypeFunction(pars, world),
			["id"], ["The globally unique ID to find."],
			"Returns a bank account type by ID, or null if absent.", "Lookup", ProgVariableTypes.BankAccountType));

		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("tobankaccounttype",
				[ProgVariableTypes.Bank, type], (pars, world) => new ToBankAccountTypeFunction(pars, world),
				["bank", "identifier"], ["The bank to search within.", "The ID or case-insensitive name to find within the owner."],
				"Returns a bank account type within the supplied bank, or null if either is absent. Numeric arguments are IDs.",
				"Lookup", ProgVariableTypes.BankAccountType));
		}
	}
}

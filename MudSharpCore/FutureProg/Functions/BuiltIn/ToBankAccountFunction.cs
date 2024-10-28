using System.Collections.Generic;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToBankAccountFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"tobankaccount",
				[ProgVariableTypes.Number], // the parameters the function takes
				(pars, gameworld) => new ToBankAccountFunction(pars, gameworld),
				new List<string>
				{
					"id"
				}, // parameter names
				new List<string>
				{
					"The ID of the bank account"
				}, // parameter help text
				"Returns the bank account referenced by the ID, or null if not found", // help text for the function,
				"Lookup", // the category to which this function belongs,
				ProgVariableTypes.BankAccount // the return type of the function
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"tobankaccount",
				[ProgVariableTypes.Text], // the parameters the function takes
				(pars, gameworld) => new ToBankAccountFunction(pars, gameworld),
				new List<string>
				{
					"code"
				}, // parameter names
				new List<string>
				{
					"The code of the bank account in the form bank:accn"
				}, // parameter help text
				"Returns the bank account referenced by the code, or null if not found", // help text for the function,
				"Lookup", // the category to which this function belongs,
				ProgVariableTypes.BankAccount // the return type of the function
			)
		);
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"tobankaccount",
				[
					ProgVariableTypes.Bank,
					ProgVariableTypes.Text
				], // the parameters the function takes
				(pars, gameworld) => new ToBankAccountFunction(pars, gameworld),
				new List<string>
				{
					"bank",
					"code"
				}, // parameter names
				new List<string>
				{
					"The home bank",
					"The code of the bank account in the form bank:accn or just an accn for the bank"
				}, // parameter help text
				"Returns the bank account referenced by the code, or null if not found", // help text for the function,
				"Lookup", // the category to which this function belongs,
				ProgVariableTypes.BankAccount // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected ToBankAccountFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.BankAccount; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Number))
		{
			var id = (long)(decimal)ParameterFunctions[0].Result.GetObject;
			Result = Gameworld.BankAccounts.Get(id);
			return StatementResult.Normal;
		}

		
		if (ParameterFunctions.Count == 1)
		{
			(Result, _) = Bank.FindBankAccount((string)ParameterFunctions[0].Result.GetObject, null, null);
			return StatementResult.Normal;
		}

		var bank = ParameterFunctions[0].Result?.GetObject as IBank;
		(Result, _) = Bank.FindBankAccount((string)ParameterFunctions[1].Result.GetObject, bank, null);
		return StatementResult.Normal;
	}
}
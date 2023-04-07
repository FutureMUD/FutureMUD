using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.FutureProg.Functions.Currency;

internal class LoadCurrencyFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public LoadCurrencyFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Item;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var currency = (ICurrency)ParameterFunctions[0].Result;
		if (currency == null)
		{
			ErrorMessage = "Null currency in LoadCurrency function.";
			return StatementResult.Error;
		}

		var success = true;
		var amount = ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Number)
			? (decimal)ParameterFunctions[1].Result.GetObject
			: currency.GetBaseCurrency((string)ParameterFunctions[1].Result.GetObject, out success);

		if (!success)
		{
			ErrorMessage = "Incorrect currency amount for currency " + currency.Name + ": " +
			               (string)ParameterFunctions[1].Result.GetObject;
			return StatementResult.Error;
		}

		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			currency.FindCoinsForAmount(amount, out success).Select(x => Tuple.Create(x.Key, x.Value)));
		_gameworld.Add(newItem);
		Result = newItem;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcurrency",
			new[] { FutureProgVariableTypes.Currency, FutureProgVariableTypes.Number },
			(pars, gameworld) => new LoadCurrencyFunction(pars, gameworld)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcurrency",
			new[] { FutureProgVariableTypes.Currency, FutureProgVariableTypes.Text },
			(pars, gameworld) => new LoadCurrencyFunction(pars, gameworld)
		));
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.FutureProg.Functions.Currency;

internal class LoadCoinsFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public LoadCoinsFunction(IList<IFunction> parameters, IFuturemud gameworld)
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
			ErrorMessage = "Null currency in LoadCoins function.";
			return StatementResult.Error;
		}

		var ss = new StringStack((string)ParameterFunctions[1].Result.GetObject);
		var coins = new Dictionary<ICoin, int>();
		while (true)
		{
			var samount = ss.Pop();
			if (string.IsNullOrEmpty(samount))
			{
				ErrorMessage = "You must enter the specific coins which you want to load.";
				return StatementResult.Error;
			}

			if (!int.TryParse(samount, out var amount))
			{
				ErrorMessage = "You must enter a whole number of coins of each type to load.";
				return StatementResult.Error;
			}

			var scoin = ss.PopSpeech();
			if (string.IsNullOrEmpty(scoin))
			{
				ErrorMessage = "Which coin do you want to load " + amount + " of?";
				return StatementResult.Error;
			}

			var coin =
				currency.Coins.FirstOrDefault(
					x =>
						x.Name.StartsWith(scoin, StringComparison.InvariantCultureIgnoreCase) ||
						x.Name.Replace(x.PluralWord, x.PluralWord.Pluralise())
						 .StartsWith(scoin, StringComparison.InvariantCultureIgnoreCase));
			if (coin == null)
			{
				ErrorMessage = "There is no such coin as \"" + scoin + "\" for this currency.";
				return StatementResult.Error;
			}

			if (coins.ContainsKey(coin))
			{
				ErrorMessage = "You cannot specify the same coin twice.";
				return StatementResult.Error;
			}

			coins.Add(coin, amount);
			if (ss.IsFinished)
			{
				break;
			}
		}

		var newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
			coins.Select(x => Tuple.Create(x.Key, x.Value)));
		_gameworld.Add(newItem);
		Result = newItem;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadcoins",
			new[] { FutureProgVariableTypes.Currency, FutureProgVariableTypes.Text },
			(pars, gameworld) => new LoadCoinsFunction(pars, gameworld)
		));
	}
}
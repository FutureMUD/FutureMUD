using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.FutureProg.Functions.Currency;

internal class TakeCurrencyFunction : BuiltInFunction
{
	public TakeCurrencyFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"takecurrency",
			new[]
			{
				ProgVariableTypes.Currency, ProgVariableTypes.Number | ProgVariableTypes.Text,
				ProgVariableTypes.Location | ProgVariableTypes.Character | ProgVariableTypes.Item,
				ProgVariableTypes.Boolean, ProgVariableTypes.Boolean
			},
			(pars, gameworld) => new TakeCurrencyFunction(pars)
		));
	}

	#region Overrides of Function

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	#region Overrides of BuiltInFunction

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var currency = (ICurrency)ParameterFunctions[0].Result;
		if (currency == null)
		{
			ErrorMessage = "Null currency in TakeCurrency function.";
			return StatementResult.Error;
		}

		var respectGetRules = (bool?)ParameterFunctions[3].Result.GetObject ?? false;
		var giveChange = (bool?)ParameterFunctions[4].Result.GetObject ?? false;

		var success = true;
		var amount = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? (decimal)ParameterFunctions[1].Result.GetObject
			: currency.GetBaseCurrency((string)ParameterFunctions[1].Result.GetObject, out success);

		if (!success)
		{
			ErrorMessage = "Incorrect currency amount for currency " + currency.Name + ": " +
			               (string)ParameterFunctions[1].Result.GetObject;
			return StatementResult.Error;
		}

		var parameter1 = ParameterFunctions[2].Result;
		if (parameter1 == null)
		{
			ErrorMessage = "Null Character, Location or Item given to TakeCurrency function.";
			return StatementResult.Error;
		}

		var targetPiles = Enumerable.Empty<ICurrencyPile>();
		var gameitem = parameter1 as IGameItem;
		if (gameitem != null)
		{
			targetPiles = gameitem.RecursiveGetItems<ICurrencyPile>(respectGetRules);
		}

		var location = parameter1 as ICell;
		if (location != null)
		{
			targetPiles = location.GameItems.RecursiveGetItems<ICurrencyPile>(respectGetRules);
		}

		var character = parameter1 as ICharacter;
		if (character != null)
		{
			targetPiles = character.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(respectGetRules);
		}

		if (!targetPiles.Any())
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var targetCoins = currency.FindCurrency(targetPiles, amount);

		var value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
		if (value < amount)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var changeAmount = value - amount;

		foreach (
			var item in
			targetCoins.Where(item => !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		if (changeAmount > 0 && giveChange)
		{
			var coins = currency.FindCoinsForAmount(changeAmount, out success);
			var changeItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
				coins.Select(x => Tuple.Create(x.Key, x.Value)));

			if (location != null)
			{
				location.Insert(changeItem);
			}
			else if (character != null)
			{
				if (character.Body.CanGet(changeItem, 0))
				{
					character.Body.Get(changeItem, silent: true);
				}
				else
				{
					changeItem.RoomLayer = character.RoomLayer;
					character.Location.Insert(changeItem);
				}
			}
			else if (gameitem != null)
			{
				var gameItemContainer = gameitem.GetItemType<IContainer>();
				if (gameItemContainer?.CanPut(changeItem) == true)
				{
					gameItemContainer.Put(null, changeItem);
				}
				else
				{
					gameitem.TrueLocations.FirstOrDefault()?.Insert(changeItem);
				}
			}
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	#endregion

	#endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.Currency;

internal class GiveCurrencyFunction : BuiltInFunction
{
	public static bool GiveCurrency(ICurrency currency, decimal amount, object target, bool respectGetRules)
	{
		var gameworld = currency.Gameworld;
		ICurrencyPile targetPile = null;
		if (target is IGameItem gameitem)
		{
			targetPile = gameitem.RecursiveGetItems<ICurrencyPile>(respectGetRules).FirstOrDefault();
			if (targetPile == null)
			{
				var newItem = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(
					currency, Enumerable.Empty<Tuple<ICoin, int>>());
				gameworld.Add(newItem);

				var container = gameitem.GetItemType<IContainer>();
				if (container == null)
				{
					return false;
				}

				container.Put(null, newItem, false);
			}
		}

		if (target is ICell location)
		{
			targetPile = location.GameItems.RecursiveGetItems<ICurrencyPile>(respectGetRules).FirstOrDefault();
			if (targetPile == null)
			{
				var newItem = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(
					currency, Enumerable.Empty<Tuple<ICoin, int>>());
				gameworld.Add(newItem);
				location.Insert(newItem, true);
			}
		}

		if (target is ICharacter character)
		{
			targetPile = character.Body.AllItems.RecursiveGetItems<ICurrencyPile>(respectGetRules).FirstOrDefault();
			if (targetPile == null)
			{
				var newItem = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(
					currency, Enumerable.Empty<Tuple<ICoin, int>>());
				gameworld.Add(newItem);

				if (character.Body.CanGet(newItem, 0))
				{
					character.Body.Get(newItem, silent: true);
				}
				else
				{
					var targetContainer =
						character.Body.AllItems.RecursiveGetItems<IContainer>(respectGetRules)
						         .FirstOrDefault(
							         x =>
								         !respectGetRules ||
								         (x.Parent.IsItemType<IOpenable>() &&
								          x.Parent.GetItemType<IOpenable>().IsOpen &&
								          x.CanPut(newItem)));
					if (targetContainer == null)
					{
						newItem.RoomLayer = character.RoomLayer;
						character.Location.Insert(newItem, true);
					}
					else
					{
						targetContainer.Put(null, newItem, false);
					}
				}
			}
		}

		if (targetPile == null)
		{
			return false;
		}

		bool exact;
		targetPile.AddCoins(currency.FindCoinsForAmount(amount, out exact).Select(x => Tuple.Create(x.Key, x.Value)));

		return true;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
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
			ErrorMessage = "Null currency in GiveCurrency function.";
			return StatementResult.Error;
		}

		var respectGetRules = (bool?)ParameterFunctions[3].Result.GetObject ?? false;

		var success = true;
		var amount = ParameterFunctions[1].ReturnType == FutureProgVariableTypes.Number
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
			ErrorMessage = "Null Character, Location or Item given to GiveCurrency function.";
			return StatementResult.Error;
		}

		ICurrencyPile targetPile = null;
		if (parameter1 is IGameItem gameitem)
		{
			targetPile = gameitem.RecursiveGetItems<ICurrencyPile>(respectGetRules).FirstOrDefault();
			if (targetPile == null)
			{
				var newItem = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(
					currency, Enumerable.Empty<Tuple<ICoin, int>>());
				_gameworld.Add(newItem);

				var container = gameitem.GetItemType<IContainer>();
				if (container == null)
				{
					Result = new BooleanVariable(false);
					return StatementResult.Normal;
				}

				container.Put(null, newItem, false);
			}
		}

		if (parameter1 is ICell location)
		{
			targetPile = location.GameItems.RecursiveGetItems<ICurrencyPile>(respectGetRules).FirstOrDefault();
			if (targetPile == null)
			{
				var newItem = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(
					currency, Enumerable.Empty<Tuple<ICoin, int>>());
				_gameworld.Add(newItem);
				location.Insert(newItem, true);
			}
		}

		if (parameter1 is ICharacter character)
		{
			targetPile = character.Body.AllItems.RecursiveGetItems<ICurrencyPile>(respectGetRules).FirstOrDefault();
			if (targetPile == null)
			{
				var newItem = GameItems.Prototypes.CurrencyGameItemComponentProto.CreateNewCurrencyPile(
					currency, Enumerable.Empty<Tuple<ICoin, int>>());
				_gameworld.Add(newItem);

				if (character.Body.CanGet(newItem, 0))
				{
					character.Body.Get(newItem, silent: true);
				}
				else
				{
					var targetContainer =
						character.Body.AllItems.RecursiveGetItems<IContainer>(respectGetRules)
						         .FirstOrDefault(
							         x =>
								         !respectGetRules ||
								         (x.Parent.IsItemType<IOpenable>() &&
								          x.Parent.GetItemType<IOpenable>().IsOpen &&
								          x.CanPut(newItem)));
					if (targetContainer == null)
					{
						character.Location.Insert(newItem, true);
					}
					else
					{
						targetContainer.Put(null, newItem, false);
					}
				}
			}
		}

		if (targetPile == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		bool exact;
		targetPile.AddCoins(currency.FindCoinsForAmount(amount, out exact).Select(x => Tuple.Create(x.Key, x.Value)));

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	private readonly IFuturemud _gameworld;

	public GiveCurrencyFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"givecurrency",
			new FutureProgVariableTypes[]
			{
				FutureProgVariableTypes.Currency, FutureProgVariableTypes.Number | FutureProgVariableTypes.Text,
				FutureProgVariableTypes.Location | FutureProgVariableTypes.Character | FutureProgVariableTypes.Item,
				FutureProgVariableTypes.Boolean
			},
			(IList<IFunction> pars, IFuturemud gameworld) => new GiveCurrencyFunction(pars, gameworld)
		));
	}
}
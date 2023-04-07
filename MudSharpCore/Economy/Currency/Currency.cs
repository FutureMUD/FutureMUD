using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Economy.Currency;

public static class CurrencyExtensions
{
	public static decimal TotalValue(this Dictionary<ICurrencyPile, Dictionary<ICoin, int>> coins)
	{
		return coins.Sum(pile =>
			pile.Value.Sum(coin =>
				coin.Key.Value * coin.Value
			)
		);
	}
}

public class Currency : FrameworkItem, ICurrency
{
	public Currency(MudSharp.Models.Currency currency, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = currency.Id;
		_name = currency.Name;
		foreach (var item in currency.CurrencyDivisions)
		{
			_currencyDivisions.Add(new CurrencyDivision(item));
		}

		PatternDictionary = new Dictionary<CurrencyDescriptionPatternType, List<ICurrencyDescriptionPattern>>();
		foreach (var value in Enum.GetValues(typeof(CurrencyDescriptionPatternType)))
		{
			PatternDictionary.Add((CurrencyDescriptionPatternType)value, new List<ICurrencyDescriptionPattern>());
		}

		foreach (var item in currency.CurrencyDescriptionPatterns.OrderBy(x => x.Order))
		{
			PatternDictionary[(CurrencyDescriptionPatternType)item.Type].Add(new CurrencyDescriptionPattern(item,
				this, (CurrencyDescriptionPatternType)item.Type));
			if ((CurrencyDescriptionPatternType)item.Type == CurrencyDescriptionPatternType.Long)
			{
				PatternDictionary[CurrencyDescriptionPatternType.Wordy].Add(new CurrencyDescriptionPattern(item,
					this, CurrencyDescriptionPatternType.Wordy));
			}
		}

		foreach (var item in currency.Coins)
		{
			_coins.Add(new Coin(item));
		}
	}

	public override string FrameworkItemType => "Currency";

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; }

	#endregion

	public override string ToString()
	{
		return $"Currency [#{Id}]: {Name.Proper()}";
	}

	#region ICurrency Members

	private readonly List<ICoin> _coins = new();
	public IEnumerable<ICoin> Coins => _coins;

	private readonly List<ICurrencyDivision> _currencyDivisions = new();
	public IEnumerable<ICurrencyDivision> CurrencyDivisions => _currencyDivisions;

	public string Describe(decimal value, CurrencyDescriptionPatternType type)
	{
		if (value == 0.0M)
		{
			return "nothing";
		}

		return
			PatternDictionary[type].FirstOrDefault(
				                       x => x.ApplicabilityProg == null ||
				                            ((bool?)x.ApplicabilityProg.Execute(value) ?? true))
			                       .Describe(value);
	}

	public Dictionary<CurrencyDescriptionPatternType, List<ICurrencyDescriptionPattern>> PatternDictionary { get; }

	private static readonly Regex _baseCurrencyRegex =
		new(@"([^\d\s]){0,1}(\d{1,}[\.\d]{0,})[ ]{0,1}([a-zA-Z]+)*");

	public decimal GetBaseCurrency(string pattern, out bool success)
	{
		decimal baseSum = 0;
		var found = false;
		foreach (Match match in _baseCurrencyRegex.Matches(pattern))
		{
			var division =
				CurrencyDivisions.FirstOrDefault(x => x.Patterns.Any(y => y.IsMatch(match.Groups[0].Value)));
			if (division == null)
			{
				success = false;
				return 0.0M;
			}

			found = true;

			baseSum +=
				decimal.Parse(
					division.Patterns.Select(x => x.Match(match.Groups[0].Value)).First(x => x.Success).Groups[1]
					        .Value) *
				division.BaseUnitConversionRate;
		}

		success = found;
		return baseSum;
	}

	public bool TryGetBaseCurrency(string pattern, out decimal amount)
	{
		var found = false;
		amount = 0.0M;
		foreach (Match match in _baseCurrencyRegex.Matches(pattern))
		{
			var division =
				CurrencyDivisions.FirstOrDefault(x => x.Patterns.Any(y => y.IsMatch(match.Groups[0].Value)));
			if (division == null)
			{
				amount = 0.0M;
				return false;
			}

			found = true;
			amount +=
				decimal.Parse(
					division.Patterns.Select(x => x.Match(match.Groups[0].Value)).First(x => x.Success).Groups[1]
					        .Value) *
				division.BaseUnitConversionRate;
		}

		return found;
	}

	public Dictionary<ICoin, int> FindCoinsForAmount(decimal amount, out bool exactMatch)
	{
		var remainingAmount = amount;
		var results = new Dictionary<ICoin, int>();
		foreach (var coin in Coins.OrderByDescending(x => x.Value))
		{
			var coinAmount = (int)(remainingAmount / coin.Value);
			if (coinAmount > 0)
			{
				results.Add(coin, coinAmount);
				remainingAmount %= coin.Value;
			}
		}

		exactMatch = remainingAmount == 0;
		return results;
	}

	public Dictionary<ICurrencyPile, Dictionary<ICoin, int>> FindCurrency(IEnumerable<ICurrencyPile> targetPiles,
		decimal amount)
	{
		targetPiles = targetPiles.Where(x => x.Currency == this).ToList();
		var availableCoins =
			targetPiles.SelectMany(x => x.Coins).Select(x => x.Item1).Distinct().OrderByDescending(x => x.Value);
		var results = new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>();
		foreach (var coin in availableCoins)
		foreach (
			var pile in
			targetPiles.Where(x => x.Coins.Any(y => y.Item1 == coin))
			           .Select(x => Tuple.Create(x, x.Coins.First(y => y.Item1 == coin))))
		{
			var coinAmount = Math.Min((int)(amount / coin.Value), pile.Item2.Item2);
			if (coinAmount > 0)
			{
				if (!results.ContainsKey(pile.Item1))
				{
					results.Add(pile.Item1, new Dictionary<ICoin, int>());
				}

				results[pile.Item1][pile.Item2.Item1] = coinAmount;
				amount -= coinAmount * coin.Value;
				if (amount == 0)
				{
					return results;
				}
			}
		}

		// If we get here, we couldn't make exact change. Step back up in reverse and see what is the minimum amount we can go over.
		foreach (var coin in availableCoins.Reverse())
		{
			var targetPile =
				targetPiles.FirstOrDefault(
					x =>
						x.Coins.Any(y => y.Item1 == coin) &&
						(!results.ContainsKey(x) || !results[x].ContainsKey(coin) ||
						 results[x][coin] < x.Coins.Single(y => y.Item1 == coin).Item2));
			if (targetPile != null)
			{
				if (!results.ContainsKey(targetPile))
				{
					results.Add(targetPile, new Dictionary<ICoin, int>());
				}

				if (!results[targetPile].ContainsKey(coin))
				{
					results[targetPile][coin] = 0;
				}

				results[targetPile][coin] += 1;
				return results;
			}
		}

		// Ah well
		return results;
	}

	#endregion

	#region IFutureProgVariable Members

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			default:
				throw new NotSupportedException("Invalid IFutureProgVariableType requested in Currency.GetProperty");
		}
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Currency;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Currency, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
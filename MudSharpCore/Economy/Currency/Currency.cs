using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
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

public class Currency : SaveableItem, ICurrency
{
	public Currency(MudSharp.Models.Currency currency, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = currency.Id;
		_name = currency.Name;
		BaseCurrencyToGlobalBaseCurrencyConversion = currency.BaseCurrencyToGlobalBaseCurrencyConversion;
		foreach (var item in currency.CurrencyDivisions)
		{
			_currencyDivisions.Add(new CurrencyDivision(gameworld, item));
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
			_coins.Add(new Coin(gameworld, item));
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

	public decimal BaseCurrencyToGlobalBaseCurrencyConversion { get; private set; }

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
			case "conversion":
				return new NumberVariable(BaseCurrencyToGlobalBaseCurrencyConversion);
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
			{ "name", FutureProgVariableTypes.Text },
			{ "conversion", FutureProgVariableTypes.Number }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The database ID of the currency" },
			{ "name", "The name of the currency" },
			{ "conversion", "The conversion rate of this currency to a global base currency" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Currency, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	public override void Save()
	{
		var dbitem = FMDB.Context.Currencies.Find(Id);
		dbitem.Name = Name;
		dbitem.BaseCurrencyToGlobalBaseCurrencyConversion = BaseCurrencyToGlobalBaseCurrencyConversion;
		Changed = false;
	}

	public const string HelpText = @"";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "conversion":
				return BuildingCommandConversion(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this currency?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Currencies.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a currency called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the currency {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandConversion(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !decimal.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0M)
		{
			actor.OutputHandler.Send("You must enter a valid conversion rate to universal currency that is 0 or higher.");
			return false;
		}

		BaseCurrencyToGlobalBaseCurrencyConversion = value;
		Changed = true;
		actor.OutputHandler.Send("");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Currency #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Conversion to Global: {BaseCurrencyToGlobalBaseCurrencyConversion.ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Currency Divisions".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		foreach (var division in _currencyDivisions) {
			sb.AppendLine(division.Name.TitleCase().ColourName());
			sb.AppendLine($"Conversion To Base: {division.BaseUnitConversionRate.ToString("N0", actor).ColourValue()}");
			sb.AppendLine(StringUtilities.GetTextTable(
				from pattern in division.Patterns
				select new List<string>
				{
					pattern.ToString()
				},
				new List<string>
				{
					"Regex Patterns"
				},
				actor,
				Telnet.Yellow
			));
		}
		sb.AppendLine();
		sb.AppendLine("Patterns".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
				from pattern in PatternDictionary.SelectMany(x => x.Value)
				select new List<string>
				{
					pattern.Id.ToString("N0", actor),
					pattern.Type.DescribeEnum(),
					pattern.ApplicabilityProg?.MXPClickableFunctionName() ?? "",
					pattern.NegativeValuePrefix,
					pattern.Elements.Count().ToString("N0", actor)
				},
				new List<string>
				{
					"Id",
					"Type",
					"Prog",
					"Negative",
					"# Elements"
				},
				actor,
				Telnet.Yellow
			));
		foreach (var patternType in PatternDictionary)
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from pattern in patternType.Value
				select new List<string>
				{
					patternType.Key.DescribeEnum(),
					pattern.Id.ToString("N0", actor),
					pattern.ApplicabilityProg?.MXPClickableFunctionName() ?? "",
					pattern.NegativeValuePrefix,
					pattern.Elements.Count().ToString("N0", actor)
				},
				new List<string>
				{
					"Type",
					"Id",
					"Prog",
					"Negative",
					"# Elements"
				},
				actor,
				Telnet.Yellow
			));
		}
		sb.AppendLine();
		sb.AppendLine("Coins".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
				from coin in Coins
				select new List<string>
				{
					coin.Id.ToString("N0", actor),
					coin.Name,
					coin.ShortDescription,
					coin.GeneralForm,
					coin.PluralWord,
					coin.Value.ToString("N0", actor),
					Gameworld.UnitManager.Describe(coin.Weight, Framework.Units.UnitType.Mass, actor),
					coin.FullDescription
				},
				new List<string>
				{
					"Id",
					"Name",
					"SDesc",
					"General",
					"Plural",
					"Value",
					"Weight",
					"Desc"
				},
				actor,
				Telnet.Yellow,
				7
			));
		return sb.ToString();
	}
}
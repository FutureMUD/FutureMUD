using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
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
	private void LoadFromDatabase(Models.Currency currency)
	{
		foreach (var item in currency.CurrencyDivisions)
		{
			_currencyDivisions.Add(new CurrencyDivision(Gameworld, item, this));
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
			var coin = new Coin(Gameworld, item, this);
			_coins.Add(coin);
			Gameworld.Add(coin);
		}
	}
	public Currency(MudSharp.Models.Currency currency, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = currency.Id;
		_name = currency.Name;
		BaseCurrencyToGlobalBaseCurrencyConversion = currency.BaseCurrencyToGlobalBaseCurrencyConversion;
		LoadFromDatabase(currency);
	}

	public Currency(IFuturemud gameworld, string name, string lowestDivision, string lowestCoin)
	{
		Gameworld = gameworld;
		_name = name;
		BaseCurrencyToGlobalBaseCurrencyConversion = 1.0M;
		using (new FMDB())
		{
			var dbitem = new Models.Currency
			{
				Name = name,
				BaseCurrencyToGlobalBaseCurrencyConversion = BaseCurrencyToGlobalBaseCurrencyConversion
			};
			FMDB.Context.Currencies.Add(dbitem);
			var dbdivision = new Models.CurrencyDivision
			{
				Currency = dbitem,
				BaseUnitConversionRate = 1.0M,
				IgnoreCase = true,
				Name = lowestDivision
			};
			dbitem.CurrencyDivisions.Add(dbdivision);
			dbdivision.CurrencyDivisionAbbreviations.Add(new Models.CurrencyDivisionAbbreviation
			{
				CurrencyDivision = dbdivision,
				Pattern = $"(-?\\d+(?:\\.\\d+)*)(?:\\s*(?:{lowestDivision.ToLowerInvariant().Pluralise()}|{lowestDivision.ToLowerInvariant()}|{lowestDivision.ToLowerInvariant()[0]}))$"
			});
			dbitem.CurrencyDescriptionPatterns.Add(new Models.CurrencyDescriptionPattern
			{
				Currency = dbitem,
				FutureProgId = Gameworld.AlwaysTrueProg.Id,
				Order = 1,
				Type = (int)CurrencyDescriptionPatternType.Casual,
				NegativePrefix = "negative ",
				UseNaturalAggregationStyle = false,
				CurrencyDescriptionPatternElements = new List<Models.CurrencyDescriptionPatternElement>
				{
					new()
					{
						Pattern = $"{{0}} {lowestDivision.ToLowerInvariant()}",
						Order = 0,
						ShowIfZero = false,
						PluraliseWord = lowestDivision.ToLowerInvariant(),
						AlternatePattern = null,
						RoundingMode = (int)RoundingMode.Truncate,
						SpecialValuesOverrideFormat = false,
						CurrencyDivision = dbdivision,
					}
				}
			});
			dbitem.CurrencyDescriptionPatterns.Add(new Models.CurrencyDescriptionPattern
			{
				Currency = dbitem,
				FutureProgId = Gameworld.AlwaysTrueProg.Id,
				Order = 1,
				Type = (int)CurrencyDescriptionPatternType.Short,
				NegativePrefix = "-",
				UseNaturalAggregationStyle = false,
				CurrencyDescriptionPatternElements = new List<Models.CurrencyDescriptionPatternElement>
				{
					new()
					{
						Pattern = $"{{0}}{lowestDivision.ToLowerInvariant()[0]}",
						Order = 0,
						ShowIfZero = true,
						PluraliseWord = string.Empty,
						AlternatePattern = null,
						RoundingMode = (int)RoundingMode.Truncate,
						SpecialValuesOverrideFormat = false,
						CurrencyDivision = dbdivision,
					}
				}
			});
			dbitem.CurrencyDescriptionPatterns.Add(new Models.CurrencyDescriptionPattern
			{
				Currency = dbitem,
				FutureProgId = Gameworld.AlwaysTrueProg.Id,
				Order = 1,
				Type = (int)CurrencyDescriptionPatternType.ShortDecimal,
				NegativePrefix = "-",
				UseNaturalAggregationStyle = false,
				CurrencyDescriptionPatternElements = new List<Models.CurrencyDescriptionPatternElement>
				{
					new()
					{
						Pattern = $"{{0}}{lowestDivision.ToLowerInvariant()[0]}",
						Order = 0,
						ShowIfZero = true,
						PluraliseWord = string.Empty,
						AlternatePattern = null,
						RoundingMode = (int)RoundingMode.Truncate,
						SpecialValuesOverrideFormat = false,
						CurrencyDivision = dbdivision,
					}
				}
			});
			dbitem.CurrencyDescriptionPatterns.Add(new Models.CurrencyDescriptionPattern
			{
				Currency = dbitem,
				FutureProgId = Gameworld.AlwaysTrueProg.Id,
				Order = 1,
				Type = (int)CurrencyDescriptionPatternType.Long,
				NegativePrefix = "negative ",
				UseNaturalAggregationStyle = false,
				CurrencyDescriptionPatternElements = new List<Models.CurrencyDescriptionPatternElement>
				{
					new()
					{
						Pattern = $"{{0}} {lowestDivision.ToLowerInvariant()}",
						Order = 0,
						ShowIfZero = false,
						PluraliseWord = lowestDivision.ToLowerInvariant(),
						AlternatePattern = null,
						RoundingMode = (int)RoundingMode.Truncate,
						SpecialValuesOverrideFormat = false,
						CurrencyDivision = dbdivision,
					}
				}
			});
			dbitem.CurrencyDescriptionPatterns.Add(new Models.CurrencyDescriptionPattern
			{
				Currency = dbitem,
				FutureProgId = Gameworld.AlwaysTrueProg.Id,
				Order = 1,
				Type = (int)CurrencyDescriptionPatternType.Wordy,
				NegativePrefix = "negative ",
				UseNaturalAggregationStyle = true,
				CurrencyDescriptionPatternElements = new List<Models.CurrencyDescriptionPatternElement>
				{
					new()
					{
						Pattern = $"{{0}} {lowestDivision.ToLowerInvariant()}",
						Order = 0,
						ShowIfZero = false,
						PluraliseWord = lowestDivision.ToLowerInvariant(),
						AlternatePattern = null,
						RoundingMode = (int)RoundingMode.Truncate,
						SpecialValuesOverrideFormat = false,
						CurrencyDivision = dbdivision,
					}
				}
			});
			dbitem.Coins.Add(new()
			{
				Name = lowestCoin.ToLowerInvariant(),
				ShortDescription = $"a {lowestCoin.ToLowerInvariant()} coin",
				FullDescription = $"This is a small, round coin made of precious metal called a {lowestCoin}, worth 1 {lowestDivision}.",
				Value = 1.0M,
				Weight = 5,
				GeneralForm = "coin",
				PluralWord = lowestCoin.ToLowerInvariant(),
				UseForChange = true,
				Currency = dbitem,
			});
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			LoadFromDatabase(dbitem);
		}
	}

	private Currency(Currency rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		BaseCurrencyToGlobalBaseCurrencyConversion = rhs.BaseCurrencyToGlobalBaseCurrencyConversion;
		using (new FMDB())
		{
			var dbitem = new Models.Currency
			{
				Name = name,
				BaseCurrencyToGlobalBaseCurrencyConversion = BaseCurrencyToGlobalBaseCurrencyConversion
			};
			FMDB.Context.Currencies.Add(dbitem);

			var divisionMap = new Dictionary<long, Models.CurrencyDivision>();
			foreach (var division in rhs.CurrencyDivisions)
			{
				var dbdivision = new Models.CurrencyDivision
				{
					Name = division.Name,
					BaseUnitConversionRate = division.BaseUnitConversionRate,
					IgnoreCase = division.IgnoreCase,
					Currency = dbitem,
				};
				divisionMap[division.Id] = dbdivision;
			}
		}
	}

	public ICurrency Clone(string name)
	{
		return new Currency(this, name);
	}

	public override string FrameworkItemType => "Currency";

	public override string ToString()
	{
		return $"Currency [#{Id}]: {Name.Proper()}";
	}

	#region ICurrency Members

	private readonly List<ICoin> _coins = new();
	public IEnumerable<ICoin> Coins => _coins;

	public void AddCoin(ICoin coin)
	{
		_coins.Add(coin);
	}

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

	public CollectionDictionary<CurrencyDescriptionPatternType, ICurrencyDescriptionPattern> PatternDictionary { get; } =
		new();

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

		amount = Math.Truncate(amount);
		return found;
	}

	public Dictionary<ICoin, int> FindCoinsForAmount(decimal amount, out bool exactMatch)
	{
		var remainingAmount = amount;
		var results = new Dictionary<ICoin, int>();
		foreach (var coin in Coins.Where(x => x.UseForChange).OrderByDescending(x => x.Value))
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

	public IProgVariable GetProperty(string property)
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

	public ProgVariableTypes Type => ProgVariableTypes.Currency;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "conversion", ProgVariableTypes.Number }
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
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Currency, DotReferenceHandler(),
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

	public const string HelpText = @"You can use the following commands to edit currencies:

	#3name <name>#0 - sets the name of this currency
	#3conversion <rate>#0 - sets the global currency conversion rate (to global base currency)
	#3adddivision <name> <rate>#0 - adds a new currency division
	#3remdivision <id|name>#0 - removes a currency division
	#3division <id|name> name <name>#0 - sets a new name for the division
	#3division <id|name> base <amount>#0 - sets the amount of base currency this division is worth
	#3division <id|name> ignorecase#0 - toggles ignoring case in the regular expression patterns for the division
	#3division <id|name> addabbr <regex>#0 - adds a regular expression pattern for this division
	#3division <id|name> remabbr <##>#0 - removes a particular pattern abbreviation for this division
	#3division <id|name> abbr <##> <regex>#0 - overwrites the regular expression pattern at the specified index for this division
	#3addpattern <type>#0 - adds a new pattern of the specified type
	#3removepattern <id>#0 - removes a pattern
	#3pattern <id> order <##>#0 - changes the order in which this pattern is evaluated for applicability
	#3pattern <id> prog <which>#0 - sets the prog that controls applicability for this pattern
	#3pattern <id> negative <prefix>#0 - sets a prefix applied to negative values for this pattern (e.g. #2-#0 or #2negative #0.) Be sure to include spaces if necessary
	#3pattern <id> natural#0 - toggles natural aggregation style for pattern elements (commas plus ""and"") rather than just concatenation
	#3pattern <id> addelement <division> <plural> <pattern>#0 - adds a new pattern element
	#3pattern <id> remelement <id|##>#0 - deletes an element.
	#3pattern <id> element <id|##order> zero#0 - toggles showing this element if it is zero
	#3pattern <id> element <id|##order> specials#0 - toggles special values totally overriding the pattern instead of just the value part
	#3pattern <id> element <id|##order> order <##>#0 - changes the order this element appears in the list of its pattern
	#3pattern <id> element <id|##order> pattern <pattern>#0 - sets the pattern for the element. Use #3{0}#0 for the numerical value.
	#3pattern <id> element <id|##order> last <pattern>#0 - sets an alternate pattern if this is the last element in the display. Use #3{0}#0 for the numerical value.
	#3pattern <id> element <id|##order> last none#0 - clears the last alternative pattern
	#3pattern <id> element <id|##order> plural <word>#0 - sets the word in the pattern that should be used for pluralisation
	#3pattern <id> element <id|##order> rounding <truncate|round|noround>#0 - changes the rounding mode for this element
	#3pattern <id> element <id|##order> addspecial <value> <text>#0 - adds or sets a special value
	#3pattern <id> element <id|##order> remspecial <value>#0 - removes a special value";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "conversion":
				return BuildingCommandConversion(actor, command);
			case "adddivision":
			case "addivision":
			case "adddiv":
				return BuildingCommandAddDivision(actor, command);
			case "remdivision":
			case "remdiv":
			case "removedivision":
				return BuildingCommandRemoveDivision(actor, command);
			case "division":
			case "div":
				return BuildingCommandDivision(actor, command);
			case "pattern":
				return BuildingCommandPattern(actor, command);
			case "addpattern":
				return BuildingCommandAddPattern(actor, command);
			case "removepattern":
				return BuildingCommandRemovePattern(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandRemovePattern(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which pattern do you want to remove?");
			return false;
		}

		var pattern = PatternDictionary.Values.SelectMany(x => x).GetById(command.SafeRemainingArgument);
		if (pattern is null)
		{
			actor.OutputHandler.Send("There is no such pattern.");
			return false;
		}

		actor.OutputHandler.Send($"Are you sure you want to delete the currency description pattern with ID #{pattern.Id.ToString("N0", actor)}? This action is irreversible.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "deleting a currency description pattern",
			AcceptAction = text =>
			{
				if (PatternDictionary[pattern.Type].Count <= 1)
				{
					actor.OutputHandler.Send("You cannot delete the last pattern for a type.");
					return;
				}

				actor.OutputHandler.Send("You delete the pattern.");
				pattern.Delete();
				PatternDictionary.Remove(pattern.Type, pattern);
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the pattern.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to delete the pattern.");
			},
			Keywords = new List<string>
			{
				"delete",
				"pattern"
			}
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandAddPattern(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which type do you want to make a new pattern for? The valid types are {Enum.GetValues<CurrencyDescriptionPatternType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out CurrencyDescriptionPatternType type))
		{
			actor.OutputHandler.Send($"That is not a valid type. The valid types are {Enum.GetValues<CurrencyDescriptionPatternType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;

		}

		var pattern = new CurrencyDescriptionPattern(this, type);
		PatternDictionary[type].Add(pattern);
		actor.OutputHandler.Send($"You create a new pattern for the {type.DescribeEnum()} type.");
		return true;
	}

	private bool BuildingCommandPattern(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency pattern do you want to edit?");
			return false;
		}

		var pattern = PatternDictionary.Values.SelectMany(x => x).GetById(command.PopSpeech());
		if (pattern is null)
		{
			actor.OutputHandler.Send("There is no such currency pattern.");
			return false;
		}

		return pattern.BuildingCommand(actor, command);
	}

	private bool BuildingCommandRemoveDivision(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which division do you want to remove?");
			return false;
		}

		var division = CurrencyDivisions.GetByIdOrName(command.PopSpeech());
		if (division is null)
		{
			actor.OutputHandler.Send($"The {Name.ColourName()} currency has no such currency division.");
			return false;
		}

		actor.OutputHandler.Send($"Are you sure you want to remove the {division.Name.ColourName()} currency division? This will permanently remove all associated information such as patterns for this division as well.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = $"Deleting the {division.Name.ColourName()} currency division",
			AcceptAction = text =>
			{
				if (_currencyDivisions.Count == 1)
				{
					actor.OutputHandler.Send("You cannot delete the last currency division. Create some new ones first.");
					return;
				}

				division.Delete();
				_currencyDivisions.Remove(division);
				actor.OutputHandler.Send($"You delete the {division.Name.ColourName()} currency division.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {division.Name.ColourName()} currency division.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to delete the {division.Name.ColourName()} currency division.");
			},
			Keywords = new List<string>{ "delete", "division", "currency"}
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandAddDivision(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new currency division?");
			return false;
		}

		var name = command.PopSpeech().ToLowerInvariant();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the conversion rate between this division and the base currency rate?");
			return false;
		}

		if (!decimal.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		var division = new CurrencyDivision(Gameworld, name, value, this);
		_currencyDivisions.Add(division);
		actor.OutputHandler.Send($"You create a new currency division called {division.Name.TitleCase().ColourName()} with a value in base currency of {value.ToString("N3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDivision(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency division do you want to edit?");
			return false;
		}

		var division = CurrencyDivisions.GetByIdOrName(command.PopSpeech());
		if (division is null)
		{
			actor.OutputHandler.Send("There is no such currency division.");
			return false;
		}

		return division.BuildingCommand(actor, command);
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
					Gameworld.UnitManager.Describe(coin.Weight, Framework.Units.UnitType.Mass, actor).ColourValue(),
					coin.UseForChange.ToColouredString()
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
					"Change?"
				},
				actor,
				Telnet.Yellow,
				7
			));
		return sb.ToString();
	}
}
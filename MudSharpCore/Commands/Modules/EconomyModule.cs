using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using Dapper;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Trees;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Markets;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Property;
using MudSharp.Form.Shape;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Prototypes;
using MudSharp.Migrations;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Butchering;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using AuctionBid = MudSharp.Economy.AuctionBid;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MudSharp.Commands.Modules;

internal class EconomyModule : Module<ICharacter>
{
	private EconomyModule()
		: base("Economy")
	{
		IsNecessary = true;
	}

	public static EconomyModule Instance { get; } = new();

	private static void CountItem(IGameItem item, Dictionary<ICurrency, decimal> results, StringBuilder sb,
		IPerceiver voyeur, IGameItem parent = null)
	{
		var currency = item.GetItemType<ICurrencyPile>();
		if (currency != null)
		{
			if (!results.ContainsKey(currency.Currency))
			{
				results[currency.Currency] = 0;
			}

			results[currency.Currency] += currency.Coins.Sum(y => y.Item1.Value * y.Item2);
			foreach (var coin in currency.Coins.OrderBy(x => x.Item1.Value))
			{
				sb.AppendLine(
					$"\t{coin.Item1.ShortDescription.Colour(Telnet.Green)} (x{coin.Item2}) = {currency.Currency.Describe(coin.Item2 * coin.Item1.Value, CurrencyDescriptionPatternType.ShortDecimal).Colour(Telnet.Green)}{(parent != null ? $" [in {parent.HowSeen(voyeur)}]" : "")}");
			}

			return;
		}

		var container = item.GetItemType<IContainer>();
		if (container != null)
		{
			foreach (var contained in container.Contents)
			{
				CountItem(contained, results, sb, voyeur, item);
			}
		}
	}

	[PlayerCommand("Count", "count")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[HelpInfo("count",
		"The count command is used to take stock of the physical currency that you are carrying (including in containers), to help you understand how much money you have. It will also display a list of bank accounts that you own (though you need to go to a bank to see the balances of these).",
		AutoHelp.HelpArg)]
	protected static void Count(ICharacter character, string command)
	{
		var results = new Dictionary<ICurrency, decimal>();
		var sb = new StringBuilder();
		sb.AppendLine("You have the following currency items on your person:");
		foreach (var item in character.Body.ExternalItems)
		{
			CountItem(item, results, sb, character);
		}

		sb.AppendLine();
		if (!results.Any())
		{
			sb.AppendLine("\tNone. You are completely broke.".ColourError());
		}
		else
		{
			sb.AppendLine("This comes to a total of " +
						  results.Select(
							  x =>
								  x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal)
								   .Colour(Telnet.Green)).ListToString());
		}

		var bankAccounts = character.Gameworld.BankAccounts.Where(x => x.IsAccountOwner(character)).ToList();
		if (bankAccounts.Any())
		{
			sb.AppendLine();
			sb.AppendLine("You also know that you have accounts with the following banks:");
			sb.AppendLine();
			foreach (var account in bankAccounts)
			{
				sb.AppendLine(
					$"\t{account.AccountReference.ColourValue()} - {account.BankAccountType.Name.ColourName()} with {account.Bank.Name.ColourName()}");
			}
		}

		character.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Appraise", "appraise")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[HelpInfo("appraise",
		@"The appraise command is used to quickly judge the value of an item, or a container full of items. There are a few different ways that you can use this command:

	#3appraise#0 - appraises the value of all items you are carrying and that are present in the room
	#3appraise <thing>#0 - appraises the value of a thing and its contents (if a container)",
		AutoHelp.HelpArg)]
	protected static void Appraise(ICharacter actor, string command)
	{
		if (actor.Currency is null)
		{
			actor.OutputHandler.Send("You must first set a currency to use before you can use this command.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		var td = actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("AppraiseCommandSkill"));
		if (!actor.IsAdministrator() && 
			actor.Gameworld.GetStaticBool("AppraiseCommandRequiresSkill") &&
			td is not null &&
			actor.TraitValue(td) <= 0.0)
		{
			actor.OutputHandler.Send(
				$"Without the {td.Name.ColourName()} skill, you have no idea how much things are worth.");
			return;
		}

		IGameItem target = null;
		if (!ss.IsFinished)
		{
			target = actor.TargetItem(ss.SafeRemainingArgument);
			if (target is null)
			{
				actor.OutputHandler.Send("There is nothing like that here for you to appraise.");
				return;
			}
		}

		var fuzzinessFloor = 1.0M;
		var fuzzinessCeiling = 1.0M;
		if (td is not null && !actor.IsAdministrator())
		{
			var check = actor.Gameworld.GetCheck(CheckType.AppraiseItemCheck);
			var difficulty = target is null
				? Difficulty.VeryHard
				: (target.IsItemType<IContainer>() ? Difficulty.Normal : Difficulty.Easy);
			var result = check.Check(actor, difficulty, td, target);
			var skew = 0.0M;
			switch (result.Outcome)
			{
				case Outcome.MajorFail:
					skew = (decimal)RandomUtilities.DoubleRandom(-0.5, 0.5);
					fuzzinessCeiling = 1.5M + skew;
					fuzzinessFloor = 0.5M + skew;
					break;
				case Outcome.Fail:
					skew = (decimal)RandomUtilities.DoubleRandom(-0.3, 0.3);
					fuzzinessCeiling = 1.3M + skew;
					fuzzinessFloor = 0.7M + skew;
					break;
				case Outcome.MinorFail:
					skew = (decimal)RandomUtilities.DoubleRandom(-0.2, 0.2);
					fuzzinessCeiling = 1.2M + skew;
					fuzzinessFloor = 0.8M + skew;
					break;
				case Outcome.MinorPass:
					skew = (decimal)RandomUtilities.DoubleRandom(-0.1, 0.1);
					fuzzinessCeiling = 1.1M + skew;
					fuzzinessFloor = 0.9M + skew;
					break;
				case Outcome.Pass:
					skew = (decimal)RandomUtilities.DoubleRandom(-0.05, 0.05);
					fuzzinessCeiling = 1.05M + skew;
					fuzzinessFloor = 0.95M + skew;
					break;
				case Outcome.MajorPass:
					break;
			}
		}

		(decimal minimum, decimal maximum) CalculateMinimumMaximum(IGameItem item)
		{
			if (item.GetItemType<ICurrencyPile>() is { } cp)
			{
				return (
					cp.TotalValue * cp.Currency.BaseCurrencyToGlobalBaseCurrencyConversion * fuzzinessFloor / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion,
					cp.TotalValue * cp.Currency.BaseCurrencyToGlobalBaseCurrencyConversion * fuzzinessCeiling / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion);
			}
			return (item.Prototype.CostInBaseCurrency * fuzzinessFloor / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion,
					item.Prototype.CostInBaseCurrency * fuzzinessCeiling / actor.Currency.BaseCurrencyToGlobalBaseCurrencyConversion);
		}

		string DescribeCurrencyRange(decimal minimum, decimal maximum)
		{
			if (minimum == maximum)
			{
				return actor.Currency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
			}

			return $"{actor.Currency.Describe(minimum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} to {actor.Currency.Describe(maximum, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}";
		}

		void EvaluateItem(IGameItem item, List<(string ItemDescription, string ValueDescription, int Levels)> list,
			ref decimal minTotal, ref decimal maxTotal, int level, bool includeContents)
		{
			var (min, max) = CalculateMinimumMaximum(item);
			minTotal += min;
			maxTotal += max;
			list.Add((item.HowSeen(actor), DescribeCurrencyRange(min,max), level));
			if (includeContents &&
				item.GetItemType<IContainer>() is { } container &&
				(actor.IsAdministrator() ||
				 container.Transparent ||
				 (container is IOpenable op && op.IsOpen)
				 )
				)
			{
				foreach (var content in container.Contents)
				{
					EvaluateItem(content, list, ref minTotal, ref maxTotal, level + 1, true);
				}
			}
		}

		var sb = new StringBuilder();
		if (target is null)
		{
			var results = new List<(string ItemDescription, string ValueDescription, int Levels)>();
			var minTotal = 0.0M;
			var maxTotal = 0.0M;
			foreach (var item in actor.Location.LayerGameItems(actor.RoomLayer))
			{
				if (!actor.CanSee(item))
				{
					continue;
				}

				EvaluateItem(item, results, ref minTotal, ref maxTotal, 0, true);
			}
			sb.AppendLine($"You appraise the value of the contents of the room:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(from result in results select new List<string>
			{
				new string('*', result.Levels),
				result.ItemDescription,
				result.ValueDescription
			}, new List<string>
			{
				"Level",
				"Item",
				"Value"
			}, actor));
			sb.AppendLine($"Total Value: {DescribeCurrencyRange(minTotal, maxTotal)}");
		}
		else if (target.GetItemType<IContainer>() is { } targetContainer && (targetContainer.Transparent || (targetContainer is IOpenable op && op.IsOpen) || actor.IsAdministrator()))
		{
			var results = new List<(string ItemDescription, string ValueDescription, int Levels)>();
			var minTotal = 0.0M;
			var maxTotal = 0.0M;
			sb.AppendLine($"You appraise the value of {target.HowSeen(actor)}:");
			sb.AppendLine();
			EvaluateItem(target, results, ref minTotal, ref maxTotal, 0, true);
			sb.AppendLine(StringUtilities.GetTextTable(from result in results select new List<string>
			{
				new string('*', result.Levels),
				result.ItemDescription,
				result.ValueDescription
			}, new List<string>
			{
				"Level",
				"Item",
				"Value"
			}, actor));
			sb.AppendLine($"Total Value: {DescribeCurrencyRange(minTotal, maxTotal)}");
			var topminTotal = 0.0M;
			var topmaxTotal = 0.0M;
			EvaluateItem(target, results, ref topminTotal, ref topmaxTotal, 0, false);
			sb.AppendLine($"Contents Value: {DescribeCurrencyRange(minTotal - topminTotal, maxTotal - topmaxTotal)}");
		}
		else
		{
			var (min, max) = CalculateMinimumMaximum(target);
			sb.AppendLine($"You appraise the value of {target.HowSeen(actor)}:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(new[] { new List<string>
			{
				"",
				target.HowSeen(actor),
				DescribeCurrencyRange(min,max)
			} }, new List<string>
			{
				"Level",
				"Item",
				"Value"
			}, actor));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	#region Currency

	public const string CurrencyHelp = @"This command is used to edit and view currencies. See also the closely related COIN command for editing coins.

The syntax for editing currencies is as follows:

	#3currency list#0 - lists all of the currencies (see below for filters)
    #3currency edit <which>#0 - begins editing a currency
    #3currency edit new <name> <lowest division> <lowest coin>#0 - generates a new coin
    #3currency clone <old> <new>#0 - clones an existing currency to a new one
    #3currency close#0 - stops editing a currency
    #3currency show <which>#0 - views information about a currency
    #3currency show#0 - views information about your currently editing currency
	#3currency set name <name>#0 - sets the name of this currency
	#3currency set conversion <rate>#0 - sets the global currency conversion rate (to global base currency)
	#3currency set adddivision <name> <rate>#0 - adds a new currency division
	#3currency set remdivision <id|name>#0 - removes a currency division
	#3currency set division <id|name> name <name>#0 - sets a new name for the division
	#3currency set division <id|name> base <amount>#0 - sets the amount of base currency this division is worth
	#3currency set division <id|name> ignorecase#0 - toggles ignoring case in the regular expression patterns for the division
	#3currency set division <id|name> addabbr <regex>#0 - adds a regular expression pattern for this division
	#3currency set division <id|name> remabbr <##>#0 - removes a particular pattern abbreviation for this division
	#3currency set division <id|name> abbr <##> <regex>#0 - overwrites the regular expression pattern at the specified index for this division
	#3currency set addpattern <type>#0 - adds a new pattern of the specified type
	#3currency set removepattern <id>#0 - removes a pattern
	#3currency set pattern <id|name> order <##>#0 - changes the order in which this pattern is evaluated for applicability
	#3currency set pattern <id|name> prog <which>#0 - sets the prog that controls applicability for this pattern
	#3currency set pattern <id|name> negative <prefix>#0 - sets a prefix applied to negative values for this pattern (e.g. #2-#0 or #2negative #0.) Be sure to include spaces if necessary
	#3currency set pattern <id|name> natural#0 - toggles natural aggregation style for pattern elements (commas plus ""and"") rather than just concatenation
	#3currency set pattern <id|name> addelement <division> <plural> <pattern>#0 - adds a new pattern element
	#3currency set pattern <id|name> remelement <id|##>#0 - deletes an element.
	#3currency set pattern <id|name> element <id|##order> zero#0 - toggles showing this element if it is zero
	#3currency set pattern <id|name> element <id|##order> specials#0 - toggles special values totally overriding the pattern instead of just the value part
	#3currency set pattern <id|name> element <id|##order> order <##>#0 - changes the order this element appears in the list of its pattern
	#3currency set pattern <id|name> element <id|##order> pattern <pattern>#0 - sets the pattern for the element. Use #3{0}#0 for the numerical value.
	#3currency set pattern <id|name> element <id|##order> last <pattern>#0 - sets an alternate pattern if this is the last element in the display. Use #3{0}#0 for the numerical value.
	#3currency set pattern <id|name> element <id|##order> last none#0 - clears the last alternative pattern
	#3currency set pattern <id|name> element <id|##order> plural <word>#0 - sets the word in the pattern that should be used for pluralisation
	#3currency set pattern <id|name> element <id|##order> rounding <truncate|round|noround>#0 - changes the rounding mode for this element
	#3currency set pattern <id|name> element <id|##order> addspecial <value> <text>#0 - adds or sets a special value
	#3currency set pattern <id|name> element <id|##order> remspecial <value>#0 - removes a special value";

	[PlayerCommand("Currency", "currency")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("currency", CurrencyHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Currency(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				CurrencyList(character, ss);
				break;
			case "set":
			case "show":
			case "edit":
			case "close":
			case "new":
			case "clone":
				if (!character.IsAdministrator(PermissionLevel.SeniorAdmin))
				{
					character.OutputHandler.Send("Due to the potential to break things, only Senior Admins or higher can run this specific subcommand.");
					return;
				}

				break;
			default:
				character.OutputHandler.Send(CurrencyHelp.SubstituteANSIColour());
				return;
		}

		switch (ss.Last)
		{
			case "set":
				CurrencySet(character, ss);
				break;
			case "show":
				CurrencyShow(character, ss);
				break;
			case "edit":
				CurrencyEdit(character, ss);
				break;
			case "close":
				CurrencyClose(character, ss);
				break;
			case "new":
				CurrencyNew(character, ss);
				break;
			case "clone":
				CurrencyClone(character, ss);
				break;
		}
	}

	private static void CurrencyClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which currency do you want to clone?");
			return;
		}

		var currency = actor.Gameworld.Currencies.GetByIdOrName(ss.PopSpeech());
		if (currency is null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new currency?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Currencies.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a currency called {name.ColourName()}. Names must be unique.");
			return;
		}

		var clone = currency.Clone(name);
		actor.Gameworld.Add(clone);
		actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
		actor.AddEffect(new BuilderEditingEffect<ICurrency>(actor) { EditingItem = clone });
		actor.OutputHandler.Send($"You create a new currency called {name.ColourName()} cloned from {currency.Name.ColourName()}, which you are now editing.");
	}

	private static void CurrencyNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new currency?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (actor.Gameworld.Currencies.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a currency called {name.ColourName()}. Names must be unique.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to call the lowest currency division for your currency?");
			return;
		}

		var division = ss.PopSpeech().TitleCase();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to call the lowest coin for your currency?");
			return;
		}

		var coin = ss.SafeRemainingArgument;
		var currency = new Currency(actor.Gameworld, name, division, coin);
		actor.Gameworld.Add(currency);
		actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
		actor.AddEffect(new BuilderEditingEffect<ICurrency>(actor){EditingItem = currency});
		actor.OutputHandler.Send($"You create a new currency called {name.ColourName()}, which you are now editing.");
	}

	private static void CurrencyEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var current = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
			if (current is null)
			{
				actor.OutputHandler.Send("Which currency did you want to edit?");
				return;
			}

			actor.OutputHandler.Send(current.EditingItem.Show(actor));
			return;
		}

		var currency = actor.Gameworld.Currencies.GetByIdOrName(command.PopSpeech());
		if (currency is null)
		{
			actor.OutputHandler.Send("There is no currency like that.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
		actor.AddEffect(new BuilderEditingEffect<ICurrency>(actor) { EditingItem = currency });
		actor.OutputHandler.Send($"You are now editing the {currency.Name.ColourName()} currency.");
	}

	private static void CurrencyClose(ICharacter actor, StringStack command)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any currencies.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ICurrency>>();
		actor.OutputHandler.Send("You are no longer editing any currencies.");
	}

	private static void CurrencyList(ICharacter actor, StringStack command)
	{
		var currencies = actor.Gameworld.Currencies.ToList();
		// TODO - filters

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from currency in currencies
			select new List<string>
			{
				currency.Id.ToString("N0", actor),
				currency.Name,
				currency.BaseCurrencyToGlobalBaseCurrencyConversion.ToString("N3", actor),
			},
			new List<string>
			{
				"Id",
				"Name",
				"Conversion"
			},
			actor,
			Telnet.BoldYellow
			));
	}

	private static void CurrencySet(ICharacter actor, StringStack command)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any currencies.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, command);
	}

	private static void CurrencyShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var current = actor.EffectsOfType<BuilderEditingEffect<ICurrency>>().FirstOrDefault();
			if (current is null)
			{
				actor.OutputHandler.Send("Which currency did you want to view?");
				return;
			}

			actor.OutputHandler.Send(current.EditingItem.Show(actor));
			return;
		}

		var currency = actor.Gameworld.Currencies.GetByIdOrName(ss.PopSpeech());
		if (currency is null)
		{
			actor.OutputHandler.Send("There is no currency like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(currency.Show(actor));
			return;
		}

		var switchType = ss.PopSpeech().ToLowerInvariant();
		switch (switchType)
		{
			case "pattern":
				if (!long.TryParse(ss.PopSpeech(), out var value))
				{
					actor.OutputHandler.Send("You must enter a valid ID.");
					return;
				}

				var pattern = currency.PatternDictionary.Values.SelectMany(x => x).Get(value);
				if (pattern is null)
				{
					actor.OutputHandler.Send($"The {currency.Name.ColourValue()} currency does not have any such pattern.");
					return;
				}
				actor.OutputHandler.Send(pattern.Show(actor));
				return;
			case "division":
				var division = currency.CurrencyDivisions.GetByIdOrName(ss.PopSpeech());
				if (division is null)
				{
					actor.OutputHandler.Send($"The {currency.Name.ColourValue()} currency does not have any such division.");
					return;
				}
				actor.OutputHandler.Send(division.Show(actor));
				return;
			case "coin":
				var coin = currency.Coins.GetByIdOrName(ss.PopSpeech());
				if (coin is null)
				{
					actor.OutputHandler.Send($"The {currency.Name.ColourValue()} currency does not have any such coin.");
					return;
				}
				actor.OutputHandler.Send(coin.Show(actor));
				return;
			case "element":
				var element = currency.PatternDictionary.Values.SelectMany(x => x).SelectMany(x => x.Elements).GetById(ss.PopSpeech());
				if (element is null)
				{
					actor.OutputHandler.Send(
						$"The {currency.Name.ColourValue()} currency does not have such a pattern element.");
					return;
				}

				actor.OutputHandler.Send(element.Show(actor));
				return;
			default:
				actor.OutputHandler.Send("You must either specify #3pattern#0, #3element#0, #3coin#0 or #3division#0.".SubstituteANSIColour());
				return;
		}
	}

	#endregion

	#region Coins
	public const string CoinHelp = @"You can use this building command to edit coins, which are virtual items that exist in currency piles and used for economic transactions. Keep in mind that your coins don't have to perfectly match your currency divisions and you can even have coins that can be loaded (perhaps by an admin or a prog) but won't be used to automatically generate change for example.

The syntax for editing coins is as follows:

	#3coin list#0 - lists all of the coins (see below for filters)
    #3coin edit <which>#0 - begins editing a coin
    #3coin edit new <currency> <name> <value>#0 - generates a new coin
    #3coin clone <old> <new>#0 - clones an existing coin to a new one
    #3coin close#0 - stops editing a coin
    #3coin show <which>#0 - views information about a coin
    #3coin show#0 - views information about your currently editing coin

You can use the following search filters:

	#6+<keyword>#0 - include coins with this keyword in the name, sdesc or desc
	#6-<keyword>#0 - exclude coins with this keyword in the name, sdesc or desc
	#6<currency>#0 - only include coins from this currency
	#6change#0 - only include coins that are used for change
	#6!change#0 - only include coins that are not used for change";

	[PlayerCommand("Coin", "coin")]
	[HelpInfo("coin", CoinHelp, AutoHelp.HelpArgOrNoArg)]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Coin(ICharacter actor, string command)
	{
		BaseBuilderModule.GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.CoinHelper);
	}

	#endregion

	[PlayerCommand("List", "list")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("list", @"The list command is used with systems like shops to show you the inventory for sale.

At its simplest, the syntax is simply LIST. If there are multiple things in the room that can accept a LIST command you can specify which one you want with the syntax LIST *<thing>, e.g. LIST *vending

If you are in a shop, you can view the list output as a specific line of credit account (which may include custom discounts) with the syntax LIST ~<account> <password>, e.g. LIST ~cityguard thinblueline",
		AutoHelp.HelpArg)]
	protected static void List(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var listables = actor.Location
			.LayerGameItems(actor.RoomLayer)
			.SelectNotNull(x => x.GetItemType<IListable>())
			.Where(x => actor.IsAdministrator() || x.Parent.GetItemType<IOnOff>()?.SwitchedOn != false)
			.ToList();

		IListable listable;
		IShop shop = null;
		if (actor.Location.Shop is not null)
		{
			shop = actor.Location.Shop;
		}
		else
		{
			var stall = actor.Location.
			LayerGameItems(actor.RoomLayer).
			SelectNotNull(x => x.GetItemType<IShopStall>()).
			FirstOrDefault(x => x.Shop is not null);
			shop = stall?.Shop;
		}

		if (ss.IsFinished)
		{
			if (shop is not null)
			{
				shop.ShowList(actor, actor);
				return;
			}

			listable = listables.FirstOrDefault();
			if (listable == null)
			{
				actor.Send("There is nothing here for which you can view a list of stock.");
				return;
			}
		}
		else
		{
			if (shop is not null && ss.Peek()[0] != '*')
			{
				ILineOfCreditAccount account = null;
				var arg = ss.PopSpeech();

				bool HandleAccountArgument()
				{
					arg = arg.RemoveFirstCharacter();
					account = shop.LineOfCreditAccounts.FirstOrDefault(x => x.Name.EqualTo(arg));
					if (account == null)
					{
						// TODO - echoed by shopkeeper?
						actor.OutputHandler.Send("There is no such line of credit account associated with this shop.");
						return false;
					}

					switch (account.IsAuthorisedToUse(actor, 0.0M))
					{
						case LineOfCreditAuthorisationFailureReason.None:
						case LineOfCreditAuthorisationFailureReason.AccountOverbalanced:
						case LineOfCreditAuthorisationFailureReason.UserOverbalanced:
							break;
						case LineOfCreditAuthorisationFailureReason.NotAuthorisedAccountUser:
							// TODO - echoed by shopkeeper?
							actor.OutputHandler.Send("You are not an authorised user of that account.");
							return false;

						case LineOfCreditAuthorisationFailureReason.AccountSuspended:
							// TODO - echoed by shopkeeper?
							actor.OutputHandler.Send("That account has been suspended.");
							return false;
						default:
							actor.OutputHandler.Send("There is a problem with that account.");
							return false;
					}

					return true;
				}

				IMerchandise merch = null;
				if (arg[0] == '~')
				{
					if (!HandleAccountArgument())
					{
						return;
					}
				}
				else
				{
					merch = shop.StockedMerchandise.GetFromItemListByKeyword(arg, actor);
					if (merch == null)
					{
						actor.OutputHandler.Send(
							"There is no such merchandise for sale in this shop that you can view detailed information for.");
						return;
					}

					arg = ss.PopSpeech();
					if (!string.IsNullOrEmpty(arg) && arg[0] == '~')
					{
						if (!HandleAccountArgument())
						{
							return;
						}
					}
				}

				shop.ShowList(actor,
					account?.IsAccountOwner(actor) == false
						? actor.Gameworld.TryGetCharacter(account.AccountOwnerId, true)
						: actor, merch);
				return;
			}

			var text = ss.PopSpeech();
			if (!string.IsNullOrEmpty(text) && text[0] == '*')
			{
				text = text.RemoveFirstCharacter();
			}

			var listableItem =
				listables.Select(x => x.Parent).GetFromItemListByKeyword(text, actor);
			if (listableItem == null)
			{
				actor.Send("You do not see anything by that keyword that you can list the stock of.");
				return;
			}

			listable = listableItem.GetItemType<IListable>();
		}

		actor.Send(listable.ShowList(actor, ss.RemainingArgument ?? ""));
	}

	[PlayerCommand("Preview", "preview")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("preview", @"The preview command allows you to see the specific items that you would buy if you used a particular combination of syntax for buy.

The syntax is as follows:

	#3preview <thing>#0 - previews buying a specified item
	#3preview <quantity> <thing>#0- previews buying the specified quantity of the thing", AutoHelp.HelpArgOrNoArg)]
	protected static void Preview(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsTrading || (shop as ITransientShop)?.CurrentStall?.IsTrading == false)
		{
			actor.OutputHandler.Send("This shop is not currently trading.");
			return;
		}

		var firstArg = ss.PopSpeech();
		var target = firstArg;
		var quantity = 1;
		if (!ss.IsFinished && int.TryParse(firstArg, out var newquantity))
		{
			if (newquantity < 1)
			{
				actor.OutputHandler.Send("You must specify a quantity that is 1 or more.");
				return;
			}

			quantity = newquantity;
			target = ss.PopSpeech();
		}

		var merch = shop.StockedMerchandise.GetFromItemListByKeywordIncludingNames(target, actor);
		if (merch == null)
		{
			actor.OutputHandler.Send(
				"This shop doesn't appear to be selling anything like that.\nSee LIST for a list of merchandise for sale.");
			return;
		}

		var (truth, reason) = shop.CanBuy(actor, merch, quantity, null);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You cannot buy {quantity}x {merch.Item.ShortDescription.Colour(merch.Item.CustomColour ?? Telnet.Green)} because {reason}.");
			return;
		}

		var price = shop.PriceForMerchandise(actor, merch, quantity);
		var items = new List<IGameItem>();
		var count = 0;
		
		foreach (var item in shop.AllStockedItems)
		{
			if (count + item.Quantity <= quantity)
			{
				items.Add(item);
				count += item.Quantity;
			}
			else
			{
				items.Add(item.PeekSplit(quantity - count));
				count = quantity;
			}

			if (count >= quantity)
			{
				break;
			}
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Previewing the purchase of {quantity}x {merch.Item.ShortDescription.Colour(merch.Item.CustomColour ?? Telnet.Green)}:");
		sb.AppendLine(
			$"The price would be {shop.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).Colour(Telnet.Green)}.");
		sb.AppendLine($"You would get the following specific items:");
		foreach (var item in items)
		{
			sb.AppendLine($"\t{item.HowSeen(actor)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Buy", "buy")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("buy",
		@"The buy command is used in shops to buy goods from the shop. It is used in conjunction with the LIST command to see merchandise for sale and the PREVIEW command to preview items for sale.

The syntax for this command is as follows:

	#3buy <thing>#0 - buys a specified item
	#3buy <quantity> <thing>#0- buys the specified quantity of the thing
	#3buy [<quantity>] <thing> account <accountname>#0 - buys the the thing with a line of credit account
	#3buy [<quantity>] <thing> with <item>#0 - buys the thing with a payment item such as a cheque, credit card, writ, etc.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Buy(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsTrading || (shop as ITransientShop)?.CurrentStall?.IsTrading == false)
		{
			actor.OutputHandler.Send("This shop is not currently trading.");
			return;
		}

		var firstArg = ss.PopSpeech();
		var target = firstArg;
		var quantity = 1;
		if (!ss.IsFinished && int.TryParse(firstArg, out var newquantity))
		{
			if (newquantity < 1)
			{
				actor.OutputHandler.Send("You must specify a quantity that is 1 or more.");
				return;
			}

			quantity = newquantity;
			target = ss.PopSpeech();
		}

		IMerchandise merch;
		if (int.TryParse(target, out var value))
		{
			merch = shop.StockedMerchandise.ElementAtOrDefault(value - 1);
		}
		else
		{
			merch = shop.StockedMerchandise.GetFromItemListByKeywordIncludingNames(target, actor);
		}

		if (merch == null)
		{
			actor.OutputHandler.Send(
				"This shop doesn't appear to be selling anything like that.\nSee LIST for a list of merchandise for sale.");
			return;
		}

		IPaymentMethod payment = null;
		if (ss.IsFinished)
		{
			payment = new ShopCashPayment(shop.Currency, shop, actor);
		}
		else
		{
			switch (ss.PopSpeech().ToLowerInvariant())
			{
				case "account":
				case "credit":
				case "cred":
					if (ss.IsFinished)
					{
						actor.OutputHandler.Send(
							"What is the name of the line of credit account you'd like to bill to?");
						return;
					}

					var accn = ss.PopSpeech();
					var loc = shop.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(accn));
					if (loc == null)
					{
						// TODO - echoed by shopkeep?
						actor.OutputHandler.Send("There is no line of credit account associated with this shop.");
						return;
					}

					payment = new LineOfCreditPayment(actor, loc);
					break;
				case "with":
				case "card":
				case "keycard":
					if (!actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
					{
						goto default;
					}

					if (shop.BankAccount is null)
					{
						actor.OutputHandler.Send("This shop does not accept non-cash payment.");
						return;
					}

					if (ss.IsFinished)
					{
						actor.OutputHandler.Send("What do you want to pay with?");
						return;
					}

					var item = actor.TargetPersonalItem(ss.SafeRemainingArgument);
					if (item is null)
					{
						actor.OutputHandler.Send("You don't see anything like that.");
						return;
					}

					var paymentItem = item.GetItemType<IBankPaymentItem>();
					if (paymentItem is null)
					{
						actor.OutputHandler.Send(
							$"{item.HowSeen(actor, true)} is not something that be used to pay for things.");
						return;
					}

					payment = new BankPayment(actor, paymentItem, shop);
					break;
				default:
					if (actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
					{
						actor.OutputHandler.Send(
							$"If you specify an argument after the thing you want to buy, it must be either CREDIT <account name> or WITH <card>.");
					}
					else
					{
						actor.OutputHandler.Send(
							$"If you specify an argument after the thing you want to buy, it must be CREDIT <account name>.");
					}

					return;
			}
		}

		var (truth, reason) = shop.CanBuy(actor, merch, quantity, payment);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You cannot buy {quantity}x {merch.Item.ShortDescription.Colour(merch.Item.CustomColour ?? Telnet.Green)} because {reason}");
			return;
		}

		shop.Buy(actor, merch, quantity, payment);
	}

	[PlayerCommand("Sell", "Sell")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("sell", @"The sell command is used to sell items that you have to a shop. Not all shops buy items, and shops have to be explicitly set to buy the items that you're selling. Finally, if you're holding a stack of something, you will be trying to sell the whole stack. Try splitting it up if you want to sell less.

The syntax for this command is as follows:

	#3sell <item>#0", AutoHelp.HelpArgOrNoArg)]
	protected static void Sell(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var item = actor.TargetHeldItem(ss.PopSpeech());
		if (item is null)
		{
			actor.OutputHandler.Send("You aren't holding anything like that.");
			return;
		}

		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsTrading || (shop as ITransientShop)?.CurrentStall?.IsTrading == false)
		{
			actor.OutputHandler.Send("This shop is not currently trading.");
			return;
		}

		IMerchandise merch = null;
		if (!ss.IsFinished)
		{
			merch = shop.Merchandises
			            .Where(x => x.IsMerchandiseFor(item))
			            .GetFromItemListByKeyword(ss.PopSpeech(), actor);
			if (merch == null)
			{
				actor.OutputHandler.Send(
					$"There is no matching merchandise profile for {item.HowSeen(actor)} with the specified keywords. The shop will not buy that item.");
				return;
			}
		}
		else
		{
			merch = shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item));
			if (merch == null)
			{
				actor.OutputHandler.Send(
					$"There is no merchandise profile for items like {item.HowSeen(actor)}. The shop will not buy that item.");
				return;
			}
		}

		if (!merch.WillBuy)
		{
			actor.OutputHandler.Send($"This shop does not buy items like {item.HowSeen(actor)}.");
			return;
		}

		var condition = Math.Min(item.Condition, item.DamageCondition);
		if (condition < merch.MinimumConditionToBuy)
		{
			actor.OutputHandler.Send($"Unfortunately, {item.HowSeen(actor)} is in too poor condition for this shop to accept.");
			return;
		}

		IPaymentMethod payment = null;
		if (ss.IsFinished)
		{
			payment = new ShopCashPayment(shop.Currency, shop, actor);
		}
		else
		{
			switch (ss.PopSpeech().ToLowerInvariant())
			{
				case "account":
				case "credit":
				case "cred":
					if (ss.IsFinished)
					{
						actor.OutputHandler.Send(
							"What is the name of the line of credit account you'd like to credit to?");
						return;
					}

					var accn = ss.PopSpeech();
					var loc = shop.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(accn));
					if (loc == null)
					{
						// TODO - echoed by shopkeep?
						actor.OutputHandler.Send("There is no such line of credit account associated with this shop.");
						return;
					}

					payment = new LineOfCreditPayment(actor, loc);
					break;
				case "with":
				case "card":
				case "keycard":
					if (!actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
					{
						goto default;
					}

					if (shop.BankAccount is null)
					{
						actor.OutputHandler.Send("This shop does not accept non-cash payment.");
						return;
					}

					if (ss.IsFinished)
					{
						actor.OutputHandler.Send("What payment item do you want to be paid to?");
						return;
					}

					var targetItem = actor.TargetPersonalItem(ss.SafeRemainingArgument);
					if (targetItem is null)
					{
						actor.OutputHandler.Send("You don't see anything like that.");
						return;
					}

					var paymentItem = targetItem.GetItemType<IBankPaymentItem>();
					if (paymentItem is null)
					{
						actor.OutputHandler.Send(
							$"{targetItem.HowSeen(actor, true)} is not something that be used to pay for things.");
						return;
					}

					payment = new BankPayment(actor, paymentItem, shop);
					break;
				default:
					if (actor.Gameworld.GetStaticBool("KeycardPaymentsEnabled"))
					{
						actor.OutputHandler.Send(
							$"If you specify an argument after the thing you want to sell, it must be either CREDIT <account name> or WITH <card>.");
					}
					else
					{
						actor.OutputHandler.Send(
							$"If you specify an argument after the thing you want to sell, it must be CREDIT <account name>.");
					}

					return;
			}
		}

		var (truth, reason) = shop.CanSell(actor, merch, payment, item);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You cannot sell {item.HowSeen(actor)} because {reason}.");
			return;
		}

		shop.Sell(actor, merch, payment, item);
	}

	[PlayerCommand("Shop", "shop")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("shop", ShopHelpPlayers, AutoHelp.HelpArgOrNoArg, ShopHelpAdmins)]
	protected static void Shop(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clockin":
			case "clock in":
			case "clock-in":
				ShopClockIn(actor);
				return;
			case "clockout":
			case "clock out":
			case "clock-out":
				ShopClockOut(actor);
				return;
			case "employ":
				ShopEmploy(actor, ss);
				return;
			case "fire":
				ShopFire(actor, ss);
				return;
			case "manager":
				ShopManager(actor, ss);
				return;
			case "proprietor":
				ShopProprietor(actor, ss);
				return;
			case "till":
				ShopTill(actor, ss);
				return;
			case "display":
				ShopDisplay(actor, ss);
				return;
			case "stock":
				ShopStock(actor, ss);
				return;
			case "merch":
			case "merchandise":
				ShopMerchandise(actor, ss);
				return;
			case "set":
				ShopSet(actor, ss);
				return;
			case "quit":
				ShopQuit(actor, ss);
				return;
			case "dispose":
				ShopDispose(actor, ss);
				return;
			case "ledger":
				ShopLedger(actor, ss);
				return;
			case "bank":
				ShopBank(actor, ss);
				return;
			case "account":
				ShopAccount(actor, ss);
				return;
			case "payaccount":
				ShopPayAccount(actor, ss);
				return;
			case "accountstatus":
				ShopAccountStatus(actor, ss);
				return;
			case "paytax":
			case "paytaxes":
				ShopPayTax(actor, ss);
				return;
			case "info":
				ShopInfo(actor, ss);
				return;
			case "autostock":
				ShopAutostock(actor, ss);
				return;
			case "open":
				ShopOpen(actor, ss);
				return;
			case "close":
				ShopClose(actor, ss);
				return;
		}

		if (actor.IsAdministrator())
		{
			switch (ss.Last.ToLowerInvariant())
			{
				case "setupstall":
					ShopSetupStall(actor, ss);
					return;
				case "create":
					ShopCreate(actor, ss);
					return;
				case "createstall":
					ShopCreateStall(actor, ss);
					return;
				case "list":
					ShopList(actor, ss);
					return;
				case "economy":
					ShopEconomy(actor, ss);
					return;
				case "delete":
					ShopDelete(actor, ss);
					return;
				case "extend":
					ShopExtend(actor, ss);
					return;
				case "remove":
					ShopRemove(actor, ss);
					return;
			}
		}

		actor.OutputHandler.Send("That is not a valid option. See SHOP HELP for more information.");
	}

	#region Shop Subcommands

	private const string ShopHelpPlayers = @"You can use the following options with the shop command:

	#3shop payaccount <account> <amount>#0 - pays off a line of credit account
	#3shop paytax <amount>|all#0 - pays owing taxes out of the till
	#3shop accountstatus <account>#0 - inquires about the status of a line of credit account
	#3shop account ...#0 - allows store managers to configure line of credit accounts. See #3SHOP ACCOUNT HELP#0.
	#3shop clockin#0 - clocks in as an on-duty employee
	#3shop clockout#0 - clocks out as an off-duty employee	
	#3shop employ <target>#0 - employs someone with the store
	#3shop quit#0 - quits employment with this store
	#3shop fire <target>|<name>#0 - fires an employee from this store
	#3shop manager <target>|<name>#0 - toggles an employee's status as a manager
	#3shop proprietor <target>|<name>#0 - toggles and employee's status as a proprietor
	#3till <target>#0 - toggles an item being used as a till for the store
	#3shop display <target>#0 - toggles an item being used as a display cabinet for the store
	#3shop info#0 - shows detailed information about the shop
	#3shop stock <target>#0 - adds an item as shop inventory
	#3shop dispose <target>#0 - disposes of an item from shop inventory
	#3shop ledger [<period#>]#0 - views the financial ledger for the shop
	#3shop bank <account>#0 - sets the bank account for the shop
	#3shop bank none#0 - sets this shop to no longer use a bank account
	#3shop merchandise <other args>#0 - edits merchandise. See #3SHOP MERCHANDISE HELP#0.
	#3shop open <shop>#0 - opens a shop for trading
	#3shop close <shop>#0 - closes a shop to trading
	#3shop set name <name>#0 - renames a shop
	#3shop set can <prog> <whyprog>#0 - sets a prog to control who can shop here (and associated error message)
	#3shop set trading#0 - toggles whether this shop is trading
	#3shop set minfloat <amount>#0 - sets the minimum float for the shop to buy anything";

	private const string ShopHelpAdmins = @"You can use the following options with the shop command:

	#3shop payaccount <account> <amount>#0 - pays off a line of credit account
	#3shop paytax <amount>|all#0 - pays owing taxes out of the till
	#3shop accountstatus <account>#0 - inquires about the status of a line of credit account
	#3shop account ...#0 - allows store managers to configure line of credit accounts. See #3SHOP ACCOUNT HELP#0.
	#3shop clockin#0 - clocks in as an on-duty employee
	#3shop clockout#0 - clocks out as an off-duty employee	
	#3shop employ <target>#0 - employs someone with the store
	#3shop quit#0 - quits employment with this store
	#3shop fire <target>|<name>#0 - fires an employee from this store
	#3shop manager <target>|<name>#0 - toggles an employee's status as a manager
	#3shop proprietor <target>|<name>#0 - toggles and employee's status as a proprietor
	#3shop till <target>#0 - toggles an item being used as a till for the store
	#3shop display <target>#0 - toggles an item being used as a display cabinet for the store
	#3shop info#0 - shows detailed information about the shop
	#3shop stock <target>#0 - adds an item as shop inventory
	#3shop dispose <target>#0 - disposes of an item from shop inventory
	#3shop ledger [<period#>]#0 - views the financial ledger for the shop
	#3shop bank <account>#0 - sets the bank account for the shop
	#3shop bank none#0 - sets this shop to no longer use a bank account
	#3shop merchandise <other args>#0 - edits merchandise. See #3SHOP MERCHANDISE HELP#0.
	#3shop open <shop>#0 - opens a shop for trading
	#3shop close <shop>#0 - closes a shop to trading
	#3shop set name <name>#0 - renames a shop
	#3shop set can <prog> <whyprog>#0 - sets a prog to control who can shop here (and associated error message)
	#3shop set trading#0 - toggles whether this shop is trading
	#3shop set minfloat <amount>#0 - sets the minimum float for the shop to buy anything

Additionally, you can use the following shop admin subcommands:

	#3shop list#0 - lists all shops
	#3shop info <which>#0 - shows a shop you're not in
	#3shop economy#0 - a modified list that shows some economic info
	#3shop create <name> <econzone>#0 - creates a new store with the specified name
	#3shop delete#0 - deletes the shop you're currently in. Warning: Irreversible.
	#3shop extend <direction> [stockroom|workshop]#0 - extends the shop in the specified direction, optionally as the stockroom or workshop
	#3shop extend <shop> <direction> [stockroom|workshop]#0 - extends the specified shop in the specified direction, optionally as the stockroom or workshop
	#3shop remove#0 - removes the current location from its shop.
	#3shop autostock#0 - automatically loads and stocks items up to the minimum reorder levels for all merchandise
	#3shop setupstall <stall> <shop>#0 - sets up a stall item as belonging to a shop";
	private static void ShopClose(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}


		if (!actor.IsAdministrator() && !shop.IsEmployee(actor))
		{
			actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().ColourName()}.");
			return;
		}

		var tShop = shop as ITransientShop;
		if (tShop is not null)
		{
			var stall = actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<IShopStall>()).FirstOrDefault(x => x.Shop == shop);
			if (stall is null)
			{
				actor.OutputHandler.Send($"There is no stall for {shop.Name.TitleCase().ColourName()} in this location.");
				return;
			}

			if (!stall.IsTrading)
			{
				actor.OutputHandler.Send($"{stall.Parent.HowSeen(actor, true)} is not trading.");
				return;
			}

			stall.IsTrading = false;
			if (shop.IsTrading)
			{
				shop.ToggleIsTrading();
			}
			actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ close|closes $1 from business", actor, actor, stall.Parent)));
			return;
		}

		if (!shop.IsTrading)
		{
			actor.OutputHandler.Send($"{shop.Name.TitleCase().ColourName()} is not trading.");
			return;
		}

		shop.ToggleIsTrading();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ close|closes the shop for business.", actor, actor)));
	}

	private static void ShopOpen(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!actor.IsAdministrator() && !shop.IsEmployee(actor))
		{
			actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().ColourName()}.");
			return;
		}

		var tShop = shop as ITransientShop;
		if (tShop is not null)
		{
			var stall = actor.Location.LayerGameItems(actor.RoomLayer).SelectNotNull(x => x.GetItemType<IShopStall>()).FirstOrDefault(x => x.Shop == shop);
			if (stall is null)
			{
				actor.OutputHandler.Send($"There is no stall for {shop.Name.TitleCase().ColourName()} in this location.");
				return;
			}

			if (stall.IsTrading)
			{
				actor.OutputHandler.Send($"{stall.Parent.HowSeen(actor, true)} is already trading.");
				return;
			}

			stall.IsTrading = true;
			if (!shop.IsTrading)
			{
				shop.ToggleIsTrading();
			}
			actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ open|opens $1 for business, trading as {shop.Name.TitleCase().ColourName()}.", actor, actor, stall.Parent)));
			return;
		}

		if (shop.IsTrading)
		{
			actor.OutputHandler.Send($"{shop.Name.TitleCase().ColourName()} is already trading.");
			return;
		}

		shop.ToggleIsTrading();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ open|opens the shop for business.", actor, actor)));
	}

	private static void ShopSetupStall(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which shop stall item did you want to set up?");
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		if (item == null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		var itemAsStall = item.GetItemType<IShopStall>();
		if (itemAsStall is null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not a shop stall.");
			return;
		}

		if (itemAsStall.IsTrading)
		{
			actor.OutputHandler.Send("You can't change the shop associated with a trading stall. Close the stall first.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What shop do you want to set this stall to be associated with?");
			return;
		}

		if (ss.SafeRemainingArgument.Equals("none"))
		{
			if (itemAsStall.Shop is not null)
			{
				var oldShop = itemAsStall.Shop;
				itemAsStall.Shop = null;
				oldShop.CurrentStall = null;
				oldShop.StocktakeAllMerchandise();
			}
			
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is no longer affiliated with any shop.");
			return;
		}

		var shop = actor.Gameworld.Shops.GetByIdOrName(ss.SafeRemainingArgument);
		if (shop is null)
		{
			actor.OutputHandler.Send("There is no such shop.");
			return;
		}

		var tShop = shop as ITransientShop;
		if (tShop is null)
		{
			actor.OutputHandler.Send($"{shop.Name.TitleCase().ColourName()} is not a transient shop and so cannot be used with a stall.");
			return;
		}

		if (tShop.CurrentStall is not null)
		{
			if (tShop.CurrentStall == itemAsStall)
			{
				actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is already the stall for that shop.");
				return;
			}

			tShop.CurrentStall.Shop = null;
			tShop.CurrentStall.IsTrading = false;	
		}

		itemAsStall.Shop = tShop;
		tShop.CurrentStall = itemAsStall;
		tShop.StocktakeAllMerchandise();
		actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is now affiliated with the {shop.Name.TitleCase().ColourName()} shop.");
	}

	private static bool DoShopCommandFindShop(ICharacter actor, out IShop shop)
	{
		if (actor.Location.Shop is not null)
		{
			shop = actor.Location.Shop;
			return true;
		}

		var stall = actor.Location.
			LayerGameItems(actor.RoomLayer).
			SelectNotNull(x => x.GetItemType<IShopStall>()).
			FirstOrDefault(x => x.Shop is not null);
		if (stall is null)
		{
			actor.OutputHandler.Send("You are not currently at a shop or in the presence of a market stall.");
			shop = null;
			return false;
		}

		shop = stall.Shop;
		return true;
	}

	private static void ShopBank(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}
		
		if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send(
				"You are not a proprietor of this establishment and so cannot alter its bank account details.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a bank account or use {"none".ColourCommand()} to clear one.");
			return;
		}

		if (ss.PeekSpeech().EqualTo("none"))
		{
			shop.BankAccount = null;
			actor.OutputHandler.Send("This shop no longer has a bank account.");
			return;
		}

		var (account, error) = Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
		if (account is null)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!account.IsAuthorisedAccountUser(actor))
		{
			actor.OutputHandler.Send("You are not authorised to use that bank account.");
			return;
		}

		shop.BankAccount = account;
		actor.OutputHandler.Send(
			$"This shop will now use the {account.AccountReference.ColourValue()} bank account for its transactions.");
	}

	private static void ShopPayTax(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not a manager of this establishment and so cannot pay its taxes.");
			return;
		}

		decimal amount;
		if (ss.SafeRemainingArgument.EqualTo("all"))
		{
			amount = shop.EconomicZone.OutstandingTaxesForShop(shop);
		}
		else
		{
			amount = shop.Currency.GetBaseCurrency(ss.SafeRemainingArgument, out var success);
			if (!success || amount <= 0.0M)
			{
				actor.OutputHandler.Send(
					$"That is not a valid amount of the {shop.Currency.Name.ColourName()} currency.");
				return;
			}
		}

		if (amount <= 0.0M)
		{
			actor.OutputHandler.Send("This shop does not owe any money in taxes.");
			return;
		}

		var currencyPiles = shop.GetCurrencyPilesForShop().ToList();
		var targetCoins = shop.Currency.FindCurrency(currencyPiles, amount);
		var value = targetCoins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));

		if (value < amount)
		{
			actor.OutputHandler.Send(
				$"This shop cannot afford to pay that much. The most it could pay is {shop.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
			return;
		}

		var change = value - amount;
		foreach (var item in targetCoins.Where(item =>
					 !item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value)))))
		{
			item.Key.Parent.Delete();
		}

		var counter = new Counter<ICoin>();
		foreach (var item in targetCoins)
			foreach (var coin in item.Value)
			{
				counter[coin.Key] += coin.Value;
			}

		if (change > 0.0M)
		{
			shop.AddCurrencyToShop(CurrencyGameItemComponentProto.CreateNewCurrencyPile(shop.Currency,
					shop.Currency.FindCoinsForAmount(change, out _)));
		}

		shop.EconomicZone.PayTaxesForShop(shop, amount);
		actor.OutputHandler.Send(
			$"You pay {shop.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in taxes for {shop.Name.TitleCase().ColourName()}. The shop now owes {shop.Currency.Describe(shop.EconomicZone.OutstandingTaxesForShop(shop), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in taxes.");
	}
	private static void ShopAccountStatus(ICharacter actor, StringStack command)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsTrading)
		{
			actor.OutputHandler.Send("This shop is not currently trading.");
			return;
		}

		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		if (account.AccountUsers.All(x => x.Id != actor.Id) && !actor.IsAdministrator() && !shop.IsEmployee(actor))
		{
			actor.OutputHandler.Send("You are not authorised to view that account.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Line of Credit Account {account.AccountName.ColourName()} for {shop.Name.TitleCase().ColourName()}");
		sb.AppendLine(account.IsSuspended
			? $"The account is currently suspended from trading.".Colour(Telnet.BoldMagenta)
			: $"The account is currently in good standing.".Colour(Telnet.BoldGreen));
		sb.AppendLine(
			$"The account has a limit of {shop.Currency.Describe(account.AccountLimit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		sb.AppendLine(
			$"The account has an outstanding balance of {shop.Currency.Describe(account.OutstandingBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		sb.AppendLine(
			$"You are personally authorised to spend {shop.Currency.Describe(account.MaximumAuthorisedToUse(actor), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		if (actor.Id == account.AccountOwnerId)
		{
			sb.AppendLine();
			sb.AppendLine("The following people are authorised to use this account:");
			foreach (var user in account.AccountUsers)
			{
				sb.AppendLine(
					$"\t{user.PersonalName.GetName(NameStyle.FullName).ColourName()}{(!user.SpendingLimit.HasValue ? " [no limit]".ColourValue() : $" [{shop.Currency.Describe(user.SpendingLimit.Value, CurrencyDescriptionPatternType.ShortDecimal)}]".ColourValue())}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ShopPayAccount(ICharacter actor, StringStack command)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsReadyToDoBusiness)
		{
			actor.OutputHandler.Send("This shop has not been properly set up to do business.");
			return;
		}

		if (!shop.IsTrading)
		{
			actor.OutputHandler.Send("This shop is not currently trading.");
			return;
		}

		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much do you want to pay towards that account's outstanding balance?");
			return;
		}

		var amount = shop.Currency.GetBaseCurrency(command.SafeRemainingArgument, out var success);
		if (!success)
		{
			actor.OutputHandler.Send($"That is not a valid amount of the {shop.Currency.Name.ColourName()} currency.");
			return;
		}

		var payment = new ShopCashPayment(shop.Currency, shop, actor);
		if (payment.AccessibleMoneyForPayment() < amount)
		{
			actor.OutputHandler.Send("You do not have enough cash on hand to pay that amount.");
			return;
		}

		payment.TakePayment(amount);
		account.PayoffAccount(amount);
		shop.CashBalance += amount;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ pay|pays $1 towards the {account.AccountName.ColourName()} line of credit account.", actor, actor,
			new DummyPerceivable(shop.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)))));
	}

	public const string ShopAccountHelp = @"You can use the following options with this subcommand:

	#3shop account list#0 - shows all line of credit accounts for this shop
	#3shop account show <account>#0 - shows information about a specific account
	#3shop account create <name> <owner> <spending limit>#0 - creates a new credit account
	#3shop account close <name>#0 - closes a credit account
	#3shop account authorise <account> <who> [<limit>]#0 - authorises someone to use an account with optional transaction limit
	#3shop account deauthorise <account> <who>#0 - deauthorises someone from an account
	#3shop account suspend <account>#0 - toggles trading on an account
	#3shop account owner <account> <who>#0 - sets a new account owner
	#3shop account limit <account> <amount>#0 - sets a new account spending limit
	#3shop account userlimit <account> <user> <limit>#0 - sets a new user-specific spending limit (use 0 for no limit)";

	private static void ShopAccount(ICharacter actor, StringStack command)
	{
		if (command.PeekSpeech().EqualToAny("?", "help"))
		{
			actor.OutputHandler.Send(ShopAccountHelp.SubstituteANSIColour());
			return;
		}

		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!actor.Location.Shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send($"You are not a manager of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			return;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				ShopAccountCreate(actor, shop, command);
				return;
			case "remove":
			case "rem":
			case "delete":
			case "close":
				ShopAccountClose(actor, shop, command);
				return;
			case "authorise":
			case "authorize":
				ShopAccountAuthorise(actor, shop, command);
				return;
			case "deauthorise":
			case "deauthorize":
				ShopAccountDeauthorise(actor, shop, command);
				return;
			case "suspend":
				ShopAccountSuspend(actor, shop, command);
				return;
			case "limit":
				ShopAccountLimit(actor, shop, command);
				return;
			case "userlimit":
			case "user limit":
			case "user_limit":
				ShopAccountUserLimit(actor, shop, command);
				return;
			case "owner":
				ShopAccountOwner(actor, shop, command);
				return;
			case "list":
				ShopAccountList(actor, shop, command);
				return;
			case "show":
				ShopAccountShow(actor, shop, command);
				return;
			default:
				actor.OutputHandler.Send(ShopAccountHelp.SubstituteANSIColour());
				break;
		}
	}

	private static void ShopAccountShow(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		actor.OutputHandler.Send(account.Show(actor));
	}

	private static void ShopAccountList(ICharacter actor, IShop shop, StringStack command)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Line of Credit Accounts for {shop.Name.TitleCase().ColourName()}:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from account in shop.LineOfCreditAccounts
			select new List<string>
			{
				account.AccountName,
				account.AccountOwnerName.GetName(NameStyle.FullName),
				shop.Currency.Describe(account.AccountLimit, CurrencyDescriptionPatternType.ShortDecimal),
				shop.Currency.Describe(account.OutstandingBalance, CurrencyDescriptionPatternType.ShortDecimal),
				account.AccountUsers.Count().ToString("N0", actor),
				account.IsSuspended.ToString()
			},
			new List<string> { "Name", "Owner", "Limit", "Outstanding", "# Users", "Suspended?" },
			actor.LineFormatLength,
			colour: Telnet.BoldYellow,
			unicodeTable: actor.Account.UseUnicode
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ShopAccountOwner(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Who do you want to set as the new owner of the {account.AccountName.ColourName()} account?");
			return;
		}

		var target = actor.TargetActor(command.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		account.SetAccountOwner(target);
		actor.OutputHandler.Send(
			$"You change the owner of the {account.AccountName.ColourName()} account to {target.HowSeen(actor)}.");
	}

	private static ILineOfCreditAccount GetAccount(ICharacter actor, IShop shop, string whichAccount)
	{
		if (string.IsNullOrWhiteSpace(whichAccount))
		{
			actor.OutputHandler.Send("Which line of credit account do you want to edit?");
			return null;
		}

		var account = shop.LineOfCreditAccounts.FirstOrDefault(x => x.AccountName.EqualTo(whichAccount));
		if (account == null)
		{
			actor.OutputHandler.Send("There is no such line of credit account.");
			return null;
		}

		return account;
	}

	private static void ShopAccountUserLimit(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which account user did you want to set the limit for?");
			return;
		}

		var user = account.AccountUsers.GetByPersonalName(command.SafeRemainingArgument);
		if (user == null)
		{
			actor.OutputHandler.Send(
				$"There is no such account user for that account. The authorised users are as follows:\n{account.AccountUsers.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToCommaSeparatedValues("\n")}");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What limit do you want to set on that user's spending on the {account.AccountName.ColourName()} account?");
			return;
		}

		var amount = account.Currency.GetBaseCurrency(command.SafeRemainingArgument, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid amount of currency.");
			return;
		}

		account.SetLimit(user, amount);
		actor.OutputHandler.Send(
			$"You set the spending limit for {user.PersonalName.GetName(NameStyle.FullName).ColourName()} on the {account.AccountName.ColourName()} account to {account.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void ShopAccountLimit(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What limit do you want to put on the outstanding balance of that account?");
			return;
		}

		var amount = account.Currency.GetBaseCurrency(command.SafeRemainingArgument, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid amount of currency.");
			return;
		}

		account.SetAccountLimit(amount);
		actor.OutputHandler.Send(
			$"You set the total outstanding balance limit on the {account.AccountName.ColourName()} account to {account.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void ShopAccountSuspend(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		account.IsSuspended = !account.IsSuspended;
		if (account.IsSuspended)
		{
			actor.OutputHandler.Send($"You suspend trading on the {account.AccountName.ColourName()} account.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"You remove the suspension of trading on the {account.AccountName.ColourName()} account.");
		}
	}

	private static void ShopAccountDeauthorise(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which account user do you want to deauthorise? The current users are as follows:\n{account.AccountUsers.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToCommaSeparatedValues("\n")}");
			return;
		}

		var user = account.AccountUsers.GetByPersonalName(command.SafeRemainingArgument);
		if (user == null)
		{
			actor.OutputHandler.Send(
				$"There is no such authorised user for that account. The current users are as follows:\n{account.AccountUsers.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToCommaSeparatedValues("\n")}");
			return;
		}

		if (user.Id == account.AccountOwnerId)
		{
			actor.OutputHandler.Send(
				"You cannot deauthorise the account owner. If you want to remove this person you must first set a new account owner.");
			return;
		}

		account.RemoveAuthorisation(user);
		actor.OutputHandler.Send(
			$"You remove {user.PersonalName.GetName(NameStyle.FullName).ColourName()} as an authorised user for the {account.AccountName.ColourName()} line of credit account.");
	}

	private static void ShopAccountAuthorise(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to authorise to use that line of credit account?");
			return;
		}

		var target = actor.TargetActor(command.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		var value = 0.0M;
		if (!command.IsFinished)
		{
			value = account.Currency.GetBaseCurrency(command.SafeRemainingArgument, out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid amount of currency for the user's spending limit.");
				return;
			}
		}

		if (account.AccountUsers.Any(x => x.Id == target.Id))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is already an authorised user of that account.");
			return;
		}

		account.AddAuthorisation(target, value);
		actor.OutputHandler.Send(
			$"You add {target.HowSeen(actor)} as an authorised user for the {account.AccountName.ColourName()} line of credit account{(value > 0.0M ? $" with a spendling limit of {account.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}" : "")}.");
	}

	private static void ShopAccountClose(ICharacter actor, IShop shop, StringStack command)
	{
		var account = GetAccount(actor, shop, command.PopSpeech());
		if (account == null)
		{
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to permanently close the {account.AccountName.ColourName()} line of credit account? This action is irreversible.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You close the {account.AccountName.ColourName()} line of credit account.");
				shop.RemoveLineOfCredit(account);
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to close the {account.AccountName.ColourName()} line of credit account.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to close the {account.AccountName.ColourName()} line of credit account.");
			},
			Keywords = new List<string> { "close", "credit", "account" },
			DescriptionString = "Closing a line of credit account"
		}));
		throw new NotImplementedException();
	}

	private static void ShopAccountCreate(ICharacter actor, IShop shop, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this line of credit account?");
			return;
		}

		var name = command.PopSpeech();
		if (shop.LineOfCreditAccounts.Any(x => x.AccountName.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a line of credit account with that name. Names must be unique.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to set as the owner of that line of credit account?");
			return;
		}

		var target = actor.TargetActor(command.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What outstanding balance limit do you want to put on the new account?");
			return;
		}

		var amount = shop.Currency.GetBaseCurrency(command.SafeRemainingArgument, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid amount of currency.");
			return;
		}

		var account = new LineOfCreditAccount(shop, name, target, amount);
		actor.Gameworld.Add(account);
		shop.AddLineOfCredit(account);
		actor.OutputHandler.Send(
			$"You create a new line of credit account for {shop.Name.TitleCase().ColourName()} called {name.ColourName()} owned by {target.HowSeen(actor)} with an outstanding balance limit of {shop.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void ShopAutostock(ICharacter actor, StringStack command)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}


		if (!actor.IsAdministrator() && !shop.IsEmployee(actor))
		{
			actor.OutputHandler.Send("You are not authorised to bring items into stock in this store.");
			return;
		}

		var stocked = new List<IGameItem>();
		if (actor.IsAdministrator())
		{
			foreach (var merchandise in shop.Merchandises)
			{
				stocked.AddRange(shop.DoAutoRestockForMerchandise(merchandise));
			}
		}

		stocked.AddRange(shop.DoAutostockAllMerchandise());

		var sb = new StringBuilder();
		sb.AppendLine("You add the following items to stock:");
		foreach (var item in stocked)
		{
			sb.AppendLine($"\t{item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ShopRemove(ICharacter actor, StringStack command)
	{
		var shop = actor.Location.Shop;
		if (shop == null)
		{
			actor.OutputHandler.Send("You are not currently in a shop.");
			return;
		}

		if (shop.ShopfrontCells.Contains(actor.Location) && shop.ShopfrontCells.Count() == 1)
		{
			actor.OutputHandler.Send(
				"You cannot remove the last shopfront location from a shop. You must add a new one before you can remove this one.");
			return;
		}

		var location = actor.Location;
		actor.OutputHandler.Send(
			$"Are you sure you want to remove this location from the shop?\nType {"accept".ColourCommand()} to accept or {"decline".ColourCommand()} to change your mind.");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send(
					$"You remove {location.HowSeen(actor)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				if (location == shop.WorkshopCell)
				{
					shop.WorkshopCell = null;
					return;
				}

				if (location == shop.StockroomCell)
				{
					shop.StockroomCell = null;
					return;
				}

				shop.RemoveShopfrontCell(location);
				location.Shop = null;
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to remove this location from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to remove this location from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			DescriptionString = $"removing a location from {shop.Name.TitleCase()}",
			Keywords = new List<string> { "remove", "shop", "location" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void ShopExtend(ICharacter actor, StringStack command)
	{
		var shop = actor.Location.Shop;
		if (shop == null)
		{
			if (!command.IsFinished)
			{
				var shopText = command.PopSpeech();
				var gshop = actor.Gameworld.Shops.GetByIdOrName(shopText);
				if (gshop is not null && !command.IsFinished)
				{
					if (gshop is not IPermanentShop ps)
					{
						actor.OutputHandler.Send(
							"This command can only be used with a permanent (brick and mortar) shop.");
						return;
					}

					shop = ps;
				}
				else
				{
					actor.OutputHandler.Send("You are not currently in a shop.");
					return;
				}
			}
			else
			{
				actor.OutputHandler.Send("You are not currently in a shop.");
				return;
			}
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which direction do you want to extend this store in?");
			return;
		}

		var exit = actor.Location.GetExitKeyword(command.PopSpeech(), actor);
		if (exit == null)
		{
			actor.OutputHandler.Send("There is no such exit.");
			return;
		}

		if (exit.Destination.Shop != null)
		{
			actor.OutputHandler.Send(
				"There is already a shop in that direction. Each location can only belong to one shop.");
			return;
		}

		if (command.IsFinished)
		{
			shop.AddShopfrontCell(exit.Destination);
			exit.Destination.Shop = shop;
			actor.OutputHandler.Send(
				$"You add {exit.Destination.HowSeen(actor)} ({exit.OutboundMovementSuffix}) to the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			return;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "store":
			case "storeroom":
			case "stock":
			case "stockroom":
				shop.StockroomCell = exit.Destination;
				exit.Destination.Shop = shop;
				actor.OutputHandler.Send(
					$"You set {exit.Destination.HowSeen(actor)} ({exit.OutboundMovementSuffix}) as the stockroom for shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				return;
			case "work":
			case "workshop":
				shop.WorkshopCell = exit.Destination;
				exit.Destination.Shop = shop;
				actor.OutputHandler.Send(
					$"You set {exit.Destination.HowSeen(actor)} ({exit.OutboundMovementSuffix}) as the workshop for shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				return;
		}

		actor.OutputHandler.Send(
			"You must either supply no argument after the exit to set the location as part of the shopfront, or use STOCKROOM to set it as a stockroom, or WORKSHOP to set it as a workshop.");
	}

	private static void ShopSet(ICharacter actor, StringStack command)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only proprietors can edit shop properties.");
			return;
		}

		shop.BuildingCommand(actor, command);
	}

	private static void ShopQuit(ICharacter actor, StringStack command)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsEmployee(actor))
		{
			actor.OutputHandler.Send("You are not an employee of this shop.");
			return;
		}

		actor.OutputHandler.Send($"Are you sure you wish to quit {shop.Name.TitleCase().ColourName()}, and end your employment with them?");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send(
					$"You quit {shop.Name.TitleCase().Colour(Telnet.Cyan)}, ending your employment with them.");
				shop.RemoveEmployee(actor);
				foreach (var employee in shop.EmployeesOnDuty)
				{
					employee.OutputHandler.Send(
						$"{actor.HowSeen(employee, true)} has quit {actor.ApparentGender(employee).Possessive()} employment with {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
				}
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to quit {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to quit {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			DescriptionString = $"quitting employment with {shop.Name.TitleCase()}",
			Keywords = new List<string> { "quit", "shop", "employment" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void ShopCreate(ICharacter actor, StringStack command)
	{
		// TODO - prevent if a stall is here too
		if (actor.Location.Shop != null)
		{
			actor.OutputHandler.Send(
				"The location you are in is already part of a shop. You must be in a non-shop location to create a new one.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this shop?");
			return;
		}

		var name = command.PopSpeech().TitleCase();

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What economic zone should this shop belong to?");
			return;
		}

		var zone = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.EconomicZones.Get(value)
			: actor.Gameworld.EconomicZones.GetByName(command.Last);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		var newShop = new PermanentShop(zone, actor.Location, name);
		actor.Gameworld.Add(newShop);
		actor.OutputHandler.Send(
			$"You create a new shop at your current location, in the {zone.Name.TitleCase().Colour(Telnet.Cyan)} economic zone called {newShop.Name.TitleCase().Colour(Telnet.Cyan)}.");
	}

	private static void ShopCreateStall(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this shop?");
			return;
		}

		var name = command.PopSpeech().TitleCase();

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What economic zone should this shop belong to?");
			return;
		}

		var zone = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.EconomicZones.Get(value)
			: actor.Gameworld.EconomicZones.GetByName(command.Last);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		var newShop = new TransientShop(zone, name);
		actor.Gameworld.Add(newShop);
		actor.OutputHandler.Send(
			$"You create a new transient shop in the {zone.Name.TitleCase().Colour(Telnet.Cyan)} economic zone called {newShop.Name.TitleCase().Colour(Telnet.Cyan)}.");
	}

	private static void ShopEconomy(ICharacter actor, StringStack command)
	{
		IEconomicZone zone = null;
		if (!command.IsFinished)
		{
			zone = long.TryParse(command.PopSpeech(), out var value)
				? actor.Gameworld.EconomicZones.Get(value)
				: actor.Gameworld.EconomicZones.GetByName(command.Last);
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zone.");
				return;
			}
		}

		var shops = actor.Gameworld.Shops.ToList();
		if (zone != null)
		{
			shops = shops.Where(x => x.EconomicZone == zone).ToList();
		}

		actor.OutputHandler.Send(
			$"List of shops{(zone != null ? $" for economic zone {zone.Name.TitleCase().Colour(Telnet.Cyan)}" : "")}:");
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from shop in shops
			let transactions = shop.TransactionRecords
								   .Where(x => x.RealDateTime >=
											   shop.EconomicZone.CurrentFinancialPeriod.FinancialPeriodStart).ToList()
			let pshop = shop as IPermanentShop
			let cash = pshop?.TillItems.RecursiveGetItems<ICurrencyPile>(false).Where(x => x.Currency == shop.Currency)
						   .Sum(x => x.Coins.Sum(y => y.Item2 * y.Item1.Value)) ?? 0.0M
			let merchandise = shop.StocktakeAllMerchandise()
								  .Sum(x => x.Key.EffectivePrice * (x.Value.InStockroomCount + x.Value.OnFloorCount))
			select new[]
			{
				shop.Id.ToString("N0", actor),
				shop.Name.TitleCase(),
				shop.Currency.Describe(cash, CurrencyDescriptionPatternType.ShortDecimal),
				shop.Currency.Describe(merchandise, CurrencyDescriptionPatternType.ShortDecimal),
				shop.Currency.Describe(
					transactions
						.Where(x => !x.TransactionType.In(ShopTransactionType.Float, ShopTransactionType.Withdrawal))
						.Sum(x => x.NetValue), CurrencyDescriptionPatternType.ShortDecimal),
				shop.Currency.Describe(transactions.Sum(x => x.Tax), CurrencyDescriptionPatternType.ShortDecimal),
				shop.Currency.Describe(cash + merchandise + (shop.BankAccount?.CurrentBalance ?? 0.0M),
					CurrencyDescriptionPatternType.ShortDecimal)
			},
			new[]
			{
				"ID",
				"Name",
				"Cash",
				"Inventory",
				"P/L CFP",
				"Tax CFP",
				"Value"
			},
			colour: Telnet.Green,
			maxwidth: actor.LineFormatLength,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void ShopList(ICharacter actor, StringStack command)
	{
		IEconomicZone zone = null;
		if (!command.IsFinished)
		{
			zone = long.TryParse(command.PopSpeech(), out var value)
				? actor.Gameworld.EconomicZones.Get(value)
				: actor.Gameworld.EconomicZones.GetByName(command.Last);
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such economic zone.");
				return;
			}
		}

		var shops = actor.Gameworld.Shops.ToList();
		if (zone != null)
		{
			shops = shops.Where(x => x.EconomicZone == zone).ToList();
		}

		actor.OutputHandler.Send(
			$"List of shops{(zone != null ? $" for economic zone {zone.Name.TitleCase().Colour(Telnet.Cyan)}" : "")}:");
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from shop in shops
			let pshop = shop as IPermanentShop
			select new[]
			{
				shop.Id.ToString("N0", actor),
				shop.Name.TitleCase(),
				shop.IsTrading.ToString(actor),
				shop.EmployeeRecords.Count().ToString("N0", actor),
				shop.EmployeesOnDuty.Count().ToString("N0", actor),
				pshop?.ShopfrontCells.Select(x =>
					x.GetFriendlyReference(actor).FluentTagMXP("send",
						$"href='goto {x.Id}'")).FirstOrDefault() ?? "",
				(shop is ITransientShop).ToColouredString()
			},
			new[]
			{
				"ID",
				"Name",
				"Open?",
				"Employs",
				"Working",
				"Storefront",
				"Transient?"
			},
			colour: Telnet.Green,
			maxwidth: actor.LineFormatLength,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void ShopDelete(ICharacter actor, StringStack command)
	{
		var shop = actor.Location.Shop;
		if (shop == null)
		{
			actor.OutputHandler.Send("You are not currently in a shop.");
			return;
		}

		if (!actor.IsAdministrator(PermissionLevel.SeniorAdmin))
		{
			actor.OutputHandler.Send("This action can only be performed by senior administrators.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to delete the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}? This is irreversible.\nType {"accept".ColourCommand()} to proceed or {"decline".ColourCommand()} to change your mind.");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You delete the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				shop.Delete();
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide against deleting the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide against deleting the shop {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			DescriptionString = "deleting a shop",
			Keywords = new List<string> { "delete", "shop" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void ShopInfo(ICharacter actor, StringStack command)
	{
		var shop = (IShop)actor.Location.Shop;
		if (!command.IsFinished && actor.IsAdministrator())
		{
			shop = long.TryParse(command.PopSpeech(), out var value)
				? actor.Gameworld.Shops.Get(value)
				: actor.Gameworld.Shops.GetByName(command.Last);
			if (shop == null)
			{
				actor.OutputHandler.Send("There is no such shop.");
				return;
			}
		}

		if (shop == null)
		{
			actor.OutputHandler.Send("You are not in a shop.");
			return;
		}

		shop.ShowInfo(actor);
	}

	private static void ShopLedger(ICharacter actor, StringStack command)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!actor.IsAdministrator() && !shop.IsManager(actor))
		{
			actor.OutputHandler.Send("You are not authorised to view the financial ledger of this store.");
			return;
		}

		var period = default(IFinancialPeriod);
		if (!command.IsFinished)
		{
			if (!int.TryParse(command.SafeRemainingArgument, out var value) || value > 0)
			{
				actor.OutputHandler.Send(
					"You must enter a valid number equal or less than 0. Zero represents the current financial period, -1 the previous financial period and so on.");
				return;
			}

			value *= -1;
			period = shop.EconomicZone.FinancialPeriods.OrderByDescending(x => x.FinancialPeriodStart)
						 .ElementAtOrDefault(value);
			if (period == null)
			{
				actor.OutputHandler.Send("There is no such financial period for that shop.");
				return;
			}
		}
		else
		{
			period = shop.EconomicZone.CurrentFinancialPeriod;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Transaction record for {shop.Name.ColourName()} for financial period {period.FinancialPeriodStartMUD.Date.Display(CalendarDisplayMode.Short).ColourName()} to {period.FinancialPeriodEndMUD.Date.Display(CalendarDisplayMode.Short).ColourName()}:");
		sb.AppendLine();
		var records = shop.TransactionRecords.Where(x => period.InPeriod(x.RealDateTime))
						  .OrderByDescending(x => x.RealDateTime).ToList();
		sb.AppendLine(
			$"Total Net for Period: {shop.Currency.Describe(records.Sum(x => x.NetValue), CurrencyDescriptionPatternType.ShortDecimal)}");
		sb.AppendLine(
			$"Total Tax Collected for Period: {shop.Currency.Describe(records.Sum(x => x.Tax), CurrencyDescriptionPatternType.ShortDecimal)}");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from transaction in records
			select new List<string>
			{
				transaction.MudDateTime.Date.Display(CalendarDisplayMode.Short),
				transaction.MudDateTime.Time.Display(TimeDisplayTypes.Immortal),
				transaction.TransactionType.DescribeEnum(),
				transaction.Currency.Describe(transaction.PretaxValue, CurrencyDescriptionPatternType.ShortDecimal),
				transaction.Currency.Describe(transaction.Tax, CurrencyDescriptionPatternType.ShortDecimal),
				transaction.Currency.Describe(transaction.NetValue, CurrencyDescriptionPatternType.ShortDecimal)
			},
			new List<string>
			{
				"Date",
				"Time",
				"Type",
				"Pretax",
				"Tax",
				"Net"
			},
			actor.LineFormatLength,
			colour: Telnet.BoldYellow,
			unicodeTable: actor.Account.UseUnicode
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ShopClockIn(ICharacter actor)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!actor.Location.Shop.IsEmployee(actor))
		{
			actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			return;
		}

		if (actor.Location.Shop.EmployeesOnDuty.Contains(actor))
		{
			actor.OutputHandler.Send("You are already clocked-in and available for duty.");
			return;
		}

		actor.Location.Shop.EmployeeClockIn(actor);
	}

	private static void ShopClockOut(ICharacter actor)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!actor.Location.Shop.IsEmployee(actor))
		{
			actor.OutputHandler.Send($"You are not an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			return;
		}

		if (!actor.Location.Shop.EmployeesOnDuty.Contains(actor))
		{
			actor.OutputHandler.Send("You are already clocked-out and off duty.");
			return;
		}

		actor.Location.Shop.EmployeeClockOut(actor);
	}

	private static void ShopEmploy(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send($"You are not a manager of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who is it that you're proposing to employ?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that.");
			return;
		}

		if (shop.IsEmployee(target))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is already an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			return;
		}

		actor.OutputHandler.Send(
			$"You propose to employ {target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} in {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
		target.OutputHandler.Send(
			$"{actor.HowSeen(target, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is proposing to make you an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.\nUse {"accept".ColourCommand()} to sign on, or {"decline".ColourCommand()} to say no.");
		target.AddEffect(new Accept(target, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (shop.IsEmployee(target))
				{
					target.OutputHandler.Send($"You are already an employee of this store.");
					return;
				}

				shop.AddEmployee(target);
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} is now an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				target.OutputHandler.Send($"You are now an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} declines to join the employ of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				target.OutputHandler.Send(
					$"You decline to become an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreSelf)} declines to join the employ of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				target.OutputHandler.Send(
					$"You decline to become an employee of {shop.Name.TitleCase().Colour(Telnet.Cyan)}");
			},
			Keywords = new List<string> { "shop", "employ" },
			DescriptionString = "Signing up as an employee of the shop"
		}), TimeSpan.FromSeconds(120));
	}

	private static void ShopFire(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not a manager of this shop.");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		IEmployeeRecord record = null;
		if (target != null)
		{
			record = actor.Location.Shop.EmployeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == target.Id);
		}

		record ??= actor.Location.Shop.EmployeeRecords.FirstOrDefault(x =>
			x.Name.GetName(NameStyle.FullName).EqualTo(ss.Last));

		if (record == null)
		{
			actor.OutputHandler.Send("You don't employ anyone like that.");
			return;
		}

		if (record.IsProprietor)
		{
			actor.OutputHandler.Send("You cannot fire someone who is a proprietor.");
			return;
		}

		if (record.IsManager && !shop.IsProprietor(actor))
		{
			actor.OutputHandler.Send("You can't fire other managers, only the proprietor can.");
			return;
		}


		actor.OutputHandler.Send(
			"Are you sure that you want to fire {} from {}?\nUse {} to confirm or {} to change your mind.");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (!shop.IsEmployee(target))
				{
					actor.OutputHandler.Send(
						$"{record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} is no longer an employee, so you can't fire them.");
					return;
				}

				shop.RemoveEmployee(record);
				actor.OutputHandler.Send(
					$"You fire {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				target = actor.Gameworld.Actors.Get(record.EmployeeCharacterId);
				if (target != null)
				{
					target.OutputHandler.Send($"You have been fired from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				}
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to fire {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to fire {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} from {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
			},
			Keywords = new List<string> { },
			DescriptionString = "firing an employee from the shop"
		}), TimeSpan.FromSeconds(120));
	}

	private static void ShopDisplay(ICharacter actor, StringStack ss)
	{
		if (actor.Location.Shop == null)
		{
			actor.OutputHandler.Send("You are not currently at a permanent shop.");
			return;
		}

		if (!actor.Location.Shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not a manager of this shop.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which item do you want to toggle as a display cabinet?");
			return;
		}

		var item = actor.TargetLocalItem(ss.PopSpeech());
		if (item == null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		var container = item.GetItemType<IContainer>();
		if (container == null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not suitable for use as a display cabinet.");
			return;
		}

		if (actor.Location.Shop.TillItems.Contains(item))
		{
			actor.OutputHandler.Send("An item cannot be marked as both a display cabinet and a till at the same time.");
			return;
		}

		if (actor.Location.Shop.DisplayContainers.Contains(item))
		{
			actor.Location.Shop.RemoveDisplayContainer(item);
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} will no longer be used as a display cabinet.");
			return;
		}

		actor.Location.Shop.AddDisplayContainer(item);
		actor.OutputHandler.Send($"{item.HowSeen(actor, true)} will now be used as a display cabinet.");
	}

	private static void ShopTill(ICharacter actor, StringStack ss)
	{
		if (actor.Location.Shop == null)
		{
			actor.OutputHandler.Send("You are not currently at a permanent shop.");
			return;
		}

		if (!actor.Location.Shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not a manager of this shop.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which item do you want to toggle as a till?");
			return;
		}

		var item = actor.TargetLocalItem(ss.PopSpeech());
		if (item == null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		// TODO - actual cash register type tills
		var container = item.GetItemType<IContainer>();
		if (container == null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not suitable for use as a till.");
			return;
		}

		if (actor.Location.Shop.DisplayContainers.Contains(item))
		{
			actor.OutputHandler.Send("An item cannot be marked as both a display cabinet and a till at the same time.");
			return;
		}

		if (actor.Location.Shop.TillItems.Contains(item))
		{
			actor.Location.Shop.RemoveTillItem(item);
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is no longer being used as a till for the shop.");
			return;
		}

		actor.Location.Shop.AddTillItem(item);
		actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is now being used as a till for the shop.");
	}

	private static void ShopManager(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}


		if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not the proprietor of this shop.");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		IEmployeeRecord record = null;
		if (target != null)
		{
			record = shop.EmployeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == target.Id);
		}

		record ??= shop.EmployeeRecords.FirstOrDefault(x =>
			x.Name.GetName(NameStyle.FullName).EqualTo(ss.Last));

		if (record == null)
		{
			actor.OutputHandler.Send("You don't employ anyone like that.");
			return;
		}

		if (record.IsProprietor)
		{
			actor.OutputHandler.Send("You cannot remove the manager status of someone who is a proprietor.");
			return;
		}

		record.IsManager = !record.IsManager;
		shop.Changed = true;
		actor.OutputHandler.Send(
			$"You {(record.IsManager ? "promote" : "demote")} {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} {(record.IsManager ? "to the position of manager" : "to merely an employee")}.");
		target = actor.Gameworld.Actors.Get(record.EmployeeCharacterId);
		if (target != null)
		{
			target.OutputHandler.Send(
				$"You have been {(record.IsManager ? "promoted to the position of manager of this shop" : "demoted to merely an employee of this shop")}.");
		}
	}

	private static void ShopProprietor(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsProprietor(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not the proprietor of this shop.");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		IEmployeeRecord record = null;
		if (target != null)
		{
			record = shop.EmployeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == target.Id);
		}

		record ??= shop.EmployeeRecords.FirstOrDefault(x => x.Name.GetName(NameStyle.FullName).EqualTo(ss.Last));

		if (record == null)
		{
			actor.OutputHandler.Send("You don't employ anyone like that.");
			return;
		}

		if (record.IsProprietor)
		{
			actor.OutputHandler.Send(
				$"{record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} is already a proprietor.");
			return;
		}

		actor.OutputHandler.Send(
			$"You are proposing to promote {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to the position of proprietor of {shop.Name.TitleCase().Colour(Telnet.Cyan)}. This decision is irreversable, and they will be a full owner with all rights unless they subsequently quit.\nTo go through with this decision, type {"accept".ColourCommand()} or type {"decline".ColourCommand()} to change your mind.");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				record.IsManager = true;
				record.IsProprietor = true;
				shop.Changed = true;
				actor.OutputHandler.Send(
					$"You promote {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to proprietor of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				target = actor.Gameworld.Actors.Get(record.EmployeeCharacterId);
				if (target != null)
				{
					target.OutputHandler.Send(
						$"You have been promoted to the proprietor of {shop.Name.TitleCase().Colour(Telnet.Cyan)}.");
				}
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide against promoting {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to proprietor.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide against promoting {record.Name.GetName(NameStyle.FullName).Colour(Telnet.Cyan)} to proprietor.");
			},
			Keywords = new List<string> { "proprietor", "shop", "promote" },
			DescriptionString = "Promote someone to proprietor of a shop"
		}), TimeSpan.FromSeconds(120));
	}

	private static void ShopStock(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsEmployee(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not an employee of this shop.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which item do you want to put into stock for sale?");
			return;
		}

		var item = actor.TargetLocalOrHeldItem(ss.PopSpeech());
		if (item == null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		IMerchandise merch = null;
		if (!ss.IsFinished)
		{
			merch = shop.Merchandises
						.Where(x => x.IsMerchandiseFor(item))
						.GetFromItemListByKeyword(ss.PopSpeech(), actor);
			if (merch == null)
			{
				actor.OutputHandler.Send(
					$"There is no matching merchandise profile for {item.HowSeen(actor)} with the specified keywords. A manager or proprietor must first create a merchandise profile before it can be brought into stock.");
				return;
			}
		}
		else
		{
			merch = shop.Merchandises.FirstOrDefault(x => x.IsMerchandiseFor(item));
			if (merch == null)
			{
				actor.OutputHandler.Send(
					$"There is no merchandise profile for items like {item.HowSeen(actor)}. A manager or proprietor must first create a merchandise profile before it can be brought into stock.");
				return;
			}
		}

		if (item.AffectedBy<ItemOnDisplayInShop>())
		{
			actor.OutputHandler.Send(
				$"{item.HowSeen(actor, true)} is already in stock. Use the dispose command if you want to take it out of inventory.");
			return;
		}

		shop.AddToStock(actor, item, merch);
	}

	private static void ShopDispose(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsEmployee(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not an employee of this shop.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which item do you want to put dispose of from the for-sale inventory of the shop?");
			return;
		}

		var item = actor.TargetLocalOrHeldItem(ss.PopSpeech());
		if (item == null)
		{
			actor.OutputHandler.Send("You don't see anything like that.");
			return;
		}

		if (!item.AffectedBy<ItemOnDisplayInShop>())
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not in stock.");
			return;
		}

		var merch = shop.Merchandises.FirstOrDefault(x => x.Item.Id == item.Prototype.Id);
		if (merch == null)
		{
			actor.OutputHandler.Send(
				$"There is no merchandise profile for items like {item.HowSeen(actor)}. A manager or proprietor must first create a merchandise profile before it can be disposed of.");
			return;
		}

		shop.DisposeFromStock(actor, item);
	}

	private static void ShopMerchandise(ICharacter actor, StringStack ss)
	{
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ShopMerchandiseList(actor, ss);
				return;
			case "show":
				ShopMerchandiseShow(actor, ss);
				return;
			case "edit":
				ShopMerchandiseEdit(actor, ss);
				return;
			case "new":
				ShopMerchandiseNew(actor, ss);
				return;
			case "clone":
				ShopMerchandiseClone(actor, ss);
				return;
			case "delete":
				ShopMerchandiseDelete(actor, ss);
				return;
			case "set":
				ShopMerchandiseSet(actor, ss);
				return;
			default:
				ShopMerchandiseHelp(actor);
				return;
		}
	}

	private const string ShopMerchandiseHelpText = @"You can use the following subcommands for working with merchandise:
	#1Note: All SHOP MERCHANDISE commands must be done from a room within the shop.#0

	#3shop merch list#0 - lists all merchandise records
	#3shop merch edit <record>#0 - opens a merchandise record for editing
	#3shop merch edit#0 - equivalent to SHOW <edited record>
	#3shop merch show <record>#0 - shows information about the specified merchandise record
	#3shop merch new <name> <id>|<target> <price>|default [<custom description>]#0 - creates a new record with the specified item and price, and optional custom LIST description
	#3shop merch clone <new name>#0 - clones the currently edited record to an identical new record
	#3shop merch delete#0 - deletes the current merchandise record
	#3shop merch set name <name>#0 - sets the name of a merchandise
	#3shop merch set proto <target>#0 - sets the item type associated with a merchandise
	#3shop merch set default#0 - toggles whether this is the default merch record for similar items
	#3shop merch set price <price>#0 - sets the pre-tax price
	#3shop merch set desc clear#0 - clears a custom list description
	#3shop merch set desc <description>#0 - sets a custom list description
	#3shop merch set container clear#0 - clears a preferred display container
	#3shop merch set container <target>#0 - sets a preferred display container";

	private const string ShopMerchandiseAdminHelpText =
		@"You can use the following subcommands for working with merchandise:
	#1Note: All SHOP MERCHANDISE commands must be done from a room within the shop.#0

	#3shop merch list#0 - lists all merchandise records
	#3shop merch edit <record>#0 - opens a merchandise record for editing
	#3shop merch edit#0 - equivalent to SHOW <edited record>
	#3shop merch show <record>#0 - shows information about the specified merchandise record
	#3shop merch new <name> <id>|<target> <price>|default [<custom description>]#0 - creates a new record with the specified item and price, and optional custom LIST description
	#3shop merch clone <new name>#0 - clones the currently edited record to an identical new record
	#3shop merch delete#0 - deletes the current merchandise record
	#3shop merch set name <name>#0 - sets the name of a merchandise
	#3shop merch set proto <target>#0 - sets the item type associated with a merchandise
	#3shop merch set default#0 - toggles whether this is the default merch record for similar items
	#3shop merch set price <price>#0 - sets the pre-tax price
	#3shop merch set desc clear#0 - clears a custom list description
	#3shop merch set desc <description>#0 - sets a custom list description
	#3shop merch set container clear#0 - clears a preferred display container
	#3shop merch set container <target>#0 - sets a preferred display container
	#3shop merch set reorder off#0 - turns auto-reordering off
	#3shop merch set reorder <price> <quantity> [<weight>]#0 - enables auto-reordering with the specified price, quantity and optional minimum weight
	#3shop merch set preserve#0 - toggles preservation of item variables when reordering";

	private static void ShopMerchandiseHelp(ICharacter actor)
	{
		actor.OutputHandler.Send(
			actor.IsAdministrator()
				? ShopMerchandiseAdminHelpText.SubstituteANSIColour()
				: ShopMerchandiseHelpText.SubstituteANSIColour()
		);
	}

	private static void ShopMerchandiseSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
						   .FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not currently editing any merchandise entries.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ShopMerchandiseDelete(ICharacter actor, StringStack ss)
	{
		var editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
						   .FirstOrDefault(x => x.EditingItem.Shop == actor.Location.Shop);
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not currently editing any merchandise entries.");
			return;
		}

		var record = editing.EditingItem;
		actor.OutputHandler.Send(
			$"Are you sure that you want to delete the merchandise record '{editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}'?\nUse {"accept".ColourCommand()} to proceed or {"decline".ColourCommand()} to change your mind.");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send(
					$"You delete merchandise record {editing.EditingItem.ListDescription.ColourObject()} ({editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}).");
				record.Shop.RemoveMerchandise(record);
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide against deleting merchandise record {editing.EditingItem.ListDescription.ColourObject()} ({editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}).");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide against deleting merchandise record {editing.EditingItem.ListDescription.ColourObject()} ({editing.EditingItem.Name.TitleCase().Colour(Telnet.Cyan)}).");
			},
			DescriptionString = "Deleting a merchandise record",
			Keywords = new List<string> { "merchandise", "delete" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void ShopMerchandiseClone(ICharacter actor, StringStack ss)
	{
		var editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
						   .FirstOrDefault(x => x.EditingItem.Shop == actor.Location.Shop);
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not currently editing any merchandise entries.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must supply a name for your cloned merchandise record.");
			return;
		}

		var name = ss.PopSpeech();

		var newMerch = new Merchandise((Merchandise)editing.EditingItem, name);
		editing.EditingItem.Shop.AddMerchandise(newMerch);
		actor.OutputHandler.Send(
			$"You clone merchandise record for {editing.EditingItem.ListDescription.ColourObject()} into a new record called {name.TitleCase().Colour(Telnet.Cyan)}, which you are now editing.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IMerchandise>>());
		actor.AddEffect(new BuilderEditingEffect<IMerchandise>(actor) { EditingItem = newMerch });
	}

	private static void ShopMerchandiseNew(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}


		if (!shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only managers or proprietors of the shop can create new merchandise records.");
			return;
		}

		void HelpText()
		{
			if (actor.IsAdministrator())
			{
				actor.OutputHandler.Send(
					"The syntax for this command is SHOP MERCHANDISE NEW <name> <id>|<target> <price>|default [<custom description>]");
				return;
			}

			actor.OutputHandler.Send(
				"The syntax for this command is SHOP MERCHANDISE NEW <name> <target> <price>|default [<custom description>]");
		}

		if (ss.IsFinished)
		{
			HelpText();
			return;
		}

		var name = ss.PopSpeech();

		if (ss.IsFinished)
		{
			HelpText();
			return;
		}

		var text = ss.PopSpeech();
		IGameItemProto proto;
		if (actor.IsAdministrator() && long.TryParse(text, out var value))
		{
			proto = actor.Gameworld.ItemProtos.Get(value);
			if (proto == null)
			{
				actor.OutputHandler.Send("There is no such item prototype.");
				return;
			}
		}
		else
		{
			var target = actor.TargetLocalOrHeldItem(text);
			if (target == null)
			{
				actor.OutputHandler.Send("You don't see anything like that.");
				return;
			}

			proto = target.Prototype;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What pre-tax price do you want to give to this merchandise entry?");
			return;
		}

		decimal price;
		if (ss.PeekSpeech().EqualTo("default"))
		{
			price = -1.0M;
			ss.PopSpeech();
		}
		else
		{
			price = shop.Currency.GetBaseCurrency(ss.PopSpeech(), out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid price.");
				return;
			}
		}
		
		var newMerch = new Merchandise(shop, name, proto, price, shop.Merchandises.All(x => x.Item.Id != proto.Id),
			null, ss.SafeRemainingArgument);
		shop.AddMerchandise(newMerch);
		actor.OutputHandler.Send(
			$"You create a new merchandise entry for {newMerch.ListDescription.ColourObject()} ({newMerch.Name.TitleCase().Colour(Telnet.Cyan)}), which you are now editing.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IMerchandise>>());
		actor.AddEffect(new BuilderEditingEffect<IMerchandise>(actor) { EditingItem = newMerch });
		return;
	}

	private static void ShopMerchandiseEdit(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}


		if (!shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only managers or proprietors of the shop can edit merchandise records.");
			return;
		}

		if (ss.IsFinished)
		{
			var editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
							   .FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which merchandise record would you like to edit?");
				return;
			}

			editing.EditingItem.ShowToBuilder(actor);
			return;
		}

		var text = ss.PopSpeech();
		var merch = shop.Merchandises.GetFromItemListByKeywordIncludingNames(text, actor);
		if (merch == null)
		{
			actor.OutputHandler.Send("There is no merchandise record like that for you to edit.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IMerchandise>>());
		actor.AddEffect(new BuilderEditingEffect<IMerchandise>(actor) { EditingItem = merch });
		actor.OutputHandler.Send(
			$"You are now editing the merchandise record {merch.Name.TitleCase().Colour(Telnet.Cyan)}.");
	}

	private static void ShopMerchandiseShow(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only managers or proprietors of the shop can view merchandise records.");
			return;
		}

		if (ss.IsFinished)
		{
			var editing = actor.EffectsOfType<BuilderEditingEffect<IMerchandise>>()
							   .FirstOrDefault(x => x.EditingItem.Shop == shop);
			if (editing == null)
			{
				actor.OutputHandler.Send("Which merchandise record would you like to view?");
				return;
			}

			editing.EditingItem.ShowToBuilder(actor);
			return;
		}

		var text = ss.PopSpeech();
		var merch = shop.Merchandises.GetFromItemListByKeywordIncludingNames(text, actor);
		if (merch == null)
		{
			actor.OutputHandler.Send("There is no merchandise record like that for you to view.");
			return;
		}

		merch.ShowToBuilder(actor);
	}

	private static void ShopMerchandiseList(ICharacter actor, StringStack ss)
	{
		if (!DoShopCommandFindShop(actor, out var shop))
		{
			return;
		}

		if (!shop.IsManager(actor) && !actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only managers or proprietors of the shop can view merchandise records.");
			return;
		}

		var stockTake = shop.StocktakeAllMerchandise();
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from merch in shop.Merchandises
			orderby merch.Name
			select new[]
			{
				merch.Name,
				merch.ListDescription,
				shop.Currency.Describe(merch.EffectivePrice, CurrencyDescriptionPatternType.Short),
				stockTake[merch].OnFloorCount.ToString("N0", actor),
				stockTake[merch].InStockroomCount.ToString("N0", actor)
			},
			new[] { "Name", "List Description", "Price", "On Display", "In Store" },
			actor.LineFormatLength,
			truncatableColumnIndex: 1,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	#endregion

	#region Banks

	public const string BankHelpText =
		@"The bank command is used to interact with bank accounts. All of the commands need to be done at a bank branch.

The syntax for using banks is as follows:

	#3bank accounts#0 - shows all the bank accounts you have access to
	#3bank open <type>#0 - opens a new bank account
	#3bank openclan <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank openshop <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank types#0 - shows what types of bank accounts this bank offers
	#3bank alias <account#>#0 - sets the alias of a bank account
	#3bank preview <type>#0 - previews the fees/interest of a bank account type
	#3bank close <account#>#0 - permanently closes a bank account
	#3bank show <account#>#0 - shows information about an account
	#3bank transactions <account#>#0 - shows transaction history for a bank account
	#3bank deposit <account#> <amount>#0 - deposits money into an account
	#3bank withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank transfer <fromaccount#>#0 <toaccount#> <amount>#0 - transfers money to another account
	#3bank transfer <fromaccount#>#0 <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank
	#3bank requestitem <account#>#0 - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank cancelitems <account#>#0 - requests that the bank cancel all issued items, e.g. if lost or stolen

Additionally, if you are the manager of a bank, you can use the following additional commands:

	#3bank manager balance#0 - shows information about the banks funds and liabilities
	#3bank manager audit [<who>]#0 - shows audit logs (optionally for a specific person)
	#3bank manager status <account> <active|suspended|locked>#0 - changes the status of a bank account
	#3bank manager close <account>#0 - permanently closes an account (can get around restrictions)
	#3bank manager credit <account> <amount> <comment>#0 - credits an existing account
	#3bank manager accounts#0 - view a list of accounts
	#3bank manager account <accn>#0 - view info about an account
	#3bank manager rollover <account> <newaccount>#0 - closes an account and rolls balance into a new one
	#3bank manager withdraw <amount>#0 - withdraws money from the cash reserves
	#3bank manager deposit <amount>#0 - deposits money into the cash reserves
	#3bank manager exchange <from> <to> <rate>#0 - sets the currency exchange rate";

	public const string BankAdminHelpText =
		@"The bank command is used to create and edit banks. The commands are as follows:

	#3bank list#0 - lists all of the banks
	#3bank show <which>#0 - shows information about a bank
	#3bank edit new <name> <code> <economiczone>#0 - creates a new bank
	#3bank clone <which> <name> <code>#0 - clones a bank into a new bank
	#3bank edit <which>#0 - begins to edit a bank
	#3bank edit#0 - alias for BANK SHOW <currently editing bank>
	#3bank close#0 - stops editing a bank
	#3bank set ...#0 - edits the properties of a bank. See bank set ? for more info.

The player version of the command is explained below. All of the commands need to be done at a bank branch.
Note: There are two different ways to access the player commands, which is necessary to avoid clashes with the admin versions. You may choose either version

Syntax Option 1:

	#3bank accounts#0 - shows all the bank accounts you have access to
	#3bank open <type>#0 - opens a new bank account
	#3bank openclan <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank openshop <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank types#0 - shows what types of bank accounts this bank offers
	#3bank alias <account#>#0 - sets the alias of a bank account
	#3bank closeaccount <account#>#0 - permanently closes a bank account
	#3bank showaccount <account#>#0 - shows information about an account
	#3bank transactions <account#>#0 - shows transaction history for a bank account
	#3bank deposit <account#> <amount>#0 - deposits money into an account
	#3bank withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank transfer <fromaccount#> <toaccount#> <amount>#0 - transfers money to another account
	#3bank transfer <fromaccount#> <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank
	#3bank requestitem <account#>#0 - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank cancelitems <account#>#0 - requests that the bank cancel all issued items, e.g. if lost or stolen

Syntax Option 2:

	#3bank accounts#0 - shows all the bank accounts you have access to
	#3bank account open <type>#0 - opens a new bank account
	#3bank account clanopen <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank account shopopen <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank account types#0 - shows what types of bank accounts this bank offers
	#3bank account alias <account#>#0 - sets the alias of a bank account
	#3bank account close <account#>#0 - permanently closes a bank account
	#3bank account show <account#>#0 - shows information about an account
	#3bank account transactions <account#>#0 - shows transaction history for a bank account
	#3bank account deposit <account#> <amount>#0 - deposits money into an account
	#3bank account withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank account transfer <fromaccount#> <toaccount#> <amount>#0 - transfers money to another account
	#3bank account transfer <fromaccount#> <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank	
	#3bank account requestitem <account#>#0 - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank account cancelitems <account#>#0 - requests that the bank cancel all issued items, e.g. if lost or stolen

Additionally, if you are the manager of a bank, you can use the following additional commands:

	#3bank manager balance#0 - shows information about the banks funds and liabilities
	#3bank manager audit [<who>]#0 - shows audit logs (optionally for a specific person)
	#3bank manager status <account> <active|suspended|locked>#0 - changes the status of a bank account
	#3bank manager close <account>#0 - permanently closes an account (can get around restrictions)
	#3bank manager accounts#0 - view a list of accounts
	#3bank manager account <accn>#0 - view info about an account
	#3bank manager credit <account> <amount> <comment>#0 - credits an existing account
	#3bank manager rollover <account> <newaccount>#0 - closes an account and rolls balance into a new one
	#3bank manager withdraw <amount>#0 - withdraws money from the cash reserves
	#3bank manager deposit <amount>#0 - deposits money into the cash reserves
	#3bank manager exchange <from> <to> <rate>#0 - sets the currency exchange rate";

	[PlayerCommand("Bank", "bank")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[HelpInfo("bank", BankHelpText, AutoHelp.HelpArgOrNoArg, BankAdminHelpText)]
	protected static void Bank(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "edit":
			case "set":
			case "view":
			case "clone":
			case "list":
				BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.BankHelper);
				return;
			case "close":
				if (!actor.IsAdministrator())
				{
					goto case "closeaccount";
				}

				BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.BankHelper);
				return;
			case "show":
				if (!actor.IsAdministrator())
				{
					goto case "showaccount";
				}

				BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.BankHelper);
				return;
			case "account":
				BuildingCommandBankAccount(actor, ss);
				return;
			case "accounts":
				BuildingCommandBankAccounts(actor, ss);
				return;
			case "manager":
				BankManager(actor, ss);
				return;
			case "deposit":
				BuildingCommandBankAccount(actor,
					new StringStack("deposit" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "withdraw":
				BuildingCommandBankAccount(actor,
					new StringStack("withdraw" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "transactions":
			case "history":
				BuildingCommandBankAccount(actor,
					new StringStack("transactions" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "transfer":
			case "trans":
			case "xfer":
				BuildingCommandBankAccount(actor,
					new StringStack("transfer" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "alias":
				BuildingCommandBankAccount(actor,
					new StringStack("alias" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "closeaccount":
				BuildingCommandBankAccount(actor,
					new StringStack("close" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "showaccount":
				BuildingCommandBankAccount(actor,
					new StringStack("show" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "types":
				BuildingCommandBankAccount(actor,
					new StringStack("types" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "preview":
				BuildingCommandBankAccount(actor,
					new StringStack("preview" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "open":
			case "openaccount":
				BuildingCommandBankAccount(actor,
					new StringStack("open" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "openshop":
			case "openshopaccount":
				BuildingCommandBankAccount(actor,
					new StringStack("shopopen" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "openclan":
			case "openclanaccount":
				BuildingCommandBankAccount(actor,
					new StringStack("clanopen" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "requestitem":
				BuildingCommandBankAccount(actor,
					new StringStack("requestitem" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			case "cancelitems":
				BuildingCommandBankAccount(actor,
					new StringStack("cancelitems" + ss.RemainingArgument.LeadingSpaceIfNotEmpty()));
				return;
			default:
				actor.OutputHandler.Send((actor.IsAdministrator() ? BankAdminHelpText : BankHelpText)
					.SubstituteANSIColour());
				return;
		}
	}

	private static void BankManager(ICharacter actor, StringStack ss)
	{
		var bank = actor.Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(actor.Location));
		if (bank == null)
		{
			actor.OutputHandler.Send("You are not currently at a bank.");
			return;
		}

		if (!bank.IsManager(actor))
		{
			actor.OutputHandler.Send($"You are not a manager of {bank.Name.ColourName()}.");
			return;
		}

		bank.ManagerCommand(actor, ss);
	}

	private static void BuildingCommandBankAccounts(ICharacter actor, StringStack ss)
	{
		var bank = actor.Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(actor.Location));
		if (bank == null)
		{
			actor.OutputHandler.Send("You are not currently at a bank.");
			return;
		}

		var accounts = bank.BankAccounts.Where(x => x.IsAuthorisedAccountUser(actor)).ToList();
		if (!accounts.Any())
		{
			actor.OutputHandler.Send($"You don't have any bank accounts with {bank.Name.ColourName()}.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"You have access to the following bank accounts with {bank.Name.ColourName()}:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from account in accounts
			select new List<string>
			{
				$"{account.Bank.Code}:{account.AccountNumber.ToString("F0",actor)}",
				account.BankAccountType.Name,
				account.Name,
				account.AccountStatus.DescribeEnum(),
				bank.EconomicZone.Currency.Describe(account.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue(),
				account.IsAccountOwner(actor) ? "You" : account.AccountOwner switch
				{
					ICharacter ch => ch.PersonalName.GetName(NameStyle.FullName),
					IClan clan => clan.FullName,
					IShop shop => shop.Name,
					_ => "Unknown"
				}
			},
			new List<string>
			{
				"Account",
				"Account Type",
				"Alias",
				"Status",
				"Balance",
				"Owner"
			},
			actor,
			Telnet.Yellow
		));

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void BuildingCommandBankAccount(ICharacter actor, StringStack ss)
	{
		var bank = actor.Gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(actor.Location));
		if (bank == null)
		{
			actor.OutputHandler.Send("You are not currently at a bank.");
			return;
		}

		var text = ss.PopForSwitch();
		if (text.EqualToAny("open", "new", "create"))
		{
			BankAccountCreate(actor, ss, bank);
			return;
		}

		if (text.EqualTo("clanopen"))
		{
			BankAccountCreateClan(actor, ss, bank);
			return;
		}

		if (text.EqualTo("shopopen"))
		{
			BankAccountCreateShop(actor, ss, bank);
			return;
		}

		if (text.EqualTo("types"))
		{
			BankAccountTypes(actor, bank);
			return;
		}

		if (text.EqualTo("preview"))
		{
			BankAccountPreview(actor, ss, bank);
			return;
		}

		switch (text)
		{
			case "deposit":
			case "withdraw":
			case "transfer":
			case "close":
			case "show":
			case "transactions":
			case "requestitem":
			case "cancelitems":
			case "alias":
				break;
			default:
				actor.OutputHandler.Send(@"The valid options for this sub-command are as follows:

	#3bank account open <type>#0 - opens a new bank account
	#3bank account clanopen <type> <clan>#0 - opens a new bank account on behalf of a clan
	#3bank account shopopen <type> <shop>#0 - opens a new bank account on behalf of a shop
	#3bank account types#0 - shows what types of bank accounts this bank offers
	#3bank account alias <account#>#0 - sets the alias of a bank account
	#3bank account close <account#>#0 - permanently closes a bank account
	#3bank account show <account#>#0 - shows information about an account
	#3bank account transactions <account#>#0 - shows transaction history for an account
	#3bank account deposit <account#> <amount>#0 - deposits money into an account
	#3bank account preview <type>#0 - previews an account type
	#3bank account withdraw <account#> <amount>#0 - withdraws money from an account
	#3bank account transfer <fromaccount#> <toaccount#> <amount>#0 - transfers money to another account
	#3bank account transfer <fromaccount#> <bank>:<toaccount#> <amount>#0 - transfers money to an account at another bank
	#3bank account requestitem <account#> - requests that the bank issue you a payment item (e.g. chequebook, key card, etc)
	#3bank account cancelitems <account#> - requests that the bank cancel all issued items, e.g. if lost or stolen"
					.SubstituteANSIColour());
				return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must supply an account number for that action.");
			return;
		}

		_ = int.TryParse(ss.PopSpeech(), out var accn);

		var account =
			bank.BankAccounts.FirstOrDefault(x => ((accn > 0 && x.AccountNumber == accn) || x.Name.StartsWith(ss.Last, StringComparison.InvariantCultureIgnoreCase)) && x.IsAuthorisedAccountUser(actor));
		if (account == null)
		{
			actor.OutputHandler.Send("You don't have access to any bank account with that account number or alias.");
			return;
		}

		switch (text)
		{
			case "show":
				BankAccountShow(actor, ss, account, bank);
				return;
			case "transactions":
				BankAccountTransactions(actor, ss, account, bank);
				return;
		}

		switch (account.AccountStatus)
		{
			case BankAccountStatus.Locked:
				actor.OutputHandler.Send("That account has been locked and currently cannot be used.");
				return;
			case BankAccountStatus.Suspended:
				actor.OutputHandler.Send("That account has been suspended by the bank and cannot be used.");
				return;
			case BankAccountStatus.Closed:
				actor.OutputHandler.Send("That account has been closed, and cannot be used.");
				return;
		}

		switch (text)
		{
			case "deposit":
				BankAccountDeposit(actor, ss, account, bank);
				return;
			case "withdraw":
				BankAccountWithdraw(actor, ss, account, bank);
				return;
			case "transfer":
				BankAccountTransfer(actor, ss, account, bank);
				return;
			case "close":
				BankAccountClose(actor, ss, account, bank);
				return;
			case "requestitem":
				BankAccountRequestItem(actor, ss, account, bank);
				return;
			case "cancelitems":
				BankAccountCancelItems(actor, ss, account, bank);
				return;
			case "alias":
				BankAccountAlias(actor, ss, account, bank);
				return;
		}
	}

	private static void BankAccountAlias(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What alias do you want to give to that account?");
			return;
		}

		account.SetName(ss.SafeRemainingArgument);
		actor.OutputHandler.Send($"Bank account {account.AccountReference.ColourName()} now has the alias {account.Name.ColourCommand()} for use with bank commands.");
	}

	private static void BankAccountPreview(ICharacter actor, StringStack ss, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of bank account would you like to preview?\nYou can see the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		var typeText = ss.SafeRemainingArgument;
		var type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(typeText)) ??
				   bank.BankAccountTypes.FirstOrDefault(x =>
					   x.Name.StartsWith(typeText, StringComparison.InvariantCultureIgnoreCase));
		if (type == null)
		{
			actor.OutputHandler.Send(
				$"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		actor.OutputHandler.Send(type.ShowToCustomer(actor));
	}

	private static void BankAccountCreateShop(ICharacter actor, StringStack ss, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of bank account would you like to open?\nYou can see the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		var typeText = ss.PopSpeech();
		var type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(typeText)) ??
				   bank.BankAccountTypes.FirstOrDefault(x =>
					   x.Name.StartsWith(typeText, StringComparison.InvariantCultureIgnoreCase));
		if (type == null)
		{
			actor.OutputHandler.Send(
				$"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What shop are you trying to open a bank account on behalf of?");
			return;
		}

		var shopText = ss.SafeRemainingArgument;
		var shop = actor.Gameworld.Shops
						.FirstOrDefault(x => x.Name.StartsWith(shopText, StringComparison.InvariantCultureIgnoreCase));
		if (shop == null)
		{
			actor.OutputHandler.Send("There is no shop by that name.");
			return;
		}

		if (!shop.IsManager(actor))
		{
			actor.OutputHandler.Send($"You are not authorised to act on behalf of {shop.Name.ColourName()}.");
			return;
		}

		var (success, error) = type.CanOpenAccount(shop);
		if (!success)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var account = type.OpenAccount(shop);
		bank.AddAccount(account);
		actor.OutputHandler.Send(
			$"You open a new {type.Name.ColourName()} account for {shop.Name.ColourName()} with {bank.Name.ColourName()}. The account number is {account.AccountNumber.ToString("N0", actor).ColourName()}.");
	}

	private static void BankAccountCreateClan(ICharacter actor, StringStack ss, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of bank account would you like to open?\nYou can see the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		var typeText = ss.PopSpeech();
		var type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(typeText)) ??
				   bank.BankAccountTypes.FirstOrDefault(x =>
					   x.Name.StartsWith(typeText, StringComparison.InvariantCultureIgnoreCase));
		if (type == null)
		{
			actor.OutputHandler.Send(
				$"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What clan are you trying to open a bank account on behalf of?");
			return;
		}

		var clanText = ss.SafeRemainingArgument;
		var clan = actor.Gameworld.Clans.GetByNameOrAbbreviation(clanText);
		if (clan == null)
		{
			actor.OutputHandler.Send("There is no clan by that name.");
			return;
		}

		var membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		if (!actor.IsAdministrator() && (membership == null ||
										 !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateBudgets)))
		{
			actor.OutputHandler.Send($"You are not authorised to act on behalf of {clan.Name.ColourName()}.");
			return;
		}

		var (success, error) = type.CanOpenAccount(clan);
		if (!success)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var account = type.OpenAccount(clan);
		bank.AddAccount(account);
		actor.OutputHandler.Send(
			$"You open a new {type.Name.ColourName()} account for {clan.Name.ColourName()} with {bank.Name.ColourName()}. The account number is {account.AccountNumber.ToString("N0", actor).ColourName()}.");
	}

	private static void BankAccountRequestItem(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		if (account.BankAccountType.NumberOfPermittedPaymentItems == 0 ||
			actor.Gameworld.ItemProtos.Get(account.BankAccountType.PaymentItemPrototype ?? 0)?.Status !=
			RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"Your bank account does not permit the issue of payment items.");
			return;
		}

		if (account.NumberOfIssuedPaymentItems >= account.BankAccountType.NumberOfPermittedPaymentItems)
		{
			actor.OutputHandler.Send(
				"You have already had the maximum number of payment items issued to you. You must cancel the existing ones before you can have any more.");
			return;
		}

		var item = account.CreateNewPaymentItem();
		if (item is null)
		{
			actor.OutputHandler.Send($"Your bank account does not permit the issue of payment items.");
			return;
		}

		actor.Gameworld.Add(item);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is issued $1 by the bank.", actor, actor, item)));
		if (actor.Body.CanGet(item, 0))
		{
			actor.Body.Get(item, silent: true);
		}
		else
		{
			item.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(item, true);
			item.SetPosition(PositionUndefined.Instance, PositionModifier.None, actor, null);
			actor.OutputHandler.Send(
				$"Your {actor.Body.Prototype.WielderDescriptionPlural.ToLowerInvariant()} were full so you set the item on the ground.");
		}
	}

	private static void BankAccountCancelItems(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		if (account.NumberOfIssuedPaymentItems <= 0)
		{
			actor.OutputHandler.Send(
				"You don't currently have any issued payment items to cancel with that account.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure that you want to cancel all your existing issued payment items? They will no longer be able to be used to pay for things.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				account.CancelPaymentItems();
				actor.OutputHandler.Send(
					$"You cancel all your existing payment items for account {account.AccountReference.ColourName()}.");
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to cancel your payment items."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to cancel your payment items."); },
			Keywords = new List<string> { "cancel", "payment" },
			DescriptionString = "cancelling your payment items from your bank account"
		}), TimeSpan.FromSeconds(120));
	}

	private static void BankAccountDeposit(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much money do you want to deposit?");
			return;
		}

		if (actor.Currency != bank.PrimaryCurrency)
		{
			actor.OutputHandler.Send(
				$"This bank conducts transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency. You must {$"set currency {bank.PrimaryCurrency.Name}".MXPSend()} before using its services.");
			return;
		}

		if (!bank.PrimaryCurrency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send("That is not a valid amount of currency.");
			return;
		}

		var targetCoins = bank.PrimaryCurrency.FindCurrency(
			actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			actor.OutputHandler.Send("You aren't holding any currency of that type.");
			return;
		}

		var coinValue = targetCoins.TotalValue();
		if (coinValue < amount)
		{
			actor.OutputHandler.Send(
				$"You aren't holding enough money to make a deposit of that size.\nThe largest deposit that you could make is {bank.PrimaryCurrency.Describe(coinValue, CurrencyDescriptionPatternType.Short).ColourValue()}.");
			return;
		}

		var change = 0.0M;
		if (coinValue > amount)
		{
			change = coinValue - amount;
		}

		foreach (var item in targetCoins)
		{
			if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				actor.Body.Take(item.Key.Parent);
				item.Key.Parent.Delete();
			}
		}

		account.Deposit(amount);
		bank.CurrencyReserves[bank.PrimaryCurrency] += amount;
		bank.Changed = true;
		var moneyDescription = bank.PrimaryCurrency.Describe(amount, CurrencyDescriptionPatternType.Short)
								   .ColourValue();
		actor.OutputHandler.Send(
			$"You deposit {moneyDescription} into account {account.NameWithAlias}.");
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ deposits {moneyDescription} into a bank account.", actor, actor),
			flags: OutputFlags.SuppressSource));

		if (change > 0.0M)
		{
			var changeItem =
				CurrencyGameItemComponentProto.CreateNewCurrencyPile(bank.PrimaryCurrency,
					bank.PrimaryCurrency.FindCoinsForAmount(change, out _));
			if (actor.Body.CanGet(changeItem, 0))
			{
				actor.Body.Get(changeItem, silent: true);
			}
			else
			{
				actor.Location.Insert(changeItem, true);
				actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
			}
		}
	}

	private static void BankAccountTransfer(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What account do you want to transfer money into?");
			return;
		}

		IBank bankTarget = null;
		IBankAccount accountTarget = null;
		var accTarget = ss.PopSpeech();

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much money do you want to transfer?");
			return;
		}

		if (actor.Currency != bank.PrimaryCurrency)
		{
			actor.OutputHandler.Send(
				$"This bank conducts transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency. You must {$"set currency {bank.PrimaryCurrency.Name}".MXPSend()} before using its services.");
			return;
		}

		if (!bank.PrimaryCurrency.TryGetBaseCurrency(ss.PopSpeech(), out var amount))
		{
			actor.OutputHandler.Send("That is not a valid amount of currency.");
			return;
		}

		var (target, accountError) = Economy.Banking.Bank.FindBankAccount(accTarget, bank, actor);
		if (target == null)
		{
			actor.OutputHandler.Send(accountError);
			return;
		}

		accountTarget = target;
		bankTarget = accountTarget.Bank;

		if (accountTarget == account)
		{
			actor.OutputHandler.Send("You can't transfer money from an account to itself.");
			return;
		}

		var (success, error) = account.CanWithdraw(amount, false);
		if (!success)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var targetAmount = amount;
		if (bankTarget.PrimaryCurrency != bank.PrimaryCurrency)
		{
			if (bankTarget.ExchangeRates[(bank.PrimaryCurrency, bankTarget.PrimaryCurrency)] == 0.0M)
			{
				actor.OutputHandler.Send(
					$"{bankTarget.Name.ColourName()} does not accept transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency.");
				return;
			}

			targetAmount *= bankTarget.ExchangeRates[(bank.PrimaryCurrency, bankTarget.PrimaryCurrency)];
		}

		account.WithdrawFromTransfer(amount, bankTarget.Code, accountTarget.AccountNumber, ss.SafeRemainingArgument);
		accountTarget.DepositFromTransfer(targetAmount, bank.Code, account.AccountNumber, ss.SafeRemainingArgument);
		if (bank != bankTarget)
		{
			bank.CurrencyReserves[bank.PrimaryCurrency] -= amount;
			bank.Changed = true;
			bankTarget.CurrencyReserves[bankTarget.PrimaryCurrency] += targetAmount;
			bankTarget.Changed = true;
		}

		var moneyDescription = bank.PrimaryCurrency.Describe(amount, CurrencyDescriptionPatternType.Short)
								   .ColourValue();
		actor.OutputHandler.Send(
			$"You transfer {moneyDescription} from account {account.AccountNumber.ToString("F0", actor).ColourName()} into account {accountTarget.AccountNumber.ToString("F0", actor).ColourName()}{(bank != bankTarget ? $" of {bank.Name.ColourName()}" : "")}{(ss.SafeRemainingArgument.Length > 0 ? $", with reference {ss.SafeRemainingArgument.ColourCommand()}." : "")}.");
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ transfers {moneyDescription} between bank accounts.", actor, actor),
			flags: OutputFlags.SuppressSource));
	}

	private static void BankAccountWithdraw(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much money do you want to withdraw?");
			return;
		}

		if (actor.Currency != bank.PrimaryCurrency)
		{
			actor.OutputHandler.Send(
				$"This bank conducts transactions in the {bank.PrimaryCurrency.Name.ColourValue()} currency. You must {$"set currency {bank.PrimaryCurrency.Name}".MXPSend()} before using its services.");
			return;
		}

		if (!bank.PrimaryCurrency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send("That is not a valid amount of currency.");
			return;
		}

		var (success, reason) = account.CanWithdraw(amount, false);
		if (!success)
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		if (amount > bank.CurrencyReserves[bank.PrimaryCurrency])
		{
			actor.OutputHandler.Send(
				$"Unfortunately, {bank.Name.ColourName()} does not have enough currency to honour your withdrawal.");
			return;
		}

		account.Withdraw(amount);
		bank.CurrencyReserves[bank.PrimaryCurrency] -= amount;
		bank.Changed = true;
		var moneyDescription = bank.PrimaryCurrency.Describe(amount, CurrencyDescriptionPatternType.Short)
								   .ColourValue();
		actor.OutputHandler.Send(
			$"You withdraw {moneyDescription} from account {account.NameWithAlias}.");
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ withdraws {moneyDescription} from a bank account.", actor, actor),
			flags: OutputFlags.SuppressSource));

		var currencyItem =
			CurrencyGameItemComponentProto.CreateNewCurrencyPile(bank.PrimaryCurrency,
				bank.PrimaryCurrency.FindCoinsForAmount(amount, out _));
		if (actor.Body.CanGet(currencyItem, 0))
		{
			actor.Body.Get(currencyItem, silent: true);
		}
		else
		{
			currencyItem.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(currencyItem, true);
			actor.OutputHandler.Send("You couldn't hold your money, so it is on the ground.");
		}
	}

	private static void BankAccountShow(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		actor.OutputHandler.Send(account.Show(actor));
	}

	private static void BankAccountTransactions(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		actor.OutputHandler.Send(account.ShowTransactions(actor));
	}

	private static void BankAccountClose(ICharacter actor, StringStack ss, IBankAccount account, IBank bank)
	{
		var (truth, reason) = account.BankAccountType.CanCloseAccount(actor, account);
		if (!truth)
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to close {account.Name.ColourName()} with {bank.Name.ColourName()}? This action cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				var (success, error) = account.CloseAccount(actor);
				if (!success)
				{
					actor.OutputHandler.Send(error);
				}
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to close the bank account."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to close the bank account."); },
			DescriptionString = "Closing a bank account",
			Keywords = new List<string> { "close", "bank", "account" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void BankAccountTypes(ICharacter actor, IBank bank)
	{
		var sb = new StringBuilder();
		var accounts = bank.BankAccountTypes
						   .Where(x => x.CanOpenAccount(actor).Truth)
						   .ToList();
		if (!accounts.Any())
		{
			actor.OutputHandler.Send("There are no account types that you are eligible to open right now.");
			return;
		}

		sb.AppendLine($"You can open the following types of accounts with {bank.Name.ColourName()}:");

		foreach (var account in accounts)
		{
			sb.AppendLine();
			sb.AppendLine($"\t[{account.Name.Colour(Telnet.BoldCyan)}]");
			sb.AppendLine();
			sb.AppendLine(account.CustomerDescription.Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
			sb.AppendLine(
				$"See {$"bank preview {account.Name.ToLowerInvariant()}".MXPSend()} for a full fee statement.");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void BankAccountCreate(ICharacter actor, StringStack ss, IBank bank)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What type of bank account would you like to open?\nYou can see the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		var type = bank.BankAccountTypes.FirstOrDefault(x => x.Name.EqualTo(ss.SafeRemainingArgument)) ??
				   bank.BankAccountTypes.FirstOrDefault(x =>
					   x.Name.StartsWith(ss.SafeRemainingArgument, StringComparison.InvariantCultureIgnoreCase));
		if (type == null)
		{
			actor.OutputHandler.Send(
				$"There is no such bank account type.\nSee the list of options with {"bank account types".MXPSend()}.");
			return;
		}

		var (success, error) = bank.CanOpenAccount(actor, type);
		if (!success)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var account = type.OpenAccount(actor);
		bank.AddAccount(account);
		actor.OutputHandler.Send(
			$"You open a new {type.Name.ColourName()} account with {bank.Name.ColourName()}. The account number is {account.AccountNumber.ToString("N0", actor).ColourName()}.");
	}

	#endregion

	#region Auctions

	public const string AuctionHelp =
		@"The auction command is used to interact with auction houses, and it must be used at a location that is an auction house. You should also see the related command AUCTIONS.

The syntax for using this command is as follows:

	#3auction preview <item>#0 - view an item currently being auctioned
	#3auction sell <item> <price> <bank code>:<accn> [<buyout price>]#0 - lists an item for sale
	#3auction bid <item> <bid>#0 - makes a bid on an item
	#3auction buyout <item>#0 - pays the buyout price on an item
	#3auction claim#0 - claims all items won or not sold
	#3auction refund#0 - claims all money owed for unsuccessful bids
	#3auction cancel <item>#0 - cancels an item up for auction";

	public const string AuctionHelpAdmins = @"This command is used to create and edit auction houses.

The syntax for using this command is as follows:

	#3auction list#0 - lists all auction houses
	#3auction edit <which>#0 - begins editing an auction house
	#3auction close#0 - stops editing an auction house
	#3auction show <which>#0 - views an auction house
	#3auction edit new <name> <economic zone> <bank>#0 - creates a new auction house based in your current location
	#3auction set name <name>#0 - renames the auction house
	#3auction set economiczone <which>#0 - changes the economic zone
	#3auction set fee <amount>#0 - sets the flat fee for listing an item
	#3auction set rate <%>#0 - sets the percentage fee for listing an item
	#3auction set bank <bank code>:<accn>#0 - changes the bank account for revenues
	#3auction set time <time period>#0 - sets the amount of time auctions run for
	#3auction set location#0 - changes the location of the auction house to the current cell
	#3auction claimfor <id>#0 - claims all items for a specific character ID

There is also the player version of the command, which is used to interact with auction houses, and it must be used at a location that is an auction house. You should also see the related command AUCTIONS.

The syntax for using this command is as follows:

	#3auction preview <item>#0 - view an item currently being auctioned
	#3auction sell <item> <price> <bank code>:<accn> [<buyout price>]#0 - lists an item for sale
	#3auction bid <item> <bid>#0 - makes a bid on an item
	#3auction buyout <item>#0 - pays the buyout price on an item
	#3auction claim#0 - claims all items won or not sold
	#3auction refund#0 - claims all money owed for unsuccessful bids
	#3auction cancel <item>#0 - cancels an item up for auction

Note: Admins can use the #3auction cancel#0 subcommand on other people's items";

	[PlayerCommand("Auction", "auction")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("auction", AuctionHelp, AutoHelp.HelpArgOrNoArg, AuctionHelpAdmins)]
	protected static void Auction(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "edit":
			case "close":
			case "set":
			case "show":
			case "view":
			case "clone":
			case "list":
				BuilderModule.GenericBuildingCommand(actor, ss.GetUndo(), EditableItemHelper.AuctionHelper);
				return;
		}

		var auctionHouse = actor.Gameworld.AuctionHouses.FirstOrDefault(x => x.AuctionHouseCell == actor.Location);
		if (auctionHouse == null)
		{
			actor.OutputHandler.Send("You are not currently at an auction house.");
			return;
		}

		switch (ss.Last.ToLowerInvariant().CollapseString())
		{
			case "preview":
				AuctionPreview(actor, auctionHouse, ss);
				return;
			case "bid":
				AuctionBid(actor, auctionHouse, ss);
				return;
			case "refund":
				AuctionRefund(actor, auctionHouse, ss);
				return;
			case "buyout":
				AuctionBuyout(actor, auctionHouse, ss);
				return;
			case "sell":
				AuctionSell(actor, auctionHouse, ss);
				return;
			case "claim":
				AuctionClaim(actor, auctionHouse, ss);
				return;
			case "cancel":
				AuctionCancel(actor, auctionHouse, ss);
				return;
			default:
				actor.OutputHandler.Send(actor.IsAdministrator() ? AuctionHelpAdmins : AuctionHelp);
				return;
		}
	}

	private static void AuctionCancel(ICharacter actor, IAuctionHouse? auctionHouse, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which auction item do you want to cancel the auction of?");
			return;
		}

		var item = auctionHouse.ActiveAuctionItems.GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (item == null)
		{
			actor.OutputHandler.Send("There is no such item currently being auctioned.");
			return;
		}

		if (!actor.IsAdministrator())
		{
			if (item.ListingCharacterId != actor.Id)
			{
				actor.OutputHandler.Send(
					$"You were not the one who listed {item.Item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)}, so you cannot cancel its sale.");
				return;
			}

			if (auctionHouse.AuctionBids[item].Count > 0)
			{
				actor.OutputHandler.Send(
					$"Unfortunately you cannot cancel the sale of {item.Item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} as there have already been bids placed.");
				return;
			}
		}

		auctionHouse.CancelItem(item);
		var gameitem = item.Item;
		gameitem.Login();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ cancel|cancels the auction of $1 with {auctionHouse.Name.ColourName()}.", actor, actor, gameitem)));
		if (actor.Body.CanGet(gameitem, 0))
		{
			actor.Body.Get(gameitem, silent: true);
		}
		else
		{
			gameitem.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(gameitem);
			gameitem.SetPosition(PositionUndefined.Instance, PositionModifier.Before, actor, null);
			actor.OutputHandler.Send(
				$"You were unable to pick up {gameitem.HowSeen(actor)}, so it is on the ground at your feet.");
		}
	}

	private static void AuctionClaim(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
	{
		var unclaimed = auctionHouse.UnclaimedItems.Where(x =>
										x.WinningBid?.Bidder == actor || (x.WinningBid == null &&
																		  x.AuctionItem.ListingCharacterId == actor.Id))
									.ToList();
		if (!unclaimed.Any())
		{
			actor.OutputHandler.Send("You do not have any unclaimed auction items.");
			return;
		}

		foreach (var item in unclaimed)
		{
			auctionHouse.ClaimItem(item.AuctionItem);
		}

		var items = unclaimed.Select(x => x.AuctionItem.Item).ToList();
		foreach (var item in items)
		{
			item.Login();
		}

		IGameItem givenItem = null;
		if (items.Count > 1)
		{
			givenItem = PileGameItemComponentProto.CreateNewBundle(items);
			actor.Gameworld.Add(givenItem);
		}
		else
		{
			givenItem = items.Single();
		}

		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ claim|claims $1 from {auctionHouse.Name.ColourName()}.", actor, actor, givenItem)));
		if (actor.Body.CanGet(givenItem, 0))
		{
			actor.Body.Get(givenItem, silent: true);
		}
		else
		{
			givenItem.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(givenItem);
			givenItem.SetPosition(PositionUndefined.Instance, PositionModifier.Before, actor, null);
			actor.OutputHandler.Send(
				$"You were unable to pick up {givenItem.HowSeen(actor)}, so it is on the ground at your feet.");
		}
	}

	private static void AuctionSell(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What is it that you want to sell?");
			return;
		}

		var item = actor.TargetHeldItem(ss.PopSpeech());
		if (item == null)
		{
			actor.OutputHandler.Send("You aren't holding anything like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What price do you want to list {item.HowSeen(actor)} for? Prices must be in the {auctionHouse.EconomicZone.Currency.Name.TitleCase().ColourName()} currency.");
			return;
		}

		if (!auctionHouse.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out var price))
		{
			actor.OutputHandler.Send(
				$"The value {ss.Last.ColourValue()} is not a valid amount of {auctionHouse.EconomicZone.Currency.Name.TitleCase().ColourName()}.");
			return;
		}

		if (price <= auctionHouse.AuctionListingFeeFlat)
		{
			actor.OutputHandler.Send(
				$"You must list {item.HowSeen(actor)} for a price greater than {auctionHouse.EconomicZone.Currency.Describe(auctionHouse.AuctionListingFeeFlat, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a bank account into which any proceeds will be transferred. Use the format BANKCODE:ACCOUNT#.");
			return;
		}

		var bankString = ss.PopSpeech();
		var split = bankString.Split(':');
		if (split.Length != 2)
		{
			actor.OutputHandler.Send($"You must use the format BANKCODE:ACCOUNT# to specify the bank account.");
			return;
		}

		var bankTarget = actor.Gameworld.Banks.GetByName(split[0]) ??
						 actor.Gameworld.Banks.FirstOrDefault(x =>
							 x.Code.StartsWith(split[0], StringComparison.InvariantCultureIgnoreCase));
		if (bankTarget == null)
		{
			actor.OutputHandler.Send("There is no bank with that name or bank code.");
			return;
		}

		if (!int.TryParse(split[1], out var accn) || accn <= 0)
		{
			actor.OutputHandler.Send("The account number to transfer money into must be a number greater than zero.");
			return;
		}

		var accountTarget = bankTarget.BankAccounts.FirstOrDefault(x =>
			x.AccountNumber == accn && x.AccountStatus == BankAccountStatus.Active);
		if (accountTarget == null)
		{
			actor.OutputHandler.Send(
				$"The supplied account number is not a valid account number for {bankTarget.Name.ColourName()}.");
			return;
		}

		var buyout = 0.0M;
		if (!ss.IsFinished)
		{
			if (!auctionHouse.EconomicZone.Currency.TryGetBaseCurrency(ss.PopSpeech(), out buyout))
			{
				actor.OutputHandler.Send(
					$"The value {ss.Last.ColourValue()} is not a valid amount of {auctionHouse.EconomicZone.Currency.Name.TitleCase().ColourName()}.");
				return;
			}

			if (buyout < price)
			{
				actor.OutputHandler.Send(
					"You must specify a buyout price that is higher than the starting sale price.");
				return;
			}
		}

		var itemDesc = item.HowSeen(actor);
		actor.OutputHandler.Send(
			$"Are you sure you want to list {itemDesc} for sale at a reserve price of {auctionHouse.EconomicZone.Currency.Describe(price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (!actor.Body.HeldOrWieldedItems.Contains(item) || !actor.Body.CanDrop(item, 1))
				{
					actor.OutputHandler.Send($"You no longer have {itemDesc}.");
					return;
				}

				if (actor.Location != auctionHouse.AuctionHouseCell)
				{
					actor.OutputHandler.Send($"You are no longer in the auction house.");
					return;
				}

				actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ list|lists $1 for sale on the auction house.",
					actor, actor, item)));
				actor.Body.Take(item);
				item.Drop(null);
				auctionHouse.AddAuctionItem(new AuctionItem
				{
					BankAccount = accountTarget,
					ListingDateTime = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
					FinishingDateTime =
						new MudDateTime(auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime) +
						auctionHouse.DefaultListingTime,
					Item = item,
					MinimumPrice = price,
					BuyoutPrice = buyout > 0.0M ? buyout : null,
					ListingCharacterId = actor.Id
				});
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to list {itemDesc} for sale on the auction house.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to list {itemDesc} for sale on the auction house.");
			},
			DescriptionString = "List an item for sale on the auction house"
		}), TimeSpan.FromSeconds(120));
	}

	private static void AuctionBuyout(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which auction item do you want to buy out?");
			return;
		}

		var item = auctionHouse.ActiveAuctionItems.GetFromItemListByKeyword(ss.SafeRemainingArgument, actor);
		if (item == null)
		{
			actor.OutputHandler.Send("There is no such item currently being auctioned.");
			return;
		}

		if (!item.BuyoutPrice.HasValue)
		{
			actor.OutputHandler.Send(
				$"{item.Item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} does not have a buyout price.");
			return;
		}

		var amount = item.BuyoutPrice.Value;
		var currency = auctionHouse.EconomicZone.Currency;
		var targetCoins = currency.FindCurrency(actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			actor.OutputHandler.Send("You aren't holding any currency of that type.");
			return;
		}

		var coinValue = targetCoins.TotalValue();
		if (coinValue < amount)
		{
			actor.OutputHandler.Send(
				$"You aren't holding enough money to pay the {currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()} buyout price.");
			return;
		}

		var change = 0.0M;
		if (coinValue > amount)
		{
			change = coinValue - amount;
		}

		foreach (var coinItem in targetCoins)
		{
			if (!coinItem.Key.RemoveCoins(coinItem.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				actor.Body.Take(coinItem.Key.Parent);
				coinItem.Key.Parent.Delete();
			}
		}

		auctionHouse.AddBid(item, new AuctionBid
		{
			Bidder = actor,
			Bid = amount,
			BidDateTime = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
		});
		var moneyDescription = currency.Describe(amount, CurrencyDescriptionPatternType.Short)
									   .ColourValue();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ pay|pays out the buyout price of $2 on $1 at {auctionHouse.Name.ColourName()}.", actor, actor,
			item.Item, new DummyPerceivable(moneyDescription))));

		if (change > 0.0M)
		{
			var changeItem =
				CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
					currency.FindCoinsForAmount(change, out _));
			if (actor.Body.CanGet(changeItem, 0))
			{
				actor.Body.Get(changeItem, silent: true);
			}
			else
			{
				changeItem.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(changeItem, true);
				actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
			}
		}
	}

	private static void AuctionRefund(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
	{
		var owed = auctionHouse.CharacterRefundsOwed[actor.Id];
		if (owed <= 0.0M)
		{
			actor.OutputHandler.Send($"{auctionHouse.Name.ColourName()} does not owe you any refunds.");
			return;
		}

		if (!auctionHouse.ClaimRefund(actor))
		{
			actor.OutputHandler.Send(
				$"{auctionHouse.Name.ColourName()} does not have enough money to pay what they owe you.");
			return;
		}

		var moneyDescription = auctionHouse.EconomicZone.Currency.Describe(owed, CurrencyDescriptionPatternType.Short)
										   .ColourValue();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ claim|claims a $1 refund from {auctionHouse.Name.ColourName()}.", actor, actor,
			new DummyPerceivable(moneyDescription))));

		var currencyItem =
			CurrencyGameItemComponentProto.CreateNewCurrencyPile(auctionHouse.EconomicZone.Currency,
				auctionHouse.EconomicZone.Currency.FindCoinsForAmount(owed, out _));
		if (actor.Body.CanGet(currencyItem, 0))
		{
			actor.Body.Get(currencyItem, silent: true);
		}
		else
		{
			currencyItem.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(currencyItem, true);
			actor.OutputHandler.Send("You couldn't hold your money, so it is on the ground.");
		}
	}

	private static void AuctionBid(ICharacter actor, IAuctionHouse auctionHouse, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which auction item do you want to bid on?");
			return;
		}

		var item = auctionHouse.ActiveAuctionItems.GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (item == null)
		{
			actor.OutputHandler.Send("There is no such item currently being auctioned.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What bid do you want to make on {item.Item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreCanSee)}.");
			return;
		}

		var currency = auctionHouse.EconomicZone.Currency;
		if (!currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send($"That is not a valid amount of {currency.Name.ColourValue()}.");
			return;
		}

		var currentPrice = auctionHouse.AuctionBids[item].Select(x => x.Bid).DefaultIfEmpty(item.MinimumPrice)
									   .Max();
		var nextBidMinimum = !auctionHouse.AuctionBids[item].Any() ? item.MinimumPrice : currentPrice * 1.05M;
		if (amount <= nextBidMinimum)
		{
			actor.OutputHandler.Send(
				$"Your bid for {item.Item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} must be higher than {currency.Describe(nextBidMinimum, CurrencyDescriptionPatternType.Short).ColourValue()}.");
			return;
		}

		var targetCoins = currency.FindCurrency(actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ICurrencyPile>()),
			amount);
		if (!targetCoins.Any())
		{
			actor.OutputHandler.Send("You aren't holding any currency of that type.");
			return;
		}

		var coinValue = targetCoins.TotalValue();
		if (coinValue < amount)
		{
			actor.OutputHandler.Send(
				$"You aren't holding enough money to make a bid of that size.\nThe largest bid that you could make is {currency.Describe(coinValue, CurrencyDescriptionPatternType.Short).ColourValue()}.");
			return;
		}

		var change = 0.0M;
		if (coinValue > amount)
		{
			change = coinValue - amount;
		}

		foreach (var coinItem in targetCoins)
		{
			if (!coinItem.Key.RemoveCoins(coinItem.Value.Select(x => Tuple.Create(x.Key, x.Value))))
			{
				actor.Body.Take(coinItem.Key.Parent);
				coinItem.Key.Parent.Delete();
			}
		}

		auctionHouse.AddBid(item, new AuctionBid
		{
			Bidder = actor,
			Bid = amount,
			BidDateTime = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime
		});

		var moneyDescription = currency.Describe(amount, CurrencyDescriptionPatternType.Short)
									   .ColourValue();
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ bid|bids $2 on $1 at {auctionHouse.Name.ColourName()}.", actor, actor, item.Item,
			new DummyPerceivable(moneyDescription))));

		if (change > 0.0M)
		{
			var changeItem =
				CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency,
					currency.FindCoinsForAmount(change, out _));
			if (actor.Body.CanGet(changeItem, 0))
			{
				actor.Body.Get(changeItem, silent: true);
			}
			else
			{
				changeItem.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(changeItem, true);
				actor.OutputHandler.Send("You couldn't hold your change, so it is on the ground.");
			}
		}
	}

	private static void AuctionPreview(ICharacter actor, IAuctionHouse auctionHouse, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which auction item do you want to preview?");
			return;
		}

		var item = auctionHouse.ActiveAuctionItems.GetFromItemListByKeyword(command.SafeRemainingArgument, actor);
		if (item == null)
		{
			actor.OutputHandler.Send("There is no such item currently being auctioned.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(
			$"Previewing {item.Item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreCanSee)}");
		sb.AppendLine();
		sb.AppendLine(item.Item.HowSeen(actor, type: DescriptionType.Full,
			flags: PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreCanSee));
		sb.AppendLine();
		sb.AppendLine(
			$"Current Price: {auctionHouse.EconomicZone.Currency.Describe(auctionHouse.CurrentBid(item).IfZero(item.MinimumPrice), CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		if (item.BuyoutPrice.HasValue)
		{
			sb.AppendLine(
				$"Buyout Price: {auctionHouse.EconomicZone.Currency.Describe(item.BuyoutPrice.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}

		sb.AppendLine($"# of Bids: {auctionHouse.AuctionBids[item].Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Time Remaining: {(item.FinishingDateTime - auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime).Describe(actor).ColourValue()}");
		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Auctions", "auctions")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("auctions", @"", AutoHelp.HelpArg)]
	protected static void Auctions(ICharacter actor, string command)
	{
		var auctionHouse = actor.Gameworld.AuctionHouses.FirstOrDefault(x => x.AuctionHouseCell == actor.Location);
		if (auctionHouse == null)
		{
			actor.OutputHandler.Send("You are not currently at an auction house.");
			return;
		}

		var listings = auctionHouse.ActiveAuctionItems.ToList();

		var ss = new StringStack(command.RemoveFirstWord());
		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech();
			listings = listings.Where(x =>
				x.Item.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreCanSee)
				 .Contains(cmd, StringComparison.InvariantCultureIgnoreCase)).ToList();
			if (listings.Count == 0)
			{
				actor.OutputHandler.Send($"There are no auction listings with the keyword {cmd.ColourCommand()}.");
				return;
			}
		}

		var psb = new StringBuilder();

		if (auctionHouse.CharacterRefundsOwed[actor.Id] > 0)
		{
			psb.AppendLine(
				$"\nThe auction house owes you {auctionHouse.EconomicZone.Currency.Describe(auctionHouse.CharacterRefundsOwed[actor.Id], CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} in refunds.\nType {"AUCTION REFUND".MXPSend("auction refund", "Click to send AUCTION REFUND to the MUD")} to claim your money.");
		}

		var unclaimed = auctionHouse.UnclaimedItems.Where(x =>
										(x.AuctionItem.ListingCharacterId == actor.Id && x.WinningBid == null) ||
										x.WinningBid?.BidderId == actor.Id
									)
									.ToList();
		if (unclaimed.Any())
		{
			psb.AppendLine(
				$"\nYou have unclaimed items with this auction house. Type {"AUCTION CLAIM".MXPSend("auction claim", "Click to send AUCTION CLAIM to the MUD")} to claim your items.");
		}

		if (listings.Count == 0)
		{
			actor.OutputHandler.Send(
				$"{auctionHouse.Name.TitleCase().ColourName()} currently has no listings.{psb.ToString()}");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(auctionHouse.Name.TitleCase().ColourName());
		sb.AppendLine();
		var i = 1;
		var now = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		sb.AppendLine(StringUtilities.GetTextTable(
			from listing in listings
			orderby listing.ListingDateTime
			select new List<string>
			{
				(i++).ToString("N0", actor),
				listing.Item.HowSeen(actor,
					flags: PerceiveIgnoreFlags.IgnoreLoadThings | PerceiveIgnoreFlags.IgnoreCanSee),
				auctionHouse.AuctionBids[listing].Count.ToString("N0", actor),
				auctionHouse.EconomicZone.Currency.Describe(
					auctionHouse.AuctionBids[listing].Select(x => x.Bid).DefaultIfEmpty(listing.MinimumPrice).Max(),
					CurrencyDescriptionPatternType.ShortDecimal),
				(listing.FinishingDateTime - now).Describe(actor),
				listing.BuyoutPrice.HasValue
					? auctionHouse.EconomicZone.Currency.Describe(listing.BuyoutPrice.Value,
						CurrencyDescriptionPatternType.ShortDecimal)
					: "N/A"
			},
			new List<string>
			{
				"#",
				"Item",
				"# Bids",
				"Current Bid",
				"Time Remaining",
				"Buyout"
			},
			actor.LineFormatLength,
			colour: Telnet.Yellow,
			unicodeTable: actor.Account.UseUnicode
		));
		if (psb.Length > 0)
		{
			sb.Append(psb.ToString());
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	#endregion

	#region Jobs

	[PlayerCommand("Jobs", "jobs")]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("jobs", @"The jobs command allows you to see three bits of information:

	1) Which jobs you currently hold (or that you no longer hold but still owe you money)
	2) Which jobs you or your clans have listed
	3) Which jobs are hiring

Of these 3, only the 1st one can be done from anywhere. The other 2 items need to be done from a location flagged as a 'job market' location.

You should also see the JOB command for ways to interact with these jobs.", AutoHelp.HelpArg)]
	protected static void Jobs(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var sb = new StringBuilder();

		if (actor.ActiveJobs.Any())
		{
			sb.AppendLine("You are currently employed in the following jobs:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from job in actor.ActiveJobs
				select new List<string>
				{
					job.Id.ToString("N0", actor),
					job.Name,
					job.JobCommenced.Date.Display(CalendarDisplayMode.Short),
					job.JobDueToEnd?.Date.Display(CalendarDisplayMode.Short) ?? "",
					job.Listing is IOngoingJobListing ojl
						? ojl.PayReference.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)
						: "",
					job.RevenueEarned
					   .Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
					   .DefaultIfEmpty("None").ListToString(),
					job.BackpayOwed
					   .Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
					   .DefaultIfEmpty("None").ListToString(),
					job.Listing.Employer is ICharacter ch
						? ch.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Magenta)
						: ((IClan)job.Listing.Employer).FullName.ColourName()
				},
				new List<string>
				{
					"Id",
					"Name",
					"Commenced",
					"Ends",
					"Payday",
					"Earned",
					"Owed",
					"Employer"
				},
				actor,
				Telnet.Green
			));

			var load = actor.ActiveJobs.Sum(x => x.FullTimeEquivalentRatio);
			sb.AppendLine(
				$"You are currently working at {load.ToString("P0", actor).ColourValue()} of a full time work load.");
		}

		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
		if (ez is null)
		{
			sb.AppendLine($"\nYou are not at a location where jobs can be posted or accepted.");
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var ownJobs = actor.Gameworld.JobListings.Where(x => x.EconomicZone == ez && x.IsAuthorisedToEdit(actor))
						   .ToList();
		if (ownJobs.Any())
		{
			sb.AppendLine($"You have the following job listings in the {ez.Name.ColourName()} economic zone:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from job in ownJobs
				select new List<string>
				{
					job.Id.ToString("N0", actor),
					job.Name,
					job.ActiveJobs.Count(x => !x.IsJobComplete).ToString("N0", actor),
					job.NetFinancialPosition.Select(x =>
						   x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
					   .ListToString(),
					double.IsInfinity(job.DaysOfSolvency) ? "Forever" : job.DaysOfSolvency.ToString("N1", actor),
					job.Employer is ICharacter ch
						? ch.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Magenta)
						: ((IClan)job.Employer).FullName.ColourName()
				},
				new List<string>
				{
					"Id",
					"Name",
					"Employees",
					"Net Position",
					"Days Solvent",
					"Employer"
				},
				actor,
				Telnet.Green
			));
		}

		// TODO - filters
		var jobs = actor.Gameworld.JobListings
						.Where(x => !x.IsArchived && x.IsReadyToBePosted && x.EconomicZone == ez)
						.OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
						.ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
						.ThenBy(x => x.Name)
						.ToList();
		if (jobs.Any())
		{
			sb.AppendLine($"There are the following job listings locally:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from job in jobs
				select new List<string>
				{
					job.Id.ToString("N0", actor),
					job.Name,
					job.Employer is ICharacter ch
						? ch.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Magenta)
						: ((IClan)job.Employer).FullName.ColourName(),
					job.PayDescriptionForJobListing(),
					job.IsEligibleForJob(actor).Truth.ToString(),
					job.MaximumNumberOfSimultaneousEmployees == 0
						? "Unlimited"
						: (job.MaximumNumberOfSimultaneousEmployees - job.ActiveJobs.Count(x => !x.IsJobComplete))
						.ToString("N0", actor)
				},
				new List<string>
				{
					"Id",
					"Name",
					"Employer",
					"Pay",
					"Eligible?",
					"# Positions"
				},
				actor,
				Telnet.Green
			));
		}
		else
		{
			sb.AppendLine($"There are currently no jobs listed locally.");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private const string JobHelp =
		@"The job command is used to interact with job listings and jobs that you hold. Most of these commands need to be done from a location that is flagged as a job market in the area's economic zone.

You should see the closely related JOBS command for a list of jobs that you manage or that are posted in your area.

The following basic syntaxes are used to interact with jobs:

	#3job preview <job>#0 - shows details about a posted job you're interested in
	#3job apply <job>#0 - applies for a job
	#3job quit <job>#0 - quits your job #2[works anywhere]#0
	#3job payday#0 - collects all monies you've earned from jobs
	#3job bankpayday <bank>:<account>#0 - collects all money from jobs into a bank account #2[works anywhere]#0

If you're interested in posting a job rather than applying for one, you can use the following syntaxes:

	#3job show <your job>#0 - shows employer information about a job
	#3job edit <your job>#0 - begins to edit a job listing
	#3job close#0 - stops editing a job listing
	#3job edit#0 - an alias for doing #3job show#0 on the job you're currently editing
	#3job new <name>#0 - creates a new job with you as the employer
	#3job newclan <clan> <name>#0 - creates a new job with a clan as the employer

The following commands require you to be editing a job listing:

	#3job deposit <money>#0 - deposits money into the coffers of the job to pay for payroll
	#3job withdraw <money>#0 - withdraws money from the coffers of the job
	#3job employees#0 - lists all employees currently working on this job
	#3job fire <who>#0 - fires an employee from the job
	#3job ready#0 - toggles the readiness of this posting for the job market
	#3job finish#0 - ends the job and pays out all the employees
	#3job set name <name>#0 - renames this job listing
	#3job set desc#0 - drops you into an editor to write a description for this job
	#3job set ratio <verycasual|parttime|fulltime|overtime|punishing>#0 - sets the job effort ratio
	#3job set employees#0 - permits an unlimited number of simultaneous employees
	#3job set employees <##>#0 - sets the maximum number of simultaneous employees
	#3job set clan <name>#0 - sets a clan that employees get membership in
	#3job set clan none#0 - clears the clan from this job
	#3job set rank <name>#0 - sets a clan rank for employees
	#3job set rank none#0 - clears the rank from this job
	#3job set paygrade <name>#0 - sets a clan paygrade for employees
	#3job set paygrade none#0 - clears the paygrade from this job
	#3job set appointment <name>#0 - sets a clan appointment for employees
	#3job set appointment none#0 - clears the appointment from this job
	#3job set term <time>#0 - sets the maximum term employees can hold this job
	#3job set term#0 - clears the term limit from this job
	#3job set prog <prog>#0 - sets a prog that controls who can hold this job

Note: There may be additional properties that can be edited depending on the type of job.";

	[PlayerCommand("Job", "job")]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("job", JobHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Job(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "bankpayday":
				JobPayday(actor, ss, true);
				return;
			case "quit":
				JobQuit(actor, ss);
				return;
			case "preview":
				JobPreview(actor, ss);
				return;
		}

		ss = ss.GetUndo();
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
		if (ez is null)
		{
			actor.OutputHandler.Send($"You are not at a location where jobs can be interacted with.");
			return;
		}


		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "edit":
				JobEdit(actor, ss);
				return;
			case "close":
				JobClose(actor, ss);
				return;
			case "new":
				JobNew(actor, ss);
				return;
			case "newclan":
				JobNewClan(actor, ss);
				return;
			case "set":
				JobSet(actor, ss);
				return;
			case "show":
				JobShow(actor, ss);
				return;
			case "apply":
				JobApply(actor, ss);
				return;
			case "employees":
				JobEmployees(actor, ss);
				return;
			case "fire":
				JobFire(actor, ss);
				return;
			case "deposit":
				JobDeposit(actor, ss);
				return;
			case "withdraw":
				JobWithdraw(actor, ss);
				return;
			case "payday":
				JobPayday(actor, ss, false);
				return;
			case "finish":
				JobFinish(actor, ss);
				return;
			case "ready":
				JobReady(actor, ss);
				return;
		}
	}

	private static void JobNewClan(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What clan do you want to create a job listing for?");
			return;
		}

		var clan = ClanModule.GetTargetClan(actor, ss.PopSpeech());
		if (clan is null)
		{
			actor.OutputHandler.Send(actor.IsAdministrator()
				? "There is no such clan"
				: "You are not a member of any such clan.");
			return;
		}

		var membership = actor.ClanMemberships.FirstOrDefault(x => !x.IsArchivedMembership && x.Clan == clan);
		if (!actor.IsAdministrator() && (membership is null ||
										 !membership.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanJobs)))
		{
			actor.OutputHandler.Send(
				$"You do not have permission to manage job listings in the {clan.FullName.ColourName()} clan.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new job?");
			return;
		}

		var ez = actor.Gameworld.EconomicZones.First(x => x.JobFindingCells.Contains(actor.Location));
		var name = ss.PopSpeech().TitleCase();
		var job = new OngoingJobListing(actor.Gameworld, name, ez, clan, ez.Currency);
		actor.Gameworld.Add(job);
		actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
		actor.AddEffect(new BuilderEditingEffect<IJobListing>(actor) { EditingItem = job });
		actor.OutputHandler.Send(
			$"You create a new ongoing job called {name.ColourName()} with Id #{job.Id.ToString("N0", actor)}, which are you now editing.");
	}

	private static void JobReady(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any job listings.");
			return;
		}

		var job = effect.EditingItem;
		if (job.IsArchived)
		{
			actor.OutputHandler.Send(
				$"The {job.Name.ColourName()} job is already finished, so it cannot be ready for listing.");
			return;
		}

		job.IsReadyToBePosted = !job.IsReadyToBePosted;
		if (!job.IsReadyToBePosted && job.ActiveJobs.Any(x => !x.IsJobComplete))
		{
			actor.OutputHandler.Send(
				$"You withdraw the {job.Name.ColourName()} job from public advertisement to new applicants, but existing employees remain.");
		}
		else if (!job.IsReadyToBePosted)
		{
			actor.OutputHandler.Send($"You withdraw the {job.Name.ColourName()} job from public advertisement.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"The {job.Name.ColourName()} job is now publicly listed and ready for applicants.");
		}
	}

	private static void JobFinish(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any job listings.");
			return;
		}

		var job = effect.EditingItem;
		if (job.IsArchived)
		{
			actor.OutputHandler.Send(
				$"The {job.Name.ColourName()} job is already finished. If you want to remove it from your list, you need to ensure that all employees are paid any backpay that they are owed.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to finish the {job.Name.ColourName()} job? This action cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (job.IsArchived)
				{
					actor.OutputHandler.Send($"The {job.Name.ColourName()} job has already been finished.");
					return;
				}

				actor.OutputHandler.Send($"You finish the {job.Name.ColourName()} job.");
				job.FinishJob();
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to finish the {job.Name.ColourName()} job.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to finish the {job.Name.ColourName()} job.");
			},
			Keywords = new List<string>
			{
				"finish",
				"job"
			},
			DescriptionString = "Finishing a job listing"
		}), TimeSpan.FromSeconds(120));
	}

	private static void JobPayday(ICharacter actor, StringStack ss, bool useBank)
	{
		if (!actor.ActiveJobs.Any())
		{
			actor.OutputHandler.Send("You don't have any jobs, so nobody is going to pay you anything.");
			return;
		}

		IBankAccount bankAccount = null;
		if (useBank)
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which bank account do you want to deposit your pay into?");
				return;
			}

			var (account, error) = Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
			if (account is null)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (!account.IsAuthorisedAccountUser(actor))
			{
				actor.OutputHandler.Send("You are not authorised to use that bank account.");
				return;
			}

			bankAccount = account;
		}

		var owed = new DecimalCounter<ICurrency>();
		var paid = new DecimalCounter<ICurrency>();
		foreach (var job in actor.ActiveJobs)
		{
			owed.Add(job.BackpayOwed);
			paid.Add(job.RevenueEarned);
		}

		var sb = new StringBuilder();
		if (paid.All(x => x.Value <= 0.0M))
		{
			sb.AppendLine($"None of your jobs have paid you anything yet.");
		}
		else
		{
			if (useBank)
			{
				foreach (var item in paid.ToList())
				{
					if (item.Key != bankAccount.Currency)
					{
						if (!bankAccount.Bank.ExchangeRates.ContainsKey((item.Key, bankAccount.Currency)))
						{
							sb.AppendLine(
								$"One of your jobs pays you in {item.Key.Name.ColourValue()}, which can't be deposited into the bank account {bankAccount.AccountReference.ColourName()}. You must collect your pay in coin or specify a different bank account.");
							return;
						}

						paid[bankAccount.Currency] +=
							bankAccount.Bank.ExchangeRates[(item.Key, bankAccount.Currency)] * item.Value;
						paid.Remove(item.Key);
					}
				}

				foreach (var item in paid)
				{
					bankAccount.DepositFromTransaction(item.Value, "Deposit from payday");
				}

				sb.AppendLine(
					$"You deposit {paid.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()} in pay into the bank account {bankAccount.AccountReference.ColourName()} from your jobs.");
			}
			else
			{
				sb.AppendLine(
					$"You collect {paid.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()} in pay from your jobs.");
				foreach (var item in paid)
				{
					var pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(item.Key,
						item.Key.FindCoinsForAmount(item.Value, out _));
					if (actor.Body.CanGet(pile, 0))
					{
						actor.Body.Get(pile, 0, silent: true);
					}
					else
					{
						pile.RoomLayer = actor.RoomLayer;
						actor.Location.Insert(pile, true);
						pile.PositionTarget = actor;
						sb.AppendLine($"You couldn't hold {pile.HowSeen(actor)}, so it is on the ground.");
					}
				}
			}
		}

		foreach (var job in actor.ActiveJobs)
		{
			if (job.RevenueEarned.Any(x => x.Value > 0.0M))
			{
				job.RevenueEarned.Clear();
				job.Changed = true;
			}

			var jobOwed = job.BackpayOwed;
			if (jobOwed.Any())
			{
				var employer = job.Listing.Employer is ICharacter ch
					? ch.PersonalName.GetName(NameStyle.FullName).ColourCharacter()
					: ((IClan)job.Listing.Employer).FullName.ColourName();
				sb.AppendLine(
					$"You are owed {jobOwed.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()} by your employer {employer} from your {job.Name.ColourValue()} job.");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void JobWithdraw(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any job listings.");
			return;
		}

		if (actor.Currency is null)
		{
			actor.OutputHandler.Send(
				$"You have not set a currency. You must first CURRENCY SET <currency> before you can use this command.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much do you want to withdraw from the coffers of that job listing?");
			return;
		}

		var job = effect.EditingItem;
		if (!actor.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid amount of {actor.Currency.Name.ColourValue()}.");
			return;
		}

		if (job.MoneyPaidIn[actor.Currency] >= amount)
		{
			job.MoneyPaidIn[actor.Currency] -= amount;
			job.Changed = true;
			var pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(actor.Currency,
				actor.Currency.FindCoinsForAmount(amount, out _));
			actor.OutputHandler.Send(
				$"You withdraw {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from the coffers of job {job.Name.ColourName()}.");
			if (actor.Body.CanGet(pile, 0))
			{
				actor.Body.Get(pile, 0, silent: true);
			}
			else
			{
				pile.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(pile, true);
				pile.PositionTarget = actor;
				actor.OutputHandler.Send($"You couldn't hold {pile.HowSeen(actor)}, so it is on the ground.");
			}

			return;
		}

		actor.OutputHandler.Send(
			$"There is not enough money in the coffers of {job.Name.ColourName()} for you to withdraw that much.");
	}

	private static void JobDeposit(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any job listings.");
			return;
		}

		if (actor.Currency is null)
		{
			actor.OutputHandler.Send(
				$"You have not set a currency. You must first CURRENCY SET <currency> before you can use this command.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("How much do you want to deposit into the coffers of that job listing?");
			return;
		}

		var job = effect.EditingItem;
		if (!actor.Currency.TryGetBaseCurrency(ss.SafeRemainingArgument, out var amount))
		{
			actor.OutputHandler.Send(
				$"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid amount of {actor.Currency.Name.ColourValue()}.");
			return;
		}

		var payment = new OtherCashPayment(actor.Currency, actor);
		var accessible = payment.AccessibleMoneyForPayment();
		if (accessible >= amount)
		{
			payment.TakePayment(amount);
			job.MoneyPaidIn[actor.Currency] += amount;
			job.Changed = true;
			actor.OutputHandler.Send(
				$"You deposit {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} into the coffers of the job {job.Name.ColourName()}.");
			return;
		}

		if (accessible == 0.0M)
		{
			actor.OutputHandler.Send(
				$"There is no money in the coffers of {job.Name.ColourName()} for the {actor.Currency.Name.ColourValue()} currency.");
			return;
		}

		actor.OutputHandler.Send(
			$"There is not enough money in the coffers of {job.Name.ColourName()} for you to withdraw {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.\nThe most you could withdraw is {actor.Currency.Describe(accessible, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
	}

	private static void JobFire(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any job listings.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Who do you want to fire from that job? You can view the currency employees with {"job employees".MXPSend("job employees")}.");
			return;
		}

		var employees = effect.EditingItem.ActiveJobs.Where(x => !x.IsJobComplete).Select(x => x.Character)
							  .ToList();
		var employee = employees.GetFromItemListByKeywordIncludingNames(ss.SafeRemainingArgument, actor);
		if (employee is null)
		{
			actor.OutputHandler.Send(
				$"There is no one like that currency working for the {effect.EditingItem.Name.ColourName()} job.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				var activeJob = effect.EditingItem.ActiveJobs.FirstOrDefault(x => x.Character == employee);
				if (activeJob is null || activeJob.IsJobComplete)
				{
					actor.OutputHandler.Send(
						$"You can't fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} because {employee.ApparentGender(actor).Subjective()} {(employee.ApparentGender(actor).UseThirdPersonVerbForms ? "are" : "is")} no longer an employee of the job {effect.EditingItem.Name.ColourName()}");
					return;
				}

				activeJob.FireFromJob();
				actor.OutputHandler.Send(
					$"You fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send(
					$"You decide not to fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to fire {employee.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} from the job {effect.EditingItem.Name.ColourName()}.");
			},
			DescriptionString = $"Firing {employee.PersonalName.GetName(NameStyle.FullName)} from their job",
			Keywords = new List<string> { "fire" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void JobEmployees(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any job listings.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Employees of the {effect.EditingItem.Name.ColourName()} job:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in effect.EditingItem.ActiveJobs
			select new List<string>
			{
				item.Character.PersonalName.GetName(NameStyle.FullName),
				item.Character.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf),
				item.JobCommenced.Date.Display(CalendarDisplayMode.Short),
				item.JobDueToEnd?.Date.Display(CalendarDisplayMode.Short) ?? "",
				item.IsJobComplete.ToString(),
				item.BackpayOwed
					.Where(x => x.Value >= 0.0M)
					.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue())
					.DefaultIfEmpty("None")
					.ListToString()
			},
			new List<string>
			{
				"Name",
				"Appearance",
				"Started",
				"Ending",
				"Complete?",
				"Owed Backpay"
			},
			actor,
			Telnet.Green
		));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void JobQuit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which of your jobs do you want to quit?");
			return;
		}

		var job = actor.ActiveJobs.Where(x => !x.IsJobComplete).GetByIdOrName(ss.SafeRemainingArgument);
		if (job is null)
		{
			actor.OutputHandler.Send("You're not working any job like that.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you want to quit your {job.Name.ColourName()} job?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				var activeJob = actor.ActiveJobs.FirstOrDefault(x => x.Listing == job.Listing);
				if (activeJob is null || activeJob.IsJobComplete)
				{
					actor.OutputHandler.Send(
						$"You can't quit your {job.Name.ColourName()} job because it's already complete.");
					return;
				}

				actor.OutputHandler.Send($"You quit your {job.Name.ColourName()} job.");
				job.QuitJob();
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to quit your {job.Name.ColourName()} job.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to quit your {job.Name.ColourName()} job.");
			},
			DescriptionString = "Quitting your job",
			Keywords = new List<string> { "quit", "job" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void JobApply(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
		var jobs = actor.Gameworld.JobListings
						.Where(x => !x.IsArchived && x.IsReadyToBePosted && x.EconomicZone == ez)
						.OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
						.ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
						.ThenBy(x => x.Name)
						.ToList();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which job would you like to apply for? Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
			return;
		}

		var job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
		if (job is null)
		{
			actor.OutputHandler.Send(
				$"There is no job like that for you to apply for. Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
			return;
		}

		var (truth, error) = job.IsEligibleForJob(actor);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Are you sure that you want to apply for the {job.Name.ColourName()} job?");
		var effort = job.FullTimeEquivalentRatio +
					 actor.ActiveJobs.Where(x => !x.IsJobComplete).Sum(x => x.FullTimeEquivalentRatio);
		if (effort > 1.0)
		{
			sb.AppendLine(
				$"Warning: This job will put you over {1.0.ToString("P", actor)} equivalent of full time hours. This will impact your skill rolls due to overwork.");
		}

		sb.Append(Accept.StandardAcceptPhrasing);
		actor.OutputHandler.Send(sb.ToString());
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (job.IsArchived || !job.IsReadyToBePosted)
				{
					actor.OutputHandler.Send($"Unfortunately, the {job.Name.ColourName()} job has been withdrawn.");
					return;
				}

				if (job.ActiveJobs.Any(x => !x.IsJobComplete && x.Listing == job && x.Character == actor))
				{
					actor.OutputHandler.Send("You already have that job.");
					return;
				}

				var (truth, error) = job.IsEligibleForJob(actor);
				if (!truth)
				{
					actor.OutputHandler.Send(error);
					return;
				}

				job.ApplyForJob(actor);
				actor.OutputHandler.Send($"You apply for the {job.Name.ColourName()} job, and are now employed!");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send($"You decide not to apply for the {job.Name.ColourName()} job.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send($"You decide not to apply for the {job.Name.ColourName()} job.");
			},
			DescriptionString = "Accepting a job application",
			Keywords = new List<string> { "job", "application" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void JobPreview(ICharacter actor, StringStack ss)
	{
		var ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.JobFindingCells.Contains(actor.Location));
		var jobs =
				ez is not null
					? actor.Gameworld.JobListings
						   .Where(x => !x.IsArchived && x.IsReadyToBePosted && x.EconomicZone == ez)
						   .OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
						   .ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
						   .ThenBy(x => x.Name)
						   .ToList()
					: actor.ActiveJobs.Select(x => x.Listing)
						   .OrderByDescending(x => x.IsEligibleForJob(actor).Truth)
						   .ThenByDescending(x => x.Employer.FrameworkItemType.EqualTo("clan"))
						   .ThenBy(x => x.Name)
						   .ToList()
			;

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which job would you like to preview? Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
			return;
		}

		var job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
		if (job is null)
		{
			actor.OutputHandler.Send(
				$"There is no job like that for you to preview. Use {"jobs".MXPSend("jobs")} to see a list of available jobs.");
			return;
		}

		actor.OutputHandler.Send(job.ShowToPlayer(actor));
	}

	private static void JobNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new job?");
			return;
		}

		var ez = actor.Gameworld.EconomicZones.First(x => x.JobFindingCells.Contains(actor.Location));
		var name = ss.PopSpeech().TitleCase();
		var job = new OngoingJobListing(actor.Gameworld, name, ez, actor, ez.Currency);
		actor.Gameworld.Add(job);
		actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
		actor.AddEffect(new BuilderEditingEffect<IJobListing>(actor) { EditingItem = job });
		actor.OutputHandler.Send(
			$"You create a new ongoing job called {name.ColourName()} with Id #{job.Id.ToString("N0", actor)}, which are you now editing.");
	}

	private static void JobSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any job listings.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void JobShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which job listing do you want to view?");
			return;
		}

		var ez = actor.Gameworld.EconomicZones.First(x => x.JobFindingCells.Contains(actor.Location));
		var jobs = actor.Gameworld.JobListings.Where(x => x.EconomicZone == ez && x.IsAuthorisedToEdit(actor))
						.ToList();
		var job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
		if (job is null)
		{
			actor.OutputHandler.Send("There is no job listing that you have access to like that.");
			return;
		}

		actor.OutputHandler.Send(job.Show(actor));
	}

	private static void JobClose(ICharacter actor, StringStack ss)
	{
		actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
		actor.OutputHandler.Send($"You are no longer editing any job listings.");
	}

	private static void JobEdit(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IJobListing>>().FirstOrDefault();
		if (ss.IsFinished && effect is not null)
		{
			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which job listing would you like to edit?");
			return;
		}

		var ez = actor.Gameworld.EconomicZones.First(x => x.JobFindingCells.Contains(actor.Location));
		var jobs = actor.Gameworld.JobListings.Where(x => x.EconomicZone == ez && x.IsAuthorisedToEdit(actor))
						.ToList();
		var job = jobs.GetByIdOrName(ss.SafeRemainingArgument);
		if (job is null)
		{
			actor.OutputHandler.Send("There is no job listing that you have access to like that.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IJobListing>>();
		actor.AddEffect(new BuilderEditingEffect<IJobListing>(actor) { EditingItem = job });
		actor.OutputHandler.Send(
			$"You open job listing #{job.Id.ToString("N0", actor)} ({job.Name.ColourName()}) for editing.");
	}

	#endregion

	#region Market Related Code

	#region Market Influence Templates
	public const string MarketInfluenceTemplateHelpText = @"This command is used to create and edit Market Influence Templates. These are templates for creating market influences which apply supply or demand changes for goods in a market.

It is recommended that you use this command rather than creating market influences directly with #3MARKETINFLUENCE#0, but that is also an option.

The syntax for this command is as follows:

	#3mit list#0 - shows all market influence templates
	#3mit show <id>#0 - shows a particular market influence template
	#3mit edit <id>#0 - begins editing a market influence template
	#3mit edit#0 - an alias for #3mit show <editing id>#0
	#3mit close#0 - stops editing a market influence template
	#3mit clone <name>#0 - clones an existing template and then begins editing the clone
	#3mit new <name>#0 - creates a new market influence template
	#3mit set name <name>#0 - sets a new name
	#3mit set about#0 - drops you into an editor to write an about info for builders
	#3mit set desc#0 - drops you into an editor to write a description for players
	#3mit set know <prog>#0 - sets the prog that controls if players know about this
	#3mit set impact <category> <supply%> <demand%>#0 - adds or edits an impact for a category
	#3mit set remimpact <category>#0 - removes the impact for a category";

	[PlayerCommand("MarketInfluenceTemplate", "marketinfluencetemplate", "mit")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("MarketInfluenceTemplate", MarketInfluenceTemplateHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void MarketInfluenceTemplate(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				MarketInfluenceTemplateList(actor, ss);
				return;
			case "new":
			case "create":
				MarketInfluenceTemplateNew(actor, ss);
				return;
			case "clone":
				MarketInfluenceTemplateClone(actor, ss);
				return;
			case "set":
				MarketInfluenceTemplateSet(actor, ss);
				return;
			case "edit":
				MarketInfluenceTemplateEdit(actor, ss);
				return;
			case "close":
				MarketInfluenceTemplateClose(actor, ss);
				return;
			case "show":
			case "view":
				MarketInfluenceTemplateShow(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(MarketInfluenceTemplateHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void MarketInfluenceTemplateNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name would you like to give to this market influence template?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.MarketInfluenceTemplates.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a market influence template called {name.ColourName()}. Names must be unique.");
			return;
		}

		var template = new MarketInfluenceTemplate(actor.Gameworld, name);
		actor.Gameworld.Add(template);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketInfluenceTemplate>(actor) { EditingItem = template });
		actor.OutputHandler.Send($"You are create a new market influence template called {name.ColourValue()}, which you are now editing.");
	}

	private static void MarketInfluenceTemplateClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which market influence template do you want to clone?");
			return;
		}

		var old = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.SafeRemainingArgument);
		if (old is null)
		{
			actor.OutputHandler.Send("There is no market influence template like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new market influence template?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.MarketInfluenceTemplates.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a market influence template called {name.ColourName()}. Names must be unique.");
			return;
		}

		var category = old.Clone(name);
		actor.Gameworld.Add(category);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketInfluenceTemplate>(actor) { EditingItem = category });
		actor.OutputHandler.Send($"You are clone market influence template {old.Name.ColourValue()} to a new market influence template called {category.Name.ColourName()}, which you are now editing.");
	}

	private static void MarketInfluenceTemplateSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any market influence templates.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void MarketInfluenceTemplateEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market influence template would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var template = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.SafeRemainingArgument);
		if (template is null)
		{
			actor.OutputHandler.Send("There is no market influence template like that.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketInfluenceTemplate>(actor) { EditingItem = template });
		actor.OutputHandler.Send($"You are now editing the {template.Name.ColourName()} market influence template.");
	}

	private static void MarketInfluenceTemplateClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any market influence templates.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluenceTemplate>>();
		actor.OutputHandler.Send("You are no longer editing any market influence templates.");
	}

	private static void MarketInfluenceTemplateShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluenceTemplate>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market influence template would you like to show?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var category = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.SafeRemainingArgument);
		if (category is null)
		{
			actor.OutputHandler.Send("There is no market influence template like that.");
			return;
		}

		actor.OutputHandler.Send(category.Show(actor));
	}

	private static void MarketInfluenceTemplateList(ICharacter actor, StringStack ss)
	{
		var templates = actor.Gameworld.MarketInfluenceTemplates.ToList();
		// TODO - filters
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in templates
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.CharacterKnowsAboutInfluenceProg.MXPClickableFunctionName(),
				item.TemplateSummary,
				actor.Gameworld.MarketInfluences.Count(x => x.MarketInfluenceTemplate == item).ToString("N0", actor),
				actor.Gameworld.MarketInfluences.Count(x => x.MarketInfluenceTemplate == item && x.Applies(null, x.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)).ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Prog",
				"Summary",
				"# Infl",
				"# Active"
			},
			actor,
			Telnet.Cyan,
			3
		));
	}
	#endregion

	#region Market Influences
	public const string MarketInfluenceHelpText = @"The Market Influence command is used to create and manage 

	#3mi list#0 - shows all market influences
	#3mi show <id>#0 - shows a particular market influence
	#3mi edit <id>#0 - begins editing a market influence
	#3mi edit#0 - an alias for #3mi show <editing id>#0
	#3mi close#0 - stops editing a market influence
	#3mi clone <name>#0 - clones an existing influence and then begins editing the clone
	#3mi new <market> <date> <name>#0 - creates a new market influence
	#3mi begin <market> <template> [<from>] [<until>]#0 - creates a new market influence from a template
	#3mi end <id>#0 - ends a market influence
	#3mi set name <name>#0 - sets a new name
	#3mi set desc#0 - drops you into an editor to write a description for players
	#3mi set know <prog>#0 - sets the prog that controls if players know about this
	#3mi set impact <category> <supply%> <demand%>#0 - adds or edits an impact for a category
	#3mi set remimpact <category>#0 - removes the impact for a category
	#3mi set applies <date>#0 - the date that this impact applies from
	#3mi set until <date>#0 - the date that this impact applies until
	#3mi set until always#0 - removes the expiry date for this impact
	#3mi set duration <timespan>#0 - an alternative way to set until based on duration";
	[PlayerCommand("MarketInfluence", "marketinfluence", "mi")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("MarketInfluence", MarketInfluenceHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void MarketInfluence(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				MarketInfluenceList(actor, ss);
				return;
			case "new":
			case "create":
				MarketInfluenceNew(actor, ss);
				return;
			case "begin":
				MarketInfluenceBegin(actor, ss);
				return;
			case "end":
				MarketInfluenceEnd(actor, ss);
				return;
			case "clone":
				MarketInfluenceClone(actor, ss);
				return;
			case "set":
				MarketInfluenceSet(actor, ss);
				return;
			case "edit":
				MarketInfluenceEdit(actor, ss);
				return;
			case "close":
				MarketInfluenceClose(actor, ss);
				return;
			case "show":
			case "view":
				MarketInfluenceShow(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(MarketInfluenceHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void MarketInfluenceEnd(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which market influence do you want end?");
			return;
		}

		var influence = actor.Gameworld.MarketInfluences.GetById(ss.SafeRemainingArgument);
		if (influence is null)
		{
			actor.OutputHandler.Send("There is no such market influence.");
			return;
		}

		if (influence.AppliesUntil is not null && influence.AppliesUntil <= influence.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
		{
			actor.OutputHandler.Send("That influence has already ended.");
			return;
		}

		var sb = new StringBuilder();
		sb.Append("Are you sure that you want to end market influence #");
		sb.Append(influence.Id.ToString("N0", actor));
		sb.Append(" (");
		sb.Append(influence.Name.ColourValue());
		sb.AppendLine(")?");
		sb.Append("This influence ");
		var now = influence.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		if (influence.AppliesFrom > now)
		{
			sb.Append("has not yet begun".Colour(Telnet.Yellow));
		}
		else
		{
			sb.Append($"began at {influence.AppliesFrom}".Colour(Telnet.Green));
		}

		sb.Append(" and ");
		if (influence.AppliesUntil is null)
		{
			sb.Append("continues until cancelled".Colour(Telnet.Cyan));
		}
		else
		{
			if (influence.AppliesUntil > now)
			{
				sb.Append("currently applies".Colour(Telnet.Green));
			}
			else
			{
				sb.Append("has already ended".Colour(Telnet.Red));
			}
		}

		sb.AppendLine(".");
		sb.AppendLine(Accept.StandardAcceptPhrasing);
		actor.OutputHandler.Send(sb.ToString());
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "ending a market influence",
			AcceptAction = text =>
			{
				var end = new MudDateTime(influence.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime) - TimeSpan.FromSeconds(1);
				influence.AppliesUntil = end;
				actor.OutputHandler.Send("You end the market influence.");
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to end the market influence.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to end the market influence.");
			},
			Keywords = ["influence", "end"]
		}), TimeSpan.FromSeconds(120));
	}

	private static void MarketInfluenceBegin(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which market do you want to begin an influence in?");
			return;
		}

		var market = actor.Gameworld.Markets.GetByIdOrName(ss.PopSpeech());
		if (market is null)
		{
			actor.OutputHandler.Send("There is no such market.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which template do you want to use for the influence?");
			return;
		}

		var template = actor.Gameworld.MarketInfluenceTemplates.GetByIdOrName(ss.PopSpeech());
		if (template is null)
		{
			actor.OutputHandler.Send("There is no such market influence template.");
			return;
		}

		var begin = market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		if (!ss.IsFinished)
		{
			if (!MudDateTime.TryParse(ss.PopSpeech(), market.EconomicZone.FinancialPeriodReferenceCalendar, market.EconomicZone.FinancialPeriodReferenceClock, out begin))
			{
				actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid date and time.{MudDateTime.TryParseHelpText(actor, market.EconomicZone)}");
				return;
			}
		}

		var end = default(MudDateTime);
		if (!ss.IsFinished)
		{
			if (!MudDateTime.TryParse(ss.PopSpeech(), market.EconomicZone.FinancialPeriodReferenceCalendar, market.EconomicZone.FinancialPeriodReferenceClock, out end))
			{
				actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid date and time.{MudDateTime.TryParseHelpText(actor, market.EconomicZone)}");
				return;
			}
		}

		var influence = new MarketInfluence(market, template, template.Name, begin, end);
		actor.Gameworld.Add(influence);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
		actor.OutputHandler.Send($"You are create a new market influence for the {market.Name.ColourName()} market from the template {template.Name.ColourValue()}, which you are now editing.");
	}

	private static void MarketInfluenceNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which market do you want to create an influence for?");
			return;
		}

		var market = actor.Gameworld.Markets.GetByIdOrName(ss.PopSpeech());
		if (market is null)
		{
			actor.OutputHandler.Send("There is no such market.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What date should this influence apply from?");
			return;
		}

		if (!MudDateTime.TryParse(ss.PopSpeech(), market.EconomicZone.FinancialPeriodReferenceCalendar, market.EconomicZone.FinancialPeriodReferenceClock, out var date))
		{
			actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid date and time.{MudDateTime.TryParseHelpText(actor, market.EconomicZone)}");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name would you like to give to this market influence?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.MarketInfluenceTemplates.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a market influence template called {name.ColourName()}. Names must be unique.");
			return;
		}

		var influence = new MarketInfluence(market, name, "This influence has no detailed description", date, null);
		actor.Gameworld.Add(influence);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
		actor.OutputHandler.Send($"You are create a new market influence for the {market.Name.ColourName()} market called {name.ColourValue()}, which you are now editing.");
	}

	private static void MarketInfluenceClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which market influence do you want to clone?");
			return;
		}

		var old = actor.Gameworld.MarketInfluences.GetByIdOrName(ss.SafeRemainingArgument);
		if (old is null)
		{
			actor.OutputHandler.Send("There is no market influence like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new market influence?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();

		var influence = old.Clone(name);
		actor.Gameworld.Add(influence);
		influence.Market.ApplyMarketInfluence(influence);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
		actor.OutputHandler.Send($"You are clone market influence {old.Name.ColourValue()} to a new market influence called {influence.Name.ColourName()}, which you are now editing.");
	}

	private static void MarketInfluenceSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any market influences.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void MarketInfluenceEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market influence would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var influence = actor.Gameworld.MarketInfluences.GetByIdOrName(ss.SafeRemainingArgument);
		if (influence is null)
		{
			actor.OutputHandler.Send("There is no market influence like that.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketInfluence>(actor) { EditingItem = influence });
		actor.OutputHandler.Send($"You are now editing the {influence.Name.ColourName()} market influence.");
	}

	private static void MarketInfluenceClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any market influences.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarketInfluence>>();
		actor.OutputHandler.Send("You are no longer editing any market influences.");
	}

	private static void MarketInfluenceShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketInfluence>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market influence would you like to show?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var category = actor.Gameworld.MarketInfluences.GetByIdOrName(ss.SafeRemainingArgument);
		if (category is null)
		{
			actor.OutputHandler.Send("There is no market influences like that.");
			return;
		}

		actor.OutputHandler.Send(category.Show(actor));
	}

	private static void MarketInfluenceList(ICharacter actor, StringStack ss)
	{
		var influences = actor.Gameworld.MarketInfluences.ToList();
		// TODO - filters
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in influences
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.CharacterKnowsAboutInfluenceProg.MXPClickableFunctionName(),
				item.Market.Name,
				item.MarketInfluenceTemplate?.Name ?? "",
				item.AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				item.AppliesUntil?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short) ?? "Until Removed",
				item.Applies(null, item.Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime).ToColouredString()
			},
			new List<string>
			{
				"Id",
				"Name",
				"Prog",
				"Market",
				"Template",
				"From",
				"Until",
				"Active"
			},
			actor,
			Telnet.Cyan,
			3
		));
	}
	#endregion

	#region Market Categories
	public const string MarketCategoryHelpText = @"This command allows you to edit market categories. Market categories are groupings of goods or services that have the same price multipliers in a market. 

These categories can be broad or specific, for example, you could have ""Food"" as a category or separate ""Luxury Food"", ""Staple Foods"", etc.

Not every market needs to have every category, but categories themselves can be shared between different markets.

The syntax for working with categories is as follows:

	#3mc list#0 - shows all market categories
	#3mc show <id>#0 - shows a particular market category
	#3mc edit <id>#0 - begins editing a market category
	#3mc edit#0 - an alias for #3mc show <editing id>#0
	#3mc close#0 - stops editing a market category
	#3mc clone <name>#0 - clones an existing market category and then begins editing the clone
	#3mc new <tag> <name>#0 - creates a new market category with a specified default item tag
	#3mc set name <name>#0 - changes the name
	#3mc set eover <%>#0 - changes the elasticity for oversupply
	#3mc set eunder <%>#0 - changes the elasticity for undersupply
	#3mc set desc#0 - drops you into an editor to set the description
	#3mc set tag <tag>#0 - toggles an item tag as being a part of this category";

	[PlayerCommand("MarketCategory", "marketcategory", "mc")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void MarketCategory(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				MarketCategoryList(actor, ss);
				return;
			case "new":
			case "create":
				MarketCategoryNew(actor, ss);
				return;
			case "clone":
				MarketCategoryClone(actor, ss);
				return;
			case "set":
				MarketCategorySet(actor, ss);
				return;
			case "edit":
				MarketCategoryEdit(actor, ss);
				return;
			case "close":
				MarketCategoryClose(actor, ss);
				return;
			case "show":
			case "view":
				MarketCategoryShow(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(MarketCategoryHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void MarketCategoryNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What item tag would you like this market category to apply to?");
			return;
		}

		var tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
		if (tag is null)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name would you like to give to this market category?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.MarketCategories.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a market category called {name.ColourName()}. Names must be unique.");
			return;
		}

		var category = new MarketCategory(actor.Gameworld, name, tag);
		actor.Gameworld.Add(category);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketCategory>(actor) { EditingItem = category });
		actor.OutputHandler.Send($"You are create a new market category called {name.ColourValue()} that applies to the tag {tag.FullName.ColourName()}, which you are now editing.");
	}

	private static void MarketCategoryClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which market category do you want to clone?");
			return;
		}

		var old = actor.Gameworld.MarketCategories.GetByIdOrName(ss.SafeRemainingArgument);
		if (old is null)
		{
			actor.OutputHandler.Send("There is no market category like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new market category?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.MarketCategories.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a market category called {name.ColourName()}. Names must be unique.");
			return;
		}

		var category = old.Clone(name);
		actor.Gameworld.Add(category);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketCategory>(actor) { EditingItem = category });
		actor.OutputHandler.Send($"You are clone market category {old.Name.ColourValue()} to a new market category called {category.Name.ColourName()}, which you are now editing.");
	}

	private static void MarketCategorySet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any market categories.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void MarketCategoryEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market category would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var category = actor.Gameworld.MarketCategories.GetByIdOrName(ss.SafeRemainingArgument);
		if (category is null)
		{
			actor.OutputHandler.Send("There is no market category like that.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
		actor.AddEffect(new BuilderEditingEffect<IMarketCategory>(actor) { EditingItem = category });
		actor.OutputHandler.Send($"You are now editing the {category.Name.ColourName()} market category.");
	}

	private static void MarketCategoryClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any market categories.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarketCategory>>();
		actor.OutputHandler.Send("You are no longer editing any market categories.");
	}

	private static void MarketCategoryShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarketCategory>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market category would you like to show?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var category = actor.Gameworld.MarketCategories.GetByIdOrName(ss.SafeRemainingArgument);
		if (category is null)
		{
			actor.OutputHandler.Send("There is no market category like that.");
			return;
		}

		actor.OutputHandler.Send(category.Show(actor));
	}

	private static void MarketCategoryList(ICharacter actor, StringStack ss)
	{
		var categories = actor.Gameworld.MarketCategories.ToList();
		// TODO - filters
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in categories
			let prices = actor.Gameworld.Markets.Where(x => x.MarketCategories.Contains(item))
			                  .Select(x => x.PriceMultiplierForCategory(item))
			                  .DefaultIfEmpty(1.0M)
			                  .ToList()
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.ElasticityFactorBelow.ToString("N3", actor),
				item.ElasticityFactorAbove.ToString("N3", actor),
				actor.Gameworld.Markets.Count(x => x.MarketCategories.Contains(item)).ToString("N0", actor),
				prices.Min().ToString("P2", actor),
				prices.Average().ToString("P2", actor),
				prices.Max().ToString("P2", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"E(Under)",
				"E(Over)",
				"# Markets",
				"Min Price",
				"Avg Price",
				"Max Price"
			},
			actor,
			Telnet.Cyan
		));
	}

	#endregion

	#region Markets
	public const string MarketHelpText = @"This command allows you to create and edit markets, which can be used to control prices of various goods in way that responds to supply and demand changes.

There are several related commands, #3marketcategory#0, #3marketinfluencetemplate#0 and #3markettemplate#0.

The syntax for this command is as follows:

	#3market list#0 - shows all markets
	#3market show <id>#0 - shows a particular market
	#3market edit <id>#0 - begins editing a market
	#3market edit#0 - an alias for #3market show <editing id>#0
	#3market close#0 - stops editing a market
	#3market clone <name>#0 - clones an existing market and then begins editing the clone
	#3market new <ez> <name>#0 - creates a new market in an economic zone
	#3market set name <name>#0 - changes the name
	#3market set ez <zone>#0 - changes the economic zone
	#3market set category <which>#0 - toggles a category as being part of the market
	#3market set desc#0 - drops you into an editor for the market's description
	#3market set formula <formula>#0 - edits the market's price formula";

	[PlayerCommand("Market", "market")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Market(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				MarketList(actor, ss);
				return;
			case "new":
			case "create":
				MarketNew(actor, ss);
				return;
			case "clone":
				MarketClone(actor, ss);
				return;
			case "set":
				MarketSet(actor, ss);
				return;
			case "edit":
				MarketEdit(actor, ss);
				return;
			case "close":
				MarketClose(actor, ss);
				return;
			case "show":
			case "view":
				MarketShow(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(MarketHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void MarketClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which market do you want to clone?");
			return;
		}

		var old = actor.Gameworld.Markets.GetByIdOrName(ss.SafeRemainingArgument);
		if (old is null)
		{
			actor.OutputHandler.Send("There is no market like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new market?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Markets.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a market called {name.ColourName()}. Names must be unique.");
			return;
		}

		var market = old.Clone(name);
		actor.Gameworld.Add(market);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
		actor.AddEffect(new BuilderEditingEffect<IMarket>(actor) { EditingItem = market });
		actor.OutputHandler.Send($"You are clone market {old.Name.ColourValue()} to a new market called {market.Name.ColourName()}, which you are now editing.");
	}

	private static void MarketShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market would you like to show?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var market = actor.Gameworld.Markets.GetByIdOrName(ss.SafeRemainingArgument);
		if (market is null)
		{
			actor.OutputHandler.Send("There is no market like that.");
			return;
		}

		actor.OutputHandler.Send(market.Show(actor));
	}

	private static void MarketList(ICharacter actor, StringStack ss)
	{
		var markets = actor.Gameworld.Markets.ToList();
		// TODO - filters
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in markets select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.EconomicZone.Name,
			},
			new List<string>
			{
				"ID",
				"Name",
				"Economic Zone"
			},
			actor,
			Telnet.Yellow
		));
	}

	private static void MarketNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to create a market in?");
			return;
		}

		var ez = actor.Gameworld.EconomicZones.GetByIdOrName(ss.PopSpeech());
		if (ez is null)
		{
			actor.OutputHandler.Send("There is no such economic zones.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new market?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.Markets.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a market called {name.ColourName()}. Names must be unique.");
			return;
		}

		var market = new Market(actor.Gameworld, name, ez);
		actor.Gameworld.Add(market);
		actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
		actor.AddEffect(new BuilderEditingEffect<IMarket>(actor) { EditingItem = market });
		actor.OutputHandler.Send($"You are create a new market in the {ez.Name.ColourValue()} economic zone called {market.Name.ColourName()}, which you are now editing.");
	}

	private static void MarketSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any markets.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void MarketEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which market would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var market = actor.Gameworld.Markets.GetByIdOrName(ss.SafeRemainingArgument);
		if (market is null)
		{
			actor.OutputHandler.Send("There is no market like that.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
		actor.AddEffect(new BuilderEditingEffect<IMarket>(actor) { EditingItem = market });
		actor.OutputHandler.Send($"You are now editing the {market.Name.ColourName()} market.");
	}

	private static void MarketClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IMarket>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any markets.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IMarket>>();
		actor.OutputHandler.Send("You are no longer editing any markets.");
	}
	#endregion Markets
	#endregion
}
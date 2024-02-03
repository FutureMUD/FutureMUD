using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.ThirdPartyCode;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

namespace MudSharp.Economy.Currency;

public class CurrencyDivision : SaveableItem, ICurrencyDivision
{
	public ICurrency Currency { get; }

	public CurrencyDivision(IFuturemud gameworld, MudSharp.Models.CurrencyDivision division, ICurrency currency)
	{
		Gameworld = gameworld;
		Currency = currency;
		_id = division.Id;
		_name = division.Name;
		IgnoreCase = division.IgnoreCase;
		BaseUnitConversionRate = division.BaseUnitConversionRate;
		_patterns =
			division.CurrencyDivisionAbbreviations.Select(x =>
				        new Regex(x.Pattern, division.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None))
			        .ToList();
	}

	public CurrencyDivision(IFuturemud gameworld, string name, decimal conversion, ICurrency currency)
	{
		Gameworld = gameworld;
		_name = name;
		Currency = currency;
		BaseUnitConversionRate = conversion;
		IgnoreCase = true;
		_patterns = new() { new Regex($"(-?\\d+(?:\\.\\d+)*)(?:\\s*(?:{name.Pluralise()}|{name}|{name[0]}))$", RegexOptions.IgnoreCase) };
		using (new FMDB())
		{
			var dbitem = new Models.CurrencyDivision
			{
				CurrencyId = currency.Id,
				BaseUnitConversionRate = BaseUnitConversionRate,
				Name = Name,
				IgnoreCase = IgnoreCase,
			};
			FMDB.Context.CurrencyDivisions.Add(dbitem);
			dbitem.CurrencyDivisionAbbreviations.Add(new Models.CurrencyDivisionAbbreviation
			{
				CurrencyDivision = dbitem,
				Pattern = _patterns[0].ToString()
			});
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.CurrencyDivisions.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.CurrencyDivisions.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public override string FrameworkItemType => "CurrencyDivision";

	#region ICurrencyDivision Members

	public decimal BaseUnitConversionRate { get; private set; }

	private readonly List<Regex> _patterns;
	public IEnumerable<Regex> Patterns => _patterns;

	public bool IgnoreCase { get; private set; }

	public const string HelpText = @"You can use the following options to edit currency divisions:

	#3name <name>#0 - sets a new name
	#3base <amount>#0 - sets the amount of base currency this division is worth
	#3ignorecase#0 - toggles ignoring case in the regular expression patterns
	#3addabbr <regex>#0 - adds a regular expression pattern for this division
	#3remabbr <##>#0 - removes a particular pattern abbreviation
	#3abbr <##> <regex>#0 - overwrites the regular expression pattern at the specified index

Note, the first capture group in the regular expression must be the number. If your regex isn't strong,
you can use the below example and modify it for your division:

#6(-?\d+(?:\.\d+)*)(?:\s*(?:bits|bit|b))$#0

In most cases you'd only need to edit the #6bits|bit|b#0 portion of the above to match any names or abbreviations.";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "base":
				return BuildingCommandBase(actor, command);
			case "ignorecase":
			case "case":
				return BuildingCommandIgnoreCase(actor);
			case "addabbrev":
			case "addabbreviation":
			case "addabbr":
				return BuildingCommandAddAbbreviation(actor, command);
			case "abbreviation":
			case "abbr":
			case "abbrev":
				return BuildingCommandAbbreviation(actor, command);
			case "remabbreviation":
			case "removeabbreviation":
			case "remabbr":
			case "remabbrev":
			case "removeabbr":
			case "removeabbrev":
				return BuildingCommandRemoveAbbreviation(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandAddAbbreviation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What regular expression did you want to use for this pattern?");
			return false;
		}

		var abbrev = command.SafeRemainingArgument;
		try
		{
			var regex = new Regex(abbrev, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
			_patterns.Add(regex);
		}
		catch (Exception e)
		{
			actor.OutputHandler.Send("The regex you specified was not valid.");
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"You add the following pattern for this currency division: {abbrev.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandRemoveAbbreviation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which abbreviation did you want to remove?");
			return false;
		}

		if (_patterns.Count <= 1)
		{
			actor.OutputHandler.Send("All currency divisions must have at least one abbreviation pattern. Create a new one first before you delete the last one.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 || value > _patterns.Count)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid index. You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_patterns.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var pattern = _patterns[value - 1];
		_patterns.RemoveAt(value - 1);
		Changed = true;
		actor.OutputHandler.Send($"You remove the {value.ToOrdinal().ColourValue()} pattern for this currency division; {pattern.ToString().ColourCommand()}");
		return true;
	}

	private bool BuildingCommandAbbreviation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which abbreviation do you want to edit?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1 || value > _patterns.Count)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid index. You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_patterns.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which regular expression pattern did you want to set for that pattern?");
			return false;
		}

		var abbrev = command.SafeRemainingArgument;
		try
		{
			var regex = new Regex(abbrev, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
			_patterns[value - 1] = regex;
		}
		catch (Exception e)
		{
			actor.OutputHandler.Send("The regex you specified was not valid.");
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"You set the {value.ToOrdinal().ColourValue()} pattern for this currency division to the following: {abbrev.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandIgnoreCase(ICharacter actor)
	{
		IgnoreCase = !IgnoreCase;
		var patterns = _patterns.ToList();
		_patterns.Clear();
		foreach (var pattern in patterns)
		{
			_patterns.Add(new Regex(pattern.ToString(), IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
		}
		Changed = true;
		actor.OutputHandler.Send($"This currency division will {IgnoreCase.NowNoLonger()} ignore case when considering pattern matches.");
		return true;
	}

	private bool BuildingCommandBase(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !decimal.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0M)
		{
			actor.OutputHandler.Send("You must enter a valid conversion between this currency division and the base division.");
			return false;
		}

		BaseUnitConversionRate = value;
		Changed = true;
		actor.OutputHandler.Send($"This currency division is now worth {value.ToString("N3", actor).ColourValue()} base currency units.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this currency division?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant();
		if (Currency.CurrencyDivisions.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a currency division for this currency with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename this currency division from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Currency Division #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.White));
		sb.AppendLine();
		sb.AppendLine($"Base Units: {BaseUnitConversionRate.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Ignore Case: {IgnoreCase.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Patterns:");
		var i = 1;
		foreach (var pattern in _patterns )
		{
			sb.AppendLine();
			sb.AppendLine($"\t{i++.ToString("N0", actor)}) {pattern.ToString().ColourCommand()}");
		}
		return sb.ToString();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.CurrencyDivisions.Find(Id);
		dbitem.Name = Name;
		dbitem.BaseUnitConversionRate = BaseUnitConversionRate;
		dbitem.IgnoreCase = IgnoreCase;
		FMDB.Context.CurrencyDivisionAbbreviations.RemoveRange(dbitem.CurrencyDivisionAbbreviations);
		foreach (var pattern in _patterns)
		{
			dbitem.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
			{
				CurrencyDivision = dbitem,
				Pattern = pattern.ToString()
			});
		}
		Changed = false;
	}

	#endregion
}
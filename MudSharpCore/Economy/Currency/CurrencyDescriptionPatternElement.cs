using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Functions.Mathematical;
using MudSharp.Models;

namespace MudSharp.Economy.Currency;

public class CurrencyDescriptionPatternElement : SaveableItem, ICurrencyDescriptionPatternElement
{
	private readonly CurrencyDescriptionPattern _parent;
	private readonly Dictionary<decimal, string> _specialValues = new();

	public CurrencyDescriptionPatternElement(MudSharp.Models.CurrencyDescriptionPatternElement element,
		CurrencyDescriptionPattern parentPattern, ICurrency parent)
	{
		Gameworld = parent.Gameworld;
		_id = element.Id;
		Order = element.Order;
		Pattern = element.Pattern;
		AlternatePattern = element.AlternatePattern;
		PluraliseWord = element.PluraliseWord;
		_parent = parentPattern;
		Rounding = (RoundingMode)element.RoundingMode;
		TargetDivision = parent.CurrencyDivisions.First(x => x.Id == element.CurrencyDivisionId);
		SpecialValuesOverridePattern = element.SpecialValuesOverrideFormat;
		ShowIfZero = element.ShowIfZero;
		foreach (var item in element.CurrencyDescriptionPatternElementSpecialValues)
		{
			_specialValues.Add(item.Value, item.Text);
		}
	}

	public CurrencyDescriptionPatternElement(CurrencyDescriptionPattern parentPattern, string pattern,
		string plural, ICurrencyDivision targetDivision)
	{
		Gameworld = parentPattern.Gameworld;
		_parent = parentPattern;
		TargetDivision = targetDivision;
		Pattern = pattern;
		AlternatePattern = string.Empty;
		PluraliseWord = plural;
		Rounding = RoundingMode.Round;
		SpecialValuesOverridePattern = false;
		ShowIfZero = false;
		Order = parentPattern.Elements.Count() + 1;
		using (new FMDB())
		{
			var dbitem = new Models.CurrencyDescriptionPatternElement
			{
				Order = Order,
				ShowIfZero = ShowIfZero,
				CurrencyDivisionId = TargetDivision.Id,
				CurrencyDescriptionPatternId = parentPattern.Id,
				PluraliseWord = PluraliseWord,
				AlternatePattern = AlternatePattern,
				RoundingMode = (int)Rounding,
				SpecialValuesOverrideFormat = SpecialValuesOverridePattern,
			};
			FMDB.Context.CurrencyDescriptionPatternElements.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "CurrencyDescriptionPatternElement";

	public override string ToString()
	{
		return
			$"CurrencyDescriptionPatternElement [#{Id}]: pattern: \"{Pattern}\" alternate: \"{AlternatePattern}\" plural: \"{PluraliseWord}\"";
	}

	#region ICurrencyDescriptionPatternElement Members

	public RoundingMode Rounding { get; private set; }

	public string Describe(decimal amount)
	{
		var originalAmount = amount;
		switch (Rounding)
		{
			case RoundingMode.Round:
				amount = Math.Round(amount);
				break;
			case RoundingMode.Truncate:
				amount = (int)amount;
				break;
		}

		var remainder = amount != originalAmount;

		var pattern = remainder || string.IsNullOrEmpty(AlternatePattern) ? Pattern : AlternatePattern;
		string value;
		if (_specialValues.ContainsKey(amount))
		{
			if (SpecialValuesOverridePattern)
			{
				return _specialValues[amount];
			}

			value = string.Format(pattern, _specialValues[amount]);
		}
		else if (_parent.Type == CurrencyDescriptionPatternType.Casual ||
		         _parent.Type == CurrencyDescriptionPatternType.Wordy)
		{
			value = string.Format(pattern, ((int)amount).ToWordyNumber());
		}
		else
		{
			value = string.Format(pattern, amount);
		}

		if (!string.IsNullOrEmpty(PluraliseWord) &&
		    (amount >= 2.0M || amount <= -2.0M || (amount > -1.0M && amount < 1.0M)))
		{
			return value.Replace(PluraliseWord, PluraliseWord.Pluralise());
		}

		return value;
	}

	public ICurrencyDivision TargetDivision { get; private set; }

	public bool ShowIfZero { get; private set; }
	private int _order;

	public int Order
	{
		get => _order;
		set
		{
			_order = value;
			Changed = true;
		}
	}

	public string Pattern { get; private set; }

	public string PluraliseWord { get; private set; }

	public bool SpecialValuesOverridePattern { get; private set; }

	public string AlternatePattern { get; private set; }

	public IReadOnlyDictionary<decimal, string> SpecialValues => _specialValues.AsReadOnly();
	#endregion

	public override void Save()
	{
		var dbitem = FMDB.Context.CurrencyDescriptionPatternElements.Find(Id);
		dbitem.Order = Order;
		dbitem.Pattern = Pattern;
		dbitem.AlternatePattern = AlternatePattern;
		dbitem.PluraliseWord = PluraliseWord;
		dbitem.RoundingMode = (int)Rounding;
		dbitem.CurrencyDivisionId = TargetDivision.Id;
		dbitem.SpecialValuesOverrideFormat = SpecialValuesOverridePattern;
		dbitem.ShowIfZero = ShowIfZero;
		FMDB.Context.CurrencyDescriptionPatternElementSpecialValues.RemoveRange(dbitem
			.CurrencyDescriptionPatternElementSpecialValues);
		foreach (var item in SpecialValues)
		{
			dbitem.CurrencyDescriptionPatternElementSpecialValues.Add(new CurrencyDescriptionPatternElementSpecialValues
			{
				CurrencyDescriptionPatternElement = dbitem,
				Text = item.Value,
				Value = item.Key
			});
		}
		Changed = false;
	}

	public const string HelpText = @"You can use the following options with this building command:

	#3zero#0 - toggles showing this element if it is zero
	#3specials#0 - toggles special values totally overriding the pattern instead of just the value part
	#3order <##>#0 - changes the order this element appears in the list of its pattern
	#3pattern <pattern>#0 - sets the pattern for the element. Use #3{0}#0 for the numerical value.
	#3last <pattern>#0 - sets an alternate pattern if this is the last element in the display. Use #3{0}#0 for the numerical value.
	#3last none#0 - clears the last alternative pattern
	#3plural <word>#0 - sets the word in the pattern that should be used for pluralisation
	#3rounding <truncate|round|noround>#0 - changes the rounding mode for this element
	#3addspecial <value> <text>#0 - adds or sets a special value
	#3remspecial <value>#0 - removes a special value";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "order":
				return BuildingCommandOrder(actor, command);
			case "zero":
				return BuildingCommandZero(actor);
			case "specials":
			case "specialsoverride":
			case "override":
				return BuildingCommandSpecialsOverride(actor);
			case "plural":
			case "pluralword":
				return BuildingCommandPluralWord(actor, command);
			case "pattern":
				return BuildingCommandPattern(actor, command);
			case "last":
			case "lastpattern":
			case "alternate":
			case "alternative":
			case "alternatepattern":
			case "alternativepattern":
				return BuildingCommandAlternativePattern(actor, command);
			case "round":
			case "rounding":
				return BuildingCommandRounding(actor, command);
			case "addspecial":
				return BuildingCommandAddSpecial(actor, command);
			case "removespecial":
			case "remspecial":
				return BuildingCommandRemoveSpecial(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandAddSpecial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !decimal.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number value for the special value.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a text override for the special value.");
			return false;
		}

		_specialValues[value] = command.GetSafeRemainingArgument(false);
		Changed = true;
		actor.OutputHandler.Send($"The value {value.ToString("N3", actor).ColourValue()} will now be replaced with the text {_specialValues[value].ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandRemoveSpecial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !decimal.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number value for the special value.");
			return false;
		}

		if (!_specialValues.ContainsKey(value))
		{
			actor.OutputHandler.Send("There is no such special value to remove.");
			return false;
		}

		_specialValues.Remove(value);
		Changed = true;
		actor.OutputHandler.Send($"You delete the special value associated with the {value.ToString("N3", actor).ColourValue()} value.");
		return true;
	}

	private bool BuildingCommandRounding(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which rounding type do you want to use? The valid options are {Enum.GetValues<RoundingMode>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out RoundingMode value))
		{
			actor.OutputHandler.Send($"That is not a valid rounding mode. The valid options are {Enum.GetValues<RoundingMode>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		Rounding = value;
		Changed = true;
		switch (value)
		{
			case RoundingMode.Truncate:
				actor.OutputHandler.Send("Currency values for this element will now truncate (i.e. drop decimals and just show integer values).");
				break;
			case RoundingMode.Round:
				actor.OutputHandler.Send("Currency values for this element will now be rounded using normal rounding rules (<0.5 down, >=0.5 up etc).");
				break;
			case RoundingMode.NoRounding:
				actor.OutputHandler.Send("Currency values for this element will no longer be rounded, and will show as decimals.");
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		return true;
	}

	private bool BuildingCommandAlternativePattern(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What is the format you want to use for the alternate pattern when this is the last element? Alternately you can specify {"none".ColourCommand()} to have no alternate pattern. Use {"{0}".ColourCommand()} as a token for the numerical value.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			AlternatePattern = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This element will no longer have an alternate pattern when it is the last element.");
			return true;
		}

		var pattern = command.SafeRemainingArgument;
		if (!pattern.IsValidFormatString(new[] { true }))
		{
			actor.OutputHandler.Send($"The text {pattern.ColourCommand()} is not a valid format string. Hint: Use {"{0}".ColourCommand()} as a token for the numerical value.");
			return false;
		}

		AlternatePattern = pattern;
		Changed = true;
		actor.OutputHandler.Send($"You set the alternate final element pattern to {pattern.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPattern(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What is the format you want to use for the pattern? Use {"{0}".ColourCommand()} as a token for the numerical value.");
			return false;
		}

		var pattern = command.SafeRemainingArgument;
		if (!pattern.IsValidFormatString(new[]{ true}))
		{
			actor.OutputHandler.Send($"The text {pattern.ColourCommand()} is not a valid format string. Hint: Use {"{0}".ColourCommand()} as a token for the numerical value.");
			return false;
		}

		Pattern = pattern;
		Changed = true;
		actor.OutputHandler.Send($"You set the element pattern to {pattern.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPluralWord(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which word in this element is pluralisation done on?");
			return false;
		}

		PluraliseWord = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"This pattern will now pluralise on the word {PluraliseWord.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSpecialsOverride(ICharacter actor)
	{
		SpecialValuesOverridePattern = !SpecialValuesOverridePattern;
		Changed = true;
		actor.OutputHandler.Send($"This pattern will {SpecialValuesOverridePattern.NowNoLonger()} allow special values to override the pattern.");
		return true;
	}

	private bool BuildingCommandZero(ICharacter actor)
	{
		ShowIfZero = !ShowIfZero;
		Changed = true;
		actor.OutputHandler.Send($"This pattern will {ShowIfZero.NowNoLonger()} show if the value is zero for its target division.");
		return true;
	}

	private bool BuildingCommandOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What order should this element be evaluated in?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 || value > _parent.Elements.Count())
		{
			actor.OutputHandler.Send($"You must enter a valid number between {1.ToString("N0", actor).ColourValue()} and {_parent.Elements.Count().ToString("N0", actor).ColourValue()}.");
			return false;
		}

		if (Order == value)
		{
			actor.OutputHandler.Send($"This element is already the {value.ToOrdinal().ColourValue()} one evaluated.");
			return false;
		}

		_parent.ReorderElement(this, value-1);
		Changed = true;
		actor.OutputHandler.Send(
			$"You reorder this element to be the {value.ToOrdinal().ColourValue()} one evaluated.");
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Currency Description Pattern Element #{Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine($"Currency: {_parent.Currency.Name.ColourValue()}");
		sb.AppendLine($"Pattern: #{_parent.Id.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Target Division: {TargetDivision.Name.ColourValue()}");
		sb.AppendLine($"Order: {Order.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Show If Zero: {ShowIfZero.ToColouredString()}");
		sb.AppendLine($"Pluralise Word: {PluraliseWord.ColourCommand()}");
		sb.AppendLine($"Specials Override: {SpecialValuesOverridePattern.ToColouredString()}");
		sb.AppendLine($"Pattern: {Pattern.ColourCommand()}");
		sb.AppendLine($"Alternate If Last: {AlternatePattern?.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Special Values:");
		sb.AppendLine();
		if (SpecialValues.Count == 0)
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			foreach (var (value, special) in SpecialValues)
			{
				sb.AppendLine($"\t{value.ToString("N3", actor).ColourValue()} = {special.ColourCommand()}");
			}
		}
		return sb.ToString();
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.CurrencyDescriptionPatternElements.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.CurrencyDescriptionPatternElements.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}
}
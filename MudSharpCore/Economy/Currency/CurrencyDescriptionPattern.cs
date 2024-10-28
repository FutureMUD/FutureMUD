using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using Org.BouncyCastle.Asn1.Pkcs;

namespace MudSharp.Economy.Currency;

public class CurrencyDescriptionPattern : SaveableItem, ICurrencyDescriptionPattern
{
	public ICurrency Currency { get; }

	private readonly List<ICurrencyDescriptionPatternElement> _elements =
		new();

	public IEnumerable<ICurrencyDescriptionPatternElement> Elements => _elements;

	public string NegativeValuePrefix { get; private set; }

	public CurrencyDescriptionPattern(MudSharp.Models.CurrencyDescriptionPattern pattern, ICurrency parent,
		CurrencyDescriptionPatternType type)
	{
		Gameworld = parent.Gameworld;
		Currency = parent;
		_id = pattern.Id;
		Type = type;
		Order = pattern.Order;
		UseNaturalAggregationStyle = pattern.UseNaturalAggregationStyle;
		ApplicabilityProg = pattern.FutureProgId.HasValue
			? Gameworld.FutureProgs.Get(pattern.FutureProgId.Value)
			: null;
		NegativeValuePrefix = pattern.NegativePrefix;
		foreach (var item in pattern.CurrencyDescriptionPatternElements.OrderBy(x => x.Order))
		{
			_elements.Add(new CurrencyDescriptionPatternElement(item, this, parent));
		}
	}

	public CurrencyDescriptionPattern(ICurrency parent, CurrencyDescriptionPatternType type)
	{
		Gameworld = parent.Gameworld;
		Currency = parent;
		Type = type;
		UseNaturalAggregationStyle = false;
		ApplicabilityProg = Gameworld.AlwaysTrueProg;
		switch (type)
		{

			case CurrencyDescriptionPatternType.Short:
			case CurrencyDescriptionPatternType.ShortDecimal:
				NegativeValuePrefix = "-";
				break;
			case CurrencyDescriptionPatternType.Casual:
			case CurrencyDescriptionPatternType.Long:
			case CurrencyDescriptionPatternType.Wordy:
				NegativeValuePrefix = "negative ";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}

		using (new FMDB())
		{
			var dbitem = new Models.CurrencyDescriptionPattern
			{ 
				Type = (int)Type,
				CurrencyId = Currency.Id,
				FutureProgId = ApplicabilityProg.Id,
				NegativePrefix = NegativeValuePrefix,
				Order = parent.PatternDictionary[type].Count + 1,
				UseNaturalAggregationStyle = UseNaturalAggregationStyle,
			};
			FMDB.Context.CurrencyDescriptionPatterns.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		switch (type)
		{

			case CurrencyDescriptionPatternType.Short:
			case CurrencyDescriptionPatternType.ShortDecimal:
				foreach (var division in parent.CurrencyDivisions)
				{
					_elements.Add(new CurrencyDescriptionPatternElement(this, $"{{0}}{division.Name[0]}", "", division));
				}
				break;
			case CurrencyDescriptionPatternType.Casual:
			case CurrencyDescriptionPatternType.Long:
			case CurrencyDescriptionPatternType.Wordy:
				foreach (var division in parent.CurrencyDivisions)
				{
					_elements.Add(new CurrencyDescriptionPatternElement(this, $"{{0}} {division.Name}", division.Name, division));
				}
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.CurrencyDescriptionPatterns.Find(Id);
		dbitem.Type = (int)Type;
		dbitem.Order = Order;
		dbitem.UseNaturalAggregationStyle = UseNaturalAggregationStyle;
		dbitem.FutureProgId = ApplicabilityProg?.Id;
		dbitem.NegativePrefix = NegativeValuePrefix;
		Changed = false;
		// TODO
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.CurrencyDescriptionPatterns.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.CurrencyDescriptionPatterns.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void DivisionDeleted(ICurrencyDivision division)
	{
		var remove = _elements.Where(x => x.TargetDivision == division).ToList();
		foreach (var item in remove)
		{
			item.Delete();
			_elements.Remove(item);
		}
	}

	public void ReorderElement(ICurrencyDescriptionPatternElement element, int targetIndex)
	{
		var list = new List<ICurrencyDescriptionPatternElement>();
		var i = 0;
		foreach (var item in _elements)
		{
			if (item == element && targetIndex != i)
			{
				continue;
			}

			if (targetIndex == i)
			{
				list.Add(element);
				element.Order = i + 1;
				i++;
			}

			list.Add(item);
			item.Order = i + 1;
			i++;
		}

		if (!list.Contains(element))
		{
			list.Add(element);
			element.Order = i + 2;
		}

		_elements.Clear();
		_elements.AddRange(list);
		Changed = true;
	}

	public override string FrameworkItemType => "CurrencyDescriptionPattern";

	public override string ToString()
	{
		return
			$"CurrencyDescriptionPattern [#{Id}]: {Enum.GetName(typeof(CurrencyDescriptionPatternType), Type)} ({_elements.Count} elements)";
	}

	#region ICurrencyDescriptionPattern Members

	public CurrencyDescriptionPatternType Type { get; private set; }

	public int Order { get; set; }

	/// <summary>
	///     A FutureProg returning Boolean that accepts a Single Number parameter
	/// </summary>
	public IFutureProg ApplicabilityProg { get; private set; }

	/// <summary>
	/// If true, use ListToString instead of simple concatenation (e.g. use "and" to link last two elements)
	/// </summary>
	public bool UseNaturalAggregationStyle { get; private set; }

	public string Describe(decimal value)
	{
		var outList = new StringBuilder();
		if (value < 0.0M)
		{
			outList.Append(NegativeValuePrefix);
			value = Math.Abs(value);
		}

		var elements = new List<string>();
		foreach (var item in _elements.OrderBy(x => x.Order))
		{
			if (value / item.TargetDivision.BaseUnitConversionRate >= 1.0M || item == _elements.Last() ||
			    item.ShowIfZero)
			{
				var ivalue = value / item.TargetDivision.BaseUnitConversionRate;
				value %= item.TargetDivision.BaseUnitConversionRate;

				if ((ivalue != 0 && (item.Rounding != RoundingMode.Truncate || (int)ivalue != 0)) ||
				    item.ShowIfZero)
				{
					elements.Add(item.Describe(ivalue));
				}
			}
		}

		if (UseNaturalAggregationStyle)
		{
			outList.Append(elements.ListToString());
		}
		else
		{
			foreach (var item in elements)
			{
				outList.Append(item);
			}
		}

		return outList.ToString().NormaliseSpacing(true).Trim();
	}

	public const string HelpText = @"You can use the following options with this command:

	#3order <##>#0 - changes the order in which this pattern is evaluated for applicability
	#3prog <which>#0 - sets the prog that controls applicability for this pattern
	#3negative <prefix>#0 - sets a prefix applied to negative values for this pattern (e.g. #2-#0 or #2negative #0.) Be sure to include spaces if necessary
	#3natural#0 - toggles natural aggregation style for pattern elements (commas plus ""and"") rather than just concatenation
	#3addelement <division> <plural> <pattern>#0 - adds a new pattern element
	#3remelement <id|##>#0 - deletes an element.
	#3element <id|##order> zero#0 - toggles showing this element if it is zero
	#3element <id|##order> specials#0 - toggles special values totally overriding the pattern instead of just the value part
	#3element <id|##order> order <##>#0 - changes the order this element appears in the list of its pattern
	#3element <id|##order> pattern <pattern>#0 - sets the pattern for the element. Use #3{0}#0 for the numerical value.
	#3element <id|##order> last <pattern>#0 - sets an alternate pattern if this is the last element in the display. Use #3{0}#0 for the numerical value.
	#3element <id|##order> last none#0 - clears the last alternative pattern
	#3element <id|##order> plural <word>#0 - sets the word in the pattern that should be used for pluralisation
	#3element <id|##order> rounding <truncate|round|noround>#0 - changes the rounding mode for this element
	#3element <id|##order> addspecial <value> <text>#0 - adds or sets a special value
	#3element <id|##order> remspecial <value>#0 - removes a special value";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "order":
				return BuildingCommandOrder(actor, command);
			case "natural":
				return BuildingCommandNatural(actor, command);
			case "negative":
			case "prefix":
			case "negativeprefix":
				return BuildingCommandNegativePrefix(actor, command);
			case "prog":
				return BuildingCommandApplicabilityProg(actor, command);
			case "element":
				return BuildingCommandElement(actor, command);
			case "addelement":
				return BuildingCommandAddElement(actor, command);
			case "removeelement":
			case "remelement":
				return BuildingCommandRemoveElement(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandRemoveElement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which element do you want to edit?");
			return false;
		}

		var element = Elements.GetByIdOrOrder(command.PopSpeech());
		if (element is null)
		{
			actor.OutputHandler.Send("This pattern has no such element.");
			return false;
		}

		if (_elements.Count(x => x.TargetDivision == element.TargetDivision) <= 1)
		{
			actor.OutputHandler.Send("You cannot delete the last pattern for a currency division.");
			return false;
		}

		actor.OutputHandler.Send($"Are you sure you want to delete the currency description pattern element #{element.Id.ToString("N0", actor)} ({element.Pattern.ColourCommand()}). This action cannot be undone.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = $"Deleting currency description pattern element {element.Id.ToString("N0", actor)}",
			AcceptAction = text =>
			{
				if (_elements.Count(x => x.TargetDivision == element.TargetDivision) <= 1)
				{
					actor.OutputHandler.Send("You cannot delete the last pattern for a currency division.");
					return;
				}

				actor.OutputHandler.Send($"You delete the currency description pattern element #{element.Id.ToString("N0", actor).ColourValue()}.");
				_elements.Remove(element);
				element.Delete();
			},
			RejectAction = text =>
			{
				actor.OutputHandler.Send("You decide not to delete the currency description pattern.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send("You decide not to delete the currency description pattern.");
			},
			Keywords = new List<string>
			{
				"element",
				"delete"
			}
		}), TimeSpan.FromSeconds(120));
		return true;
	}

	private bool BuildingCommandElement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which element do you want to edit?");
			return false;
		}

		var element = Elements.GetByIdOrOrder(command.PopSpeech());
		if (element is null)
		{
			actor.OutputHandler.Send("This pattern has no such element.");
			return false;
		}

		return element.BuildingCommand(actor, command);
	}

	private bool BuildingCommandAddElement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which currency division should this new element target?");
			return false;
		}

		var division = Currency.CurrencyDivisions.GetByIdOrName(command.PopSpeech());
		if (division is null)
		{
			actor.OutputHandler.Send($"The {Currency.Name.ColourName()} currency does not have any such currency division.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What word should be used to pluralise in the pattern?");
			return false;
		}

		var plural = command.PopSpeech().ToLowerInvariant();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the pattern text for this currency description pattern element?");
			return false;
		}

		var pattern = command.SafeRemainingArgument;
		if (!pattern.IsValidFormatString(new[] { true }))
		{
			actor.OutputHandler.Send($"The text {pattern.ColourCommand()} is not a valid format string. Hint: Use {"{0}".ColourCommand()} as a token for the numerical value.");
			return false;
		}

		var element = new CurrencyDescriptionPatternElement(this, pattern, plural, division);
		_elements.Add(element);
		actor.OutputHandler.Send($"You create a new currency description pattern element with ID #{element.Id.ToString("N0", actor)} and pattern {pattern.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandApplicabilityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor.Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new ProgVariableTypes[]
			{
				ProgVariableTypes.Number
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ApplicabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This pattern will now use the {prog.MXPClickableFunctionName()} prog to determine if it applies.");
		return true;
	}

	private bool BuildingCommandNegativePrefix(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prefix for negative values.");
			return false;
		}

		NegativeValuePrefix = command.GetSafeRemainingArgument(false);
		actor.OutputHandler.Send($"The text \"{NegativeValuePrefix.ColourValue()}\" will now be prefixed to any negative values.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandNatural(ICharacter actor, StringStack command)
	{
		UseNaturalAggregationStyle = !UseNaturalAggregationStyle;
		Changed = true;
		actor.OutputHandler.Send($"This element will {UseNaturalAggregationStyle.NowNoLonger()} use a natural aggregation style (with commas and 'and')");
		return true;
	}

	private bool BuildingCommandOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which order do you want this pattern to be evaluated in? Lower numbers are evaluated first.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (value < 1 || value > Currency.PatternDictionary[Type].Count)
		{
			actor.OutputHandler.Send($"You must enter a number between {1.ToString("N0", actor).ColourValue()} and {Currency.PatternDictionary[Type].Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		var currentIndex = Currency.PatternDictionary[Type].IndexOf(this);
		Currency.PatternDictionary[Type].RemoveAt(currentIndex);
		Currency.PatternDictionary[Type].Insert(value - 1, this);
		for (var i = 0; i < Currency.PatternDictionary[Type].Count; i++)
		{
			Currency.PatternDictionary[Type][i].Order = i + 1;
			Currency.PatternDictionary[Type][i].Changed = true;
		}

		actor.OutputHandler.Send($"This currency description pattern will now be evaluated {value.ToOrdinal().ColourValue()}.");
		return true;
	}
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Currency Description Pattern #{Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {Type.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Evaluation Order: {Order.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Use Natural Aggregation: {UseNaturalAggregationStyle.ToColouredString()}");
		sb.AppendLine($"Negative Prefix: {NegativeValuePrefix.ColourCommand()}");
		sb.AppendLine($"Applicability Prog: {ApplicabilityProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Elements:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from element in Elements
			orderby element.Order
			select new List<string>
			{
				element.Id.ToString("N0", actor),
				element.Pattern,
				element.Order.ToString("N0", actor),
				element.TargetDivision.Name,
				element.ShowIfZero.ToColouredString(),
				element.PluraliseWord,
				element.Rounding.DescribeEnum(),
				element.SpecialValuesOverridePattern.ToColouredString(),
				element.SpecialValues.Count.ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Pattern",
				"Order",
				"Division",
				"Show If Zero",
				"Pluralise On",
				"Rounding",
				"Specials Override",
				"# Specials"
			},
			actor,
			Telnet.Yellow
		)) ;
		return sb.ToString();
	}

	#endregion
}
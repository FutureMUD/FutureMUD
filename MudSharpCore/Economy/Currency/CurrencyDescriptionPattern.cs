using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
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
		foreach (var item in _elements)
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

	public const string HelpText = @"";

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

		return element.BuildingCommand(actor, command);
	}

	private bool BuildingCommandElement(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandAddElement(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandApplicabilityProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor.Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new FutureProgVariableTypes[]
			{
				FutureProgVariableTypes.Number
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

		NegativeValuePrefix = command.SafeRemainingArgument;
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
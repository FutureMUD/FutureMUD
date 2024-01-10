using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using Org.BouncyCastle.Asn1.Pkcs;

namespace MudSharp.Economy.Currency;

public class CurrencyDescriptionPattern : SaveableItem, ICurrencyDescriptionPattern
{
	private readonly List<ICurrencyDescriptionPatternElement> _elements =
		new();

	public IEnumerable<ICurrencyDescriptionPatternElement> Elements => _elements;

	public string NegativeValuePrefix { get; private set; }

	public CurrencyDescriptionPattern(MudSharp.Models.CurrencyDescriptionPattern pattern, ICurrency parent,
		CurrencyDescriptionPatternType type)
	{
		Gameworld = parent.Gameworld;
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

	public override string FrameworkItemType => "CurrencyDescriptionPattern";

	public override string ToString()
	{
		return
			$"CurrencyDescriptionPattern [#{Id}]: {Enum.GetName(typeof(CurrencyDescriptionPatternType), Type)} ({_elements.Count} elements)";
	}

	#region ICurrencyDescriptionPattern Members

	public CurrencyDescriptionPatternType Type { get; }

	public int Order { get; }

	/// <summary>
	///     A FutureProg returning Boolean that accepts a Single Number parameter
	/// </summary>
	public IFutureProg ApplicabilityProg { get; }

	public bool UseNaturalAggregationStyle { get; }

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
				value = value % item.TargetDivision.BaseUnitConversionRate;

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

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
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

	public override void Save()
	{
		Changed = false;
		// TODO
	}

	#endregion
}
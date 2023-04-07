using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Economy.Currency;

public class CurrencyDescriptionPattern : FrameworkItem, ICurrencyDescriptionPattern
{
	private readonly List<ICurrencyDescriptionPatternElement> _elements =
		new();

	private readonly string _negativeValuePrefix;

	public CurrencyDescriptionPattern(MudSharp.Models.CurrencyDescriptionPattern pattern, Currency parent,
		CurrencyDescriptionPatternType type)
	{
		_id = pattern.Id;
		Type = type;
		Order = pattern.Order;
		UseNaturalAggregationStyle = pattern.UseNaturalAggregationStyle;
		ApplicabilityProg = pattern.FutureProgId.HasValue
			? parent.Gameworld.FutureProgs.Get(pattern.FutureProgId.Value)
			: null;
		_negativeValuePrefix = pattern.NegativePrefix;
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
			outList.Append(_negativeValuePrefix);
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

	#endregion
}
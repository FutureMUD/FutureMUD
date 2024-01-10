using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Economy.Currency;

public class CurrencyDescriptionPatternElement : SaveableItem, ICurrencyDescriptionPatternElement
{
	private readonly string _alternatePattern;
	private readonly CurrencyDescriptionPattern _parent;
	private readonly string _pattern;
	private readonly string _pluraliseWord;
	private readonly Dictionary<decimal, string> _specialValues = new();
	private readonly bool _specialValuesOverridePattern;
	private int _order;

	public CurrencyDescriptionPatternElement(MudSharp.Models.CurrencyDescriptionPatternElement element,
		CurrencyDescriptionPattern parentPattern, ICurrency parent)
	{
		Gameworld = parent.Gameworld;
		_id = element.Id;
		_order = element.Order;
		_pattern = element.Pattern;
		_alternatePattern = element.AlternatePattern;
		_pluraliseWord = element.PluraliseWord;
		_parent = parentPattern;
		Rounding = (RoundingMode)element.RoundingMode;
		TargetDivision = parent.CurrencyDivisions.First(x => x.Id == element.CurrencyDivisionId);
		_specialValuesOverridePattern = element.SpecialValuesOverrideFormat;
		ShowIfZero = element.ShowIfZero;
		foreach (var item in element.CurrencyDescriptionPatternElementSpecialValues)
		{
			_specialValues.Add(item.Value, item.Text);
		}
	}

	public override string FrameworkItemType => "CurrencyDescriptionPatternElement";

	public override string ToString()
	{
		return
			$"CurrencyDescriptionPatternElement [#{Id}]: pattern: \"{Pattern}\" alternate: \"{AlternatePattern}\" plural: \"{PluraliseWord}\"";
	}

	#region ICurrencyDescriptionPatternElement Members

	public RoundingMode Rounding { get; }

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

	public ICurrencyDivision TargetDivision { get; }

	public bool ShowIfZero { get; }
	public int Order => _order;

	public string Pattern => _pattern;

	public string PluraliseWord => _pluraliseWord;

	public bool SpecialValuesOverridePattern => _specialValuesOverridePattern;

	public string AlternatePattern => _alternatePattern;

	public IReadOnlyDictionary<decimal, string> SpecialValues => _specialValues.AsReadOnly();
	#endregion

	public override void Save()
	{
		// TODO
		Changed = false;
	}
}
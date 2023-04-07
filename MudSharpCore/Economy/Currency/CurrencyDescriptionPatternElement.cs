using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Economy.Currency;

public class CurrencyDescriptionPatternElement : FrameworkItem, ICurrencyDescriptionPatternElement
{
	private readonly string _alternatePattern;
	private readonly CurrencyDescriptionPattern _parent;
	private readonly string _pattern;
	private readonly string _pluraliseWord;
	private readonly Dictionary<decimal, string> _specialValues = new();
	private readonly bool _specialValuesOverridePattern;

	public CurrencyDescriptionPatternElement(MudSharp.Models.CurrencyDescriptionPatternElement element,
		CurrencyDescriptionPattern parentPattern, Currency parent)
	{
		_id = element.Id;
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
			$"CurrencyDescriptionPatternElement [#{Id}]: pattern: \"{_pattern}\" alternate: \"{_alternatePattern}\" plural: \"{_pluraliseWord}\"";
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

		var pattern = remainder || string.IsNullOrEmpty(_alternatePattern) ? _pattern : _alternatePattern;
		string value;
		if (_specialValues.ContainsKey(amount))
		{
			if (_specialValuesOverridePattern)
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

		if (!string.IsNullOrEmpty(_pluraliseWord) &&
		    (amount >= 2.0M || amount <= -2.0M || (amount > -1.0M && amount < 1.0M)))
		{
			return value.Replace(_pluraliseWord, _pluraliseWord.Pluralise());
		}

		return value;
	}

	public ICurrencyDivision TargetDivision { get; }

	public bool ShowIfZero { get; }

	#endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
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
	public int Order { get; private set; }

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

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
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
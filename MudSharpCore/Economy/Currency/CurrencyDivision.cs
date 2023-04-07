using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;

namespace MudSharp.Economy.Currency;

public class CurrencyDivision : FrameworkItem, ICurrencyDivision
{
	public CurrencyDivision(MudSharp.Models.CurrencyDivision division)
	{
		_id = division.Id;
		_name = division.Name;
		BaseUnitConversionRate = division.BaseUnitConversionRate;
		_patterns =
			division.CurrencyDivisionAbbreviations.Select(x =>
				        new Regex(x.Pattern, division.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None))
			        .ToList();
	}

	public override string FrameworkItemType => "CurrencyDivision";

	#region ICurrencyDivision Members

	public decimal BaseUnitConversionRate { get; }

	private readonly List<Regex> _patterns;
	public IEnumerable<Regex> Patterns => _patterns;

	#endregion
}
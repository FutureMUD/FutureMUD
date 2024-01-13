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

namespace MudSharp.Economy.Currency;

public class CurrencyDivision : SaveableItem, ICurrencyDivision
{
	public CurrencyDivision(IFuturemud gameworld, MudSharp.Models.CurrencyDivision division)
	{
		Gameworld = gameworld;
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

	public decimal BaseUnitConversionRate { get; private set; }

	private readonly List<Regex> _patterns;
	public IEnumerable<Regex> Patterns => _patterns;

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new System.NotImplementedException();
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Currency Division #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.FunctionYellow, Telnet.White));
		sb.AppendLine();
		sb.AppendLine($"Base Units: {BaseUnitConversionRate.ToString("N0", actor).ColourValue()}");
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
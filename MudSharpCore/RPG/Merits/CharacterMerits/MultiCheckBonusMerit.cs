using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MultiCheckBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected MultiCheckBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		CheckTypes = (from element in definition.Element("Checks")?.Elements() ?? Enumerable.Empty<XElement>()
		              select (CheckType)int.Parse(element.Attribute("type")?.Value ?? "0")).ToList();
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	public IEnumerable<CheckType> CheckTypes { get; set; }
	public double SpecificBonus { get; set; }

	#region Implementation of ICheckBonusMerit

	public double CheckBonus(ICharacter ch, CheckType type)
	{
		return CheckTypes.Contains(type) ? SpecificBonus : 0.0;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Multi Check Bonus",
			(merit, gameworld) => new MultiCheckBonusMerit(merit, gameworld));
	}
}
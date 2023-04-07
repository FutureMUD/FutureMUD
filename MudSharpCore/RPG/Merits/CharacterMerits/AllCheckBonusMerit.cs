using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AllCheckBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected AllCheckBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	public double SpecificBonus { get; set; }

	#region Implementation of ICheckBonusMerit

	public double CheckBonus(ICharacter ch, CheckType type)
	{
		return SpecificBonus;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Check Bonus",
			(merit, gameworld) => new AllCheckBonusMerit(merit, gameworld));
	}
}
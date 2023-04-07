using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SpecificCheckBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected SpecificCheckBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		CheckType = (CheckType)int.Parse(definition.Attribute("type")?.Value ?? "0");
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	public CheckType CheckType { get; set; }
	public double SpecificBonus { get; set; }

	#region Implementation of ICheckBonusMerit

	public double CheckBonus(ICharacter ch, CheckType type)
	{
		return type == CheckType ? SpecificBonus : 0.0;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Specific Check Bonus",
			(merit, gameworld) => new SpecificCheckBonusMerit(merit, gameworld));
	}
}
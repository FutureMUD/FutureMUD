using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AllInfectionResistanceMerit : CharacterMeritBase, IInfectionResistanceMerit
{
	protected AllInfectionResistanceMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = int.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	protected int SpecificBonus { get; set; }

	public Difficulty GetNewInfectionDifficulty(Difficulty original, InfectionType type)
	{
		return original.StageDown(SpecificBonus);
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Infection Bonus",
			(merit, gameworld) => new AllInfectionResistanceMerit(merit, gameworld));
	}
}
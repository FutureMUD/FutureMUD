using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AllSkillBonusMerit : CharacterMeritBase, ITraitBonusMerit
{
	protected AllSkillBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0.0");
	}

	public double SpecificBonus { get; set; }

	#region Implementation of ITraitBonusMerit

	public double BonusForTrait(ITraitDefinition trait, TraitBonusContext context)
	{
		return trait.TraitType == TraitType.Skill ? SpecificBonus : 0.0;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Skill Bonus",
			(merit, gameworld) => new AllSkillBonusMerit(merit, gameworld));
	}
}
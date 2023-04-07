using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using Chargen = MudSharp.CharacterCreation.Chargen;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class CheckCategoryBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected CheckCategoryBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
		AppliesToHealingChecks = bool.Parse(definition.Attribute("healing")?.Value ?? "false");
		AppliesToFriendlyChecks = bool.Parse(definition.Attribute("friendly")?.Value ?? "false");
		AppliesToHostileChecks = bool.Parse(definition.Attribute("hostile")?.Value ?? "false");
		AppliesToActiveChecks = bool.Parse(definition.Attribute("active")?.Value ?? "false");
		AppliesToPerceptionChecks = bool.Parse(definition.Attribute("perception")?.Value ?? "false");
		AppliesToLanguageChecks = bool.Parse(definition.Attribute("language")?.Value ?? "false");
	}

	public double SpecificBonus { get; set; }

	public bool AppliesToHealingChecks { get; set; }
	public bool AppliesToFriendlyChecks { get; set; }
	public bool AppliesToHostileChecks { get; set; }
	public bool AppliesToActiveChecks { get; set; }
	public bool AppliesToPerceptionChecks { get; set; }
	public bool AppliesToLanguageChecks { get; set; }

	#region Implementation of ICheckBonusMerit

	public double CheckBonus(ICharacter ch, CheckType type)
	{
		if ((AppliesToHealingChecks && type.IsHealingCheck()) ||
		    (AppliesToFriendlyChecks && type.IsTargettedFriendlyCheck()) ||
		    (AppliesToHostileChecks && type.IsTargettedHostileCheck()) ||
		    (AppliesToActiveChecks && type.IsGeneralActivityCheck()) ||
		    (AppliesToPerceptionChecks && type.IsPerceptionCheck()) ||
		    (AppliesToLanguageChecks && type.IsLanguageCheck()))
		{
			return SpecificBonus;
		}

		return 0.0;
	}

	#endregion

	public override bool Applies(IHaveMerits owner)
	{
		return Applies(owner, default(ICharacter));
	}

	public override bool Applies(IHaveMerits owner, IPerceivable target)
	{
		if (owner is ICharacter ownerAsCharacter)
		{
			return (bool?)ApplicabilityProg?.Execute(ownerAsCharacter, target) ?? true;
		}

		if (owner is IBody ownerAsBody)
		{
			return (bool?)ApplicabilityProg?.Execute(ownerAsBody.Actor, target) ?? true;
		}

		return owner is Chargen ownerAsChargen;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Check Category Bonus",
			(merit, gameworld) => new CheckCategoryBonusMerit(merit, gameworld));
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using Chargen = MudSharp.CharacterCreation.Chargen;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ScaledCheckCategoryBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected ScaledCheckCategoryBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		GetBonusAmountProg = gameworld.FutureProgs.Get(long.Parse(definition.Attribute("bonusprog")?.Value ?? "0"));
		AppliesToHealingChecks = bool.Parse(definition.Attribute("healing")?.Value ?? "false");
		AppliesToFriendlyChecks = bool.Parse(definition.Attribute("friendly")?.Value ?? "false");
		AppliesToHostileChecks = bool.Parse(definition.Attribute("hostile")?.Value ?? "false");
		AppliesToActiveChecks = bool.Parse(definition.Attribute("active")?.Value ?? "false");
		AppliesToPerceptionChecks = bool.Parse(definition.Attribute("perception")?.Value ?? "false");
		AppliesToLanguageChecks = bool.Parse(definition.Attribute("language")?.Value ?? "false");
	}

	public bool AppliesToHealingChecks { get; set; }
	public bool AppliesToFriendlyChecks { get; set; }
	public bool AppliesToHostileChecks { get; set; }
	public bool AppliesToActiveChecks { get; set; }
	public bool AppliesToPerceptionChecks { get; set; }
	public bool AppliesToLanguageChecks { get; set; }

	public IFutureProg GetBonusAmountProg { get; set; }

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
			var amount = GetBonusAmountProg?.Execute(ch);
			if (amount == null)
			{
				return 0.0;
			}

			return (double)(decimal)amount;
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
		MeritFactory.RegisterMeritInitialiser("Scaled Check Category Bonus",
			(merit, gameworld) => new ScaledCheckCategoryBonusMerit(merit, gameworld));
	}
}
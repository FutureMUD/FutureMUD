using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Health;

public static class WoundExtensions
{
	public static void ProcessPassiveWounds(this IEnumerable<IWound> wounds)
	{
		var parents = wounds.Select(x => x.Parent).OfType<IMortalPerceiver>().Distinct();
		var itemParents = parents.OfType<IGameItem>().ToList();
		var chParent = parents.OfType<ICharacter>().ToList();
		var supressing = chParent.Any(x => x.AffectedBy<SupressWoundMessages>());
		if (supressing)
		{
			foreach (var parent in itemParents)
			{
				parent.AddEffect(new SupressWoundMessages(parent));
			}
		}

		if (supressing)
		{
			var message = chParent.SelectNotNull(x => x.CombinedEffectsOfType<SupressWoundMessages>().FirstOrDefault())
			                      .First().TargetTookWoundsEmote;
			if (message != null)
			{
				foreach (var parent in parents)
				{
					if (parent is IGameItem item && (item.InInventoryOf != null || item.ContainedIn != null))
					{
						continue;
					}

					parent.OutputHandler.Handle(new EmoteOutput(new Emote(message, parent, parent),
						style: OutputStyle.NoNewLine, flags: OutputFlags.SuppressObscured));
				}
			}
		}

		wounds = wounds.Shuffle().ToList();
		foreach (var wound in wounds)
		{
			if (wound == null)
			{
				throw new ArgumentException(
					"There was a NULL value in the collection of wounds passed into ProcessPassiveWounds",
					nameof(wounds));
			}

			wound.Parent.ProcessPassiveWound(wound);
		}

		foreach (var parent in parents)
		{
			parent.StartHealthTick();
			parent.CheckHealthStatus();
			parent.RemoveAllEffects<SupressWoundMessages>();
		}
	}

	public static bool CanCauseOrganBleeding(this DamageType type)
	{
		switch (type)
		{
			case DamageType.Hypoxia:
			case DamageType.Necrotic:
				return false;
		}

		return true;
	}

	public static bool CanLodge(this DamageType type)
	{
		switch (type)
		{
			case DamageType.BallisticArmourPiercing:
			case DamageType.Ballistic:
			case DamageType.Chopping:
			case DamageType.Piercing:
			case DamageType.Slashing:
			case DamageType.ArmourPiercing:
				return true;
			default:
				return false;
		}
	}

	public static bool CanSever(this DamageType type)
	{
		switch (type)
		{
			case DamageType.Bite:
			case DamageType.Chopping:
			case DamageType.Claw:
			case DamageType.Slashing:
			case DamageType.Shearing:
			case DamageType.Shrapnel:
			case DamageType.Shockwave:
				return true;
			default:
				return false;
		}
	}

	public static string Describe(this PromptType type)
	{
		var sb = new StringBuilder();
		if (type.HasFlag(PromptType.FullBrief))
		{
			sb.Append("Brief ");
		}
		else if (type.HasFlag(PromptType.Classic))
		{
			sb.Append("Classic ");
		}
		else
		{
			sb.Append("Full ");
		}

		if (type.HasFlag(PromptType.PositionInfo))
		{
			sb.Append("(Position)");
		}

		if (type.HasFlag(PromptType.SpeakInfo))
		{
			sb.Append("(Language)");
		}

		if (type.HasFlag(PromptType.StealthInfo))
		{
			sb.Append("(Stealth)");
		}

		return sb.ToString();
	}

	public static WoundSeverity StageUp(this WoundSeverity severity, int degrees)
	{
		var result = (int)severity + degrees;
		if (result >= (int)WoundSeverity.Horrifying)
		{
			return WoundSeverity.Horrifying;
		}

		if (result <= (int)WoundSeverity.None)
		{
			return WoundSeverity.None;
		}

		return (WoundSeverity)result;
	}

	public static WoundSeverity StageDown(this WoundSeverity severity, int degrees)
	{
		var result = (int)severity - degrees;
		if (result <= (int)WoundSeverity.None)
		{
			return WoundSeverity.None;
		}

		if (result >= (int)WoundSeverity.Horrifying)
		{
			return WoundSeverity.Horrifying;
		}

		return (WoundSeverity)result;
	}

	public static string Describe(this WoundSeverity severity)
	{
		switch (severity)
		{
			case WoundSeverity.None:
				return "Insignificant";
			case WoundSeverity.Superficial:
				return "Superficial";
			case WoundSeverity.Minor:
				return "Minor";
			case WoundSeverity.Small:
				return "Small";
			case WoundSeverity.Moderate:
				return "Moderate";
			case WoundSeverity.Severe:
				return "Severe";
			case WoundSeverity.VerySevere:
				return "Very Severe";
			case WoundSeverity.Grievous:
				return "Grievous";
			case WoundSeverity.Horrifying:
				return "Horrifying";
		}

		return "Unknown";
	}

	public static string Describe(this DamageType type)
	{
		switch (type)
		{
			case DamageType.Slashing:
				return "Slashing";
			case DamageType.Chopping:
				return "Chopping";
			case DamageType.Crushing:
				return "Crushing";
			case DamageType.Piercing:
				return "Piercing";
			case DamageType.ArmourPiercing:
				return "Armour Piercing";
			case DamageType.Ballistic:
				return "Ballistic";
			case DamageType.Burning:
				return "Burning";
			case DamageType.Freezing:
				return "Freezing";
			case DamageType.Chemical:
				return "Chemical";
			case DamageType.Shockwave:
				return "Shockwave";
			case DamageType.Claw:
				return "Claw";
			case DamageType.Bite:
				return "Bite";
			case DamageType.Electrical:
				return "Electrical";
			case DamageType.Hypoxia:
				return "Hypoxia";
			case DamageType.Cellular:
				return "Cellular";
			case DamageType.Sonic:
				return "Sonic";
			case DamageType.BallisticArmourPiercing:
				return "Ballistic Armour Piercing";
			case DamageType.Shearing:
				return "Shearing";
			case DamageType.Wrenching:
				return "Wrenching";
			case DamageType.Shrapnel:
				return "Shrapnel";
			case DamageType.Necrotic:
				return "Necrotic";
			case DamageType.Falling:
				return "Falling";
			case DamageType.Eldritch:
				return "Eldritch";
			case DamageType.Arcane:
				return "Arcane";
		}

		return "Unknown";
	}

	public static bool TryGetDamageType(string text, out DamageType damageType)
	{
		switch (text.ToLowerInvariant())
		{
			case "slashing":
				damageType = DamageType.Slashing;
				return true;
			case "chopping":
				damageType = DamageType.Chopping;
				return true;
			case "crushing":
				damageType = DamageType.Crushing;
				return true;
			case "piercing":
				damageType = DamageType.Piercing;
				return true;
			case "ballistic":
				damageType = DamageType.Ballistic;
				return true;
			case "burning":
				damageType = DamageType.Burning;
				return true;
			case "freezing":
				damageType = DamageType.Freezing;
				return true;
			case "chemical":
				damageType = DamageType.Chemical;
				return true;
			case "shockwave":
				damageType = DamageType.Shockwave;
				return true;
			case "bite":
				damageType = DamageType.Bite;
				return true;
			case "claw":
				damageType = DamageType.Claw;
				return true;
			case "electrical":
				damageType = DamageType.Electrical;
				return true;
			case "hypoxia":
			case "hypoxic":
				damageType = DamageType.Hypoxia;
				return true;
			case "cellular":
				damageType = DamageType.Cellular;
				return true;
			case "sonic":
				damageType = DamageType.Sonic;
				return true;
			case "shearing":
				damageType = DamageType.Shearing;
				return true;
			
			case "armourpiercing":
			case "ap":
			case "armorpiercing":
			case "armour piercing":
			case "armor piercing":
				damageType = DamageType.ArmourPiercing;
				return true;
			case "balliasticarmourpiercing":
			case "bap":
			case "balliasticarmorpiercing":
			case "balliasticarmour piercing":
			case "balliasticarmor piercing":
				damageType = DamageType.BallisticArmourPiercing;
				return true;
			case "wrenching":
			case "wrench":
				damageType = DamageType.Wrenching;
				return true;
			case "shrapnel":
				damageType = DamageType.Shrapnel;
				return true;
			case "necrotic":
			case "necrosis":
				damageType = DamageType.Necrotic;
				return true;
			case "falling":
				damageType = DamageType.Falling;
				return true;
		}

		damageType = DamageType.Ballistic;
		return false;
	}

	public static string Describe(this TreatmentType type)
	{
		switch (type)
		{
			case TreatmentType.Mend:
				return "Mending Damage";
			case TreatmentType.Repair:
				return "Repairing Damage";
			case TreatmentType.Trauma:
				return "Controlling Bleeding Trauma";
			case TreatmentType.Remove:
				return "Removing Lodged Items";
			case TreatmentType.Close:
				return "Closing Traumatic Wounds";
			case TreatmentType.Relocation:
				return "Relocating Dislocated Joints";
			case TreatmentType.Set:
				return "Setting Broken Bones";
			case TreatmentType.Clean:
				return "Cleaning Wounds";
			case TreatmentType.Antiseptic:
				return "Controlling Bacterial Infection";
			case TreatmentType.AntiInflammatory:
				return "Reducing Inflammation";
			case TreatmentType.Tend:
				return "Tending To The Wounds";
		}

		return "Unknown";
	}

	public static bool GetTreatmentType(string text, out TreatmentType treatmentType)
	{
		var treatmentTypes = Enum.GetValues(typeof(TreatmentType)).OfType<TreatmentType>().ToList();
		if (treatmentTypes.Any(x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase)))
		{
			treatmentType =
				treatmentTypes.FirstOrDefault(
					x => x.Describe().Equals(text, StringComparison.InvariantCultureIgnoreCase));
			return true;
		}

		var diffNames = treatmentTypes.Select(x => Tuple.Create(x, Enum.GetName(typeof(TreatmentType), x))).ToList();
		if (!diffNames.Any(x => x.Item2.Equals(text, StringComparison.InvariantCultureIgnoreCase)))
		{
			treatmentType = TreatmentType.None;
			return false;
		}

		treatmentType =
			diffNames.First(x => x.Item2.Equals(text, StringComparison.InvariantCultureIgnoreCase)).Item1;
		return true;
	}

	public static bool TryParseWoundSeverity(string text, out WoundSeverity severity)
	{
		var enums = Enum.GetValues(typeof(WoundSeverity)).OfType<WoundSeverity>().ToList();
		if (enums.Any(x => x.Describe().EqualTo(text)))
		{
			severity = enums.FirstOrDefault(x => x.Describe().EqualTo(text));
			return true;
		}

		var enumsWithNames = enums.Select(x => (Value: x, Name: Enum.GetName(typeof(WoundSeverity), x),
			CamelCase: Enum.GetName(typeof(WoundSeverity), x).SplitCamelCase())).ToList();
		if (!enumsWithNames.Any(x => x.Name.EqualTo(text) || x.CamelCase.EqualTo(text)))
		{
			severity = WoundSeverity.None;
			return false;
		}

		severity = enumsWithNames.FirstOrDefault(x => x.Name.EqualTo(text) || x.CamelCase.EqualTo(text)).Value;
		return true;
	}
}
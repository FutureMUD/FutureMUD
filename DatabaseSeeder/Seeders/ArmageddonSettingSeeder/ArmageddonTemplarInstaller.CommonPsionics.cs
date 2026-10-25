#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTemplarInstaller
{
	public const string NormalPsionicsMeritKey = "arm.merit.normal_psionics";
	public const string PsionicistMeritKey = "arm.merit.psionicist_psionics";
	public const string MagicPsionicProgressionKey = "arm.merit.magic_class_psionic_progression";

	internal static long? OptionalPsionicMerit(FuturemudDatabaseContext db, string key)
	{
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is null) return null;
		if (record.Module != Module || record.SeedBaseline is null || db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
			throw new InvalidOperationException($"Invalid or competing psionic merit ownership: {key}.");
		return RequireOwned<Merit>(db, key);
	}

	private static void InstallCommonPsionics(FuturemudDatabaseContext db, MagicSchool school,
		IReadOnlyDictionary<string, TraitDefinition> skills, Dictionary<string, MagicPower> shared,
		long allowed, long unavailable, long willpower, long wisdom, long freeUpkeep, long resource, long normalDifficulty,
		ICollection<string> messages)
	{
		var powers = new Dictionary<string, (MagicPower Power, long Trait)>(StringComparer.Ordinal);
		foreach (var (key, power) in shared) powers.Add(key, (power, skills[TraitForPower(key)].Id));
		var specs = new (string Key, string Model, string Verb, bool Sustained, int Cost)[]
		{
			("babble", "babble", "babble", false, 10),
			("sense_presence", "psionicsensepresence", "sensepresence", true, 8),
			("probe", "psionicprobe", "probe", false, 10),
			("disorient", "psionicdisorient", "disorient", false, 14),
			("project", "projectemotion", "project", false, 8),
			("rejuvenate", "psionicrejuvenate", "rejuvenate", false, 8),
			("conceal", "mindconceal", "conceal", true, 10),
			("suggest", "suggest", "suggest", false, 14),
			("compel", "psioniccontrol", "compel", false, 18),
			("vanish", "psionicvanish", "vanish", true, 14)
		};
		foreach (var spec in specs)
		{
			var trait = skills["arm.skill." + spec.Key];
			var stock = PsionicStockContent.Powers.SingleOrDefault(x => x.Type == spec.Model);
			var xml = stock is null ? NewBasePowerDefinition(spec.Key, allowed, unavailable,
				new XElement("PsionicTrace", new XElement("Enabled", true), new XElement("DurationSeconds", 900),
					new XElement("ReadDifficulty", (int)Difficulty.Hard), new XElement("Description", "a lingering disturbance in the Way"))) :
				PsionicStockContent.Definition(stock, trait.Id, resource, allowed, allowed, unavailable, normalDifficulty, advanced: true);
			xml.Element("InvocationCosts")?.Remove();
			xml.Element("SustainResourceCosts")?.Remove();
			xml.SetElementValue("Verb", spec.Verb);
			xml.SetElementValue("SkillCheckTrait", trait.Id);
			xml.SetElementValue("SkillCheckDifficulty", (int)Difficulty.Normal);
			xml.SetElementValue("MinimumSuccessThreshold", (int)Outcome.MinorPass);
			xml.SetElementValue("PowerDistance", (int)MagicPowerDistance.AnyConnectedMind);
			xml.SetElementValue("DetectableWithDetectMagic", (int)Difficulty.Normal);
			xml.SetElementValue("SuccessEcho", "Your thought takes shape in the Way.");
			xml.SetElementValue("FailEcho", "You cannot shape the mental impulse.");
			if (spec.Key == "compel") xml.SetElementValue("RequireSameLocation", false);
			if (spec.Key == "disorient") xml.SetElementValue("AffectedTrait", FindAttribute(db, "Agility", "Dexterity", "Coordination", "Speed").Id);
			if (spec.Sustained)
			{
				xml.SetElementValue("BeginVerb", spec.Verb);
				xml.SetElementValue("EndVerb", "end" + spec.Verb);
				xml.SetElementValue("BeginEmote", "You extend your awareness through the Way.");
				xml.SetElementValue("EndEmote", "You release your mental focus.");
				xml.SetElementValue("FailEmote", "Your mental focus slips away.");
				xml.SetElementValue("Duration", 300000);
				xml.SetElementValue("ConcentrationPointsToSustain", 1);
				xml.SetElementValue("SustainPenalty", -2);
				xml.Add(new XElement("StunUpkeep", new XAttribute("skillTrait", trait.Id), new XAttribute("wisdomTrait", wisdom),
					new XAttribute("freeProg", freeUpkeep), new XAttribute("attemptsPerMinute", 60)));
			}
			if (spec.Key == "conceal")
			{
				xml.SetElementValue("AllSchools", true);
				xml.SetElementValue("HideActivity", true);
				xml.SetElementValue("AuditDifficultyStages", 2);
			}
			xml.Add(new XElement("BodilyCost", new XAttribute("base", spec.Cost), new XAttribute("skillTrait", trait.Id),
				new XAttribute("willpowerTrait", willpower), new XAttribute("verbs", spec.Verb)));
			var help = spec.Key switch
			{
				"rejuvenate" => "Use LPSI REJUVENATE <health|hunger|thirst|sobriety|stamina>. Spend stun to improve that condition; failure worsens it.",
				"vanish" => "Use LPSI VANISH <room|near|far|veryfar> or LPSI ENDVANISH. Wider ranges require 25, 50 and 75 skill. Near covers adjacent rooms, far your zone and veryfar your shard.",
				"sense_presence" => "Use LPSI SENSEPRESENCE or LPSI ENDSENSEPRESENCE. Sense nearby minds through sneaking and unconcealed psionic vanishing.",
				"conceal" => "Use LPSI CONCEAL or LPSI ENDCONCEAL to veil or reveal your mental signature.",
				"compel" => "Use LPSI COMPEL <contacted target> <command> to impose one restricted command remotely.",
				"project" => "Use LPSI PROJECT <contacted target> <feeling> to project an emotion.",
				"suggest" => "Use LPSI SUGGEST <contacted target> <thought> to plant a thought.",
				"probe" => "Use LPSI PROBE <contacted target>. More skill reveals physical details, attributes, identity, plans, affiliations and contacts. The target must be of the same race.",
				_ => $"Use LPSI {spec.Verb.ToUpperInvariant()} <contacted target>."
			};
			var power = (MagicPower)Put(db, "arm.power.psionicist." + spec.Key, new MagicPower
			{
				Name = trait.Name, MagicSchoolId = school.Id, PowerModel = spec.Model,
				Blurb = help, ShowHelp = help, Definition = xml.ToString()
			}, ["Name", "MagicSchoolId", "PowerModel", "Blurb", "ShowHelp", "Definition"], messages);
			powers.Add(spec.Key, (power, trait.Id));
		}
		InstallProfile("normal", "Normal", ["contact", "connectback", "say", "barrier", "expel"], NormalPsionicsMeritKey);
		var sourceSkills = ArmageddonMundaneProgression.Roles.Single(x => x.Name == "Psionicist" && x.Primary).Skills.Select(x => x.Key).ToHashSet();
		var psionicist = powers.Where(x => sourceSkills.Contains("arm.skill." + (x.Key == "vision" ? "masquerade" : x.Key)) || x.Key is "say" or "connectback").Select(x => x.Key).ToArray();
		if (psionicist.Length != 29) throw new InvalidOperationException("The Psionicist profile must cover all 27 source powers and both Contact communication commands.");
		InstallProfile("psionicist", "Psionicist", psionicist, PsionicistMeritKey);
		InstallMagicPsionicProgression(db, skills, messages);
		messages.Add("Psionics installed: normal Way abilities for Armageddon classes, with shared powers and expanded Lirathan Templar and Psionicist profiles. The separate Psionics seeder is not required.");

		void InstallProfile(string key, string name, string[] selected, string meritKey)
		{
			var definition = new XElement("Definition", new XElement("ConcentrationTrait", skills["arm.skill.contact"].Id),
				new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
				selected.Select(x => new XElement("Power", new XAttribute("trait", powers[x].Trait), new XAttribute("minvalue", 1), new XAttribute("power", powers[x].Power.Id))));
			var capability = (MagicCapability)Put(db, "arm.capability." + key + "_psionics", new MagicCapability
			{
				Name = name + " Psionics", CapabilityModel = "skilllevel", PowerLevel = key == "normal" ? 1 : 3,
				MagicSchoolId = school.Id, Definition = definition.ToString()
			}, ["Name", "CapabilityModel", "PowerLevel", "MagicSchoolId", "Definition"], messages);
			Put(db, meritKey, new Merit { Name = capability.Name, Type = "Magic Capability", MeritType = 0, MeritScope = 2,
				Definition = new XElement("Definition", new XElement("ChargenAvailableProg", db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id),
					new XElement("ApplicabilityProg", db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id),
					new XElement("ChargenBlurb", "Abilities of the Way granted by your Armageddon class."), new XElement("DescriptionText", "@ wield|wields the Way."),
					new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() }, ["Name", "Type", "MeritType", "MeritScope", "Definition"], messages);
		}
	}

	private static void InstallMagicPsionicProgression(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills, ICollection<string> messages)
	{
		var roles = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == ArmageddonProgressionInstaller.Module &&
			x.EntityType == nameof(ChargenRole) && !x.Retired && x.LogicalId != null).Select(x => x.LogicalId!.Value).ToArray();
		var rules = roles.SelectMany(role => new[] { (Key: "contact", Opening: 20), (Key: "barrier", Opening: 5), (Key: "expel", Opening: 5) }.Select(skill =>
			new XElement("Rule", new XAttribute("role", role), new XAttribute("trait", skills["arm.skill." + skill.Key].Id),
				new XAttribute("primary", true), new XAttribute("opening", skill.Opening), new XAttribute("cap", 80),
				skill.Key == "expel" ? new XAttribute("parent", skills["arm.skill.barrier"].Id) : null,
				new XAttribute("threshold", skill.Key == "expel" ? 70 : 0), new XAttribute("bonus", 0))));
		Put(db, MagicPsionicProgressionKey, new Merit { Name = "Magic Class Psionic Progression", Type = "Role Skill Progression", MeritScope = 2, MeritType = 0,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id),
				new XElement("ApplicabilityProg", db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id), new XElement("DescriptionText", "@ practise|practises the Way."),
				new XElement("ChargenBlurb", "Normal Way progression for magic classes."), new XElement("Rules", rules)).ToString() },
			["Name", "Type", "MeritScope", "MeritType", "Definition"], messages);
	}
}

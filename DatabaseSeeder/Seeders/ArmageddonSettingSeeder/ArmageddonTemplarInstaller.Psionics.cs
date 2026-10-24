#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTemplarInstaller
{
	private sealed record TemplarPower(string Key, string Name, string Model, string Verb, string SkillKey, int Cost,
		MagicPowerDistance Distance, string? Extra = null, bool Sustained = false);

	private static void InstallLirathanPsionics(FuturemudDatabaseContext db, Role lirathanRole,
		IReadOnlyDictionary<string, ChargenRole> roles, IReadOnlyDictionary<string, TraitDefinition> skills, ICollection<string> messages)
	{
		var school = (MagicSchool)Put(db, "arm.school.lirathan_templar", new MagicSchool { Name = "Armageddon Psionics",
			SchoolVerb = "lpsi", SchoolAdjective = "psionic", PowerListColour = "boldmagenta" },
			[nameof(MagicSchool.Name), nameof(MagicSchool.SchoolVerb), nameof(MagicSchool.SchoolAdjective), nameof(MagicSchool.PowerListColour)], messages);
		var canInvoke = TextOrBooleanPowerProg(db, "arm.templar.lirathan.power.allowed", "return true", ProgVariableTypes.Boolean, messages);
		var unavailable = TextOrBooleanPowerProg(db, "arm.templar.lirathan.power.unavailable", "return \"That Lirathan psionic power is unavailable.\"", ProgVariableTypes.Text, messages);
		var inTuluk = Prog(db, "arm.templar.lirathan.tuluk.free_upkeep", "LirathanTemplarInTuluk", "return false",
			[(ProgVariableTypes.Character, "ch")], messages);
		var freeUpkeep = Prog(db, "arm.psionics.lirathan.free_upkeep", "ArmageddonLirathanFreePsionicUpkeep",
			$"return Contains(@ch.Roles, ToRole({roles[lirathanRole.Key].Id})) and @{inTuluk.FunctionName}(@ch)",
			[(ProgVariableTypes.Character, "ch")], messages);
		var normalDifficulty = (FutureProg)Put(db, "arm.psionics.normal_difficulty", new FutureProg
		{
			FunctionName = "ArmageddonPsionicDifficulty", FunctionText = $"return {(int)Difficulty.Normal}", ReturnType = (long)ProgVariableTypes.Number,
			ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(), AcceptsAnyParameters = true,
			Category = "Armageddon", Subcategory = "Psionics", FunctionComment = "Default mental expulsion difficulty.", Public = false
		}, ["FunctionName", "FunctionText", "ReturnTypeDefinition", "AcceptsAnyParameters", "Category", "Subcategory", "FunctionComment", "Public"], messages);
		var sourceResource = RequireOwned<MagicResource>(db, ArmageddonProgressionInstaller.SourceResourceKey);
		var willpower = FindAttribute(db, "Willpower", "Wisdom");
		var wisdom = FindAttribute(db, "Wisdom", "Intelligence");
		var trace = new XElement("PsionicTrace", new XElement("Enabled", true), new XElement("DurationSeconds", 900),
			new XElement("ReadDifficulty", (int)Difficulty.Hard), new XElement("Description", "a lingering disturbance in the Way"));
		var target = new TemplarPower[]
		{
			new("locate", "Lirathan Mind Locate", "psioniclocate", "locate", "arm.skill.locate", 8, MagicPowerDistance.SamePlaneOnly),
			new("mindblast", "Lirathan Mindblast", "psionicmindblast", "mindblast", "arm.skill.mindblast", 18, MagicPowerDistance.SamePlaneOnly,
				"<HealthFraction>0.25</HealthFraction><StunFraction>0.25</StunFraction>"),
			new("mindwipe", "Lirathan Mindwipe", "psionicmindwipe", "mindwipe", "arm.skill.mindwipe", 18, MagicPowerDistance.SamePlaneOnly,
				"<StaminaFraction>0.25</StaminaFraction><StunFraction>0.25</StunFraction>"),
			new("control", "Lirathan Mental Control", "psioniccontrol", "control", "arm.skill.control", 18, MagicPowerDistance.SameLocationOnly),
			new("vision", "Lirathan Mental Vision", "psionicvision", "masquerade", "arm.skill.masquerade", 14, MagicPowerDistance.SamePlaneOnly),
			new("illusion", "Lirathan Room Illusion", "psionicillusion", "illusion", "arm.skill.illusion", 10, MagicPowerDistance.SameLocationOnly),
			new("cathexis", "Lirathan Cathexis", "psioniccathexis", "cathexis", "arm.skill.cathexis", 8, MagicPowerDistance.SamePlaneOnly),
			new("dome", "Lirathan Psionic Dome", "psionicdome", "dome", "arm.skill.dome", 8, MagicPowerDistance.SameLocationOnly, Sustained: true)
		};
		var powers = new Dictionary<string, MagicPower>(StringComparer.Ordinal);
		foreach (var spec in target)
		{
			if (!skills.TryGetValue(spec.SkillKey, out var trait)) throw new InvalidOperationException($"Missing source Lirathan skill {spec.SkillKey}.");
			var definition = NewBasePowerDefinition(spec.Key, canInvoke.Id, unavailable.Id, trace);
			if (spec.Sustained)
			{
				definition.Add(new XElement("BeginVerb", "dome"), new XElement("EndVerb", "enddome"),
					new XElement("SkillCheckTrait", trait.Id), new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
					new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass), new XElement("BeginEmote", "You establish a psionic dome over the room."),
					new XElement("EndEmote", "You release the psionic dome."), new XElement("FailEmote", "Your effort to form a dome slips away."),
					new XElement("Duration", 300000), new XElement("ConcentrationPointsToSustain", 1), new XElement("SustainPenalty", -2),
					new XElement("SustainResourceCosts"));
				definition.Add(new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal));
			}
			else if (spec.Model == "psioniccathexis" || spec.Model == "psionicillusion")
			{
				definition.Add(new XElement("SkillCheckTrait", trait.Id), new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
					new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass));
				if (spec.Model == "psionicillusion") definition.Add(new XElement("Verbs", new XElement("Verb", "illusion")));
			}
			else
			{
				definition.Add(new XElement("Verb", spec.Verb), new XElement("PowerDistance", (int)spec.Distance),
					new XElement("SkillCheckDifficulty", (int)Difficulty.Normal), new XElement("SkillCheckTrait", trait.Id),
					new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass), new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
					new XElement("FailEcho", "You fail to shape the thought."), new XElement("SuccessEcho", "You focus your mind and the Way answers."));
				if (spec.Extra is not null) definition.Add(XElement.Parse("<Extras>" + spec.Extra + "</Extras>").Elements().ToArray());
			}
			definition.Add(new XElement("BodilyCost", new XAttribute("base", spec.Cost), new XAttribute("skillTrait", trait.Id),
				new XAttribute("willpowerTrait", willpower.Id), new XAttribute("verbs", spec.Verb)));
			if (spec.Sustained || spec.Model == "psioniccathexis" || spec.Model == "psionicillusion")
				definition.Add(new XElement("StunUpkeep", new XAttribute("skillTrait", trait.Id), new XAttribute("wisdomTrait", wisdom.Id),
					new XAttribute("freeProg", freeUpkeep.Id), new XAttribute("attemptsPerMinute", 60)));
			var row = Put(db, "arm.power.lirathan." + spec.Key, new MagicPower { Name = spec.Name.Replace("Lirathan", "Armageddon", StringComparison.Ordinal), MagicSchoolId = school.Id,
				PowerModel = spec.Model, Blurb = "A technique of the Way, powered by mental effort.",
				ShowHelp = $"Use LPSI {spec.Verb.ToUpperInvariant()} with the target or invocation described by the power.", Definition = definition.ToString() },
				[nameof(MagicPower.Name), nameof(MagicPower.MagicSchoolId), nameof(MagicPower.PowerModel), nameof(MagicPower.Blurb), nameof(MagicPower.ShowHelp), nameof(MagicPower.Definition)], messages);
			powers.Add(spec.Key, row);
		}

		var stockMapping = new (string Verb, string Trait, bool Advanced, string? VerbOverride, bool? Thinks, bool? Feels)[]
		{
			("contact", "arm.skill.contact", true, null, null, null), ("barrier", "arm.skill.barrier", true, null, null, null),
			("connectback", "arm.skill.contact", true, null, null, null), ("say", "arm.skill.contact", true, null, null, null),
			("expel", "arm.skill.expel", true, null, null, null), ("allspeak", "arm.skill.allspeak", true, null, null, null),
			("hear", "arm.skill.hear", true, null, true, true),
			("hear", "arm.skill.thoughtsense", true, "thoughtsense", true, false), ("hear", "arm.skill.empathy", true, "empathy", false, true),
			("clairaudience", "arm.skill.clairaudience", true, null, null, null), ("trace", "arm.skill.trace", true, null, null, null),
			("magicksense", "arm.skill.magicksense", true, null, null, null)
		};
		foreach (var entry in stockMapping)
		{
			var stock = PsionicStockContent.Powers.Single(x => x.Verb == entry.Verb);
			var trait = skills[entry.Trait];
			var xml = PsionicStockContent.Definition(stock, trait.Id, sourceResource, canInvoke.Id, canInvoke.Id,
				unavailable.Id, normalDifficulty.Id, advanced: entry.Advanced);
			xml.Element("InvocationCosts")?.Remove();
		xml.Element("SustainResourceCosts")?.Remove();
			xml.SetElementValue("Resource", sourceResource);
			xml.SetElementValue("SkillCheckTrait", trait.Id);
			if (entry.VerbOverride is not null)
			{
				xml.SetElementValue("BeginVerb", entry.VerbOverride);
				xml.SetElementValue("EndVerb", "end" + entry.VerbOverride);
			}
			if (entry.Trait is "arm.skill.thoughtsense" or "arm.skill.empathy") xml.SetElementValue("PowerDistance", MagicPowerDistance.SameLocationOnly);
			if (entry.Thinks is { } thinks) xml.SetElementValue("ShowThinks", thinks);
			if (entry.Feels is { } feels) xml.SetElementValue("ShowFeels", feels);
			var invocationVerb = entry.VerbOverride ?? entry.Verb;
			xml.Add(new XElement("BodilyCost", new XAttribute("base", stock.Cost switch { <= 2 => 8, <= 3 => 10, <= 5 => 14, _ => 18 }),
				new XAttribute("skillTrait", trait.Id), new XAttribute("willpowerTrait", willpower.Id), new XAttribute("verbs", invocationVerb == "say" ? "say,tell" : invocationVerb)));
			if (stock.Seconds > 0) xml.Add(new XElement("StunUpkeep", new XAttribute("skillTrait", trait.Id), new XAttribute("wisdomTrait", wisdom.Id),
				new XAttribute("freeProg", freeUpkeep.Id), new XAttribute("attemptsPerMinute", 60)));
			var key = entry.Verb is "say" or "connectback" ? entry.Verb : entry.Trait.Split('.').Last();
			var definition = xml.ToString();
			var row = Put(db, "arm.power.lirathan." + key, new MagicPower { Name = "Armageddon " + key, MagicSchoolId = school.Id,
				PowerModel = stock.Type, Blurb = stock.Help.Replace("{range}", "the same plane", StringComparison.Ordinal),
				ShowHelp = stock.Help, Definition = definition },
				[nameof(MagicPower.Name), nameof(MagicPower.MagicSchoolId), nameof(MagicPower.PowerModel), nameof(MagicPower.Blurb), nameof(MagicPower.ShowHelp), nameof(MagicPower.Definition)], messages);
			powers.Add(key, row);
		}
		InstallLirathanShadowwalk(db, school, skills["arm.skill.shadowwalk"], canInvoke.Id, unavailable.Id, powers, messages);
		var silentTrait = skills["arm.skill.silent_psi"];
		var silentXml = NewBasePowerDefinition("silentpsi", canInvoke.Id, unavailable.Id, trace);
		silentXml.Add(new XElement("BeginVerb", "silentpsi"), new XElement("EndVerb", "endsilentpsi"),
			new XElement("SkillCheckTrait", silentTrait.Id), new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
			new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass), new XElement("ConcentrationPointsToSustain", 1),
			new XElement("SustainPenalty", -2), new XElement("DetectableWithDetectMagic", (int)Difficulty.Hard), new XElement("SustainResourceCosts"),
			new XElement("AllSchools", true), new XElement("HideActivity", true), new XElement("AuditDifficultyStages", 2),
			new XElement("BodilyCost", new XAttribute("base", 10), new XAttribute("skillTrait", silentTrait.Id), new XAttribute("willpowerTrait", willpower.Id), new XAttribute("verbs", "silentpsi")),
			new XElement("StunUpkeep", new XAttribute("skillTrait", silentTrait.Id), new XAttribute("wisdomTrait", wisdom.Id), new XAttribute("freeProg", freeUpkeep.Id), new XAttribute("attemptsPerMinute", 60)));
		powers.Add("silentpsi", (MagicPower)Put(db, "arm.power.lirathan.silentpsi", new MagicPower { Name = "Lirathan Silent Psi", MagicSchoolId = school.Id,
			PowerModel = "mindconceal", Blurb = "Conceal mental identity and activity.", ShowHelp = "Use LPSI SILENTPSI or LPSI ENDSILENTPSI to veil or reveal your mental signature.", Definition = silentXml.ToString() },
			[nameof(MagicPower.Name), nameof(MagicPower.MagicSchoolId), nameof(MagicPower.PowerModel), nameof(MagicPower.Blurb), nameof(MagicPower.ShowHelp), nameof(MagicPower.Definition)], messages));

		var allPowers = powers.Select(x => (Key: x.Key, Power: x.Value, Trait: skills[TraitForPower(x.Key)] )).ToArray();
		var capabilityDefinition = new XElement("Definition", new XElement("ConcentrationTrait", skills["arm.skill.allspeak"].Id),
			new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
			allPowers.Select(x => new XElement("Power", new XAttribute("trait", x.Trait.Id), new XAttribute("minvalue", 1), new XAttribute("power", x.Power.Id))));
		var capability = (MagicCapability)Put(db, "arm.capability.lirathan_templar_psionics", new MagicCapability { Name = "Armageddon Lirathan Templar Psionics",
			CapabilityModel = "skilllevel", PowerLevel = 2, MagicSchoolId = school.Id, Definition = capabilityDefinition.ToString() },
			[nameof(MagicCapability.Name), nameof(MagicCapability.CapabilityModel), nameof(MagicCapability.PowerLevel), nameof(MagicCapability.MagicSchoolId), nameof(MagicCapability.Definition)], messages);
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse");
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue");
		var merit = (Merit)Put(db, "arm.merit.lirathan_templar_psionics", new Merit { Name = "Armageddon Lirathan Templar Psionics", Type = "Magic Capability",
			MeritType = 0, MeritScope = 2, Definition = new XElement("Definition", new XElement("ChargenAvailableProg", no.Id), new XElement("ApplicabilityProg", yes.Id),
				new XElement("ChargenBlurb", "Lirathan Templar Way powers; class selection supplies this configured capability."),
				new XElement("DescriptionText", "@ command|commands the Way through Lirathan psionic techniques."), new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() },
			["Name", "Type", "MeritType", "MeritScope", "Definition"], messages);
		var role = roles[lirathanRole.Key];
		InstallCommonPsionics(db, school, skills, powers, canInvoke.Id, unavailable.Id, willpower.Id, wisdom.Id, freeUpkeep.Id, sourceResource, normalDifficulty.Id, messages);
		messages.Add($"Lirathan Templar psionic capability installed with {allPowers.Length} per-skill powers, source bodily costs, and a builder-configurable free-upkeep territory binding.");
	}

	private static XElement NewBasePowerDefinition(string key, long canInvoke, long unavailable, XElement trace) => new("Definition",
		new XElement("SeededIdentity", "arm.psionic.lirathan." + key), new XElement("PowerTags", new XElement("Tag", "psionic")),
		new XElement("IsPsionic", true), new XElement(trace), new XElement("CanInvokePowerProg", canInvoke), new XElement("WhyCantInvokePowerProg", unavailable),
		new XElement("InvocationCosts", new XElement("Verbs")));

	private static TraitDefinition FindAttribute(FuturemudDatabaseContext db, params string[] names)
	{
		foreach (var name in names)
			if (db.TraitDefinitions.AsEnumerable().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && (x.Type == 1 || x.Type == 3)) is { } trait) return trait;
		throw new InvalidOperationException("Lirathan psionic bodily cost needs a canonical Willpower/Wisdom attribute trait.");
	}

	private static string TraitForPower(string key) => key switch
	{
		"contact" or "say" or "connectback" => "arm.skill.contact", "barrier" => "arm.skill.barrier", "expel" => "arm.skill.expel", "allspeak" => "arm.skill.allspeak",
		"thoughtsense" => "arm.skill.thoughtsense", "empathy" => "arm.skill.empathy", "clairaudience" => "arm.skill.clairaudience", "trace" => "arm.skill.trace",
		"magicksense" => "arm.skill.magicksense", "locate" => "arm.skill.locate", "mindblast" => "arm.skill.mindblast", "mindwipe" => "arm.skill.mindwipe",
		"control" => "arm.skill.control", "vision" => "arm.skill.masquerade", "illusion" => "arm.skill.illusion", "cathexis" => "arm.skill.cathexis", "dome" => "arm.skill.dome",
		"shadowwalk" => "arm.skill.shadowwalk", "hear" => "arm.skill.hear", "silentpsi" => "arm.skill.silent_psi",
		_ => throw new InvalidOperationException("Unknown Lirathan power skill binding: " + key)
	};

	private static FutureProg TextOrBooleanPowerProg(FuturemudDatabaseContext db, string key, string text, ProgVariableTypes type, ICollection<string> messages)
	{
		var returnDefinition = type.ToStorageString();
		var actual = (FutureProg)Put(db, key, new FutureProg { FunctionName = type == ProgVariableTypes.Boolean ? "LirathanTemplarPowerAllowed" : "LirathanTemplarPowerUnavailable",
			FunctionText = text, ReturnType = (long)type, ReturnTypeDefinition = returnDefinition, Category = "Armageddon", Subcategory = "Lirathan Psionics",
			FunctionComment = "Open invocation prog for the installed Lirathan Templar powers.", Public = false, AcceptsAnyParameters = true, StaticType = 0 },
			["FunctionName", "FunctionText", "ReturnTypeDefinition", "Category", "Subcategory", "FunctionComment", "Public", "AcceptsAnyParameters", "StaticType"], messages);
		return actual;
	}
}

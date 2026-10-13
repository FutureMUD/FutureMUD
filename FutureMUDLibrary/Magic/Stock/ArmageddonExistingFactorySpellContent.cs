#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public sealed record ArmageddonFlameKnifeBindings(IReadOnlyList<long> TemporaryKnives,
	IReadOnlyList<long> PermanentStaffs, long ConjurationRankSixTagId, long EligibilityProgId,
	long LifetimeMultiplierProgId);
public sealed record ArmageddonServitorBindings(long ControlEligibilityProgId, long CommandableAiId, long CombatEndAiId);
public sealed record ArmageddonFuryBindings(long OrdinaryAttributeId, long EligibilityProgId);
public sealed record ArmageddonCalmBindings(long OpposedTraitId, long EligibilityProgId, IReadOnlyDictionary<int, Difficulty> Saves);

/// <summary>Pure locked S4 definitions. Callers resolve typed native dependencies; no identities are invented here.</summary>
public static class ArmageddonExistingFactorySpellContent
{
	public const string WeaponLifetime = "(grade+1)*1500*0.75";
	public const string StormLifetime = "(24+rand(1,6))*90*grade*0.75";
	public const string GradeLifetime = "60*grade";
	public static IReadOnlyList<string> Keys => ["arm.spell.sand_effigy", "arm.spell.walking_shadow",
		"arm.spell.raise_servitor", "arm.spell.roused_fury", "arm.spell.still_anger", "arm.spell.flame_knife",
		"arm.spell.sand_knife", "arm.spell.storm_spear"];
	public static IReadOnlyList<string> PendingKeys => [];
	public static IReadOnlyList<string> ServitorCommands => ["follow", "hit", "flee", "stop", "stand", "sit",
		"kneel", "emote", "get", "drop", "give", "wear", "remove", "wield", "unwield"];
	public const string ServitorCommandSource = "return cancommandanimation(@servitor, @commander)";
	public const string ServitorEligibilitySource = "return lowercase(@corpse.location.terrain.name) != \"silt\" and lowercase(@corpse.location.terrain.name) != \"shallows\"";
	public const string FlameEligibilitySource = "return lowercase(@caster.location.terrain.name) != \"water plane\"";
	public const string FlameMultiplierSource = "if (lowercase(@caster.location.terrain.name) == \"shadow plane\")\nreturn 0.5\nend if\nreturn 1";
	public static ArmageddonUtilitySpellContent SandEffigy(long planeId, long anchorPrototypeId)
	{
		var policy = new SpellProjectionConfiguration(SpellProjectionKind.SandEffigy, planeId, anchorPrototypeId, 60, 0, 0, RequiresHeldSourceItem: true);
		if (policy.Error(7) is { } error) throw new ArgumentException(error);
		return new("arm.spell.sand_effigy", "Sand Effigy",
			"Borrows one exact persisted ordinary item held by your original primary body to supply a figurine anchor and an immobile secondary presence for 60*grade seconds. Identity, skills and resources remain shared; inventory is never cloned. Source loss or drift, damage or created-anchor loss collapses the owned presence; the source item is never claimed or deleted.",
			GradeLifetime, 20, "$0 shape|shapes a sand effigy through $1.", (resource, cost, _) =>
				Base("arm.spell.sand_effigy", "item", resource, cost, 20,
					new XElement("Effect", new XAttribute("type", "createprojection"), new XAttribute("version", 1), SpellProjectionAnchor.Definition(policy))));
	}
	public static ArmageddonUtilitySpellContent WalkingShadow(long planeId) =>
		Projection(new(SpellProjectionKind.WalkingShadow, planeId, 0, 60, 10, 0, RoomDistancePerGrade: 10), "Walking Shadow", 7);
	private static ArmageddonUtilitySpellContent Projection(SpellProjectionConfiguration policy, string name, double minimum)
	{
		if (policy.Error(7) is { } error) throw new ArgumentException(error);
		const string key = "arm.spell.walking_shadow";
		return new(key, name,
			"Creates one focused walking shadow for 60*grade seconds, bounded to 10*grade native room edges from the primary anchor. Your tranced primary remains vulnerable; physical combat, manipulation and teleportation are unavailable.",
			GradeLifetime, minimum, "$0 shape|shapes a temporary presence.", (resource, cost, _) =>
				Base(key, "self", resource, cost, minimum,
					new XElement("Effect", new XAttribute("type", "createprojection"), new XAttribute("version", 1), SpellProjectionAnchor.Definition(policy))));
	}

	public static ArmageddonUtilitySpellContent SandKnife(IReadOnlyDictionary<int, long> prototypes)
	{
		var outputs = GradeItems(prototypes);
		return Weapon("arm.spell.sand_knife", "Sand Knife", 7, WeaponLifetime, "standard", outputs,
			"Creates one usable temporary sand knife of the requested grade. Every grade expires; no permanent output or component is required.");
	}

	public static ArmageddonUtilitySpellContent StormSpear(IReadOnlyDictionary<int, long> prototypes)
	{
		var outputs = GradeItems(prototypes);
		return Weapon("arm.spell.storm_spear", "Storm Spear", 20, StormLifetime, "primaryhand", outputs,
			"Creates one temporary electrical spear of the requested grade in the recipient's free primary hand. Missing hand capacity refuses before payment.") with { DurationFormula = "0" };
	}

	public static ArmageddonUtilitySpellContent FlameKnife(ArmageddonFlameKnifeBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		if (bindings.TemporaryKnives.Count != 6 || bindings.PermanentStaffs.Count != 8 ||
			bindings.TemporaryKnives.Concat(bindings.PermanentStaffs).Any(x => x <= 0) ||
			bindings.TemporaryKnives.Concat(bindings.PermanentStaffs).Distinct().Count() != 14 ||
			bindings.ConjurationRankSixTagId <= 0 || bindings.EligibilityProgId <= 0 || bindings.LifetimeMultiplierProgId <= 0)
			throw new ArgumentException("Flame Knife requires six distinct temporary knives, eight distinct permanent staves, a native rank-six tag and two native environment progs.");
		var knives = bindings.TemporaryKnives.ToArray();
		var staffs = bindings.PermanentStaffs.ToArray();
		return new("arm.spell.flame_knife", "Flame Knife",
			"Grades one through six create one temporary burning knife. Grade seven consumes one directly carried Conjuration rank-six descendant and chooses one of eight permanent burning staves. Water Plane refuses and Shadow Plane halves temporary lifetime.",
			WeaponLifetime, 7, "$0 gather|gathers flames into a weapon.", (resource, cost, _) =>
			{
				var definition = Base("arm.spell.flame_knife", "character", resource, cost, 7,
					ItemEffect("flame-knife", WeaponLifetime, "standard", knives[0]));
				var lifecycle = definition.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!;
				lifecycle.Add(new XElement("PermanentOutput", new XAttribute("grade", 7), staffs[0]),
					new XElement("EligibilityProg", bindings.EligibilityProgId),
					new XElement("LifetimeMultiplierProg", bindings.LifetimeMultiplierProgId),
					new XElement("GradeOutputs", knives.Select((id, index) => Output(index + 1, [id])), Output(7, staffs)));
				definition.Element("Plan")!.Add(new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
					new XAttribute("tag", bindings.ConjurationRankSixTagId), new XAttribute("secondtag", 0),
					new XAttribute("quantity", 1), new XAttribute("carriedonly", true), new XAttribute("grade", 7))));
				return definition;
			});
	}

	public static ArmageddonUtilitySpellContent RaiseServitor(ArmageddonServitorBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		if (bindings.ControlEligibilityProgId <= 0 || bindings.CommandableAiId <= 0 || bindings.CombatEndAiId <= 0 ||
			bindings.CommandableAiId == bindings.CombatEndAiId)
			throw new ArgumentException("Select native corpse control eligibility and distinct Commandable/CombatEnd AI identities.");
		return new("arm.spell.raise_servitor", "Raise Servitor",
			"Animates one eligible final-death corpse with its exact original body and possessions. Native creator commands and animation last 60*grade seconds; retirement restores the same corpse and custody.",
			GradeLifetime, 7, "$0 call|calls a borrowed body into motion.", (resource, cost, _) =>
				Base("arm.spell.raise_servitor", "item", resource, cost, 7,
					new XElement("Effect", new XAttribute("type", "animatecorpse"),
						new XElement("AllowPCs", true), new XElement("AllowNPCs", true), new XElement("AllowAdmins", false),
						new XElement("AllowFinal", true), new XElement("AllowSkeletal", false),
						new XElement("ArtificialIntelligences", new XElement("AI", bindings.CommandableAiId), new XElement("AI", bindings.CombatEndAiId)),
						new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
							new XElement("Family", "raise-servitor"), new XElement("Seconds", GradeLifetime),
							new XElement("Control", new XElement("Seconds", GradeLifetime), new XElement("EligibilityProg", bindings.ControlEligibilityProgId)),
							new XElement("FollowCaster", true)))));
	}

	public static XElement ServitorCommandAi(long commandProgId)
	{
		if (commandProgId <= 0) throw new ArgumentOutOfRangeException(nameof(commandProgId));
		return new("Definition", new XElement("CanCommandProg", commandProgId), new XElement("WhyCannotCommandProg", 0),
			new XElement("CommandIssuedEmote", ""), new XElement("BannedCommands"),
			new XElement("IncludedCommands", ServitorCommands.Select(x => new XElement("Command", x))));
	}
	public static XElement ServitorCombatAi(long alwaysTrueProgId)
	{
		if (alwaysTrueProgId <= 0) throw new ArgumentOutOfRangeException(nameof(alwaysTrueProgId));
		return new("Definition", new XElement("WillAcceptTruce", alwaysTrueProgId), new XElement("WillAcceptTargetIncapacitated", alwaysTrueProgId),
			new XElement("OnOfferedTruce", 0), new XElement("OnTargetIncapacitated", 0), new XElement("OnNoNaturalTargets", 0));
	}

	public static ArmageddonUtilitySpellContent RousedFury(ArmageddonFuryBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		if (bindings.OrdinaryAttributeId <= 0 || bindings.EligibilityProgId <= 0) throw new ArgumentException("Select an ordinary native attribute and eligibility prog.");
		return Emotion("roused_fury", "Roused Fury", "Fury", 20, bindings.OrdinaryAttributeId, bindings.EligibilityProgId, [],
			"Applies native Fury with an ordinary attribute bonus equal to requested grade for 60*grade seconds. Stamina and combat remain native and are never reset.");
	}
	public static ArmageddonUtilitySpellContent StillAnger(ArmageddonCalmBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		if (bindings.OpposedTraitId <= 0 || bindings.EligibilityProgId <= 0 || bindings.Saves.Count != 7 ||
			Enumerable.Range(1, 7).Any(x => !bindings.Saves.TryGetValue(x, out var value) || !Enum.IsDefined(value)))
			throw new ArgumentException("Select a native opposed trait, eligibility prog and seven exact grade saves.");
		return Emotion("still_anger", "Still Anger", "Calm", 7, bindings.OpposedTraitId, bindings.EligibilityProgId,
			bindings.Saves.OrderBy(x => x.Key).ToArray(),
			"Applies native selective Calm for 60*grade seconds. Source counters precede the residual save; prepared combat cessation precedes payment. Admitted incoming hostility, including a missed attack, breaks it.");
	}
	private static ArmageddonUtilitySpellContent Emotion(string suffix, string name, string kind, double minimum,
		long trait, long eligibility, IReadOnlyList<KeyValuePair<int, Difficulty>> saves, string description) =>
		new("arm.spell." + suffix, name, description, "0", minimum, "$0 shape|shapes an emotional enchantment.",
			(resource, cost, _) =>
			{
				var definition = Base("arm.spell." + suffix, "character", resource, cost, minimum,
					new XElement("Effect", new XAttribute("type", kind == "Fury" ? "sourcefury" : "sourcecalm"),
						new XElement("SourceProfile", new XAttribute("version", 1), new XAttribute("kind", kind),
							new XAttribute("eligibility", eligibility), new XAttribute("intensity", kind == "Fury" ? 5 : 11), new XAttribute("trait", trait),
							new XAttribute("units", kind == "Fury" ? 1 : 0), new XAttribute("attackbreak", kind == "Calm"),
							new XAttribute("gradepolicy", "Canonical"),
							new XElement("Lifetime", new XAttribute("group", "armageddon." + suffix), new XAttribute("seconds", 60), new XAttribute("cap", 7)),
							new XElement("Fallback", new XAttribute("numerator", kind == "Fury" ? 1 : 2), new XAttribute("denominator", 1), new XAttribute("endurance", 0)),
							new XElement("Terrains"), new XElement("Saves", saves.Select(x => new XElement("Grade", new XAttribute("number", x.Key), new XAttribute("difficulty", (int)x.Value)))),
							new XElement("HistoricalExceptions", new XAttribute("status", "unmapped")))));
				definition.Element("Trigger")!.Element("TargetFilterProg")!.Value = eligibility.ToString(System.Globalization.CultureInfo.InvariantCulture);
				return definition;
			});

	private static Dictionary<int, long> GradeItems(IReadOnlyDictionary<int, long> source)
	{
		ArgumentNullException.ThrowIfNull(source);
		if (source.Count != 7 || Enumerable.Range(1, 7).Any(x => !source.TryGetValue(x, out var id) || id <= 0) || source.Values.Distinct().Count() != 7)
			throw new ArgumentException("Select seven distinct positive native item prototype IDs by external requested grade.");
		return source.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
	}
	private static ArmageddonUtilitySpellContent Weapon(string key, string name, double minimum, string lifetime,
		string placement, Dictionary<int, long> outputs, string description) => new(key, name, description, lifetime, minimum,
		"$0 shape|shapes a magical weapon.", (resource, cost, _) =>
		{
			var definition = Base(key, "character", resource, cost, minimum,
				ItemEffect(key[10..].Replace('_', '-'), lifetime, placement, outputs[1]));
			definition.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!.Add(
				new XElement("GradeOutputs", outputs.Select(x => Output(x.Key, [x.Value]))));
			return definition;
		});
	private static XElement ItemEffect(string family, string lifetime, string placement, long prototype) =>
		new("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", "base"), new XElement("ItemPrototypeId", prototype),
			new XElement("ItemSkinId", 0), new XElement("Quantity", 1), new XElement("LoadString", ""),
			new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
				new XElement("Family", family), new XElement("Seconds", lifetime), placement == "standard" ? null : new XElement("Placement", placement)));
	private static XElement Output(int grade, IEnumerable<long> prototypes) => new("Output", new XAttribute("grade", grade), prototypes.Select(x => new XElement("Prototype", x)));
	private static XElement Base(string key, string trigger, long resource, long cost, double minimum, XElement effect) =>
		ArmageddonUtilitySpellContent.Definition(key, trigger, resource, cost, 0, 30, minimum, effect);
}

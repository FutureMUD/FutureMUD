#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic.Lifecycle;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace MudSharp.Magic;

/// <summary>Source-informed stock using a declared native weapon profile and Creation rank tags.</summary>
public static class ArmageddonStormSpearStock
{
	public const string Key = "arm.spell.storm_spear";
	public const string Name = "Storm Spear";
	// Recovered event heap: four units per twelve quarter-second pulses, nominally 0.75 seconds/unit.
	// Native policy uses that nominal conversion as an exact absolute deadline; no batching/load delay.
	public const string LifetimeSeconds = "(24+rand(1,6))*90*grade*0.75";
	public const double MinimumEnergy = 20;

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, IGameItemProto weapon, IReadOnlyList<ITag> creationRanks)
	{
		if (world.SpellOwnedItems is null || !ReferenceEquals(school.Gameworld, world) || world.Traits.Get(trait.Id) != trait ||
			world.MagicResources.Get(resource.Id) != resource || NativeItemCreationEligibility.Error(weapon, world) is { })
			throw new InvalidOperationException("Select existing world definitions and an approved plain native weapon prototype.");
		var melee = weapon.GetItemType<MeleeWeaponGameItemComponentProto>();
		if (melee?.WeaponType is null || !melee.WeaponType.Attacks.Any(x => x.MoveType == BuiltInCombatMoveType.UseWeaponAttack &&
			x.HandednessOptions is AttackHandednessOptions.Any or AttackHandednessOptions.OneHandedOnly) ||
			melee.WeaponType.Attacks.Any(x => x.Profile.DamageType != DamageType.Electrical))
			throw new InvalidOperationException("The Storm Spear prototype needs a usable native electrical melee weapon profile; historical JavaScript damage is unavailable.");
		if (creationRanks.Count != 5 || creationRanks.Any(x => world.Tags.Get(x.Id) != x) ||
			creationRanks.Select(x => x.Id).Distinct().Count() != 5 ||
			creationRanks.Skip(1).Where((tag, index) => !tag.IsA(creationRanks[index])).Any())
			throw new InvalidOperationException("Select five distinct Creation rank tags, zero through four, with each higher rank a descendant of the previous rank.");
		if (world.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(Name)))
			throw new InvalidOperationException("Storm Spear already exists in that school; edit or clone it instead.");
		var no = world.AlwaysFalseProg ?? throw new InvalidOperationException("The always-false support prog is unavailable.");
		using var scope = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction();
		if (FMDB.Context.MagicSpells.Any(x => x.MagicSchoolId == school.Id && x.Name == Name))
			throw new InvalidOperationException("A persisted Storm Spear already exists in that school.");
		var row = new Db.MagicSpell
		{
			Name = Name, MagicSchoolId = school.Id, CastingTraitDefinitionId = trait.Id, SpellKnownProgId = no.Id,
			Blurb = "Conjure and wield a temporary spear of lightning.",
			Description = "Consume one directly carried Creation component and conjure a spear into the recipient's free primary hand. " +
				"Creation rank must be at least grade minus three, with ranks below zero using rank zero. All grades expire. " +
				"The native electrical weapon profile is a builder-authored adaptation; its damage is not recovered historical JavaScript. " +
				"Expiry uses the recovered event loop's nominal conversion and preserves foreign possessions. Configure capability membership and acquisition separately.",
			CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			CastingEmote = "$0 draw|draws a spear from crackling lightning.", FailCastingEmote = "$0 fail|fails to bind the lightning.",
			TargetEmote = "", TargetNullEmote = "The lightning does not form.", TargetResistedEmote = "",
			AppliedEffectsAreExclusive = true, ScrollInscriptionAllowed = false, Definition = "<Definition />"
		};
		FMDB.Context.MagicSpells.Add(row); FMDB.Context.SaveChanges();
		var duration = new Db.TraitExpression { Name = $"Storm Spear #{row.Id} duration", Expression = LifetimeSeconds };
		var cost = new Db.TraitExpression { Name = $"Storm Spear #{row.Id} energy", Expression = "20*grade" };
		FMDB.Context.TraitExpressions.AddRange(duration, cost); FMDB.Context.SaveChanges();
		row.EffectDurationExpressionId = duration.Id;
		row.Definition = Definition(resource.Id, cost.Id, weapon.Id, creationRanks.Select(x => x.Id).ToArray()).ToString();
		FMDB.Context.SaveChanges(); transaction.Commit();
		world.Add(new TraitExpression(duration, world)); world.Add(new TraitExpression(cost, world));
		var result = new MagicSpell(row, world); world.Add(result); return result;
	}

	internal static XElement Definition(long resource, long costExpression, long prototype, IReadOnlyList<long> ranks) => new("Definition",
		new XElement("StockIdentity", Key),
		new XElement("Trigger", new XAttribute("type", "character"), new XElement("MinimumPower", (int)SpellPower.ExtremelyWeak),
			new XElement("MaximumPower", (int)SpellPower.ExtremelyStrong), new XElement("TargetFilterProg", 0), new XElement("CanTargetSelf", true)),
		new XElement("Costs", new XElement("Cost", new XAttribute("resource", resource), new XAttribute("expression", costExpression))),
		new XElement("Effects", new XElement("Effect", new XAttribute("type", "createitem"),
			new XElement("ItemQuality", "base"), new XElement("ItemPrototypeId", prototype), new XElement("ItemSkinId", 0),
			new XElement("Quantity", 1), new XElement("LoadString", ""),
			new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
				new XElement("Family", "storm-spear"), new XElement("Seconds", LifetimeSeconds), new XElement("Placement", "primaryhand")))),
		new XElement("CasterEffects"),
		new XElement("Plan", new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
			new XAttribute("tag", ranks[0]), new XAttribute("secondtag", 0), new XAttribute("quantity", 1), new XAttribute("carriedonly", true),
			new XElement("GradeRank", new XAttribute("offset", -3), ranks.Select((tag, rank) =>
				new XElement("Rank", new XAttribute("minimum", rank), new XAttribute("tag", tag))))))),
		new XElement("ControlledPower", new XAttribute("schema", 1), new XAttribute("version", 1),
			new XAttribute("overreachCost", 1.5), new XAttribute("overreachDifficulty", 1), new XAttribute("masteryChance", 0.25),
			new XAttribute("masterySeconds", 600), new XAttribute("skillSeconds", 60), new XAttribute("openingSkill", 30),
			new XElement("Efficiency", new XAttribute("type", "source"), new XAttribute("minimum", MinimumEnergy), new XAttribute("scale", 1)),
			new[] { 0, 20, 40, 55, 70, 85, 95 }.Select((skill, index) => new XElement("Grade", new XAttribute("number", index + 1),
				new XAttribute("power", (int)SpellPower.ExtremelyWeak + index), new XAttribute("skill", skill), new XAttribute("difficulty", index / 2))),
			new XElement("ScalarBindings")));
}

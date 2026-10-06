#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic.Lifecycle;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace MudSharp.Magic;

/// <summary>Recovered Fire Jambiya grade/component/lifetime rules with explicit authored native profiles.</summary>
public static class ArmageddonFlameKnifeStock
{
	public const string Key = "arm.spell.flame_knife";
	public const string Name = "Flame Knife";
	public const string LifetimeSeconds = "(grade+1)*1500*0.75";
	public const double MinimumEnergy = 7;
	public const string EligibilitySource = "return lowercase(@caster.location.terrain.name) != \"water plane\"";
	public const string MultiplierSource = "if (lowercase(@caster.location.terrain.name) == \"shadow plane\")\nreturn 0.5\nend if\nreturn 1";

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, IReadOnlyList<IGameItemProto> knives, IReadOnlyList<IGameItemProto> staffs, ITag monComponent)
	{
		if (world.SpellOwnedItems is null || !ReferenceEquals(school.Gameworld, world) || world.Traits.Get(trait.Id) != trait ||
			world.MagicResources.Get(resource.Id) != resource || world.Tags.Get(monComponent.Id) != monComponent)
			throw new InvalidOperationException("Select existing world definitions and the Conjuration component rank-six tag.");
		if (knives.Count != 6 || staffs.Count != 8 || knives.Concat(staffs).Select(x => x.Id).Distinct().Count() != 14)
			throw new InvalidOperationException("Select six distinct temporary grade prototypes and eight distinct permanent staff prototypes.");
		foreach (var prototype in knives.Concat(staffs))
		{
			if (NativeItemCreationEligibility.Error(prototype, world) is { } error) throw new InvalidOperationException(error);
			var weapon = prototype.GetItemType<MeleeWeaponGameItemComponentProto>()?.WeaponType;
			if (weapon is null || !weapon.Attacks.Any(x => x.MoveType == BuiltInCombatMoveType.UseWeaponAttack) ||
				weapon.Attacks.Any(x => x.Profile.DamageType != DamageType.Burning))
				throw new InvalidOperationException("Select usable native burning melee profiles. Historical objects 462-467 and 1386-1393 are unavailable; profiles are explicitly authored adaptations.");
		}
		if (world.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(Name)))
			throw new InvalidOperationException("Flame Knife already exists in that school; edit or clone it instead.");
		var no = world.AlwaysFalseProg ?? throw new InvalidOperationException("The always-false support prog is unavailable.");
		using var scope = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB(); using var transaction = FMDB.Context.Database.BeginTransaction();
		if (FMDB.Context.MagicSpells.Any(x => x.MagicSchoolId == school.Id && x.Name == Name))
			throw new InvalidOperationException("A persisted Flame Knife already exists in that school.");
		var row = new Db.MagicSpell
		{
			Name = Name, MagicSchoolId = school.Id, CastingTraitDefinitionId = trait.Id, SpellKnownProgId = no.Id,
			Blurb = "Create a temporary flame weapon, or a permanent staff at mon.",
			Description = "Grades one through six create their selected temporary flame weapon without a component. " +
				"Grade seven consumes one directly carried rank-six-or-higher Conjuration component and chooses one of eight permanent staffs. " +
				"Water Plane terrain blocks creation; Shadow Plane terrain halves temporary lifetime. Edit the support progs to map your world. " +
				"This bounded stock targets the caster and places normally in inventory or at their location; wield using native equipment commands. " +
				"Profiles are declared native policy; the printed source minimum is seven energy, distinct from C metadata minimum zero. Add acquisition separately.",
			CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			CastingEmote = "$0 gather|gathers flames into a weapon.", FailCastingEmote = "$0 fail|fails to shape the flames.",
			TargetEmote = "", TargetNullEmote = "The flames do not take shape.", TargetResistedEmote = "",
			AppliedEffectsAreExclusive = true, ScrollInscriptionAllowed = false, Definition = "<Definition />"
		};
		FMDB.Context.MagicSpells.Add(row); FMDB.Context.SaveChanges();
		Db.FutureProg Prog(string suffix, string text, ProgVariableTypes returnType)
		{
			var model = new Db.FutureProg { FunctionName = $"armflame_{row.Id}_{suffix}", FunctionText = text,
				ReturnTypeDefinition = returnType.ToStorageString(), FunctionComment = "Editable Flame Knife source environment mapping.",
				Category = "Magic", Subcategory = Name, Public = false };
			model.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			var check = new FutureProg.FutureProg(model, world);
			if (!check.Compile()) throw new InvalidOperationException("Stock environment prog failed: " + check.CompileError);
			FMDB.Context.FutureProgs.Add(model); return model;
		}
		var eligibility = Prog("environment", EligibilitySource, ProgVariableTypes.Boolean);
		var multiplier = Prog("lifetime", MultiplierSource, ProgVariableTypes.Number);
		var duration = new Db.TraitExpression { Name = $"Flame Knife #{row.Id} duration", Expression = LifetimeSeconds };
		var cost = new Db.TraitExpression { Name = $"Flame Knife #{row.Id} energy", Expression = "7*grade" };
		FMDB.Context.TraitExpressions.AddRange(duration, cost); FMDB.Context.SaveChanges();
		row.EffectDurationExpressionId = duration.Id;
		row.Definition = Definition(resource.Id, cost.Id, knives.Select(x => x.Id).ToArray(), staffs.Select(x => x.Id).ToArray(), monComponent.Id, eligibility.Id, multiplier.Id).ToString();
		FMDB.Context.SaveChanges(); transaction.Commit();
		foreach (var model in new[] { eligibility, multiplier }) { var prog = new FutureProg.FutureProg(model, world); prog.Compile(); world.Add(prog); }
		world.Add(new TraitExpression(duration, world)); world.Add(new TraitExpression(cost, world));
		var result = new MagicSpell(row, world); world.Add(result); return result;
	}

	internal static XElement Definition(long resource, long costExpression, IReadOnlyList<long> knives, IReadOnlyList<long> staffs, long monComponent, long eligibility, long multiplier)
	{
		var result = new XElement("Definition", new XElement("StockIdentity", Key),
			new XElement("Trigger", new XAttribute("type", "self"), new XElement("MinimumPower", (int)SpellPower.ExtremelyWeak), new XElement("MaximumPower", (int)SpellPower.ExtremelyStrong)),
			new XElement("Costs", new XElement("Cost", new XAttribute("resource", resource), new XAttribute("expression", costExpression))),
			new XElement("Effects", new XElement("Effect", new XAttribute("type", "createitem"),
				new XElement("ItemQuality", "base"), new XElement("ItemPrototypeId", knives[0]), new XElement("ItemSkinId", 0), new XElement("Quantity", 1), new XElement("LoadString", ""),
				new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
					new XElement("Family", "flame-knife"), new XElement("Seconds", LifetimeSeconds), new XElement("Placement", "standard")))),
			new XElement("CasterEffects"),
			new XElement("Plan", new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
				new XAttribute("tag", monComponent), new XAttribute("secondtag", 0), new XAttribute("quantity", 1), new XAttribute("carriedonly", true), new XAttribute("grade", 7)))),
			new XElement("ControlledPower", new XAttribute("schema", 1), new XAttribute("version", 1),
				new XAttribute("overreachCost", 1.5), new XAttribute("overreachDifficulty", 1), new XAttribute("masteryChance", 0.25), new XAttribute("masterySeconds", 600), new XAttribute("skillSeconds", 60), new XAttribute("openingSkill", 30),
				new XElement("Efficiency", new XAttribute("type", "source"), new XAttribute("minimum", MinimumEnergy), new XAttribute("scale", 1)),
				new[] { 0, 20, 40, 55, 70, 85, 95 }.Select((skill, index) => new XElement("Grade", new XAttribute("number", index + 1),
					new XAttribute("power", (int)SpellPower.ExtremelyWeak + index), new XAttribute("skill", skill), new XAttribute("difficulty", index / 2))), new XElement("ScalarBindings")));
		var lifecycle = result.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!;
		lifecycle.Element("Family")!.Value = "flame-knife"; lifecycle.Element("Seconds")!.Value = LifetimeSeconds;
		lifecycle.Element("Placement")!.Value = "standard";
		lifecycle.Add(new XElement("PermanentOutput", new XAttribute("grade", 7), staffs[0]),
			new XElement("EligibilityProg", eligibility), new XElement("LifetimeMultiplierProg", multiplier),
			new XElement("GradeOutputs", knives.Select((id, index) => new XElement("Output", new XAttribute("grade", index+1), new XElement("Prototype", id))),
				new XElement("Output", new XAttribute("grade", 7), staffs.Select(id => new XElement("Prototype", id)))));
		return result;
	}
}

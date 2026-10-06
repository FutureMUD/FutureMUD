#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Climate;
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

/// <summary>Recovered Sand Jambiya rules with explicitly selected native weapon and environment mappings.</summary>
public static class ArmageddonSandKnifeStock
{
	public const string Key = "arm.spell.sand_knife";
	public const string Name = "Sand Knife";
	// Source (grade+1)*3000/2 heap units, at the shared event loop's nominal 0.75 seconds/unit.
	public const string LifetimeSeconds = "(grade+1)*1500*0.75";
	// Sand Knife is outside the approved Sorcerer tree. This is authored native balance, not C metadata (0).
	public const double MinimumEnergy = 5;
	internal static string EligibilitySource(long sandstormTag, long sandstormWeather) =>
		"if (lowercase(@caster.location.terrain.name) == \"inside\" or lowercase(@caster.location.terrain.name) == \"city\")\nreturn false\nend if\n" +
		"if (lowercase(@caster.location.terrain.name) == \"desert\" or lowercase(@caster.location.terrain.name) == \"earth plane\" or " +
		"lowercase(@caster.location.terrain.name) == \"silt\" or lowercase(@caster.location.terrain.name) == \"shallows\" or lowercase(@caster.location.terrain.name) == \"salt flats\")\nreturn true\nend if\n" +
		$"if (isnull(tag({sandstormTag})))\nreturn false\nend if\n" +
		$"if (istagged(@caster.location, tag({sandstormTag})))\nreturn true\nend if\n" +
		$"return not(isnull(@caster.location.weather)) and @caster.location.weather.id == {sandstormWeather}";

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, IReadOnlyList<IGameItemProto> knives, IReadOnlyList<IGameItemProto> staffs,
		ITag monComponent, ITag sandstormTag, IWeatherEvent sandstormWeather)
	{
		if (world.SpellOwnedItems is null || !ReferenceEquals(school.Gameworld, world) || world.Traits.Get(trait.Id) != trait ||
			world.MagicResources.Get(resource.Id) != resource || world.Tags.Get(monComponent.Id) != monComponent ||
			world.Tags.Get(sandstormTag.Id) != sandstormTag || world.WeatherEvents.Get(sandstormWeather.Id) != sandstormWeather || monComponent == sandstormTag)
			throw new InvalidOperationException("Select existing world definitions, distinct Creation rank-six and sandstorm room tags, and a sandstorm weather event.");
		if (knives.Count != 6 || staffs.Count != 8 || knives.Concat(staffs).Select(x => x.Id).Distinct().Count() != 14)
			throw new InvalidOperationException("Select six distinct temporary grade prototypes and eight distinct permanent staff prototypes.");
		foreach (var prototype in knives.Concat(staffs))
		{
			if (NativeItemCreationEligibility.Error(prototype, world) is { } error) throw new InvalidOperationException(error);
			var weapon = prototype.GetItemType<MeleeWeaponGameItemComponentProto>()?.WeaponType;
			if (weapon is null || !weapon.Attacks.Any(x => x.MoveType == BuiltInCombatMoveType.UseWeaponAttack &&
				x.HandednessOptions is AttackHandednessOptions.Any or AttackHandednessOptions.OneHandedOnly) ||
				weapon.Attacks.Any(x => x.Profile.DamageType is not (DamageType.Slashing or DamageType.Chopping or DamageType.Crushing or DamageType.Piercing)))
				throw new InvalidOperationException("Select usable one-hand native physical melee profiles. Historical objects 456-461 and 1378-1385 are unavailable; profiles are authored adaptations.");
		}
		if (world.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(Name)))
			throw new InvalidOperationException("Sand Knife already exists in that school; edit or clone it instead.");
		var no = world.AlwaysFalseProg ?? throw new InvalidOperationException("The always-false support prog is unavailable.");
		using var scope = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB(); using var transaction = FMDB.Context.Database.BeginTransaction();
		if (FMDB.Context.MagicSpells.Any(x => x.MagicSchoolId == school.Id && x.Name == Name))
			throw new InvalidOperationException("A persisted Sand Knife already exists in that school.");
		var row = new Db.MagicSpell
		{
			Name = Name, MagicSchoolId = school.Id, CastingTraitDefinitionId = trait.Id, SpellKnownProgId = no.Id,
			Blurb = "Shape a temporary sand weapon, or a permanent staff at mon.",
			Description = "Grades one through six create their selected temporary sand weapon without a component. " +
				"Grade seven consumes one directly carried rank-six-or-higher Creation component and chooses one of eight permanent staffs. " +
				"Desert, Earth Plane, Silt, Shallows and Salt Flats terrain supply sand; a selected room tag or weather event supplies storm sand elsewhere. Inside and City terrain always refuse. Edit the support prog to map your world. " +
				"This bounded stock targets the caster and places normally in inventory or at their location; wield using native equipment commands. No shadow lifetime reduction applies. " +
				"Profiles and the five-energy lower bound are authored native policy; source C minimum zero and base-power fifteen are separate metadata. Sand Knife is outside the approved Sorcerer source tree. Add acquisition separately.",
			CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			CastingEmote = "$0 shape|shapes sand into a weapon.", FailCastingEmote = "$0 fail|fails to bind the sand.",
			TargetEmote = "", TargetNullEmote = "There is not enough sand to form a weapon.", TargetResistedEmote = "",
			AppliedEffectsAreExclusive = true, ScrollInscriptionAllowed = false, Definition = "<Definition />"
		};
		FMDB.Context.MagicSpells.Add(row); FMDB.Context.SaveChanges();
		var environment = new Db.FutureProg { FunctionName = $"armsand_{row.Id}_environment", FunctionText = EligibilitySource(sandstormTag.Id, sandstormWeather.Id),
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionComment = "Editable sufficient-sand terrain, room-tag and selected-weather mapping; source numeric condition has no native equivalent.", Category = "Magic", Subcategory = Name, Public = false };
		environment.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		var check = new FutureProg.FutureProg(environment, world);
		if (!check.Compile()) throw new InvalidOperationException("Stock environment prog failed: " + check.CompileError);
		var duration = new Db.TraitExpression { Name = $"Sand Knife #{row.Id} duration", Expression = LifetimeSeconds };
		var cost = new Db.TraitExpression { Name = $"Sand Knife #{row.Id} energy", Expression = "5*grade" };
		FMDB.Context.FutureProgs.Add(environment); FMDB.Context.TraitExpressions.AddRange(duration, cost); FMDB.Context.SaveChanges();
		row.EffectDurationExpressionId = duration.Id;
		row.Definition = Definition(resource.Id, cost.Id, knives.Select(x => x.Id).ToArray(), staffs.Select(x => x.Id).ToArray(), monComponent.Id, environment.Id).ToString();
		FMDB.Context.SaveChanges(); transaction.Commit();
		var prog = new FutureProg.FutureProg(environment, world); prog.Compile(); world.Add(prog);
		world.Add(new TraitExpression(duration, world)); world.Add(new TraitExpression(cost, world));
		var result = new MagicSpell(row, world); world.Add(result); return result;
	}

	internal static XElement Definition(long resource, long costExpression, IReadOnlyList<long> knives, IReadOnlyList<long> staffs, long monComponent, long eligibility) =>
		new("Definition", new XElement("StockIdentity", Key),
			new XElement("Trigger", new XAttribute("type", "self"), new XElement("MinimumPower", (int)SpellPower.ExtremelyWeak), new XElement("MaximumPower", (int)SpellPower.ExtremelyStrong)),
			new XElement("Costs", new XElement("Cost", new XAttribute("resource", resource), new XAttribute("expression", costExpression))),
			new XElement("Effects", new XElement("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", "base"), new XElement("ItemPrototypeId", knives[0]), new XElement("ItemSkinId", 0), new XElement("Quantity", 1), new XElement("LoadString", ""),
				new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"), new XElement("Family", "sand-knife"), new XElement("Seconds", LifetimeSeconds), new XElement("Placement", "standard"),
					new XElement("PermanentOutput", new XAttribute("grade", 7), staffs[0]), new XElement("EligibilityProg", eligibility),
					new XElement("GradeOutputs", knives.Select((id, index) => new XElement("Output", new XAttribute("grade", index + 1), new XElement("Prototype", id))),
						new XElement("Output", new XAttribute("grade", 7), staffs.Select(id => new XElement("Prototype", id))))))),
			new XElement("CasterEffects"), new XElement("Plan", new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"), new XAttribute("tag", monComponent), new XAttribute("secondtag", 0), new XAttribute("quantity", 1), new XAttribute("carriedonly", true), new XAttribute("grade", 7)))),
			new XElement("ControlledPower", new XAttribute("schema", 1), new XAttribute("version", 1), new XAttribute("overreachCost", 1.5), new XAttribute("overreachDifficulty", 1), new XAttribute("masteryChance", 0.25), new XAttribute("masterySeconds", 600), new XAttribute("skillSeconds", 60), new XAttribute("openingSkill", 30),
				new XElement("Efficiency", new XAttribute("type", "source"), new XAttribute("minimum", MinimumEnergy), new XAttribute("scale", 1)),
				new[] { 0, 20, 40, 55, 70, 85, 95 }.Select((skill, index) => new XElement("Grade", new XAttribute("number", index + 1), new XAttribute("power", (int)SpellPower.ExtremelyWeak + index), new XAttribute("skill", skill), new XAttribute("difficulty", index / 2))), new XElement("ScalarBindings")));
}

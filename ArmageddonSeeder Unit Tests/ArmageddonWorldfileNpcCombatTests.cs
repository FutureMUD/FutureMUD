#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using NativeExpression = MudSharp.Body.Traits.TraitExpression;
using TraitDefinition = MudSharp.Models.TraitDefinition;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public partial class ArmageddonWorldfileNpcCombatTests
{
	private int _oldBonusScale;
	[TestInitialize]
	public void Initialise()
	{
		var field = typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!;
		_oldBonusScale = (int)field.GetValue(null)!; field.SetValue(null, 10);
	}
	[TestCleanup]
	public void Cleanup() => typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, _oldBonusScale);

	[TestMethod]
	public void SourceMeans_ConvolvesBeforeAgingClampingAndNonlinearReaction()
	{
		var reaction = new int[101]; reaction[2] = 100; reaction[3] = 10;
		var strengthAge = Enumerable.Repeat(1000, 21).ToArray(); strengthAge[10] = 500;
		var agilityAge = Enumerable.Repeat(1000, 21).ToArray(); agilityAge[10] = 1500;
		var race = new ArmWorldRace(new("fixture", 1), 1, "fixture", "f", ["1d2", "1d2"], [2, 3], "", [0, 20], []);
		var guild = new ArmWorldGuild(new("fixture", 1), 1, "fixture", "f", [0, 0], [100, 200, 4, 5], 0, []);
		var means = ArmageddonWorldfileWriter.SourceNpcCombatMeans(-1, race, guild, new(reaction, strengthAge, agilityAge));
		Assert.AreEqual(1d, means.Strength);
		Assert.AreEqual(2d, means.Agility);
		Assert.AreEqual(5d, means.Reaction, "Average reactions over aged rolls, not reaction of the mean agility.");
		var npc = Npc(-8, -9);
		var rating = ArmageddonWorldfileWriter.SourceNpcCombatRating(npc, race, guild, means);
		Assert.AreEqual(6d, rating.Offence); Assert.AreEqual(8d, rating.Defence);
		var signed = ArmageddonWorldfileWriter.SourceNpcCombatRating(Npc(-200, -300), null, null, (10, 10, 0));
		Assert.AreEqual(-180d, signed.Offence); Assert.AreEqual(-280d, signed.Defence);
	}

	[TestMethod]
	public void Install_NativeChecksUseLiveSkillsOnceAndPreserveUnprofiledFallback()
	{
		using var fixture = new Fixture();
		var native = fixture.NativeRuntime();
		var actor = native.Actor;
		var target = native.Target;
		actor.SetProfile(106.8, 100.8); target.SetProfile(63.8, 77.8);
		var melee = native.Check(CheckType.MeleeWeaponCheck);
		var untrained = melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object);
		actor.Values[native.Weapon.Id] = 10;
		var piercing = melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object);
		actor.Values[native.Weapon.Id] = 50;
		var slashing = melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object);
		Assert.AreEqual(64.95, untrained, 0.1); Assert.AreEqual(73.63, piercing, 0.1); Assert.AreEqual(91.01, slashing, 0.1);
		var dodge = native.Check(CheckType.DodgeCheck);
		var baseDefence = dodge.TargetNumber(target.Mock.Object, Difficulty.Normal, null!, actor.Mock.Object);
		Assert.AreEqual(35.05, baseDefence, 0.1);
		foreach (var type in new[] { CheckType.DodgeCheck, CheckType.ParryCheck, CheckType.BlockCheck })
		{
			var skill = native.ActiveSkills[type];
			var check = native.Check(type);
			target.Values[skill] = 50;
			Assert.AreEqual(baseDefence + (100 - baseDefence) * 0.5, check.TargetNumber(target.Mock.Object, Difficulty.Normal, null!, actor.Mock.Object), 0.000001);
			var expression = check.TargetNumberExpression;
			Assert.AreEqual(1, expression.Parameters.Values.Count(x => !x.IsTarget && x.Trait.Id == skill), "No duplicate learning bindings from the fallback branch.");
			target.Values[skill] = 0;
		}
		target.Values[fixture.ProfileId] = 0;
		Assert.AreEqual(93.7, melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object), 0.1);
		actor.Values[fixture.ProfileId] = 0;
		Assert.AreEqual(50d, melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object));
		target.SetProfile(100, 100);
		Assert.AreEqual(50d, melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object), "Native actor attacking an imported target retains native check rules.");
		actor.SetProfile(0, 0); target.SetProfile(0, 0); actor.Values[native.Weapon.Id] = 0;
		Assert.AreEqual(50d, melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object), "Present zero-valued profiles must not fall back to zero native skill.");
		actor.SetProfile(166.5, -23.5); target.SetProfile(341.9, 304.8);
		Assert.IsTrue(melee.TargetNumber(actor.Mock.Object, Difficulty.Normal, native.Weapon, target.Mock.Object) < 10);
		Assert.IsTrue(dodge.TargetNumber(target.Mock.Object, Difficulty.Normal, null!, actor.Mock.Object) > 90);
	}

	[TestMethod]
	public void Install_RatingsAreFixedSignedNativeTraitsWithHeadroomAndNonlearningReferences()
	{
		using var fixture = new Fixture();
		var native = fixture.NativeRuntime();
		foreach (var model in fixture.Db.TraitDefinitions.Where(x => x.TraitGroup == "Worldfile Combat"))
		{
			var definition = (SkillDefinition)native.Traits.Get(model.Id);
			var value = model.Alias == "profile" ? 1d : model.Alias == "offence" ? 400d : -80d;
			var trait = new Skill(definition, value, native.Actor.Mock.Object);
			Assert.AreEqual(value, trait.Value, "Do not clamp source scores to 100 or discard their sign.");
			Assert.IsTrue(model.Hidden); Assert.AreEqual(0d, model.BranchMultiplier);
			Assert.AreEqual("non-improving", fixture.Db.Improvers.Single(x => x.Id == model.ImproverId).Type);
		}
		foreach (var check in fixture.Db.Checks.Where(x => x.TraitExpression.Name.StartsWith("Worldfile ")))
		{
			var expression = native.Expressions.Get(check.TraitExpressionId);
			Assert.IsFalse(expression.HasErrors(), expression.Error);
			Assert.IsTrue(expression.Parameters.Where(x => x.Key.StartsWith("armcombat_")).All(x => !x.Value.CanImprove && !x.Value.CanBranch));
		}
		Assert.AreEqual(5, fixture.Db.SeederManagedRecords.Count(x => x.StableKey.StartsWith("combat.check.")));
	}

	[TestMethod]
	public void RealCharacter_SkillMeritsAndUnrelatedEffectsCannotInventOrDisableACombatProfile()
	{
		using var fixture = new Fixture();
		var runtime = fixture.NativeRuntime();
		var character = (MudSharp.Character.Character)RuntimeHelpers.GetUninitializedObject(typeof(MudSharp.Character.Character));
		var body = new Mock<IBody>(); body.SetupGet(x => x.Merits).Returns([]); body.SetupGet(x => x.ExternalItems).Returns([]);
		typeof(MudSharp.Character.Character).GetProperty("Gameworld")!.SetValue(character, runtime.World.Object);
		typeof(MudSharp.Character.Character).GetProperty("Body")!.SetValue(character, body.Object);
		var heldWeaponSkill = new Mock<ITrait>(); heldWeaponSkill.SetupGet(x => x.Definition).Returns(runtime.Weapon); heldWeaponSkill.SetupGet(x => x.Value).Returns(40);
		var traits = new List<ITrait> { heldWeaponSkill.Object };
		typeof(MudSharp.Character.Character).GetField("_characterTraits", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(character, traits);
		var bonus = (AllSkillBonusMerit)RuntimeHelpers.GetUninitializedObject(typeof(AllSkillBonusMerit)); bonus.SpecificBonus = 5;
		var merit = new Mock<ITraitBonusMerit>(); merit.Setup(x => x.Applies(character)).Returns(true);
		merit.Setup(x => x.BonusForTrait(It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>())).Returns((ITraitDefinition trait, TraitBonusContext context) => bonus.BonusForTrait(trait, context));
		typeof(MudSharp.Character.Character).GetField("_merits", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(character, new List<IMerit> { merit.Object });
		var unrelated = new Mock<ITraitDefinition>(); unrelated.SetupGet(x => x.Id).Returns(99);
		var effect = new SingleTraitBonusEffect(character, unrelated.Object, 5, null!);
		var effects = new Mock<IEffectHandler>();
		effects.Setup(x => x.EffectsOfType<ITraitBonusEffect>(It.IsAny<Predicate<ITraitBonusEffect>>())).Returns([effect]);
		typeof(PerceivedItem).GetProperty("EffectHandler")!.SetValue(character, effects.Object);
		runtime.Target.SetProfile(100, 100);
		var expression = runtime.Check(CheckType.MeleeWeaponCheck).TargetNumberExpression;
		Assert.AreEqual(45d, expression.EvaluateWithTarget(character, runtime.Target.Mock.Object, runtime.Weapon), "A real unprofiled character must retain its native effective weapon skill, without querying effects for absent rating skills.");
		foreach (var model in fixture.Db.TraitDefinitions.Where(x => x.TraitGroup == "Worldfile Combat"))
			traits.Add(new Skill((SkillDefinition)runtime.Traits.Get(model.Id), model.Alias == "profile" ? 1 : 100, character));
		bonus.SpecificBonus = -5;
		var expected = 5 + 90 / (1 + Math.Exp(-35d / 21));
		Assert.AreEqual(expected, expression.EvaluateWithTarget(character, runtime.Target.Mock.Object, runtime.Weapon), 0.000001, "Only the active weapon skill receives the penalty; raw marker and O/D stay fixed.");
	}

	private static ArmWorldNpc Npc(int offence, int defence) => new(new("fixture", 1), 1, 1, "fixture", "fixture", "", "", "",
		new int[4], [0, offence, defence, 0, 100, 0], new int[2], new int[7], -1, [], [], [], []);

	internal sealed class Fixture : IDisposable
	{
		internal readonly MudSharp.Database.FuturemudDatabaseContext Db;
		internal readonly long ProfileId;
		internal readonly Dictionary<CheckType, long> ActiveSkills = new();
		internal readonly Dictionary<CheckType, long> OriginalExpressions = new();
		internal readonly ArmageddonWorldfileWriter Writer;
		internal Fixture(ArmageddonWorldfilePackage? source = null)
		{
			Db = ArmageddonInstallationSettingsTests.Fixture();
			ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(Db));
			var template = new CheckTemplate { Id = 88000, Name = "Combat fixture", CheckMethod = "Standard", FailIfTraitMissingMode = 0 };
			foreach (var difficulty in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty { Difficulty = (int)difficulty, Modifier = 0 });
			Db.CheckTemplates.Add(template);
			var next = 88000L;
			foreach (var type in new[] { CheckType.MeleeWeaponCheck, CheckType.NaturalWeaponAttack, CheckType.DodgeCheck, CheckType.ParryCheck, CheckType.BlockCheck })
			{
				var skill = new TraitDefinition { Id = ++next, Name = type.ToString(), Type = 0, OwnerScope = 1 };
				Db.TraitDefinitions.Add(skill); ActiveSkills[type] = skill.Id;
				var expression = new MudSharp.Models.TraitExpression { Id = ++next, Name = type.ToString(), Expression = type == CheckType.MeleeWeaponCheck ? "variable" : "native:" + skill.Id };
				Db.TraitExpressions.Add(expression); OriginalExpressions[type] = expression.Id;
				var check = Db.Checks.SingleOrDefault(x => x.Type == (int)type);
				if (check is null) { check = new Check { Type = (int)type }; Db.Checks.Add(check); }
				check.TraitExpressionId = expression.Id; check.TraitExpression = expression; check.CheckTemplate = template;
				check.CheckTemplateId = template.Id; check.MaximumDifficultyForImprovement = (int)Difficulty.Impossible;
			}
			Db.SaveChanges();
			Db.RangedWeaponTypes.Add(new MudSharp.Models.RangedWeaponTypes { Id = 88040, Name = "Throwing Knife", RangedWeaponType = (int)MudSharp.Combat.RangedWeaponType.Thrown,
				FireTraitId = 17, OperateTraitId = 17, AccuracyBonusExpression = "0", DamageBonusExpression = "0", DefaultRangeInRooms = 2 });
			Db.SaveChanges();
			var tables = new ArmWorldCombatCalibration(new int[101], Enumerable.Repeat(1000, 21).ToArray(), Enumerable.Repeat(1000, 21).ToArray());
			var writer = new ArmageddonWorldfileWriter(Db, source ?? new ArmageddonWorldfilePackage { CombatCalibration = tables });
			Writer = writer;
			typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcCombatRatings", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
			ProfileId = Db.TraitDefinitions.Single(x => x.Alias == "profile").Id;
		}
		public void Dispose() => Db.Dispose();
		internal Runtime NativeRuntime() => new(this);
	}

	internal sealed class Person(long profile, long offence, long defence)
	{
		internal readonly Mock<ICharacter> Mock = new();
		internal readonly Dictionary<long, double> Values = new();
		internal void SetProfile(double o, double d) { Values[profile] = 1; Values[offence] = o; Values[defence] = d; }
	}

	internal sealed class Runtime
	{
		internal readonly Mock<IFuturemud> World = new();
		internal readonly All<ITraitDefinition> Traits = new();
		internal readonly All<ITraitExpression> Expressions = new();
		internal readonly Person Actor;
		internal readonly Person Target;
		internal readonly ITraitDefinition Weapon;
		internal readonly Dictionary<CheckType, long> ActiveSkills;
		private readonly Fixture _fixture;
		internal Runtime(Fixture fixture)
		{
			_fixture = fixture; ActiveSkills = fixture.ActiveSkills;
			World.SetupGet(x => x.Traits).Returns(Traits); World.SetupGet(x => x.TraitExpressions).Returns(Expressions);
			World.SetupGet(x => x.TraitDecorators).Returns(new All<ITraitValueDecorator>());
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			World.SetupGet(x => x.ImprovementModels).Returns(new All<IImprovementModel>());
			foreach (var row in fixture.Db.TraitExpressions) Expressions.Add(new NativeExpression(row, World.Object));
			foreach (var row in fixture.Db.TraitDefinitions)
			{
				if (row.TraitGroup == "Worldfile Combat")
				{
					var definition = new SkillDefinition(row, World.Object); definition.Initialise(row); Traits.Add(definition);
				}
				else
				{
					var definition = new Mock<ITraitDefinition>(); definition.SetupGet(x => x.Id).Returns(row.Id);
					definition.SetupGet(x => x.Name).Returns(row.Name); definition.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character); Traits.Add(definition.Object);
				}
			}
			var offence = fixture.Db.TraitDefinitions.Single(x => x.Alias == "offence").Id;
			var defence = fixture.Db.TraitDefinitions.Single(x => x.Alias == "defence").Id;
			Actor = new(fixture.ProfileId, offence, defence); Target = new(fixture.ProfileId, offence, defence);
			foreach (var person in new[] { Actor, Target })
			{
				person.Mock.SetupGet(x => x.Gameworld).Returns(World.Object); person.Mock.SetupGet(x => x.VisionPercentage).Returns(1);
				person.Mock.SetupGet(x => x.Merits).Returns([]);
				person.Mock.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>())).Returns((ITraitDefinition trait, TraitBonusContext _) => person.Values.GetValueOrDefault(trait.Id));
				person.Mock.Setup(x => x.TraitRawValue(It.IsAny<ITraitDefinition>())).Returns((ITraitDefinition trait) => person.Values.GetValueOrDefault(trait.Id));
			}
			Weapon = Traits.Get(fixture.ActiveSkills[CheckType.MeleeWeaponCheck]);
		}
		internal ICheck Check(CheckType type) => new StandardCheck(_fixture.Db.Checks.Single(x => x.Type == (int)type), World.Object);
	}
}

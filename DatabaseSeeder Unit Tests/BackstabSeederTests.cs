#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using WeaponAttack = MudSharp.Models.WeaponAttack;
using WeaponType = MudSharp.Models.WeaponType;
using ArmourType = MudSharp.Models.ArmourType;
using ShieldType = MudSharp.Models.ShieldType;
using GameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BackstabSeederTests
{
	[TestMethod]
	[DataRow("soi", false)]
	[DataRow("soi", true)]
	[DataRow("broad", false)]
	[DataRow("broad", true)]
	[DataRow("weapons", false)]
	[DataRow("weapons", true)]
	public void CombatInstall_AllSkillModes_BlackjackSharesClubBindingsAndRerunsSafely(string skillMode, bool separateParry)
	{
		using var context = Context();
		context.Add(new Account { Id = 1, Name = "Builder" });
		context.Add(new GameItemComponentProto { Id = 1, Name = "Holdable", Type = "Holdable", Definition = "<Definition />" });
		context.AddRange(new TraitDefinition { Id = 1, Name = "Strength", Alias = "str", Type = 1 },
			new TraitDefinition { Id = 2, Name = "Agility", Alias = "agi", Type = 1 },
			new TraitDefinition { Id = 3, Name = "Blocking" });
		var parry = new TraitDefinition { Id = 4, Name = "Parrying" };
		context.Add(parry);
		context.AddRange(new FutureProg { FunctionName = "AlwaysTrue" }, new FutureProg { FunctionName = "AlwaysFalse" },
			new Improver { Name = "Skill Improver" }, new TraitDecorator { Name = "General Skill" },
			new ArmourType { Name = "Shield Armour" }, new ShieldType { Name = "Existing Shield" }, new BodypartShape { Name = "Head Back" });
		foreach (var name in new[] { "throat", "scalp", "bneck", "rhand", "lhand", "rshin", "lshin", "rbreast", "belly" })
			context.Add(new BodypartProto { Name = name });
		foreach (var name in new[] { "DefaultBowMeleeWeaponType", "DefaultCrossbowMeleeWeaponType", "DefaultGunMeleeWeaponType", "DefaultMusketMeleeWeaponType" })
			context.Add(new StaticConfiguration { SettingName = name, Definition = "0" });
		context.SaveChanges();
		var answers = new Dictionary<string, string>
		{
			["skilloption"] = skillMode, ["parryoption"] = separateParry ? "yes" : "no",
			["random"] = "static", ["messagestyle"] = "compact"
		};
		var skills = new Dictionary<string, TraitDefinition> { ["Block"] = context.TraitDefinitions.Single(x => x.Name == "Blocking") };
		if (separateParry) skills.Add("Parrying", parry);
		var seeder = new CombatSeeder();
		typeof(CombatSeeder).GetMethod("SeedDataWeapons", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(seeder, [context, answers, skills]);
		var reconcile = typeof(CombatSeeder).GetMethod("EnsureExpandedStockCombatContent", BindingFlags.NonPublic | BindingFlags.Instance)!;
		reconcile.Invoke(seeder, [context, answers, null]);
		var club = context.WeaponTypes.Single(x => x.Name == "Club");
		var blackjack = context.WeaponTypes.Single(x => x.Name == "Blackjack");
		Assert.AreEqual(club.AttackTraitId, blackjack.AttackTraitId);
		Assert.AreEqual(club.ParryTraitId, blackjack.ParryTraitId);
		Assert.AreEqual((int)WeaponClassification.NonLethal, blackjack.Classification);
		Assert.AreEqual(1, blackjack.Reach); Assert.AreEqual(-1.0, blackjack.ParryBonus); Assert.AreEqual(2.0, blackjack.StaminaPerParry);
		var component = context.GameItemComponentProtos.Single(x => x.Name == "Melee_Blackjack");
		Assert.AreEqual(blackjack.Id.ToString(), XElement.Parse(component.Definition).Element("WeaponType")!.Value);
		var attack = context.WeaponAttacks.Single(x => x.Name == "Blackjack Sapping Blow");
		attack.Weighting = 71;
		context.SaveChanges();
		reconcile.Invoke(seeder, [context, answers, null]);
		Assert.AreEqual(71.0, attack.Weighting);
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "Melee_Blackjack"));
		Assert.AreEqual(3, context.WeaponAttacks.Count(x => x.WeaponTypeId == blackjack.Id));
		var customPain = context.TraitExpressions.Single(x => x.Name == "Blackjack Pain");
		customPain.Expression = "777";
		context.SaveChanges();
		answers["balance"] = "combat-rebalance";
		reconcile.Invoke(seeder, [context, answers, null]);
		var currentNormal = context.TraitExpressions.Single(x => x.Name == "Weapon Damage - Normal").Expression;
		Assert.AreEqual($"0.25 * ({currentNormal})", attack.DamageExpression.Expression);
		Assert.AreEqual($"2 * ({currentNormal})", attack.StunExpression.Expression);
		Assert.AreEqual("777", customPain.Expression);
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);

	private static (TraitExpression Normal, TraitExpression Good, WeaponType Blackjack) Weapons(FuturemudDatabaseContext context)
	{
		var normal = new TraitExpression { Name = "Normal", Expression = "20" };
		var good = new TraitExpression { Name = "Good", Expression = "30" };
		var blackjack = new WeaponType { Name = "Blackjack" };
		context.AddRange(normal, good, blackjack, new WeaponType { Name = "Knife" }, new WeaponType { Name = "Dagger" },
			new WeaponType { Name = "Club" }, new WeaponType { Name = "Training Dagger" }, new BodypartShape { Name = "Head Back" });
		context.SaveChanges();
		return (normal, good, blackjack);
	}

	[TestMethod]
	public void WeaponBackstabs_FreshAndRerun_UseExactProfilesAndRecipients()
	{
		using var context = Context();
		var f = Weapons(context);
		Assert.AreEqual(5, CombatSeeder.EnsureBackstabAttacks(context, f.Blackjack, f.Normal, f.Good, SeedCombatMessageStyle.Compact));
		var ids = context.WeaponAttacks.OrderBy(x => x.Id).Select(x => x.Id).ToArray();
		Assert.AreEqual(0, CombatSeeder.EnsureBackstabAttacks(context, f.Blackjack, f.Normal, f.Good, SeedCombatMessageStyle.Compact));
		CollectionAssert.AreEqual(ids, context.WeaponAttacks.OrderBy(x => x.Id).Select(x => x.Id).ToArray());
		CollectionAssert.AreEquivalent(new[] { "Knife", "Dagger", "Blackjack" }, context.WeaponAttacks
			.Where(x => x.MoveType == (int)BuiltInCombatMoveType.BackstabAttack).Select(x => x.WeaponType.Name).ToArray());
		Assert.AreEqual(5, context.CombatMessages.Count());
		foreach (var attack in context.WeaponAttacks)
		{
			Assert.AreEqual(1, attack.MaximumTargets);
			Assert.AreEqual((int)Difficulty.Normal, attack.BaseAttackerDifficulty);
			Assert.AreEqual((int)Difficulty.Easy, attack.BaseDodgeDifficulty);
			Assert.AreEqual((int)Difficulty.Easy, attack.BaseParryDifficulty);
			Assert.AreEqual((int)Difficulty.VeryEasy, attack.BaseBlockDifficulty);
		}
		var knife = context.WeaponAttacks.Single(x => x.Name == "Knife Backstab");
		Assert.AreEqual(f.Normal.Id, knife.DamageExpressionId);
		AssertProfile(knife, 5, 1.1, 200, 0.25, 5, 0, 0);
		var dagger = context.WeaponAttacks.Single(x => x.Name == "Dagger Backstab");
		Assert.AreEqual(f.Good.Id, dagger.DamageExpressionId);
		AssertProfile(dagger, 4, 1, 200, 0, 0, 10, 0.1);
		var sap = context.WeaponAttacks.Single(x => x.Name == "Blackjack Sapping Blow");
		AssertProfile(sap, 4, 1.1, 300, 0.75, 5, 0, 0);
		Assert.AreEqual("Head Back", context.BodypartShapes.Find(sap.BodypartShapeId)!.Name);
		Assert.AreEqual("0.25 * (20)", sap.DamageExpression.Expression);
		Assert.AreEqual("0.5 * (20)", sap.PainExpression.Expression);
		Assert.AreEqual("2 * (20)", sap.StunExpression.Expression);
		foreach (var attack in context.WeaponAttacks.Where(x => x.WeaponTypeId == f.Blackjack.Id))
		{
			Assert.AreEqual((int)AttackHandednessOptions.OneHandedOnly, attack.HandednessOptions);
			Assert.AreEqual((int)DamageType.Crushing, attack.DamageType);
			Assert.AreEqual(0L, attack.Intentions & (long)CombatMoveIntentions.Kill);
			if (attack != sap)
			{
				Assert.IsNull(attack.AdditionalInfo);
				Assert.AreEqual(f.Normal.Id, attack.StunExpressionId);
				Assert.AreEqual(3.0, attack.StaminaCost);
			}
		}
	}

	[TestMethod]
	public void WeaponBackstabs_MissingNativeHeadBackRefusesBeforeCreatingContent()
	{
		using var context = Context();
		var f = Weapons(context);
		context.BodypartShapes.Remove(context.BodypartShapes.Single());
		context.BodypartShapes.Add(new BodypartShape { Name = "Head" }); // A synthetic generic head is not native Head Back.
		context.SaveChanges();
		var expressions = context.TraitExpressions.Count();
		var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
			CombatSeeder.EnsureBackstabAttacks(context, f.Blackjack, f.Normal, f.Good, SeedCombatMessageStyle.Compact));
		StringAssert.Contains(error.Message, "Head Back");
		Assert.AreEqual(expressions, context.TraitExpressions.Count());
		Assert.AreEqual(0, context.WeaponAttacks.Count());
		Assert.AreEqual(0, context.CombatMessages.Count());
	}

	[TestMethod]
	public void WeaponBackstabs_PartialInstall_RepairsMissingMessageAndPreservesCustomFields()
	{
		using var context = Context();
		var f = Weapons(context);
		CombatSeeder.EnsureBackstabAttacks(context, f.Blackjack, f.Normal, f.Good, SeedCombatMessageStyle.Compact);
		var attack = context.WeaponAttacks.Single(x => x.Name == "Dagger Backstab");
		attack.Weighting = 17;
		attack.AdditionalInfo = "<Backstab><Penetration>3</Penetration></Backstab>";
		var message = context.CombatMessages.Single(x => x.CombatMessagesWeaponAttacks.Any(y => y.WeaponAttackId == attack.Id));
		context.RemoveRange(message.CombatMessagesWeaponAttacks);
		context.Remove(message);
		context.SaveChanges();
		CombatSeeder.EnsureBackstabAttacks(context, f.Blackjack, f.Normal, f.Good, SeedCombatMessageStyle.Compact);
		Assert.AreEqual(17.0, attack.Weighting);
		Assert.AreEqual("3", XElement.Parse(attack.AdditionalInfo).Element("Penetration")!.Value);
		Assert.AreEqual(5, context.CombatMessages.Count());
	}

	[TestMethod]
	public void AnimalBackstabs_OnlyLeopardAndJaguar_InheritAnatomyQualityAndDonorChecks()
	{
		using var context = Context();
		var expression = new TraitExpression { Name = "Animal Claw Damage", Expression = "40" };
		context.Add(expression); context.SaveChanges();
		var donor = new WeaponAttack
		{
			Name = "Claw Swipe", MoveType = (int)BuiltInCombatMoveType.NaturalWeaponAttack,
			DamageExpression = expression, PainExpression = expression, StunExpression = expression,
			DamageType = (int)DamageType.Claw, BaseAttackerDifficulty = (int)Difficulty.Normal,
			RequiredPositionStateIds = "1", BaseDodgeDifficulty = (int)Difficulty.Easy
		};
		context.Add(donor);
		foreach (var name in new[] { "Leopard", "Jaguar", "Tiger", "Cat" })
		{
			var race = new Race { Name = name };
			foreach (var alias in new[] { "rfpaw", "lfpaw" })
				context.Add(new RacesWeaponAttacks { Race = race, WeaponAttack = donor,
					Bodypart = new BodypartProto { Name = alias }, Quality = (int)ItemQuality.Good });
		}
		context.SaveChanges();
		BackstabSeederHelper.EnsureAnimalBackstabs(context);
		BackstabSeederHelper.EnsureAnimalBackstabs(context);
		var attack = context.WeaponAttacks.Single(x => x.Name == BackstabSeederHelper.PredatoryBackRake);
		Assert.AreEqual("1.5 * (40)", attack.DamageExpression.Expression);
		Assert.AreEqual(donor.BaseDodgeDifficulty, attack.BaseDodgeDifficulty);
		Assert.AreEqual(donor.RequiredPositionStateIds, attack.RequiredPositionStateIds);
		var links = context.RacesWeaponAttacks.Where(x => x.WeaponAttackId == attack.Id).ToList();
		Assert.AreEqual(4, links.Count);
		CollectionAssert.AreEquivalent(new[] { "Leopard", "Jaguar" }, links.Select(x => x.Race.Name).Distinct().ToArray());
		Assert.IsTrue(links.All(x => x.Quality == (int)ItemQuality.Good));
		var xml = XElement.Parse(attack.AdditionalInfo);
		Assert.AreEqual("5", xml.Element("AttackerOffence")!.Value);
		Assert.AreEqual("5", xml.Element("TargetDefence")!.Value);
	}

	[TestMethod]
	public void MythicalAndSupernaturalBackstabs_OnlySelectedTemplates_DeclareExactConfiguration()
	{
		CollectionAssert.AreEqual(new[] { "Manticore" }, MythicalAnimalSeeder.TemplatesForTesting.Values
			.Where(x => x.Attacks.Any(a => a.AttackName == BackstabSeederHelper.ManticoreBackstab)).Select(x => x.Name).ToArray());
		CollectionAssert.AreEqual(new[] { "Werewolf Hybrid" }, SupernaturalSeeder.TemplatesForTesting.Values
			.Where(x => x.Attacks.Any(a => a.AttackName == BackstabSeederHelper.WerewolfBackstab)).Select(x => x.Name).ToArray());
		var definition = SupernaturalSeeder.SupernaturalAttackDefinitionsForTesting[BackstabSeederHelper.WerewolfBackstab];
		Assert.AreEqual(BuiltInCombatMoveType.BackstabAttackUnarmed, definition.MoveTypeOverride);
		Assert.AreEqual(1.25, definition.Backstab!.Multiplier);
		Assert.AreEqual(6.0, definition.Backstab.Stamina);
		Assert.AreEqual(1.3, definition.Backstab.Delay);
		Assert.AreEqual(150.0, definition.Backstab.Weighting);
		Assert.AreEqual(5.0, definition.Backstab.Configuration.TargetOffence);
		Assert.AreEqual(5.0, definition.Backstab.Configuration.TargetDefence);
		Assert.AreEqual(0.0, definition.Backstab.Configuration.ArmourBypass);
		Assert.AreEqual(0, MythicalAnimalSeeder.ValidateTemplateCatalogForTesting().Count);
		Assert.AreEqual(0, SupernaturalSeeder.ValidateTemplateCatalogForTesting().Count);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NaturalBackstabs_ActualRaceReconciliation_FreshAndRepeatedInstallPreservesCustomLinks(bool supernatural)
	{
		using var context = Context();
		var shape = new BodypartShape { Name = "Claw" };
		var body = new BodyProto { Name = "Test Body" };
		var race = new Race { Name = supernatural ? "Werewolf Hybrid" : "Manticore", BaseBody = body };
		var aliases = supernatural ? new[] { "rhand", "lhand" } : new[] { "rfpaw", "lfpaw", "rrpaw", "lrpaw" };
		context.AddRange(shape, body, race);
		foreach (var alias in aliases) context.Add(new BodypartProto { Name = alias, Body = body, BodypartShape = shape });
		var damage = new TraitExpression { Name = "Animal Claw Damage", Expression = "40" };
		var pain = new TraitExpression { Name = "Physical Pain", Expression = "20" };
		var stun = new TraitExpression { Name = "Physical Stun", Expression = "10" };
		context.AddRange(damage, pain, stun); context.SaveChanges();
		var donor = new WeaponAttack
		{
			Name = "Claw Donor", BodypartShapeId = shape.Id, DamageExpression = damage, PainExpression = pain,
			StunExpression = stun, DamageType = (int)DamageType.Claw, MaximumTargets = 3,
			BaseDodgeDifficulty = (int)Difficulty.Easy, RequiredPositionStateIds = "1"
		};
		context.Add(donor); context.SaveChanges();
		var name = supernatural ? BackstabSeederHelper.WerewolfBackstab : BackstabSeederHelper.ManticoreBackstab;
		var configuration = supernatural ? BackstabSeederHelper.WerewolfConfiguration : BackstabSeederHelper.ManticoreConfiguration;
		var attack = BackstabSeederHelper.EnsureNaturalAttack(context, name, donor, configuration,
			supernatural ? 1.25 : 1.5, 6, supernatural ? 1.3 : 1.5, 150, "@ rake|rakes $1 with &0's {0}");
		Assert.AreEqual(1, attack.MaximumTargets);
		Assert.AreEqual(supernatural ? "1.25 * (20)" : "1.5 * (20)", attack.PainExpression.Expression);
		Assert.AreEqual(supernatural ? "1.25 * (10)" : "1.5 * (10)", attack.StunExpression.Expression);
		var custom = new WeaponAttack { Name = "Builder Custom Attack" };
		context.Add(new RacesWeaponAttacks { Race = race, Bodypart = context.BodypartProtos.First(), WeaponAttack = custom,
			Quality = (int)ItemQuality.Legendary });
		context.SaveChanges();
		void Reconcile()
		{
			if (supernatural)
			{
				var seeder = new SupernaturalSeeder();
				typeof(SupernaturalSeeder).GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(seeder, context);
				typeof(SupernaturalSeeder).GetMethod("SeedNaturalAttacks", BindingFlags.NonPublic | BindingFlags.Instance)!
					.Invoke(seeder, [race, SupernaturalSeeder.TemplatesForTesting[race.Name]]);
			}
			else
			{
				var seeder = new MythicalAnimalSeeder();
				typeof(MythicalAnimalSeeder).GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(seeder, context);
				typeof(MythicalAnimalSeeder).GetMethod("ReconcileMythicalNaturalAttackLinks", BindingFlags.NonPublic | BindingFlags.Instance)!
					.Invoke(seeder, []);
			}
			context.SaveChanges();
		}
		Reconcile(); Reconcile();
		var links = context.RacesWeaponAttacks.Where(x => x.WeaponAttackId == attack.Id).ToList();
		Assert.AreEqual(aliases.Length, links.Count);
		CollectionAssert.AreEquivalent(aliases, links.Select(x => x.Bodypart.Name).ToArray());
		Assert.AreEqual(1, context.RacesWeaponAttacks.Count(x => x.WeaponAttackId == custom.Id && x.Quality == (int)ItemQuality.Legendary));
	}

	private static void AssertProfile(WeaponAttack attack, double stamina, double delay, double weight,
		double reel, double targetOffence, double penetration, double bypass)
	{
		Assert.AreEqual(stamina, attack.StaminaCost); Assert.AreEqual(delay, attack.BaseDelay); Assert.AreEqual(weight, attack.Weighting);
		var xml = XElement.Parse(attack.AdditionalInfo);
		Assert.AreEqual("Normal", xml.Element("Resist")!.Value);
		Assert.AreEqual(reel, (double)xml.Element("Reel")!);
		Assert.AreEqual(targetOffence, (double)xml.Element("TargetOffence")!);
		Assert.AreEqual(penetration, (double)xml.Element("Penetration")!);
		Assert.AreEqual(bypass, (double)xml.Element("ArmourBypass")!);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NaturalFormulaRerun_RefreshesCanonicalRandomnessAndPreservesCustomSourceVariants(bool custom)
	{
		using var context = Context();
		context.Add(new TraitDefinition { Id = 1, Name = "Strength", Type = 1 });
		var basis = custom ? "42" : "1.0 * (str:1 + (3 * quality)) * sqrt(degree+1)";
		var source = new TraitExpression { Name = "Animal Claw Damage", Expression = basis + " * rand(0.7,1.0)" };
		var donor = new WeaponAttack { Name = "Claw Swipe", DamageExpression = source, PainExpression = source, StunExpression = source };
		context.Add(donor); context.SaveChanges();
		var attack = BackstabSeederHelper.EnsureNaturalAttack(context, "Test Back Rake", donor,
			BackstabSeederHelper.PredatorConfiguration, 1.5, 5, 1.3, 200, "@ rake|rakes $1");
		if (custom) attack.DamageExpression.Expression = "1.5 * (42)";
		else source.Expression = basis + " * rand(0.2,1.0)";
		context.SaveChanges();
		BackstabSeederHelper.EnsureNaturalAttack(context, "Test Back Rake", donor,
			BackstabSeederHelper.PredatorConfiguration, 1.5, 5, 1.3, 200, "@ rake|rakes $1");
		Assert.AreEqual(custom ? "1.5 * (42)" : $"1.5 * ({source.Expression})", attack.DamageExpression.Expression);
	}
}

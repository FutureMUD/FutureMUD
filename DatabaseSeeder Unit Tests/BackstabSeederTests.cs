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

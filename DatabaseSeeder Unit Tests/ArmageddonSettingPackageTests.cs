#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using TraitDefinition = MudSharp.Models.TraitDefinition;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonSettingPackageTests
{
	internal static readonly IReadOnlyDictionary<string, string> Answers = new Dictionary<string, string>
	{
		["complexity"] = "armageddon", ["skillcapmodel"] = "class", ["skillgainmodel"] = "armageddon",
		["branching"] = "no", ["gerund"] = "no", ["modern"] = "no"
	};

	internal static FuturemudDatabaseContext Fixture()
	{
		var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		db.TraitDefinitions.AddRange(new[] { "Strength", "Constitution", "Agility", "Dexterity", "Intelligence", "Perception", "Willpower" }
			.Select((name, index) => new TraitDefinition { Id = index + 1, Name = name, Type = 1, Alias = name[..3].ToLowerInvariant() }));
		db.FutureProgs.AddRange(new FutureProg { Id = 1, FunctionName = "AlwaysTrue" }, new FutureProg { Id = 2, FunctionName = "AlwaysFalse" });
		db.StaticConfigurations.AddRange(new StaticConfiguration { SettingName = "DefaultSkillImprover", Definition = "0" }, new StaticConfiguration { SettingName = "DefaultSkillDecorator", Definition = "0" });
		db.LanguageDifficultyModels.Add(new LanguageDifficultyModels { Id = 1, Name = "Default" });
		db.SaveChanges();
		return db;
	}

	[TestMethod]
	public void MundaneProgression_InstallsCompleteRolesAndPreservesCustomisationsOnResume()
	{
		using var db = Fixture();
		db.Accounts.Add(new Account { Id = 1, Name = "Installer" });
		db.SaveChanges();
		new SkillPackageSeeder().SeedData(db, Answers);
		var messages = new List<string>();
		ArmageddonMundaneProgression.Install(db, messages);
		Assert.AreEqual(7, db.ChargenRoles.Count(x => x.Type == 0));
		Assert.AreEqual(24, db.ChargenRoles.Count(x => x.Type == 1));
		Assert.AreEqual(0, db.ChargenRolesTraits.Count(), "Native additive adjustments must not duplicate the composed grants.");
		Assert.AreEqual(33, db.ChargenRolesMerits.Count());
		var owned = ArmageddonSkillPackage.Resolve(db);
		var merchant = db.ChargenRoles.Single(x => x.Name == "Merchant");
		var linguist = db.ChargenRoles.Single(x => x.Name == "Linguist");
		var data = XElement.Parse(db.Merits.Single(x => x.Type == "Role Skill Progression").Definition);
		var rules = new RoleSkillProgression(data.Element("Rules")!.Elements("Rule").Select(x => new RoleSkillRule((long)x.Attribute("role")!, (long)x.Attribute("trait")!,
			(bool)x.Attribute("primary")!, (double)x.Attribute("opening")!, (double)x.Attribute("cap")!, (long?)x.Attribute("parent"), (double)x.Attribute("threshold")!, (double)x.Attribute("bonus")!)));
		Assert.AreEqual(317, rules.Rules.Count);
		var composition = rules.Compose([merchant.Id, linguist.Id]);
		Assert.AreEqual(50.0, composition[owned["arm.skill.literacy"].Id].ComposedOpening);
		Assert.AreEqual(75.0, composition[owned["arm.skill.literacy"].Id].Cap);
		Assert.AreEqual(35.0, composition[owned["arm.skill.handwriting"].Id].ComposedOpening);
		Assert.AreEqual(100.0, composition[owned["arm.skill.cavilish"].Id].ComposedOpening);
		Assert.AreEqual(200.0, composition[owned["arm.skill.cavilish"].Id].Cap);
		var ids = db.ChargenRoles.ToDictionary(x => x.Id, x => x.Name);
		merchant.Name = "Builder merchant";
		owned["arm.skill.peek"].Expression.Expression = "87";
		db.ChargenRolesMerits.Remove(db.ChargenRolesMerits.Single(x => x.ChargenRoleId == linguist.Id));
		db.CharacterTraits.Add(new CharacterTrait { CharacterId = 9000, TraitDefinitionId = owned["arm.skill.peek"].Id, Value = 57, AdditionalValue = 3 });
		db.SaveChanges();
		ArmageddonMundaneProgression.Install(db, messages);
		Assert.AreEqual(31, db.ChargenRoles.Count());
		Assert.IsTrue(db.ChargenRoles.All(x => ids.Keys.Contains(x.Id)));
		Assert.AreEqual("Builder merchant", merchant.Name);
		Assert.AreEqual("87", owned["arm.skill.peek"].Expression.Expression);
		Assert.AreEqual(32, db.ChargenRolesMerits.Count(), "Builder-removed links stay removed.");
		Assert.AreEqual(57.0, db.CharacterTraits.Single().Value);
		Assert.AreEqual(3.0, db.CharacterTraits.Single().AdditionalValue);
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void Manifest_PreservesSourceDistinctionsAndApprovedAliases()
	{
		Assert.AreEqual(112, ArmageddonSkillPackage.Skills.Count(x => !x.Scaffold));
		Assert.AreEqual(5, ArmageddonSkillPackage.Skills.Count(x => x.Language));
		Assert.AreEqual("arm.skill.ride", ArmageddonSkillPackage.CanonicalKey("arm.skill.mount"));
		Assert.AreEqual("arm.skill.guarding", ArmageddonSkillPackage.CanonicalKey("arm.skill.guard"));
		Assert.IsFalse(ArmageddonSkillPackage.Skills.Any(x => x.Key == "arm.skill.analyze"));
		foreach (var key in new[] { "backstab", "sap", "bash", "kick", "pilot", "literacy", "handwriting", "forgery", "stone_working", "stonecrafting" })
			Assert.IsTrue(ArmageddonSkillPackage.Skills.Any(x => x.Key == "arm.skill." + key), key);
		Assert.AreEqual("ArmageddonMagicSeeder", ArmageddonMagicInstaller.Package, "Public renaming must retain durable ownership.");
		Assert.AreEqual("Armageddon Setting", new ArmageddonSettingSeeder().Name);
	}

	[TestMethod]
	public void FrozenRoleContracts_HaveCompleteCanonicalCoverage()
	{
		Dictionary<string, string>[] Table(string name)
		{
			using var stream = typeof(ArmageddonSkillPackage).Assembly.GetManifestResourceStream("DatabaseSeeder.Armageddon." + name)!;
			using var reader = new StreamReader(stream);
			var columns = reader.ReadLine()!.Split('\t');
			var rows = new List<Dictionary<string, string>>();
			while (reader.ReadLine() is { } line)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				var values = line.Split('\t');
				Assert.AreEqual(columns.Length, values.Length, name);
				rows.Add(columns.Select((column, index) => (column, values[index])).ToDictionary(x => x.column, x => x.Item2));
			}
			return rows.ToArray();
		}
		var guilds = Table("guild_memberships.tsv");
		var subclasses = Table("subguild_memberships.tsv");
		Assert.AreEqual(226, guilds.Length);
		Assert.AreEqual(7, guilds.Select(x => x["guild"]).Distinct().Count());
		Assert.AreEqual(93, subclasses.Length);
		Assert.AreEqual(24, subclasses.Select(x => x["subguild"]).Distinct().Count());
		Assert.AreEqual(172, Table("subguild_main_overlaps.tsv").Length);
		Assert.AreEqual(7, Table("optional_writing_grants_20261016.tsv").Length);
		var keys = ArmageddonSkillPackage.Skills.Select(x => x.Key).ToHashSet();
		foreach (var row in guilds.Concat(subclasses))
		{
			var key = ArmageddonSkillPackage.CanonicalKey(row["canonical_key"]);
			Assert.IsTrue(key is "arm.skill.analyze" or ArmageddonMundaneMerits.BardKey or ArmageddonMundaneMerits.MercenaryKey || keys.Contains(key), key);
		}
	}

	[TestMethod]
	public void EarlySkills_BindCanonicalActionsAndRetainBuilderRenamesOnRerun()
	{
		using var db = Fixture();
		var seeder = new SkillPackageSeeder();
		seeder.SeedData(db, Answers);
		var owned = ArmageddonSkillPackage.Resolve(db);
		Assert.AreEqual(142, ArmageddonSkillPackage.Skills.Count);
		Assert.AreEqual(200.0, double.Parse(owned["arm.skill.sirihish"].Expression.Expression));
		Assert.AreEqual((int)TraitOwnerScope.Character, owned["arm.skill.peek"].OwnerScope);
		Assert.AreEqual(owned["arm.skill.ride"].Id, owned["arm.skill.mount"].Id);
		StringAssert.Contains(db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.DirectionSenseCheck).TraitExpression.Expression,
			$":{owned["arm.skill.direction_sense"].Id}");
		StringAssert.Contains(db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.PeekCheck).TraitExpression.Expression,
			$":{owned["arm.skill.peek"].Id}");
		StringAssert.Contains(db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.VehicleChargeCheck).TraitExpression.Expression,
			$":{owned["arm.skill.pilot"].Id}");
		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, new ArmageddonSettingSeeder().ShouldSeedData(db));
		Assert.ThrowsException<InvalidOperationException>(() => new SkillSeeder().SeedData(db, new Dictionary<string, string>()));
		foreach (var binding in ArmageddonSkillPackage.StaticTraitBindings)
			Assert.AreEqual(owned[binding.Value].Id.ToString(), db.StaticConfigurations.Find(binding.Key)!.Definition);
		foreach (var key in new[] { "VehiclePilotEnabled", "DirectionSenseEnabled", "ArmageddonHostileGuardAttemptsEnabled", "HagglingEnabled" })
		{
			Assert.AreEqual("true", db.StaticConfigurations.Find(key)!.Definition, key);
			db.StaticConfigurations.Find(key)!.Definition = "false";
		}
		foreach (var key in new[] { "ElectricalCommandEnabled", "ProgrammingCommandEnabled" }) { Assert.AreEqual("false", db.StaticConfigurations.Find(key)!.Definition, key); db.StaticConfigurations.Find(key)!.Definition = "true"; }
		var ids = owned.ToDictionary(x => x.Key, x => x.Value.Id);
		db.CharacterTraits.Add(new CharacterTrait { CharacterId = 9000, TraitDefinitionId = owned["arm.skill.peek"].Id, Value = 57, AdditionalValue = 3 });
		owned["arm.skill.peek"].Name = "Builder covert inspection";
		owned["arm.skill.peek"].Expression.Expression = "87";
		db.SaveChanges();
		seeder.SeedData(db, Answers);
		foreach (var key in new[] { "VehiclePilotEnabled", "DirectionSenseEnabled", "ArmageddonHostileGuardAttemptsEnabled", "HagglingEnabled" }) Assert.AreEqual("false", db.StaticConfigurations.Find(key)!.Definition, key);
		foreach (var key in new[] { "ElectricalCommandEnabled", "ProgrammingCommandEnabled" }) Assert.AreEqual("true", db.StaticConfigurations.Find(key)!.Definition, key);
		var after = ArmageddonSkillPackage.Resolve(db);
		Assert.IsTrue(ids.All(x => after[x.Key].Id == x.Value));
		Assert.AreEqual("Builder covert inspection", after["arm.skill.peek"].Name);
		Assert.AreEqual("87", after["arm.skill.peek"].Expression.Expression);
		Assert.AreEqual(57.0, db.CharacterTraits.Single().Value);
		Assert.AreEqual(3.0, db.CharacterTraits.Single().AdditionalValue);
		Assert.IsTrue(db.Languages.Any(x => x.Name == "Admin Speech"));
	}

	[TestMethod]
	public void PendingSourceActions_DoNotExposeGenericStockAsSourceParity()
	{
		using var db = Fixture();
		db.ManualCombatCommands.AddRange(new ManualCombatCommand { PrimaryVerb = "bash", PlayerUsable = true, NpcUsable = true },
			new ManualCombatCommand { PrimaryVerb = "kick", PlayerUsable = true, NpcUsable = true });
		db.CombatActions.Add(new CombatAction { Name = "Bash", UsabilityProgId = 1 });
		db.WeaponAttacks.AddRange(new WeaponAttack { Name = "Knife Backstab", FutureProgId = 1 },
			new WeaponAttack { Name = "Blackjack Sapping Blow", FutureProgId = 1 },
			new WeaponAttack { Name = "Knife Stab", FutureProgId = 1 });
		db.SaveChanges();
		CombatSeeder.GatePendingArmageddonActions(db);
		db.SaveChanges();
		Assert.IsTrue(db.ManualCombatCommands.All(x => !x.PlayerUsable && !x.NpcUsable && x.UsabilityProgId == 2));
		Assert.AreEqual(2L, db.CombatActions.Single().UsabilityProgId);
		Assert.AreEqual(1L, db.WeaponAttacks.Single(x => x.Name == "Knife Backstab").FutureProgId);
		Assert.AreEqual(1L, db.WeaponAttacks.Single(x => x.Name == "Blackjack Sapping Blow").FutureProgId);
		Assert.AreEqual(1L, db.WeaponAttacks.Single(x => x.Name == "Knife Stab").FutureProgId);
	}

	[TestMethod]
	public void EarlySkills_RefuseStockWorldConversionBeforeWriting()
	{
		using var db = Fixture();
		db.TraitDefinitions.Add(new TraitDefinition { Name = "Custom skill", Type = 0 });
		db.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => new SkillPackageSeeder().SeedData(db, Answers));
		Assert.AreEqual(8, db.TraitDefinitions.Count());
		Assert.AreEqual(0, db.CheckTemplates.Count());
		Assert.AreEqual(0, db.SeederManagedRecords.Count());
	}

	[TestMethod]
	public void MundaneMeritDefinitions_AreScopedAndPreserveBuilderEdits()
	{
		using var db = Fixture();
		var messages = new List<string>();
		var ids = ArmageddonMundaneMerits.Install(db, messages);
		var bard = db.Merits.Single(x => x.Id == ids[ArmageddonMundaneMerits.BardKey]);
		var xml = XElement.Parse(bard.Definition);
		Assert.AreEqual("50", (string?)xml.Attribute("branching"));
		Assert.AreEqual("5", (string?)xml.Attribute("improving"));
		CollectionAssert.AreEqual(new[] { "Language" }, xml.Element("Groups")!.Elements("Group").Select(x => x.Value).ToArray());
		var mercenary = db.Merits.Single(x => x.Id == ids[ArmageddonMundaneMerits.MercenaryKey]);
		Assert.AreEqual("2", (string?)XElement.Parse(mercenary.Definition).Attribute("multiplier"));
		mercenary.Definition = "<Definition multiplier='1.5'/>";
		db.SaveChanges();
		var after = ArmageddonMundaneMerits.Install(db, messages);
		Assert.IsTrue(ids.All(x => after[x.Key] == x.Value));
		Assert.AreEqual("<Definition multiplier='1.5'/>", mercenary.Definition);
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
		Assert.AreEqual(0, db.ChargenRolesMerits.Count(), "Definition installation must not imply role or player grants.");
	}
}

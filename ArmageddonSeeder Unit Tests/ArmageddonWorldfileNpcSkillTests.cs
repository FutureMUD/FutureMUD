#nullable enable

using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Traits;
using MudSharp.Models;
using TraitDefinition = MudSharp.Models.TraitDefinition;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileNpcSkillTests
{
	[TestMethod]
	public void SavedSkills_ReviewedAliasesPreserveValuesWithoutGrantingSpellNames()
	{
		var bindings = ArmageddonWorldfileWriter.NpcSkillBindings(
			[Trait(1, "Dual-Wield"), Trait(2, "Throwing"), Trait(3, "Drive"), Trait(4, "Ride"), Trait(5, "Block")], []);
		var names = new string[246];
		names[127] = "dual wield"; names[128] = "throw"; names[134] = "pilot";
		names[245] = "shield use";
		names[56] = "mount"; names[50] = "guardian";
		var skills = new Dictionary<long, double>();
		var unmapped = ArmageddonWorldfileWriter.ApplyNpcSavedSkills(skills,
			[[127, 73], [128, 41], [134, 62], [56, 100], [50, 90], [245, 55]], names, bindings, null);
		Assert.AreEqual(73d, skills[1]);
		Assert.AreEqual(41d, skills[2]);
		Assert.AreEqual(62d, skills[3]);
		Assert.IsFalse(skills.ContainsKey(4), "The spell named mount must not become the Ride skill.");
		Assert.AreEqual(55d, skills[5]);
		CollectionAssert.AreEqual(new[] { 56, 50 }, unmapped.Select(x => x.Index).ToArray());
	}

	[TestMethod]
	public void SavedSkills_OwnedIdentitiesSurviveRenamingAndBeatUnrelatedNameMatches()
	{
		var bindings = ArmageddonWorldfileWriter.NpcSkillBindings([Trait(22, "Stone Tossing"), Trait(23, "Throwing")],
			[new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = ArmageddonSkillPackage.Module,
				EntityType = nameof(TraitDefinition), StableKey = "arm.skill.throw", LogicalId = 22 }]);
		Assert.AreEqual(22L, bindings["throw"]);
		Assert.AreEqual(22L, bindings["Throwing"]);
		var skills = new Dictionary<long, double>();
		var names = new string[92];
		names[91] = "willpower";
		Assert.AreEqual(0, ArmageddonWorldfileWriter.ApplyNpcSavedSkills(skills, [[91, 67]], names, bindings, 77).Count);
		Assert.AreEqual(67d, skills[77], "The separately installed Willpower identity must not depend on its display name.");
	}

	[TestMethod]
	public void RacialSkills_ApplySourceDefaultsOnceWithApprovedNativeCap()
	{
		var bindings = ArmageddonWorldfileWriter.NpcSkillBindings([Trait(1, "Steal"), Trait(2, "Haggle"), Trait(3, "Sneak"), Trait(4, "Hide"), Trait(5, "Climb")], []);
		var halfling = new Dictionary<long, double> { [3] = 95, [5] = 12 };
		ArmageddonWorldfileWriter.ApplyNpcRacialSkills(halfling, 12, bindings);
		Assert.AreEqual(100d, halfling[3]);
		Assert.AreEqual(20d, halfling[4]);
		Assert.AreEqual(22d, halfling[5]);
		var elf = new Dictionary<long, double> { [1] = 40 };
		ArmageddonWorldfileWriter.ApplyNpcRacialSkills(elf, 2, bindings);
		Assert.AreEqual(50d, elf[1]);
		Assert.AreEqual(20d, elf[2]);
		var mantis = new Dictionary<long, double>();
		ArmageddonWorldfileWriter.ApplyNpcRacialSkills(mantis, 4, bindings);
		Assert.AreEqual(1, mantis.Count);
		Assert.AreEqual(20d, mantis[4]);
		var desertElf = new Dictionary<long, double>();
		ArmageddonWorldfileWriter.ApplyNpcRacialSkills(desertElf, 98, bindings);
		Assert.AreEqual(2, desertElf.Count);
		Assert.AreEqual(20d, desertElf[3]);
		Assert.AreEqual(20d, desertElf[4]);
	}

	[TestMethod]
	public void PrivateCorpus_ReviewedAliasesRecoverRetainedNpcSkillRecords()
	{
		ArmageddonWorldfileTestData.RequireAvailable();
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var retained = source.Resets.Where(x => x.Opcode == "M").Select(x => x.Arguments[1]).ToHashSet();
		var bindings = ArmageddonWorldfileWriter.NpcSkillBindings([Trait(1, "Dual-Wield"), Trait(2, "Throwing"), Trait(3, "Drive")], []);
		var recovered = 0;
		foreach (var npc in source.Npcs.Where(x => retained.Contains(x.Vnum)))
		{
			var rows = npc.Skills.Where(x => x[0] is 127 or 128 or 134).ToArray();
			var skills = new Dictionary<long, double>();
			var missing = ArmageddonWorldfileWriter.ApplyNpcSavedSkills(skills, rows, source.Skills, bindings, null);
			Assert.AreEqual(0, missing.Count, $"NPC {npc.Vnum} has an unresolved reviewed alias.");
			recovered += rows.Length;
		}
	}

	private static TraitDefinition Trait(long id, string name) => new()
		{ Id = id, Name = name, Type = 0, OwnerScope = (int)TraitOwnerScope.Character };
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockProgressionTests
{
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void ClassOpenings_FreshInstall_LeavesOpeningSkillsToCapabilityEnrolment(ArmageddonProgressionMode mode)
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture(true);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		ArmageddonInstallation.Prepare(factory, mode);
		var result = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status);
		Assert.AreEqual(10, db.ChargenRolesMerits.Count());
		Assert.AreEqual(0, db.ChargenRolesTraits.Count(), "Role traits must not add culture starting values and Class adjustments before authored enrolment.");
	}

	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void ClassOpenings_LegacyRoleLinks_ReconcileCanonicalOnlyAndPreserveBuilderEdits(ArmageddonProgressionMode mode, bool edited)
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture(true);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		ArmageddonInstallation.Prepare(factory, mode);
		var first = ArmageddonProgressionInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		var stock = new ArmageddonStockProgression(mode);
		var capability = stock.Capabilities.Single(x => x.Key == "arm.capability.sorcerer");
		var role = db.SeederManagedRecords.Single(x => x.StableKey == capability.RoleKey);
		var roleId = role.LogicalId!.Value;
		var meritIds = db.ChargenRolesMerits.Where(x => x.ChargenRoleId == roleId).Select(x => x.MeritId).ToArray();
		var legacy = stock.Bindings.Where(x => x.CapabilityKey == capability.Key && x.Starting)
			.GroupBy(x => first.Identities[x.TraitKey])
			.Select(x => new { Id = x.Key, Amount = x.Max(y => y.Opening), Give = true })
			.OrderBy(x => x.Id).ToArray();
		db.ChargenRolesTraits.RemoveRange(db.ChargenRolesTraits.Where(x => x.ChargenRoleId == roleId));
		db.SaveChanges();
		foreach (var value in legacy)
			db.ChargenRolesTraits.Add(new ChargenRolesTrait { ChargenRoleId = roleId, TraitId = value.Id, Amount = value.Amount, GiveIfDoesntHave = true });
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(role.SeedBaseline!)!;
		baseline["RoleTraits"] = JsonSerializer.Serialize(legacy);
		role.SeedBaseline = JsonSerializer.Serialize(baseline);
		db.SaveChanges();
		if (edited)
		{
			var join = db.ChargenRolesTraits.First(x => x.ChargenRoleId == roleId);
			join.Amount += 1;
			db.SaveChanges();
		}
		var expected = db.ChargenRolesTraits.Where(x => x.ChargenRoleId == roleId).OrderBy(x => x.TraitId)
			.Select(x => new { Id = x.TraitId, x.Amount, Give = x.GiveIfDoesntHave }).ToArray();
		for (var rerun = 0; rerun < 2; rerun++)
		{
			var result = ArmageddonProgressionInstaller.Install(factory);
			Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
			db.ChangeTracker.Clear();
			Assert.AreEqual(roleId, db.SeederManagedRecords.Single(x => x.StableKey == capability.RoleKey).LogicalId);
			CollectionAssert.AreEqual(meritIds, db.ChargenRolesMerits.Where(x => x.ChargenRoleId == roleId).Select(x => x.MeritId).ToArray());
			var actual = db.ChargenRolesTraits.Where(x => x.ChargenRoleId == roleId).OrderBy(x => x.TraitId)
				.Select(x => new { Id = x.TraitId, x.Amount, Give = x.GiveIfDoesntHave }).ToArray();
			Assert.AreEqual(edited ? JsonSerializer.Serialize(expected) : "[]", JsonSerializer.Serialize(actual));
			if (edited) Assert.IsTrue(result.Messages.Any(x => x.Contains("RoleTraits", StringComparison.Ordinal) && x.Contains("preserv", StringComparison.OrdinalIgnoreCase)));
		}
		Assert.AreEqual(0, db.Characters.Count());
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}
}

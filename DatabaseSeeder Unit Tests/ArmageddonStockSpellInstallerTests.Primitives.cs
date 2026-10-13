#nullable enable

using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void CanonicalDefinitions_EveryInstalledEntryMatchesLockedCostsGradesAndTypedChildren(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var entries = manifest.RootElement.GetProperty("entries").EnumerateArray().ToDictionary(x => x.GetProperty("key").GetString()!);
		var claims = db.SeederManagedRecords.Where(x => x.Module == ArmageddonStockSpellInstaller.Module && x.EntityType == "MagicSpell").ToArray();
		Assert.AreEqual(36, claims.Length);
		var settings = ArmageddonInstallation.Read(db)!;
		var source = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonProgressionInstaller.SourceResourceKey).LogicalId!.Value;
		foreach (var claim in claims)
		{
			Assert.IsFalse(claim.Retired); Assert.IsNotNull(claim.SeedBaseline); Assert.IsNull(claim.RevisionNumber);
			var spell = db.MagicSpells.Single(x => x.Id == claim.LogicalId);
			var root = XElement.Parse(spell.Definition);
			Assert.AreEqual(claim.StableKey, (string?)root.Element("StockIdentity"), claim.StableKey);
			var entry = entries[claim.StableKey];
			var minimum = entry.GetProperty("minimum_energy").GetDouble();
			Assert.AreEqual(minimum, (double)root.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!, claim.StableKey);
			var expectedTarget = entry.GetProperty("target_type").GetString();
			if (claim.StableKey == "arm.spell.unravel_enchantment")
			{
				// Inventory retains its broad historical label; the locked defining contract selects character dispel.
				Assert.AreEqual("perceivable", expectedTarget);
				expectedTarget = "character";
			}
			Assert.AreEqual(expectedTarget, (string?)root.Element("Trigger")!.Attribute("type"), claim.StableKey);
			CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), root.Element("ControlledPower")!.Elements("Grade").Select(x => (int)x.Attribute("number")!).ToArray(), claim.StableKey);
			var duration = db.SeederManagedRecords.Single(x => x.StableKey == claim.StableKey + ".duration");
			var cost = db.SeederManagedRecords.Single(x => x.StableKey == claim.StableKey + ".cost");
			Assert.AreEqual("TraitExpression", duration.EntityType); Assert.AreEqual("TraitExpression", cost.EntityType);
			Assert.AreEqual(duration.LogicalId, spell.EffectDurationExpressionId);
			Assert.AreEqual(source, (long)root.Element("Costs")!.Element("Cost")!.Attribute("resource")!);
			Assert.AreEqual(cost.LogicalId, (long?)root.Element("Costs")!.Element("Cost")!.Attribute("expression"));
			Assert.AreEqual($"{minimum.ToString(System.Globalization.CultureInfo.InvariantCulture)}*grade", db.TraitExpressions.Single(x => x.Id == cost.LogicalId).Expression, claim.StableKey);
			Assert.IsFalse(spell.ScrollInscriptionAllowed);
			Assert.AreEqual(settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg], spell.SpellKnownProgId);
		}
	}

	[TestMethod]
	public void PrimitiveBindings_ExactNativeAttributesViewerPolicyAndVoidPayloadRemainDistinctFromCosts()
	{
		using var db = Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		db.ChangeTracker.Clear();
		XElement Effect(string suffix) => XElement.Parse(db.MagicSpells.Single(x => x.Id ==
			db.SeederManagedRecords.Single(c => c.StableKey == "arm.spell." + suffix).LogicalId).Definition).Element("Effects")!.Elements().Single();
		Assert.AreEqual(db.TraitDefinitions.Single(x => x.Alias == "str").Id, (long)Effect("earthen_strength").Attribute("trait")!);
		Assert.AreEqual(db.TraitDefinitions.Single(x => x.Alias == "str").Id, (long)Effect("sapping_weight").Attribute("trait")!);
		var settings = ArmageddonInstallation.Read(db)!;
		Assert.AreEqual(settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg], (long)Effect("veil_from_sight").Element("FilterProg")!);
		var still = db.MagicSpells.Single(x => x.Id == db.SeederManagedRecords.Single(c => c.StableKey == "arm.spell.still_limbs").LogicalId);
		Assert.AreEqual(settings.CapacityAttribute, still.ResistingTraitDefinitionId); Assert.AreEqual((int)Difficulty.Normal, still.ResistingDifficulty);
		var payload = (long)Effect("empty_aura").Element("Resource")!;
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.resource.void").LogicalId, payload);
		Assert.AreNotEqual(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonProgressionInstaller.SourceResourceKey).LogicalId, payload);
		Assert.AreEqual("-5*grade", Effect("empty_aura").Element("Formula")!.Value);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
	}

	[DataTestMethod]
	[DataRow("strength")][DataRow("viewer")][DataRow("reserve")]
	public void PrimitiveDependencyLoss_HoldsNewBatchAndPreservesEarlierCommittedSlices(string missing)
	{
		using var db = Fixture();
		var settings = ArmageddonInstallation.Read(db)!;
		if (missing == "strength") db.TraitDefinitions.Remove(db.TraitDefinitions.Single(x => x.Alias == "str"));
		else if (missing == "viewer") db.FutureProgs.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]).AcceptsAnyParameters = false;
		else db.SeederManagedRecords.Single(x => x.StableKey == "arm.resource.void").Retired = true;
		db.SaveChanges();
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
		Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == ArmageddonStockSpellContent.ReadAttunementKey));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.spell.borrowed_tongues"));
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}
}

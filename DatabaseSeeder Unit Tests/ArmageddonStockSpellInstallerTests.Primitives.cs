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
		CollectionAssert.AreEquivalent(ArmageddonStockSpellInstaller.CanonicalKeys.ToArray(), claims.Select(x => x.StableKey).ToArray());
		Assert.AreEqual(152, ArmageddonStockSpellInstaller.CanonicalKeys.Count);
		var settings = ArmageddonInstallation.Read(db)!;
		var source = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonProgressionInstaller.SourceResourceKey).LogicalId!.Value;
		foreach (var claim in claims)
		{
			Assert.IsFalse(claim.Retired); Assert.IsNotNull(claim.SeedBaseline); Assert.IsNull(claim.RevisionNumber);
			var spell = db.MagicSpells.Single(x => x.Id == claim.LogicalId);
			var root = XElement.Parse(spell.Definition);
			Assert.AreEqual(claim.StableKey, (string?)root.Element("StockIdentity"), claim.StableKey);
			var entry = entries[claim.StableKey];
			Assert.AreEqual("Armageddon " + entry.GetProperty("native_school").GetString(), db.MagicSchools.Single(x => x.Id == spell.MagicSchoolId).Name, claim.StableKey);
			var minimum = entry.GetProperty("minimum_energy").GetDouble();
			Assert.AreEqual(minimum, (double)root.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!, claim.StableKey);
			var expectedTarget = entry.GetProperty("target_type").GetString();
			if (claim.StableKey == ArmageddonOpenThresholdSpellContent.Key)
			{
				Assert.AreEqual("room", expectedTarget);
				expectedTarget = "thresholddestination";
			}
			if (claim.StableKey is ArmageddonWeightedTravelSpellContent.WaystepKey or ArmageddonWeightedTravelSpellContent.BanishKey)
				expectedTarget = claim.StableKey == ArmageddonWeightedTravelSpellContent.WaystepKey ? "waystepdestination" : "banishtraveller";
			if (claim.StableKey is ArmageddonTracePathSpellContent.Key or ArmageddonRemoteTravellerSpellContent.JoinKey or ArmageddonRemoteTravellerSpellContent.DrawKey)
			{
				Assert.AreEqual(claim.StableKey == ArmageddonRemoteTravellerSpellContent.JoinKey ? "room" : "character", expectedTarget, claim.StableKey);
				expectedTarget = "recognisedcharacter";
			}
			if (claim.StableKey == ArmageddonGroundTremorSpellContent.Key)
			{
				Assert.AreEqual("characters", expectedTarget);
				expectedTarget = "room"; // The dedicated room adapter also runs when local protected bodies leave zero injury recipients.
			}
			if (claim.StableKey == ArmageddonReturnTetherSpellContent.Key)
			{
				Assert.AreEqual("character", expectedTarget);
				expectedTarget = "self"; // The locked anchor always records and returns the caster's exact body.
			}
			if (claim.StableKey == ArmageddonExchangePlacesSpellContent.Key)
			{
				Assert.AreEqual("character", expectedTarget);
				expectedTarget = "mindcontactcharacter"; // Source transference selects an exact active outbound contact.
			}
			if (claim.StableKey is "arm.spell.seeking_wind" or "arm.spell.wind_courier" or "arm.spell.haunting_shape")
			{
				// Inventory's room delivery has an exact typed mission-input trigger, documented in the casting contract.
				Assert.AreEqual("room", expectedTarget);
				expectedTarget = "roommission";
			}
			if (claim.StableKey == "arm.spell.sustain_meal")
			{
				Assert.AreEqual("character", expectedTarget);
				Assert.IsFalse(string.IsNullOrWhiteSpace(ArmageddonCanonicalUtilityContent.SustainMealTargetException));
				expectedTarget = "room"; // Keep the locked caster-room food delivery, not character inventory placement.
			}
			if (claim.StableKey is "arm.spell.raise_servitor" or "arm.spell.voice_of_remains")
			{
				Assert.AreEqual("corpse", expectedTarget);
				expectedTarget = "item"; // Native corpse component admission specializes the item trigger.
			}
			if (claim.StableKey == "arm.spell.unravel_enchantment")
			{
				// Inventory retains its broad historical label; the locked defining contract selects character dispel.
				Assert.AreEqual("perceivable", expectedTarget);
				expectedTarget = "character";
			}
			if (claim.StableKey is ArmageddonWindForceSpellContent.GustHandsKey or ArmageddonWindForceSpellContent.DrivingGustKey or
				ArmageddonWindHammerSpellContent.Key)
			{
				Assert.AreEqual("character_and_exit", expectedTarget);
				expectedTarget = "characterexit"; // Native two-part target trigger.
			}
			if (claim.StableKey is ArmageddonAreaHarmSpellContent.CinderRainKey or ArmageddonAreaHarmSpellContent.ThunderclapKey or
				ArmageddonAreaHarmSpellContent.ScouringGustKey or ArmageddonAreaHarmSpellContent.CausticSprayKey or
				ArmageddonLeapingLightningSpellContent.Key)
			{
				Assert.AreEqual("characters", expectedTarget);
				expectedTarget = "character"; // The explicit controlled-area policy selects and guards physical character recipients.
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
			if (claim.StableKey == ArmageddonLeapingLightningSpellContent.Key)
			{
				var area = root.Element("ControlledPower")!.Element("Area")!;
				Assert.AreEqual(1, (int)area.Attribute("applicationBase")!);
				Assert.AreEqual(1, (int)area.Attribute("applicationsPerGrade")!);
				Assert.AreEqual(15, (int)area.Attribute("applications")!);
				Assert.AreEqual(0.25, (double)area.Attribute("casterDamage")!);
				var chain = area.Element("ChainLightning")!;
				foreach (var (attribute, key) in new[]
				{
					("siltTag", ArmageddonStockSupportInstaller.SiltTagKey),
					("shallowsTag", ArmageddonStockSupportInstaller.ShallowsTagKey),
					("waterPlane", ArmageddonStockSupportInstaller.WaterPlaneKey),
					("lightningPlane", ArmageddonStockSupportInstaller.LightningPlaneKey)
				})
					Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == key).LogicalId,
						(long?)chain.Attribute(attribute), key);
				var damage = root.Element("Effects")!.Element("Effect")!;
				Assert.AreEqual(11, (int)damage.Element("DamageType")!);
				Assert.AreEqual(ArmageddonLeapingLightningSpellContent.ProtectionTag,
					(string?)damage.Element("InterdictionTag"));
				Assert.IsNotNull(damage.Element("IdentitySafeDamage"));
			}
			if (claim.StableKey == "arm.spell.charged_aegis")
			{
				var ward = root.Element("Effects")!.Elements("Effect").Single(x =>
					(string?)x.Attribute("type") == "personaltagward");
				Assert.AreEqual(ArmageddonLeapingLightningSpellContent.ProtectionTag, (string?)ward.Element("Tag"));
				Assert.AreEqual(MagicInterdictionCoverage.Incoming.ToString(), (string?)ward.Element("Coverage"));
			}
		}
	}

	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void DrowningGrip_UsesNativeBreathingInterruptionAndBoundedResistedLifetime(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		var spell = db.MagicSpells.Single(x => x.Id == result.Identities[ArmageddonDrowningGripSpellContent.Key]);
		var root = XElement.Parse(spell.Definition);
		Assert.AreEqual("drowninggrip", (string?)root.Element("Effects")!.Elements().Single().Attribute("type"));
		Assert.AreEqual("character", (string?)root.Element("Trigger")!.Attribute("type"));
		Assert.AreEqual(false, (bool)root.Element("Trigger")!.Element("CanTargetSelf")!);
		Assert.AreEqual("5*grade", db.TraitExpressions.Single(x => x.Id == result.Identities[ArmageddonDrowningGripSpellContent.Key + ".duration"]).Expression);
		Assert.IsTrue(spell.ResistingTraitDefinitionId > 0);
		Assert.AreEqual((int)Difficulty.Normal, spell.ResistingDifficulty);
		Assert.AreEqual(0, root.Element("Plan")!.Elements().Count());
	}

	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void FixedConcealmentAndWaterBreathing_UseExactViewerAndWaterBindings(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var installed = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		XElement Definition(string key) => XElement.Parse(db.MagicSpells.Single(x => x.Id == installed.Identities[key]).Definition);
		var concealment = Definition("arm.spell.false_presence");
		Assert.AreEqual("invisibility", (string?)concealment.Element("Effects")!.Elements().Single().Attribute("type"));
		Assert.AreEqual(ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg],
			(long)concealment.Element("Effects")!.Elements().Single().Element("FilterProg")!);
		Assert.AreEqual("60*grade", db.TraitExpressions.Single(x => x.Id == installed.Identities["arm.spell.false_presence.duration"]).Expression);
		var water = Definition("arm.spell.water_breathing");
		var effect = water.Element("Effects")!.Elements().Single();
		Assert.AreEqual("sourcewaterbreathing", (string?)effect.Attribute("type"));
		var scope = effect.Element("WaterScope")!;
		Assert.AreEqual(1, (int)scope.Attribute("version")!);
		using var alias = JsonDocument.Parse(db.SeederManagedRecords.Single(x => x.StableKey == "arm.liquid.water").SeedBaseline!);
		var waterId = alias.RootElement.GetProperty("NativeId").GetInt64();
		Assert.AreEqual(waterId, (long)scope.Elements("Liquid").Single().Attribute("id")!);
		Assert.IsTrue(db.Liquids.Any(x => x.Id == waterId && x.Density > 0));
		Assert.AreEqual("0", db.TraitExpressions.Single(x => x.Id == installed.Identities["arm.spell.water_breathing.duration"]).Expression);
		var see = Definition("arm.spell.see_the_unbodied");
		var sourceScope = see.Element("Effects")!.Elements().Single().Element("SourceScope")!;
		Assert.AreEqual(2, (int)sourceScope.Attribute("version")!);
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.SiltTagKey).LogicalId,
			(long)sourceScope.Attribute("siltTag")!);
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.ShadowPlaneKey).LogicalId,
			(long)sourceScope.Attribute("shadowPlane")!);
		var rankTags = Enumerable.Range(0, 5).Select(rank =>
		{
			var key = "arm.material.divination.rank" + rank;
			Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == key));
			return db.SeederManagedRecords.Single(x => x.StableKey == key + ".tag").LogicalId!.Value;
		}).ToArray();
		for (var rank = 1; rank < rankTags.Length; rank++)
			Assert.AreEqual(rankTags[rank - 1], db.Tags.Single(x => x.Id == rankTags[rank]).ParentId);
		var action = see.Element("Plan")!.Element("Phase")!.Element("Action")!;
		Assert.AreEqual(true, (bool)action.Attribute("carriedonly")!);
		Assert.AreEqual(1, (int)action.Attribute("quantity")!);
		Assert.AreEqual(-3, (int)action.Element("GradeRank")!.Attribute("offset")!);
		CollectionAssert.AreEqual(rankTags, action.Element("GradeRank")!.Elements("Rank").Select(x => (long)x.Attribute("tag")!).ToArray());
		var sheltering = Definition("arm.spell.sheltering_veil");
		var shelterScope = sheltering.Element("Effects")!.Elements().Single().Element("SourceArmourScope")!;
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.WaterPlaneKey).LogicalId,
			(long)shelterScope.Attribute("waterPlane")!);
		var clerical = Enumerable.Range(0, 5).Select(rank =>
		{
			var key = "arm.material.clerical.rank" + rank;
			Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == key));
			return db.SeederManagedRecords.Single(x => x.StableKey == key + ".tag").LogicalId!.Value;
		}).ToArray();
		for (var rank = 1; rank < clerical.Length; rank++)
			Assert.AreEqual(clerical[rank - 1], db.Tags.Single(x => x.Id == clerical[rank]).ParentId);
		var shelterAction = sheltering.Element("Plan")!.Element("Phase")!.Element("Action")!;
		Assert.AreEqual(true, (bool)shelterAction.Attribute("carriedonly")!);
		Assert.AreEqual(1, (int)shelterAction.Attribute("quantity")!);
		Assert.AreEqual(-3, (int)shelterAction.Element("GradeRank")!.Attribute("offset")!);
		CollectionAssert.AreEqual(clerical, shelterAction.Element("GradeRank")!.Elements("Rank").Select(x => (long)x.Attribute("tag")!).ToArray());
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
	}

	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void MovementTempo_InstalledChildrenRetainExactOrdinaryAgilityBinding(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		using var agilityAlias = JsonDocument.Parse(db.SeederManagedRecords.Single(x =>
			x.StableKey == "arm.binding.trait.agility").SeedBaseline!);
		var agility = agilityAlias.RootElement.GetProperty("NativeId").GetInt64();
		Assert.IsTrue(db.TraitDefinitions.Any(x => x.Id == agility && x.Type == 1 && x.DerivedType == 0 &&
			x.OwnerScope == 0 && x.TraitGroup == "Physical"));
		foreach (var key in new[] { "arm.spell.fleet_step", "arm.spell.lightning_pace", "arm.spell.leadfoot" })
		{
			var spell = db.MagicSpells.Single(x => x.Id == installed.Identities[key]);
			var root = XElement.Parse(spell.Definition);
			var effects = root.Element("Effects")!.Elements().ToArray();
			Assert.AreEqual(2, effects.Length, key);
			Assert.AreEqual("movementtempo", (string?)effects[0].Attribute("type"), key);
			if (key == "arm.spell.fleet_step")
				Assert.AreEqual("staminaexpendrate", (string?)effects[1].Attribute("type"), key);
			else
			{
				Assert.AreEqual("boost", (string?)effects[1].Attribute("type"), key);
				Assert.AreEqual(agility, (long)effects[1].Attribute("trait")!, key);
			}
		}
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}

	[TestMethod]
	public void FixedWaterBreathing_RefusesEarlierModuleOwnershipWithoutAdoption()
	{
		using var db = Fixture();
		db.SeederManagedRecords.Add(new()
		{
			Seeder = ArmageddonMagicInstaller.Package,
			Module = ArmageddonWaterSeeInstaller.Module,
			StableKey = ArmageddonWaterSeeInstaller.WaterBreathingKey,
			EntityType = nameof(MudSharp.Models.MagicSpell),
			LogicalId = 987654,
			ManifestVersion = ArmageddonWaterSeeInstaller.Version,
			SeedBaseline = "{}"
		});
		db.SaveChanges();
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status);
		Assert.IsFalse(result.Identities.ContainsKey("arm.spell.water_breathing"));
		Assert.AreEqual(ArmageddonWaterSeeInstaller.Module,
			db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonWaterSeeInstaller.WaterBreathingKey).Module);
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
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void ReserveAndDiseaseBindings_UseExactNativeModesAndCharacterResistance(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db)).Status);
		var spells = db.SeederManagedRecords.Where(x => x.Module == ArmageddonStockSpellInstaller.Module && x.EntityType == "MagicSpell")
			.ToDictionary(x => x.StableKey, x => db.MagicSpells.Single(s => s.Id == x.LogicalId));
		XElement Effect(string suffix) => XElement.Parse(spells["arm.spell." + suffix].Definition).Element("Effects")!.Elements().Single();
		Assert.IsTrue((bool)Effect("second_breath").Element("DirectStaminaAdjustment")!);
		Assert.IsTrue((bool)Effect("steal_breath").Element("DirectStaminaAdjustment")!);
		Assert.IsTrue((bool)Effect("empty_aura").Element("ExactResourceDepletion")!);
		Assert.AreEqual("Armageddon Lightning", db.MagicSchools.Single(x => x.Id == spells["arm.spell.steal_breath"].MagicSchoolId).Name);
		var resistanceKey = mode == ArmageddonProgressionMode.Classic ? ArmageddonStockSpellContent.ReadAttunementKey + ".skill" : "arm.skill.water_casting";
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == resistanceKey).LogicalId, spells["arm.spell.sow_sickness"].ResistingTraitDefinitionId);
		Assert.AreEqual(5L, (long)Effect("sow_sickness").Element("InfectionType")!);
		Assert.AreEqual("arm.effect.disease", Effect("sow_sickness").Element("OwnedFamily")!.Value);
		Assert.AreEqual("arm.effect.disease", Effect("purge_disease").Element("OwnedFamily")!.Value);
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

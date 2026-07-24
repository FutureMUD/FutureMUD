#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RangedWeaponType = MudSharp.Combat.RangedWeaponType;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EraItemDependencyCompletionTests
{
	private static readonly string[] HouseholdAndToolComponents =
	[
		"Container_PreIndustrial_CompartmentBox",
		"Container_PreIndustrial_LiddedBasket",
		"Container_PreIndustrial_LiddedHamper",
		"Container_PreIndustrial_Display_Plinth",
		"LContainer_PreIndustrial_Cup_150ml",
		"LContainer_PreIndustrial_Bowl_750ml",
		"LContainer_PreIndustrial_Basin_5L",
		"LContainer_PreIndustrial_Ewer_2L",
		"LContainer_PreIndustrial_Pitcher_4L",
		"LContainer_PreIndustrial_Pot_12L",
		"LContainer_PreIndustrial_StorageJar_12L",
		"LContainer_PreIndustrial_StorageJar_30L",
		"LContainer_PreIndustrial_Vat_125L",
		"LContainer_PreIndustrial_Vat_500L",
		"LockingContainer_PreIndustrial_SmallCabinet",
		"LockingContainer_PreIndustrial_LargeCabinet",
		"LockingContainer_PreIndustrial_DrawerChest",
		"LockingContainer_PreIndustrial_DisplayCabinet",
		"LockingContainer_PreIndustrial_Desk",
		"CashRegister_PreIndustrial_TillChest",
		"Container_CartridgeBandolier",
		"Tool_Artillery_General",
		"Tool_CartridgeMaking_General",
		"Tool_Gunsmithing_General"
	];

	private static readonly string[] TackComponents =
	[
		"RidingGear_Saddle",
		"RidingGear_SaddlePad",
		"RidingGear_Bridle",
		"RidingGear_Reins",
		"RidingGear_Bit",
		"RidingGear_Stirrups",
		"RidingGear_PackSaddle",
		"RidingGear_BitlessBridle",
		"RidingGear_RidingHarness",
		"HitchGear_LeadRope",
		"HitchGear_Yoke",
		"HitchGear_Harness",
		"HitchGear_Chain",
		"HitchGear_Rope",
		"HitchGear_Traces",
		"HitchGear_TowBar"
	];

	private static readonly string[] EraWearComponents =
	[
		"Wear_Stays",
		"Wear_Breeches",
		"Wear_ArmHarness",
		"Wear_LegHarness",
		"Wear_ShoulderArmHarness",
		"Wear_HalfArmourHarness",
		"Wear_ThreeQuarterHarness",
		"Wear_FullPlateHarness"
	];

	private static readonly string[] EraMaterials =
	[
		"gourd shell",
		"papier-mache",
		"birch bark",
		"hemp cloth",
		"brocade",
		"damask",
		"silk gauze",
		"featherwork",
		"beadwork"
	];

	private static readonly string[] SupportedMilitaryWearAndTools =
	[
		"Wear_ArmHarness",
		"Wear_FullPlateHarness",
		"Wear_HalfArmourHarness",
		"Wear_LegHarness",
		"Wear_ShoulderArmHarness",
		"Wear_ThreeQuarterHarness",
		"Tool_Artillery_General",
		"Tool_CartridgeMaking_General",
		"Tool_Gunsmithing_General",
		"Container_CartridgeBandolier"
	];

	private static readonly string[] StandardsSignalsAndInstruments =
	[
		"Instrument_Antiquity_WoodenLyre",
		"Instrument_Antiquity_Kithara",
		"Instrument_Antiquity_ReedFlute",
		"Instrument_Antiquity_DoubleAulos",
		"Instrument_Antiquity_FrameDrum",
		"Instrument_Antiquity_Sistrum",
		"Instrument_Antiquity_BronzeWarHorn",
		"Instrument_Antiquity_ShipSignalTrumpet",
		"Instrument_Antiquity_TempleRitualRattle",
		"SignalInstrument_FieldDrum",
		"SignalInstrument_KettleDrum",
		"SignalInstrument_Fife",
		"SignalInstrument_SpeakingTrumpet",
		"MilitaryStandard_InfantryColour",
		"MilitaryStandard_CavalryStandard",
		"MilitaryStandard_Guidon",
		"MilitaryStandard_NavalEnsign",
		"MilitaryStandard_Pennant",
		"MilitaryStandard_SignalFlag"
	];

	private static readonly string[] SupportedMilitaryStandardsAndSignals =
	[
		"SignalInstrument_FieldDrum",
		"SignalInstrument_KettleDrum",
		"SignalInstrument_Fife",
		"SignalInstrument_SpeakingTrumpet",
		"MilitaryStandard_InfantryColour",
		"MilitaryStandard_CavalryStandard",
		"MilitaryStandard_Guidon",
		"MilitaryStandard_NavalEnsign",
		"MilitaryStandard_Pennant",
		"MilitaryStandard_SignalFlag"
	];

	private static readonly string[] AntiquityDeferredItems =
	[
		"antiquity_senet_game_board",
		"antiquity_mehen_game_board",
		"antiquity_latrunculi_board",
		"antiquity_royal_game_board",
		"antiquity_mancala_board",
		"antiquity_temple_divination_board",
		"antiquity_tavern_game_set",
		"antiquity_wooden_measuring_rod",
		"antiquity_temple_libation_table",
		"antiquity_oil_lamp_shrine",
		"antiquity_oracular_tripod",
		"antiquity_blood_offering_bowl"
	];

	[TestMethod]
	public void MaintainedCatalogues_ContainExactSupportedSetsWithoutDuplicates()
	{
		using JsonDocument componentDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		var components = componentDocument.RootElement
			.EnumerateArray()
			.Select(x => (
				Name: x.GetProperty("Component Name").GetString()!,
				Type: x.GetProperty("Component Type").GetString()!))
			.ToArray();
		Assert.AreEqual(components.Length,
			components.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Seeded_Item_Components.json must not contain duplicate prototype names.");

		string[] supportedComponents = HouseholdAndToolComponents
			.Concat(EraWearComponents)
			.Concat(CombatSeeder.EraDependencyCombatComponentNamesForTesting)
			.Concat(TackComponents)
			.Concat(StandardsSignalsAndInstruments)
			.ToArray();
		Assert.AreEqual(116, supportedComponents.Length);
		Assert.AreEqual(116, supportedComponents.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (string name in supportedComponents)
		{
			Assert.AreEqual(1, components.Count(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)),
				$"Seeded_Item_Components.json should contain exactly one {name} entry.");
		}

		Dictionary<string, string> expectedTypes = new(StringComparer.OrdinalIgnoreCase)
		{
			["Container_PreIndustrial_Display_Plinth"] = "Container",
			["LockingContainer_PreIndustrial_SmallCabinet"] = "LockingContainer",
			["LContainer_PreIndustrial_Cup_150ml"] = "Liquid Container",
			["Wear_Stays"] = "Wearable",
			["Armour_Brigandine"] = "Armour",
			["Melee_Lance"] = "MeleeWeapon",
			["CompositeBow_Light"] = "Bow",
			["Crossbow_Pellet"] = "Crossbow",
			["Crossbow_Cranequin"] = "Crossbow",
			["Bayonet_Plug"] = "BayonetAttachment",
			["MusketPaperCartridge_0.45 Bore"] = "MusketCartridge",
			["CashRegister_PreIndustrial_TillChest"] = "LockingCashRegister",
			["Blowgun_Long"] = "Blowgun",
			["Throwing_Disc"] = "ThrownWeapon",
			["Tool_Gunsmithing_General"] = "HandTool",
			["Instrument_Antiquity_WoodenLyre"] = "Instrument",
			["SignalInstrument_FieldDrum"] = "SignalInstrument",
			["MilitaryStandard_InfantryColour"] = "MilitaryStandard"
		};
		foreach ((string name, string type) in expectedTypes)
		{
			Assert.AreEqual(type, components.Single(x => x.Name == name).Type, name);
		}

		using JsonDocument materialDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Materials.json"));
		string[] materials = materialDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Material Name").GetString()!)
			.ToArray();
		Assert.AreEqual(materials.Length, materials.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Seeded_Materials.json must not contain duplicate material names.");
		foreach (string material in EraMaterials)
		{
			Assert.AreEqual(1, materials.Count(x => x.Equals(material, StringComparison.OrdinalIgnoreCase)),
				$"Seeded_Materials.json should contain exactly one {material} entry.");
		}

		Assert.IsTrue(materials.Contains("mother-of-pearl", StringComparer.OrdinalIgnoreCase));
		Assert.IsFalse(materials.Contains("mother of pearl", StringComparer.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void MilitaryRequests_PartitionExactlyIntoSixtySixSupportedAndNinetyDeferred()
	{
		string militaryLedger = ReadSource("Design Documents", "Seeding",
			"FutureMUD_EarlyModern_Military_Firearms_Uniforms_Naval_Dependency_Ledger.md");
		HashSet<string> militaryRequests = ParsePrototypeTableNames(militaryLedger);
		Assert.AreEqual(156, militaryRequests.Count,
			"The source military ledger should retain all 156 exact request names.");

		HashSet<string> supportedMilitary = CombatSeeder.EraDependencyCombatComponentNamesForTesting
			.Where(x => !new[] { "Armour_RigidMetal", "Armour_CoatOfPlates", "Armour_Splinted" }
				.Contains(x, StringComparer.OrdinalIgnoreCase))
			.Concat(SupportedMilitaryWearAndTools)
			.Concat(SupportedMilitaryStandardsAndSignals)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.AreEqual(66, supportedMilitary.Count);
		Assert.IsTrue(supportedMilitary.IsSubsetOf(militaryRequests));

		HashSet<string> deferredMilitary = militaryRequests
			.Except(supportedMilitary, StringComparer.OrdinalIgnoreCase)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.AreEqual(90, deferredMilitary.Count);

		string consolidatedLedger = ReadSource("Design Documents", "Seeding",
			"FutureMUD_Item_Content_Engine_Dependency_Ledger.md");
		HashSet<string> consolidatedDeferred = ParsePrototypeTableNames(consolidatedLedger);
		Assert.AreEqual(90, consolidatedDeferred.Count);
		Assert.IsTrue(deferredMilitary.IsSubsetOf(consolidatedDeferred));
		Assert.IsTrue(consolidatedDeferred.SetEquals(deferredMilitary));
	}

	[TestMethod]
	public void DeferredProfiles_AreAbsentAndAntiquityReferencesAreComplete()
	{
		string consolidatedLedger = ReadSource("Design Documents", "Seeding",
			"FutureMUD_Item_Content_Engine_Dependency_Ledger.md");
		HashSet<string> deferred = ParsePrototypeTableNames(consolidatedLedger);
		Assert.AreEqual(90, deferred.Count);

		using JsonDocument componentDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		HashSet<string> seeded = componentDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Component Name").GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.AreEqual(0, deferred.Count(seeded.Contains),
			"Engine-dependent prototype names must remain absent from the maintained seeded catalogue.");

		Assert.AreEqual(12, AntiquityDeferredItems.Length);
		Assert.AreEqual(12, AntiquityDeferredItems.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (string item in AntiquityDeferredItems)
		{
			Assert.AreEqual(1, Regex.Matches(consolidatedLedger, $"`{Regex.Escape(item)}`").Count,
				$"{item} should appear exactly once in the consolidated Antiquity backlog.");
		}

		StringAssert.Contains(consolidatedLedger,
			"`SignalInstrument` is a military specialization of the general `Instrument` capability");
	}

	[TestMethod]
	public void CombatDependencyUpsert_RepairsPartialFixturesAndKeepsAssociationsStableOnRerun()
	{
		using FuturemudDatabaseContext context = BuildCombatContext();
		CombatSeeder seeder = new();
		Dictionary<string, string> answers = new(StringComparer.OrdinalIgnoreCase)
		{
			["installweapons"] = "yes",
			["installranged"] = "yes"
		};

		seeder.EnsureEraDependencyCombatContentForTesting(context, answers);
		seeder.EnsureEraDependencyCombatContentForTesting(context, answers);

		foreach (string name in CombatSeeder.EraDependencyCombatComponentNamesForTesting
			         .Where(x => !x.StartsWith("MusketPaperCartridge_", StringComparison.OrdinalIgnoreCase)))
		{
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), name);
		}

		ArmourType brigandine = context.ArmourTypes.Single(x => x.Name == "Brigandine");
		Assert.AreEqual(1, brigandine.MinimumPenetrationDegree);
		Assert.AreEqual(1.0, brigandine.BaseDifficultyDegrees);
		Assert.AreEqual("<Definition donor=\"metal-scale\" />", brigandine.Definition);

		WeaponType lance = context.WeaponTypes.Single(x => x.Name == "Lance");
		Assert.AreEqual(5, lance.Reach);
		Assert.IsTrue(context.WeaponAttacks.Any(x =>
			x.WeaponTypeId == lance.Id && x.Name == "Lance: Donor Attack"));
		Assert.AreEqual(1, context.WeaponAttacks.Count(x =>
			x.WeaponTypeId == lance.Id && x.Name == "Lance: Donor Attack"));

		RangedWeaponTypes pellet = context.RangedWeaponTypes.Single(x => x.Name == "Pellet Crossbow");
		Assert.AreEqual("Sling Bullet", pellet.SpecificAmmunitionGrade);
		Assert.AreEqual((int)RangedWeaponType.Crossbow, pellet.RangedWeaponType);
		Assert.IsTrue(context.AmmunitionTypes.Single(x => x.Name == "Sling Bullet")
			.RangedWeaponTypes
			.Split(' ')
			.Contains(((int)RangedWeaponType.Crossbow).ToString()));

		GameItemComponentProto lanceComponent = context.GameItemComponentProtos.Single(x => x.Name == "Melee_Lance");
		StringAssert.Contains(lanceComponent.Definition, $"<WeaponType>{lance.Id}</WeaponType>");
		GameItemComponentProto pelletComponent = context.GameItemComponentProtos.Single(x => x.Name == "Crossbow_Pellet");
		StringAssert.Contains(pelletComponent.Definition,
			$"<RangedWeaponType>{pellet.Id}</RangedWeaponType>");
	}

	[TestMethod]
	public void StandardsSignalsAndInstruments_ExposeExactNineteenProfilesAndThirtyNineStockReferences()
	{
		Assert.AreEqual(19, StandardsSignalsAndInstruments.Length);
		Assert.AreEqual(19,
			StandardsSignalsAndInstruments.Distinct(StringComparer.OrdinalIgnoreCase).Count());

		var antiquityInstrumentReferences = ItemSeeder.AntiquityComponentGapItemStableReferencesForTesting
			.Where(x => new[]
			{
				"antiquity_wooden_lyre", "antiquity_kithara", "antiquity_reed_flute",
				"antiquity_double_aulos", "antiquity_frame_drum", "antiquity_sistrum",
				"antiquity_bronze_war_horn", "antiquity_ship_signal_trumpet",
				"antiquity_temple_ritual_rattle"
			}.Contains(x, StringComparer.OrdinalIgnoreCase))
			.ToArray();
		Assert.AreEqual(9, antiquityInstrumentReferences.Length);
		Assert.AreEqual(30, ItemSeeder.EarlyModernStandardsAndSignalsStableReferencesForTesting.Count);
		Assert.AreEqual(30,
			ItemSeeder.EarlyModernStandardsAndSignalsStableReferencesForTesting
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Count());

		using JsonDocument componentDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Item_Components.json"));
		var catalogueNames = componentDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Component Name").GetString()!)
			.ToArray();
		foreach (var name in StandardsSignalsAndInstruments)
		{
			Assert.AreEqual(1, catalogueNames.Count(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)), name);
		}
	}

	[TestMethod]
	public void MusketPaperCartridgeUpsert_UsesMatchingChargeAndIsIdempotent()
	{
		using FuturemudDatabaseContext context = BuildCombatContext();
		context.Tags.Add(new Tag { Id = 6, Name = "Paper Cartridges" });
		var bores = new[] { "0.45 Bore", "0.55 Bore", "0.6 Bore", "0.65 Bore", "0.7 Bore", "0.75 Bore", "0.8 Bore" };
		for (var i = 0; i < bores.Length; i++)
		{
			var bore = double.Parse(bores[i].Split(' ')[0]);
			context.GameItemComponentProtos.Add(new GameItemComponentProto
			{
				Id = 100 + i,
				RevisionNumber = 0,
				Type = "MusketCartridge",
				Name = $"MusketCartridge_{bores[i]}",
				Description = "legacy cartridge",
				Definition =
					$"<Definition><AmmoType>1</AmmoType><BulletBore>{bore}</BulletBore><BulletProto>{1000 + i}</BulletProto></Definition>",
				EditableItem = CurrentEditableItem(100 + i)
			});
			context.GameItemComponentProtos.Add(new GameItemComponentProto
			{
				Id = 200 + i,
				RevisionNumber = 0,
				Type = "Musket",
				Name = $"TestMusket_{bores[i]}",
				Description = "test musket",
				Definition =
					$"<Definition><BulletBore>{bore}</BulletBore><PowderVolumePerShot>{7.0 + i / 10.0}</PowderVolumePerShot></Definition>",
				EditableItem = CurrentEditableItem(200 + i)
			});
		}

		context.SaveChanges();
		CombatSeeder seeder = new();
		Dictionary<string, string> answers = new(StringComparer.OrdinalIgnoreCase)
		{
			["installweapons"] = "no",
			["installranged"] = "no",
			["installmuskets"] = "yes"
		};
		seeder.EnsureEraDependencyCombatContentForTesting(context, answers);
		seeder.EnsureEraDependencyCombatContentForTesting(context, answers);

		for (var i = 0; i < bores.Length; i++)
		{
			var name = $"MusketPaperCartridge_{bores[i]}";
			Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == name), name);
			var definition = XElement.Parse(context.GameItemComponentProtos.Single(x => x.Name == name).Definition);
			Assert.AreEqual(7.0 + i / 10.0, (double)definition.Element("PowderMass")!, 0.000001, name);
			Assert.IsTrue((bool)definition.Element("IncludesWad")!, name);
			Assert.AreEqual(1000 + i, (long)definition.Element("BulletProto")!, name);
			Assert.AreEqual(1, (long)definition.Element("AmmoType")!, name);
		}
	}

	private static EditableItem CurrentEditableItem(long id)
	{
		return new EditableItem
		{
			Id = id,
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = 1,
			BuilderDate = DateTime.UtcNow,
			BuilderComment = "test",
			ReviewerAccountId = 1,
			ReviewerDate = DateTime.UtcNow,
			ReviewerComment = "test"
		};
	}

	private static FuturemudDatabaseContext BuildCombatContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		FuturemudDatabaseContext context = new(options);
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "SeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			UseMxp = false,
			UseMsp = false,
			UseMccp = false,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			PromptType = 0,
			TabRoomDescriptions = false,
			CodedRoomDescriptionAdditionsOnNewLine = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});

		(string Name, string Marker)[] armourDonors =
		[
			("Metal Scale", "metal-scale"),
			("Boiled Leather", "boiled-leather"),
			("Platemail", "platemail"),
			("Ultra Heavy Clothing", "padded"),
			("Leather Scale", "leather-scale"),
			("Studded Leather", "studded")
		];
		long armourId = 1;
		foreach ((string name, string marker) in armourDonors)
		{
			context.ArmourTypes.Add(new ArmourType
			{
				Id = armourId++,
				Name = name,
				Definition = $"<Definition donor=\"{marker}\" />"
			});
		}

		context.ArmourTypes.Add(new ArmourType
		{
			Id = armourId,
			Name = "Brigandine",
			MinimumPenetrationDegree = 99,
			BaseDifficultyDegrees = 99,
			StackedDifficultyDegrees = 99,
			Definition = "<Definition wrong=\"true\" />"
		});

		string[] weaponDonors =
		[
			"Dagger", "Halberd", "Long Spear", "Longsword", "Rapier", "Training Dagger", "Training Spear",
			"Training Halberd", "Training Longsword", "Training Rapier", "Club"
		];
		long weaponId = 1;
		long attackId = 1;
		foreach (string name in weaponDonors)
		{
			WeaponType weapon = new()
			{
				Id = weaponId++,
				Name = name,
				Classification = 1,
				ParryBonus = 0.5,
				Reach = 2,
				StaminaPerParry = 1.0
			};
			weapon.WeaponAttacks.Add(new WeaponAttack
			{
				Id = attackId++,
				Name = "Donor Attack",
				Verb = 1,
				DamageExpressionId = 1,
				StunExpressionId = 1,
				PainExpressionId = 1,
				Weighting = 1.0,
				MaximumTargets = 1,
				StaminaCost = 1.0,
				BaseDelay = 1.0,
				AdditionalInfo = string.Empty,
				RequiredPositionStateIds = string.Empty
			});
			context.WeaponTypes.Add(weapon);
		}

		string[] rangedDonors =
		[
			"Longbow", "Shortbow", "Crossbow", "Hand Crossbow", "Blowgun", "Throwing Axe", "Throwing Knife"
		];
		long rangedId = 1;
		foreach (string name in rangedDonors)
		{
			context.RangedWeaponTypes.Add(new RangedWeaponTypes
			{
				Id = rangedId++,
				Name = name,
				Classification = 1,
				FireTraitId = 1,
				OperateTraitId = 1,
				DefaultRangeInRooms = 3,
				AccuracyBonusExpression = "0",
				DamageBonusExpression = "1",
				SpecificAmmunitionGrade = name.Contains("bow", StringComparison.OrdinalIgnoreCase)
					? "Arrow"
					: name.Contains("Crossbow", StringComparison.OrdinalIgnoreCase)
						? "Bolt"
						: name == "Blowgun"
							? "Blowgun Dart"
							: string.Empty,
				AmmunitionCapacity = 1,
				RangedWeaponType = name.Contains("Crossbow", StringComparison.OrdinalIgnoreCase)
					? (int)RangedWeaponType.Crossbow
					: name.Contains("bow", StringComparison.OrdinalIgnoreCase)
						? (int)RangedWeaponType.Bow
						: name == "Blowgun"
							? (int)RangedWeaponType.Blowgun
							: (int)RangedWeaponType.Thrown,
				StaminaToFire = 1.0,
				StaminaPerLoadStage = 1.0,
				LoadDelay = 1.0,
				ReadyDelay = 1.0,
				FireDelay = 1.0,
				AimBonusLostPerShot = 1.0
			});
		}

		context.AmmunitionTypes.Add(new AmmunitionTypes
		{
			Id = 1,
			Name = "Sling Bullet",
			SpecificType = "Sling Bullet",
			RangedWeaponTypes = ((int)RangedWeaponType.Sling).ToString(),
			DamageExpression = "1",
			StunExpression = "1",
			PainExpression = "1"
		});
		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Cranequin" },
			new Tag { Id = 2, Name = "Goat's Foot" },
			new Tag { Id = 3, Name = "Lever" },
			new Tag { Id = 4, Name = "Spanning Hook" },
			new Tag { Id = 5, Name = "Windlass" });
		context.SaveChanges();
		return context;
	}

	private static HashSet<string> ParsePrototypeTableNames(string markdown)
	{
		return Regex.Matches(markdown, @"(?m)^\| `(?<name>[^`]+)` \| \d+ \|")
			.Cast<Match>()
			.Select(x => x.Groups["name"].Value)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}

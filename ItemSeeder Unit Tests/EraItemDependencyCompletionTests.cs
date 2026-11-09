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
		var dagger = context.WeaponTypes.Single(x => x.Name == "Dagger");
		var backstab = new WeaponAttack
		{
			Name = "Dagger Backstab", WeaponType = dagger,
			MoveType = (int)MudSharp.Combat.BuiltInCombatMoveType.BackstabAttack,
			RequiredPositionStateIds = "1", AdditionalInfo = "<Backstab />",
			DamageExpressionId = 1, PainExpressionId = 1, StunExpressionId = 1
		};
		context.WeaponAttacks.Add(backstab);
		context.SaveChanges();

		seeder.EnsureEraDependencyCombatContentForTesting(context, answers);
		seeder.EnsureEraDependencyCombatContentForTesting(context, answers);
		Assert.AreEqual(1, context.WeaponAttacks.Count(x => x.MoveType == (int)MudSharp.Combat.BuiltInCombatMoveType.BackstabAttack),
			"Era weapon clones must not spread the dagger's backstab to bayonets.");
		Assert.AreEqual(dagger.Id, context.WeaponAttacks.Single(x => x.MoveType ==
			(int)MudSharp.Combat.BuiltInCombatMoveType.BackstabAttack).WeaponTypeId);

		ArmourType brigandine = context.ArmourTypes.Single(x => x.Name == "Brigandine");
		Assert.AreEqual("<Definition donor=\"metal-scale\" />", brigandine.Definition);

		WeaponType lance = context.WeaponTypes.Single(x => x.Name == "Lance");
		Assert.IsTrue(context.WeaponAttacks.Any(x =>
			x.WeaponTypeId == lance.Id && x.Name == "Lance: Donor Attack"));
		Assert.AreEqual(1, context.WeaponAttacks.Count(x =>
			x.WeaponTypeId == lance.Id && x.Name == "Lance: Donor Attack"));
		Assert.AreEqual(1, context.WeaponAttacks.Count(x =>
			x.WeaponTypeId == lance.Id && x.Name == "Lance: Couched Charge" &&
			x.MoveType == (int)MudSharp.Combat.BuiltInCombatMoveType.CouchedLanceAttack));
		Assert.IsFalse(context.WeaponAttacks.Any(x =>
			x.WeaponTypeId == lance.Id && x.Name == "Lance: Donor Mounted Fallback"));
		WeaponType sabre = context.WeaponTypes.Single(x => x.Name == "Sabre");
		Assert.AreEqual(1, context.WeaponAttacks.Count(x =>
			x.WeaponTypeId == sabre.Id && x.Name == "Sabre: Mounted Sabre Cut" &&
			x.MoveType == (int)MudSharp.Combat.BuiltInCombatMoveType.MountedWeaponAttack));
		Assert.IsFalse(context.WeaponAttacks.Any(x =>
			x.WeaponTypeId == sabre.Id && x.Name == "Sabre: Donor Mounted Fallback"));
		WeaponType trainingLance = context.WeaponTypes.Single(x => x.Name == "Training Lance");
		Assert.AreEqual(1, context.WeaponAttacks.Count(x =>
			x.WeaponTypeId == trainingLance.Id && x.Name == "Training Lance: Couched Charge" &&
			x.MoveType == (int)MudSharp.Combat.BuiltInCombatMoveType.CouchedLanceAttack));
		WeaponType trainingSabre = context.WeaponTypes.Single(x => x.Name == "Training Sabre");
		Assert.AreEqual(1, context.WeaponAttacks.Count(x =>
			x.WeaponTypeId == trainingSabre.Id && x.Name == "Training Sabre: Mounted Sabre Cut" &&
			x.MoveType == (int)MudSharp.Combat.BuiltInCombatMoveType.MountedWeaponAttack));
		WeaponType hookedPolearm = context.WeaponTypes.Single(x => x.Name == "Hooked Polearm");
		Assert.AreEqual(1, context.WeaponAttacks.Count(x =>
			x.WeaponTypeId == hookedPolearm.Id && x.Name == "Hooked Polearm: Hook and Pull" &&
			x.MoveType == (int)MudSharp.Combat.BuiltInCombatMoveType.PullToMelee));

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
	public void MusketPaperCartridgeUpsert_UsesMatchingBarrelBoreAndIgnoresIncompleteMusketDefinitions()
	{
		using FuturemudDatabaseContext context = BuildCombatContext();
		context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Id = 50,
			RevisionNumber = 0,
			Type = "Musket",
			Name = "Incomplete Legacy Musket",
			Description = "A legacy musket component without a barrel-bore value.",
			Definition = "<Definition><PowderVolumePerShot>1</PowderVolumePerShot></Definition>",
			EditableItem = CurrentEditableItem(50)
		});
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
					$"<Definition><BarrelBore>{bore}</BarrelBore><PowderVolumePerShot>{7.0 + i / 10.0}</PowderVolumePerShot></Definition>",
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
			if (name is "Long Spear" or "Training Spear" or "Longsword" or "Training Longsword")
			{
				weapon.WeaponAttacks.Add(new WeaponAttack
				{
					Id = attackId++,
					Name = "Donor Mounted Fallback",
					MoveType = name.Contains("Spear", StringComparison.Ordinal)
						? (int)MudSharp.Combat.BuiltInCombatMoveType.CouchedLanceAttack
						: (int)MudSharp.Combat.BuiltInCombatMoveType.MountedWeaponAttack,
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
			}
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
		context.SaveChanges();
		new CoreDataSeeder().SeedTagsForTesting(context);
		return context;
	}
}

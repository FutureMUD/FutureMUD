#nullable enable

using DatabaseSeeder;
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
using System.Reflection;
using MaterialType = MudSharp.Form.Material.MaterialType;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RenaissanceMilitarySeederTests
{

	[TestMethod]
	public void DependencyValidation_ReportsExactEarlyGunProfileAndRerunGuidance()
	{
		var missing = ItemSeeder.ValidateRenaissanceMilitaryDependenciesForTesting(
			ItemSeeder.RenaissanceMilitaryMaterialsForTesting,
			ItemSeeder.RenaissanceMilitaryTagsForTesting,
			ItemSeeder.RenaissanceMilitaryComponentsForTesting
				.Where(x => !x.Equals("Musket_Matchlock_Arquebus55", StringComparison.OrdinalIgnoreCase)));

		CollectionAssert.Contains(missing.ToArray(), "Missing seeded component: Musket_Matchlock_Arquebus55");
		StringAssert.Contains(ItemSeeder.RenaissanceMilitaryPrerequisiteRerunGuidanceForTesting, "early-gun packages");
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void MilitarySeed_RerunPreservesInstalledItemAndOutfitIds()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);

		var seeder = new ItemSeeder();
		seeder.SeedRenaissanceMilitaryForTesting(context);
		context.SaveChanges();
		var items = context.GameItemProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		var outfits = context.OutfitTemplates.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(items.Length > 0);
		Assert.IsTrue(outfits.Length > 0);
		seeder.SeedRenaissanceMilitaryForTesting(context);
		context.SaveChanges();
		CollectionAssert.AreEqual(items, context.GameItemProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(outfits, context.OutfitTemplates.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void MissingPrerequisite_ThrowsBeforeTheMilitaryStageWritesAnyPrototype()
	{
		using var context = BuildContext();
		SeedPrerequisites(context, "Musket_Matchlock_Arquebus55");

		var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
			new ItemSeeder().SeedRenaissanceMilitaryForTesting(context));

		StringAssert.Contains(exception.Message, "Missing seeded component: Musket_Matchlock_Arquebus55");
		StringAssert.Contains(exception.Message, "no Renaissance item stage has been written");
		Assert.AreEqual(0, context.GameItemProtos.Count());
	}

	[TestMethod]
	public void ItemSeederMetadata_RequiresAnimalMountedWearComponents()
	{
		IDatabaseSeeder seeder = new ItemSeeder();
		var metadata = seeder.Metadata;
		Assert.AreEqual(SeederRepeatabilityMode.Idempotent, metadata.RepeatabilityMode);
		Assert.AreEqual(SeederUpdateCapability.FullReconcile, metadata.UpdateCapability);
		Assert.IsTrue(seeder.SafeToRunMoreThanOnce);
		Assert.IsFalse((metadata.DependencySeederTypes ?? []).Any());
		using var context = BuildContext();
		foreach (var (id, name, provider) in new[] { (1L, "Wear_Saddle", "Animal Seeder"), (2L, "Container_Table", "Kickstart") })
		{
			var requirement = metadata.Prerequisites.Single(x => x.Description.Contains(name, StringComparison.Ordinal));
			StringAssert.Contains(requirement.Description, provider);
			Assert.IsFalse(requirement.IsSatisfied(context));
			context.GameItemComponentProtos.Add(new GameItemComponentProto
			{
				Id = id, Name = name, Description = "Builder supplied", Type = "test", Definition = "<Definition/>"
			});
			context.SaveChanges();
			Assert.IsTrue(requirement.IsSatisfied(context));
		}
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context, string? omittedComponent = null)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "RenaissanceSeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
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
			HasBeenActiveInWeek = true,
			HintsEnabled = true
		});

		var materialId = 1L;
		foreach (var name in ItemSeeder.RenaissanceMilitaryMaterialsForTesting)
		{
			context.Materials.Add(new Material
			{
				Id = materialId++,
				Name = name,
				MaterialDescription = name,
				Density = 1.0,
				Organic = true,
				Type = 0,
				BehaviourType = 0,
				ResidueSdesc = string.Empty,
				ResidueDesc = string.Empty,
				ResidueColour = "grey"
			});
		}

		var tagId = 1L;
		foreach (var path in ItemSeeder.RenaissanceMilitaryTagsForTesting)
		{
			context.Tags.Add(new Tag { Id = tagId++, Name = path });
		}

		var componentId = 1L;
		foreach (var name in ItemSeeder.RenaissanceMilitaryComponentsForTesting
			         .Where(x => !x.Equals(omittedComponent, StringComparison.OrdinalIgnoreCase)))
		{
			context.GameItemComponentProtos.Add(new GameItemComponentProto
			{
				Id = componentId++,
				Name = name,
				Type = "test",
				Definition = "<Definition />",
				Description = name,
				EditableItemId = componentId,
				RevisionNumber = 0
			});
		}

		context.SaveChanges();
	}
}

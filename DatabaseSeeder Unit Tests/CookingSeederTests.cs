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
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CookingSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context)
	{
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
			CharacterNameOverlaySetting = 0,
			AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});

		context.GameItemComponentProtos.Add(Component(1, "Holdable"));
		context.GameItemComponentProtos.Add(Component(2, "Stack_Number", "Stackable"));
		context.Materials.Add(Material(1, "apple"));
		context.Materials.Add(Material(2, "blueberry"));
		context.Materials.Add(Material(3, "mushroom"));
		context.Materials.Add(Material(4, "muffin"));
		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 1,
			Name = "Cooking",
			Type = 0,
			OwnerScope = 0,
			TraitGroup = "Crafting",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		});
		context.SaveChanges();
	}

	private static MudSharp.Models.GameItemComponentProto Component(long id, string name, string type = "Test")
	{
		return new MudSharp.Models.GameItemComponentProto
		{
			Id = id,
			Name = name,
			Type = type,
			Description = $"{name} marker",
			Definition = "<Definition />",
			RevisionNumber = 0,
			EditableItem = Editable()
		};
	}

	private static Material Material(long id, string name)
	{
		return new Material
		{
			Id = id,
			Name = name,
			MaterialDescription = name,
			Type = 0,
			BehaviourType = 0,
			Density = 1.0,
			Organic = true,
			ResidueSdesc = string.Empty,
			ResidueDesc = string.Empty,
			ResidueColour = "green"
		};
	}

	private static EditableItem Editable()
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = 1,
			BuilderDate = DateTime.UtcNow,
			BuilderComment = "test",
			ReviewerAccountId = 1,
			ReviewerComment = "test",
			ReviewerDate = DateTime.UtcNow
		};
	}

	[TestMethod]
	public void SeedData_InstallsAndRerunsWithoutDuplicatingFoodOrCrafts()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new CookingSeeder();
		var prior0 = context.GameItemComponentProtos.Select(x => x.Id).ToArray();
		var prior1 = context.GameItemProtos.Select(x => x.Id).ToArray();
		var prior2 = context.Crafts.Select(x => x.Id).ToArray();
		seeder.SeedData(context, new Dictionary<string, string>());
		var installed0 = context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		var installed1 = context.GameItemProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed1.Except(prior1).Any(), "The first installation must create records beyond its prerequisites.");
		var installed2 = context.Crafts.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed2.Except(prior2).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedData(context, new Dictionary<string, string>());
		CollectionAssert.AreEqual(installed0, context.GameItemComponentProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed1, context.GameItemProtos.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed2, context.Crafts.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void CookingSeeder_BlocksWhenUsefulItemComponentsAreMissing()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		context.GameItemComponentProtos.RemoveRange(context.GameItemComponentProtos);
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, new CookingSeeder().ShouldSeedData(context));
	}

	[TestMethod]
	public void CookingSeeder_AllowsDuplicateTagNamesInDifferentHierarchies()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		var animalProduct = new Tag { Id = 1, Name = "Animal Product" };
		var agriculture = new Tag { Id = 2, Name = "Agriculture" };
		context.Tags.AddRange(
			animalProduct,
			agriculture,
			new Tag { Id = 3, Name = "Apiary Product", Parent = animalProduct, ParentId = animalProduct.Id },
			new Tag { Id = 4, Name = "Apiary Product", Parent = agriculture, ParentId = agriculture.Id },
			new Tag { Id = 5, Name = "Food", Parent = animalProduct, ParentId = animalProduct.Id });
		context.SaveChanges();

		new CookingSeeder().SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(2, context.Tags.Count(x => x.Name == "Apiary Product"));
		Assert.AreEqual(1, context.Tags.Count(x => x.Name == "Food" && x.ParentId == null));
		Assert.AreEqual(1, context.Tags.Count(x => x.Name == "Prepared Food" && x.Parent!.Name == "Food"));
	}
}

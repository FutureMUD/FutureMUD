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
	public void CookingSeeder_InstallsPreparedFoodDirectItemsAndRecipeWithoutLegacyFood()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		CookingSeeder seeder = new();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));

		seeder.SeedData(context, new Dictionary<string, string>());
		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "PreparedFood_Apple"));
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "PreparedFood_Berry_Stack"));
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "PreparedFood_Baked_Apple"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Type == "Food"));

		var berryDefinition = XElement.Parse(context.GameItemComponentProtos.Single(x => x.Name == "PreparedFood_Berry_Stack").Definition);
		Assert.AreEqual("PerStackUnit", berryDefinition.Attribute("ServingScope")!.Value);
		Assert.IsTrue(berryDefinition.Element("Full")!.Value.Contains("They are made of"));
		var appleDefinition = XElement.Parse(context.GameItemComponentProtos.Single(x => x.Name == "PreparedFood_Apple").Definition);
		Assert.IsTrue(appleDefinition.Element("Full")!.Value.Contains("It is made of"));
		var mushroomDefinition = XElement.Parse(context.GameItemComponentProtos.Single(x => x.Name == "PreparedFood_Mushroom_Stack").Definition);
		Assert.IsTrue(mushroomDefinition.Element("Full")!.Value.Contains("They are made of"));

		var apple = context.GameItemProtos.Single(x => x.ShortDescription == "a crisp red apple");
		Assert.IsTrue(apple.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Name == "PreparedFood_Apple"));
		Assert.IsTrue(apple.GameItemProtosTags.Any(x => x.Tag.Name == "Forageable Food"));

		var berries = context.GameItemProtos.Single(x => x.ShortDescription == "a handful of blueberries");
		Assert.IsTrue(berries.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Name == "Stack_Number"));
		Assert.IsTrue(berries.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Name == "PreparedFood_Berry_Stack"));

		var craft = context.Crafts.Single(x => x.Name == "bake apple");
		Assert.AreEqual("Cooking", craft.Category);
		Assert.AreEqual(1, craft.CraftProducts.Count(x => x.ProductType == "CookedFoodProduct"));
		Assert.AreEqual(1, craft.CraftInputs.Count(x => x.InputType == "Tag"));
		var productDefinition = XElement.Parse(craft.CraftProducts.Single(x => x.ProductType == "CookedFoodProduct").Definition);
		Assert.AreEqual("false", productDefinition.Element("RemoveDrugsAndFoodEffects")!.Value);

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
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
}

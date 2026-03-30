#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Health;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HealthSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedAccount(FuturemudDatabaseContext context)
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
		context.SaveChanges();
	}

	[TestMethod]
	public void ValidateDefaultSurgeryTargetAliasesForTesting_CurrentTargets_HasNoIssues()
	{
		var issues = HealthSeeder.ValidateDefaultSurgeryTargetAliasesForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void BuildTargetDefinition_OrganicHumanoid_ResolvesInheritedAndLocalBodyparts()
	{
		using var context = BuildContext();
		var humanoidBody = new BodyProto
		{
			Id = 1,
			Name = "Humanoid"
		};
		var organicHumanoidBody = new BodyProto
		{
			Id = 2,
			Name = "Organic Humanoid",
			CountsAs = humanoidBody,
			CountsAsId = humanoidBody.Id
		};

		context.BodyProtos.AddRange(humanoidBody, organicHumanoidBody);
		context.BodypartProtos.AddRange(
			new BodypartProto
			{
				Id = 10,
				Body = humanoidBody,
				BodyId = humanoidBody.Id,
				Name = "lupperarm",
				Description = "left upper arm"
			},
			new BodypartProto
			{
				Id = 11,
				Body = organicHumanoidBody,
				BodyId = organicHumanoidBody.Id,
				Name = "brain",
				Description = "brain"
			});

		var definition = HealthSeeder.BuildTargetDefinition(context, organicHumanoidBody, "lupperarm", "brain");
		var targetIds = XElement.Parse(definition)
			.Element("Parts")!
			.Elements("Part")
			.Select(x => long.Parse(x.Value))
			.ToArray();

		CollectionAssert.AreEqual(new long[] { 10, 11 }, targetIds);
	}

	[TestMethod]
	public void SeedDrugDeliveryExamplesForTesting_OnlyCreatesItemsForSupportedVectors()
	{
		using var context = BuildContext();
		SeedAccount(context);
		context.Drugs.AddRange(
			new Drug { Id = 1, Name = "IngestedOnly", DrugVectors = (int)DrugVector.Ingested, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 2, Name = "TouchedOnly", DrugVectors = (int)DrugVector.Touched, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 3, Name = "InhaledOnly", DrugVectors = (int)DrugVector.Inhaled, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 4, Name = "BothDrug", DrugVectors = (int)(DrugVector.Ingested | DrugVector.Touched), IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 });
		context.SaveChanges();

		var seeder = new HealthSeeder();
		seeder.SeedDrugDeliveryExamplesForTesting(context);

		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "Pill_IngestedOnly"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "TopicalCream_IngestedOnly"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "Pill_TouchedOnly"));
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "TopicalCream_TouchedOnly"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "Pill_InhaledOnly"));
		Assert.AreEqual(0, context.GameItemComponentProtos.Count(x => x.Name == "TopicalCream_InhaledOnly"));
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "Pill_BothDrug"));
		Assert.AreEqual(1, context.GameItemComponentProtos.Count(x => x.Name == "TopicalCream_BothDrug"));
	}
}

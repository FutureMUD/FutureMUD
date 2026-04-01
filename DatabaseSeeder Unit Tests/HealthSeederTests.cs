#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Health;
using MudSharp.Models;
using Drug = MudSharp.Models.Drug;

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

	private static void SeedShouldSeedPrerequisites(FuturemudDatabaseContext context)
	{
		SeedAccount(context);
		context.Races.Add(new Race
		{
			Id = 1,
			Name = "Organic Humanoid",
			Description = "Organic Humanoid test race",
			BaseBodyId = 0,
			AllowedGenders = "Male Female Neuter NonBinary",
			AttributeBonusProgId = 0,
			DiceExpression = "1d100",
			CorpseModelId = 0,
			DefaultHealthStrategyId = 0,
			BreathingModel = "Simple",
			CommunicationStrategyType = "HumanoidCommunicationStrategy",
			HandednessOptions = "Left,Right",
			MaximumDragWeightExpression = "100",
			MaximumLiftWeightExpression = "100",
			EatCorpseEmoteText = "@ eat|eats $0.",
			BreathingVolumeExpression = "1",
			HoldBreathLengthExpression = "1"
		});
		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Arterial Clamp" },
			new Tag { Id = 2, Name = "Bonesaw" },
			new Tag { Id = 3, Name = "Forceps" },
			new Tag { Id = 4, Name = "Scalpel" },
			new Tag { Id = 5, Name = "Surgical Suture Needle" });
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

	[TestMethod]
	public void ShouldSeedData_ModernTierInstallWithoutOtherTiers_ReturnsMayAlreadyInstalled()
	{
		using var context = BuildContext();
		SeedShouldSeedPrerequisites(context);

		context.Knowledges.AddRange(
			new Knowledge { Id = 1, Name = "Diagnostic Medicine", Description = "Diagnostic Medicine", LongDescription = "Diagnostic Medicine", Type = "Test", Subtype = "Test", LearnableType = 0, LearnDifficulty = 0, TeachDifficulty = 0, LearningSessionsRequired = 0 },
			new Knowledge { Id = 2, Name = "Clinical Medicine", Description = "Clinical Medicine", LongDescription = "Clinical Medicine", Type = "Test", Subtype = "Test", LearnableType = 0, LearnDifficulty = 0, TeachDifficulty = 0, LearningSessionsRequired = 0 },
			new Knowledge { Id = 3, Name = "Surgery", Description = "Surgery", LongDescription = "Surgery", Type = "Test", Subtype = "Test", LearnableType = 0, LearnDifficulty = 0, TeachDifficulty = 0, LearningSessionsRequired = 0 });

		context.SurgicalProcedures.AddRange(
			new SurgicalProcedure { Id = 1, Name = "Triage", ProcedureName = "Triage", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Triage", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 2, Name = "Physical", ProcedureName = "Physical", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Physical", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 3, Name = "Stitch Up", ProcedureName = "Stitch Up", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Stitch Up", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 4, Name = "Exploratory Surgery", ProcedureName = "Exploratory Surgery", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Exploratory Surgery", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 5, Name = "Trauma Control", ProcedureName = "Trauma Control", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Trauma Control", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 6, Name = "Organ Extraction", ProcedureName = "Organ Extraction", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Organ Extraction", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 7, Name = "General Organ Repair", ProcedureName = "General Organ Repair", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "General Organ Repair", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 8, Name = "Bone Setting", ProcedureName = "Bone Setting", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Bone Setting", Definition = string.Empty, TargetBodyTypeId = 0 });

		context.Drugs.AddRange(
			new Drug { Id = 1, Name = "General Anaesthetic", DrugVectors = (int)DrugVector.Inhaled, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 2, Name = "Opioid Analgesic", DrugVectors = (int)DrugVector.Ingested, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 3, Name = "Muscle Relaxant", DrugVectors = (int)DrugVector.Injected, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 4, Name = "Local Anaesthetic", DrugVectors = (int)DrugVector.Touched, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 5, Name = "Broad-Spectrum Antibiotic", DrugVectors = (int)DrugVector.Ingested, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 6, Name = "Antibiotic Ointment", DrugVectors = (int)DrugVector.Touched, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 7, Name = "Antifungal Course", DrugVectors = (int)(DrugVector.Ingested | DrugVector.Touched), IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 8, Name = "Burn Gel", DrugVectors = (int)DrugVector.Touched, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 });

		context.GameItemComponentProtos.AddRange(
			new GameItemComponentProto { Id = 1, Name = "Pill_Opioid_Analgesic", Type = "Pill", Description = "Pill_Opioid_Analgesic", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 2, Name = "TopicalCream_Local_Anaesthetic", Type = "TopicalCream", Description = "TopicalCream_Local_Anaesthetic", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 3, Name = "Pill_Broad_Spectrum_Antibiotic", Type = "Pill", Description = "Pill_Broad_Spectrum_Antibiotic", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 4, Name = "TopicalCream_Antibiotic_Ointment", Type = "TopicalCream", Description = "TopicalCream_Antibiotic_Ointment", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 5, Name = "Pill_Antifungal_Course", Type = "Pill", Description = "Pill_Antifungal_Course", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 6, Name = "TopicalCream_Antifungal_Course", Type = "TopicalCream", Description = "TopicalCream_Antifungal_Course", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 7, Name = "TopicalCream_Burn_Gel", Type = "TopicalCream", Description = "TopicalCream_Burn_Gel", Definition = "<Definition />", EditableItem = new EditableItem() });

		context.SaveChanges();

		var seeder = new HealthSeeder();
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	public void ShouldSeedData_PrimitiveTierInstallWithoutOtherTiers_ReturnsMayAlreadyInstalled()
	{
		using var context = BuildContext();
		SeedShouldSeedPrerequisites(context);

		context.Knowledges.Add(new Knowledge { Id = 1, Name = "Medicine", Description = "Medicine", LongDescription = "Medicine", Type = "Test", Subtype = "Test", LearnableType = 0, LearnDifficulty = 0, TeachDifficulty = 0, LearningSessionsRequired = 0 });

		context.SurgicalProcedures.AddRange(
			new SurgicalProcedure { Id = 1, Name = "Hasty Triage", ProcedureName = "Hasty Triage", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Hasty Triage", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 2, Name = "Primitive Stitching", ProcedureName = "Primitive Stitching", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Primitive Stitching", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 3, Name = "Exploratory Surgery", ProcedureName = "Exploratory Surgery", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Exploratory Surgery", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 4, Name = "Trauma Control", ProcedureName = "Trauma Control", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Trauma Control", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 5, Name = "Organ Extraction", ProcedureName = "Organ Extraction", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Organ Extraction", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 6, Name = "Crude Organ Repair", ProcedureName = "Crude Organ Repair", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Crude Organ Repair", Definition = string.Empty, TargetBodyTypeId = 0 },
			new SurgicalProcedure { Id = 7, Name = "Bone Setting", ProcedureName = "Bone Setting", MedicalSchool = "Test", Procedure = 0, BaseCheckBonus = 0.0, Check = 0, ProcedureBeginEmote = string.Empty, ProcedureDescriptionEmote = string.Empty, ProcedureGerund = "Bone Setting", Definition = string.Empty, TargetBodyTypeId = 0 });

		context.Drugs.AddRange(
			new Drug { Id = 1, Name = "Willow Bark Tea", DrugVectors = (int)DrugVector.Ingested, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 2, Name = "Mandrake Draught", DrugVectors = (int)(DrugVector.Ingested | DrugVector.Inhaled), IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 3, Name = "Honey Poultice", DrugVectors = (int)DrugVector.Touched, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 4, Name = "Garlic Salve", DrugVectors = (int)(DrugVector.Ingested | DrugVector.Touched), IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 5, Name = "Mint Infusion", DrugVectors = (int)DrugVector.Ingested, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 6, Name = "Ephedra Brew", DrugVectors = (int)DrugVector.Ingested, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 },
			new Drug { Id = 7, Name = "Foxglove Tincture", DrugVectors = (int)DrugVector.Ingested, IntensityPerGram = 1.0, RelativeMetabolisationRate = 0.1 });

		context.GameItemComponentProtos.AddRange(
			new GameItemComponentProto { Id = 1, Name = "Pill_Willow_Bark_Tea", Type = "Pill", Description = "Pill_Willow_Bark_Tea", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 2, Name = "Pill_Mandrake_Draught", Type = "Pill", Description = "Pill_Mandrake_Draught", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 3, Name = "TopicalCream_Honey_Poultice", Type = "TopicalCream", Description = "TopicalCream_Honey_Poultice", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 4, Name = "Pill_Garlic_Salve", Type = "Pill", Description = "Pill_Garlic_Salve", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 5, Name = "TopicalCream_Garlic_Salve", Type = "TopicalCream", Description = "TopicalCream_Garlic_Salve", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 6, Name = "Pill_Mint_Infusion", Type = "Pill", Description = "Pill_Mint_Infusion", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 7, Name = "Pill_Ephedra_Brew", Type = "Pill", Description = "Pill_Ephedra_Brew", Definition = "<Definition />", EditableItem = new EditableItem() },
			new GameItemComponentProto { Id = 8, Name = "Pill_Foxglove_Tincture", Type = "Pill", Description = "Pill_Foxglove_Tincture", Definition = "<Definition />", EditableItem = new EditableItem() });

		context.SaveChanges();

		var seeder = new HealthSeeder();
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}
}

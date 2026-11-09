#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureSeederHeritageCoverageTests
{

	[TestMethod]
	public void DarkAgesEthnicityExpansion_RerunsWithoutDuplicatesAndRepairsCharacteristics()
	{
		using FuturemudDatabaseContext context = BuildContext();
		Race human = BuildHumanRace();
		PopulationBloodModel bloodModel = new() { Name = "Test Blood Model" };
		FutureProg alwaysTrue = new()
		{
			FunctionName = "AlwaysTrue",
			FunctionComment = "Test helper",
			FunctionText = "return true",
			ReturnType = 1,
			Category = "Test",
			Subcategory = "Test"
		};
		CharacteristicDefinition definition = new()
		{
			Name = "Test Feature",
			Type = 0,
			Pattern = ".*",
			Description = "Test feature",
			Model = "standard",
			Definition = "<Definition />"
		};
		CharacteristicProfile expectedProfile = new()
		{
			Name = "Test Profile",
			Definition = "<Definition />",
			Type = "standard",
			Description = "Expected profile",
			TargetDefinition = definition
		};
		CharacteristicProfile wrongProfile = new()
		{
			Name = "Wrong Profile",
			Definition = "<Definition />",
			Type = "standard",
			Description = "Wrong profile",
			TargetDefinition = definition
		};
		context.AddRange(human, bloodModel, alwaysTrue, definition, expectedProfile, wrongProfile);

		foreach ((string nameCulture, string[] ethnicities) in
				CultureSeeder.DarkAgesAndMedievalEthnicityCoverageForTesting)
		{
			NameCulture culture = new() { Name = nameCulture, Definition = "<NameCulture />" };
			context.NameCultures.Add(culture);
			Ethnicity template = new()
			{
				Name = ethnicities[0],
				ChargenBlurb = "Template ethnicity",
				ParentRace = human,
				EthnicGroup = "Template",
				EthnicSubgroup = "Template",
				PopulationBloodModel = bloodModel,
				AvailabilityProg = alwaysTrue
			};
			template.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
			{
				Ethnicity = template,
				CharacteristicDefinition = definition,
				CharacteristicProfile = expectedProfile
			});
			template.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures
			{
				Ethnicity = template,
				NameCulture = culture,
				Gender = (short)Gender.Indeterminate
			});
			context.Ethnicities.Add(template);
		}

		context.SaveChanges();
		CultureSeeder seeder = new();
		SetPrivateField(seeder, "_context", context);
		SetPrivateField(seeder, "_humanRace", human);
		SetPrivateField(seeder, "_alwaysTrueProg", alwaysTrue);
		GetPrivateDictionary<string, PopulationBloodModel>(seeder, "_bloodModels")[bloodModel.Name] = bloodModel;
		GetPrivateDictionary<string, CharacteristicDefinition>(seeder, "_definitions")[definition.Name] = definition;
		GetPrivateDictionary<string, CharacteristicProfile>(seeder, "_profiles")[expectedProfile.Name] = expectedProfile;
		GetPrivateDictionary<string, CharacteristicProfile>(seeder, "_profiles")[wrongProfile.Name] = wrongProfile;

		InvokePrivate(seeder, "SeedDarkAgesAndMedievalHeritageExpansion");

		Ethnicity bavarian = context.Ethnicities.Single(x => x.Name == "High Medieval Bavarian");
		EthnicitiesCharacteristics originalCharacteristic = bavarian.EthnicitiesCharacteristics.Single();
		context.EthnicitiesCharacteristics.Remove(originalCharacteristic);
		context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
		{
			Ethnicity = bavarian,
			CharacteristicDefinition = definition,
			CharacteristicProfile = wrongProfile
		});
		context.SaveChanges();

		InvokePrivate(seeder, "SeedDarkAgesAndMedievalHeritageExpansion");

		Assert.AreEqual(1, context.Ethnicities.Count(x => x.Name == "High Medieval Bavarian"));
		Assert.AreEqual(expectedProfile.Id,
			context.EthnicitiesCharacteristics.Single(x => x.EthnicityId == bavarian.Id).CharacteristicProfileId);

	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options =
			new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static Race BuildHumanRace()
	{
		return new Race
		{
			Name = "Human",
			Description = "Test race",
			AllowedGenders = "0 1 2 3 4",
			DiceExpression = "1d1",
			CommunicationStrategyType = "humanoid",
			MaximumDragWeightExpression = "1",
			MaximumLiftWeightExpression = "1",
			HoldBreathLengthExpression = "1",
			BreathingVolumeExpression = "1",
			EatCorpseEmoteText = string.Empty,
			HandednessOptions = "1"
		};
	}

	private static void SetPrivateField(object target, string name, object value)
	{
		target.GetType()
			.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(target, value);
	}

	private static Dictionary<TKey, TValue> GetPrivateDictionary<TKey, TValue>(object target, string name)
		where TKey : notnull
	{
		return (Dictionary<TKey, TValue>)target.GetType()
			.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(target)!;
	}

	private static object? InvokePrivate(object target, string methodName, params object[] args)
	{
		return target.GetType()
			.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(target, args);
	}
}

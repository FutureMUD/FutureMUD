#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DbNameCulture = MudSharp.Models.NameCulture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureSeederNameAndHeightDefaultTests
{
	[TestMethod]
	public void CultureSeeder_CulturePackQuestion_AdvertisesRenamedHistoricalPacks()
	{
		using FuturemudDatabaseContext context = BuildContext();
		CultureSeeder seeder = new();
		var question = seeder.SeederQuestions.Single(x => x.Id == "culturepacks");

		foreach (var era in new[] { "Antiquity", "Dark Ages", "Medieval", "Renaissance", "Early Modern" })
		{
			Assert.IsTrue(question.Validator(era, context).Success);
			Assert.IsNotNull(CultureSeeder.ToolkitEra(era));
		}

		Assert.IsTrue(question.Validator("earth-darkagesandmedieval", context).Success);
		Assert.IsTrue(question.Validator("dark ages and medieval", context).Success);
		Assert.IsTrue(question.Validator("earth-renaissanceeurope", context).Success);
		Assert.IsTrue(question.Validator("earth-renaissanceworldexpansion", context).Success);
		Assert.IsTrue(question.Validator("earth-medievaleurope", context).Success,
			"The old package key should remain a hidden compatibility alias for saved answers.");
	}

	[TestMethod]
	public void CultureSeeder_MissingOptionalDistinctiveFeature_DoesNotBlockCulturePacks()
	{
		CultureSeeder seeder = new();

		seeder.AddEthnicityVariable("unused", "Distinctive Feature", "All Distinctive Features");

	}

	[TestMethod]
	public void RenaissanceWorldNameCultureLookup_IsCaseInsensitiveAfterClientEvaluation()
	{
		DbNameCulture[] cultures =
		[
			new DbNameCulture { Name = "Renaissance Armenian", Definition = "<Definition />" }
		];

		Assert.IsTrue(CultureSeeder.ContainsNameCultureForTesting(cultures, "renaissance armenian"));
		Assert.IsFalse(CultureSeeder.ContainsNameCultureForTesting(cultures, "Renaissance Amazigh"));
	}

	[TestMethod]
	public void CultureSeeder_FallbackProfiles_MakeBaseNameCulturesReadyAndRemainRerunnable()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.NameCultures.Add(new DbNameCulture
		{
			Name = "Admin",
			Definition = BuildNameCultureDefinition((NameUsage.BirthName, 1, 1))
		});
		context.SaveChanges();

		CultureSeeder seeder = new();
		RunSimpleNameSeeding(seeder, context);
		var originalProfileIds = context.RandomNameProfiles.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(originalProfileIds.Length > 0);
		RunSimpleNameSeeding(seeder, context);
		CollectionAssert.AreEqual(originalProfileIds, context.RandomNameProfiles.Select(x => x.Id).OrderBy(x => x).ToArray());

		foreach (var culture in context.NameCultures)
			AssertCultureHasReadyCompatibleProfile(context, culture.Name, Gender.Male);
	}

	[TestMethod]
	public void RomanAntiquityNameSeeder_NameCultureDefinitionsAndProfilesUseValidElements()
	{
		using FuturemudDatabaseContext context = BuildContext();
		CultureSeeder seeder = new();
		SetSeederContext(seeder, context);

		InvokePrivate(seeder, "SeedLatinNames", context);
		InvokePrivate(seeder, "SeedAntiquityRegionalNameCultures");

		foreach (DbNameCulture culture in context.NameCultures.ToList())
		{
			XElement root = XElement.Parse(culture.Definition);
			HashSet<int> definedUsages = root.Element("Elements")!
				.Elements("Element")
				.Select(x => int.Parse(x.Attribute("Usage")!.Value))
				.ToHashSet();

			foreach (XElement pattern in root.Element("Patterns")!.Elements("Pattern"))
			{
				List<int> parameters = pattern.Attribute("Params")!.Value
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(int.Parse)
					.ToList();
				foreach (int parameter in parameters)
				{
					Assert.IsTrue(definedUsages.Contains(parameter),
						$"{culture.Name} pattern {pattern.Attribute("Style")!.Value} references undefined usage {(NameUsage)parameter}.");
				}

				int highestPlaceholder = Regex.Matches(pattern.Attribute("Text")!.Value, @"\{(\d+)\}")
					.Select(x => int.Parse(x.Groups[1].Value))
					.DefaultIfEmpty(-1)
					.Max();
				Assert.IsTrue(highestPlaceholder < parameters.Count,
					$"{culture.Name} pattern {pattern.Attribute("Style")!.Value} has more placeholders than parameters.");
			}

			string regexText = root.Element("NameEntryRegex")!.Value.Trim();
			Regex nameRegex = new(regexText);
			foreach (string groupName in nameRegex.GetGroupNames().Where(x => !int.TryParse(x, out _)))
			{
				Assert.IsTrue(Enum.TryParse(groupName, true, out NameUsage usage),
					$"{culture.Name} regex uses unknown name element group {groupName}.");
				Assert.IsTrue(definedUsages.Contains((int)usage),
					$"{culture.Name} regex captures {usage.DescribeEnum()} but does not define that element.");
			}
		}

		foreach (MudSharp.Models.RandomNameProfile profile in context.RandomNameProfiles.ToList())
		{
			DbNameCulture culture = context.NameCultures.Single(x => x.Id == profile.NameCultureId);
			HashSet<int> definedUsages = XElement.Parse(culture.Definition)
				.Element("Elements")!
				.Elements("Element")
				.Select(x => int.Parse(x.Attribute("Usage")!.Value))
				.ToHashSet();

			foreach (RandomNameProfilesDiceExpressions dice in context.RandomNameProfilesDiceExpressions
				         .Where(x => x.RandomNameProfileId == profile.Id))
			{
				Assert.IsTrue(definedUsages.Contains(dice.NameUsage),
					$"{profile.Name} has dice for undefined {((NameUsage)dice.NameUsage).DescribeEnum()}.");
				Assert.IsTrue(context.RandomNameProfilesElements.Any(x =>
						x.RandomNameProfileId == profile.Id &&
						x.NameUsage == dice.NameUsage),
					$"{profile.Name} has dice for {((NameUsage)dice.NameUsage).DescribeEnum()} but no elements.");
			}

			foreach (RandomNameProfilesElements element in context.RandomNameProfilesElements
				         .Where(x => x.RandomNameProfileId == profile.Id))
			{
				Assert.IsTrue(definedUsages.Contains(element.NameUsage),
					$"{profile.Name} has element {element.Name} for undefined {((NameUsage)element.NameUsage).DescribeEnum()}.");
			}
		}
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void RunSimpleNameSeeding(CultureSeeder seeder, FuturemudDatabaseContext context)
	{
		SetSeederContext(seeder, context);
		InvokePrivate(seeder, "SeedSimple", context);
		InvokePrivate(seeder, "EnsureFallbackRandomNameProfiles");
	}

	private static void SetSeederContext(CultureSeeder seeder, FuturemudDatabaseContext context)
	{
		typeof(CultureSeeder)
			.GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(seeder, context);
	}

	private static object? InvokePrivate(object target, string methodName, params object[] args)
	{
		return target.GetType()
			.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(target, args);
	}

	private static void AssertCultureHasReadyCompatibleProfile(
		FuturemudDatabaseContext context,
		string cultureName,
		Gender targetGender)
	{
		DbNameCulture culture = context.NameCultures.Single(x => x.Name == cultureName);
		bool hasReadyProfile = context.RandomNameProfiles
			.Where(x => x.NameCultureId == culture.Id)
			.AsEnumerable()
			.Any(x => IsCompatibleGender((Gender)x.Gender, targetGender) && IsReadyProfile(context, x.Id));

		Assert.IsTrue(hasReadyProfile, $"Expected {cultureName} to have a ready random name profile compatible with {targetGender.DescribeEnum()}.");
	}

	private static bool IsReadyProfile(FuturemudDatabaseContext context, long profileId)
	{
		List<int> usages = context.RandomNameProfilesDiceExpressions
			.Where(x => x.RandomNameProfileId == profileId)
			.Select(x => x.NameUsage)
			.Distinct()
			.ToList();
		return usages.Count > 0 &&
		       usages.All(usage => context.RandomNameProfilesElements.Any(x =>
			       x.RandomNameProfileId == profileId && x.NameUsage == usage));
	}

	private static bool IsCompatibleGender(Gender profileGender, Gender targetGender)
	{
		return profileGender == Gender.NonBinary ||
		       profileGender == Gender.Indeterminate ||
		       profileGender == targetGender;
	}

	private static string BuildNameCultureDefinition(params (NameUsage Usage, int Minimum, int Maximum)[] elements)
	{
		return
			$"<NameCulture><Patterns><Pattern Style=\"0\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"1\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"2\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"3\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"4\" Text=\"{{0}}\" Params=\"0\" /><Pattern Style=\"5\" Text=\"{{0}}\" Params=\"0\" /></Patterns><Elements>{string.Concat(elements.Select(x => $"<Element Usage=\"{(int)x.Usage}\" MinimumCount=\"{x.Minimum}\" MaximumCount=\"{x.Maximum}\" Name=\"{x.Usage}\"><![CDATA[Test]]></Element>"))}</Elements><NameEntryRegex><![CDATA[^(?<birthname>[\\w '-]+)$]]></NameEntryRegex></NameCulture>";
	}
}

#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.NPC.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VariableNPCTemplateNameProfileTests
{
	[TestMethod]
	public void ResolveAutomaticNameProfile_EthnicityCulturePresent_PrefersEthnicityProfile()
	{
		Mock<IRandomNameProfile> cultureProfile = CreateProfile("Culture Profile", Gender.Male, true);
		Mock<IRandomNameProfile> ethnicityProfile = CreateProfile("Ethnicity Profile", Gender.Male, true);
		Mock<INameCulture> cultureNameCulture = CreateNameCulture(cultureProfile.Object);
		Mock<INameCulture> ethnicityNameCulture = CreateNameCulture(ethnicityProfile.Object);
		Mock<ICulture> culture = new();
		culture.Setup(x => x.NameCultureForGender(Gender.Male)).Returns(cultureNameCulture.Object);
		Mock<IEthnicity> ethnicity = new();
		ethnicity.Setup(x => x.NameCultureForGender(Gender.Male)).Returns(ethnicityNameCulture.Object);

		IRandomNameProfile? result = VariableNPCTemplate.ResolveAutomaticNameProfile(culture.Object, ethnicity.Object, Gender.Male);

		Assert.AreSame(ethnicityProfile.Object, result);
	}

	[TestMethod]
	public void ResolveAutomaticNameProfile_EthnicityCultureMissing_FallsBackToCultureProfile()
	{
		Mock<IRandomNameProfile> cultureProfile = CreateProfile("Culture Profile", Gender.Female, true);
		Mock<INameCulture> cultureNameCulture = CreateNameCulture(cultureProfile.Object);
		Mock<ICulture> culture = new();
		culture.Setup(x => x.NameCultureForGender(Gender.Female)).Returns(cultureNameCulture.Object);
		Mock<IEthnicity> ethnicity = new();
		ethnicity.Setup(x => x.NameCultureForGender(Gender.Female)).Returns((INameCulture)null!);

		IRandomNameProfile? result = VariableNPCTemplate.ResolveAutomaticNameProfile(culture.Object, ethnicity.Object, Gender.Female);

		Assert.AreSame(cultureProfile.Object, result);
	}

	[TestMethod]
	public void BuilderCommands_CultureAndEthnicity_RefreshAutomaticNameProfiles()
	{
		string source = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates", "VariableNPCTemplate.cs"));
		Assert.IsTrue(source.Contains("_culture = culture;") && source.Contains("RefreshAutomaticNameProfiles();"),
			"Culture changes should immediately refresh auto-selected name profiles.");
		Assert.IsTrue(source.Contains("_ethnicity = ethnicity;") && source.Contains("RefreshAutomaticNameProfiles();"),
			"Ethnicity changes should immediately refresh auto-selected name profiles.");
		Assert.IsFalse(source.Contains("_culture.NameCultureForGender(gender.Value).RandomNameProfiles"),
			"Culture auto-fill should use the shared ethnicity-first automatic profile resolver.");
	}

	private static Mock<IRandomNameProfile> CreateProfile(string name, Gender profileGender, bool compatible)
	{
		Mock<IRandomNameProfile> mock = new();
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.Gender).Returns(profileGender);
		mock.SetupGet(x => x.IsReady).Returns(true);
		mock.Setup(x => x.IsCompatibleGender(It.IsAny<Gender>()))
			.Returns<Gender>(gender => compatible && (profileGender == Gender.NonBinary || profileGender == Gender.Indeterminate || profileGender == gender));
		return mock;
	}

	private static Mock<INameCulture> CreateNameCulture(params IRandomNameProfile[] profiles)
	{
		Mock<INameCulture> mock = new();
		mock.SetupGet(x => x.RandomNameProfiles).Returns(profiles.ToList());
		return mock;
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}

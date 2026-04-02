#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using Calendar = MudSharp.Models.Calendar;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CelestialSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
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

		context.Clocks.Add(new Clock
		{
			Id = 1,
			Definition = "<Clock />",
			Seconds = 0,
			Minutes = 0,
			Hours = 12,
			PrimaryTimezoneId = 1
		});

		context.Calendars.Add(new Calendar
		{
			Id = 1,
			Definition = "<calendar><alias>test</alias><shortname>Test Calendar</shortname><fullname>Test Calendar</fullname></calendar>",
			Date = "1-1-1",
			FeedClockId = 1
		});

		context.SaveChanges();
	}

	private static Dictionary<string, string> BuildAnswers(
		bool installSun = false,
		bool installMoon = false,
		bool installGasGiant = false)
	{
		return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["installsun"] = installSun ? "yes" : "no",
			["suncalendar"] = "1",
			["sunname"] = "The Sun",
			["sunepoch"] = "1-jan-2000",
			["installmoon"] = installMoon ? "yes" : "no",
			["mooncalendar"] = "1",
			["moonname"] = "The Moon",
			["moonepoch"] = "21-jan-2000",
			["installgasgiantmoon"] = installGasGiant ? "yes" : "no",
			["gasgiantcalendar"] = "1",
			["gasgiantsunepoch"] = "1-jan-2000",
			["gasgiantmoonepoch"] = "1-jan-2000"
		};
	}

	private static XElement GetDefinition(Celestial celestial)
	{
		return XElement.Parse(celestial.Definition);
	}

	private static string GetPackage(Celestial celestial)
	{
		return GetDefinition(celestial).Element("SeederPackage")!.Value;
	}

	[TestMethod]
	public void SeedData_EarthMoonPackage_CreatesLinkedCelestials()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new CelestialSeeder();

		seeder.SeedData(context, BuildAnswers(installSun: true, installMoon: true));

		var celestials = context.Celestials.ToList();
		Assert.AreEqual(4, celestials.Count);

		var sun = celestials.Single(x => x.CelestialType == "Sun");
		var moon = celestials.Single(x => x.CelestialType == "PlanetaryMoon" && GetPackage(x) == "EarthMoonView");
		var planet = celestials.Single(x => x.CelestialType == "PlanetFromMoon" && GetPackage(x) == "EarthMoonView");
		var sunFromMoon = celestials.Single(x => x.CelestialType == "SunFromPlanetaryMoon" && GetPackage(x) == "EarthMoonView");

		Assert.AreEqual("EarthSun", GetPackage(sun));

		var planetDefinition = GetDefinition(planet);
		var sunFromMoonDefinition = GetDefinition(sunFromMoon);

		Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Moon")!.Value);
		Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Sun")!.Value);
		Assert.AreEqual("Earth", planetDefinition.Element("Name")!.Value);
		Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Moon")!.Value);
		Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Sun")!.Value);
		Assert.AreEqual("The Sun", sunFromMoonDefinition.Element("Name")!.Value);
		Assert.IsNotNull(sunFromMoonDefinition.Element("Illumination"));
	}

	[TestMethod]
	public void SeedData_EarthMoonPackageWithoutEarthSun_ThrowsClearError()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new CelestialSeeder();

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			seeder.SeedData(context, BuildAnswers(installMoon: true)));

		StringAssert.Contains(exception.Message, "requires a matching Earth-facing Sun");
	}

	[TestMethod]
	public void SeedData_GasGiantPackage_CreatesSelfContainedBundle()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new CelestialSeeder();

		seeder.SeedData(context, BuildAnswers(installGasGiant: true));

		var celestials = context.Celestials.ToList();
		Assert.AreEqual(4, celestials.Count);

		var sun = celestials.Single(x => x.CelestialType == "Sun");
		var moon = celestials.Single(x => x.CelestialType == "PlanetaryMoon");
		var planet = celestials.Single(x => x.CelestialType == "PlanetFromMoon");
		var sunFromMoon = celestials.Single(x => x.CelestialType == "SunFromPlanetaryMoon");

		Assert.AreEqual("GasGiantMoonView", GetPackage(sun));
		Assert.AreEqual("GasGiantMoonView", GetPackage(moon));
		Assert.AreEqual("GasGiantMoonView", GetPackage(planet));
		Assert.AreEqual("GasGiantMoonView", GetPackage(sunFromMoon));

		var sunDefinition = GetDefinition(sun);
		var moonDefinition = GetDefinition(moon);
		var planetDefinition = GetDefinition(planet);
		var sunFromMoonDefinition = GetDefinition(sunFromMoon);

		Assert.AreEqual("Sol", sunDefinition.Element("Name")!.Value);
		Assert.AreEqual("Ganymede", moonDefinition.Element("Name")!.Value);
		Assert.AreEqual("Jupiter", planetDefinition.Element("Name")!.Value);
		Assert.AreEqual("Sol", sunFromMoonDefinition.Element("Name")!.Value);
		Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Moon")!.Value);
		Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Sun")!.Value);
		Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Moon")!.Value);
		Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Sun")!.Value);
		Assert.AreEqual(
			sunDefinition.Element("Illumination")!.ToString(SaveOptions.DisableFormatting),
			sunFromMoonDefinition.Element("Illumination")!.ToString(SaveOptions.DisableFormatting));
	}

	[TestMethod]
	public void ShouldSeedData_PartialPackagesRemain_ReturnsExtraPackagesAvailable()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new CelestialSeeder();

		seeder.SeedData(context, BuildAnswers(installSun: true));

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));
	}
}

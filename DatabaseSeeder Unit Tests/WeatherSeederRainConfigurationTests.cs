#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeatherSeederRainConfigurationTests
{
	[DataTestMethod]
	[DataRow("full", "false", "true")]
	[DataRow("soak", "true", "false")]
	public void ConfigurePuddleSettingForRainMode_ReconcilesExistingSetting(string rainMode, string initialValue,
		string expectedValue)
	{
		using var context = CreateContext();
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "PuddlesEnabled",
			Definition = initialValue
		});
		context.SaveChanges();

		WeatherSeeder.ConfigurePuddleSettingForRainMode(context, rainMode);
		context.SaveChanges();
		WeatherSeeder.ConfigurePuddleSettingForRainMode(context, rainMode);
		context.SaveChanges();

		Assert.AreEqual(expectedValue, context.StaticConfigurations.Single(x => x.SettingName == "PuddlesEnabled").Definition);
		Assert.AreEqual(1, context.StaticConfigurations.Count(x => x.SettingName == "PuddlesEnabled"));
	}

	[TestMethod]
	public void ConfigurePuddleSettingForRainMode_FullCreatesEnabledSetting()
	{
		using var context = CreateContext();

		WeatherSeeder.ConfigurePuddleSettingForRainMode(context, "full");
		context.SaveChanges();

		Assert.AreEqual("true", context.StaticConfigurations.Single(x => x.SettingName == "PuddlesEnabled").Definition);
	}

	[TestMethod]
	public void ConfigurePuddleSettingForRainMode_NonePreservesExistingSetting()
	{
		using var context = CreateContext();
		context.StaticConfigurations.Add(new StaticConfiguration
		{
			SettingName = "PuddlesEnabled",
			Definition = "false"
		});
		context.SaveChanges();

		WeatherSeeder.ConfigurePuddleSettingForRainMode(context, "none");
		context.SaveChanges();

		Assert.AreEqual("false", context.StaticConfigurations.Single(x => x.SettingName == "PuddlesEnabled").Definition);
	}

	private static FuturemudDatabaseContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase($"{nameof(WeatherSeederRainConfigurationTests)}_{Guid.NewGuid():N}")
			.Options;
		return new FuturemudDatabaseContext(options);
	}
}

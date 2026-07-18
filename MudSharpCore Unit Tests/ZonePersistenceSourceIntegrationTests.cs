#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ZonePersistenceSourceIntegrationTests
{
	[TestMethod]
	public void Save_PersistsWeatherControllerId()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Construction", "Zone.cs"));
		var saveStart = source.IndexOf("public override void Save()", StringComparison.Ordinal);
		var registerStart = source.IndexOf("public static void RegisterPerceivableType", saveStart,
			StringComparison.Ordinal);

		Assert.IsTrue(saveStart >= 0, "Zone.Save() was not found.");
		Assert.IsTrue(registerStart > saveStart, "The end of Zone.Save() was not found.");
		StringAssert.Contains(source[saveStart..registerStart],
			"dbzone.WeatherControllerId = WeatherController?.Id;");
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

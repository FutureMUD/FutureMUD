using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Checks;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PsionicPowerSeederSourceTests
{
	private static readonly CheckType[] NewPsionicChecks =
	[
		CheckType.DangerSenseNearbyThreat,
		CheckType.DangerSenseDefense,
		CheckType.EmpathyPower,
		CheckType.HexPower,
		CheckType.ClairvoyancePower,
		CheckType.PresciencePower,
		CheckType.SensitivityPower,
		CheckType.SensitivityCapabilityRead,
		CheckType.PsychicBoltPower
	];

	[TestMethod]
	public void SkillSeeders_RegisterOldSoiPsionicChecksAsVariableChecks()
	{
		var skillSeeder = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "SkillSeeder.cs"));
		var packageSeeder = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "SkillPackageSeeder.cs"));

		foreach (var check in NewPsionicChecks.Select(x => $"CheckType.{x}"))
		{
			StringAssert.Contains(skillSeeder, check);
			StringAssert.Contains(packageSeeder, check);
		}

		StringAssert.Contains(skillSeeder, "Expression = \"variable\"");
		StringAssert.Contains(packageSeeder, "Expression = \"variable\"");
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

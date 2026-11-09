#nullable enable
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseSeeder.Seeders;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonTemplarManifestTests
{

	[TestMethod]
	public void GeneratedRoleRuleThresholds_UseParentGoOnAndPreserveResolvedWarriorValues()
	{
		var jihaen = new[]
		{
			("arm.skill.rescue", (string?)null, 80.0), ("arm.skill.bandage", (string?)"arm.skill.rescue", 40.0),
			("arm.skill.brew", (string?)"arm.skill.bandage", 80.0), ("arm.skill.parry", (string?)null, 55.0),
			("arm.skill.archery", (string?)"arm.skill.parry", 55.0), ("arm.skill.throw", (string?)"arm.skill.parry", 60.0)
		};
		var generated = ArmageddonTemplarInstaller.BuildRoleRuleThresholds(jihaen);
		Assert.AreEqual(80.0, generated["arm.skill.bandage"]);
		Assert.AreEqual(40.0, generated["arm.skill.brew"]);
		Assert.AreEqual(55.0, generated["arm.skill.archery"]);
		Assert.AreEqual(55.0, generated["arm.skill.throw"]);
		Assert.AreEqual(0.0, generated["arm.skill.rescue"]);

		var southern = ArmageddonTemplarInstaller.BuildRoleRuleThresholds([("arm.skill.warrior_example", (string?)"arm.skill.parent", 73.0)],
			sourceThresholdsAlreadyResolved: true);
		Assert.AreEqual(73.0, southern["arm.skill.warrior_example"]);
	}
}

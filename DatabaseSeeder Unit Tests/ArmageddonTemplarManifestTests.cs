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
	private static JsonDocument Manifest()
	{
		using var stream = Assembly.Load("DatabaseSeeder").GetManifestResourceStream("DatabaseSeeder.Armageddon.templar_classes.json");
		Assert.IsNotNull(stream, "Templar manifest must be embedded as an additive seeder module.");
		return JsonDocument.Parse(stream);
	}

	[TestMethod]
	public void NorthernTrees_RetainPublishedOpeningsCapsAndBranches()
	{
		using var manifest = Manifest();
		var roles = manifest.RootElement.GetProperty("roles").EnumerateArray().ToArray();
		Assert.AreEqual(3, roles.Length);
		var jihaen = roles.Single(x => x.GetProperty("key").GetString() == "arm.class.jihaen_templar");
		Assert.AreEqual("Male", jihaen.GetProperty("gender").GetString());
		var neck = jihaen.GetProperty("skills").EnumerateArray().Single(x => x.GetProperty("trait").GetString() == "arm.skill.contact");
		Assert.AreEqual(5, neck.GetProperty("opening").GetDouble());
		Assert.AreEqual(60, neck.GetProperty("cap").GetDouble());
		var brew = jihaen.GetProperty("skills").EnumerateArray().Single(x => x.GetProperty("trait").GetString() == "arm.skill.brew");
		Assert.AreEqual("arm.skill.bandage", brew.GetProperty("parent").GetString());
		Assert.AreEqual(80, brew.GetProperty("go_on").GetDouble());
		var lirathan = roles.Single(x => x.GetProperty("key").GetString() == "arm.class.lirathan_templar");
		Assert.AreEqual("Female", lirathan.GetProperty("gender").GetString());
		var shadow = lirathan.GetProperty("skills").EnumerateArray().Single(x => x.GetProperty("trait").GetString() == "arm.skill.shadowwalk");
		Assert.AreEqual(10, shadow.GetProperty("opening").GetDouble());
		Assert.AreEqual(90, shadow.GetProperty("cap").GetDouble());
		Assert.AreEqual("arm.skill.clairaudience", shadow.GetProperty("parent").GetString());
	}

	[TestMethod]
	public void SouthernConfiguration_UsesWarriorTreeAndBoundedGradeSevenShrineSelections()
	{
		using var manifest = Manifest();
		var southern = manifest.RootElement.GetProperty("roles").EnumerateArray().Single(x => x.GetProperty("key").GetString() == "arm.class.southern_templar");
		Assert.AreEqual("arm.class.warrior", southern.GetProperty("progression_from").GetString());
		var magic = manifest.RootElement.GetProperty("southern_magic");
		Assert.AreEqual(7, magic.GetProperty("fixed_grade").GetInt32());
		Assert.AreEqual(3, magic.GetProperty("selection_limit").GetInt32());
		Assert.IsFalse(magic.GetProperty("activation_default").GetBoolean());
		Assert.AreEqual(4, magic.GetProperty("choices").EnumerateObject().Count());
		Assert.AreEqual(12, magic.GetProperty("choices").EnumerateObject().Sum(x => x.Value.GetArrayLength()));
	}

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

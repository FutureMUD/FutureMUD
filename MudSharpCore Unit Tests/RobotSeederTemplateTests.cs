#nullable enable

using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RobotSeederTemplateTests
{
	[TestMethod]
	public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
	{
		var issues = RobotSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void TemplatesForTesting_KeyVariants_UseExpectedBodyAssignments()
	{
		Assert.AreEqual("Robot Humanoid", RobotSeeder.TemplatesForTesting["Robot Humanoid"].BodyKey);
		Assert.AreEqual("Spider Crawler Robot", RobotSeeder.TemplatesForTesting["Spider Crawler Robot"].BodyKey);
		Assert.AreEqual("Winged Robot", RobotSeeder.TemplatesForTesting["Winged Robot"].BodyKey);
		Assert.AreEqual("Cyborg Humanoid", RobotSeeder.TemplatesForTesting["Cyborg"].BodyKey);
		Assert.AreEqual("Roomba Robot", RobotSeeder.TemplatesForTesting["Roomba Robot"].BodyKey);
		Assert.AreEqual("Robot Dog", RobotSeeder.TemplatesForTesting["Robot Dog"].BodyKey);
		Assert.AreEqual("Robot Cockroach", RobotSeeder.TemplatesForTesting["Robot Cockroach"].BodyKey);
	}

	[TestMethod]
	public void TemplatesForTesting_AttachmentVariants_UseIntegratedWeaponBodies()
	{
		var sawRobot = RobotSeeder.TemplatesForTesting["Circular Saw Robot"];
		Assert.IsFalse(sawRobot.CanUseWeapons, "Saw-hand robots should not retain normal weapon handling.");
		CollectionAssert.AreEquivalent(
			new[] { "Circular Saw Slash", "Elbow", "Bite", "Snap Kick" },
			sawRobot.Attacks.Select(x => x.AttackName).ToArray());
		CollectionAssert.AreEquivalent(
			new[] { "rsaw", "lsaw" },
			sawRobot.BodypartUsages!.Select(x => x.BodypartAlias).ToArray());

		var hammerRobot = RobotSeeder.TemplatesForTesting["Pneumatic Hammer Robot"];
		Assert.IsFalse(hammerRobot.CanUseWeapons, "Hammer-hand robots should not retain normal weapon handling.");
		CollectionAssert.Contains(hammerRobot.Attacks.Select(x => x.AttackName).ToList(), "Pneumatic Hammer Blow");

		var swordRobot = RobotSeeder.TemplatesForTesting["Sword-Hand Robot"];
		Assert.IsFalse(swordRobot.CanUseWeapons, "Sword-hand robots should not retain normal weapon handling.");
		CollectionAssert.Contains(swordRobot.Attacks.Select(x => x.AttackName).ToList(), "Sword-Hand Lunge");
	}

	[TestMethod]
	public void TemplatesForTesting_MobilityVariants_ExposeExpectedFlavourAndLiquids()
	{
		var spider = RobotSeeder.TemplatesForTesting["Spider Crawler Robot"];
		CollectionAssert.AreEquivalent(
			new[] { "rleg1", "lleg1", "rleg2", "lleg2", "rleg3", "lleg3", "rleg4", "lleg4" },
			spider.BodypartUsages!.Select(x => x.BodypartAlias).ToArray(),
			"Spider crawlers should expose all crawler legs as flavour bodyparts.");

		var wheeled = RobotSeeder.TemplatesForTesting["Wheeled Robot"];
		Assert.IsFalse(wheeled.CanClimb, "Wheeled robots should not be seeded as climbers.");
		CollectionAssert.AreEquivalent(
			new[] { "rwheel", "lwheel" },
			wheeled.BodypartUsages!.Select(x => x.BodypartAlias).ToArray());

		var trackedUtility = RobotSeeder.TemplatesForTesting["Tracked Utility Robot"];
		Assert.AreEqual("machine oil", trackedUtility.BloodLiquidName,
			"Utility robots should use machine oil as their circulatory fluid.");
		CollectionAssert.AreEquivalent(
			new[] { "rtrack", "ltrack" },
			trackedUtility.BodypartUsages!.Select(x => x.BodypartAlias).ToArray());
	}

	[TestMethod]
	public void TemplatesForTesting_Cyborg_RemainsOnlyPlayableRobotRace()
	{
		var playableRaces = RobotSeeder.TemplatesForTesting
			.Where(x => x.Value.Playable)
			.Select(x => x.Key)
			.ToArray();

		CollectionAssert.AreEquivalent(new[] { "Cyborg" }, playableRaces);
		Assert.IsTrue(RobotSeeder.TemplatesForTesting["Cyborg"].UsesHumanoidCharacteristics,
			"Cyborgs should retain the humanoid characteristic matrix.");
		Assert.AreEqual("Human", RobotSeeder.TemplatesForTesting["Cyborg"].ParentRaceName,
			"Cyborgs should inherit from the human race line for presentation purposes.");
	}
}

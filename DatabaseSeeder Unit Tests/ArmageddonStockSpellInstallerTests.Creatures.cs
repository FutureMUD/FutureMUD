#nullable enable

using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	public void Creatures_CanonicalDefinitionsBindAll49ApprovedTemplatesAndLockedValues()
	{
		using var db = Fixture();
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		foreach (var contribution in ArmageddonCreatureSpellContent.Contributions)
		{
			var approved = manifest.RootElement.GetProperty("entries").EnumerateArray().Single(x => x.GetProperty("key").GetString() == contribution.Key);
			Assert.AreEqual(approved.GetProperty("minimum_energy").GetDouble(), contribution.MinimumEnergy);
			Assert.AreEqual("arm.capability." + approved.GetProperty("native_school").GetString()!.ToLowerInvariant(), contribution.NativeCapabilityKey);
			var spell = db.MagicSpells.Single(x => x.Id == installed.Identities[contribution.Key]);
			var definition = XElement.Parse(spell.Definition);
			var effect = definition.Element("Effects")!.Elements().Single();
			Assert.AreEqual("createnpc", (string?)effect.Attribute("type"));
			var mapping = effect.Element("GradeTemplates")!.Elements("Template").ToDictionary(x => (int)x.Attribute("grade")!, x => (long)x);
			Assert.AreEqual(7, mapping.Count); Assert.AreEqual(7, mapping.Values.Distinct().Count());
			foreach (var grade in Enumerable.Range(1, 7))
			{
				var key = $"arm.npc.{contribution.NpcFamily}.g{grade}";
				Assert.IsTrue(approved.GetProperty("prototype_dependencies").EnumerateArray().Any(x => x.GetString() == key));
				var claim = db.SeederManagedRecords.Single(x => x.StableKey == key);
				Assert.AreEqual(claim.LogicalId, mapping[grade]);
				Assert.IsTrue(db.NpcTemplates.Include(x => x.EditableItem).Any(x => x.Id == mapping[grade] &&
					x.RevisionNumber == claim.RevisionNumber && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current));
			}
			Assert.AreEqual("TemporaryCleanup", (string?)effect.Element("Lifecycle")!.Attribute("mode"));
			Assert.AreEqual("120*grade", effect.Element("Lifecycle")!.Element("Seconds")!.Value);
			Assert.AreEqual(contribution.CountByGrade ? "grade" : "1", effect.Element("Lifecycle")!.Element("Count")!.Value);
			Assert.AreEqual(contribution.GuardCaster, (bool)effect.Element("Lifecycle")!.Element("GuardCaster")!);
			Assert.AreEqual(contribution.Mission == SpellNpcMissionKind.None ? "room" : "roommission", (string?)definition.Element("Trigger")!.Attribute("type"));
			if (contribution.Mission != SpellNpcMissionKind.None)
			{
				Assert.AreEqual("1", (string?)definition.Element("Trigger")!.Attribute("version"));
				Assert.AreEqual(contribution.Mission.ToString(), (string?)definition.Element("Trigger")!.Element("Mission"));
				Assert.AreEqual(contribution.Mission.ToString(), (string?)effect.Element("Mission")!.Attribute("kind"));
			}
			Assert.AreEqual(contribution.MinimumEnergy, (double)definition.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
			Assert.AreEqual(contribution.OpeningSkill, (int)definition.Element("ControlledPower")!.Attribute("openingSkill")!);
			Assert.AreEqual("0", db.TraitExpressions.Single(x => x.Id == spell.EffectDurationExpressionId).Expression);
			Assert.AreEqual($"{contribution.MinimumEnergy}*grade", db.TraitExpressions.Single(x => x.Id == installed.Identities[contribution.Key + ".cost"]).Expression);
			Assert.IsFalse(spell.ScrollInscriptionAllowed); Assert.IsFalse(definition.Element("Plan")!.HasElements);
		}
		Assert.AreEqual(0, db.Characters.Count()); Assert.AreEqual(0, db.Bodies.Count()); Assert.AreEqual(0, db.GameItems.Count());
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}

	[DataTestMethod]
	[DataRow("missing")][DataRow("retired")][DataRow("module")][DataRow("version")][DataRow("type")]
	[DataRow("revision")][DataRow("withdrawn")][DataRow("controller")][DataRow("purpose")][DataRow("riding")]
	public void Creatures_InvalidUnselectedGradeDependencyHoldsRetainingEarlierRead(string corruption)
	{
		using var db = Fixture();
		var claim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.npc.earth_mount.g7");
		var template = db.NpcTemplates.Include(x => x.EditableItem).Include(x => x.NpctemplatesArtificalIntelligences).Single(x => x.Id == claim.LogicalId);
		switch (corruption)
		{
			case "missing": db.NpcTemplates.Remove(template); break;
			case "retired": claim.Retired = true; break;
			case "module": claim.Module = "wrong"; break;
			case "version": claim.ManifestVersion = "unknown"; break;
			case "type": claim.EntityType = nameof(GameItemProto); break;
			case "revision": claim.RevisionNumber = 1; break;
			case "withdrawn": template.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign; break;
			case "controller": db.Remove(template.NpctemplatesArtificalIntelligences.Single(x => db.ArtificialIntelligences.Any(y => y.Id == x.AiId && y.Type == "SpellCompanion"))); break;
			case "riding": db.Remove(template.NpctemplatesArtificalIntelligences.Single(x => db.ArtificialIntelligences.Any(y => y.Id == x.AiId && y.Type == "Mount"))); break;
			case "purpose":
				var aiClaim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.ai.earth_mount");
				var ai = db.ArtificialIntelligences.Single(x => x.Id == aiClaim.LogicalId);
				var definition = XElement.Parse(ai.Definition); definition.Element("Purpose")!.Value = "owned_guard"; ai.Definition = definition.ToString();
				break;
		}
		db.SaveChanges(); db.ChangeTracker.Clear();
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, installed.Status, string.Join("\n", installed.Messages));
		Assert.AreEqual(3, installed.Identities.Count);
		Assert.AreEqual(1, db.MagicSpells.Count());
		Assert.AreEqual(3, db.SeederManagedRecords.Count(x => x.Module == ArmageddonStockSpellInstaller.Module));
		Assert.IsTrue(installed.Identities.ContainsKey(ArmageddonStockSpellContent.ReadAttunementKey));
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}
}

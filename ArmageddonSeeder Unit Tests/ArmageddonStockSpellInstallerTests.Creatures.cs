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
	[TestCategory("Integration")]
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
		Assert.AreEqual(6, installed.Identities.Count);
		Assert.AreEqual(2, db.MagicSpells.Count());
		Assert.AreEqual(6, db.SeederManagedRecords.Count(x => x.Module == ArmageddonStockSpellInstaller.Module));
		Assert.IsTrue(installed.Identities.ContainsKey(ArmageddonStockSpellContent.ReadAttunementKey));
		Assert.IsTrue(installed.Identities.ContainsKey("arm.spell.unravel_enchantment"));
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}
}

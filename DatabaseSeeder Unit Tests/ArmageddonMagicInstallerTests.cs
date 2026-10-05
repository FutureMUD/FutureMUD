#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonMagicInstallerTests
{
	private static (FuturemudDatabaseContext Db, ArmageddonMagicInstallPlan Plan) Fixture()
	{
		var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		db.Accounts.Add(new() { Id = 1, Name = "installer" }); db.Materials.Add(new() { Id = 1, Name = "wood" });
		db.MagicSchools.Add(new() { Id = 1, Name = "selected school" }); db.MagicResources.Add(new() { Id = 1, Name = "selected resource" });
		db.Liquids.Add(new() { Id = 1, Name = "selected water" }); db.WearProfiles.Add(new() { Id = 1, Name = "selected profile", Type = "Direct" });
		var no = Prog(1, "AlwaysFalse", "return false"); var mend = Prog(2, "MendEligibility", "return true");
		foreach (var name in new[] { "target", "caster" }) mend.FutureProgsParameters.Add(new() { ParameterIndex = mend.FutureProgsParameters.Count, ParameterName = name, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.AddRange(no, mend);
		db.GameItemComponentProtos.AddRange(ArmageddonNativeLightFixture.Components(1));
		var light = new GameItemProto { Id = 1, Name = "selected light", EditableItem = new() { RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
		for (var i = 0; i < 3; i++) light.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = i + 1 });
		db.GameItemProtos.Add(light);
		var keys = new[] { "sense_enchantment", "unravel_enchantment", "mend_flesh", "draw_water", "hovering_light" };
		var skills = new Dictionary<string, long>();
		for (var i = 0; i < keys.Length; i++) { db.TraitDefinitions.Add(new() { Id = i + 1, Name = keys[i], Type = 0, OwnerScope = 1 }); skills.Add("arm.spell." + keys[i], i + 1); }
		db.SaveChanges(); db.ChangeTracker.Clear();
		return (db, new(true, 1, 1, skills, 1, 2, 1, 1, 0, 1, 0, 1, 1));
	}
	private static FutureProg Prog(long id, string name, string text) => new() { Id = id, FunctionName = name, FunctionText = text,
		ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionComment = "", Category = "Tests", Subcategory = "Installer" };
	private static ArmageddonInstallResult Run(FuturemudDatabaseContext db, ArmageddonMagicInstallPlan plan)
	{ db.ChangeTracker.Clear(); return ArmageddonMagicInstaller.Install(db, plan); }
	private static void Installed(ArmageddonInstallResult result) => Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));

	[TestMethod] public void DecliningNeverQueriesEvenDisposedContext()
	{
		var (db, plan) = Fixture(); db.Dispose();
		Assert.AreEqual(ArmageddonInstallStatus.Declined, ArmageddonMagicInstaller.Install(db, plan with { Install = false }).Status);
	}
	[TestMethod] public void RealContentIsEditableAndRerunKeepsEveryIdentityAndBaseline()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var first = Run(db, plan); Installed(first); Assert.AreEqual(21, first.Identities.Count);
			Assert.AreEqual(5, db.MagicSpells.Count()); Assert.AreEqual(3, db.GameItemProtos.Count());
			Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.MagicCapabilities.Count());
			Assert.IsTrue(db.MagicSpells.All(x => !x.ScrollInscriptionAllowed));
			var baselines = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => (x.SeedBaseline, x.AppliedAt));
			var second = Run(db, plan); Installed(second); CollectionAssert.AreEquivalent(first.Identities.ToArray(), second.Identities.ToArray());
			foreach (var record in db.SeederManagedRecords) Assert.AreEqual(baselines[record.StableKey], (record.SeedBaseline, record.AppliedAt));
			Assert.AreEqual("2*grade*grade", XElement.Parse(db.MagicSpells.Single(x => x.Name == "Mend Flesh").Definition).Element("Effects")!.Element("Effect")!.Element("HealingAmount")!.Value);
			foreach (var device in db.GameItemComponentProtos.Where(x => x.Type == "ChargedMagicDevice"))
			{ var xml = XElement.Parse(device.Definition); Assert.AreEqual(1, (int)xml.Element("Eligibility")!); Assert.AreEqual(2, (int)xml.Element("Role")!); }
		}
	}
	[TestMethod] public void RenameDefinitionAndRemovedItemLinkSurviveRepeatedRuns()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var spell = db.MagicSpells.Single(x => x.Name == "Mend Flesh");
			spell.Name = "Builder remedy"; spell.Definition = spell.Definition.Replace("2*grade*grade", "3*grade");
			var item = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "Armageddon blank wand");
			db.Remove(item.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemComponentProtoId == 1)); db.SaveChanges();
			for (var i = 0; i < 2; i++) { var result = Run(db, plan); Installed(result); Assert.IsTrue(result.Messages.Any(x => x.Contains("preserved builder edit"))); }
			Assert.AreEqual("Builder remedy", db.MagicSpells.Single(x => x.Id == spell.Id).Name);
			StringAssert.Contains(db.MagicSpells.Single(x => x.Id == spell.Id).Definition, "3*grade");
			Assert.AreEqual(1, db.GameItemProtosGameItemComponentProtos.Count(x => x.GameItemProtoId == item.Id));
		}
	}
	[DataTestMethod] [DataRow(false)] [DataRow(true)] public void MissingOrRetiredOwnedContentBlocksWithoutResurrection(bool retire)
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.spell.mend_flesh");
			if (retire) record.Retired = true; else db.MagicSpells.Remove(db.MagicSpells.Single(x => x.Id == record.LogicalId));
			db.SaveChanges(); var count = db.MagicSpells.Count(); var result = Run(db, plan);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status); Assert.AreEqual(count, db.MagicSpells.Count());
			Assert.AreEqual(0, result.Identities.Count); StringAssert.Contains(string.Join(" ", result.Messages), "arm.spell.mend_flesh");
		}
	}
	[TestMethod] public void UnownedCollisionBlocksWholeBatchAndDoesNotAdopt()
	{
		var (db, plan) = Fixture(); using (db)
		{
			db.MagicSpells.Add(new() { Name = "Mend Flesh", MagicSchoolId = 1, Definition = "<Definition/>" }); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status);
			Assert.AreEqual(1, db.MagicSpells.Count()); Assert.AreEqual(0, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void UnownedCloneIsNotClaimedOrChanged()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var original = db.MagicSpells.Single(x => x.Name == "Mend Flesh");
			db.MagicSpells.Add(new() { Name = "Builder clone", MagicSchoolId = 1, Definition = original.Definition }); db.SaveChanges();
			Installed(Run(db, plan)); Assert.AreEqual(6, db.MagicSpells.Count()); Assert.AreEqual(21, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void UnbaselinedOwnedFieldsRemainUntouched()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.spell.mend_flesh"); record.SeedBaseline = null;
			db.MagicSpells.Single(x => x.Id == record.LogicalId).Description = "Builder unbaselined content"; db.SaveChanges();
			var result = Run(db, plan); Installed(result); Assert.IsTrue(result.Messages.Any(x => x.Contains("unbaselined field")));
			Assert.AreEqual("Builder unbaselined content", db.MagicSpells.Single(x => x.Id == record.LogicalId).Description);
		}
	}
	[TestMethod] public void CompetingOwnerIsRefused()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.charged_wand");
			db.SeederManagedRecords.Add(new() { Seeder = "Other", StableKey = "other", EntityType = record.EntityType, LogicalId = record.LogicalId, RevisionNumber = record.RevisionNumber }); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status);
		}
	}
	[TestMethod] public void WrongSkillScopeNeverInstalls()
	{
		var (db, plan) = Fixture(); using (db)
		{
			db.TraitDefinitions.Single(x => x.Id == 1).OwnerScope = 0; db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(0, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void LaterBuilderRevisionIsPreservedAndBlocksOldRevisionWrites()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan)); var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.charged_wand");
			db.GameItemProtos.Add(new() { Id = record.LogicalId!.Value, RevisionNumber = 1, Name = "Builder later wand", EditableItem = new() { RevisionStatus = 2 } }); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(2, db.GameItemProtos.Count(x => x.Id == record.LogicalId));
		}
	}
	[TestMethod] public void WrongEligibilitySignatureOrCompilerFailureHasNoWrites()
	{
		var (db, plan) = Fixture(); using (db)
		{
			var mend = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == plan.MendEligibilityProg);
			mend.FutureProgsParameters.First().ParameterTypeDefinition = ProgVariableTypes.Item.ToStorageString(); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(0, db.SeederManagedRecords.Count());
			db.FutureProgs.Single(x => x.Id == plan.MendEligibilityProg).FunctionText = "return no_such_function()"; db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Failed, Run(db, plan).Status); Assert.AreEqual(0, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void BindingChangeReplacesOnlyUnchangedOwnedLinksAndKeepsExtraBuilderLinks()
	{
		var (db, plan) = Fixture(); using (db)
		{
			Installed(Run(db, plan));
			db.GameItemComponentProtos.Add(new() { Id = 99, Type = "Holdable", Name = "replacement holdable", Definition = "<Definition/>", EditableItem = new() { RevisionStatus = 4 } });
			db.GameItemComponentProtos.Add(new() { Id = 98, Type = "Salvageable", Name = "builder extra component", Definition = "<Definition/>", EditableItem = new() { RevisionStatus = 4 } });
			var item = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "Armageddon blank wand");
			item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = 98 }); db.SaveChanges();
			Installed(Run(db, plan with { HoldableComponent = 99 }));
			var ids = db.GameItemProtosGameItemComponentProtos.Where(x => x.GameItemProtoId == item.Id).Select(x => x.GameItemComponentProtoId).ToArray();
			CollectionAssert.Contains(ids, 98L); CollectionAssert.Contains(ids, 99L); CollectionAssert.DoesNotContain(ids, 1L);
		}
	}
	[DataTestMethod]
	[DataRow(false, 0)] [DataRow(true, 0)] [DataRow(false, 7)] [DataRow(true, 7)]
	public void FirstInstallReservesForeignDeletedPrototypeIdentitiesAcrossRetirementAndRevisions(bool retired, int revision)
	{
		var (db, plan) = Fixture(); using (db)
		{
			var componentId = db.GameItemComponentProtos.Max(x => x.Id) + 1;
			var itemId = db.GameItemProtos.Max(x => x.Id) + 1;
			var component = new GameItemComponentProto { Id = componentId, RevisionNumber = revision, Name = "foreign deleted component", Type = "Holdable", Definition = "<Definition/>", EditableItem = new() { RevisionStatus = 4 } };
			var item = new GameItemProto { Id = itemId, RevisionNumber = revision, Name = "foreign deleted item", MaterialId = 1, EditableItem = new() { RevisionStatus = 4 } };
			db.AddRange(component, item); db.SaveChanges();
			foreach (var (type, id) in new[] { (nameof(GameItemComponentProto), componentId), (nameof(GameItemProto), itemId) })
				db.SeederManagedRecords.Add(new() { Seeder = "ForeignPackage", Module = "foreign", EntityType = type, StableKey = "foreign." + type,
					LogicalId = id, RevisionNumber = revision, Retired = retired, SeedBaseline = "{\"Name\":\"foreign baseline\"}", AppliedFingerprint = "foreign fingerprint", ManifestVersion = "foreign-1", AppliedAt = DateTime.UnixEpoch });
			db.SaveChanges(); db.RemoveRange(component, item); db.SaveChanges();
			var before = JsonSerializer.Serialize(db.SeederManagedRecords.AsNoTracking().OrderBy(x => x.Id).ToArray());
			var first = Run(db, plan); Installed(first);
			Assert.IsTrue(first.Identities.Where(x => x.Key.StartsWith("arm.component.")).All(x => x.Value > componentId));
			Assert.IsTrue(first.Identities.Where(x => x.Key.StartsWith("arm.item.")).All(x => x.Value > itemId));
			Assert.IsFalse(db.GameItemComponentProtos.Any(x => x.Id == componentId)); Assert.IsFalse(db.GameItemProtos.Any(x => x.Id == itemId));
			var second = Run(db, plan); Installed(second); CollectionAssert.AreEquivalent(first.Identities.ToArray(), second.Identities.ToArray());
			Assert.AreEqual(before, JsonSerializer.Serialize(db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == "ForeignPackage").OrderBy(x => x.Id).ToArray()));
		}
	}
	[DataTestMethod] [DataRow(nameof(MagicSpell), 1L)] [DataRow(nameof(TraitExpression), 1L)] [DataRow(nameof(FutureProg), 3L)]
	public void EveryNewClaimChecksRetainedOwnershipAfterPreflight(string entityType, long logicalId)
	{
		var (db, plan) = Fixture(); using (db)
		{
			var result = ArmageddonMagicInstaller.Install(db, plan, phase =>
			{
				if (phase != ArmageddonInstallCheckpoint.PreflightComplete) return;
				db.SeederManagedRecords.Add(new() { Seeder = "ForeignPackage", EntityType = entityType, StableKey = "foreign.deleted-identity", LogicalId = logicalId, Retired = true });
				db.SaveChanges();
			});
			Assert.AreEqual(ArmageddonInstallStatus.Failed, result.Status); StringAssert.Contains(string.Join(" ", result.Messages), "retained ownership");
			// In-memory transactions do not roll back earlier rows; assert the guarded
			// identity is never claimed. Whole-batch rollback is qualified in MySQL.
			Assert.AreEqual(0, db.SeederManagedRecords.Count(x => x.Seeder == ArmageddonMagicInstaller.Package && x.EntityType == entityType && x.LogicalId == logicalId));
			Assert.AreEqual(1, db.SeederManagedRecords.Count(x => x.Seeder == "ForeignPackage" && x.EntityType == entityType && x.LogicalId == logicalId && x.Retired));
		}
	}
}

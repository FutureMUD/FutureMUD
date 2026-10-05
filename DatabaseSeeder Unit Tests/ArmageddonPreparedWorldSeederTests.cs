#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonPreparedWorldSeederTests
{
	private static readonly InMemoryDatabaseRoot PreparedStores = new();
	private static (FuturemudDatabaseContext Db, ArmageddonPreparedWorldBindings Bindings) Fixture()
	{
		var (source, traditions) = ArmageddonTraditionInstallerTests.Fixture();
		using var dependencies = source;
		// Proxy settings intentionally create separate EF service providers. Share their root,
		// while each unique database name isolates a fixture and avoids one provider per case.
		var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), PreparedStores, x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		db.MagicSchools.AddRange(source.MagicSchools.AsNoTracking());
		db.MagicResources.AddRange(source.MagicResources.AsNoTracking());
		db.TraitDecorators.AddRange(source.TraitDecorators.AsNoTracking());
		db.TraitExpressions.AddRange(source.TraitExpressions.AsNoTracking());
		db.Improvers.AddRange(source.Improvers.AsNoTracking());
		db.TraitDefinitions.AddRange(source.TraitDefinitions.AsNoTracking());
		db.FutureProgs.AddRange(source.FutureProgs.AsNoTracking());
		db.MagicCapabilities.AddRange(source.MagicCapabilities.AsNoTracking());
		db.SaveChanges();
		// Copy prepared dependencies only, never the helper's unowned stock spells.
		db.Accounts.Add(new() { Id = 1, Name = "disposable builder" }); db.Materials.Add(new() { Id = 1, Name = "wood" });
		db.Liquids.Add(new() { Id = 1, Name = "selected water" }); db.WearProfiles.Add(new() { Id = 1, Name = "native profile", Type = "Direct" });
		db.TraitDefinitions.Add(new() { Id = 100, Name = "explicit body attribute", Type = 1, OwnerScope = 0, DecoratorId = 1 });
		db.TraitExpressions.Add(new() { Id = 100, Name = "authored native capacity", Expression = "variable*10" });
		db.MagicResources.Single().Definition = "<Definition><AttributeCapacity version='1' attribute='100' expression='100' basis='raw'/></Definition>";
		var eligibility = new FutureProg { Id = 3, FunctionName = "explicitMend", FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
		foreach (var name in new[] { "target", "caster" }) eligibility.FutureProgsParameters.Add(new() { ParameterIndex = eligibility.FutureProgsParameters.Count,
			ParameterName = name, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(eligibility);
		db.GameItemComponentProtos.AddRange(ArmageddonNativeLightFixture.Components(1));
		var light = new GameItemProto { Id = 1, Name = "selected light", EditableItem = new() { RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
		for (var i = 0; i < 3; i++) light.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = i + 1 });
		db.GameItemProtos.Add(light);
		var keys = ArmageddonMagicInstaller.Content(new(false, 0, 0, new Dictionary<string, long>(), 0, 0, 0, 0, 0, 0, 0, 0, 0)).Select(x => x.Key).ToArray();
		var skills = new Dictionary<string, long>();
		for (var i = 0; i < keys.Length; i++)
		{
			db.TraitDefinitions.Add(new() { Id = i + 10, Name = "external " + keys[i], Type = 0, OwnerScope = 1 }); skills.Add(keys[i], i + 10);
		}
		db.SaveChanges(); db.ChangeTracker.Clear();
		return (db, new(new(true, 1, 1, skills, 1, 3, 1, 1, 0, 1, 0, 1, 1), 1, 1, 2, 1, traditions.SupportSkills, 100, 100, "raw",
			ArmageddonTraditionInstaller.Variants.ToDictionary(x => x, _ => (IReadOnlyList<MagicGatheringMethodKind>)new[] { MagicGatheringMethodKind.Self })));
	}
	private static Func<FuturemudDatabaseContext> Factory(FuturemudDatabaseContext db)
	{
		var options = (DbContextOptions<FuturemudDatabaseContext>)db.GetService<IDbContextOptions>();
		return () => new FuturemudDatabaseContext(options);
	}
	private static void Completed(ArmageddonPreparedWorldInstallResult result) => Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, result.Describe());
	private static Dictionary<string, string> Answers(ArmageddonPreparedWorldBindings bindings) => new()
	{
		[ArmageddonMagicSeeder.InstallQuestion] = "yes", [ArmageddonMagicSeeder.BindingsQuestion] = ArmageddonMagicSeeder.SerializeBindings(bindings)
	};

	[TestMethod] public void OptInAlwaysDefaultsNoDespiteHistoricalYesAndDeclineDoesNotQuery()
	{
		var (db, _) = Fixture(); using (db)
		{
			var seeder = new ArmageddonMagicSeeder(); var question = seeder.Questions.First();
			db.SeederChoices.Add(new() { Seeder = seeder.Name, Choice = question.Id, Answer = "yes", DateTime = DateTime.UtcNow, Version = "old" }); db.SaveChanges();
			Assert.AreEqual("no", SeederAnswerMemory.GetRememberedAnswer(db, seeder, question, new Dictionary<string, string>()));
			Assert.IsFalse(question.PersistAnswer); Assert.IsFalse(question.AutoReuseLastAnswer);
			Assert.IsFalse(seeder.Questions.Last().Filter(db, new Dictionary<string, string> { [question.Id] = "no" }));
			db.Dispose(); StringAssert.Contains(seeder.SeedData(db, new Dictionary<string, string> { [question.Id] = "no" }), "declined");
		}
	}
	[TestMethod] public void BindingDocumentIsStrictAndCapacityCannotBeInvented()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var json = ArmageddonMagicSeeder.SerializeBindings(bindings); Assert.IsTrue(ArmageddonMagicSeeder.ValidateBindings(json, db).Success);
			foreach (var invalid in new[] { "null", "{}", json.Insert(1, "\"Unexpected\":1,"), json.Insert(1, "\"ReserveResource\":1,"),
				json.Replace("\"Self\"", "0"), ArmageddonMagicSeeder.SerializeBindings(bindings with { CapacityAttribute = 1 }),
				ArmageddonMagicSeeder.SerializeBindings(bindings with { CapacityBasis = "guessed" }), ArmageddonMagicSeeder.SerializeBindings(bindings with { ReserveResource = 0 }) })
				Assert.IsFalse(ArmageddonMagicSeeder.ValidateBindings(invalid, db).Success, invalid);
			Assert.AreEqual(0, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void ExplicitRoutesRemainWithinApprovedMatrixAndTemplateIsRevalidated()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var full = new Dictionary<string, IReadOnlyList<MagicGatheringMethodKind>>
			{
				["sorcerer"] = [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Gentle, MagicGatheringMethodKind.Land],
				["preserver"] = [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Gentle], ["defiler"] = [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Land]
			};
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { AllowedMethods = full }));
			full["defiler"] = [MagicGatheringMethodKind.Gentle];
			Assert.IsFalse(ArmageddonMagicSeeder.ValidateBindings(ArmageddonMagicSeeder.SerializeBindings(bindings with { AllowedMethods = full }), db).Success);
		}
	}
	[TestMethod] public void SharedExecutorInstallsFourClosedAdmissionsWithStableRerunsAndNoPlayerMutation()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var seeder = new ArmageddonMagicSeeder();
			var execution = SeederExecutionService.Execute(db, seeder, seeder.Questions, Answers(bindings), new Version(1, 0));
			Assert.IsTrue(execution.Success, execution.Exception?.ToString()); StringAssert.Contains(execution.Message!, "4/82 stored");
			var ids = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
			Assert.AreEqual(196, ids.Count); Assert.AreEqual(6, db.MagicSpells.Count());
			Assert.IsFalse(db.SeederManagedRecords.Any(x => x.Module == ArmageddonProvisionInstaller.Module));
			Assert.IsFalse(db.SeederChoices.Any(x => x.Choice == ArmageddonMagicSeeder.InstallQuestion));
			Assert.AreEqual(1, db.SeederChoices.Count(x => x.Choice == ArmageddonMagicSeeder.BindingsQuestion));
			for (var i = 0; i < 2; i++)
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(result);
				Assert.AreEqual(4, result.Modules.Count); Assert.IsTrue(result.Availability.All(x => x.StoredAdmissions.Count == 4 && x.WithoutStoredAdmission.Count == 78));
				Assert.IsTrue(db.SeederManagedRecords.AsNoTracking().AsEnumerable().All(x => ids[x.StableKey] == x.LogicalId));
			}
			Assert.AreEqual(0, db.CharacterTraits.Count()); Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
			Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.ChargenRoles.Count()); Assert.AreEqual(0, db.CharactersMagicResources.Count());
		}
	}
	[TestMethod] public void PersistedBuilderAdmissionOverrideIsReportedInsteadOfDesiredClosure()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var owned = db.MagicCapabilities.Single(x => x.Name == "Armageddon partial defiler"); var xml = XElement.Parse(owned.Definition);
			xml.Element("Casting")!.Elements("Admission").Last().Remove(); owned.Definition = xml.ToString(); db.SaveChanges();
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(result);
			Assert.AreEqual(3, result.Availability.Single(x => x.Variant == "defiler").StoredAdmissions.Count);
			Assert.AreEqual(4, result.Availability.Single(x => x.Variant == "sorcerer").StoredAdmissions.Count);
			Assert.AreEqual(xml.ToString(), db.MagicCapabilities.AsNoTracking().Single(x => x.Id == owned.Id).Definition);
		}
	}
	[TestMethod] public void InteractiveLazyProxyCallerUsesIndependentPlainModelContextsOnRerun()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var original = (DbContextOptions<FuturemudDatabaseContext>)db.GetService<IDbContextOptions>();
			using var interactive = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>(original).UseLazyLoadingProxies().Options);
			var seeder = new ArmageddonMagicSeeder();
			for (var i = 0; i < 2; i++)
			{
				var result = SeederExecutionService.Execute(interactive, seeder, seeder.Questions, Answers(bindings), new Version(1, 0));
				Assert.IsTrue(result.Success, result.Exception?.ToString()); StringAssert.Contains(result.Message!, "4/82 stored");
			}
			Assert.AreEqual(196, db.SeederManagedRecords.Count());
		}
	}
	[TestMethod] public void MismatchedProvisionSelectionIsBlockedBeforeAnyModuleAndRetiredRecordsArePreserved()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Assert.IsTrue(ArmageddonPreparedWorldInstaller.NewProvisionsQualified);
			var blocked = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings with { Provisions = new(true, 999, 1, 1, 1, 2, [], 1, []) });
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, blocked.Status); Assert.AreEqual(0, blocked.Modules.Count); Assert.AreEqual(0, db.SeederManagedRecords.Count());
			db.SeederManagedRecords.Add(new() { Seeder = ArmageddonMagicInstaller.Package, Module = ArmageddonProvisionInstaller.Module,
				StableKey = ArmageddonReviewedProvisionContent.SustainMealKey, EntityType = "MagicSpell", LogicalId = 1000, Retired = true }); db.SaveChanges();
			blocked = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, blocked.Status); Assert.AreEqual(1, db.SeederManagedRecords.Count());
			Assert.IsTrue(db.SeederManagedRecords.Single().Retired);
		}
	}
	[TestMethod] public void LostModuleAcknowledgementStopsAndFreshRerunCompletesSameOwnedIds()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{ if (module == ArmageddonMagicInstaller.Module && point == ArmageddonInstallCheckpoint.AfterCommit) throw new Exception("lost acknowledgement"); });
			Assert.AreEqual(ArmageddonInstallStatus.CommittedConfirmationFailed, result.Status); Assert.AreEqual(1, result.Modules.Count);
			Assert.AreEqual(21, db.SeederManagedRecords.Count()); var ids = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
			StringAssert.Contains(result.Describe(), "earlier completed modules remain committed");
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			Assert.IsTrue(db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonMagicInstaller.Module).AsEnumerable().All(x => ids[x.StableKey] == x.LogicalId));
		}
	}
	[TestMethod] public void LaterInvalidNativeGatheringStopsAfterCommittedUtilitiesWithoutClaimingRollback()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			var template = db.MagicCapabilities.Single(); var xml = XElement.Parse(template.Definition); xml.Element("Gathering")!.Elements("Method").First(x => (string?)x.Attribute("kind") == "Self").SetAttributeValue("stamina", 0);
			template.Definition = xml.ToString(); db.SaveChanges();
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings);
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status); Assert.AreEqual(2, result.Modules.Count);
			Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Modules[0].Status); Assert.AreEqual(21, db.SeederManagedRecords.Count());
			StringAssert.Contains(result.Describe(), "earlier completed modules remain committed"); Assert.AreEqual(0, result.Availability.Count);
		}
	}
	[TestMethod] public void EveryProfileDeclinesViaRealQuestionWorkflowWithCompleteInventory()
	{
#if DEBUG
		var (db, _) = Fixture(); using (db)
		foreach (var profile in DebugSeederReplayProfiles.All)
		{
			var validation = SeederReplayRunner.Validate(profile, SeederCatalogue.GetEnabledSeeders()); Assert.IsTrue(validation.IsValid, string.Join("\n", validation.Errors));
			var step = profile.Steps.Single(x => x.SeederType == typeof(ArmageddonMagicSeeder)); Assert.AreEqual(2, step.Answers.Count);
			var seeder = new ArmageddonMagicSeeder(); var prior = new Dictionary<string, string>();
			foreach (var question in seeder.Questions)
			{
				var answer = step.Answers.Single(x => x.Id == question.Id).Answer;
				if (!SeederQuestionWorkflow.IsActive(question, db, prior)) { Assert.AreEqual("none", answer); continue; }
				Assert.IsTrue(SeederQuestionWorkflow.Validate(question, answer, db).Success); prior.Add(question.Id, answer);
			}
			Assert.AreEqual("no", prior[ArmageddonMagicSeeder.InstallQuestion]);
		}
#endif
	}
}

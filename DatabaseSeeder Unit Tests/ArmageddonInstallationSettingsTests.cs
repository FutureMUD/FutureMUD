#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Accounts;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using ChargenScreenStoryboard = MudSharp.Models.ChargenScreenStoryboard;
using Material = MudSharp.Models.Material;
using Liquid = MudSharp.Models.Liquid;
using Account = MudSharp.Models.Account;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonInstallationSettingsTests
{
	private static readonly InMemoryDatabaseRoot Stores = new();
	internal static FuturemudDatabaseContext Fixture(bool classes = true, FuturemudDatabaseContext? legacy = null)
	{
		var db = legacy ?? new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), Stores, x => x.EnableNullChecks(false)).Options);
		db.SeederChoices.Add(new SeederChoice { Seeder = new ChargenSeeder().Name, Choice = "class", Answer = classes ? "yes" : "no" });
		db.ChargenScreenStoryboards.Add(new ChargenScreenStoryboard { Id = 1, ChargenStage = (int)ChargenStage.SelectRole,
			ChargenType = "RolePicker", StageDefinition = $"<Screen><RoleTypes><RoleType Type=\"Class\" CanSelectNone=\"{(!classes).ToString().ToLowerInvariant()}\" /></RoleTypes></Screen>" });
		if (legacy is null)
		{
		db.TraitDefinitions.AddRange(new TraitDefinition { Id = 1, Name = "Wisdom", Type = 1, TraitGroup = "Mental" }, new TraitDefinition { Id = 2, Name = "Admin language", Type = 0 });
		db.TraitDecorators.Add(new TraitDecorator { Id = 1, Name = "General Skill", Type = "SimpleNumeric" });
		db.Improvers.Add(new Improver { Id = 1, Name = "Skill Improver", Type = "classic" });
		db.FutureProgs.AddRange(new FutureProg { Id = 1, FunctionName = "AlwaysTrue", FunctionText = "return true", ReturnType = (long)ProgVariableTypes.Boolean },
			new FutureProg { Id = 2, FunctionName = "AlwaysFalse", FunctionText = "return false", ReturnType = (long)ProgVariableTypes.Boolean });
		}
		else
		{
			db.TraitDefinitions.Add(new TraitDefinition { Id = 9999, Name = "Wisdom", Type = 1, TraitGroup = "Mental" });
			db.TraitDecorators.Single(x => x.Id == 1).Name = "General Skill";
			db.Improvers.Single(x => x.Id == 1).Name = "Skill Improver";
			db.FutureProgs.Single(x => x.Id == 1).FunctionName = "AlwaysFalse";
			db.FutureProgs.Single(x => x.Id == 2).FunctionName = "AlwaysTrue";
		}
		db.BodyProtos.Add(new BodyProto { Id = 1 });
		db.BodypartProtos.Add(new BodypartProto { Id = 1, BodyId = 1 });
		db.HealthStrategies.Add(new HealthStrategy { Id = 1 });
		db.Races.Add(new Race { Id = 1, Name = "Human", BaseBodyId = 1, DefaultHealthStrategyId = 1 });
		db.Calendars.Add(new Calendar { Id = 1 });
		db.NameCultures.Add(new NameCulture { Id = 1 });
		db.FutureProgs.Add(new FutureProg { Id = 9000, FunctionName = "StockStartingValue", FunctionText = "return 100", ReturnType = (long)ProgVariableTypes.Number, AcceptsAnyParameters = true });
		db.Cultures.Add(new Culture { Id = 1, Name = "Admin", PrimaryCalendarId = 1, SkillStartingValueProgId = 9000 });
		db.CulturesNameCultures.Add(new CulturesNameCultures { CultureId = 1, NameCultureId = 1 });
		db.LanguageDifficultyModels.Add(new LanguageDifficultyModels { Id = 1 });
		db.Languages.Add(new Language { Id = 1, Name = "Admin Speech", LinkedTraitId = legacy is null ? 2 : 1, DifficultyModel = 1 });
		db.Materials.AddRange(new Material { Id = 1, Name = "wood", BehaviourType = (int)MaterialBehaviourType.Wood, Density = 700 },
			new Material { Id = 2, Name = "stone", BehaviourType = (int)MaterialBehaviourType.Stone, Density = 2000 });
		db.Liquids.Add(new Liquid { Id = 1, Name = "water", Density = 1 });
		db.UnitsOfMeasure.AddRange(new UnitOfMeasure { Id = 1, Name = "kilogram", System = "Metric", Type = (int)UnitType.Mass, BaseMultiplier = 1000 },
			new UnitOfMeasure { Id = 2, Name = "metre", System = "Metric", Type = (int)UnitType.Length, BaseMultiplier = 100 },
			new UnitOfMeasure { Id = 3, Name = "litre", System = "Metric", Type = (int)UnitType.FluidVolume, BaseMultiplier = 1 });
		db.AuthorityGroups.Add(new AuthorityGroup { Id = 1, AuthorityLevel = (int)PermissionLevel.Founder });
		db.Accounts.Add(new Account { Id = 1, AuthorityGroupId = 1 });
		db.CheckTemplates.Add(new CheckTemplate { Id = 1 });
		if (legacy is null) db.TraitExpressions.Add(new TraitExpression { Id = 1, Expression = "1" });
		foreach (var type in new[] { CheckType.CastSpellCheck, CheckType.TraitBranchCheck, CheckType.CraftOutcomeCheck, CheckType.CraftQualityCheck })
			db.Checks.Add(new Check { Type = (int)type, CheckTemplateId = 1, TraitExpressionId = 1 });
		db.SaveChanges();
		return db;
	}

	internal static Func<FuturemudDatabaseContext> Factory(FuturemudDatabaseContext db)
	{
		var options = (DbContextOptions<FuturemudDatabaseContext>)db.GetService<IDbContextOptions>();
		return () => new FuturemudDatabaseContext(options);
	}

	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)]
	[DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)]
	[DataRow(ArmageddonProgressionMode.Simple, true)]
	public void FixedDirectionMatrix_OnlySettingsAndMirrors_RerunPreservesIdentity(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(classes);
		var settings = ArmageddonInstallation.Prepare(Factory(db), mode);
		Assert.AreEqual(mode, settings.Mode); Assert.AreEqual(classes, settings.ClassesEnabled);
		using var first = Factory(db)();
		var record = first.SeederManagedRecords.Single();
		var mirrorIds = first.SeederChoices.Where(x => x.Seeder == "__shared__").Select(x => x.Id).ToArray();
		Assert.AreEqual(3, mirrorIds.Length); Assert.IsNull(record.LogicalId);
		Assert.AreEqual(0, first.MagicCapabilities.Count()); Assert.AreEqual(0, first.MagicSpells.Count());
		Assert.AreEqual(0, first.ChargenRoles.Count()); Assert.AreEqual(0, first.Characters.Count()); Assert.AreEqual(0, first.GameItemProtos.Count());
		ArmageddonInstallation.Prepare(Factory(db));
		using var second = Factory(db)();
		Assert.AreEqual(record.Id, second.SeederManagedRecords.Single().Id);
		Assert.AreEqual(record.AppliedAt, second.SeederManagedRecords.Single().AppliedAt);
		CollectionAssert.AreEqual(mirrorIds, second.SeederChoices.Where(x => x.Seeder == "__shared__").Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void ContradictoryModeAttributeOrNativeBindings_RefusedWithoutMutation()
	{
		using var db = Fixture();
		ArmageddonInstallation.Prepare(Factory(db), ArmageddonProgressionMode.Simple);
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db), ArmageddonProgressionMode.Classic));
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db), capacityAttribute: 123));
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db), nativeOverrides: new Dictionary<ArmageddonNativeBinding, long> { [ArmageddonNativeBinding.Wood] = 2 }));
		Assert.AreEqual(1, db.SeederManagedRecords.Count()); Assert.AreEqual(4, db.SeederChoices.Count());
	}

	[TestMethod]
	public void ClassAnswerAndStoryboardDisagreement_StopsBeforeSettings()
	{
		using var db = Fixture();
		db.ChargenScreenStoryboards.Single().StageDefinition = "<Screen><RoleTypes><RoleType Type=\"Class\" CanSelectNone=\"true\" /></RoleTypes></Screen>";
		db.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db)));
		Assert.AreEqual(0, db.SeederManagedRecords.Count()); Assert.AreEqual(1, db.SeederChoices.Count());
	}

	[TestMethod]
	public void ConflictingSharedClassAnswer_StopsBeforeSettings()
	{
		using var db = Fixture();
		db.SeederChoices.Add(new SeederChoice { Seeder = "__shared__", Choice = "use-classes", Answer = "no" }); db.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db)));
		Assert.AreEqual(0, db.SeederManagedRecords.Count());
	}

	[DataTestMethod]
	[DataRow("unit")][DataRow("prog")][DataRow("body")][DataRow("check")][DataRow("names")][DataRow("cultureprog")]
	public void MissingOrIncompatibleTypedFoundation_StopsBeforeSettings(string failure)
	{
		using var db = Fixture();
		switch (failure)
		{
			case "unit": db.UnitsOfMeasure.Single(x => x.Id == 1).BaseMultiplier = -1; break;
			case "prog": db.FutureProgs.Single(x => x.Id == 1).FunctionText = "return 1"; break;
			case "body": db.BodypartProtos.RemoveRange(db.BodypartProtos); break;
			case "check": db.Checks.RemoveRange(db.Checks); break;
			case "names": db.NameCultures.RemoveRange(db.NameCultures); break;
			case "cultureprog": db.FutureProgs.Single(x => x.Id == 9000).ReturnType = (long)ProgVariableTypes.Boolean; db.FutureProgs.Single(x => x.Id == 9000).FunctionText = "return true"; break;
		}
		db.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db)));
		Assert.AreEqual(0, db.SeederManagedRecords.Count()); Assert.AreEqual(1, db.SeederChoices.Count());
	}

	[TestMethod]
	public void AmbiguousNativeMapping_RequiresExplicitId_ThenSurvivesRename()
	{
		using var db = Fixture();
		db.Materials.Add(new Material { Id = 3, Name = "wood", BehaviourType = (int)MaterialBehaviourType.Wood, Density = 800 }); db.SaveChanges();
		Assert.AreEqual(0L, ArmageddonInstallation.ResolveNativeBindings(db)[ArmageddonNativeBinding.Wood]);
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db)));
		ArmageddonInstallation.Prepare(Factory(db), nativeOverrides: new Dictionary<ArmageddonNativeBinding, long> { [ArmageddonNativeBinding.Wood] = 3 });
		db.Materials.Single(x => x.Id == 3).Name = "Builder renamed"; db.SaveChanges();
		Assert.AreEqual(3L, ArmageddonInstallation.Prepare(Factory(db)).NativeBindings[ArmageddonNativeBinding.Wood]);
	}

	[DataTestMethod]
	[DataRow("missing")][DataRow("corrupt")][DataRow("retired")][DataRow("mirror")][DataRow("competition")][DataRow("duplicate")]
	public void DamagedOwnershipOrMirrors_FailClosed(string failure)
	{
		using var db = Fixture(); ArmageddonInstallation.Prepare(Factory(db));
		var record = db.SeederManagedRecords.Single();
		switch (failure)
		{
			case "missing": db.SeederManagedRecords.Remove(record); break;
			case "corrupt": record.SeedBaseline = "{}"; break;
			case "retired": record.Retired = true; break;
			case "mirror": db.SeederChoices.Single(x => x.Choice == ArmageddonInstallation.ModeAnswerKey).Answer = "Simple"; break;
			case "competition": db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = "other", StableKey = ArmageddonInstallation.SettingsKey }); break;
			case "duplicate": db.SeederChoices.Add(new SeederChoice { Seeder = "__shared__", Choice = ArmageddonInstallation.ModeAnswerKey, Answer = "Classic" }); break;
		}
		db.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Read(db));
	}

	[TestMethod]
	public void FailureBeforeSave_WritesNothing_LostAcknowledgementPreservesFixedDirection()
	{
		using var db = Fixture();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db), checkpoint: c =>
		{ if (c == ArmageddonInstallCheckpoint.ContentCreated) throw new InvalidOperationException("before save"); }));
		Assert.AreEqual(0, db.SeederManagedRecords.Count()); Assert.AreEqual(1, db.SeederChoices.Count());
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db), ArmageddonProgressionMode.Simple, checkpoint: c =>
		{ if (c == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("lost acknowledgement"); }));
		using var fresh = Factory(db)();
		Assert.AreEqual(ArmageddonProgressionMode.Simple, ArmageddonInstallation.Read(fresh)!.Mode);
		ArmageddonInstallation.Prepare(Factory(db));
		Assert.AreEqual(1, fresh.SeederManagedRecords.Count()); Assert.AreEqual(4, fresh.SeederChoices.Count());
	}

	[TestMethod]
	public void LegacyTypedOwnership_Preserved_MissingReferencedEntityBlocks()
	{
		using var db = Fixture();
		db.MagicSchools.Add(new MagicSchool { Id = 9, Name = "Builder renamed" });
		db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = ArmageddonMagicInstaller.Module,
			EntityType = nameof(MagicSchool), StableKey = "arm.school.arcane", LogicalId = 9, AppliedFingerprint = "builder baseline" }); db.SaveChanges();
		var settings = ArmageddonInstallation.Prepare(Factory(db), enableStockForNewInstall: true);
		Assert.IsFalse(settings.StockAvailabilityByDefault, "Legacy package claims must not be activated as a fresh v1 install.");
		Assert.IsFalse(ArmageddonInstallation.Read(db)!.StockAvailabilityByDefault);
		Assert.AreEqual("builder baseline", db.SeederManagedRecords.Single(x => x.LogicalId == 9).AppliedFingerprint);
		Assert.AreEqual("Builder renamed", db.MagicSchools.Single().Name);
		db.MagicSchools.RemoveRange(db.MagicSchools); db.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonInstallation.Prepare(Factory(db)));
	}

	[TestMethod]
	public void CompletedLegacyTraditionMerits_KeepOwnedIdsAndBaselines()
	{
		var (legacy, plan) = ArmageddonTraditionInstallerTests.Fixture();
		using var db = legacy;
		var installed = ArmageddonTraditionInstaller.Install(db, plan);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join(Environment.NewLine, installed.Messages));
		var claims = db.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == nameof(Merit)).ToArray();
		Assert.AreEqual(3, claims.Length);
		Fixture(legacy: db); db.ChangeTracker.Clear();
		ArmageddonInstallation.Prepare(Factory(db));
		foreach (var claim in claims)
		{
			var preserved = db.SeederManagedRecords.AsNoTracking().Single(x => x.Id == claim.Id);
			Assert.AreEqual(claim.LogicalId, preserved.LogicalId); Assert.AreEqual(claim.SeedBaseline, preserved.SeedBaseline);
			Assert.IsTrue(db.Merits.Any(x => x.Id == claim.LogicalId));
		}
	}

	[TestMethod]
	public void IncompatibleAutomaticPrimitive_OffersCompatibleExplicitBinding()
	{
		using var db = Fixture();
		db.Materials.Single(x => x.Id == 1).BehaviourType = (int)MaterialBehaviourType.Stone;
		db.Materials.Add(new Material { Id = 3, Name = "oak", BehaviourType = (int)MaterialBehaviourType.Wood, Density = 800 }); db.SaveChanges();
		var question = new ArmageddonMagicSeeder().Questions.Single(x => x.Id == ArmageddonMagicSeeder.NativeBindingsQuestion);
		Assert.IsTrue(question.Filter(db, new Dictionary<string, string> { [ArmageddonMagicSeeder.FullInstallQuestion] = "yes" }));
		Assert.IsTrue(question.Validator("{\"Wood\":3}", db).Success);
		var settings = ArmageddonInstallation.Prepare(Factory(db), nativeOverrides: ArmageddonInstallation.ParseOverrides("{\"Wood\":3}"));
		Assert.AreEqual(3L, settings.NativeBindings[ArmageddonNativeBinding.Wood]);
	}

	[TestMethod]
	public void EmbeddedStockGraph_Valid_MissingDependencyAndCrossCapabilityParentRefused()
	{
		var json = ArmageddonStockManifest.Read(); Assert.AreEqual(0, ArmageddonStockManifest.Validate(json).Count);
		var missing = JsonNode.Parse(json)!;
		missing["prototype_catalogue"]![0]!["dependencies"]![0] = "unknown.prototype";
		Assert.IsTrue(ArmageddonStockManifest.Validate(missing.ToJsonString()).Count > 0);
		var cross = JsonNode.Parse(json)!;
		cross["classic_policy"]!["elemental_rows"]![1]!["parent_binding_key"] = "arm.research.guild.earth.armor";
		Assert.IsTrue(ArmageddonStockManifest.Validate(cross.ToJsonString()).Count > 0);
	}

	[DataTestMethod]
	[DataRow("{\"Wood\":1,\"wood\":2}")][DataRow("{\"Characters\":1}")][DataRow("{\"Wood\":0}")][DataRow("{\"8\":1}")]
	public void NativeOverrides_OnlyKnownUniquePositiveFields(string json) => Assert.ThrowsException<FormatException>(() => ArmageddonInstallation.ParseOverrides(json));

	[TestMethod]
	public void NativeQuestionValidator_RejectsWrongTypedExplicitBinding()
	{
		using var db = Fixture();
		var question = new ArmageddonMagicSeeder().Questions.Single(x => x.Id == ArmageddonMagicSeeder.NativeBindingsQuestion);
		Assert.IsFalse(question.Validator("{\"Wood\":2}", db).Success);
		Assert.IsTrue(question.Validator("{}", db).Success);
	}

	[TestMethod]
	public void FixedSettings_BlockLegacyPreparedApiBeforeAnyModule()
	{
		using var db = Fixture(); ArmageddonInstallation.Prepare(Factory(db));
		var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), null!);
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status);
		Assert.AreEqual(0, result.Modules.Count); Assert.AreEqual(0, db.MagicSpells.Count());
	}

	[TestMethod]
	public void SharedExecutor_CatalogueFailureRetainsDurableSettingsAndProgressionWithoutGenericAnswers()
	{
		using var db = Fixture();
		var seeder = new ArmageddonMagicSeeder();
		var result = SeederExecutionService.Execute(db, seeder, seeder.Questions, new Dictionary<string, string>
		{ [ArmageddonMagicSeeder.FullInstallQuestion] = "yes", [ArmageddonMagicSeeder.ModeQuestion] = "Simple" }, new Version(1, 0));
		Assert.IsFalse(result.Success, "The deliberately minimal S1/S2 fixture lacks S3 catalogue prerequisites.");
		Assert.AreEqual(4, db.SeederChoices.Count()); Assert.IsTrue(db.SeederManagedRecords.Count() > 1);
		Assert.AreEqual(10, db.MagicCapabilities.Count()); Assert.AreEqual(10, db.ChargenRoles.Count());
		StringAssert.Contains(result.Exception!.Message, "supplies");
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.Module == "stock-catalogue"));
	}
}

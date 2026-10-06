#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonPreparedWorldSeederTests
{
	private static ArmageddonPreparedWorldBindings ProvisionSelections(FuturemudDatabaseContext db, ArmageddonPreparedWorldBindings bindings)
	{
		var decorator = (db.StackDecorators.Max(x => (long?)x.Id) ?? 0) + 1;
		var componentId = db.GameItemComponentProtos.Max(x => x.Id) + 1;
		var foodId = db.GameItemProtos.Max(x => x.Id) + 1; var foods = new List<ArmageddonFoodPrototype>();
		var wine = db.Liquids.Max(x => x.Id) + 1;
		db.StackDecorators.Add(new() { Id = decorator, Name = "selected bites", Type = "Bites", Definition = "<Definition><Range Min='0' Max='99' Item='partly eaten {0}'/></Definition>" });
		db.GameItemComponentProtos.Add(new() { Id = componentId, Type = "Food", Name = "selected food", Definition = $"<Definition Satiation='2' Water='0' Thirst='0' Alcohol='0' Bites='4' Decorator='{decorator}'><OnEatProg>0</OnEatProg></Definition>", EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } });
		for (var i = 0; i < 3; i++)
		{
			var food = new GameItemProto { Id = foodId + i, Name = "selected meal " + i, EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } };
			foreach (var component in new[] { 1L, componentId }) food.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component });
			db.GameItemProtos.Add(food); foods.Add(new(food.Id, 0));
		}
		db.Liquids.Add(new() { Id = wine, Name = "selected wine", AlcoholLitresPerLitre = 0.12 }); db.SaveChanges();
		long Skill(string key) => db.SeederManagedRecords.Single(x => x.StableKey == key + ".skill").LogicalId!.Value;
		return bindings with { Provisions = new(true, bindings.Utilities.School, bindings.Utilities.Resource, bindings.Utilities.AlwaysFalseProg,
			Skill(ArmageddonReviewedProvisionContent.SustainMealKey), Skill(ArmageddonReviewedProvisionContent.DrawWineKey),
			[new(32, 0, foods)], wine, [new(32, 0, wine)]) };
	}
	private static string ProvisionPolicy(FuturemudDatabaseContext db) => JsonSerializer.Serialize(new
	{
		Records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonProvisionInstaller.Module).OrderBy(x => x.StableKey)
			.Select(x => new { x.StableKey, x.LogicalId, x.SeedBaseline, x.AppliedFingerprint }).ToArray(),
		Spells = db.MagicSpells.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonProvisionInstaller.Module && y.EntityType == "MagicSpell" && y.LogicalId == x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Definition }).ToArray(),
		Expressions = db.TraitExpressions.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonProvisionInstaller.Module && y.EntityType == "TraitExpression" && y.LogicalId == x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Expression }).ToArray()
	});
	[TestMethod]
	public void ExplicitProvisionSelectionComposesSevenAdmissionsWithoutPlayerMutation()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings)); bindings = ProvisionSelections(db, bindings);
			var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(result);
			Assert.AreEqual(5, result.Modules.Count); Assert.AreEqual(203, db.SeederManagedRecords.Count());
			Assert.IsTrue(result.Availability.All(x => x.StoredAdmissions.Count == 7));
			var policy = CapabilityMeritPolicy(db); var provisions = ProvisionPolicy(db);
			for (var i = 0; i < 2; i++) Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			Assert.AreEqual(policy, CapabilityMeritPolicy(db)); Assert.AreEqual(provisions, ProvisionPolicy(db));
			Assert.AreEqual(0, db.CharacterCastingEnrolments.Count()); Assert.AreEqual(0, db.CharacterTraits.Count());
			Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.CharactersMagicResources.Count());
		}
	}
	[DataTestMethod]
	[DataRow("sorcerer", ArmageddonReviewedProvisionContent.SustainMealKey)]
	[DataRow("preserver", ArmageddonReviewedProvisionContent.SustainMealKey)]
	[DataRow("defiler", ArmageddonReviewedProvisionContent.SustainMealKey)]
	[DataRow("sorcerer", ArmageddonReviewedProvisionContent.DrawWineKey)]
	[DataRow("preserver", ArmageddonReviewedProvisionContent.DrawWineKey)]
	[DataRow("defiler", ArmageddonReviewedProvisionContent.DrawWineKey)]
	public void ExactProvisionRemovalSurvivesCompletePlanReruns(string variant, string key)
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings)); bindings = ProvisionSelections(db, bindings);
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var spell = db.SeederManagedRecords.Single(x => x.StableKey == key).LogicalId!.Value;
			var cap = db.MagicCapabilities.Single(x => x.Name == "Armageddon partial " + variant); var xml = XElement.Parse(cap.Definition);
			xml.Element("Casting")!.Elements("Admission").Single(x => (long?)x.Attribute("spell") == spell).Remove(); cap.Definition = xml.ToString(); db.SaveChanges();
			var policy = CapabilityMeritPolicy(db); var provisions = ProvisionPolicy(db);
			for (var i = 0; i < 2; i++)
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(result);
				var actual = result.Availability.Single(x => x.Variant == variant);
				Assert.AreEqual(6, actual.StoredAdmissions.Count); Assert.IsFalse(actual.StoredAdmissions.Contains(key));
				Assert.AreEqual(policy, CapabilityMeritPolicy(db)); Assert.AreEqual(provisions, ProvisionPolicy(db));
			}
		}
	}
	[DataTestMethod]
	[DataRow(ArmageddonInstallCheckpoint.ContentCreated)]
	[DataRow(ArmageddonInstallCheckpoint.BeforeCommit)]
	public void ProvisionInterruptionPreservesCompletePolicyAndResumes(ArmageddonInstallCheckpoint boundary)
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings)); bindings = ProvisionSelections(db, bindings);
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var before = CapabilityMeritPolicy(db); var provision = ProvisionPolicy(db);
			var stopped = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{ if (module == ArmageddonProvisionInstaller.Module && point == boundary) throw new InvalidOperationException("Provision interruption"); });
			Assert.AreEqual(ArmageddonInstallStatus.Failed, stopped.Status); Assert.AreEqual(3, stopped.Modules.Count);
			Assert.AreEqual(before, CapabilityMeritPolicy(db)); Assert.AreEqual(provision, ProvisionPolicy(db));
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings)); Assert.AreEqual(before, CapabilityMeritPolicy(db));
		}
	}
	[TestMethod]
	public void NewProvisionCommitLostAcknowledgementCannotAdvancePartialPolicy()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings)); bindings = ProvisionSelections(db, bindings);
			var before = CapabilityMeritPolicy(db);
			var stopped = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings, (module, point) =>
			{ if (module == ArmageddonProvisionInstaller.Module && point == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("Lost provision acknowledgement"); });
			Assert.AreEqual(ArmageddonInstallStatus.CommittedConfirmationFailed, stopped.Status); Assert.AreEqual(3, stopped.Modules.Count);
			Assert.AreEqual(203, db.SeederManagedRecords.Count()); Assert.AreEqual(before, CapabilityMeritPolicy(db));
			var ids = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
			var resumed = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(resumed);
			Assert.IsTrue(resumed.Availability.All(x => x.StoredAdmissions.Count == 7));
			Assert.IsTrue(db.SeederManagedRecords.AsNoTracking().AsEnumerable().All(x => ids[x.StableKey] == x.LogicalId));
		}
	}
	[TestMethod]
	public void ProvisionAndDisabledCastingBuilderOverridesKeepStockBaselines()
	{
		var (db, bindings) = Fixture(); using (db)
		{
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings)); bindings = ProvisionSelections(db, bindings);
			Completed(ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings));
			var meal = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedProvisionContent.SustainMealKey).LogicalId!.Value;
			db.MagicSpells.Single(x => x.Id == meal).Name = "builder meal";
			var cost = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedProvisionContent.SustainMealKey + ".cost").LogicalId!.Value;
			db.TraitExpressions.Single(x => x.Id == cost).Expression = "9*grade";
			var cap = db.MagicCapabilities.Single(x => x.Name == "Armageddon partial preserver"); var xml = XElement.Parse(cap.Definition);
			xml.Element("Casting")!.SetAttributeValue("enabled", false); cap.Definition = xml.ToString(); db.SaveChanges();
			var policy = CapabilityMeritPolicy(db); var provisions = ProvisionPolicy(db);
			for (var i = 0; i < 2; i++)
			{
				var result = ArmageddonPreparedWorldInstaller.Install(Factory(db), bindings); Completed(result);
				Assert.IsFalse(result.Availability.Single(x => x.Variant == "preserver").Enabled);
				Assert.AreEqual(policy, CapabilityMeritPolicy(db)); Assert.AreEqual(provisions, ProvisionPolicy(db));
			}
		}
	}
}

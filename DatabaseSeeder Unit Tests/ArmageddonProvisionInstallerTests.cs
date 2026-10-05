#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonProvisionInstallerTests
{
	private static (FuturemudDatabaseContext Db, ArmageddonProvisionInstallPlan Plan, ArmageddonTraditionInstallPlan Traditions) Fixture()
	{
		var (db, traditions) = ArmageddonTraditionInstallerTests.Fixture(); var installed = ArmageddonTraditionInstaller.Install(db, traditions);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.StackDecorators.Add(new() { Id = 1, Name = "native bites", Type = "Bites", Definition = "<Definition><Range Min='0' Max='99' Item='partly eaten {0}'/></Definition>" });
		db.GameItemComponentProtos.Add(new() { Id = 1, Type = "Holdable", Name = "native holdable", Definition = "<Definition/>", EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } });
		db.GameItemComponentProtos.Add(new() { Id = 2, Type = "Food", Name = "native edible", Definition = "<Definition Satiation='2' Water='0' Thirst='0' Alcohol='0' Bites='4' Decorator='1'><OnEatProg>0</OnEatProg></Definition>", EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } });
		for (var i = 1; i <= 6; i++)
		{
			var proto = new GameItemProto { Id = i, Name = "native meal " + i, EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } };
			foreach (var component in new[] { 1L, 2L }) proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component });
			db.GameItemProtos.Add(proto);
		}
		db.Liquids.AddRange(new Liquid { Id = 1, Name = "native wine", AlcoholLitresPerLitre = 0.12 }, new Liquid { Id = 2, Name = "authored recipe", AlcoholLitresPerLitre = 0.08 });
		var predicate = new MudSharp.Models.FutureProg { Id = 3, FunctionName = "authored category", FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
		predicate.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() }); db.FutureProgs.Add(predicate);
		db.SaveChanges(); db.ChangeTracker.Clear();
		ArmageddonFoodPrototype[] Pool(int start) => Enumerable.Range(start, 3).Select(x => new ArmageddonFoodPrototype(x, 0)).ToArray();
		return (db, new(true, 1, 1, 1, installed.Identities[ArmageddonReviewedProvisionContent.SustainMealKey + ".skill"],
			installed.Identities[ArmageddonReviewedProvisionContent.DrawWineKey + ".skill"], [new(1, 3, Pool(4)), new(32, 0, Pool(1))], 1, [new(1, 3, 2), new(32, 0, 1)]), traditions);
	}
	private static ArmageddonInstallResult Run(FuturemudDatabaseContext db, ArmageddonProvisionInstallPlan plan) { db.ChangeTracker.Clear(); return ArmageddonProvisionInstaller.Install(db, plan); }
	private static void Completed(ArmageddonInstallResult result) => Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
	[TestMethod] public void SevenOwnedRecordsCloseHistoricalProvisionPathsWithoutPlayerRefresh()
	{
		var (db, plan, traditions) = Fixture(); using (db)
		{
			var result = Run(db, plan); Completed(result); Assert.AreEqual(7, result.Identities.Count); Assert.AreEqual(178, db.SeederManagedRecords.Count());
			var bindings = new Dictionary<string, long>(traditions.ImplementedSpells);
			foreach (var key in new[] { ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.DrawWineKey }) bindings.Add(key, result.Identities[key]);
			db.ChangeTracker.Clear(); var expanded = ArmageddonTraditionInstaller.Install(db, traditions with { ImplementedSpells = bindings });
			Assert.AreEqual(ArmageddonInstallStatus.Completed, expanded.Status, string.Join("\n", expanded.Messages)); Assert.AreEqual(6, expanded.AvailableSpells.Count); Assert.AreEqual(76, expanded.UnavailableSpells.Count);
			foreach (var cap in db.MagicCapabilities.Where(x => x.Id != 1))
			{
				var rows = XElement.Parse(cap.Definition).Element("Casting")!.Elements("Admission").ToArray(); Assert.AreEqual(6, rows.Length);
				foreach (var key in new[] { ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.DrawWineKey, ArmageddonReviewedUtilityContent.HoveringLightKey })
				{
					var row = rows.Single(x => (long)x.Attribute("spell")! == bindings[key]); Assert.AreEqual(30, (double)row.Attribute("opening")!);
					Assert.AreEqual(90, (double)row.Attribute("rawCap")!); Assert.AreEqual(80, (double)row.Element("Prerequisite")!.Attribute("proficiency")!);
				}
			}
			Assert.IsTrue(expanded.UnavailableSpells.Contains(ArmageddonReviewedUtilityContent.MendFleshKey)); Assert.AreEqual(0, db.CharacterCastingEnrolments.Count()); Assert.AreEqual(0, db.CharacterTraits.Count());
		}
	}
	[TestMethod] public void ProfilesAndRecipesUseExistingNativeSchemaAndDependencies()
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			var result = Run(db, plan); Completed(result);
			var food = XElement.Parse(db.MagicSpells.Find(result.Identities[ArmageddonReviewedProvisionContent.SustainMealKey])!.Definition);
			Assert.AreEqual("1350*grade", food.Descendants("Lifecycle").Single().Element("Seconds")!.Value);
			CollectionAssert.AreEqual(new[] { 1, 32 }, food.Descendants("Profile").Select(x => (int)x.Attribute("order")!).ToArray());
			var wine = XElement.Parse(db.MagicSpells.Find(result.Identities[ArmageddonReviewedProvisionContent.DrawWineKey])!.Definition);
			CollectionAssert.AreEqual(new[] { 2L, 1L }, wine.Descendants("Recipe").Select(x => (long)x.Attribute("liquid")!).ToArray());
			Assert.AreEqual("0.5*grade", wine.Descendants("ContainerFill").Single().Element("Litres")!.Value); Assert.AreEqual(6, db.GameItemProtos.Count()); Assert.AreEqual(2, db.Liquids.Count());
		}
	}
	[TestMethod] public void ReorderedProfileInputsDoNotChangeBaselinesOrAppliedTimes()
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			Completed(Run(db, plan)); var before = db.SeederManagedRecords.Where(x => x.Module == ArmageddonProvisionInstaller.Module).AsNoTracking().ToDictionary(x => x.StableKey, x => (x.LogicalId, x.SeedBaseline, x.AppliedAt));
			Completed(Run(db, plan with { FoodProfiles = plan.FoodProfiles.Reverse().ToArray(), WineRecipes = plan.WineRecipes.Reverse().ToArray() }));
			foreach (var row in db.SeederManagedRecords.Where(x => x.Module == ArmageddonProvisionInstaller.Module)) Assert.AreEqual(before[row.StableKey], (row.LogicalId, row.SeedBaseline, row.AppliedAt));
		}
	}
	[TestMethod] public void BuilderEditsAndUnownedClonesSurviveRepeatedReruns()
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			var result = Run(db, plan); Completed(result); var spell = db.MagicSpells.Find(result.Identities[ArmageddonReviewedProvisionContent.SustainMealKey])!;
			spell.Name = "Builder food"; var xml = XElement.Parse(spell.Definition); xml.Descendants("Profile").First().Remove(); spell.Definition = xml.ToString();
			var cost = db.TraitExpressions.Find(result.Identities[ArmageddonReviewedProvisionContent.SustainMealKey + ".cost"])!; cost.Expression = "9*grade";
			db.MagicSpells.Add(new() { Name = "Unowned food clone", Definition = spell.Definition, MagicSchoolId = spell.MagicSchoolId }); db.SaveChanges(); var definition = spell.Definition;
			for (var i = 0; i < 2; i++) Completed(Run(db, plan));
			Assert.AreEqual(definition, db.MagicSpells.Find(spell.Id)!.Definition); Assert.AreEqual("9*grade", db.TraitExpressions.Find(cost.Id)!.Expression); Assert.IsTrue(db.MagicSpells.Any(x => x.Name == "Unowned food clone"));
		}
	}
	[DataTestMethod] [DataRow(false)] [DataRow(true)] public void DeletedOrRetiredOwnedRowsBlockWholeModule(bool retire)
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			var result = Run(db, plan); Completed(result); var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedProvisionContent.DrawWineKey);
			if (retire) record.Retired = true; else db.Remove(db.MagicSpells.Find(record.LogicalId)!); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(178, db.SeederManagedRecords.Count());
		}
	}
	[DataTestMethod] [DataRow("revision")] [DataRow("bites")] [DataRow("eatprog")] [DataRow("predicate")] [DataRow("fallback")] [DataRow("wine")] [DataRow("plane")] [DataRow("sharedskill")]
	public void MalformedOrMissingSelectionsFailClosedBeforeAnyModuleWrites(string fault)
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			if (fault == "revision") plan = plan with { FoodProfiles = [new(32, 0, [new(1, 99), new(2, 0), new(3, 0)])] };
			if (fault is "bites" or "eatprog") { var component = db.GameItemComponentProtos.Find(2L, 0)!; var xml = XElement.Parse(component.Definition); if (fault == "bites") xml.SetAttributeValue("Bites", double.NaN); else xml.Element("OnEatProg")!.Value = "3"; component.Definition = xml.ToString(); }
			if (fault == "predicate") { db.FutureProgs.Find(3L)!.FunctionText = "return 1"; db.FutureProgs.Find(3L)!.ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(); }
			if (fault == "fallback") plan = plan with { WineRecipes = [new(32, 3, 1)] };
			if (fault == "wine") plan = plan with { Wine = 999 };
			if (fault == "plane") plan = plan with { WineBonusPlane = 999 };
			if (fault == "sharedskill") plan = plan with { WineSkill = plan.MealSkill };
			db.SaveChanges(); Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(0, db.SeederManagedRecords.Count(x => x.Module == ArmageddonProvisionInstaller.Module)); Assert.AreEqual(5, db.MagicSpells.Count());
		}
	}
	[TestMethod] public void UnownedStockIdentityCannotBeAdopted()
	{
		var (db, plan, _) = Fixture(); using (db)
		{
			db.MagicSpells.Add(new() { Name = "Renamed builder provision", Definition = ArmageddonReviewedProvisionContent.DrawWine(1).BuildDefinition(1, 1, 0).ToString() }); db.SaveChanges();
			Assert.AreEqual(ArmageddonInstallStatus.Blocked, Run(db, plan).Status); Assert.AreEqual(0, db.SeederManagedRecords.Count(x => x.Module == ArmageddonProvisionInstaller.Module));
		}
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.Models;
using WearProfile = MudSharp.Models.WearProfile;
using Match = System.Text.RegularExpressions.Match;

using Fixture = MudSharp_Unit_Tests.HumanWearProfileFixture;
using static MudSharp_Unit_Tests.HumanWearProfileFixture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HumanWearLayerTests
{
	[TestMethod]
	public void FreshAndIdenticalRerun_HaveExactProfileReferencesStableIdsAndNoChanges()
	{
		using var fixture = new Fixture();
		Assert.IsTrue(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.IsTrue(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		var before = fixture.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		CollectionAssert.AreEqual(before, fixture.Signatures());
		foreach (var name in ExpectedProfiles)
		{
			var profile = fixture.Context.WearProfiles.Single(x => x.Name == name);
			Assert.AreEqual(1L, profile.BodyPrototypeId);
			var component = fixture.Context.GameItemComponentProtos.Single(x => x.Name == ComponentName(name));
			Assert.AreEqual("Wearable", component.Type);
			var xml = XElement.Parse(component.Definition);
			var references = xml.Element("Profiles")!.Elements("Profile").Select(x => (long)x).ToArray();
			CollectionAssert.Contains(references, profile.Id);
			CollectionAssert.Contains(references, (long)xml.Element("Profiles")!.Attribute("Default")!);
			Assert.AreEqual(references.Length, references.Distinct().Count());
			Assert.IsTrue(references.All(id => fixture.Context.WearProfiles.Any(x => x.Id == id)));
		}
		var coat = XElement.Parse(fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Hooded_Long_Coat").Definition).Element("Profiles")!;
		Assert.AreEqual(fixture.Context.WearProfiles.Single(x => x.Name == "Hooded Long Coat Lowered").Id, (long)coat.Attribute("Default")!);
		Assert.AreEqual(2, coat.Elements("Profile").Count());
	}

	[TestMethod]
	public void Rerun_RestoresMissingComponentWithoutReplacingProfileOrOtherStock()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var profile = fixture.Context.WearProfiles.Single(x => x.Name == "Split Drawers");
		var profileId = profile.Id;
		var removed = fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Split_Drawers");
		fixture.Context.GameItemComponentProtos.Remove(removed);
		fixture.Context.SaveChanges();
		Assert.IsTrue(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.IsTrue(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		Assert.AreEqual(profileId, fixture.Context.WearProfiles.Single(x => x.Name == "Split Drawers").Id);
		Assert.AreEqual(1, fixture.Context.GameItemComponentProtos.Count(x => x.Name == "Wear_Split_Drawers"));
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
	}

	[TestMethod]
	public void Rerun_PreservesCustomizedNewProfilesAndEveryExistingComponentRevision()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var profile = fixture.Context.WearProfiles.Single(x => x.Name == "Camisole");
		profile.Description = "Builder's alternative camisole coverage";
		profile.WearlocProfiles = "<Profiles><Profile Bodypart=\"belly\" Mandatory=\"true\" Transparent=\"false\" NoArmour=\"false\" PreventsRemoval=\"false\" HidesSevered=\"false\" /></Profiles>";
		var component = fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Camisole");
		component.Description = "Builder's lightweight camisole";
		component.Definition = component.Definition.Replace("</Definition>", "<LayerWeightConsumption>0.1</LayerWeightConsumption></Definition>", StringComparison.Ordinal);
		fixture.Context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Id = component.Id, RevisionNumber = 1, Name = component.Name, Type = component.Type,
			Description = "Builder's later revision", Definition = component.Definition
		});
		fixture.Context.SaveChanges();
		var before = fixture.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		CollectionAssert.AreEqual(before, fixture.Signatures());
	}

	[TestMethod]
	public void FreshWearStock_AndBaselineUpgradeHaveTheSameCompleteDefinitions()
	{
		using var fresh = new Fixture(seedBaseline: false);
		HumanSeeder.SeedHumanWearProfilesForTesting(fresh.Context);
		using var upgrade = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(upgrade.Context);
		CollectionAssert.AreEqual(upgrade.Signatures(), fresh.Signatures());
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fresh.Context));
		var before = fresh.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fresh.Context));
		CollectionAssert.AreEqual(before, fresh.Signatures());
	}

	[TestMethod]
	public void ThicknessVariants_PreserveSourceSemanticsAndExistingConfigurations()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var components = fixture.Context.GameItemComponentProtos.ToArray();
		foreach (var stock in HumanSeeder.WearLayerStock)
		{
			var source = components.Single(x => x.Name == stock.SourceComponent);
			var variant = components.Single(x => x.Name == stock.Name);
			Assert.AreNotEqual(source.Id, variant.Id);
			var sourceXml = XElement.Parse(source.Definition);
			var variantXml = XElement.Parse(variant.Definition);
			Assert.AreEqual(stock.LayerWeight, (double)variantXml.Element("LayerWeightConsumption")!, stock.Name);
			if (stock.Bulky is { } bulky)
			{
				Assert.AreEqual(bulky, (bool)variantXml.Attribute("Bulky")!, stock.Name);
				sourceXml.SetAttributeValue("Bulky", bulky);
			}
			sourceXml.Elements("LayerWeightConsumption").Remove();
			variantXml.Elements("LayerWeightConsumption").Remove();
			Assert.IsTrue(XNode.DeepEquals(sourceXml, variantXml), stock.Name);
		}
		var target = components.Single(x => x.Name == "Wear_Camisole_Layer_0_25");
		target.Description = "Builder's custom layering";
		target.Definition = target.Definition.Replace("0.25", "0.3", StringComparison.Ordinal);
		fixture.Context.GameItemComponentProtos.Add(new GameItemComponentProto
		{
			Id = target.Id, RevisionNumber = 1, Name = target.Name, Type = target.Type,
			Description = "Builder's later revision", Definition = target.Definition
		});
		fixture.Context.SaveChanges();
		var before = fixture.Signatures();
		Assert.IsFalse(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		CollectionAssert.AreEqual(before, fixture.Signatures());
		fixture.Context.GameItemComponentProtos.Remove(components.Single(x => x.Name == "Wear_Split_Drawers_Layer_0_25"));
		fixture.Context.SaveChanges();
		Assert.IsTrue(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.IsTrue(HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context));
		Assert.IsFalse(HumanSeeder.HasMissingHumanWearProfilesForTesting(fixture.Context));
		Assert.AreEqual("Builder's custom layering", target.Description);
	}

	[TestMethod]
	public void MissingThicknessBatch_BadSourceFailsWithoutPartialVariantCreation()
	{
		using var fixture = new Fixture();
		HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context);
		var names = HumanSeeder.WearLayerStock.Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
		fixture.Context.GameItemComponentProtos.RemoveRange(fixture.Context.GameItemComponentProtos.AsEnumerable().Where(x => names.Contains(x.Name)));
		fixture.Context.GameItemComponentProtos.Single(x => x.Name == "Wear_Vest").Type = "Holdable";
		fixture.Context.SaveChanges();
		var before = fixture.Signatures();
		StringAssert.Contains(Assert.ThrowsExactly<InvalidDataException>(() => HumanSeeder.RefreshHumanWearProfilesForTesting(fixture.Context)).Message, "Wear_Vest");
		CollectionAssert.AreEqual(before, fixture.Signatures());
		Assert.AreEqual(0, fixture.Context.ChangeTracker.Entries().Count(x => x.State is EntityState.Added or EntityState.Modified));
	}
}

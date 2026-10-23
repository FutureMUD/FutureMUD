#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederDecimalOwnershipTests
{
	[DataTestMethod]
	[DataRow("6.0", "6.0000000000000000000000000000")]
	[DataRow("8.0", "8.000000000000000000000000000")]
	[DataRow("24", "24.000000000000000000000000000")]
	[DataRow("0.00", "0.0000000000000000000000000000")]
	[DataRow("0.159", "0.1590000000000000000000000000")]
	[DataRow("0.0000000000000000000000000001", "0.0000000000000000000000000001")]
	[DataRow("79228162514264337593543950335", "79228162514264337593543950335")]
	public void ProviderCostScale_RetainsCompleteStockOwnership(string authored, string persisted)
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var definition = RepresentativeCatalogue.Braies() with { Cost = Price(authored) };
		var item = new ItemSeeder().InstallCatalogueItemForTesting(context, definition);
		context.SaveChanges();
		var record = Owner(context);
		var fingerprint = record.AppliedFingerprint;
		item.CostInBaseCurrency = Price(persisted);
		context.SaveChanges();
		var updated = definition with { WeightInGrams = 161 };
		new ItemSeeder().InstallCatalogueItemForTesting(context, updated);
		context.SaveChanges();
		Assert.AreEqual(161.0, item.Weight, "A representation-only provider change must not hide untouched stock from updates.");
		Assert.AreNotEqual(fingerprint, record.AppliedFingerprint);
		var updatedFingerprint = record.AppliedFingerprint;
		item.CostInBaseCurrency = Price(persisted);
		new ItemSeeder().InstallCatalogueItemForTesting(context, updated);
		context.SaveChanges();
		Assert.AreEqual(updatedFingerprint, record.AppliedFingerprint);
		Assert.AreEqual(item.Id, record.LogicalId);
	}

	[TestMethod]
	public void ProviderCostScale_ProvesOlderStockBeforeApplyingNewAuthoredPrice()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var original = RepresentativeCatalogue.Braies() with { Cost = 6.0m };
		var item = new ItemSeeder().InstallCatalogueItemForTesting(context, original);
		context.SaveChanges();
		item.CostInBaseCurrency = Price("6.0000000000000000000000000000");
		context.SaveChanges();
		new ItemSeeder().InstallCatalogueItemForTesting(context, original with { Cost = 7.00m });
		context.SaveChanges();
		Assert.AreEqual(7.00m, item.CostInBaseCurrency);
		Assert.AreEqual(item.Id, Owner(context).LogicalId);
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void ProviderCostScale_DoesNotHideNumericPriceOrDescriptionCustomisation(bool changePrice)
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var definition = RepresentativeCatalogue.Braies() with { Cost = 6.0m };
		var item = new ItemSeeder().InstallCatalogueItemForTesting(context, definition);
		context.SaveChanges();
		var fingerprint = Owner(context).AppliedFingerprint;
		item.CostInBaseCurrency = changePrice ? 6.01m : Price("6.0000000000000000000000000000");
		if (!changePrice) item.FullDescription = "Builder's garment description";
		context.SaveChanges();
		new ItemSeeder().InstallCatalogueItemForTesting(context, definition with { WeightInGrams = 161, Cost = 7.00m });
		context.SaveChanges();
		Assert.AreEqual(160.0, item.Weight);
		Assert.AreEqual(changePrice ? 6.01m : 6.0m, item.CostInBaseCurrency);
		if (!changePrice) Assert.AreEqual("Builder's garment description", item.FullDescription);
		Assert.AreEqual(fingerprint, Owner(context).AppliedFingerprint);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ProviderCostScale_AllowsOnlyCompleteUniqueUnmanagedAdoption(bool legacyName)
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var definition = RepresentativeCatalogue.Braies() with { Cost = 6.0m, AllowLegacyShortDescriptionMatch = legacyName };
		var item = new ItemSeeder().InstallCatalogueItemForTesting(context, definition);
		context.SaveChanges();
		var fingerprint = Owner(context).AppliedFingerprint;
		context.SeederManagedRecords.Remove(Owner(context));
		if (legacyName) item.UniqueName = null;
		item.CostInBaseCurrency = Price("6.0000000000000000000000000000");
		context.SaveChanges();
		new ItemSeeder().InstallCatalogueItemForTesting(context, definition);
		context.SaveChanges();
		Assert.AreEqual(item.Id, Owner(context).LogicalId);
		Assert.AreEqual(fingerprint, Owner(context).AppliedFingerprint);
		Assert.AreEqual(1, context.GameItemProtos.Count());
	}

	[TestMethod]
	public void ProviderCostScale_DoesNotPermitUnmanagedPriceDrift()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var definition = RepresentativeCatalogue.Braies() with { Cost = 6.0m };
		var item = new ItemSeeder().InstallCatalogueItemForTesting(context, definition);
		context.SaveChanges();
		context.SeederManagedRecords.Remove(Owner(context));
		item.CostInBaseCurrency = 6.01m;
		context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => new ItemSeeder().InstallCatalogueItemForTesting(context, definition));
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
		Assert.AreEqual(6.01m, item.CostInBaseCurrency);
	}

	[TestMethod]
	public void ProviderCostScale_PreservesTheExistingMissingLinkRepairBoundary()
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var definition = RepresentativeCatalogue.Braies() with { Cost = 6.0m };
		var item = new ItemSeeder().InstallCatalogueItemForTesting(context, definition);
		context.SaveChanges();
		var fingerprint = Owner(context).AppliedFingerprint;
		var link = item.GameItemProtosTags.First();
		var tagId = link.TagId;
		context.GameItemProtosTags.Remove(link);
		item.GameItemProtosTags.Remove(link);
		item.CostInBaseCurrency = Price("6.0000000000000000000000000000");
		context.SaveChanges();
		new ItemSeeder().InstallCatalogueItemForTesting(context, definition);
		context.SaveChanges();
		Assert.IsTrue(item.GameItemProtosTags.Any(x => x.TagId == tagId));
		Assert.AreEqual(fingerprint, Owner(context).AppliedFingerprint);
	}

	private static decimal Price(string value) => decimal.Parse(value, CultureInfo.InvariantCulture);
	private static SeederManagedRecord Owner(FuturemudDatabaseContext context) =>
		context.SeederManagedRecords.Single(x => x.EntityType == "item");
}

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPI_Engine_Worldfile_Converter;

namespace RPI_Engine_Worldfile_Converter_Tests;

[TestClass]
public class RpiShopConversionTests
{
	[TestMethod]
	public void ShopTransformer_MapsRpiLocationsAndDeliveryMerchandise()
	{
		var parser = new RpiNpcWorldfileParser();
		var corpus = parser.ParseDirectory(GetNpcFixtureDirectory());
		var transformer = new FutureMudShopTransformer(
		[
			BuildConvertedItem(300, RPIItemType.Light, "a brass lamp", 100.0M)
		]);

		var conversion = transformer.Convert(corpus.Npcs);
		var shop = conversion.Shops.Single();

		Assert.AreEqual(1002, shop.KeeperVnum);
		Assert.AreEqual(ShopConversionStatus.Ready, shop.Status);
		Assert.AreEqual(100, shop.ShopVnum);
		Assert.AreEqual(200, shop.StoreVnum);
		Assert.AreEqual("RPI Shop 100 - Careful Shopkeeper", shop.ShopName);
		Assert.AreEqual(1, shop.Merchandise.Count);

		var merchandise = shop.Merchandise.Single();
		Assert.AreEqual(300, merchandise.DeliveryVnum);
		Assert.AreEqual(RPIItemType.Light, merchandise.SourceItemType);
		Assert.AreEqual(120.0M, merchandise.BasePrice);
		Assert.AreEqual(80.0M, merchandise.AutoReorderPrice);
		Assert.AreEqual(0.6667M, merchandise.BaseBuyModifier);
		Assert.IsTrue(merchandise.WillBuy);
		Assert.IsTrue(shop.Warnings.Any(x => x.Code == "legacy-trades-in-retained"));
		Assert.IsTrue(shop.Warnings.Any(x => x.Code == "shopkeeper-ai-deferred"));
	}

	[TestMethod]
	public void ShopValidation_RequiresImportedRoomsAndWarnsForMissingItemPrototype()
	{
		var parser = new RpiNpcWorldfileParser();
		var corpus = parser.ParseDirectory(GetNpcFixtureDirectory());
		var conversion = new FutureMudShopTransformer(
		[
			BuildConvertedItem(300, RPIItemType.Light, "a brass lamp", 100.0M)
		]).Convert(corpus.Npcs);
		var baseline = BuildBaseline(
			cellIds: new HashSet<long> { 100 },
			itemProtosByVnum: new Dictionary<int, IReadOnlyList<FutureMudShopItemProtoReference>>());

		var issues = FutureMudShopValidation.Validate(baseline, conversion.Shops);

		Assert.IsTrue(issues.Any(x =>
			x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase) &&
			x.Message.Contains("stockroom cell id 200", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(issues.Any(x =>
			x.Severity.Equals("warning", StringComparison.OrdinalIgnoreCase) &&
			x.Message.Contains("Delivery vnum 300", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void ShopValidation_WarnsForExistingStructuralShop()
	{
		var parser = new RpiNpcWorldfileParser();
		var corpus = parser.ParseDirectory(GetNpcFixtureDirectory());
		var conversion = new FutureMudShopTransformer(
		[
			BuildConvertedItem(300, RPIItemType.Light, "a brass lamp", 100.0M)
		]).Convert(corpus.Npcs);
		var baseline = BuildBaseline(
			cellIds: new HashSet<long> { 100, 200 },
			itemProtosByVnum: new Dictionary<int, IReadOnlyList<FutureMudShopItemProtoReference>>
			{
				[300] = [new FutureMudShopItemProtoReference(5000, 300, "RPIIMPORT|objs.1|300|Light|", "lamp", "a brass lamp", 100.0M, RPIItemType.Light)]
			},
			existingShopKeys: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { FutureMudShopBaselineCatalog.BuildShopKey(100, 200) });

		var issues = FutureMudShopValidation.Validate(baseline, conversion.Shops);

		Assert.IsFalse(issues.Any(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(issues.Any(x =>
			x.Severity.Equals("warning", StringComparison.OrdinalIgnoreCase) &&
			x.Message.Contains("existing FutureMUD shop", StringComparison.OrdinalIgnoreCase)));
	}

	private static ConvertedItemDefinition BuildConvertedItem(int vnum, RPIItemType itemType, string shortDescription, decimal cost)
	{
		return new ConvertedItemDefinition
		{
			Vnum = vnum,
			SourceFile = "objs.1",
			Zone = 1,
			SourceKey = $"objs.1#{vnum}",
			SourceItemType = itemType,
			Status = ConversionStatus.FunctionalImport,
			BaseName = shortDescription,
			Keywords = "lamp",
			ShortDescription = shortDescription,
			LongDescription = $"{shortDescription} is here.",
			FullDescription = $"{shortDescription}.",
			MaterialName = "bronze",
			BaseItemQuality = 3,
			Size = 3,
			WeightGrams = 100.0,
			CostInBaseCurrency = cost,
			RawOvals = new[] { 0, 0, 0, 0 },
		};
	}

	private static FutureMudShopBaselineCatalog BuildBaseline(
		IReadOnlySet<long> cellIds,
		IReadOnlyDictionary<int, IReadOnlyList<FutureMudShopItemProtoReference>> itemProtosByVnum,
		IReadOnlySet<string>? existingShopKeys = null)
	{
		return new FutureMudShopBaselineCatalog
		{
			DefaultEconomicZone = new FutureMudEconomicZoneReference(1, "Default Economy", 10),
			CellIds = cellIds,
			ItemProtosByLegacyVnum = itemProtosByVnum,
			ExistingShopKeys = existingShopKeys ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase),
			ExistingShopNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
		};
	}

	private static string GetNpcFixtureDirectory()
	{
		var candidates = new[]
		{
			Path.Combine(AppContext.BaseDirectory, "Fixtures", "Npcs"),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Fixtures", "Npcs")),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter Tests", "Fixtures", "Npcs"))
		};

		return candidates.First(x => File.Exists(Path.Combine(x, "mobs.1")));
	}
}

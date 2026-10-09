#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
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
			x.Message.Contains("stockroom room for RPI vnum 200", StringComparison.OrdinalIgnoreCase)));
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

	[TestMethod]
	public void ShopImporter_FallbackMapping_BuildsModelWithActualRoomIds()
	{
		var mappings = FutureMudShopRoomMapping.FromAudit(BuildRoomAudit(RoomEntries()), SourceRooms());
		var baseline = BuildBaseline(new HashSet<long> { 100, 200, 9001, 9002 },
			new Dictionary<int, IReadOnlyList<FutureMudShopItemProtoReference>>(), roomMappings: mappings);
		var definition = new FutureMudShopTransformer().Convert(
			new RpiNpcWorldfileParser().ParseDirectory(GetNpcFixtureDirectory()).Npcs).Shops.Single();
		Assert.IsFalse(FutureMudShopValidation.Validate(baseline, [definition]).Any(x => x.Severity == "error"));
		var importer = new FutureMudShopImporter(null!, baseline);
		var shop = (MudSharp.Models.Shop)typeof(FutureMudShopImporter)
			.GetMethod("BuildShop", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(importer, [definition, Array.Empty<(ConvertedShopMerchandiseDefinition, FutureMudShopItemProtoReference)>()])!;
		Assert.AreEqual(9001L, shop.ShopsStoreroomRooms.Single().RoomId);
		Assert.AreEqual(9002L, shop.StockroomId);
		Assert.AreEqual("9001|9002", baseline.ResolvedShopKey(definition));
	}

	[DataTestMethod]
	[DataRow("missing")]
	[DataRow("source-mismatch")]
	[DataRow("not-created")]
	[DataRow("null-id")]
	[DataRow("duplicate-vnum")]
	[DataRow("duplicate-target")]
	public void ShopRoomMapping_UnverifiedAuditEntries_RejectsBinding(string kind)
	{
		var entries = RoomEntries().ToList();
		switch (kind)
		{
			case "missing": entries.RemoveAt(0); break;
			case "source-mismatch": entries[0] = entries[0] with { SourceKey = "rooms.9#100" }; break;
			case "not-created": entries[0] = entries[0] with { Action = "skipped-existing" }; break;
			case "null-id": entries[0] = entries[0] with { RoomId = null }; break;
			case "duplicate-vnum": entries.Add(entries[0] with { RoomId = 9100 }); break;
			case "duplicate-target": entries[1] = entries[1] with { RoomId = 9001 }; break;
		}
		var mappings = FutureMudShopRoomMapping.FromAudit(BuildRoomAudit(entries), SourceRooms());
		Assert.IsFalse(mappings.ContainsKey(100));
	}

	[TestMethod]
	public void ShopImporter_RawExistingIdsWithoutLiveMapping_SkipsBeforePersistence()
	{
		var definition = new FutureMudShopTransformer().Convert(
			new RpiNpcWorldfileParser().ParseDirectory(GetNpcFixtureDirectory()).Npcs).Shops.Single();
		foreach (var mappings in new[] { new Dictionary<int, long>(), new Dictionary<int, long> { [100] = 9001, [200] = 9002 } })
		{
			var baseline = BuildBaseline(new HashSet<long> { 100, 200 },
				new Dictionary<int, IReadOnlyList<FutureMudShopItemProtoReference>>(), roomMappings: mappings);
			var result = new FutureMudShopImporter(null!, baseline).Apply([definition], execute: true);
			Assert.AreEqual(1, result.SkippedInvalidCount);
			Assert.AreEqual(0, result.InsertedCount);
			Assert.AreEqual("skipped-invalid", result.Audit.Shops.Single().Action);
		}
	}

	[TestMethod]
	public void ShopRoomMapping_AmbiguousSourceCorpus_RejectsBinding()
	{
		var rooms = SourceRooms();
		var mappings = FutureMudShopRoomMapping.FromAudit(BuildRoomAudit(RoomEntries()),
			rooms.Append(rooms[0] with { SourceKey = "rooms.2#100" }));
		Assert.IsFalse(mappings.ContainsKey(100));
	}

	[TestMethod]
	public void ShopRoomMapping_HistoricalAudit_UsesCellIdInsteadOfRemovedParent()
	{
		var audit = JsonSerializer.SerializeToElement(new { Execute = true, Rooms = new[]
		{
			new { SourceKey = "rooms.1#100", Vnum = 100, Action = "created", RoomId = 77, CellId = 9001 }
		} });
		Assert.AreEqual(9001L, FutureMudShopRoomMapping.FromAudit(audit, SourceRooms())[100]);
	}

	[TestMethod]
	public void ShopRoomMapping_DryRunOrUnversionedAudit_RejectsBinding()
	{
		Assert.ThrowsException<InvalidDataException>(() => FutureMudShopRoomMapping.FromAudit(
			JsonSerializer.SerializeToElement(new { Execute = false, Rooms = RoomEntries() }), SourceRooms()));
		var unknown = JsonSerializer.SerializeToElement(new { Execute = true, Rooms = new[]
		{
			new { SourceKey = "rooms.1#100", Vnum = 100, Action = "created", RoomId = 77 }
		} });
		Assert.AreEqual(0, FutureMudShopRoomMapping.FromAudit(unknown, SourceRooms()).Count);
	}

	private static JsonElement BuildRoomAudit(IEnumerable<RoomApplyAuditRoomEntry> rooms) =>
		JsonSerializer.SerializeToElement(new { Execute = true, Rooms = rooms });

	private static RoomApplyAuditRoomEntry[] RoomEntries() =>
	[
		new("rooms.1#100", 100, "zone-1", "created", 1, null, 9001, 11),
		new("rooms.1#200", 200, "zone-1", "created", 1, null, 9002, 12)
	];

	private static IReadOnlyList<ConvertedRoomDefinition> SourceRooms() => new FutureMudRoomTransformer().Convert(
		new[] { 100, 200 }.Select(vnum => new RpiRoomRecord
		{
			Vnum = vnum, SourceFile = "rooms.1", Zone = 1, Name = "Imported Room", Description = "An imported room.",
			RawFlags = 0, RoomFlags = RpiRoomFlags.None, RawSectorType = (int)RpiRoomSectorType.Inside,
			SectorType = RpiRoomSectorType.Inside, Deity = 0
		})).Rooms;

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
		IReadOnlySet<string>? existingShopKeys = null,
		IReadOnlyDictionary<int, long>? roomMappings = null)
	{
		return new FutureMudShopBaselineCatalog
		{
			DefaultEconomicZone = new FutureMudEconomicZoneReference(1, "Default Economy", 10),
			RoomIds = cellIds,
			ImportedRoomIdsByVnum = roomMappings ?? cellIds.ToDictionary(x => checked((int)x), x => x),
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

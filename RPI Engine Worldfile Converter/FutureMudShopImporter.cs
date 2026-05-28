#nullable enable

using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using SystemCultureInfo = System.Globalization.CultureInfo;

namespace RPI_Engine_Worldfile_Converter;

public enum ShopConversionStatus
{
	Ready,
	Invalid,
}

public sealed record ShopConversionWarning(string Code, string Message);

public sealed record ConvertedShopMerchandiseDefinition(
	int DeliveryVnum,
	string SourceItemKey,
	RPIItemType SourceItemType,
	string MerchandiseName,
	string ListDescription,
	decimal BasePrice,
	decimal AutoReorderPrice,
	decimal BaseBuyModifier,
	bool WillBuy,
	IReadOnlyList<ShopConversionWarning> Warnings);

public sealed record ConvertedShopDefinition
{
	public required int KeeperVnum { get; init; }
	public required string SourceFile { get; init; }
	public required int Zone { get; init; }
	public required string SourceKey { get; init; }
	public required string ShopName { get; init; }
	public required ShopConversionStatus Status { get; init; }
	public required int ShopVnum { get; init; }
	public required int StoreVnum { get; init; }
	public required double Markup { get; init; }
	public required double Discount { get; init; }
	public required IReadOnlyList<RpiNpcShopEconomyProfile> EconomyProfiles { get; init; }
	public required int NoBuyFlags { get; init; }
	public required IReadOnlyList<string> AdditionalEconomyValues { get; init; }
	public required IReadOnlyList<int> DeliveryVnums { get; init; }
	public required IReadOnlyList<int> TradesIn { get; init; }
	public required IReadOnlyList<ConvertedShopMerchandiseDefinition> Merchandise { get; init; }
	public required IReadOnlyList<ShopConversionWarning> Warnings { get; init; }

	public string StructuralKey => FutureMudShopBaselineCatalog.BuildShopKey(ShopVnum, StoreVnum);
}

public sealed record ShopConversionResult(IReadOnlyList<ConvertedShopDefinition> Shops);

public sealed record ShopAnalysisSummary(
	int TotalMobCount,
	int ParsedMobCount,
	int FailureCount,
	int ParseWarningCount,
	int ShopCount,
	int ReadyShopCount,
	int InvalidShopCount,
	int MerchandiseCount,
	string BaselineStatus,
	IReadOnlyDictionary<string, int> StatusCounts,
	IReadOnlyDictionary<string, int> WarningCodeCounts,
	IReadOnlyDictionary<string, int> MissingDependencyCounts);

public sealed record FutureMudShopValidationIssue(string SourceKey, string Severity, string Message);

public sealed record FutureMudEconomicZoneReference(long Id, string Name, long CurrencyId);

public sealed record FutureMudShopItemProtoReference(
	long Id,
	int LegacyVnum,
	string Marker,
	string Name,
	string ShortDescription,
	decimal CostInBaseCurrency,
	RPIItemType SourceItemType);

public sealed record ShopApplyAuditEntry(
	string SourceKey,
	int KeeperVnum,
	int ShopVnum,
	int StoreVnum,
	string Action,
	long? ShopId,
	int MerchandiseCount,
	IReadOnlyList<int> SkippedDeliveryVnums);

public sealed record ShopApplyAuditReport(
	DateTime GeneratedUtc,
	bool Execute,
	IReadOnlyList<ShopApplyAuditEntry> Shops);

public sealed record FutureMudShopImportResult(
	int InsertedCount,
	int InsertedMerchandiseCount,
	int SkippedExistingCount,
	int SkippedInvalidCount,
	IReadOnlyList<FutureMudShopValidationIssue> Issues,
	ShopApplyAuditReport Audit);

public sealed class FutureMudShopTransformer
{
	private static readonly Regex LegacyColourRegex = new(@"#[0-9A-Za-z]", RegexOptions.Compiled);
	private static readonly Regex NonNameTextRegex = new(@"[^a-z0-9 '\-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private readonly IReadOnlyDictionary<int, ConvertedItemDefinition> _itemsByVnum;
	private readonly IReadOnlyDictionary<int, int> _duplicateItemVnumCounts;
	private readonly IReadOnlySet<int> _knownRoomVnums;

	public FutureMudShopTransformer(
		IEnumerable<ConvertedItemDefinition>? convertedItems = null,
		IEnumerable<ConvertedRoomDefinition>? convertedRooms = null)
	{
		var importableItems = (convertedItems ?? Array.Empty<ConvertedItemDefinition>())
			.Where(x => x.Status != ConversionStatus.SkippedImport)
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.Vnum)
			.ThenBy(x => x.SourceKey, StringComparer.OrdinalIgnoreCase)
			.ToList();

		_itemsByVnum = importableItems
			.GroupBy(x => x.Vnum)
			.ToDictionary(
				x => x.Key,
				x => x.First());
		_duplicateItemVnumCounts = importableItems
			.GroupBy(x => x.Vnum)
			.Where(x => x.Count() > 1)
			.ToDictionary(x => x.Key, x => x.Count());
		_knownRoomVnums = (convertedRooms ?? Array.Empty<ConvertedRoomDefinition>())
			.Select(x => x.Vnum)
			.ToHashSet();
	}

	public ShopConversionResult Convert(IEnumerable<RpiNpcRecord> npcs)
	{
		var shops = npcs
			.Where(x => x.Shop is not null)
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.Vnum)
			.Select(ConvertShop)
			.ToList();

		return new ShopConversionResult(shops);
	}

	private ConvertedShopDefinition ConvertShop(RpiNpcRecord npc)
	{
		var sourceShop = npc.Shop!;
		List<ShopConversionWarning> warnings = [];
		var status = ShopConversionStatus.Ready;

		if (sourceShop.ShopVnum <= 0)
		{
			status = ShopConversionStatus.Invalid;
			warnings.Add(new ShopConversionWarning(
				"invalid-shop-vnum",
				"RPI shopkeeper has no usable shop_vnum; FutureMUD cannot place the shopfront."));
		}
		else if (_knownRoomVnums.Count > 0 && !_knownRoomVnums.Contains(sourceShop.ShopVnum))
		{
			warnings.Add(new ShopConversionWarning(
				"missing-source-shopfront-room",
				$"RPI shop_vnum {sourceShop.ShopVnum.ToString(SystemCultureInfo.InvariantCulture)} was not present in the parsed room corpus."));
		}

		if (sourceShop.StoreVnum <= 0)
		{
			warnings.Add(new ShopConversionWarning(
				"missing-stockroom-vnum",
				"RPI shopkeeper has no usable store_vnum; the FutureMUD shop can be created, but imported stockroom behavior will need builder follow-up."));
		}
		else if (_knownRoomVnums.Count > 0 && !_knownRoomVnums.Contains(sourceShop.StoreVnum))
		{
			warnings.Add(new ShopConversionWarning(
				"missing-source-stockroom-room",
				$"RPI store_vnum {sourceShop.StoreVnum.ToString(SystemCultureInfo.InvariantCulture)} was not present in the parsed room corpus."));
		}

		if (sourceShop.TradesIn.Count > 0)
		{
			warnings.Add(new ShopConversionWarning(
				"legacy-trades-in-retained",
				"RPI trades_in values are legacy item-type categories; v1 only turns them into buyback on delivered item prototypes of the same type."));
		}

		if (sourceShop.EconomyProfiles.Count > 1 ||
		    sourceShop.NoBuyFlags != 0 ||
		    sourceShop.AdditionalEconomyValues.Any(x => x != "0"))
		{
			warnings.Add(new ShopConversionWarning(
				"legacy-economy-profiles-retained",
				"RPI economy flag profiles and nobuy flags are preserved in export data, but v1 imports fixed merchandise prices."));
		}

		warnings.Add(new ShopConversionWarning(
			"shopkeeper-ai-deferred",
			"FutureMUD shop objects are imported; live shopkeeper employment/AI attachment remains a later pass."));

		var tradesIn = sourceShop.TradesIn
			.Where(x => x >= 0)
			.ToHashSet();
		var merchandise = sourceShop.DeliveryVnums
			.Where(x => x > 0)
			.Distinct()
			.OrderBy(x => x)
			.Select(x => ConvertMerchandise(sourceShop, x, tradesIn, warnings))
			.Where(x => x is not null)
			.Select(x => x!)
			.ToList();

		foreach (var invalidDelivery in sourceShop.DeliveryVnums.Where(x => x <= 0).Distinct().OrderBy(x => x))
		{
			warnings.Add(new ShopConversionWarning(
				"invalid-delivery-vnum",
				$"Delivery vnum {invalidDelivery.ToString(SystemCultureInfo.InvariantCulture)} is not importable as merchandise."));
		}

		return new ConvertedShopDefinition
		{
			KeeperVnum = npc.Vnum,
			SourceFile = npc.SourceFile,
			Zone = npc.Zone,
			SourceKey = npc.SourceKey,
			ShopName = BuildShopName(npc, sourceShop),
			Status = status,
			ShopVnum = sourceShop.ShopVnum,
			StoreVnum = sourceShop.StoreVnum,
			Markup = sourceShop.Markup,
			Discount = sourceShop.Discount,
			EconomyProfiles = sourceShop.EconomyProfiles,
			NoBuyFlags = sourceShop.NoBuyFlags,
			AdditionalEconomyValues = sourceShop.AdditionalEconomyValues,
			DeliveryVnums = sourceShop.DeliveryVnums,
			TradesIn = sourceShop.TradesIn,
			Merchandise = merchandise,
			Warnings = warnings,
		};
	}

	private ConvertedShopMerchandiseDefinition? ConvertMerchandise(
		RpiNpcShopRecord shop,
		int deliveryVnum,
		IReadOnlySet<int> tradesIn,
		ICollection<ShopConversionWarning> shopWarnings)
	{
		if (!_itemsByVnum.TryGetValue(deliveryVnum, out var item))
		{
			shopWarnings.Add(new ShopConversionWarning(
				"unmapped-delivery-item",
				$"Delivery vnum {deliveryVnum.ToString(SystemCultureInfo.InvariantCulture)} did not resolve to an importable item prototype."));
			return null;
		}

		List<ShopConversionWarning> merchandiseWarnings = [];
		if (_duplicateItemVnumCounts.TryGetValue(deliveryVnum, out var duplicateCount))
		{
			var warning = new ShopConversionWarning(
				"ambiguous-delivery-item-vnum",
				$"Delivery vnum {deliveryVnum.ToString(SystemCultureInfo.InvariantCulture)} matched {duplicateCount.ToString(SystemCultureInfo.InvariantCulture)} converted items; the first source item was selected.");
			shopWarnings.Add(warning);
			merchandiseWarnings.Add(warning);
		}

		var basePrice = RoundMoney(item.CostInBaseCurrency * (decimal)shop.Markup);
		var autoReorderPrice = RoundMoney(item.CostInBaseCurrency * (decimal)shop.Discount);
		var baseBuyModifier = CalculateBaseBuyModifier(shop, basePrice, autoReorderPrice);
		var willBuy = tradesIn.Contains((int)item.SourceItemType);
		if (willBuy)
		{
			merchandiseWarnings.Add(new ShopConversionWarning(
				"legacy-trades-in-mapped",
				$"RPI trades_in includes item type {(int)item.SourceItemType} ({item.SourceItemType}); this merchandise will be buyable by the shop."));
		}

		return new ConvertedShopMerchandiseDefinition(
			deliveryVnum,
			item.SourceKey,
			item.SourceItemType,
			BuildMerchandiseName(item),
			item.ShortDescription,
			basePrice,
			autoReorderPrice,
			baseBuyModifier,
			willBuy,
			merchandiseWarnings);
	}

	private static decimal CalculateBaseBuyModifier(RpiNpcShopRecord shop, decimal basePrice, decimal autoReorderPrice)
	{
		if (basePrice > 0.0M && autoReorderPrice > 0.0M)
		{
			return decimal.Round(autoReorderPrice / basePrice, 4, MidpointRounding.AwayFromZero);
		}

		if (shop.Markup > 0.0 && shop.Discount > 0.0)
		{
			return decimal.Round((decimal)(shop.Discount / shop.Markup), 4, MidpointRounding.AwayFromZero);
		}

		return 0.3M;
	}

	private static decimal RoundMoney(decimal value)
	{
		return decimal.Round(Math.Max(value, 0.0M), 2, MidpointRounding.AwayFromZero);
	}

	private static string BuildShopName(RpiNpcRecord npc, RpiNpcShopRecord shop)
	{
		var nameSeed = CleanNameText(npc.ShortDescription);
		if (string.IsNullOrWhiteSpace(nameSeed))
		{
			nameSeed = $"keeper {npc.Vnum.ToString(SystemCultureInfo.InvariantCulture)}";
		}

		var title = SystemCultureInfo.InvariantCulture.TextInfo.ToTitleCase(nameSeed);
		return Truncate($"RPI Shop {shop.ShopVnum.ToString(SystemCultureInfo.InvariantCulture)} - {title}", 100);
	}

	private static string BuildMerchandiseName(ConvertedItemDefinition item)
	{
		var nameSeed = CleanNameText(item.ShortDescription);
		return string.IsNullOrWhiteSpace(nameSeed)
			? $"RPI item {item.Vnum.ToString(SystemCultureInfo.InvariantCulture)}"
			: Truncate(SystemCultureInfo.InvariantCulture.TextInfo.ToTitleCase(nameSeed), 100);
	}

	private static string CleanNameText(string text)
	{
		var cleaned = LegacyColourRegex.Replace(text, string.Empty)
			.Replace('~', ' ')
			.Trim()
			.ToLowerInvariant();

		foreach (var article in new[] { "a ", "an ", "the " })
		{
			if (cleaned.StartsWith(article, StringComparison.OrdinalIgnoreCase))
			{
				cleaned = cleaned[article.Length..];
				break;
			}
		}

		cleaned = NonNameTextRegex.Replace(cleaned, " ");
		return Regex.Replace(cleaned, @"\s+", " ").Trim();
	}

	private static string Truncate(string text, int length)
	{
		return text.Length <= length ? text : text[..length].TrimEnd();
	}
}

public sealed class FutureMudShopBaselineCatalog
{
	private static readonly Regex ItemImportMarkerRegex = new(
		@"^RPIIMPORT\|(?<file>[^|]+)\|(?<vnum>-?\d+)\|(?<type>[^|]+)\|",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	public required FutureMudEconomicZoneReference? DefaultEconomicZone { get; init; }
	public required IReadOnlySet<long> CellIds { get; init; }
	public required IReadOnlyDictionary<int, IReadOnlyList<FutureMudShopItemProtoReference>> ItemProtosByLegacyVnum { get; init; }
	public required IReadOnlySet<string> ExistingShopKeys { get; init; }
	public required IReadOnlySet<string> ExistingShopNames { get; init; }

	public static FutureMudShopBaselineCatalog Load(FuturemudDatabaseContext context)
	{
		var economicZone = context.EconomicZones
			.OrderBy(x => x.Id)
			.Select(x => new FutureMudEconomicZoneReference(x.Id, x.Name, x.CurrencyId))
			.FirstOrDefault();
		var cellIds = context.Cells
			.Select(x => x.Id)
			.ToHashSet();
		var itemProtos = LoadImportedItemPrototypes(context);
		var shops = context.Shops
			.Include(x => x.ShopsStoreroomCells)
			.ToList();
		var existingShopKeys = shops
			.SelectMany(shop => shop.ShopsStoreroomCells
				.Select(cell => BuildShopKey(cell.CellId, shop.StockroomCellId ?? 0L)))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var existingShopNames = shops
			.Select(x => x.Name)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		return new FutureMudShopBaselineCatalog
		{
			DefaultEconomicZone = economicZone,
			CellIds = cellIds,
			ItemProtosByLegacyVnum = itemProtos
				.GroupBy(x => x.LegacyVnum)
				.ToDictionary(
					x => x.Key,
					x => (IReadOnlyList<FutureMudShopItemProtoReference>)x
						.OrderBy(y => y.Id)
						.ToList()),
			ExistingShopKeys = existingShopKeys,
			ExistingShopNames = existingShopNames,
		};
	}

	public static string BuildShopKey(long shopVnum, long storeVnum)
	{
		return $"{shopVnum.ToString(SystemCultureInfo.InvariantCulture)}|{storeVnum.ToString(SystemCultureInfo.InvariantCulture)}";
	}

	public bool HasExistingShop(ConvertedShopDefinition definition)
	{
		return ExistingShopKeys.Contains(definition.StructuralKey) ||
		       ExistingShopNames.Contains(definition.ShopName);
	}

	public bool TryResolveItemPrototype(
		int legacyVnum,
		out FutureMudShopItemProtoReference reference,
		out bool ambiguous)
	{
		if (!ItemProtosByLegacyVnum.TryGetValue(legacyVnum, out var references) || references.Count == 0)
		{
			reference = default!;
			ambiguous = false;
			return false;
		}

		reference = references[0];
		ambiguous = references.Count > 1;
		return true;
	}

	private static IReadOnlyList<FutureMudShopItemProtoReference> LoadImportedItemPrototypes(FuturemudDatabaseContext context)
	{
		return context.GameItemProtos
			.Include(x => x.EditableItem)
			.Where(x => x.EditableItem != null &&
			            x.EditableItem.BuilderComment != null &&
			            x.EditableItem.BuilderComment.StartsWith("RPIIMPORT|"))
			.AsEnumerable()
			.Select(TryBuildItemProtoReference)
			.Where(x => x is not null)
			.Select(x => x!)
			.ToList();
	}

	private static FutureMudShopItemProtoReference? TryBuildItemProtoReference(GameItemProto proto)
	{
		var marker = proto.EditableItem.BuilderComment
			.Split('\n', StringSplitOptions.RemoveEmptyEntries)
			.FirstOrDefault() ?? string.Empty;
		var match = ItemImportMarkerRegex.Match(marker);
		if (!match.Success ||
		    !int.TryParse(match.Groups["vnum"].Value, NumberStyles.Integer, SystemCultureInfo.InvariantCulture, out var vnum) ||
		    !Enum.TryParse<RPIItemType>(match.Groups["type"].Value, ignoreCase: true, out var itemType))
		{
			return null;
		}

		return new FutureMudShopItemProtoReference(
			proto.Id,
			vnum,
			marker,
			proto.Name,
			proto.ShortDescription,
			proto.CostInBaseCurrency,
			itemType);
	}
}

public static class FutureMudShopValidation
{
	public static IReadOnlyList<FutureMudShopValidationIssue> Validate(
		FutureMudShopBaselineCatalog catalog,
		IEnumerable<ConvertedShopDefinition> definitions)
	{
		List<FutureMudShopValidationIssue> issues = [];
		var ordered = definitions
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.KeeperVnum)
			.ToList();

		if (catalog.DefaultEconomicZone is null)
		{
			issues.Add(new FutureMudShopValidationIssue(
				"baseline",
				"error",
				"The target database does not contain an economic zone to attach imported shops to."));
		}

		HashSet<string> seenStructuralKeys = new(StringComparer.OrdinalIgnoreCase);
		foreach (var definition in ordered)
		{
			if (!seenStructuralKeys.Add(definition.StructuralKey))
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"warning",
					$"Another converted shop already targets shopfront/store pair {definition.StructuralKey}; apply-shops will import the first one only."));
			}

			if (definition.Status != ShopConversionStatus.Ready)
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"error",
					"Shop conversion is invalid and cannot be imported."));
			}

			if (definition.ShopVnum <= 0)
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"error",
					"Shop has no positive RPI shop_vnum for the FutureMUD shopfront."));
			}
			else if (!catalog.CellIds.Contains(definition.ShopVnum))
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"error",
					$"Missing FutureMUD shopfront cell id {definition.ShopVnum.ToString(SystemCultureInfo.InvariantCulture)}. Run apply-rooms before apply-shops."));
			}

			if (definition.StoreVnum <= 0)
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"warning",
					"Shop has no positive RPI store_vnum; no stockroom cell will be assigned."));
			}
			else if (!catalog.CellIds.Contains(definition.StoreVnum))
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"error",
					$"Missing FutureMUD stockroom cell id {definition.StoreVnum.ToString(SystemCultureInfo.InvariantCulture)}. Run apply-rooms before apply-shops."));
			}

			if (catalog.HasExistingShop(definition))
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"warning",
					"An existing FutureMUD shop already uses this generated name or shopfront/stockroom pair; apply-shops will skip it."));
			}

			foreach (var merchandise in definition.Merchandise)
			{
				if (!catalog.TryResolveItemPrototype(merchandise.DeliveryVnum, out _, out var ambiguous))
				{
					issues.Add(new FutureMudShopValidationIssue(
						definition.SourceKey,
						"warning",
						$"Delivery vnum {merchandise.DeliveryVnum.ToString(SystemCultureInfo.InvariantCulture)} has no imported FutureMUD item prototype; that merchandise row will be skipped."));
					continue;
				}

				if (ambiguous)
				{
					issues.Add(new FutureMudShopValidationIssue(
						definition.SourceKey,
						"warning",
						$"Delivery vnum {merchandise.DeliveryVnum.ToString(SystemCultureInfo.InvariantCulture)} matched multiple imported item prototypes; the lowest prototype id will be used."));
				}
			}

			if (definition.Merchandise.Count == 0)
			{
				issues.Add(new FutureMudShopValidationIssue(
					definition.SourceKey,
					"warning",
					"Shop has no importable delivery merchandise; apply-shops will create the shop shell only."));
			}
		}

		return issues;
	}
}

public sealed class FutureMudShopImporter
{
	private readonly FuturemudDatabaseContext _context;
	private readonly FutureMudShopBaselineCatalog _catalog;

	public FutureMudShopImporter(FuturemudDatabaseContext context, FutureMudShopBaselineCatalog catalog)
	{
		_context = context;
		_catalog = catalog;
	}

	public IReadOnlyList<FutureMudShopValidationIssue> Validate(IEnumerable<ConvertedShopDefinition> definitions)
	{
		return FutureMudShopValidation.Validate(_catalog, definitions);
	}

	public FutureMudShopImportResult Apply(IEnumerable<ConvertedShopDefinition> definitions, bool execute)
	{
		var ordered = definitions
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.KeeperVnum)
			.ToList();
		var issues = Validate(ordered).ToList();
		var invalidSourceKeys = issues
			.Where(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.SourceKey)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var globalFatal = invalidSourceKeys.Contains("baseline");

		HashSet<string> existingShopKeys = _catalog.ExistingShopKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
		HashSet<string> existingShopNames = _catalog.ExistingShopNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
		List<ShopApplyAuditEntry> auditEntries = [];
		var skippedExistingCount = 0;
		var skippedInvalidCount = 0;
		var insertedCount = 0;
		var insertedMerchandiseCount = 0;

		foreach (var definition in ordered)
		{
			if (existingShopKeys.Contains(definition.StructuralKey) || existingShopNames.Contains(definition.ShopName))
			{
				skippedExistingCount++;
				auditEntries.Add(BuildAuditEntry(definition, "skipped-existing", null, 0, definition.DeliveryVnums));
				continue;
			}

			if (globalFatal ||
			    invalidSourceKeys.Contains(definition.SourceKey) ||
			    definition.Status != ShopConversionStatus.Ready)
			{
				skippedInvalidCount++;
				auditEntries.Add(BuildAuditEntry(definition, "skipped-invalid", null, 0, definition.DeliveryVnums));
				continue;
			}

			var resolvedMerchandise = ResolveMerchandise(definition, out var skippedDeliveryVnums);
			if (!execute)
			{
				auditEntries.Add(BuildAuditEntry(definition, "would-create", null, resolvedMerchandise.Count, skippedDeliveryVnums));
				existingShopKeys.Add(definition.StructuralKey);
				existingShopNames.Add(definition.ShopName);
				continue;
			}

			var shop = BuildShop(definition, resolvedMerchandise);
			_context.Shops.Add(shop);
			_context.SaveChanges();

			insertedCount++;
			insertedMerchandiseCount += resolvedMerchandise.Count;
			existingShopKeys.Add(definition.StructuralKey);
			existingShopNames.Add(definition.ShopName);
			auditEntries.Add(BuildAuditEntry(definition, "created", shop.Id, resolvedMerchandise.Count, skippedDeliveryVnums));
		}

		return new FutureMudShopImportResult(
			insertedCount,
			insertedMerchandiseCount,
			skippedExistingCount,
			skippedInvalidCount,
			issues,
			new ShopApplyAuditReport(DateTime.UtcNow, execute, auditEntries));
	}

	private IReadOnlyList<(ConvertedShopMerchandiseDefinition Merchandise, FutureMudShopItemProtoReference Proto)> ResolveMerchandise(
		ConvertedShopDefinition definition,
		out IReadOnlyList<int> skippedDeliveryVnums)
	{
		List<(ConvertedShopMerchandiseDefinition Merchandise, FutureMudShopItemProtoReference Proto)> resolved = [];
		List<int> skipped = [];

		foreach (var merchandise in definition.Merchandise)
		{
			if (!_catalog.TryResolveItemPrototype(merchandise.DeliveryVnum, out var proto, out _))
			{
				skipped.Add(merchandise.DeliveryVnum);
				continue;
			}

			resolved.Add((merchandise, proto));
		}

		skippedDeliveryVnums = skipped;
		return resolved;
	}

	private MudSharp.Models.Shop BuildShop(
		ConvertedShopDefinition definition,
		IReadOnlyList<(ConvertedShopMerchandiseDefinition Merchandise, FutureMudShopItemProtoReference Proto)> merchandise)
	{
		var economicZone = _catalog.DefaultEconomicZone ?? throw new InvalidOperationException("Cannot import shops without a FutureMUD economic zone.");
		var shop = new MudSharp.Models.Shop
		{
			Name = definition.ShopName,
			CurrencyId = economicZone.CurrencyId,
			IsTrading = true,
			EconomicZoneId = economicZone.Id,
			EmployeeRecords = "<Employees/>",
			CashBalance = 0.0M,
			ExpectedCashBalance = 0.0M,
			ShopType = "Permanent",
			MinimumFloatToBuyItems = 0.0M,
			AutopayTaxes = true,
			StockroomCellId = definition.StoreVnum > 0 ? (long)definition.StoreVnum : null,
		};

		shop.ShopsStoreroomCells.Add(new ShopsStoreroomCell
		{
			Shop = shop,
			CellId = definition.ShopVnum,
		});

		foreach (var (definitionMerchandise, proto) in merchandise)
		{
			shop.Merchandises.Add(new Merchandise
			{
				Name = definitionMerchandise.MerchandiseName,
				AutoReordering = true,
				AutoReorderPrice = definitionMerchandise.AutoReorderPrice,
				BasePrice = definitionMerchandise.BasePrice,
				DefaultMerchandiseForItem = true,
				ItemProtoId = proto.Id,
				ListDescription = definitionMerchandise.ListDescription,
				MinimumStockLevels = 1,
				MinimumStockLevelsByWeight = 0.0,
				PreserveVariablesOnReorder = true,
				WillSell = true,
				WillBuy = definitionMerchandise.WillBuy,
				BaseBuyModifier = definitionMerchandise.BaseBuyModifier,
				SalesMarkupMultiplier = 1.0M,
				MinimumConditionToBuy = 1.0,
				MaximumStockLevelsToBuy = definitionMerchandise.WillBuy ? 0 : 1,
				IgnoreMarketPricing = true,
				PermitItemDecayOnStockedItems = false,
			});
		}

		return shop;
	}

	private static ShopApplyAuditEntry BuildAuditEntry(
		ConvertedShopDefinition definition,
		string action,
		long? shopId,
		int merchandiseCount,
		IReadOnlyList<int> skippedDeliveryVnums)
	{
		return new ShopApplyAuditEntry(
			definition.SourceKey,
			definition.KeeperVnum,
			definition.ShopVnum,
			definition.StoreVnum,
			action,
			shopId,
			merchandiseCount,
			skippedDeliveryVnums);
	}
}

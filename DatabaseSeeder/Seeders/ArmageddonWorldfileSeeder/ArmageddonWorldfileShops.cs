#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private readonly Dictionary<int, EconomicZone> _economicZones = new();
	private readonly Dictionary<int, List<Room>> _generatedShopfronts = new();
	private Exit _nativeExitTemplate = null!;
	private readonly List<Room> _auxiliaryRooms = [];

	private void InstallShops()
	{
		_nativeExitTemplate = _db.Exits.AsNoTracking().OrderBy(x => x.Id).FirstOrDefault()
			?? throw new InvalidOperationException("Core installation must provide a native exit template for generated shopfront links.");
		var sourceCalendarId = _db.ShardsCalendars.AsNoTracking()
			.Select(x => (long?)x.CalendarId).FirstOrDefault() ?? _db.Calendars.OrderBy(x => x.Id).Select(x => (long?)x.Id).First();
		var clock = _db.Clocks.OrderBy(x => x.Id).First();
		var timezoneId = clock.PrimaryTimezoneId;
		var timezoneName = _db.Timezones.AsNoTracking().Single(x => x.Id == timezoneId).Name;
		// Every room in this pinned corpus has CITY_NONE. Native shops still require an economic zone.
		if (_source.Rooms.Any(x => x.City != 0)) throw new InvalidOperationException("A nonzero source city requires a reviewed city commodity binding.");
		foreach (var city in new[] { 0 })
		{
			var zone = _zones[_source.Rooms.OrderBy(x => x.Vnum).First().Zone];
			var economicZone = new EconomicZone
			{
				Id = Next<EconomicZone>(), Name = "Unincorporated Trade", CurrencyId = _currency.Id,
				ZoneForTimePurposesId = zone.Id, ReferenceClockId = clock.Id, ReferenceCalendarId = sourceCalendarId,
				ReferenceTime = $"{timezoneName} {clock.Hours}:{clock.Minutes}:{clock.Seconds}",
				IntervalType = (int)IntervalType.Monthly, IntervalAmount = 1, PreviousFinancialPeriodsToKeep = 24,
				PermitTaxableLosses = false, OutstandingTaxesOwed = 0.0m, TotalRevenueHeld = 0.0m,
				EstateDefaultDiscoverTime = "28d", EstateClaimPeriodLength = "14d"
			};
			_db.EconomicZones.Add(economicZone);
			_economicZones.Add(city, economicZone);
		}
		Flush();

		var resetRoomsByKeeper = _source.Resets
			.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && x.Arguments.Length >= 4)
			.GroupBy(x => x.Arguments[1])
			.ToDictionary(x => x.Key, x => x.Select(r => r.Arguments[3]).Distinct().Order().ToArray());
		var roomZones = _source.Rooms.ToDictionary(x => x.Vnum, x => x.Zone);
		var shops = 0;
		var usedKeepers = new HashSet<int>();
		var usedFronts = new HashSet<long>();
		var sourceItems = _source.Items.ToDictionary(x => x.Vnum);
		var importedShopIds = new List<long>();
		foreach (var source in _source.Shops.OrderBy(x => x.Index))
		{
			if (!usedKeepers.Add(source.Keeper))
			{
				Record("shop.shadowed." + source.Index, "SourceShop", null, null, source);
				Adapt("shadowed shop", source.Index, "The source trading lookup uses the first shop for this keeper; this later record is archived without a second competing shop.");
				continue;
			}
			var keeperNpc = _source.Npcs.SingleOrDefault(x => x.Vnum == source.Keeper);
			var keeperRooms = resetRoomsByKeeper.GetValueOrDefault(source.Keeper, []);
			var candidateVnum = keeperRooms.Where(_rooms.ContainsKey).Select(x => (int?)x).FirstOrDefault();
			var zoneId = candidateVnum is { } roomVnum ? roomZones[roomVnum] : ResolveShopZone(source, keeperNpc?.Source.Path);
			Room front;
			var candidate = candidateVnum is { } vnum && !_interiorTemplates.Contains(vnum) ? _rooms[vnum] : null;
			if (candidate is not null && usedFronts.Add(candidate.Id))
			{
				front = candidate;
			}
			else
			{
				front = CreateGeneratedShopfront(source, zoneId);
				Adapt("shop placement", source.Index,
					$"Keeper Vnum {source.Keeper} has no exclusive qualified M-reset storefront; a generated storefront was connected to zone {zoneId}'s imported entry room.");
			}

			var stockroom = CreateShopStockroom(source, zoneId);
			var openingCash = keeperNpc is null ? 0 : Math.Max(0, keeperNpc.Money[0] + keeperNpc.Money[1] / 2);
			var shop = new Shop
			{
				Id = Next<Shop>(), Name = ShopName(source), CurrencyId = _currency.Id,
				EconomicZoneId = _economicZones[0].Id, IsTrading = true, ShopType = "Permanent",
				EmployeeRecords = "<Employees/>", CashBalance = openingCash, ExpectedCashBalance = openingCash,
				MinimumFloatToBuyItems = 0.0m, AutopayTaxes = false, StockroomId = stockroom.Id
			};
			shop.ShopsStoreroomRooms.Add(new ShopsStoreroomRoom { Shop = shop, RoomId = front.Id });
			_db.Shops.Add(shop);
			importedShopIds.Add(shop.Id);
			var migratedStock = MoveMerchantStock(source, sourceItems);
			foreach (var producingVnum in source.Producing.Where(x => x >= 0).Distinct())
			{
				if (!_items.TryGetValue(producingVnum, out var produced) || migratedStock.Any(x => x.SourceVnum == producingVnum)) continue;
				migratedStock.Add(new(produced, producingVnum, []));
				Adapt("shop producing sample", source.Index, $"Producing item {Key(producingVnum)} had no transferred stock sample; native permanent stock supplies one exemplar for autoreorder.");
			}
			var stockPrototypes = _db.GameItemProtos.Local.Where(x => _effectiveItems.ContainsKey(x.Id)).ToArray();
			foreach (var item in stockPrototypes)
			{
				var match = Regex.Match(item.UniqueName ?? "", "^Vnum(?<id>[0-9]+)");
				if (!match.Success || !sourceItems.TryGetValue(int.Parse(match.Groups["id"].Value, CultureInfo.InvariantCulture), out var original) || original.Physical[1] <= 0) continue;
				var sellIndex = Array.IndexOf(source.SellTypes, original.ItemType);
				var buyIndex = Array.IndexOf(source.BuyTypes, original.ItemType);
				var willSell = original.ItemType != 0 && sellIndex >= 0;
				var willBuy = original.ItemType != 0 && buyIndex >= 0 && !(original.ItemType == 19 && original.Values[3] != 0);
				if (!willBuy && !willSell) continue;
				var salePrice = willSell ? SourceShopPrice(original.Physical[1], source.BuyMultipliers[sellIndex]) : original.Physical[1];
				var payout = willBuy ? SourceShopPrice(original.Physical[1], source.SellMultipliers[buyIndex], source.Producing.Contains(original.Vnum)) : 0m;
				if (salePrice <= 0) { salePrice = 1; willSell = false; }
				var producing = source.Producing.Contains(original.Vnum) && willSell && migratedStock.Any(x => x.Prototype.Id == item.Id);
				var merchandise = new Merchandise
				{
					Id = Next<Merchandise>(), Name = item.Name, ShopId = shop.Id, MerchandiseType = 0,
					ItemProtoId = item.Id, AutoReordering = producing, AutoReorderPrice = 0.0m,
					BasePrice = salePrice, WillSell = willSell, WillBuy = willBuy && payout > 0,
					BaseBuyModifier = SourceBuybackModifier(salePrice, payout), SalesMarkupMultiplier = 1.0m,
					DefaultMerchandiseForItem = true, ListDescription = item.ShortDescription, MinimumStockLevels = producing ? 1 : 0,
					MinimumStockLevelsByWeight = 0.0, MaximumStockLevelsToBuy = int.MaxValue, MinimumConditionToBuy = original.ItemType == 19 ? 1.0 : 0.0,
					IgnoreMarketPricing = true, PermitItemDecayOnStockedItems = true
				};
				_db.Merchandises.Add(merchandise);
				foreach (var stock in migratedStock.Where(x => x.Prototype.Id == item.Id))
					SeedShopStockItem(stock, stockroom, shop, merchandise);
			}
			Record("shop." + source.Index, nameof(Shop), shop.Id, null,
				new { Source = source, FrontRoomId = front.Id, StockroomId = stockroom.Id, Disposition = "Permanent shop with source-linked merchandise and generated native stockroom." });
			shops++;
		}
		_counts["shops"] = shops;
		_counts["shop stockrooms"] = shops;
		Flush();
		// Merchandise's legacy EF zero sentinel otherwise omits this explicit zero
		// on insert and the database substitutes its 0.95 default. Apply only to
		// these newly imported shops, inside the same initial-install transaction.
		_db.Merchandises.Where(x => importedShopIds.Contains(x.ShopId) && x.MinimumConditionToBuy == 0.95)
			.ExecuteUpdate(setters => setters.SetProperty(x => x.MinimumConditionToBuy, 0.0));
		foreach (var room in _auxiliaryRooms) room.CurrentOverlayId = room.RoomOverlays.Single().Id;
		Adapt("shop economy", 0, "Source CITY_NONE rooms share a required native Unincorporated Trade region with no invented taxes. Initial merchant trade cash and sale stock are moved into permanent shops; keeper presence is irrelevant. Native autoreorder replaces infinite producing stock. Random opening stock, hourly virtual sales, source opening hours and clan discounts remain source provenance rather than new shop functionality.");
		Flush();
	}

	internal static decimal SourceShopPrice(int cost, double multiplier, bool producingBuyback = false)
	{
		var amount = (float)cost * (float)multiplier;
		if (producingBuyback) amount *= 0.5f;
		return (decimal)Math.Floor(Math.Max(0.0, amount) + 0.5);
	}

	internal static decimal SourceBuybackModifier(decimal price, decimal payout)
	{
		var ratio = payout / price;
		// Native transaction totals truncate. Bias a repeating decimal upward below one coin,
		// rather than letting e.g. 7 * (2/7) round just below 2 and pay only 1.
		if (price * ratio < payout) ratio += 0.00000000000000000001m;
		return ratio;
	}

	private Room CreateGeneratedShopfront(ArmWorldShop source, int zoneIndex)
	{
		if (!_generatedShopfronts.TryGetValue(zoneIndex, out var fronts))
		{
			fronts = [];
			_generatedShopfronts.Add(zoneIndex, fronts);
		}
		var sourceAnchor = _source.Rooms.Where(x => x.Zone == zoneIndex && !_interiorTemplates.Contains(x.Vnum)).OrderBy(x => x.Vnum).FirstOrDefault()
			?? throw new InvalidOperationException($"Cannot place shop {source.Index}: source zone {zoneIndex} has no imported rooms.");
		var anchor = fronts.LastOrDefault() ?? _rooms[sourceAnchor.Vnum];
		var room = CreateAuxiliaryRoom(zoneIndex, "shopfront." + source.Index,
			ShopName(source), $"The storefront of {ShopName(source)} is here.");
		var exit = new Exit
		{
			Id = Next<Exit>(), RoomId1 = anchor.Id, RoomId2 = room.Id,
			Direction1 = (int)CardinalDirection.Unknown,
			Direction2 = (int)CardinalDirection.Unknown,
			Keywords1 = "shopfront", Keywords2 = "street", PrimaryKeyword1 = "shopfront", PrimaryKeyword2 = "street",
			Verb1 = "enter", Verb2 = "leave", OutboundTarget1 = ShopName(source), InboundTarget2 = ShopName(source),
			InboundTarget1 = "the street", OutboundTarget2 = "the street",
			TimeMultiplier = _nativeExitTemplate.TimeMultiplier, AcceptsDoor = false,
			InboundDescription1 = "", InboundDescription2 = "",
			OutboundDescription1 = "", OutboundDescription2 = "",
			DoorSize = _nativeExitTemplate.DoorSize, MaximumSizeToEnter = _nativeExitTemplate.MaximumSizeToEnter,
			MaximumSizeToEnterUpright = _nativeExitTemplate.MaximumSizeToEnterUpright,
			ClimbDifficulty = _nativeExitTemplate.ClimbDifficulty, BlockedLayers = _nativeExitTemplate.BlockedLayers ?? ""
		};
		_db.Exits.Add(exit);
		_db.RoomOverlaysExits.Add(new RoomOverlayExit { ExitId = exit.Id, RoomOverlayId = anchor.RoomOverlays.Single().Id });
		_db.RoomOverlaysExits.Add(new RoomOverlayExit { ExitId = exit.Id, RoomOverlayId = room.RoomOverlays.Single().Id });
		fronts.Add(room);
		return room;
	}

	private Room CreateShopStockroom(ArmWorldShop source, int zoneIndex) => CreateAuxiliaryRoom(zoneIndex,
		"shopstockroom." + source.Index, "The Storeroom of " + ShopName(source), "The Storeroom of " + ShopName(source) + ".");

	private void FinalizeAuxiliaryRooms()
	{
		if (_auxiliaryRooms.Count == 0) return;
		// Persist both sides before setting the circular current-overlay reference.
		// Services add custody rooms after the earlier shop finalisation pass.
		Flush();
		foreach (var room in _auxiliaryRooms) room.CurrentOverlayId = room.RoomOverlays.Single().Id;
		Flush();
	}

	private Room CreateAuxiliaryRoom(int zoneIndex, string uniqueSuffix, string name, string description)
	{
		var terrain = _db.Terrains.AsNoTracking().Single(x => x.Name == "Urban Street");
		var room = new Room
		{
			Id = Next<Room>(), UniqueName = "armw-" + uniqueSuffix, ZoneId = _zones[zoneIndex].Id,
			EffectData = "<Effects/>", X = 0, Y = 0, Z = 0
		};
		var overlay = new RoomOverlay
		{
			Id = Next<RoomOverlay>(), Name = room.UniqueName, Room = room, RoomId = room.Id,
			RoomName = name, RoomDescription = description, RoomOverlayPackageId = _overlays.Id,
			RoomOverlayPackageRevisionNumber = 0, TerrainId = terrain.Id, AtmosphereId = terrain.AtmosphereId,
			AtmosphereType = terrain.AtmosphereType, OutdoorsType = terrain.DefaultRoomOutdoorsType,
			AmbientLightFactor = 1.0, AddedLight = 0.0, SafeQuit = true
		};
		_db.Rooms.Add(room);
		_db.RoomOverlays.Add(overlay);
		_auxiliaryRooms.Add(room);
		_zones[zoneIndex].DefaultRoomId ??= room.Id;
		return room;
	}

	private int ResolveShopZone(ArmWorldShop shop, string? keeperPath)
	{
		// These four records outlive their keeper prototypes. This is an explicit
		// storefront recovery map, not a city inferred from a numeric vnum.
		if (shop.Index is 62 or 86 or 87 or 88)
		{
			var configured = shop.Index == 62 ? 24 : 6;
			Adapt("orphan shop placement", shop.Index, $"Missing keeper {Key(shop.Keeper)} uses configured source zone {configured}; no NPC ownership or presence is required.");
			return configured;
		}
		var fileName = keeperPath is null ? "" : System.IO.Path.GetFileNameWithoutExtension(keeperPath);
		var match = Regex.Match(fileName, "^(?:n|z)(?<zone>[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		if (match.Success && int.TryParse(match.Groups["zone"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var sourceZone) &&
		    _zones.ContainsKey(sourceZone)) return sourceZone;
		throw new InvalidOperationException($"Shop {shop.Index} keeper Vnum {shop.Keeper} has no M reset and no zone-qualified NPC source path '{keeperPath ?? "(missing)"}'.");
	}

	private string ShopName(ArmWorldShop source) =>
		"The Shop of " + (_source.Npcs.FirstOrDefault(x => x.Vnum == source.Keeper)?.ShortDescription.Trim() ?? Key(source.Keeper));

	private sealed record ShopStockPlan(GameItemProto Prototype, int SourceVnum, List<ShopStockPlan> Contents);

	private List<ShopStockPlan> MoveMerchantStock(ArmWorldShop shop, IReadOnlyDictionary<int, ArmWorldItem> sources)
	{
		var result = new List<ShopStockPlan>();
		var selectedExemplar = false;
		var family = _db.NpcTemplates.Local.Where(x => x.UniqueName == Key(shop.Keeper) ||
			x.UniqueName?.StartsWith(Key(shop.Keeper) + "_Reset", StringComparison.Ordinal) == true).OrderBy(x => x.Id).ToArray();
		foreach (var template in family)
		{
			var definition = XElement.Parse(template.Definition);
			var cashId = (long?)definition.Element("OnLoadProg");
			var cashProg = _progs.FirstOrDefault(x => x.Id == cashId);
			if (cashProg is not null)
				cashProg.FunctionText = string.Join("\n", cashProg.FunctionText.Split('\n').Where(x => !x.TrimStart().StartsWith("givecurrency(", StringComparison.OrdinalIgnoreCase)));
			foreach (var outfitId in definition.Element("TemplateLoadAdditions")?.Element("Outfits")?.Elements("Outfit")
				.Select(x => (long)x.Attribute("template")!) ?? [])
			{
				var outfit = _db.OutfitTemplates.Local.Single(x => x.Id == outfitId);
				var rows = outfit.OutfitTemplateItems.ToArray();
				var transferred = new HashSet<OutfitTemplateItem>();
				ShopStockPlan Plan(OutfitTemplateItem row)
				{
					transferred.Add(row);
					var proto = _db.GameItemProtos.Local.Single(x => x.Id == row.GameItemProtoId);
					var source = _effectiveItems[proto.Id].Source;
					return new(proto, source.Vnum, rows.Where(x => x.ContainerKey == row.TemplateKey).Select(Plan).ToList());
				}
				var roots = rows.Where(x => x.Placement == (int)OutfitTemplateItemPlacement.Inventory &&
					_effectiveItems.TryGetValue(x.GameItemProtoId, out var effective) &&
					shop.SellTypes.Contains(effective.Source.ItemType) && effective.Source.Physical[1] > 0).Select(Plan).ToArray();
				if (!selectedExemplar && roots.Length > 0) { result.AddRange(roots); selectedExemplar = true; }
				foreach (var row in transferred)
				{
					outfit.OutfitTemplateItems.Remove(row);
					_db.OutfitTemplateItems.Remove(row);
				}
			}
		}
		Record("shop.stock-transfer." + shop.Index, "SourceShopStockTransfer", null, null,
			new { shop.Keeper, CashDisposition = "Trade cash moved to permanent shop; removed from merchant NPC load programs", Stock = result.Select(x => new { x.SourceVnum, Prototype = x.Prototype.Id }),
				Policy = "First authored populated keeper loadout supplies opening sale stock. All corresponding keeper sale-stock loadouts are removed; equipped possessions remain NPC equipment." });
		return result;
	}

	private GameItem SeedShopStockItem(ShopStockPlan stock, Room room, Shop shop, Merchandise? merchandise)
	{
		var proto = stock.Prototype;
		var (source, values) = _effectiveItems[proto.Id];
		var effect = new XElement("Effects");
		if (merchandise is not null)
			effect.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "ItemOnDisplayInShop"),
				new XElement("Original", 0), new XElement("Remaining", 0), new XElement("Effect", new XElement("Shop", shop.Id), new XElement("Merch", merchandise.Id))));
		var item = new GameItem { Id = Next<GameItem>(), GameItemProtoId = proto.Id, GameItemProtoRevision = proto.RevisionNumber,
			Quality = proto.BaseItemQuality, MaterialId = proto.MaterialId, Size = proto.Size, Condition = 1.0, RoomLayer = 0,
			OwnerId = shop.Id, OwnerType = "Shop", EffectData = effect.ToString() };
		_db.GameItems.Add(item);
		var children = stock.Contents.Select(child => SeedShopStockItem(child, room, shop, null)).ToArray();
		foreach (var child in children) child.ContainerId = item.Id;
		foreach (var componentLink in proto.GameItemProtosGameItemComponentProtos)
		{
			var component = _db.GameItemComponentProtos.Local.Single(x => x.Id == componentLink.GameItemComponentProtoId);
			var template = XElement.Parse(component.Definition);
			var state = new XElement("Definition");
			switch (component.Type)
			{
				case "Holdable": case "MeleeWeapon": case "Ammunition": case "Armour": case "Wearable": case "Beltable": case "Belt": case "Chair": case "Keyword Descriptions": break;
				case "Simple Key": state.Add(new XElement("Pattern", checked(source.Vnum + 1))); break;
				case "Container": case "LockingContainer":
					state.Add(new XAttribute("Open", (values[1] & 4) == 0), new XElement("Locks"), children.Select(x => new XElement("Contained", x.Id)));
					if (component.Type == "LockingContainer") state.Add(new XElement("Pattern", _initialLockPatterns[proto.Id]), new XElement("IsLocked", (values[1] & 8) != 0));
					break;
				case "Food": state.Add(new XElement("Bites", (double?)template.Attribute("Bites") ?? 1.0)); break;
				case "Liquid Container":
					state.Add(new XAttribute("Open", true), new XElement("Locks"));
					state.Add(_initialLiquidContents.TryGetValue(proto.Id, out var liquid)
						? new XElement("Mix", new XElement("Liquid", new XAttribute("id", liquid.Liquid), new XAttribute("amount", liquid.Amount.ToString("R", CultureInfo.InvariantCulture))))
						: new XElement("NoLiquid"));
					break;
				case "Prog Light": state.Add(new XElement("Lit", (values[5] & 1) != 0)); break;
				case "Torch": state.Add(new XElement("RemainingFuel", (int?)template.Element("SecondsOfFuel") ?? 0), new XElement("Lit", (values[5] & 1) != 0)); break;
				case "Bow": state.Add(new XElement("Wielded", 0), new XElement("Loaded", 0)); break;
				case "ThrownWeapon": state.Add(new XElement("Wielded", 0)); break;
				case "Crossbow": state.Add(new XElement("Wielded", 0), new XElement("Loaded", 0), new XElement("Magazine"), new XElement("IsReadied", false), new XElement("IsEmplaced", false)); break;
				case "Sling": case "Blowgun": state.Add(new XElement("Loaded", 0)); break;
				case "Table": state.Add(new XElement("Flipped", false)); break;
				case "Currency Pile": state.Add(new XAttribute("Currency", _currency.Id), new XElement("Coins", new XElement("Coin", new XAttribute("Id", _bit.Id), new XAttribute("Count", values[0])))); break;
				default: throw new InvalidOperationException($"Shop stock component {component.Type} has no qualified native initial state.");
			}
			item.GameItemComponents.Add(new GameItemComponent { Id = Next<GameItemComponent>(), GameItemComponentProtoId = component.Id,
				GameItemComponentProtoRevision = component.RevisionNumber, Definition = state.ToString() });
		}
		if (merchandise is not null) item.RoomsGameItems.Add(new RoomsGameItems { RoomId = room.Id });
		_counts["opening shop stock items"] = _counts.GetValueOrDefault("opening shop stock items") + 1;
		return item;
	}
}

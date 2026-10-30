#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private readonly Dictionary<int, WearProfile[]> _wearProfiles = new();
	private readonly Dictionary<int, long> _wearProfileComponents = new();
	private readonly Dictionary<(int Mask, int Size), long> _sizedWearProfileComponents = new();
	private readonly Dictionary<string, long> _weaponTypes = new(StringComparer.Ordinal);
	private readonly Dictionary<string, GameItemProto> _resetItemVariants = new(StringComparer.Ordinal);
	private readonly Dictionary<long, (ArmWorldItem Source, int[] Values)> _effectiveItems = new();
	private long _humanBodyId;
	private long _holdableComponentId;
	private long _trueProgId;
	private long _wearableScalarSizeProgId;
	private readonly Dictionary<string, long> _nativeArmourTypes = new(StringComparer.OrdinalIgnoreCase);

	private static readonly IReadOnlyDictionary<int, string[]> WearBitLocations = new Dictionary<int, string[]>
	{
		[1 << 1] = ["rindexfinger", "lindexfinger", "rmiddlefinger", "lmiddlefinger", "rringfinger", "lringfinger", "rpinkyfinger", "lpinkyfinger", "rthumb", "lthumb"],
		[1 << 2] = ["neck", "bneck"], [1 << 3] = ["abdomen", "belly", "rbreast", "lbreast", "uback", "lback"],
		[1 << 4] = ["scalp", "bhead", "forehead", "face"], [1 << 5] = ["rthigh", "lthigh", "rcalf", "lcalf", "rshin", "lshin", "rknee", "lknee"],
		[1 << 6] = ["rfoot", "lfoot"], [1 << 7] = ["rhand", "lhand"],
		[1 << 8] = ["rupperarm", "lupperarm", "rforearm", "lforearm"], [1 << 9] = ["uback", "lback"],
		[1 << 10] = ["abdomen", "belly", "rbreast", "lbreast", "uback", "lback"],
		[1 << 11] = ["abdomen", "groin", "rhip", "lhip"], [1 << 12] = ["rwrist", "lwrist"],
		[1 << 15] = ["abdomen", "belly"], [1 << 17] = ["scalp"], [1 << 18] = ["face", "chin", "nose"],
		[1 << 19] = ["rankle", "lankle"], [1 << 20] = ["lear"], [1 << 21] = ["rear"],
		[1 << 22] = ["rforearm", "lforearm"], [1 << 23] = ["bhead", "forehead", "scalp"],
		[1 << 24] = ["throat", "neck"], [1 << 25] = ["rshoulder", "lshoulder"],
		[1 << 26] = ["uback", "lback", "rshoulder", "lshoulder"]
	};
	private static readonly IReadOnlyDictionary<int, string> SourceItemTypeNames = new Dictionary<int, string>
	{
		[1] = "Light", [2] = "Component", [3] = "Wand", [4] = "Staff", [5] = "Weapon", [6] = "Art", [7] = "Tool", [8] = "Treasure",
		[9] = "Armour", [10] = "Potion", [11] = "Worn", [12] = "Other", [13] = "Skin", [14] = "Scroll", [15] = "Container",
		[16] = "Note", [17] = "Drink Container", [18] = "Key", [19] = "Food", [20] = "Money", [21] = "Pen", [22] = "Boat",
		[23] = "Wagon", [24] = "Dart", [25] = "Spice", [26] = "Chit", [27] = "Poison", [28] = "Bow", [29] = "Teleport",
		[30] = "Fire", [31] = "Mask", [32] = "Furniture", [33] = "Herb", [34] = "Cure", [35] = "Scent", [36] = "Playable",
		[37] = "Mineral", [38] = "Powder", [39] = "Jewelry", [40] = "Musical", [41] = "Rumor Board"
	};

	private void InstallItems()
	{
		var sizingSetting = _db.StaticConfigurations.Find("EnableWearableSizing");
		if (sizingSetting is null)
		{
			sizingSetting = new StaticConfiguration { SettingName = "EnableWearableSizing", Definition = "true" };
			_db.StaticConfigurations.Add(sizingSetting);
		}
		else
		{
			sizingSetting.Definition = "true";
		}
		_trueProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg];
		_wearableScalarSizeProgId = InstallWearableScalarSizeProg();
		_holdableComponentId = Component("holdable", "Holdable", new XElement("Definition")).Id;
		_humanBodyId = _db.Races.AsNoTracking().Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.HumanRace]).BaseBodyId;
		foreach (var (sourceType, nativeName) in new[]
		{
			(0, "Heavy Clothing"), (1, "Ultra Heavy Clothing"), (2, "Boiled Leather"),
			(3, "Metal Scale"), (4, "Chainmail"), (5, "Platemail")
		})
	{
			var matches = _db.ArmourTypes.AsNoTracking().Where(x => x.Name == nativeName).ToArray();
			if (matches.Length != 1) throw new InvalidOperationException($"Combat must provide one ArmourType named {nativeName} for native source armour class {sourceType}.");
			_nativeArmourTypes.Add(sourceType.ToString(CultureInfo.InvariantCulture), matches[0].Id);
		}
		foreach (var source in _source.Items.OrderBy(x => x.Vnum))
		{
			var item = CreateItemProto(source, source.Values, "base", null);
			if (!_items.TryAdd(source.Vnum, item)) throw new InvalidOperationException($"Duplicate item Vnum {source.Vnum}.");
			Record("item." + Key(source.Vnum), nameof(GameItemProto), item.Id, item.RevisionNumber,
				new { Source = source, SourceType = SourceItemTypeNames.GetValueOrDefault(source.ItemType, "Unknown"),
					Disposition = DescribeItemDisposition(source.ItemType) });
		}
		foreach (var type in _source.Items.Select(x => x.ItemType).Distinct().Order())
			if (type is not (1 or 4 or 5 or 9 or 11 or 15 or 17 or 18 or 19 or 20 or 23 or 24 or 27 or 28 or 32 or 34))
				Adapt("item type", type, $"Source {SourceItemTypeNames.GetValueOrDefault(type, "Unknown")} prototypes keep native Vnum identity and source fields as Holdable items; no equivalent native component was qualified.");
		InstallPlayerTailoringTools();
		_counts["item prototypes"] = _items.Count;
		InstallDoors();
	}

	private static string DescribeItemDisposition(int type) => type switch
	{
		1 => "Native Torch or permanent light component; source lit state is initialized.",
		4 => "Placed functional staves receive native energy reservoirs with mapped spell, source capacity and grade costs; invalid and unplaced devices remain inert.",
		5 => "Native melee weapon and damage profile from Armageddon attack class and value slots.",
		9 => "Native armour and wearable fit use source size slot 2 with the Armageddon scalar fit rule; protection points retained in provenance.",
		11 => "Wearable profiles use Armageddon wear flags; source size slot 0 is retained as the fixed fit.",
		15 => "Native container with source capacity and key pattern where present.",
		17 => "Native liquid container with source capacity; legacy liquid identity and amount retained in provenance.",
		18 => "Native Simple Key with source Vnum-derived pattern.",
		19 => "Native food component with source satiation hours.",
		20 => "Native currency pile with authored default amount.",
		23 => "Reviewed native vehicle, instanced dwelling or shared entrance; invalid entrances explicitly reported.",
		28 => "Native Bow and ranged type use source range and pull values.",
		24 => "Native piercing thrown dart uses source damage dice and bonus, native throwing rules and poison delivery.",
		27 => "Source poison vial uses a consumption/application prog and calibrated native toxin delivery.",
		34 => "Native oral antidote delivery and family-aware toxin purge.",
		32 => "Native Table or Chair component selected by source description.",
		_ => "Native Holdable fallback; source fields remain in provenance."
	};

	private void InstallDoors()
	{
		var wood = _materials.Values.FirstOrDefault(x => x.Name.Contains("wood", StringComparison.OrdinalIgnoreCase))
			?? _materials.Values.First();
		GameItemComponentProto DoorComponent(int pickDifficulty) => Component("armageddon.door." + pickDifficulty, "LockingDoor", new XElement("Definition",
			new XElement("ForceDifficulty", 10), new XElement("PickDifficulty", pickDifficulty),
			new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$")),
			new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$")),
			new XElement("LockEmoteNoActor", new XCData("@ lock|locks")),
			new XElement("UnlockEmoteNoActor", new XCData("@ unlock|unlocks")),
			new XElement("LockEmoteOtherSide", new XCData("$0 is locked from the other side.")),
			new XElement("UnlockEmoteOtherSide", new XCData("$0 is unlocked from the other side.")),
			new XElement("LockType", "Armageddon Worldfile Key"),
			new XElement("InstalledExitDescription", "door"), new XElement("CanBeOpenedByPlayers", true),
			new XElement("Uninstall", new XAttribute("CanPlayersUninstall", false),
				new XAttribute("UninstallDifficultyHingeSide", 10), new XAttribute("UninstallDifficultyNotHingeSide", 10), new XAttribute("UninstallTrait", 0)),
			new XElement("Smash", new XAttribute("CanPlayersSmash", false), new XAttribute("SmashDifficulty", 10))));
		var doorCount = 0;
		foreach (var (exit, sourceRoom, side, reverse) in _doors)
		{
			if (exit.DoorId.HasValue) continue;
			var doorType = DoorComponent(SourceDoorPickDifficulty(side.Key, side.Flags));
			var doorName = string.IsNullOrWhiteSpace(side.Keyword) ? "a door" : side.Keyword;
			var proto = new GameItemProto
			{
				Id = Next<GameItemProto>(), Name = doorName, UniqueName = $"armw-door-{sourceRoom.Vnum}-{side.Direction}",
				Keywords = doorName, ShortDescription = doorName, LongDescription = doorName + " is here.",
				FullDescription = "A door fitted into the exit.", MaterialId = wood.Id, Size = (int)SizeCategory.Large,
				Weight = 20.0 * _kilograms, BaseItemQuality = (int)ItemQuality.Standard, ReadOnly = true,
				CostInBaseCurrency = 0.0m, EditableItem = Approval(), PermitPlayerSkins = false
			};
			_db.GameItemProtos.Add(proto);
			Attach(proto, doorType);
			var initialState = _doorResetStates.GetValueOrDefault((sourceRoom.Vnum, side.Direction), 1);
			var sourceKey = Math.Max(0, side.Key);
			var keyItemExists = side.Key >= 0 && _source.Items.Any(x => x.Vnum == sourceKey && x.ItemType == 18) && _items.ContainsKey(sourceKey);
			var hinge = _rooms[sourceRoom.Vnum];
			var destinationRoom = _rooms[side.Destination];
			var locked = initialState == 2;
			var state = initialState == 0 ? (int)DoorState.Open : (int)DoorState.Closed;
			var pattern = 0;
			if (keyItemExists)
			{
				pattern = _keyPatterns.GetValueOrDefault(sourceKey, checked(sourceKey + 1));
			}
			else if (locked)
			{
				pattern = checked(_source.Items.Max(x => x.Vnum) + 2 + doorCount);
				Adapt("door lock", sourceRoom.Vnum,
					$"Exit direction {side.Direction} starts locked but has no imported matching key; a nonmatching native pattern {pattern} preserves the locked, pickable state.");
			}
			var component = new GameItemComponent
			{
				Id = Next<GameItemComponent>(), GameItemComponentProtoId = doorType.Id,
				GameItemComponentProtoRevision = doorType.RevisionNumber,
				Definition = new XElement("Definition", new XAttribute("State", state),
					new XElement("HingeCell", hinge.Id), new XElement("OpenDirectionCell", destinationRoom.Id),
					new XElement("IsLocked", locked), new XElement("Pattern", pattern)).ToString()
			};
			var gameItem = new GameItem
			{
				Id = Next<GameItem>(), Quality = (int)ItemQuality.Standard, GameItemProtoId = proto.Id,
				GameItemProtoRevision = proto.RevisionNumber, RoomLayer = 0, Condition = 1.0,
				MaterialId = wood.Id, Size = (int)SizeCategory.Large, EffectData = "<Effects/>"
			};
			component.GameItemId = gameItem.Id;
			gameItem.GameItemComponents.Add(component);
			_db.GameItems.Add(gameItem);
			exit.DoorId = gameItem.Id;
			Record($"door.{sourceRoom.Vnum}.{side.Direction}", nameof(GameItem), gameItem.Id, null,
				new { Source = side, ResetState = initialState, SourceKeyVnum = sourceKey,
					Disposition = "Native installed door item; reset open/closed/locked state applied." });
			doorCount++;
		}
		_counts["doors"] = doorCount;
		Flush();
	}

	internal static int SourceDoorPickDifficulty(int key, int flags)
	{
		// skills.c uses EX_DIFF_1/2/4 and treats negative key numbers as no lock.
		if (key < 0) return 10;
		return ((flags >> 12) & 7) switch
		{
			0 => 0, 1 => 2, 2 => 4, 3 => 5, 4 => 6, 5 => 7, 6 => 9, _ => 10
		};
	}

	private GameItemProto GetResetItem(ArmWorldItem source, int[] values, string state,
		IReadOnlyList<ArmWorldExtra>? extraDescriptions = null, bool forceLit = false, int forcedWearMask = 0)
	{
		if (values.Length != 6) throw new ArgumentException("A reset item must retain all six source value slots.", nameof(values));
		values = EffectiveResetValues(source, values, forceLit);
		if (forcedWearMask == 0 && source.Limit < 0 && values.SequenceEqual(source.Values) && (extraDescriptions is null || extraDescriptions.Count == 0))
			return _items[source.Vnum];
		var key = source.Vnum + ":" + string.Join(",", values) + ":" + (source.Limit >= 0 ? state : "effective") + ":" +
			(extraDescriptions is null ? "" : string.Join("|", extraDescriptions.Select(x => x.Keyword + "=" + x.Description))) + ":wear=" + forcedWearMask;
		if (_resetItemVariants.TryGetValue(key, out var existing)) return existing;
		var variant = CreateItemProto(source, values, "reset-" + Hash(key)[..16], extraDescriptions, forcedWearMask);
		_resetItemVariants.Add(key, variant);
		Record("item.variant." + Hash(key), nameof(GameItemProto), variant.Id, variant.RevisionNumber,
			new { SourceVnum = source.Vnum, EffectiveValues = values, State = state, ExtraDescriptions = extraDescriptions, ForcedWearMask = forcedWearMask });
		return variant;
	}

	internal static int[] EffectiveResetValues(ArmWorldItem source, int[] resetValues, bool forceLit = false)
	{
		var values = resetValues.ToArray();
		if (source.ItemType == 1)
		{
			values[5] |= source.Values[5] & 254;
			for (var i = 1; i <= 4; i++) if (values[i] == 0) values[i] = source.Values[i];
			if (forceLit) values[5] |= 1;
		}
		if (source.ItemType == 9 && values[3] == 0) values[3] = source.Values[3];
		return values;
	}

	private GameItemProto CreateItemProto(ArmWorldItem source, int[] values, string variantKey,
		IReadOnlyList<ArmWorldExtra>? resetDescriptions, int forcedWearMask = 0)
	{
		var itemType = source.ItemType;
		var material = _materials[source.Material];
		var sourceWeight = source.Physical[0];
		var sizeIndex = source.ItemType switch { 11 => 0, 9 => 2, _ => -1 };
		if (sizeIndex >= 0 && source.Values[sizeIndex] > 0 && source.Values[sizeIndex] != values[sizeIndex])
			sourceWeight = checked((int)((long)sourceWeight * values[sizeIndex] / source.Values[sizeIndex]));
		var kilograms = Math.Max(0.0, sourceWeight) / 100.0 * _kilograms;
		if (source.Physical[0] < 0) Adapt("item weight", source.Vnum, $"Invalid negative source weight {source.Physical[0]} repaired to zero.");
		var name = source.Name.Trim();
		var proto = new GameItemProto
		{
			Id = Next<GameItemProto>(), Name = name, UniqueName = variantKey == "base" ? Key(source.Vnum) : Key(source.Vnum) + "-" + variantKey,
			Keywords = name, ShortDescription = source.ShortDescription, LongDescription = source.LongDescription,
			FullDescription = source.FullDescription, MaterialId = material.Id, Size = (int)SizeCategory.Normal,
			Weight = kilograms, BaseItemQuality = (int)ItemQuality.Standard, ReadOnly = true,
			CostInBaseCurrency = Math.Max(0, source.Physical[1]), EditableItem = Approval(), PermitPlayerSkins = false
		};
		_db.GameItemProtos.Add(proto);
		_effectiveItems.Add(proto.Id, (source, values.ToArray()));
		if (source.ItemType != 23 || (source.Flags[2] & 1) != 0)
			Attach(proto, _db.GameItemComponentProtos.Local.Single(x => x.Id == _holdableComponentId));
		AddItemBehavior(proto, source, values, variantKey, forcedWearMask);
		var descriptions = source.ExtraDescriptions.Concat(resetDescriptions ?? []).ToArray();
		var folded = descriptions.FirstOrDefault(x => x.Keyword.Trim().Equals(source.Name.Trim(), StringComparison.Ordinal));
		if (string.IsNullOrEmpty(proto.FullDescription) && folded is not null) proto.FullDescription = folded.Description;
		var keywords = descriptions.Where(x => !x.Keyword.Trim().Equals(source.Name.Trim(), StringComparison.Ordinal)).ToArray();
		if (keywords.Length > 0)
			Attach(proto, Component($"keyworddescriptions.{proto.Id}", "Keyword Descriptions", new XElement("Definition", KeywordDescriptionsXml(keywords))));
		if (descriptions.LastOrDefault(x => x.Keyword == "[DROP_DESC]") is { } dropDescription) proto.LongDescription = dropDescription.Description;
		// Authored scenery proxies whose source drop text explicitly says players must not see them.
		if (source.Vnum is 49525 or 49708)
			AddItemOnLoadProg(proto, "AdminInvisible", "addadmininviseffect(@item)\nreturn true");
		return proto;
	}

	private void AddItemBehavior(GameItemProto proto, ArmWorldItem source, int[] values, string variantKey, int forcedWearMask)
	{
		var itemType = source.ItemType;
		if (itemType == 20)
		{
			if (values[0] <= 0) throw new InvalidOperationException($"Currency item Vnum {source.Vnum} has nonpositive reset amount {values[0]} ({variantKey}).");
			var definition = new XElement("Definition", new XAttribute("Decorator", _db.StackDecorators.AsNoTracking().OrderBy(x => x.Id).First().Id),
				new XAttribute("DefaultCurrency", _currency.Id), new XAttribute("DefaultAmount", values[0].ToString(CultureInfo.InvariantCulture)));
			var currencyComponent = Component($"currency.{source.Vnum}.{variantKey}", "Currency Pile", definition);
			Attach(proto, currencyComponent);
		}
		if (itemType == 18)
		{
			var pattern = _keyPatterns.GetValueOrDefault(source.Vnum, checked(source.Vnum + 1));
			_keyPatterns[source.Vnum] = pattern;
			Attach(proto, Component("simplekey", "Simple Key", new XElement("Definition",
				new XElement("LockType", "Armageddon Worldfile Key"))));
			var onLoad = Prog("ArmageddonWorldfileKeyPattern" + proto.Id.ToString(CultureInfo.InvariantCulture),
				$"setlockpattern(@item, {pattern.ToString(CultureInfo.InvariantCulture)})\nreturn true",
				MudSharp.FutureProg.ProgVariableTypes.Boolean, ("item", MudSharp.FutureProg.ProgVariableTypes.Item));
			proto.GameItemProtosOnLoadProgs.Add(new GameItemProtosOnLoadProgs
			{
				GameItemProtoId = proto.Id, GameItemProtoRevisionNumber = proto.RevisionNumber,
				FutureProgId = onLoad.Id
			});
		}

		if (itemType is 5 or 24)
		{
			var combat = CreateMeleeComponent(source, values, variantKey);
			if (combat.MeleeComponent is not null) Attach(proto, combat.MeleeComponent);
			if (combat.RangedComponent is not null) Attach(proto, combat.RangedComponent);
			if (itemType == 5 && (source.Flags[1] & ((1 << 9) | (1 << 21))) != 0)
				Attach(proto, CreateProjectileAmmo(source, values, variantKey));
		}
		else if (itemType == 9)
		{
			var armourId = ResolveArmourType(source, values);
			var armour = Component($"armour.{source.Vnum}.{variantKey}", "Armour",
				new XElement("Definition", new XElement("ArmourType", armourId), new XElement("ApplyArmourPenalties", true)));
			Attach(proto, armour);
		}
		else if (itemType == 15) Attach(proto, CreateContainerComponent(proto, source, values, variantKey));
		else if (itemType == 17) Attach(proto, CreateLiquidContainerComponent(proto, source, values, variantKey));
		else if (itemType == 19) Attach(proto, CreateFoodComponent(source, values, variantKey));
		else if (itemType == 1) Attach(proto, CreateLightComponent(proto, source, values, variantKey));
		else if (itemType == 28) Attach(proto, CreateBowComponent(source, values, variantKey));
		else if (itemType == 23) AddEnterable(proto, source, values);
		else if (itemType == 32)
		{
			Attach(proto, CreateFurnitureComponent(source, values, variantKey));
			Attach(proto, Component($"furniturecontents.{source.Vnum}.{variantKey}", "Container", new XElement("Definition",
				new XAttribute("Weight", Math.Max(0, values[0]) / 100.0 * _kilograms), new XAttribute("MaxSize", (int)SizeCategory.Titanic),
				new XAttribute("Preposition", "on"), new XAttribute("Closable", false), new XAttribute("Transparent", true), new XAttribute("OnceOnly", false))));
		}

		var wearMask = (source.Flags[2] | forcedWearMask) & ~1;
		var supportedWearMask = WearBitLocations.Keys.Aggregate(0, (mask, bit) => mask | bit);
		var unsupportedWearMask = wearMask & ~(supportedWearMask | (1 << 16));
		if (unsupportedWearMask != 0)
			Adapt("item wear flags", source.Vnum, $"Unmapped equipment positions 13 or 14 have mask {unsupportedWearMask}; source mask is retained.");
		wearMask &= supportedWearMask;
		if ((source.Flags[2] & (1 << 16)) != 0)
			Attach(proto, Component("beltable", "Beltable", new XElement("Definition")));
		if ((source.Flags[2] & (1 << 15)) != 0)
			Attach(proto, Component("belt", "Belt", new XElement("Definition",
				new XAttribute("MaximumNumberOfBeltedItems", 2), new XAttribute("MaximumSize", (int)SizeCategory.Titanic))));
		if (wearMask != 0)
		{
			var wearable = GetWearableComponent(wearMask, SourceWearableSize(source.ItemType, values));
			Attach(proto, wearable);
		}
	}

	internal static int? SourceWearableSize(int sourceItemType, int[] effectiveValues) => sourceItemType switch
	{
		11 => effectiveValues[0],
		9 => effectiveValues[2],
		_ => null
	};

	internal static XElement SourceWearableSizing(int sourceSize, long scalarProgId) => new("Sizing",
		new XAttribute("enabled", true), new XAttribute("firstwear", false),
		new XAttribute("heightTolerance", 0.15), new XAttribute("weightTolerance", 0.30),
		new XAttribute("scalarProg", scalarProgId), new XAttribute("scalarDivisor", 20.0),
		new XAttribute("scalarFlat", 1.0),
		new XElement("Fit", new XAttribute("kind", "Scalar"), new XAttribute("height", 0),
			new XAttribute("weight", 0), new XAttribute("scalar", sourceSize)));

	private GameItemComponentProto CreateContainerComponent(GameItemProto proto, ArmWorldItem source, int[] values, string variantKey)
	{
		var sourceFlags = values[1];
		var capacityKg = Math.Max(0.0, values[0] - Math.Max(0, source.Physical[0])) / 100.0 * _kilograms;
		var keyVnum = values[2];
		var keyItemExists = keyVnum >= 0 && _source.Items.Any(x => x.Vnum == keyVnum && x.ItemType == 18);
		var lockedComponent = keyItemExists || (sourceFlags & 8) != 0;
		AddItemOnLoadProg(proto, "ContainerState", $"forceopen(@item, {((sourceFlags & 4) == 0 ? "true" : "false")}, false)\nsetlocked(@item, {((sourceFlags & 8) != 0 ? "true" : "false")}, false)\nreturn true");
		var definition = new XElement("Definition", new XAttribute("Weight", capacityKg),
			new XAttribute("MaxSize", (int)SizeCategory.Titanic), new XAttribute("Preposition", "in"),
			new XAttribute("Closable", (sourceFlags & 1) != 0), new XAttribute("Transparent", false),
			new XAttribute("OnceOnly", false));
		if (!lockedComponent)
			return Component($"container.{source.Vnum}.{variantKey}", "Container", definition);
		definition.Add(new XElement("ForceDifficulty", 2), new XElement("PickDifficulty", (sourceFlags & (1 << 1)) != 0 ? 10 : 2),
			new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$")),
			new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$")),
			new XElement("LockEmoteNoActor", new XCData("@ lock|locks")), new XElement("UnlockEmoteNoActor", new XCData("@ unlock|unlocks")),
			new XElement("LockType", "Armageddon Worldfile Key"));
		var component = Component($"lockingcontainer.{source.Vnum}.{variantKey}", "LockingContainer", definition);
		var pattern = keyItemExists ? checked(keyVnum + 1) : checked(int.MaxValue - (int)proto.Id);
		_initialLockPatterns[proto.Id] = pattern;
		if (keyItemExists) _keyPatterns[keyVnum] = pattern;
		else Adapt("container lock", source.Vnum, "Starts locked without an imported key; retained as a native pickable lock with an unpaired pattern.");
		AddItemOnLoadProg(proto, "ContainerPattern", $"setlockpattern(@item, {pattern.ToString(CultureInfo.InvariantCulture)})\nreturn true");
		return component;
	}

	private readonly Dictionary<long, (long Liquid, double Amount)> _initialLiquidContents = new();
	private readonly Dictionary<long, int> _initialLockPatterns = new();
	private static readonly string?[] NativeDrinkNames =
	[
		"water", "lager", "red wine", "amber ale", "dark ale", "whiskey", "amber ale", "whiskey", "red wine",
		"swamp water", "milk", "tea", "coffee", "Human Blood", "salt water", "vodka", null, null,
		"dark ale", "amber ale", "whiskey", "whiskey", "red wine", "red wine", "red wine", "red wine", "red wine",
		"red wine", "red wine", "red wine", "red wine"
	];
	private GameItemComponentProto CreateLiquidContainerComponent(GameItemProto proto, ArmWorldItem source, int[] values, string variantKey)
	{
		var litres = _db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Litre]).BaseMultiplier;
		var capacity = Math.Max(0.0, values[0]) * 0.05 * litres;
		var amount = Math.Clamp(values[1], 0, Math.Max(0, values[0])) * 0.05 * litres;
		var nativeName = values[2] >= 0 && values[2] < NativeDrinkNames.Length ? NativeDrinkNames[values[2]] : null;
		var liquid = nativeName is null ? null : _db.Liquids.SingleOrDefault(x => x.Name == nativeName)
			?? throw new InvalidOperationException($"Core/Human must provide native liquid {nativeName}.");
		if (amount > 0 && liquid is not null)
		{
			_initialLiquidContents[proto.Id] = (liquid.Id, amount);
			AddItemOnLoadProg(proto, "LiquidContents", $"setliquid(@item, {liquid.Id}, {amount.ToString("R", CultureInfo.InvariantCulture)})\nreturn true");
		}
		Adapt("item liquid container", source.Vnum,
			$"Source abstract capacity/current {values[0]}/{values[1]} use explicit seeder calibration 50 mL per unit, preserving the clamped fill fraction. Liquid index {values[2]} maps to {nativeName ?? "no qualified liquid (empty)"}; regional beverage flavour, spice and poison effects remain provenance.");
		return Component($"liquidcontainer.{source.Vnum}.{variantKey}", "Liquid Container", new XElement("Definition",
			new XAttribute("LiquidCapacity", capacity), new XAttribute("Closable", true), new XAttribute("Transparent", false),
			new XAttribute("WeightLimit", 0.0), new XAttribute("OnceOnly", false), new XAttribute("AdjustQuantityProg", 0),
			new XAttribute("DefaultLiquid", 0), new XAttribute("CanBeEmptiedWhenInRoom", true)));
	}

	private GameItemComponentProto CreateFoodComponent(ArmWorldItem source, int[] values, string variantKey)
	{
		var decorator = _db.StackDecorators.AsNoTracking().Single(x => x.Name == "Bites");
		return Component($"food.{source.Vnum}.{variantKey}", "Food", new XElement("Definition",
			new XAttribute("Satiation", Math.Max(0, values[0])), new XAttribute("Water", 0), new XAttribute("Thirst", 0),
			new XAttribute("Alcohol", 0), new XAttribute("Bites", 1), new XAttribute("Decorator", decorator.Id),
			new XElement("Taste", new XCData(source.Name)), new XElement("OnEatProg", 0)));
	}

	private GameItemComponentProto CreateLightComponent(GameItemProto proto, ArmWorldItem source, int[] values, string variantKey)
	{
		var illumination = Math.Max(1.0, values[3]);
		var permanent = values[0] < 0;
		var component = permanent
			? Component($"light.{source.Vnum}.{variantKey}", "Prog Light", new XElement("Definition",
				new XElement("IlluminationProvided", illumination), new XElement("MultiplyIlluminationBySpellGrade", false)))
			: Component($"torch.{source.Vnum}.{variantKey}", "Torch", new XElement("Definition",
				new XElement("IlluminationProvided", illumination), new XElement("SecondsOfFuel", Math.Max(0, values[0]) * 600),
				new XElement("RequiresIgnitionSource", false), new XElement("LightEmote", new XCData("@ light|lights $0.")),
				new XElement("ExtinguishEmote", new XCData("@ extinguish|extinguishes $0.")),
				new XElement("TenPercentFuelEcho", new XCData("$0 is nearly out.")), new XElement("FuelExpendedEcho", new XCData("$0 goes out."))));
		AttachLightStateProto(proto, values);
		return component;
	}

	private void AttachLightStateProto(GameItemProto proto, int[] values)
	{
		if ((values[5] & 1) == 0) return;
		AddItemOnLoadProg(proto, "LightState", "setlit(@item, true)\nreturn true");
	}

	private void AddItemOnLoadProg(GameItemProto? proto, string suffix, string text)
	{
		if (proto is null) return;
		var prog = Prog($"ArmageddonWorldfile{suffix}{proto.Id}", text,
			MudSharp.FutureProg.ProgVariableTypes.Boolean, ("item", MudSharp.FutureProg.ProgVariableTypes.Item));
		proto.GameItemProtosOnLoadProgs.Add(new GameItemProtosOnLoadProgs
		{
			GameItemProtoId = proto.Id, GameItemProtoRevisionNumber = proto.RevisionNumber, FutureProgId = prog.Id
		});
	}

	private GameItemComponentProto CreateBowComponent(ArmWorldItem source, int[] values, string variantKey)
	{
		var family = SourceRangedFamily(source.Vnum, values[2]);
		var native = family == RangedWeaponType.Bow
			? _db.RangedWeaponTypes.AsNoTracking().Single(x => x.Name == (source.Name.Contains("long", StringComparison.OrdinalIgnoreCase) ? "Longbow" : "Shortbow"))
			: _db.RangedWeaponTypes.AsNoTracking().Where(x => x.RangedWeaponType == (int)family).OrderBy(x => x.Id).First();
		var ranged = CloneRangedWeaponType(native, source, variantKey,
			native.DamageBonusExpression, Math.Clamp(values[0], 1, 20));
		ranged.SpecificAmmunitionGrade = "Armageddon Projectile";
		var componentType = family.ToString();
		Adapt("item ranged weapon", source.Vnum, $"Native {componentType} loading uses source arrow/bolt flags and projectile damage. Source loaded-ammunition slot {values[3]} is archived; imported weapons begin unloaded. Native range/timing/checks remain; source pull strength {values[1]} is not an added damage bonus.");
		var definition = new XElement("Definition",
			new XElement("RangedWeaponType", ranged.Id), new XElement("StaminaPerTick", 1),
			new XElement("MeleeWeaponType", _db.WeaponTypes.First(x => x.Name == "Club").Id), new XElement("CanWieldProg", 0), new XElement("WhyCannotWieldProg", 0));
		if (family == RangedWeaponType.Bow && values[1] > 0)
		{
			var strength = _db.TraitDefinitions.AsNoTracking().Where(x => x.Type == 1 &&
				(x.Name == "Strength" || x.Name == "Upper Body Strength")).OrderBy(x => x.Name == "Strength" ? 0 : 1).FirstOrDefault()
				?? throw new InvalidOperationException("Bow draw strength needs a native Strength or Upper Body Strength attribute.");
			var gate = Prog($"ArmageddonWorldfileBowStrength{ranged.Id}", $"return gettrait(@actor, totrait({strength.Id})) + 3 >= {values[1]}",
				MudSharp.FutureProg.ProgVariableTypes.Boolean, ("actor", MudSharp.FutureProg.ProgVariableTypes.Character), ("item", MudSharp.FutureProg.ProgVariableTypes.Item));
			var why = Prog($"ArmageddonWorldfileBowStrengthWhy{ranged.Id}", "return \"You are not strong enough to draw this bow.\"",
				MudSharp.FutureProg.ProgVariableTypes.Text, ("actor", MudSharp.FutureProg.ProgVariableTypes.Character), ("item", MudSharp.FutureProg.ProgVariableTypes.Item));
			definition.SetElementValue("CanWieldProg", gate.Id);
			definition.SetElementValue("WhyCannotWieldProg", why.Id);
			Adapt("bow strength", source.Vnum, "Source strength + 3 draw threshold is enforced by the native wield permission, earlier than the source fire-time check.");
		}
		if (family == RangedWeaponType.Crossbow)
			definition.Add(new XElement("RequiredSpanningToolTag", 0), new XElement("ReadyEmote", new XCData("@ ready|readies $1.")),
				new XElement("MagazineCapacity", 0), new XElement("RequiresEmplacement", false), new XElement("RepeatsWithoutReady", false));
		return Component($"ranged.{source.Vnum}.{variantKey}", componentType, definition);
	}

	internal static RangedWeaponType SourceRangedFamily(int vnum, int loading) => vnum switch
	{
		14400 => RangedWeaponType.Sling, 39033 or 50387 => RangedWeaponType.Blowgun,
		_ => loading == 0 ? RangedWeaponType.Bow : RangedWeaponType.Crossbow
	};

	private GameItemComponentProto CreateProjectileAmmo(ArmWorldItem source, int[] values, string variantKey)
	{
		var arrow = (source.Flags[1] & (1 << 9)) != 0;
		var family = arrow ? (int)RangedWeaponType.Bow : (int)RangedWeaponType.Crossbow;
		var baseline = _db.AmmunitionTypes.AsNoTracking().AsEnumerable().First(x =>
			x.RangedWeaponTypes.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(family.ToString(CultureInfo.InvariantCulture)));
		var ammo = (AmmunitionTypes)_db.Entry(baseline).CurrentValues.ToObject();
		ammo.Id = Next<AmmunitionTypes>();
		ammo.Name = $"Armageddon {Key(source.Vnum)} {variantKey}";
		ammo.SpecificType = "Armageddon Projectile";
		ammo.RangedWeaponTypes = string.Join(" ", new[]
		{
			arrow ? ((int)RangedWeaponType.Bow).ToString(CultureInfo.InvariantCulture) : null,
			(source.Flags[1] & (1 << 21)) != 0 ? $"{(int)RangedWeaponType.Crossbow} {(int)RangedWeaponType.Sling} {(int)RangedWeaponType.Blowgun}" : null
		}.Where(x => x is not null));
		var roll = values[1] > 0 && values[2] > 0 ? $"dice({values[1]},{values[2]})+({values[0]})" : values[0].ToString(CultureInfo.InvariantCulture);
		ammo.DamageExpression = $"max(0,{roll})";
		ammo.PainExpression = ammo.StunExpression = $"max(0,({roll})/2)";
		ammo.DamageType = (int)ResolveWeaponDamageType(source, values);
		_db.AmmunitionTypes.Add(ammo);
		return Component($"ammunition.{source.Vnum}.{variantKey}", "Ammunition", new XElement("Definition", new XElement("AmmoType", ammo.Id)));
	}

	private RangedWeaponTypes CloneRangedWeaponType(RangedWeaponTypes native, ArmWorldItem source, string variantKey,
		string damageExpression, int range)
	{
		var ranged = new RangedWeaponTypes
		{
			Id = Next<RangedWeaponTypes>(), Name = $"Armageddon Vnum {source.Vnum} {variantKey}",
			Classification = native.Classification, FireTraitId = native.FireTraitId, OperateTraitId = native.OperateTraitId,
			FireableInMelee = native.FireableInMelee, DefaultRangeInRooms = range,
			AccuracyBonusExpression = native.AccuracyBonusExpression, DamageBonusExpression = damageExpression,
			AmmunitionLoadType = native.AmmunitionLoadType, SpecificAmmunitionGrade = native.SpecificAmmunitionGrade,
			AmmunitionCapacity = native.AmmunitionCapacity, RangedWeaponType = native.RangedWeaponType,
			StaminaToFire = native.StaminaToFire, StaminaPerLoadStage = native.StaminaPerLoadStage,
			CoverBonus = native.CoverBonus, BaseAimDifficulty = native.BaseAimDifficulty, LoadDelay = native.LoadDelay,
			ReadyDelay = native.ReadyDelay, FireDelay = native.FireDelay, AimBonusLostPerShot = native.AimBonusLostPerShot,
			RequiresFreeHandToReady = native.RequiresFreeHandToReady, AlwaysRequiresTwoHandsToWield = native.AlwaysRequiresTwoHandsToWield,
			MinimumFiringPositionStateId = native.MinimumFiringPositionStateId
		};
		_db.RangedWeaponTypes.Add(ranged);
		return ranged;
	}

	private GameItemComponentProto CreateFurnitureComponent(ArmWorldItem source, int[] values, string variantKey)
	{
		if (source.Name.Contains("chair", StringComparison.OrdinalIgnoreCase))
			return Component($"chair.{source.Vnum}.{variantKey}", "Chair", new XElement("Definition",
				new XAttribute("ChairSlotsUsed", 1), new XAttribute("ChairOccupantCapacity", Math.Max(1, values[2]))));
		return Component($"table.{source.Vnum}.{variantKey}", "Table", new XElement("Definition",
			new XAttribute("MaximumChairSlots", Math.Max(0, values[2])),
			new XElement("Cover", new XElement("Flipped", 0), new XElement("NotFlipped", 0),
				new XElement("Expression", new XCData("1")), new XElement("Message", new XCData("It cannot be flipped.")))));
	}

	private long ResolveArmourType(ArmWorldItem source, int[] values)
	{
		var name = source.Name;
		var type = name.Contains("plate", StringComparison.OrdinalIgnoreCase) ? 5
			: name.Contains("chain", StringComparison.OrdinalIgnoreCase) || name.Contains("mail", StringComparison.OrdinalIgnoreCase) ? 4
			: name.Contains("scale", StringComparison.OrdinalIgnoreCase) ? 3
			: name.Contains("leather", StringComparison.OrdinalIgnoreCase) ? 2
			: source.Material is 2 or 3 or 4 ? 1 : 0;
		var key = type + ":" + Math.Max(0, values[0]);
		if (_nativeArmourTypes.TryGetValue(key, out var existing)) return existing;
		var baseline = _db.ArmourTypes.Single(x => x.Id == _nativeArmourTypes[type.ToString(CultureInfo.InvariantCulture)]);
		var clone = (ArmourType)_db.Entry(baseline).CurrentValues.ToObject();
		clone.Id = Next<ArmourType>();
		clone.Name = $"Armageddon {baseline.Name} {Math.Max(0, values[0])} AP";
		var xml = XElement.Parse(baseline.Definition);
		var reduction = (Math.Max(0, values[0]) / 10.0).ToString("R", CultureInfo.InvariantCulture);
		foreach (var (group, variable) in new[] { ("DissipateExpressions", "damage"), ("DissipateExpressionsPain", "pain"), ("DissipateExpressionsStun", "stun"),
			("AbsorbExpressions", "damage"), ("AbsorbExpressionsPain", "pain"), ("AbsorbExpressionsStun", "stun") })
		{
			var element = xml.Element(group) ?? new XElement(group);
			if (element.Parent is null) xml.Add(element);
			foreach (var damageType in Enum.GetValues<DamageType>())
			{
				if (damageType is not (DamageType.Slashing or DamageType.Chopping or DamageType.Crushing or DamageType.Piercing or DamageType.Bite or DamageType.Claw or DamageType.Shearing)) continue;
				var expression = element.Elements("Expression").FirstOrDefault(x => (int?)x.Attribute("damagetype") == (int)damageType);
				if (expression is null) { expression = new XElement("Expression", new XAttribute("damagetype", (int)damageType)); element.Add(expression); }
				expression.Value = group.StartsWith("Dissipate", StringComparison.Ordinal) ? variable : $"max(0,{variable}-({reduction}))";
			}
		}
		clone.Definition = xml.ToString();
		_db.ArmourTypes.Add(clone);
		_nativeArmourTypes.Add(key, clone.Id);
		Adapt("item armour", source.Vnum, $"Native {baseline.Name} profile uses source AP/10 expected absorption ({reduction}); native condition/penetration and damage-type transformations remain. Source random remainder and material wear are approximated.");
		return clone.Id;
	}

	private WearProfile[] GetWearProfiles(int bit)
	{
		if (_wearProfiles.TryGetValue(bit, out var cached)) return cached;
		var locations = WearBitLocations[bit];
		var alternatives = bit is (1 << 1) or (1 << 12) or (1 << 19) or (1 << 25)
			? locations.Select(x => new[] { x }).ToArray()
			: bit == (1 << 26) ? new[] { new[] { "uback", "rshoulder" }, new[] { "uback", "lshoulder" } }
			: new[] { locations };
		var bodyIds = BodyAncestry(_humanBodyId);
		var bodypartNames = _db.BodypartProtos.AsNoTracking().Where(x => bodyIds.Contains(x.BodyId))
			.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var missing = alternatives.SelectMany(x => x).Where(x => !bodypartNames.Contains(x)).Distinct().ToArray();
		if (missing.Length != 0)
			throw new InvalidOperationException($"Human body is missing wear locations: {string.Join(", ", missing)}.");
		var profiles = alternatives.Select((parts, index) => new WearProfile
		{
			Id = Next<WearProfile>(), BodyPrototypeId = _humanBodyId, Name = $"Armageddon wear {bit} alternative {index + 1}", Type = "Direct",
			Description = "Imported alternative equipment location: " + string.Join(", ", parts),
			WearStringInventory = "worn on", WearAction1st = "put", WearAction3rd = "puts", WearAffix = "on",
			RequireContainerIsEmpty = false,
			WearlocProfiles = new XElement("Profiles", parts.Select(x => new XElement("Profile",
				new XAttribute("Bodypart", x), new XAttribute("Transparent", false), new XAttribute("NoArmour", false),
				new XAttribute("PreventsRemoval", true), new XAttribute("Mandatory", true), new XAttribute("HidesSevered", false)))).ToString()
		}).ToArray();
		_db.WearProfiles.AddRange(profiles);
		_wearProfiles.Add(bit, profiles);
		return profiles;
	}

	private GameItemComponentProto GetWearableComponent(int wearMask, int? sourceSize = null)
	{
		if (sourceSize is null && _wearProfileComponents.TryGetValue(wearMask, out var existing))
			return _db.GameItemComponentProtos.Local.Single(x => x.Id == existing);
		if (sourceSize is not null && _sizedWearProfileComponents.TryGetValue((wearMask, sourceSize.Value), out existing))
			return _db.GameItemComponentProtos.Local.Single(x => x.Id == existing);
		var profiles = WearBitLocations.Keys.Where(bit => (wearMask & bit) != 0).SelectMany(GetWearProfiles).ToArray();
		if (profiles.Length == 0) throw new InvalidOperationException($"No native wear locations for mask {wearMask}.");
		var definition = new XElement("Definition",
			new XAttribute("DisplayInventoryWhenWorn", true), new XAttribute("Bulky", false),
			new XElement("WearableProg", 0), new XElement("WhyCannotWearProg", 0),
			new XElement("SeeThroughDamageRatio", 0.5), new XElement("LayerWeightConsumption", 1),
			new XElement("Waterproof", new XAttribute("ratio", 0.5), false),
			new XElement("Profiles", new XAttribute("Default", profiles[0].Id), profiles.Select(x => new XElement("Profile", x.Id))));
		if (sourceSize is { } fitSize) definition.Add(SourceWearableSizing(fitSize, _wearableScalarSizeProgId));
		var key = sourceSize is null ? "wearable." + wearMask : $"wearable.{wearMask}.size.{sourceSize.Value}";
		var component = Component(key, "Wearable", definition);
		if (sourceSize is null) _wearProfileComponents.Add(wearMask, component.Id);
		else _sizedWearProfileComponents.Add((wearMask, sourceSize.Value), component.Id);
		return component;
	}

	internal static (int Bit, int Index) SourceEquipmentLocation(int slot) => slot switch
		{
			0 => (15, 0), 1 => (1, 0), 2 => (1, 1), 3 => (2, 0), 4 or 39 => (9, 0),
			5 => (3, 0), 6 => (4, 0), 7 => (5, 0), 8 => (6, 0), 9 => (7, 0), 10 => (8, 0),
			12 => (10, 0), 13 => (11, 0), 14 => (12, 0), 15 => (12, 1), 20 => (17, 0), 21 => (18, 0),
			22 => (19, 0), 23 => (20, 0), 24 => (21, 0), 25 => (22, 0), 26 => (23, 0), 27 => (19, 1),
			28 => (1, 2), 29 => (1, 3), 30 => (1, 4), 31 => (1, 5), 32 => (1, 6), 33 => (1, 7),
			34 => (1, 8), 35 => (1, 9), 36 => (24, 0), 37 => (25, 0), 38 => (25, 1),
			40 => (26, 0), 41 => (26, 1), _ => (-1, 0)
		};

	internal static int ResetForcedWearMask(ArmWorldItem item, int slot)
	{
		var (bit, _) = SourceEquipmentLocation(slot);
		return bit < 0 ? 0 : (1 << bit) & ~item.Flags[2];
	}

	private long? SourceEquipmentProfile(ArmWorldItem item, int slot, bool resetOverride = false)
	{
		var (bit, index) = SourceEquipmentLocation(slot);
		return bit >= 0 && (resetOverride || (item.Flags[2] & (1 << bit)) != 0) ? GetWearProfiles(1 << bit)[index].Id : null;
	}

	private (GameItemComponentProto? MeleeComponent, GameItemComponentProto? RangedComponent) CreateMeleeComponent(ArmWorldItem source, int[] values, string variantKey)
	{
		var skillType = source.ItemType == 24 ? "Knife" : values[3] switch
		{
			1 or 7 or 8 => "Club", 2 or 3 or 9 or 10 or 11 or 12 or 13 or 14 => "Short Spear",
			4 => "Axe", 5 => "Longsword", 6 or 16 or 17 => "Knife", 15 => "Long Spear", _ => "Club"
		};
		var baseType = _db.WeaponTypes.AsNoTracking().SingleOrDefault(x => x.Name == skillType)
			?? throw new InvalidOperationException($"Combat must provide native weapon type {skillType} for source weapon Vnum {source.Vnum}.");
		var combatKey = $"{source.Vnum}.{variantKey}";
		var type = new WeaponType
		{
			Id = Next<WeaponType>(), Name = "Armageddon Vnum " + source.Vnum + " " + variantKey,
			Classification = baseType.Classification, AttackTraitId = baseType.AttackTraitId, ParryTraitId = baseType.ParryTraitId,
			ParryBonus = baseType.ParryBonus, Reach = baseType.Reach, StaminaPerParry = baseType.StaminaPerParry
		};
		_db.WeaponTypes.Add(type);
		var template = _db.WeaponAttacks.AsNoTracking().Where(x => x.WeaponTypeId == baseType.Id).OrderBy(x => x.Id).FirstOrDefault()
			?? throw new InvalidOperationException($"Native Combat weapon type {skillType} has no attack template.");
		var dice = Math.Max(0, values[1]);
		var sides = Math.Max(0, values[2]);
		var bonus = values[0];
		var roll = dice > 0 && sides > 0 ? $"dice({dice},{sides})+({bonus})" : bonus.ToString(CultureInfo.InvariantCulture);
		var damage = AddWeaponExpression(source.Vnum, variantKey, "damage", $"max(0,{roll})");
		var pain = AddWeaponExpression(source.Vnum, variantKey, "pain", $"max(0,({roll})/2)");
		var stun = AddWeaponExpression(source.Vnum, variantKey, "stun", $"max(0,({roll})/2)");
		var damageType = source.ItemType == 24 ? DamageType.Piercing : ResolveWeaponDamageType(source, values);
		var attack = CopyWeaponAttack(template, type.Id, damage, pain, stun, damageType,
			"Armageddon Vnum " + source.Vnum + " attack");
		_db.WeaponAttacks.Add(attack);
		if (source.ItemType == 24)
		{
			var nativeThrown = _db.RangedWeaponTypes.AsNoTracking().Where(x => x.RangedWeaponType == (int)RangedWeaponType.Thrown)
				.OrderBy(x => x.Name == "Throwing Knife" ? 0 : 1).ThenBy(x => x.Id).First();
			var ranged = CloneRangedWeaponType(nativeThrown, source, variantKey, $"max(0,{roll})", nativeThrown.DefaultRangeInRooms);
			return (Component("dart." + combatKey, "ThrownWeapon", new XElement("Definition",
				new XElement("MeleeWeaponType", type.Id), new XElement("RangedWeaponType", ranged.Id))), null);
		}
		var component = Component("melee." + combatKey, "MeleeWeapon",
			new XElement("Definition", new XElement("WeaponType", type.Id), new XElement("CanWieldProg", 0), new XElement("WhyCannotWieldProg", 0)));
		return (component, null);
	}

	private TraitExpression AddWeaponExpression(int vnum, string state, string kind, string expression)
	{
		var result = new TraitExpression { Id = Next<TraitExpression>(), Name = $"Armageddon Vnum {vnum} {state} {kind}", Expression = expression };
		_db.TraitExpressions.Add(result);
		return result;
	}

	private static DamageType ResolveWeaponDamageType(ArmWorldItem source, int[] values)
	{
		var sourceHitType = values[3] switch
		{
			0 or 1 or 8 => DamageType.Crushing,
			2 or 3 or 9 or 13 or 14 or 16 => DamageType.Piercing,
			4 => DamageType.Chopping,
			5 or 15 or 17 => DamageType.Slashing,
			6 => DamageType.Claw,
			7 => DamageType.Shearing,
			10 or 11 => DamageType.Bite,
			12 => DamageType.Piercing,
			_ => (DamageType?)null
		};
		if (sourceHitType.HasValue) return sourceHitType.Value;
		var name = source.Name;
		if (new[] { "club", "mace", "hammer", "blunt", "staff", "maul", "sap" }.Any(x => name.Contains(x, StringComparison.OrdinalIgnoreCase)))
			return DamageType.Crushing;
		if (new[] { "spear", "arrow", "bolt", "dagger", "piercing", "point" }.Any(x => name.Contains(x, StringComparison.OrdinalIgnoreCase)))
			return DamageType.Piercing;
		return DamageType.Slashing;
	}

	private WeaponAttack CopyWeaponAttack(WeaponAttack source, long weaponTypeId, TraitExpression damage,
		TraitExpression pain, TraitExpression stun, DamageType damageType, string name) => new()
	{
		Id = Next<WeaponAttack>(), WeaponTypeId = weaponTypeId, Verb = source.Verb, FutureProgId = source.FutureProgId,
		BaseAttackerDifficulty = source.BaseAttackerDifficulty, BaseBlockDifficulty = source.BaseBlockDifficulty,
		BaseDodgeDifficulty = source.BaseDodgeDifficulty, BaseParryDifficulty = source.BaseParryDifficulty,
		BaseAngleOfIncidence = source.BaseAngleOfIncidence, RecoveryDifficultySuccess = source.RecoveryDifficultySuccess,
		RecoveryDifficultyFailure = source.RecoveryDifficultyFailure, MoveType = source.MoveType, Intentions = source.Intentions,
		ExertionLevel = source.ExertionLevel, DamageType = (int)damageType, DamageExpressionId = damage.Id,
		StunExpressionId = stun.Id, PainExpressionId = pain.Id, Weighting = source.Weighting,
		MaximumTargets = source.MaximumTargets, BodypartShapeId = source.BodypartShapeId, StaminaCost = source.StaminaCost,
		BaseDelay = source.BaseDelay, Name = name, Orientation = source.Orientation, Alignment = source.Alignment,
		AdditionalInfo = source.AdditionalInfo, HandednessOptions = source.HandednessOptions,
		RequiredPositionStateIds = source.RequiredPositionStateIds, OnUseProgId = source.OnUseProgId
	};
}

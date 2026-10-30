#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private readonly FuturemudDatabaseContext _db;
	private readonly ArmageddonWorldfilePackage _source;
	private readonly ArmageddonWorldfilePackage _archive;
	private readonly ArmageddonInstallationSettings _settings;
	private readonly Dictionary<Type, long> _nextIds = new();
	private readonly Dictionary<int, Zone> _zones = new();
	private readonly Dictionary<int, Room> _rooms = new();
	private readonly Dictionary<int, GameItemProto> _items = new();
	private readonly Dictionary<int, NpcTemplate> _npcs = new();
	private readonly Dictionary<int, Race> _races = new();
	private readonly Dictionary<int, Clan> _clans = new();
	private readonly Dictionary<int, Material> _materials = new();
	private readonly Dictionary<string, GameItemComponentProto> _components = new(StringComparer.Ordinal);
	private readonly List<object> _adaptations = [];
	private readonly List<FutureProg> _progs = [];
	private readonly Dictionary<string, int> _counts = new(StringComparer.Ordinal);
	private readonly long _builder;
	private readonly double _kilograms;
	private RoomOverlayPackage _overlays = null!;
	private Currency _currency = null!;
	private long _emptyForageProfile;
	private readonly Dictionary<int, long> _forageProfiles = new();
	private readonly Dictionary<int, int> _keyPatterns = new();
	public string Report { get; private set; } = "";

	internal ArmageddonWorldfileWriter(FuturemudDatabaseContext db, ArmageddonWorldfilePackage source)
	{
		_db = db;
		_archive = source;
		_source = source.ForInstallation();
		_settings = ArmageddonInstallation.Read(db)!;
		_builder = _settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount];
		_kilograms = db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
	}

	internal static IReadOnlyList<string> Preflight(FuturemudDatabaseContext db, ArmageddonWorldfilePackage source)
	{
		source = source.ForInstallation();
		var errors = new List<string>();
		ArmageddonInstallationSettings? settings = null;
		try
		{
			settings = ArmageddonInstallation.Read(db);
			if (settings is null) errors.Add("Install Armageddon Setting with its native foundations first.");
			else errors.AddRange(ArmageddonInstallation.ValidateFoundations(db, settings));
		}
		catch (InvalidOperationException error) { errors.Add(error.Message); }
		if (!db.Shards.Any() || !db.Zones.Any()) errors.Add("Core and Time must supply a native shard and zone.");
		if (!db.Clocks.Any() || !db.Calendars.Any()) errors.Add("Install native clocks and calendars with Time.");
		var hp = db.HealthStrategies.AsNoTracking().Where(x => x.Type == "BrainHitpoints" && x.Name == "Human HP").ToArray();
		if (hp.Length != 1) errors.Add("Human must supply exactly one BrainHitpoints strategy named Human HP.");
		else
		{
			try
			{
				var xml = XElement.Parse(hp[0].Definition);
				if (!long.TryParse(xml.Element("MaximumHitPointsExpression")?.Value, out var expression) ||
				    !db.TraitExpressions.Any(x => x.Id == expression)) errors.Add("Human HP needs a valid maximum hit-points expression.");
			}
			catch (System.Xml.XmlException) { errors.Add("Human HP has invalid XML; repair the Human health foundation."); }
		}
		foreach (var name in new[] { "Club", "Short Spear", "Long Spear", "Axe", "Longsword", "Knife" })
		{
			var types = db.WeaponTypes.Where(x => x.Name == name).Select(x => x.Id).ToArray();
			if (types.Length != 1 || !db.WeaponAttacks.Any(x => x.WeaponTypeId == types[0]))
				errors.Add($"Combat must supply one {name} weapon type with a native attack template.");
		}
		foreach (var name in new[] { "Heavy Clothing", "Ultra Heavy Clothing", "Boiled Leather", "Metal Scale", "Chainmail", "Platemail" })
			if (db.ArmourTypes.Count(x => x.Name == name) != 1) errors.Add($"Combat must supply one armour type named {name}.");
		foreach (var name in new[] { "Shortbow", "Longbow" })
			if (db.RangedWeaponTypes.Count(x => x.Name == name) != 1) errors.Add($"Combat must supply one ranged weapon type named {name}.");
		if (!db.RangedWeaponTypes.Any(x => x.RangedWeaponType == (int)MudSharp.Combat.RangedWeaponType.Thrown))
			errors.Add("Combat must supply a native thrown weapon type for imported darts.");
		foreach (var name in RequiredBodies)
			if (!db.BodyProtos.Any(x => x.Name == name)) errors.Add($"Animal foundations must supply body {name}.");
		foreach (var name in RaceBodyByType.Values.Distinct())
			if (!db.Races.Any(x => db.BodyProtos.Any(b => b.Id == x.BaseBodyId && b.Name == name)))
				errors.Add($"Animal must supply a baseline race using {name} anatomy.");
		foreach (var name in SectorTerrains.Distinct())
			if (!db.Terrains.Any(x => x.Name == name)) errors.Add($"Core must supply terrain {name}.");
		if (!db.StackDecorators.Any(x => x.Name == "Bites")) errors.Add("Core must supply the Bites stack decorator.");
		foreach (var race in source.Npcs.Select(x => x.Identity[6]).Distinct())
			if (source.PhysicalGenerators.Count(x => x.Race == race) != 1)
				errors.Add($"The packaged source needs exactly one physical generator for NPC race {race}.");
		if (settings is not null && errors.Count == 0)
		{
			try { BindMaterials(db, settings, source.Items.Select(x => x.Material).Append(3)); }
			catch (InvalidOperationException error) { errors.Add(error.Message); }
		}
		void Collisions(IEnumerable<string?> names, IEnumerable<int> vnums, string type)
		{
			var existing = names.Where(x => x is not null).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var collision = vnums.Select(Key).FirstOrDefault(existing.Contains);
			if (collision is not null) errors.Add($"Existing {type} unique name {collision} conflicts with this import; preserve and resolve it explicitly.");
		}
		Collisions(db.Rooms.AsNoTracking().Select(x => x.UniqueName).AsEnumerable(), source.Rooms.Select(x => x.Vnum), "room");
		Collisions(db.GameItemProtos.AsNoTracking().Select(x => x.UniqueName).AsEnumerable(), source.Items.Select(x => x.Vnum), "item");
		Collisions(db.NpcTemplates.AsNoTracking().Select(x => x.UniqueName).AsEnumerable(), source.Npcs.Select(x => x.Vnum), "NPC template");
		return errors;
	}

	internal void Install()
	{
		Adapt("excluded zone", 76, "Nessalin's playground: zone, room 76000 and zone reset commands deliberately excluded; object definitions reused elsewhere remain eligible.");
		var admittedItems = _source.Items.Select(x => x.Vnum).ToHashSet();
		foreach (var item in _archive.Items.Where(x => !admittedItems.Contains(x.Vnum)))
		{
			var reason = ArmageddonWorldfilePackage.ExcludedEnterables.Contains(item.Vnum)
				? "Missing entrance room; unused in retained-world resets and shop production. Deliberately excluded rather than inventing an interior."
				: "Creator playground-only definition; no retained-world reset, shop, forage, craft, key or public C source dependency.";
			Adapt("excluded item", item.Vnum, reason);
			Record("excluded.item." + Key(item.Vnum), "ArmageddonExcludedItem", null, null, new { Source = item, Reason = reason });
		}
		foreach (var zoneIndex in ArmageddonWorldfilePackage.ExcludedPlaygroundZones)
			Record($"excluded.zone.{zoneIndex}", "ArmageddonExcludedZone", null, null, new
			{
				Zone = _archive.Zones.SingleOrDefault(x => x.Index == zoneIndex), Rooms = _archive.Rooms.Where(x => x.Zone == zoneIndex),
				Resets = _archive.Resets.Where(x => x.Zone == zoneIndex), Reason = "Creator playground deliberately excluded."
			});
		foreach (var npc in _archive.Npcs.Where(x => x.Source.Path == "npcs/z57.npc"))
			Record($"excluded.npc.{npc.Vnum}", "ArmageddonExcludedNpc", null, null,
				new { Source = npc, Reason = "Unused zone 57 playground definition; no retained-world references." });
		var marker = Record("installation", "ArmageddonWorldfileInstallation", null, null,
			new { State = "Creating", _source.Fingerprint });
		Flush();
		Console.WriteLine("Armageddon Worldfile: creating rooms and exits...");
		InstallRooms();
		InstallClansAndCurrency();
		InstallLegacyUntal();
		InstallWillpowerSkill();
		Console.WriteLine("Armageddon Worldfile: creating equipment...");
		InstallItems();
		InstallTables();
		Console.WriteLine("Armageddon Worldfile: creating NPC definitions...");
		InstallNpcTemplates();
		ConfigureTunnelCosts();
		InstallForagingAndCrafts();
		Console.WriteLine("Armageddon Worldfile: configuring population...");
		InstallPopulation();
		InstallGanZienCamp();
		InstallArenaRelease();
		Console.WriteLine("Armageddon Worldfile: creating permanent shops and stock...");
		InstallShops();
		InstallNativeServices();
		InstallNpcBehaviours();
		InstallVendorServices();
		InstallWagonStorage();
		InstallTailoring();
		InstallCustomClothing();
		InstallPostal();
		InstallNpcCommandSpecials();
		InstallRoomCommands();
		InstallNarrativeSpecials();
		InstallHealerSpecials();
		InstallGamblingSpecials();
		InstallTraderSpecials();
		InstallNpcReactions();
		InstallSandstorms();
		InstallLightning();
		InstallItemFlavour();
		InstallPlantGrowth();
		InstallHuntingWhistles();
		InstallKruthDecks();
		InstallBoards();
		InstallBindingItems();
		InstallRubyPendant();
		InstallItemGames();
		InstallLegacyPoisonEffects();
		InstallBandaging();
		InstallPoisonDelivery();
		InstallPoisonCleansing();
		InstallNoteComponents();
		InstallWritingImplements();
		InstallPlacedMasks();
		InstallPlacedSpellComponents();
		InstallPlacedTravelRunes();
		InstallPlacedMagicDevices();
		InstallHealingPotions();
		InstallSpiceConsumption();
		InstallMulRage();
		InstallGuardSpecials();
		InstallMantisVendors();
		InstallNpcInitialStates();
		InstallJustice();
		InstallJailNeeds();
		FinalizeInteriorTemplates();
		FinalizeAuxiliaryRooms();
		if (ArmageddonChargenPolicy.ConfigureWorldfileStart(_db))
			Adapt("character starting location", 50615, "The stock Guest Lounge chargen start now begins at the Gaj in Allanak. Custom starting locations and commencement progs are preserved.");
		FinalizeNpcLoadPrograms();
		Console.WriteLine("Armageddon Worldfile: validating generated programs...");
		CompileProgs();
		marker.SeedBaseline = JsonSerializer.Serialize(new { State = "Complete", Source = _source.Fingerprint,
			Counts = _counts, Adaptations = _adaptations, SourceManifest = _source.Manifest });
		marker.AppliedFingerprint = Hash(marker.SeedBaseline);
		Flush();
		Report = "Armageddon world created. " + string.Join(", ", _counts.Select(x => $"{x.Value:N0} {x.Key}")) +
			$". {_adaptations.Count:N0} explicit adaptation/omission records are retained with source provenance. " +
			"This world cannot be installed again.";
	}

	private static string Key(int vnum) => "Vnum" + vnum.ToString(CultureInfo.InvariantCulture);
	private long[] BodyAncestry(long bodyId)
	{
		var ids = new HashSet<long>();
		long? current = bodyId;
		while (current.HasValue && ids.Add(current.Value))
			current = _db.BodyProtos.Where(x => x.Id == current.Value).Select(x => x.CountsAsId).Single();
		return ids.ToArray();
	}
	private static string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
	private long Next<T>() where T : class
	{
		if (!_nextIds.TryGetValue(typeof(T), out var value))
			value = (_db.Set<T>().Max(x => (long?)EF.Property<long>(x, "Id")) ?? 0) + 1;
		_nextIds[typeof(T)] = value + 1;
		return value;
	}

	private EditableItem Approval() => new()
	{
		BuilderAccountId = _builder, ReviewerAccountId = _builder, BuilderDate = DateTime.UtcNow,
		ReviewerDate = DateTime.UtcNow, RevisionStatus = (int)RevisionStatus.Current, RevisionNumber = 0
	};

	private void Flush()
	{
		_db.ChangeTracker.DetectChanges();
		_db.SaveChanges();
	}

	private SeederManagedRecord Record(string key, string type, long? id, int? revision, object raw)
	{
		var json = JsonSerializer.Serialize(raw);
		var record = new SeederManagedRecord
		{
			Seeder = ArmageddonWorldfileSeeder.PackageName, Module = "worldfile-provenance", StableKey = key,
			EntityType = type, LogicalId = id, RevisionNumber = revision, SeedBaseline = json,
			AppliedFingerprint = Hash(json), ManifestVersion = "worldfile-1", AppliedAt = DateTime.UtcNow
		};
		_db.SeederManagedRecords.Add(record);
		return record;
	}

	private void Adapt(string type, int sourceId, string reason) => _adaptations.Add(new { Type = type, SourceId = sourceId, Reason = reason });
	private static XElement KeywordDescriptionsXml(IEnumerable<ArmWorldExtra> descriptions) => new("KeywordDescriptions",
		descriptions.Select(x => new XElement("Entry", new XElement("Keyword", x.Keyword), new XElement("Prose", x.Description))));
	private GameItemComponentProto Component(string key, string type, XElement definition)
	{
		if (_components.TryGetValue(key, out var existing)) return existing;
		var component = new GameItemComponentProto
		{
			Id = Next<GameItemComponentProto>(), RevisionNumber = 0, Type = type, Name = "Armageddon " + key,
			Description = "Historical worldfile component", Definition = definition.ToString(), EditableItem = Approval()
		};
		_db.GameItemComponentProtos.Add(component);
		_components.Add(key, component);
		return component;
	}

	private static void Attach(GameItemProto item, GameItemComponentProto component) =>
		item.GameItemProtosGameItemComponentProtos.Add(new()
		{
			GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber
		});
}

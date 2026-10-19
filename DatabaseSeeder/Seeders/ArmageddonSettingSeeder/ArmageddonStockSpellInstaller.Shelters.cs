#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSpellInstaller
{
	internal const string FallbackBindingKey = "arm.binding.room.s4_fallback";

	private static SeederManagedRecord CatalogueClaim<T>(FuturemudDatabaseContext db, string key, bool revisioned) where T : class
	{
		var claims = db.SeederManagedRecords.AsNoTracking()
			.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key)
			.ToArray();
		if (claims.Length != 1 || claims[0] is not { Retired: false, LogicalId: > 0, SeedBaseline: not null } record ||
			record.Module != ArmageddonCatalogueOwnership.Module || record.ManifestVersion != ArmageddonCatalogueOwnership.Version ||
			record.EntityType != typeof(T).Name || (record.RevisionNumber is not null) != revisioned ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
			throw new StockDependencyException($"{key}: complete unique typed S3 ownership is required before this S4 contribution; no name adoption.");
		return record;
	}

	private static ArmageddonCanonicalSpellContribution ResolveShelter(FuturemudDatabaseContext db, ArmageddonShelterSpellContent shelter)
	{
		var claim = CatalogueClaim<AutobuilderRoomTemplate>(db, shelter.TemplateKey, false);
		var template = db.AutobuilderRoomTemplates.AsNoTracking().SingleOrDefault(x => x.Id == claim.LogicalId)
			?? throw new StockDependencyException(shelter.TemplateKey + ": restore the exact owned autobuilder template.");
		XElement root;
		try { root = XElement.Parse(template.Definition); }
		catch (System.Xml.XmlException) { throw new StockDependencyException(shelter.TemplateKey + ": the native presentation XML is unreadable."); }
		var info = template.TemplateType switch
		{
			"room by terrain" => root.Element("Default")?.Element("Terrain"),
			"simple" => root,
			_ => null
		};
		if (root.Name != "Template" || root.DescendantsAndSelf().Any(x => x.Name.Namespace != XNamespace.None) || info is null ||
			info.Elements("DefaultTerrain").Count() != 1 || !long.TryParse(info.Element("DefaultTerrain")!.Value, out var terrainId) ||
			!int.TryParse(info.Element("OutdoorsType")?.Value, out var outdoors) ||
			outdoors != (int)RoomOutdoorsType.Indoors && outdoors != (int)RoomOutdoorsType.IndoorsNoLight)
			throw new StockDependencyException(shelter.TemplateKey + ": a declared deterministic native indoor presentation is required.");
		var interior = db.Terrains.AsNoTracking().SingleOrDefault(x => x.Id == terrainId);
		if (interior is null || !IsDryGroundTerrain(interior) || !ValidGas(db, interior.AtmosphereId, interior.AtmosphereType))
			throw new StockDependencyException(shelter.TemplateKey + ": the exact interior terrain/gas must support dry native ground level.");
		var terrains = db.Terrains.AsNoTracking().AsEnumerable().Where(IsDryGroundTerrain).Select(x => x.Id).Order().ToArray();
		if (terrains.Length == 0) throw new StockDependencyException("No supported native dry-ground terrain admits the authored shelter default.");
		long water = 0; long liquid = 0;
		if (shelter.Kind == SpellShelterKind.SpringHaven)
		{
			var vesselClaim = CatalogueClaim<GameItemProto>(db, "arm.item.haven_water", true);
			var vessel = db.GameItemProtos.AsNoTracking().Include(x => x.EditableItem)
				.Include(x => x.GameItemProtosGameItemComponentProtos).ThenInclude(x => x.GameItemComponent).ThenInclude(x => x.EditableItem)
				.Include(x => x.GameItemProtosOnLoadProgs)
				.SingleOrDefault(x => x.Id == vesselClaim.LogicalId && x.RevisionNumber == vesselClaim.RevisionNumber);
			if (vessel?.EditableItem?.RevisionStatus != (int)RevisionStatus.Current ||
				vessel.GameItemProtosGameItemComponentProtos.Count != 1 ||
				vessel.GameItemProtosGameItemComponentProtos.Single().GameItemComponent.Type != "Liquid Container" ||
				vessel.MorphGameItemProtoId is not null || vessel.MorphTimeSeconds > 0 || vessel.GameItemProtosOnLoadProgs.Any())
				throw new StockDependencyException("Spring Haven requires its exact approved native finite liquid-container prototype.");
			var componentClaim = CatalogueClaim<GameItemComponentProto>(db, "arm.item.haven_water.component", true);
			var component = vessel.GameItemProtosGameItemComponentProtos.Single().GameItemComponent;
			if (component.Id != componentClaim.LogicalId || component.RevisionNumber != componentClaim.RevisionNumber ||
				component.EditableItem?.RevisionStatus != (int)RevisionStatus.Current)
				throw new StockDependencyException("Spring Haven requires its exact approved owned container component.");
			XElement container;
			try { container = XElement.Parse(component.Definition); }
			catch (System.Xml.XmlException) { throw new StockDependencyException("Spring Haven container XML is unreadable."); }
			var settings = ArmageddonInstallation.Read(db)!;
			var litres = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Litre]).BaseMultiplier;
			if (!double.TryParse((string?)container.Attribute("LiquidCapacity"), NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var capacity) ||
				!double.IsFinite(capacity) || !double.IsFinite(litres) || litres <= 0 || capacity < 3.5 * litres ||
				!bool.TryParse((string?)container.Attribute("Closable"), out var closable) || closable ||
				!long.TryParse((string?)container.Attribute("AdjustQuantityProg"), out var adjust) || adjust != 0 ||
				!long.TryParse((string?)container.Attribute("DefaultLiquid"), out var defaultLiquid) || defaultLiquid != 0)
				throw new StockDependencyException("Spring Haven needs an open unscripted container supporting the full3.5-litre grade-seven supply without reload refill.");
			water = vessel.Id;
			liquid = CatalogueAlias(db, "arm.liquid.water", nameof(Liquid));
			if (liquid != settings.NativeBindings[ArmageddonNativeBinding.Water])
				throw new StockDependencyException("Spring Haven's typed water alias no longer matches the fixed native installation binding.");
		}
		long[]? schools = null;
		if (shelter.Kind == SpellShelterKind.SeveringRefuge)
		{
			schools = new[] { "water", "earth", "fire", "wind", "lightning", "shadow", "void" }
				.Select(key => ProgressionSchool(db, "arm.school." + key)).ToArray();
		}
		// Borrow one native destination. No persistent/static Room is materialised by installation.
		var fallback = ResolveFallback(db);
		return new(shelter.NativeCapabilityKey, shelter.Bind(template.Id, terrains, fallback, water, liquid, schools));
	}

	private static long ProgressionSchool(FuturemudDatabaseContext db, string key)
	{
		var claims = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key).ToArray();
		if (claims.Length != 1 || claims[0] is not { Retired: false, LogicalId: > 0, RevisionNumber: null, SeedBaseline: not null } record ||
			record.Module != ArmageddonProgressionInstaller.Module || record.ManifestVersion != ArmageddonProgressionInstaller.Version ||
			record.EntityType != nameof(MagicSchool) || !db.MagicSchools.Any(x => x.Id == record.LogicalId) ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
			throw new StockDependencyException(key + ": the exact unique typed progression school is required for the owned ward.");
		return record.LogicalId!.Value;
	}

	private static long CatalogueAlias(FuturemudDatabaseContext db, string key, string nativeType)
	{
		var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key).ToArray();
		if (records.Length != 1 || records[0].ManifestVersion != "stock-catalogue-links-1" ||
			records[0].EntityType != ArmageddonCatalogueLinks.AliasType || !ArmageddonCatalogueLinks.Valid(db, records[0]))
			throw new StockDependencyException(key + ": its exact declared typed native alias is required.");
		var alias = JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(records[0].SeedBaseline!)!;
		if (alias.NativeType != nativeType) throw new StockDependencyException(key + ": native alias type does not match its declared binding.");
		return alias.NativeId;
	}

	internal static bool IsDryGroundTerrain(Terrain terrain)
	{
		if (string.IsNullOrWhiteSpace(terrain.TerrainBehaviourMode)) return false;
		var token = new StringStack(terrain.TerrainBehaviourMode).PopSpeech().ToLowerInvariant();
		// Authored builder-editable admission defaults, using known native dry GroundLevel modes.
		return token is "indoors" or "standard" or "outdoors" or "trees" or "talltrees" or "cave" or "cavetrees" or "rooftop" or "rooftops";
	}

	private static bool ValidGas(FuturemudDatabaseContext db, long? id, string? type) =>
		id is > 0 && string.Equals(type, "gas", StringComparison.OrdinalIgnoreCase) && db.Gases.Any(x => x.Id == id && x.Density > 0);

	internal static bool IsSafeFallback(FuturemudDatabaseContext db, long id)
	{
		var room = db.Rooms.AsNoTracking().Include(x => x.CurrentOverlay).ThenInclude(x => x.Terrain)
			.Include(x => x.CurrentOverlay).ThenInclude(x => x.RoomOverlayPackage).ThenInclude(x => x.EditableItem)
			.Include(x => x.RouteRoom).SingleOrDefault(x => x.Id == id);
		var overlay = room?.CurrentOverlay;
		if (room is null || room.Temporary || room.HostedVehicleId is not null || room.HostedVehicleCompartmentId is not null ||
			overlay is null || overlay.Id != room.CurrentOverlayId || overlay.RoomId != id || overlay.Terrain is null ||
			!IsDryGroundTerrain(overlay.Terrain) || overlay.RoomOverlayPackage?.EditableItem?.RevisionStatus != (int)RevisionStatus.Current ||
			!ValidGas(db, overlay.AtmosphereId ?? overlay.Terrain.AtmosphereId,
				overlay.AtmosphereId.HasValue ? overlay.AtmosphereType : overlay.Terrain.AtmosphereType)) return false;
		return room.RouteRoom is not { } route || route.RoomId == id && route.LengthMetres > 0 &&
			route.DefaultPositionMetres >= 0 && route.DefaultPositionMetres <= route.LengthMetres &&
			route.MetresPerRoomEquivalent > 0 && route.TopologyVersion > 0;
	}

	private static long ResolveFallback(FuturemudDatabaseContext db)
	{
		var retained = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == FallbackBindingKey).ToArray();
		if (retained.Length != 0)
		{
			if (retained.Length != 1 || retained[0].ManifestVersion != "stock-catalogue-links-1" ||
				retained[0].EntityType != ArmageddonCatalogueLinks.AliasType || !ArmageddonCatalogueLinks.Valid(db, retained[0]))
				throw new StockDependencyException("The retained typed S4 fallback is unavailable or unsafe; preserve and restore explicitly.");
			var alias = JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(retained[0].SeedBaseline!)!;
			if (alias.NativeType != nameof(Room) || !IsSafeFallback(db, alias.NativeId))
				throw new StockDependencyException("The retained S4 fallback is not a safe permanent native Room.");
			return alias.NativeId;
		}
		var screens = db.ChargenScreenStoryboards.AsNoTracking().Where(x =>
			x.ChargenStage == (int)ChargenStage.SelectStartingLocation && x.ChargenType == "StartingLocationPicker").ToArray();
		if (screens.Length != 1) throw new StockDependencyException("Configure one native starting-location picker before binding the S4 recovery destination.");
		XElement screen;
		try { screen = XElement.Parse(screens[0].StageDefinition); }
		catch (System.Xml.XmlException) { throw new StockDependencyException("The native starting-location picker XML is unreadable."); }
		if (screen.Name.Namespace != XNamespace.None || screen.Name.LocalName is not ("Screen" or "Definition") ||
			screen.Elements("Locations").Count() != 1)
			throw new StockDependencyException("Use the native starting-location picker serializer for S4 recovery bindings.");
		var seen = new HashSet<long>();
		foreach (var location in screen.Element("Locations")!.Elements("Location"))
		{
			if (location.Elements("Location").Count() != 1 || !long.TryParse(location.Element("Location")!.Value, out var id) ||
				id <= 0 || !seen.Add(id) || !IsSafeFallback(db, id)) continue;
			return ArmageddonCatalogueLinks.Bind(db, FallbackBindingKey, nameof(Room), () => id);
		}
		throw new StockDependencyException("No configured starting location is a current permanent dry-ground safe recovery Room; no Room is guessed or created.");
	}
}

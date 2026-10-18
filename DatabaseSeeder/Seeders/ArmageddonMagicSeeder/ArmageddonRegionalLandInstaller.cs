#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using RegionalProfile = MudSharp.Magic.Environment.RegionalGatheringProfile;

namespace DatabaseSeeder.Seeders;

/// <summary>Reusable source-informed regional and local land presets; never binds an existing game region or room.</summary>
public static class ArmageddonRegionalLandInstaller
{
	public const string LifeYieldKey = "armageddon-life";
	public const string MixedProfileKey = "arm.land.regional.mixed";
	public sealed record LandPreset(string Key, string Name, double Life, string PriceClass, string BaseTerrain);
	public static IReadOnlyList<LandPreset> Presets { get; } = new LandPreset[]
	{
		new("heavy_forest", "heavy forest", 5, "fertile", "Broadleaf Forest"),
		new("light_forest", "light forest", 4, "fertile", "Woodland"),
		new("fertile", "field grassland or thornland", 3, "fertile", "Grasslands"),
		new("cavern", "cavern road or ruins", 2, "cavern", "Cavern"),
		new("hills", "hills or mountains", 2, "hills", "Hills"),
		new("sewer", "sewer", 2, "barren", "Sewer"),
		new("barren", "desert salt silt or shallows", 1, "barren", "Sandy Desert"),
		new("urban", "inside or city", 0, "urban", "Residence"),
		new("forbidden", "forbidden land", 0, "forbidden", "Grasslands")
	};
	public static string RegionalKey(string preset) => "arm.land.regional." + preset;
	public static string TerrainKey(string preset) => "arm.land.terrain." + preset;
	public static string LocalProfileKey(string preset) => "arm.land.local." + preset;
	public static string ForageKey(string preset) => "arm.land.forage." + preset;

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory)
	{
		using var db = factory();
		var messages = new List<string>();
		var ids = new Dictionary<string, long>();
		var writing = false;
		var committing = false;
		var committed = false;
		try
		{
			var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Install fixed Armageddon settings first.");
			if (db.ChangeTracker.HasChanges() || db.Database.CurrentTransaction is not null)
				throw new InvalidOperationException("Use a fresh dedicated context without pending writes or a transaction.");
			long Require(string key, string type)
			{
				var claim = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
				if (claim?.EntityType != type || claim.Retired || claim.LogicalId is not > 0) throw new InvalidOperationException($"{key}: missing typed land dependency.");
				return claim.LogicalId.Value;
			}
			var marker = Require(ArmageddonProgressionInstaller.LandMarkerResourceKey, nameof(MagicResource));
			var priceTags = ArmageddonStockGatheringInstaller.TerrainClasses.ToDictionary(x => x.Key,
				x => Require(ArmageddonStockGatheringInstaller.TerrainTagKey(x.Key), nameof(Tag)));
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			writing = true;
			var mixed = new Dictionary<long, double>();
			var editorCodes = db.Terrains.Select(x => x.TerrainEditorText).AsEnumerable().Where(x => x is not null).ToHashSet(StringComparer.OrdinalIgnoreCase);
			foreach (var preset in Presets)
			{
				var regionalKey = RegionalKey(preset.Key);
				var desiredProfile = new RegionalProfile(1, "Armageddon " + preset.Name, 1, preset.Life, .1, new Dictionary<long, double>());
				var regional = PutRegional(db, owned, regionalKey, desiredProfile);
				ids.Add(regionalKey, regional.Id);
				var forage = new ForagableProfile { Id = owned.Id<ForagableProfile>(ForageKey(preset.Key)), RevisionNumber = 0,
					Name = "Armageddon life - " + preset.Name, EditableItem = owned.Approval() };
				if (preset.Life > 0)
				{
					forage.ForagableProfilesMaximumYields.Add(new() { ForageType = LifeYieldKey, Yield = preset.Life });
					// Local fallback is finite. The approved .1/hour recovery belongs to the shared region, once per owner.
					forage.ForagableProfilesHourlyYieldGains.Add(new() { ForageType = LifeYieldKey, Yield = 0 });
				}
				forage = owned.Put(ForageKey(preset.Key), forage, []);
				var local = owned.Put(LocalProfileKey(preset.Key), new MagicGenerator
				{
					Name = "Armageddon local living land - " + preset.Name, Type = "environmental",
					Definition = new XElement("Definition", new XAttribute("version", 1), new XElement("PressureHalfLifeSeconds", 3600),
						new XElement("NaturalRepairPerMinute", 0), new XElement("Outputs", new XElement("Output", new XAttribute("resource", marker),
							new XAttribute("basecapacity", 0), new XAttribute("baserate", 0), new XElement("Maximum", "0"), new XElement("Rate", "0"))),
						new XElement("Inputs"), new XElement("Organic", new XAttribute("version", 1), new XAttribute("scaraddendum", false),
							new XElement("Sources", new XElement("Source", new XAttribute("selector", "forage:" + LifeYieldKey),
								new XAttribute("kind", "Forage"), new XAttribute("foragekey", LifeYieldKey), new XElement("Uses"), new XElement("Definitions"))),
							new XElement("Penalties"))).ToString()
				}, [ArmageddonProgressionInstaller.LandMarkerResourceKey, ForageKey(preset.Key)]);
				ids.Add(LocalProfileKey(preset.Key), local.Id);
				var sources = db.Terrains.Where(x => x.Name == preset.BaseTerrain).Take(2).ToArray();
				if (sources.Length != 1)
				{
					messages.Add($"{preset.Name}: reusable profiles installed; a unique '{preset.BaseTerrain}' terrain is absent, so no physical terrain preset was fabricated.");
					continue;
				}
				var terrainKey = TerrainKey(preset.Key);
				var terrain = new Terrain();
				db.Entry(terrain).CurrentValues.SetValues(sources[0]);
				terrain.Id = owned.Id<Terrain>(terrainKey);
				terrain.Name = "Armageddon " + preset.Name;
				terrain.DefaultTerrain = false;
				terrain.DefaultAgricultureFieldProfileId = null;
				terrain.ForagableProfileId = forage.Id;
				terrain.EnvironmentalMagicProfileId = local.Id;
				var tags = (terrain.TagInformation ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					.Select(long.Parse).Where(x => !priceTags.Values.Contains(x)).Append(priceTags[preset.PriceClass]).Distinct();
				terrain.TagInformation = string.Join(",", tags);
				var retained = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == terrainKey);
				var baseline = retained?.SeedBaseline is { } json ? JsonSerializer.Deserialize<Dictionary<string, string>>(json) : null;
				terrain.TerrainEditorText = baseline?.TryGetValue(nameof(Terrain.TerrainEditorText), out var code) == true
					? JsonSerializer.Deserialize<string>(code)! : Enumerable.Range(0, 26 * 10).Select(x => $"{(char)('A' + x / 10)}{x % 10}")
						.First(editorCodes.Add);
				terrain = owned.Put(terrainKey, terrain, [ForageKey(preset.Key), LocalProfileKey(preset.Key), ArmageddonStockGatheringInstaller.TerrainTagKey(preset.PriceClass)]);
				ids.Add(terrainKey, terrain.Id);
				mixed[terrain.Id] = preset.Life;
			}
			var mixedProfile = PutRegional(db, owned, MixedProfileKey, new(1, "Armageddon mixed living land", 1, 0, .1, mixed));
			ids.Add(MixedProfileKey, mixedProfile.Id);
			db.SaveChanges();
			committing = true;
			transaction?.Commit();
			committed = true;
			messages.Add("Installed nine terrain-family regional/local profiles, finite native fallback life and available owned terrain presets; mixed-region capacity sums terrain contributions. Shared recovery is .1 life per online real hour per region. No existing region, room, terrain, live stock or player was rebound/refilled.");
			return new(ArmageddonInstallStatus.Completed, messages, ids);
		}
		catch (Exception error)
		{
			if (!committed) ids.Clear();
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerun. " : writing ? "Regional land batch failed before commit; relational transaction rolled back. " : "Regional land preflight blocked before writes. ") + error.Message);
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : writing ? ArmageddonInstallStatus.Failed : ArmageddonInstallStatus.Blocked, messages, ids);
		}
	}
	private static RegionalGatheringProfile PutRegional(FuturemudDatabaseContext db, ArmageddonCatalogueOwnership owned, string key, RegionalProfile profile)
	{
		var claim = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		var old = claim?.LogicalId is { } id ? db.RegionalGatheringProfiles.Find(id) : null;
		var definition = old?.Definition;
		var row = owned.Put(key, new RegionalGatheringProfile { Name = profile.Name, Revision = 1, Definition = profile.Definition() }, []);
		if (old is not null && definition != row.Definition) row.Revision++;
		return row;
	}
}

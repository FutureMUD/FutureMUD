using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Health;
using MudSharp.Models;

#nullable enable

namespace DatabaseSeeder.Seeders;

/// <summary>Optional definitions and field-owned overlays; never enables an existing world.</summary>
public partial class EnvironmentalExposureSeeder : IDatabaseSeeder
{
	private FuturemudDatabaseContext _context = null!;
	private readonly List<string> _notes = new();
	public IReadOnlyList<string> ReportNotes => _notes.AsReadOnly();
	private double _litresPerBase = 1;
	public int SortOrder => 100000;
	public string Name => "Environmental Exposure";
	public string Tagline => "Material-selective hazards, protective preparations and demonstration equipment";
	public string FullDescription => "Audits installed solids, liquids and gases. Optionally installs natural/industrial and fantasy exposure definitions. Preserves builder edits and never enables exposure or creates hazardous rooms.";
	public bool SafeToRunMoreThanOnce => true;
	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
	[
		("natural", "Install natural and industrial environmental hazards and alchemical preparations? (yes/no)", (_, _) => true, Validate),
		("fantasy", "Install the optional fantasy hazards and magical protective preparations? (yes/no)", (_, _) => true, Validate)
	];
	private static (bool, string) Validate(string answer, FuturemudDatabaseContext _) =>
		answer.Equals("yes", StringComparison.OrdinalIgnoreCase) || answer.Equals("no", StringComparison.OrdinalIgnoreCase)
			? (true, "") : (false, "Please answer yes or no.");
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context) => !context.Materials.Any() || !context.Liquids.Any()
		? ShouldSeedResult.PrerequisitesNotMet : context.SeederManagedRecords.Any(x => x.Seeder == nameof(EnvironmentalExposureSeeder))
			? ShouldSeedResult.ExtraPackagesAvailable : ShouldSeedResult.ReadyToInstall;

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers)
	{
		_context = context; _notes.Clear();
		var unit = context.StaticConfigurations.FirstOrDefault(x => x.SettingName == "BaseFluidUOMToLitres")?.Definition;
		_litresPerBase = double.TryParse(unit, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var value) && value > 0 && double.IsFinite(value) ? value : 1;
		bool Yes(string key) => answers.TryGetValue(key, out var answer) && answer.Equals("yes", StringComparison.OrdinalIgnoreCase);
		using var transaction = context.Database.BeginTransaction();
		UpgradeBiologicalAcid();
		if (Yes("natural") || Yes("fantasy"))
		{
			InstallMaterialResponses();
			InstallEquipmentMaterials(Yes("fantasy"));
		}
		if (Yes("natural")) { InstallProfiles(false); InstallPreparations(false); InstallEquipment(); }
		if (Yes("fantasy")) { InstallProfiles(true); InstallPreparations(true); }
		context.SaveChanges();
		var audit = ExposureCatalogueAudit.Capture(context);
		transaction.Commit();
		return $"Audited {audit.Count(x => x.Kind == "material")} solids, {audit.Count(x => x.Kind == "liquid")} liquids and {audit.Count(x => x.Kind == "gas")} gases. " +
			"Exposure mode was not changed; no live rooms were created. Rates are authored game balance, pending calibration against your health profile.\n" +
			string.Join("\n", _notes.Distinct());
	}

	internal static Guid StableId(string key) => new(SHA256.HashData(Encoding.UTF8.GetBytes("FutureMUD:EnvironmentalExposure:" + key))[..16]);
	private SeederManagedRecord Record(string type, string key, long id, string module)
	{
		var record = _context.SeederManagedRecords.Local.SingleOrDefault(x => x.Seeder == nameof(EnvironmentalExposureSeeder) && x.EntityType == type && x.StableKey == key)
			?? _context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == nameof(EnvironmentalExposureSeeder) && x.EntityType == type && x.StableKey == key);
		if (record is null)
		{
			record = new() { Seeder = nameof(EnvironmentalExposureSeeder), EntityType = type, StableKey = key };
			_context.SeederManagedRecords.Add(record);
		}
		record.LogicalId = id; record.Module = module; record.ManifestVersion = "1"; record.AppliedAt = DateTime.UtcNow;
		return record;
	}

	// Name collisions are never adopted as whole records. Only this seeder's persisted identity may be updated.
	private T? Owned<T>(string key, string module, T desired, Func<T?> collision) where T : class
	{
		var type = typeof(T).Name;
		var record = _context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == nameof(EnvironmentalExposureSeeder) && x.EntityType == type && x.StableKey == key);
		var primaryKey = _context.Model.FindEntityType(typeof(T))!.FindPrimaryKey()!;
		var row = record?.LogicalId is { } id ? _context.Set<T>().Find(primaryKey.Properties.Select(x => x.Name == "Id" ? (object)id : record.RevisionNumber ?? 0).ToArray()) : null;
		var created = row is null;
		if (created)
		{
			if (collision() is not null) { _notes.Add($"Preserved unowned {type} name collision: {key}. Rename it or explicitly reconcile its identity before installing this entry."); return null; }
			row = desired; _context.Add(row); _context.SaveChanges();
		}
		var entry = _context.Entry(row!);
		var fields = entry.Metadata.GetProperties().Where(x => !x.IsPrimaryKey() && x.Name is not ("SurfaceReactionInfo" or "ExposureInfo" or "EditableItemId")).ToArray();
		Dictionary<string, string> Values(T entity) => fields.ToDictionary(p => p.Name,
			p => JsonSerializer.Serialize(_context.Entry(entity).Property(p.Name).CurrentValue, p.ClrType));
		record = Record(type, key, (long)entry.Property("Id").CurrentValue!, module);
		if (primaryKey.Properties.Any(x => x.Name == "RevisionNumber")) record.RevisionNumber = (int)entry.Property("RevisionNumber").CurrentValue!;
		var merged = SeederManagedRecordReconciler.Reconcile(record, Values(row!), Values(desired), created, _notes);
		foreach (var field in fields) entry.Property(field.Name).CurrentValue = JsonSerializer.Deserialize(merged[field.Name], field.ClrType);
		return row;
	}

	private void Overlay(string type, long id, string key, string? current, string desired, bool safeInitialField, Action<string> apply)
	{
		var record = Record(type, key, id, "catalogue-overlay");
		var merged = SeederManagedRecordReconciler.Reconcile(record,
			new Dictionary<string, string> { ["definition"] = current ?? "" }, new Dictionary<string, string> { ["definition"] = desired },
			record.SeedBaseline is null && safeInitialField, _notes);
		apply(merged["definition"]);
	}

	private Liquid? Liquid(string name, string module)
	{
		var water = _context.Liquids.OrderBy(x => x.Id).FirstOrDefault(x => x.Name == "water");
		if (water is null) { _notes.Add($"Deferred {name}: canonical water is missing."); return null; }
		var desired = new Liquid();
		_context.Entry(desired).CurrentValues.SetValues(water);
		desired.Id = 0; desired.Name = name; desired.Description = name; desired.LongDescription = $"A quantity of {name} is here.";
		desired.TasteText = "an unpleasant chemical taste"; desired.VagueTasteText = "an unpleasant taste";
		desired.SmellText = "a sharp mineral smell"; desired.VagueSmellText = "an unfamiliar smell";
		desired.WaterLitresPerLitre = 0; desired.AlcoholLitresPerLitre = 0; desired.FoodSatiatedHoursPerLitre = 0; desired.DrinkSatiatedHoursPerLitre = 0;
		desired.DraughtProgId = null; desired.DrugId = null; desired.DrugGramsPerUnitVolume = 0; desired.SurfaceReactionInfo = null!;
		desired.CountAsId = null; desired.StaleLiquidId = null; desired.SpoiledLiquidId = null; desired.StaleAfterSeconds = null; desired.SpoilAfterSeconds = null;
		desired.GasFormId = null; desired.DriedResidueId = null; desired.ResidueVolumePercentage = 0; desired.SolventId = null;
		return Owned("liquid:" + name, module, desired, () => _context.Liquids.FirstOrDefault(x => x.Name == name));
	}

	private Gas? Gas(string name, string module)
	{
		// Existing canonical gases keep all their physiology, drugs, aliases and builder metadata.
		var existing = _context.Gases.SingleOrDefault(x => x.Name == name);
		if (existing is not null) return existing;
		return Owned("gas:" + name, module, new Gas { Name = name, Description = name, DisplayColour = "green", SmellText = "a pungent mineral smell",
			VagueSmellText = "an unfamiliar smell", Density = 1.5, Viscosity = 0.018, SpecificHeatCapacity = 1, ThermalConductivity = 0.02,
			BoilingPoint = -20, SmellIntensity = 10, OxidationFactor = 0 }, () => _context.Gases.FirstOrDefault(x => x.Name == name));
	}

	internal static XElement BiologicalAcidReaction(long animalSkinTagId)
	{
		var legacy = new XElement("Reaction", new XAttribute("DamageType", (int)DamageType.Chemical), new XAttribute("DamagePerTick", 125.0),
			new XAttribute("PainPerTick", 175.0), new XAttribute("StunPerTick", 0.0), new XElement("Tags", new XElement("Tag", animalSkinTagId)));
		var rule = new XElement(legacy);
		rule.Add(new XAttribute("Version", 2), new XAttribute("Id", StableId("biological-acid")), new XAttribute("Name", "Biological acid on animal skin"),
			new XAttribute("Routes", 1), new XAttribute("Category", "chemical"), new XAttribute("Channel", "chemical"), new XAttribute("Priority", 0),
			new XAttribute("DamageRate", 2.5), new XAttribute("PainRate", 3.5), new XAttribute("Consumption", 1), new XAttribute("ConsumptionRate", 0.001),
			new XElement("Legacy", legacy));
		return rule;
	}
	private void UpgradeBiologicalAcid()
	{
		var acid = _context.Liquids.SingleOrDefault(x => x.Name == "animal acid");
		if (acid is null || string.IsNullOrWhiteSpace(acid.SurfaceReactionInfo)) return;
		try
		{
			var root = XElement.Parse(acid.SurfaceReactionInfo);
			foreach (var rule in root.Elements("Reaction").ToArray())
			{
				if ((int?)rule.Attribute("Version") is >= 2) continue;
				var tags = rule.Element("Tags")?.Elements("Tag").Select(x => (long)x).ToArray() ?? Array.Empty<long>();
				if (tags.Length != 1 || _context.Tags.Find(tags[0])?.Name != "Animal Skin" || (int?)rule.Attribute("DamageType") != (int)DamageType.Chemical ||
					(double?)rule.Attribute("DamagePerTick") != 125 || (double?)rule.Attribute("PainPerTick") != 175 || (double?)rule.Attribute("StunPerTick") != 0)
				{ _notes.Add("Preserved customised animal acid v1 rule; use explicit builder conversion with chosen rates."); continue; }
				var upgraded = BiologicalAcidReaction(tags[0]);
				upgraded.SetAttributeValue("ConsumptionRate", 0.001 / _litresPerBase);
				upgraded.SetAttributeValue("Id", (string?)rule.Attribute("Id") ?? new Guid(SHA256.HashData(Encoding.UTF8.GetBytes(rule.ToString(SaveOptions.DisableFormatting)))[..16]).ToString());
				rule.ReplaceWith(upgraded);
			}
			acid.SurfaceReactionInfo = root.ToString();
		}
		catch (System.Xml.XmlException) { _notes.Add("Preserved malformed animal acid XML; builder repair is required."); }
	}
}

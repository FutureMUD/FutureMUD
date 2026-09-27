using System.Globalization;
using System.Security.Cryptography;
using MudSharp.Health;

#nullable enable

namespace MudSharp.Form.Material;

/// <summary>Versioned fluid reaction; historical coefficients are never inferred from continuous rates.</summary>
public sealed class LiquidSurfaceReaction : ILiquidSurfaceReaction, IEnvironmentalReaction
{
	private readonly IFuturemud _gameworld;
	private readonly List<ITag> _targetTags = new();
	private readonly List<string> _loadErrors = new();
	private XElement? _legacy;
	private long _materialId, _spentId, _applicabilityId, _intensityId, _notificationId;

	public LiquidSurfaceReaction(IFuturemud gameworld) { _gameworld = gameworld; }
	public LiquidSurfaceReaction(ILiquidSurfaceReaction rhs, IFuturemud gameworld)
		: this(rhs is LiquidSurfaceReaction concrete ? concrete.SaveToXml() : LegacyXml(rhs), gameworld) { }

	public LiquidSurfaceReaction(XElement root, IFuturemud gameworld) : this(gameworld)
	{
		try
		{
			Id = Guid.TryParse((string?)root.Attribute("Id"), out var id) ? id :
				new Guid(SHA256.HashData(Encoding.UTF8.GetBytes(root.ToString(SaveOptions.DisableFormatting)))[..16]);
			Version = (int?)root.Attribute("Version") ?? 1;
			DamageType = (DamageType)((int?)root.Attribute("DamageType") ?? 0);
			DamagePerTick = Read(root, "DamagePerTick");
			PainPerTick = Read(root, "PainPerTick");
			StunPerTick = Read(root, "StunPerTick");
			_legacy = root.Element("Legacy")?.Element("Reaction") is { } legacy ? new XElement(legacy) : null;
			foreach (var element in root.Element("Tags")?.Elements("Tag") ?? Enumerable.Empty<XElement>())
			{
				var tag = gameworld.Tags.Get((long)element);
				if (tag is null) _loadErrors.Add($"Missing target tag {element.Value}.");
				else _targetTags.Add(tag);
			}
			if (Version < 2) return;
			Name = (string?)root.Attribute("Name") ?? "reaction";
			Channel = (string?)root.Attribute("Channel") ?? "chemical";
			Category = (string?)root.Attribute("Category") ?? "chemical";
			Routes = (ExposureRoute)((int?)root.Attribute("Routes") ?? 1);
			Priority = (int?)root.Attribute("Priority") ?? 0;
			NoReaction = (bool?)root.Attribute("NoReaction") ?? false;
			_materialId = (long?)root.Attribute("Material") ?? 0;
			_spentId = (long?)root.Attribute("SpentLiquid") ?? 0;
			_applicabilityId = (long?)root.Attribute("ApplicabilityProg") ?? 0;
			_intensityId = (long?)root.Attribute("IntensityProg") ?? 0;
			_notificationId = (long?)root.Attribute("NotificationProg") ?? 0;
			DamageRate = Read(root, "DamageRate"); PainRate = Read(root, "PainRate"); StunRate = Read(root, "StunRate");
			Consumption = (ReactionConsumption)((int?)root.Attribute("Consumption") ?? 0);
			ConsumptionRate = Read(root, "ConsumptionRate");
			MinimumTemperature = (double?)root.Attribute("MinimumTemperature");
			MaximumTemperature = (double?)root.Attribute("MaximumTemperature");
			Message = (string?)root.Element("Message");
		}
		catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException)
		{
			_loadErrors.Add("Malformed reaction data: " + ex.Message);
		}
	}

	private static double Read(XElement root, string name) => double.Parse((string?)root.Attribute(name) ?? "0", CultureInfo.InvariantCulture);
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = "reaction";
	public int Version { get; private set; } = 1;
	public ExposureRoute Routes { get; set; } = ExposureRoute.LiquidContact;
	public string Channel { get; set; } = "chemical";
	public string Category { get; set; } = "chemical";
	public int Priority { get; set; }
	public bool NoReaction { get; set; }
	public ISolid? TargetMaterial { get => _gameworld.Materials.Get(_materialId); set => _materialId = value?.Id ?? 0; }
	public IEnumerable<ITag> TargetTags => _targetTags;
	public DamageType DamageType { get; set; }
	public double DamagePerTick { get; set; }
	public double PainPerTick { get; set; }
	public double StunPerTick { get; set; }
	public double DamageRate { get; set; }
	public double PainRate { get; set; }
	public double StunRate { get; set; }
	public double? MinimumTemperature { get; set; }
	public double? MaximumTemperature { get; set; }
	public ReactionConsumption Consumption { get; set; }
	public double ConsumptionRate { get; set; }
	public ILiquid? SpentLiquid { get => _gameworld.Liquids.Get(_spentId); set => _spentId = value?.Id ?? 0; }
	public IFutureProg? ApplicabilityProg { get => _gameworld.FutureProgs.Get(_applicabilityId); set => _applicabilityId = value?.Id ?? 0; }
	public IFutureProg? IntensityProg { get => _gameworld.FutureProgs.Get(_intensityId); set => _intensityId = value?.Id ?? 0; }
	public IFutureProg? NotificationProg { get => _gameworld.FutureProgs.Get(_notificationId); set => _notificationId = value?.Id ?? 0; }
	public string? Message { get; set; }
	public bool HasLegacy => Version == 1 || _legacy is not null;
	public ILiquidSurfaceReaction? Legacy => Version == 1 ? this : _legacy is null ? null : new LiquidSurfaceReaction(_legacy, _gameworld);

	public void Convert(double damage, double pain, double stun, bool preserveLegacy = true)
	{
		if (!ExposureArithmetic.Valid(damage) || !ExposureArithmetic.Valid(pain) || !ExposureArithmetic.Valid(stun)) throw new ArgumentOutOfRangeException(nameof(damage));
		if (Version == 1 && preserveLegacy) _legacy = LegacyXml(this);
		Version = 2;
		DamageRate = damage; PainRate = pain; StunRate = stun;
	}

	public bool ToggleTargetTag(ITag tag)
	{
		if (_targetTags.Remove(tag)) return false;
		_targetTags.Add(tag);
		return true;
	}

	public IEnumerable<string> ValidationErrors(bool gas)
	{
		foreach (var error in _loadErrors) yield return error;
		if (Version is not (1 or 2)) yield return "Unknown reaction version.";
		if (!Enum.IsDefined(DamageType)) yield return "Unsupported damage type.";
		if (new[] { DamagePerTick, PainPerTick, StunPerTick, DamageRate, PainRate, StunRate, ConsumptionRate }.Any(x => !ExposureArithmetic.Valid(x))) yield return "Rates must be finite and non-negative.";
		if (Version == 1) yield break;
		const ExposureRoute all = ExposureRoute.LiquidContact | ExposureRoute.GasContact | ExposureRoute.Inhalation | ExposureRoute.AmbientHeat | ExposureRoute.Ingestion | ExposureRoute.Injection;
		if (Routes == ExposureRoute.None || (Routes & ~all) != 0) yield return "Select real exposure routes.";
		if (gas && ((Routes & ~(ExposureRoute.GasContact | ExposureRoute.Inhalation)) != 0 || Consumption != ReactionConsumption.None || ConsumptionRate != 0 || _spentId != 0)) yield return "Gas rules cannot consume liquid, produce spent liquid or use liquid routes.";
		if (TargetMaterial is null && !_targetTags.Any()) yield return "Select a material or target tag.";
		if (_materialId != 0 && TargetMaterial is null) yield return "Missing target material.";
		if (string.IsNullOrWhiteSpace(Channel) || string.IsNullOrWhiteSpace(Category)) yield return "Channel and category cannot be empty.";
		if (!Enum.IsDefined(Consumption) || Consumption == ReactionConsumption.PerExposure && ConsumptionRate <= 0.0) yield return "Consuming rules require a positive rate.";
		if (MinimumTemperature is { } min && !double.IsFinite(min) || MaximumTemperature is { } max && !double.IsFinite(max) || MinimumTemperature > MaximumTemperature) yield return "Invalid temperature range.";
		if (_applicabilityId != 0 && ApplicabilityProg is null || _intensityId != 0 && IntensityProg is null || _notificationId != 0 && NotificationProg is null) yield return "Missing configured FutureProg.";
		if (ApplicabilityProg is { } applicability && !ExposureProgContract.Valid(applicability, "applicability") ||
			IntensityProg is { } intensity && !ExposureProgContract.Valid(intensity, "intensity") ||
			NotificationProg is { } notification && !ExposureProgContract.Valid(notification, "notification"))
			yield return "Configured FutureProg has an incompatible signature. " + ExposureProgContract.Description;
		if (_spentId != 0 && (SpentLiquid is not { } spent || spent.SurfaceReactions.Any() || spent.EnvironmentalReactions.Any() ||
			_gameworld.MagicalSubstances.Any(x => x.Bindings.Any(b => b.Carrier == MudSharp.Magic.SubstanceCarrier.Liquid && b.Id == _spentId)))) yield return "Spent liquid must exist, be non-hazardous and have no reagent payload.";
	}

	private static XElement LegacyXml(ILiquidSurfaceReaction reaction) => new("Reaction",
		new XAttribute("DamageType", (int)reaction.DamageType), new XAttribute("DamagePerTick", reaction.DamagePerTick),
		new XAttribute("PainPerTick", reaction.PainPerTick), new XAttribute("StunPerTick", reaction.StunPerTick),
		new XElement("Tags", reaction.TargetTags.Select(x => new XElement("Tag", x.Id))));

	public XElement SaveToXml()
	{
		var root = LegacyXml(this);
		root.Add(new XAttribute("Id", Id), new XAttribute("Version", Version));
		if (Version == 1) return root;
		root.Add(new XAttribute("Name", Name), new XAttribute("Routes", (int)Routes), new XAttribute("Channel", Channel),
			new XAttribute("Category", Category), new XAttribute("Priority", Priority), new XAttribute("NoReaction", NoReaction),
			new XAttribute("Material", _materialId), new XAttribute("DamageRate", DamageRate), new XAttribute("PainRate", PainRate),
			new XAttribute("StunRate", StunRate), new XAttribute("Consumption", (int)Consumption), new XAttribute("ConsumptionRate", ConsumptionRate),
			new XAttribute("SpentLiquid", _spentId), new XAttribute("ApplicabilityProg", _applicabilityId), new XAttribute("IntensityProg", _intensityId),
			new XAttribute("NotificationProg", _notificationId), MinimumTemperature is { } min ? new XAttribute("MinimumTemperature", min) : null,
			MaximumTemperature is { } max ? new XAttribute("MaximumTemperature", max) : null,
			Message is null ? null : new XElement("Message", new XCData(Message)), _legacy is null ? null : new XElement("Legacy", new XElement(_legacy)));
		return root;
	}
}

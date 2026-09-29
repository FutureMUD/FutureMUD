#nullable enable

using System.Globalization;
using MudSharp.Construction;

namespace MudSharp.NPC.AI;

/// <summary>Versioned, opt-in configuration; older Animal definitions keep their original acquisition.</summary>
public sealed class AnimalHuntingSettings
{
	public bool Enabled { get; set; }
	public AnimalPeoplePreyPolicy People { get; set; } = AnimalPeoplePreyPolicy.Never;
	public AnimalPreySelection Selection { get; set; } = AnimalPreySelection.Safest;
	public AnimalHuntOpening Opening { get; set; }
	public AnimalHuntFollowup Followup { get; set; }
	public RoomLayer? PreferredLayer { get; set; }
	public bool Opportunistic { get; set; } = true;
	public double EngageThreshold { get; set; } = 60.0;
	public double AbandonThreshold { get; set; } = 35.0;
	public double StarvationAdjustment { get; set; } = 10.0;
	public double ConfidenceBias { get; set; }
	public int PursuitRange { get; set; } = 5;
	public TimeSpan PursuitTimeout { get; set; } = TimeSpan.FromMinutes(5);
	public TimeSpan LostTimeout { get; set; } = TimeSpan.FromSeconds(60);
	public int? MinimumSizeDifference { get; set; }
	public int? MaximumSizeDifference { get; set; }
	public long ClassificationProgId { get; set; }
	public long EligibilityProgId { get; set; }
	public long PreferenceProgId { get; set; }
	public HashSet<long> IncludedRaces { get; } = [];
	public HashSet<long> ExcludedRaces { get; } = [];
	public Dictionary<long, double> PreferredRaces { get; } = [];
	public Dictionary<string, double> Weights { get; } = new(StringComparer.OrdinalIgnoreCase)
	{
		["size"] = 10, ["injury"] = 20, ["vulnerability"] = 25, ["tactic"] = 10,
		["support"] = 5, ["weapons"] = -10, ["owninjury"] = -20, ["fatigue"] = -20
	};

	public void SetAssessmentProfile(string profile)
	{
		(EngageThreshold, AbandonThreshold) = profile.ToLowerInvariant() switch
		{
			"cautious" => (70.0, 45.0), "bold" => (50.0, 25.0), _ => (60.0, 35.0)
		};
	}

	public XElement Save() => new("Hunting",
		new XAttribute("version", 1), new XAttribute("enabled", Enabled),
		new XElement("People", People), new XElement("Selection", Selection),
		new XElement("Opening", Opening), new XElement("Followup", Followup),
		new XElement("PreferredLayer", PreferredLayer?.ToString() ?? ""),
		new XElement("Opportunistic", Opportunistic), new XElement("Engage", EngageThreshold),
		new XElement("Abandon", AbandonThreshold), new XElement("Starvation", StarvationAdjustment),
		new XElement("Confidence", ConfidenceBias), new XElement("Range", PursuitRange),
		new XElement("TimeoutSeconds", PursuitTimeout.TotalSeconds), new XElement("LostSeconds", LostTimeout.TotalSeconds),
		new XElement("MinimumSize", MinimumSizeDifference), new XElement("MaximumSize", MaximumSizeDifference),
		new XElement("ClassificationProg", ClassificationProgId), new XElement("EligibilityProg", EligibilityProgId),
		new XElement("PreferenceProg", PreferenceProgId),
		new XElement("Include", IncludedRaces.Order().Select(x => new XElement("Race", x))),
		new XElement("Exclude", ExcludedRaces.Order().Select(x => new XElement("Race", x))),
		new XElement("Prefer", PreferredRaces.OrderBy(x => x.Key).Select(x => new XElement("Race", new XAttribute("id", x.Key), x.Value))),
		new XElement("Weights", Weights.Select(x => new XElement("Weight", new XAttribute("name", x.Key), x.Value))));

	public static AnimalHuntingSettings Load(XElement? root)
	{
		var result = new AnimalHuntingSettings();
		if (root is null) return result;
		T ReadEnum<T>(string name, T fallback) where T : struct, Enum =>
			Enum.TryParse<T>(root.Element(name)?.Value, true, out var value) && Enum.IsDefined(value) ? value : fallback;
		double Number(string name, double fallback) => double.TryParse(root.Element(name)?.Value,
			NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && double.IsFinite(value) ? value : fallback;
		result.Enabled = bool.TryParse(root.Attribute("enabled")?.Value, out var enabled) && enabled;
		result.People = ReadEnum("People", result.People);
		result.Selection = ReadEnum("Selection", result.Selection);
		result.Opening = ReadEnum("Opening", result.Opening);
		result.Followup = ReadEnum("Followup", result.Followup);
		result.PreferredLayer = Enum.TryParse<RoomLayer>(root.Element("PreferredLayer")?.Value, true, out var layer) && Enum.IsDefined(layer) ? layer : null;
		result.Opportunistic = !bool.TryParse(root.Element("Opportunistic")?.Value, out var opportunity) || opportunity;
		result.EngageThreshold = Math.Clamp(Number("Engage", 60), 0, 100);
		result.AbandonThreshold = Math.Clamp(Number("Abandon", 35), 0, result.EngageThreshold);
		result.StarvationAdjustment = Math.Clamp(Number("Starvation", 10), 0, 100);
		result.ConfidenceBias = Math.Clamp(Number("Confidence", 0), -100, 100);
		result.PursuitRange = (int)Math.Clamp(Number("Range", 5), 1, 20);
		result.PursuitTimeout = TimeSpan.FromSeconds(Math.Clamp(Number("TimeoutSeconds", 300), 1, 86400));
		result.LostTimeout = TimeSpan.FromSeconds(Math.Clamp(Number("LostSeconds", 60), 1, 86400));
		result.MinimumSizeDifference = int.TryParse(root.Element("MinimumSize")?.Value, out var min) ? min : null;
		result.MaximumSizeDifference = int.TryParse(root.Element("MaximumSize")?.Value, out var max) ? max : null;
		long Identifier(string name) => long.TryParse(root.Element(name)?.Value, out var id) && id > 0 ? id : 0;
		result.ClassificationProgId = Identifier("ClassificationProg");
		result.EligibilityProgId = Identifier("EligibilityProg");
		result.PreferenceProgId = Identifier("PreferenceProg");
		foreach (var item in root.Element("Include")?.Elements("Race") ?? []) if (long.TryParse(item.Value, out var id) && id > 0) result.IncludedRaces.Add(id);
		foreach (var item in root.Element("Exclude")?.Elements("Race") ?? []) if (long.TryParse(item.Value, out var id) && id > 0) result.ExcludedRaces.Add(id);
		foreach (var item in root.Element("Prefer")?.Elements("Race") ?? [])
			if (long.TryParse(item.Attribute("id")?.Value, out var id) && id > 0 && double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && double.IsFinite(value)) result.PreferredRaces[id] = value;
		foreach (var item in root.Element("Weights")?.Elements("Weight") ?? [])
			if (item.Attribute("name")?.Value is { } name && result.Weights.ContainsKey(name) && double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && double.IsFinite(value)) result.Weights[name] = Math.Clamp(value, -1000, 1000);
		return result;
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

internal sealed record ClothingSourceLocation(string File, int Line)
{
	public override string ToString() => $"{File}:{Line}";
	internal InvalidDataException Error(string message) => new($"{this}: {message}");
}

internal sealed record ClothingWearableBinding(long ComponentId, int RevisionNumber,
	long DefaultProfileId, IReadOnlyList<long> ProfileIds, bool Bulky, double LayerWeight);

internal static class WearableComponentValidation
{
	internal static ClothingWearableBinding BindWearable(GameItemComponentProto component,
		IReadOnlyCollection<WearProfile> wearProfiles, IReadOnlyCollection<long> progIds, ClothingSourceLocation source)
	{
		try
		{
			var root = XElement.Parse(component.Definition);
			if (root.Name != "Definition") throw source.Error($"Wearable {component.Name} requires a Definition XML root.");
			foreach (var name in new[] { "Profiles", "LayerWeightConsumption", "WearableProg", "WhyCannotWearProg", "Waterproof", "SeeThroughDamageRatio" })
				if (root.Elements(name).Count() > 1) throw source.Error($"Wearable {component.Name} repeats XML element {name}.");
			var profilesElement = root.Element("Profiles") ?? throw source.Error($"Wearable {component.Name} has no Profiles element.");
			var profileIds = profilesElement.Elements("Profile").Select(x => long.Parse(x.Value, CultureInfo.InvariantCulture)).ToArray();
			if (profileIds.Length == 0 || profileIds.Any(x => x <= 0) || profileIds.Distinct().Count() != profileIds.Length)
				throw source.Error($"Wearable {component.Name} requires unique, positive wear-profile references.");
			var defaultId = long.Parse(profilesElement.Attribute("Default")?.Value ?? "0", CultureInfo.InvariantCulture);
			if (!profileIds.Contains(defaultId)) throw source.Error($"Wearable {component.Name} default profile must be one of its declared profiles.");
			foreach (var id in profileIds)
			{
				var matches = wearProfiles.Where(x => x.Id == id).ToArray();
				if (matches.Length != 1) throw source.Error($"Wearable {component.Name} has a missing or ambiguous wear profile {id}.");
				if (matches[0].Type is not ("Direct" or "Shape"))
					throw source.Error($"Wear profile {matches[0].Name} has unknown runtime type {matches[0].Type}.");
			}
			var layerWeight = Number(root.Element("LayerWeightConsumption")?.Value, 1.0);
			if (layerWeight < 0) throw source.Error($"Wearable {component.Name} has negative layer consumption.");
			var bulky = Boolean(root.Attribute("Bulky")?.Value, false);
			_ = Boolean(root.Attribute("DisplayInventoryWhenWorn")?.Value, true);
			_ = Boolean(root.Element("Waterproof")?.Value, false);
			_ = Ratio(root.Element("SeeThroughDamageRatio")?.Value);
			var waterproof = root.Element("Waterproof");
			if (waterproof is not null && waterproof.Attribute("ratio") is null)
				throw source.Error($"Wearable {component.Name} Waterproof requires a ratio attribute for runtime loading.");
			_ = Ratio(waterproof?.Attribute("ratio")?.Value);
			foreach (var name in new[] { "WearableProg", "WhyCannotWearProg" })
			{
				var id = long.Parse(root.Element(name)?.Value ?? "0", CultureInfo.InvariantCulture);
				if (id < 0 || (id > 0 && !progIds.Contains(id))) throw source.Error($"Wearable {component.Name} has an unresolved {name} reference {id}.");
				if (id > 0 && name == "WearableProg")
					throw source.Error($"Wearable {component.Name} has a conditional wear program; its body/item-dependent eligibility requires explicit proof before stock clothing admission.");
			}
			return new(component.Id, component.RevisionNumber, defaultId, Array.AsReadOnly(profileIds), bulky, layerWeight);
		}
		catch (Exception ex) when (ex is XmlException or FormatException or OverflowException)
		{
			throw source.Error($"Invalid Wearable XML on {component.Name}: {ex.Message}");
		}
	}

	private static bool Boolean(string? value, bool fallback) => value is null ? fallback : bool.Parse(value);
	private static double Ratio(string? value)
	{
		var result = Number(value, 0.5);
		return result is >= 0 and <= 1 ? result : throw new FormatException("Wearable damage ratios must be between 0 and 1.");
	}

	private static double Number(string? value, double fallback)
	{
		var result = value is null ? fallback : double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
		return double.IsFinite(result) ? result : throw new FormatException("Wearable numeric values must be finite.");
	}
}

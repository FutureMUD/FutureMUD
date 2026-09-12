#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

internal sealed record ClothingWearLocationBinding(long TargetId, int Count, bool Mandatory,
	bool Transparent, bool NoArmour, bool PreventsRemoval, bool HidesSevered, bool IsWearLocation = true);

internal sealed record ClothingWearProfileBinding(long ProfileId, string Name, long BodyId, bool IsShape,
	IReadOnlyList<ClothingWearLocationBinding> Locations);

internal sealed record ClothingWornEntryBinding(ClothingSourceLocation Source, string EntryKey,
	ClothingWearableBinding Wearable, ClothingWearProfileBinding Profile);

/// <summary>
/// Read-only validation of the persisted wear-profile geometry. This resolves the designed body's
/// definitions, not race/gender-specific anatomy, item sizing or an individual wearer's fit.
/// </summary>
internal sealed class IndustrialisedClothingWearProfiles
{
	private sealed record Body(long Id, long? ParentId);
	private sealed record Part(long Id, long BodyId, string Name, long ShapeId, BodypartTypeEnum Type);
	private sealed record Shape(long Id, string Name);
	private readonly ILookup<long, Body> _bodies;
	private readonly Part[] _parts;
	private readonly Shape[] _shapes;

	internal IndustrialisedClothingWearProfiles(IEnumerable<BodyProto> bodies, IEnumerable<BodypartProto> parts,
		IEnumerable<BodypartShape> shapes)
	{
		_bodies = bodies.Select(x => new Body(x.Id, x.CountsAsId)).ToLookup(x => x.Id);
		_parts = parts.Select(x => new Part(x.Id, x.BodyId, x.Name, x.BodypartShapeId, (BodypartTypeEnum)x.BodypartType)).ToArray();
		_shapes = shapes.Select(x => new Shape(x.Id, x.Name)).ToArray();
	}

	internal static IndustrialisedClothingWearProfiles Read(FuturemudDatabaseContext context) =>
		new(context.BodyProtos.AsNoTracking().ToArray(), context.BodypartProtos.AsNoTracking().ToArray(),
			context.BodypartShapes.AsNoTracking().ToArray());

	internal static double MaximumLayerWeight(IEnumerable<StaticConfiguration> settings, ClothingSourceLocation source)
	{
		const string key = "MaximumLayerWeight";
		var matches = settings.Where(x => x.SettingName.Equals(key, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (matches.Length > 1 || (matches.Length == 1 && matches[0].SettingName != key))
			throw source.Error($"Missing exact or ambiguous {key} setting.");
		var value = matches.Length == 1 ? matches[0].Definition : DefaultStaticSettings.DefaultStaticConfigurations[key];
		if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) || !double.IsFinite(result) || result < 0)
			throw source.Error($"{key} must be a finite nonnegative number.");
		return result;
	}

	/// <summary>
	/// Rejects definite conflicts at mandatory direct locations. This is a lower-bound screen only:
	/// optional coverage, shape placement, CountsAs collisions, sizing and wearer anatomy still need
	/// complete ensemble proof at the later outfit/content gates. No successful call is a certificate
	/// of a finished production outfit.
	/// </summary>
	internal static void ValidateMandatoryLayers(IEnumerable<ClothingWornEntryBinding> entries, double maximum)
	{
		var worn = new Dictionary<long, List<ClothingWornEntryBinding>>();
		foreach (var entry in entries)
		{
			if (!double.IsFinite(maximum) || maximum < 0) throw entry.Source.Error("Invalid maximum layer weight.");
			if (entry.Wearable.LayerWeight > maximum)
				throw entry.Source.Error($"Outfit entry {entry.EntryKey} alone exceeds MaximumLayerWeight {maximum.ToString(CultureInfo.InvariantCulture)}.");
			if (entry.Profile.IsShape) continue;
			foreach (var location in entry.Profile.Locations.Where(x => x.Mandatory))
			{
				if (!worn.TryGetValue(location.TargetId, out var prior)) worn.Add(location.TargetId, prior = []);
				if (prior.Sum(x => x.Wearable.LayerWeight) + entry.Wearable.LayerWeight > maximum)
					throw entry.Source.Error($"Outfit entry {entry.EntryKey} exceeds MaximumLayerWeight at mandatory bodypart {location.TargetId}; earlier entries: {string.Join(", ", prior.Select(x => x.EntryKey))}.");
				var bulky = prior.FirstOrDefault(x => x.Wearable.Bulky);
				if (entry.Wearable.Bulky && bulky is not null)
					throw entry.Source.Error($"Bulky outfit entries {bulky.EntryKey} and {entry.EntryKey} conflict at mandatory bodypart {location.TargetId}.");
				prior.Add(entry);
			}
		}
	}

	internal ClothingWearProfileBinding Bind(WearProfile profile, ClothingSourceLocation source)
	{
		var label = $"Wear profile {profile.Name} (#{profile.Id})";
		if (profile.Type is not ("Direct" or "Shape")) throw source.Error($"{label} has unknown runtime type {profile.Type}.");
		var bodyIds = new HashSet<long>();
		long? bodyId = profile.BodyPrototypeId;
		while (bodyId.HasValue)
		{
			if (bodyId <= 0 || !bodyIds.Add(bodyId.Value)) throw source.Error($"{label} has an invalid or cyclic designed-body ancestry.");
			var matches = _bodies[bodyId.Value].ToArray();
			if (matches.Length != 1) throw source.Error($"{label} has a missing or ambiguous body prototype {bodyId}.");
			bodyId = matches[0].ParentId;
		}
		var parts = _parts.Where(x => bodyIds.Contains(x.BodyId) && IsExternalLocation(x.Type)).ToArray();
		try
		{
			var root = XElement.Parse(profile.WearlocProfiles);
			if (root.Name != "Profiles") throw source.Error($"{label} requires a Profiles geometry root.");
			var isShape = profile.Type == "Shape";
			var elementName = isShape ? "Shape" : "Profile";
			if (!root.Elements().Any() || root.Elements().Any(x => x.Name != elementName))
				throw source.Error($"{label} requires nonempty {elementName} geometry, with no unknown location elements.");
			var locations = new List<ClothingWearLocationBinding>();
			foreach (var element in root.Elements())
			{
				var target = Required(element, isShape ? "ShapeId" : "Bodypart");
				var count = isShape ? int.Parse(Required(element, "Count"), CultureInfo.InvariantCulture) : 1;
				if (count <= 0) throw source.Error($"{label} requires a positive shape count.");
				var mandatory = bool.Parse(Required(element, "Mandatory"));
				long targetId;
				var wearableLocation = true;
				if (isShape)
				{
					var matches = _shapes.Where(x => Matches(target, x.Id, x.Name)).ToArray();
					if (matches.Length != 1 || matches[0].Id <= 0) throw source.Error($"{label} has a missing or ambiguous shape {target}.");
					targetId = matches[0].Id;
					if (mandatory && parts.Count(x => x.ShapeId == targetId && IsWearLocation(x.Type)) < count)
						throw source.Error($"{label} requires {count} locations of shape {target}, unavailable on its designed body.");
				}
				else
				{
					var matches = parts.Where(x => Matches(target, x.Id, x.Name)).ToArray();
					if (matches.Length != 1 || matches[0].Id <= 0)
						throw source.Error($"{label} has a missing, ambiguous or non-wearable bodypart {target} in its designed body.");
					targetId = matches[0].Id;
					wearableLocation = IsWearLocation(matches[0].Type);
					if (mandatory && !wearableLocation)
						throw source.Error($"{label} requires non-wearable bodypart {target} on its designed body.");
					if (_shapes.Count(x => x.Id == matches[0].ShapeId) != 1)
						throw source.Error($"{label} bodypart {target} has a missing or ambiguous shape.");
				}
				if (locations.Any(x => x.TargetId == targetId)) throw source.Error($"{label} repeats the same resolved location {target}.");
				locations.Add(new(targetId, count, mandatory,
					bool.Parse(Required(element, "Transparent")), bool.Parse(Required(element, "NoArmour")),
					bool.Parse(Required(element, "PreventsRemoval")), bool.Parse(element.Attribute("HidesSevered")?.Value ?? "false"), wearableLocation));
			}
			if (isShape ? !locations.Any(x => parts.Any(p => p.ShapeId == x.TargetId && IsWearLocation(p.Type)))
				: !locations.Any(x => x.IsWearLocation))
				throw source.Error($"{label} has no wearable locations on its designed body.");
			return new(profile.Id, profile.Name, profile.BodyPrototypeId, isShape, locations.AsReadOnly());
		}
		catch (Exception ex) when (ex is XmlException or FormatException or OverflowException)
		{
			throw source.Error($"Invalid geometry for {label}: {ex.Message}");
		}
	}

	private static string Required(XElement element, string attribute) => element.Attribute(attribute)?.Value
		?? throw new FormatException($"Missing required {attribute} attribute.");

	private static bool Matches(string target, long id, string name) =>
		long.TryParse(target, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric)
			? numeric == id : name.Equals(target, StringComparison.InvariantCultureIgnoreCase);

	// Kept explicit and tested against the runtime factory: not every external part implements IWear.
	// DirectWearProfile loads all external targets, including optional tongues. Preserve those
	// references for wearer-specific CountsAs resolution, but do not treat them as designed-body IWear.
	internal static bool IsExternalLocation(BodypartTypeEnum type) => IsWearLocation(type) || type == BodypartTypeEnum.Tongue;

	internal static bool IsWearLocation(BodypartTypeEnum type) => type is
		BodypartTypeEnum.Wear or BodypartTypeEnum.GrabbingWielding or BodypartTypeEnum.Grabbing or BodypartTypeEnum.Wielding or
		BodypartTypeEnum.Standing or BodypartTypeEnum.Eye or BodypartTypeEnum.Mouth or BodypartTypeEnum.Wing or
		BodypartTypeEnum.Joint or BodypartTypeEnum.Fin or BodypartTypeEnum.Gill or BodypartTypeEnum.Blowhole or
		BodypartTypeEnum.BonyDrapeable or BodypartTypeEnum.BonyGrabbingWielding or BodypartTypeEnum.NonImmobilisingBonyDrapeable;
}

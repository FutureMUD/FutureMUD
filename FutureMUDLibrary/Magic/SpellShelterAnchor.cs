#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Versioned native fields, never an ID search through arbitrary definitions.</summary>
public sealed record SpellShelterAnchor(SpellShelterKind Kind, long AnchorRoomId, long FallbackRoomId,
	double? AnchorRoutePosition, int MaximumOccupants, long AnchorOverlayId = 0)
{
	public const string Family = "native-occupied-shelter-v1";

	public string Save()
	{
		Validate();
		return new XElement("Shelter", new XAttribute("version", 1), new XElement("Kind", Kind),
			new XElement("AnchorRoom", AnchorRoomId), new XElement("FallbackRoom", FallbackRoomId),
			new XElement("MaximumOccupants", MaximumOccupants),
			new XElement("AnchorOverlay", AnchorOverlayId),
			AnchorRoutePosition is { } position ? new XElement("AnchorRoutePosition", position.ToString("R", CultureInfo.InvariantCulture)) : null)
			.ToString(SaveOptions.DisableFormatting);
	}

	public static SpellShelterAnchor Load(string definition)
	{
		var root = XElement.Parse(definition);
		if (root.Name != "Shelter" || (string?)root.Attribute("version") != "1" ||
			root.Elements().Any(x => x.Name != "Kind" && x.Name != "AnchorRoom" && x.Name != "FallbackRoom" &&
				x.Name != "MaximumOccupants" && x.Name != "AnchorRoutePosition" && x.Name != "AnchorOverlay") ||
			root.Elements().GroupBy(x => x.Name).Any(x => x.Count() != 1))
			throw new FormatException("Unsupported shelter anchor schema.");
		var value = new SpellShelterAnchor(Enum.Parse<SpellShelterKind>(root.Element("Kind")!.Value),
			(long)root.Element("AnchorRoom")!, (long)root.Element("FallbackRoom")!,
			root.Element("AnchorRoutePosition") is { } position ? double.Parse(position.Value, CultureInfo.InvariantCulture) : null,
			(int)root.Element("MaximumOccupants")!, (long?)root.Element("AnchorOverlay") ?? 0);
		value.Validate();
		return value;
	}

	public void Validate()
	{
		if (!Enum.IsDefined(Kind) || AnchorRoomId <= 0 || FallbackRoomId <= 0 || AnchorOverlayId < 0 || MaximumOccupants is < 1 or > 128 ||
			AnchorRoutePosition is { } p && (!double.IsFinite(p) || p < 0))
			throw new ArgumentException("Invalid shelter anchor, fallback, capacity or coordinate.");
	}
}

#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;

namespace MudSharp.Magic;

/// <summary>Exact source and authored policy. Owned instance/body/item IDs are immutable journal claims.</summary>
public sealed record SpellProjectionAnchor(long InstanceId, long BodyId, long RoomId, int Layer, double? RoutePosition,
	SpellProjectionConfiguration Configuration)
{
	public const string Family = "native-identity-projection-v1";
	public XElement Save() => new("ProjectionAnchor", new XAttribute("version", 1),
		new XAttribute("instance", InstanceId), new XAttribute("body", BodyId), new XAttribute("room", RoomId),
		new XAttribute("layer", Layer), new XAttribute("point", RoutePosition?.ToString("R", System.Globalization.CultureInfo.InvariantCulture) ?? ""),
		Definition(Configuration));
	public static XElement Definition(SpellProjectionConfiguration c) => new("Policy", new XElement("Kind", c.Kind),
		new XElement("Plane", c.PlaneId), new XElement("EffigyPrototype", c.EffigyPrototypeId),
		new XElement("SecondsPerGrade", c.SecondsPerGrade), new XElement("MaximumRoomDistance", c.MaximumRoomDistance),
		new XElement("BacklashDamage", c.BacklashDamage), new XElement("CrossClosedDoors", c.CrossClosedDoors));
	public static SpellProjectionConfiguration ReadPolicy(XElement root)
	{
		var fields = new[] { "Kind", "Plane", "EffigyPrototype", "SecondsPerGrade", "MaximumRoomDistance", "BacklashDamage", "CrossClosedDoors" };
		if (root.Name != "Policy" || root.HasAttributes || root.Elements().Count() != fields.Length ||
			fields.Any(x => root.Elements(x).Count() != 1) || root.Elements().Any(x => x.HasElements || x.HasAttributes))
			throw new FormatException("Unknown, duplicate or missing projection policy fields.");
		return new(Enum.Parse<SpellProjectionKind>(root.Element("Kind")!.Value), (long)root.Element("Plane")!,
			(long)root.Element("EffigyPrototype")!, (double)root.Element("SecondsPerGrade")!,
			(int)root.Element("MaximumRoomDistance")!, (double)root.Element("BacklashDamage")!, (bool)root.Element("CrossClosedDoors")!);
	}
	public static SpellProjectionAnchor Load(string value)
	{
		var root = XElement.Parse(value);
		var fields = new[] { "version", "instance", "body", "room", "layer", "point" };
		if (root.Name != "ProjectionAnchor" || (int?)root.Attribute("version") != 1 ||
			root.Attributes().Count() != fields.Length || fields.Any(x => root.Attribute(x) is null) ||
			root.Elements().Count() != 1 || root.Element("Policy") is null)
			throw new FormatException("Invalid projection anchor schema.");
		var point = root.Attribute("point")!.Value;
		var result = new SpellProjectionAnchor((long)root.Attribute("instance")!, (long)root.Attribute("body")!,
			(long)root.Attribute("room")!, (int)root.Attribute("layer")!,
			point.Length == 0 ? null : double.Parse(point, System.Globalization.CultureInfo.InvariantCulture), ReadPolicy(root.Element("Policy")!));
		if (result.InstanceId <= 0 || result.BodyId <= 0 || result.RoomId <= 0 || !Enum.IsDefined((RoomLayer)result.Layer) ||
			result.RoutePosition is { } coordinate && (!double.IsFinite(coordinate) || coordinate < 0) || result.Configuration.Error(1) is not null)
			throw new FormatException("Invalid projection anchor values.");
		return result;
	}
}

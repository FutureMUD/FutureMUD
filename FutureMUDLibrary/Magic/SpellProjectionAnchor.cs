#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;

namespace MudSharp.Magic;

public sealed record SpellProjectionBorrowedSource(long ItemId, long PrototypeId, int PrototypeRevision,
	long HolderBodyId, long HolderInstanceId, int Quantity);

/// <summary>Exact source and authored policy. Owned instance/body/item IDs are immutable journal claims.</summary>
public sealed record SpellProjectionAnchor(long InstanceId, long BodyId, long RoomId, int Layer, double? RoutePosition,
	SpellProjectionConfiguration Configuration, SpellProjectionBorrowedSource? BorrowedSource = null)
{
	public long? BorrowedSourceItemId => BorrowedSource?.ItemId;
	public const string Family = "native-identity-projection-v1";
	public XElement Save() => new("ProjectionAnchor", new XAttribute("version", 1),
		new XAttribute("instance", InstanceId), new XAttribute("body", BodyId), new XAttribute("room", RoomId),
		new XAttribute("layer", Layer), new XAttribute("point", RoutePosition?.ToString("R", System.Globalization.CultureInfo.InvariantCulture) ?? ""),
		Definition(Configuration), BorrowedSource is { } source ? new XElement("BorrowedSourceItem", new XAttribute("version", 1),
			new XAttribute("itemId", source.ItemId), new XAttribute("prototypeId", source.PrototypeId), new XAttribute("prototypeRevision", source.PrototypeRevision),
			new XAttribute("holderBodyId", source.HolderBodyId), new XAttribute("holderInstanceId", source.HolderInstanceId), new XAttribute("quantity", source.Quantity)) : null);
	public static XElement Definition(SpellProjectionConfiguration c) => new("Policy", new XElement("Kind", c.Kind),
		new XElement("Plane", c.PlaneId), new XElement("EffigyPrototype", c.EffigyPrototypeId),
		new XElement("SecondsPerGrade", c.SecondsPerGrade), new XElement("MaximumRoomDistance", c.MaximumRoomDistance),
		new XElement("BacklashDamage", c.BacklashDamage), new XElement("CrossClosedDoors", c.CrossClosedDoors),
		c.RoomDistancePerGrade == 0 ? null : new XElement("RequestedGradeRange", new XAttribute("version", 1),
			new XAttribute("perGrade", c.RoomDistancePerGrade), new XAttribute("grade", c.RangeGrade)),
		c.RequiresHeldSourceItem ? new XElement("HeldSourceItem", new XAttribute("version", 1)) : null);
	public static SpellProjectionConfiguration ReadPolicy(XElement root)
	{
		var fields = new[] { "Kind", "Plane", "EffigyPrototype", "SecondsPerGrade", "MaximumRoomDistance", "BacklashDamage", "CrossClosedDoors" };
		var range = root.Element("RequestedGradeRange");
		var source = root.Element("HeldSourceItem");
		if (root.Name != "Policy" || root.HasAttributes || root.Elements().Count() != fields.Length + (range is null ? 0 : 1) + (source is null ? 0 : 1) ||
			fields.Any(x => root.Elements(x).Count() != 1) || root.Elements().Where(x => x != range && x != source).Any(x => x.HasElements || x.HasAttributes) ||
			source is not null && (source.HasElements || !string.IsNullOrWhiteSpace(source.Value) || source.Attributes().Count() != 1 || (int?)source.Attribute("version") != 1) ||
			range is not null && (range.HasElements || !string.IsNullOrWhiteSpace(range.Value) || range.Attributes().Count() != 3 ||
				(int?)range.Attribute("version") != 1 || (int?)range.Attribute("perGrade") != 10 ||
				(int?)range.Attribute("grade") is not (>= 0 and <= 7)))
			throw new FormatException("Unknown, duplicate or missing projection policy fields.");
		return new(Enum.Parse<SpellProjectionKind>(root.Element("Kind")!.Value), (long)root.Element("Plane")!,
			(long)root.Element("EffigyPrototype")!, (double)root.Element("SecondsPerGrade")!,
			(int)root.Element("MaximumRoomDistance")!, (double)root.Element("BacklashDamage")!, (bool)root.Element("CrossClosedDoors")!,
			(int?)range?.Attribute("perGrade") ?? 0, (int?)range?.Attribute("grade") ?? 0, source is not null);
	}
	public static SpellProjectionAnchor Load(string value)
	{
		var root = XElement.Parse(value);
		var fields = new[] { "version", "instance", "body", "room", "layer", "point" };
		var source = root.Element("BorrowedSourceItem");
		if (root.Name != "ProjectionAnchor" || (int?)root.Attribute("version") != 1 ||
			root.Attributes().Count() != fields.Length || fields.Any(x => root.Attribute(x) is null) ||
			root.Elements().Count() != 1 + (source is null ? 0 : 1) || root.Element("Policy") is null)
			throw new FormatException("Invalid projection anchor schema.");
		var sourceFields = new[] { "version", "itemId", "prototypeId", "prototypeRevision", "holderBodyId", "holderInstanceId", "quantity" };
		if (source is not null && (source.HasElements || !string.IsNullOrWhiteSpace(source.Value) || source.Attributes().Count() != sourceFields.Length ||
			sourceFields.Any(x => source.Attribute(x) is null) || (int?)source.Attribute("version") != 1))
			throw new FormatException("Invalid borrowed projection source schema.");
		var point = root.Attribute("point")!.Value;
		var result = new SpellProjectionAnchor((long)root.Attribute("instance")!, (long)root.Attribute("body")!,
			(long)root.Attribute("room")!, (int)root.Attribute("layer")!,
			point.Length == 0 ? null : double.Parse(point, System.Globalization.CultureInfo.InvariantCulture), ReadPolicy(root.Element("Policy")!),
			source is null ? null : new((long)source.Attribute("itemId")!, (long)source.Attribute("prototypeId")!, (int)source.Attribute("prototypeRevision")!,
				(long)source.Attribute("holderBodyId")!, (long)source.Attribute("holderInstanceId")!, (int)source.Attribute("quantity")!));
		if (result.InstanceId <= 0 || result.BodyId <= 0 || result.RoomId <= 0 || !Enum.IsDefined((RoomLayer)result.Layer) ||
			result.RoutePosition is { } coordinate && (!double.IsFinite(coordinate) || coordinate < 0) ||
			result.Configuration.RoomDistancePerGrade != 0 && result.Configuration.RangeGrade == 0 ||
			result.Configuration.Error(Math.Max(1, result.Configuration.RangeGrade)) is not null ||
			result.Configuration.RequiresHeldSourceItem != (result.BorrowedSource is not null) ||
			result.BorrowedSource is { } borrowed && (borrowed.ItemId <= 0 || borrowed.PrototypeId <= 0 || borrowed.PrototypeRevision < 0 ||
				borrowed.Quantity != 1 || borrowed.HolderBodyId != result.BodyId || borrowed.HolderInstanceId != result.InstanceId))
			throw new FormatException("Invalid projection anchor values.");
		return result;
	}
}

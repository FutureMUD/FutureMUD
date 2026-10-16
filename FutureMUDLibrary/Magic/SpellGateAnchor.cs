#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Immutable authority for exactly two temporary rooms and their original endpoints.</summary>
public sealed record SpellGateAnchor(long SourceRoomId, long SourceOverlayId, long DestinationRoomId,
	long DestinationOverlayId, long MarkItemId, Guid MarkParentIdentity, long AutobuilderTemplateId,
	long UndeadMeritId,
	int SuffocationPulseSeconds)
{
	public const string Family = "native-nilaz-gate-v1";

	public string Save()
	{
		Validate();
		return new XElement("NilazGate", new XAttribute("version", 1),
			new XElement("SourceRoom", SourceRoomId), new XElement("SourceOverlay", SourceOverlayId),
			new XElement("DestinationRoom", DestinationRoomId), new XElement("DestinationOverlay", DestinationOverlayId),
			new XElement("MarkItem", MarkItemId), new XElement("MarkParent", MarkParentIdentity),
			new XElement("AutobuilderTemplate", AutobuilderTemplateId),
			new XElement("UndeadMerit", UndeadMeritId),
			new XElement("SuffocationPulseSeconds", SuffocationPulseSeconds)).ToString(SaveOptions.DisableFormatting);
	}

	public static SpellGateAnchor Load(string xml)
	{
		var root = XElement.Parse(xml);
		var names = new[] { "SourceRoom", "SourceOverlay", "DestinationRoom", "DestinationOverlay",
			"MarkItem", "MarkParent", "AutobuilderTemplate", "UndeadMerit", "SuffocationPulseSeconds" };
		if (root.Name != "NilazGate" || (string?)root.Attribute("version") != "1" ||
			root.Attributes().Count() != 1 || root.Elements().Count() != names.Length ||
			root.Elements().Select(x => x.Name.LocalName).Order().SequenceEqual(names.Order()) == false ||
			root.Elements().Any(x => x.HasAttributes || x.HasElements || x.Name.Namespace != XNamespace.None))
			throw new FormatException("Unsupported Nilaz gate provenance.");
		var value = new SpellGateAnchor((long)root.Element("SourceRoom")!, (long)root.Element("SourceOverlay")!,
			(long)root.Element("DestinationRoom")!, (long)root.Element("DestinationOverlay")!,
			(long)root.Element("MarkItem")!, Guid.Parse(root.Element("MarkParent")!.Value),
			(long)root.Element("AutobuilderTemplate")!, (long)root.Element("UndeadMerit")!,
			(int)root.Element("SuffocationPulseSeconds")!);
		value.Validate();
		return value;
	}

	public void Validate()
	{
		if (SourceRoomId <= 0 || SourceOverlayId <= 0 || DestinationRoomId <= 0 ||
			DestinationOverlayId <= 0 || SourceRoomId == DestinationRoomId || MarkItemId <= 0 ||
			MarkParentIdentity == Guid.Empty || AutobuilderTemplateId <= 0 || UndeadMeritId <= 0 ||
			SuffocationPulseSeconds is < 10 or > 3600)
			throw new ArgumentException("A Nilaz gate needs exact distinct endpoints, a marked item and a bounded pulse interval.");
	}
}

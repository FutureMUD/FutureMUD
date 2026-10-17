#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.GameItems;

namespace MudSharp.Magic;

public enum SpellPocketAccess { Bearer, Creator }

public sealed record SpellPocketConfiguration(long PrototypeId, double CapacityPerGrade,
	SizeCategory MaximumSize, double SecondsPerGrade, SpellPocketAccess Access, long FallbackRoomId)
{
	public void Validate()
	{
		if (PrototypeId <= 0 || FallbackRoomId <= 0 || !Enum.IsDefined(MaximumSize) || !Enum.IsDefined(Access) ||
			!double.IsFinite(CapacityPerGrade) || CapacityPerGrade <= 0 || !double.IsFinite(CapacityPerGrade * 7) ||
			!double.IsFinite(SecondsPerGrade) || SecondsPerGrade < 0.000001 || SecondsPerGrade * 7 > TimeSpan.FromDays(365).TotalSeconds)
			throw new ArgumentException("Pocket policy requires positive finite capacity/timing, valid size/access and explicit native prototype/fallback.");
	}

	/// <summary>Match the lifecycle journal's native datetime(6) precision before creation.</summary>
	public TimeSpan DurationForGrade(int grade)
	{
		Validate();
		if (grade is < 1 or > 7) throw new ArgumentOutOfRangeException(nameof(grade));
		return TimeSpan.FromTicks(checked((long)Math.Round(SecondsPerGrade * grade * 1_000_000,
			MidpointRounding.AwayFromZero) * 10));
	}

	public XElement Save() => new("Pocket", new XAttribute("version", 1), new XElement("Prototype", PrototypeId),
		new XElement("CapacityPerGrade", CapacityPerGrade), new XElement("MaximumSize", MaximumSize),
		new XElement("SecondsPerGrade", SecondsPerGrade), new XElement("Access", Access), new XElement("FallbackRoom", FallbackRoomId));

	public static SpellPocketConfiguration Load(XElement root, bool validate = true)
	{
		var fields = new[] { "Prototype", "CapacityPerGrade", "MaximumSize", "SecondsPerGrade", "Access", "FallbackRoom" };
		if (root.Name != "Pocket" || (string?)root.Attribute("version") != "1" || root.Attributes().Count() != 1 ||
			root.Elements().Count() != fields.Length || fields.Any(x => root.Elements(x).Count() != 1) ||
			root.Elements().Any(x => x.HasElements || x.HasAttributes)) throw new FormatException("Invalid pocket policy envelope.");
		var result = new SpellPocketConfiguration((long)root.Element("Prototype")!, (double)root.Element("CapacityPerGrade")!,
			Enum.Parse<SizeCategory>(root.Element("MaximumSize")!.Value), (double)root.Element("SecondsPerGrade")!,
			Enum.Parse<SpellPocketAccess>(root.Element("Access")!.Value), (long)root.Element("FallbackRoom")!);
		if (validate) result.Validate(); return result;
	}
}

public sealed record SpellPocketAnchor(long SourceItemId, int Grade, SpellPocketConfiguration Configuration)
{
	public const string Family = "native-item-pocket-v1";
	public double Capacity => Configuration.CapacityPerGrade * Grade;
	public string Save()
	{
		Configuration.Validate();
		if (SourceItemId <= 0 || Grade is < 1 or > 7) throw new ArgumentException("Pocket creation requires an exact borrowed source item and grade.");
		return new XElement("PocketAnchor", new XAttribute("version", 1), new XElement("SourceItem", SourceItemId),
			new XElement("Grade", Grade), Configuration.Save()).ToString(SaveOptions.DisableFormatting);
	}
	public static SpellPocketAnchor Load(string text)
	{
		var root = XElement.Parse(text);
		if (root.Name != "PocketAnchor" || (string?)root.Attribute("version") != "1" || root.Attributes().Count() != 1 ||
			root.Elements().Count() != 3 || new[] { "SourceItem", "Grade", "Pocket" }.Any(x => root.Elements(x).Count() != 1) ||
			root.Elements().Where(x => x.Name != "Pocket").Any(x => x.HasElements || x.HasAttributes))
			throw new FormatException("Invalid pocket creation envelope.");
		var result = new SpellPocketAnchor((long)root.Element("SourceItem")!, (int)root.Element("Grade")!, SpellPocketConfiguration.Load(root.Element("Pocket")!));
		result.Save(); return result;
	}
}

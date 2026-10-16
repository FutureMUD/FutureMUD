#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic;

/// <summary>Explicit native school and invocation-tag selectors; any match interdicts.</summary>
public sealed record SpellShelterWardConfiguration(IReadOnlyCollection<long> SchoolIds,
	IReadOnlyCollection<string> Tags, MagicInterdictionCoverage Coverage, bool IncludesSubschools = true)
{
	public void Validate()
	{
		if (!Enum.IsDefined(Coverage) || SchoolIds.Count + Tags.Count is < 1 or > 64 ||
			SchoolIds.Any(x => x <= 0) || SchoolIds.Distinct().Count() != SchoolIds.Count ||
			Tags.Any(x => string.IsNullOrWhiteSpace(x) || x.Length > 128 || x != x.Trim()) ||
			Tags.Distinct(StringComparer.OrdinalIgnoreCase).Count() != Tags.Count)
			throw new ArgumentException("Select 1-64 distinct native schools or invocation tags and a valid ward coverage.");
	}

	public XElement Save()
	{
		Validate();
		return new XElement("Ward", new XAttribute("version", 1), new XElement("Coverage", Coverage),
			new XElement("IncludesSubschools", IncludesSubschools), SchoolIds.OrderBy(x => x).Select(x => new XElement("School", x)),
			Tags.OrderBy(x => x, StringComparer.Ordinal).Select(x => new XElement("Tag", x)));
	}

	public static SpellShelterWardConfiguration Load(XElement root)
	{
		if (root.Name != "Ward" || (string?)root.Attribute("version") != "1" ||
			root.Attributes().Any(x => x.Name != "version") ||
			root.Elements().Any(x => x.Name != "Coverage" && x.Name != "IncludesSubschools" && x.Name != "School" && x.Name != "Tag") ||
			root.Elements("Coverage").Count() != 1 || root.Elements("IncludesSubschools").Count() != 1 ||
			root.Elements().Any(x => x.HasElements || x.HasAttributes))
			throw new FormatException("Unsupported shelter ward schema.");
		var value = new SpellShelterWardConfiguration(root.Elements("School").Select(x => (long)x).ToArray(),
			root.Elements("Tag").Select(x => x.Value).ToArray(), Enum.Parse<MagicInterdictionCoverage>(root.Element("Coverage")!.Value),
			(bool)root.Element("IncludesSubschools")!);
		value.Validate();
		return value;
	}
}

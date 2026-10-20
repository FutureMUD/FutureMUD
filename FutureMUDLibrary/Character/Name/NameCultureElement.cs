#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Character.Name;

/// <summary>One independently entered component. Rules describe naming declarations, never game entitlements.</summary>
public record NameCultureElement
{
	public NameCultureElement() { }

	public NameCultureElement(XElement definition)
	{
		Usage = (NameUsage)int.Parse(definition.Attribute("Usage")!.Value);
		MinimumCount = int.Parse(definition.Attribute("MinimumCount")!.Value);
		MaximumCount = int.Parse(definition.Attribute("MaximumCount")!.Value);
		Name = definition.Attribute("Name")!.Value;
		ChargenBlurb = string.Concat(definition.Nodes().OfType<XText>().Select(x => x.Value));
		DistinctFromUsage = int.TryParse(definition.Attribute("DistinctFromUsage")?.Value, out var distinct) ? (NameUsage)distinct : null;
		ConfirmationPrompt = definition.Element("ConfirmationPrompt")?.Value;
		DisplayChoicePrompt = definition.Element("DisplayChoicePrompt")?.Value;
		DefaultDisplay = !bool.TryParse(definition.Attribute("DefaultDisplay")?.Value, out var display) || display;
		SuggestedValues = definition.Element("SuggestedValues")?.Elements("Value").Select(x => x.Value).ToArray() ?? [];
		AllowedValues = definition.Element("AllowedValues")?.Elements("Value").Select(x => x.Value).ToArray() ?? [];
		if (definition.Element("ParentValues") is { } parent)
		{
			ParentUsage = (NameUsage)int.Parse(parent.Attribute("Usage")!.Value);
			ParentValues = parent.Elements("Parent").ToDictionary(x => x.Attribute("Value")!.Value,
				x => x.Elements("Value").Select(v => v.Value).ToArray(), StringComparer.Ordinal);
		}
	}

	public XElement SaveToXml() => new("Element",
		new XAttribute("Usage", (int)Usage), new XAttribute("MinimumCount", MinimumCount),
		new XAttribute("MaximumCount", MaximumCount), new XAttribute("Name", Name),
		DistinctFromUsage is { } distinct ? new XAttribute("DistinctFromUsage", (int)distinct) : null,
		DefaultDisplay ? null : new XAttribute("DefaultDisplay", false), new XCData(ChargenBlurb),
		ConfirmationPrompt is null ? null : new XElement("ConfirmationPrompt", ConfirmationPrompt),
		DisplayChoicePrompt is null ? null : new XElement("DisplayChoicePrompt", DisplayChoicePrompt),
		SuggestedValues.Count == 0 ? null : new XElement("SuggestedValues", SuggestedValues.Select(x => new XElement("Value", x))),
		AllowedValues.Count == 0 ? null : new XElement("AllowedValues", AllowedValues.Select(x => new XElement("Value", x))),
		ParentUsage is null ? null : new XElement("ParentValues", new XAttribute("Usage", (int)ParentUsage.Value),
			ParentValues.Select(x => new XElement("Parent", new XAttribute("Value", x.Key), x.Value.Select(v => new XElement("Value", v))))));

	public NameUsage Usage { get; init; }
	public int MinimumCount { get; init; }
	/// <summary>-1 means repeatable without a cultural maximum; technical input limits still apply.</summary>
	public int MaximumCount { get; init; }
	public string Name { get; init; } = "";
	public string ChargenBlurb { get; init; } = "";
	public string? ConfirmationPrompt { get; init; }
	public string? DisplayChoicePrompt { get; init; }
	public bool DefaultDisplay { get; init; } = true;
	public IReadOnlyList<string> SuggestedValues { get; init; } = Array.Empty<string>();
	public IReadOnlyList<string> AllowedValues { get; init; } = Array.Empty<string>();
	public NameUsage? ParentUsage { get; init; }
	public NameUsage? DistinctFromUsage { get; init; }
	public IReadOnlyDictionary<string, string[]> ParentValues { get; init; } = new Dictionary<string, string[]>();

	public bool AcceptsCount(int count) => count >= MinimumCount && (MaximumCount < 0 || count <= MaximumCount);

	public string? ValidateValues(IReadOnlyList<string> values, IReadOnlyDictionary<NameUsage, List<string>> selections)
	{
		if (!AcceptsCount(values.Count)) return $"Select {MinimumCount} to {(MaximumCount < 0 ? "any number of" : MaximumCount.ToString())} {Name} values.";
		if (values.Count == 0) return null;
		if (DistinctFromUsage is { } distinct && selections.TryGetValue(distinct, out var other) && values.Any(x => other.Contains(x, StringComparer.OrdinalIgnoreCase)))
			return $"Do not duplicate {Name} in another name component.";
		if (AllowedValues.Count > 0 && values.Any(x => !AllowedValues.Contains(x, StringComparer.Ordinal)))
			return $"Select a supplied {Name}, preserving its exact spelling.";
		if (ParentUsage is { } parent && (!selections.TryGetValue(parent, out var parents) || parents.Count != 1 ||
			!ParentValues.TryGetValue(parents[0], out var allowed) || values.Any(x => !allowed.Contains(x, StringComparer.Ordinal))))
			return $"Select {Name} only from the supplied list for its selected parent. A blank list means no values were supplied.";
		return null;
	}
}

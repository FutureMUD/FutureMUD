#nullable enable

using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Work.Agriculture;
using Db = MudSharp.Models;

namespace MudSharp.Character;

public static partial class NpcArchiveReferencePolicy
{
	/// <summary>Classifies only verified entity/property contracts; other columns retain the generic scan.</summary>
	public static bool HasReferenceOrUncertainty(Type entityType, string propertyName, string? value,
		long characterId, long bodyId, params long[] additionalPhysicalIds)
		=> HasReferenceOrUncertainty(entityType, propertyName, value, characterId, bodyId, out _, additionalPhysicalIds);

	public static bool HasReferenceOrUncertainty(Type entityType, string propertyName, string? value,
		long characterId, long bodyId, out string diagnostic, params long[] additionalPhysicalIds)
	{
		diagnostic = string.Empty;
		Func<XElement, bool>? classifier = entityType == typeof(Db.AgricultureOperation) ? IsOperation :
			entityType == typeof(Db.AgricultureCropDefinition) ? IsCrop :
			entityType == typeof(Db.AgricultureFieldProfile) ? IsProfile :
			entityType == typeof(Db.ArmourType) ? IsArmour : null;
		if (propertyName != nameof(Db.AgricultureOperation.Definition) || classifier is null)
		{
			return HasReferenceOrUncertainty(value, characterId, bodyId, additionalPhysicalIds);
		}

		// These loaders use an empty definition as the legacy, reference-free default.
		if (string.IsNullOrWhiteSpace(value)) return entityType == typeof(Db.ArmourType);
		try
		{
			var root = XElement.Parse(value);
			if (entityType == typeof(Db.ArmourType)) return !TryValidateArmour(root, out diagnostic);
			return !classifier(root);
		}
		catch (XmlException) { return true; }
	}

	// These codecs contain scalars and static builder names, never live character/body/instance/wound IDs.
	// Reject unknown shape even when it contains no matching ID: a future codec must be classified explicitly.
	private static bool IsProfile(XElement root) =>
		HasShape(root, "Profile", ["uses"], ["Score"], ["Score"]) && FieldUses(root) &&
		root.Elements().All(IsScoreValue);

	private static bool IsOperation(XElement root) =>
		HasShape(root, "Operation", ["woodlandYieldMultiplier", "woodlandYieldCost", "herdYieldMultiplier", "herdYieldCost"],
			["AllowedUses", "Apiary", "Score"], ["Score"]) &&
		IntegerAttributes(root, "woodlandYieldCost", "herdYieldCost") &&
		DoubleAttributes(root, "woodlandYieldMultiplier", "herdYieldMultiplier") &&
		root.Elements().All(child => child.Name.LocalName switch
		{
			"AllowedUses" => HasShape(child, "AllowedUses", ["uses"], []) && FieldUses(child),
			"Apiary" => HasShape(child, "Apiary", ["installHives", "pollinationRadius", "tendHealthDelta",
					"tendStoresDelta", "tendYieldDelta", "yieldMultiplier", "yieldCost"], ["Outputs"]) &&
				IntegerAttributes(child, "installHives", "pollinationRadius", "tendHealthDelta", "tendStoresDelta",
					"tendYieldDelta", "yieldCost") && DoubleAttributes(child, "yieldMultiplier") &&
				child.Elements().All(IsCommodities),
			"Score" => IsScoreValue(child),
			_ => false
		});

	private static bool IsCrop(XElement root) =>
		HasShape(root, "Crop", ["growthDays", "harvestWindowDays", "perennial", "harvestCycleDays", "minMoisture",
			"maxMoisture", "minTemperature", "maxTemperature"], ["Pollination", "PlantingWindows", "ScoreRanges", "Seeds", "Outputs"]) &&
		IntegerAttributes(root, "growthDays", "harvestWindowDays", "harvestCycleDays", "minMoisture", "maxMoisture",
			"minTemperature", "maxTemperature") && BooleanAttribute(root, "perennial") &&
		root.Elements().All(child => child.Name.LocalName switch
		{
			"Pollination" => HasShape(child, "Pollination", ["dependency", "healthBonus", "yieldBonus"], []) &&
				OptionalEnumAttribute<AgriculturePollinationDependency>(child, "dependency") &&
				IntegerAttributes(child, "healthBonus", "yieldBonus"),
			"PlantingWindows" => HasShape(child, "PlantingWindows", [], ["Window"], ["Window"]) &&
				child.Elements().All(window => HasShape(window, "Window", ["type", "value"], []) &&
					NamedAttribute(window, "value") && EnumValue<AgriculturePlantingWindowType>((string?)window.Attribute("type"))),
			"ScoreRanges" => HasShape(child, "ScoreRanges", [], ["Score"], ["Score"]) &&
				child.Elements().All(score => HasShape(score, "Score", ["type", "min", "max"], []) &&
					NamedAttribute(score, "type") && IntegerAttributes(score, "min", "max")),
			"Seeds" or "Outputs" => IsCommodities(child),
			_ => false
		});

	private static bool IsCommodities(XElement root) =>
		HasShape(root, root.Name.LocalName, [], ["Commodity"], ["Commodity"]) &&
		root.Elements().All(child => HasShape(child, "Commodity", ["material", "weight", "tag"], []) &&
			NamedAttribute(child, "material") && DoubleAttributes(child, "weight"));

	private static bool IsScoreValue(XElement score) =>
		HasShape(score, "Score", ["type", "value"], []) && NamedAttribute(score, "type") &&
		IntegerAttributes(score, "value");

	private static bool FieldUses(XElement element) =>
		((string?)element.Attribute("uses") ?? string.Empty)
		.Split([' ', ',', ';', '|'], StringSplitOptions.RemoveEmptyEntries)
		.All(EnumValue<AgricultureFieldUse>);

	private static bool HasShape(XElement element, string name, string[] attributes, string[] children,
		params string[] repeatedChildren) =>
		element.Name == XName.Get(name) &&
		element.Attributes().All(x => x.Name.Namespace == XNamespace.None && attributes.Contains(x.Name.LocalName)) &&
		element.Nodes().All(x => x is XElement || x is XComment || x is XText text && string.IsNullOrWhiteSpace(text.Value)) &&
		element.Elements().All(x => x.Name.Namespace == XNamespace.None && children.Contains(x.Name.LocalName)) &&
		element.Elements().GroupBy(x => x.Name).All(x => x.Count() == 1 || repeatedChildren.Contains(x.Key.LocalName));

	private static bool NamedAttribute(XElement element, string name) => !string.IsNullOrWhiteSpace((string?)element.Attribute(name));

	private static bool IntegerAttributes(XElement element, params string[] names) => names.All(name =>
		element.Attribute(name) is not { } attribute || int.TryParse(attribute.Value, NumberStyles.Integer,
			CultureInfo.InvariantCulture, out _));

	private static bool DoubleAttributes(XElement element, params string[] names) => names.All(name =>
		element.Attribute(name) is not { } attribute || double.TryParse(attribute.Value, NumberStyles.Float,
			CultureInfo.InvariantCulture, out var number) && double.IsFinite(number));

	private static bool BooleanAttribute(XElement element, string name) =>
		element.Attribute(name) is not { } attribute || attribute.Value.Trim() is "true" or "false" or "0" or "1";

	private static bool OptionalEnumAttribute<T>(XElement element, string name) where T : struct, Enum =>
		element.Attribute(name) is not { } attribute || EnumValue<T>(attribute.Value);

	private static bool EnumValue<T>(string? value) where T : struct, Enum =>
		Enum.TryParse<T>(value, true, out var result) && Enum.IsDefined(result);
}

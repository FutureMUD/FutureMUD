#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Work.Loot;

public enum LootChoiceKind
{
	Nothing = 0,
	Item = 1,
	Commodity = 2,
	LootTable = 3
}

public sealed class LootTableDefinition
{
	public const int CurrentAlgorithmVersion = 1;

	public int AlgorithmVersion { get; set; } = CurrentAlgorithmVersion;
	public List<LootVariantDefinition> Variants { get; } = [];

	public LootTableDefinition Clone() => Load(ToCanonicalXml());

	public string ToCanonicalXml()
	{
		var root = new XElement("LootTable", new XAttribute("algorithm", AlgorithmVersion));
		foreach (var variant in Variants.OrderBy(x => x.Key, StringComparer.Ordinal))
		{
			var variantElement = new XElement("Variant", new XAttribute("key", variant.Key));
			for (var groupIndex = 0; groupIndex < variant.Groups.Count; groupIndex++)
			{
				var group = variant.Groups[groupIndex];
				var groupElement = new XElement("Group",
					new XAttribute("order", groupIndex),
					new XAttribute("key", group.Key),
					new XAttribute("repeatMin", group.RepeatMinimum),
					new XAttribute("repeatMax", group.RepeatMaximum),
					new XAttribute("destination", group.DestinationKey));

				for (var choiceIndex = 0; choiceIndex < group.Choices.Count; choiceIndex++)
				{
					var choice = group.Choices[choiceIndex];
					var choiceElement = new XElement("Choice",
						new XAttribute("order", choiceIndex),
						new XAttribute("key", choice.Key),
						new XAttribute("weight", choice.Weight),
						new XAttribute("kind", choice.Kind.ToString()));

					switch (choice.Kind)
					{
						case LootChoiceKind.Item:
							choiceElement.Add(
								new XAttribute("prototype", choice.ItemPrototypeId),
								new XAttribute("revision", choice.ItemPrototypeRevision),
								new XAttribute("quantityMin", choice.QuantityMinimum),
								new XAttribute("quantityMax", choice.QuantityMaximum),
								new XAttribute("qualityMin", choice.QualityMinimum),
								new XAttribute("qualityMax", choice.QualityMaximum));
							if (choice.StartsClosed)
							{
								choiceElement.Add(new XAttribute("closed", true));
							}
							if (choice.StartsLocked)
							{
								choiceElement.Add(new XAttribute("locked", true));
							}
							if (!string.IsNullOrEmpty(choice.ResultKey))
							{
								choiceElement.Add(new XAttribute("resultKey", choice.ResultKey));
							}

							foreach (var characteristic in choice.Characteristics
							             .OrderBy(x => x.DefinitionId)
							             .ThenBy(x => x.ValueId))
							{
								choiceElement.Add(new XElement("Characteristic",
									new XAttribute("definition", characteristic.DefinitionId),
									new XAttribute("value", characteristic.ValueId)));
							}
							break;
						case LootChoiceKind.Commodity:
							choiceElement.Add(
								new XAttribute("material", choice.CommodityMaterialId),
								new XAttribute("massMin", FormatDouble(choice.MassMinimum)),
								new XAttribute("massMax", FormatDouble(choice.MassMaximum)));
							if (choice.CommodityTagId is not null)
							{
								choiceElement.Add(new XAttribute("tag", choice.CommodityTagId.Value));
							}
							break;
						case LootChoiceKind.LootTable:
							choiceElement.Add(
								new XAttribute("table", choice.NestedTableId),
								new XAttribute("revision", choice.NestedTableRevision),
								new XAttribute("variant", choice.NestedVariant));
							break;
					}

					groupElement.Add(choiceElement);
				}

				variantElement.Add(groupElement);
			}

			root.Add(variantElement);
		}

		return root.ToString(SaveOptions.DisableFormatting);
	}

	public string ComputeHash()
	{
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(ToCanonicalXml())));
	}

	public static LootTableDefinition Load(string xml)
	{
		var root = XElement.Parse(xml, LoadOptions.None);
		if (!root.Name.LocalName.Equals("LootTable", StringComparison.Ordinal))
		{
			throw new FormatException("LootTable definition root is invalid.");
		}

		var definition = new LootTableDefinition
		{
			AlgorithmVersion = RequiredInt(root, "algorithm")
		};

		foreach (var variantElement in root.Elements("Variant")
		                                  .OrderBy(x => RequiredString(x, "key"), StringComparer.Ordinal))
		{
			var variant = new LootVariantDefinition { Key = RequiredString(variantElement, "key") };
			foreach (var groupElement in variantElement.Elements("Group")
			                                            .OrderBy(x => RequiredInt(x, "order"))
			                                            .ThenBy(x => RequiredString(x, "key"), StringComparer.Ordinal))
			{
				var group = new LootRollGroupDefinition
				{
					Key = RequiredString(groupElement, "key"),
					RepeatMinimum = RequiredInt(groupElement, "repeatMin"),
					RepeatMaximum = RequiredInt(groupElement, "repeatMax"),
					DestinationKey = RequiredString(groupElement, "destination")
				};

				foreach (var choiceElement in groupElement.Elements("Choice")
				                                           .OrderBy(x => RequiredInt(x, "order"))
				                                           .ThenBy(x => RequiredString(x, "key"), StringComparer.Ordinal))
				{
					if (!Enum.TryParse<LootChoiceKind>(RequiredString(choiceElement, "kind"), out var kind))
					{
						throw new FormatException("LootTable choice kind is invalid.");
					}

					var choice = new LootChoiceDefinition
					{
						Key = RequiredString(choiceElement, "key"),
						Weight = RequiredLong(choiceElement, "weight"),
						Kind = kind
					};

					switch (kind)
					{
						case LootChoiceKind.Item:
							choice.ItemPrototypeId = RequiredLong(choiceElement, "prototype");
							choice.ItemPrototypeRevision = RequiredInt(choiceElement, "revision");
							choice.QuantityMinimum = RequiredInt(choiceElement, "quantityMin");
							choice.QuantityMaximum = RequiredInt(choiceElement, "quantityMax");
							choice.QualityMinimum = RequiredInt(choiceElement, "qualityMin");
							choice.QualityMaximum = RequiredInt(choiceElement, "qualityMax");
							choice.StartsClosed = OptionalBool(choiceElement, "closed");
							choice.StartsLocked = OptionalBool(choiceElement, "locked");
							choice.ResultKey = (string?)choiceElement.Attribute("resultKey");
							foreach (var characteristic in choiceElement.Elements("Characteristic"))
							{
								choice.Characteristics.Add(new LootCharacteristicValue
								{
									DefinitionId = RequiredLong(characteristic, "definition"),
									ValueId = RequiredLong(characteristic, "value")
								});
							}
							break;
						case LootChoiceKind.Commodity:
							choice.CommodityMaterialId = RequiredLong(choiceElement, "material");
							choice.CommodityTagId = OptionalLong(choiceElement, "tag");
							choice.MassMinimum = RequiredDouble(choiceElement, "massMin");
							choice.MassMaximum = RequiredDouble(choiceElement, "massMax");
							break;
						case LootChoiceKind.LootTable:
							choice.NestedTableId = RequiredLong(choiceElement, "table");
							choice.NestedTableRevision = RequiredInt(choiceElement, "revision");
							choice.NestedVariant = RequiredString(choiceElement, "variant");
							break;
					}

					group.Choices.Add(choice);
				}

				variant.Groups.Add(group);
			}

			definition.Variants.Add(variant);
		}

		return definition;
	}

	private static string FormatDouble(double value) => value.ToString("R", CultureInfo.InvariantCulture);
	private static string RequiredString(XElement element, string name) =>
		(string?)element.Attribute(name) ?? throw new FormatException($"Missing LootTable attribute {name}.");
	private static int RequiredInt(XElement element, string name) =>
		int.Parse(RequiredString(element, name), NumberStyles.Integer, CultureInfo.InvariantCulture);
	private static long RequiredLong(XElement element, string name) =>
		long.Parse(RequiredString(element, name), NumberStyles.Integer, CultureInfo.InvariantCulture);
	private static long? OptionalLong(XElement element, string name) =>
		long.TryParse((string?)element.Attribute(name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
			? value
			: null;
	private static bool OptionalBool(XElement element, string name) =>
		bool.TryParse((string?)element.Attribute(name), out var value) && value;
	private static double RequiredDouble(XElement element, string name) =>
		double.Parse(RequiredString(element, name), NumberStyles.Float, CultureInfo.InvariantCulture);
}

public sealed class LootVariantDefinition
{
	public string Key { get; set; } = "default";
	public List<LootRollGroupDefinition> Groups { get; } = [];
}

public sealed class LootRollGroupDefinition
{
	public string Key { get; set; } = string.Empty;
	public int RepeatMinimum { get; set; } = 1;
	public int RepeatMaximum { get; set; } = 1;
	public string DestinationKey { get; set; } = "target";
	public List<LootChoiceDefinition> Choices { get; } = [];
}

public sealed class LootChoiceDefinition
{
	public string Key { get; set; } = string.Empty;
	public long Weight { get; set; } = 1;
	public LootChoiceKind Kind { get; set; }

	public long ItemPrototypeId { get; set; }
	public int ItemPrototypeRevision { get; set; }
	public int QuantityMinimum { get; set; } = 1;
	public int QuantityMaximum { get; set; } = 1;
	public int QualityMinimum { get; set; } = 5;
	public int QualityMaximum { get; set; } = 5;
	public bool StartsClosed { get; set; }
	public bool StartsLocked { get; set; }
	public string? ResultKey { get; set; }
	public List<LootCharacteristicValue> Characteristics { get; } = [];

	public long CommodityMaterialId { get; set; }
	public long? CommodityTagId { get; set; }
	public double MassMinimum { get; set; }
	public double MassMaximum { get; set; }

	public long NestedTableId { get; set; }
	public int NestedTableRevision { get; set; }
	public string NestedVariant { get; set; } = "default";
}

public sealed class LootCharacteristicValue
{
	public long DefinitionId { get; set; }
	public long ValueId { get; set; }
}

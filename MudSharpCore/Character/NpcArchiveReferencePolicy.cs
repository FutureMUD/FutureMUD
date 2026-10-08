#nullable enable

using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MudSharp.Character;

/// <summary>Unclassified serialized references hold compaction; ownership is not reference proof.</summary>
public static partial class NpcArchiveReferencePolicy
{
	public static bool HasReferenceOrUncertainty(string? value, long characterId, long bodyId,
		params long[] additionalPhysicalIds)
	{
		if (string.IsNullOrWhiteSpace(value)) return false;
		if (value.TrimStart().StartsWith('{') || value.TrimStart().StartsWith('['))
		{
			try
			{
				using var json = JsonDocument.Parse(value);
				return HasJsonReference(json.RootElement, characterId, bodyId, additionalPhysicalIds);
			}
			catch (JsonException) { return true; }
		}
		if (value.TrimStart().StartsWith('<'))
		{
			try
			{
				var xml = XElement.Parse(value);
				return xml.DescendantsAndSelf().Any(x => !x.HasElements && Matches(x.Value, characterId, bodyId, additionalPhysicalIds) ||
					x.Attributes().Any(a => Matches(a.Value, characterId, bodyId, additionalPhysicalIds)));
			}
			catch (XmlException) { return true; }
		}
		return Matches(value, characterId, bodyId, additionalPhysicalIds);
	}

	private static bool HasJsonReference(JsonElement value, long characterId, long bodyId, long[] additionalPhysicalIds) =>
		value.ValueKind switch
		{
			JsonValueKind.Object => value.EnumerateObject().Any(x => Matches(x.Name, characterId, bodyId, additionalPhysicalIds) ||
				HasJsonReference(x.Value, characterId, bodyId, additionalPhysicalIds)),
			JsonValueKind.Array => value.EnumerateArray().Any(x => HasJsonReference(x, characterId, bodyId, additionalPhysicalIds)),
			JsonValueKind.String => Matches(value.GetString()!, characterId, bodyId, additionalPhysicalIds),
			JsonValueKind.Number => value.TryGetDecimal(out var number) &&
				new[] { characterId, bodyId }.Concat(additionalPhysicalIds).Any(x => x > 0 && number == x),
			_ => false
		};

	private static bool Matches(string value, long characterId, long bodyId, IEnumerable<long> additionalPhysicalIds) =>
		new[] { characterId, bodyId }.Concat(additionalPhysicalIds).Any(id => id > 0 && Regex.IsMatch(value,
			$@"(?<![\d.]){id.ToString(CultureInfo.InvariantCulture)}(?![\d.])", RegexOptions.CultureInvariant));

	public static bool IsEmptyEffects(string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) return true;
		try
		{
			var xml = XElement.Parse(value);
			return xml.Name.LocalName == "Effects" && !xml.HasElements &&
				!xml.HasAttributes && string.IsNullOrWhiteSpace(xml.Value);
		}
		catch (XmlException) { return false; }
	}
}

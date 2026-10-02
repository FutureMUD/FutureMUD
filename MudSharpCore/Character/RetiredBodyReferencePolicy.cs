#nullable enable

using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace MudSharp.Character;

/// <summary>Conservative guard for persisted body references outside relational foreign keys.</summary>
public static class RetiredBodyReferencePolicy
{
	public static bool HasReferenceOrUncertainty(string? definition, long bodyId)
	{
		if (string.IsNullOrWhiteSpace(definition)) return false;
		try
		{
			var xml = XElement.Parse(definition);
			return xml.DescendantsAndSelf().Any(x => IsReference(x.Name.LocalName, x.Value, bodyId) ||
				x.Attributes().Any(a => IsReference(a.Name.LocalName, a.Value, bodyId)));
		}
		catch (XmlException)
		{
			return true;
		}
	}

	private static bool IsReference(string name, string value, long bodyId) =>
		(name.EndsWith("body", StringComparison.OrdinalIgnoreCase) ||
		 name.EndsWith("bodyid", StringComparison.OrdinalIgnoreCase) ||
		 name.EndsWith("body_id", StringComparison.OrdinalIgnoreCase)) &&
		long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) && id == bodyId;
}

#nullable enable

using System.Xml;
using System.Xml.Linq;

namespace MudSharp.Character;

/// <summary>Structural proof that the owned graph has no retained effects.</summary>
public static class NpcArchiveReferencePolicy
{
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

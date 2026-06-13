using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System;
using System.Globalization;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Character;

public sealed record AstralProjectionInstanceMetadata(
	long AnchorCharacterId,
	long AnchorInstanceId,
	long ProjectionBodyId,
	long PlaneId,
	AstralProjectionAnchorPolicy AnchorPolicy,
	long SourceSpellId,
	string FormKey
);

public static class CharacterInstanceMetadata
{
	public static string CreateAstralProjectionEffectData(
		long anchorCharacterId,
		long anchorInstanceId,
		long projectionBodyId,
		long planeId,
		AstralProjectionAnchorPolicy anchorPolicy,
		long sourceSpellId,
		string formKey)
	{
		return new XElement(
			"Effects",
			new XElement(
				"AstralProjection",
				new XAttribute("AnchorCharacterId", anchorCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorInstanceId", anchorInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("ProjectionBodyId", projectionBodyId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("PlaneId", planeId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorPolicy", anchorPolicy.ToString()),
				new XAttribute("SourceSpellId", sourceSpellId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("FormKey", formKey)
			)
		).ToString(SaveOptions.DisableFormatting);
	}

	public static bool TryGetAstralProjectionMetadata(
		string? effectData,
		out AstralProjectionInstanceMetadata metadata)
	{
		metadata = new AstralProjectionInstanceMetadata(0, 0, 0, 0, AstralProjectionAnchorPolicy.Helpless, 0,
			string.Empty);
		if (string.IsNullOrWhiteSpace(effectData))
		{
			return false;
		}

		XElement root;
		try
		{
			root = XElement.Parse(effectData!);
		}
		catch (Exception)
		{
			return false;
		}

		var element = root.Name == "AstralProjection"
			? root
			: root.Element("AstralProjection");
		if (element is null)
		{
			return false;
		}

		if (!TryGetLong(element, "AnchorCharacterId", out var anchorCharacterId) ||
		    !TryGetLong(element, "AnchorInstanceId", out var anchorInstanceId) ||
		    !TryGetLong(element, "ProjectionBodyId", out var projectionBodyId) ||
		    !TryGetLong(element, "PlaneId", out var planeId) ||
		    !TryGetLong(element, "SourceSpellId", out var sourceSpellId))
		{
			return false;
		}

		if (!Enum.TryParse<AstralProjectionAnchorPolicy>(
			    (string?)element.Attribute("AnchorPolicy") ?? AstralProjectionAnchorPolicy.Helpless.ToString(),
			    true,
			    out var anchorPolicy))
		{
			anchorPolicy = AstralProjectionAnchorPolicy.Helpless;
		}

		metadata = new AstralProjectionInstanceMetadata(
			anchorCharacterId,
			anchorInstanceId,
			projectionBodyId,
			planeId,
			anchorPolicy,
			sourceSpellId,
			(string?)element.Attribute("FormKey") ?? string.Empty
		);
		return true;
	}

	private static bool TryGetLong(XElement element, string attribute, out long value)
	{
		return long.TryParse(
			(string?)element.Attribute(attribute),
			NumberStyles.Integer,
			CultureInfo.InvariantCulture,
			out value);
	}
}

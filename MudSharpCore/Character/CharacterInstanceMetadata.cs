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

public sealed record MagicalCopyInstanceMetadata(
	long AnchorCharacterId,
	long AnchorInstanceId,
	long CopyBodyId,
	long PlaneId,
	long SourceSpellId,
	string FormKey,
	bool PlayerFocusable,
	bool Intangible,
	CharacterInstancePersistencePolicy PersistencePolicy
);

public sealed record PhysicalCloneInstanceMetadata(
	long AnchorCharacterId,
	long AnchorInstanceId,
	long CloneBodyId,
	long SourceSpellId,
	string FormKey,
	bool PlayerFocusable,
	CharacterInstancePersistencePolicy PersistencePolicy
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

	public static string CreateMagicalCopyEffectData(
		long anchorCharacterId,
		long anchorInstanceId,
		long copyBodyId,
		long planeId,
		long sourceSpellId,
		string formKey,
		bool playerFocusable,
		bool intangible,
		CharacterInstancePersistencePolicy persistencePolicy)
	{
		return new XElement(
			"Effects",
			new XElement(
				"MagicalCopy",
				new XAttribute("AnchorCharacterId", anchorCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorInstanceId", anchorInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("CopyBodyId", copyBodyId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("PlaneId", planeId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("SourceSpellId", sourceSpellId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("FormKey", formKey),
				new XAttribute("PlayerFocusable", playerFocusable),
				new XAttribute("Intangible", intangible),
				new XAttribute("PersistencePolicy", persistencePolicy.ToString())
			)
		).ToString(SaveOptions.DisableFormatting);
	}

	public static string CreatePhysicalCloneEffectData(
		long anchorCharacterId,
		long anchorInstanceId,
		long cloneBodyId,
		long sourceSpellId,
		string formKey,
		bool playerFocusable,
		CharacterInstancePersistencePolicy persistencePolicy)
	{
		return new XElement(
			"Effects",
			new XElement(
				"PhysicalClone",
				new XAttribute("AnchorCharacterId", anchorCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorInstanceId", anchorInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("CloneBodyId", cloneBodyId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("SourceSpellId", sourceSpellId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("FormKey", formKey),
				new XAttribute("PlayerFocusable", playerFocusable),
				new XAttribute("PersistencePolicy", persistencePolicy.ToString())
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

	public static bool TryGetMagicalCopyMetadata(
		string? effectData,
		out MagicalCopyInstanceMetadata metadata)
	{
		metadata = new MagicalCopyInstanceMetadata(0, 0, 0, 0, 0, string.Empty, false, false,
			CharacterInstancePersistencePolicy.DespawnOnReboot);
		var element = GetMetadataElement(effectData, "MagicalCopy");
		if (element is null)
		{
			return false;
		}

		if (!TryGetLong(element, "AnchorCharacterId", out var anchorCharacterId) ||
		    !TryGetLong(element, "AnchorInstanceId", out var anchorInstanceId) ||
		    !TryGetLong(element, "CopyBodyId", out var copyBodyId) ||
		    !TryGetLong(element, "PlaneId", out var planeId) ||
		    !TryGetLong(element, "SourceSpellId", out var sourceSpellId))
		{
			return false;
		}

		if (!Enum.TryParse<CharacterInstancePersistencePolicy>(
			    (string?)element.Attribute("PersistencePolicy") ??
			    CharacterInstancePersistencePolicy.DespawnOnReboot.ToString(),
			    true,
			    out var persistencePolicy))
		{
			persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot;
		}

		metadata = new MagicalCopyInstanceMetadata(
			anchorCharacterId,
			anchorInstanceId,
			copyBodyId,
			planeId,
			sourceSpellId,
			(string?)element.Attribute("FormKey") ?? string.Empty,
			TryGetBool(element, "PlayerFocusable"),
			TryGetBool(element, "Intangible"),
			persistencePolicy
		);
		return true;
	}

	public static bool TryGetPhysicalCloneMetadata(
		string? effectData,
		out PhysicalCloneInstanceMetadata metadata)
	{
		metadata = new PhysicalCloneInstanceMetadata(0, 0, 0, 0, string.Empty, false,
			CharacterInstancePersistencePolicy.DespawnOnReboot);
		var element = GetMetadataElement(effectData, "PhysicalClone");
		if (element is null)
		{
			return false;
		}

		if (!TryGetLong(element, "AnchorCharacterId", out var anchorCharacterId) ||
		    !TryGetLong(element, "AnchorInstanceId", out var anchorInstanceId) ||
		    !TryGetLong(element, "CloneBodyId", out var cloneBodyId) ||
		    !TryGetLong(element, "SourceSpellId", out var sourceSpellId))
		{
			return false;
		}

		if (!Enum.TryParse<CharacterInstancePersistencePolicy>(
			    (string?)element.Attribute("PersistencePolicy") ??
			    CharacterInstancePersistencePolicy.DespawnOnReboot.ToString(),
			    true,
			    out var persistencePolicy))
		{
			persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot;
		}

		metadata = new PhysicalCloneInstanceMetadata(
			anchorCharacterId,
			anchorInstanceId,
			cloneBodyId,
			sourceSpellId,
			(string?)element.Attribute("FormKey") ?? string.Empty,
			TryGetBool(element, "PlayerFocusable"),
			persistencePolicy
		);
		return true;
	}

	private static XElement? GetMetadataElement(string? effectData, string elementName)
	{
		if (string.IsNullOrWhiteSpace(effectData))
		{
			return null;
		}

		XElement root;
		try
		{
			root = XElement.Parse(effectData!);
		}
		catch (Exception)
		{
			return null;
		}

		return root.Name == elementName
			? root
			: root.Element(elementName);
	}

	private static bool TryGetBool(XElement element, string attribute)
	{
		return bool.TryParse((string?)element.Attribute(attribute), out var value) && value;
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

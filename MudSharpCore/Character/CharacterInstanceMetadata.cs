using System.Globalization;

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

public sealed record PossessedBodyInstanceMetadata(
	long AnchorCharacterId,
	long AnchorInstanceId,
	long ShellBodyId,
	long SourceTargetCharacterId,
	long SourceTargetInstanceId,
	long SourceSpellId,
	string FormKey,
	CharacterInstancePersistencePolicy PersistencePolicy
);

public sealed record PossessedCorpseInstanceMetadata(
	long AnchorCharacterId,
	long AnchorInstanceId,
	long CorpseItemId,
	long OriginalCharacterId,
	long OriginalBodyId,
	long SourceSpellId,
	CharacterInstancePersistencePolicy PersistencePolicy
);

public sealed record AnimatedCorpseInstanceMetadata(
	long AnchorCharacterId,
	long AnchorInstanceId,
	long CorpseItemId,
	long OriginalCharacterId,
	long OriginalBodyId,
	long SourceSpellId,
	IReadOnlyList<long> ArtificialIntelligenceIds,
	CharacterInstancePersistencePolicy PersistencePolicy
);

public sealed record ScriptedAiInstanceMetadata(
	long AnchorCharacterId,
	long AnchorInstanceId,
	long BodyId,
	string FormKey,
	IReadOnlyList<long> ArtificialIntelligenceIds,
	bool CloneInventory
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

	public static string CreatePossessedBodyEffectData(
		long anchorCharacterId,
		long anchorInstanceId,
		long shellBodyId,
		long sourceTargetCharacterId,
		long sourceTargetInstanceId,
		long sourceSpellId,
		string formKey,
		CharacterInstancePersistencePolicy persistencePolicy)
	{
		return new XElement(
			"Effects",
			new XElement(
				"PossessedBody",
				new XAttribute("AnchorCharacterId", anchorCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorInstanceId", anchorInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("ShellBodyId", shellBodyId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("SourceTargetCharacterId",
					sourceTargetCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("SourceTargetInstanceId",
					sourceTargetInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("SourceSpellId", sourceSpellId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("FormKey", formKey),
				new XAttribute("PersistencePolicy", persistencePolicy.ToString())
			)
		).ToString(SaveOptions.DisableFormatting);
	}

	public static string CreatePossessedCorpseEffectData(
		long anchorCharacterId,
		long anchorInstanceId,
		long corpseItemId,
		long originalCharacterId,
		long originalBodyId,
		long sourceSpellId,
		CharacterInstancePersistencePolicy persistencePolicy)
	{
		return new XElement(
			"Effects",
			new XElement(
				"PossessedCorpse",
				new XAttribute("AnchorCharacterId", anchorCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorInstanceId", anchorInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("CorpseItemId", corpseItemId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("OriginalCharacterId", originalCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("OriginalBodyId", originalBodyId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("SourceSpellId", sourceSpellId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("PersistencePolicy", persistencePolicy.ToString())
			)
		).ToString(SaveOptions.DisableFormatting);
	}

	public static string CreateAnimatedCorpseEffectData(
		long anchorCharacterId,
		long anchorInstanceId,
		long corpseItemId,
		long originalCharacterId,
		long originalBodyId,
		long sourceSpellId,
		IEnumerable<long> artificialIntelligenceIds,
		CharacterInstancePersistencePolicy persistencePolicy)
	{
		return new XElement(
			"Effects",
			new XElement(
				"AnimatedCorpse",
				new XAttribute("AnchorCharacterId", anchorCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorInstanceId", anchorInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("CorpseItemId", corpseItemId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("OriginalCharacterId", originalCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("OriginalBodyId", originalBodyId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("SourceSpellId", sourceSpellId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("PersistencePolicy", persistencePolicy.ToString()),
				new XElement(
					"ArtificialIntelligences",
					artificialIntelligenceIds
						.Distinct()
						.Select(x => new XElement("AI",
							new XAttribute("Id", x.ToString(CultureInfo.InvariantCulture)))))
			)
		).ToString(SaveOptions.DisableFormatting);
	}

	public static string CreateScriptedAiEffectData(
		long anchorCharacterId,
		long anchorInstanceId,
		long bodyId,
		string formKey,
		IEnumerable<long> artificialIntelligenceIds,
		bool cloneInventory)
	{
		return new XElement(
			"Effects",
			new XElement(
				"ScriptedAi",
				new XAttribute("AnchorCharacterId", anchorCharacterId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("AnchorInstanceId", anchorInstanceId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("BodyId", bodyId.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("FormKey", formKey),
				new XAttribute("CloneInventory", cloneInventory),
				new XElement(
					"ArtificialIntelligences",
					artificialIntelligenceIds
						.Distinct()
						.Select(x => new XElement("AI",
							new XAttribute("Id", x.ToString(CultureInfo.InvariantCulture)))))
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

	public static bool TryGetPossessedBodyMetadata(
		string? effectData,
		out PossessedBodyInstanceMetadata metadata)
	{
		metadata = new PossessedBodyInstanceMetadata(0, 0, 0, 0, 0, 0, string.Empty,
			CharacterInstancePersistencePolicy.DespawnOnReboot);
		var element = GetMetadataElement(effectData, "PossessedBody");
		if (element is null)
		{
			return false;
		}

		if (!TryGetLong(element, "AnchorCharacterId", out var anchorCharacterId) ||
		    !TryGetLong(element, "AnchorInstanceId", out var anchorInstanceId) ||
		    !TryGetLong(element, "ShellBodyId", out var shellBodyId) ||
		    !TryGetLong(element, "SourceTargetCharacterId", out var sourceTargetCharacterId) ||
		    !TryGetLong(element, "SourceTargetInstanceId", out var sourceTargetInstanceId) ||
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

		metadata = new PossessedBodyInstanceMetadata(
			anchorCharacterId,
			anchorInstanceId,
			shellBodyId,
			sourceTargetCharacterId,
			sourceTargetInstanceId,
			sourceSpellId,
			(string?)element.Attribute("FormKey") ?? string.Empty,
			persistencePolicy
		);
		return true;
	}

	public static bool TryGetPossessedCorpseMetadata(
		string? effectData,
		out PossessedCorpseInstanceMetadata metadata)
	{
		metadata = new PossessedCorpseInstanceMetadata(0, 0, 0, 0, 0, 0,
			CharacterInstancePersistencePolicy.DespawnOnReboot);
		var element = GetMetadataElement(effectData, "PossessedCorpse");
		if (element is null)
		{
			return false;
		}

		if (!TryGetLong(element, "AnchorCharacterId", out var anchorCharacterId) ||
		    !TryGetLong(element, "AnchorInstanceId", out var anchorInstanceId) ||
		    !TryGetLong(element, "CorpseItemId", out var corpseItemId) ||
		    !TryGetLong(element, "OriginalCharacterId", out var originalCharacterId) ||
		    !TryGetLong(element, "OriginalBodyId", out var originalBodyId) ||
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

		metadata = new PossessedCorpseInstanceMetadata(
			anchorCharacterId,
			anchorInstanceId,
			corpseItemId,
			originalCharacterId,
			originalBodyId,
			sourceSpellId,
			persistencePolicy
		);
		return true;
	}

	public static bool TryGetAnimatedCorpseMetadata(
		string? effectData,
		out AnimatedCorpseInstanceMetadata metadata)
	{
		metadata = new AnimatedCorpseInstanceMetadata(0, 0, 0, 0, 0, 0, Array.Empty<long>(),
			CharacterInstancePersistencePolicy.DespawnOnReboot);
		var element = GetMetadataElement(effectData, "AnimatedCorpse");
		if (element is null)
		{
			return false;
		}

		if (!TryGetLong(element, "AnchorCharacterId", out var anchorCharacterId) ||
		    !TryGetLong(element, "AnchorInstanceId", out var anchorInstanceId) ||
		    !TryGetLong(element, "CorpseItemId", out var corpseItemId) ||
		    !TryGetLong(element, "OriginalCharacterId", out var originalCharacterId) ||
		    !TryGetLong(element, "OriginalBodyId", out var originalBodyId) ||
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

		metadata = new AnimatedCorpseInstanceMetadata(
			anchorCharacterId,
			anchorInstanceId,
			corpseItemId,
			originalCharacterId,
			originalBodyId,
			sourceSpellId,
			GetArtificialIntelligenceIds(element),
			persistencePolicy
		);
		return true;
	}

	public static bool TryGetScriptedAiMetadata(
		string? effectData,
		out ScriptedAiInstanceMetadata metadata)
	{
		metadata = new ScriptedAiInstanceMetadata(0, 0, 0, string.Empty, Array.Empty<long>(), false);
		var element = GetMetadataElement(effectData, "ScriptedAi");
		if (element is null)
		{
			return false;
		}

		if (!TryGetLong(element, "AnchorCharacterId", out var anchorCharacterId) ||
		    !TryGetLong(element, "AnchorInstanceId", out var anchorInstanceId) ||
		    !TryGetLong(element, "BodyId", out var bodyId))
		{
			return false;
		}

		metadata = new ScriptedAiInstanceMetadata(
			anchorCharacterId,
			anchorInstanceId,
			bodyId,
			(string?)element.Attribute("FormKey") ?? string.Empty,
			GetArtificialIntelligenceIds(element),
			TryGetBool(element, "CloneInventory")
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

	private static IReadOnlyList<long> GetArtificialIntelligenceIds(XElement element)
	{
		var aiIds = new List<long>();
		foreach (var aiElement in element.Element("ArtificialIntelligences")?.Elements("AI") ??
		                          Enumerable.Empty<XElement>())
		{
			if (long.TryParse(
				    (string?)aiElement.Attribute("Id"),
				    NumberStyles.Integer,
				    CultureInfo.InvariantCulture,
				    out var value) &&
			    !aiIds.Contains(value))
			{
				aiIds.Add(value);
			}
		}

		return aiIds;
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

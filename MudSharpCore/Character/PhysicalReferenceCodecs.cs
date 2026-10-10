#nullable enable

using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Character;

/// <summary>Pure readers for identity fields consumed by concrete runtime loaders. No text-ID fallback.</summary>
public static class PhysicalReferenceCodecs
{
	public static IEnumerable<PhysicalEntityReference> Effects(string? value, bool instanceMetadata = false, PhysicalReferenceTargets? targets = null)
	{
		if (string.IsNullOrWhiteSpace(value)) yield break;
		var root = XElement.Parse(value);
		if (root.Name != "Effects" && !instanceMetadata)
			throw new FormatException("Known EffectData must have an Effects container.");
		foreach (var effect in root.Elements("Effect"))
			foreach (var reference in Effect(effect, targets)) yield return reference;
		if (instanceMetadata)
			foreach (var reference in InstanceMetadata(root, targets)) yield return reference;
	}

	private static IEnumerable<PhysicalEntityReference> Effect(XElement envelope, PhysicalReferenceTargets? targets)
	{
		bool Needed(PhysicalEntityKind kind) => targets is null || targets.HasKind(kind);
		var type = envelope.Element("Type")?.Value ?? throw new FormatException("Effects/Effect/Type is missing.");
		var root = envelope.Element("Effect");
		var fields = type switch
		{
			"HasLegalCounsel" => new[] { "Lawyer" },
			"InCustodyOfEnforcer" => ["Enforcer"],
			"Lawyering" => ["EngagedBy"],
			"OnTrial" => ["Prosecutor", "Defender"],
			"MagicClairaudienceConcentration" => ["Target"],
			"PsionicTrace" => ["SourceCharacterId", "TargetCharacterId"],
			"WitnessedClanMemberDeath" => ["Member"],
			"Trap" or "TrapPayloadSchedule" => ["CreatorId", "TargetCharacterId"],
			"SpellIdentify" => ["CasterId"],
			"SpellNpcGuardian" => ["CreatorId"],
			"SpellReciteProxy" => ["CasterId", "LinkedCharacterId"],
			"SpellLiveBodyPossession" => ["AnchorCharacterId", "TargetCharacterId"],
			"SpellPossessedBody" => ["AnchorCharacterId", "SourceTargetCharacterId"],
			"SpellCorpsePossession" or "SpellAnimatedCorpse" => ["AnchorCharacterId", "OriginalCharacterId"],
			"SpellDeadSpeak" => ["AnchorCharacterId", "OriginalCharacterId", "LinkedCharacterId"],
			"MonsterState" => ["Provoker", "WarningTarget"],
			"SpellPhantomIllusion" or "SpellSubjectiveDescription" => ["CasterId"],
			_ => Array.Empty<string>()
		};
		foreach (var field in Needed(PhysicalEntityKind.Character) ? fields : [])
		{
			if (root?.Element(field) is { } element)
				yield return Read(PhysicalEntityKind.Character, element.Value, $"{type}/{field}");
			else if (type is not ("OnTrial" or "Trap" or "TrapPayloadSchedule" or "SpellPhantomIllusion" or "SpellSubjectiveDescription" or "SpellPossessedBody" or "SpellCorpsePossession" or "SpellAnimatedCorpse" or "SpellDeadSpeak" or "SpellReciteProxy" or "MonsterState") &&
				!(type == "PsionicTrace" && field == "TargetCharacterId"))
				throw new FormatException($"{type}/{field} is missing.");
		}
		if (Needed(PhysicalEntityKind.Character) && type == "SpellSubjectiveDescription" && root?.Element("CasterId") is null &&
			root?.Element("FixedPerceiver") is { } fixedPerceiver)
			yield return Read(PhysicalEntityKind.Character, fixedPerceiver.Value, "SpellSubjectiveDescription/FixedPerceiver");
		if (type is "MagicSpellParent" or "SubstanceExposure")
		{
			if (root is null) throw new FormatException($"{type}/Effect is missing.");
			if (Needed(PhysicalEntityKind.Character))
				yield return Read(PhysicalEntityKind.Character, Required(root, "Caster"), $"{type}/Caster");
			if (Needed(PhysicalEntityKind.CharacterInstance) && root.Element("CasterInstance") is { } instance)
				yield return Read(PhysicalEntityKind.CharacterInstance, instance.Value, $"{type}/CasterInstance");
			foreach (var child in root.Element("Children")?.Elements("Effect") ?? [])
				foreach (var reference in Effect(child, targets)) yield return reference;
		}
		if (Needed(PhysicalEntityKind.Character) && type is "AnimalHunt" or "MonsterIntent")
			yield return Read(PhysicalEntityKind.Character, Required(root, "Target"), $"{type}/Target");
		if (Needed(PhysicalEntityKind.Character) && type is "DelayedPsychicSuggestion" or "NpcBurrowFood")
		{
			var field = type == "DelayedPsychicSuggestion" ? "source" : "PendingVictimId";
			yield return Read(PhysicalEntityKind.Character, root?.Attribute(field)?.Value ?? "0", $"{type}/@{field}");
		}
		var bodyFields = type switch
		{
			"BodyBackup" or "SpellBodyBackup" => new[] { "BackupBodyId" },
			"SubstanceExposure" => ["SourceBody"],
			"ForcedTransformationBaseline" => ["BaselineBodyId"],
			"SpellTransformForm" => ["FormBodyId", "PriorBodyId"],
			"SpellLiveBodyPossession" => ["TargetBodyId"],
			"SpellPossessedBody" => ["ShellBodyId"],
			"SpellCorpsePossession" or "SpellAnimatedCorpse" or "SpellDeadSpeak" => ["OriginalBodyId"],
			_ => Array.Empty<string>()
		};
		foreach (var field in Needed(PhysicalEntityKind.Body) ? bodyFields : [])
			yield return Read(PhysicalEntityKind.Body, root?.Element(field)?.Value ?? "0", $"{type}/{field}");
		var instanceFields = type switch
		{
			"SpellIdentify" => new[] { "CasterInstanceId" },
			"SpellNpcGuardian" => ["CreatorInstanceId"],
			"SpellReciteProxy" => ["CasterInstanceId", "LinkedInstanceId"],
			"SpellLiveBodyPossession" => ["AnchorInstanceId", "TargetInstanceId"],
			"SpellPossessedBody" => ["AnchorInstanceId", "ShellInstanceId", "SourceTargetInstanceId"],
			"SpellCorpsePossession" or "SpellAnimatedCorpse" => ["AnchorInstanceId", "AnimatedInstanceId"],
			"SpellDeadSpeak" => ["AnchorInstanceId", "AnimatedInstanceId", "LinkedInstanceId"],
			_ => Array.Empty<string>()
		};
		foreach (var field in Needed(PhysicalEntityKind.CharacterInstance) ? instanceFields : [])
			if (root?.Element(field) is { } element)
				yield return Read(PhysicalEntityKind.CharacterInstance, element.Value, $"{type}/{field}");
		if (type is "ZeroGravityTether" or "SpellZeroGravityTether")
			foreach (var reference in Typed(Required(root, "AnchorType"), Required(root, "AnchorId"), $"{type}/Anchor"))
				yield return reference;
	}

	private static IEnumerable<PhysicalEntityReference> InstanceMetadata(XElement root, PhysicalReferenceTargets? targets)
	{
		bool Needed(PhysicalEntityKind kind) => targets is null || targets.HasKind(kind);
		if (root.Element("OwnedProjection") is { } owned)
		{
			if (Needed(PhysicalEntityKind.Character)) yield return Read(PhysicalEntityKind.Character, RequiredAttribute(owned, "AnchorCharacterId"), "OwnedProjection/@AnchorCharacterId");
			if (Needed(PhysicalEntityKind.CharacterInstance)) yield return Read(PhysicalEntityKind.CharacterInstance, RequiredAttribute(owned, "AnchorInstanceId"), "OwnedProjection/@AnchorInstanceId");
		}
		foreach (var name in new[] { "AstralProjection", "MagicalCopy", "PhysicalClone", "PossessedBody", "PossessedCorpse", "AnimatedCorpse", "ScriptedAi" })
		{
			var node = root.Name == name ? root : root.Element(name);
			if (node is null) continue;
			if (Needed(PhysicalEntityKind.Character))
				yield return Read(PhysicalEntityKind.Character, RequiredAttribute(node, "AnchorCharacterId"), $"{name}/@AnchorCharacterId");
			if (Needed(PhysicalEntityKind.CharacterInstance))
				yield return Read(PhysicalEntityKind.CharacterInstance, RequiredAttribute(node, "AnchorInstanceId"), $"{name}/@AnchorInstanceId");
			if (name == "PossessedBody")
			{
				if (Needed(PhysicalEntityKind.Character))
					yield return Read(PhysicalEntityKind.Character, RequiredAttribute(node, "SourceTargetCharacterId"), "PossessedBody/@SourceTargetCharacterId");
				if (Needed(PhysicalEntityKind.CharacterInstance))
					yield return Read(PhysicalEntityKind.CharacterInstance, RequiredAttribute(node, "SourceTargetInstanceId"), "PossessedBody/@SourceTargetInstanceId");
			}
			var bodyField = name switch
			{
				"AstralProjection" => "ProjectionBodyId",
				"MagicalCopy" => "CopyBodyId",
				"PhysicalClone" => "CloneBodyId",
				"PossessedBody" => "ShellBodyId",
				"PossessedCorpse" or "AnimatedCorpse" => "OriginalBodyId",
				_ => "BodyId"
			};
			if (Needed(PhysicalEntityKind.Body))
				yield return Read(PhysicalEntityKind.Body, RequiredAttribute(node, bodyField), $"{name}/@{bodyField}");
		}
	}

	public static IEnumerable<PhysicalEntityReference> Component(string type, string value, Func<long, long?> legacyBody)
	{
		if (type is not ("Corpse" or "Bodypart")) yield break;
		var root = XElement.Parse(value);
		var bodyField = type == "Corpse" ? "OriginalBody" : "OriginalBodyId";
		var body = Read(PhysicalEntityKind.Body, root.Element(bodyField)?.Value ?? "0", $"{type}/{bodyField}");
		if (body.Id == 0 && (int?)root.Element("RemainsContext") is null or 0)
		{
			var characterField = type == "Corpse" ? "OriginalCharacter" : "OriginalCharacterId";
			var character = Read(PhysicalEntityKind.Character, Required(root, characterField), $"{type}/{characterField}");
			body = new(PhysicalEntityKind.Body, legacyBody(character.Id) ?? 0, $"{type}/{characterField}/LegacyFinalBody");
		}
		yield return body;
		if (type == "Bodypart")
			foreach (var wound in root.Element("Wounds")?.Elements("Wound") ?? [])
				yield return Read(PhysicalEntityKind.Wound, wound.Value, "Bodypart/Wounds/Wound");
	}

	public static IEnumerable<PhysicalEntityReference> Group(string value)
	{
		var members = XElement.Parse(value).Element("Members");
		foreach (var member in members?.Elements() ?? [])
		{
			if (member.Name.LocalName.Equals("Member", StringComparison.OrdinalIgnoreCase))
			{
				yield return Read(PhysicalEntityKind.Character, member.Attribute("character")?.Value ?? member.Attribute("id")?.Value ?? "0", "Group/Members/Member/@character");
				if (member.Attribute("instance") is { } instance)
					yield return Read(PhysicalEntityKind.CharacterInstance, instance.Value, "Group/Members/Member/@instance");
			}
			else
			{
				yield return Read(PhysicalEntityKind.Character, member.Value, "Group/Members/LegacyMember");
			}
		}
	}

	public static bool IsCharacterVariableType(string typeDefinition)
	{
		var type = ProgVariableTypes.FromStorageString(typeDefinition);
		var collection = type.HasFlag(ProgVariableTypes.Collection);
		var dictionary = type.HasFlag(ProgVariableTypes.Dictionary);
		var collectionDictionary = type.HasFlag(ProgVariableTypes.CollectionDictionary);
		var underlying = collection ? type ^ ProgVariableTypes.Collection : dictionary ? type ^ ProgVariableTypes.Dictionary :
			collectionDictionary ? type ^ ProgVariableTypes.CollectionDictionary : type;
		return underlying == ProgVariableTypes.Character;
	}

	public static IEnumerable<PhysicalEntityReference> Variable(string typeDefinition, string value)
	{
		if (!IsCharacterVariableType(typeDefinition)) yield break;
		var type = ProgVariableTypes.FromStorageString(typeDefinition);
		var collection = type.HasFlag(ProgVariableTypes.Collection);
		var dictionary = type.HasFlag(ProgVariableTypes.Dictionary);
		var collectionDictionary = type.HasFlag(ProgVariableTypes.CollectionDictionary);
		var root = XElement.Parse(value);
		var nodes = collection ? root.Elements("var") : dictionary || collectionDictionary ?
			root.Elements("value").SelectMany(x => x.Elements("var")) : [root];
		foreach (var node in nodes)
			yield return Read(PhysicalEntityKind.Character, node.Value, "Variable/CharacterValue");
	}

	public static IEnumerable<PhysicalEntityReference> RouteMotion(string? value)
	{
		if (string.IsNullOrWhiteSpace(value)) yield break;
		using var json = JsonDocument.Parse(value);
		if (!json.RootElement.TryGetProperty("Participants", out var participants)) yield break;
		foreach (var participant in participants.EnumerateArray())
		{
			if (participant.GetProperty("Type").GetString() != "Character") continue;
			yield return new(PhysicalEntityKind.Character, participant.GetProperty("Id").GetInt64(), "RouteMotion/Participants/Character/Id");
			if (participant.TryGetProperty("InstanceId", out var instance) && instance.ValueKind != JsonValueKind.Null)
				yield return new(PhysicalEntityKind.CharacterInstance, instance.GetInt64(), "RouteMotion/Participants/Character/InstanceId");
		}
	}

	public static IEnumerable<PhysicalEntityReference> UserInput(string? value)
	{
		if (!ComputerProcessWaitArguments.TryParseUserInput(value, out var characterId, out _))
			throw new FormatException("Computer/UserInput/CharacterId or TerminalItemId is invalid.");
		return [new(PhysicalEntityKind.Character, characterId, "Computer/UserInput/CharacterId")];
	}

	public static IEnumerable<PhysicalEntityReference> Typed(string type, string id, string field)
	{
		PhysicalEntityKind? kind = type switch
		{
			"Character" => PhysicalEntityKind.Character,
			"CharacterInstance" => PhysicalEntityKind.CharacterInstance,
			"Body" => PhysicalEntityKind.Body,
			"Wound" => PhysicalEntityKind.Wound,
			_ => null
		};
		if (kind is { } known) yield return Read(known, id, field);
	}

	private static string Required(XElement? root, string field) => root?.Element(field)?.Value ??
		throw new FormatException($"{field} is missing from its reference contract.");
	private static string RequiredAttribute(XElement root, string field) => root.Attribute(field)?.Value ??
		throw new FormatException($"{root.Name}/@{field} is missing from its reference contract.");
	private static PhysicalEntityReference Read(PhysicalEntityKind kind, string value, string field) =>
		long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
			? new(kind, id, field)
			: throw new FormatException($"{field} is not a valid {kind} identity.");
}

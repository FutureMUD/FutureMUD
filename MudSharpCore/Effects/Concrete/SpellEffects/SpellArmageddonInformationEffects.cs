#nullable enable

using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellIdentifyEffect : SimpleSpellStatusEffectBase, IIdentifyLookEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellIdentify", (effect, owner) => new SpellIdentifyEffect(effect, owner));
	}

	public SpellIdentifyEffect(IPerceivable owner, IMagicSpellEffectParent parent, long casterId, long casterInstanceId,
		IFutureProg identifyProg, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		CasterId = casterId;
		CasterInstanceId = casterInstanceId;
		IdentifyProgId = identifyProg.Id;
	}

	private SpellIdentifyEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		CasterId = long.Parse(trueRoot?.Element("CasterId")?.Value ?? "0");
		CasterInstanceId = long.Parse(trueRoot?.Element("CasterInstanceId")?.Value ?? "0");
		IdentifyProgId = long.Parse(trueRoot?.Element("IdentifyProgId")?.Value ??
		                           trueRoot?.Element("ProgId")?.Value ?? "0");
	}

	public long CasterId { get; }
	public long CasterInstanceId { get; }
	public long IdentifyProgId { get; }
	protected override string SpecificEffectType => "SpellIdentify";

	public string? GetLookText(ICharacter target)
	{
		var prog = Gameworld.FutureProgs.Get(IdentifyProgId);
		if (prog is null)
		{
			return null;
		}

		var caster = CasterId > 0
			? CharacterInstanceIdentityComparer.ResolvePhysicalInstance(Gameworld, CasterId, CasterInstanceId, true)
			: null;
		var text = caster is not null && prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character])
			? prog.ExecuteString(target, caster)
			: prog.MatchesParameters([ProgVariableTypes.Character])
				? prog.ExecuteString(target)
				: string.Empty;

		return string.IsNullOrWhiteSpace(text) ? null : text;
	}

	public override string Describe(IPerceiver voyeur)
	{
		var prog = Gameworld.FutureProgs.Get(IdentifyProgId);
		return $"Providing identify LOOK text from {prog?.MXPClickableFunctionNameWithId() ?? $"missing prog #{IdentifyProgId.ToString("N0", voyeur)}"}";
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("CasterId", CasterId),
			new XElement("CasterInstanceId", CasterInstanceId),
			new XElement("IdentifyProgId", IdentifyProgId)
		);
	}
}

public sealed class SpellReciteProxyEffect : SimpleSpellStatusEffectBase, IReciteProxyEffect, IHandleEventsEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellReciteProxy", (effect, owner) => new SpellReciteProxyEffect(effect, owner));
	}

	public SpellReciteProxyEffect(
		IPerceivable owner,
		IMagicSpellEffectParent parent,
		long casterId,
		long casterInstanceId,
		long linkedCharacterId,
		long linkedInstanceId,
		double relayChance,
		string reciteEcho,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		CasterId = casterId;
		CasterInstanceId = casterInstanceId;
		LinkedCharacterId = linkedCharacterId;
		LinkedInstanceId = linkedInstanceId;
		RelayChance = relayChance;
		ReciteEcho = reciteEcho;
	}

	private SpellReciteProxyEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		CasterId = long.Parse(trueRoot?.Element("CasterId")?.Value ?? "0");
		CasterInstanceId = long.Parse(trueRoot?.Element("CasterInstanceId")?.Value ?? "0");
		LinkedCharacterId = long.Parse(trueRoot?.Element("LinkedCharacterId")?.Value ?? "0");
		LinkedInstanceId = long.Parse(trueRoot?.Element("LinkedInstanceId")?.Value ?? "0");
		RelayChance = double.Parse(trueRoot?.Element("RelayChance")?.Value ?? "1.0");
		ReciteEcho = trueRoot?.Element("ReciteEcho")?.Value ?? ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho;
	}

	public long CasterId { get; }
	public long CasterInstanceId { get; }
	public long LinkedCharacterId { get; }
	public long LinkedInstanceId { get; }
	public double RelayChance { get; }
	public string ReciteEcho { get; }
	protected override string SpecificEffectType => "SpellReciteProxy";

	public bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (Owner is not ICharacter proxy || proxy.Location is null || !TryGetSpeechEvent(type, arguments,
			    out var speaker, out var target, out var volume, out var language, out var accent, out var message))
		{
			return false;
		}

		if (!IsLinkedSpeaker(speaker) || CharacterInstanceIdentityComparer.SamePhysicalInstance(proxy, speaker))
		{
			return false;
		}

		if (RelayChance < 1.0 && RandomUtilities.DoubleRandom(0.0, 1.0) > RelayChance)
		{
			return false;
		}

		var spoken = new SpokenLanguageInfo(language, accent, volume, message, Outcome.Pass, speaker, target, proxy);
		proxy.OutputHandler.Handle(new LanguageOutput(
			new Emote(ReciteEcho, proxy, target),
			spoken,
			null,
			flags: OutputFlags.SuppressObscured));
		return false;
	}

	public bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.CharacterSpeaksWitness) ||
		       types.Contains(EventType.CharacterSpeaksDirectWitness) ||
		       types.Contains(EventType.CharacterSpeaksNearbyWitness);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Reciting speech from character #{LinkedCharacterId.ToString("N0", voyeur)} with {(RelayChance).ToString("P2", voyeur)} relay chance.";
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("CasterId", CasterId),
			new XElement("CasterInstanceId", CasterInstanceId),
			new XElement("LinkedCharacterId", LinkedCharacterId),
			new XElement("LinkedInstanceId", LinkedInstanceId),
			new XElement("RelayChance", RelayChance),
			new XElement("ReciteEcho", new XCData(ReciteEcho))
		);
	}

	private bool IsLinkedSpeaker(ICharacter speaker)
	{
		return LinkedInstanceId > 0
			? speaker.InstanceId == LinkedInstanceId
			: CharacterInstanceIdentityComparer.IdentityId(speaker) == LinkedCharacterId;
	}

	private static bool TryGetSpeechEvent(EventType type, dynamic[] arguments, out ICharacter speaker,
		out IPerceivable? target, out AudioVolume volume, out ILanguage language, out IAccent accent, out string message)
	{
		speaker = null!;
		target = null;
		volume = AudioVolume.Decent;
		language = null!;
		accent = null!;
		message = string.Empty;

		if (arguments.Length == 0 || arguments[0] is not ICharacter parsedSpeaker)
		{
			return false;
		}

		speaker = parsedSpeaker;
		switch (type)
		{
			case EventType.CharacterSpeaksWitness:
				if (arguments.Length < 6)
				{
					return false;
				}

				volume = arguments[2];
				language = arguments[3];
				accent = arguments[4];
				message = arguments[5];
				return true;
			case EventType.CharacterSpeaksDirectWitness:
			case EventType.CharacterSpeaksNearbyWitness:
				if (arguments.Length < 7)
				{
					return false;
				}

				target = arguments[1] as IPerceivable;
				volume = arguments[3];
				language = arguments[4];
				accent = arguments[5];
				message = arguments[6];
				return true;
			default:
				return false;
		}
	}
}

public sealed class SpellDeadSpeakEffect : SimpleSpellStatusEffectBase, IDeadSpeakEffect
{
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellDeadSpeak", (effect, owner) => new SpellDeadSpeakEffect(effect, owner));
	}

	public SpellDeadSpeakEffect(
		IPerceivable owner,
		IMagicSpellEffectParent parent,
		long anchorCharacterId,
		long anchorInstanceId,
		long corpseItemId,
		long originalCharacterId,
		long originalBodyId,
		long animatedInstanceId,
		long originalLocationId,
		int originalRoomLayer,
		long sourceSpellId,
		long linkedCharacterId,
		long linkedInstanceId,
		double relayChance,
		CharacterInstancePersistencePolicy persistencePolicy,
		string roomEcho,
		string collapseEcho,
		string restoreEcho,
		string reciteEcho,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		AnchorCharacterId = anchorCharacterId;
		AnchorInstanceId = anchorInstanceId;
		CorpseItemId = corpseItemId;
		OriginalCharacterId = originalCharacterId;
		OriginalBodyId = originalBodyId;
		AnimatedInstanceId = animatedInstanceId;
		OriginalLocationId = originalLocationId;
		OriginalRoomLayer = originalRoomLayer;
		SourceSpellId = sourceSpellId;
		LinkedCharacterId = linkedCharacterId;
		LinkedInstanceId = linkedInstanceId;
		RelayChance = relayChance;
		PersistencePolicy = persistencePolicy;
		RoomEcho = roomEcho;
		CollapseEcho = collapseEcho;
		RestoreEcho = restoreEcho;
		ReciteEcho = reciteEcho;
	}

	private SpellDeadSpeakEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		AnchorCharacterId = long.Parse(trueRoot?.Element("AnchorCharacterId")?.Value ?? "0");
		AnchorInstanceId = long.Parse(trueRoot?.Element("AnchorInstanceId")?.Value ?? "0");
		CorpseItemId = long.Parse(trueRoot?.Element("CorpseItemId")?.Value ?? "0");
		OriginalCharacterId = long.Parse(trueRoot?.Element("OriginalCharacterId")?.Value ?? "0");
		OriginalBodyId = long.Parse(trueRoot?.Element("OriginalBodyId")?.Value ?? "0");
		AnimatedInstanceId = long.Parse(trueRoot?.Element("AnimatedInstanceId")?.Value ?? "0");
		OriginalLocationId = long.Parse(trueRoot?.Element("OriginalLocationId")?.Value ?? "0");
		OriginalRoomLayer = int.Parse(trueRoot?.Element("OriginalRoomLayer")?.Value ?? "0");
		SourceSpellId = long.Parse(trueRoot?.Element("SourceSpellId")?.Value ?? "0");
		LinkedCharacterId = long.Parse(trueRoot?.Element("LinkedCharacterId")?.Value ?? "0");
		LinkedInstanceId = long.Parse(trueRoot?.Element("LinkedInstanceId")?.Value ?? "0");
		RelayChance = double.Parse(trueRoot?.Element("RelayChance")?.Value ?? "1.0");
		PersistencePolicy = (trueRoot?.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.TemporaryEffectBound;
		RoomEcho = trueRoot?.Element("RoomEcho")?.Value ?? ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRoomEcho;
		CollapseEcho = trueRoot?.Element("CollapseEcho")?.Value ??
		               ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakCollapseEcho;
		RestoreEcho = trueRoot?.Element("RestoreEcho")?.Value ??
		              ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRestoreEcho;
		ReciteEcho = trueRoot?.Element("ReciteEcho")?.Value ?? ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho;
	}

	public long AnchorCharacterId { get; }
	public long AnchorInstanceId { get; }
	public long CorpseItemId { get; }
	public long OriginalCharacterId { get; }
	public long OriginalBodyId { get; }
	public long AnimatedInstanceId { get; }
	public long OriginalLocationId { get; }
	public int OriginalRoomLayer { get; }
	public long SourceSpellId { get; }
	public long LinkedCharacterId { get; }
	public long LinkedInstanceId { get; }
	public double RelayChance { get; }
	public IReadOnlyCollection<long> ArtificialIntelligenceIds => Array.Empty<long>();
	public CharacterInstancePersistencePolicy PersistencePolicy { get; }
	public string RoomEcho { get; }
	public string CollapseEcho { get; }
	public string RestoreEcho { get; }
	public string ReciteEcho { get; }
	protected override string SpecificEffectType => "SpellDeadSpeak";

	public override void InitialEffect()
	{
		base.InitialEffect();
		var animated = AnimatedCharacter();
		if (animated is null)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

		EnsureReciteProxy(animated);
		EmitRoomEcho(animated, RoomEcho, animated, Owner);
	}

	public override void Login()
	{
		base.Login();
		Owner.RemoveEffect(this, true);
	}

	public override void RemovalEffect()
	{
		if (_removing)
		{
			return;
		}

		_removing = true;
		try
		{
			var animated = AnimatedCharacter();
			var restoreLocation = animated?.Location;
			var restoreLayer = animated?.RoomLayer;
			if (animated is not null)
			{
				animated.RemoveAllEffects<IReciteProxyEffect>(
					x => x.CasterId == AnchorCharacterId && x.LinkedCharacterId == LinkedCharacterId,
					true);
				if (!string.IsNullOrWhiteSpace(CollapseEcho))
				{
					EmitRoomEcho(animated, CollapseEcho, animated);
				}
			}

			if (animated is ICharacterInstance instance)
			{
				CharacterInstanceService.Retire(instance, out _, true, animated.State.HasFlag(CharacterState.Dead),
					false);
			}

			RestoreCorpseItem(animated, restoreLocation, restoreLayer);
			base.RemovalEffect();
		}
		finally
		{
			_removing = false;
		}
	}

	public override void CancelEffect()
	{
		RemovalEffect();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Animating corpse item #{CorpseItemId.ToString("N0", voyeur)} as dead-speaking instance #{AnimatedInstanceId.ToString("N0", voyeur)}.";
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("AnchorCharacterId", AnchorCharacterId),
			new XElement("AnchorInstanceId", AnchorInstanceId),
			new XElement("CorpseItemId", CorpseItemId),
			new XElement("OriginalCharacterId", OriginalCharacterId),
			new XElement("OriginalBodyId", OriginalBodyId),
			new XElement("AnimatedInstanceId", AnimatedInstanceId),
			new XElement("OriginalLocationId", OriginalLocationId),
			new XElement("OriginalRoomLayer", OriginalRoomLayer),
			new XElement("SourceSpellId", SourceSpellId),
			new XElement("LinkedCharacterId", LinkedCharacterId),
			new XElement("LinkedInstanceId", LinkedInstanceId),
			new XElement("RelayChance", RelayChance),
			new XElement("PersistencePolicy", PersistencePolicy),
			new XElement("RoomEcho", new XCData(RoomEcho)),
			new XElement("CollapseEcho", new XCData(CollapseEcho)),
			new XElement("RestoreEcho", new XCData(RestoreEcho)),
			new XElement("ReciteEcho", new XCData(ReciteEcho))
		);
	}

	private void EnsureReciteProxy(ICharacter animated)
	{
		if (animated.EffectsOfType<IReciteProxyEffect>().Any(x =>
			    x.CasterId == AnchorCharacterId &&
			    x.LinkedCharacterId == LinkedCharacterId &&
			    x.LinkedInstanceId == LinkedInstanceId))
		{
			return;
		}

		animated.AddEffect(new SpellReciteProxyEffect(
			animated,
			ParentEffect,
			AnchorCharacterId,
			AnchorInstanceId,
			LinkedCharacterId,
			LinkedInstanceId,
			RelayChance,
			ReciteEcho,
			ApplicabilityProg));
	}

	private void RestoreCorpseItem(ICharacter? animated, ICell? animatedLocation, RoomLayer? animatedLayer)
	{
		if (Owner is not IGameItem corpse || corpse.Location is not null)
		{
			return;
		}

		var location = animatedLocation ?? Gameworld.Cells.Get(OriginalLocationId);
		if (location is null)
		{
			Gameworld.SystemMessage(
				$"Could not restore corpse item #{CorpseItemId.ToString("N0")} after dead speak: cell #{OriginalLocationId.ToString("N0")} could not be resolved.",
				true);
			return;
		}

		var layer = animatedLayer ?? (RoomLayer)OriginalRoomLayer;
		corpse.RoomLayer = layer;
		location.Insert(corpse, true);
		if (animated is not null && !string.IsNullOrWhiteSpace(RestoreEcho))
		{
			location.Handle(
				layer,
				new EmoteOutput(
					new Emote(RestoreEcho, animated, animated, corpse),
					flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
		}
	}

	private ICharacter? AnimatedCharacter()
	{
		return CharacterInstanceIdentityComparer.ResolvePhysicalInstance(Gameworld, OriginalCharacterId,
			AnimatedInstanceId, true, false);
	}

	private static void EmitRoomEcho(ICharacter source, string echo, params IPerceivable[] targets)
	{
		if (string.IsNullOrWhiteSpace(echo) || source.Location is null)
		{
			return;
		}

		source.Location.Handle(
			source.RoomLayer,
			new EmoteOutput(
				new Emote(echo, source, targets),
				flags: OutputFlags.SuppressObscured | OutputFlags.SuppressSource));
	}
}

internal static class ArmageddonInformationSpellEffectDefaults
{
	public const string DefaultReciteEcho = "@ recite|recites in a hollow borrowed voice";
	public const string DefaultDeadSpeakTargetEcho = "";
	public const string DefaultDeadSpeakRoomEcho = "@ jerk|jerks upright, dead lips moving with borrowed words.";
	public const string DefaultDeadSpeakCollapseEcho = "@ collapse|collapses as the borrowed voice leaves &0.";
	public const string DefaultDeadSpeakRestoreEcho = "@ settle|settles back into $1.";
}

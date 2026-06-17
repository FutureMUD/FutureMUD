#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public sealed class SpellLiveBodyPossessionEffect : SimpleSpellStatusEffectBase, ILiveBodyPossessionEffect,
	INoQuitEffect, IPauseAIEffect
{
	private readonly PossessionControlRuntimeState? _runtimeState;
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellLiveBodyPossession", (effect, owner) => new SpellLiveBodyPossessionEffect(effect, owner));
	}

	public SpellLiveBodyPossessionEffect(
		IPerceivable owner,
		IMagicSpellEffectParent parent,
		long anchorCharacterId,
		long anchorInstanceId,
		long targetCharacterId,
		long targetInstanceId,
		long targetBodyId,
		long sourceSpellId,
		CharacterInstancePersistencePolicy persistencePolicy,
		PossessionControlRuntimeState runtimeState,
		string possessionEcho,
		string roomEcho,
		string collapseEcho,
		string backlashEcho,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		AnchorCharacterId = anchorCharacterId;
		AnchorInstanceId = anchorInstanceId;
		TargetCharacterId = targetCharacterId;
		TargetInstanceId = targetInstanceId;
		TargetBodyId = targetBodyId;
		SourceSpellId = sourceSpellId;
		PersistencePolicy = persistencePolicy;
		_runtimeState = runtimeState;
		PossessionEcho = possessionEcho;
		RoomEcho = roomEcho;
		CollapseEcho = collapseEcho;
		BacklashEcho = backlashEcho;
	}

	private SpellLiveBodyPossessionEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		AnchorCharacterId = long.Parse(trueRoot?.Element("AnchorCharacterId")?.Value ?? "0");
		AnchorInstanceId = long.Parse(trueRoot?.Element("AnchorInstanceId")?.Value ?? "0");
		TargetCharacterId = long.Parse(trueRoot?.Element("TargetCharacterId")?.Value ?? "0");
		TargetInstanceId = long.Parse(trueRoot?.Element("TargetInstanceId")?.Value ?? "0");
		TargetBodyId = long.Parse(trueRoot?.Element("TargetBodyId")?.Value ?? "0");
		SourceSpellId = long.Parse(trueRoot?.Element("SourceSpellId")?.Value ?? "0");
		PersistencePolicy = (trueRoot?.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.TemporaryEffectBound;
		PossessionEcho = trueRoot?.Element("PossessionEcho")?.Value ??
		                 SeizeBodySpellEffect.DefaultPossessionEcho;
		RoomEcho = trueRoot?.Element("RoomEcho")?.Value ?? SeizeBodySpellEffect.DefaultRoomEcho;
		CollapseEcho = trueRoot?.Element("CollapseEcho")?.Value ?? SeizeBodySpellEffect.DefaultCollapseEcho;
		BacklashEcho = trueRoot?.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public long AnchorCharacterId { get; }
	public long AnchorInstanceId { get; }
	public long TargetCharacterId { get; }
	public long TargetInstanceId { get; }
	public long TargetBodyId { get; }
	public long SourceSpellId { get; }
	public CharacterInstancePersistencePolicy PersistencePolicy { get; }
	public string PossessionEcho { get; }
	public string RoomEcho { get; }
	public string CollapseEcho { get; }
	public string BacklashEcho { get; }
	public string NoQuitReason => "You cannot quit while commanding a possessed body.";
	protected override string SpecificEffectType => "SpellLiveBodyPossession";

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("AnchorCharacterId", AnchorCharacterId),
			new XElement("AnchorInstanceId", AnchorInstanceId),
			new XElement("TargetCharacterId", TargetCharacterId),
			new XElement("TargetInstanceId", TargetInstanceId),
			new XElement("TargetBodyId", TargetBodyId),
			new XElement("SourceSpellId", SourceSpellId),
			new XElement("PersistencePolicy", PersistencePolicy),
			new XElement("PossessionEcho", new XCData(PossessionEcho)),
			new XElement("RoomEcho", new XCData(RoomEcho)),
			new XElement("CollapseEcho", new XCData(CollapseEcho)),
			new XElement("BacklashEcho", new XCData(BacklashEcho))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Possessed by spell #{SourceSpellId.ToString("N0", voyeur)} from character #{AnchorCharacterId.ToString("N0", voyeur)}.";
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (_runtimeState is null || Owner is not ICharacter target)
		{
			return;
		}

		EmitRoomEcho(target, RoomEcho, target);
		SendIfNotSuppressed(target, PossessionEcho);
		target.Body.Look();
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
			if (_runtimeState is not null)
			{
				var anchor = AnchorCharacter();
				var target = TargetCharacter();
				PossessionControlService.RestoreLivePossession(anchor, target, _runtimeState);
				if (anchor is not null)
				{
					SendIfNotSuppressed(anchor, CollapseEcho);
					SendIfNotSuppressed(anchor, BacklashEcho);
				}
			}

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

	private ICharacter? AnchorCharacter()
	{
		return AnchorCharacterId > 0 ? Gameworld.TryGetCharacter(AnchorCharacterId, true) : null;
	}

	private ICharacter? TargetCharacter()
	{
		return Owner as ICharacter ??
		       CharacterInstanceIdentityComparer.ResolvePhysicalInstance(Gameworld, TargetCharacterId, TargetInstanceId,
			       true, false);
	}

	private static void SendIfNotSuppressed(ICharacter character, string echo)
	{
		if (!string.IsNullOrWhiteSpace(echo))
		{
			character.OutputHandler?.Send(echo.SubstituteANSIColour());
		}
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

public sealed class SpellCorpsePossessionEffect : SimpleSpellStatusEffectBase, ICorpsePossessionEffect
{
	private readonly PossessionControlRuntimeState? _runtimeState;
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellCorpsePossession", (effect, owner) => new SpellCorpsePossessionEffect(effect, owner));
	}

	public SpellCorpsePossessionEffect(
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
		CharacterInstancePersistencePolicy persistencePolicy,
		PossessionControlRuntimeState runtimeState,
		string possessionEcho,
		string roomEcho,
		string collapseEcho,
		string backlashEcho,
		string restoreEcho,
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
		PersistencePolicy = persistencePolicy;
		_runtimeState = runtimeState;
		PossessionEcho = possessionEcho;
		RoomEcho = roomEcho;
		CollapseEcho = collapseEcho;
		BacklashEcho = backlashEcho;
		RestoreEcho = restoreEcho;
	}

	private SpellCorpsePossessionEffect(XElement root, IPerceivable owner)
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
		PersistencePolicy = (trueRoot?.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.TemporaryEffectBound;
		PossessionEcho = trueRoot?.Element("PossessionEcho")?.Value ??
		                 PossessCorpseSpellEffect.DefaultPossessionEcho;
		RoomEcho = trueRoot?.Element("RoomEcho")?.Value ?? PossessCorpseSpellEffect.DefaultRoomEcho;
		CollapseEcho = trueRoot?.Element("CollapseEcho")?.Value ?? PossessCorpseSpellEffect.DefaultCollapseEcho;
		BacklashEcho = trueRoot?.Element("BacklashEcho")?.Value ?? string.Empty;
		RestoreEcho = trueRoot?.Element("RestoreEcho")?.Value ?? PossessCorpseSpellEffect.DefaultRestoreEcho;
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
	public CharacterInstancePersistencePolicy PersistencePolicy { get; }
	public string PossessionEcho { get; }
	public string RoomEcho { get; }
	public string CollapseEcho { get; }
	public string BacklashEcho { get; }
	public string RestoreEcho { get; }
	protected override string SpecificEffectType => "SpellCorpsePossession";

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
			new XElement("PersistencePolicy", PersistencePolicy),
			new XElement("PossessionEcho", new XCData(PossessionEcho)),
			new XElement("RoomEcho", new XCData(RoomEcho)),
			new XElement("CollapseEcho", new XCData(CollapseEcho)),
			new XElement("BacklashEcho", new XCData(BacklashEcho)),
			new XElement("RestoreEcho", new XCData(RestoreEcho))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Animating corpse item #{CorpseItemId.ToString("N0", voyeur)} as instance #{AnimatedInstanceId.ToString("N0", voyeur)}.";
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (_runtimeState is null)
		{
			return;
		}

		var animated = AnimatedCharacter();
		if (animated is null)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

		EmitRoomEcho(animated, RoomEcho, animated, Owner);
		SendIfNotSuppressed(animated, PossessionEcho);
		animated.Body.Look();
	}

	public override void Login()
	{
		base.Login();
		if (_runtimeState is null)
		{
			Owner.RemoveEffect(this, true);
		}
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
			var anchor = AnchorCharacter();
			var animated = AnimatedCharacter();
			var restoreLocation = animated?.Location;
			var restoreLayer = animated?.RoomLayer;
			if (_runtimeState is not null)
			{
				PossessionControlService.RestoreLivePossession(anchor, animated, _runtimeState);
				if (anchor is not null)
				{
					SendIfNotSuppressed(anchor, CollapseEcho);
					SendIfNotSuppressed(anchor, BacklashEcho);
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
				$"Could not restore corpse item #{CorpseItemId.ToString("N0")} after possession: cell #{OriginalLocationId.ToString("N0")} could not be resolved.",
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

	private ICharacter? AnchorCharacter()
	{
		return AnchorCharacterId > 0 ? Gameworld.TryGetCharacter(AnchorCharacterId, true) : null;
	}

	private ICharacter? AnimatedCharacter()
	{
		return CharacterInstanceIdentityComparer.ResolvePhysicalInstance(Gameworld, OriginalCharacterId,
			AnimatedInstanceId, true, false);
	}

	private static void SendIfNotSuppressed(ICharacter character, string echo)
	{
		if (!string.IsNullOrWhiteSpace(echo))
		{
			character.OutputHandler?.Send(echo.SubstituteANSIColour());
		}
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

public sealed class CorpsePossessionDispelProxyEffect : Effect, IPossessionDispelProxyEffect, INoQuitEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("CorpsePossessionDispelProxy",
			(effect, owner) => new CorpsePossessionDispelProxyEffect(effect, owner));
	}

	public CorpsePossessionDispelProxyEffect(IPerceivable owner, long corpseItemId, IFutureProg? prog = null)
		: base(owner, prog)
	{
		CorpseItemId = corpseItemId;
	}

	private CorpsePossessionDispelProxyEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		CorpseItemId = long.Parse(root.Element("Effect")?.Element("CorpseItemId")?.Value ?? "0");
	}

	public long CorpseItemId { get; }
	public string NoQuitReason => "You cannot quit while commanding a possessed corpse.";

	public IEnumerable<IPerceivable> AdditionalDispelTargets
	{
		get
		{
			var item = Gameworld.Items.Get(CorpseItemId);
			return item is null ? [] : [item];
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Dispel proxy for possessed corpse item #{CorpseItemId.ToString("N0", voyeur)}.";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("CorpseItemId", CorpseItemId)
		);
	}

	protected override string SpecificEffectType => "CorpsePossessionDispelProxy";
}

public sealed class SpellAnimatedCorpseEffect : SimpleSpellStatusEffectBase, IAnimatedCorpseEffect
{
	private bool _removing;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellAnimatedCorpse", (effect, owner) => new SpellAnimatedCorpseEffect(effect, owner));
	}

	public SpellAnimatedCorpseEffect(
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
		IEnumerable<long> artificialIntelligenceIds,
		CharacterInstancePersistencePolicy persistencePolicy,
		string roomEcho,
		string collapseEcho,
		string restoreEcho,
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
		ArtificialIntelligenceIds = artificialIntelligenceIds.Distinct().ToArray();
		PersistencePolicy = persistencePolicy;
		RoomEcho = roomEcho;
		CollapseEcho = collapseEcho;
		RestoreEcho = restoreEcho;
	}

	private SpellAnimatedCorpseEffect(XElement root, IPerceivable owner)
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
		ArtificialIntelligenceIds = trueRoot?.Element("ArtificialIntelligences")?.Elements("AI")
		                                  .Select(x => long.TryParse(x.Attribute("id")?.Value ?? x.Value, out var value)
			                                  ? value
			                                  : 0L)
		                                  .Where(x => x > 0)
		                                  .Distinct()
		                                  .ToArray() ?? Array.Empty<long>();
		PersistencePolicy = (trueRoot?.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.TemporaryEffectBound;
		RoomEcho = trueRoot?.Element("RoomEcho")?.Value ?? AnimateCorpseSpellEffect.DefaultRoomEcho;
		CollapseEcho = trueRoot?.Element("CollapseEcho")?.Value ?? AnimateCorpseSpellEffect.DefaultCollapseEcho;
		RestoreEcho = trueRoot?.Element("RestoreEcho")?.Value ?? AnimateCorpseSpellEffect.DefaultRestoreEcho;
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
	public IReadOnlyCollection<long> ArtificialIntelligenceIds { get; }
	public CharacterInstancePersistencePolicy PersistencePolicy { get; }
	public string RoomEcho { get; }
	public string CollapseEcho { get; }
	public string RestoreEcho { get; }
	protected override string SpecificEffectType => "SpellAnimatedCorpse";

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
			new XElement("PersistencePolicy", PersistencePolicy),
			new XElement("ArtificialIntelligences",
				ArtificialIntelligenceIds.Select(x => new XElement("AI", new XAttribute("id", x)))),
			new XElement("RoomEcho", new XCData(RoomEcho)),
			new XElement("CollapseEcho", new XCData(CollapseEcho)),
			new XElement("RestoreEcho", new XCData(RestoreEcho))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Animating corpse item #{CorpseItemId.ToString("N0", voyeur)} as AI instance #{AnimatedInstanceId.ToString("N0", voyeur)}.";
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		var animated = AnimatedCharacter();
		if (animated is null)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

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
			if (animated is not null && !string.IsNullOrWhiteSpace(CollapseEcho))
			{
				EmitRoomEcho(animated, CollapseEcho, animated);
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
				$"Could not restore corpse item #{CorpseItemId.ToString("N0")} after corpse animation: cell #{OriginalLocationId.ToString("N0")} could not be resolved.",
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

public sealed class CorpseAnimationDispelProxyEffect : Effect, IDispelMagicProxyEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("CorpseAnimationDispelProxy",
			(effect, owner) => new CorpseAnimationDispelProxyEffect(effect, owner));
	}

	public CorpseAnimationDispelProxyEffect(IPerceivable owner, long corpseItemId, IFutureProg? prog = null)
		: base(owner, prog)
	{
		CorpseItemId = corpseItemId;
	}

	private CorpseAnimationDispelProxyEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		CorpseItemId = long.Parse(root.Element("Effect")?.Element("CorpseItemId")?.Value ?? "0");
	}

	public long CorpseItemId { get; }

	public IEnumerable<IPerceivable> AdditionalDispelTargets
	{
		get
		{
			var item = Gameworld.Items.Get(CorpseItemId);
			return item is null ? [] : [item];
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Dispel proxy for animated corpse item #{CorpseItemId.ToString("N0", voyeur)}.";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("CorpseItemId", CorpseItemId)
		);
	}

	protected override string SpecificEffectType => "CorpseAnimationDispelProxy";
}

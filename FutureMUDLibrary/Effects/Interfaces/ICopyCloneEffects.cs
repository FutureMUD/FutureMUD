using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Effects.Interfaces;

public interface IMagicalCopyEffect : IEffectSubtype
{
	long AnchorInstanceId { get; }
	long CopyInstanceId { get; }
	long CopyBodyId { get; }
	long SourceSpellId { get; }
	string FormKey { get; }
	bool PlayerFocusable { get; }
	bool Intangible { get; }
	long PlaneId { get; }
	CharacterInstancePersistencePolicy PersistencePolicy { get; }
}

public interface IPhysicalCloneEffect : IEffectSubtype
{
	long AnchorInstanceId { get; }
	long CloneInstanceId { get; }
	long CloneBodyId { get; }
	long SourceSpellId { get; }
	string FormKey { get; }
	bool PlayerFocusable { get; }
	CharacterInstancePersistencePolicy PersistencePolicy { get; }
}

public interface IPossessedBodyEffect : IEffectSubtype
{
	long AnchorCharacterId { get; }
	long AnchorInstanceId { get; }
	long ShellInstanceId { get; }
	long ShellBodyId { get; }
	long SourceTargetCharacterId { get; }
	long SourceTargetInstanceId { get; }
	long SourceSpellId { get; }
	string FormKey { get; }
	CharacterInstancePersistencePolicy PersistencePolicy { get; }
}

public interface ILiveBodyPossessionEffect : IEffectSubtype
{
	long AnchorCharacterId { get; }
	long AnchorInstanceId { get; }
	long TargetCharacterId { get; }
	long TargetInstanceId { get; }
	long TargetBodyId { get; }
	long SourceSpellId { get; }
	CharacterInstancePersistencePolicy PersistencePolicy { get; }
}

public interface ICorpsePossessionEffect : IEffectSubtype
{
	long AnchorCharacterId { get; }
	long AnchorInstanceId { get; }
	long CorpseItemId { get; }
	long OriginalCharacterId { get; }
	long OriginalBodyId { get; }
	long AnimatedInstanceId { get; }
	long OriginalLocationId { get; }
	int OriginalRoomLayer { get; }
	long SourceSpellId { get; }
	CharacterInstancePersistencePolicy PersistencePolicy { get; }
}

public interface IAnimatedCorpseEffect : IEffectSubtype
{
	long AnchorCharacterId { get; }
	long AnchorInstanceId { get; }
	long CorpseItemId { get; }
	long OriginalCharacterId { get; }
	long OriginalBodyId { get; }
	long AnimatedInstanceId { get; }
	long OriginalLocationId { get; }
	int OriginalRoomLayer { get; }
	long SourceSpellId { get; }
	IReadOnlyCollection<long> ArtificialIntelligenceIds { get; }
	CharacterInstancePersistencePolicy PersistencePolicy { get; }
}

public interface IDeadSpeakEffect : IAnimatedCorpseEffect
{
	long LinkedCharacterId { get; }
	long LinkedInstanceId { get; }
	double RelayChance { get; }
	string ReciteEcho { get; }
}

public interface IDispelMagicProxyEffect : IEffectSubtype
{
	IEnumerable<IPerceivable> AdditionalDispelTargets { get; }
}

public interface IPossessionDispelProxyEffect : IDispelMagicProxyEffect;

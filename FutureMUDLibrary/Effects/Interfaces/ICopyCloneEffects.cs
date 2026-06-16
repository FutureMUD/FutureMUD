using MudSharp.Character;
using MudSharp.Effects;

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

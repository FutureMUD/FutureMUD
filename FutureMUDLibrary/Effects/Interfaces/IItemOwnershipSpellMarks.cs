#nullable enable

using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces;

/// <summary>A finite spell mark that refuses taking an item by anyone other than its legal owner.</summary>
public interface IItemTakeInterdictionEffect : IEffectSubtype
{
	ItemOwnershipReference OwnerReference { get; }
	bool IsActive { get; }
	bool AllowsTake(ICharacter actor);
}

/// <summary>An exact item and owner binding for travel effects; display names are never identities.</summary>
public interface ISpellTravelAnchorEffect : IEffectSubtype
{
	long ItemId { get; }
	ItemOwnershipReference OwnerReference { get; }
	bool IsActive { get; }
}

/// <summary>An Arcane Mark's frozen room, distinct from a rune item's current physical location.</summary>
public interface ISpellMarkedRoomEffect : IEffectSubtype
{
	long ItemId { get; }
	long SavedRoomId { get; }
	int RemainingUses { get; }
	bool IsActive { get; }
}

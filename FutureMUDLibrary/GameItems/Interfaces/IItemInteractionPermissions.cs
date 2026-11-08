#nullable enable

using System;

namespace MudSharp.GameItems.Interfaces;

/// <summary>Optional restrictions on ordinary item interactions; creation and forced placement remain separate.</summary>
[Flags]
public enum ItemInteractionRestriction
{
	None = 0,
	Pickup = 1,
	Drop = 2,
	Give = 4,
	Put = 8,
	Sheathe = 16,
	RoomListing = 32,
	OneHandedWield = 64
}

/// <summary>Builder-authored permissions independent of an item's ability to be held, worn or wielded.</summary>
public interface IItemInteractionPermissions : IGameItemComponent
{
	ItemInteractionRestriction Restrictions { get; }
	void SetPermission(ItemInteractionRestriction interaction, bool permitted);
	/// <summary>Runs only on an actual ordinary pickup attempt, never during feasibility queries.</summary>
	bool HandlePickupAttempt(MudSharp.Character.ICharacter actor);
}

public static class ItemInteractionPermissions
{
	/// <summary>All attached permission components must allow an ordinary interaction.</summary>
	public static bool Permits(this IGameItem item, ItemInteractionRestriction interaction) =>
		!System.Linq.Enumerable.Any(item.GetItemTypes<IItemInteractionPermissions>(), x => (x.Restrictions & interaction) != 0);
}

#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp.GameItems;

/// <summary>
/// Detaches one prepared loading participant without stealing custody established by a callback.
/// Completion remains specific to that participant; reentrant commands use the ordinary gates.
/// </summary>
internal static class ComponentItemTransfer
{
	// Release only the exact ammunition owned by this accepted shot. Callback custody wins.
	internal static bool ReleaseFiredItem(IGameItem item, IGameItem weapon)
	{
		if (item.Deleted || item.Destroyed || DirectLocationOf(item) is not null || HasDirectBodyCustody(item) ||
			item.GetItemType<IBeltable>()?.ConnectedTo is not null) return false;
		if (ReferenceEquals(item.ContainedIn, weapon)) item.ContainedIn = null;
		return IsDetached(item);
	}
	internal static IRoom? DirectLocationOf(IGameItem item) => item is GameItem native ? native.DirectLocation : item.Location;
	internal static bool HasDirectBodyCustody(IGameItem item) =>
		item.GetItemType<IHoldable>()?.HeldBy is not null || item.GetItemType<IWearable>()?.WornBy is not null ||
		item.GetItemType<IProsthetic>()?.InstalledBody is not null || item.GetItemType<IImplant>()?.InstalledBody is not null;
	internal static bool IsDetached(IGameItem? item) => item is { Deleted: false, Destroyed: false } &&
		item.InInventoryOf is null && item.ContainedIn is null && DirectLocationOf(item) is null &&
		item.GetItemType<IBeltable>()?.ConnectedTo is null;

	internal static IGameItem? TakeOne(ICharacter actor, IGameItem source)
	{
		using var execution = CommandExecutionScope.EnterBodyOperation(actor);
		if (!CanDetach(actor, source)) return null;
		var stack = source.GetItemType<IStackable>();
		if (!CommandExecutionScope.TryContinue(actor)) return null;
		if (stack is not null && stack.Quantity > 1)
		{
			if (!CanDetach(actor, source)) return null;
			CommandExecutionScope.MarkCommitted(actor);
			var split = stack.Split(1);
			split.Login();
			split.HandleEvent(EventType.ItemFinishedLoading, split);
			return IsDetached(split) ? split : null;
		}

		actor.Body.Take(source);
		return IsDetached(source) ? source : null;
	}

	internal static IGameItem? TakeByWeight(ICharacter actor, IGameItem source, double weight)
	{
		using var execution = CommandExecutionScope.EnterBodyOperation(actor);
		if (!CanDetach(actor, source)) return null;
		var whole = source.DropsWholeByWeight(weight);
		if (!CanDetach(actor, source)) return null;
		if (whole)
		{
			actor.Body.Take(source);
			return IsDetached(source) ? source : null;
		}

		CommandExecutionScope.MarkCommitted(actor);
		// Passing the body would claim the split in HeldBy without installing it in native hand lists.
		var split = source.GetByWeight(null, weight);
		return IsDetached(split) ? split : null;
	}

	private static bool CanDetach(ICharacter actor, IGameItem source) =>
		CommandExecutionScope.TryContinue(actor) && !source.Deleted && !source.Destroyed &&
		ReferenceEquals(source.InInventoryOf, actor.Body) && source.ContainedIn is null && DirectLocationOf(source) is null;

	internal static bool Contain(IGameItem? item, IGameItem destination)
	{
		if (!CommandExecutionScope.TryContinue() || !IsDetached(item) || destination.Deleted || destination.Destroyed) return false;
		if (item is GameItem native)
			native.TrySetContainedIn(destination,
				() => CommandExecutionScope.TryContinue() && IsDetached(item) && !destination.Deleted && !destination.Destroyed,
				() => CommandExecutionScope.MarkCommitted());
		else item!.ContainedIn = destination;
		return !item.Deleted && !item.Destroyed && ReferenceEquals(item.ContainedIn, destination) &&
			DirectLocationOf(item) is null && item.GetItemType<IHoldable>()?.HeldBy is null;
	}

	internal static bool ContainPrepared(IGameItem? item, IGameItem destination, Func<bool> exactState,
		Action adoptReference, Action releaseClaimedReference)
	{
		bool Ready() => CommandExecutionScope.TryContinue() && IsDetached(item) &&
			!destination.Deleted && !destination.Destroyed && exactState();
		if (!Ready()) return false;
		if (item is GameItem native)
			native.TrySetContainedIn(destination, Ready, () => { adoptReference(); CommandExecutionScope.MarkCommitted(); });
		else
		{
			item!.ContainedIn = destination;
			if (!item.Deleted && !item.Destroyed && ReferenceEquals(item.ContainedIn, destination) && exactState())
			{ adoptReference(); CommandExecutionScope.MarkCommitted(); }
		}
		var owned = item is { Deleted: false, Destroyed: false } && ReferenceEquals(item.ContainedIn, destination) &&
			DirectLocationOf(item) is null && item.GetItemType<IHoldable>()?.HeldBy is null;
		if (!owned) releaseClaimedReference();
		return owned;
	}
}

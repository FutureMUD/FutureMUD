#nullable enable
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Economy.Lending;

namespace MudSharp.GameItems;

public partial class GameItem
{
	internal void SetLendingOwner(IFrameworkItem? owner)
	{
		var before = _owner;
		var reference = _ownerReference;
		var old = OwnershipReference;
		LoanService.OnRollback(() => { _owner = before; _ownerReference = reference; Changed = true; });
		_owner = owner;
		_ownerReference = owner == null ? null : new FrameworkItemReference(CharacterInstanceIdentityComparer.FrameworkItemId(owner), owner.FrameworkItemType, Gameworld);
		Changed = true;
		LoanService.AfterCommit(() =>
		{
			if (old.HasValue && old != OwnershipReference) LoanService.EndRetainedPawnOwnership(this);
			HandleEvent(EventType.ItemOwnershipChanged, this, old?.FrameworkItemType ?? "", old?.Id ?? 0L,
				OwnershipReference?.FrameworkItemType ?? "", OwnershipReference?.Id ?? 0L);
		});
	}

	internal void EnlistLendingCustody()
	{
		LoanService.OnRollback(CaptureCustodyRollback());
		foreach (var component in Components.Cast<GameItemComponent>())
		{
			var restore = component.CaptureCustodyRollback() ?? throw new InvalidOperationException("This item cannot participate in a reversible lending transfer.");
			LoanService.OnRollback(() => { restore(); component.Changed = true; });
		}
		if (InInventoryOf is MudSharp.Body.Implementations.Body body) LoanService.OnRollback(body.CaptureCustodyInventoryRollback());
		if (ContainedIn?.GetItemType<IContainer>() is GameItemComponent container)
		{
			var restore = container.CaptureCustodyRollback() ?? throw new InvalidOperationException("This container cannot participate in a reversible lending transfer.");
			LoanService.OnRollback(() => { restore(); container.Changed = true; });
		}
		if (Location is ICustodyRollbackLocation room) LoanService.OnRollback(room.CaptureCustodyMembershipRollback(new IGameItem[] { this }));
	}

	internal void SetLendingPosition(SpatialLocation? location)
	{
		var restore = CaptureCustodyRollback();
		LoanService.OnRollback(restore);
		_containedIn = null;
		SetPreparedSpatialState(location);
		Changed = true;
	}
}

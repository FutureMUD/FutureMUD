using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.Body;
using MudSharp.Movement;

#nullable enable
namespace MudSharp.Magic.Vancian;

/// <summary>Transient work: normal movement/combat/state/logout cancellation, plus identity-focus cancellation.</summary>
public sealed class VancianTimedAction : SimpleCharacterAction
{
	public Guid OperationId { get; }
	private readonly Func<bool>? _stillValid;
	internal void FinishExternally()
	{
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}
	private readonly HashSet<IGameItem> _observed = [];
	public VancianTimedAction(ICharacter actor, Guid operationId, string description, Action completion, Action? cancellation = null, Func<bool>? stillValid = null, IEnumerable<IGameItem>? observedItems = null)
		: base(actor, _ => completion(), description, "$0 stop|stops working on the magic.",
			"$0 must stop working on the magic before moving.", ["general", "movement"], description,
			_ => cancellation?.Invoke())
	{
		OperationId = operationId; _stillValid = stillValid;
		actor.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += Validate;
		foreach (var item in observedItems ?? [])
			for (var current = item; current is not null && _observed.Add(current); current = current.ContainedIn)
			{
				current.OnInventoryChange += ItemChanged; current.OnRemovedFromLocation += ItemRemoved;
				current.OnDeleted += ItemRemoved; current.OnQuit += ItemRemoved;
				if (current.GetItemType<IOpenable>() is { } openable) openable.OnClose += Closed;
			}
	}
	private void Closed(IOpenable _) => Validate();
	private void ItemChanged(InventoryState oldState, InventoryState newState, IGameItem item) => Validate();
	private void ItemRemoved(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	private void Validate()
	{
		try { if (_stillValid?.Invoke() != false) return; } catch { /* Changed or invalid policy cancels precommit work. */ }
		CharacterOwner.RemoveEffect(this, true);
	}
	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		CharacterOwner.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= Validate;
		foreach (var item in _observed)
		{
			item.OnInventoryChange -= ItemChanged; item.OnRemovedFromLocation -= ItemRemoved; item.OnDeleted -= ItemRemoved; item.OnQuit -= ItemRemoved;
			if (item.GetItemType<IOpenable>() is { } openable) openable.OnClose -= Closed;
		}
		_observed.Clear();
	}
	protected override void TargetQuit(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetDeleted(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetDied(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetMoved(object sender, MoveEventArgs args) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetEngagedInMelee(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void OwnerStateChanged(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
}

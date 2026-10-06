using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Movement;

#nullable enable
namespace MudSharp.Magic.Casting;

/// <summary>Transient action only; the service owns prepaid work and its durable deadline.</summary>
public sealed class MagicPracticeAction : SimpleCharacterAction
{
	public Guid OperationId { get; }
	private readonly Func<bool> _stillValid;
	private readonly bool _allowMovement;
	private readonly IBody _body;
	private bool _finished;

	internal MagicPracticeAction(ICharacter actor, Guid id, string description, bool allowMovement,
		Action completion, Action cancellation, Func<bool> stillValid)
		: base(actor, _ => completion(), description, "$0 stop|stops practising magic.",
			"$0 must stop practising magic before moving.", allowMovement ? ["general"] : ["general", "movement"],
			description, _ => cancellation())
	{
		OperationId = id; _stillValid = stillValid; _allowMovement = allowMovement;
		_body = actor.Body;
		actor.CurrentBodyChanged += BodyChanged;
		actor.InvalidPositionTargets += InputsChanged;
		_body.OnInventoryChange += InventoryChanged;
		_body.OnWounded += Wounded;
		actor.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += Validate;
	}

	public override void ExpireEffect()
	{
		if (_finished) return;
		_finished = true;
		OnStopAction = null;
		try { base.ExpireEffect(); }
		finally { ReleaseEventHandlers(); }
	}

	public override void RemovalEffect()
	{
		if (_finished) { ReleaseEventHandlers(); return; }
		_finished = true;
		try { base.RemovalEffect(); }
		finally { ReleaseEventHandlers(); }
	}

	internal void Abandon()
	{
		_finished = true;
		OnStopAction = null;
		try { CharacterOwner.RemoveEffect(this); }
		finally { ReleaseEventHandlers(); }
	}

	private void BodyChanged(ICharacter _, IBody oldBody, IBody newBody) => CharacterOwner.RemoveEffect(this, true);
	private void InputsChanged(object? sender, EventArgs args) => Validate();
	private void InventoryChanged(InventoryState oldState, InventoryState newState, IGameItem item) => Validate();
	private void Wounded(IMortalPerceiver owner, IWound wound) => Validate();
	private void Validate()
	{
		if (_finished) return;
		try { if (_stillValid()) return; }
		catch { /* A failed validation cannot leave prepaid work eligible for completion. */ }
		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		CharacterOwner.CurrentBodyChanged -= BodyChanged;
		CharacterOwner.InvalidPositionTargets -= InputsChanged;
		_body.OnInventoryChange -= InventoryChanged;
		_body.OnWounded -= Wounded;
		CharacterOwner.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= Validate;
	}

	// Keep the cancellation callback: CharacterAction's defaults clear it on these events.
	protected override void TargetQuit(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetDeleted(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetDied(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetMoved(object sender, MoveEventArgs args) { if (!_allowMovement) CharacterOwner.RemoveEffect(this, true); }
	protected override void TargetWantsToMove(IPerceivable perceivable, PerceivableRejectionResponse response)
	{
		if (!_allowMovement) base.TargetWantsToMove(perceivable, response);
	}
	protected override void TargetEngagedInMelee(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void OwnerStateChanged(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
}

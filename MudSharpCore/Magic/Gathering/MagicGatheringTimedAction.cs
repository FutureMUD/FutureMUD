#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.Movement;

namespace MudSharp.Magic.Gathering;

/// <summary>
/// Transient per-actor preparation only. It uses the ordinary action interruption hooks and a bounded live-action
/// validity fallback; it never subscribes rooms or capabilities to an environmental scheduler.
/// </summary>
public sealed class MagicGatheringTimedAction : SimpleCharacterAction
{
	public Guid OperationId { get; }
	private readonly Func<bool> _stillValid;

	public MagicGatheringTimedAction(ICharacter actor, Guid operationId, string description, Action completion,
		Action cancellation, Func<bool> stillValid)
		: base(actor, _ => completion(), description, "$0 stop|stops gathering magic.",
			"$0 must stop gathering magic before moving.", ["general", "movement"], description,
			_ => cancellation())
	{
		OperationId = operationId;
		_stillValid = stillValid;
		actor.CurrentBodyChanged += BodyChanged;
		actor.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += Validate;
	}

	internal void FinishExternally()
	{
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	internal void CancelExternally()
	{
		CharacterOwner.RemoveEffect(this, true);
	}

	public override void ExpireEffect()
	{
		// Expiry is the controlled completion path, not an interruption.
		OnStopAction = null;
		base.ExpireEffect();
	}

	private void BodyChanged(ICharacter actor, IBody oldBody, IBody newBody) => CharacterOwner.RemoveEffect(this, true);

	private void Validate()
	{
		try
		{
			if (_stillValid())
			{
				return;
			}
		}
		catch
		{
			// A policy/configuration exception must never leave a prepaid-looking live action behind.
		}

		CharacterOwner.RemoveEffect(this, true);
	}

	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		CharacterOwner.CurrentBodyChanged -= BodyChanged;
		CharacterOwner.Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= Validate;
	}

	protected override void TargetQuit(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetDeleted(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetDied(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetMoved(object sender, MoveEventArgs args) => CharacterOwner.RemoveEffect(this, true);
	protected override void TargetEngagedInMelee(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
	protected override void OwnerStateChanged(IPerceivable _) => CharacterOwner.RemoveEffect(this, true);
}

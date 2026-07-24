#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.Movement;

namespace MudSharp.Effects.Concrete;

public class PlayingInstrument : Effect
{
	private bool _released;

	public PlayingInstrument(ICharacter owner, InstrumentGameItemComponent instrument) : base(owner)
	{
		CharacterOwner = owner;
		Instrument = instrument;
		RegisterEvents();
	}

	public ICharacter CharacterOwner { get; }
	public InstrumentGameItemComponent Instrument { get; }

	protected override string SpecificEffectType => "PlayingInstrument";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Playing {Instrument.Parent.HowSeen(voyeur)}";
	}

	public override void ExpireEffect()
	{
		if (!Instrument.CanContinue(CharacterOwner))
		{
			EndEffect();
			return;
		}

		Instrument.PerformTick();
		if (Instrument.IsBeingPlayed)
		{
			Owner.Reschedule(this, Instrument.TickInterval);
		}
	}

	public override void Login()
	{
		RegisterEvents();
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
		Instrument.EffectEnded(CharacterOwner);
	}

	private void RegisterEvents()
	{
		_released = false;
		CharacterOwner.Body.OnInventoryChange -= BodyOnInventoryChange;
		CharacterOwner.Body.OnInventoryChange += BodyOnInventoryChange;
		CharacterOwner.OnMoved -= CharacterOnMoved;
		CharacterOwner.OnMoved += CharacterOnMoved;
		CharacterOwner.OnEngagedInMelee -= CharacterInvalidated;
		CharacterOwner.OnEngagedInMelee += CharacterInvalidated;
		CharacterOwner.OnStateChanged -= CharacterStateChanged;
		CharacterOwner.OnStateChanged += CharacterStateChanged;
		CharacterOwner.OnDeath -= CharacterInvalidated;
		CharacterOwner.OnDeath += CharacterInvalidated;
		CharacterOwner.OnQuit -= CharacterInvalidated;
		CharacterOwner.OnQuit += CharacterInvalidated;
		CharacterOwner.OnDeleted -= CharacterInvalidated;
		CharacterOwner.OnDeleted += CharacterInvalidated;
		Instrument.Parent.OnDeath -= CharacterInvalidated;
		Instrument.Parent.OnDeath += CharacterInvalidated;
		Instrument.Parent.OnDeleted -= CharacterInvalidated;
		Instrument.Parent.OnDeleted += CharacterInvalidated;
	}

	private void ReleaseEvents()
	{
		if (_released)
		{
			return;
		}

		_released = true;
		CharacterOwner.Body.OnInventoryChange -= BodyOnInventoryChange;
		CharacterOwner.OnMoved -= CharacterOnMoved;
		CharacterOwner.OnEngagedInMelee -= CharacterInvalidated;
		CharacterOwner.OnStateChanged -= CharacterStateChanged;
		CharacterOwner.OnDeath -= CharacterInvalidated;
		CharacterOwner.OnQuit -= CharacterInvalidated;
		CharacterOwner.OnDeleted -= CharacterInvalidated;
		Instrument.Parent.OnDeath -= CharacterInvalidated;
		Instrument.Parent.OnDeleted -= CharacterInvalidated;
	}

	private void CharacterStateChanged(IPerceivable owner)
	{
		if (!CharacterState.Able.HasFlag(CharacterOwner.State))
		{
			EndEffect();
		}
	}

	private void BodyOnInventoryChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item == Instrument.Parent || !Instrument.CanContinue(CharacterOwner))
		{
			EndEffect();
		}
	}

	private void CharacterOnMoved(object? sender, MoveEventArgs e)
	{
		EndEffect();
	}

	private void CharacterInvalidated(IPerceivable owner)
	{
		EndEffect();
	}

	private void EndEffect()
	{
		CharacterOwner.RemoveEffect(this, true);
	}
}

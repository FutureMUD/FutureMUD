using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class BowDrainStamina : Effect
{
	public ICharacter CharacterOwner { get; set; }
	public IGameItem Bow { get; set; }
	public BowGameItemComponent BowComponent { get; set; }

	public BowDrainStamina(ICharacter owner, BowGameItemComponent bow) : base(owner)
	{
		CharacterOwner = owner;
		Bow = bow.Parent;
		BowComponent = bow;
		RegisterEvents();
	}

	public override void Login()
	{
		RegisterEvents();
	}

	private void RegisterEvents()
	{
		CharacterOwner.Body.OnInventoryChange += Body_OnInventoryChange;
		CharacterOwner.OnDeath += Owner_OnDeath;
		CharacterOwner.OnDeleted += Owner_OnDeleted;
		CharacterOwner.OnStateChanged += Owner_OnStateChanged;
		Bow.OnDeath += Bow_OnDeath;
		Bow.OnDeleted += Bow_OnDeleted;
		BowComponent.OnFire += Bow_OnFire;
		BowComponent.OnUnready += Bow_OnUnready;
	}

	private void ReleaseEvents()
	{
		CharacterOwner.Body.OnInventoryChange -= Body_OnInventoryChange;
		CharacterOwner.OnDeath -= Owner_OnDeath;
		CharacterOwner.OnDeleted -= Owner_OnDeleted;
		CharacterOwner.OnStateChanged -= Owner_OnStateChanged;
		Bow.OnDeath -= Bow_OnDeath;
		Bow.OnDeleted -= Bow_OnDeleted;
		BowComponent.OnFire -= Bow_OnFire;
		BowComponent.OnUnready -= Bow_OnUnready;
		BowComponent.IsReadied = false;
	}

	private void Bow_OnUnready(IPerceivable owner)
	{
		CharacterOwner.RemoveEffect(this);
		ReleaseEvents();
	}

	private void Bow_OnFire(IPerceivable owner)
	{
		CharacterOwner.RemoveEffect(this);
		ReleaseEvents();
	}

	private void Bow_OnDeleted(IPerceivable owner)
	{
		CharacterOwner.RemoveEffect(this);
		ReleaseEvents();
	}

	private void Bow_OnDeath(IPerceivable owner)
	{
		CharacterOwner.RemoveEffect(this);
		ReleaseEvents();
	}

	private void Owner_OnStateChanged(IPerceivable owner)
	{
		if (!CharacterOwner.State.HasFlag(CharacterState.Able))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ release|releases the tension on $0.", CharacterOwner, Bow), flags: OutputFlags.InnerWrap));
			CharacterOwner.RemoveEffect(this);
			ReleaseEvents();
		}
	}

	private void Owner_OnDeleted(IPerceivable owner)
	{
		CharacterOwner.RemoveEffect(this);
		ReleaseEvents();
	}

	private void Owner_OnDeath(IPerceivable owner)
	{
		CharacterOwner.RemoveEffect(this);
		ReleaseEvents();
	}

	private void Body_OnInventoryChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item == Bow && newState != InventoryState.Wielded)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ release|releases the tension on $0.", CharacterOwner, Bow), flags: OutputFlags.InnerWrap));
			CharacterOwner.RemoveEffect(this);
			ReleaseEvents();
		}

		if (!CharacterOwner.Body.FunctioningFreeHands.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ can no longer hold the bowstring on $0, and release|releases the tension.", CharacterOwner,
					Bow), flags: OutputFlags.InnerWrap));
			CharacterOwner.RemoveEffect(this);
			ReleaseEvents();
		}
	}

	public override void ExpireEffect()
	{
		CharacterOwner.SpendStamina(BowComponent.StaminaPerTick);
		if (CharacterOwner.CurrentStamina <= 0.0)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					"@ release|releases the tension on $1 because #0 are|is too exhausted to continue holding it drawn any longer.",
					CharacterOwner, CharacterOwner, Bow), flags: OutputFlags.InnerWrap));
			BowComponent.Unready(null);
			CharacterOwner.RemoveEffect(this);
			ReleaseEvents();
			return;
		}

		Owner.Reschedule(this, TimeSpan.FromSeconds(5));
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	protected override string SpecificEffectType => "BowDrainStamina";

	public override string Describe(IPerceiver voyeur)
	{
		return $"A readied bow is draining that stamina of its wielder.";
	}
}
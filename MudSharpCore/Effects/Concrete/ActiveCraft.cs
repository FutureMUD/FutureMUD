using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class ActiveCraftEffect : Effect, IEffectSubtype, ILDescSuffixEffect, IRemoveOnStateChange,
	IRemoveOnMovementEffect, IRemoveOnMeleeCombat, IActiveCraftEffect
{
	public ICharacter CharacterOwner { get; private set; }
	public IActiveCraftGameItemComponent Component { get; set; }

	public ActiveCraftEffect(ICharacter owner) : base(owner)
	{
		CharacterOwner = owner;
	}

	protected override string SpecificEffectType => "ActiveCraft";

	#region Overrides of Effect

	public override IEnumerable<string> Blocks => new[] { "general", "movement" };

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return Component.Craft.ActionDescription;
	}

	public override bool IsBlockingEffect(string blockingType)
	{
		return string.IsNullOrWhiteSpace(blockingType) || Blocks.Any(x => x.EqualTo(blockingType));
	}

	public override bool CanBeStoppedByPlayer => true;

	#endregion

	public override string Describe(IPerceiver voyeur)
	{
		return $"Crafting {Component.Craft.Name} - Phase {Component.Phase}";
	}

	public bool ShouldRemove(CharacterState newState)
	{
		return newState.HasFlag(CharacterState.Dead) || !CharacterState.Able.HasFlag(newState);
	}

	bool IRemoveOnMovementEffect.ShouldRemove()
	{
		return true;
	}

	/// <summary>Fires when a scheduled effect is cancelled before it matures</summary>
	public override void CancelEffect()
	{
		ReleaseEvents();
		Component.Craft.PauseCraft(CharacterOwner, Component, this);
		Owner.RemoveEffect(this);
	}

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	public TimeSpan NextPhaseDuration { get; set; }

	public override void ExpireEffect()
	{
		var (success, finished) = Component.DoNextPhase(this);
		if (finished)
		{
			base.ExpireEffect();
			Component.ReleaseItems(CharacterOwner.Location, CharacterOwner.RoomLayer);
			Component.Parent.Delete();
			return;
		}

		if (!success)
		{
			return;
		}

		Owner.Reschedule(this, NextPhaseDuration);
	}

	public override void Login()
	{
		SubscribeEvents();
	}

	public void SubscribeEvents()
	{
		CharacterOwner.OnQuit += CharacterOwner_OnQuit;
		CharacterOwner.OnDeath += CharacterOwner_OnDeath;
		CharacterOwner.OnMoved += CharacterOwner_OnMoved;
		CharacterOwner.OnWantsToMove += CharacterOwner_OnWantsToMove;
		CharacterOwner.OnStateChanged += CharacterOwner_OnStateChanged;

		var parent = Component.Parent;
		parent.OnDeath += Item_OnDeath;
		parent.OnQuit += Item_OnQuit;
		parent.OnDeleted += Item_OnDeleted;
		parent.OnRemovedFromLocation += Item_OnRemovedFromLocation;
	}

	private void ReleaseEvents(IGameItem item)
	{
		item.OnDeath -= Item_OnDeath;
		item.OnDeleted -= Item_OnDeleted;
		item.OnQuit -= Item_OnQuit;
		item.OnRemovedFromLocation -= Item_OnRemovedFromLocation;
		item.OnInventoryChange -= Item_OnInventoryChange;
	}

	public void ReleaseEvents()
	{
		CharacterOwner.OnQuit -= CharacterOwner_OnQuit;
		CharacterOwner.OnDeath -= CharacterOwner_OnDeath;
		CharacterOwner.OnMoved -= CharacterOwner_OnMoved;
		CharacterOwner.OnWantsToMove -= CharacterOwner_OnWantsToMove;
		CharacterOwner.OnStateChanged -= CharacterOwner_OnStateChanged;
		ReleaseEvents(Component.Parent);
	}

	private void Item_OnInventoryChange(Body.InventoryState oldState, Body.InventoryState newState, IGameItem item)
	{
		var verb = "";
		switch (oldState)
		{
			case Body.InventoryState.Wielded:
				verb = "wielding";
				break;
			case Body.InventoryState.Held:
				verb = "holding";
				break;
			case Body.InventoryState.Worn:
				verb = "wearing";
				break;
		}

		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ stop|stops {Component.Craft.ActionDescription} because #0 is no longer {verb} $1.", CharacterOwner,
			CharacterOwner, item)));
		CancelEffect();
	}

	private void Item_OnRemovedFromLocation(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ stop|stops {Component.Craft.ActionDescription} because $1 is gone.", CharacterOwner, CharacterOwner,
			owner)));
		CancelEffect();
	}

	private void Item_OnQuit(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ stop|stops {Component.Craft.ActionDescription} because $1 is gone.", CharacterOwner, CharacterOwner,
			owner)));
		CancelEffect();
	}

	private void Item_OnDeleted(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ stop|stops {Component.Craft.ActionDescription} because $1 is gone.", CharacterOwner, CharacterOwner,
			owner)));
		CancelEffect();
	}

	private void Item_OnDeath(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ stop|stops {Component.Craft.ActionDescription} because $1 is gone.", CharacterOwner, CharacterOwner,
			owner)));
		CancelEffect();
	}

	private void CharacterOwner_OnStateChanged(IPerceivable owner)
	{
		if (!CharacterState.Able.HasFlag(CharacterOwner.State))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ stop|stops {Component.Craft.ActionDescription} because #0 is {CharacterOwner.State.Describe()}.",
				CharacterOwner, CharacterOwner)));
			CancelEffect();
		}
	}

	private void CharacterOwner_OnWantsToMove(IPerceivable owner, PerceivableRejectionResponse response)
	{
		response.Rejected = true;
		response.Reason = $"You can't move while you are {Component.Craft.ActionDescription}.";
	}

	private void CharacterOwner_OnMoved(object sender, Movement.MoveEventArgs e)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				$"@ stop|stops {Component.Craft.ActionDescription} because #0 have|has left the area.",
				CharacterOwner, CharacterOwner)));
		CancelEffect();
	}

	private void CharacterOwner_OnDeath(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				$"@ stop|stops {Component.Craft.ActionDescription} because #0 have|has died!",
				CharacterOwner, CharacterOwner)));
		CancelEffect();
	}

	private void CharacterOwner_OnQuit(IPerceivable owner)
	{
		CharacterOwner.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				$"@ stop|stops {Component.Craft.ActionDescription} because #0 have|has left the area.",
				CharacterOwner, CharacterOwner)));
		CancelEffect();
	}

	#region Implementation of ILDescSuffixEffect

	public string SuffixFor(IPerceiver voyeur)
	{
		return Component.Craft.ActionDescription;
	}

	public bool SuffixApplies()
	{
		return true;
	}

	#endregion
}
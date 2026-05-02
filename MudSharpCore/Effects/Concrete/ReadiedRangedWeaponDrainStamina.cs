using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Linq;

#nullable enable annotations

namespace MudSharp.Effects.Concrete;

public class ReadiedRangedWeaponDrainStamina : Effect
{
	private bool _released;

	public ICharacter CharacterOwner { get; set; }
	public IGameItem Weapon { get; set; }
	internal IReadiedRangedWeaponStaminaSource WeaponComponent { get; set; }

	internal ReadiedRangedWeaponDrainStamina(ICharacter owner, IReadiedRangedWeaponStaminaSource weapon) : base(owner)
	{
		CharacterOwner = owner;
		Weapon = weapon.Parent;
		WeaponComponent = weapon;
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
		Weapon.OnDeath += Weapon_OnDeath;
		Weapon.OnDeleted += Weapon_OnDeleted;
		WeaponComponent.OnFire += Weapon_OnFire;
		WeaponComponent.OnUnready += Weapon_OnUnready;
	}

	private void ReleaseEvents()
	{
		if (_released)
		{
			return;
		}

		_released = true;
		CharacterOwner.Body.OnInventoryChange -= Body_OnInventoryChange;
		CharacterOwner.OnDeath -= Owner_OnDeath;
		CharacterOwner.OnDeleted -= Owner_OnDeleted;
		CharacterOwner.OnStateChanged -= Owner_OnStateChanged;
		Weapon.OnDeath -= Weapon_OnDeath;
		Weapon.OnDeleted -= Weapon_OnDeleted;
		WeaponComponent.OnFire -= Weapon_OnFire;
		WeaponComponent.OnUnready -= Weapon_OnUnready;
		WeaponComponent.IsReadied = false;
	}

	private void EndEffect()
	{
		CharacterOwner.RemoveEffect(this);
		ReleaseEvents();
	}

	private void Weapon_OnUnready(IPerceivable owner)
	{
		EndEffect();
	}

	private void Weapon_OnFire(IPerceivable owner)
	{
		EndEffect();
	}

	private void Weapon_OnDeleted(IPerceivable owner)
	{
		EndEffect();
	}

	private void Weapon_OnDeath(IPerceivable owner)
	{
		EndEffect();
	}

	private void Owner_OnStateChanged(IPerceivable owner)
	{
		if (!CharacterOwner.State.HasFlag(CharacterState.Able))
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(WeaponComponent.ReadiedStaminaReleaseEmote, CharacterOwner, Weapon, CharacterOwner),
				flags: OutputFlags.InnerWrap));
			EndEffect();
		}
	}

	private void Owner_OnDeleted(IPerceivable owner)
	{
		EndEffect();
	}

	private void Owner_OnDeath(IPerceivable owner)
	{
		EndEffect();
	}

	private void Body_OnInventoryChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item == Weapon && newState != InventoryState.Wielded)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(WeaponComponent.ReadiedStaminaReleaseEmote, CharacterOwner, Weapon, CharacterOwner),
				flags: OutputFlags.InnerWrap));
			EndEffect();
			return;
		}

		if (WeaponComponent.ReadiedUseRequiresFreeHand && !CharacterOwner.Body.FunctioningFreeHands.Any())
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(WeaponComponent.ReadiedStaminaNoFreeHandEmote, CharacterOwner, Weapon, CharacterOwner),
				flags: OutputFlags.InnerWrap));
			EndEffect();
		}
	}

	public override void ExpireEffect()
	{
		CharacterOwner.SpendStamina(WeaponComponent.StaminaPerTick);
		if (CharacterOwner.CurrentStamina <= 0.0)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(
				new Emote(WeaponComponent.ReadiedStaminaExhaustedEmote, CharacterOwner, Weapon, CharacterOwner),
				flags: OutputFlags.InnerWrap));
			WeaponComponent.Unready(null);
			EndEffect();
			return;
		}

		Owner.Reschedule(this, TimeSpan.FromSeconds(5));
	}

	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	protected override string SpecificEffectType => "ReadiedRangedWeaponDrainStamina";

	public override string Describe(IPerceiver voyeur)
	{
		return WeaponComponent.ReadiedStaminaEffectDescription;
	}
}

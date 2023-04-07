using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class PreparingCoupDeGrace : CharacterActionWithTarget, IAffectProximity
{
	public PreparingCoupDeGrace(ICharacter owner, IMeleeWeapon weapon, IFixedBodypartWeaponAttack attack,
		ICharacter target, IFutureProg applicabilityProg = null)
		: base(owner, target, null, "preparing to coup-de-grace $1", "", "", new[] { "general", "movement" },
			"preparing a coup-de-grace on $1")
	{
		Action = perceivable =>
		{
			if (Attack.UsableAttack(owner, Weapon.Parent, target, Weapon.HandednessForWeapon(owner), false,
				    BuiltInCombatMoveType.CoupDeGrace))
			{
				var coupMove = new CoupDeGrace(Attack, TargetCharacter)
				{
					Assailant = CharacterOwner,
					Weapon = weapon,
					Emote = Emote
				};
				var result = coupMove.ResolveMove(null);
				coupMove.ResolveBloodSpray(result);
			}
		};
		OnStopAction =
			perceivable =>
			{
				Owner.OutputHandler.Handle(
					new EmoteOutput(new Emote($"@ are|is no longer preparing to coup-de-grace $1 with $2", owner,
						owner, target, Weapon.Parent)));
			};
		Weapon = weapon;
		Attack = attack;
		SetupCoupDeGraceEventHandlers();
	}

	public IMeleeWeapon Weapon { get; set; }
	public IFixedBodypartWeaponAttack Attack { get; set; }
	public PlayerEmote Emote { get; set; }

	#region Overrides of TargetedBlockingDelayedAction

	protected override EmoteOutput GetCancelEmote(string emoteString)
	{
		return
			new EmoteOutput(new Emote("@ stop|stops preparing to coup-de-grace $1 with $2", CharacterOwner,
				CharacterOwner, Target, Weapon.Parent));
	}

	protected override EmoteOutput GetWhyCannotMoveEmote(IMove mover)
	{
		return
			new EmoteOutput(new Emote("@ cannot move because $0 $0|are|is about to coup-de-grace $1 with $2", mover,
				Owner, Target, Weapon.Parent));
	}

	protected override void TargetWantsToMove(IPerceivable perceivable, PerceivableRejectionResponse response)
	{
		if (perceivable == Owner)
		{
			response.Reason =
				GetWhyCannotMoveEmote(perceivable == Owner ? CharacterOwner : TargetMover)
					.ParseFor(perceivable == Owner ? CharacterOwner : TargetMover);
			response.Rejected = true;
		}
	}

	protected void SetupCoupDeGraceEventHandlers()
	{
		Weapon.Parent.OnDeath += WeaponDestroyed;
		Weapon.Parent.OnDeleted += WeaponLost;
		Weapon.Parent.OnQuit += WeaponLost;
		CharacterOwner.Body.OnInventoryChange += WeaponInventoryStateChange;
		TargetCharacter.OnJoinCombat += TargetEngagedInCombat;
		CharacterOwner.OnJoinCombat += TargetEngagedInCombat;
	}

	protected override void ReleaseEventHandlers()
	{
		base.ReleaseEventHandlers();
		Weapon.Parent.OnDeath -= WeaponDestroyed;
		Weapon.Parent.OnDeleted -= WeaponLost;
		Weapon.Parent.OnQuit -= WeaponLost;
		CharacterOwner.Body.OnInventoryChange -= WeaponInventoryStateChange;
		TargetCharacter.OnJoinCombat -= TargetEngagedInCombat;
		CharacterOwner.OnJoinCombat -= TargetEngagedInCombat;
	}

	protected void TargetEngagedInCombat(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote(
				$"@ are|is no longer preparing to coup-de-grace $1 because {(perceivable == CharacterOwner ? "$0 $0|have|has" : "$1 $1|have|has")} been engaged in combat!"));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected void WeaponDestroyed(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote("@ are|is no longer preparing to coup-de-grace $1 because $2 has been destroyed!"));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected void WeaponLost(IPerceivable perceivable)
	{
		CharacterOwner.OutputHandler.Handle(
			GetCancelEmote("@ are|is no longer preparing to coup-de-grace $1 because #0 no longer have|has $2"));
		OnStopAction = null;
		CharacterOwner.RemoveEffect(this, true);
	}

	protected void WeaponInventoryStateChange(InventoryState oldState, InventoryState newState, IGameItem item)
	{
		if (item == Weapon.Parent && oldState == InventoryState.Wielded)
		{
			CharacterOwner.OutputHandler.Handle(
				GetCancelEmote(
					"@ are|is no longer preparing to coup-de-grace $1 because #0 are|is no longer wielding $2"));
			OnStopAction = null;
			CharacterOwner.RemoveEffect(this, true);
		}
	}

	#endregion

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Preparing to coup-de-grace {Target.HowSeen(voyeur)} with {Attack.Name.A_An()} from {Weapon.Parent.HowSeen(voyeur)}";
	}

	protected override string SpecificEffectType => "PreparingCoupDeGrace";

	#endregion

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (Target == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}
}
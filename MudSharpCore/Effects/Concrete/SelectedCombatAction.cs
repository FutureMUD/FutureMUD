using System.Linq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class SelectedCombatAction : CombatEffectBase, ISelectedCombatAction
{
	internal abstract class CombatActionType
	{
		public abstract ICombatMove GetCombatMove(ICharacter actor);
		public abstract string Describe(IPerceiver voyeur);
	}

	internal class StruggleAction : CombatActionType
	{
		public override string Describe(IPerceiver voyeur)
		{
			return $"struggle free from their impairment.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new StruggleMove { Assailant = actor };
		}
	}

	internal class BreakoutAction : CombatActionType
	{
		public override string Describe(IPerceiver voyeur)
		{
			return "breaking out from their impairment";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new BreakoutMove(actor);
		}
	}

	internal class ChargeAction : CombatActionType
	{
		public ICharacter Target { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"charge into melee with {Target.HowSeen(voyeur)}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			actor.CombatTarget = Target;
			// TODO - override default melee strategy when charging manually
			return new ChargeToMeleeMove { Assailant = actor };
		}
	}

	internal class MoveToMeleeAction : CombatActionType
	{
		public ICharacter Target { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"move into melee with {Target.HowSeen(voyeur)}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			actor.CombatTarget = Target;
			return new MoveToMeleeMove { Assailant = actor };
		}
	}

	internal class GetAction : CombatActionType
	{
		public IGameItem TargetItem { get; init; }

		public IEmote PlayerEmote { get; set; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"get {TargetItem.HowSeen(voyeur)}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new RetrieveItemMove(actor, TargetItem);
		}
	}

	internal class DrawAction : CombatActionType
	{
		public IWieldable TargetItem { get; init; }

		public IWield SpecificHand { get; init; }

		public IEmote PlayerEmote { get; set; }

		public ItemCanWieldFlags Flags { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return
				$"draw {TargetItem.Parent.HowSeen(voyeur)}{(SpecificHand != null ? $" into their {SpecificHand.FullDescription()}" : "")}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new DrawAndWieldMove
				{ Assailant = actor, Weapon = TargetItem, SpecificHand = SpecificHand, Flags = Flags };
		}
	}

	internal class WieldAction : CombatActionType
	{
		public IWieldable TargetItem { get; init; }

		public IWield SpecificHand { get; init; }

		public IEmote PlayerEmote { get; init; }

		public ItemCanWieldFlags Flags { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"wield {TargetItem.Parent.HowSeen(voyeur)}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new WieldMove
			{
				Assailant = actor, Item = TargetItem, SpecificHand = SpecificHand, PlayerEmote = PlayerEmote,
				Flags = Flags
			};
		}
	}

	internal class TakeCoverAction : CombatActionType
	{
		public CombatantCover Cover { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return
				$"take cover {(Cover.CoverItem != null ? $"behind {Cover.CoverItem.Parent.HowSeen(voyeur)}" : $"using {Cover.Cover.DescriptionString}")}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			if (!actor.CanSpendStamina(TakeCover.MoveStaminaCost(actor)))
			{
				return new TooExhaustedMove { Assailant = actor };
			}

			return new TakeCover
			{
				Assailant = actor,
				Cover = Cover
			};
		}
	}

	internal class RemoveItemAction : CombatActionType
	{
		public IGameItem Item { get; init; }
		public IEmote PlayerEmote { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"removing {Item.HowSeen(voyeur)}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new RemoveItemMove
			{
				Assailant = actor,
				Item = Item,
				PlayerEmote = PlayerEmote
			};
		}
	}

	internal class WearItemAction : CombatActionType
	{
		public IGameItem Item { get; init; }
		public IEmote PlayerEmote { get; init; }
		public string SpecificProfile { get; init; }

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new WearItemMove
			{
				Assailant = actor,
				Item = Item,
				PlayerEmote = PlayerEmote,
				SpecificProfile = SpecificProfile
			};
		}

		public override string Describe(IPerceiver voyeur)
		{
			return $"wearing {Item.HowSeen(voyeur)}.";
		}
	}

	internal class AimItemAction : CombatActionType
	{
		public IPerceiver Target { get; init; }
		public IRangedWeapon Weapon { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"aim {Weapon.Parent.HowSeen(voyeur)} at {Target?.HowSeen(voyeur) ?? "the sky"}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			if (Target == null)
			{
				actor.Aim = new AimInformation(null, actor, Enumerable.Empty<ICellExit>(), Weapon);
				return new AimRangedWeaponMove(actor, null, Weapon);
			}

			if (actor.Aim == null)
			{
				if (actor.Location == Target.Location)
				{
					actor.Aim = new AimInformation(Target, actor, Enumerable.Empty<ICellExit>(), Weapon);
				}
				else
				{
					var path = actor.PathBetween(Target,
						Weapon.WeaponType.DefaultRangeInRooms,
						false, false, true).ToList();
					if (!path.Any())
					{
						actor.Send("Your no longer have a valid aim to your target.");
						return null;
					}

					actor.Aim = new AimInformation(Target, actor, path, Weapon);
				}
			}

			return new AimRangedWeaponMove(actor, Target, Weapon);
		}
	}

	internal class FireItemAction : CombatActionType
	{
		public IPerceiver Target { get; init; }
		public IRangedWeapon Weapon { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"firing {Weapon.Parent.HowSeen(voyeur)} at {Target?.HowSeen(voyeur) ?? "the sky"}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new RangedWeaponAttackMove(actor, Target, Weapon);
		}
	}

	internal class LoadItemAction : CombatActionType
	{
		public IRangedWeapon Weapon { get; init; }
		public required LoadMode LoadMode { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"loading {Weapon.Parent.HowSeen(voyeur)}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new LoadRangedWeaponMove { Assailant = actor, Weapon = Weapon, Mode = LoadMode};
		}
	}

	internal class ReadyItemAction : CombatActionType
	{
		public IRangedWeapon Weapon { get; init; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"readying {Weapon.Parent.HowSeen(voyeur)}.";
		}

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new ReadyRangedWeaponMove { Assailant = actor, Weapon = Weapon };
		}
	}

	internal class SmashItemAction : CombatActionType
	{
		public IGameItem Target { get; init; }
		public IGameItem ParentItem { get; init; }
		public IMeleeWeapon Weapon { get; init; }
		public IWeaponAttack Attack { get; init; }

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new MeleeWeaponSmashItemAttack(Attack)
			{
				Assailant = actor,
				Target = Target,
				ParentItem = ParentItem,
				Weapon = Weapon
			};
		}

		public override string Describe(IPerceiver voyeur)
		{
			return
				$"smashing {Target.HowSeen(voyeur)}{(ParentItem != null ? $"on {ParentItem.HowSeen(voyeur)}" : "")} with {Weapon.Parent.HowSeen(voyeur)}.";
		}
	}

	internal class SmashItemUnarmedAction : CombatActionType
	{
		public IGameItem Target { get; init; }
		public IGameItem ParentItem { get; init; }
		public INaturalAttack Attack { get; init; }

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new UnarmedSmashItemAttack(Attack.Attack)
			{
				Assailant = actor,
				Target = Target,
				ParentItem = ParentItem,
				NaturalAttack = Attack
			};
		}

		public override string Describe(IPerceiver voyeur)
		{
			return
				$"smashing {Target.HowSeen(voyeur)}{(ParentItem != null ? $"on {ParentItem.HowSeen(voyeur)}" : "")} with {Attack.Bodypart.FullDescription()}.";
		}
	}

	internal class RepositionAction : CombatActionType
	{
		public IPositionState TargetState { get; init; }
		public PositionModifier TargetModifier { get; init; }
		public IPerceivable TargetTarget { get; init; }
		public IEmote TargetEmote { get; init; }
		public IEmote Emote { get; init; }

		#region Overrides of CombatActionType

		public override ICombatMove GetCombatMove(ICharacter actor)
		{
			return new RepositionMove
			{
				Assailant = actor,
				TargetState = TargetState,
				TargetModifier = TargetModifier,
				TargetEmote = TargetEmote,
				TargetTarget = TargetTarget,
				Emote = Emote
			};
		}

		public override string Describe(IPerceiver voyeur)
		{
			return $"{TargetState.DefaultDescription()}";
		}

		#endregion
	}

	public static SelectedCombatAction GetEffectStruggle(ICharacter actor)
	{
		return new SelectedCombatAction(actor, new StruggleAction());
	}

	public static SelectedCombatAction GetEffectBreakout(ICharacter actor)
	{
		return new SelectedCombatAction(actor, new BreakoutAction());
	}

	public static SelectedCombatAction GetEffectGetItem(ICharacter actor, IGameItem target, IEmote playerEmote)
	{
		return new SelectedCombatAction(actor, new GetAction
		{
			TargetItem = target,
			PlayerEmote = playerEmote
		});
	}

	public static SelectedCombatAction GetEffectDrawItem(ICharacter actor, IWieldable target, IWield specificHand,
		IEmote playerEmote, ItemCanWieldFlags flags)
	{
		return new SelectedCombatAction(actor, new DrawAction
		{
			TargetItem = target,
			SpecificHand = specificHand,
			PlayerEmote = playerEmote,
			Flags = flags
		});
	}

	public static SelectedCombatAction GetEffectWieldItem(ICharacter actor, IWieldable target, IEmote playerEmote,
		IWield specificHand, ItemCanWieldFlags flags)
	{
		return new SelectedCombatAction(actor, new WieldAction
		{
			TargetItem = target,
			PlayerEmote = playerEmote,
			Flags = flags,
			SpecificHand = specificHand
		});
	}

	public static SelectedCombatAction GetEffectTakeCover(ICharacter actor, CombatantCover cover)
	{
		return new SelectedCombatAction(actor, new TakeCoverAction
		{
			Cover = cover
		});
	}

	public static SelectedCombatAction GetEffectRemoveItem(ICharacter actor, IGameItem item, IEmote playerEmote)
	{
		return new SelectedCombatAction(actor, new RemoveItemAction
		{
			Item = item,
			PlayerEmote = playerEmote
		});
	}

	public static SelectedCombatAction GetEffectWearItem(ICharacter actor, IGameItem item, IEmote playerEmote,
		string specificProfile)
	{
		return new SelectedCombatAction(actor, new WearItemAction
		{
			Item = item,
			PlayerEmote = playerEmote,
			SpecificProfile = specificProfile
		});
	}

	public static SelectedCombatAction GetEffectAimItem(ICharacter actor, IPerceiver target, IRangedWeapon weapon)
	{
		return new SelectedCombatAction(actor, new AimItemAction
		{
			Target = target,
			Weapon = weapon
		});
	}

	public static SelectedCombatAction GetEffectFireItem(ICharacter actor, IPerceiver target, IRangedWeapon weapon)
	{
		return new SelectedCombatAction(actor, new FireItemAction
		{
			Target = target,
			Weapon = weapon
		});
	}

	public static SelectedCombatAction GetEffectLoadItem(ICharacter actor, IRangedWeapon weapon, LoadMode mode)
	{
		return new SelectedCombatAction(actor, new LoadItemAction
		{
			Weapon = weapon,
			LoadMode = mode
		});
	}

	public static SelectedCombatAction GetEffectReadyItem(ICharacter actor, IRangedWeapon weapon)
	{
		return new SelectedCombatAction(actor, new ReadyItemAction
		{
			Weapon = weapon
		});
	}

	public static SelectedCombatAction GetEffectSmashItem(ICharacter actor, IGameItem target, IGameItem parent,
		IMeleeWeapon weapon, IWeaponAttack attack)
	{
		return new SelectedCombatAction(actor, new SmashItemAction
		{
			Weapon = weapon,
			Target = target,
			ParentItem = parent,
			Attack = attack
		});
	}

	public static SelectedCombatAction GetEffectSmashItemUnarmed(ICharacter actor, IGameItem target, IGameItem parent,
		INaturalAttack attack)
	{
		return new SelectedCombatAction(actor, new SmashItemUnarmedAction
		{
			Target = target,
			ParentItem = parent,
			Attack = attack
		});
	}

	public static SelectedCombatAction GetEffectCharge(ICharacter actor, ICharacter target)
	{
		return new SelectedCombatAction(actor, new ChargeAction
		{
			Target = target
		});
	}

	public static SelectedCombatAction GetEffectMoveToMelee(ICharacter actor, ICharacter target)
	{
		return new SelectedCombatAction(actor, new MoveToMeleeAction
		{
			Target = target
		});
	}

	public static SelectedCombatAction GetEffectReposition(
		ICharacter actor, IPositionState state, PositionModifier modifier, IPerceivable target, IEmote emote,
		IEmote pmote)
	{
		return new SelectedCombatAction(actor, new RepositionAction
		{
			Emote = emote,
			TargetEmote = pmote,
			TargetModifier = modifier,
			TargetState = state,
			TargetTarget = target
		});
	}

	private readonly CombatActionType _action;

	private SelectedCombatAction(ICharacter owner, CombatActionType action, IFutureProg applicabilityProg = null) :
		base(owner, owner.Combat, applicabilityProg)
	{
		_action = action;
	}

	public ICombatMove GetMove(ICharacter actor)
	{
		return _action.GetCombatMove(actor);
	}

	public bool ShouldRemove(CharacterState state)
	{
		return !CharacterState.Able.HasFlag(state);
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Choosing to manually {_action.Describe(voyeur).Colour(Telnet.Cyan)}.";
	}

	protected override string SpecificEffectType { get; } = "SelectedCombatAction";

	#endregion
}
#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.FutureProg;

namespace MudSharp.Magic.Powers;

public sealed class MagicSmashPower : MagicAttackPower, IMagicSmashPower
{
	public override string PowerType => "Magic Smash";
	public override string DatabaseType => "magicsmash";
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.MagicPowerSmashItem;
	protected override bool TargetsItems => true;
	protected override ProgVariableTypes InvocationTargetType => ProgVariableTypes.Item;
	public new static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("magicsmash", (model, world) => new MagicSmashPower(model, world));
		MagicPowerFactory.RegisterBuilderLoader("magicsmash", (world, school, name, actor, command) =>
		{
			var trait = world.Traits.GetByIdOrName(command.PopSpeech());
			var attack = world.WeaponAttacks.GetByIdOrName(command.SafeRemainingArgument);
			if (trait is null || attack is null) { actor.Send("Specify a casting trait and weapon attack profile."); return null; }
			return new MagicSmashPower(world, school, name, trait, attack);
		});
	}
	private MagicSmashPower(Models.MagicPower model, IFuturemud world) : base(model, world) { }
	private MagicSmashPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait, IWeaponAttack attack)
		: base(world, school, name, trait, attack)
	{
		Blurb = "Smash an item with magical force";
		_showHelpText = "Use this power's verb and a visible item. In combat the smash is queued; outside combat it resolves with a recovery delay.";
		Changed = true;
	}
	public override bool CanInvokePower(ICharacter invoker, ICharacter target) => false;
	public bool CanInvokePower(ICharacter actor, IGameItem target)
	{
		if (target is null || target.Destroyed || target.Deleted || !actor.Powers.Contains(this) ||
		    !actor.State.IsAble() || actor.Movement is not null || actor.IsBlocked("general").Truth ||
		    actor.EffectsOfType<CommandDelay>().Any(x => x.IsDelayed("magicsmash")) ||
		    !actor.CanSee(target) || !actor.CanSpendStamina(StaminaCost) || !HasValidAttackSpell ||
		    !CanAffordToInvokePower(actor, Verbs.First()).Truth || (!DealsDamage && AttackSpell is null) ||
		    target.InInventoryOf is not null && target.InInventoryOf != actor.Body) return false;
		if (WeaponAttack.RequiredPositionStates.Any() && !WeaponAttack.RequiredPositionStates.Contains(actor.PositionState)) return false;
		if (CanInvokePowerProg?.ExecuteBool(actor, target) == false || WeaponAttack.UsabilityProg?.ExecuteBool(actor, null, target) == false) return false;
		return target.InInventoryOf == actor.Body || (AttackRange == MagicAttackRange.Ranged
			? NaturalRangedAttackMoveBase.TargetIsInRange(actor, target, RangeInRooms) : actor.ColocatedWith(target));
	}
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var target = actor.TargetItem(command.SafeRemainingArgument) ?? actor.SeenTargets.OfType<IGameItem>()
			.Where(x => actor.CanSee(x)).GetFromItemListByKeyword(command.SafeRemainingArgument, actor);
		if (target is null || !CanInvokePower(actor, target))
		{ actor.Send("You cannot smash that item with this power at present."); return; }
		if (actor.Combat is not null)
		{
			actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectMagicSmash(actor, target, this));
			actor.Send($"You prepare to smash {target.HowSeen(actor)} with {Name.ColourName()}."); return;
		}
		var move = new MagicPowerSmashItemMove(actor, target, this);
		var result = move.ResolveMove(null!);
		if (!move.UsesStaminaWithResult(result)) return;
		actor.SpendStamina(move.StaminaCost);
		actor.AddEffect(new CommandDelay(actor, "magicsmash"), TimeSpan.FromSeconds(Math.Max(1, BaseDelay)));
	}
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("rider"))
		{ actor.Send("Character control riders do not apply to item attacks. Configure an attackitem spell payload instead."); return false; }
		return base.BuildingCommand(actor, command);
	}
}

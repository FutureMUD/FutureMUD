#nullable enable

using MudSharp.Health;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public sealed class BackstabAttackMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target)
	: MeleeWeaponAttack(owner, weapon, attack, target, physicalGuard: () =>
		BackstabCombat.HasProficiency(owner, attack))
{
	protected override ITraitDefinition AttackCheckTrait => ((IBackstabAttack)Attack).ProficiencyTrait ?? base.AttackCheckTrait;
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.BackstabAttack;
	public override string Description => "Backstabbing with a weapon";
	public override bool UsesStaminaWithResult(CombatMoveResult result) =>
		!ReferenceEquals(result, CombatMoveResult.Irrelevant) && base.UsesStaminaWithResult(result);
	protected override double TargetPenetrationBonus => ((IBackstabAttack)Attack).PenetrationBonus;
	protected override IDamage PrepareTargetDamage(IDamage damage) =>
		BackstabCombat.ModifyDamage((IBackstabAttack)Attack, damage);
	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		var error = "This backstab is not correctly configured.";
		if (Attack is not IBackstabAttack || !BackstabCombat.Validate(Assailant, PrimaryCharacterTarget!, new(Attack, Weapon, null), out error))
		{
			RejectUnexecutedCommand();
			Assailant.OutputHandler.Send(Attack is IBackstabAttack ? error : "This backstab is not correctly configured.");
			return CombatMoveResult.Irrelevant;
		}
		var result = base.ResolveMove(defenderMove);
		BackstabCombat.ApplyEffects(Assailant, PrimaryCharacterTarget!, (IBackstabAttack)Attack, result);
		return result;
	}
}

public sealed class BackstabAttackUnarmedMove(ICharacter owner, INaturalAttack attack, ICharacter target)
	: NaturalAttackMove(owner, attack, target, () => BackstabCombat.HasProficiency(owner, attack.Attack))
{
	protected override ITraitDefinition AttackCheckTrait => ((IBackstabAttack)Attack).ProficiencyTrait;
	public override CheckType Check => ((IBackstabAttack)Attack).ProficiencyTraitId.HasValue ? CheckType.CombatMoveCheck : base.Check;
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.BackstabAttackUnarmed;
	public override string Description => "Backstabbing with a natural weapon";
	public override bool UsesStaminaWithResult(CombatMoveResult result) =>
		!ReferenceEquals(result, CombatMoveResult.Irrelevant) && base.UsesStaminaWithResult(result);
	protected override double TargetPenetrationBonus => ((IBackstabAttack)Attack).PenetrationBonus;
	protected override IDamage PrepareTargetDamage(IDamage damage) =>
		BackstabCombat.ModifyDamage((IBackstabAttack)Attack, damage);
	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		using var execution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		var error = "This backstab is not correctly configured.";
		if (Attack is not IBackstabAttack || !BackstabCombat.Validate(Assailant, PrimaryCharacterTarget!, new(Attack, null, NaturalAttack), out error))
		{
			RejectUnexecutedCommand();
			Assailant.OutputHandler.Send(Attack is IBackstabAttack ? error : "This backstab is not correctly configured.");
			return CombatMoveResult.Irrelevant;
		}
		var result = base.ResolveMove(defenderMove);
		BackstabCombat.ApplyEffects(Assailant, PrimaryCharacterTarget!, (IBackstabAttack)Attack, result);
		return result;
	}
}

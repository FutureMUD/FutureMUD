#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public sealed class DualWieldDoubleAttackMove : CombatMoveBase, ICombatMoveSequence
{
	private readonly IBody _actorBody;
	private readonly IBody _targetBody;
	private readonly (IMeleeWeapon Weapon, IWeaponAttack Attack)[] _plans;
	private readonly List<CombatMoveResolution> _resolutions = [];
	private bool _started;
	private double _delay;

	public DualWieldDoubleAttackMove(ICharacter actor, ICharacter target, IMeleeWeapon firstWeapon,
		IWeaponAttack firstAttack, IMeleeWeapon secondWeapon, IWeaponAttack secondAttack)
	{
		if (ReferenceEquals(firstWeapon.Parent, secondWeapon.Parent)) throw new ArgumentException("Two distinct weapons are required.");
		Assailant = actor;
		Target = target;
		OriginalCombat = actor.Combat;
		_actorBody = actor.Body;
		_targetBody = target.Body;
		_characterTargets.Clear(); _targets.Clear(); PrimaryTarget = target;
		_plans = [(firstWeapon, firstAttack), (secondWeapon, secondAttack)];
		_delay = Math.Max(firstAttack.BaseDelay, secondAttack.BaseDelay);
	}

	public ICombat OriginalCombat { get; }
	public ICharacter Target { get; }
	public double StaminaBudget { get; private set; }
	public IReadOnlyList<ICombatMove> Children { get; private set; } = [];
	public IReadOnlyList<CombatMoveResolution> Resolutions => _resolutions;
	public override string Description => "Attacking with both wielded weapons";
	public override CheckType Check => CheckType.DualWieldCoordinationCheck;
	public override double BaseDelay => _delay;
	public override bool UsesStaminaWithResult(CombatMoveResult result) => false;
	public override CombatMoveResult ResolveMove(ICombatMove defenderMove) =>
		OriginalCombat is ICombatSequenceResolver resolver ? resolver.ResolveSequence(this) : CombatMoveResult.Irrelevant;

	internal bool PhysicalReady() =>
		!ReferenceEquals(Assailant, Target) &&
		ReferenceEquals(Assailant.Body, _actorBody) && ReferenceEquals(Target.Body, _targetBody) &&
		ReferenceEquals(Assailant.Combat, OriginalCombat) && ReferenceEquals(Target.Combat, OriginalCombat) &&
		OriginalCombat is not null && OriginalCombat.Combatants.Any(x => ReferenceEquals(x, Assailant)) &&
		OriginalCombat.Combatants.Any(x => ReferenceEquals(x, Target)) &&
		Assailant.State.IsAble() && Target.State.IsAble() && ReferenceEquals(Assailant.CombatTarget, Target) &&
		Assailant.Movement is null && Target.Movement is null && Assailant.RidingMount?.Movement is null &&
		!Assailant.Effects.Any(x => x.IsBlockingEffect("combat") || x.IsBlockingEffect("general") || x is ClinchEffect or IGrappling or IBeingGrappled) &&
		!Target.Effects.Any(x => x is ClinchEffect or IGrappling or IBeingGrappled) &&
		Assailant.MeleeRange && Assailant.ColocatedWith(Target) &&
		_plans.All(x => DualWieldCombat.WeaponReady(Assailant, x.Weapon));

	private bool Validate((IMeleeWeapon Weapon, IWeaponAttack Attack) plan)
	{
		if (!PhysicalReady() || !DualWieldCombat.EligibleProfile(plan.Attack) ||
		    !Assailant.CombatSettings.ClassificationsAllowed.Contains(plan.Weapon.Classification)) return false;
		if (!CombatStrategyFactory.GetStrategy(Assailant.CombatStrategyMode).WillAttack(Assailant, Target) ||
		    !VehicleCombatService.Instance.CanCrossVehicleBoundary(Assailant, Target, false, false, out _) ||
		    !plan.Attack.UsableAttack(Assailant, plan.Weapon.Parent, Target,
			    plan.Weapon.HandednessForWeapon(Assailant), false, BuiltInCombatMoveType.UseWeaponAttack)) return false;
		return CommandExecutionScope.TryContinue() && PhysicalReady();
	}

	public bool TryPrepare()
	{
		if (_started) return false;
		_started = true;
		if (!DualWieldCombat.Enabled(Gameworld) || !_plans.All(Validate)) return false;
		StaminaBudget = _plans.Sum(x => MeleeWeaponAttack.MoveStaminaCost(Assailant, x.Attack));
		if (!double.IsFinite(StaminaBudget) || StaminaBudget < 0 || !Assailant.CanSpendStamina(StaminaBudget) || !PhysicalReady()) return false;
		var check = Gameworld.GetCheck(Check);
		var difficulty = (Difficulty)DualWieldCombat.Integer(Gameworld, "DualWieldCoordinationDifficulty", 5, 0, 10);
		var outcome = check.Check(Assailant, difficulty, (ITraitDefinition)null!, Target).Outcome;
		if (outcome is Outcome.None or Outcome.NotTested || !CommandExecutionScope.TryContinue() || !PhysicalReady()) return false;
		var stages = DualWieldCombat.Stages(Gameworld, outcome);
		Children = _plans.Select(x => (ICombatMove)new MeleeWeaponAttack(Assailant, x.Weapon, x.Attack, Target,
			stages, PhysicalReady)).ToArray();
		_delay *= DualWieldCombat.Number(Gameworld, "DualWieldDoubleAttackRecoveryMultiplier", 1.5);
		return true;
	}

	public bool CanExecute(ICombatMove child)
	{
		var index = Children.ToList().IndexOf(child);
		return index >= 0 && Validate(_plans[index]) &&
		       Assailant.CanSpendStamina(child.StaminaCost) && PhysicalReady();
	}

	public void RecordResolution(CombatMoveResolution resolution) => _resolutions.Add(resolution);

	public CombatMoveResult CompleteResolution()
	{
		var executed = _resolutions.Where(x => x.Executed).ToArray();
		if (executed.Length == 0) return CombatMoveResult.Irrelevant;
		if (executed.Length == 1) _delay = executed[0].Move.BaseDelay;
		return new CombatMoveResult
		{
			MoveWasSuccessful = executed.Any(x => x.Result.MoveWasSuccessful),
			RecoveryDifficulty = executed.Max(x => x.Result.RecoveryDifficulty),
			AttackerOutcome = executed[0].Result.AttackerOutcome,
			DefenderOutcome = executed[0].Result.DefenderOutcome,
			WoundsCaused = executed.SelectMany(x => x.Result.WoundsCaused).ToArray(),
			SelfWoundsCaused = executed.SelectMany(x => x.Result.SelfWoundsCaused).ToArray()
		};
	}
}

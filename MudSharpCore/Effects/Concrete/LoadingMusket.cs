using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class UnjammingGun : CharacterActionWithTargetAndTool
{
	private readonly CommandExecutionScope.Continuation _commandContinuation;
    public static TimeSpan EffectDuration(ICharacter actor, IJammableWeapon Weapon, IGameItem ramrod)
    {
        ICheck check = actor.Gameworld.GetCheck(CheckType.UnjamGun);
        Difficulty difficulty = Difficulty.Normal;
        if (actor.Combat is not null)
        {
            difficulty = difficulty.StageUp(2);
        }

        if (actor.IsEngagedInMelee)
        {
            difficulty = difficulty.StageUp(2);
        }

        CheckOutcome outcome = check.Check(actor, difficulty, Weapon.Parent, Weapon);
        TraitExpression te = new(actor.Gameworld.GetStaticConfiguration("UnjammingGunDurationExpression"), actor.Gameworld);
        return TimeSpan.FromSeconds(te.EvaluateWith(actor, Weapon.WeaponType.OperateTrait,
            TraitBonusContext.UnjamGunDuration,
            ("degrees", outcome.CheckDegrees()),
            ("faildegrees", outcome.FailureDegrees()),
            ("successdegrees", outcome.SuccessDegrees()),
            ("difficulty", (int)difficulty)));
    }

    public IJammableWeapon Weapon { get; private set; }
    public IGameItem Ramrod { get; private set; }
    public DesiredItemState RamrodFinalisationState { get; private set; }

    /// <inheritdoc />
    public UnjammingGun(ICharacter owner, IJammableWeapon target, IGameItem ramrod, DesiredItemState originalState) : base(owner, target.Parent, [(ramrod, DesiredItemState.Held)])
    {
		_commandContinuation = CommandExecutionScope.CaptureContinuation(owner);
        Weapon = target;
        Ramrod = ramrod;
        RamrodFinalisationState = originalState;
        WhyCannotMoveEmoteString = "@ cannot move because #0 %0|are|is unjamming $1.";
        LDescAddendum = "unjamming $1";
        _blocks.Add("general");
        _blocks.Add("movement");
        ActionDescription = $"unjamming $1";
    }

    #region Overrides of CharacterAction

    public override void InitialEffect()
    {
		using var execution = _commandContinuation.Enter();
		if (!CommandExecutionScope.TryContinue()) return;
        CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Weapon.StartUnjamEmote, CharacterOwner, CharacterOwner, Weapon.Parent, Ramrod)));
    }

    /// <inheritdoc />
    public override void ExpireEffect()
    {
		using var execution = _commandContinuation.Enter();
		if (!CommandExecutionScope.TryContinue()) { Owner.RemoveEffect(this, true); return; }
        ICheck check = Gameworld.GetCheck(CheckType.UnjamGun);
        Difficulty difficulty = Difficulty.Normal;
        if (CharacterOwner.Combat is not null)
        {
            difficulty = difficulty.StageUp(2);
        }

        if (CharacterOwner.IsEngagedInMelee)
        {
            difficulty = difficulty.StageUp(2);
        }

        CheckOutcome outcome = check.Check(CharacterOwner, difficulty, Weapon.Parent, Weapon);
		if (!CommandExecutionScope.TryContinue()) { Owner.RemoveEffect(this, true); return; }
        if (outcome.IsFail())
        {
            CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Weapon.FailUnjamEmote, CharacterOwner, CharacterOwner, Weapon.Parent, Ramrod)));
			if (!CommandExecutionScope.TryContinue()) { Owner.RemoveEffect(this, true); return; }
			var duration = EffectDuration(CharacterOwner, Weapon, Ramrod);
			if (!CommandExecutionScope.TryContinue()) { Owner.RemoveEffect(this, true); return; }
			Owner.Reschedule(this, duration);
            return;
        }

        CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(Weapon.FinishUnjamEmote, CharacterOwner, CharacterOwner, Weapon.Parent, Ramrod)));
		if (!CommandExecutionScope.TryContinue()) { Owner.RemoveEffect(this, true); return; }
		CommandExecutionScope.MarkCommitted();
        Weapon.IsJammed = false;
        Owner.RemoveEffect(this, true);
    }
    #endregion
}

public class LoadingMusket : CharacterActionWithTarget
{
	private readonly CommandExecutionScope.Continuation _commandContinuation;
    public IRangedWeapon Weapon { get; private set; }

    public LoadMode LoadMode { get; private set; }

    public LoadingMusket(ICharacter owner, IRangedWeapon weapon, LoadMode loadMode) : base(owner, weapon.Parent)
    {
		_commandContinuation = CommandExecutionScope.CaptureContinuation(owner);
        Weapon = weapon;
        LoadMode = loadMode;
        WhyCannotMoveEmoteString = "@ cannot move because #0 %0|are|is loading $1";
        ActionDescription = "loading $1";
		LDescAddendum = "loading $1";
		_blocks.Add("general");
		_blocks.Add("movement");
    }

    protected override string SpecificEffectType => "LoadingMusket";

    public override string Describe(IPerceiver voyeur)
    {
        return $"Loading {Weapon.Parent.HowSeen(voyeur)} in the {LoadMode.DescribeEnum().ColourName()} mode";
    }

    #region Overrides of Effect

    /// <inheritdoc />
    public override void ExpireEffect()
    {
		using var execution = _commandContinuation.Enter();
        base.ExpireEffect();
		if (!CommandExecutionScope.TryContinue()) return;
        if (Weapon.LoadStage < 4)
        {
            Weapon.Load((ICharacter)Owner, false, LoadMode);
        }
    }

    #endregion
}

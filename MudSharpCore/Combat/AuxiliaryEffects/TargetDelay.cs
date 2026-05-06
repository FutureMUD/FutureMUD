using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.AuxiliaryEffects;

#nullable enable

internal sealed class TargetDelay : OpposedAuxiliaryEffectBase
{
	public TargetDelay(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
	}

	private TargetDelay(IFuturemud gameworld, MudSharp.Body.Traits.ITraitDefinition defenseTrait,
		Difficulty defenseDifficulty) : base(gameworld, defenseTrait, defenseDifficulty, 1.5, 0.5, 6.0)
	{
	}

	protected override string TypeName => "targetdelay";
	protected override string EffectName => "Target Delay";
	protected override string AmountName => "Delay";
	protected override string AmountUnit => "s";
	protected override double DefaultFlatAmount => 1.5;
	protected override double DefaultPerDegreeAmount => 0.5;
	protected override double DefaultMaximumAmount => 6.0;

	public static void RegisterTypeHelp()
	{
		AuxiliaryCombatAction.RegisterBuilderParser("targetdelay", (action, actor, command) =>
		{
			return !TryParseDefenseArguments(action, actor, command, out var trait, out var difficulty)
				? null
				: new TargetDelay(actor.Gameworld, trait, difficulty);
		},
			@"The Target Delay effect makes a successful auxiliary move delay the target's next combat action.

The syntax to create this type is as follows:

	#3auxiliary set add targetdelay [defense trait] [difficulty]#0

If omitted, the defense trait defaults to the auxiliary action's check trait and difficulty defaults to Normal. The default delay is 1.5 seconds plus 0.5 seconds per opposed success degree, capped at 6 seconds.",
			true);
	}

	public override XElement Save()
	{
		var root = new XElement("Effect");
		SaveCommonAttributes(root);
		return root;
	}

	public override string DescribeForShow(ICharacter actor)
	{
		return $"{EffectName} | {DescribeCommon(actor)}";
	}

	public override void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome)
	{
		if (target is not ICharacter tch)
		{
			return;
		}

		if (!TryGetOpposedSuccess(attacker, target, outcome, out var opposed))
		{
			SendEcho(FailureEcho, attacker, tch);
			return;
		}

		var delay = CalculateAmount(opposed);
		if (delay <= 0.0)
		{
			return;
		}

		Gameworld.Scheduler.DelayScheduleType(tch, ScheduleType.Combat,
			System.TimeSpan.FromSeconds(delay * CombatBase.CombatSpeedMultiplier));
		SendEcho(SuccessEcho, attacker, tch);
	}
}

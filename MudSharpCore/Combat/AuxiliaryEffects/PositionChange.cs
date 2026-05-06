using System;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.AuxiliaryEffects;

#nullable enable

internal sealed class PositionChange : OpposedAuxiliaryEffectBase
{
	private IPositionState Position { get; set; }
	private bool UseKnockdown { get; set; }

	public PositionChange(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		Position = PositionState.GetState(root.Attribute("position")?.Value ?? PositionSprawled.Instance.Id.ToString()) ??
			PositionSprawled.Instance;
		UseKnockdown = root.Attribute("knockdown")?.Value.Equals("true", StringComparison.OrdinalIgnoreCase) != false;
	}

	private PositionChange(IFuturemud gameworld, MudSharp.Body.Traits.ITraitDefinition defenseTrait,
		Difficulty defenseDifficulty) : base(gameworld, defenseTrait, defenseDifficulty, 1.5, 0.5, 5.0)
	{
		Position = PositionSprawled.Instance;
		UseKnockdown = true;
		SuccessEcho = "$0 send|sends $1 sprawling off balance.";
	}

	protected override string TypeName => "positionchange";
	protected override string EffectName => "Position Change";
	protected override string AmountName => "Delay";
	protected override string AmountUnit => "s";
	protected override double DefaultFlatAmount => 1.5;
	protected override double DefaultPerDegreeAmount => 0.5;
	protected override double DefaultMaximumAmount => 5.0;

	public static void RegisterTypeHelp()
	{
		AuxiliaryCombatAction.RegisterBuilderParser("positionchange", (action, actor, command) =>
		{
			return !TryParseDefenseArguments(action, actor, command, out var trait, out var difficulty)
				? null
				: new PositionChange(actor.Gameworld, trait, difficulty);
		},
			@"The Position Change effect changes the target's combat position on a successful opposed auxiliary move.

The syntax to create this type is as follows:

	#3auxiliary set add positionchange [defense trait] [difficulty]#0

If omitted, the defense trait defaults to the auxiliary action's check trait and difficulty defaults to Normal. The default performs the existing combat knockdown and delays the target for 1.5 seconds plus 0.5 seconds per opposed success degree, capped at 5 seconds.",
			true);
	}

	public override XElement Save()
	{
		var root = new XElement("Effect",
			new XAttribute("position", Position.Id),
			new XAttribute("knockdown", UseKnockdown));
		SaveCommonAttributes(root);
		return root;
	}

	public override string DescribeForShow(ICharacter actor)
	{
		return $"{EffectName} | {(UseKnockdown ? "Knockdown".ColourValue() : Position.DescribeLocationMovementParticiple.ColourValue())} | {DescribeCommon(actor)}";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "knockdown":
			case "trip":
				UseKnockdown = !UseKnockdown;
				actor.OutputHandler.Send($"This effect will {UseKnockdown.NowNoLonger()} use the stock combat knockdown.");
				return true;
			case "position":
			case "state":
				return BuildingCommandPosition(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandPosition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which position state should this effect force the target into?");
			return false;
		}

		var state = PositionState.GetState(command.SafeRemainingArgument);
		if (state is null)
		{
			actor.OutputHandler.Send("There is no such position state.");
			return false;
		}

		Position = state;
		UseKnockdown = false;
		actor.OutputHandler.Send($"This effect will now force the target to be {state.DefaultDescription().ColourValue()}.");
		return true;
	}

	protected override void AppendSpecificShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Use Combat Knockdown: {UseKnockdown.ToColouredString()}");
		sb.AppendLine($"Fallback Position: {Position.DescribeLocationMovementParticiple.ColourValue()}");
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

		if (UseKnockdown)
		{
			tch.DoCombatKnockdown();
		}
		else if (tch.CanMovePosition(Position, true))
		{
			tch.SetPosition(Position, PositionModifier.None, null, null);
		}

		var delay = CalculateAmount(opposed);
		if (delay > 0.0)
		{
			Gameworld.Scheduler.DelayScheduleType(tch, ScheduleType.Combat,
				TimeSpan.FromSeconds(delay * CombatBase.CombatSpeedMultiplier));
		}

		SendEcho(SuccessEcho, attacker, tch);
	}
}

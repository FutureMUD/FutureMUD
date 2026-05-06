using System;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.AuxiliaryEffects;

#nullable enable

internal sealed class FacingChange : OpposedAuxiliaryEffectBase
{
	private enum FacingSubject
	{
		Attacker,
		Target
	}

	private enum FacingDirection
	{
		Improve,
		Worsen
	}

	private FacingSubject Subject { get; set; }
	private FacingDirection Direction { get; set; }

	public FacingChange(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		Subject = root.Attribute("subject")?.Value.TryParseEnum(out FacingSubject subject) == true
			? subject
			: FacingSubject.Attacker;
		Direction = root.Attribute("direction")?.Value.TryParseEnum(out FacingDirection direction) == true
			? direction
			: FacingDirection.Improve;
	}

	private FacingChange(IFuturemud gameworld, MudSharp.Body.Traits.ITraitDefinition defenseTrait,
		Difficulty defenseDifficulty) : base(gameworld, defenseTrait, defenseDifficulty, 1.0, 0.0, 1.0)
	{
		Subject = FacingSubject.Attacker;
		Direction = FacingDirection.Improve;
	}

	protected override string TypeName => "facing";
	protected override string EffectName => "Facing Change";
	protected override string AmountName => "Facing Steps";
	protected override double DefaultFlatAmount => 1.0;
	protected override double DefaultPerDegreeAmount => 0.0;
	protected override double DefaultMaximumAmount => 1.0;

	public static void RegisterTypeHelp()
	{
		AuxiliaryCombatAction.RegisterBuilderParser("facing", (action, actor, command) =>
		{
			return !TryParseDefenseArguments(action, actor, command, out var trait, out var difficulty)
				? null
				: new FacingChange(actor.Gameworld, trait, difficulty);
		},
			@"The Facing effect changes combat facing on a successful opposed auxiliary move.

The syntax to create this type is as follows:

	#3auxiliary set add facing [defense trait] [difficulty]#0

If omitted, the defense trait defaults to the auxiliary action's check trait and difficulty defaults to Normal. The default improves the attacker's facing by one step, usually from front to flank.",
			true);
	}

	public override XElement Save()
	{
		var root = new XElement("Effect",
			new XAttribute("subject", Subject),
			new XAttribute("direction", Direction));
		SaveCommonAttributes(root);
		return root;
	}

	public override string DescribeForShow(ICharacter actor)
	{
		return $"{EffectName} | {Subject.DescribeEnum()} {Direction.DescribeEnum()} | {DescribeCommon(actor)}";
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "subject":
			case "target":
			case "who":
				return BuildingCommandSubject(actor, command);
			case "direction":
			case "mode":
				return BuildingCommandDirection(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSubject(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out FacingSubject subject))
		{
			actor.OutputHandler.Send("Do you want this facing change to affect the attacker or the target?");
			return false;
		}

		Subject = subject;
		actor.OutputHandler.Send($"This effect will now affect the {Subject.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDirection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out FacingDirection direction))
		{
			actor.OutputHandler.Send("Do you want this effect to improve or worsen facing?");
			return false;
		}

		Direction = direction;
		actor.OutputHandler.Send($"This effect will now {Direction.DescribeEnum().ToLowerInvariant().ColourValue()} facing.");
		return true;
	}

	protected override void AppendSpecificShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Subject: {Subject.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Direction: {Direction.DescribeEnum().ColourValue()}");
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

		var steps = Math.Max(1, (int)Math.Round(CalculateAmount(opposed), MidpointRounding.AwayFromZero));
		var actor = Subject == FacingSubject.Attacker ? attacker : tch;
		var defender = Subject == FacingSubject.Attacker ? tch : attacker;
		for (var i = 0; i < steps; i++)
		{
			if (Direction == FacingDirection.Improve)
			{
				CombatPositioningUtilities.ImproveCombatPosition(actor, defender);
				continue;
			}

			CombatPositioningUtilities.WorsenCombatPosition(actor, defender);
		}

		SendEcho(SuccessEcho, attacker, tch);
	}
}

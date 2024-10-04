using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using MudSharp.Character;

namespace MudSharp.Combat;

public abstract class CombatAction : SaveableItem
{
	public virtual IEnumerable<string> Keywords => new[] { Name };
	public abstract string ActionTypeName { get; }

	public BuiltInCombatMoveType MoveType { get; set; }
	public CombatMoveIntentions Intentions { get; set; }
	public Difficulty RecoveryDifficultyFailure { get; set; }
	public Difficulty RecoveryDifficultySuccess { get; set; }
	public ExertionLevel ExertionLevel { get; set; }
	public double StaminaCost { get; set; }
	public double BaseDelay { get; set; }
	public double Weighting { get; set; }
	public IFutureProg UsabilityProg { get; set; }
	protected readonly List<IPositionState> _requiredPositionStates = new();
	public IEnumerable<IPositionState> RequiredPositionStates => _requiredPositionStates;

	public abstract string HelpText { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "position":
			case "pos":
				return BuildingCommandPosition(actor, command);
			case "weight":
				return BuildingCommandWeight(actor, command);
			case "exertion":
				return BuildingCommandExertion(actor, command);
			case "recover":
				return BuildingCommandRecover(actor, command);
			case "stamina":
				return BuildingCommandStamina(actor, command);
			case "prog":
			case "futureprog":
			case "usability":
			case "usabilityprog":
			case "usability prog":
			case "usability futureprog":
				return BuildingCommandProg(actor, command);
			case "intention":
				return BuildingCommandIntention(actor, command);
			case "delay":
			case "speed":
				return BuildingCommandDelay(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandPosition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must enter a position state to toggle. The valid position states are: {PositionState.States.OrderBy(x => x.DescribeLocationMovementParticiple).Select(x => x.DescribeLocationMovementParticiple.TitleCase().ColourValue()).ListToString()}.");
			return false;
		}

		var state = PositionState.GetState(command.SafeRemainingArgument);
		if (state == null)
		{
			actor.OutputHandler.Send(
				$"There is no such position state. The valid position states are: {PositionState.States.OrderBy(x => x.DescribeLocationMovementParticiple).Select(x => x.DescribeLocationMovementParticiple.TitleCase().ColourValue()).ListToString()}.");
			return false;
		}

		if (_requiredPositionStates.Contains(state))
		{
			_requiredPositionStates.Remove(state);
			actor.OutputHandler.Send(
				$"The {state.DescribeLocationMovementParticiple.TitleCase().ColourValue()} position is no longer a valid position for this {ActionTypeName}.{(_requiredPositionStates.Count == 0 ? "Warning: There are no valid position states for this attack.".Colour(Telnet.Red) : "")}");
		}
		else
		{
			_requiredPositionStates.Add(state);
			actor.OutputHandler.Send(
				$"The {state.DescribeLocationMovementParticiple.TitleCase().ColourValue()} position is now a valid position for this {ActionTypeName}.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"How many seconds of delay should this {ActionTypeName} have as a base?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var delay))
		{
			actor.OutputHandler.Send($"You must enter a number of seconds delay for this {ActionTypeName}.");
			return false;
		}

		BaseDelay = delay;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {ActionTypeName} will now be delayed by {delay.ToString("N2", actor).Colour(Telnet.Green)} seconds as a base.");
		return true;
	}

	private bool BuildingCommandIntention(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an intention or list or intentions to toggle.");
			return false;
		}

		var intentions = Intentions;
		while (!command.IsFinished)
		{
			if (!CombatExtensions.TryParseCombatMoveIntention(command.PopSpeech(), out var value))
			{
				actor.OutputHandler.Send($"There is no such combat intention as {command.Last}.");
				return false;
			}

			if (intentions.HasFlag(value))
			{
				intentions &= ~value;
			}
			else
			{
				intentions |= value;
			}
		}

		Intentions = intentions;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {ActionTypeName} now has the following intention flags: {Intentions.GetFlags().OfType<CombatMoveIntentions>().Select(x => x.Describe().Colour(Telnet.Green)).ListToString()}.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can either specify a prog to control usability of this {ActionTypeName}, or {"none".ColourCommand()} to clear an existing one.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			UsabilityProg = null;
			actor.OutputHandler.Send($"This {ActionTypeName} will no longer use any prog to control its usability (it will always be valid).");
			Changed = true;
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (prog.ReturnType != FutureProgVariableTypes.Boolean)
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean (truth) value. The selected prog ({prog.FunctionName}) returns {prog.ReturnType.Describe().Colour(Telnet.Cyan)}.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
				{ FutureProgVariableTypes.Character, FutureProgVariableTypes.Item, FutureProgVariableTypes.Character }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that accepts parameters character (attacker), item (weapon), character (target).");
			return false;
		}

		UsabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {ActionTypeName} will now use the prog {UsabilityProg.FunctionName.Colour(Telnet.Cyan)} to control whether it is a valid {ActionTypeName} or not.");
		return true;
	}

	private bool BuildingCommandStamina(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"How much base stamina cost should this {ActionTypeName} incur?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("That is not a valid amount of stamina to use.");
			return false;
		}

		StaminaCost = value;
		Changed = true;
		actor.OutputHandler.Send($"This {ActionTypeName} will now use {StaminaCost.ToString("N2", actor)} stamina.");
		return true;
	}

	private bool BuildingCommandRecover(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must specify a difficulty for recovery for a successful and unsuccessful {ActionTypeName}. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var success))
		{
			actor.OutputHandler.Send(
				"You must enter a valid difficulty for success. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a difficulty for recovery for a successful and unsuccessful {ActionTypeName}. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var failure))
		{
			actor.OutputHandler.Send(
				"You must enter a valid difficulty for failure. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		RecoveryDifficultySuccess = success;
		RecoveryDifficultyFailure = failure;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {ActionTypeName} is now rated at {success.Describe().Colour(Telnet.Green)} difficulty to recover from when successful, and {failure.Describe().Colour(Telnet.Green)} when unsuccessful.");
		return true;
	}

	private bool BuildingCommandExertion(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What minimum exertion level should be set on the attacker after using this {ActionTypeName}?");
			return false;
		}

		if (!Enum.TryParse<ExertionLevel>(command.PopSpeech(), true, out var result))
		{
			actor.OutputHandler.Send(
				$"That is not a valid exertion level. The valid values are {Enum.GetNames(typeof(ExertionLevel)).Select(x => x.Colour(Telnet.Green)).ListToString()}.");
		}

		ExertionLevel = result;
		Changed = true;
		actor.OutputHandler.Send($"This {ActionTypeName} will now set its attacker to a minimum of {ExertionLevel.Describe().Colour(Telnet.Green)} exertion");
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What should the relative weight of using this {ActionTypeName} be? A good practice is to use 100 for things that are the baseline likelihood.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send(
				$"You must enter a valid number for the relative likelihood of using this {ActionTypeName}.");
			return false;
		}

		Weighting = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The relative likelihood of using this {ActionTypeName} is now {Weighting.ToString("N2", actor)}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What name do you want to give to this {ActionTypeName}?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"This {ActionTypeName} is now called {Name.Colour(Telnet.Cyan)}");
		return true;
	}
}

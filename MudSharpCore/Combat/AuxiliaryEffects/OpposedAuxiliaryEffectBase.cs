using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.AuxiliaryEffects;

#nullable enable

internal abstract class OpposedAuxiliaryEffectBase : IAuxiliaryEffect
{
	protected OpposedAuxiliaryEffectBase(XElement root, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		DefenseTrait = gameworld.Traits.Get(long.Parse(root.Attribute("defensetrait")!.Value, CultureInfo.InvariantCulture)) ??
			throw new ApplicationException($"Missing DefenseTrait for {EffectName}: {root.Attribute("defensetrait")!.Value}");
		DefenseDifficulty = (Difficulty)int.Parse(root.Attribute("defensedifficulty")!.Value, CultureInfo.InvariantCulture);
		MinimumDegree = root.Attribute("minimumdegree") is XAttribute minimum
			? (OpposedOutcomeDegree)int.Parse(minimum.Value, CultureInfo.InvariantCulture)
			: OpposedOutcomeDegree.Marginal;
		FlatAmount = ReadDouble(root, "flatamount", DefaultFlatAmount);
		PerDegreeAmount = ReadDouble(root, "perdegreeamount", DefaultPerDegreeAmount);
		MaximumAmount = ReadDouble(root, "maximumamount", DefaultMaximumAmount);
		SuccessEcho = root.Attribute("successecho")?.Value;
		FailureEcho = root.Attribute("failureecho")?.Value;
	}

	protected OpposedAuxiliaryEffectBase(IFuturemud gameworld, ITraitDefinition defenseTrait, Difficulty defenseDifficulty,
		double flatAmount, double perDegreeAmount, double maximumAmount)
	{
		Gameworld = gameworld;
		DefenseTrait = defenseTrait;
		DefenseDifficulty = defenseDifficulty;
		MinimumDegree = OpposedOutcomeDegree.Marginal;
		FlatAmount = flatAmount;
		PerDegreeAmount = perDegreeAmount;
		MaximumAmount = maximumAmount;
	}

	public IFuturemud Gameworld { get; }
	public ITraitDefinition DefenseTrait { get; set; }
	public Difficulty DefenseDifficulty { get; set; }
	public OpposedOutcomeDegree MinimumDegree { get; set; }
	public double FlatAmount { get; set; }
	public double PerDegreeAmount { get; set; }
	public double MaximumAmount { get; set; }
	public string? SuccessEcho { get; set; }
	public string? FailureEcho { get; set; }

	protected abstract string TypeName { get; }
	protected abstract string EffectName { get; }
	protected abstract string AmountName { get; }
	protected virtual string AmountUnit => string.Empty;
	protected abstract double DefaultFlatAmount { get; }
	protected abstract double DefaultPerDegreeAmount { get; }
	protected abstract double DefaultMaximumAmount { get; }

	private static double ReadDouble(XElement root, string attribute, double defaultValue)
	{
		return root.Attribute(attribute) is XAttribute value
			? double.Parse(value.Value, CultureInfo.InvariantCulture)
			: defaultValue;
	}

	protected static bool TryParseDefenseArguments(
		AuxiliaryCombatAction action,
		ICharacter actor,
		StringStack command,
		out ITraitDefinition trait,
		out Difficulty difficulty)
	{
		trait = action.CheckTrait;
		difficulty = Difficulty.Normal;
		if (command.IsFinished)
		{
			return true;
		}

		var traitText = command.PopSpeech();
		var foundTrait = actor.Gameworld.Traits.GetByIdOrName(traitText);
		if (foundTrait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}
		trait = foundTrait;

		if (command.IsFinished)
		{
			return true;
		}

		if (!command.PopSpeech().TryParseEnum(out difficulty))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. The valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		return true;
	}

	protected void SaveCommonAttributes(XElement root)
	{
		root.Add(
			new XAttribute("type", TypeName),
			new XAttribute("defensetrait", DefenseTrait.Id.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("defensedifficulty", ((int)DefenseDifficulty).ToString(CultureInfo.InvariantCulture)),
			new XAttribute("minimumdegree", ((int)MinimumDegree).ToString(CultureInfo.InvariantCulture)),
			new XAttribute("flatamount", FlatAmount.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("perdegreeamount", PerDegreeAmount.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("maximumamount", MaximumAmount.ToString(CultureInfo.InvariantCulture))
		);

		if (!string.IsNullOrWhiteSpace(SuccessEcho))
		{
			root.Add(new XAttribute("successecho", SuccessEcho));
		}

		if (!string.IsNullOrWhiteSpace(FailureEcho))
		{
			root.Add(new XAttribute("failureecho", FailureEcho));
		}
	}

	protected bool TryGetOpposedSuccess(ICharacter attacker, IPerceiver target, CheckOutcome outcome, out OpposedOutcome opposed)
	{
		opposed = new OpposedOutcome(OpposedOutcomeDirection.Stalemate, OpposedOutcomeDegree.None);
		if (target is not ICharacter tch)
		{
			return false;
		}

		var defenderOutcome = Gameworld.GetCheck(CheckType.CombatMoveCheck)
		                               .Check(tch, DefenseDifficulty, DefenseTrait, attacker);
		opposed = new OpposedOutcome(outcome, defenderOutcome);
		return opposed.Outcome == OpposedOutcomeDirection.Proponent && opposed.Degree >= MinimumDegree;
	}

	protected double CalculateAmount(OpposedOutcome opposed)
	{
		var amount = FlatAmount + PerDegreeAmount * (int)opposed.Degree;
		if (MaximumAmount > 0.0)
		{
			amount = Math.Min(amount, MaximumAmount);
		}

		return Math.Max(0.0, amount);
	}

	protected string DescribeCommon(ICharacter actor)
	{
		return $"vs {DefenseTrait.Name.ColourValue()}@{DefenseDifficulty.DescribeColoured()} | min {MinimumDegree.DescribeColour()} | {AmountName}: {FlatAmount.ToString("N2", actor).ColourValue()}{AmountUnit}+{PerDegreeAmount.ToString("N2", actor).ColourValue()}{AmountUnit}/degree capped {MaximumAmount.ToString("N2", actor).ColourValue()}{AmountUnit}";
	}

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "trait":
			case "skill":
			case "defensetrait":
				return BuildingCommandTrait(actor, command);
			case "difficulty":
			case "diff":
			case "defensedifficulty":
				return BuildingCommandDifficulty(actor, command);
			case "minimum":
			case "minimumdegree":
			case "mindegree":
			case "min":
				return BuildingCommandMinimum(actor, command);
			case "amount":
			case "base":
			case "flat":
			case "flatamount":
				return BuildingCommandFlatAmount(actor, command);
			case "perdegree":
			case "scale":
			case "scaling":
				return BuildingCommandPerDegree(actor, command);
			case "maximum":
			case "max":
			case "cap":
				return BuildingCommandMaximum(actor, command);
			case "successecho":
			case "success":
				return BuildingCommandSuccessEcho(actor, command);
			case "failureecho":
			case "fail":
			case "failure":
				return BuildingCommandFailureEcho(actor, command);
			case "clearecho":
			case "clear":
				return BuildingCommandClearEcho(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	protected virtual string BuildingHelpText => $@"You can use the following syntax to edit this effect:

	#3trait <which>#0 - sets the trait used to defend against this move
	#3difficulty <diff>#0 - sets the difficulty of defending
	#3minimum <degree>#0 - sets the minimum opposed success degree required
	#3amount <value>#0 - sets the flat {AmountName.ToLowerInvariant()} amount
	#3perdegree <value>#0 - sets the {AmountName.ToLowerInvariant()} amount per success degree
	#3max <value>#0 - sets the maximum {AmountName.ToLowerInvariant()} amount, or 0 for no cap
	#3successecho <emote>#0 - sets an optional success echo
	#3failureecho <emote>#0 - sets an optional failed defense echo
	#3clearecho success|failure|all#0 - clears optional echoes";

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait should be used for the defense test against this effect?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		DefenseTrait = trait;
		actor.OutputHandler.Send($"The defense test against this effect will now use the {trait.Name.ColourValue()} trait.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the defense test against this effect be? The valid values are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty difficulty))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. The valid values are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		DefenseDifficulty = difficulty;
		actor.OutputHandler.Send($"The defense test against this effect will now be at {difficulty.DescribeColoured()} difficulty.");
		return true;
	}

	private bool BuildingCommandMinimum(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What minimum opposed outcome degree should this effect require? The valid values are {Enum.GetValues<OpposedOutcomeDegree>().Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out OpposedOutcomeDegree degree))
		{
			actor.OutputHandler.Send($"That is not a valid opposed outcome degree. The valid values are {Enum.GetValues<OpposedOutcomeDegree>().Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		MinimumDegree = degree;
		actor.OutputHandler.Send($"This effect will now require at least a {degree.DescribeColour()} opposed success.");
		return true;
	}

	private bool BuildingCommandFlatAmount(ICharacter actor, StringStack command)
	{
		return BuildingCommandDouble(actor, command, AmountName, value =>
		{
			FlatAmount = value;
			actor.OutputHandler.Send($"The flat {AmountName.ToLowerInvariant()} amount is now {value.ToString("N2", actor).ColourValue()}{AmountUnit}.");
		});
	}

	private bool BuildingCommandPerDegree(ICharacter actor, StringStack command)
	{
		return BuildingCommandDouble(actor, command, $"{AmountName} per degree", value =>
		{
			PerDegreeAmount = value;
			actor.OutputHandler.Send($"The {AmountName.ToLowerInvariant()} amount per degree is now {value.ToString("N2", actor).ColourValue()}{AmountUnit}.");
		});
	}

	private bool BuildingCommandMaximum(ICharacter actor, StringStack command)
	{
		return BuildingCommandDouble(actor, command, $"maximum {AmountName}", value =>
		{
			MaximumAmount = value;
			actor.OutputHandler.Send($"The maximum {AmountName.ToLowerInvariant()} amount is now {value.ToString("N2", actor).ColourValue()}{AmountUnit}.");
		});
	}

	private static bool BuildingCommandDouble(ICharacter actor, StringStack command, string amountName, Action<double> action)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should the {amountName.ToLowerInvariant()} value be?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		action(value);
		return true;
	}

	private bool BuildingCommandSuccessEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should be shown when this effect succeeds?");
			return false;
		}

		SuccessEcho = command.SafeRemainingArgument;
		actor.OutputHandler.Send($"This effect will now show the following success echo: {SuccessEcho.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandFailureEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should be shown when the defender resists this effect?");
			return false;
		}

		FailureEcho = command.SafeRemainingArgument;
		actor.OutputHandler.Send($"This effect will now show the following resisted echo: {FailureEcho.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandClearEcho(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "success":
				SuccessEcho = null;
				actor.OutputHandler.Send("The success echo has been cleared.");
				return true;
			case "failure":
			case "fail":
				FailureEcho = null;
				actor.OutputHandler.Send("The failure echo has been cleared.");
				return true;
			case "all":
			case "":
				SuccessEcho = null;
				FailureEcho = null;
				actor.OutputHandler.Send("All optional echoes have been cleared.");
				return true;
			default:
				actor.OutputHandler.Send("Do you want to clear the success echo, failure echo, or all echoes?");
				return false;
		}
	}

	protected void SendEcho(string? echo, ICharacter attacker, IPerceivable target, params IPerceivable[] additionalTargets)
	{
		if (string.IsNullOrWhiteSpace(echo))
		{
			return;
		}

		var targets = new[] { attacker, target }.Concat(additionalTargets).ToArray();
		attacker.OutputHandler.Handle(new EmoteOutput(new Emote(echo, attacker, targets),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{EffectName} Effect".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Defense Trait: {DefenseTrait.Name.ColourValue()} @ {DefenseDifficulty.DescribeColoured()}");
		sb.AppendLine($"Minimum Degree: {MinimumDegree.DescribeColour()}");
		sb.AppendLine($"Flat {AmountName}: {FlatAmount.ToString("N2", actor).ColourValue()}{AmountUnit}");
		sb.AppendLine($"{AmountName} Per Degree: {PerDegreeAmount.ToString("N2", actor).ColourValue()}{AmountUnit}");
		sb.AppendLine($"Maximum {AmountName}: {MaximumAmount.ToString("N2", actor).ColourValue()}{AmountUnit}");
		sb.AppendLine($"Success Echo: {(SuccessEcho?.ColourCommand() ?? "None".Colour(Telnet.Red))}");
		sb.AppendLine($"Failure Echo: {(FailureEcho?.ColourCommand() ?? "None".Colour(Telnet.Red))}");
		AppendSpecificShow(actor, sb);
		return sb.ToString();
	}

	protected virtual void AppendSpecificShow(ICharacter actor, StringBuilder sb)
	{
	}

	public abstract XElement Save();
	public abstract string DescribeForShow(ICharacter actor);
	public abstract void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome);
}

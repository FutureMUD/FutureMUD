using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Traits.Improvement;

/// <summary>
///     Classic Improvement has a fixed % chance on failure to improve, and improves by LV each time it does so
/// </summary>
public class ClassicImprovement : ImprovementModel
{
	protected ClassicImprovement()
	{

	}

	public ClassicImprovement(IFuturemud gameworld, Improver improver)
	{
		Gameworld = gameworld;
		_id = improver.Id;
		_name = improver.Name;
		LoadFromXml(XElement.Parse(improver.Definition));
	}

	protected ClassicImprovement(ClassicImprovement rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		using (new FMDB())
		{
			var definition = rhs.SaveDefinition();
			var dbitem = new Models.Improver
			{
				Name = name,
				Type = "classic",
				Definition = definition.ToString()
			};
			FMDB.Context.Improvers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			LoadFromXml(definition);
		}
	}

	public ClassicImprovement(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		ImprovementChance = 0.1;
		ImprovementExpression = new TraitExpression("10 - (variable/10)", gameworld);
		ImproveOnSuccess = true;
		ImproveOnFail = true;
		NoGainSecondsDiceExpression = "1d500+4000";
		DifficultyThresholdInterval = 10;
		using (new FMDB())
		{
			var dbitem = new Models.Improver
			{
				Name = name,
				Type = "classic",
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.Improvers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	/// <inheritdoc />
	public override IImprovementModel Clone(string name)
	{
		return new ClassicImprovement(this, name);
	}

	public double ImprovementChance { get; set; }

	public TraitExpression ImprovementExpression { get; set; }

#nullable enable
	public IFutureProg? ImprovementProg { get; set; }
#nullable restore

	public string NoGainSecondsDiceExpression { get; set; }

	public bool ImproveOnFail { get; set; }

	public bool ImproveOnSuccess { get; set; }

	public double DifficultyThresholdInterval { get; set; }

	private Difficulty ThresholdDifficulty(double value)
	{
		if (DifficultyThresholdInterval == 0)
		{
			return Difficulty.Automatic;
		}

		var ratio = (int)(value / DifficultyThresholdInterval);
		if (ratio < 0)
		{
			return Difficulty.Automatic;
		}

		if (ratio > 10)
		{
			return Difficulty.Impossible;
		}

		return (Difficulty)ratio;
	}

	/// <inheritdoc />
	public override bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType,
		bool ignoreTemporaryBlockers)
	{
		if (!ignoreTemporaryBlockers && person is IHaveEffects ihe && ihe.AffectedBy<INoTraitGainEffect>(trait.Definition))
		{
			return false;
		}

		if (trait is null)
		{
			return true;
		}

		if (trait.Value >= person.TraitMaxValue(trait.Definition))
		{
			return false;
		}

		if (ThresholdDifficulty(trait.RawValue) > difficulty)
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public override string ImproverType => "classic";

	/// <inheritdoc />
	protected override string SubtypeHelpText => @"
	#3chance <%>#0 - sets the chance to get an improvement
	#3amount <expression>#0 - how much to gain when you get an improvement
	#3prog <which>#0 - sets a prog to determine how much gain you get (instead of an expression)
	#3fail#0 - toggles improving on failed rolls
	#3success#0 - toggles improving on successful rolls
	#3interval <##>#0 - sets the minimum difficulty interval per existing trait point
	#3nogain <dice expression>#0 - sets the no gain period in seconds

Note: The formula for the #3amount#0 expression is a trait expression and also has a custom parameter called #6value#0 which contains the current value of the trait in question.";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "chance":
				return BuildingCommandChance(actor, command);
			case "amount":
				return BuildingCommandAmount(actor, command);
			case "interval":
				return BuildingCommandInterval(actor, command);
			case "nogain":
				return BuildingCommandNoGain(actor, command);
			case "fail":
			case "failure":
				return BuildingCommandFail(actor);
			case "success":
			case "succeed":
				return BuildingCommandSuccess(actor);
			case "prog":
				return BuildingCommandProg(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog. To clear an existing prog, use the #3amount#0 subcommand to set an improvement amount instead.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Number,
			[
				[ ProgVariableTypes.Character],
				[ ProgVariableTypes.Character, ProgVariableTypes.Trait]
			])
			.LookupProg();
		if (prog is null)
		{
			return false;
		}

		ImprovementProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This improver will now use the prog {prog.MXPClickableFunctionName()} to determine how much the skill improves.");
		return true;
	}

	private bool BuildingCommandSuccess(ICharacter actor)
	{
		ImproveOnSuccess = !ImproveOnSuccess;
		Changed = true;
		actor.OutputHandler.Send($"This improver will {ImproveOnSuccess.NowNoLonger()} check for improvement on successful rolls.");
		return true;
	}

	private bool BuildingCommandFail(ICharacter actor)
	{
		ImproveOnFail = !ImproveOnFail;
		Changed = true;
		actor.OutputHandler.Send($"This improver will {ImproveOnFail.NowNoLonger()} check for improvement on failed rolls.");
		return true;
	}

	private bool BuildingCommandNoGain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a dice expression for how many seconds for the no-gain effect to last.");
			return false;
		}

		var expr = command.SafeRemainingArgument;
		if (!Dice.IsDiceExpression(expr))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		NoGainSecondsDiceExpression = expr;
		Changed = true;
		actor.OutputHandler.Send($"The length of the no-gain effect will now be {$"{NoGainSecondsDiceExpression} seconds".ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInterval(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What interval of skill value should lead to a one level increase in difficulty required for improvement?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		if (value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a value over 0.");
			return false;
		}

		DifficultyThresholdInterval = value;
		Changed = true;
		var sb = new StringBuilder();
		sb.AppendLine($"You change the difficulty threshold interval to {value.ToString("N0", actor).ColourValue()}.");
		sb.AppendLine();
		sb.AppendLine("This leads to the following skill values vs minimum difficulties:");
		sb.AppendLine();
		var cumulative = 0.0;
		var difficulty = Difficulty.Automatic;
		while (true)
		{
			if (difficulty == Difficulty.Impossible)
			{
				sb.AppendLine($"{$"{cumulative.ToString("N2", actor)}+".ColourValue()}: {difficulty.DescribeColoured()}");
				break;
			}

			sb.AppendLine($"{cumulative.ToString("N2", actor).ColourValue()} - {(cumulative + value).ToString("N2", actor).ColourValue()}: {difficulty.DescribeColoured()}");
			cumulative += value;
			difficulty = difficulty.StageUp();
		}

		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What expression do you want to use to determine how many points the skill will go up by?");
			return false;
		}

		var expr = command.SafeRemainingArgument;
		var formula = new TraitExpression(expr, Gameworld);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		ImprovementExpression = formula;
		Changed = true;
		actor.OutputHandler.Send($"This trait will now gain {expr.ColourCommand()} points when it skills up.");
		return true;
	}

	private bool BuildingCommandChance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage chance should this trait have of improving when the conditions are met?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		ImprovementChance = value;
		Changed = true;
		actor.OutputHandler.Send($"This trait will now have a {value.ToString("P2", actor).ColourValue()} chance of improving each time it is valid.");
		return true;
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Improver #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {ImproverType.TitleCase().ColourValue()}");
		sb.AppendLine($"Improvement Chance: {ImprovementChance.ToString("P2", actor).ColourValue()}");
		if (ImprovementProg is not null)
		{
			sb.AppendLine($"Improvement Prog: {ImprovementProg.MXPClickableFunctionName()}");
		}
		else
		{
			sb.AppendLine($"Improvement Amount: {ImprovementExpression.OriginalFormulaText.ColourCommand()}");
		}
		sb.AppendLine($"Improve On Fail: {ImproveOnFail.ToColouredString()}");
		sb.AppendLine($"Improve On Success: {ImproveOnSuccess.ToColouredString()}");
		sb.AppendLine($"Difficulty Interval: {DifficultyThresholdInterval.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"No Gain Expression: {$"{NoGainSecondsDiceExpression} seconds".ColourValue()}");
		return sb.ToString();
	}

	public override double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome,
		TraitUseType usetype)
	{
		if (person is IHaveEffects ihe && ihe.AffectedBy<INoTraitGainEffect>(trait.Definition))
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [NoTraitGain Effect]");
			return 0.0;
		}

		if (!ImproveOnSuccess && outcome.IsPass())
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [No Improvement on Success]");
			return 0.0;
		}

		if (!ImproveOnFail && outcome.IsFail())
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [No Improvement on Failure]");
			return 0.0;
		}

		if (trait.Value >= person.TraitMaxValue(trait.Definition))
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [Trait above maximum value]");
			return 0.0;
		}

		var chance = ImprovementChance;
		if (person is IPerceivableHaveTraits ipht)
		{
			chance *= ipht.Merits
			              .OfType<ITraitLearningMerit>()
			              .Where(x => x.Applies(ipht))
			              .Select(x =>
				              x.SkillLearningChanceModifier(ipht, trait.Definition, outcome, difficulty, usetype))
			              .Aggregate(1.0, (accum, mult) => accum * mult);
		}

		if (Constants.Random.NextDouble() > chance)
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				$"-- NoGain [Failed roll {chance:P4} chance]");
			return 0.0;
		}

		if (ThresholdDifficulty(trait.RawValue) > difficulty)
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				$"-- NoGain [Difficulty below {ThresholdDifficulty(trait.RawValue)} threshold]");
			return 0.0;
		}

		var gain =
			ImprovementProg is not null && person is ICharacter ch ?
			ImprovementProg.ExecuteDouble(ch, trait.Definition) :
			ImprovementExpression.EvaluateWith(person, trait.Definition, TraitBonusContext.None, ("value", trait.RawValue));
		if (gain <= 0.0)
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement, $"-- NoGain [Gain amount was {gain:N2}]");
			return 0.0;
		}
		var noGainTimespan = TimeSpan.FromSeconds(Dice.Roll(NoGainSecondsDiceExpression));
		if (person is IHaveEffects phe && noGainTimespan.TotalSeconds > 0)
		{
			phe.AddEffect(new NoTraitGain((IPerceivable)person, trait.Definition), noGainTimespan);
		}
				
		trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement, $"-- Skill Gain of {gain:N4}");
		return gain;
	}

	protected virtual void LoadFromXml(XElement root)
	{
		ImprovementChance = Convert.ToDouble(root.Attribute("Chance").Value);
		ImprovementExpression = new TraitExpression(new Expression(root.Attribute("Expression").Value), new Dictionary<string, ITraitDefinition>());
		ImprovementProg = root.Attribute("ImprovementProg") != null
			? Gameworld.FutureProgs.Get(long.Parse(root.Attribute("ImprovementProg").Value))
			: null;
		ImproveOnFail = root.Attribute("ImproveOnFail") != null
			? bool.Parse(root.Attribute("ImproveOnFail").Value)
			: true;
		ImproveOnSuccess = root.Attribute("ImproveOnSuccess") != null
			? bool.Parse(root.Attribute("ImproveOnSuccess").Value)
			: true;
		DifficultyThresholdInterval = root.Attribute("DifficultyThresholdInterval") != null
			? double.Parse(root.Attribute("DifficultyThresholdInterval").Value)
			: 0.0;
		NoGainSecondsDiceExpression = root.Attribute("NoGainSecondsDiceExpression") != null
			? root.Attribute("NoGainSecondsDiceExpression").Value
			: "0";
	}

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XAttribute("Chance", ImprovementChance),
			new XAttribute("Expression", ImprovementExpression.OriginalFormulaText),
			new XAttribute("ImprovementProg", ImprovementProg?.Id ?? 0),
			new XAttribute("ImproveOnFail", ImproveOnFail),
			new XAttribute("ImproveOnSuccess", ImproveOnSuccess),
			new XAttribute("DifficultyThresholdInterval", DifficultyThresholdInterval),
			new XAttribute("NoGainSecondsDiceExpression", NoGainSecondsDiceExpression)
		);
	}
}
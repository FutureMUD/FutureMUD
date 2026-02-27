#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;

namespace MudSharp.Arenas;

public sealed class ArenaEventType : SaveableItem, IArenaEventType
{
	private readonly List<IArenaEventTypeSide> _sides = new();
	private const decimal DefaultEloKFactor = 32.0m;

	public ArenaEventType(MudSharp.Models.ArenaEventType model, CombatArena arena,
		Func<long, ICombatantClass?> classLookup, IArenaEliminationStrategy? eliminationStrategy = null)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		_id = model.Id;
		_name = model.Name;
		BringYourOwn = model.BringYourOwn;
		RegistrationDuration = TimeSpan.FromSeconds(model.RegistrationDurationSeconds);
		PreparationDuration = TimeSpan.FromSeconds(model.PreparationDurationSeconds);
		TimeLimit = model.TimeLimitSeconds.HasValue
			? TimeSpan.FromSeconds(model.TimeLimitSeconds.Value)
			: null;
		AutoScheduleInterval = model.AutoScheduleIntervalSeconds.HasValue
			? TimeSpan.FromSeconds(model.AutoScheduleIntervalSeconds.Value)
			: null;
		AutoScheduleReferenceTime = model.AutoScheduleReferenceTime;
		BettingModel = (BettingModel)model.BettingModel;
		AppearanceFee = model.AppearanceFee;
		VictoryFee = model.VictoryFee;
		IntroProg = model.IntroProgId.HasValue ? Gameworld.FutureProgs.Get(model.IntroProgId.Value) : null;
		ScoringProg = model.ScoringProgId.HasValue ? Gameworld.FutureProgs.Get(model.ScoringProgId.Value) : null;
		ResolutionOverrideProg = model.ResolutionOverrideProgId.HasValue
			? Gameworld.FutureProgs.Get(model.ResolutionOverrideProgId.Value)
			: null;
		EloStyle = Enum.IsDefined(typeof(ArenaEloStyle), model.EloStyle)
			? (ArenaEloStyle)model.EloStyle
			: ArenaEloStyle.TeamAverage;
		EloKFactor = model.EloKFactor > 0.0m ? model.EloKFactor : DefaultEloKFactor;
		EliminationMode = Enum.IsDefined(typeof(ArenaEliminationMode), model.EliminationMode)
			? (ArenaEliminationMode)model.EliminationMode
			: ArenaEliminationMode.NoElimination;
		AllowSurrender = model.AllowSurrender;
		EliminationStrategy = eliminationStrategy;

		foreach (var side in model.ArenaEventTypeSides)
		{
			_sides.Add(new ArenaEventTypeSide(side, this, classLookup));
		}
	}

	public CombatArena Arena { get; }
	ICombatArena IArenaEventType.Arena => Arena;

	public IEnumerable<IArenaEventTypeSide> Sides => _sides;
	public bool BringYourOwn { get; private set; }
	public TimeSpan RegistrationDuration { get; private set; }
	public TimeSpan PreparationDuration { get; private set; }
	public TimeSpan? TimeLimit { get; private set; }
	public TimeSpan? AutoScheduleInterval { get; private set; }
	public DateTime? AutoScheduleReferenceTime { get; private set; }
	public bool AutoScheduleEnabled => AutoScheduleInterval.HasValue &&
	                                  AutoScheduleInterval.Value > TimeSpan.Zero &&
	                                  AutoScheduleReferenceTime.HasValue;
	public BettingModel BettingModel { get; private set; }
	public decimal AppearanceFee { get; private set; }
	public decimal VictoryFee { get; private set; }
	public IFutureProg? IntroProg { get; private set; }
	public IFutureProg? ScoringProg { get; private set; }
	public IFutureProg? ResolutionOverrideProg { get; private set; }
	public ArenaEloStyle EloStyle { get; private set; }
	public decimal EloKFactor { get; private set; }
	public ArenaEliminationMode EliminationMode { get; private set; }
	public bool AllowSurrender { get; private set; }
	public IArenaEliminationStrategy? EliminationStrategy { get; private set; }

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Arena Event Type #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Arena: {Arena.Name.ColourName()}");
		sb.AppendLine($"BYO Gear: {BringYourOwn.ToColouredString()}");
		sb.AppendLine($"Registration Duration: {RegistrationDuration.Describe(actor).ColourValue()}");
		sb.AppendLine($"Preparation Duration: {PreparationDuration.Describe(actor).ColourValue()}");
		sb.AppendLine(TimeLimit.HasValue
			? $"Time Limit: {TimeLimit.Value.Describe(actor).ColourValue()}"
			: $"Time Limit: {"None".Colour(Telnet.Green)}");
		sb.AppendLine($"Betting Model: {BettingModel.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Appearance Fee: {Arena.Currency.Describe(AppearanceFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Victory Fee: {Arena.Currency.Describe(VictoryFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Auto Schedule: {DescribeAutoSchedule(actor)}");
		sb.AppendLine($"Intro Prog: {IntroProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Scoring Prog: {ScoringProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Resolution Override Prog: {ResolutionOverrideProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Elo Style: {EloStyle.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Elo K-Factor: {EloKFactor.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Elimination Mode: {EliminationMode.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Allow Surrender: {AllowSurrender.ToColouredString()}");
		if (EliminationStrategy != null)
		{
			sb.AppendLine($"Elimination Strategy: {EliminationStrategy.GetType().Name.ColourName()}");
		}

		sb.AppendLine();
		sb.AppendLine("Sides:");
		sb.AppendLine();
		foreach (var side in _sides.OrderBy(x => x.Index))
		{
			sb.AppendLine(side.Show(actor));
		}

		return sb.ToString();
	}

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this event type
	#3byo#0 - toggles bring-your-own equipment
	#3registration <timespan>#0 - sets the registration duration
	#3preparation <timespan>#0 - sets the preparation duration
	#3timelimit <timespan>|none#0 - sets or clears the time limit
	#3autoschedule <interval> <reference>|off#0 - sets or clears recurring event creation
	#3betting <fixed|parimutuel>#0 - sets the betting model
	#3appearance <amount>#0 - sets the appearance fee
	#3victory <amount>#0 - sets the victory fee
	#3introprog <prog>|none#0 - sets the intro prog
	#3scoringprog <prog>|none#0 - sets the scoring prog
	#3resolutionprog <prog>|none#0 - sets the resolution override prog
	#3elostyle <style>#0 - sets which Elo variant is used for rating updates
	#3elok <value>#0 - sets the Elo K-factor for this event type
	#3elimination <none|points|knockdown|knockout|death>#0 - sets how bouts end automatically
	#3surrender#0 - toggles whether combatants can surrender
	#3side <index> ...#0 - issues a building command to a specific side";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "byo":
			case "bring":
			case "bringyourown":
				return BuildingCommandBringYourOwn(actor);
			case "registration":
			case "register":
			case "reg":
				return BuildingCommandRegistration(actor, command);
			case "preparation":
			case "prep":
				return BuildingCommandPreparation(actor, command);
			case "timelimit":
			case "limit":
			case "time":
				return BuildingCommandTimeLimit(actor, command);
			case "autoschedule":
			case "schedule":
			case "repeat":
				return BuildingCommandAutoSchedule(actor, command);
			case "betting":
			case "bet":
				return BuildingCommandBetting(actor, command);
			case "appearance":
			case "appearancefee":
				return BuildingCommandAppearance(actor, command);
			case "victory":
			case "victoryfee":
				return BuildingCommandVictory(actor, command);
			case "introprog":
			case "intro":
				return BuildingCommandIntroProg(actor, command);
			case "scoringprog":
			case "score":
				return BuildingCommandScoringProg(actor, command);
			case "resolutionprog":
			case "resolution":
			case "resolve":
				return BuildingCommandResolutionProg(actor, command);
			case "elostyle":
			case "ratingstyle":
			case "ratingmethod":
				return BuildingCommandEloStyle(actor, command);
			case "elok":
			case "kfactor":
			case "ratingk":
				return BuildingCommandEloKFactor(actor, command);
			case "elimination":
			case "elim":
				return BuildingCommandElimination(actor, command);
			case "surrender":
				return BuildingCommandSurrender(actor);
			case "side":
			case "sides":
				return BuildingCommandSide(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this event type?".SubstituteANSIColour());
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Arena.EventTypes.Any(x => !ReferenceEquals(x, this) && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an event type called {name.ColourName()} in this arena.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This event type is now called {name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandBringYourOwn(ICharacter actor)
	{
		BringYourOwn = !BringYourOwn;
		Changed = true;
		actor.OutputHandler.Send($"Bring-your-own equipment is now {(BringYourOwn ? "enabled" : "disabled")} for this event type."
			.SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandRegistration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the registration duration be?".SubstituteANSIColour());
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var mts))
		{
			actor.OutputHandler.Send("That is not a valid duration.");
			return false;
		}

		RegistrationDuration = mts;
		Changed = true;
		actor.OutputHandler.Send($"Registration duration is now {RegistrationDuration.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPreparation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the preparation duration be?".SubstituteANSIColour());
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var mts))
		{
			actor.OutputHandler.Send("That is not a valid duration.");
			return false;
		}

		PreparationDuration = mts;
		Changed = true;
		actor.OutputHandler.Send($"Preparation duration is now {PreparationDuration.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTimeLimit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the time limit be? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			TimeLimit = null;
			Changed = true;
			actor.OutputHandler.Send("This event type will have no time limit.".SubstituteANSIColour());
			return true;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var mts))
		{
			actor.OutputHandler.Send("That is not a valid duration.");
			return false;
		}

		TimeLimit = mts;
		Changed = true;
		actor.OutputHandler.Send($"Time limit is now {TimeLimit.Value.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandAutoSchedule(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Current auto-schedule: {DescribeAutoSchedule(actor)}\nUse #3autoschedule every <interval> [from] <reference>#0 or #3autoschedule off#0."
					.SubstituteANSIColour());
			return false;
		}

		var firstToken = command.PopForSwitch();
		if (firstToken.EqualToAny("none", "off", "clear", "remove", "disable"))
		{
			ConfigureAutoSchedule(null, null);
			actor.OutputHandler.Send("Automatic scheduling is now disabled for this event type.".Colour(Telnet.Green));
			return true;
		}

		var intervalText = firstToken;
		if (firstToken.EqualTo("every"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a recurrence interval.".ColourError());
				return false;
			}

			intervalText = command.PopSpeech();
		}

		if (!MudTimeSpan.TryParse(intervalText, actor, out var intervalMud))
		{
			actor.OutputHandler.Send(
				"That is not a valid interval. Examples: #36h#0, #390m#0, #31d 2h#0.".SubstituteANSIColour());
			return false;
		}

		var interval = intervalMud.AsTimeSpan();
		if (interval <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send("The interval must be greater than zero.".ColourError());
			return false;
		}

		if (!command.IsFinished && command.PeekSpeech().EqualToAny("from", "at", "start", "starting"))
		{
			command.PopSpeech();
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What reference date/time should this recurrence use?".ColourCommand());
			return false;
		}

		if (!DateUtilities.TryParseDateTimeOrRelative(command.SafeRemainingArgument, actor.Account, false,
			    out var referenceUtc))
		{
			actor.OutputHandler.Send(
				"That is not a valid reference date/time. Examples: #310:00#0 or #32026-02-18 10:00#0."
					.SubstituteANSIColour());
			return false;
		}

		ConfigureAutoSchedule(interval, referenceUtc);
		actor.OutputHandler.Send(
			$"Automatic scheduling is now {DescribeAutoSchedule(actor)} for this event type.".Colour(Telnet.Green));
		return true;
	}

	private bool BuildingCommandBetting(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What betting model should this event type use? (fixed/parimutuel)");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<BettingModel>(out var model))
		{
			actor.OutputHandler.Send(
				$"That is not a valid betting model. Valid options are {Enum.GetValues<BettingModel>().ListToColouredString()}.");
			return false;
		}

		BettingModel = model;
		Changed = true;
		actor.OutputHandler.Send($"Betting model is now {BettingModel.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandAppearance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the appearance fee be?");
			return false;
		}

		if (!Arena.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid amount.");
			return false;
		}

		AppearanceFee = value;
		Changed = true;
		actor.OutputHandler.Send($"Appearance fee is now {Arena.Currency.Describe(AppearanceFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandVictory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the victory fee be?");
			return false;
		}

		if (!Arena.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid amount.");
			return false;
		}

		VictoryFee = value;
		Changed = true;
		actor.OutputHandler.Send($"Victory fee is now {Arena.Currency.Describe(VictoryFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandIntroProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which intro prog should this event type use? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			IntroProg = null;
			Changed = true;
			actor.OutputHandler.Send("Intro prog cleared.".SubstituteANSIColour());
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Void,
			ArenaProgParameters.EventProgParameterSets).LookupProg();
		if (prog == null)
		{
			return false;
		}

		IntroProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"Intro prog set to {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandScoringProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which scoring prog should this event type use? Use #3none#0 to clear.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			if (EliminationMode == ArenaEliminationMode.PointsElimination)
			{
				actor.OutputHandler.Send(
					"You cannot clear the scoring prog while elimination mode is #3Points Elimination#0. Change elimination mode first."
						.SubstituteANSIColour());
				return false;
			}

			ScoringProg = null;
			Changed = true;
			actor.OutputHandler.Send("Scoring prog cleared.".SubstituteANSIColour());
			return true;
		}

		var requiredType = EliminationMode == ArenaEliminationMode.PointsElimination
			? ProgVariableTypes.Number | ProgVariableTypes.Collection
			: ProgVariableTypes.Void;
		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, requiredType,
			ArenaProgParameters.EventProgParameterSets).LookupProg();
		if (prog == null)
		{
			return false;
		}

		ScoringProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"Scoring prog set to {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandResolutionProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which resolution override prog should this event type use? Use #3none#0 to clear."
				.SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			ResolutionOverrideProg = null;
			Changed = true;
			actor.OutputHandler.Send("Resolution override prog cleared.".SubstituteANSIColour());
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument,
			ProgVariableTypes.Number | ProgVariableTypes.Collection, ArenaProgParameters.EventProgParameterSets).LookupProg();
		if (prog == null)
		{
			return false;
		}

		ResolutionOverrideProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"Resolution override prog set to {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandEloStyle(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which Elo style should this event type use? Valid options are {Enum.GetValues<ArenaEloStyle>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<ArenaEloStyle>(out var style))
		{
			actor.OutputHandler.Send(
				$"That is not a valid Elo style. Valid options are {Enum.GetValues<ArenaEloStyle>().ListToColouredString()}.");
			return false;
		}

		EloStyle = style;
		Changed = true;
		actor.OutputHandler.Send($"Elo style is now {EloStyle.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEloKFactor(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What Elo K-factor should this event type use? It must be greater than zero. Current value is {EloKFactor.ToString("N2", actor).ColourValue()}.");
			return false;
		}

		if (!decimal.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0m)
		{
			actor.OutputHandler.Send("You must enter a number greater than zero for the Elo K-factor.");
			return false;
		}

		EloKFactor = value;
		Changed = true;
		actor.OutputHandler.Send($"Elo K-factor is now {EloKFactor.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandElimination(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What elimination mode should this event type use? Valid options are {Enum.GetValues<ArenaEliminationMode>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (!TryParseEliminationMode(command.SafeRemainingArgument, out var mode))
		{
			actor.OutputHandler.Send(
				$"That is not a valid elimination mode. Valid options are {Enum.GetValues<ArenaEliminationMode>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		if (mode == ArenaEliminationMode.PointsElimination && ScoringProg is null)
		{
			actor.OutputHandler.Send(
				"You must set a #3scoringprog#0 before selecting #3Points Elimination#0.".SubstituteANSIColour());
			return false;
		}

		if (mode == ArenaEliminationMode.PointsElimination &&
		    ScoringProg is not null &&
		    !ScoringProg.ReturnType.CompatibleWith(ProgVariableTypes.Number | ProgVariableTypes.Collection))
		{
			actor.OutputHandler.Send(
				$"The current scoring prog {ScoringProg.MXPClickableFunctionName()} does not return a collection of numbers, which is required for points elimination.");
			return false;
		}

		EliminationMode = mode;
		Changed = true;
		actor.OutputHandler.Send($"Elimination mode is now {EliminationMode.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSurrender(ICharacter actor)
	{
		AllowSurrender = !AllowSurrender;
		Changed = true;
		actor.OutputHandler.Send(
			$"Combatant surrender is now {(AllowSurrender ? "enabled" : "disabled")} for this event type.");
		return true;
	}

	private bool BuildingCommandSide(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which side do you want to edit?".SubstituteANSIColour());
			return false;
		}

		if (!ArenaSideIndexUtilities.TryParseDisplayIndex(command.PopSpeech(), out var index))
		{
			actor.OutputHandler.Send("You must specify the numeric index of the side starting at 1.");
			return false;
		}

		var side = _sides.FirstOrDefault(x => x.Index == index);
		if (side == null)
		{
			actor.OutputHandler.Send("There is no side with that index.".ColourError());
			return false;
		}

		return side.BuildingCommand(actor, command);
	}

	private string DescribeAutoSchedule(ICharacter actor)
	{
		if (!AutoScheduleEnabled)
		{
			return "Disabled".ColourError();
		}

		return
			$"Every {AutoScheduleInterval!.Value.Describe(actor).ColourValue()} from {AutoScheduleReferenceTime!.Value.ToString("f", actor).ColourValue()}";
	}

	private static bool TryParseEliminationMode(string text, out ArenaEliminationMode mode)
	{
		mode = ArenaEliminationMode.NoElimination;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (text.TryParseEnum<ArenaEliminationMode>(out mode))
		{
			return true;
		}

		switch (text.CollapseString())
		{
			case "none":
			case "off":
			case "noelim":
			case "noelimination":
				mode = ArenaEliminationMode.NoElimination;
				return true;
			case "points":
			case "point":
			case "score":
			case "scoring":
				mode = ArenaEliminationMode.PointsElimination;
				return true;
			case "knockdown":
			case "down":
			case "kd":
				mode = ArenaEliminationMode.KnockDown;
				return true;
			case "knockout":
			case "ko":
				mode = ArenaEliminationMode.Knockout;
				return true;
			case "death":
			case "kill":
			case "lethal":
				mode = ArenaEliminationMode.Death;
				return true;
			default:
				return false;
		}
	}

	public IArenaEvent CreateInstance(DateTime scheduledTime, IEnumerable<IArenaReservation>? reservations = null)
	{
		return Arena.CreateEvent(this, scheduledTime, reservations);
	}

	public void ConfigureAutoSchedule(TimeSpan? interval, DateTime? referenceTime)
	{
		var isEnabled = interval.HasValue && interval.Value > TimeSpan.Zero && referenceTime.HasValue;
		AutoScheduleInterval = isEnabled ? interval : null;
		AutoScheduleReferenceTime = isEnabled ? referenceTime : null;
		Changed = true;
		Gameworld.ArenaScheduler.SyncRecurringSchedule(this);
	}

	public IArenaEventType Clone(string newName, ICharacter originator)
	{
		_ = originator;

		if (string.IsNullOrWhiteSpace(newName))
		{
			throw new ArgumentException("Clone name must be provided.", nameof(newName));
		}

		using (new FMDB())
		{
			var dbType = new MudSharp.Models.ArenaEventType
			{
				ArenaId = Arena.Id,
				Name = newName,
				BringYourOwn = BringYourOwn,
				RegistrationDurationSeconds = (int)RegistrationDuration.TotalSeconds,
				PreparationDurationSeconds = (int)PreparationDuration.TotalSeconds,
				TimeLimitSeconds = TimeLimit.HasValue ? (int)TimeLimit.Value.TotalSeconds : null,
				AutoScheduleIntervalSeconds = AutoScheduleInterval.HasValue
					? (int)AutoScheduleInterval.Value.TotalSeconds
					: null,
				AutoScheduleReferenceTime = AutoScheduleReferenceTime,
				BettingModel = (int)BettingModel,
				EliminationMode = (int)EliminationMode,
				AllowSurrender = AllowSurrender,
				AppearanceFee = AppearanceFee,
				VictoryFee = VictoryFee,
				IntroProgId = IntroProg?.Id,
				ScoringProgId = ScoringProg?.Id,
				ResolutionOverrideProgId = ResolutionOverrideProg?.Id,
				EloStyle = (int)EloStyle,
				EloKFactor = EloKFactor
			};
			foreach (var side in _sides.OfType<ArenaEventTypeSide>())
			{
				var dbSide = new MudSharp.Models.ArenaEventTypeSide
				{
					Index = side.Index,
					Capacity = side.Capacity,
					Policy = (int)side.Policy,
					MinimumRating = side.MinimumRating,
					MaximumRating = side.MaximumRating,
					AllowNpcSignup = side.AllowNpcSignup,
					AutoFillNpc = side.AutoFillNpc,
					OutfitProgId = side.OutfitProg?.Id,
					NpcLoaderProgId = side.NpcLoaderProg?.Id
				};
				foreach (var cls in side.EligibleClasses)
				{
					dbSide.ArenaEventTypeSideAllowedClasses.Add(new ArenaEventTypeSideAllowedClass
					{
						ArenaCombatantClassId = cls.Id
					});
				}

				dbType.ArenaEventTypeSides.Add(dbSide);
			}

			FMDB.Context.ArenaEventTypes.Add(dbType);
			FMDB.Context.SaveChanges();
			var newType = new ArenaEventType(dbType, Arena, Arena.GetCombatantClass, EliminationStrategy);
			Arena.AddEventType(newType);
			return newType;
		}
	}

	public override string FrameworkItemType => "ArenaEventType";

	public override void Save()
	{
		if (!Changed)
		{
			return;
		}

		using (new FMDB())
		{
			var dbType = FMDB.Context.ArenaEventTypes.Find(Id);
			if (dbType == null)
			{
				return;
			}

			dbType.Name = Name;
			dbType.BringYourOwn = BringYourOwn;
			dbType.RegistrationDurationSeconds = (int)RegistrationDuration.TotalSeconds;
			dbType.PreparationDurationSeconds = (int)PreparationDuration.TotalSeconds;
			dbType.TimeLimitSeconds = TimeLimit.HasValue ? (int)TimeLimit.Value.TotalSeconds : null;
			dbType.AutoScheduleIntervalSeconds = AutoScheduleInterval.HasValue
				? (int)AutoScheduleInterval.Value.TotalSeconds
				: null;
			dbType.AutoScheduleReferenceTime = AutoScheduleReferenceTime;
			dbType.BettingModel = (int)BettingModel;
			dbType.EliminationMode = (int)EliminationMode;
			dbType.AllowSurrender = AllowSurrender;
			dbType.AppearanceFee = AppearanceFee;
			dbType.VictoryFee = VictoryFee;
			dbType.IntroProgId = IntroProg?.Id;
			dbType.ScoringProgId = ScoringProg?.Id;
			dbType.ResolutionOverrideProgId = ResolutionOverrideProg?.Id;
			dbType.EloStyle = (int)EloStyle;
			dbType.EloKFactor = EloKFactor;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

}

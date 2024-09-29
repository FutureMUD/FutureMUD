using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Models;

namespace MudSharp.Magic.Powers;

public class SensePower : MagicPowerBase
{
	public override string PowerType => "Sense";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("sense", (power, gameworld) => new SensePower(power, gameworld));
    }

    protected override XElement SaveDefinition()
    {
        var definition = new XElement("Definition",
            new XElement("Verb", Verb),
            new XElement("EmoteVisible", EmoteVisible),
            new XElement("EmoteText", new XCData(EmoteText)),
            new XElement("EchoHeader", new XCData(EchoHeader)),
            new XElement("NoTargetsFoundEcho", new XCData(NoTargetsFoundEcho)),
            new XElement("SenseType", (long)SenseType),
            new XElement("MinimumSuccessThreshold", (int)MinimumSuccessThreshold),
            new XElement("CommandDelay", CommandDelay.TotalSeconds),
            new XElement("SenseTargetFilterProg", SenseTargetFilterProg.Id),
            new XElement("TargetDifficultyProg", TargetDifficultyProg.Id),
            new XElement("CheckTrait", SkillCheckTrait.Id),
            new XElement("PowerDistance", (int)PowerDistance)
        );
        return definition;
    }

    public SensePower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var definition = XElement.Parse(power.Definition);
		Verb = definition.Element("Verb")?.Value ??
		       throw new ApplicationException($"Missing Verb element in SensePower definition {Id}");
		EmoteText = definition.Element("EmoteText")?.Value ??
		            throw new ApplicationException($"Missing EmoteText element in SensePower definition {Id}");
		EchoHeader = definition.Element("EchoHeader")?.Value ??
		             throw new ApplicationException($"Missing EchoHeader element in SensePower definition {Id}");
		NoTargetsFoundEcho = definition.Element("NoTargetsFoundEcho")?.Value ??
		                     throw new ApplicationException(
			                     $"Missing NoTargetsFoundEcho element in SensePower definition {Id}");
		if (!Utilities.TryParseEnum<MagicPowerDistance>(
			    definition.Element("PowerDistance")?.Value ??
			    throw new ApplicationException($"Missing PowerDistance element in SensePower definition {Id}"),
			    out var powerDistance))
		{
			throw new ApplicationException($"Invalid MagicPowerDistance in SensePower definition {Id}");
		}

		PowerDistance = powerDistance;

		if (!Utilities.TryParseEnum<FutureProgVariableTypes>(
			    definition.Element("SenseType")?.Value ??
			    throw new ApplicationException($"Missing SenseType element in SensePower definition {Id}"),
			    out var senseType))
		{
			throw new ApplicationException($"Invalid SenseType in SensePower definition {Id}");
		}

		SenseType = senseType;

		EmoteVisible = bool.Parse(definition.Element("EmoteVisible")?.Value ??
		                          throw new ApplicationException(
			                          $"Missing EmoteVisible element in SensePower definition {Id}"));

		CommandDelay = TimeSpan.FromSeconds(double.Parse(definition.Element("CommandDelay")?.Value ??
		                                                 throw new ApplicationException(
			                                                 $"Missing CommandDelay element in SensePower definition {Id}")));
		SkillCheckTrait =
			long.TryParse(
				definition.Element("CheckTrait")?.Value ??
				throw new ApplicationException($"Missing CheckTrait element in SensePower definition {Id}"),
				out var value)
				? Gameworld.Traits.Get(value)
				: Gameworld.Traits.GetByName(definition.Element("CheckTrait").Value);
		if (SkillCheckTrait == null)
		{
			throw new ApplicationException($"CheckTrait not found in SeensePower definition {Id}");
        }

        MinimumSuccessThreshold = (Outcome)int.Parse(definition.Element("MinimumSuccessThreshold")?.Value ?? "1");

        var targetFilterProg =
			long.TryParse(
				definition.Element("SenseTargetFilterProg")?.Value ??
				throw new ApplicationException($"Missing SenseTargetFilterProg element in SensePower definition {Id}"),
				out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(definition.Element("SenseTargetFilterProg").Value);
		if (targetFilterProg == null)
		{
			throw new ApplicationException($"SenseTargetFilterProg not found in SensePower definition {Id}");
		}

		if (!targetFilterProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			throw new ApplicationException(
				$"SenseTargetFilterProg ({targetFilterProg.Name} #{targetFilterProg.Id}) did not return boolean in SensePower definition {Id}");
		}

		SenseTargetFilterProg = targetFilterProg;

		var targetDifficultyProg =
			long.TryParse(
				definition.Element("TargetDifficultyProg")?.Value ??
				throw new ApplicationException($"Missing TargetDifficultyProg element in SensePower definition {Id}"),
				out value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(definition.Element("TargetDifficultyProg").Value);
		if (targetDifficultyProg == null)
		{
			throw new ApplicationException($"TargetDifficultyProg not found in SensePower definition {Id}");
		}

		if (!targetDifficultyProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			throw new ApplicationException(
				$"TargetDifficultyProg ({targetFilterProg.Name} #{targetFilterProg.Id}) did not return number in SensePower definition {Id}");
		}

		TargetDifficultyProg = targetDifficultyProg;

		switch (senseType)
		{
			case FutureProgVariableTypes.Perceivable:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceivable }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceivable parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceivable }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceivable parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			case FutureProgVariableTypes.Perceiver:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceiver }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceiver parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Perceiver }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Perceiver parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			case FutureProgVariableTypes.Character:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Character parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Character }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept a Character parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			case FutureProgVariableTypes.Item:
				if (!targetFilterProg.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
				{
					throw new ApplicationException(
						$"SenseTargetFilterProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept an Item parameter as specified by the SenseType in SensePower {Id}.");
				}

				if (!targetDifficultyProg.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
				{
					throw new ApplicationException(
						$"TargetDifficultyProg ({targetFilterProg.FunctionName} #{targetFilterProg.Id}) did not accept an Item parameter as specified by the SenseType in SensePower {Id}.");
				}

				break;
			default:
				throw new ApplicationException($"SenseType in SensePower definition {Id} was of an invalid type.");
		}
	}

	public string Verb { get; protected set; }
	public MagicPowerDistance PowerDistance { get; protected set; }
	public TimeSpan CommandDelay { get; protected set; }

	public string EmoteText { get; protected set; }

	public bool EmoteVisible { get; protected set; }

	public string EchoHeader { get; protected set; }

	public string NoTargetsFoundEcho { get; protected set; }

	public ITraitDefinition SkillCheckTrait { get; protected set; }

	public IFutureProg TargetDifficultyProg { get; protected set; }

	public FutureProgVariableTypes SenseType { get; protected set; }

	public IFutureProg SenseTargetFilterProg { get; protected set; }

    public Outcome MinimumSuccessThreshold { get; protected set; }

    public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		var (truth, missing) = CanAffordToInvokePower(actor, verb);
		if (!truth)
		{
			actor.OutputHandler.Send(
				$"You can't do that because you lack sufficient {missing.Name.Colour(Telnet.BoldMagenta)}.");
			return;
		}

		if (CanInvokePowerProg?.Execute<bool?>(actor) == false)
		{
			actor.OutputHandler.Send(WhyCantInvokePowerProg.Execute(actor)?.ToString() ??
			                         "You can't use that power at this time.");
			return;
		}

		if (!HandleGeneralUseRestrictions(actor))
		{
			return;
		}

		ConsumePowerCosts(actor, verb);
		actor.AddEffect(new CommandDelayMagicPower(actor, this), CommandDelay);
		if (EmoteVisible)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(EmoteText, actor, actor)));
		}
		else
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(EmoteText, actor, actor)));
		}

		var targets = new List<(IPerceivable Target, ICell Location, RoomLayer Layer)>();

		switch (SenseType)
		{
			case FutureProgVariableTypes.Perceivable:
			case FutureProgVariableTypes.Perceiver:
				switch (PowerDistance)
				{
					case MagicPowerDistance.SameLocationOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.AdjacentLocationsOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));

						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));

						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.Characters.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.GameItems.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						break;
					case MagicPowerDistance.SameAreaOnly:
						if (actor.Location.Areas.Any())
						{
							foreach (var area in actor.Location.Areas)
							{
								targets.AddRange(
									area.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
								targets.AddRange(area.GameItems.Select(x =>
									((IPerceivable)x, x.Location, x.RoomLayer)));
							}
						}
						else
						{
							goto case MagicPowerDistance.AdjacentLocationsOnly;
						}

						break;
					case MagicPowerDistance.SameZoneOnly:
						targets.AddRange(
							actor.Location.Zone.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.Zone.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SameShardOnly:
						targets.AddRange(
							actor.Location.Shard.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.Shard.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SamePlaneOnly:
						goto case MagicPowerDistance.SameShardOnly;
				}

				break;
			case FutureProgVariableTypes.Character:
				switch (PowerDistance)
				{
					case MagicPowerDistance.SameLocationOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.AdjacentLocationsOnly:
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.Characters.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						break;
					case MagicPowerDistance.SameAreaOnly:
						if (actor.Location.Areas.Any())
						{
							foreach (var area in actor.Location.Areas)
							{
								targets.AddRange(
									area.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
							}
						}
						else
						{
							goto case MagicPowerDistance.AdjacentLocationsOnly;
						}

						break;
					case MagicPowerDistance.SameZoneOnly:
						targets.AddRange(
							actor.Location.Zone.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SameShardOnly:
						targets.AddRange(
							actor.Location.Shard.Characters.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SamePlaneOnly:
						goto case MagicPowerDistance.SameShardOnly;
				}

				break;
			case FutureProgVariableTypes.Item:
				switch (PowerDistance)
				{
					case MagicPowerDistance.SameLocationOnly:
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.AdjacentLocationsOnly:
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(
							actor.Location.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						targets.AddRange(actor.Location.ExitsFor(actor, true).Select(x => x.Destination).Distinct()
						                      .SelectMany(x =>
							                      x.GameItems.Select(y => ((IPerceivable)y, x, y.RoomLayer))));
						break;
					case MagicPowerDistance.SameAreaOnly:
						if (actor.Location.Areas.Any())
						{
							foreach (var area in actor.Location.Areas)
							{
								targets.AddRange(area.GameItems.Select(x =>
									((IPerceivable)x, x.Location, x.RoomLayer)));
							}
						}
						else
						{
							goto case MagicPowerDistance.AdjacentLocationsOnly;
						}

						break;
					case MagicPowerDistance.SameZoneOnly:
						targets.AddRange(
							actor.Location.Zone.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SameShardOnly:
						targets.AddRange(
							actor.Location.Shard.GameItems.Select(x => ((IPerceivable)x, x.Location, x.RoomLayer)));
						break;
					case MagicPowerDistance.SamePlaneOnly:
						goto case MagicPowerDistance.SameShardOnly;
				}

				break;
		}

		var check = Gameworld.GetCheck(CheckType.MagicSensePower);
		var results = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, SkillCheckTrait);

		var final = targets
		            .Distinct()
		            .Where(x => SenseTargetFilterProg.Execute<bool?>(x.Target) == true)
		            .Where(x => results[TargetDifficultyProg.ExecuteString(x.Target).ParseEnumWithDefault(Difficulty.Normal)] >= MinimumSuccessThreshold)
		            .ToList();

		if (!final.Any())
		{
			actor.OutputHandler.Send(NoTargetsFoundEcho);
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine(EchoHeader);
		foreach (var grouping in final.GroupBy(x => x.Location))
		{
			sb.AppendLine();
			if (grouping.Key == actor.Location)
			{
				sb.AppendLine("Here:".ColourRoom());
			}
			else
			{
				var directions = actor.Location.PathBetween(grouping.Key, 50, exit => true).ToList();
				if (!directions.Any())
				{
					sb.AppendLine(grouping.Key.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee));
				}
				else
				{
					sb.AppendLine(
						$"{grouping.Key.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} ({directions.DescribeDirectionsToFrom()})");
				}
			}

			foreach (var item in grouping.OrderByDescending(x => x.Layer))
			{
				sb.AppendLine(
					$"\t{item.Target.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee)} [{item.Layer.PositionalDescription()}]");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	public override IEnumerable<string> Verbs => new[] { Verb };

    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
        sb.AppendLine($"Power Verb: {Verb.ColourCommand()}");
        sb.AppendLine($"Skill Check Trait: {SkillCheckTrait.Name.ColourValue()}");
        sb.AppendLine($"Skill Check Difficulty Prog: {TargetDifficultyProg.MXPClickableFunctionName()}");
        sb.AppendLine($"Minimum Success Threshold: {MinimumSuccessThreshold.DescribeColour()}");
        sb.AppendLine($"Target Filter Prog: {SenseTargetFilterProg.MXPClickableFunctionName()}");
        sb.AppendLine($"Power Distance: {PowerDistance.DescribeEnum().ColourValue()}");
        sb.AppendLine($"Command Delay: {CommandDelay.DescribePreciseBrief().ColourValue()}");
        sb.AppendLine($"Emote Visible To Others: {EmoteVisible.ToColouredString()}");
        sb.AppendLine($"Sense Types: {SenseType.GetSingleFlags().Select(x => x.Describe().ColourValue()).ListToString()}");
        sb.AppendLine();
        sb.AppendLine("Emotes:");
        sb.AppendLine();
        sb.AppendLine($"Emote: {EmoteText.ColourCommand()}");
        sb.AppendLine($"Echo Header: {EchoHeader.ColourCommand()}");
        sb.AppendLine($"No Targets Found Echo: {NoTargetsFoundEcho.ColourCommand()}");
    }

    #region Building Commands
    /// <inheritdoc />
    protected override string SubtypeHelpText => @"	#3begin <verb>#0 - sets the verb to activate this power
    #3end <verb>#0 - sets the verb to end this power
    #3skill <which>#0 - sets the skill used in the skill check
    #3difficulty <difficulty>#0 - sets the difficulty of the skill check
    #3threshold <outcome>#0 - sets the minimum outcome for skill success
    #3distance <distance>#0 - sets the distance that this power can detect targets at
    #3delay <seconds>#0 - sets the delay in seconds before the results are shown
    #3senses character|item|both#0 - sets what type of thing this power senses
    #3filter <prog>#0 - sets the prog that filters if a target is seen
    #3emotevisible#0 - toggles if the emote echoes to others or just the user
    #3emote <text>#0 - sets the power emote
    #3header <text>#0 - sets the header of the output
    #3notargets <text>#0 - sets the echo for no targets found";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "verb":
                return BuildingCommandVerb(actor, command);
            case "skill":
            case "trait":
                return BuildingCommandSkill(actor, command);
            case "difficulty":
                return BuildingCommandDifficulty(actor, command);
            case "threshold":
                return BuildingCommandThreshold(actor, command);
            case "distance":
                return BuildingCommandDistance(actor, command);
			case "delay":
                return BuildingCommandDelay(actor, command);
			case "emotevisible":
                return BuildingCommandEmoteVisible(actor);
			case "emote":
                return BuildingCommandEmote(actor, command);
			case "header":
                return BuildingCommandHeader(actor, command);
			case "notarget":
			case "notargets":
			case "notargetecho":
			case "notargetsecho":
                return BuildingCommandNoTargetsEcho(actor, command);
			case "filter":
			case "filterprog":
                return BuildingCommandFilterProg(actor, command);
			case "senses":
			case "sense":
                return BuildingCommandSense(actor, command);

		}
        return base.BuildingCommand(actor, command.GetUndo());
    }



    #region Building Subcommands
    private bool BuildingCommandSense(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which type of thing does this power detect? Valid options are #3character#0, #3item#0 and #3perceiver#0 (i.e. both).");
            return false;
        }

        switch (command.SafeRemainingArgument.ToLowerInvariant())
        {
            case "character":
            case "characters":
            case "ch":
                SenseType = FutureProgVariableTypes.Character;
                actor.OutputHandler.Send("This power will now detect characters.");
                if (!SenseTargetFilterProg.MatchesParameters([FutureProgVariableTypes.Character]))
                {
                    SenseTargetFilterProg = Gameworld.AlwaysTrueProg;
                    actor.OutputHandler.Send($"Note: The Sense Target Prog was no longer valid and has been set back to default.".ColourError());
                }
                break;
            case "item":
            case "obj":
            case "objects":
            case "object":
            case "items":
            case "gameitem":
            case "gameitems":
                SenseType = FutureProgVariableTypes.Item;
                actor.OutputHandler.Send("This power will now detect items.");
                if (!SenseTargetFilterProg.MatchesParameters([FutureProgVariableTypes.Item]))
                {
                    SenseTargetFilterProg = Gameworld.AlwaysTrueProg;
                    actor.OutputHandler.Send($"Note: The Sense Target Prog was no longer valid and has been set back to default.".ColourError());
                }
                break;
            case "perceiver":
            case "perceivable":
            case "thing":
            case "both":
                SenseType = FutureProgVariableTypes.Perceivable;
                actor.OutputHandler.Send("This power will now detect both items and characters.");
                if (!SenseTargetFilterProg.MatchesParameters([FutureProgVariableTypes.Perceivable]))
                {
                    SenseTargetFilterProg = Gameworld.AlwaysTrueProg;
                    actor.OutputHandler.Send($"Note: The Sense Target Prog was no longer valid and has been set back to default.".ColourError());
                }
                break;
            default:
                actor.OutputHandler.Send($"That is not a valid type of thing to detect. Valid options are #3character#0, #3item#0 and #3perceiver#0 (i.e. both).");
                return false;
        }

        Changed = true;
        return true;
    }

    private bool BuildingCommandFilterProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which prog should be used to filter whether a target is valid?");
            return false;
        }

        var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean, [
            [FutureProgVariableTypes.Character],
            [FutureProgVariableTypes.Item],
            [FutureProgVariableTypes.Perceivable],
            [FutureProgVariableTypes.Perceiver],
        ]).LookupProg();
        if (prog is null)
        {
            return false;
        }

        SenseTargetFilterProg = prog;
        Changed = true;
        if (prog.MatchesParameters([FutureProgVariableTypes.Perceivable]))
        {
            SenseType = FutureProgVariableTypes.Perceivable;
        }
        else if (prog.MatchesParameters([FutureProgVariableTypes.Character]))
        {
            SenseType = FutureProgVariableTypes.Character;
        }
        else
        {
            SenseType = FutureProgVariableTypes.Item;
        }

        actor.OutputHandler.Send($"This power will now use the {prog.MXPClickableFunctionName()} prog to filter targets.");
        return true;
    }

    private bool BuildingCommandNoTargetsEcho(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to set the no targets echo for the results to?");
            return false;
        }

        NoTargetsFoundEcho = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The no targets echo for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandHeader(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to set the header line for the results to?");
            return false;
        }

        EchoHeader = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The results header line for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to set the command emote to?");
            return false;
        }

        var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        EmoteText = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The command emote for this power is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandEmoteVisible(ICharacter actor)
    {
        EmoteVisible = !EmoteVisible;
        Changed = true;
        actor.OutputHandler.Send($"This power will {EmoteVisible.NowNoLonger()} have a visible emote to others.");
        return true;
    }

    private bool BuildingCommandDelay(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What should the delay in seconds be before the power-user gets the output of the search?");
            return false;
        }

        if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid positive number of seconds.");
            return false;
        }

        CommandDelay = TimeSpan.FromSeconds(value);
        Changed = true;
        actor.OutputHandler.Send($"The delay between using the power and getting the output is now {CommandDelay.DescribePreciseBrief(actor).ColourValue()}.");
        return true;
    }
    private bool BuildingCommandVerb(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which verb should be used to activate this power?");
            return false;
        }

        var verb = command.SafeRemainingArgument.ToLowerInvariant();

        var costs = InvocationCosts[Verb].ToList();
        InvocationCosts[verb] = costs;
        InvocationCosts.Remove(Verb);
        Verb = verb;
        Changed = true;
        actor.OutputHandler.Send($"This magic power will now use the verb {verb.ColourCommand()} to invoke the power.");
        return true;
    }

    private bool BuildingCommandDistance(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"At what distance should this power be able to be used? The valid options are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum(out MagicPowerDistance value))
        {
            actor.OutputHandler.Send($"That is not a valid distance. The valid options are {Enum.GetValues<MagicPowerDistance>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
            return false;
        }

        PowerDistance = value;
        Changed = true;
        actor.OutputHandler.Send($"This magic power can now be used against {value.LongDescription().ColourValue()}.");
        return true;
    }

    private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What is the minimum success threshold for this power to work? See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum(out Outcome value))
        {
            actor.OutputHandler.Send($"That is not a valid outcome. See {"show outcomes".MXPSend("show outcomes")} for a list of valid values.");
            return false;
        }

        MinimumSuccessThreshold = value;
        Changed = true;
        actor.OutputHandler.Send($"The power user will now need to achieve a {value.DescribeColour()} in order to activate this power.");
        return true;
    }

    private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What prog should be used to determine the difficulty of the skill check?");
            return false;
        }

        var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Text,
            [
                [FutureProgVariableTypes.Character],
                [FutureProgVariableTypes.Character, FutureProgVariableTypes.Character]
            ]
        ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        TargetDifficultyProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This power's skill check will now be at a difficulty determined by the prog {prog.MXPClickableFunctionName()}.");
        return true;
    }

    private bool BuildingCommandSkill(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which skill or trait should be used for this power's skill check?");
            return false;
        }

        var skill = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
        if (skill is null)
        {
            actor.OutputHandler.Send("That is not a valid skill or trait.");
            return false;
        }

        SkillCheckTrait = skill;
        Changed = true;
        actor.OutputHandler.Send($"This magic power will now use the {skill.Name.ColourName()} skill for its skill check.");
        return true;
    }
    #endregion Building Subcommands
    #endregion Building Commands
}
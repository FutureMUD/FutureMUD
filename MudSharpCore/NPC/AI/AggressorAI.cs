using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Models;
using MudSharp.Construction;
using MoreLinq;

namespace MudSharp.NPC.AI;

public class AggressorAI : ArtificialIntelligenceBase
{
    public AggressorAI(ArtificialIntelligence ai, IFuturemud gameworld)
        : base(ai, gameworld)
    {
        LoadFromXml(XElement.Parse(ai.Definition));
    }

    protected AggressorAI(IFuturemud gameworld, string name, string type = "Aggressor") : base(gameworld, name, type)
    {
        WillAttackProg = Gameworld.AlwaysFalseProg;
        EngageDelayDiceExpression = "1000+1d1000";
        EngageEmote = string.Empty;
        DatabaseInitialise();
    }

    protected AggressorAI()
    {

    }

    public IFutureProg WillAttackProg { get; set; }
    public uint? AcquisitionRange { get; private set; }
    public string EngageDelayDiceExpression { get; set; }
    public string EngageEmote { get; set; }
	public IFutureProg? OpeningProg { get; private set; }
	public bool RespectDispositionMemory { get; private set; }
	public bool AggressiveClassification { get; private set; } = true;
    public override bool CountsAsAggressive => AggressiveClassification;

    public static void RegisterLoader()
    {
        RegisterAIType("Aggressor", (ai, gameworld) => new AggressorAI(ai, gameworld));
        RegisterAIBuilderInformation("aggressor", (gameworld, name) => new AggressorAI(gameworld, name), new AggressorAI().HelpText);
    }

    private void LoadFromXml(XElement root)
    {
        WillAttackProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillAttackProg").Value));
        AcquisitionRange = uint.TryParse(root.Element("AcquisitionRange")?.Value, out var range) ? range : null;
        EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression").Value;
        EngageEmote = root.Element("EngageEmote")?.Value ?? string.Empty;
		OpeningProg = Gameworld.FutureProgs.Get((long?)root.Element("OpeningProg") ?? 0);
		RespectDispositionMemory = (bool?)root.Element("RespectDispositionMemory") ?? false;
		AggressiveClassification = (bool?)root.Element("AggressiveClassification") ?? true;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("WillAttackProg", WillAttackProg.Id),
			new XElement("OpeningProg", OpeningProg?.Id ?? 0),
			new XElement("RespectDispositionMemory", RespectDispositionMemory),
			new XElement("AggressiveClassification", AggressiveClassification),
            AcquisitionRange.HasValue ? new XElement("AcquisitionRange", AcquisitionRange.Value) : null,
            new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
            new XElement("EngageEmote", new XCData(EngageEmote))
        ).ToString();
    }

    private bool CheckAllTargetsForAttack(ICharacter ch)
    {
        if (ch.State.HasFlag(CharacterState.Dead) || ch.State.HasFlag(CharacterState.Stasis))
        {
            return false;
        }

        if (!CharacterState.Able.HasFlag(ch.State))
        {
            return false;
        }

        if (ch.Combat != null && ch.CombatTarget is ICharacter ctch &&
            PredatorAIHelpers.WillAttack(ch, ctch, WillAttackProg, false))
        {
            return false;
        }

        if (ch.Effects.Any(x => x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")))
        {
            return false;
        }

		if (RespectDispositionMemory && NpcMemoryReactions.TryAvoidFearedCharacter(ch)) return true;

        foreach (ICharacter tch in ch.Location.CharactersInSpatialVicinity(ch).Except(ch).Shuffle(Constants.Random).OrderBy(x => PsychicDispositionQuery.ForDecision(ch, x)))
        {
            if (CheckForAttack(ch, tch))
            {
                return true;
            }
        }

        uint range = AcquisitionRange ?? (uint)ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
                            .Where(x => x.IsReadied || x.CanReady(ch))
                            .Select(x => (int)x.WeaponType.DefaultRangeInRooms)
                            .DefaultIfEmpty(0).Max();
        // TODO - natural attacks
        if (range > 0)
        //TODO: With this, AI can find you through doorways it doesn't have direct LOS into, which doesn't seem fair
        //Worth revisiting at some point.
        {
            foreach (ICharacter tch in ch.Location.RoomsInVicinity(range, true, true).Except(ch.Location)
                                  .SelectMany(x => x.Characters).ToList())
            {
                if (CheckForAttack(ch, tch))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        switch (type)
        {
            case EventType.TenSecondTick:
                ICharacter ch = (ICharacter)arguments[0];
                if (ch.State.IsDead() || ch.State.IsInStatis())
                {
                    return false;
                }
                return CheckAllTargetsForAttack(ch);
            case EventType.CharacterEnterRoomWitness:
                ch = (ICharacter)arguments[3];
                if (ch.State.IsDead() || ch.State.IsInStatis())
                {
                    return false;
                }
                return CheckForAttack(ch, (ICharacter)arguments[0]);
        }

        return false;
    }

    public override bool HandlesEvent(params EventType[] types)
    {
        foreach (EventType type in types)
        {
            switch (type)
            {
                case EventType.CharacterEnterRoomWitness:
                case EventType.TenSecondTick:
                    return true;
            }
        }

        return false;
    }

    public virtual bool CheckForAttack(ICharacter aggressor, ICharacter target)
    {
		if (RespectDispositionMemory && NpcMemoryReactions.SuppressAttack(aggressor, target)) return false;
		if (PsychicDispositionQuery.ForDecision(aggressor, target) > 0 && target.CombatTarget != aggressor) return false;
        return PredatorAIHelpers.CheckForAttack(aggressor, target, WillAttackProg, EngageDelayDiceExpression,
            EngageEmote, false, OpeningProg, RespectDispositionMemory);
    }

    /// <inheritdoc />
    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine($"Type: {AIType.ColourValue()}");
        sb.AppendLine();
        sb.AppendLine($"Will Attack Prog: {WillAttackProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Opening Prog: {OpeningProg?.MXPClickableFunctionName() ?? "None".ColourCommand()}");
		sb.AppendLine($"Respect Disposition Memory: {RespectDispositionMemory.ToColouredString()}");
		sb.AppendLine($"Classified Aggressive: {AggressiveClassification.ToColouredString()}");
        sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} milliseconds");
        sb.AppendLine($"Acquisition Range: {(AcquisitionRange is { } range ? range.ToString("N0", actor).ColourValue() + " rooms" : "Readied weapon range")}");
        sb.AppendLine($"Engage Emote: {EngageEmote?.ColourCommand() ?? ""}");
        return sb.ToString();
    }

    protected override string TypeHelpText => @"	#3attackprog <prog>#0 - sets the prog that controls target selection
	#3openingprog <boolean(character,character) prog|none>#0 - attempts an authored opener; true consumes normal engagement
	#3memory#0 - toggles avoiding remembered fears and refusing attacks on liked or feared characters
	#3aggressive#0 - toggles aggressive classification (does not change the target prog)
	#3range <rooms|weapon>#0 - sets acquisition radius (0 = local; weapon = legacy weapon range)
	#3emote <emote>#0 - sets the engage emote ($0 = npc, $1 = target)
	#3emote clear#0 - clears the emote (won't do an emote when engaging)
	#3delay <dice expression>#0 - sets the delay (in ms) before attacking when spotting a target";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
			case "aggressive":
				AggressiveClassification = !AggressiveClassification;
				Changed = true;
				actor.OutputHandler.Send($"Aggressive classification is now {AggressiveClassification.ToColouredString()}.");
				return true;
			case "memory":
				RespectDispositionMemory = !RespectDispositionMemory;
				Changed = true;
				actor.OutputHandler.Send($"Respecting disposition memory is now {RespectDispositionMemory.ToColouredString()}.");
				return true;
			case "openingprog":
				if (command.SafeRemainingArgument.EqualTo("none")) OpeningProg = null;
				else
				{
					var opening = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
						ProgVariableTypes.Boolean, [ProgVariableTypes.Character, ProgVariableTypes.Character]).LookupProg();
					if (opening is null) return false;
					OpeningProg = opening;
				}
				Changed = true;
				actor.OutputHandler.Send("Opening prog updated.");
				return true;
            case "range":
            case "acquisitionrange":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("Specify an acquisition range in rooms, or weapon for the weapon-derived range.");
                    return false;
                }
                var text = command.PopSpeech();
                if (text.EqualTo("weapon")) AcquisitionRange = null;
                else if (uint.TryParse(text, out var range)) AcquisitionRange = range;
                else
                {
                    actor.OutputHandler.Send("The acquisition range must be a nonnegative whole number of rooms, or weapon.");
                    return false;
                }
                Changed = true;
                actor.OutputHandler.Send("The acquisition range is now " + (AcquisitionRange is { } value ? value.ToString("N0", actor).ColourValue() + " rooms." : "derived from readied weapons."));
                return true;
            case "attackprog":
                return BuildingCommandAttackProg(actor, command);
            case "emote":
            case "engageemote":
            case "attackemote":
                return BuildingCommandEngageEmote(actor, command);
            case "delay":
            case "engagedelay":
            case "attackdelay":
                return BuildingCommandEngageDelay(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandEngageDelay(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must supply a dice expression for a number of milliseconds.");
            return false;
        }

        if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
        {
            actor.OutputHandler.Send(
                $"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
            return false;
        }

        EngageDelayDiceExpression = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The NPC will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before attacking.");
        return true;
    }

    private bool BuildingCommandEngageEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either supply an emote or use {"clear".ColourCommand()} to remove the emote.");
            return false;
        }

        string text = command.SafeRemainingArgument;
        if (text.EqualToAny("remove", "clear", "delete"))
        {
            EngageEmote = string.Empty;
            Changed = true;
            actor.OutputHandler.Send("This NPC will no longer do any emote when engaging targets.");
            return true;
        }

        Emote emote = new(text, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        EngageEmote = text;
        Changed = true;
        actor.OutputHandler.Send($"The NPC will now do the following emote when engaging:\n{text.ColourCommand()}");
        return true;
    }

    private bool BuildingCommandAttackProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which prog should be used to control whether this NPC will attack a target?");
            return false;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
            ProgVariableTypes.Boolean, new List<ProgVariableTypes>
            {
                ProgVariableTypes.Character,
                ProgVariableTypes.Character
            }).LookupProg();
        if (prog is null)
        {
            return false;
        }

        WillAttackProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This NPC will now use the {prog.MXPClickableFunctionName()} prog to determine whether to attack a target.");
        return true;
    }
}

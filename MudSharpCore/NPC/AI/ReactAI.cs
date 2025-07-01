using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.GameItems;

namespace MudSharp.NPC.AI;

public class ReactAI : ArtificialIntelligenceBase
{
    private class Reaction
    {
        public EventType EventType { get; set; }
        public IFutureProg Prog { get; set; } = null!;
        public string Emote { get; set; } = string.Empty;
        public IEnumerable<IEnumerable<ProgVariableTypes>> ProgParams { get; set; } = Enumerable.Empty<IEnumerable<ProgVariableTypes>>();
    }

    private readonly Dictionary<string, Reaction> _reactions = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<EventType, string> _reverseLookup = new();

    public ReactAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
    {
        LoadFromXml(XElement.Parse(ai.Definition));
    }

    private ReactAI() { }

    public ReactAI(IFuturemud gameworld, string name) : base(gameworld, name, "React")
    {
        InitialiseDefaults();
        DatabaseInitialise();
    }

    public static void RegisterLoader()
    {
        RegisterAIType("React", (ai, gameworld) => new ReactAI(ai, gameworld));
        RegisterAIBuilderInformation("react", (gameworld, name) => new ReactAI(gameworld, name), new ReactAI().HelpText);
    }

    private void InitialiseDefaults()
    {
        AddReaction("greet", EventType.CharacterEnterCellFinishWitness, new[]{new[]{ProgVariableTypes.Character, ProgVariableTypes.Character}});
        AddReaction("farewell", EventType.CharacterLeaveCellWitness, new[]{new[]{ProgVariableTypes.Character, ProgVariableTypes.Character}});
        AddReaction("weather", EventType.WeatherChanged, new[]{new[]{ProgVariableTypes.Character, ProgVariableTypes.WeatherEvent, ProgVariableTypes.WeatherEvent}});
        AddReaction("gift", EventType.CharacterGiveItemWitness, new[]{new[]{ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Character}});
        AddReaction("damage", EventType.CharacterDamagedWitness, new[]{new[]{ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Character}});
        AddReaction("hide", EventType.CharacterHidesWitness, new[]{new[]{ProgVariableTypes.Character, ProgVariableTypes.Character}});
    }

    private void AddReaction(string key, EventType type, IEnumerable<IEnumerable<ProgVariableTypes>> param)
    {
        _reactions[key] = new Reaction { EventType = type, Prog = Gameworld.AlwaysTrueProg, Emote = string.Empty, ProgParams = param };
        _reverseLookup[type] = key;
    }

    private void LoadFromXml(XElement root)
    {
        InitialiseDefaults();
        foreach (var rx in root.Elements("Reaction"))
        {
            var key = rx.Attribute("key")?.Value ?? string.Empty;
            if (!_reactions.ContainsKey(key))
            {
                continue;
            }
            var progid = long.Parse(rx.Element("Prog")?.Value ?? "0");
            _reactions[key].Prog = Gameworld.FutureProgs.Get(progid) ?? Gameworld.AlwaysTrueProg;
            _reactions[key].Emote = rx.Element("Emote")?.Value ?? string.Empty;
        }
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            from pair in _reactions
            select new XElement("Reaction",
                new XAttribute("key", pair.Key),
                new XElement("Prog", pair.Value.Prog?.Id ?? 0L),
                new XElement("Emote", new XCData(pair.Value.Emote ?? string.Empty))
            )
        ).ToString();
    }

    protected override string TypeHelpText => @"    #3emote <reaction> <emote>#0 - sets an emote for the reaction
        #3emote <reaction> clear#0 - clears the emote
        #3prog <reaction> <prog>#0 - sets a prog controlling the reaction
Valid reactions: greet, farewell, weather, gift, damage, hide";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "emote":
                return BuildingCommandEmote(actor, command);
            case "prog":
                return BuildingCommandProg(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a reaction and an emote, or 'clear'.");
            return false;
        }
        var reaction = command.PopSpeech();
        if (!_reactions.ContainsKey(reaction))
        {
            actor.OutputHandler.Send("That is not a valid reaction.");
            return false;
        }
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must supply an emote or 'clear'.");
            return false;
        }
        if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
        {
            _reactions[reaction].Emote = string.Empty;
            Changed = true;
            actor.OutputHandler.Send($"The {reaction} reaction will no longer emote.");
            return true;
        }

        var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }
        _reactions[reaction].Emote = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"The {reaction} reaction will now emote: {command.SafeRemainingArgument.ColourCommand()}");
        return true;
    }

    private bool BuildingCommandProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a reaction and a prog.");
            return false;
        }
        var reaction = command.PopSpeech();
        if (!_reactions.ContainsKey(reaction))
        {
            actor.OutputHandler.Send("That is not a valid reaction.");
            return false;
        }
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which prog should be used?");
            return false;
        }
        var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, _reactions[reaction].ProgParams).LookupProg();
        if (prog is null)
        {
            return false;
        }
        _reactions[reaction].Prog = prog;
        Changed = true;
        actor.OutputHandler.Send($"The {reaction} reaction will now use the prog {prog.MXPClickableFunctionName()}.");
        return true;
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        if (!_reverseLookup.TryGetValue(type, out var key))
        {
            return false;
        }
        var reaction = _reactions[key];
        switch (type)
        {
            case EventType.CharacterEnterCellFinishWitness:
                var moverIn = arguments[0] as ICharacter;
                var witnessIn = arguments[3] as ICharacter;
                if (witnessIn?.Id != Id || moverIn == witnessIn)
                    return false;
                if (reaction.Prog?.ExecuteBool(witnessIn, moverIn) == false)
                    return false;
                if (!string.IsNullOrWhiteSpace(reaction.Emote))
                    witnessIn.OutputHandler.Handle(new EmoteOutput(new Emote(reaction.Emote, witnessIn, witnessIn, moverIn)));
                return true;
            case EventType.CharacterLeaveCellWitness:
                var moverOut = arguments[0] as ICharacter;
                var witnessOut = arguments[3] as ICharacter;
                if (witnessOut?.Id != Id || moverOut == witnessOut)
                    return false;
                if (reaction.Prog?.ExecuteBool(witnessOut, moverOut) == false)
                    return false;
                if (!string.IsNullOrWhiteSpace(reaction.Emote))
                    witnessOut.OutputHandler.Handle(new EmoteOutput(new Emote(reaction.Emote, witnessOut, witnessOut, moverOut)));
                return true;
            case EventType.WeatherChanged:
                var ch = arguments[0] as ICharacter;
                if (ch?.Id != Id)
                    return false;
                if (reaction.Prog?.ExecuteBool(ch, arguments[1], arguments[2]) == false)
                    return false;
                if (!string.IsNullOrWhiteSpace(reaction.Emote))
                    ch.OutputHandler.Handle(new EmoteOutput(new Emote(reaction.Emote, ch)));
                return true;
            case EventType.CharacterGiveItemWitness:
                var giver = arguments[0] as ICharacter;
                var receiver = arguments[1] as ICharacter;
                var item = arguments[2] as IGameItem;
                var witnessGift = arguments[3] as ICharacter;
                if (witnessGift?.Id != Id)
                    return false;
                if (reaction.Prog?.ExecuteBool(witnessGift, giver, receiver, item) == false)
                    return false;
                if (!string.IsNullOrWhiteSpace(reaction.Emote))
                    witnessGift.OutputHandler.Handle(new EmoteOutput(new Emote(reaction.Emote, witnessGift, witnessGift, giver)));
                return true;
            case EventType.CharacterDamagedWitness:
                var victim = arguments[0] as ICharacter;
                var weapon = arguments[1] as IGameItem;
                var aggressor = arguments[2] as ICharacter;
                var witnessDamage = arguments[3] as ICharacter;
                if (witnessDamage?.Id != Id)
                    return false;
                if (reaction.Prog?.ExecuteBool(witnessDamage, victim, weapon, aggressor) == false)
                    return false;
                if (!string.IsNullOrWhiteSpace(reaction.Emote))
                    witnessDamage.OutputHandler.Handle(new EmoteOutput(new Emote(reaction.Emote, witnessDamage, witnessDamage, victim)));
                return true;
            case EventType.CharacterHidesWitness:
                var hidden = arguments[0] as ICharacter;
                var witnessHide = arguments[1] as ICharacter;
                if (witnessHide?.Id != Id)
                    return false;
                if (reaction.Prog?.ExecuteBool(witnessHide, hidden) == false)
                    return false;
                if (!string.IsNullOrWhiteSpace(reaction.Emote))
                    witnessHide.OutputHandler.Handle(new EmoteOutput(new Emote(reaction.Emote, witnessHide, witnessHide, hidden)));
                return true;
        }
        return false;
    }

    public override bool HandlesEvent(params EventType[] types)
    {
        return types.Any(t => _reverseLookup.ContainsKey(t));
    }
}

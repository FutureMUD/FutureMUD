using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class IdleEmoterAI : ArtificialIntelligenceBase
{
    private readonly List<string> _emotes = new();
    public IEnumerable<string> Emotes => _emotes;
    public string EmoteDelayDiceExpression { get; set; }

    public IdleEmoterAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
    {
        LoadFromXml(XElement.Parse(ai.Definition));
    }

    private IdleEmoterAI() { }

    public IdleEmoterAI(IFuturemud gameworld, string name) : base(gameworld, name, "IdleEmoter")
    {
        EmoteDelayDiceExpression = "60+1d60";
        DatabaseInitialise();
    }

    public static void RegisterLoader()
    {
        RegisterAIType("IdleEmoter", (ai, gameworld) => new IdleEmoterAI(ai, gameworld));
        RegisterAIBuilderInformation("idleemoter", (gameworld, name) => new IdleEmoterAI(gameworld, name), new IdleEmoterAI().HelpText);
    }

    private void LoadFromXml(XElement root)
    {
        EmoteDelayDiceExpression = root.Element("EmoteDelayDiceExpression")?.Value ?? "60+1d60";
        _emotes.AddRange(root.Elements("Emote").Select(x => x.Value));
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
                new XElement("EmoteDelayDiceExpression", new XCData(EmoteDelayDiceExpression)),
                from emote in Emotes select new XElement("Emote", new XCData(emote))
            ).ToString();
    }

    protected override string TypeHelpText => @"    #3delay <expression>#0 - sets delay between idle emotes in seconds
        #3addemote <emote>#0 - adds an emote to the idle list
        #3rememote <##>#0 - removes an emote from the list";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "delay":
                return BuildingCommandDelay(actor, command);
            case "addemote":
                return BuildingCommandAddEmote(actor, command);
            case "rememote":
            case "deleteemote":
            case "removeemote":
                return BuildingCommandRemoveEmote(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandDelay(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must enter a valid dice expression for seconds between idle emotes.");
            return false;
        }
        if (!Dice.IsDiceExpression(command.SafeRemainingArgument))
        {
            actor.OutputHandler.Send($"{command.SafeRemainingArgument.ColourCommand()} is not a valid dice expression.");
            return false;
        }
        EmoteDelayDiceExpression = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now emote every {EmoteDelayDiceExpression.ColourValue()} seconds when idle.");
        return true;
    }

    private bool BuildingCommandAddEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What emote do you want to add?");
            return false;
        }
        var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }
        _emotes.Add(command.SafeRemainingArgument);
        Changed = true;
        actor.OutputHandler.Send($"Emote {command.SafeRemainingArgument.ColourCommand()} added to idle list.");
        return true;
    }

    private bool BuildingCommandRemoveEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var index) || index < 1 || index > _emotes.Count)
        {
            actor.OutputHandler.Send($"You must specify a valid emote number between 1 and {_emotes.Count.ToString("N0", actor)}.");
            return false;
        }
        actor.OutputHandler.Send($"Emote {_emotes[index-1].ColourCommand()} removed from list.");
        _emotes.RemoveAt(index-1);
        Changed = true;
        return true;
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        if (type != EventType.TenSecondTick)
        {
            return false;
        }
        var ch = arguments[0] as ICharacter;
        if (ch is null || ch.Id != Id)
        {
            return false;
        }
        if (!IsGenerallyAble(ch))
        {
            return false;
        }
        if (!_emotes.Any())
        {
            return false;
        }
        if (ch.EffectsOfType<DelayedAction>().Any(x => x.ActionDescription == "idle emote"))
        {
            return false;
        }
        if (ch.Movement != null || ch.Combat != null)
        {
            return false;
        }
        ch.AddEffect(new DelayedAction(ch, x =>
        {
            var emote = _emotes.GetRandomElement();
            ch.OutputHandler.Handle(new EmoteOutput(new Emote(emote, ch)));
        }, "idle emote"), TimeSpan.FromSeconds(Dice.Roll(EmoteDelayDiceExpression)));
        return false;
    }

    public override bool HandlesEvent(params EventType[] types)
    {
        return types.Contains(EventType.TenSecondTick);
    }
}

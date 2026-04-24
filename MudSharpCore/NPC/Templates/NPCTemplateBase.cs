using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EditableItem = MudSharp.Framework.Revision.EditableItem;

namespace MudSharp.NPC.Templates;

public abstract class NPCTemplateBase : EditableItem, INPCTemplate
{
    protected NPCTemplateBase(NpcTemplate template, IFuturemud gameworld) : base(template.EditableItem)
    {
        Gameworld = gameworld;
        _id = template.Id;
        _name = template.Name;
        RevisionNumber = template.RevisionNumber;
        XElement definition = XElement.Parse(template.Definition);
        XElement element = definition.Element("OnLoadProg");
        if (element != null)
        {
            OnLoadProg = gameworld.FutureProgs.Get(long.Parse(element.Value));
        }

        element = definition.Element("HealthStrategy");
        if (element != null)
        {
            HealthStrategy = gameworld.HealthStrategies.Get(long.Parse(element.Value));
        }

        element = definition.Element("DefaultCombatSetting");
        if (element != null)
        {
            DefaultCombatSetting = gameworld.CharacterCombatSettings.Get(long.Parse(element.Value));
        }

        foreach (NpcTemplatesArtificalIntelligences ai in template.NpctemplatesArtificalIntelligences)
        {
            ArtificialIntelligences.Add(Gameworld.AIs.Get(ai.AiId));
        }
    }

    protected NPCTemplateBase(IFuturemud gameworld, IAccount originator, string type) : base(originator)
    {
        Gameworld = gameworld;
        _name = "Unnamed NPC Template";
        using (new FMDB())
        {
            NpcTemplate dbnew = new()
            {
                Id = Gameworld.NpcTemplates.NextID(),
                Name = _name,
                Type = type,
                Definition = "<Definition/>"
            };
            FMDB.Context.NpcTemplates.Add(dbnew);
            dbnew.EditableItem = new Models.EditableItem();
            dbnew.RevisionNumber = RevisionNumber;
            dbnew.EditableItem.BuilderAccountId = BuilderAccountID;
            dbnew.EditableItem.BuilderDate = BuilderDate;
            dbnew.EditableItem.RevisionStatus = (int)Status;
            FMDB.Context.EditableItems.Add(dbnew.EditableItem);
            FMDB.Context.SaveChanges();
            _id = dbnew.Id;
        }
    }

    protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
    {
        return Gameworld.NpcTemplates.GetAll(Id);
    }

    public abstract string HelpText { get; }

    public abstract string ReferenceDescription(IPerceiver voyeur);

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "ai":
                return BuildingCommandAI(actor, command);
            case "onload":
            case "onloadprog":
                return BuildingCommandOnLoadProg(actor, command);
            case "healthstrategy":
            case "health":
                return BuildingCommandHealthStrategy(actor, command);
            case "combatsetting":
            case "combat":
                return BuildingCommandCombatSetting(actor, command);
            default:
                actor.OutputHandler.Send($@"{HelpText}
	#3onload <prog>#0 - adds an onload prog to this NPC
	#3onload clear#0 - clears an onload prog
	#3combatsetting <setting>#0 - sets the default combat setting for this NPC
	#3combatsetting none#0 - clears the combat setting override
	#3ai add <which>#0 - adds an AI routine to this NPC
	#3ai remove <which>#0 - removes an AI routine from this NPC".SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandHealthStrategy(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You can either specify a health strategy, or use #3none#0 to reset to default.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            HealthStrategy = null;
            Changed = true;
            actor.OutputHandler.Send($"This NPC Template will now just use the default health strategy for the character.");
            return true;
        }

        IHealthStrategy strategy = Gameworld.HealthStrategies.GetByIdOrName(command.SafeRemainingArgument);
        if (strategy is null)
        {
            actor.OutputHandler.Send($"There is no health strategy identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        HealthStrategy = strategy;
        Changed = true;
        actor.OutputHandler.Send($"This NPC Template will now use the {strategy.Name.ColourName()} health strategy instead of its default.");
        return true;
    }

    public List<IArtificialIntelligence> ArtificialIntelligences { get; } = new();

    public static INPCTemplate LoadTemplate(NpcTemplate template, IFuturemud gameworld)
    {
        switch (template.Type)
        {
            case "Simple":
                return new SimpleNPCTemplate(template, gameworld);
            case "Variable":
                return new VariableNPCTemplate(template, gameworld);
            default:
                throw new NotSupportedException();
        }
    }

    private bool BuildingCommandCombatSetting(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"You must either specify a global combat setting, or use {"none".ColourCommand()} to clear the override.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            DefaultCombatSetting = null;
            Changed = true;
            actor.OutputHandler.Send("This NPC Template will now use racial or global combat-setting defaults.");
            return true;
        }

        ICharacterCombatSettings setting = long.TryParse(command.SafeRemainingArgument, out long value)
            ? Gameworld.CharacterCombatSettings.Get(value)
            : Gameworld.CharacterCombatSettings.GetByName(command.SafeRemainingArgument);
        if (setting is null)
        {
            actor.OutputHandler.Send($"There is no combat setting identified by {command.SafeRemainingArgument.ColourCommand()}.");
            return false;
        }

        if (!setting.GlobalTemplate)
        {
            actor.OutputHandler.Send("NPC Templates can only use global combat settings as defaults.");
            return false;
        }

        DefaultCombatSetting = setting;
        Changed = true;
        actor.OutputHandler.Send($"This NPC Template will now use {setting.Name.ColourName()} as its default combat setting.");
        return true;
    }

    private bool BuildingCommandOnLoadProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send(
                "What do you want to do with the OnLoad Prog for this NPC? You can either clear it, or specify one to set.");
            return true;
        }

        if (command.PeekSpeech().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            OnLoadProg = null;
            Changed = true;
            actor.Send("This NPC Template will now not use any prog when the template is loaded.");
            return true;
        }

        IFutureProg prog = long.TryParse(command.PopSpeech(), out long value)
            ? Gameworld.FutureProgs.Get(value)
            : Gameworld.FutureProgs.FirstOrDefault(
                x => x.FunctionName.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
        if (prog == null)
        {
            actor.Send("There is no such prog for you to set this NPC Template to.");
            return true;
        }

        if (!prog.MatchesParameters(new List<ProgVariableTypes>
            {
                ProgVariableTypes.Character
            }))
        {
            actor.Send("The prog must match a single character parameter.");
            return true;
        }

        OnLoadProg = prog;
        Changed = true;
        actor.Send("You set the OnLoadProg for this NPC Template to {0} (#{1}).", prog.FunctionName, prog.Id);
        return true;
    }

    private bool BuildingCommandAI(ICharacter actor, StringStack command)
    {
        bool add = false;
        switch (command.PopSpeech())
        {
            case "add":
                add = true;
                break;
            case "remove":
                break;
            default:
                actor.OutputHandler.Send("Do you want to add or remove an AI?");
                return true;
        }

        IArtificialIntelligence ai = long.TryParse(command.PopSpeech(), out long value)
            ? Gameworld.AIs.Get(value)
            : Gameworld.AIs.FirstOrDefault(
                x => x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));

        if (ai == null)
        {
            actor.OutputHandler.Send("There is no such AI.");
            return true;
        }

        if (add)
        {
            if (ArtificialIntelligences.Contains(ai))
            {
                actor.OutputHandler.Send("This NPC already has that AI Routine.");
                return true;
            }

            ArtificialIntelligences.Add(ai);
        }
        else
        {
            if (!ArtificialIntelligences.Contains(ai))
            {
                actor.OutputHandler.Send("This NPC does not have that AI Routine.");
                return true;
            }

            ArtificialIntelligences.Remove(ai);
        }

        Changed = true;
        actor.OutputHandler.Send(
            $"You {(add ? "add" : "remove")} AI Routine #{ai.Id} ({ai.Name}) {(add ? "to" : "from")} this NPC.");
        return true;
    }

    protected abstract ICharacterTemplate CharacterTemplate(ICell location);

    protected static List<int> RollRandomStats(int numberOfStats, int totalCap, int individualCap,
        string diceExpression)
    {
        List<int> results = new();
        for (int i = 0; i < numberOfStats; i++)
        {
            results.Add(Dice.Roll(diceExpression));
        }

        BalanceRandomStats(results, totalCap, individualCap);
        results.Sort();
        results.Reverse();
        return results;
    }

    protected static List<int> RollRandomStats(IReadOnlyList<IAttributeDefinition> attributes, IRace race,
        int totalCap, int individualCap)
    {
        List<string> diceExpressions = attributes
            .Select(race.AttributeDiceExpression)
            .ToList();
        if (diceExpressions.Distinct(StringComparer.InvariantCultureIgnoreCase).Count() == 1)
        {
            return RollRandomStats(attributes.Count, totalCap, individualCap, diceExpressions[0]);
        }

        List<int> results = diceExpressions.Select(Dice.Roll).ToList();
        BalanceRandomStats(results, totalCap, individualCap);
        return results;
    }

    private static void BalanceRandomStats(List<int> results, int totalCap, int individualCap)
    {
        int difference = totalCap - results.Sum();
        if (difference < 0)
        {
            for (int i = 0; i > difference; i--)
            {
                int whichStat = Constants.Random.Next(0, results.Count);
                results[whichStat] -= 1;
            }
        }
        else if (difference > 0)
        {
            for (int i = 0; i < difference; i++)
            {
                int whichStat = Constants.Random.Next(0, results.Count);
                if (results[whichStat] == individualCap)
                {
                    i--;
                    continue;
                }

                results[whichStat] += 1;
            }
        }

        while (results.Any(x => x > individualCap))
        {
            int whichStat = results.FindIndex(x => x > individualCap);
            results[whichStat] -= 1;
            whichStat = Constants.Random.Next(0, results.Count);
            results[whichStat] += 1;
        }
    }

    #region INPCTemplate Members

    public ICharacterTemplate GetCharacterTemplate(ICell cell = null)
    {
        return CharacterTemplate(cell);
    }

    public ICharacter CreateNewCharacter(ICell location)
    {
        ICharacterTemplate template = CharacterTemplate(location);
        NPC npc = new(Gameworld, template, this);
        return npc;
    }

    public abstract string NPCTemplateType { get; }

    public IFutureProg? OnLoadProg { get; set; }
    public IHealthStrategy? HealthStrategy { get; set; }
    public ICharacterCombatSettings? DefaultCombatSetting { get; set; }

    public abstract INPCTemplate Clone(ICharacter builder);

    #endregion
}

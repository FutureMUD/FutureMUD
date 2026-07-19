using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.NPC.AI;
using EditableItem = MudSharp.Framework.Revision.EditableItem;

#nullable enable
#nullable disable warnings

namespace MudSharp.NPC.Templates;

public abstract partial class NPCTemplateBase : EditableItem, INPCTemplate
{
    protected NPCTemplateBase(NpcTemplate template, IFuturemud gameworld) : base(template.EditableItem)
    {
        Gameworld = gameworld;
        _id = template.Id;
        _name = template.Name;
        UniqueName = NPCTemplateLookupExtensions.NormaliseUniqueName(template.UniqueName);
        BuilderNotes = string.IsNullOrWhiteSpace(template.BuilderNotes) ? null : template.BuilderNotes;
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

        LoadTemplateLoadAdditions(definition);

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
                Definition = "<Definition/>",
                UniqueName = null,
                BuilderNotes = null
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
            case "unique":
            case "uniquename":
            case "key":
                return BuildingCommandUniqueName(actor, command);
            case "comment":
            case "notes":
            case "buildercomment":
            case "buildernotes":
                return BuildingCommandBuilderNotes(actor, command);
            case "clan":
                return BuildingCommandClan(actor, command);
            case "outfit":
                return BuildingCommandOutfit(actor, command);
            case "hook":
                return BuildingCommandHook(actor, command);
            case "bank":
            case "account":
            case "bankaccount":
                return BuildingCommandBank(actor, command);
            case "implant":
                return BuildingCommandImplant(actor, command);
            case "prosthetic":
                return BuildingCommandProsthetic(actor, command);
            default:
                actor.OutputHandler.Send($@"{HelpText}
	#3unique <name>#0 - sets a unique lookup name for this NPC template
	#3unique clear#0 - clears the unique lookup name
	#3comment <text>#0 - overwrites the builder comment
	#3comment append <text>#0 - appends to the builder comment
	#3comment edit#0 - edits the builder comment in the editor
	#3comment clear#0 - clears the builder comment
	#3onload <prog>#0 - adds an onload prog to this NPC
	#3onload clear#0 - clears an onload prog
	#3combatsetting <setting>#0 - sets the default combat setting for this NPC
	#3combatsetting none#0 - clears the combat setting override
	#3ai add <which>#0 - adds an AI routine to this NPC
	#3ai remove <which>#0 - removes an AI routine from this NPC
	#3clan add <clan> <rank> [paygrade <paygrade>]#0 - adds a load-time clan membership
	#3clan remove <clan>#0 - removes a load-time clan membership
	#3clan appointment add <clan> <appointment>#0 - adds a load-time clan appointment
	#3clan appointment remove <clan> <appointment>#0 - removes a load-time clan appointment
	#3outfit add <template> [name <name>]#0 - materialises an outfit template when loaded
	#3outfit remove <template|#>#0 - removes a load-time outfit template
	#3hook add <hook>#0 - installs a hook when loaded
	#3hook remove <hook>#0 - removes a load-time hook
	#3bank add <type> [balance <amount>] [name <name>]#0 - creates a bank account when loaded
	#3bank remove <type|#>#0 - removes a load-time bank account
	#3implant add <key> <proto> [bodypart <part>]#0 - installs an implant item when loaded
	#3implant remove <key>#0 - removes a load-time implant
	#3implant power <key> <power-key|none>#0 - configures implant power
	#3implant neural <key> <neural-key|none>#0 - configures implant neural control
	#3prosthetic add <key> <proto>#0 - installs a prosthetic item when loaded
	#3prosthetic remove <key>#0 - removes a load-time prosthetic".SubstituteANSIColour());
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

    private bool BuildingCommandUniqueName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What unique name do you want to set for this NPC template?");
            return false;
        }

        var uniqueName = NPCTemplateLookupExtensions.NormaliseUniqueName(command.SafeRemainingArgument);
        if (uniqueName is null || uniqueName.EqualToAny("none", "clear", "delete", "remove"))
        {
            UniqueName = null;
            Changed = true;
            actor.OutputHandler.Send("This NPC template no longer has a unique lookup name.");
            return true;
        }

        if (!NPCTemplateLookupExtensions.IsValidUniqueName(uniqueName))
        {
            actor.OutputHandler.Send("Unique names cannot be entirely numeric, because numeric NPC template input is reserved for IDs.");
            return false;
        }

        var conflict = Gameworld.NpcTemplates.GetActiveUniqueNameConflict(uniqueName, Id);
        if (conflict is not null)
        {
            actor.OutputHandler.Send(
                $"The unique name {uniqueName.ColourCommand()} is already used by {conflict.EditHeader().ColourName()}.");
            return false;
        }

        UniqueName = uniqueName;
        Changed = true;
        actor.OutputHandler.Send($"This NPC template can now be looked up by the unique name {UniqueName.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandBuilderNotes(ICharacter actor, StringStack command)
    {
        var subCommand = command.PopSpeech();
        if (subCommand.Length == 0)
        {
            actor.OutputHandler.Send("Do you want to set, append, edit or clear the builder comment?");
            return false;
        }

        switch (subCommand.ToLowerInvariant())
        {
            case "clear":
            case "none":
            case "delete":
            case "remove":
                BuilderNotes = null;
                Changed = true;
                actor.OutputHandler.Send("You clear the builder comment for this NPC template.");
                return true;
            case "edit":
                actor.OutputHandler.Send("Enter the builder comment in the editor below.");
                actor.EditorMode(BuildingCommandBuilderNotesPost, BuildingCommandBuilderNotesCancel, 1.0,
                    BuilderNotes, suppliedArguments: [false]);
                return true;
            case "append":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("Enter the text to append to the builder comment in the editor below.");
                    actor.EditorMode(BuildingCommandBuilderNotesPost, BuildingCommandBuilderNotesCancel, 1.0,
                        null, suppliedArguments: [true]);
                    return true;
                }

                AppendBuilderNotes(command.SafeRemainingArgument);
                actor.OutputHandler.Send("You append to the builder comment for this NPC template.");
                return true;
        }

        var text = command.IsFinished ? subCommand : $"{subCommand} {command.SafeRemainingArgument}";
        BuilderNotes = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        Changed = true;
        actor.OutputHandler.Send("You set the builder comment for this NPC template.");
        return true;
    }

    private void BuildingCommandBuilderNotesPost(string text, IOutputHandler handler, object[] arguments)
    {
        var append = (bool)arguments[0];
        if (append)
        {
            AppendBuilderNotes(text);
            handler.Send("You append to the builder comment for this NPC template.");
            return;
        }

        BuilderNotes = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        Changed = true;
        handler.Send("You set the builder comment for this NPC template.");
    }

    private void BuildingCommandBuilderNotesCancel(IOutputHandler handler, object[] arguments)
    {
        handler.Send("You decide not to change the builder comment.");
    }

    private void AppendBuilderNotes(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.Length == 0)
        {
            return;
        }

        BuilderNotes = string.IsNullOrWhiteSpace(BuilderNotes)
            ? trimmed
            : $"{BuilderNotes.TrimEnd()}\n{trimmed}";
        Changed = true;
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

    public string? UniqueName { get; protected set; }

    public string? BuilderNotes { get; protected set; }

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

    public override bool CanSubmit()
    {
        return NPCTemplateLookupExtensions.IsValidUniqueName(UniqueName) &&
               Gameworld.NpcTemplates.GetActiveUniqueNameConflict(UniqueName, Id) is null;
    }

    public override string WhyCannotSubmit()
    {
        if (!NPCTemplateLookupExtensions.IsValidUniqueName(UniqueName))
        {
            return "The unique name cannot be entirely numeric, because numeric NPC template input is reserved for IDs.";
        }

        var uniqueNameConflict = Gameworld.NpcTemplates.GetActiveUniqueNameConflict(UniqueName, Id);
        if (uniqueNameConflict is not null)
        {
            return
                $"The unique name {UniqueName!.ColourCommand()} is already used by {uniqueNameConflict.EditHeader().ColourName()}.";
        }

        return base.WhyCannotSubmit();
    }
}

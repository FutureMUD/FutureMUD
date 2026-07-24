using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;

#nullable enable annotations

namespace MudSharp.GameItems.Prototypes;

public class ContainerGameItemComponentProto : GameItemComponentProto, IContainerPrototype, IOpenablePrototype, ILockablePrototype
{
    /// <summary>
    ///     The total allowable weight that can be contained by this container
    /// </summary>
    public double WeightLimit { get; protected set; } = 1000;

    /// <summary>
    ///     The maximum SizeCategory of item that may be contained by this container
    /// </summary>
    public SizeCategory MaximumContentsSize { get; protected set; } = SizeCategory.Small;

    /// <summary>
    ///     Usually either "in" or "on"
    /// </summary>
    public string ContentsPreposition { get; protected set; } = "in";

    /// <summary>
    ///     Whether or not this container can be opened and closed
    /// </summary>
    public bool Closable { get; protected set; } = true;

    public bool Transparent { get; protected set; } = false;
    public List<ITag> AllowedTags { get; } = [];
    public List<ITag> BlockedTags { get; } = [];

    /// <summary>
    ///     A container that is OnceOnly can only be opened once - once opened, it can never be closed again
    /// </summary>
    public bool OnceOnly { get; protected set; } = false;

    public override bool WarnBeforePurge => true;

    public override string TypeDescription => "Container";

    protected override void LoadFromXml(XElement root)
    {
        XAttribute attr = root.Attribute("Weight");
        if (attr != null)
        {
            WeightLimit = double.Parse(attr.Value);
        }

        attr = root.Attribute("MaxSize");
        if (attr != null)
        {
            MaximumContentsSize = (SizeCategory)int.Parse(attr.Value);
        }

        attr = root.Attribute("Preposition");
        if (attr != null)
        {
            ContentsPreposition = attr.Value;
        }

        attr = root.Attribute("Closable");
        if (attr != null)
        {
            Closable = bool.Parse(attr.Value);
        }

        attr = root.Attribute("Transparent");
        if (attr != null)
        {
            Transparent = bool.Parse(attr.Value);
        }

        attr = root.Attribute("OnceOnly");
        if (attr != null)
        {
            OnceOnly = bool.Parse(attr.Value);
        }

        AllowedTags.Clear();
        AllowedTags.AddRange(root.Element("AllowedTags")?.Elements("Tag")
            .Select(x => Gameworld.Tags.Get(long.Parse(x.Value)))
            .Where(x => x is not null)
            .Select(x => x!)
            ?? Enumerable.Empty<ITag>());
        BlockedTags.Clear();
        BlockedTags.AddRange(root.Element("BlockedTags")?.Elements("Tag")
            .Select(x => Gameworld.Tags.Get(long.Parse(x.Value)))
            .Where(x => x is not null)
            .Select(x => x!)
            ?? Enumerable.Empty<ITag>());
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("container", true,
            (gameworld, account) => new ContainerGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("Container",
            (proto, gameworld) => new ContainerGameItemComponentProto(proto, gameworld));

        manager.AddTypeHelpInfo(
            "Container",
            $"Makes an item into a {"[container]".Colour(Telnet.BoldGreen)}",
            BuildingHelpText
        );
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new ContainerGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new ContainerGameItemComponent(component, this, parent);
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new ContainerGameItemComponentProto(proto, gameworld));
    }

    protected override string SaveToXml()
    {
        return
            new XElement("Definition", new XAttribute("Weight", WeightLimit),
                new XAttribute("MaxSize", (int)MaximumContentsSize),
                new XAttribute("Preposition", ContentsPreposition), new XAttribute("Closable", Closable),
                new XAttribute("Transparent", Transparent), new XAttribute("OnceOnly", OnceOnly),
                new XElement("AllowedTags", AllowedTags.Select(x => new XElement("Tag", x.Id))),
                new XElement("BlockedTags", BlockedTags.Select(x => new XElement("Tag", x.Id)))).ToString();
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return $@"{"Container Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

This item can contain {Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor)} and up to {MaximumContentsSize.ToString().Colour(Telnet.Cyan)} size objects. 
It {(Transparent ? "is" : "is not")} transparent and {(Closable ? OnceOnly ? "can only be opened a single time" : "can be opened and closed" : "cannot be opened and closed")}.
Allowed Tags: {(AllowedTags.Any() ? AllowedTags.Select(x => x.Name.ColourName()).ListToString() : "Any".ColourValue())}
Blocked Tags: {(BlockedTags.Any() ? BlockedTags.Select(x => x.Name.ColourName()).ListToString() : "None".ColourValue())}";
    }

    private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
    {
        Transparent = !Transparent;
        actor.Send("This container is {0} transparent.", Transparent ? "now" : "no longer");
        Changed = true;
        return true;
    }

    private bool BuildingCommand_Closable(ICharacter actor, StringStack command)
    {
        Closable = !Closable;
        actor.OutputHandler.Send("This container is " + (Closable ? "now" : "no longer") + " able to be closed.");
        Changed = true;
        return true;
    }

    public bool BuildingCommand_OnceOnly(ICharacter actor, StringStack command)
    {
        OnceOnly = !OnceOnly;
        actor.OutputHandler.Send("This container is " + (OnceOnly ? "now" : "no longer") +
                                 " openable only a single time.");
        Changed = true;
        return true;
    }

    private bool BuildingCommand_WeightLimit(ICharacter actor, StringStack command)
    {
        string weightCmd = command.SafeRemainingArgument;
        double result = actor.Gameworld.UnitManager.GetBaseUnits(weightCmd, UnitType.Mass, out bool success);
        if (success)
        {
            WeightLimit = result;
            actor.OutputHandler.Send(
                $"This container will now hold {actor.Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).ColourValue()}.");
            return true;
        }

        actor.OutputHandler.Send("That is not a valid weight.");
        return false;
    }

    private bool BuildingCommand_MaxSize(ICharacter actor, StringStack command)
    {
        string cmd = command.PopSpeech().ToLowerInvariant();
        if (cmd.Length == 0)
        {
            actor.OutputHandler.Send("What size do you want to set the limit for this component to?");
            return false;
        }

        List<SizeCategory> size = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().ToList();
        SizeCategory target;
        if (size.Any(x => x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal)))
        {
            target = size.FirstOrDefault(x =>
                x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
        }
        else
        {
            actor.OutputHandler.Send("That is not a valid item size. See SHOW ITEMSIZES for a correct list.");
            return false;
        }

        MaximumContentsSize = target;
        Changed = true;
        actor.OutputHandler.Send("This container will now only take items of up to size \"" + target.Describe() +
                                 "\".");
        return true;
    }

    private bool BuildingCommand_Preposition(ICharacter actor, StringStack command)
    {
        string preposition = command.PopSpeech().ToLowerInvariant();
        if (string.IsNullOrEmpty(preposition))
        {
            actor.OutputHandler.Send("What preposition do you want to use for this container?");
            return false;
        }

        ContentsPreposition = preposition;
        Changed = true;
        actor.OutputHandler.Send("The contents of this container will now be described as \"" + ContentsPreposition +
                                 "\" it.");
        return true;
    }

    private const string BuildingHelpText =
        @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3close#0 - toggles whether this container opens and closes
	#3size <max size>#0 - sets the maximum size of the objects that can be put in this container
	#3weight#0 - sets the maximum weight of items this container can hold
	#3transparent#0 - toggles whether you can see the contents when closed
	#3once#0 - toggles whether this container only opens once
	#3preposition <on|in|etc>#0 - sets the preposition used to display contents. Usually on or in.
	#3allow <tag>#0 - toggles an allowed contents tag
	#3allow add|remove <tag>#0 - explicitly adds or removes an allowed contents tag
	#3allow clear#0 - clears the allow list
	#3block <tag>#0 - toggles a blocked contents tag
	#3block add|remove <tag>#0 - explicitly adds or removes a blocked contents tag
	#3block clear#0 - clears the block list";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "close":
            case "openable":
            case "closable":
                return BuildingCommand_Closable(actor, command);
            case "capacity":
            case "weight":
            case "weight limit":
            case "weight capacity":
            case "limit":
                return BuildingCommand_WeightLimit(actor, command);
            case "once":
            case "onceonly":
                return BuildingCommand_OnceOnly(actor, command);
            case "maximum size":
            case "max size":
            case "maxsize":
            case "size":
                return BuildingCommand_MaxSize(actor, command);
            case "preposition":
                return BuildingCommand_Preposition(actor, command);
            case "transparent":
                return BuildingCommand_Transparent(actor, command);
            case "allow":
            case "allowed":
                return BuildingCommandTag(actor, command, AllowedTags, "allowed");
            case "block":
            case "blocked":
                return BuildingCommandTag(actor, command, BlockedTags, "blocked");
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandTag(ICharacter actor, StringStack command, List<ITag> tags, string name)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which tag should be toggled in the {name} list? Use {"clear".ColourCommand()} to clear it.");
            return false;
        }

        var action = command.PeekSpeech();
        if (action.EqualTo("clear"))
        {
            command.PopSpeech();
            tags.Clear();
            Changed = true;
            actor.OutputHandler.Send($"The {name} contents-tag list is now clear.");
            return true;
        }

        var explicitAdd = action.EqualTo("add");
        var explicitRemove = action.EqualToAny("remove", "delete");
        if (explicitAdd || explicitRemove)
        {
            command.PopSpeech();
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which tag should be {(explicitRemove ? "removed from" : "added to")} the {name} list?");
            return false;
        }

        var tagText = command.SafeRemainingArgument;
        var tag = long.TryParse(tagText, out var value)
            ? Gameworld.Tags.Get(value)
            : Gameworld.Tags.GetByName(tagText);
        if (tag is null)
        {
            actor.OutputHandler.Send("There is no such tag.");
            return false;
        }

        if (explicitRemove)
        {
            if (!tags.Remove(tag))
            {
                actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} was not in the {name} list.");
                return false;
            }

            actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} is no longer {name}.");
        }
        else if (explicitAdd)
        {
            if (tags.Contains(tag))
            {
                actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} is already {name}.");
                return false;
            }

            tags.Add(tag);
            actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} is now {name}.");
        }
        else if (tags.Contains(tag))
        {
            tags.Remove(tag);
            actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} is no longer {name}.");
        }
        else
        {
            tags.Add(tag);
            actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} is now {name}.");
        }

        Changed = true;
        return true;
    }

    #region Constructors

    protected ContainerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected ContainerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : base(gameworld, originator, "Container")
    {
    }

    #endregion
}

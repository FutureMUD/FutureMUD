using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Construction.Autobuilder.Rooms;

public abstract class AutobuilderRoomBase : SaveableItem, IAutobuilderRoom
{
    protected bool ApplyAutobuilderTagsAsFrameworkTags { get; set; }

    // Selector text still drives description/group matching; IDs supply independent framework identity.
    protected Dictionary<string, long> FrameworkTagBindings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    protected XElement SaveFrameworkTagBindings() => new("FrameworkTagBindings",
        FrameworkTagBindings.Select(x => new XElement("Binding", new XAttribute("selector", x.Key), new XAttribute("id", x.Value))));

    protected AutobuilderRoomBase(IFuturemud gameworld, string name)
    {
        Gameworld = gameworld;
        _name = name;
    }

    protected AutobuilderRoomBase(Models.AutobuilderRoomTemplate room, IFuturemud gameworld)
    {
        _id = room.Id;
        _name = room.Name;
        Gameworld = gameworld;
        LoadFromXml(XElement.Parse(room.Definition));
    }

    protected virtual void LoadFromXml(XElement root)
    {
        ApplyAutobuilderTagsAsFrameworkTags =
            bool.Parse(root.Element("ApplyAutobuilderTagsAsFrameworkTags")?.Value ?? "false");
        FrameworkTagBindings = root.Element("FrameworkTagBindings")?.Elements("Binding")
            .ToDictionary(x => x.Attribute("selector")!.Value, x => long.Parse(x.Attribute("id")!.Value), StringComparer.OrdinalIgnoreCase)
            ?? new(StringComparer.OrdinalIgnoreCase);
        foreach (var binding in FrameworkTagBindings)
            if (Gameworld.Tags.Get(binding.Value) is null)
                throw new ApplicationException($"Autobuilder {Name}: selector {binding.Key} refers to missing framework tag #{binding.Value}.");
    }

    protected abstract XElement SaveToXml();

    protected void ApplyTagsToRoom(IRoom room, string[] tags)
    {
        ApplyTagsToRoom(room, [], tags);
    }

    protected void ApplyTagsToRoom(IRoom room, IReadOnlyCollection<ITag> frameworkTags, string[] tags)
    {
        if (!ApplyAutobuilderTagsAsFrameworkTags)
        {
            return;
        }

        if (frameworkTags.Count > 0)
        {
            foreach (ITag frameworkTag in frameworkTags.DistinctBy(x => x.Id))
            {
                room.AddTag(frameworkTag);
            }

            return;
        }

        var resolved = new List<ITag>();
        foreach (string tag in tags)
        {
            ITag fwTag;
            if (FrameworkTagBindings.TryGetValue(tag, out var id))
                fwTag = Gameworld.Tags.Get(id);
            else
            {
                if (FrameworkTagBindings.Count > 0) continue; // Unbound text labels have no framework identity.
                var matches = Gameworld.Tags.Where(x => x.Name.EqualTo(tag)).Take(2).ToArray();
                if (matches.Length > 1)
                    throw new ApplicationException($"Autobuilder {Name}: ambiguous legacy selector '{tag}'; bind an explicit framework tag ID.");
                fwTag = matches.SingleOrDefault();
            }
            if (fwTag != null)
            {
                resolved.Add(fwTag);
            }
        }
        foreach (var tag in resolved.DistinctBy(x => x.Id)) room.AddTag(tag);
    }

    #region Overrides of Item

    public sealed override string FrameworkItemType => "AutobuilderRoom";

    #endregion

    #region Implementation of IAutobuilderRoom

    public abstract IAutobuilderRoom Clone(string newName);

    public abstract IRoom CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription,
        params string[] tags);
    public virtual IRoom CreateRoom(ICharacter builder, ITerrain specifiedTerrain, bool deferDescription,
        IReadOnlyCollection<ITag> frameworkTags, params string[] tags) =>
        CreateRoom(builder, specifiedTerrain, deferDescription, tags);

    public virtual void RedescribeRoom(IRoom room, params string[] tags)
    {
        ApplyTagsToRoom(room, tags);
    }

    protected string BuildingHelpText => $@"You can use the following options with this command:

	#3name <name>#0 - renames this room template
	#3summary <text>#0 - a summary of this template for LIST and SHOW
	#3applytags#0 - toggles whether room tags are applied as framework tags{SubtypeHelpText}";

    protected abstract string SubtypeHelpText { get; }

    public virtual bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "name":
                return BuildingCommandName(actor, command);
            case "applytags":
                return BuildingCommandTags(actor, command);
            case "byline":
            case "summary":
                return BuildingCommandByline(actor, command);
            default:
                actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandByline(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What summary do you want to add to this room template?");
            return false;
        }

        ShowCommandByline = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send(
            $"The summary for this autobuilder room template is now {ShowCommandByline.ColourCommand()}.");
        return true;
    }

    private bool BuildingCommandTags(ICharacter actor, StringStack command)
    {
        ApplyAutobuilderTagsAsFrameworkTags = !ApplyAutobuilderTagsAsFrameworkTags;
        Changed = true;
        actor.OutputHandler.Send(
            $"This room builder template will {(ApplyAutobuilderTagsAsFrameworkTags ? "now" : "no longer")} apply framework tags to created rooms. Feature Rectangle masks use their tag IDs directly; generated named features use explicit bindings when supplied; legacy names must be unambiguous.");
        return true;
    }

    protected bool BuildingCommandName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name do you want to give to this room builder template?");
            return false;
        }

        string name = command.SafeRemainingArgument.TitleCase();
        if (Gameworld.AutobuilderRooms.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send(
                "There is already a room builder template with that name. Names must be unique.");
            return false;
        }

        actor.OutputHandler.Send($"You rename the room builder template {_name.ColourName()} to {name.ColourName()}.");
        _name = name;
        Changed = true;
        return true;
    }

    public override void Save()
    {
        AutobuilderRoomTemplate dbitem = FMDB.Context.AutobuilderRoomTemplates.Find(Id);
        dbitem.Name = Name;
        dbitem.Definition = SaveToXml().ToString();
        Changed = false;
    }

    public abstract string Show(ICharacter builder);
    public string ShowCommandByline { get; protected set; }

    #endregion
}

using MudSharp.Construction;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.GameItems.Groups;

public abstract class GameItemGroupForm : SaveableItem, IGameItemGroupForm
{
    private readonly IGameItemGroup _parent;
    protected readonly List<IRoom> Rooms = new();
    public sealed override string FrameworkItemType => "GameItemGroupForm";

    protected GameItemGroupForm(IGameItemGroup parent)
    {
        _parent = parent;
        Gameworld = parent.Gameworld;
    }

    protected GameItemGroupForm(ItemGroupForm form, IGameItemGroup parent)
    {
        _id = form.Id;
        _parent = parent;
        Gameworld = parent.Gameworld;
    }

    protected abstract string GameItemGroupFormType { get; }
    public abstract string Show(IPerceiver voyeur);

    #region IGameItemGroupForm Members

    public bool Applies(IRoom room)
    {
        return !Rooms.Any() || Rooms.Contains(room);
    }

    public bool Applies(long cellId)
    {
        return !Rooms.Any() || Rooms.Any(x => x.Id == cellId);
    }

    public bool SpecialFormFor(long cellId)
    {
        return Rooms.Any(x => x.Id == cellId);
    }

    public abstract string Describe(IPerceiver voyeur, IEnumerable<IGameItem> items);

    public virtual void BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.Last.ToLowerInvariant())
        {
            case "cell":
            case "room":
            case "location":
                BuildingCommandRoom(actor, command);
                break;
            default:
                actor.Send("That is not a valid option for editing Item Group Forms.");
                return;
        }
    }

    private void BuildingCommandRoom(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which cell do you want to add to or remove from this Item Group Form?");
            return;
        }

        if (!long.TryParse(command.PopSpeech(), out long value))
        {
            actor.Send(
                "What is the ID number of the cell that you wish to add to or remove from this Item Group Form?");
            return;
        }

        IRoom room = Gameworld.Rooms.Get(value);
        if (room == null)
        {
            actor.Send("There is no such cell.");
            return;
        }

        if (Rooms.Contains(room))
        {
            Rooms.Remove(room);
            Changed = true;
            actor.Send("The Cell {0} (#{1:N0}) will no longer use that Item Group Form.", room.Name, room.Id);
            return;
        }

        Rooms.Add(room);
        actor.Send("The cell {0} (#{1:N0}) will now use this Item Group Form.", room.Name, room.Id);
        foreach (GameItemGroupForm form in _parent.Forms.Except(this).Where(x => x.Applies(room)).Cast<GameItemGroupForm>())
        {
            form.Rooms.Remove(room);
            form.Changed = true;
            actor.Send("The cell was removed from form {0:N0}.", form.Id);
        }

        Changed = true;
    }

    public abstract string LookDescription(IPerceiver voyeur, IEnumerable<IGameItem> items);

    #endregion
}
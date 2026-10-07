using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public class Territory : Effect, IEffectSubtype
{
    private readonly List<(IRoom Room, HashSet<string> Flags)> _rooms = new();
    public IEnumerable<IRoom> Rooms => _rooms.Select(x => x.Room);

    public void AddRoom(IRoom room)
    {
        if (!_rooms.Any(x => x.Room == room))
        {
            _rooms.Add((room, new HashSet<string>()));
            Changed = true;
        }
    }

    public void RemoveRoom(IRoom room)
    {
        _rooms.RemoveAll(x => x.Room == room);
        Changed = true;
    }

    public void TagRoom(IRoom room, string tag)
    {
        if (!_rooms.Any(x => x.Room == room))
        {
            return;
        }

        _rooms.First(x => x.Room == room).Flags.Add(tag.ToLowerInvariant());
        Changed = true;
    }

    public void UntagRoom(IRoom room, string tag)
    {
        if (!_rooms.Any(x => x.Room == room))
        {
            return;
        }

        _rooms.First(x => x.Room == room).Flags.Remove(tag.ToLowerInvariant());
        Changed = true;
    }

    public bool HasFlag(IRoom room, string flag)
    {
        flag = flag.ToLowerInvariant();
        return _rooms.Any(x => x.Room == room && x.Flags.Contains(flag));
    }

    #region Static Initialisation

    public static void InitialiseEffectType()
    {
        RegisterFactory("Territory", (effect, owner) => new Territory(effect, owner));
    }

    #endregion

    #region Constructors

    public Territory(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
    {
    }

    protected Territory(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        LoadFromXML(effect.Element("Effect"));
    }

    #endregion

    #region Saving and Loading

    protected override XElement SaveDefinition()
    {
        return
            new XElement("Effect",
                from room in _rooms
                select new XElement("Cell", new XAttribute("id", room.Room.Id),
                    from flag in room.Flags
                    select new XElement("Flag", new XCData(flag)))
            );
    }

    protected void LoadFromXML(XElement root)
    {
        foreach (XElement item in root.Elements("Cell"))
        {
            IRoom room = Gameworld.Rooms.Get(long.Parse(item.Attribute("id").Value));
            if (room == null)
            {
                Changed = true;
                continue;
            }

            (IRoom room, HashSet<string> flags) tuple = (room, flags: new HashSet<string>());
            foreach (XElement flag in item.Elements("Flag"))
            {
                tuple.flags.Add(flag.Value);
            }
        }
    }

    #endregion

    #region Overrides of Effect

    protected override string SpecificEffectType => "Territory";

    public override string Describe(IPerceiver voyeur)
    {
        return $"Has territory [{_rooms.Select(x => x.Room.Id.ToString("F0")).ListToCommaSeparatedValues(" ")}]";
    }

    public override bool SavingEffect => true;

    #endregion
}
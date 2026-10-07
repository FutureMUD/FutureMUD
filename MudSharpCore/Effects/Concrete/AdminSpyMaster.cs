using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public class AdminSpyMaster : Effect, IEffectSubtype
{
    public List<IRoom> SpiedRooms { get; } = new();
    public List<AdminSpy> SpyEffects { get; } = new();
    public ICharacter CharacterOwner { get; }

    public AdminSpyMaster(ICharacter owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
    {
        CharacterOwner = owner;
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;
    }

    public AdminSpyMaster(ICharacter owner, AdminSpyMaster other) : base(owner, null)
    {
        CharacterOwner = owner;
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;

        foreach (IRoom room in other.SpiedRooms)
        {
            SpiedRooms.Add(room);
            AdminSpy childEffect = new(room, CharacterOwner);
            room.AddEffect(childEffect);
            SpyEffects.Add(childEffect);
        }
    }

    #region Overrides of Effect

    /// <inheritdoc />
    public override void RemovalEffect()
    {
        foreach (AdminSpy effect in SpyEffects)
        {
            effect.Owner.RemoveEffect(effect);
        }

        SpyEffects.Clear();
        CharacterOwner.OnQuit -= CharacterOwner_OnQuit;
    }

    #endregion

    private void CharacterOwner_OnQuit(IPerceivable owner)
    {
        RemovalEffect();
    }

    public AdminSpyMaster(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        CharacterOwner = (ICharacter)owner;
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;
        foreach (XElement spy in effect.Element("Effect").Elements("Spy"))
        {
            IRoom room = Gameworld.Rooms.Get(long.Parse(spy.Value));
            if (room != null)
            {
                SpiedRooms.Add(room);
                AdminSpy childEffect = new(room, CharacterOwner);
                room.AddEffect(childEffect);
                SpyEffects.Add(childEffect);
            }
        }
    }

    public void RemoveSpiedRoom(IRoom room)
    {
        SpiedRooms.Remove(room);
        room.RemoveEffect(SpyEffects.FirstOrDefault(x => x.Owner == room), true);
        if (!SpiedRooms.Any())
        {
            CharacterOwner.RemoveEffect(this);
        }

        Changed = true;
    }

    public void AddSpiedRoom(IRoom room)
    {
        if (SpiedRooms.Contains(room))
        {
            return;
        }

        SpiedRooms.Add(room);
        AdminSpy effect = new(room, CharacterOwner);
        room.AddEffect(effect);
        SpyEffects.Add(effect);
        Changed = true;
    }

    #region Overrides of Effect

    public override bool SavingEffect => true;

    public static void InitialiseEffectType()
    {
        RegisterFactory("AdminSpyMaster", (effect, owner) => new AdminSpyMaster(effect, owner));
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            from spy in SpiedRooms
            select new XElement("Spy", spy.Id)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Spying on {SpyEffects.Count} locations";
    }

    protected override string SpecificEffectType => "AdminSpyMaster";

    public override void Login()
    {
        CharacterOwner.OnQuit += CharacterOwner_OnQuit;
        foreach (IRoom room in SpiedRooms)
        {
            AdminSpy child = new(room, CharacterOwner);
            room.AddEffect(child);
            SpyEffects.Add(child);
        }
    }

    #endregion
}
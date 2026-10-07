using MudSharp.Construction;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class WatchMaster : Effect, IEffectSubtype, ICheckBonusEffect
{
    public List<IRoom> SpiedRooms { get; } = new();
    public List<Watch> WatchEffects { get; } = new();
    public ICharacter CharacterOwner { get; }

    public WatchMaster(ICharacter owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
    {
        CharacterOwner = owner;
    }

    protected override string SpecificEffectType => "WatchMaster";

    public void RemoveSpiedRoom(IRoom room)
    {
        SpiedRooms.Remove(room);
        room.RemoveEffect(WatchEffects.First(x => x.Owner == room), true);
        if (!SpiedRooms.Any())
        {
            CharacterOwner.RemoveEffect(this);
        }
    }

    public void AddSpiedRoom(IRoom room)
    {
        if (SpiedRooms.Contains(room))
        {
            return;
        }

        SpiedRooms.Add(room);
        Watch effect = new(room, CharacterOwner);
        room.AddEffect(effect);
        WatchEffects.Add(effect);
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Watching {WatchEffects.Count} locations";
    }

    public bool AppliesToCheck(CheckType type)
    {
        return (type.IsPerceptionCheck() && type != CheckType.WatchLocation) || type.IsGeneralActivityCheck() ||
               type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
    }

    public double CheckBonus => Gameworld.GetStaticDouble("WatchLocationPerceptionBonus");
}
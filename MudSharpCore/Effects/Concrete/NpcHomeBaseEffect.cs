#nullable enable
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public sealed class NpcHomeBaseEffect : Effect
{
    private long _homeRoomId;
    private long _anchorItemId;

    public NpcHomeBaseEffect(ICharacter owner)
        : base(owner)
    {
    }

    private NpcHomeBaseEffect(XElement root, IPerceivable owner)
        : base(root, owner)
    {
        XElement effect = root.Element("Effect") ?? throw new ArgumentException("Invalid NPC home-base effect definition.");
        _homeRoomId = long.Parse(effect.Attribute("HomeCellId")?.Value ?? "0");
        _anchorItemId = long.Parse(effect.Attribute("AnchorItemId")?.Value ?? "0");
    }

    public static void InitialiseEffectType()
    {
        RegisterFactory("NpcHomeBase", (effect, owner) => new NpcHomeBaseEffect(effect, owner));
    }

    public static NpcHomeBaseEffect GetOrCreate(ICharacter owner)
    {
        NpcHomeBaseEffect? existing = owner.CombinedEffectsOfType<NpcHomeBaseEffect>().FirstOrDefault();
        if (existing is not null)
        {
            return existing;
        }

        existing = new NpcHomeBaseEffect(owner);
        owner.AddEffect(existing);
        return existing;
    }

    public IRoom? HomeRoom => _homeRoomId > 0 ? Gameworld.Rooms.Get(_homeRoomId) : null;
    public IGameItem? AnchorItem => _anchorItemId > 0 ? Gameworld.TryGetItem(_anchorItemId, true) : null;
    public bool HasHome => HomeRoom is not null;
    public bool HasAnchor => AnchorItem is not null;

    public void SetHomeRoom(IRoom? room)
    {
        long newId = room?.Id ?? 0L;
        if (_homeRoomId == newId)
        {
            return;
        }

        _homeRoomId = newId;
        Changed = true;
    }

    public void SetAnchorItem(IGameItem? item)
    {
        long newId = item?.Id ?? 0L;
        if (_anchorItemId == newId)
        {
            return;
        }

        _anchorItemId = newId;
        Changed = true;
    }

    public void ClearAnchorItem()
    {
        SetAnchorItem(null);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XAttribute("HomeCellId", _homeRoomId),
            new XAttribute("AnchorItemId", _anchorItemId));
    }

    public override string Describe(IPerceiver voyeur)
    {
        string home = HomeRoom?.HowSeen(voyeur) ?? "no home";
        string anchor = AnchorItem?.HowSeen(voyeur) ?? "no anchor";
        return $"NPC home base at {home}, anchor {anchor}.";
    }

    public override bool SavingEffect => true;

    protected override string SpecificEffectType => "NpcHomeBase";
}

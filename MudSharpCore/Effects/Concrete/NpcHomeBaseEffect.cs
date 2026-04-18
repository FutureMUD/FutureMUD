#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public sealed class NpcHomeBaseEffect : Effect
{
    private long _homeCellId;
    private long _anchorItemId;

    public NpcHomeBaseEffect(ICharacter owner)
        : base(owner)
    {
    }

    private NpcHomeBaseEffect(XElement root, IPerceivable owner)
        : base(root, owner)
    {
        XElement effect = root.Element("Effect") ?? throw new ArgumentException("Invalid NPC home-base effect definition.");
        _homeCellId = long.Parse(effect.Attribute("HomeCellId")?.Value ?? "0");
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

    public ICell? HomeCell => _homeCellId > 0 ? Gameworld.Cells.Get(_homeCellId) : null;
    public IGameItem? AnchorItem => _anchorItemId > 0 ? Gameworld.TryGetItem(_anchorItemId, true) : null;
    public bool HasHome => HomeCell is not null;
    public bool HasAnchor => AnchorItem is not null;

    public void SetHomeCell(ICell? cell)
    {
        long newId = cell?.Id ?? 0L;
        if (_homeCellId == newId)
        {
            return;
        }

        _homeCellId = newId;
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
            new XAttribute("HomeCellId", _homeCellId),
            new XAttribute("AnchorItemId", _anchorItemId));
    }

    public override string Describe(IPerceiver voyeur)
    {
        string home = HomeCell?.HowSeen(voyeur) ?? "no home";
        string anchor = AnchorItem?.HowSeen(voyeur) ?? "no anchor";
        return $"NPC home base at {home}, anchor {anchor}.";
    }

    public override bool SavingEffect => true;

    protected override string SpecificEffectType => "NpcHomeBase";
}

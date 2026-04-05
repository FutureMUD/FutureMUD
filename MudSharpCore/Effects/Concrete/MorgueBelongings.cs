using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class MorgueBelongings : Effect, IEffect
{
    public long CharacterOwnerId { get; }
    public long EstateId { get; }
    public long EconomicZoneId { get; }

    public static void InitialiseEffectType()
    {
        RegisterFactory("MorgueBelongings", (effect, owner) => new MorgueBelongings(effect, owner));
    }

    public MorgueBelongings(IGameItem owner, ICharacter characterOwner, IFrameworkItem estate, IFrameworkItem economicZone)
        : base(owner, null)
    {
        CharacterOwnerId = characterOwner.Id;
        EstateId = estate?.Id ?? 0;
        EconomicZoneId = economicZone.Id;
    }

    protected MorgueBelongings(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        XElement root = effect.Element("Effect");
        CharacterOwnerId = long.Parse(root.Attribute("owner").Value);
        EstateId = long.Parse(root.Attribute("estate").Value);
        EconomicZoneId = long.Parse(root.Attribute("zone").Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XAttribute("owner", CharacterOwnerId),
            new XAttribute("estate", EstateId),
            new XAttribute("zone", EconomicZoneId));
    }

    protected override string SpecificEffectType => "MorgueBelongings";
    public override string Describe(IPerceiver voyeur)
    {
        return $"Morgue belongings bundle for deceased character #{CharacterOwnerId.ToString("N0", voyeur)}";
    }

    public override bool SavingEffect => true;
}

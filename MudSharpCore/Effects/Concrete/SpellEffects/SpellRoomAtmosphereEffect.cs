using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Form.Material;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellRoomAtmosphereEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, IAffectAtmosphere
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellRoomAtmosphere", (effect, owner) => new SpellRoomAtmosphereEffect(effect, owner));
    }

    public SpellRoomAtmosphereEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, IFluid atmosphere, string descAddendum, ANSIColour colour) : base(owner, parent, prog)
    {
        Atmosphere = atmosphere;
        DescAddendum = descAddendum;
        AddendumColour = colour;
    }

    protected SpellRoomAtmosphereEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        var tr = root.Element("Effect");
        var id = long.Parse(tr.Element("AtmosphereId").Value);
        var type = tr.Element("AtmosphereType").Value;
        Atmosphere = type.Equals("gas", StringComparison.InvariantCultureIgnoreCase)
            ? (IFluid)Gameworld.Gases.Get(id)
            : Gameworld.Liquids.Get(id);
        DescAddendum = tr.Element("DescAddendum").Value;
        AddendumColour = Telnet.GetColour(tr.Element("AddendumColour").Value);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
            new XElement("AtmosphereId", Atmosphere.Id),
            new XElement("AtmosphereType", Atmosphere.MaterialBehaviour == MaterialBehaviourType.Gas ? "gas" : "liquid"),
            new XElement("DescAddendum", new XCData(DescAddendum)),
            new XElement("AddendumColour", AddendumColour.Name)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Room Atmosphere - {Atmosphere.Name.ColourValue()}";
    }

    protected override string SpecificEffectType => "SpellRoomAtmosphere";

    public string DescAddendum { get; set; }
    public ANSIColour AddendumColour { get; set; }
    public IFluid Atmosphere { get; set; }

    public string GetAdditionalText(IPerceiver voyeur, bool colour)
    {
        if (string.IsNullOrEmpty(DescAddendum))
        {
            return string.Empty;
        }
        return colour ? DescAddendum.Colour(AddendumColour) : DescAddendum;
    }

    public bool PlayerSet => false;
}

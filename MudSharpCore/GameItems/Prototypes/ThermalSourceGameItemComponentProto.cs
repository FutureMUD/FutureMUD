#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Globalization;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public abstract class ThermalSourceGameItemComponentProto : GameItemComponentProto, IProduceHeatPrototype
{
    protected ThermalSourceGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type) : base(gameworld,
        originator, type)
    {
        ActiveDescriptionAddendum = "It is currently active.";
        InactiveDescriptionAddendum = "It is currently inactive.";
    }

    protected ThermalSourceGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto,
        gameworld)
    {
    }

    public double AmbientHeat { get; protected set; }
    public double IntimateHeat { get; protected set; }
    public double ImmediateHeat { get; protected set; }
    public double ProximateHeat { get; protected set; }
    public double DistantHeat { get; protected set; }
    public double VeryDistantHeat { get; protected set; }
    public string ActiveDescriptionAddendum { get; protected set; } = string.Empty;
    public string InactiveDescriptionAddendum { get; protected set; } = string.Empty;

    public double HeatFor(Proximity proximity)
    {
        return proximity switch
        {
            Proximity.Intimate => IntimateHeat,
            Proximity.Immediate => ImmediateHeat,
            Proximity.Proximate => ProximateHeat,
            Proximity.Distant => DistantHeat,
            Proximity.VeryDistant => VeryDistantHeat,
            _ => 0.0
        };
    }

    protected void LoadThermalDefinitionFromXml(XElement root)
    {
        AmbientHeat = double.Parse(root.Element("AmbientHeat")?.Value ?? "0");
        IntimateHeat = double.Parse(root.Element("IntimateHeat")?.Value ?? "0");
        ImmediateHeat = double.Parse(root.Element("ImmediateHeat")?.Value ?? "0");
        ProximateHeat = double.Parse(root.Element("ProximateHeat")?.Value ?? "0");
        DistantHeat = double.Parse(root.Element("DistantHeat")?.Value ?? "0");
        VeryDistantHeat = double.Parse(root.Element("VeryDistantHeat")?.Value ?? "0");
        ActiveDescriptionAddendum = root.Element("ActiveDescriptionAddendum")?.Value ?? "It is currently active.";
        InactiveDescriptionAddendum = root.Element("InactiveDescriptionAddendum")?.Value ?? "It is currently inactive.";
    }

    protected XElement SaveThermalDefinitionToXml(XElement root)
    {
        root.Add(
            new XElement("AmbientHeat", AmbientHeat),
            new XElement("IntimateHeat", IntimateHeat),
            new XElement("ImmediateHeat", ImmediateHeat),
            new XElement("ProximateHeat", ProximateHeat),
            new XElement("DistantHeat", DistantHeat),
            new XElement("VeryDistantHeat", VeryDistantHeat),
            new XElement("ActiveDescriptionAddendum", new XCData(ActiveDescriptionAddendum)),
            new XElement("InactiveDescriptionAddendum", new XCData(InactiveDescriptionAddendum))
        );
        return root;
    }

    protected bool BuildingCommandThermalProfile(ICharacter actor, string commandText, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must supply a signed numeric temperature change for that band.");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value))
        {
            actor.OutputHandler.Send("You must supply a valid signed numeric temperature change.");
            return false;
        }

        switch (commandText)
        {
            case "ambient":
                AmbientHeat = value;
                break;
            case "intimate":
                IntimateHeat = value;
                break;
            case "immediate":
                ImmediateHeat = value;
                break;
            case "proximate":
            case "proximity":
                ProximateHeat = value;
                break;
            case "distant":
                DistantHeat = value;
                break;
            case "verydistant":
            case "very-distant":
            case "very_distant":
                VeryDistantHeat = value;
                break;
            default:
                return false;
        }

        Changed = true;
        actor.OutputHandler.Send($"That thermal output is now {value.ToString("N2", actor).ColourValue()}.");
        return true;
    }

    protected bool BuildingCommandStateDescription(ICharacter actor, bool active, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What descriptive text should be shown when it is in that state?");
            return false;
        }

        Emote emote = new(command.SafeRemainingArgument, actor, actor);
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        if (active)
        {
            ActiveDescriptionAddendum = command.SafeRemainingArgument;
        }
        else
        {
            InactiveDescriptionAddendum = command.SafeRemainingArgument;
        }

        Changed = true;
        actor.OutputHandler.Send($"The {(active ? "active" : "inactive")} description addendum is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    public string ThermalProfileDisplay(IFormatProvider actor)
    {
        actor ??= CultureInfo.InvariantCulture;
        return
            $"Ambient {AmbientHeat.ToString("N2", actor).ColourValue()}, Intimate {IntimateHeat.ToString("N2", actor).ColourValue()}, Immediate {ImmediateHeat.ToString("N2", actor).ColourValue()}, Proximate {ProximateHeat.ToString("N2", actor).ColourValue()}, Distant {DistantHeat.ToString("N2", actor).ColourValue()}, VeryDistant {VeryDistantHeat.ToString("N2", actor).ColourValue()}";
    }

    protected const string ThermalBuildingHelpText = @"
	#3ambient <value>#0 - sets the room-wide thermal effect for indoor rooms
	#3intimate <value>#0 - sets the thermal effect at intimate proximity
	#3immediate <value>#0 - sets the thermal effect at immediate proximity
	#3proximate <value>#0 - sets the thermal effect at proximate proximity
	#3distant <value>#0 - sets the thermal effect at distant proximity
	#3verydistant <value>#0 - sets the thermal effect at very distant proximity
	#3activedesc <text>#0 - sets the addendum shown while the source is active
	#3inactivedesc <text>#0 - sets the addendum shown while the source is inactive";
}

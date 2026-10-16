using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable annotations

namespace MudSharp.GameItems.Prototypes;

public class ProgLightGameItemComponentProto : GameItemComponentProto, ILightablePrototype, IProduceLightPrototype
{
    private const string SpecificBuildingHelpText = @"
	#3illumination <lux>#0 - sets the illumination provided by the light
	#3spellgrade#0 - toggles multiplying illumination by the persisted creation spell grade (1-7)
	Grade-scaled lights without valid creation authority, including copied lights, provide no illumination.";

    private static readonly string CombinedBuildingHelpText = @$"You can use the following options with this component:
	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component{SpecificBuildingHelpText}";

    protected ProgLightGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : this(gameworld, originator, "Prog Light")
    {
    }

    protected ProgLightGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type)
        : base(gameworld, originator, type)
    {
        IlluminationProvided = 400;
        Changed = true;
    }

    protected ProgLightGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    public double IlluminationProvided { get; protected set; }
	public bool MultiplyIlluminationBySpellGrade { get; protected set; }
    public override string TypeDescription => "Prog Light";

    protected override void LoadFromXml(XElement root)
    {
        IlluminationProvided = (double)root.Element("IlluminationProvided")!;
		MultiplyIlluminationBySpellGrade = (bool?)root.Element("MultiplyIlluminationBySpellGrade") ?? false;
    }

    private bool BuildingCommand_Illumination(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many lux of illumination should this torch provide when lit?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value))
        {
            actor.Send("How many lux of illumination should this torch provide when lit?");
            return false;
        }

        if (value < 1)
        {
            actor.Send("Torches must provide a positive amount of illumination.");
            return false;
        }

        IlluminationProvided = value;
        Changed = true;
		actor.Send($"This light will now provide {IlluminationProvided.ToString("N2", actor).ColourValue()} lux{(MultiplyIlluminationBySpellGrade ? " per creation spell grade" : "")} when lit.");
        return true;
    }

	private bool BuildingCommand_SpellGrade(ICharacter actor)
	{
		MultiplyIlluminationBySpellGrade = !MultiplyIlluminationBySpellGrade;
		Changed = true;
		actor.Send($"This light {(MultiplyIlluminationBySpellGrade ? "now" : "no longer")} multiplies its illumination by the persisted creation spell grade. Grade scaling: {MultiplyIlluminationBySpellGrade.ToColouredString()}.");
		if (MultiplyIlluminationBySpellGrade)
		{
			actor.Send("Unbound, copied, and invalid-grade lights provide no illumination while grade scaling is enabled.");
		}
		return true;
	}

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "lux":
            case "illumination":
                return BuildingCommand_Illumination(actor, command);
			case "spellgrade":
				return BuildingCommand_SpellGrade(actor);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    public override string ShowBuildingHelp => CombinedBuildingHelpText;

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{1} (#{2:N0}r{3:N0}, {4})\n\nThis is a programmable light that provides {0:N2} lux{5} when lit.\nCreation spell grade scaling: {6}. {7}",
            IlluminationProvided,
            "Programmable Light Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
			MultiplyIlluminationBySpellGrade ? " per creation spell grade" : "",
			MultiplyIlluminationBySpellGrade.ToColouredString(),
			MultiplyIlluminationBySpellGrade ? "Unbound, copied, and invalid-grade lights provide no illumination." : ""
        );
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new object[]
            {
                new XElement("IlluminationProvided", IlluminationProvided),
				new XElement("MultiplyIlluminationBySpellGrade", MultiplyIlluminationBySpellGrade)
            }
        ).ToString();
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("proglight", true,
            (gameworld, account) => new ProgLightGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("Prog Light",
            (proto, gameworld) => new ProgLightGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "ProgLight",
            $"A {"[light source]".Colour(Telnet.Pink)} when {"[powered]".Colour(Telnet.Magenta)}, can only be turned on/off by progs",
            CombinedBuildingHelpText
        );
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new ProgLightGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new ProgLightGameItemComponent(component, this, parent);
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new ProgLightGameItemComponentProto(proto, gameworld));
    }
}

using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ProgLightGameItemComponentProto : GameItemComponentProto
{
	protected ProgLightGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Prog Light")
	{
		IlluminationProvided = 400;
		Changed = true;
	}

	protected ProgLightGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public double IlluminationProvided { get; protected set; }
	public override string TypeDescription => "Prog Light";

	protected override void LoadFromXml(XElement root)
	{
		IlluminationProvided = double.Parse(root.Element("IlluminationProvided").Value);
	}

	private bool BuildingCommand_Illumination(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many lux of illumination should this torch provide when lit?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
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
		actor.Send("This torch will now provide {0:N2} lux of illumination when lit.", IlluminationProvided);
		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "lux":
			case "illumination":
				return BuildingCommand_Illumination(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tillumination <lux> - sets the illumination provided by the light";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{1} (#{2:N0}r{3:N0}, {4})\n\nThis is a programmable light that provides {0:N2} lux of illumination when lit.",
			IlluminationProvided,
			"Programmable Light Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new object[]
			{
				new XElement("IlluminationProvided", IlluminationProvided)
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
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
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
#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class ComputerMediaInterfaceGameItemComponentProto : PoweredMachineBaseGameItemComponentProto,
	IComputerMediaInterfacePrototype
{
	private const string SpecificBuildingHelpText = @"
	#3capabilities <audio|video|av>#0 - sets the media carried by this interface
	#3input <name>#0 - sets the Media application input name
	#3output <name>#0 - sets the Media application output name
	#3endpoint <key>#0 - sets the stable local endpoint key
	#3siblings#0 - toggles accepting a source endpoint on the same composite item";

	public ComputerMediaInterfaceGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Computer Media Interface")
	{
		Capabilities = MediaCapabilities.Audio | MediaCapabilities.Video;
		InputName = "media-in";
		OutputName = "media-out";
		EndpointKey = "computer-media";
		AcceptSiblingSources = true;
		Wattage = 3.0;
		WattageDiscountPerQuality = 0.1;
	}

	protected ComputerMediaInterfaceGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public MediaCapabilities Capabilities { get; protected set; }
	public string InputName { get; protected set; } = "media-in";
	public string OutputName { get; protected set; } = "media-out";
	public string EndpointKey { get; protected set; } = "computer-media";
	public bool AcceptSiblingSources { get; protected set; }
	public override string TypeDescription => "Computer Media Interface";
	protected override string ComponentDescriptionOLCByline => "This item is a powered local media gateway for a sibling computer host";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Capabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\n" +
		       $"Input: {InputName.ColourCommand()}\nOutput: {OutputName.ColourCommand()}\nEndpoint: {EndpointKey.ColourCommand()}\nSibling Binding: {AcceptSiblingSources.ToColouredString()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true, out var capabilities)
			? capabilities
			: MediaCapabilities.Audio | MediaCapabilities.Video;
		InputName = NormaliseName(root.Element("InputName")?.Value, "media-in");
		OutputName = NormaliseName(root.Element("OutputName")?.Value, "media-out");
		EndpointKey = NormaliseName(root.Element("EndpointKey")?.Value, "computer-media");
		AcceptSiblingSources = !bool.TryParse(root.Element("AcceptSiblingSources")?.Value, out var siblings) || siblings;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("Capabilities", Capabilities));
		root.Add(new XElement("InputName", InputName));
		root.Add(new XElement("OutputName", OutputName));
		root.Add(new XElement("EndpointKey", EndpointKey));
		root.Add(new XElement("AcceptSiblingSources", AcceptSiblingSources));
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "capabilities":
			case "capability":
				if (command.IsFinished || !MediaComponentUtilities.TryParseCapabilities(command.PopSpeech(), out var capabilities))
				{
					actor.Send("Choose audio, video or av for this computer media interface.");
					return false;
				}

				Capabilities = capabilities;
				Changed = true;
				actor.Send($"This interface now carries {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}.");
				return true;
			case "input":
				return BuildingCommandName(actor, command, true);
			case "output":
				return BuildingCommandName(actor, command, false);
			case "endpoint":
				var endpoint = command.SafeRemainingArgument.Trim();
				if (!IsEndpointName(endpoint))
				{
					actor.Send("Endpoint keys must be a single non-empty word.");
					return false;
				}

				EndpointKey = endpoint;
				Changed = true;
				actor.Send($"This interface's media endpoint is now {EndpointKey.ColourCommand()}.");
				return true;
			case "siblings":
			case "sibling":
				AcceptSiblingSources = !AcceptSiblingSources;
				Changed = true;
				actor.Send($"This interface will {(AcceptSiblingSources ? "now".ColourValue() : "no longer".ColourError())} accept a sibling source.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command, bool input)
	{
		var name = command.SafeRemainingArgument.Trim();
		if (!IsEndpointName(name))
		{
			actor.Send("Media endpoint names must be a single non-empty word.");
			return false;
		}

		if (input)
		{
			InputName = name;
		}
		else
		{
			OutputName = name;
		}

		Changed = true;
		actor.Send($"This interface's Media application {(input ? "input" : "output")} is now {name.ColourCommand()}.");
		return true;
	}

	private static bool IsEndpointName(string value)
	{
		return !string.IsNullOrWhiteSpace(value) && !value.Any(char.IsWhiteSpace);
	}

	private static string NormaliseName(string? value, string fallback)
	{
		var trimmed = value?.Trim() ?? string.Empty;
		return IsEndpointName(trimmed) ? trimmed : fallback;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("computermediainterface", true,
			(gameworld, account) => new ComputerMediaInterfaceGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("computer media interface", false,
			(gameworld, account) => new ComputerMediaInterfaceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Computer Media Interface",
			(proto, gameworld) => new ComputerMediaInterfaceGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Computer Media Interface",
			$"Makes an item a powered {"[computer media interface]".Colour(Telnet.BoldGreen)} gateway for a sibling computer host",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ComputerMediaInterfaceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ComputerMediaInterfaceGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ComputerMediaInterfaceGameItemComponentProto(proto, gameworld));
	}
}

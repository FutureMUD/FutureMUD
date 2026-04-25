#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class FileSignalGeneratorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3filename <name>#0 - sets the backing signal file name
	#3initial <text>#0 - sets the initial file contents for new items
	#3public#0 - toggles whether new items expose the file publicly over FTP
	#3capacity <bytes>#0 - sets the storage capacity of the backing file system

#6Notes:#0

	This powered signal generator reads a numeric value from its configured file and emits that value on the default #3signal#0 endpoint while switched on and powered.
	The file can then be edited locally with the #3programming item#0 command or remotely through the existing computer file-transfer tools when the parent host advertises FTP.";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected FileSignalGeneratorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "File Signal Generator")
	{
		UseMountHostPowerSource = true;
		Wattage = 40.0;
		SignalFileName = "signal.txt";
		InitialFileContents = "0";
		FileCapacityInBytes = 4096L;
		PubliclyAccessibleByDefault = false;
	}

	protected FileSignalGeneratorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string SignalFileName { get; protected set; } = "signal.txt";
	public string InitialFileContents { get; protected set; } = "0";
	public long FileCapacityInBytes { get; protected set; } = 4096L;
	public bool PubliclyAccessibleByDefault { get; protected set; }
	public override string TypeDescription => "File Signal Generator";

	protected override string ComponentDescriptionOLCByline => "This item is a powered file-backed signal generator";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"Signal Output: numeric value read from {SignalFileName.ColourCommand()}\nInitial File Contents: {InitialFileContents.ColourCommand()}\nFile Capacity: {FileCapacityInBytes.ToString("N0", actor).ColourValue()} bytes\nPublic By Default: {PubliclyAccessibleByDefault.ToColouredString()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		SignalFileName = root.Element("SignalFileName")?.Value ?? "signal.txt";
		InitialFileContents = root.Element("InitialFileContents")?.Value ?? "0";
		FileCapacityInBytes = long.TryParse(root.Element("FileCapacityInBytes")?.Value, out var capacity)
			? Math.Max(1L, capacity)
			: 4096L;
		PubliclyAccessibleByDefault = bool.TryParse(root.Element("PubliclyAccessibleByDefault")?.Value, out var isPublic) &&
		                              isPublic;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("SignalFileName", new XCData(SignalFileName)));
		root.Add(new XElement("InitialFileContents", new XCData(InitialFileContents)));
		root.Add(new XElement("FileCapacityInBytes", FileCapacityInBytes));
		root.Add(new XElement("PubliclyAccessibleByDefault", PubliclyAccessibleByDefault));
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "filename":
			case "file":
				return BuildingCommandFileName(actor, command);
			case "initial":
			case "initialtext":
			case "initialvalue":
				return BuildingCommandInitialText(actor, command);
			case "public":
				return BuildingCommandPublic(actor);
			case "capacity":
			case "storage":
				return BuildingCommandCapacity(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandFileName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What file name should this signal generator use?");
			return false;
		}

		var fileName = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(fileName))
		{
			actor.Send("You must specify a valid file name.");
			return false;
		}

		SignalFileName = fileName;
		Changed = true;
		actor.Send($"This signal generator will now read from {SignalFileName.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandInitialText(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What initial text should new signal files contain?");
			return false;
		}

		InitialFileContents = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"New items will now seed {SignalFileName.ColourCommand()} with {InitialFileContents.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPublic(ICharacter actor)
	{
		PubliclyAccessibleByDefault = !PubliclyAccessibleByDefault;
		Changed = true;
		actor.Send(
			$"New items will {(PubliclyAccessibleByDefault ? "now".ColourValue() : "no longer".ColourError())} expose {SignalFileName.ColourCommand()} publicly over FTP by default.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many bytes of file capacity should this component reserve?");
			return false;
		}

		if (!long.TryParse(command.SafeRemainingArgument, out var capacity) || capacity <= 0L)
		{
			actor.Send("You must enter a positive number of bytes.");
			return false;
		}

		FileCapacityInBytes = capacity;
		Changed = true;
		actor.Send($"This component will now reserve {FileCapacityInBytes.ToString("N0", actor).ColourValue()} bytes for its backing file system.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("filesignalgenerator", true,
			(gameworld, account) => new FileSignalGeneratorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("file signal generator", false,
			(gameworld, account) => new FileSignalGeneratorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("File Signal Generator",
			(proto, gameworld) => new FileSignalGeneratorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"FileSignalGenerator",
			$"A {"[powered]".Colour(Telnet.Magenta)} {SignalComponentUtilities.SignalGeneratorTag} that emits a numeric value read from a backing file",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new FileSignalGeneratorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new FileSignalGeneratorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new FileSignalGeneratorGameItemComponentProto(proto, gameworld));
	}
}

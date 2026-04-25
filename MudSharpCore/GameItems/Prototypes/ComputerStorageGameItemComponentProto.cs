#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ComputerStorageGameItemComponentProto : GameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3capacity <bytes>#0 - sets the storage capacity for files on this device";

	private static readonly string CombinedBuildingHelpText =
		$@"You can use the following options with this component:
	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component{SpecificBuildingHelpText}";

	public ComputerStorageGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Computer Storage")
	{
		StorageCapacityInBytes = 1048576;
	}

	protected ComputerStorageGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public long StorageCapacityInBytes { get; protected set; }
	public override string TypeDescription => "Computer Storage";

	protected override void LoadFromXml(XElement root)
	{
		StorageCapacityInBytes = long.TryParse(root.Element("StorageCapacityInBytes")?.Value, out var capacity)
			? capacity
			: 1048576L;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("StorageCapacityInBytes", StorageCapacityInBytes)).ToString();
	}

	public override string ShowBuildingHelp => $@"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "capacity":
			case "storage":
				return BuildingCommandCapacity(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !long.TryParse(command.PopSpeech(), out var capacity) || capacity < 0)
		{
			actor.Send("How many bytes of storage should this device provide?");
			return false;
		}

		StorageCapacityInBytes = capacity;
		Changed = true;
		actor.Send(
			$"This storage device now provides {StorageCapacityInBytes.ToString("N0", actor).ColourValue()} bytes.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Computer Storage Game Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nCapacity: {StorageCapacityInBytes.ToString("N0", actor).ColourValue()} bytes";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("computerstorage", true,
			(gameworld, account) => new ComputerStorageGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("computer storage", false,
			(gameworld, account) => new ComputerStorageGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Computer Storage",
			(proto, gameworld) => new ComputerStorageGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Computer Storage",
			$"Makes an item {"[computer storage]".Colour(Telnet.BoldGreen)} for files and executables mounted into a computer host",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ComputerStorageGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ComputerStorageGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ComputerStorageGameItemComponentProto(proto, gameworld));
	}
}

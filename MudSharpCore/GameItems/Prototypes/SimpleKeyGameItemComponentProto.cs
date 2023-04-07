using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class SimpleKeyGameItemComponentProto : GameItemComponentProto, IHaveSimpleLockType
{
	public string LockType { get; private set; }
	public override string TypeDescription => "Simple Key";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("key", true,
			(gameworld, account) => new SimpleKeyGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Simple Key",
			(proto, gameworld) => new SimpleKeyGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Key",
			$"Item is a {"[key]".Colour(Telnet.Yellow)} that opens a simple mechanical {"[lock]".Colour(Telnet.Yellow)}",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}\n\nThis item is a simple key, which will open mechanical locks of type {3}.",
			"Simple Key Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			LockType != null ? LockType.Colour(Telnet.Green) : "None".Colour(Telnet.Red)
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("LockType");
		if (element != null)
		{
			LockType = element.Value;
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LockType", LockType ?? "")
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new SimpleKeyGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SimpleKeyGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SimpleKeyGameItemComponentProto(proto, gameworld));
	}

	#region Constructors

	protected SimpleKeyGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected SimpleKeyGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Simple Key")
	{
		LockType = "Lever Lock";
		Changed = true;
	}

	#endregion

	#region Building Commands

	#region Overrides of GameItemComponentProto

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tlock <type> - sets the lock type that this key works with";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "lock":
			case "type":
			case "locktype":
			case "lock type":
				return BuildingCommandLocktype(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandLocktype(ICharacter actor, StringStack command)
	{
		var types = Gameworld.ItemProtos.SelectNotNull(x => x.GetItemType<IHaveSimpleLockType>())
		                     .Select(x => x.LockType)
		                     .Distinct()
		                     .ToList();

		if (command.IsFinished)
		{
			actor.Send("What type do you want to set this key too?");

			if (types.Count > 0)
			{
				actor.Send(
					"Other locks and keys have the following lock types: {0}".ColourIncludingReset(Telnet.Yellow),
					types.Select(x => x.ColourValue()).ListToString());
			}

			return false;
		}

		LockType = command.PopSpeech().TitleCase();
		Changed = true;
		actor.Send("This key now matches locks of type {0}.", LockType.ColourValue());
		if (!types.Contains(LockType))
		{
			actor.Send(
				"Warning: There are no other locks or keys with this lock type. Check that you actually intended to create a new locking scheme."
					.Colour(Telnet.Yellow));
		}

		return true;
	}

	#endregion

	#endregion Building Commands
}
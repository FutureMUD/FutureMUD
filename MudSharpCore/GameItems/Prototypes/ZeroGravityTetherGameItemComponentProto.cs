using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ZeroGravityTetherGameItemComponentProto : GameItemComponentProto, IZeroGravityTetherItemPrototype
{
	protected ZeroGravityTetherGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "ZeroGravityTether")
	{
		MaximumRooms = 3;
		Changed = true;
	}

	protected ZeroGravityTetherGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public override string TypeDescription => "ZeroGravityTether";

	public int MaximumRooms { get; protected set; }

	protected override void LoadFromXml(XElement root)
	{
		MaximumRooms = int.Parse(root.Element("MaximumRooms")?.Value ?? "3");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumRooms", MaximumRooms)
		).ToString();
	}

	private const string BuildingHelpText = "You can use the following options with this component:\n\trooms <number> - sets the maximum room length of the tether";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "rooms":
			case "length":
				return BuildingCommandRooms(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandRooms(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var rooms) || rooms < 0)
		{
			actor.OutputHandler.Send("Enter a non-negative maximum number of rooms.");
			return false;
		}

		MaximumRooms = rooms;
		Changed = true;
		actor.OutputHandler.Send($"This tether can now extend up to {MaximumRooms.ToString("N0", actor).ColourValue()} rooms.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $"{ "Zero Gravity Tether Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nMaximum room length: {MaximumRooms.ToString("N0", actor).ColourValue()}.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ZeroGravityTetherGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ZeroGravityTetherGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new ZeroGravityTetherGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ZeroGravityTether".ToLowerInvariant(), true, (gameworld, account) => new ZeroGravityTetherGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ZeroGravityTether", (proto, gameworld) => new ZeroGravityTetherGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("ZeroGravityTether", "A zero-gravity tether line with a maximum room length.", BuildingHelpText);
	}
}

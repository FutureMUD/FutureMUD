using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class DragAidGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "DragAid";

	#region Constructors

	protected DragAidGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"DragAid")
	{
		EffortMultiplier = 2.0;
		MaximumUsers = 5;
	}

	protected DragAidGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MaximumUsers = int.Parse(root.Element("MaximumUsers").Value);
		EffortMultiplier = int.Parse(root.Element("EffortMultiplier").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumUsers", MaximumUsers),
			new XElement("EffortMultiplier", EffortMultiplier)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DragAidGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DragAidGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("DragAid".ToLowerInvariant(), true,
			(gameworld, account) => new DragAidGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("DragAid", (proto, gameworld) => new DragAidGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"DragAid",
			$"Makes this item a drag aid, that can be used when dragging something to help increase capacity",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new DragAidGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tusers <#> - the number of people who can use this drag aid at once\n\tbonus <%> - sets the drag capacity multiplier for using this drag aid";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "multiplier":
			case "mult":
			case "bonus":
				return BuildingCommandMultiplier(actor, command);
			case "maximum":
			case "users":
			case "max":
				return BuildingCommandUsers(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	public bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What should the multiplier be for this dragging aid?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var value) || value <= 1.0)
		{
			actor.Send($"The value must be a valid percentage greater than {1.0.ToString("P0", actor)}.");
			return false;
		}

		EffortMultiplier = value;
		Changed = true;
		actor.Send(
			$"This item will now multiply the dragging capacity of its owner by {EffortMultiplier.ToString("P3", actor).ColourValue()} when used as an aid.");
		return true;
	}

	public bool BuildingCommandUsers(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many people should be able to use this aid simultaneously?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value) || value < 1)
		{
			actor.Send("The value must be a valid number equal to or greater than 1.");
			return false;
		}

		MaximumUsers = value;
		Changed = true;
		actor.Send(
			$"This item can now be used simultaneously by {MaximumUsers.ToString("N0", actor).ColourValue()} people.");
		return true;
	}

	#endregion

	public double EffortMultiplier { get; set; }

	public int MaximumUsers { get; set; }

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item aids in dragging people and items when attached, connected, worn or lodged. It provides a multiplier of {4:N3} to the effective dragging capacity of the dragger, and can be used simultaneously by a maximum of {5:N0} draggers.",
			"DragAid Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			EffortMultiplier,
			MaximumUsers
		);
	}
}
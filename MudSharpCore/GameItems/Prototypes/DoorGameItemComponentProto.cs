using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class DoorGameItemComponentProto : GameItemComponentProto
{
	protected DoorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected DoorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Door")
	{
		CanPlayersUninstall = false;
		CanPlayersSmash = false;
		UninstallDifficultyHingeSide = Difficulty.Impossible;
		UninstallDifficultyNotHingeSide = Difficulty.Impossible;
		SmashDifficulty = Difficulty.Impossible;
		CanBeOpenedByPlayers = true;
		Changed = true;
	}

	/// <summary>
	///     A short string, designed to be parsed for characteristics, which appears in brackets after an exit description in
	///     rooms.
	///     e.g. heavy iron door
	/// </summary>
	public string InstalledExitDescription { get; protected set; }

	/// <summary>
	///     Whether this door permits people to see through it
	/// </summary>
	public bool SeeThrough { get; protected set; }

	public bool CanPlayersUninstall { get; protected set; }
	public bool CanPlayersSmash { get; protected set; }
	public Difficulty UninstallDifficultyHingeSide { get; protected set; }
	public Difficulty UninstallDifficultyNotHingeSide { get; protected set; }
	public Difficulty SmashDifficulty { get; protected set; }
	public ITraitDefinition UninstallTrait { get; protected set; }
	public bool CanFireThrough { get; protected set; }
	public bool CanBeOpenedByPlayers { get; protected set; }


	public override string TypeDescription => "Door";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is {4} door, and when installed in an exit will show as {5}. {6}. {7}. {8}. {9}.",
			"Door Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			SeeThrough ? "a transparent" : "an opaque",
			InstalledExitDescription.Colour(Telnet.Yellow),
			CanPlayersUninstall
				? $"It can be removed by players at a difficulty of {UninstallDifficultyHingeSide.Describe().Colour(Telnet.Green)} (hinge) / {UninstallDifficultyNotHingeSide.Describe().Colour(Telnet.Green)} (non-hinge) with the {(UninstallTrait != null ? UninstallTrait.Name.TitleCase().Colour(Telnet.Green) : "None")} skill"
				: "It cannot be removed by players",
			CanPlayersSmash
				? $"It can be smashed by players at a difficulty of {SmashDifficulty.Describe().Colour(Telnet.Green)}"
				: "It cannot be smashed by players",
			CanFireThrough ? "It can be fired through when closed" : "It cannot be fired through when closed",
			CanBeOpenedByPlayers
				? "It can be opened/closed by players through normal means"
				: "It can only be opened/closed via progs"
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("SeeThrough");
		if (attr != null)
		{
			SeeThrough = Convert.ToBoolean(attr.Value);
		}

		attr = root.Attribute("CanFireThrough");
		if (attr != null)
		{
			CanFireThrough = bool.Parse(attr.Value);
		}

		var element = root.Element("InstalledExitDescription");
		if (element != null)
		{
			InstalledExitDescription = element.Value;
		}

		element = root.Element("CanBeOpenedByPlayers");
		CanBeOpenedByPlayers = element != null ? bool.Parse(element.Value) : true;

		element = root.Element("Uninstall");
		if (element == null)
		{
			CanPlayersUninstall = true;
			UninstallDifficultyHingeSide = Difficulty.Normal;
			UninstallDifficultyNotHingeSide = Difficulty.ExtremelyHard;
		}
		else
		{
			attr = element.Attribute("CanPlayersUninstall");
			if (attr != null)
			{
				CanPlayersUninstall = bool.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallDifficultyHingeSide");
			if (attr != null)
			{
				UninstallDifficultyHingeSide = (Difficulty)int.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallDifficultyNotHingeSide");
			if (attr != null)
			{
				UninstallDifficultyNotHingeSide = (Difficulty)int.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallTrait");
			if (attr != null)
			{
				UninstallTrait = Gameworld.Traits.Get(long.Parse(attr.Value));
			}
		}

		element = root.Element("Smash");
		if (element == null)
		{
			CanPlayersSmash = true;
			SmashDifficulty = Difficulty.Normal;
		}
		else
		{
			attr = element.Attribute("CanPlayersSmash");
			if (attr != null)
			{
				CanPlayersSmash = bool.Parse(attr.Value);
			}

			attr = element.Attribute("SmashDifficulty");
			if (attr != null)
			{
				SmashDifficulty = (Difficulty)int.Parse(attr.Value);
			}
		}
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XAttribute("SeeThrough", SeeThrough),
				new XAttribute("CanFireThrough", CanFireThrough),
				new XElement("InstalledExitDescription",
					!string.IsNullOrWhiteSpace(InstalledExitDescription) ? InstalledExitDescription : "door"),
				new XElement("CanBeOpenedByPlayers", CanBeOpenedByPlayers),
				new XElement("Uninstall", new XAttribute("CanPlayersUninstall", CanPlayersUninstall),
					new XAttribute("UninstallDifficultyHingeSide", (int)UninstallDifficultyHingeSide),
					new XAttribute("UninstallDifficultyNotHingeSide", (int)UninstallDifficultyNotHingeSide),
					new XAttribute("UninstallTrait", UninstallTrait?.Id ?? 0)),
				new XElement("Smash", new XAttribute("CanPlayersSmash", CanPlayersSmash),
					new XAttribute("SmashDifficulty", (int)SmashDifficulty))).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("door", true,
			(gameworld, account) => new DoorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Door", (proto, gameworld) => new DoorGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Door",
			$"Turns the item into a {"[door]".Colour(Telnet.Yellow)} that can be installed in doorways",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DoorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DoorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new DoorGameItemComponentProto(proto, gameworld));
	}

	private bool BuildingCommand_Uninstallable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (!CanPlayersUninstall)
			{
				actor.Send(
					"This door component is already not removable by players. If you want to make it removable you must specify additional arguments.");
				return false;
			}

			CanPlayersUninstall = false;
			Changed = true;
			actor.Send("This door is no longer removable by players.");
			return true;
		}

		var hingeDifficultyText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.Send(
				"What difficulty do you want players on the non-hinge side of the door to have when removing this door?");
			return false;
		}

		var nonHingeDifficultyText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.Send("What trait will players use to remove this door?");
			return false;
		}

		var traitText = command.PopSpeech();

		if (!CheckExtensions.GetDifficulty(hingeDifficultyText, out var hingeDifficulty))
		{
			actor.OutputHandler.Send($"The text {hingeDifficultyText.ColourCommand()} is not a valid difficulty.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(nonHingeDifficultyText, out var nonHingeDifficulty))
		{
			actor.OutputHandler.Send($"The text {nonHingeDifficultyText.ColourCommand()} is not a valid difficulty.");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(traitText);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		CanPlayersUninstall = true;
		UninstallDifficultyHingeSide = hingeDifficulty;
		UninstallDifficultyNotHingeSide = nonHingeDifficulty;
		UninstallTrait = trait;
		Changed = true;
		actor.Send(
			"This door will now be removeable by players with the {0} trait at difficulties {1} (hinge) and {2} (non-hinge).",
			trait.Name.TitleCase().Colour(Telnet.Green),
			hingeDifficulty.DescribeColoured(),
			nonHingeDifficulty.DescribeColoured()
		);
		return true;
	}

	private bool BuildingCommand_Smashable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (!CanPlayersSmash)
			{
				actor.Send(
					"This door component is already not smashable by players. If you want to make it smashable you must specify additional arguments.");
				return false;
			}

			CanPlayersSmash = false;
			Changed = true;
			actor.Send("This door is no longer smashable by players.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.Send("What difficulty do you want players to have when smashing this door?");
			return false;
		}

		var difficultyText = command.PopSpeech();

		if (!CheckExtensions.GetDifficulty(difficultyText, out var difficulty))
		{
			actor.Send($"The text {difficultyText.ColourCommand()} is not a valid difficulty.");
			return false;
		}

		CanPlayersSmash = true;
		SmashDifficulty = difficulty;
		Changed = true;
		actor.Send("This door will now be smashable by players at a difficulty of {0}.",
			difficulty.DescribeColoured());
		return true;
	}

	private bool BuildingCommand_InstalledExitDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the installed exit description to?");
			return false;
		}

		InstalledExitDescription = command.SafeRemainingArgument.Trim();
		actor.OutputHandler.Send(
			$"You set the Installed Exit Description for this door to {InstalledExitDescription.ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_SeeThrough(ICharacter actor, StringStack command)
	{
		switch (command.Last.ToLowerInvariant())
		{
			case "transparent":
				SeeThrough = true;
				break;
			case "opaque":
				SeeThrough = false;
				break;
			default:
				switch (command.PopSpeech().ToLowerInvariant())
				{
					case "true":
						SeeThrough = true;
						break;
					case "false":
						SeeThrough = false;
						break;
					default:
						actor.OutputHandler.Send("That is not a valid option for the door's see-through property.");
						return false;
				}

				break;
		}

		Changed = true;
		actor.OutputHandler.Send($"The door is now {(SeeThrough ? "transparent" : "opaque")}.");
		return true;
	}

	private bool BuildingCommand_Fire(ICharacter actor, StringStack command)
	{
		CanFireThrough = !CanFireThrough;
		actor.Send(CanFireThrough
			? "You can now fire ranged weapons through this component when it is closed."
			: "You can no longer fire ranged weapons through this component when it is closed.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_CanBeOpenedByPlayers(ICharacter actor, StringStack command)
	{
		CanBeOpenedByPlayers = !CanBeOpenedByPlayers;
		Changed = true;
		actor.OutputHandler.Send(CanBeOpenedByPlayers
			? "This door can now be opened by players using ordinary means like the OPEN command."
			: "This door can no longer be opened by players using ordinary means, and must be opened and closed via progs.");
		return true;
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3uninstallable <hinge side difficulty> <other side difficulty> <uninstall trait>#0 - sets the door as uninstallable
	#3uninstallable#0 - sets the door as not uninstallable by players
	#3smashable <difficulty>#0 - sets the door as smashable by players
	#3smashable#0 - sets the door as not smashable
	#3installed <keyword>#0 - sets the keyword for this door as viewed in exits (e.g. iron door)
	#3transparent#0 - sets the door as transparent
	#3opaque#0 - sets the door as opaque
	#3fire#0 - toggles whether the door can be fired through (e.g. gate)
	#3openable#0 - toggles whether players can open this door with the OPEN/CLOSE commands";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "removable":
			case "uninstall":
			case "uninstallable":
				return BuildingCommand_Uninstallable(actor, command);
			case "smashable":
				return BuildingCommand_Smashable(actor, command);
			case "installed description":
			case "installed":
			case "installed_description":
			case "exit_description":
			case "exit description":
			case "exitdesc":
			case "exit":
				return BuildingCommand_InstalledExitDescription(actor, command);
			case "see through":
			case "seethrough":
			case "transparent":
			case "opaque":
				return BuildingCommand_SeeThrough(actor, command);
			case "fire":
				return BuildingCommand_Fire(actor, command);
			case "open":
			case "openable":
			case "canbeopened":
			case "canopen":
				return BuildingCommand_CanBeOpenedByPlayers(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}
}
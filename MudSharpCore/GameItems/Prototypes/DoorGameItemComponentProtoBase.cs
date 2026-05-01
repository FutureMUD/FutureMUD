#nullable enable

using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public abstract class DoorGameItemComponentProtoBase : GameItemComponentProto, IDoorPrototype
{
	protected DoorGameItemComponentProtoBase(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected DoorGameItemComponentProtoBase(IFuturemud gameworld, IAccount originator, string type)
		: base(gameworld, originator, type)
	{
		CanPlayersUninstall = false;
		CanPlayersSmash = false;
		UninstallDifficultyHingeSide = Difficulty.Impossible;
		UninstallDifficultyNotHingeSide = Difficulty.Impossible;
		SmashDifficulty = Difficulty.Impossible;
		CanBeOpenedByPlayers = true;
		Changed = true;
	}

	public string InstalledExitDescription { get; protected set; } = "door";
	public bool SeeThrough { get; protected set; }
	public bool CanPlayersUninstall { get; protected set; }
	public bool CanPlayersSmash { get; protected set; }
	public Difficulty UninstallDifficultyHingeSide { get; protected set; }
	public Difficulty UninstallDifficultyNotHingeSide { get; protected set; }
	public Difficulty SmashDifficulty { get; protected set; }
	public ITraitDefinition? UninstallTrait { get; protected set; }
	public bool CanFireThrough { get; protected set; }
	public bool CanBeOpenedByPlayers { get; protected set; } = true;

	protected void LoadDoorPrototypeData(XElement root)
	{
		var attr = root.Attribute("SeeThrough");
		if (attr is not null)
		{
			SeeThrough = Convert.ToBoolean(attr.Value);
		}

		attr = root.Attribute("CanFireThrough");
		if (attr is not null)
		{
			CanFireThrough = bool.Parse(attr.Value);
		}

		var element = root.Element("InstalledExitDescription");
		if (element is not null)
		{
			InstalledExitDescription = element.Value;
		}

		element = root.Element("CanBeOpenedByPlayers");
		CanBeOpenedByPlayers = element is null || bool.Parse(element.Value);

		element = root.Element("Uninstall");
		if (element is null)
		{
			CanPlayersUninstall = true;
			UninstallDifficultyHingeSide = Difficulty.Normal;
			UninstallDifficultyNotHingeSide = Difficulty.ExtremelyHard;
		}
		else
		{
			attr = element.Attribute("CanPlayersUninstall");
			if (attr is not null)
			{
				CanPlayersUninstall = bool.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallDifficultyHingeSide");
			if (attr is not null)
			{
				UninstallDifficultyHingeSide = (Difficulty)int.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallDifficultyNotHingeSide");
			if (attr is not null)
			{
				UninstallDifficultyNotHingeSide = (Difficulty)int.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallTrait");
			if (attr is not null && long.TryParse(attr.Value, out var uninstallTraitId) && uninstallTraitId > 0)
			{
				UninstallTrait = Gameworld.Traits.Get(uninstallTraitId);
			}
		}

		element = root.Element("Smash");
		if (element is null)
		{
			CanPlayersSmash = true;
			SmashDifficulty = Difficulty.Normal;
		}
		else
		{
			attr = element.Attribute("CanPlayersSmash");
			if (attr is not null)
			{
				CanPlayersSmash = bool.Parse(attr.Value);
			}

			attr = element.Attribute("SmashDifficulty");
			if (attr is not null)
			{
				SmashDifficulty = (Difficulty)int.Parse(attr.Value);
			}
		}
	}

	protected XElement SaveDoorPrototypeData(XElement root)
	{
		root.Add(new XAttribute("SeeThrough", SeeThrough));
		root.Add(new XAttribute("CanFireThrough", CanFireThrough));
		root.Add(new XElement("InstalledExitDescription",
			!string.IsNullOrWhiteSpace(InstalledExitDescription) ? InstalledExitDescription : "door"));
		root.Add(new XElement("CanBeOpenedByPlayers", CanBeOpenedByPlayers));
		root.Add(new XElement("Uninstall", new XAttribute("CanPlayersUninstall", CanPlayersUninstall),
			new XAttribute("UninstallDifficultyHingeSide", (int)UninstallDifficultyHingeSide),
			new XAttribute("UninstallDifficultyNotHingeSide", (int)UninstallDifficultyNotHingeSide),
			new XAttribute("UninstallTrait", UninstallTrait?.Id ?? 0)));
		root.Add(new XElement("Smash", new XAttribute("CanPlayersSmash", CanPlayersSmash),
			new XAttribute("SmashDifficulty", (int)SmashDifficulty)));
		return root;
	}

	protected string DescribeDoorCharacteristics(ICharacter actor, bool includeCanBeOpenedByPlayers)
	{
		var openableText = includeCanBeOpenedByPlayers
			? $"\nPlayers may {(CanBeOpenedByPlayers ? "open or close it normally".ColourValue() : "not open or close it normally".ColourError())}."
			: string.Empty;
		return
			$"This item is {(SeeThrough ? "a transparent" : "an opaque")} door, and when installed in an exit will show as {InstalledExitDescription.Colour(Telnet.Yellow)}. " +
			$"{(CanPlayersUninstall
				? $"It can be removed by players at a difficulty of {UninstallDifficultyHingeSide.DescribeColoured()} (hinge) / {UninstallDifficultyNotHingeSide.DescribeColoured()} (non-hinge) with the {(UninstallTrait is not null ? UninstallTrait.Name.TitleCase().Colour(Telnet.Green) : "None")} skill"
				: "It cannot be removed by players")}. " +
			$"{(CanPlayersSmash
				? $"It can be smashed by players at a difficulty of {SmashDifficulty.DescribeColoured()}"
				: "It cannot be smashed by players")}. " +
			$"{(CanFireThrough ? "It can be fired through when closed" : "It cannot be fired through when closed")}." +
			openableText;
	}

	protected bool BuildingCommandUninstallable(ICharacter actor, StringStack command)
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
		if (trait is null)
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
			nonHingeDifficulty.DescribeColoured());
		return true;
	}

	protected bool BuildingCommandSmashable(ICharacter actor, StringStack command)
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

	protected bool BuildingCommandInstalledExitDescription(ICharacter actor, StringStack command)
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

	protected bool BuildingCommandSeeThrough(ICharacter actor, StringStack command)
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

	protected bool BuildingCommandFire(ICharacter actor)
	{
		CanFireThrough = !CanFireThrough;
		actor.Send(CanFireThrough
			? "You can now fire ranged weapons through this component when it is closed."
			: "You can no longer fire ranged weapons through this component when it is closed.");
		Changed = true;
		return true;
	}

	protected bool BuildingCommandCanBeOpenedByPlayers(ICharacter actor)
	{
		CanBeOpenedByPlayers = !CanBeOpenedByPlayers;
		Changed = true;
		actor.OutputHandler.Send(CanBeOpenedByPlayers
			? "This door can now be opened by players using ordinary means like the OPEN command."
			: "This door can no longer be opened by players using ordinary means, and must be opened and closed automatically.");
		return true;
	}

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "removable":
            case "uninstall":
            case "uninstallable":
                return BuildingCommandUninstallable(actor, command);
            case "smashable":
                return BuildingCommandSmashable(actor, command);
            case "installed description":
            case "installed":
            case "installed_description":
            case "exit_description":
            case "exit description":
            case "exitdesc":
            case "exit":
                return BuildingCommandInstalledExitDescription(actor, command);
            case "see through":
            case "seethrough":
            case "transparent":
            case "opaque":
                return BuildingCommandSeeThrough(actor, command);
            case "fire":
                return BuildingCommandFire(actor);
            case "open":
            case "openable":
            case "canbeopened":
            case "canopen":
                return BuildingCommandCanBeOpenedByPlayers(actor);
            default:
                return base.BuildingCommand(actor, command);
        }
    }
}

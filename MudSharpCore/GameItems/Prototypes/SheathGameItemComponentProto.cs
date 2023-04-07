using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class SheathGameItemComponentProto : GameItemComponentProto
{
	public SizeCategory MaximumSize { get; protected set; }
	public Difficulty StealthDrawDifficulty { get; protected set; }
	public bool DesignedForGuns { get; protected set; }

	public override string TypeDescription => "Sheath";

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("StealthDrawDifficulty");
		if (attr != null)
		{
			StealthDrawDifficulty = (Difficulty)int.Parse(attr.Value);
		}

		attr = root.Attribute("MaximumSize");
		if (attr != null)
		{
			MaximumSize = (SizeCategory)int.Parse(attr.Value);
		}

		attr = root.Attribute("DesignedForGuns");
		DesignedForGuns = attr != null && bool.Parse(attr.Value);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("sheath", true,
			(gameworld, account) => new SheathGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Sheath", (proto, gameworld) => new SheathGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Sheath",
			$"A special {"[container]".Colour(Telnet.BoldGreen)} that can hold one weapon and works with the {"[sheath]".Colour(Telnet.Yellow)} command",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new SheathGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SheathGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new SheathGameItemComponentProto(proto, gameworld));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XAttribute("StealthDrawDifficulty", (int)StealthDrawDifficulty),
			new XAttribute("MaximumSize", (int)MaximumSize),
			new XAttribute("DesignedForGuns", DesignedForGuns)
		).ToString();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{3:N0}r{4:N0}, {5})\n\nThis item can sheath {6} of up to size {1} and is {2} to draw stealthily from.",
			"Sheath Item Component".Colour(Telnet.Cyan),
			MaximumSize.Describe().Colour(Telnet.Green),
			StealthDrawDifficulty.Describe().Colour(Telnet.Green),
			Id,
			RevisionNumber,
			Name,
			DesignedForGuns ? "firearms" : "melee weapons"
		);
	}

	private bool BuildingCommandStealthDrawDifficulty(ICharacter character, StringStack command)
	{
		var difficultyText = command.PopSpeech();
		var difficulty =
			Enum.GetValues(typeof(Difficulty))
			    .OfType<Difficulty>()
			    .FirstOrDefault(
				    x => x.Describe().Equals(difficultyText, StringComparison.InvariantCultureIgnoreCase));
		if (difficulty.IsDefault())
		{
			character.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		StealthDrawDifficulty = difficulty;
		Changed = true;
		character.OutputHandler.Send("It is now " + StealthDrawDifficulty.Describe().Colour(Telnet.Green) +
		                             " to stealthily draw from this sheath.");
		return true;
	}

	private bool BuildingCommandMaximumSize(ICharacter character, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			character.OutputHandler.Send("What size do you want to set the limit for this component to?");
			return false;
		}

		var size = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().ToList();
		SizeCategory target;
		if (size.Any(x => x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase)))
		{
			target = size.FirstOrDefault(x =>
				x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));
		}
		else
		{
			character.OutputHandler.Send("That is not a valid item size. See SHOW ITEMSIZES for a correct list.");
			return false;
		}

		MaximumSize = target;
		Changed = true;
		character.OutputHandler.Send("This sheath will now only allow items of up to size \"" + target.Describe() +
		                             "\".");
		return true;
	}

	public bool BuildingCommandGuns(ICharacter character, StringStack command)
	{
		DesignedForGuns = !DesignedForGuns;
		Changed = true;
		character.Send($"This sheath is now designed for {(DesignedForGuns ? "firearms" : "melee weapons")}.");
		return true;
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tsize <size> - the maximum size of the item this sheath can hold\n\tdifficulty <difficulty> - the difficulty to stealthily draw from this sheath\n\tgun - toggles whether the sheath is for guns or melee weapons";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter character, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "maximum size":
			case "max size":
			case "maxsize":
			case "size":
				return BuildingCommandMaximumSize(character, command);
			case "difficulty":
			case "stealthdifficulty":
			case "stealth difficulty":
				return BuildingCommandStealthDrawDifficulty(character, command);
			case "guns":
			case "firearms":
			case "gun":
			case "firearm":
				return BuildingCommandGuns(character, command);
			default:
				return base.BuildingCommand(character, command);
		}
	}

	#region Constructors

	protected SheathGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected SheathGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Sheath")
	{
		StealthDrawDifficulty = Difficulty.Normal;
		MaximumSize = SizeCategory.Small;
		Changed = true;
	}

	#endregion
}
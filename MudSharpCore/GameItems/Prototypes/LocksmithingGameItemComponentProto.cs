using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class LocksmithingGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Locksmithing Tool";
	public int DifficultyAdjustment { get; set; }
	public bool UsableForInstallation { get; set; }
	public bool UsableForFabrication { get; set; }
	public bool UsableForConfiguration { get; set; }
	public bool Breakable { get; set; }

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("locksmithing", true,
			(gameworld, account) => new LocksmithingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("locksmith", false,
			(gameworld, account) => new LocksmithingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("locksmithing tools", false,
			(gameworld, account) => new LocksmithingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("LocksmithingTools", false,
			(gameworld, account) => new LocksmithingGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Locksmithing Tool",
			(proto, gameworld) => new LocksmithingGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"LocksmithingTools",
			$"The item becomes usable as a tool for locksmithing actions",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Locksmithing Tool Game Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}\n\nThis item is a locksmithing tool and {(UsableForInstallation ? "can".Colour(Telnet.Green) : "cannot".Colour(Telnet.Red))} be used for installation and configuration. It {(Breakable ? "can".Colour(Telnet.Green) : "cannot".Colour(Telnet.Red))} break upon major failure. It provides {(DifficultyAdjustment == 0 ? "no penalty or bonus".Colour(Telnet.Yellow) + " to locksmithing checks." : DifficultyAdjustment < 0 ? $"a penalty of {(DifficultyAdjustment * -1).ToString("N0").Colour(Telnet.Red)} degrees" : $"a bonus of {DifficultyAdjustment.ToString("N0").Colour(Telnet.Green)} degrees")}.";
	}

	protected override void LoadFromXml(XElement root)
	{
		DifficultyAdjustment = int.Parse(root.Element("DifficultyAdjustment")?.Value ?? "0");
		UsableForInstallation = bool.Parse(root.Element("UsableForInstallation")?.Value ?? "False");
		UsableForConfiguration = bool.Parse(root.Element("UsableForConfiguration")?.Value ?? "False");
		UsableForFabrication = bool.Parse(root.Element("UsableForFabrication")?.Value ?? "False");
		Breakable = bool.Parse(root.Element("Breakable")?.Value ?? "False");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("DifficultyAdjustment", DifficultyAdjustment),
			new XElement("UsableForInstallation", UsableForInstallation),
			new XElement("UsableForConfiguration", UsableForConfiguration),
			new XElement("UsableForFabrication", UsableForFabrication),
			new XElement("Breakable", Breakable)
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LocksmithingGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new LocksmithingGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LocksmithingGameItemComponentProto(proto, gameworld));
	}

	#region Constructors

	protected LocksmithingGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected LocksmithingGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Locksmithing Tool")
	{
		Changed = true;
	}

	#endregion

	#region Building Commands

	#region Overrides of GameItemComponentProto

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbonus <#> - a positive (bonus) or negative (penalty) number of difficulty steps this tool imposes\n\tbreakable - toggles whether it breaks on a major failure\n\tinstall - toggles whether it can be used to install locks\n\tconfig - toggles whether it can be used to configure locks\n\tfabrication - toggles whether it can be used to fabricate locks and keys";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "bonus":
			case "penalty":
			case "adjustment":
			case "difficulty":
			case "diff":
				return BuildingCommandDifficulty(actor, command);
			case "breakable":
			case "break":
				return BuildingCommandBreakable(actor, command);
			case "installation":
			case "install":
				return BuildingCommandInstallation(actor, command);
			case "configuration":
			case "config":
				return BuildingCommandConfiguration(actor, command);
			case "fabrication":
				return BuildingCommandFabrication(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	#endregion

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What difficulty adjustment should this locksmithing tool give to locksmithing checks? Use negative values for penalties and positive values for bonuses, or zero for none.");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send(
				"You must enter a number for the difficulty adjustment. Use negative values for penalties and positive values for bonuses, or zero for none.");
			return false;
		}

		if (value > 10 || value < -10)
		{
			actor.Send("The maximum number of degrees of bonus or penalty is 10.");
			return false;
		}

		DifficultyAdjustment = value;
		Changed = true;
		actor.Send(
			$"This set of locksmithing tools now gives a {(DifficultyAdjustment < 0 ? "penalty" : "bonus")} of {Math.Abs(DifficultyAdjustment)} {(Math.Abs(DifficultyAdjustment) == 1 ? "degree" : "degrees")}.");
		return true;
	}

	private bool BuildingCommandBreakable(ICharacter actor, StringStack command)
	{
		Breakable = !Breakable;
		actor.Send(Breakable
			? "This item can now break in a major failure."
			: "This item can no longer break in a major failure.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandInstallation(ICharacter actor, StringStack command)
	{
		UsableForInstallation = !UsableForInstallation;
		actor.Send(Breakable
			? "This item can now be used for installation."
			: "This item can no longer be used for installation.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandFabrication(ICharacter actor, StringStack command)
	{
		UsableForFabrication = !UsableForFabrication;
		actor.Send(Breakable
			? "This item can now be used for fabrication."
			: "This item can no longer be used for fabrication.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandConfiguration(ICharacter actor, StringStack command)
	{
		UsableForConfiguration = !UsableForConfiguration;
		actor.Send(Breakable
			? "This item can now be used for configuration."
			: "This item can no longer be used for configuration.");
		Changed = true;
		return true;
	}

	#endregion Building Commands
}
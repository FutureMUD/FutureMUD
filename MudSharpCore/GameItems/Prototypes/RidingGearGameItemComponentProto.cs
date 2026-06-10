using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class RidingGearGameItemComponentProto : GameItemComponentProto, IRidingGearPrototype
{
	public override string TypeDescription => "RidingGear";

	protected RidingGearGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "RidingGear")
	{
		Roles = RidingGearRole.Saddle;
	}

	protected RidingGearGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Roles = Enum.TryParse<RidingGearRole>(root.Element("Roles")?.Value, out var roles)
			? roles
			: RidingGearRole.None;
		ControlBonus = double.TryParse(root.Element("ControlBonus")?.Value, out var control) ? control : 0.0;
		StabilityBonus = double.TryParse(root.Element("StabilityBonus")?.Value, out var stability) ? stability : 0.0;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Roles", Roles),
			new XElement("ControlBonus", ControlBonus),
			new XElement("StabilityBonus", StabilityBonus)
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new RidingGearGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RidingGearGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("RidingGear".ToLowerInvariant(), true,
			(gameworld, account) => new RidingGearGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("RidingGear", (proto, gameworld) => new RidingGearGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("RidingGear",
			"Makes this item count as tack or other riding gear for mounts.",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new RidingGearGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\trole <role> - toggles a riding gear role\n\tcontrol <bonus> - sets the riding control bonus or penalty\n\tstability <bonus> - sets the stay-mounted stability bonus or penalty";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "role":
			case "roles":
				return BuildingCommandRole(actor, command);
			case "control":
			case "controlbonus":
				return BuildingCommandControl(actor, command);
			case "stability":
			case "stabilitybonus":
				return BuildingCommandStability(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandRole(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send($"Which riding gear role do you want to toggle? Valid roles are {ValidRolesText(actor)}.");
			return false;
		}

		if (!TryParseRole(command.SafeRemainingArgument, out var role) || role == RidingGearRole.None)
		{
			actor.Send($"That is not a valid riding gear role. Valid roles are {ValidRolesText(actor)}.");
			return false;
		}

		Roles = Roles.HasFlag(role) ? Roles & ~role : Roles | role;
		Changed = true;
		actor.Send($"This riding gear now has the following roles: {DescribeRoles(actor)}.");
		return true;
	}

	private bool BuildingCommandControl(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send($"What control bonus should this gear provide? Every {StandardCheck.BonusesPerDifficultyLevel.ToString("N0", actor).ColourValue()} points is about one difficulty stage.");
			return false;
		}

		ControlBonus = value;
		Changed = true;
		actor.Send($"This riding gear now provides a control modifier of {ControlBonus.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandStability(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send($"What stability bonus should this gear provide? Every {StandardCheck.BonusesPerDifficultyLevel.ToString("N0", actor).ColourValue()} points is about one difficulty stage.");
			return false;
		}

		StabilityBonus = value;
		Changed = true;
		actor.Send($"This riding gear now provides a stay-mounted modifier of {StabilityBonus.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private static bool TryParseRole(string text, out RidingGearRole role)
	{
		var normalised = text.Replace(" ", string.Empty).Replace("-", string.Empty);
		return Enum.TryParse(normalised, true, out role);
	}

	private static string ValidRolesText(ICharacter actor)
	{
		return Enum.GetValues<RidingGearRole>()
		           .Where(x => x != RidingGearRole.None)
		           .Select(x => x.DescribeEnum().ColourName())
		           .ListToString();
	}

	private string DescribeRoles(ICharacter actor)
	{
		return Enum.GetValues<RidingGearRole>()
		           .Where(x => x != RidingGearRole.None && Roles.HasFlag(x))
		           .Select(x => x.DescribeEnum().ColourName())
		           .DefaultIfEmpty("none".ColourError())
		           .ListToString();
	}

	public RidingGearRole Roles { get; set; }
	public double ControlBonus { get; set; }
	public double StabilityBonus { get; set; }

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nRoles: {4}\nControl Modifier: {5:N2}\nStability Modifier: {6:N2}",
			"RidingGear Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			DescribeRoles(actor),
			ControlBonus,
			StabilityBonus
		);
	}
}

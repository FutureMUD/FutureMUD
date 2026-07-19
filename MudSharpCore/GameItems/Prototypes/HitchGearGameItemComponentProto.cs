using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class HitchGearGameItemComponentProto : GameItemComponentProto, IHitchGearPrototype
{
	public override string TypeDescription => "HitchGear";

	protected HitchGearGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "HitchGear")
	{
		Roles = HitchGearRole.Rope;
		EffortMultiplier = 2.0;
		MaximumUsers = 5;
		MaximumTowedWeight = 0.0;
	}

	protected HitchGearGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Roles = Enum.TryParse<HitchGearRole>(root.Element("Roles")?.Value, out var roles)
			? roles
			: HitchGearRole.None;
		MaximumUsers = int.TryParse(root.Element("MaximumUsers")?.Value, out var users) ? users : 1;
		EffortMultiplier = double.TryParse(root.Element("EffortMultiplier")?.Value, out var multiplier) ? multiplier : 1.0;
		MaximumTowedWeight = double.TryParse(root.Element("MaximumTowedWeight")?.Value, out var weight) ? weight : 0.0;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Roles", Roles),
			new XElement("MaximumUsers", MaximumUsers),
			new XElement("EffortMultiplier", EffortMultiplier),
			new XElement("MaximumTowedWeight", MaximumTowedWeight)
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new HitchGearGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new HitchGearGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("HitchGear".ToLowerInvariant(), true,
			(gameworld, account) => new HitchGearGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("HitchGear", (proto, gameworld) => new HitchGearGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("HitchGear",
			"Makes this item usable as gear for hitching mounts, characters and vehicles together.",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new HitchGearGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\trole <role> - toggles a hitch gear role\n\tusers <#> - the number of people who can use this hitch gear at once\n\tbonus <%> - sets the drag capacity multiplier when this hitch gear is used\n\tweight <#> - sets the maximum towed weight for this gear, or 0 for unlimited";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "role":
			case "roles":
				return BuildingCommandRole(actor, command);
			case "multiplier":
			case "mult":
			case "bonus":
				return BuildingCommandMultiplier(actor, command);
			case "maximum":
			case "users":
			case "max":
				return BuildingCommandUsers(actor, command);
			case "weight":
			case "maxtowed":
			case "towed":
				return BuildingCommandMaximumTowedWeight(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandRole(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send($"Which hitch gear role do you want to toggle? Valid roles are {ValidRolesText(actor)}.");
			return false;
		}

		if (!TryParseRole(command.SafeRemainingArgument, out var role) || role == HitchGearRole.None)
		{
			actor.Send($"That is not a valid hitch gear role. Valid roles are {ValidRolesText(actor)}.");
			return false;
		}

		Roles = Roles.HasFlag(role) ? Roles & ~role : Roles | role;
		Changed = true;
		actor.Send($"This hitch gear now has the following roles: {DescribeRoles(actor)}.");
		return true;
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What should the drag capacity multiplier be for this hitch gear?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var value) || value < 1.0)
		{
			actor.Send($"The value must be a valid percentage equal to or greater than {1.0.ToString("P0", actor)}.");
			return false;
		}

		EffortMultiplier = value;
		Changed = true;
		actor.Send($"This item will now multiply dragging capacity by {EffortMultiplier.ToString("P3", actor).ColourValue()} when used as hitch gear.");
		return true;
	}

	private bool BuildingCommandUsers(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many people should be able to use this hitch gear simultaneously?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1)
		{
			actor.Send("The value must be a valid number equal to or greater than 1.");
			return false;
		}

		MaximumUsers = value;
		Changed = true;
		actor.Send($"This hitch gear can now be used simultaneously by {MaximumUsers.ToString("N0", actor).ColourValue()} people.");
		return true;
	}

	private bool BuildingCommandMaximumTowedWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send("What maximum towed weight should this hitch gear support? Use 0 for unlimited.");
			return false;
		}

		MaximumTowedWeight = value;
		Changed = true;
		actor.Send(MaximumTowedWeight == 0.0
			? "This hitch gear no longer has its own maximum towed weight limit."
			: $"This hitch gear can now support up to {MaximumTowedWeight.ToString("N2", actor).ColourValue()} towed weight.");
		return true;
	}

	private static bool TryParseRole(string text, out HitchGearRole role)
	{
		var normalised = text.Replace(" ", string.Empty).Replace("-", string.Empty);
		return Enum.TryParse(normalised, true, out role);
	}

	private static string ValidRolesText(ICharacter actor)
	{
		return Enum.GetValues<HitchGearRole>()
		           .Where(x => x != HitchGearRole.None)
		           .Select(x => x.DescribeEnum().ColourName())
		           .ListToString();
	}

	private string DescribeRoles(ICharacter actor)
	{
		return Enum.GetValues<HitchGearRole>()
		           .Where(x => x != HitchGearRole.None && Roles.HasFlag(x))
		           .Select(x => x.DescribeEnum().ColourName())
		           .DefaultIfEmpty("none".ColourError())
		           .ListToString();
	}

	public HitchGearRole Roles { get; set; }
	public double EffortMultiplier { get; set; }
	public int MaximumUsers { get; set; }
	public double MaximumTowedWeight { get; set; }

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nRoles: {4}\nDrag Multiplier: {5:N3}\nMaximum Users: {6:N0}\nMaximum Towed Weight: {7}",
			"HitchGear Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			DescribeRoles(actor),
			EffortMultiplier,
			MaximumUsers,
			MaximumTowedWeight == 0.0 ? "unlimited".ColourValue() : MaximumTowedWeight.ToString("N2", actor).ColourValue()
		);
	}
}

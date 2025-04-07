using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ThrownWeaponGameItemComponentProto : GameItemComponentProto
{
	protected ThrownWeaponGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ThrownWeapon")
	{
		RangedWeaponType =
			gameworld.RangedWeaponTypes.FirstOrDefault(x => x.RangedWeaponType == Combat.RangedWeaponType.Thrown);
	}

	protected ThrownWeaponGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IRangedWeaponType RangedWeaponType { get; set; }
	public IWeaponType MeleeWeaponType { get; set; }
#nullable enable
	public IFutureProg? CanWieldProg { get; private set; }
	public IFutureProg? WhyCannotWieldProg { get; private set; }
#nullable restore
	public override string TypeDescription => "ThrownWeapon";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("RangedWeaponType");
		if (element != null)
		{
			RangedWeaponType = Gameworld.RangedWeaponTypes.Get(long.Parse(element.Value));
		}

		element = root.Element("MeleeWeaponType");
		if (element != null)
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
		CanWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanWieldProg")?.Value ?? "0"));
		WhyCannotWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotWieldProg")?.Value ?? "0"));
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a thrown weapon of ranged type {4} and melee type {5}.\nThe CanWield prog is {6} and the WhyCannotWield prog is {7}.",
			"Thrown Weapon Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RangedWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			MeleeWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			CanWieldProg?.MXPClickableFunctionName() ?? "None".ColourError(),
			WhyCannotWieldProg?.MXPClickableFunctionName() ?? "None".ColourError()
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
				new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0),
				new XElement("CanWieldProg", CanWieldProg?.Id ?? 0),
				new XElement("WhyCannotWieldProg", WhyCannotWieldProg?.Id ?? 0)
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("throwing weapon", true,
			(gameworld, account) => new ThrownWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("throwing", false,
			(gameworld, account) => new ThrownWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("thrown", false,
			(gameworld, account) => new ThrownWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("thrown weapon", false,
			(gameworld, account) => new ThrownWeaponGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ThrownWeapon",
			(proto, gameworld) => new ThrownWeaponGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Thrown",
			$"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with throwing weapon mechanics",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.\n\tmelee <melee type> - sets the melee weapon type for this component. See {"show weapons".FluentTagMXP("send", "href='show weapons'")} for a list."
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ThrownWeaponGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ThrownWeaponGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ThrownWeaponGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	public override string ShowBuildingHelp =>
		$@"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3ranged <ranged type>#0 - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.
	#3melee <melee type>#0 - sets the melee weapon type for this component. See {"show weapons".FluentTagMXP("send", "href='show weapons'")} for a list.
	#3canwield <prog>#0 - sets a prog controlling if this can be wielded
	#3canwield none#0 - removes a canwield prog
	#3whycantwield <prog>#0 - sets a prog giving the error message if canwield fails
	#3whycantwield none#0 - clears the whycantwield prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ranged":
			case "ranged type":
			case "rangedtype":
				return BuildingCommand_Type(actor, command);
			case "melee":
			case "melee type":
			case "meleetype":
				return BuildingCommand_MeleeType(actor, command);
			case "canwield":
			case "canwieldprog":
				return BuildingCommandCanWieldProg(actor, command);
			case "whycantwield":
			case "whycantwieldprog":
			case "whycannotwield":
			case "whycannotwieldprog":
				return BuildingCommandWhyCannotWieldProg(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandCanWieldProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a prog, or the keyword #3none#0 to remove one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CanWieldProg = null;
			Changed = true;
			actor.OutputHandler.Send($"This item will no longer use a prog to determine if it can be wielded.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Item]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanWieldProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This item will now use the {prog.MXPClickableFunctionName()} prog to determine if it can be wielded.");
		return true;
	}

	private bool BuildingCommandWhyCannotWieldProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a prog, or the keyword #3none#0 to remove one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CanWieldProg = null;
			Changed = true;
			actor.OutputHandler.Send($"This item will no longer use a prog to generate an error message if it cannot be wielded.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Item]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WhyCannotWieldProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This item will now use the {prog.MXPClickableFunctionName()} prog to generate an error message if it cannot be wielded.");
		return true;
	}

	private bool BuildingCommand_MeleeType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which melee weapon type do you want to set for this ranged weapon?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.WeaponTypes.Get(value)
			: actor.Gameworld.WeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such melee weapon type.");
			return false;
		}

		MeleeWeaponType = type;
		actor.Send(
			$"This ranged weapon will now be of melee type {MeleeWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Type(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which weapon type do you want to set for this ranged weapon?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.RangedWeaponTypes.Get(value)
			: actor.Gameworld.RangedWeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such ranged weapon type.");
			return false;
		}

		if (type.RangedWeaponType != Combat.RangedWeaponType.Thrown)
		{
			actor.Send("You can only give throwing weapons a ranged weapon type that is also for a throwing weapon.");
			return false;
		}

		RangedWeaponType = type;
		actor.Send(
			$"This ranged weapon will now be of type {RangedWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		return RangedWeaponType != null && MeleeWeaponType != null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (RangedWeaponType == null)
		{
			return "You must first give this component a Ranged Weapon Type.";
		}

		return MeleeWeaponType == null
			? "You must first give this component a Melee Weapon Type."
			: base.WhyCannotSubmit();
	}

	#endregion
}
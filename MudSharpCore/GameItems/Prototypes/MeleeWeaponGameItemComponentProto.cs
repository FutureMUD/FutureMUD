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

public class MeleeWeaponGameItemComponentProto : GameItemComponentProto
{
	protected MeleeWeaponGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "MeleeWeapon")
	{
	}

	protected MeleeWeaponGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IWeaponType WeaponType { get; private set; }
#nullable enable
	public IFutureProg? CanWieldProg { get; private set; }
	public IFutureProg? WhyCannotWieldProg { get; private set; }
#nullable restore

	public override string TypeDescription => "MeleeWeapon";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("WeaponType");
		if (element != null)
		{
			WeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
		CanWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanWieldProg")?.Value ?? "0"));
		WhyCannotWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotWieldProg")?.Value ?? "0"));
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a melee weapon of type {4}.\nThe CanWield prog is {5} and the WhyCannotWield prog is {6}.",
			"Melee Weapon Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			WeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			CanWieldProg?.MXPClickableFunctionName() ?? "None".ColourError(),
			WhyCannotWieldProg?.MXPClickableFunctionName() ?? "None".ColourError()
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("WeaponType", WeaponType?.Id ?? 0),
				new XElement("CanWieldProg", CanWieldProg?.Id ?? 0),
				new XElement("WhyCannotWieldProg", WhyCannotWieldProg?.Id ?? 0)
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("weapon", true,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("melee weapon", false,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("meleeweapon", false,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("melee", false,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));

		manager.AddDatabaseLoader("MeleeWeapon",
			(proto, gameworld) => new MeleeWeaponGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"MeleeWeapon",
			$"Turns an item into a {"[melee weapon]".Colour(Telnet.BoldGreen)}",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tmelee <melee type> - sets the melee weapon type for this component. See {"show weapons".FluentTagMXP("send", "href='show weapons'")} for a list."
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new MeleeWeaponGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MeleeWeaponGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MeleeWeaponGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	public override string ShowBuildingHelp =>
		$@"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3melee <melee type>#0 - sets the melee weapon type for this component. See {"show weapons".FluentTagMXP("send", "href='show weapons'")} for a list.
	#3canwield <prog>#0 - sets a prog controlling if this can be wielded
	#3canwield none#0 - removes a canwield prog
	#3whycantwield <prog>#0 - sets a prog giving the error message if canwield fails
	#3whycantwield none#0 - clears the whycantwield prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "melee":
				return BuildingCommand_Type(actor, command);
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

	private bool BuildingCommand_Type(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which weapon type do you want to set for this melee weapon?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.WeaponTypes.Get(value)
			: actor.Gameworld.WeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such weapon type.");
			return false;
		}

		WeaponType = type;
		actor.Send($"This melee weapon will now be of type {WeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		return WeaponType != null && base.CanSubmit();
	}

	#region Overrides of EditableItem

	public override string WhyCannotSubmit()
	{
		return WeaponType == null ? "You must first give this component a Weapon Type." : base.WhyCannotSubmit();
	}

	#endregion

	#endregion
}
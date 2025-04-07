using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class WieldablePropGameItemComponentProto : GameItemComponentProto
{
	private static readonly string _showString = "Wieldable Prop Item Component".Colour(Telnet.Cyan) + "\n\n" +
	                                             "This item can be wielded by Characters.\n";

	protected WieldablePropGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Wieldable")
	{
	}

	protected WieldablePropGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Wieldable";

	protected override void LoadFromXml(XElement root)
	{
		CanWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanWieldProg")?.Value ?? "0"));
		WhyCannotWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotWieldProg")?.Value ?? "0"));
	}

#nullable enable
	public IFutureProg? CanWieldProg { get; private set; }
	public IFutureProg? WhyCannotWieldProg { get; private set; }
#nullable restore

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "canwield":
			case "canwieldprog":
				return BuildingCommandCanWieldProg(actor, command);
			case "whycantwield":
			case "whycantwieldprog":
			case "whycannotwield":
			case "whycannotwieldprog":
				return BuildingCommandWhyCannotWieldProg(actor, command);
		}
		return base.BuildingCommand(actor, command);
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

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3canwield <prog>#0 - sets a prog controlling if this can be wielded
	#3canwield none#0 - removes a canwield prog
	#3whycantwield <prog>#0 - sets a prog giving the error message if canwield fails
	#3whycantwield none#0 - clears the whycantwield prog";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\n\nThis item can be wielded by characters.\nThe CanWield prog is {4} and the WhyCannotWield prog is {5}.",
			"Wieldable Prop Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			CanWieldProg?.MXPClickableFunctionName() ?? "None".ColourError(),
			WhyCannotWieldProg?.MXPClickableFunctionName() ?? "None".ColourError());
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
				new XElement("CanWieldProg", CanWieldProg?.Id ?? 0),
				new XElement("WhyCannotWieldProg", WhyCannotWieldProg?.Id ?? 0))
			.ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("wieldable", true,
			(gameworld, account) => new WieldablePropGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Wieldable",
			(proto, gameworld) => new WieldablePropGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Wieldable",
			$"Makes the item able to be wielded, even though it's not a weapon. Use for props.",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new WieldablePropGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new WieldablePropGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new WieldablePropGameItemComponentProto(proto, gameworld));
	}
}
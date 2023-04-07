using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Decorators;

namespace MudSharp.GameItems.Prototypes;

public class StackableGameItemComponentProto : GameItemComponentProto
{
	protected StackableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected StackableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Stackable")
	{
	}

	public IStackDecorator DescriptionDecorator { get; protected set; }
	public override string TypeDescription => "Stackable";

	protected override void LoadFromXml(XElement root)
	{
		var attribute = root.Attribute("Decorator");
		DescriptionDecorator = attribute != null
			? Gameworld.StackDecorators.Get(long.Parse(attribute.Value))
			: Gameworld.StackDecorators.First();
	}

	protected override string SaveToXml()
	{
		return "<Definition Decorator=\"" + (DescriptionDecorator?.Id ?? 0) + "\"/>";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("stackable", true,
			(gameworld, account) => new StackableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Stackable",
			(proto, gameworld) => new StackableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Stackable",
			$"Makes an item {"[stackable]".Colour(Telnet.Yellow)} (i.e. one item with quantity)",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new StackableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new StackableGameItemComponent(component, this, parent);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{2:N0}r{3:N0}, {4})\n\nThis item can be stacked. It uses the {1} decorator to describe quantities.",
			"Stackable Item Component".Colour(Telnet.Cyan),
			DescriptionDecorator != null
				? DescriptionDecorator.Name.TitleCase().Colour(Telnet.Cyan)
				: "undefined".Colour(Telnet.Red),
			Id,
			RevisionNumber,
			Name
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new StackableGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tdecorator <which>";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "decorator":
				return BuildingCommand_Decorator(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Decorator(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which stack decorator do you want to apply for this stackable?\nSee {"show stacks".FluentTagMXP("send", "href='show stacks' hint='Show a list of stack decorators'")} to see a list of valid options");
			return false;
		}

		var decorator = Gameworld.StackDecorators.GetByIdOrName(command.SafeRemainingArgument);
		if (decorator == null)
		{
			actor.OutputHandler.Send("That is not a valid decorator for stackable items.");
			return false;
		}

		DescriptionDecorator = decorator;
		Changed = true;
		actor.OutputHandler.Send(
			$"You set the decorator for this stackable to {DescriptionDecorator.Name.ColourValue()}.");
		return true;
	}
}
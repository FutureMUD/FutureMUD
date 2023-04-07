using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class WornTraitChangerGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "WornTraitChanger";

	#region Constructors

	protected WornTraitChangerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "WornTraitChanger")
	{
	}

	protected WornTraitChangerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		foreach (var element in root.Elements("Modifier"))
		{
			var trait = Gameworld.Traits.Get(long.Parse(element.Attribute("trait").Value));
			if (trait == null)
			{
				continue;
			}

			TraitModifiers.Add((trait, (TraitBonusContext)int.Parse(element.Attribute("context").Value),
				double.Parse(element.Attribute("bonus").Value)));
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			from bonus in TraitModifiers
			select new XElement("Modifier",
				new XAttribute("trait", bonus.Trait.Id),
				new XAttribute("bonus", bonus.Modifier),
				new XAttribute("context", (int)bonus.Context)
			)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new WornTraitChangerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new WornTraitChangerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("WornTraitChanger".ToLowerInvariant(), true,
			(gameworld, account) => new WornTraitChangerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("wornbonus", false,
			(gameworld, account) => new WornTraitChangerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("WornTraitChanger",
			(proto, gameworld) => new WornTraitChangerGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"WornTraitChanger",
			$"When worn, gives bonus/penalty to skills or attributes. Must be combined with a {"[wearable]".Colour(Telnet.BoldYellow)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new WornTraitChangerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbonus <trait> <modifier> <context> - adds or sets a bonus for a trait\n\tremove <trait> - removes all bonuses associated with a trait";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "remove":
				return BuildingCommandRemove(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you want to remove bonuses for?");
			return false;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.Last);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		TraitModifiers.RemoveAll(x => x.Trait == trait);
		Changed = true;
		actor.OutputHandler.Send($"You remove any bonuses associated with the {trait.Name.ColourValue()} trait.");
		return true;
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait do you want to add or edit a bonus for?");
			return false;
		}

		var trait = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Traits.Get(value)
			: Gameworld.Traits.GetByName(command.Last);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What bonus to this trait should be applied when this item is worn?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var bonus))
		{
			actor.OutputHandler.Send("You must specify a number for the bonus.");
			return false;
		}

		var context = TraitBonusContext.None;
		if (!command.IsFinished)
		{
			if (!Enum.TryParse<TraitBonusContext>(command.PopSpeech(), true, out context))
			{
				actor.OutputHandler.Send(
					$"That is not a valid context. Valid contexts include: {Enum.GetNames(typeof(TraitBonusContext)).Select(x => x.Colour(Telnet.Cyan)).ListToString()}");
				return false;
			}
		}

		TraitModifiers.RemoveAll(x => x.Trait == trait && x.Context == context);
		TraitModifiers.Add((trait, context, bonus));
		actor.OutputHandler.Send(
			$"When worn, this item will now give a bonus of {bonus.ToString("N2", actor).ColourValue()} to the {trait.Name.ColourValue()} trait {(context == TraitBonusContext.None ? "in all cases" : $"in the {context.DescribeEnum().Colour(Telnet.Cyan)} context")}.");
		Changed = true;
		return true;
	}

	#endregion

	public List<(ITraitDefinition Trait, TraitBonusContext Context, double Modifier)> TraitModifiers { get; } =
		new List<(ITraitDefinition Trait, TraitBonusContext context, double modifier)>();

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item provides bonuses to traits when worn:{4}",
			"WornTraitChanger Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			TraitModifiers
				.Select(x =>
					$"\t{x.Trait.Name.ColourValue()}: {x.Modifier.ToString("N2", actor).ColourValue()} for context {x.Context.DescribeEnum().Colour(Telnet.Cyan)}")
				.ListToCommaSeparatedValues("\n")
		);
	}

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		if (!TraitModifiers.Any())
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (!TraitModifiers.Any())
		{
			return "You must add at least one trait modifier.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion
}
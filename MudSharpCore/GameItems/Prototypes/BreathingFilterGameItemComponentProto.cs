using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class BreathingFilterGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "BreathingFilter";

	public Dictionary<IGas, IGas> FilteredGases { get; } = new();

	public double VolumePerFilter { get; protected set; }

	public long FilterProtoId { get; protected set; }

	#region Constructors

	protected BreathingFilterGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "BreathingFilter")
	{
		VolumePerFilter = 25000;
	}

	protected BreathingFilterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		VolumePerFilter = double.Parse(root.Element("VolumePerFilter").Value);
		FilterProtoId = long.Parse(root.Element("FilterProtoId").Value);
		foreach (var gas in root.Element("Gases").Elements())
		{
			FilteredGases[Gameworld.Gases.Get(long.Parse(gas.Attribute("from").Value))] =
				Gameworld.Gases.Get(long.Parse(gas.Attribute("to").Value));
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VolumePerFilter", VolumePerFilter),
			new XElement("FilterProtoId", FilterProtoId),
			new XElement("Gases",
				from pair in FilteredGases
				select new XElement("Gas", new XAttribute("from", pair.Key.Id), new XAttribute("to", pair.Value.Id)))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BreathingFilterGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BreathingFilterGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("BreathingFilter".ToLowerInvariant(), true,
			(gameworld, account) => new BreathingFilterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("BreathingFilter",
			(proto, gameworld) => new BreathingFilterGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"BreathingFilter",
			$"A {"[container]".Colour(Telnet.BoldGreen)} for filters that combined with another {"[wearable]".Colour(Telnet.BoldYellow)} transforms one gas type to another (e.g. smoke to breathable air)",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BreathingFilterGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tfilter <id> - sets the proto to use as a consumable\n\tcapacity <volume> - sets the capacity per consumable change\n\tconvert <from> <to> - converts one gas to another\n\tclear <gas> - removes a gas from the list of converted gases";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "filter":
				return BuildingCommandFilter(actor, command);
			case "capacity":
				return BuildingCommandCapacity(actor, command);
			case "convert":
				return BuildingCommandConvert(actor, command);
			case "clear":
				return BuildingCommandClear(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandConvert(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What gas would you like this filter to convert?");
			return false;
		}

		var from = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Gases.Get(value)
			: Gameworld.Gases.GetByName(command.Last);
		if (from == null)
		{
			actor.OutputHandler.Send("There is no such gas to filter.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What gas should this filter convert {from.Name.Colour(from.DisplayColour)} to?");
			return false;
		}

		var to = long.TryParse(command.PopSpeech(), out value)
			? Gameworld.Gases.Get(value)
			: Gameworld.Gases.GetByName(command.Last);
		if (to == null)
		{
			actor.OutputHandler.Send("There is no such gas to convert to.");
			return false;
		}

		FilteredGases[from] = to;
		Changed = true;
		actor.OutputHandler.Send(
			$"This breathing filter will now convert {from.Name.Colour(from.DisplayColour)} into {to.Name.Colour(to.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandClear(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which gas do you want to clear from the list of filtered gases?");
			return false;
		}

		var gas = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Gases.Get(value)
			: Gameworld.Gases.GetByName(command.Last);
		if (gas == null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return false;
		}

		if (!FilteredGases.ContainsKey(gas))
		{
			actor.OutputHandler.Send(
				$"This breathing filter does not filter the {gas.Name.Colour(gas.DisplayColour)} gas.");
			return false;
		}

		FilteredGases.Remove(gas);
		Changed = true;
		actor.OutputHandler.Send(
			$"This breathing filter no longer filters the {gas.Name.Colour(gas.DisplayColour)} gas.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How much volume of gas should this breathing filter filter before needing a consumable change?");
			return false;
		}

		var amount = Gameworld.UnitManager.GetBaseUnits(command.PopSpeech(), Framework.Units.UnitType.FluidVolume,
			out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid volume of gas.");
			return false;
		}

		VolumePerFilter = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This breathing filter now filters {Gameworld.UnitManager.DescribeMostSignificantExact(VolumePerFilter, Framework.Units.UnitType.FluidVolume, actor).ColourValue()} of gas before needing a consumable change.");
		return true;
	}

	private bool BuildingCommandFilter(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What is the ID number of the item prototype that serves as the consumable filter for this breathing filter?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid item prototype ID.");
			return false;
		}

		var proto = Gameworld.ItemProtos.Get(value);
		if (proto == null)
		{
			actor.OutputHandler.Send("That is not a valid item prototype ID.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("You must choose an item prototype that is approved for use.");
			return false;
		}

		FilterProtoId = proto.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This breathing filter will now use item #{FilterProtoId.ToString("N0", actor)} ({proto.ShortDescription.ColourObject()}) as a consumable filter cartridge.");
		return true;
	}

	#endregion

	public override bool CanSubmit()
	{
		if (FilterProtoId == 0)
		{
			return false;
		}

		if (!FilteredGases.Any())
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (FilterProtoId == 0)
		{
			return "You must first set a filter prototype ID.";
		}

		if (!FilteredGases.Any())
		{
			return "You must set at least one gas to be filtered.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item filters breathable gases. It consumes filters of proto {4}. It can filter {5} of gas before needing a filter change. {6}.",
			"BreathingFilter Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			FilterProtoId == 0
				? "Not Set".Colour(Telnet.Red)
				: $"#{FilterProtoId.ToString("N0", actor)}".Colour(Telnet.Cyan),
			Gameworld.UnitManager
			         .DescribeMostSignificantExact(VolumePerFilter, Framework.Units.UnitType.FluidVolume, actor)
			         .ColourValue(),
			FilteredGases.Any()
				? $"It filters {FilteredGases.Select(x => $"{x.Key.Name.Colour(x.Key.DisplayColour)} to {x.Value.Name.Colour(x.Value.DisplayColour)}").ListToString()}"
				: "No filtered gases have been set as yet"
		);
	}
}
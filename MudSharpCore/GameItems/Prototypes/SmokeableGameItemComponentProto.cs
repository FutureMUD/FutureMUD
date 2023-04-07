using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class SmokeableGameItemComponentProto : GameItemComponentProto
{
	protected SmokeableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Smokeable")
	{
		Changed = true;
	}

	protected SmokeableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public int SecondsOfFuel { get; set; }
	public int SecondsPerDrag { get; set; }
	public IFutureProg OnDragProg { get; set; }
	public string PlayerDescriptionEffectString { get; set; }
	public string RoomDescriptionEffectString { get; set; }
	public double SecondsOfEffectPerSecondOfFuel { get; set; }
	public IDrug Drug { get; set; }
	public double GramsPerDrag { get; set; }

	public override string TypeDescription => "Smokeable";

	protected override void LoadFromXml(XElement root)
	{
		SecondsOfFuel = int.Parse(root.Element("SecondsOfFuel")?.Value ?? "0");
		SecondsPerDrag = int.Parse(root.Element("SecondsPerDrag")?.Value ?? "0");
		SecondsOfEffectPerSecondOfFuel = double.Parse(root.Element("SecondsOfEffectPerSecondOfFuel")?.Value ?? "0");
		PlayerDescriptionEffectString = root.Element("PlayerDescriptionEffectString")?.Value ?? "";
		RoomDescriptionEffectString = root.Element("RoomDescriptionEffectString")?.Value ?? "";
		Drug = Gameworld.Drugs.Get(long.Parse(root.Element("Drug")?.Value ?? "0"));
		GramsPerDrag = double.Parse(root.Element("GramsPerDrag")?.Value ?? "0.0");
		OnDragProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnDragProg")?.Value ?? "0"));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tfuel <seconds> - how long this will burn for\n\tdrag <seconds> - how many seconds of burn time a drag removes\n\tdrug <which> - sets a drug to be delivered by drags\n\tdrug clear - clears a drug from the smokeable\n\tdose <weight> - the drug dose per drag\n\teffect <%> - a multiplier for how long the description effects will last\n\troomdesc <echo> - a line of text that will be added to the room when this is smoked\n\troomdesc clear - clears the room desc\n\tplayerdesc <echo> - a line of text that will be added to the players LOOK desc when smoked\n\tplayerdesc clear - clears the player desc\n\tondrag <which> - a prog that will be fired when someone smokes this\n\tondrag clear - clears the on drag prog";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "fuel":
				return BuildingCommandFuel(actor, command);
			case "drag":
				return BuildingCommandDrag(actor, command);
			case "ondrag":
				return BuildingCommandOnDrag(actor, command);
			case "effect":
				return BuildingCommandEffect(actor, command);
			case "grams":
			case "dose":
				return BuildingCommandDose(actor, command);
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "playerdesc":
				return BuildingCommandPlayerDesc(actor, command);
			case "roomdesc":
				return BuildingCommandRoomDesc(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis is a smokeable item with {4} seconds of fuel, consuming {5} seconds per drag and gives {9} seconds of effect per second of fuel. When smoked, it {6}.{10}\n\n{7}\n{8}",
			"Smokeable Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			SecondsOfFuel.ToString("N0", actor).Colour(Telnet.Green),
			SecondsPerDrag.ToString("N0", actor).Colour(Telnet.Green),
			OnDragProg == null
				? "does not execute any prog"
				: $"executes the {$"{OnDragProg.FunctionName} (#{OnDragProg.Id})".FluentTagMXP("send", $"href='show futureprog {OnDragProg.Id}'")}",
			string.IsNullOrWhiteSpace(PlayerDescriptionEffectString)
				? "It does not add any description effect to its smoker"
				: $"It adds the following description effect to its smoker: {PlayerDescriptionEffectString.Colour(Telnet.Yellow)}",
			string.IsNullOrWhiteSpace(RoomDescriptionEffectString)
				? "It does not add any description effect to the room"
				: $"It adds the following description effect to the room: {RoomDescriptionEffectString.Colour(Telnet.Yellow)}",
			SecondsOfEffectPerSecondOfFuel.ToString("N2", actor).Colour(Telnet.Green),
			Drug != null
				? $" Smoking it delivers {Gameworld.UnitManager.DescribeExact(GramsPerDrag / (1000 * Gameworld.UnitManager.BaseWeightToKilograms), UnitType.Mass, actor).Colour(Telnet.Green)} of the drug {Drug.Name.FluentTagMXP("send", $"href='show drug {Drug.Id}'")}"
				: ""
		);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SecondsOfFuel", SecondsOfFuel),
			new XElement("SecondsPerDrag", SecondsPerDrag),
			new XElement("SecondsOfEffectPerSecondOfFuel", SecondsOfEffectPerSecondOfFuel),
			new XElement("OnDragProg", OnDragProg?.Id ?? 0),
			new XElement("PlayerDescriptionEffectString", PlayerDescriptionEffectString ?? ""),
			new XElement("RoomDescriptionEffectString", RoomDescriptionEffectString ?? ""),
			new XElement("Drug", Drug?.Id ?? 0),
			new XElement("GramsPerDrag", GramsPerDrag)
		).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("smokeable", true,
			(gameworld, account) => new SmokeableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Smokeable",
			(proto, gameworld) => new SmokeableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Smokeable",
			$"Item can be {"[lit]".Colour(Telnet.Red)} and subsequently {"[smoked]".Colour(Telnet.Yellow)}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new SmokeableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SmokeableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SmokeableGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What drug do you want this smoke to deliver? Type {"clear".Colour(Telnet.Yellow)} to clear an existing drug.");
			return false;
		}

		if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			Drug = null;
			Changed = true;
			actor.Send("This smokeable no longer contains any drugs.");
			return true;
		}

		var drug = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Drugs.Get(value)
			: Gameworld.Drugs.GetByName(command.Last);
		if (drug == null)
		{
			actor.Send("There is no such drug.");
			return false;
		}

		if (!drug.DrugVectors.HasFlag(DrugVector.Inhaled))
		{
			actor.Send(
				$"You cannot use the drug {drug.Name.Colour(Telnet.Cyan)} because it does not have the 'inhaled' delivery vector.");
			return false;
		}

		Drug = drug;
		Changed = true;
		actor.Send($"This smokeable will now contain the drug {drug.Name.Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandDose(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much drug do you want this smokeable to deliver?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success) *
		            Gameworld.UnitManager.BaseWeightToKilograms * 1000;
		if (!success)
		{
			actor.Send("You must enter a valid weight of drug to deliver with each dose of this smokeable.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("The item must have a positive weight of drug.");
			return false;
		}

		GramsPerDrag = value;
		Changed = true;
		actor.Send(
			$"You set this smokeable item to have {Gameworld.UnitManager.DescribeExact(value, UnitType.Mass, actor).Colour(Telnet.Green)} of drug per drag.");
		return true;
	}

	private bool BuildingCommandFuel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds of fuel do you want this smokeable item to have?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send("You must enter a number of seconds of fuel for this smokeable item to have.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("The item must have a positive number of seconds of fuel.");
			return false;
		}

		SecondsOfFuel = value;
		Changed = true;
		actor.Send(
			$"You set this smokeable item to have {SecondsOfFuel.ToString("N0", actor).Colour(Telnet.Green)} seconds of fuel.");
		return true;
	}

	private bool BuildingCommandDrag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds of fuel do you want this smokeable item to use upon taking a drag?");
			return false;
		}

		if (!int.TryParse(command.Pop(), out var value))
		{
			actor.Send(
				"You must enter a number of seconds of fuel for this smokeable item to use upon taking a drag.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("The item must have a positive number of seconds of fuel used upon taking a drag.");
			return false;
		}

		SecondsPerDrag = value;
		Changed = true;
		actor.Send(
			$"You set this smokeable item to use {SecondsPerDrag.ToString("N0", actor).Colour(Telnet.Green)} seconds of fuel per drag.");
		return true;
	}

	private bool BuildingCommandEffect(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What multiplier should this item have in terms of the lingering effect length?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send(
				"You must enter a multiplier for this smokeable item to have in terms of the lingering effect length.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("The item must have a positive multiplier to have in terms of the lingering effect length.");
			return false;
		}

		SecondsOfEffectPerSecondOfFuel = value;
		Changed = true;
		actor.Send(
			$"You set this smokeable item to have an effect that lasts {SecondsOfEffectPerSecondOfFuel.ToString("N2", actor).Colour(Telnet.Green)} times longer than it is lit.");
		return true;
	}

	private bool BuildingCommandOnDrag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"You must either specify a prog to use for the OnDrag prog, or specify {"clear".Colour(Telnet.Yellow)} to clear it.");
			return false;
		}

		if (command.Pop().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			OnDragProg = null;
			Changed = true;
			actor.Send("You clear the OnDrag prog from this smokeable item.");
			return true;
		}

		var prog = long.TryParse(command.Last, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.Send("There is no such prog to use as an OnDrag prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.Send("The OnDrag prog must have a single character and item parameter.");
			return false;
		}

		OnDragProg = prog;
		Changed = true;
		actor.Send(
			$"This smokeable item will now execute the {OnDragProg.FunctionName} (#{OnDragProg.Id}) prog when a drag is taken.");
		return true;
	}

	private bool BuildingCommandPlayerDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"You must either specify a description to be tagged on to players when they hold this smoking item, or use {"clear".Colour(Telnet.Yellow)} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("You clear the player description tag for this smokeable.");
			PlayerDescriptionEffectString = "";
			Changed = true;
			return true;
		}

		PlayerDescriptionEffectString = command.SafeRemainingArgument.ProperSentences().Trim().SubstituteANSIColour();
		actor.Send("You change the player description tag for this smokeable to: {0}", PlayerDescriptionEffectString);
		Changed = true;
		return true;
	}

	private bool BuildingCommandRoomDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"You must either specify a description to be tagged on to rooms when smoking items are present, or use {"clear".Colour(Telnet.Yellow)} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("You clear the room description tag for this smokeable.");
			RoomDescriptionEffectString = "";
			Changed = true;
			return true;
		}

		RoomDescriptionEffectString = command.SafeRemainingArgument.ProperSentences().Trim().SubstituteANSIColour();
		actor.Send("You change the room description tag for this smokeable to: {0}", RoomDescriptionEffectString);
		Changed = true;
		return true;
	}

	#endregion
}
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class RcsThrusterGameItemComponentProto : GameItemComponentProto, IConnectableItemProto, IZeroGravityPropulsionPrototype, IConnectablePrototype
{
	protected RcsThrusterGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "RcsThruster")
	{
		Connector = new ConnectorType(MudSharp.Form.Shape.Gender.Male, Gameworld.GetStaticConfiguration("DefaultGasSocketType"), false);
		GasPerThrust = 1.0;
		Changed = true;
	}

	protected RcsThrusterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public override string TypeDescription => "RcsThruster";

	public double GasPerThrust { get; protected set; }

	public ConnectorType Connector { get; protected set; }

	public IEnumerable<ConnectorType> Connections => new[] { Connector };

	protected override void LoadFromXml(XElement root)
	{
		GasPerThrust = double.Parse(root.Element("GasPerThrust")?.Value ?? "1.0");
		var elem = root.Element("Connector");
		Connector = new ConnectorType(
			(MudSharp.Form.Shape.Gender)Convert.ToSByte(elem?.Attribute("gender")?.Value ?? ((short)MudSharp.Form.Shape.Gender.Male).ToString()),
			elem?.Attribute("type")?.Value ?? Gameworld.GetStaticConfiguration("DefaultGasSocketType"),
			bool.Parse(elem?.Attribute("powered")?.Value ?? "false")
		);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("GasPerThrust", GasPerThrust),
			new XElement("Connector",
				new XAttribute("gender", (sbyte)Connector.Gender),
				new XAttribute("type", Connector.ConnectionType),
				new XAttribute("powered", Connector.Powered))
		).ToString();
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tthrust <amount> - sets gas consumed per thrust\n\tconnectortype <type>\n\tconnectorgender <male|female|neuter>";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "thrust":
			case "gas":
				return BuildingCommandGas(actor, command);
			case "connectortype":
				return BuildingCommandConnectorType(actor, command);
			case "connectorgender":
				return BuildingCommandConnectorGender(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandGas(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much gas should be consumed per thrust? The units are litres at 1 atmosphere.");
			return false;
		}

		var amount = Gameworld.UnitManager.GetBaseUnits(command.PopSpeech(), UnitType.FluidVolume, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid volume.");
			return false;
		}

		GasPerThrust = amount;
		Changed = true;
		actor.OutputHandler.Send($"Each thrust will now consume {Gameworld.UnitManager.DescribeMostSignificantExact(GasPerThrust, UnitType.FluidVolume, actor).ColourValue()} of gas.");
		return true;
	}

	private bool BuildingCommandConnectorType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the connector type?");
			return false;
		}

		Connector = new ConnectorType(Connector.Gender, command.SafeRemainingArgument, Connector.Powered);
		Changed = true;
		actor.OutputHandler.Send($"This thruster now uses connector type {Connector.ConnectionType.ColourName()}.");
		return true;
	}

	private bool BuildingCommandConnectorGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which gender should this connector be?");
			return false;
		}

		var gender = Gendering.Get(command.PopSpeech());
		if (gender.Enum == MudSharp.Form.Shape.Gender.Indeterminate)
		{
			actor.OutputHandler.Send("That is not a valid connector gender.");
			return false;
		}

		Connector = new ConnectorType(gender.Enum, Connector.ConnectionType, Connector.Powered);
		Changed = true;
		actor.OutputHandler.Send($"This thruster now has a {gender.Enum.ToString().ToLowerInvariant()} connector.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nEach thrust consumes {4}. Connector: {5}.",
			"RCS Thruster Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.DescribeMostSignificantExact(GasPerThrust, UnitType.FluidVolume, actor).ColourValue(),
			Connector.ConnectionType.ColourValue());
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new RcsThrusterGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RcsThrusterGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new RcsThrusterGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("RcsThruster".ToLowerInvariant(), true, (gameworld, account) => new RcsThrusterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("RcsThruster", (proto, gameworld) => new RcsThrusterGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("RcsThruster", "A wearable connectable zero-gravity propulsion component that consumes gas.", BuildingHelpText);
	}
}

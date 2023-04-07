using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class CashRegisterGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "CashRegister";

	#region Constructors

	protected CashRegisterGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "CashRegister")
	{
	}

	protected CashRegisterGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("Weight");
		if (attr != null)
		{
			WeightLimit = double.Parse(attr.Value);
		}

		attr = root.Attribute("MaxSize");
		if (attr != null)
		{
			MaximumContentsSize = (SizeCategory)int.Parse(attr.Value);
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XAttribute("Weight", WeightLimit),
			new XAttribute("MaxSize", (int)MaximumContentsSize)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CashRegisterGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new CashRegisterGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("CashRegister".ToLowerInvariant(), true,
			(gameworld, account) => new CashRegisterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("CashRegister",
			(proto, gameworld) => new CashRegisterGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"CashRegister",
			$"A type of {"[container]".Colour(Telnet.BoldGreen)} that interacts with the game's shop mechanics when used as a till",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new CashRegisterGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tweight <amount> - how much weight this cash register can hold\n\tsize <size> - the maximum size of items that can be stored in this cash register";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "weight":
			case "limit":
				return BuildingCommand_WeightLimit(actor, command);
			case "size":
			case "maxsize":
			case "max size":
			case "max_size":
				return BuildingCommand_MaxSize(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_WeightLimit(ICharacter actor, StringStack command)
	{
		var weightCmd = command.SafeRemainingArgument;
		var result = actor.Gameworld.UnitManager.GetBaseUnits(weightCmd, UnitType.Mass, out var success);
		if (success)
		{
			WeightLimit = result;
			actor.OutputHandler.Send(
				$"This cash register will now hold {actor.Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).ColourValue()}.");
			return true;
		}

		actor.OutputHandler.Send("That is not a valid weight.");
		return false;
	}

	private bool BuildingCommand_MaxSize(ICharacter actor, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("What size do you want to set the limit for this cash register to?");
			return false;
		}

		var size = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().ToList();
		SizeCategory target;
		if (size.Any(x => x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal)))
		{
			target = size.FirstOrDefault(x =>
				x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
		}
		else
		{
			actor.OutputHandler.Send("That is not a valid item size. See SHOW ITEMSIZES for a correct list.");
			return false;
		}

		MaximumContentsSize = target;
		Changed = true;
		actor.OutputHandler.Send("This cash register will now only take items of up to size \"" + target.Describe() +
		                         "\".");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is a cash register for a store. It can hold {4} of items up to {5} size.",
			"CashRegister Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).ColourValue(),
			MaximumContentsSize.ToString().Colour(Telnet.Cyan)
		);
	}

	/// <summary>
	///     The total allowable weight that can be contained by this container
	/// </summary>
	public double WeightLimit { get; protected set; }

	/// <summary>
	///     The maximum SizeCategory of item that may be contained by this container
	/// </summary>
	public SizeCategory MaximumContentsSize { get; protected set; }

	public override bool WarnBeforePurge => true;
}
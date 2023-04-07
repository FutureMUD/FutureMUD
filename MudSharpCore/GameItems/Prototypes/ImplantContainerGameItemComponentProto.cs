using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ImplantContainerGameItemComponentProto : ImplantBaseGameItemComponentProto
{
	public override string TypeDescription => "ImplantContainer";

	public override bool WarnBeforePurge => true;

	#region Constructors

	protected ImplantContainerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantContainer")
	{
		ContentsPreposition = "in";
		MaximumContentsSize = SizeCategory.Small;
		WeightLimit = 1.0 / gameworld.UnitManager.BaseWeightToKilograms;
		PowerConsumptionInWatts = 0.0;
		PowerConsumptionDiscountPerQuality = 0.0;
	}

	protected ImplantContainerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
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

		attr = root.Attribute("Preposition");
		if (attr != null)
		{
			ContentsPreposition = attr.Value;
		}

		attr = root.Attribute("Closable");
		if (attr != null)
		{
			Closable = bool.Parse(attr.Value);
		}

		attr = root.Attribute("Transparent");
		if (attr != null)
		{
			Transparent = bool.Parse(attr.Value);
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return SaveToXmlWithoutConvertingToString(new XAttribute("Weight", WeightLimit),
			new XAttribute("MaxSize", (int)MaximumContentsSize),
			new XAttribute("Preposition", ContentsPreposition), new XAttribute("Closable", Closable),
			new XAttribute("Transparent", Transparent)).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantContainerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantContainerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantContainer".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantContainerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantContainer",
			(proto, gameworld) => new ImplantContainerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantContainer",
			$"An {"[implant]".Colour(Telnet.Pink)} that is also a {"[container]".Colour(Telnet.BoldGreen)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantContainerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "close":
			case "openable":
			case "closable":
				return BuildingCommand_Closable(actor, command);
			case "capacity":
			case "weight":
			case "weight limit":
			case "weight capacity":
			case "limit":
				return BuildingCommand_WeightLimit(actor, command);
			case "maximum size":
			case "max size":
			case "maxsize":
			case "size":
				return BuildingCommand_MaxSize(actor, command);
			case "preposition":
				return BuildingCommand_Preposition(actor, command);
			case "transparent":
				return BuildingCommand_Transparent(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
	{
		Transparent = !Transparent;
		actor.Send("This container is {0} transparent.", Transparent ? "now" : "no longer");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Closable(ICharacter actor, StringStack command)
	{
		Closable = !Closable;
		actor.OutputHandler.Send("This container is " + (Closable ? "now" : "no longer") + " closable.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_WeightLimit(ICharacter actor, StringStack command)
	{
		var weightCmd = command.SafeRemainingArgument;
		var result = actor.Gameworld.UnitManager.GetBaseUnits(weightCmd, UnitType.Mass, out var success);
		if (success)
		{
			WeightLimit = result;
			actor.OutputHandler.Send(
				$"This container will now hold {actor.Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).ColourValue()}.");
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
			actor.OutputHandler.Send("What size do you want to set the limit for this component to?");
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
		actor.OutputHandler.Send("This container will now only take items of up to size \"" + target.Describe() +
		                         "\".");
		return true;
	}

	private bool BuildingCommand_Preposition(ICharacter actor, StringStack command)
	{
		var preposition = command.Pop().ToLowerInvariant();
		if (string.IsNullOrEmpty(preposition))
		{
			actor.OutputHandler.Send("What preposition do you want to use for this container?");
			return false;
		}

		ContentsPreposition = preposition;
		Changed = true;
		actor.OutputHandler.Send("The contents of this container will now be described as \"" + ContentsPreposition +
		                         "\" it.");
		return true;
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets the body prototype this implant is used with\n\tbodypart <bodypart> - sets the bodypart prototype this implant is used with\n\texternal - toggles whether this implant is external\n\texternaldesc <desc> - an alternate sdesc used when installed and external\n\tpower <watts> - how many watts of power to use\n\tdiscount <watts> - how many watts of power usage to discount per point of quality\n\tgrace <percentage> - the grace percentage of hp damage before implant function reduces\n\tclose - toggles whether this container opens and closes\n\tsize <max size> - sets the maximum size of the objects that can be put in this container\n\tweight - sets the maximum weight of items this container can hold\n\ttransparent - toggles whether you can see the contents when closed\n\tonce - toggles whether this container only opens once\n\tpreposition <on|in|etc> - sets the preposition used to display contents. Usually on or in.\n\tspace <#> - the amount of 'space' in a bodypart that the implant takes up\n\tdifficulty <difficulty> - how difficulty it is for surgeons to install this implant";

	public override string ShowBuildingHelp => BuildingHelpText;

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return ComponentDescriptionOLC(actor, "This item is an implantable container",
			$" It can contain {Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor)} and up to {MaximumContentsSize.ToString().Colour(Telnet.Cyan)} size objects. It {(Transparent ? "is" : "is not")} transparent and {(Closable ? "can be opened and closed" : "cannot be opened and closed")}");
	}

	/// <summary>
	///     The total allowable weight that can be contained by this container
	/// </summary>
	public double WeightLimit { get; protected set; }

	/// <summary>
	///     The maximum SizeCategory of item that may be contained by this container
	/// </summary>
	public SizeCategory MaximumContentsSize { get; protected set; }

	/// <summary>
	///     Usually either "in" or "on"
	/// </summary>
	public string ContentsPreposition { get; protected set; }

	/// <summary>
	///     Whether or not this container can be opened and closed
	/// </summary>
	public bool Closable { get; protected set; }

	public bool Transparent { get; protected set; }
}
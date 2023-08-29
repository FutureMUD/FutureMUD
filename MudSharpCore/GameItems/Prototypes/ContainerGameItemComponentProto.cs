using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ContainerGameItemComponentProto : GameItemComponentProto
{
	/// <summary>
	///     The total allowable weight that can be contained by this container
	/// </summary>
	public double WeightLimit { get; protected set; } = 1000;

	/// <summary>
	///     The maximum SizeCategory of item that may be contained by this container
	/// </summary>
	public SizeCategory MaximumContentsSize { get; protected set; } = SizeCategory.Small;

	/// <summary>
	///     Usually either "in" or "on"
	/// </summary>
	public string ContentsPreposition { get; protected set; } = "in";

	/// <summary>
	///     Whether or not this container can be opened and closed
	/// </summary>
	public bool Closable { get; protected set; } = true;

	public bool Transparent { get; protected set; } = false;

	/// <summary>
	///     A container that is OnceOnly can only be opened once - once opened, it can never be closed again
	/// </summary>
	public bool OnceOnly { get; protected set; } = false;

	public override bool WarnBeforePurge => true;

	public override string TypeDescription => "Container";

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

		attr = root.Attribute("OnceOnly");
		if (attr != null)
		{
			OnceOnly = bool.Parse(attr.Value);
		}
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("container", true,
			(gameworld, account) => new ContainerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Container",
			(proto, gameworld) => new ContainerGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Container",
			$"Makes an item into a {"[container]".Colour(Telnet.BoldGreen)}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ContainerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ContainerGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ContainerGameItemComponentProto(proto, gameworld));
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("Weight", WeightLimit),
				new XAttribute("MaxSize", (int)MaximumContentsSize),
				new XAttribute("Preposition", ContentsPreposition), new XAttribute("Closable", Closable),
				new XAttribute("Transparent", Transparent), new XAttribute("OnceOnly", OnceOnly)).ToString();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"Container Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

This item can contain {Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor)} and up to {MaximumContentsSize.ToString().Colour(Telnet.Cyan)} size objects. 
It {(Transparent ? "is" : "is not")} transparent and {(Closable ? OnceOnly ? "can only be opened a single time" : "can be opened and closed" : "cannot be opened and closed")}";
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
		actor.OutputHandler.Send("This container is " + (Closable ? "now" : "no longer") + " able to be closed.");
		Changed = true;
		return true;
	}

	public bool BuildingCommand_OnceOnly(ICharacter actor, StringStack command)
	{
		OnceOnly = !OnceOnly;
		actor.OutputHandler.Send("This container is " + (OnceOnly ? "now" : "no longer") +
								 " openable only a single time.");
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
		@"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3close#0 - toggles whether this container opens and closes
	#3size <max size>#0 - sets the maximum size of the objects that can be put in this container
	#3weight#0 - sets the maximum weight of items this container can hold
	#3transparent#0 - toggles whether you can see the contents when closed
	#3once#0 - toggles whether this container only opens once
	#3preposition <on|in|etc>#0 - sets the preposition used to display contents. Usually on or in.";

	public override string ShowBuildingHelp => BuildingHelpText;

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
			case "once":
			case "onceonly":
				return BuildingCommand_OnceOnly(actor, command);
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
				return base.BuildingCommand(actor, command);
		}
	}

	#region Constructors

	protected ContainerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected ContainerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Container")
	{
	}

	#endregion
}
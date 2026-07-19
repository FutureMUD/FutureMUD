using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class OfferingReceiverGameItemComponentProto : GameItemComponentProto, IOfferingReceiverPrototype
{
	protected OfferingReceiverGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "OfferingReceiver")
	{
		MaximumContentsWeight = 100000.0;
		MaximumItemSize = SizeCategory.Huge;
		ConsumptionMode = OfferingConsumptionMode.ManualBurn;
		AcceptEcho = "@ offer|offers $1 at $2.";
		BurnEcho = "@ burn|burns $1 at $2.";
		RejectEcho = "$2 rejects $1.";
		Changed = true;
	}

	protected OfferingReceiverGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "OfferingReceiver";
	public List<ITag> AllowedTags { get; } = [];
	public List<ITag> BlockedTags { get; } = [];
	public double MaximumContentsWeight { get; protected set; }
	public SizeCategory MaximumItemSize { get; protected set; }
	public OfferingConsumptionMode ConsumptionMode { get; protected set; }
	public IGameItemProto? ResidueItemProto { get; protected set; }
	public IFutureProg? CanOfferProg { get; protected set; }
	public IFutureProg? OnOfferProg { get; protected set; }
	public IFutureProg? OnBurnProg { get; protected set; }
	public string AcceptEcho { get; protected set; } = string.Empty;
	public string BurnEcho { get; protected set; } = string.Empty;
	public string RejectEcho { get; protected set; } = string.Empty;

	protected override void LoadFromXml(XElement root)
	{
		AllowedTags.Clear();
		AllowedTags.AddRange(root.Element("AllowedTags")?.Elements("Tag")
		                         .Select(x => Gameworld.Tags.Get(long.Parse(x.Value)))
		                         .Where(x => x is not null)
		                         .Select(x => x!)
		                         ?? Enumerable.Empty<ITag>());
		BlockedTags.Clear();
		BlockedTags.AddRange(root.Element("BlockedTags")?.Elements("Tag")
		                         .Select(x => Gameworld.Tags.Get(long.Parse(x.Value)))
		                         .Where(x => x is not null)
		                         .Select(x => x!)
		                         ?? Enumerable.Empty<ITag>());
		MaximumContentsWeight = double.Parse(root.Element("MaximumContentsWeight")?.Value ?? "100000.0");
		MaximumItemSize = (SizeCategory)int.Parse(root.Element("MaximumItemSize")?.Value ?? ((int)SizeCategory.Huge).ToString());
		ConsumptionMode = root.Element("ConsumptionMode")?.Value.TryParseEnum<OfferingConsumptionMode>(out var mode) == true
			? mode
			: OfferingConsumptionMode.ManualBurn;
		ResidueItemProto = Gameworld.ItemProtos.Get(long.Parse(root.Element("ResidueItemProto")?.Value ?? "0"));
		CanOfferProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanOfferProg")?.Value ?? "0"));
		OnOfferProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnOfferProg")?.Value ?? "0"));
		OnBurnProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnBurnProg")?.Value ?? "0"));
		AcceptEcho = root.Element("AcceptEcho")?.Value ?? "@ offer|offers $1 at $2.";
		BurnEcho = root.Element("BurnEcho")?.Value ?? "@ burn|burns $1 at $2.";
		RejectEcho = root.Element("RejectEcho")?.Value ?? "$2 rejects $1.";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("AllowedTags", AllowedTags.Select(x => new XElement("Tag", x.Id))),
			new XElement("BlockedTags", BlockedTags.Select(x => new XElement("Tag", x.Id))),
			new XElement("MaximumContentsWeight", MaximumContentsWeight),
			new XElement("MaximumItemSize", (int)MaximumItemSize),
			new XElement("ConsumptionMode", ConsumptionMode.ToString()),
			new XElement("ResidueItemProto", ResidueItemProto?.Id ?? 0),
			new XElement("CanOfferProg", CanOfferProg?.Id ?? 0),
			new XElement("OnOfferProg", OnOfferProg?.Id ?? 0),
			new XElement("OnBurnProg", OnBurnProg?.Id ?? 0),
			new XElement("AcceptEcho", new XCData(AcceptEcho)),
			new XElement("BurnEcho", new XCData(BurnEcho)),
			new XElement("RejectEcho", new XCData(RejectEcho))).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new OfferingReceiverGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new OfferingReceiverGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new OfferingReceiverGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("offeringreceiver", true,
			(gameworld, account) => new OfferingReceiverGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("offering receiver", false,
			(gameworld, account) => new OfferingReceiverGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("OfferingReceiver",
			(proto, gameworld) => new OfferingReceiverGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"OfferingReceiver",
			$"A ritual focus that receives and optionally burns item offerings",
			BuildingHelpText);
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\t#3name <name>#0 - sets the name of the component\n\t#3desc <desc>#0 - sets the description of the component\n\t#3allow <tag>|clear#0 - toggles an allowed offering tag or clears all\n\t#3block <tag>|clear#0 - toggles a blocked offering tag or clears all\n\t#3capacity <weight>#0 - sets maximum contained offering weight\n\t#3size <size>#0 - sets maximum item size\n\t#3mode manual|onoffer|record#0 - sets consumption mode\n\t#3residue <proto>|clear#0 - sets optional residue prototype\n\t#3canprog <prog>|clear#0 - sets boolean offer gate prog\n\t#3offerprog <prog>|clear#0 - sets void offer prog\n\t#3burnprog <prog>|clear#0 - sets void burn prog\n\t#3acceptecho <emote>#0 - sets accepted offering emote\n\t#3burnecho <emote>#0 - sets burning emote\n\t#3rejectecho <emote>#0 - sets rejected offering emote";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "allow":
			case "allowed":
				return BuildingCommandTag(actor, command, AllowedTags, "allowed");
			case "block":
			case "blocked":
				return BuildingCommandTag(actor, command, BlockedTags, "blocked");
			case "capacity":
				return BuildingCommandCapacity(actor, command);
			case "size":
				return BuildingCommandSize(actor, command);
			case "mode":
				return BuildingCommandMode(actor, command);
			case "residue":
				return BuildingCommandResidue(actor, command);
			case "canprog":
				return BuildingCommandProg(actor, command, "CanOffer", ProgVariableTypes.Boolean,
					prog => CanOfferProg = prog);
			case "offerprog":
			case "onoffer":
				return BuildingCommandProg(actor, command, "OnOffer", ProgVariableTypes.Void,
					prog => OnOfferProg = prog);
			case "burnprog":
			case "onburn":
				return BuildingCommandProg(actor, command, "OnBurn", ProgVariableTypes.Void,
					prog => OnBurnProg = prog);
			case "acceptecho":
				return BuildingCommandEcho(actor, command, "accept", value => AcceptEcho = value);
			case "burnecho":
				return BuildingCommandEcho(actor, command, "burn", value => BurnEcho = value);
			case "rejectecho":
				return BuildingCommandEcho(actor, command, "reject", value => RejectEcho = value);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Offering Receiver Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nMode: {ConsumptionMode.DescribeEnum().ColourName()}\nCapacity: {MaximumContentsWeight.ToString("N2", actor).ColourValue()} weight units\nMaximum Size: {MaximumItemSize.Describe().ColourName()}\nAllowed Tags: {(AllowedTags.Any() ? AllowedTags.Select(x => x.Name.ColourName()).ListToString() : "Any".ColourValue())}\nBlocked Tags: {(BlockedTags.Any() ? BlockedTags.Select(x => x.Name.ColourName()).ListToString() : "None".ColourValue())}\nResidue: {ResidueItemProto?.EditHeader().ColourName() ?? "None".ColourError()}\nCanOffer Prog: {CanOfferProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nOnOffer Prog: {OnOfferProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nOnBurn Prog: {OnBurnProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nAccept Echo: {AcceptEcho.ColourCommand()}\nBurn Echo: {BurnEcho.ColourCommand()}\nReject Echo: {RejectEcho.ColourCommand()}";
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command, List<ITag> tags, string name)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which tag should be toggled in the {name} list? Use {"clear".ColourCommand()} to clear all.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			tags.Clear();
			Changed = true;
			actor.OutputHandler.Send($"The {name} tag list is now clear.");
			return true;
		}

		var tag = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Tags.Get(value)
			: Gameworld.Tags.GetByName(command.SafeRemainingArgument);
		if (tag is null)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (tags.Contains(tag))
		{
			tags.Remove(tag);
			actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} is no longer {name}.");
		}
		else
		{
			tags.Add(tag);
			actor.OutputHandler.Send($"The tag {tag.Name.ColourName()} is now {name}.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What maximum offering weight should this focus hold?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success);
		if (!success || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive weight.");
			return false;
		}

		MaximumContentsWeight = value;
		Changed = true;
		actor.OutputHandler.Send($"This focus now holds {Gameworld.UnitManager.DescribeExact(MaximumContentsWeight, UnitType.Mass, actor).ColourValue()} of offerings.");
		return true;
	}

	private bool BuildingCommandSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<SizeCategory>(out var value))
		{
			actor.OutputHandler.Send("You must enter a valid item size.");
			return false;
		}

		MaximumItemSize = value;
		Changed = true;
		actor.OutputHandler.Send($"This focus now accepts items up to {MaximumItemSize.Describe().ColourName()} size.");
		return true;
	}

	private bool BuildingCommandMode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which mode? Valid values are manual, onoffer and record.");
			return false;
		}

		ConsumptionMode = command.PopSpeech().ToLowerInvariant() switch
		{
			"manual" => OfferingConsumptionMode.ManualBurn,
			"onoffer" or "on-offer" or "burnonoffer" or "burn-on-offer" => OfferingConsumptionMode.BurnOnOffer,
			"record" or "recordonly" or "record-only" => OfferingConsumptionMode.RecordOnly,
			_ => ConsumptionMode
		};

		Changed = true;
		actor.OutputHandler.Send($"This focus now uses {ConsumptionMode.DescribeEnum().ColourName()} mode.");
		return true;
	}

	private bool BuildingCommandResidue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which item prototype should be left as residue? Use {"clear".ColourCommand()} to delete offerings instead.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			ResidueItemProto = null;
			Changed = true;
			actor.OutputHandler.Send("Burned offerings will now leave no residue.");
			return true;
		}

		var proto = Gameworld.ItemProtos.GetByIdOrUniqueNameOrName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		ResidueItemProto = proto;
		Changed = true;
		actor.OutputHandler.Send($"Burned offerings will now leave {proto.EditHeader().ColourName()}.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command, string progName,
		ProgVariableTypes returnType, Action<IFutureProg?> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which prog should be used for {progName}? Use {"clear".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			setter(null);
			Changed = true;
			actor.OutputHandler.Send($"The {progName} prog is now clear.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, returnType,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"The {progName} prog is now {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, string name, Action<string> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What emote should be shown for the {name} action?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		var emote = new Emote(text, actor, actor);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		setter(text);
		Changed = true;
		actor.OutputHandler.Send($"The {name} echo is now {text.ColourCommand()}.");
		return true;
	}
}

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
		LiquidAcceptEcho = "@ pour|pours {0} from $1 onto $2 as a libation.";
		LiquidRejectEcho = "$2 rejects the attempted libation from $1.";
		MinimumLiquidOfferingVolume = 0.0;
		MaximumLiquidOfferingVolume = 0.0;
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
	public bool AcceptsLiquidOfferings { get; protected set; }
	public List<ITag> AllowedLiquidTags { get; } = [];
	public List<ITag> BlockedLiquidTags { get; } = [];
	public double MinimumLiquidOfferingVolume { get; protected set; }
	public double MaximumLiquidOfferingVolume { get; protected set; }
	public IFutureProg? CanOfferLiquidProg { get; protected set; }
	public IFutureProg? WhyCannotOfferLiquidProg { get; protected set; }
	public IFutureProg? OnOfferLiquidProg { get; protected set; }
	public IFutureProg? OracleResponseProg { get; protected set; }
	public string LiquidAcceptEcho { get; protected set; } = string.Empty;
	public string LiquidRejectEcho { get; protected set; } = string.Empty;

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
		AllowedLiquidTags.Clear();
		AllowedLiquidTags.AddRange(root.Element("AllowedLiquidTags")?.Elements("Tag")
		                               .Select(x => Gameworld.Tags.Get(long.Parse(x.Value)))
		                               .Where(x => x is not null)
		                               .Select(x => x!)
		                               ?? Enumerable.Empty<ITag>());
		BlockedLiquidTags.Clear();
		BlockedLiquidTags.AddRange(root.Element("BlockedLiquidTags")?.Elements("Tag")
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
		AcceptsLiquidOfferings = bool.TryParse(root.Element("AcceptsLiquidOfferings")?.Value, out var acceptsLiquids) &&
		                         acceptsLiquids;
		MinimumLiquidOfferingVolume =
			double.TryParse(root.Element("MinimumLiquidOfferingVolume")?.Value,
				System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture,
				out var minimumVolume)
				? minimumVolume
				: 0.0;
		MaximumLiquidOfferingVolume =
			double.TryParse(root.Element("MaximumLiquidOfferingVolume")?.Value,
				System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture,
				out var maximumVolume)
				? maximumVolume
				: 0.0;
		CanOfferLiquidProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanOfferLiquidProg")?.Value ?? "0"));
		WhyCannotOfferLiquidProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotOfferLiquidProg")?.Value ?? "0"));
		OnOfferLiquidProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnOfferLiquidProg")?.Value ?? "0"));
		OracleResponseProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OracleResponseProg")?.Value ?? "0"));
		LiquidAcceptEcho = root.Element("LiquidAcceptEcho")?.Value ??
		                   "@ pour|pours {0} from $1 onto $2 as a libation.";
		LiquidRejectEcho = root.Element("LiquidRejectEcho")?.Value ??
		                   "$2 rejects the attempted libation from $1.";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("AllowedTags", AllowedTags.Select(x => new XElement("Tag", x.Id))),
			new XElement("BlockedTags", BlockedTags.Select(x => new XElement("Tag", x.Id))),
			new XElement("AllowedLiquidTags", AllowedLiquidTags.Select(x => new XElement("Tag", x.Id))),
			new XElement("BlockedLiquidTags", BlockedLiquidTags.Select(x => new XElement("Tag", x.Id))),
			new XElement("MaximumContentsWeight", MaximumContentsWeight),
			new XElement("MaximumItemSize", (int)MaximumItemSize),
			new XElement("ConsumptionMode", ConsumptionMode.ToString()),
			new XElement("ResidueItemProto", ResidueItemProto?.Id ?? 0),
			new XElement("CanOfferProg", CanOfferProg?.Id ?? 0),
			new XElement("OnOfferProg", OnOfferProg?.Id ?? 0),
			new XElement("OnBurnProg", OnBurnProg?.Id ?? 0),
			new XElement("AcceptEcho", new XCData(AcceptEcho)),
			new XElement("BurnEcho", new XCData(BurnEcho)),
			new XElement("RejectEcho", new XCData(RejectEcho)),
			new XElement("AcceptsLiquidOfferings", AcceptsLiquidOfferings),
			new XElement("MinimumLiquidOfferingVolume", MinimumLiquidOfferingVolume),
			new XElement("MaximumLiquidOfferingVolume", MaximumLiquidOfferingVolume),
			new XElement("CanOfferLiquidProg", CanOfferLiquidProg?.Id ?? 0),
			new XElement("WhyCannotOfferLiquidProg", WhyCannotOfferLiquidProg?.Id ?? 0),
			new XElement("OnOfferLiquidProg", OnOfferLiquidProg?.Id ?? 0),
			new XElement("OracleResponseProg", OracleResponseProg?.Id ?? 0),
			new XElement("LiquidAcceptEcho", new XCData(LiquidAcceptEcho)),
			new XElement("LiquidRejectEcho", new XCData(LiquidRejectEcho))).ToString();
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
			$"A ritual focus that receives item offerings and optional consumptive liquid libations",
			BuildingHelpText);
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\t#3name <name>#0 - sets the name of the component\n\t#3desc <desc>#0 - sets the description of the component\n\t#3allow <tag>|clear#0 - toggles an allowed item-offering tag or clears all\n\t#3block <tag>|clear#0 - toggles a blocked item-offering tag or clears all\n\t#3capacity <weight>#0 - sets maximum contained offering weight\n\t#3size <size>#0 - sets maximum item size\n\t#3mode manual|onoffer|record#0 - sets item consumption mode\n\t#3residue <proto>|clear#0 - sets optional residue prototype\n\t#3canprog <prog>|clear#0 - sets boolean item-offering gate prog\n\t#3offerprog <prog>|clear#0 - sets void item-offering prog\n\t#3burnprog <prog>|clear#0 - sets void burn prog\n\t#3acceptecho <emote>#0 - sets accepted item-offering emote\n\t#3burnecho <emote>#0 - sets burning emote\n\t#3rejectecho <emote>#0 - sets rejected item-offering emote\n\t#3liquids on|off#0 - toggles consumptive liquid libations\n\t#3liquidallow <tag>|clear#0 - toggles an allowed liquid tag or clears all\n\t#3liquidblock <tag>|clear#0 - toggles a blocked liquid tag or clears all\n\t#3liquidminimum <volume>#0 - sets the minimum liquid offering (zero disables)\n\t#3liquidmaximum <volume>#0 - sets the maximum liquid offering (zero is unlimited)\n\t#3liquidcanprog <prog>|clear#0 - sets the boolean liquid-offering gate prog\n\t#3liquidwhyprog <prog>|clear#0 - sets its text rejection prog\n\t#3liquidofferprog <prog>|clear#0 - sets its successful void prog\n\t#3oracleprog <prog>|clear#0 - sets optional text returned privately to the offerer\n\t#3liquidecho <emote>#0 - sets the liquid acceptance emote; {0} is the liquid description\n\t#3liquidrejectecho <emote>#0 - sets the liquid rejection emote";

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
			case "liquids":
			case "liquid":
				return BuildingCommandLiquids(actor, command);
			case "liquidallow":
				return BuildingCommandTag(actor, command, AllowedLiquidTags, "allowed liquid");
			case "liquidblock":
				return BuildingCommandTag(actor, command, BlockedLiquidTags, "blocked liquid");
			case "liquidminimum":
			case "liquidmin":
				return BuildingCommandLiquidVolume(actor, command, true);
			case "liquidmaximum":
			case "liquidmax":
				return BuildingCommandLiquidVolume(actor, command, false);
			case "liquidcanprog":
				return BuildingCommandLiquidProg(actor, command, "CanOfferLiquid", ProgVariableTypes.Boolean,
					prog => CanOfferLiquidProg = prog);
			case "liquidwhyprog":
				return BuildingCommandLiquidProg(actor, command, "WhyCannotOfferLiquid", ProgVariableTypes.Text,
					prog => WhyCannotOfferLiquidProg = prog);
			case "liquidofferprog":
			case "onliquidoffer":
				return BuildingCommandLiquidProg(actor, command, "OnOfferLiquid", ProgVariableTypes.Void,
					prog => OnOfferLiquidProg = prog);
			case "oracleprog":
				return BuildingCommandLiquidProg(actor, command, "OracleResponse", ProgVariableTypes.Text,
					prog => OracleResponseProg = prog);
			case "liquidecho":
				return BuildingCommandEcho(actor, command, "liquid accept", value => LiquidAcceptEcho = value, true);
			case "liquidrejectecho":
				return BuildingCommandEcho(actor, command, "liquid reject", value => LiquidRejectEcho = value);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Offering Receiver Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nMode: {ConsumptionMode.DescribeEnum().ColourName()}\nCapacity: {MaximumContentsWeight.ToString("N2", actor).ColourValue()} weight units\nMaximum Size: {MaximumItemSize.Describe().ColourName()}\nAllowed Tags: {(AllowedTags.Any() ? AllowedTags.Select(x => x.Name.ColourName()).ListToString() : "Any".ColourValue())}\nBlocked Tags: {(BlockedTags.Any() ? BlockedTags.Select(x => x.Name.ColourName()).ListToString() : "None".ColourValue())}\nResidue: {ResidueItemProto?.EditHeader().ColourName() ?? "None".ColourError()}\nCanOffer Prog: {CanOfferProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nOnOffer Prog: {OnOfferProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nOnBurn Prog: {OnBurnProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nAccept Echo: {AcceptEcho.ColourCommand()}\nBurn Echo: {BurnEcho.ColourCommand()}\nReject Echo: {RejectEcho.ColourCommand()}\n\nAccepts Liquid Libations: {AcceptsLiquidOfferings.ToColouredString()}\nLiquid Allowed Tags: {(AllowedLiquidTags.Any() ? AllowedLiquidTags.Select(x => x.Name.ColourName()).ListToString() : "Any".ColourValue())}\nLiquid Blocked Tags: {(BlockedLiquidTags.Any() ? BlockedLiquidTags.Select(x => x.Name.ColourName()).ListToString() : "None".ColourValue())}\nLiquid Volume Range: {Gameworld.UnitManager.DescribeExact(MinimumLiquidOfferingVolume, UnitType.FluidVolume, actor).ColourValue()} to {(MaximumLiquidOfferingVolume > 0.0 ? Gameworld.UnitManager.DescribeExact(MaximumLiquidOfferingVolume, UnitType.FluidVolume, actor).ColourValue() : "Unlimited".ColourValue())}\nCan Offer Liquid Prog: {CanOfferLiquidProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nWhy Cannot Offer Liquid Prog: {WhyCannotOfferLiquidProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nOn Offer Liquid Prog: {OnOfferLiquidProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nOracle Response Prog: {OracleResponseProg?.MXPClickableFunctionName() ?? "None".ColourError()}\nLiquid Accept Echo: {LiquidAcceptEcho.ColourCommand()}\nLiquid Reject Echo: {LiquidRejectEcho.ColourCommand()}";
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

		var mode = command.PopSpeech().ToLowerInvariant();
		var newMode = mode switch
		{
			"manual" => OfferingConsumptionMode.ManualBurn,
			"onoffer" or "on-offer" or "burnonoffer" or "burn-on-offer" => OfferingConsumptionMode.BurnOnOffer,
			"record" or "recordonly" or "record-only" => OfferingConsumptionMode.RecordOnly,
			_ => (OfferingConsumptionMode?)null
		};
		if (newMode is null)
		{
			actor.OutputHandler.Send("Valid modes are manual, onoffer and record.");
			return false;
		}

		ConsumptionMode = newMode.Value;
		Changed = true;
		actor.OutputHandler.Send($"This focus now uses {ConsumptionMode.DescribeEnum().ColourName()} mode.");
		return true;
	}

	private bool BuildingCommandLiquids(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Should this focus accept liquid libations? Use {"on".ColourCommand()} or {"off".ColourCommand()}.");
			return false;
		}

		var value = command.PopSpeech().ToLowerInvariant() switch
		{
			"on" or "yes" or "true" => true,
			"off" or "no" or "false" => false,
			_ => (bool?)null
		};
		if (value is null)
		{
			actor.OutputHandler.Send($"Use either {"on".ColourCommand()} or {"off".ColourCommand()}.");
			return false;
		}

		AcceptsLiquidOfferings = value.Value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This focus will {AcceptsLiquidOfferings.NowNoLonger()} accept consumptive liquid libations.");
		return true;
	}

	private bool BuildingCommandLiquidVolume(ICharacter actor, StringStack command, bool minimum)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What {(minimum ? "minimum" : "maximum")} liquid-offering volume should this focus accept? Zero {(minimum ? "disables the minimum" : "makes the maximum unlimited")}.");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume,
			out var success);
		if (!success || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid non-negative fluid volume.");
			return false;
		}

		if (minimum && MaximumLiquidOfferingVolume > 0.0 && value > MaximumLiquidOfferingVolume)
		{
			actor.OutputHandler.Send("The minimum cannot exceed the configured maximum.");
			return false;
		}

		if (!minimum && value > 0.0 && value < MinimumLiquidOfferingVolume)
		{
			actor.OutputHandler.Send("The maximum cannot be less than the configured minimum.");
			return false;
		}

		if (minimum)
		{
			MinimumLiquidOfferingVolume = value;
		}
		else
		{
			MaximumLiquidOfferingVolume = value;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"The {(minimum ? "minimum" : "maximum")} liquid offering is now {(value > 0.0 ? Gameworld.UnitManager.DescribeExact(value, UnitType.FluidVolume, actor).ColourValue() : (minimum ? "disabled" : "unlimited").ColourValue())}.");
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

	private bool BuildingCommandLiquidProg(ICharacter actor, StringStack command, string progName,
		ProgVariableTypes returnType, Action<IFutureProg?> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which prog should be used for {progName}? Use {"clear".ColourCommand()} to clear it.");
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
			new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Item,
				ProgVariableTypes.Item,
				ProgVariableTypes.LiquidMixture,
				ProgVariableTypes.Number
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"The {progName} prog is now {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, string name, Action<string> setter,
		bool liquidDescriptionPlaceholder = false)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What emote should be shown for the {name} action?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		if (liquidDescriptionPlaceholder && !text.Contains("{0}"))
		{
			actor.OutputHandler.Send("The liquid acceptance echo must contain {0} for the liquid description.");
			return false;
		}

		var validationText = liquidDescriptionPlaceholder
			? string.Format(System.Globalization.CultureInfo.InvariantCulture, text, "some liquid")
			: text;
		var emote = new Emote(validationText, actor, actor);
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

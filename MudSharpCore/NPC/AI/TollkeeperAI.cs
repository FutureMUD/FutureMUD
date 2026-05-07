#nullable enable annotations

using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class TollkeeperAI : PathingAIBase
{
	private static readonly ProgVariableTypes CharacterCollection = ProgVariableTypes.Character | ProgVariableTypes.Collection;

	private const string DefaultDemandEmote =
		"@ say|says to $1, \"There is a toll of {toll} to pass {direction}.\"";

	private const string DefaultReminderEmote =
		"@ step|steps into $1's way and say|says, \"You need to pay the toll before passing {direction}.\"";

	private const string DefaultViolenceEmote =
		"@ shout|shouts, \"Violence will not waive the toll!\"";

	private const string DefaultPaymentAcceptedEmote =
		"@ accept|accepts $2 from $1 and permit|permits $1 to pass {direction}.";

	private const string DefaultInsufficientPaymentEmote =
		"@ return|returns $2 to $1 and say|says, \"That is not enough for the toll.\"";

	private const string DefaultWrongPaymentEmote =
		"@ return|returns $2 to $1 and say|says, \"That is not the toll I asked for.\"";

	private const string DefaultResetEmote =
		"@ resume|resumes guarding the way.";

	private const string DefaultDepositBeginEmote =
		"@ secure|secures the toll money.";

	private const string DefaultDepositCompleteEmote =
		"@ deposit|deposits $1.";

	private const string DefaultDepositFailedEmote =
		"@ frown|frowns at $1, unable to deposit the toll money.";

	private IFutureProg? TollCostProg { get; set; }
	private ICurrency? Currency { get; set; }
	private TimeSpan PermitDuration { get; set; }
	private bool MoveAsideForPaidTraffic { get; set; }
	private long DepositContainerId { get; set; }
	private bool DepositWhenQuiet { get; set; }

	private string DemandEmote { get; set; } = DefaultDemandEmote;
	private string ReminderEmote { get; set; } = DefaultReminderEmote;
	private string ViolenceEmote { get; set; } = DefaultViolenceEmote;
	private string PaymentAcceptedEmote { get; set; } = DefaultPaymentAcceptedEmote;
	private string InsufficientPaymentEmote { get; set; } = DefaultInsufficientPaymentEmote;
	private string WrongPaymentEmote { get; set; } = DefaultWrongPaymentEmote;
	private string ResetEmote { get; set; } = DefaultResetEmote;
	private string DepositBeginEmote { get; set; } = DefaultDepositBeginEmote;
	private string DepositCompleteEmote { get; set; } = DefaultDepositCompleteEmote;
	private string DepositFailedEmote { get; set; } = DefaultDepositFailedEmote;

	protected TollkeeperAI(Models.ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private TollkeeperAI()
	{
	}

	private TollkeeperAI(IFuturemud gameworld, string name) : base(gameworld, name, "Tollkeeper")
	{
		Currency = Gameworld.Currencies.Get(Gameworld.GetStaticLong("DefaultCurrencyID")) ??
		           Gameworld.Currencies.FirstOrDefault();
		TollCostProg = Gameworld.AlwaysOneProg;
		PermitDuration = TimeSpan.FromSeconds(30);
		DepositWhenQuiet = true;
		DatabaseInitialise();
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		TollCostProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("TollCostProg")?.Value ?? "0"));
		Currency = Gameworld.Currencies.Get(long.Parse(root.Element("Currency")?.Value ?? "0"));
		PermitDuration = TimeSpan.FromMilliseconds(double.Parse(root.Element("PermitDurationMilliseconds")?.Value ?? "30000"));
		MoveAsideForPaidTraffic = bool.Parse(root.Element("MoveAsideForPaidTraffic")?.Value ?? "false");
		DepositContainerId = long.Parse(root.Element("DepositContainerId")?.Value ?? "0");
		DepositWhenQuiet = bool.Parse(root.Element("DepositWhenQuiet")?.Value ?? "true");

		DemandEmote = root.Element("DemandEmote")?.Value ?? DefaultDemandEmote;
		ReminderEmote = root.Element("ReminderEmote")?.Value ?? DefaultReminderEmote;
		ViolenceEmote = root.Element("ViolenceEmote")?.Value ?? DefaultViolenceEmote;
		PaymentAcceptedEmote = root.Element("PaymentAcceptedEmote")?.Value ?? DefaultPaymentAcceptedEmote;
		InsufficientPaymentEmote = root.Element("InsufficientPaymentEmote")?.Value ?? DefaultInsufficientPaymentEmote;
		WrongPaymentEmote = root.Element("WrongPaymentEmote")?.Value ?? DefaultWrongPaymentEmote;
		ResetEmote = root.Element("ResetEmote")?.Value ?? DefaultResetEmote;
		DepositBeginEmote = root.Element("DepositBeginEmote")?.Value ?? DefaultDepositBeginEmote;
		DepositCompleteEmote = root.Element("DepositCompleteEmote")?.Value ?? DefaultDepositCompleteEmote;
		DepositFailedEmote = root.Element("DepositFailedEmote")?.Value ?? DefaultDepositFailedEmote;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("TollCostProg", TollCostProg?.Id ?? 0L),
			new XElement("Currency", Currency?.Id ?? 0L),
			new XElement("PermitDurationMilliseconds", PermitDuration.TotalMilliseconds),
			new XElement("MoveAsideForPaidTraffic", MoveAsideForPaidTraffic),
			new XElement("DepositContainerId", DepositContainerId),
			new XElement("DepositWhenQuiet", DepositWhenQuiet),
			new XElement("DemandEmote", new XCData(DemandEmote ?? string.Empty)),
			new XElement("ReminderEmote", new XCData(ReminderEmote ?? string.Empty)),
			new XElement("ViolenceEmote", new XCData(ViolenceEmote ?? string.Empty)),
			new XElement("PaymentAcceptedEmote", new XCData(PaymentAcceptedEmote ?? string.Empty)),
			new XElement("InsufficientPaymentEmote", new XCData(InsufficientPaymentEmote ?? string.Empty)),
			new XElement("WrongPaymentEmote", new XCData(WrongPaymentEmote ?? string.Empty)),
			new XElement("ResetEmote", new XCData(ResetEmote ?? string.Empty)),
			new XElement("DepositBeginEmote", new XCData(DepositBeginEmote ?? string.Empty)),
			new XElement("DepositCompleteEmote", new XCData(DepositCompleteEmote ?? string.Empty)),
			new XElement("DepositFailedEmote", new XCData(DepositFailedEmote ?? string.Empty)),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Tollkeeper", (ai, gameworld) => new TollkeeperAI(ai, gameworld));
		RegisterAIBuilderInformation("tollkeeper", (gameworld, name) => new TollkeeperAI(gameworld, name), new TollkeeperAI().HelpText);
	}

	public override bool IsReadyToBeUsed =>
		TollCostProg is not null &&
		Currency is not null &&
		PermitDuration > TimeSpan.Zero;

	private IGameItem? DepositContainer => DepositContainerId > 0
		? Gameworld.Items.Get(DepositContainerId) ?? Gameworld.TryGetItem(DepositContainerId, true)
		: null;

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine($"Ready: {IsReadyToBeUsed.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine($"Guard Location: {"Set per NPC with the toll <direction> command".ColourCommand()}");
		sb.AppendLine($"Currency: {Currency?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Toll Cost Prog: {TollCostProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Permit Duration: {PermitDuration.Describe(actor).ColourValue()}");
		sb.AppendLine($"Paid Passage Mode: {(MoveAsideForPaidTraffic ? "Move Aside".ColourValue() : "Permit Movers".ColourValue())}");
		sb.AppendLine($"Deposit Container: {DepositContainer?.HowSeen(actor).ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Deposit Only When Quiet: {DepositWhenQuiet.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Pathing:");
		sb.AppendLine($"Open Doors: {OpenDoors.ToColouredString()}");
		sb.AppendLine($"Use Keys: {UseKeys.ToColouredString()}");
		sb.AppendLine($"Smash Doors: {SmashLockedDoors.ToColouredString()}");
		sb.AppendLine($"Close Doors: {CloseDoorsBehind.ToColouredString()}");
		sb.AppendLine($"Use Doorguards: {UseDoorguards.ToColouredString()}");
		sb.AppendLine($"Move Even If Blocked: {MoveEvenIfObstructionInWay.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Emotes:");
		sb.AppendLine($"\tDemand: {DemandEmote.ColourCommand()}");
		sb.AppendLine($"\tReminder: {ReminderEmote.ColourCommand()}");
		sb.AppendLine($"\tViolence: {ViolenceEmote.ColourCommand()}");
		sb.AppendLine($"\tAccepted: {PaymentAcceptedEmote.ColourCommand()}");
		sb.AppendLine($"\tInsufficient: {InsufficientPaymentEmote.ColourCommand()}");
		sb.AppendLine($"\tWrong: {WrongPaymentEmote.ColourCommand()}");
		sb.AppendLine($"\tReset: {ResetEmote.ColourCommand()}");
		sb.AppendLine($"\tDeposit Begin: {DepositBeginEmote.ColourCommand()}");
		sb.AppendLine($"\tDeposit Complete: {DepositCompleteEmote.ColourCommand()}");
		sb.AppendLine($"\tDeposit Failed: {DepositFailedEmote.ColourCommand()}");
		return sb.ToString();
	}

	protected override string TypeHelpText =>
		@"	#3currency <currency>#0 - sets the currency paid for the toll
	#3cost <prog>#0 - sets the number prog for toll cost; params either Collection[Character] or Character, Exit, Collection[Character]
	#3permit <timespan>#0 - sets how long paid travellers may pass
	#3mode permit|aside#0 - uses per-character permits or removes the block entirely during the paid window
	#3deposit <container>#0 - sets the container where toll money should be deposited
	#3deposit clear#0 - clears the deposit container
	#3depositquiet#0 - toggles only leaving to deposit when things are quiet
	#3emote <type> <emote>#0 - sets an emote; types are demand, reminder, violence, accepted, insufficient, wrong, reset, depositbegin, depositcomplete, depositfail
	#3emote <type> clear#0 - clears an emote
	#3opendoors#0 - toggles AI using doors while depositing
	#3usekeys#0 - toggles AI using keys while depositing
	#3useguards#0 - toggles AI using doorguards while depositing
	#3closedoors#0 - toggles the AI closing doors behind them
	#3smashdoors#0 - toggles the AI smashing doors it can't get through
	#3forcemove#0 - toggles the AI moving even if it can't get through";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "cost":
			case "prog":
			case "costprog":
				return BuildingCommandCostProg(actor, command);
			case "permit":
			case "duration":
				return BuildingCommandPermitDuration(actor, command);
			case "mode":
				return BuildingCommandMode(actor, command);
			case "deposit":
			case "container":
				return BuildingCommandDepositContainer(actor, command);
			case "depositquiet":
			case "quiet":
				return BuildingCommandDepositQuiet(actor);
			case "emote":
			case "echo":
				return BuildingCommandEmote(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which currency should this tollkeeper accept? The valid currencies are {Gameworld.Currencies.Select(x => x.Name.ColourName()).ListToString()}.");
			return false;
		}

		var currency = Gameworld.Currencies.GetByIdOrName(command.SafeRemainingArgument);
		if (currency is null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return false;
		}

		Currency = currency;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now demand payment in {currency.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandCostProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should calculate the toll?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Number, new[]
			{
				new[] { CharacterCollection },
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Exit, CharacterCollection }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TollCostProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to calculate the toll.");
		return true;
	}

	private bool BuildingCommandPermitDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should a paid permit last?");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value) || (TimeSpan)value <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send("That is not a valid positive timespan.");
			return false;
		}

		PermitDuration = (TimeSpan)value;
		Changed = true;
		actor.OutputHandler.Send($"Paid travellers will now be permitted for {PermitDuration.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want the mode to be #3permit#0 or #3aside#0?".SubstituteANSIColour());
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "permit":
			case "permits":
			case "exempt":
			case "exemption":
				MoveAsideForPaidTraffic = false;
				break;
			case "aside":
			case "moveaside":
			case "open":
				MoveAsideForPaidTraffic = true;
				break;
			default:
				actor.OutputHandler.Send("That is not a valid mode. Use #3permit#0 or #3aside#0.".SubstituteANSIColour());
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send(MoveAsideForPaidTraffic
			? "This AI will now remove the guard block entirely during the paid passage window."
			: "This AI will now add paid travellers as permits to its guard block.");
		return true;
	}

	private bool BuildingCommandDepositContainer(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which container should this tollkeeper deposit money into, or do you want to #3clear#0 the deposit container?".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			DepositContainerId = 0;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer use a deposit container.");
			return true;
		}

		var item = actor.TargetItem(command.SafeRemainingArgument);
		if (item is null)
		{
			actor.OutputHandler.Send("You do not see any such item.");
			return false;
		}

		if (item.GetItemType<IContainer>() is null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not a container.");
			return false;
		}

		DepositContainerId = item.Id;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now deposit toll money into {item.HowSeen(actor).ColourName()}.");
		return true;
	}

	private bool BuildingCommandDepositQuiet(ICharacter actor)
	{
		DepositWhenQuiet = !DepositWhenQuiet;
		Changed = true;
		actor.OutputHandler.Send($"This AI will {DepositWhenQuiet.NowNoLonger()} wait until things are quiet before leaving to deposit toll money.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which emote do you want to edit? The valid types are demand, reminder, violence, accepted, insufficient, wrong, reset, depositbegin, depositcomplete and depositfail.");
			return false;
		}

		var type = command.PopForSwitch();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the emote be, or do you want to #3clear#0 it?".SubstituteANSIColour());
			return false;
		}

		if (!TryGetEmote(type, out var current, out var setter, out var description))
		{
			actor.OutputHandler.Send("That is not a valid emote type. The valid types are demand, reminder, violence, accepted, insufficient, wrong, reset, depositbegin, depositcomplete and depositfail.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			setter(string.Empty);
			Changed = true;
			actor.OutputHandler.Send($"This AI will no longer echo anything for {description.ColourName()}.");
			return true;
		}

		var text = command.SafeRemainingArgument;
		var dummy = new Emote(PrepareEmote(text, 1.0M, null, actor), actor, actor, actor, actor, actor);
		if (!dummy.Valid)
		{
			actor.OutputHandler.Send(dummy.ErrorMessage);
			return false;
		}

		setter(text);
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the following emote for {description.ColourName()}:\n{text.ColourCommand()}");
		return true;
	}

	private bool TryGetEmote(string type, out string current, out Action<string> setter, out string description)
	{
		switch (type)
		{
			case "demand":
			case "enter":
				current = DemandEmote;
				setter = value => DemandEmote = value;
				description = "demanding the toll";
				return true;
			case "reminder":
			case "move":
			case "blocked":
				current = ReminderEmote;
				setter = value => ReminderEmote = value;
				description = "reminding movers";
				return true;
			case "violence":
			case "combat":
				current = ViolenceEmote;
				setter = value => ViolenceEmote = value;
				description = "responding to violence";
				return true;
			case "accepted":
			case "accept":
			case "pay":
			case "payment":
				current = PaymentAcceptedEmote;
				setter = value => PaymentAcceptedEmote = value;
				description = "accepting payment";
				return true;
			case "insufficient":
			case "short":
				current = InsufficientPaymentEmote;
				setter = value => InsufficientPaymentEmote = value;
				description = "insufficient payment";
				return true;
			case "wrong":
			case "invalid":
				current = WrongPaymentEmote;
				setter = value => WrongPaymentEmote = value;
				description = "wrong payment";
				return true;
			case "reset":
			case "expire":
				current = ResetEmote;
				setter = value => ResetEmote = value;
				description = "resetting the block";
				return true;
			case "depositbegin":
			case "depositstart":
				current = DepositBeginEmote;
				setter = value => DepositBeginEmote = value;
				description = "starting a deposit";
				return true;
			case "depositcomplete":
			case "deposited":
				current = DepositCompleteEmote;
				setter = value => DepositCompleteEmote = value;
				description = "completing a deposit";
				return true;
			case "depositfail":
			case "depositfailed":
				current = DepositFailedEmote;
				setter = value => DepositFailedEmote = value;
				description = "failed deposit";
				return true;
			default:
				current = string.Empty;
				setter = _ => { };
				description = string.Empty;
				return false;
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterEnterCellFinishWitness:
				return HandleCharacterEnterCellFinishWitness((ICharacter)arguments[3], (ICharacter)arguments[0], (ICell)arguments[1]);
			case EventType.CharacterBeginMovementWitness:
			case EventType.CharacterStopMovementWitness:
				return HandleCharacterAttemptMovement((ICharacter)arguments[3], (ICharacter)arguments[0], (ICellExit)arguments[2]);
			case EventType.CharacterGiveItemReceiver:
				return HandleCharacterGiveItemReceiver((ICharacter)arguments[1], (ICharacter)arguments[0], (IGameItem)arguments[2]);
			case EventType.EngagedInCombat:
				return HandleEngagedInCombat((ICharacter)arguments[1], (ICharacter)arguments[0]);
			case EventType.NPCOnGameLoadFinished:
			case EventType.FiveSecondTick:
			case EventType.MinuteTick:
				HandleCurrentLocation((ICharacter)arguments[0]);
				return base.HandleEvent(type, arguments);
			default:
				return base.HandleEvent(type, arguments);
		}
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterEnterCellFinishWitness:
				case EventType.CharacterBeginMovementWitness:
				case EventType.CharacterStopMovementWitness:
				case EventType.CharacterGiveItemReceiver:
				case EventType.EngagedInCombat:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	private bool HandleCharacterEnterCellFinishWitness(ICharacter tollkeeper, ICharacter mover, ICell cell)
	{
		var exit = GuardExit(tollkeeper);
		if (!CanActAsTollkeeper(tollkeeper) || exit is null || mover == tollkeeper || cell != exit.Origin || tollkeeper.Location != exit.Origin)
		{
			return false;
		}

		if (mover.Party is not null && mover.Party.Leader != mover)
		{
			return false;
		}

		EnsureGuardEffect(tollkeeper);
		if (IsPermittedToPass(tollkeeper, mover, exit))
		{
			return false;
		}

		var movers = GetProspectiveMovers(mover, exit).ToList();
		var toll = CalculateToll(tollkeeper, exit, movers);
		if (toll <= 0.0M)
		{
			GrantPassage(tollkeeper, movers, exit);
			return true;
		}

		EmitEmote(tollkeeper, DemandEmote, toll, exit, tollkeeper, mover);
		return true;
	}

	private bool HandleCharacterAttemptMovement(ICharacter tollkeeper, ICharacter mover, ICellExit exit)
	{
		var guardExit = GuardExit(tollkeeper);
		if (!CanActAsTollkeeper(tollkeeper) || guardExit is null || mover == tollkeeper || !IsConfiguredExit(exit, guardExit))
		{
			return false;
		}

		if (mover.Party is not null && mover.Party.Leader != mover)
		{
			return false;
		}

		if (mover.RidingMount is not null)
		{
			return false;
		}

		EnsureGuardEffect(tollkeeper);
		if (IsPermittedToPass(tollkeeper, mover, exit))
		{
			return false;
		}

		var movers = GetProspectiveMovers(mover, exit).ToList();
		var toll = CalculateToll(tollkeeper, exit, movers);
		if (toll <= 0.0M)
		{
			GrantPassage(tollkeeper, movers, exit);
			return true;
		}

		EmitEmote(tollkeeper, ReminderEmote, toll, exit, tollkeeper, mover);
		return true;
	}

	private bool HandleCharacterGiveItemReceiver(ICharacter tollkeeper, ICharacter giver, IGameItem item)
	{
		if (!CanActAsTollkeeper(tollkeeper) || giver == tollkeeper || Currency is null)
		{
			return false;
		}

		var exit = GuardExit(tollkeeper);
		if (exit is null || tollkeeper.Location != exit.Origin)
		{
			return false;
		}

		var movers = GetProspectiveMovers(giver, exit).ToList();
		var toll = CalculateToll(tollkeeper, exit, movers);
		if (toll <= 0.0M)
		{
			GrantPassage(tollkeeper, movers, exit);
			ReturnPayment(tollkeeper, giver, item, WrongPaymentEmote, toll, exit);
			return true;
		}

		var currencyPiles = item
			.RecursiveGetItems<ICurrencyPile>(false)
			.Where(x => x.Currency == Currency)
			.ToList();
		if (!currencyPiles.Any())
		{
			ReturnPayment(tollkeeper, giver, item, WrongPaymentEmote, toll, exit);
			return true;
		}

		var total = currencyPiles.Sum(x => x.TotalValue);
		if (total < toll)
		{
			ReturnPayment(tollkeeper, giver, item, InsufficientPaymentEmote, toll, exit);
			return true;
		}

		EmitEmote(tollkeeper, PaymentAcceptedEmote, toll, exit, tollkeeper, giver, item);
		GrantPassage(tollkeeper, movers, exit);
		return true;
	}

	private bool HandleEngagedInCombat(ICharacter tollkeeper, ICharacter aggressor)
	{
		var exit = GuardExit(tollkeeper);
		if (!CanActAsTollkeeper(tollkeeper) || exit is null || tollkeeper.Location != exit.Origin || aggressor == tollkeeper)
		{
			return false;
		}

		EmitEmote(tollkeeper, ViolenceEmote, 0.0M, exit, tollkeeper, aggressor);
		return true;
	}

	private void HandleCurrentLocation(ICharacter tollkeeper)
	{
		if (!CanActAsTollkeeper(tollkeeper))
		{
			return;
		}

		EnsureGuardEffect(tollkeeper);

		if (DepositContainerId > 0 && tollkeeper.Location == DepositContainer?.TrueLocations.FirstOrDefault())
		{
			DepositHeldMoney(tollkeeper);
		}
	}

	private bool CanActAsTollkeeper(ICharacter tollkeeper)
	{
		return IsReadyToBeUsed &&
		       !tollkeeper.State.IsDead() &&
		       !tollkeeper.State.IsInStatis();
	}

	private ITollkeeperModeEffect? TollkeeperMode(ICharacter tollkeeper)
	{
		return tollkeeper.EffectsOfType<ITollkeeperModeEffect>().FirstOrDefault();
	}

	private ICellExit? GuardExit(ICharacter tollkeeper)
	{
		return TollkeeperMode(tollkeeper)?.Exit;
	}

	private bool IsConfiguredExit(ICellExit exit, ICellExit guardExit)
	{
		return exit.Exit.Id == guardExit.Exit.Id && exit.Origin.Id == guardExit.Origin.Id;
	}

	private bool IsPermittedToPass(ICharacter tollkeeper, ICharacter mover, ICellExit exit)
	{
		if (tollkeeper.AffectedBy<TollExitPermit>(x => x.MoveAside && x.ExitId == exit.Exit.Id && x.GuardCellId == exit.Origin.Id))
		{
			return true;
		}

		var guard = tollkeeper
			.EffectsOfType<IGuardExitEffect>(x => x.Exit?.Exit.Id == exit.Exit.Id && x.Exit?.Origin.Id == exit.Origin.Id)
			.FirstOrDefault();
		return guard?.PermittedToCross(mover, exit) == true;
	}

	private IGuardExitEffect? EnsureGuardEffect(ICharacter tollkeeper)
	{
		var exit = GuardExit(tollkeeper);
		if (exit is null || tollkeeper.Location != exit.Origin)
		{
			return null;
		}

		if (tollkeeper.AffectedBy<TollExitPermit>(x => x.MoveAside && x.ExitId == exit.Exit.Id && x.GuardCellId == exit.Origin.Id))
		{
			return null;
		}

		var guard = tollkeeper
			.EffectsOfType<IGuardExitEffect>(x => x.Exit?.Exit.Id == exit.Exit.Id && x.Exit?.Origin.Id == exit.Origin.Id)
			.FirstOrDefault();
		if (guard is not null)
		{
			return guard;
		}

		guard = new GuardingExit(tollkeeper, exit, false);
		tollkeeper.AddEffect((IEffect)guard);
		return guard;
	}

	private decimal CalculateToll(ICharacter tollkeeper, ICellExit exit, IReadOnlyCollection<ICharacter> movers)
	{
		if (TollCostProg is null)
		{
			return 0.0M;
		}

		var toll = TollCostProg.MatchesParameters(new[] { CharacterCollection })
			? TollCostProg.ExecuteDecimal(movers)
			: TollCostProg.ExecuteDecimal(tollkeeper, exit, movers);

		return Math.Max(0.0M, toll);
	}

	private IEnumerable<ICharacter> GetProspectiveMovers(ICharacter mover, ICellExit exit)
	{
		if (mover.Movement is not null)
		{
			return mover.Movement.CharacterMovers.ToList();
		}

		var primaryMovers = mover.Party?.Leader == mover
			? mover.Party.CharacterMembers.Where(x => x.InRoomLocation == mover.InRoomLocation).ToList()
			: new List<ICharacter> { mover };

		var considered = new List<ICharacter>();
		var nonDraggers = new List<ICharacter>();
		var mounts = new List<ICharacter>();
		var draggers = new List<ICharacter>();
		var helpers = new List<ICharacter>();
		var nonConsensualMovers = new List<ICharacter>();
		var targets = new List<IPerceivable>();
		var dragEffects = new List<Dragging>();

		foreach (var character in primaryMovers)
		{
			MudSharp.Movement.Movement.EvaluateCharacterForAdditionToMovement(
				mover.Party,
				character,
				exit,
				considered,
				nonDraggers,
				mounts,
				draggers,
				helpers,
				nonConsensualMovers,
				targets,
				dragEffects,
				true,
				false);
		}

		return considered
			.Concat(targets.OfType<ICharacter>())
			.Distinct()
			.ToList();
	}

	private void GrantPassage(ICharacter tollkeeper, IEnumerable<ICharacter> movers, ICellExit exit)
	{
		var moverList = movers.Distinct().ToList();
		if (!moverList.Any())
		{
			return;
		}

		var permit = tollkeeper
			.EffectsOfType<TollExitPermit>(x => x.ExitId == exit.Exit.Id && x.GuardCellId == exit.Origin.Id && x.MoveAside == MoveAsideForPaidTraffic)
			.FirstOrDefault();
		if (permit is null)
		{
			permit = new TollExitPermit(tollkeeper, exit, MoveAsideForPaidTraffic, PrepareEmote(ResetEmote, 0.0M, exit, tollkeeper));
			tollkeeper.AddEffect(permit, PermitDuration);
		}
		else
		{
			tollkeeper.Reschedule(permit, PermitDuration);
		}

		permit.AddPermittedCharacters(moverList);

		if (MoveAsideForPaidTraffic)
		{
			tollkeeper.RemoveAllEffects<IGuardExitEffect>(x => x.Exit?.Exit.Id == exit.Exit.Id && x.Exit?.Origin.Id == exit.Origin.Id, true);
			return;
		}

		var guard = EnsureGuardEffect(tollkeeper);
		if (guard is null)
		{
			return;
		}

		foreach (var mover in moverList)
		{
			guard.Exempt(mover);
		}
	}

	private void ReturnPayment(ICharacter tollkeeper, ICharacter giver, IGameItem item, string emote, decimal toll, ICellExit? exit)
	{
		EmitEmote(tollkeeper, emote, toll, exit, tollkeeper, giver, item);
		if (item.Deleted || item.InInventoryOf != tollkeeper.Body)
		{
			return;
		}

		if (tollkeeper.Body.CanGive(item, giver.Body))
		{
			tollkeeper.Body.Give(item, giver.Body);
		}
		else
		{
			tollkeeper.Body.Drop(item, silent: true);
		}
	}

	private IEnumerable<IGameItem> HeldTollMoney(ICharacter tollkeeper)
	{
		if (Currency is null)
		{
			return Enumerable.Empty<IGameItem>();
		}

		return tollkeeper.Body.ExternalItems
			.Where(x => x.RecursiveGetItems<ICurrencyPile>(false).Any(y => y.Currency == Currency))
			.ToList();
	}

	private bool ShouldDeposit(ICharacter tollkeeper)
	{
		if (DepositContainerId <= 0 || DepositContainer?.GetItemType<IContainer>() is null)
		{
			return false;
		}

		if (!HeldTollMoney(tollkeeper).Any())
		{
			return false;
		}

		return !DepositWhenQuiet || ThingsAreQuiet(tollkeeper);
	}

	private bool ThingsAreQuiet(ICharacter tollkeeper)
	{
		if (tollkeeper.Combat is not null || tollkeeper.Movement is not null)
		{
			return false;
		}

		if (tollkeeper.AffectedBy<TollExitPermit>())
		{
			return false;
		}

		var exit = GuardExit(tollkeeper);
		if (exit is null)
		{
			return true;
		}

		if (tollkeeper.Location != exit.Origin)
		{
			return true;
		}

		return !tollkeeper.Location.Characters.Any(x =>
			x != tollkeeper &&
			!x.State.IsDead() &&
			!x.State.IsInStatis() &&
			tollkeeper.CanSee(x) &&
			!IsPermittedToPass(tollkeeper, x, exit));
	}

	private void DepositHeldMoney(ICharacter tollkeeper)
	{
		var containerItem = DepositContainer;
		var container = containerItem?.GetItemType<IContainer>();
		if (containerItem is null || container is null)
		{
			return;
		}

		var items = HeldTollMoney(tollkeeper).ToList();
		if (!items.Any())
		{
			return;
		}

		var exit = GuardExit(tollkeeper);
		EmitEmote(tollkeeper, DepositBeginEmote, 0.0M, exit, tollkeeper, containerItem);
		if (!PrepareContainerForDeposit(tollkeeper, containerItem))
		{
			EmitEmote(tollkeeper, DepositFailedEmote, 0.0M, exit, tollkeeper, containerItem);
			return;
		}

		foreach (var item in items.Where(x => !x.Deleted && x.InInventoryOf == tollkeeper.Body).ToList())
		{
			if (tollkeeper.Body.CanPut(item, containerItem, null, 0, true))
			{
				tollkeeper.Body.Put(item, containerItem, null, silent: true);
			}
		}

		SecureContainerAfterDeposit(tollkeeper, containerItem);
		EmitEmote(tollkeeper, DepositCompleteEmote, 0.0M, exit, tollkeeper, containerItem);
	}

	private bool PrepareContainerForDeposit(ICharacter tollkeeper, IGameItem containerItem)
	{
		var lockable = containerItem.GetItemType<ILockable>();
		if (lockable is not null)
		{
			foreach (var itemLock in lockable.Locks.Where(x => x.IsLocked).ToList())
			{
				var key = AvailableKeys(tollkeeper).FirstOrDefault(x => itemLock.CanUnlock(tollkeeper, x));
				if (itemLock.CanUnlock(tollkeeper, null))
				{
					itemLock.Unlock(tollkeeper, null, containerItem, null);
					continue;
				}

				if (key is null)
				{
					return false;
				}

				itemLock.Unlock(tollkeeper, key, containerItem, null);
			}
		}

		var openable = containerItem.GetItemType<IOpenable>();
		if (openable is not null && !openable.IsOpen)
		{
			if (!tollkeeper.Body.CanOpen(openable))
			{
				return false;
			}

			tollkeeper.Body.Open(openable, null, null);
		}

		return true;
	}

	private void SecureContainerAfterDeposit(ICharacter tollkeeper, IGameItem containerItem)
	{
		var openable = containerItem.GetItemType<IOpenable>();
		if (openable is not null && openable.IsOpen && tollkeeper.Body.CanClose(openable))
		{
			tollkeeper.Body.Close(openable, null, null);
		}

		var lockable = containerItem.GetItemType<ILockable>();
		if (lockable is null)
		{
			return;
		}

		foreach (var itemLock in lockable.Locks.Where(x => !x.IsLocked).ToList())
		{
			var key = AvailableKeys(tollkeeper).FirstOrDefault(x => itemLock.CanLock(tollkeeper, x));
			if (itemLock.CanLock(tollkeeper, null))
			{
				itemLock.Lock(tollkeeper, null, containerItem, null);
				continue;
			}

			if (key is not null)
			{
				itemLock.Lock(tollkeeper, key, containerItem, null);
			}
		}
	}

	private IEnumerable<IKey> AvailableKeys(ICharacter tollkeeper)
	{
		return tollkeeper.Body.ExternalItems
			.SelectNotNull(x => x.GetItemType<IKey>())
			.Concat(tollkeeper.Body.ExternalItems
				.SelectNotNull(x => x.GetItemType<IContainer>())
				.Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
				.SelectMany(x => x.Contents.SelectNotNull(y => y.GetItemType<IKey>())))
			.ToList();
	}

	protected override bool WouldMove(ICharacter ch)
	{
		if (!CanActAsTollkeeper(ch))
		{
			return false;
		}

		if (ShouldDeposit(ch))
		{
			var depositLocation = DepositContainer?.TrueLocations.FirstOrDefault();
			if (depositLocation is not null && ch.Location != depositLocation)
			{
				return true;
			}
		}

		var exit = GuardExit(ch);
		return exit is not null && ch.Location != exit.Origin;
	}

	protected override (ICell? Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var target = GetDesiredLocation(ch);
		if (target is null || target == ch.Location)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		var path = ch.PathBetween(target, 12, GetSuitabilityFunction(ch)).ToList();
		if (path.Any())
		{
			return (target, path);
		}

		if (MoveEvenIfObstructionInWay)
		{
			path = ch.PathBetween(target, 12, GetSuitabilityFunction(ch, false)).ToList();
			if (path.Any())
			{
				return (target, path);
			}
		}

		return (null, Enumerable.Empty<ICellExit>());
	}

	private ICell? GetDesiredLocation(ICharacter ch)
	{
		if (ShouldDeposit(ch))
		{
			var depositLocation = DepositContainer?.TrueLocations.FirstOrDefault();
			if (depositLocation is not null && depositLocation != ch.Location)
			{
				return depositLocation;
			}
		}

		var exit = GuardExit(ch);
		if (exit is not null && ch.Location != exit.Origin)
		{
			return exit.Origin;
		}

		return null;
	}

	private string PrepareEmote(string emote, decimal toll, ICellExit? exit, IFormatProvider format)
	{
		var tollText = Currency is null
			? toll.ToString("N2", format)
			: Currency.Describe(toll, CurrencyDescriptionPatternType.Short);
		var directionText = exit?.OutboundDirectionDescription ?? "the exit";
		return (emote ?? string.Empty)
			.Replace("{toll}", tollText, StringComparison.InvariantCultureIgnoreCase)
			.Replace("{currency}", Currency?.Name ?? "currency", StringComparison.InvariantCultureIgnoreCase)
			.Replace("{direction}", directionText, StringComparison.InvariantCultureIgnoreCase);
	}

	private void EmitEmote(ICharacter tollkeeper, string emote, decimal toll, ICellExit? exit, params IPerceivable[] perceivables)
	{
		if (string.IsNullOrWhiteSpace(emote))
		{
			return;
		}

		var parsed = new Emote(PrepareEmote(emote, toll, exit, tollkeeper), tollkeeper, perceivables);
		if (!parsed.Valid)
		{
			tollkeeper.OutputHandler.Send(parsed.ErrorMessage);
			return;
		}

		tollkeeper.OutputHandler.Handle(new EmoteOutput(parsed, flags: OutputFlags.SuppressObscured));
	}
}

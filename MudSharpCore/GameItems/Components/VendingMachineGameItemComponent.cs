using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class VendingMachineGameItemComponent : GameItemComponent, IContainer, IVendingMachine, IInsertable, IOnOff
{
	protected readonly List<IGameItem> _contents = new();
	private readonly List<VendingMachineSelection> _selections = new();
	private ICurrency _currency;
	private decimal _currentBalance;
	private decimal _internalBalance;
	protected VendingMachineGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	public IEnumerable<IGameItem> Contents => _contents;

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Contents.ToList())
		{
			_contents.Remove(item);
			item.Delete();
		}
	}

	public override void Quit()
	{
		foreach (var item in Contents)
		{
			item.Quit();
		}
	}

	public override void Login()
	{
		foreach (var item in Contents)
		{
			item.Login();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new VendingMachineGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Contents || type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Full:
				return
					$"{description}\n\nThis item is a vending machine. {(SwitchedOn ? "" : "It is currently unpowered and non-functional. ")}You can type {"list".Colour(Telnet.Yellow)} to see a list of what is for sale. Then, you can insert money (see {"help insert".FluentTagMXP("send", "href='help insert' hint='show the helpfile for the insert command'")}), make a selection (see {"help select".FluentTagMXP("send", "href='help select' hint='show the helpfile for the select command'")}), and then finally type {"select refund".Colour(Telnet.Yellow)} when you are done to get your change.\n\nAny purchases, and your refunded money will be in the machine, and you'll need to retrieve them.{(SwitchedOn ? "" : "\nIt is currently switched off, and cannot vend anything.".Colour(Telnet.Red))}";
			case DescriptionType.Contents:
				if (_contents.Any())
				{
					return description + "\n\nIt has the following contents:\n" +
					       (from item in _contents
					        select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
						       twoItemJoiner: "\n");
				}

				return description + "\n\nIt is currently empty.";
		}

		return description;
	}

	public override double ComponentWeight
	{
		get { return Contents.Sum(x => x.Weight); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Contents.Sum(x => x.Buoyancy(fluidDensity));
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.CommandInput && ((string)arguments[2]).Equals("vendingmachine") &&
		    ((ICharacter)arguments[0]).IsAdministrator())
		{
			// Optional check actor keyword with * argument
			VendingMachineCommand((ICharacter)arguments[0], (StringStack)arguments[3]);
			return true;
		}

		return base.HandleEvent(type, arguments);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (VendingMachineGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			from content in Contents select new XElement("Contained", content.Id),
			new XElement("Currency", _currency?.Id ?? 0),
			new XElement("CurrentBalance", _currentBalance),
			new XElement("InternalBalance", InternalBalance),
			new XElement("SwitchedOn", SwitchedOn),
			from selection in _selections select selection.SaveSelection()).ToString();
	}

	private void VendingMachineCommand(ICharacter character, StringStack command)
	{
		if (command.IsFinished)
		{
			character.Send("What do you want to do with this vending machine? You can add, swap or remove.");
			return;
		}

		switch (command.Pop().ToLowerInvariant())
		{
			case "currency":
				VendingMachineCommandCurrency(character, command);
				return;
			case "add":
				VendingMachineCommandAdd(character, command);
				return;
			case "swap":
				VendingMachineCommandSwap(character, command);
				return;
			case "remove":
				VendingMachineCommandRemove(character, command);
				return;
			case "on":
				VendingMachineCommandOn(character, command, true);
				return;
			case "off":
				VendingMachineCommandOn(character, command, false);
				return;
			case "list":
				VendingMachineCommandList(character, command);
				return;
			case "clear":
				VendingMachineCommandClear(character, command);
				return;
			default:
				character.Send(StringUtilities.HMark +
				               "What do you want to do with this vending machine? You can: add, swap, remove, list, on, off, currency, clear");
				return;
		}
	}

	private void VendingMachineCommandCurrency(ICharacter character, StringStack command)
	{
		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark + "What currency do you want to set for this vending machine?");
			character.Send(StringUtilities.Indent + "Options are: " +
			               Gameworld.Currencies.Select(x => x.Name).ListToString());
			return;
		}

		var currency = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Currencies.Get(value)
			: Gameworld.Currencies.GetByName(command.Last);
		if (currency == null)
		{
			character.Send(StringUtilities.HMark + "That is not a valid currency.");
			character.Send(StringUtilities.Indent + "Options are: " +
			               Gameworld.Currencies.Select(x => x.Name).ListToString());
			return;
		}

		_currency = currency;
		character.Send("You set the currency of this vending machine to {0}.", currency.Name.Colour(Telnet.Green));
	}

	private void VendingMachineCommandAdd(ICharacter character, StringStack command)
	{
		if (_currency == null)
		{
			character.Send(StringUtilities.HMark +
			               "You must first select a currency for this vending machine before you may do anything else with it.");
			return;
		}

		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark +
			               "What item do you want to add to the vending machine? Syntax is vendingmachine add <proto> <keyword> <cost> \"<description>\" \"<load args>\" <onloadprog>");
			return;
		}

		var proto = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.Last, true);
		if (proto == null)
		{
			character.Send(StringUtilities.HMark + "That is not a valid prototype to load in the vending machine.");
			return;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			character.Send(StringUtilities.HMark + "Only approved item protos may be used in vending machines.");
			return;
		}

		if (proto.IsItemType<CorpseGameItemComponentProto>() || proto.IsItemType<CurrencyGameItemComponentProto>())
		{
			character.Send(StringUtilities.HMark + "Items of that type may not be used in vending machines.");
			return;
		}

		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark + "What keyword do you want people to use when purchasing this item?");
			return;
		}

		var keyword = command.PopSpeech().ToLowerInvariant();

		if (Selections.Any(x => x.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)))
		{
			character.Send(
				StringUtilities.HMark + "There is already a choice with that keyword. Please choose another.");
			return;
		}

		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark +
			               "How much should this item cost when being purchased from a vending machine?");
			return;
		}

		var costText = command.PopSpeech();
		var result = _currency.GetBaseCurrency(costText, out var success);
		if (!success)
		{
			character.Send(
				StringUtilities.HMark + "That is not a valid amount of the {0} currency for that item to cost.",
				_currency.Name.Colour(Telnet.Green));
			return;
		}

		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark +
			               "What description do you want people to see when they list this item?");
			return;
		}

		var description = command.PopSpeech();

		IFutureProg onloadProg = null;
		var extraText = "";
		if (!command.IsFinished)
		{
			extraText = command.PopSpeech();
			if (!command.IsFinished)
			{
				onloadProg = long.TryParse(command.PopSpeech(), out value)
					? Gameworld.FutureProgs.Get(value)
					: Gameworld.FutureProgs.GetByName(command.Last);
				if (onloadProg == null)
				{
					character.Send(StringUtilities.HMark + "There is no such prog for you to use as an OnLoadProg.");
					return;
				}
			}
		}

		_selections.Add(new VendingMachineSelection
		{
			Cost = result,
			Description = description,
			Keyword = keyword,
			LoadArguments = extraText,
			OnLoadProg = onloadProg,
			Prototype = proto
		});
		Changed = true;
		character.Send(
			"This vending machine will now stock item proto {0}r{1} ({2}), keyword {3} and described as {4}, costing {5}{6}{7}.",
			proto.Id,
			proto.RevisionNumber,
			proto.Name,
			keyword.Colour(Telnet.Green),
			description.Colour(Telnet.Green),
			_currency.Describe(result, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green),
			onloadProg != null
				? $" with prog {onloadProg.Id} ({onloadProg.FunctionName}) executed upon loading"
				: "",
			!string.IsNullOrEmpty(extraText)
				? $" and with extra load parameters {extraText.Colour(Telnet.Green)}"
				: ""
		);
	}

	private void VendingMachineCommandSwap(ICharacter character, StringStack command)
	{
		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark + "What is the first selection you want to swap the order of?");
			return;
		}

		var firstChoice = command.PopSpeech();
		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark + "What is the second selection you want to swap the order of?");
			return;
		}

		var secondChoice = command.PopSpeech();

		if (!Selections.Any(x => x.Keyword.Equals(firstChoice, StringComparison.InvariantCultureIgnoreCase)))
		{
			character.Send(StringUtilities.HMark + "There is no such selection for this vending machine as {0}.",
				firstChoice);
			return;
		}

		if (!Selections.Any(x => x.Keyword.Equals(secondChoice, StringComparison.InvariantCultureIgnoreCase)))
		{
			character.Send(StringUtilities.HMark + "There is no such selection for this vending machine as {0}.",
				secondChoice);
			return;
		}

		if (firstChoice.Equals(secondChoice, StringComparison.InvariantCultureIgnoreCase))
		{
			character.Send(StringUtilities.HMark + "You cannot swap a choice with itself.");
			return;
		}

		_selections.Swap(
			_selections.First(x => x.Keyword.Equals(firstChoice, StringComparison.InvariantCultureIgnoreCase)),
			_selections.First(x => x.Keyword.Equals(secondChoice, StringComparison.InvariantCultureIgnoreCase)));
		character.Send("You swap the order of selectables {0} and {1}.", firstChoice.Colour(Telnet.Yellow),
			secondChoice.Colour(Telnet.Yellow));
		Changed = true;
	}

	private void VendingMachineCommandRemove(ICharacter character, StringStack command)
	{
		if (command.IsFinished)
		{
			character.Send(StringUtilities.HMark + "Which selection do you want to remove from this vending machine?");
			return;
		}

		var choice = command.PopSpeech();
		var selection =
			Selections.FirstOrDefault(x => x.Keyword.Equals(choice, StringComparison.InvariantCultureIgnoreCase));
		if (selection == null)
		{
			character.Send(StringUtilities.HMark + "There is no such selection with a keyword {0}.",
				choice.Colour(Telnet.Yellow));
			return;
		}

		_selections.Remove(selection);
		Changed = true;
		character.Send("This vending machine will no longer stock {0}.", choice.Colour(Telnet.Yellow));
	}

	private void VendingMachineCommandOn(ICharacter character, StringStack command, bool turnOn)
	{
		if (turnOn)
		{
			if (SwitchedOn)
			{
				character.Send(StringUtilities.HMark + $"{Parent.HowSeen(character, true)} is already powered on.");
			}
			else
			{
				SwitchedOn = true;
				character.Send(StringUtilities.HMark + $"{Parent.HowSeen(character, true)} is now powered on.");
			}
		}
		else
		{
			if (!SwitchedOn)
			{
				character.Send(StringUtilities.HMark + $"{Parent.HowSeen(character, true)} is already powered off.");
			}
			else
			{
				SwitchedOn = false;
				character.Send(StringUtilities.HMark + $"{Parent.HowSeen(character, true)} is now powered off.");
			}
		}
	}

	private void VendingMachineCommandList(ICharacter character, StringStack command)
	{
		var sb = new StringBuilder();

		sb.AppendLineFormat("{0} is stocked to vend the following items:", Parent.HowSeen(character, true));
		if (Selections.Any())
		{
			sb.AppendLine();
			var index = 1;
			sb.Append(StringUtilities.GetTextTable(from item in Selections
			                                       select
				                                       new[]
				                                       {
					                                       index++.ToString(), item.Keyword,
					                                       item.Description.Colour(Telnet.Green),
					                                       _currency.Describe(item.Cost,
						                                       CurrencyDescriptionPatternType.Short),
					                                       item.LoadArguments,
					                                       item.OnLoadProg != null
						                                       ? $"{item.OnLoadProg.Id} ({item.OnLoadProg.FunctionName})"
						                                       : "",
					                                       item.Prototype.Id.ToString()
				                                       },
				new[] { "Id", "Keyword", "Description", "Cost", "Args", "OnLoad", "ProtoID" },
				character.LineFormatLength, colour: Telnet.Cyan));
		}

		sb.AppendLine();

		character.Send(sb.ToString());
	}

	private void VendingMachineCommandClear(ICharacter character, StringStack command)
	{
		if (Selections.Any())
		{
			Selections.Clear();
			character.Send(StringUtilities.HMark + $"{Parent.HowSeen(character, true).Colour(Telnet.Green)} " +
			               $"machine inventory cleared!");
		}
		else
		{
			character.Send(StringUtilities.HMark + $"{Parent.HowSeen(character, true).Colour(Telnet.Green)} " +
			               $"already had no inventory.");
		}
	}

	#region IContainer Members

	public bool CanPut(IGameItem item)
	{
		return
			(item.Size <= _prototype.MaximumContentsSize || item.IsItemType<ICommodity>()) &&
			_contents.Sum(x => x.Weight) + item.Weight <= _prototype.WeightLimit;
	}

	public int CanPutAmount(IGameItem item)
	{
		return (int)((_prototype.WeightLimit - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
	}

	public void Put(ICharacter putter, IGameItem item, bool allowMerge = true)
	{
		if (_contents.Contains(item))
		{
#if DEBUG
			throw new ApplicationException("Item duplication in container.");
#endif
			return;
		}

		if (allowMerge)
		{
			var mergeTarget = _contents.FirstOrDefault(x => x.CanMerge(item));
			if (mergeTarget != null)
			{
				mergeTarget.Merge(item);
				item.Delete();
				return;
			}
		}

		_contents.Add(item);
		item.ContainedIn = Parent;
		Changed = true;
	}

	public WhyCannotPutReason WhyCannotPut(IGameItem item)
	{
		if (item.Size > _prototype.MaximumContentsSize)
		{
			return WhyCannotPutReason.ItemTooLarge;
		}

		if (_contents.Sum(x => x.Weight) + item.Weight > _prototype.WeightLimit)
		{
			var capacity =
				(int)((_prototype.WeightLimit - _contents.Sum(x => x.Weight)) / (item.Weight / item.Quantity));
			if (item.Quantity <= 1 || capacity <= 0)
			{
				return WhyCannotPutReason.ContainerFull;
			}

			return WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity;
		}

		return WhyCannotPutReason.NotContainer;
	}

	public bool CanTake(ICharacter taker, IGameItem item, int quantity)
	{
		return _contents.Contains(item) && item.CanGet(quantity).AsBool();
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		Changed = true;
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			_contents.Remove(item);
			item.ContainedIn = null;
			return item;
		}

		return item.Get(null, quantity);
	}

	public override bool Take(IGameItem item)
	{
		if (!_contents.Contains(item))
		{
			return false;
		}

		_contents.Remove(item);
		item.ContainedIn = null;
		Changed = true;
		return true;
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		return !_contents.Contains(item)
			? WhyCannotGetContainerReason.NotContained
			: WhyCannotGetContainerReason.NotContainer;
	}

	public bool Transparent => true;

	public string ContentsPreposition => "in";

	public void Empty(ICharacter emptier, IContainer intoContainer, IEmote playerEmote = null)
	{
		var location = emptier?.Location ?? Parent.TrueLocations.FirstOrDefault();
		var contents = Contents.ToList();
		_contents.Clear();
		if (emptier is not null)
		{
			if (intoContainer == null)
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote("@ empty|empties $0 onto the ground.", emptier, Parent)).Append(
						playerEmote));
			}
			else
			{
				emptier.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote($"@ empty|empties $1 {intoContainer.ContentsPreposition}to $2.",
						emptier, emptier, Parent, intoContainer.Parent)).Append(playerEmote));
			}
		}

		foreach (var item in contents)
		{
			item.ContainedIn = null;
			if (intoContainer != null)
			{
				if (intoContainer.CanPut(item))
				{
					intoContainer.Put(emptier, item);
				}
				else if (location != null)
				{
					location.Insert(item);
					if (emptier != null)
					{
						emptier.OutputHandler.Handle(new EmoteOutput(new Emote(
							"@ cannot put $1 into $2, so #0 set|sets it down on the ground.", emptier, emptier, item,
							intoContainer.Parent)));
					}
				}
				else
				{
					item.Delete();
				}

				continue;
			}

			if (location != null)
			{
				location.Insert(item);
			}
			else
			{
				item.Delete();
			}
		}

		Changed = true;
	}

	#endregion

	#region Constructors

	public VendingMachineGameItemComponent(VendingMachineGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public VendingMachineGameItemComponent(MudSharp.Models.GameItemComponent component,
		VendingMachineGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("Currency");
		if (element != null)
		{
			_currency = Gameworld.Currencies.Get(long.Parse(element.Value));
		}

		element = root.Element("CurrentBalance");
		if (element != null)
		{
			_currentBalance = decimal.Parse(element.Value);
		}

		element = root.Element("InternalBalance");
		if (element != null)
		{
			_internalBalance = decimal.Parse(element.Value);
		}

		element = root.Element("SwitchedOn");
		_switchedOn = bool.Parse(element?.Value ?? "true");

		foreach (var item in root.Elements("Selection"))
		{
			_selections.Add(new VendingMachineSelection(item, Gameworld));
		}

		using (new FMDB())
		{
			foreach (var item in root.Elements("Contained"))
			{
				var newItem = Gameworld.TryGetItem(long.Parse(item.Value), true);
				_contents.Add(newItem);
				newItem.Get(null);
				newItem.LoadTimeSetContainedIn(Parent);
			}
		}
	}

	public VendingMachineGameItemComponent(VendingMachineGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_contents = new List<IGameItem>();
		_currency = rhs._currency;
		_currentBalance = rhs._currentBalance;
		_internalBalance = rhs._internalBalance;
		_selections.AddRange(rhs.Selections);
	}

	public override void FinaliseLoad()
	{
		foreach (var item in Contents)
		{
			item.FinaliseLoadTimeTasks();
		}
	}

	#endregion

	#region IVendingMachine Members

	public decimal InternalBalance
	{
		get => _internalBalance;
		set
		{
			_internalBalance = value;
			Changed = true;
		}
	}

	public decimal CurrentBalance
	{
		get => _currentBalance;
		set
		{
			_currentBalance = value;
			Changed = true;
		}
	}

	public ICurrency Currency => _currency;

	public IList<VendingMachineSelection> Selections => _selections;

	public string ShowList(IPerceiver voyeur, string arguments)
	{
		if (_currency == null)
		{
			return $"{Parent.HowSeen(voyeur, true)} has not yet been set up to work correctly.";
		}

		var sb = new StringBuilder();
		sb.AppendLineFormat("{0} can vend the following items:", Parent.HowSeen(voyeur, true));
		if (_selections.Any())
		{
			sb.AppendLine();
			var index = 1;
			sb.Append(StringUtilities.GetTextTable(from item in Selections
			                                       select
				                                       new[]
				                                       {
					                                       index++.ToString(), item.Keyword,
					                                       item.Description.Colour(Telnet.Green),
					                                       _currency.Describe(item.Cost,
						                                       CurrencyDescriptionPatternType.Short)
				                                       },
				new[] { "Id", "Keyword", "Description", "Cost" },
				voyeur.LineFormatLength, colour: Telnet.Cyan));
		}

		sb.AppendLine();
		sb.AppendFormat("The current balance is showing as {0}.",
			_currency.Describe(_currentBalance, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green));
		return sb.ToString();
	}

	public bool Refund(ICharacter character)
	{
		if (_currentBalance <= 0.0M)
		{
			character.Send("There is no balance to refund.");
			return false;
		}

		var coins = _currency.FindCoinsForAmount(_currentBalance, out var exact);

		var newCurrencyPile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(_currency,
			coins.Select(x => Tuple.Create(x.Key, x.Value)).ToList());
		Gameworld.Add(newCurrencyPile);
		_contents.Add(newCurrencyPile);
		_currentBalance = 0.0M;
		Changed = true;
		character.OutputHandler.Handle(
			new EmoteOutput(new Emote(_prototype.RefundMoneyEmote, character, character, Parent, newCurrencyPile)));
		return true;
	}

	#endregion

	#region IInsertable Members

	public bool CanInsert(ICharacter actor, IGameItem item)
	{
		if (item?.IsItemType<ICurrencyPile>() != true)
		{
			return false;
		}

		return item.GetItemType<ICurrencyPile>().Currency == _currency;
	}

	public bool Insert(ICharacter actor, IGameItem item, IEmote playerEmote, bool silent = false)
	{
		if (!CanInsert(actor, item))
		{
			actor.Send(WhyCannotInsert(actor, item));
			return false;
		}

		var currency = item.GetItemType<ICurrencyPile>();
		_currentBalance += currency.Coins.Sum(x => x.Item1.Value * x.Item2);
		if (!silent)
		{
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote(_prototype.InsertMoneyEmote, actor, actor, item, Parent)).Append(
					playerEmote));
		}

		actor.Body.Take(item);
		Changed = true;
		item.Delete();
		return true;
	}

	private string WhyCannotInsert(ICharacter actor, IGameItem item)
	{
		if (item == null)
		{
			return "You cannot insert nothing!";
		}

		if (!item.IsItemType<ICurrencyPile>())
		{
			return $"{item.HowSeen(actor, true)} is not a currency pile.";
		}

		if (item.GetItemType<ICurrencyPile>().Currency != _currency)
		{
			return $"{item.HowSeen(actor, true)} is not the right currency for this vending machine.";
		}

		return "You cannot insert " + item.HowSeen(actor);
	}

	#endregion

	#region ISelectable Members

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent)
	{
		if (!SwitchedOn)
		{
			character.Send(
				$"{Parent.HowSeen(character, true)} is currently switched off, and so you cannot make any selections.");
			return false;
		}

		if (argument.Equals("refund", StringComparison.InvariantCultureIgnoreCase))
		{
			return Refund(character);
		}

		VendingMachineSelection selection = null;
		if (argument.IsInteger())
		{
			selection = _selections.ElementAtOrDefault((int)long.Parse(argument) - 1);
		}
		else
		{
			selection =
				_selections.FirstOrDefault(
					x => x.Keyword.Equals(argument, StringComparison.InvariantCultureIgnoreCase));
		}

		if (selection == null)
		{
			character.Send("That is not a valid selection to make.");
			return false;
		}

		if (_currentBalance < selection.Cost)
		{
			character.OutputHandler.Handle(
				new EmoteOutput(new Emote(string.Format(_prototype.InvalidItemSelectedEmote, selection.Keyword),
					character, character, Parent)));
			return false;
		}

		var newItem = selection.Prototype.CreateNew(character);
		var newItemVariable = newItem.GetItemType<IVariable>();
		if (newItemVariable != null)
		{
			if (string.IsNullOrWhiteSpace(selection.LoadArguments))
			{
				foreach (var variable in newItemVariable.CharacteristicDefinitions)
				{
					newItemVariable.SetRandom(variable);
				}
			}
			else
			{
				var prePopulatedVariables = new Dictionary<ICharacteristicDefinition, ICharacteristicValue>();
				var regex = new Regex("(\\w+)(?:=|\\:)(\"(:?[\\w ]+)\"|(:?[\\w]+))");
				foreach (Match match in regex.Matches(selection.LoadArguments))
				{
					ICharacteristicDefinition definition = null;
					ICharacteristicValue cvalue = null;

					definition =
						newItemVariable.CharacteristicDefinitions.FirstOrDefault(
							x => x.Pattern.IsMatch(match.Groups[1].Value));
					if (definition == null)
					{
						continue;
					}

					var target = !string.IsNullOrEmpty(match.Groups[3].Value)
						? match.Groups[3].Value
						: match.Groups[4].Value;
					long valueid;
					if (target[0] == ':')
					{
						ICharacteristicProfile profile = null;
						profile = long.TryParse(target.Substring(1), out valueid)
							? Gameworld.CharacteristicProfiles.Get(valueid)
							: Gameworld.CharacteristicProfiles.FirstOrDefault(
								x =>
									x.Name.StartsWith(target.Substring(1),
										StringComparison.CurrentCultureIgnoreCase));

						if (profile == null)
						{
							continue;
						}

						cvalue = profile.GetRandomCharacteristic();
					}
					else
					{
						cvalue = long.TryParse(target, out valueid)
							? Gameworld.CharacteristicValues.Get(valueid)
							: Gameworld.CharacteristicValues.Where(x => definition.IsValue(x))
							           .FirstOrDefault(
								           x => x.Name.StartsWith(target, StringComparison.CurrentCultureIgnoreCase));
					}

					if (cvalue == null)
					{
						continue;
					}

					if (!definition.IsValue(cvalue))
					{
						continue;
					}

					prePopulatedVariables.Add(definition, cvalue);
					foreach (var characteristic in newItemVariable.CharacteristicDefinitions)
					{
						if (prePopulatedVariables.ContainsKey(characteristic))
						{
							newItemVariable.SetCharacteristic(characteristic, prePopulatedVariables[characteristic]);
						}
						else
						{
							newItemVariable.SetRandom(characteristic);
						}
					}
				}
			}
		}

		_contents.Add(newItem);
		selection.OnLoadProg?.Execute(newItem, character);

		_currentBalance -= selection.Cost;
		_internalBalance += selection.Cost;
		character.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(string.Format(_prototype.ItemSelectedEmote, selection.Keyword), character,
				character, Parent, newItem)).Append(playerEmote));
		Changed = true;
		newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
		return true;
	}

	public bool CanSelect(ICharacter character, string argument)
	{
		if (argument.Equals("refund", StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}

		VendingMachineSelection selection;
		if (argument.IsInteger())
		{
			selection = _selections.ElementAtOrDefault((int)long.Parse(argument) - 1);
		}
		else
		{
			selection =
				_selections.FirstOrDefault(
					x => x.Keyword.Equals(argument, StringComparison.InvariantCultureIgnoreCase));
		}

		return _currentBalance >= selection?.Cost;
	}

	#endregion

	#region IOnOff Members

	private bool _switchedOn = true;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			Changed = true;
		}
	}

	#endregion
}
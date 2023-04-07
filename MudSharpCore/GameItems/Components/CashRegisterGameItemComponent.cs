using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg.Statements;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;

namespace MudSharp.GameItems.Components;

public class CashRegisterGameItemComponent : GameItemComponent, IContainer, ISelectable, IOpenable
{
	protected CashRegisterGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CashRegisterGameItemComponentProto)newProto;
	}

	#region Constructors

	public CashRegisterGameItemComponent(CashRegisterGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public CashRegisterGameItemComponent(Models.GameItemComponent component, CashRegisterGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CashRegisterGameItemComponent(CashRegisterGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("Open");
		if (attr != null)
		{
			_isOpen = attr.Value == "true";
		}

		foreach (
			var item in
			root.Elements("Contained")
			    .Select(element => Gameworld.TryGetItem(long.Parse(element.Value), true))
			    .Where(item => item != null))
		{
			if (item.ContainedIn != null || item.Location != null || item.InInventoryOf != null)
			{
				Changed = true;
				Gameworld.SystemMessage(
					$"Duplicated Item: {item.HowSeen(item, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {item.Id.ToString("N0")}",
					true);
				continue;
			}

			_contents.Add(item);
			item.Get(null);
			item.LoadTimeSetContainedIn(Parent);
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CashRegisterGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XAttribute("Open", IsOpen.ToString().ToLowerInvariant()),
			from content in Contents select new XElement("Contained", content.Id)
		).ToString();
	}

	#endregion

	#region Implementation of IContainer

	private readonly List<IGameItem> _contents = new();
	public IEnumerable<IGameItem> Contents => _contents;
	public string ContentsPreposition => "in";
	public bool Transparent => false;

	public bool CanPut(IGameItem item)
	{
		return
			item != Parent &&
			IsOpen &&
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
		if (item == Parent)
		{
			return WhyCannotPutReason.CantPutContainerInItself;
		}

		if (!IsOpen)
		{
			return WhyCannotPutReason.ContainerClosed;
		}

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
		return
			IsOpen &&
			_contents.Contains(item) &&
			(taker?.Account.ActLawfully != true || IsAllowedToInteract(taker)) &&
			item.CanGet(quantity).AsBool();
	}

	public IGameItem Take(ICharacter taker, IGameItem item, int quantity)
	{
		Changed = true;
		if (quantity == 0 || item.DropsWhole(quantity))
		{
			_contents.Remove(item);
			item.ContainedIn = null;
			if (!IsAllowedToInteract(taker))
			{
				CrimeExtensions.CheckPossibleCrimeAllAuthorities(taker, CrimeTypes.Theft, null, item, "shoplifting");
			}

			return item;
		}

		var newItem = item.Get(null, quantity);
		if (!IsAllowedToInteract(taker))
		{
			CrimeExtensions.CheckPossibleCrimeAllAuthorities(taker, CrimeTypes.Theft, null, newItem, "shoplifting");
		}

		return newItem;
	}

	public WhyCannotGetContainerReason WhyCannotTake(ICharacter taker, IGameItem item)
	{
		if (!IsOpen)
		{
			return WhyCannotGetContainerReason.ContainerClosed;
		}

		if (!_contents.Contains(item))
		{
			return WhyCannotGetContainerReason.NotContained;
		}

		if (taker?.Account.ActLawfully == true && !IsAllowedToInteract(taker))
		{
			return WhyCannotGetContainerReason.UnlawfulAction;
		}

		return WhyCannotGetContainerReason.NotContainer;
	}

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

		var crime = !IsAllowedToInteract(emptier);
		foreach (var item in contents)
		{
			if (crime)
			{
				CrimeExtensions.CheckPossibleCrimeAllAuthorities(emptier, CrimeTypes.Theft, null, item,
					"shoplifting");
			}

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

	#region Implementation of ISelectable

	public bool CanSelect(ICharacter character, string argument)
	{
		switch (new StringStack(argument).PopSpeech().ToLowerInvariant())
		{
			case "open":
			case "nosale":
			case "no sale":
			case "no_sale":
				return true;
		}

		return false;
	}

	private bool IsAllowedToInteract(ICharacter character)
	{
		var shop = Parent.TrueLocations.First().Shop;
		return shop?.IsEmployee(character) != false;
	}

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
	{
		var ss = new StringStack(argument);
		var text = ss.PopSpeech();
		if (text.EqualToAny("open", "nosale", "no sale", "no_sale"))
		{
			if (IsOpen)
			{
				character.OutputHandler.Send($"The cash drawer of {Parent.HowSeen(character)} is already open.");
				return false;
			}

			var shop = Parent.TrueLocations.First().Shop;
			if (shop?.TillItems.Contains(Parent) != true || shop.IsEmployee(character) || ss.PopSpeech().EqualTo("!") ||
			    !character.Account.ActLawfully)
			{
				if (!silent)
				{
					character.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote(
							"@ select|selects the 'No Sale' button on $1, and the cash drawer opens.", character,
							character, Parent)).Append(playerEmote));
				}

				IsOpen = true;
				shop?.AddTransaction(new TransactionRecord(ShopTransactionType.AccessCashDraw, shop.Currency, shop,
					character.Location.DateTime(), shop.IsEmployee(character) ? character : null, 0.0M, 0.0M));
				if (shop?.IsEmployee(character) == false)
				{
					CrimeExtensions.CheckPossibleCrimeAllAuthorities(character, CrimeTypes.UnauthorisedDealing, null,
						Parent, "");
				}

				return true;
			}

			character.OutputHandler.Send(
				$"You may be guilty of a criminal offense if you proceed with this action. Please confirm that you want to proceed by putting a ! after your argument (e.g. select nosale !)");
			return false;
		}

		character.OutputHandler.Send(
			$"The only valid option for selecting things on {Parent.HowSeen(character)} is {"nosale".ColourCommand()}.");
		return false;
	}

	#endregion

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

	public override bool Take(IGameItem item)
	{
		if (Contents.Contains(item))
		{
			var shop = Parent.TrueLocations.First().Shop;
			var currency = item.GetItemType<ICurrencyPile>();
			if (currency is not null && shop is not null && currency.Currency == shop.Currency)
			{
				shop.AddTransaction(new TransactionRecord(ShopTransactionType.Withdrawal, currency.Currency, shop,
					shop.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime, null, currency.TotalValue,
					0.0M));
			}

			_contents.Remove(item);
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return (IsOpen && (type == DescriptionType.Contents || type == DescriptionType.Evaluate)) ||
		       type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Evaluate:
				return
					$"It can hold {Gameworld.UnitManager.DescribeMostSignificantExact(_prototype.WeightLimit, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)}.";
			case DescriptionType.Contents:
				if (_contents.Any())
				{
					return description + "\n\nIt has the following contents:\n" +
					       (from item in _contents
					        select "\t" + item.HowSeen(voyeur)).ListToString(separator: "\n", conjunction: "",
						       twoItemJoiner: "\n");
				}

				return description + "\n\nIt is currently empty.";
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.Append(description);
				sb.AppendLine();
				sb.AppendLine("It is a cash register.");
				sb.AppendLine($"It is is currently {(IsOpen ? "open" : "closed")}.".Colour(Telnet.Yellow));
				sb.AppendLine($"You can {"select".ColourCommand()} the {"nosale".ColourValue()} button.");
				if (IsOpen)
				{
					sb.AppendLine(
						$"It is {(_contents.Sum(x => x.Weight) / _prototype.WeightLimit).ToString("P2", voyeur).Colour(Telnet.Green)} full.");
				}

				return sb.ToString();
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

	public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		if (_contents.Contains(existingItem))
		{
			_contents[_contents.IndexOf(existingItem)] = newItem;
			newItem.ContainedIn = Parent;
			Changed = true;
			existingItem.ContainedIn = null;
			return true;
		}

		return false;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemContainer = newItem?.GetItemType<IContainer>();
		if (newItemContainer != null)
		{
			if (Contents.Any())
			{
				foreach (var item in Contents.ToList())
				{
					if (newItemContainer.CanPut(item))
					{
						newItemContainer.Put(null, item);
					}
					else if (location != null)
					{
						location.Insert(item);
						item.ContainedIn = null;
					}
					else
					{
						item.Delete();
					}
				}

				_contents.Clear();
			}
		}
		else
		{
			foreach (var item in Contents.ToList())
			{
				if (location != null)
				{
					location.Insert(item);
					item.ContainedIn = null;
				}
				else
				{
					item.Delete();
				}
			}

			_contents.Clear();
		}

		return false;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var truth = false;
		foreach (var content in Contents)
		{
			truth = truth || content.HandleEvent(type, arguments);
		}

		return truth;
	}

	public override void FinaliseLoad()
	{
		foreach (var item in Contents)
		{
			item.FinaliseLoadTimeTasks();
		}
	}

	#region IOpenable Members

	private bool _isOpen = true;

	public bool IsOpen
	{
		get => _isOpen;
		protected set
		{
			_isOpen = value;
			Changed = true;
		}
	}

	public bool CanOpen(IBody opener)
	{
		return false;
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		if (IsOpen)
		{
			return WhyCannotOpenReason.AlreadyOpen;
		}

		return WhyCannotOpenReason.AlternateMechanism;
	}

	public void Open()
	{
		IsOpen = true;
		OnOpen?.Invoke(this);
	}

	public bool CanClose(IBody closer)
	{
		return IsOpen;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		if (!IsOpen)
		{
			return WhyCannotCloseReason.AlreadyClosed;
		}

		return WhyCannotCloseReason.Unknown;
	}

	public void Close()
	{
		IsOpen = false;
		OnClose?.Invoke(this);
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

	#endregion
}
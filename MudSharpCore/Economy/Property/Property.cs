using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Trees;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;

namespace MudSharp.Economy.Property;

public class Property : SaveableItem, IProperty
{
	public Property(string name, IEconomicZone zone, ICell location, decimal value, IFrameworkItem owner,
		IBankAccount account)
	{
		Gameworld = zone.Gameworld;
		_name = name;
		_detailedDescription = "";
		_economicZone = zone;
		_lastSaleValue = value;
		_applyCriminalCodeInProperty = true;
		_lastChangeOfOwnership = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		_propertyLocations.Add(location);
		using (new FMDB())
		{
			var dbitem = new Models.Property();
			FMDB.Context.Properties.Add(dbitem);
			dbitem.EconomicZoneId = EconomicZone.Id;
			dbitem.ApplyCriminalCodeInProperty = ApplyCriminalCodeInProperty;
			dbitem.DetailedDescription = DetailedDescription;
			dbitem.Name = _name;
			dbitem.LastChangeOfOwnership = LastChangeOfOwnership.GetDateTimeString();
			dbitem.LastSaleValue = _lastSaleValue;
			dbitem.PropertyLocations.Add(new PropertyLocation { CellId = location.Id, Property = dbitem });
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		_propertyOwners.Add(new PropertyOwner(owner, 1.0M, account, this));
	}

	public Property(Models.Property property, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = property.Id;
		_name = property.Name;
		_detailedDescription = property.DetailedDescription;
		_economicZone = Gameworld.EconomicZones.Get(property.EconomicZoneId);
		_lastChangeOfOwnership = new MudDateTime(property.LastChangeOfOwnership, Gameworld);
		_applyCriminalCodeInProperty = property.ApplyCriminalCodeInProperty;
		_lastSaleValue = property.LastSaleValue;

		foreach (var owner in property.PropertyOwners)
		{
			_propertyOwners.Add(new PropertyOwner(owner, Gameworld));
		}

		if (property.SaleOrder != null)
		{
			_saleOrder = new PropertySaleOrder(property.SaleOrder, gameworld, this);
		}

		foreach (var order in property.LeaseOrders)
		{
			var newOrder = new PropertyLeaseOrder(order, this);
			if (property.LeaseOrderId == order.Id)
			{
				_leaseOrder = newOrder;
			}
			else
			{
				_expiredLeaseOrders.Add(newOrder);
			}
		}

		foreach (var lease in property.PropertyLeases)
		{
			var newLease = new PropertyLease(lease, this);
			if (property.LeaseId == lease.Id)
			{
				_lease = newLease;
			}
			else
			{
				_expiredLeases.Add(newLease);
			}
		}

		foreach (var location in property.PropertyLocations)
		{
			_propertyLocations.AddNotNull(Gameworld.Cells.Get(location.CellId));
		}

		foreach (var key in property.PropertyKeys)
		{
			_propertyKeys.Add(new PropertyKey(key, this, Gameworld));
		}
	}

	private IEconomicZone _economicZone;
	private readonly List<IPropertyOwner> _propertyOwners = new();
	private readonly List<ICell> _propertyLocations = new();
	private string _detailedDescription;
	private MudDateTime _lastChangeOfOwnership;
	private decimal _lastSaleValue;
	private IPropertySaleOrder _saleOrder;
	private IPropertyLeaseOrder _leaseOrder;
	private IPropertyLease _lease;
	private bool _applyCriminalCodeInProperty;
	private readonly List<IPropertyLease> _expiredLeases = new();
	private readonly List<IPropertyLeaseOrder> _expiredLeaseOrders = new();
	private readonly List<IPropertyKey> _propertyKeys = new();

	public override string FrameworkItemType => "Property";

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.Properties.Find(Id);
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.ApplyCriminalCodeInProperty = ApplyCriminalCodeInProperty;
		dbitem.DetailedDescription = DetailedDescription;
		dbitem.Name = _name;
		dbitem.SaleOrderId = SaleOrder?.Id;
		dbitem.LeaseOrderId = LeaseOrder?.Id;
		dbitem.LeaseId = Lease?.Id;
		dbitem.LastChangeOfOwnership = LastChangeOfOwnership.GetDateTimeString();
		dbitem.LastSaleValue = LastSaleValue;
		FMDB.Context.PropertyLocations.RemoveRange(dbitem.PropertyLocations);
		foreach (var cell in _propertyLocations)
		{
			dbitem.PropertyLocations.Add(new PropertyLocation { CellId = cell.Id, PropertyId = _id });
		}

		Changed = false;
	}

	#endregion

	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this command:

	name <name> - renames this property
	desc - drops you into an editor to change the detailed description
	room - toggles whether your current room is a part of the property
	zone <economic zone> - changes the economic zone of this property
	zone <economic zone> <exchange rate> - changes the economic zone of this property**
	lawful - toggles the application of crimcode in the property

** This version is necessary if the currencies are different between the two economic zones";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
			case "detail":
			case "detailed":
			case "detaileddescription":
			case "detaileddesc":
				return BuildingCommandDetailedDescription(actor);
			case "cell":
			case "room":
			case "location":
			case "add":
			case "addcell":
			case "here":
			case "addlocation":
				return BuildingCommandAddCell(actor);
			case "zone":
			case "economiczone":
			case "economyzone":
			case "ez":
				return BuildingCommandEconomicZone(actor, command);
			case "crim":
			case "crimcode":
			case "criminal":
			case "legal":
			case "lawful":
				return BuildingCommandLawful(actor);
			default:
				actor.OutputHandler.Send(HelpText);
				return false;
		}
	}

	private void DescPost(string text, IOutputHandler handler, object[] objects)
	{
		DetailedDescription = text.Trim().ProperSentences();
		Changed = true;
		handler.Send(
			$"You change the detailed description of the {Name.ColourName()} property to:\n\n{DetailedDescription.Wrap((int)objects[0])}");
	}

	private void DescCancel(IOutputHandler handler, object[] objects)
	{
		handler.Send($"You decide not to update the detailed description of the {Name.ColourName()} property.");
	}

	private bool BuildingCommandDetailedDescription(ICharacter actor)
	{
		if (!string.IsNullOrEmpty(DetailedDescription))
		{
			actor.OutputHandler.Send("Replacing:\n\n" + DetailedDescription.Wrap(actor.InnerLineFormatLength, "\t") +
			                         "\n");
		}

		actor.OutputHandler.Send("Enter the description in the editor below.");
		actor.EditorMode(DescPost, DescCancel, 1.0, suppliedArguments: new object[] { actor.InnerLineFormatLength });
		return true;
	}

	private bool BuildingCommandLawful(ICharacter actor)
	{
		ApplyCriminalCodeInProperty = !ApplyCriminalCodeInProperty;
		Changed = true;
		actor.OutputHandler.Send(
			$"This property will {(ApplyCriminalCodeInProperty ? "now" : "no longer")} apply criminal code to actions in the property.");
		return true;
	}

	private bool BuildingCommandEconomicZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to change this property to be a part of?");
			return false;
		}

		var zone = Gameworld.EconomicZones.GetByIdOrName(command.PopSpeech());
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return false;
		}

		if (zone.Currency == EconomicZone.Currency)
		{
			EconomicZone = zone;
			Changed = true;
			actor.OutputHandler.Send(
				$"The property {Name.ColourName()} is now based in the {EconomicZone.Name.ColourValue()} economic zone.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"The current economic zone for the {Name.ColourName()} property currently uses the {EconomicZone.Currency.Name.ColourValue()} currency, whereas the new economic zone uses {zone.Currency.Name.ColourValue()}. You must supply a multiplier value to convert all financial figures between these two currencies.");
			return false;
		}

		if (!decimal.TryParse(command.SafeRemainingArgument, out var multiplier) || multiplier <= 0.0M)
		{
			actor.OutputHandler.Send("That is not a valid multiplier for the currency conversion.");
			return false;
		}


		Changed = true;
		_lastSaleValue *= multiplier;
		if (_lease != null)
		{
			_lease.PaymentBalance *= multiplier;
			_lease.BondPayment *= multiplier;
			_lease.PricePerInterval *= multiplier;
		}

		if (_leaseOrder != null)
		{
			_leaseOrder.BondRequired *= multiplier;
			_leaseOrder.PricePerInterval *= multiplier;
		}

		foreach (var order in ExpiredLeaseOrders)
		{
			order.BondRequired *= multiplier;
			order.PricePerInterval *= multiplier;
		}

		if (_saleOrder != null)
		{
			_saleOrder.ReservePrice *= multiplier;
		}

		actor.OutputHandler.Send(
			$"The property {Name.ColourName()} is now based in the {zone.Name.ColourValue()} economic zone, and all prices have been converted from {EconomicZone.Currency.Name.ColourValue()} to {zone.Currency.Name.ColourValue()} with a conversion rate of {multiplier.ToString("N", actor)}.");
		EconomicZone = zone;
		return true;
	}

	private bool BuildingCommandAddCell(ICharacter actor)
	{
		if (PropertyLocations.Contains(actor.Location))
		{
			if (_propertyLocations.Count == 1)
			{
				actor.OutputHandler.Send(
					"You cannot remove the last location from a property. You must first give it another property cell.");
				return false;
			}

			_propertyLocations.Remove(actor.Location);
			Changed = true;
			actor.OutputHandler.Send(
				$"Your current location is no longer a part of the {Name.ColourName()} property.");
			return true;
		}

		var existing = Gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(actor.Location));
		if (existing != null)
		{
			actor.OutputHandler.Send(
				$"Your current location is already a part of the {existing.Name.ColourName()} property. Only one property can own each location.");
			return false;
		}

		_propertyLocations.Add(actor.Location);
		Changed = true;
		actor.OutputHandler.Send($"Your current location is now a part of the {Name.ColourName()} property.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this property?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Properties.Any(x => x.EconomicZone == EconomicZone && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a property called {name.ColourName()} in the {EconomicZone.Name.ColourValue()} economic zone. Names must be unique per zone.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the property {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"#{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor.LineFormatLength,
			actor.Account.UseUnicode, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourName()}");
		sb.AppendLine(
			$"Last Change of Ownership: {LastChangeOfOwnership.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine(
			$"Last Sale Value: {EconomicZone.Currency.Describe(_lastSaleValue, CurrencyDescriptionPatternType.Short).ColourValue()}");
		sb.AppendLine($"Apply Crimcode in Property: {ApplyCriminalCodeInProperty.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Detailed Description:");
		sb.AppendLine();
		sb.AppendLine(DetailedDescription.Wrap(actor.InnerLineFormatLength, "  "));
		sb.AppendLine();
		sb.AppendLine("Locations:");
		foreach (var cell in PropertyLocations)
		{
			sb.AppendLine(
				$"#{cell.Id.ToString("N0", actor)}) {cell.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLayers)}");
		}

		sb.AppendLine();
		sb.AppendLine($"Owners:");
		foreach (var owner in PropertyOwners)
		{
			sb.AppendLine($"\t{owner.Describe(actor)}");
		}

		sb.AppendLine();
		if (SaleOrder == null)
		{
			sb.AppendLine($"Sale Order: {"None".Colour(Telnet.Red)}");
		}
		else
		{
			switch (SaleOrder.OrderStatus)
			{
				case PropertySaleOrderStatus.Proposed:
					sb.AppendLine(
						$"Sale Order: {$"Proposed to sell for {EconomicZone.Currency.Describe(SaleOrder.ReservePrice, CurrencyDescriptionPatternType.Short).ColourValue()}".ColourIncludingReset(Telnet.BoldOrange)} ({SaleOrder.PropertyOwnerConsent.Count(x => x.Value).ToString("N0", actor)}/{PropertyOwners.Count().ToString("N0", actor)} approve)");
					break;
				case PropertySaleOrderStatus.Approved:
					sb.AppendLine(
						$"Sale Order: {$"Selling for {EconomicZone.Currency.Describe(SaleOrder.ReservePrice, CurrencyDescriptionPatternType.Short).ColourValue()}".ColourIncludingReset(Telnet.BoldCyan)}");
					break;
				default:
					sb.AppendLine($"Sale Order: {"None".Colour(Telnet.Red)}");
					break;
			}
		}

		if (LeaseOrder == null)
		{
			sb.AppendLine($"Lease Order: {"None".Colour(Telnet.Red)}");
		}
		else
		{
			sb.AppendLine(
				$"Lease Order: {$"{EconomicZone.Currency.Describe(LeaseOrder.PricePerInterval, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} {LeaseOrder.Interval.Describe(EconomicZone.FinancialPeriodReferenceCalendar)} ({LeaseOrder.MinimumLeaseDuration.Describe(actor).ColourValue()} to {LeaseOrder.MaximumLeaseDuration.Describe(actor).ColourValue()}) with {EconomicZone.Currency.Describe(LeaseOrder.BondRequired, CurrencyDescriptionPatternType.Short).ColourValue()} bond".ColourIncludingReset(Telnet.BoldWhite)}");
		}

		if (Lease == null)
		{
			sb.AppendLine($"Lease: {"None".Colour(Telnet.Red)}");
		}
		else
		{
			sb.AppendLine(
				$"Lease: {$"{EconomicZone.Currency.Describe(Lease.PricePerInterval, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} {Lease.Interval.Describe(EconomicZone.FinancialPeriodReferenceCalendar)} until {Lease.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()} with balance of {EconomicZone.Currency.Describe(Lease.PaymentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}".ColourIncludingReset(Telnet.BoldWhite)}");
		}

		if (PropertyKeys.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Keys:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from key in PropertyKeys
				select new List<string>
				{
					key.Name,
					key.GameItem.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee),
					key.AddedToPropertyOnDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
					EconomicZone.Currency.Describe(key.CostToReplace, CurrencyDescriptionPatternType.ShortDecimal),
					key.IsReturned.ToColouredString()
				},
				new List<string>
				{
					"Name",
					"Item",
					"Added",
					"Cost",
					"Returned?"
				},
				actor,
				Telnet.Green));
		}

		return sb.ToString();
	}

	#endregion

	#region Implementation of IProperty

	public IEnumerable<IShop> PropertyShops => PropertyLocations.SelectNotNull(x => x.Shop).Distinct().ToList();
	public IEnumerable<IPropertyLeaseOrder> ExpiredLeaseOrders => _expiredLeaseOrders;
	public IEnumerable<IPropertyLease> ExpiredLeases => _expiredLeases;
	public IEnumerable<IPropertyKey> PropertyKeys => _propertyKeys;

	public void ClaimShops(ICharacter who)
	{
		foreach (var shop in PropertyShops)
		{
			shop.ClearEmployees();
			shop.AddEmployee(who);
			shop.SetProprietor(who, true);
		}
	}

	public void AddKey(IPropertyKey key)
	{
		_propertyKeys.Add(key);
		Changed = true;
	}

	public void RemoveKey(IPropertyKey key)
	{
		_propertyKeys.Remove(key);
		Changed = true;
	}

	public void ExpireLease(IPropertyLease lease)
	{
		if (Lease == lease)
		{
			Lease = null;
		}

		if (!_expiredLeases.Contains(lease))
		{
			_expiredLeases.Add(lease);
		}

		foreach (var shop in PropertyLocations.SelectNotNull(x => x.Shop).Distinct())
		{
			shop.ClearEmployees();
		}

		Changed = true;
	}

	public void ExpireLeaseOrder(IPropertyLeaseOrder order)
	{
		if (LeaseOrder == order)
		{
			LeaseOrder = null;
		}

		if (!_expiredLeaseOrders.Contains(order))
		{
			_expiredLeaseOrders.Add(order);
		}

		Changed = true;
	}

	public IEconomicZone EconomicZone
	{
		get => _economicZone;
		set
		{
			_economicZone = value;
			Changed = true;
		}
	}

	public IEnumerable<IPropertyOwner> PropertyOwners => _propertyOwners;

	public IEnumerable<ICell> PropertyLocations => _propertyLocations;

	public string DetailedDescription
	{
		get => _detailedDescription;
		set
		{
			_detailedDescription = value;
			Changed = true;
		}
	}

	public MudDateTime LastChangeOfOwnership
	{
		get => _lastChangeOfOwnership;
		set
		{
			_lastChangeOfOwnership = value;
			Changed = true;
		}
	}

	public decimal LastSaleValue
	{
		get => _lastSaleValue;
		set
		{
			_lastSaleValue = value;
			Changed = true;
		}
	}

	public IPropertySaleOrder SaleOrder
	{
		get => _saleOrder;
		set
		{
			_saleOrder = value;
			Changed = true;
		}
	}

	public IPropertyLeaseOrder LeaseOrder
	{
		get => _leaseOrder;
		set
		{
			_leaseOrder = value;
			Changed = true;
		}
	}

	public IPropertyLease Lease
	{
		get => _lease;
		set
		{
			_lease = value;
			Changed = true;
		}
	}

	public bool ApplyCriminalCodeInProperty
	{
		get => _applyCriminalCodeInProperty;
		set
		{
			_applyCriminalCodeInProperty = value;
			Changed = true;
		}
	}

	public string PreviewProperty(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine(Name.GetLineWithTitle(voyeur.LineFormatLength, voyeur.Account.UseUnicode, Telnet.Cyan,
			Telnet.BoldWhite));
		sb.AppendLine();
		if (!string.IsNullOrEmpty(DetailedDescription))
		{
			sb.AppendLine(DetailedDescription);
			sb.AppendLine();
		}

		sb.AppendLine(
			$"A {_propertyLocations.Count.ToString("N0", voyeur).ColourValue()} room property in {EconomicZone.Name.ColourName()}:");
		foreach (var cell in PropertyLocations.OrderByDescending(x =>
			         x.ExitsFor(voyeur, true).Any(y => !PropertyLocations.Contains(y.Destination))))
		{
			if (voyeur.IsAdministrator())
			{
				sb.AppendLine(
					$"\t#{cell.Id.ToString("N0", voyeur)}) {cell.HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLayers)}");
			}
			else
			{
				sb.AppendLine(
					$"\t{cell.HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLayers)}");
			}
		}

		sb.AppendLine();
		sb.AppendLine(
			$"Last Sold {LastChangeOfOwnership.GetByTimeZone(voyeur.Location.TimeZone(EconomicZone.FinancialPeriodReferenceClock)).ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()} for {EconomicZone.Currency.Describe(LastSaleValue, CurrencyDescriptionPatternType.Short).ColourValue()}");
		var owner = voyeur.IsAdministrator() || _propertyOwners.Any(x =>
			x.Owner == voyeur || (x.Owner is IClan clan && voyeur.ClanMemberships.Any(y =>
				y.Clan == clan && y.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty))));

		if (owner && _propertyKeys.Any())
		{
			sb.AppendLine("\nKeys:");
			foreach (var key in _propertyKeys)
			{
				sb.AppendLine(
					$"\t[{key.Name.ColourName()}] {key.GameItem.HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee)} - {EconomicZone.Currency.Describe(key.CostToReplace, CurrencyDescriptionPatternType.Short).ColourValue()} - {(key.IsReturned ? "[returned]" : "[not returned]")}");
			}
		}

		if (owner)
		{
			sb.AppendLine("\nOwners:");
			foreach (var powner in PropertyOwners)
			{
				sb.AppendLine(
					$"\t{powner.Describe(voyeur)} [{powner.ShareOfOwnership.ToString("P", voyeur).ColourValue()}]");
			}
		}

		if (SaleOrder != null)
		{
			if (SaleOrder.ShowForSale)
			{
				sb.AppendLine(
					$"\nProperty is for sale for a price of {EconomicZone.Currency.Describe(SaleOrder.ReservePrice, CurrencyDescriptionPatternType.Short).ColourValue()}.");
			}
			else if (owner)
			{
				if (SaleOrder.PropertyOwnerConsent.All(x => x.Value))
				{
					sb.AppendLine(
						$"\nProperty will go up for sale on {SaleOrder.StartOfListing.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				}
				else
				{
					sb.AppendLine(
						$"\nProperty is proposed to be listed for sale for a price of {EconomicZone.Currency.Describe(SaleOrder.ReservePrice, CurrencyDescriptionPatternType.Short).ColourValue()}.");
					if (SaleOrder.StartOfListing > EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime)
					{
						sb.AppendLine(
							$"Property will be listed from {SaleOrder.StartOfListing.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
					}

					var missing = SaleOrder.PropertyOwnerConsent.Where(x => !x.Value).Select(x => x.Key).ToList();
					sb.AppendLine(
						$"Consent still required from {missing.Select(x => x.Describe(voyeur)).ListToString()}");
				}
			}
		}

		if (LeaseOrder != null)
		{
			if (LeaseOrder.ListedForLease)
			{
				sb.AppendLine(
					$"\nProperty is available for lease at {EconomicZone.Currency.Describe(LeaseOrder.PricePerInterval, CurrencyDescriptionPatternType.Short).ColourValue()} {LeaseOrder.Interval.Describe(EconomicZone.FinancialPeriodReferenceCalendar)} with a bond of {EconomicZone.Currency.Describe(LeaseOrder.BondRequired, CurrencyDescriptionPatternType.Short).ColourValue()}");
				sb.AppendLine(
					$"Term is {LeaseOrder.MinimumLeaseDuration.Describe(voyeur).ColourValue()} to {LeaseOrder.MaximumLeaseDuration.Describe(voyeur).ColourValue()}");
			}
			else if (owner)
			{
				sb.AppendLine(
					$"\nProperty is proposed to be leased at {EconomicZone.Currency.Describe(LeaseOrder.PricePerInterval, CurrencyDescriptionPatternType.Short).ColourValue()} {LeaseOrder.Interval.Describe(EconomicZone.FinancialPeriodReferenceCalendar)} with a bond of {EconomicZone.Currency.Describe(LeaseOrder.BondRequired, CurrencyDescriptionPatternType.Short).ColourValue()}");
				sb.AppendLine(
					$"Term is {LeaseOrder.MinimumLeaseDuration.Describe(voyeur).ColourValue()} to {LeaseOrder.MaximumLeaseDuration.Describe(voyeur).ColourValue()}");
				sb.AppendLine(
					$"{(LeaseOrder.AllowLeaseNovation ? "Allows" : "Doesn't allow")} novation, and {(LeaseOrder.AllowAutoRenew ? "allows" : "doesn't allow")} auto renewing.");
				if (LeaseOrder.AutomaticallyRelistAfterLeaseTerm)
				{
					sb.AppendLine(
						$"Automatically relists at end of term with a {LeaseOrder.FeeIncreasePercentageAfterLeaseTerm.ToString("P2", voyeur).ColourValue()} increase to all prices.");
				}

				if (LeaseOrder.CanLeaseProgCharacter != null)
				{
					sb.AppendLine(
						$"Uses the prog {LeaseOrder.CanLeaseProgCharacter.MXPClickableFunctionName()} to filter characters.");
				}

				if (LeaseOrder.CanLeaseProgClan != null)
				{
					sb.AppendLine(
						$"Uses the prog {LeaseOrder.CanLeaseProgClan.MXPClickableFunctionName()} to filter clans.");
				}

				if (LeaseOrder.PropertyOwnerConsent.Any(x => !x.Value))
				{
					var missing = LeaseOrder.PropertyOwnerConsent.Where(x => !x.Value).Select(x => x.Key).ToList();
					sb.AppendLine(
						$"Consent still required from {missing.Select(x => x.Describe(voyeur)).ListToString()}");
				}
			}
		}

		if (Lease != null)
		{
			var leaseCh = Lease.Leaseholder as ICharacter;
			var leaseClan = Lease.Leaseholder as IClan;
			if (owner || leaseCh == voyeur || voyeur.ClanMemberships.Any(x =>
				    x.Clan == leaseClan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)))
			{
				if (leaseCh != null)
				{
					sb.AppendLine(
						$"Currently leased to {leaseCh.PersonalName.GetName(NameStyle.FullName).ColourCharacter()} until {Lease.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				}
				else if (leaseClan != null)
				{
					sb.AppendLine(
						$"Currently leased to {leaseClan.FullName.ColourValue()} until {Lease.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				}
				else
				{
					sb.AppendLine(
						$"Currently leased to parties unknown until {Lease.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				}

				sb.AppendLine(
					$"Pays {EconomicZone.Currency.Describe(Lease.PricePerInterval, CurrencyDescriptionPatternType.Short).ColourValue()} {Lease.Interval.Describe(EconomicZone.FinancialPeriodReferenceCalendar)}.");
				sb.AppendLine(
					$"Holding {EconomicZone.Currency.Describe(Lease.BondPayment, CurrencyDescriptionPatternType.Short).ColourValue()} in bond.");
				sb.AppendLine(
					$"Current balance is {EconomicZone.Currency.Describe(Lease.PaymentBalance, CurrencyDescriptionPatternType.Short).ColourValue()}, last paid {Lease.LastLeasePayment.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				if (Lease.AutoRenew)
				{
					sb.AppendLine($"Will automatically renew at the end of the term.");
				}
			}
		}

		foreach (var shop in PropertyShops)
		{
			sb.AppendLine($"It comes with the shop {shop.Name.ColourName()}.");
		}

		return sb.ToString();
	}

	public void SellProperty(IFrameworkItem newOwner)
	{
		var so = SaleOrder;
		SaleOrder = null;
		LastChangeOfOwnership = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		LastSaleValue = so!.ReservePrice;
		foreach (var owner in PropertyOwners)
		{
			owner.Delete();
		}

		_propertyOwners.Clear();
		_propertyOwners.Add(new PropertyOwner(newOwner, 1.0M, null, this));
		so!.Delete();
		LeaseOrder?.ChangeConsentDueToSale(_propertyOwners[0]);
		if (Lease is null)
		{
			foreach (var shop in PropertyLocations.SelectNotNull(x => x.Shop).Distinct())
			{
				shop.ClearEmployees();
			}
		}
	}

	public void DivestOwnership(IPropertyOwner owner, decimal percentage, IFrameworkItem newOwnerItem)
	{
		var newPropertyOwner = _propertyOwners.FirstOrDefault(x => x.Owner == newOwnerItem);

		if (newPropertyOwner == null)
		{
			newPropertyOwner = new PropertyOwner(newOwnerItem, owner.ShareOfOwnership * Math.Min(1.0M, percentage),
				null, this);
			_propertyOwners.Add(newPropertyOwner);
		}
		else
		{
			newPropertyOwner.ShareOfOwnership += owner.ShareOfOwnership * percentage;
		}

		if (percentage >= 1.0M)
		{
			owner.Delete();
		}
		else
		{
			owner.ShareOfOwnership *= 1.0M - percentage;
		}

		LeaseOrder?.ResetConsent();
		SaleOrder?.ResetConsent();
	}

	public bool IsAuthorisedOwner(ICharacter who)
	{
		return PropertyOwners.Any(x => x.Owner == who || (x.Owner is IClan clan && who.ClanMemberships.Any(y =>
			y.Clan == clan && y.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty))));
	}

	public bool IsAuthorisedLeaseHolder(ICharacter who)
	{
		return Lease?.Leaseholder == who || (Lease?.Leaseholder is IClan clan && who.ClanMemberships.Any(x =>
			x.Clan == clan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)));
	}

	public bool HasUnclaimedBondPayments(ICharacter who)
	{
		return ExpiredLeases.Any(x => !x.BondReturned && (x.Leaseholder == who || (x.Leaseholder is IClan clan &&
			who.ClanMemberships.Any(y =>
				y.Clan == clan && y.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)))));
	}

	#endregion

	#region Implementation of IKeyworded

	public IEnumerable<string> Keywords => new ExplodedString(Name).Words;

	#endregion
}
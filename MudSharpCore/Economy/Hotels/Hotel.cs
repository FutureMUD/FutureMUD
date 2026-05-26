using System.Collections.Generic;
using System.Linq;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Property;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Hotels;

public sealed class Hotel : FrameworkItem, IHotel
{
	private IEmploymentHostState? _employment;

	public Hotel(IProperty property)
	{
		Property = property;
		_id = property.Id;
		_name = $"{property.Name} Hotel";
	}

	internal Hotel(IProperty property, long hotelId)
	{
		Property = property;
		_id = hotelId;
		_name = $"{property.Name} Hotel";
	}

	public override string FrameworkItemType => "Hotel";
	public IProperty Property { get; }
	public IEconomicZone EconomicZone => Property.EconomicZone;
	public ICurrency Currency => EconomicZone.Currency;
	public IBankAccount? BankAccount => Property.HotelBankAccount;
	public decimal CashBalance => Property.HotelCashBalance;
	public decimal AvailableFunds => Property.HotelAvailableFunds;
	public IEnumerable<ICell> Locations => Property.PropertyLocations;
	public IEnumerable<IHotelRoom> Rooms => Property.HotelRooms;
	public bool IsApprovedHotel => Property.IsApprovedHotel;
	public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Hotel;
	public IMarket? Market => null;

	public bool CanAccessHotelLocation(ICell cell)
	{
		return cell is not null && Property.PropertyLocations.Contains(cell);
	}
}

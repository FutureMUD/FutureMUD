using System.Collections.Generic;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Property;

#nullable enable

namespace MudSharp.Economy;

public interface IHotel : IEmploymentHost
{
	IProperty Property { get; }
	IEconomicZone EconomicZone { get; }
	ICurrency Currency { get; }
	IBankAccount? BankAccount { get; }
	decimal CashBalance { get; }
	decimal AvailableFunds { get; }
	IEnumerable<IRoom> Locations { get; }
	IEnumerable<IHotelRoom> Rooms { get; }
	bool IsApprovedHotel { get; }
	bool CanAccessHotelLocation(IRoom room);
}

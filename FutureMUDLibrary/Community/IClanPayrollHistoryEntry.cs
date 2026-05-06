using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

namespace MudSharp.Community;

public enum ClanPayrollHistoryType
{
	PayAccrued = 0,
	ManualAdjustment = 1,
	PayCollected = 2
}

public interface IClanPayrollHistoryEntry : IFrameworkItem
{
	IClan Clan { get; }
	ICharacter Character { get; }
	IRank Rank { get; }
	IPaygrade Paygrade { get; }
	IAppointment Appointment { get; }
	ICharacter Actor { get; }
	ICurrency Currency { get; }
	decimal Amount { get; }
	ClanPayrollHistoryType EntryType { get; }
	MudDateTime DateTime { get; }
	string Description { get; }
}

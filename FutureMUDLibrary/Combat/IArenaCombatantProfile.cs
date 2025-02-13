using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Combat;

#nullable enable
public interface IArenaCombatantProfile : IFrameworkItem, ISaveable
{
	bool IsArchived { get; }
	bool IsPC { get; }
	IPersonalName CombatantName { get; }
	ICharacter Character { get; }
	IArenaCombatantType ArenaCombatantType { get; }
	IBankAccount? BankAccount { get; }
	decimal UnclaimedPrizeMoney { get; }
}

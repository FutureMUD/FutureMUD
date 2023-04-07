#nullable enable
using MudSharp.Character;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy;

public interface IBankManagerAuditLog : ILateInitialisingItem
{
	IBank Bank { get; }
	MudDateTime DateTime { get;}
	string Detail { get; }
	ICharacter Character { get; }
}
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;

namespace MudSharp.Combat;

public interface IArenaMatchType : IFrameworkItem, ISaveable, IEditableItem
{
	Counter<IArenaCombatantType> CombatantCountsTeam1 { get; }
	Counter<IArenaCombatantType> CombatantCountsTeam2 { get; }
	Counter<IArenaCombatantType> CombatantCountsTeam3 { get; }
	Counter<IArenaCombatantType> CombatantCountsTeam4 { get; }
	IFutureProg? CombatantEligilityProg { get; }
	IFutureProg? OddsProg { get; }
	IFutureProg? OnMatchBeginsProg { get; }
	IFutureProg? OnMatchEndsProg { get; }
	bool CombatantsCanSurrender { get; }
	bool NPCsJoinBeforeRegistrationClosed { get; }
	bool FillWithNPCsIfEmpty { get; }
	int NumberOfRounds { get; }
	TimeSpan RoundTime { get; }
	TimeSpan IntervalTime { get; }
	ArenaMatchWinType WinType { get; }
}

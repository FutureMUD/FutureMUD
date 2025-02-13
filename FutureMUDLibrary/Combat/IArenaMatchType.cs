using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;

namespace MudSharp.Combat;

#nullable enable
public interface IArenaMatchType : IFrameworkItem, ISaveable, IEditableItem
{
	Counter<IArenaCombatantType> CombatantCountsTeam1 { get; }
	Counter<IArenaCombatantType> CombatantCountsTeam2 { get; }
	Counter<IArenaCombatantType> CombatantCountsTeam3 { get; }
	Counter<IArenaCombatantType> CombatantCountsTeam4 { get; }
	IFutureProg? CombatantEligibilityProg { get; }
	IFutureProg? OddsProg { get; }
	IFutureProg? OnMatchBeginsProg { get; }
	IFutureProg? OnMatchEndsProg { get; }
	IFutureProg MatchCanBeginProg { get; }
	IFutureProg? OutfitCombatantProg { get; }
	IFutureProg? OnMatchCreatedProg { get; }
	bool CombatantsCanSurrender { get; }
	bool NPCsJoinBeforeRegistrationClosed { get; }
	bool FillWithNPCsIfEmpty { get; }
	int NumberOfRounds { get; }
	TimeSpan RoundTime { get; }
	TimeSpan SignUpTime { get; }
	TimeSpan PrepareTime { get; }
	ArenaMatchWinType WinType { get; }
	double RelativePriorityForMatchType { get; }
	bool PermitOwnWeapons { get; }
	bool PermitOwnArmour { get; }
	bool PermitOwnClothes { get; }
	IArenaMatch CreateMatch(ICombatArena arena);
}

using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.NPC.Templates;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat;
#nullable enable

public interface ICombatArena : IFrameworkItem, ISaveable, IEditableItem
{
	IEnumerable<ICell> ArenaCells { get; }
	IEnumerable<ICell> StagingCells { get; }
	IEnumerable<ICell> StableCells { get; }
	IEnumerable<ICell> SpectatorCells { get; }
	IEnumerable<IArenaCombatantType> ArenaCombatantTypes { get; }
	IEnumerable<IArenaCombatantProfile> ArenaCombatantProfiles { get; }
	IEnumerable<IArenaMatchType> ArenaMatchTypes { get; }
	ICurrency? Currency { get; }
	decimal CashBalance { get; }
	IBankAccount? BankAccount { get; }
	bool IsManager(ICharacter actor);
}

public interface IArenaMatchBet : IFrameworkItem, ISaveable
{
	decimal BetAmount { get; }
	decimal Odds { get; }
	bool Won { get; }
	bool CashedOut { get; }
	IArenaMatch Match { get; }
	int Team { get; }
}

public enum ArenaMatchStage
{
	OpenForRegistration,
	PreparingMatch,
	MatchUnderway,
	MatchFinished
}

public enum ArenaMatchWinType
{
	Points,
	Grappled,
	KnockedOver,
	FirstBlood,
	Unconscious,
	Death
}

public interface IArenaMatch : IFrameworkItem, ISaveable
{
	IArenaMatchType MatchType { get; }
	ArenaMatchStage Stage { get; }
	DateTime MatchStart { get; }
	MudDateTime MatchStartMDT { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam1 { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam2 { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam3 { get; }
	IEnumerable<IArenaCombatantProfile> CombatantsTeam4 { get; }
	IEnumerable<IArenaMatchBet> Bets { get; }
	int CurrentRound { get; }
	DateTime? CurrentRoundEndTime { get; }
	DateTime? CurrentIntervalEndTime { get; }
}

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

public interface IArenaCombatantType : IFrameworkItem, ISaveable, IEditableItem
{
	int MinimumNumberOfNPCs { get; }
	int MaximumNumberOfPCs { get; }
	INPCTemplate? NPCTemplate { get; }
	IFutureProg PlayerQualificationProg { get; }
	IFutureProg? OnCombatantJoinsTypeProg { get; }
	IFutureProg? OnCombatantWinProg { get; }
	IFutureProg? OnCombatantLoseProg { get; }
	IFutureProg? OnCombatantLeavesTypeProg { get; }
	INameCulture? StageNameCulture { get; }
	IEnumerable<IRandomNameProfile> StageNameRandomProfiles { get; }
	bool AutoHealNPCs { get; }
}

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

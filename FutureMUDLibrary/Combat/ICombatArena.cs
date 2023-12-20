using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Migrations;
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
	IEnumerable<IArenaMatch> ArenaMatches { get; }
	IEnumerable<IArenaMatchBet> ArenaMatchBets { get; }
	IArenaMatch? ActiveMatch { get; }
	IEconomicZone EconomicZone { get; }
	ICurrency Currency { get; }
	decimal CashBalance { get; }
	IBankAccount? BankAccount { get; }
	DateTime? LastArenaMatch { get; }
	TimeSpan TimeBetweenMatches { get; }
	bool IsManager(ICharacter actor);
}
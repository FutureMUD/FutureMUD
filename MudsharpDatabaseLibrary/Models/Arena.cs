using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class Arena
{
	public Arena()
	{
		ArenaManagers = new HashSet<ArenaManager>();
		ArenaCells = new HashSet<ArenaCell>();
		ArenaCombatantClasses = new HashSet<ArenaCombatantClass>();
		ArenaEventTypes = new HashSet<ArenaEventType>();
		ArenaEvents = new HashSet<ArenaEvent>();
		ArenaRatings = new HashSet<ArenaRating>();
		ArenaFinanceSnapshots = new HashSet<ArenaFinanceSnapshot>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public long EconomicZoneId { get; set; }
	public long CurrencyId { get; set; }
	public long? BankAccountId { get; set; }
	public long? OnArenaEventPhaseProgId { get; set; }
	public decimal VirtualBalance { get; set; }
	public DateTime CreatedAt { get; set; }
	public bool IsDeleted { get; set; }
	public string SignupEcho { get; set; }

	public virtual EconomicZone EconomicZone { get; set; }
	public virtual Currency Currency { get; set; }
	public virtual BankAccount BankAccount { get; set; }
	public virtual FutureProg OnArenaEventPhaseProg { get; set; }
	public virtual ICollection<ArenaManager> ArenaManagers { get; set; }
	public virtual ICollection<ArenaCell> ArenaCells { get; set; }
	public virtual ICollection<ArenaCombatantClass> ArenaCombatantClasses { get; set; }
	public virtual ICollection<ArenaEventType> ArenaEventTypes { get; set; }
	public virtual ICollection<ArenaEvent> ArenaEvents { get; set; }
	public virtual ICollection<ArenaRating> ArenaRatings { get; set; }
	public virtual ICollection<ArenaFinanceSnapshot> ArenaFinanceSnapshots { get; set; }
}

public class ArenaManager
{
	public long Id { get; set; }
	public long ArenaId { get; set; }
	public long CharacterId { get; set; }
	public DateTime CreatedAt { get; set; }

	public virtual Arena Arena { get; set; }
	public virtual Character Character { get; set; }
}

public class ArenaCell
{
	public long Id { get; set; }
	public long ArenaId { get; set; }
	public long CellId { get; set; }
	public int Role { get; set; }

	public virtual Arena Arena { get; set; }
	public virtual Cell Cell { get; set; }
}

public class ArenaRating
{
	public long Id { get; set; }
	public long ArenaId { get; set; }
	public long CharacterId { get; set; }
	public long CombatantClassId { get; set; }
	public decimal Rating { get; set; }
	public DateTime LastUpdatedAt { get; set; }

	public virtual Arena Arena { get; set; }
	public virtual Character Character { get; set; }
	public virtual ArenaCombatantClass CombatantClass { get; set; }
}

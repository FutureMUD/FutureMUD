using System;

namespace MudSharp.Models;

public class ArenaBet
{
	public long Id { get; set; }
	public long ArenaEventId { get; set; }
	public long CharacterId { get; set; }
	public int? SideIndex { get; set; }
	public decimal Stake { get; set; }
	public decimal? FixedDecimalOdds { get; set; }
	public string ModelSnapshot { get; set; }
	public bool IsCancelled { get; set; }
	public DateTime PlacedAt { get; set; }
	public DateTime? CancelledAt { get; set; }

	public virtual ArenaEvent ArenaEvent { get; set; }
	public virtual Character Character { get; set; }
}

public class ArenaBetPool
{
	public long Id { get; set; }
	public long ArenaEventId { get; set; }
	public int? SideIndex { get; set; }
	public decimal TotalStake { get; set; }
	public decimal TakeRate { get; set; }

	public virtual ArenaEvent ArenaEvent { get; set; }
}

public class ArenaBetPayout
{
	public long Id { get; set; }
	public long ArenaEventId { get; set; }
	public long CharacterId { get; set; }
	public decimal Amount { get; set; }
	public int PayoutType { get; set; }
	public bool IsBlocked { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? CollectedAt { get; set; }

	public virtual ArenaEvent ArenaEvent { get; set; }
	public virtual Character Character { get; set; }
}

public class ArenaFinanceSnapshot
{
	public long Id { get; set; }
	public long ArenaId { get; set; }
	public long? ArenaEventId { get; set; }
	public string Period { get; set; }
	public decimal Revenue { get; set; }
	public decimal Costs { get; set; }
	public decimal TaxWithheld { get; set; }
	public decimal Profit { get; set; }
	public DateTime CreatedAt { get; set; }

	public virtual Arena Arena { get; set; }
	public virtual ArenaEvent ArenaEvent { get; set; }
}

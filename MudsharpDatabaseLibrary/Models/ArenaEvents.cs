using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class ArenaEvent
{
	public ArenaEvent()
	{
		ArenaEventSides = new HashSet<ArenaEventSide>();
		ArenaReservations = new HashSet<ArenaReservation>();
		ArenaSignups = new HashSet<ArenaSignup>();
		ArenaEliminations = new HashSet<ArenaElimination>();
		ArenaBets = new HashSet<ArenaBet>();
		ArenaBetPools = new HashSet<ArenaBetPool>();
		ArenaBetPayouts = new HashSet<ArenaBetPayout>();
		ArenaFinanceSnapshots = new HashSet<ArenaFinanceSnapshot>();
	}

	public long Id { get; set; }
	public long ArenaId { get; set; }
	public long ArenaEventTypeId { get; set; }
	public int State { get; set; }
	public bool BringYourOwn { get; set; }
	public int RegistrationDurationSeconds { get; set; }
	public int PreparationDurationSeconds { get; set; }
	public int? TimeLimitSeconds { get; set; }
	public int BettingModel { get; set; }
	public decimal AppearanceFee { get; set; }
	public decimal VictoryFee { get; set; }
	public bool PayNpcAppearanceFee { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ScheduledAt { get; set; }
	public DateTime? RegistrationOpensAt { get; set; }
	public DateTime? StartedAt { get; set; }
	public DateTime? ResolvedAt { get; set; }
	public DateTime? CompletedAt { get; set; }
	public DateTime? AbortedAt { get; set; }
	public string CancellationReason { get; set; }

	public virtual Arena Arena { get; set; }
	public virtual ArenaEventType ArenaEventType { get; set; }
	public virtual ICollection<ArenaEventSide> ArenaEventSides { get; set; }
	public virtual ICollection<ArenaReservation> ArenaReservations { get; set; }
	public virtual ICollection<ArenaSignup> ArenaSignups { get; set; }
	public virtual ICollection<ArenaElimination> ArenaEliminations { get; set; }
	public virtual ICollection<ArenaBet> ArenaBets { get; set; }
	public virtual ICollection<ArenaBetPool> ArenaBetPools { get; set; }
	public virtual ICollection<ArenaBetPayout> ArenaBetPayouts { get; set; }
	public virtual ICollection<ArenaFinanceSnapshot> ArenaFinanceSnapshots { get; set; }
}

public class ArenaEventSide
{
	public long Id { get; set; }
	public long ArenaEventId { get; set; }
	public int SideIndex { get; set; }
	public int Capacity { get; set; }
	public int Policy { get; set; }
	public bool AllowNpcSignup { get; set; }
	public bool AutoFillNpc { get; set; }
	public long? OutfitProgId { get; set; }
	public long? NpcLoaderProgId { get; set; }

	public virtual ArenaEvent ArenaEvent { get; set; }
	public virtual FutureProg OutfitProg { get; set; }
	public virtual FutureProg NpcLoaderProg { get; set; }
}

public class ArenaReservation
{
	public ArenaReservation()
	{
		ArenaSignups = new HashSet<ArenaSignup>();
	}

	public long Id { get; set; }
	public long ArenaEventId { get; set; }
	public int SideIndex { get; set; }
	public long? CharacterId { get; set; }
	public long? ClanId { get; set; }
	public DateTime ReservedAt { get; set; }
	public DateTime ExpiresAt { get; set; }

	public virtual ArenaEvent ArenaEvent { get; set; }
	public virtual Character Character { get; set; }
	public virtual Clan Clan { get; set; }
	public virtual ICollection<ArenaSignup> ArenaSignups { get; set; }
}

public class ArenaSignup
{
	public ArenaSignup()
	{
		ArenaEliminations = new HashSet<ArenaElimination>();
	}

	public long Id { get; set; }
	public long ArenaEventId { get; set; }
	public long CharacterId { get; set; }
	public long CombatantClassId { get; set; }
	public int SideIndex { get; set; }
	public bool IsNpc { get; set; }
	public string StageName { get; set; }
	public string SignatureColour { get; set; }
	public decimal? StartingRating { get; set; }
	public DateTime SignedUpAt { get; set; }
	public long? ArenaReservationId { get; set; }

	public virtual ArenaEvent ArenaEvent { get; set; }
	public virtual Character Character { get; set; }
	public virtual ArenaCombatantClass CombatantClass { get; set; }
	public virtual ArenaReservation ArenaReservation { get; set; }
	public virtual ICollection<ArenaElimination> ArenaEliminations { get; set; }
}

public class ArenaElimination
{
	public long Id { get; set; }
	public long ArenaEventId { get; set; }
	public long ArenaSignupId { get; set; }
	public long CharacterId { get; set; }
	public int Reason { get; set; }
	public DateTime OccurredAt { get; set; }

	public virtual ArenaEvent ArenaEvent { get; set; }
	public virtual ArenaSignup ArenaSignup { get; set; }
	public virtual Character Character { get; set; }
}

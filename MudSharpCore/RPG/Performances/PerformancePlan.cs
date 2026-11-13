#nullable enable

using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.RPG.Performances;

public enum PerformanceState { Draft, Booked, Gathering, Performing, Completed, Cancelled }

public sealed record CastMember(long Identity, long? Instance)
{
	public static CastMember From(ICharacter actor) =>
		new(CharacterInstanceIdentityComparer.IdentityId(actor), CharacterInstanceIdentityComparer.InstanceId(actor));
}

public sealed record PerformanceMark(long Room, RoomLayer Layer);

public sealed record PerformerReturn(CastMember Member, string Role, PerformanceMark Home, bool WalkBack);

// The outer room/layer and holder distinguish the original container from one moved elsewhere.
public sealed record PropReturn(string Key, long Item, PerformanceMark Origin, long? Container, CastMember? Lender)
{
	public CastMember? Custodian { get; set; }
	public CastMember? TransferTo { get; set; }
}

public sealed class PerformancePlan
{
	public long Id { get; set; }
	public long WorkId { get; set; }
	public long Owner { get; set; }
	public long BookedBy { get; set; }
	public bool BookedAsStaff { get; set; }
	public string Name { get; set; } = "";
	public EmploymentHostType? HostType { get; set; }
	public long HostId { get; set; }
	public Dictionary<string, PerformanceMark> Places { get; set; } = [];
	public Dictionary<string, long> Exits { get; set; } = [];
	public Dictionary<string, long> Props { get; set; } = [];
	public Dictionary<string, CastMember> Cast { get; set; } = [];
	public Dictionary<string, CastMember> Understudies { get; set; } = [];
	public Dictionary<string, List<CastMember>> Registrations { get; set; } = [];
	public Dictionary<string, CastMember> LiveCast { get; set; } = [];
	public List<CastMember> Withdrawn { get; set; } = [];
	public Dictionary<string,string> CurrentMarks { get; set; } = [];
	public Dictionary<string,string> PropHolders { get; set; } = [];
	public List<int> Sustained { get; set; } = [];
	public Guid RunId { get; set; }
	public PerformanceState State { get; set; }
	public int Cue { get; set; }
	public string Reason { get; set; } = "";
	public string LastOutcome { get; set; } = "";
	public string? ScheduledDate { get; set; }
	public long ClockRoomId { get; set; }
	public RecurringInterval? Repeat { get; set; }
	public long ConditionProgId { get; set; }
	public bool ConditionLatched { get; set; }
	public long StartInstant { get; set; }
	public long CallLeadSeconds { get; set; } = 600;
	public long GraceSeconds { get; set; } = 300;
	public long ReservedSeconds { get; set; } = 300;
	public int CueGraceSeconds { get; set; } = 120;
	public DateTime DueUtc { get; set; }
	public DateTime? FinishAfterUtc { get; set; }
	public bool Prompted { get; set; }
	public bool Yielded { get; set; }
	public Dictionary<string, long> Links { get; set; } = [];
	public List<PerformerReturn> PerformerReturns { get; set; } = [];
	public List<PropReturn> PropReturns { get; set; } = [];
	public DateTime? CleanupDueUtc { get; set; }
	public List<string> CleanupReport { get; set; } = [];
	[System.Text.Json.Serialization.JsonIgnore]
	public List<string> ReturnProgress { get; } = [];
	public bool Active => State is PerformanceState.Gathering or PerformanceState.Performing;
}

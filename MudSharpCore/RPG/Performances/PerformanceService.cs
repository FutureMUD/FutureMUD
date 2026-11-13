#nullable enable

using System.Runtime.CompilerServices;
using System.Text.Json;
using MudSharp.Accounts;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Employment;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using StagedPerformanceRecord = MudSharp.Models.StagedPerformanceRecord;
using MudSharp.Construction.Boundary;
using MudSharp.TimeAndDate;

namespace MudSharp.RPG.Performances;

public sealed partial class PerformanceService
{
	private static readonly ConditionalWeakTable<IFuturemud, PerformanceService> Services = new();
	private readonly IFuturemud _world;
	private readonly Dictionary<long, long> _selection = [];
	private readonly Dictionary<CastMember, ICharacter> _registeredBodies = [];
	private readonly Dictionary<long, DateTime> _nextConditionCheck = [];
	public List<PerformanceWork> Works { get; } = [];
	public List<PerformancePlan> Plans { get; } = [];
	public static PerformanceService For(IFuturemud world) => Services.GetValue(world, x => new(x));
	internal PerformanceService(IFuturemud world) => _world = world;

	public void Initialise()
	{
		using (new FMDB())
		{
			foreach (var row in FMDB.Context.StagedPerformanceRecords)
			{
				if (row.Kind == "work")
				{
					var work = JsonSerializer.Deserialize<PerformanceWork>(row.Definition)!;
					work.Id = row.Id;
					Works.Add(work);
				}
				else if (row.Kind == "plan")
				{
					var plan = JsonSerializer.Deserialize<PerformancePlan>(row.Definition)!;
					plan.Id = row.Id;
					Plans.Add(plan);
				}
			}
		}

		foreach (var plan in Plans.Where(x => x.Active))
		{
			End(plan, false, "Interrupted by server restart.", false);
		}
		StageLinks.ReconcileRunHandles(_world, new HashSet<string>());
		_world.HeartbeatManager.SecondHeartbeat += Tick;
	}

	private static long Persist<T>(string kind, long id, T value)
	{
		using (new FMDB())
		{
			var row = id == 0 ? new StagedPerformanceRecord { Kind = kind } :
				FMDB.Context.StagedPerformanceRecords.Find(id) ?? throw new InvalidOperationException("Missing performance record.");
			if (id == 0) FMDB.Context.StagedPerformanceRecords.Add(row);
			row.Definition = JsonSerializer.Serialize(value);
			FMDB.Context.SaveChanges();
			return row.Id;
		}
	}

	public void SaveWork(PerformanceWork work)
	{
		var isNew = work.Id == 0;
		work.Id = Persist("work", work.Id, work);
		if (isNew) Works.Add(work);
		else Works[Works.FindIndex(x => x.Id == work.Id)] = work;
	}

	private void Save(PerformancePlan plan)
	{
		var isNew = plan.Id == 0;
		plan.Id = Persist("plan", plan.Id, plan);
		if (isNew) Plans.Add(plan);
	}

	private PerformanceWork Work(PerformancePlan p) => Works.First(x => x.Id == p.WorkId);
	private ICharacter? Actor(CastMember? member)
	{
		if (member is null) return null;
		var primary = _world.Actors.Get(member.Identity);
		if (primary is null || member.Instance is null || primary.InstanceId == member.Instance) return primary;
		return primary.Identity.Instances.OfType<ICharacter>().FirstOrDefault(x => x.InstanceId == member.Instance);
	}
	private static long Identity(ICharacter a) => CharacterInstanceIdentityComparer.IdentityId(a);
	private IRoom? Room(PerformancePlan p, string key) => p.Places.TryGetValue(key, out var mark) ? _world.Rooms.Get(mark.Room) : null;
	private static bool At(PerformancePlan p, string key, ICharacter a) => p.Places.TryGetValue(key, out var mark) &&
		a.Location?.Id == mark.Room && a.RoomLayer == mark.Layer;
	private static bool Able(ICharacter actor) => actor.Location is not null && CharacterState.Able.HasFlag(actor.State) && actor.Combat is null;
	private static bool PresentPc(ICharacter a) => a.IsPlayerCharacter && Able(a) &&
		a.Identity.FocusedInstance?.InstanceId == a.InstanceId && !a.EffectsOfType<LinkdeadLogout>().Any() && a.Controller is not null;
	private IEmploymentHost? Host(PerformancePlan p) => p.HostType is null ? null :
		EmploymentHostDiscovery.LoadedHosts(_world).FirstOrDefault(x => x.EmploymentHostType == p.HostType && x.Id == p.HostId);
	private bool CanManage(ICharacter a, PerformancePlan p) => a.IsAdministrator() ||
		(p.HostType.HasValue ? Host(p)?.HasAuthority(a, EmploymentAuthority.AssignTasks) == true : p.Owner == Identity(a));
	private static bool Employs(IEmploymentHost? host, ICharacter npc) =>
		host?.EmploymentContracts.Any(x => x.Status == EmploymentStatus.Active && Identity(x.Employee) == Identity(npc)) == true;
	private bool CanUseNpc(ICharacter a, PerformancePlan p, ICharacter npc) => a.IsAdministrator() ||
		(Host(p) is { } host && host.HasAuthority(a, EmploymentAuthority.AssignTasks) && Employs(host,npc));
	private bool VenueAuthority(ICharacter a, PerformancePlan p, IRoom room, bool staff) => staff ||
		_world.Properties.Any(x => x.PropertyLocations.Contains(room) && (x.IsAuthorisedOwner(a) || x.IsAuthorisedLeaseHolder(a))) ||
		Host(p) switch
		{
			IClan clan => clan.ClanHallRooms.Contains(room) || clan.AdministrationRooms.Contains(room),
			IPermanentShop shop => shop.AllShopRooms.Contains(room),
			_ => false
		};

	internal string? AuthorityProblem(PerformancePlan p)
	{
		var organiser = _world.TryGetCharacter(p.BookedBy, true);
		if (organiser is null) return "The booking organiser is unavailable.";
		if (p.BookedAsStaff && organiser.PermissionLevel >= PermissionLevel.JuniorAdmin &&
		    organiser.Account.Authority.Level >= PermissionLevel.JuniorAdmin) return null;
		var host = Host(p);
		if (p.HostType.HasValue ? host?.HasAuthority(organiser, EmploymentAuthority.AssignTasks) != true :
		    p.Owner != Identity(organiser)) return "The organiser no longer has authority over the production.";
		foreach (var role in Work(p).Roles)
		{
			foreach (var member in Candidates(p,role))
				if (Actor(member) is { IsPlayerCharacter: false } npc && !Employs(host,npc))
					return $"The organiser no longer has authority over the NPC cast as {role.Key}.";
		}
		foreach (var key in Work(p).Cues.SelectMany(c => c.Verb switch
		         {
			         PerformanceVerb.Stage => new[] { c.Place, c.Target },
			         PerformanceVerb.Guard => new[] { c.Place },
			         _ => Array.Empty<string>()
		         }).Distinct())
			if (Room(p,key) is not { } room || !VenueAuthority(organiser,p,room,false))
				return $"The organiser no longer controls the venue at {key}.";
		foreach (var key in Work(p).Props)
			if (_world.Items.Get(p.Props.GetValueOrDefault(key)) is { HasOwner: true } item &&
			    item.Owner != organiser && item.Owner != host) return $"The organiser no longer controls {key}.";
		return null;
	}

	private bool Eligible(PerformanceRole role, ICharacter actor, bool allowPrivate)
	{
		if ((actor.IsPlayerCharacter && role.Mode == PerformanceMode.Npc) ||
		    (!actor.IsPlayerCharacter && role.Mode == PerformanceMode.Pc)) return false;
		if (role.EligibilityProgId == 0) return true;
		var prog = _world.FutureProgs.Get(role.EligibilityProgId);
		return PerformanceAuthoring.UsableRule(prog, ProgVariableTypes.Character, allowPrivate) && prog!.ExecuteBool(false, actor);
	}

	private bool StaffBooking(PerformancePlan p) => p.BookedAsStaff && Work(p).StaffAuthored &&
		_world.TryGetCharacter(p.BookedBy, true) is { PermissionLevel: >= PermissionLevel.JuniorAdmin,
			Account.Authority.Level: >= PermissionLevel.JuniorAdmin };

	private long Now(PerformancePlan p) => MudInstant.FromMudDateTime(_world.Rooms.Get(p.ClockRoomId)?.DateTime() ?? MudDateTime.Never).Ticks;
	private void WatchBody(ICharacter actor)
	{
		if (!_registeredBodies.TryAdd(CastMember.From(actor), actor)) return;
		actor.OnLocationChanged += ParticipantMoved;
	}

	private void ParticipantMoved(ILocateable owner, IRoomExit? exit)
	{
		if (owner is not ICharacter actor) return;
		var member = CastMember.From(actor);
		foreach (var p in Plans.Where(x => x.Active))
		{
			if (p.State == PerformanceState.Performing)
			{
				foreach (var (role, cast) in p.LiveCast.Where(x => x.Value == member).ToArray())
				{
					if (!At(p, p.CurrentMarks[role], actor) && !ExpectedMove(p, Work(p).Cues.ElementAtOrDefault(p.Cue), role, actor, exit))
					{
						LosePart(p, role, member);
						actor.OutputHandler.Send($"You leave the {role} part in {p.Name} when you depart your mark.");
					}
				}
				continue;
			}
			var removed = false;
			foreach (var role in Work(p).Roles.Where(x => !At(p, x.Mark, actor)))
				if (p.Registrations.TryGetValue(role.Key, out var list)) removed |= list.Remove(member);
			if (removed) Save(p);
		}
		UnwatchUnused();
	}

	internal static bool ExpectedMove(PerformancePlan p, PerformanceCue? cue, string role, ICharacter actor, IRoomExit? exit)
	{
		if (cue is not { Verb: PerformanceVerb.Travel } || cue.Role != role) return false;
		if (cue.Target.Length == 0) return true; // The performer may choose a path to this mark.
		return exit is not null && p.Exits.TryGetValue(cue.Target, out var id) && exit.Exit.Id == id &&
		       p.Places.TryGetValue(p.CurrentMarks[role], out var origin) && exit.Origin.Id == origin.Room &&
		       exit.Destination == actor.Location && At(p, cue.Place, actor);
	}

	private void UnwatchUnused()
	{
		foreach (var (member, actor) in _registeredBodies.ToArray())
		{
			if (Plans.Any(x => x.Active && x.Registrations.Values.Any(r => r.Contains(member)))) continue;
			actor.OnLocationChanged -= ParticipantMoved;
			_registeredBodies.Remove(member);
		}
	}
}

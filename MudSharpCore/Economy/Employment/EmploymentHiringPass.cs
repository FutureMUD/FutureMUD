using System.Runtime.CompilerServices;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.NPC;
using MudSharp.NPC.AI;

#nullable enable

namespace MudSharp.Economy.Employment;

internal sealed class EmploymentHiringPass
{
	private static readonly ConditionalWeakTable<IFuturemud, EmploymentHiringPass> Worlds = new();
	private readonly Dictionary<long, INPC> _dormant = new();
	private readonly HashSet<long> _unemployed = new();
	private readonly Dictionary<(long Worker, long Ai), DateTime> _lastSearch = new();

	public static EmploymentHiringPass For(IFuturemud world) => Worlds.GetOrCreateValue(world);

	public static bool IsKnownUnemployed(ICharacter worker) =>
		Worlds.TryGetValue(worker.Gameworld, out var pass) && pass._unemployed.Contains(worker.Id);

	public static void EmployeeHired(ICharacter worker)
	{
		if (worker.Gameworld is { } world && Worlds.TryGetValue(world, out var pass))
		{
			pass._unemployed.Remove(worker.Id);
		}
	}

	public static IEnumerable<INPC> DormantWorkers(IFuturemud world) =>
		Worlds.TryGetValue(world, out var pass) ? pass._dormant.Values : [];

	public static void TrackDormant(INPC worker) => For(worker.Gameworld)._dormant[worker.Id] = worker;

	public static void ForgetDormant(ICharacter worker)
	{
		if (worker.Gameworld is { } world && Worlds.TryGetValue(world, out var pass))
		{
			pass._dormant.Remove(worker.Id);
		}
	}

	private sealed record Vacancy(IJobOpening Opening, IRoom? Room, decimal Rate, int OverdueDays,
		bool OwesWages, DateTimeOffset HostTime)
	{
		public int Available { get; set; }
	}

	private sealed class Market
	{
		public HashSet<long> Employed { get; }
		public HashSet<long> Applicants { get; }
		public HashSet<(long, IJobOpening)> PreviousHires { get; }
		public Dictionary<(long, IJobOpening), DateTimeOffset> Rejections { get; }
		public LinkedList<Vacancy> Ranked { get; }

		public Market(IFuturemud world)
		{
			var hosts = EmploymentHostDiscovery.LoadedHosts(world).ToList();
			Employed = hosts.SelectMany(x => x.EmploymentContracts)
				.Where(x => x.Status is EmploymentStatus.Active or EmploymentStatus.Suspended)
				.Select(x => x.Employee.Id).ToHashSet();
			var applications = hosts.SelectMany(x => x.Employment.Applications).ToList();
			var pending = applications.Where(x => x.Status == JobApplicationStatus.Pending).ToList();
			Applicants = pending.Select(x => x.Candidate.Id).ToHashSet();
			var pendingByOpening = pending.Where(x => x.OfferedOpeningRevision == x.Opening.RevisionNumber)
				.ToLookup(x => x.Opening);
			Rejections = applications.Where(x => x.Status == JobApplicationStatus.Rejected)
				.GroupBy(x => (x.Candidate.Id, x.Opening))
				.ToDictionary(x => x.Key, x => x.Max(a => a.AppliedAt));
			PreviousHires = applications.Where(x => x.Status == JobApplicationStatus.Accepted)
				.Select(x => (x.Candidate.Id, x.Opening)).ToHashSet();
			var vacancies = new List<Vacancy>();
			foreach (var host in hosts)
			{
				var openings = host.JobOpenings.Where(x => x.Status == JobOpeningStatus.Open).ToList();
				if (openings.Count == 0) continue;
				var room = EmploymentWorkerAI.PrimaryWorkRoom(host);
				var owes = host.Payroll.OutstandingLiabilities.Any();
				var overdue = owes ? host.Payroll.MaximumOverdueDays() : 0;
				var hostTime = EmploymentClock.CurrentInstant(host);
				foreach (var opening in openings)
				{
					var available = opening.MaxPositions - opening.OccupiedPositions - pendingByOpening[opening].Count();
					if (available <= 0) continue;
					vacancies.Add(new Vacancy(opening, room,
						EmploymentCompensationEvaluator.EffectiveHourlyGlobalAmount(opening.Compensation),
						overdue, owes, hostTime) { Available = available });
				}
			}

			// Only ties on pay and arrears require comparing commute distances.
			Ranked = new LinkedList<Vacancy>(vacancies.OrderByDescending(x => x.Rate)
				.ThenBy(x => x.OverdueDays)
				.ThenBy(x => x.Opening.Employer.EmploymentHostName)
				.ThenBy(x => x.Opening.Id));
		}
	}

	public void Run(IFuturemud world)
	{
		var workers = world.NPCs.OfType<INPC>().Where(x => x.AIs.OfType<EmploymentWorkerAI>().Any())
			.Concat(_dormant.Values).ToList();
		_unemployed.Clear();
		if (workers.Count == 0)
		{
			_lastSearch.Clear();
			return;
		}

		var now = RuntimeClock.UtcNow;
		var market = new Market(world);
		foreach (var worker in workers)
		{
			try
			{
				EvaluateWorker(world, worker, market, now);
			}
			catch (Exception ex)
			{
				world.DebugMessage($"Employment hiring failed for NPC #{worker.Id}: {ex.Message}");
			}
		}

		foreach (var key in _lastSearch.Keys.Where(x => !_unemployed.Contains(x.Worker)).ToList())
		{
			_lastSearch.Remove(key);
		}
	}

	private void EvaluateWorker(IFuturemud world, INPC worker, Market market, DateTime now)
	{
		var dormant = worker.AffectedBy<EmploymentStasisEffect>();
		if (worker.State.IsDead() || (!dormant && worker.State.IsInStatis())) return;
		var ais = worker.AIs.OfType<EmploymentWorkerAI>().ToList();
		var hasJob = market.Employed.Contains(worker.Id) || worker.ActiveJobs.Any(x => !x.IsJobComplete);
		if (dormant && (hasJob || !ais.Any(x => x.UnemployedStasis)))
		{
			var room = worker.Location ?? world.Rooms.FirstOrDefault();
			if (room is not null)
			{
				worker.SilentAssumeControl(worker.CharacterController);
				world.Add(worker, true);
				room.Login(worker);
			}
			return;
		}

		if (hasJob || ais.Count == 0) return;
		_unemployed.Add(worker.Id);
		if (worker.Combat is not null || worker.AffectedBy<IPauseAIEffect>() ||
		    worker.Movement is not null || worker.Location is null) return;
		if (!dormant && !worker.State.IsAble()) return;
		if (!market.Applicants.Contains(worker.Id) && market.Ranked.Count > 0)
		{
			foreach (var ai in ais.Where(x => x.SearchEnabled))
			{
				var key = (worker.Id, ai.Id);
				if (_lastSearch.TryGetValue(key, out var last) && now - last < ai.SearchCadence) continue;
				if (!dormant && !ai.IsGenerallyAble(worker)) continue;
				_lastSearch[key] = now;
				if (Apply(worker, ai, market))
				{
					market.Applicants.Add(worker.Id);
					break;
				}
			}
		}

		if (!dormant && ais.Any(x => x.UnemployedStasis) && worker.IsPrimaryInstance &&
		    worker.BodyguardingCharacterID is null && worker.RidingMount is null && !worker.Riders.Any() &&
		    worker.CharacterController is NPCController && !worker.AffectedBy<IPossessedBodyEffect>())
		{
			worker.AddEffect(new EmploymentStasisEffect(worker));
			if (worker.Quit(true)) TrackDormant(worker);
			else worker.RemoveAllEffects<EmploymentStasisEffect>();
		}
	}

	private static bool Apply(INPC worker, EmploymentWorkerAI ai, Market market)
	{
		var profile = ai.BuildCandidateProfile(worker);
		var distances = new Dictionary<long, int>();
		LinkedListNode<Vacancy>? best = null;
		var shortest = int.MaxValue;
		for (var node = market.Ranked.First; node is not null; node = node.Next)
		{
			var vacancy = node.Value;
			if (best is not null && (best.Value.Rate != vacancy.Rate || best.Value.OverdueDays != vacancy.OverdueDays))
			{
				break;
			}

			var opening = vacancy.Opening;
			if (!ai.HostMatchesFilter(opening.Employer) ||
			    (vacancy.OwesWages && vacancy.OverdueDays >= ai.MaximumUnpaidOverdueDays) ||
			    market.PreviousHires.Contains((worker.Id, opening)) ||
			    (market.Rejections.TryGetValue((worker.Id, opening), out var rejected) &&
			     vacancy.HostTime - rejected < ai.SearchCadence * 6.0) ||
			    !EmploymentCandidateMatcher.IsMatch(opening, profile, out _))
			{
				continue;
			}

			var distance = 0;
			if (vacancy.Room is { } room && !distances.TryGetValue(room.Id, out distance))
			{
				distance = ai.CommuteDistance(worker, room);
				distances.Add(room.Id, distance);
			}

			if (distance >= shortest) continue;
			best = node;
			shortest = distance;
			if (distance == 0) break;
		}

		if (best is null) return false;
		var selected = best.Value.Opening;
		var application = selected.Employer.Employment.Apply(selected, profile);
		if (application.Status != JobApplicationStatus.Pending) return false;
		best.Value.Available--;
		if (best.Value.Available == 0)
		{
			market.Ranked.Remove(best);
		}
		return true;
	}
}

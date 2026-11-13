#nullable enable

using MudSharp.Construction;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.RPG.Performances;

public sealed partial class PerformanceService
{
	private string Describe(ICharacter actor, PerformancePlan p)
	{
		var work = Work(p);
		var sb = new StringBuilder();
		sb.AppendLine($"{p.Name.ColourName()} — {work.Name.ColourName()} revision {work.Revision.ToString("N0",actor).ColourValue()}, {p.State.DescribeEnum().ColourName()}");
		if (p.State == PerformanceState.Booked && p.ConditionProgId == 0 && p.ScheduledDate is not null)
		{
			var start = new MudDateTime(p.ScheduledDate, _world);
			sb.AppendLine($"Scheduled start: {start.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short)} {start.TimeZone?.Alias}");
		}
		var performing = p.State == PerformanceState.Performing;
		foreach (var role in work.Roles)
		{
			var assigned = Actor((performing ? p.LiveCast : p.Cast).GetValueOrDefault(role.Key));
			var mark = performing ? p.CurrentMarks.GetValueOrDefault(role.Key, role.Mark) : role.Mark;
			var cover = Actor(p.Understudies.GetValueOrDefault(role.Key));
			var signed = p.Registrations.GetValueOrDefault(role.Key) ?? [];
			var location = performing ? $"at {mark}" : $"(starting mark {mark})";
			sb.AppendLine($"{role.Key} {location}: {assigned?.HowSeen(actor) ?? "vacant"}" +
				(cover is null || cover == assigned ? "" : $" (understudy {cover.HowSeen(actor)})") +
				(signed.Count == 0 ? "" : $" — registered {signed.Select(x => Actor(x)?.HowSeen(actor) ?? "absent").ListToString()}"));
		}
		foreach (var place in work.Places) sb.AppendLine($"{place}: {Room(p,place)?.HowSeen(actor) ?? "unbound"}");
		foreach (var key in work.Props)
			sb.AppendLine($"{key}: {_world.Items.Get(p.Props.GetValueOrDefault(key))?.HowSeen(actor) ?? "unavailable"}");
		var problem = BindingProblem(actor,p);
		if (problem is not null) sb.AppendLine(problem.ColourError());
		if (p.State == PerformanceState.Performing && p.Cue < work.Cues.Count)
			sb.AppendLine($"Cue {(p.Cue+1).ToString("N0",actor)}: {work.Cues[p.Cue].Role} {work.Cues[p.Cue].Verb.DescribeEnum()}");
		if (p.Reason.Length > 0) sb.AppendLine(p.Reason);
		if (p.LastOutcome.Length > 0 && p.LastOutcome != p.Reason) sb.AppendLine($"Last outcome: {p.LastOutcome}");
		if (p.CleanupDueUtc.HasValue)
		{
			sb.AppendLine($"Returns in progress: {p.PropReturns.Select(r => r.Key).Concat(p.PerformerReturns.Select(r => r.Role)).ListToString()}.");
			foreach (var line in p.ReturnProgress) sb.AppendLine(line);
		}
		foreach (var line in p.CleanupReport) sb.AppendLine(line);
		return sb.ToString();
	}

	private string? BindingProblem(ICharacter a, PerformancePlan p)
	{
		var work = Work(p);
		foreach (var key in work.Places)
			if (Room(p,key) is null) return $"The place {key.ColourName()} has no usable room. At its mark use {$"performance bind place {key} here".ColourCommand()}.";
		foreach (var leg in work.Exits)
			if (Exit(p,leg.Key,a) is null) return $"The exit {leg.Key.ColourName()} must connect {leg.From.ColourName()} to {leg.To.ColourName()}. At its origin use {$"performance bind exit {leg.Key} <direction>".ColourCommand()}.";
		foreach (var key in work.Props)
			if (_world.Items.Get(p.Props.GetValueOrDefault(key)) is null) return $"The prop {key.ColourName()} has no available item. Use {$"performance bind prop {key} <item>".ColourCommand()} with a visible item.";
		foreach (var role in work.Roles)
		{
			if (role.EligibilityProgId != 0 && !PerformanceAuthoring.UsableRule(_world.FutureProgs.Get(role.EligibilityProgId),
				    ProgVariableTypes.Character, a.IsAdministrator() && work.StaffAuthored))
				return $"The eligibility rule for {role.Key} is unavailable.";
			if (role.Required && role.Mode == PerformanceMode.Npc && !Candidates(p,role).Any()) return $"Cast an NPC as {role.Key}.";
			foreach (var member in Candidates(p,role))
			{
				if (Actor(member) is { IsPlayerCharacter: false } npc && !CanUseNpc(a,p,npc))
					return $"The host cannot assign the NPC for {role.Key}.";
			}
		}
		if (p.Cast.Values.Concat(p.Understudies.Values).GroupBy(x => x).Any(g => g.Count() > 1))
			return "A cast member is double-booked within this production.";
		foreach (var cue in work.Cues.Where(c => c.Verb is PerformanceVerb.Stage or PerformanceVerb.Guard))
		{
			var room = Room(p,cue.Place)!;
			if (!VenueAuthority(a,p,room,a.IsAdministrator()) || cue.Verb == PerformanceVerb.Stage && !VenueAuthority(a,p,Room(p,cue.Target)!,a.IsAdministrator()))
				return $"The host requires venue authority for {cue.Place}.";
		}
		return null;
	}

	// Reserve the stage and its cast and props. A route station or audience room remains public.
	private bool Conflicts(PerformancePlan p,long start,out string reason,IEnumerable<CastMember>? proposedCast = null)
	{
		var otherBookings = Plans.Where(x => x != p && (x.Active || x.State == PerformanceState.Booked && x.ConditionProgId == 0));
		var people = (proposedCast ?? p.Cast.Values.Concat(p.Understudies.Values)).ToHashSet();
		if (Plans.Any(other => other != p && other.CleanupDueUtc.HasValue && start - p.CallLeadSeconds <= Now(p) &&
		    (other.PerformerReturns.Any(r => people.Contains(r.Member)) || other.PropReturns.Any(r => p.Props.ContainsValue(r.Item)))))
		{
			reason = "The cast or a prop is still returning from another production.";
			return true;
		}
		var stages = Work(p).Cues.Where(x => x.Verb == PerformanceVerb.Stage).Select(x => Room(p,x.Place)).ToHashSet();
		foreach (var other in otherBookings)
		{
			var end = other.Active ? Math.Max(Now(other),other.StartInstant + other.ReservedSeconds) : other.StartInstant + other.ReservedSeconds;
			if (start + p.ReservedSeconds < other.StartInstant - other.CallLeadSeconds || end < start - p.CallLeadSeconds) continue;
			if (other.Cast.Values.Concat(other.Understudies.Values).Concat(other.LiveCast.Values).Any(people.Contains))
			{ reason = "A selected performer has a conflicting booking."; return true; }
			foreach (var pair in p.Props)
				if (other.Props.ContainsValue(pair.Value)) { reason = $"The prop {pair.Key} has a conflicting booking."; return true; }
			if (Work(other).Cues.Any(x => x.Verb == PerformanceVerb.Stage && stages.Contains(Room(other,x.Place))))
			{ reason = "The stage has a conflicting booking."; return true; }
		}
		reason = "";
		return false;
	}

	private string Book(ICharacter a, PerformancePlan p,string cmd,StringStack args)
	{
		if (p.CleanupDueUtc.HasValue) return "Post-event returns are still in progress. Check PERFORMANCE STATUS.";
		if (p.Active || p.State == PerformanceState.Booked) return "This production is already booked.";
		var problem = BindingProblem(a,p);
		if (problem is not null) return problem;
		var local = a.Location.DateTime();
		if (local?.Date is null) return "A booking requires a local calendar and clock.";
		var mode = cmd == "call" ? "call" : args.PopSpeech().ToLowerInvariant();
		var when = local;
		RecurringInterval? repeat = null;
		long condition = 0;
		switch (mode)
		{
			case "once":
				var dateText = args.PopSpeech();
				if (dateText.Length == 0) return $"Supply the start in the local calendar and clock. Use {"performance book once \"<date and time>\" [every <interval>]".ColourCommand()}.";
				if (!MudDateTime.TryParse(dateText,local.Calendar,local.Clock,a,out when,out var error))
					return $"Cannot book {dateText.ColourCommand()} as a local start. {error} Quote the date and time together.";
				if (when.Date is null) return "Choose a valid in-game start date.";
				if (when < local) return "Choose a future start.";
				if (!args.IsFinished && !RecurringInterval.TryParse(args.SafeRemainingArgument,local.Calendar,out repeat,out error))
					return $"Cannot use the interval {args.SafeRemainingArgument.ColourCommand()}. {error} For a weekly performance use {"every 1 week".ColourCommand()}.";
				break;
			case "when":
				var prog = PerformanceAuthoring.FindRule(a, args.SafeRemainingArgument, ProgVariableTypes.Location, false);
				if (prog is null) return $"Choose a public Boolean prog that takes one location. Use {"performance book when <prog name>".ColourCommand()}.";
				condition = prog.Id;
				break;
			case "call": break;
			default: return "Book once or when, or open a call manually.";
		}
		var start = MudInstant.FromMudDateTime(when).Ticks + (mode == "call" ? p.CallLeadSeconds : 0);
		// Reserve enough game time for every cue's real-time deadline plus its pacing.
		p.ReservedSeconds = p.GraceSeconds + (long)Math.Ceiling(Work(p).Cues.Sum(c =>
			(double)p.CueGraceSeconds + c.Seconds + 1) * local.Clock.InGameSecondsPerRealSecond);
		if (mode != "when" && Conflicts(p,start,out var reason)) return reason;
		p.ScheduledDate = when.GetDateTimeString();
		p.BookedBy = Identity(a);
		p.BookedAsStaff = a.IsAdministrator();
		p.StartInstant = start;
		p.ClockRoomId = a.Location.Id;
		p.Repeat = repeat;
		p.ConditionProgId = condition;
		p.ConditionLatched = false;
		if (mode == "call") OpenCall(p);
		else { p.State = PerformanceState.Booked; Save(p); }
		return mode == "call" ? "Registration is open. Sign in at your starting mark." : "You book the production.";
	}
}

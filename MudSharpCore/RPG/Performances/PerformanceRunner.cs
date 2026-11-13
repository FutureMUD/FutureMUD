#nullable enable

using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.TimeAndDate;

namespace MudSharp.RPG.Performances;

public sealed partial class PerformanceService
{
	private void Tick()
	{
		foreach (var p in Plans.Where(x => x.Active || x.CleanupDueUtc.HasValue || x.State == PerformanceState.Booked).ToArray())
		{
			if (p.CleanupDueUtc.HasValue)
			{
				ContinueReturns(p);
				continue;
			}
			switch (p.State)
			{
				case PerformanceState.Booked: CheckBooking(p); break;
				case PerformanceState.Gathering: Gather(p); break;
				case PerformanceState.Performing: Advance(p); break;
			}
		}
	}

	private void CheckBooking(PerformancePlan p)
	{
		if (p.ConditionProgId != 0)
		{
			if (_nextConditionCheck.GetValueOrDefault(p.Id) > DateTime.UtcNow) return;
			_nextConditionCheck[p.Id] = DateTime.UtcNow.AddSeconds(10);
			var prog = _world.FutureProgs.Get(p.ConditionProgId);
			var room = _world.Rooms.Get(p.ClockRoomId);
			if (!PerformanceAuthoring.UsableRule(prog, ProgVariableTypes.Location, false) || room is null)
			{
				p.ConditionProgId = 0;
				End(p,false,"The booking condition or its room is no longer available.",false);
				return;
			}
			var triggered = prog!.ExecuteBool(false, room);
			if (triggered == p.ConditionLatched) return;
			p.ConditionLatched = triggered;
			if (!triggered) { Save(p); return; }
			p.StartInstant = Now(p) + p.CallLeadSeconds;
		}
		var now = Now(p);
		if (now < p.StartInstant - p.CallLeadSeconds) return;
		if (now > p.StartInstant + p.GraceSeconds)
		{
			End(p, false, "Missed its latest start.", false);
			return;
		}
		if (Conflicts(p,p.StartInstant,out var conflict))
		{
			End(p,false,conflict,false);
			return;
		}
		OpenCall(p);
	}

	private void OpenCall(PerformancePlan p)
	{
		p.RunId = Guid.NewGuid();
		p.CleanupReport.Clear();
		p.State = PerformanceState.Gathering;
		p.Cue = 0;
		p.Reason = "Registration is open.";
		p.LiveCast.Clear();
		p.CurrentMarks = Work(p).Roles.ToDictionary(x => x.Key,x => x.Mark);
		p.Sustained.Clear();
		p.PropHolders.Clear();
		p.Registrations.Clear();
		p.Withdrawn.Clear();
		p.FinishAfterUtc = null;
		p.Prompted = p.Yielded = false;
		Save(p);
	}

	private IEnumerable<CastMember> Candidates(PerformancePlan p, PerformanceRole role)
	{
		if (p.Cast.TryGetValue(role.Key, out var named)) yield return named;
		if (p.Understudies.TryGetValue(role.Key, out var cover)) yield return cover;
	}

	private bool Signed(PerformancePlan p, string role, CastMember actor) =>
		p.Registrations.TryGetValue(role, out var list) && list.Contains(actor);

	private bool ClaimedElsewhere(PerformancePlan p, CastMember candidate) =>
		Plans.Any(other => other != p &&
			(other.CleanupDueUtc.HasValue && other.PerformerReturns.Any(r => r.Member == candidate) ||
			 other.Active && (other.LiveCast.ContainsValue(candidate) || other.Cast.ContainsValue(candidate) || other.Understudies.ContainsValue(candidate))));

	private string? Gather(PerformancePlan p)
	{
		if (AuthorityProblem(p) is { } problem)
		{
			CancelUnauthorised(p,problem);
			return problem;
		}
		var work = Work(p);
		var cast = new Dictionary<string, CastMember>();
		string? shortage = null;
		foreach (var role in work.Roles.OrderByDescending(x => x.Required))
		{
			foreach (var member in Candidates(p, role))
			{
				var a = Actor(member);
				if (a is null || !Able(a) || !Eligible(role, a, StaffBooking(p)) || ClaimedElsewhere(p, member)) continue;
				if (!a.IsPlayerCharacter)
				{
					ClaimPerformer(p, role, a);
					if (!At(p, role.Mark, a))
						PerformanceActions.Try("Travel", a, Room(p, role.Mark), null, null, null, "", p.RunId, true, out _);
				}
				if (cast.ContainsValue(member) || !At(p, role.Mark, a) ||
				    (a.IsPlayerCharacter && (!PresentPc(a) || !Signed(p, role.Key, member)))) continue;
				cast.TryAdd(role.Key, member);
			}
			if (role.Required && !cast.ContainsKey(role.Key))
				shortage ??= $"Waiting for an eligible, ready {role.Key} at {role.Mark}.";
		}
		var now = Now(p);
		if (now < p.StartInstant)
		{
			p.Reason = shortage ?? "Cast ready. Waiting for the advertised start.";
			return shortage;
		}
		var missing = work.Props.FirstOrDefault(key =>
			_world.Items.Get(p.Props.GetValueOrDefault(key)) is not { } item ||
			!item.TrueLocations.Any(r => p.Places.Values.Any(mark => mark.Room == r.Id)));
		if (missing is not null) shortage ??= $"Missing prop {missing}.";
		if (shortage is not null)
		{
			p.Reason = shortage;
			if (now >= p.StartInstant + p.GraceSeconds) End(p, false, shortage);
			return shortage;
		}
		p.LiveCast = cast;
		p.State = PerformanceState.Performing;
		UnwatchUnused();
		ResetCue(p);
		Save(p);
		return null;
	}

	private static void ResetCue(PerformancePlan p)
	{
		p.Prompted = p.Yielded = false;
		p.FinishAfterUtc = null;
		p.DueUtc = DateTime.UtcNow.AddSeconds(p.CueGraceSeconds);
		p.Reason = "";
	}

	private void Advance(PerformancePlan p)
	{
		if (AuthorityProblem(p) is { } problem)
		{
			CancelUnauthorised(p,problem);
			return;
		}
		var work = Work(p);
		if (p.Cue >= work.Cues.Count) { End(p, true, "Final cue completed."); return; }
		var cue = work.Cues[p.Cue];
		if (DateTime.UtcNow >= p.DueUtc && p.FinishAfterUtc is null)
		{
			End(p, false, $"Cue {p.Cue + 1} timed out. {p.Reason}");
			return;
		}
		foreach (var (key, member) in p.LiveCast.ToArray())
		{
			var current = Actor(member);
			if (current is not null && Able(current) && (!current.IsPlayerCharacter || PresentPc(current)))
			{
				if (At(p,p.CurrentMarks[key],current) || (cue.Role == key && cue.Verb == PerformanceVerb.Travel &&
				    (cue.Target.Length == 0 || At(p,cue.Place,current) || current.Movement is not null))) continue;
				if (!current.IsPlayerCharacter)
				{
					p.Reason = $"Waiting for {key} to return to {p.CurrentMarks[key]}.";
					if (DateTime.UtcNow >= p.DueUtc) End(p,false,p.Reason);
					return;
				}
			}
			LosePart(p, key, member);
		}
		foreach (var role in work.Roles.Where(x => !p.LiveCast.ContainsKey(x.Key)))
		{
			var replacement = Candidates(p, role).Concat(p.Registrations.GetValueOrDefault(role.Key) ?? [])
				.Distinct().FirstOrDefault(m => !p.Withdrawn.Contains(m) && !p.LiveCast.ContainsValue(m) &&
					!ClaimedElsewhere(p, m) && Actor(m) is { } a && Able(a) && Eligible(role,a,StaffBooking(p)) &&
					(!a.IsPlayerCharacter || (PresentPc(a) && Signed(p,role.Key,m) && At(p,p.CurrentMarks[role.Key],a))));
			if (replacement is not null)
			{
				var performer = Actor(replacement)!;
				if (!performer.IsPlayerCharacter)
				{
					ClaimPerformer(p, role, performer);
					if (!At(p,p.CurrentMarks[role.Key],performer))
					{
						PerformanceActions.Try("Travel",performer,Room(p,p.CurrentMarks[role.Key]),null,null,null,"",p.RunId,true,out _);
						p.Reason = $"Waiting for the replacement {role.Key} to arrive at {p.CurrentMarks[role.Key]}.";
						if (DateTime.UtcNow >= p.DueUtc) End(p,false,p.Reason);
						return;
					}
				}
				p.LiveCast[role.Key] = replacement;
				Save(p);
			}
			else if (role.Required)
			{
				p.Reason = $"Waiting for a replacement for {role.Key}.";
				if (DateTime.UtcNow >= p.DueUtc) End(p,false,p.Reason);
				return;
			}
		}

		foreach (var index in p.Sustained)
		{
			var previous = work.Cues[index];
			var actor = Actor(p.LiveCast.GetValueOrDefault(previous.Role));
			if (actor is null) continue;
			var route = previous.Verb == PerformanceVerb.Guard ? Exit(p,previous.Target,actor) : null;
			if (PerformanceActions.Try(previous.Verb.ToString(),actor,Room(p,previous.Place),route,
			    _world.Items.Get(p.Props.GetValueOrDefault(previous.Target)),null,previous.Text,p.RunId,!actor.IsPlayerCharacter,out var why)) continue;
			p.Reason = $"{previous.Role}: {why}";
			if (DateTime.UtcNow >= p.DueUtc) End(p,false,p.Reason);
			return;
		}
		foreach (var (propKey, holderRole) in p.PropHolders)
		{
			var holder = Actor(p.LiveCast.GetValueOrDefault(holderRole));
			var item = _world.Items.Get(p.Props[propKey]);
			if (holder is not null && item is { ContainedIn: null } && item.InInventoryOf == holder.Body)
			{
				var member = CastMember.From(holder);
				if (p.PropReturns.FirstOrDefault(r => r.Item == item.Id) is { } loan && loan.Custodian != member)
				{
					loan.Custodian = member;
					Save(p);
				}
				continue;
			}
			// A player can complete a handoff between heartbeats.
			if (cue.Verb == PerformanceVerb.Give && cue.Target == propKey && item is { ContainedIn: null, InInventoryOf: { } body } &&
			    body == Actor(p.LiveCast.GetValueOrDefault(cue.Text))?.Body) continue;
			if (holder is { IsPlayerCharacter: false } && PerformanceActions.Try("Get",holder,null,null,item,null,"",
			    p.RunId,true,out _))
			{
				if (p.PropReturns.FirstOrDefault(r => r.Item == item?.Id) is { } recovered)
				{
					recovered.Custodian = CastMember.From(holder);
					recovered.TransferTo = null;
					Save(p);
				}
				continue;
			}
			p.Reason = $"Waiting for {holderRole} to recover {propKey}.";
			if (DateTime.UtcNow >= p.DueUtc) End(p,false,p.Reason);
			return;
		}
		if (p.FinishAfterUtc is { } after && after > DateTime.UtcNow) return;
		if (p.FinishAfterUtc is null && !Perform(p, cue)) return;
		if (p.FinishAfterUtc > DateTime.UtcNow) return;
		if (cue.Verb is PerformanceVerb.Guard or PerformanceVerb.Play && p.LiveCast.ContainsKey(cue.Role))
			p.Sustained.Add(p.Cue);
		if (cue.Verb == PerformanceVerb.Stop) p.Sustained.RemoveAll(i => work.Cues[i].Verb == PerformanceVerb.Play &&
			work.Cues[i].Target == cue.Target);
		if (cue.Verb == PerformanceVerb.Travel) p.CurrentMarks[cue.Role] = cue.Place;
		if (p.LiveCast.ContainsKey(cue.Role) && cue.Verb is PerformanceVerb.Get or PerformanceVerb.Give)
			p.PropHolders[cue.Target] = cue.Verb == PerformanceVerb.Give ? cue.Text : cue.Role;
		p.Cue++;
		ResetCue(p);
		Save(p);
	}

	private void LosePart(PerformancePlan p, string key, CastMember member)
	{
		p.LiveCast.Remove(key);
		foreach (var registrations in p.Registrations.Values) registrations.Remove(member);
		if (!p.Withdrawn.Contains(member)) p.Withdrawn.Add(member);
		if (Work(p).Cues.ElementAtOrDefault(p.Cue)?.Role == key)
		{
			p.Prompted = p.Yielded = false;
			// A completed line may still be pacing. Keep its completion marker.
		}
		if (Actor(member) is { IsPlayerCharacter: false } npc) ReleaseEarly(p, member, npc);
		UnwatchUnused();
		Save(p);
	}

	private IRoomExit? Exit(PerformancePlan p, string key, ICharacter a)
	{
		var route = Work(p).Exits.FirstOrDefault(x => x.Key == key);
		return route is null ? null : Room(p,route.From)?.ExitsFor(a)
			.FirstOrDefault(x => x.Exit.Id == p.Exits.GetValueOrDefault(key) && x.Destination == Room(p,route.To));
	}

	private bool Perform(PerformancePlan p, PerformanceCue cue)
	{
		var source = Room(p,cue.Place);
		switch (cue.Verb)
		{
			case PerformanceVerb.Stage:
			case PerformanceVerb.Unstage:
				var audience = Room(p,cue.Target);
				if (source is null || audience is null) { p.Reason = "Stage endpoint unavailable."; return false; }
				var pair = $"{cue.Place}/{cue.Target}";
				if (cue.Verb == PerformanceVerb.Stage && !p.Links.ContainsKey(pair))
				{
					var handle = StageLinks.OpenOwned(source,audience,"Stage",p.RunId.ToString("N"),p.Places[cue.Place].Layer,p.Places[cue.Target].Layer);
					if (handle <= 0) { p.Reason = "Stage link unavailable."; return false; }
					p.Links[pair] = handle;
				}
				else if (cue.Verb == PerformanceVerb.Unstage && p.Links.Remove(pair,out var id)) StageLinks.Close(_world,id);
				return true;
			case PerformanceVerb.Wait:
				p.FinishAfterUtc = DateTime.UtcNow.AddSeconds(cue.Seconds);
				return true;
		}
		if (!p.LiveCast.TryGetValue(cue.Role,out var member)) return true; // optional role omitted
		var a = Actor(member)!;
		if (cue.Verb != PerformanceVerb.Travel && !At(p,cue.Place,a))
		{
			p.Reason = $"{cue.Role} must reach {cue.Place}.";
			return false;
		}
		var exit = cue.Verb is PerformanceVerb.Travel or PerformanceVerb.Guard ? Exit(p,cue.Target,a) : null;
		if ((cue.Verb == PerformanceVerb.Guard || cue.Verb == PerformanceVerb.Travel && cue.Target.Length > 0) && exit is null)
		{
			p.Reason = $"Unavailable leg {cue.Target}.";
			return false;
		}
		var prop = p.Props.TryGetValue(cue.Target,out var itemId) ? _world.Items.Get(itemId) : null;
		var recipient = cue.Verb == PerformanceVerb.Give ? Actor(p.LiveCast.GetValueOrDefault(cue.Text)) : null;
		if (prop is not null && cue.Verb is PerformanceVerb.Get or PerformanceVerb.Give)
			PreparePropReturn(p, cue, a, recipient, prop);
		var expressive = cue.Verb is PerformanceVerb.Speak or PerformanceVerb.Sing or PerformanceVerb.Emote;
		if (a.IsPlayerCharacter && !p.Prompted)
		{
			var target = cue.Verb switch
			{
				PerformanceVerb.Give => $"{prop?.HowSeen(a) ?? cue.Target} to {recipient?.HowSeen(a) ?? cue.Text}",
				PerformanceVerb.Travel => exit?.OutboundDirectionDescription ?? cue.Place,
				PerformanceVerb.Guard => exit?.OutboundDirectionDescription ?? cue.Target,
				PerformanceVerb.Get or PerformanceVerb.Play or PerformanceVerb.Stop => prop?.HowSeen(a) ?? cue.Target,
				_ => cue.Text
			};
			a.OutputHandler.Send($"[Performance {p.Name} - cue {(p.Cue + 1).ToString("N0",a)}] {cue.Role} at {cue.Place}: {cue.Verb.DescribeEnum()} {target}\n" +
				(expressive ? "Use PERFORMANCE DONE when you yield." : "The cue advances when the action succeeds."));
			p.Prompted = true;
		}
		if (a.IsPlayerCharacter && expressive)
		{
			p.Reason = $"Waiting for {cue.Role} to yield.";
			return p.Yielded;
		}
		if (!PerformanceActions.Try(cue.Verb.ToString(),a,exit?.Destination ?? source,exit,prop,recipient,cue.Text,p.RunId,
			    !a.IsPlayerCharacter,out var reason))
		{
			p.Reason = cue.Verb == PerformanceVerb.Travel && cue.Target.Length > 0
				? $"{cue.Role} on {cue.Target}: {reason}"
				: reason;
			return false;
		}
		if (cue.Verb == PerformanceVerb.Travel && !At(p,cue.Place,a))
		{
			p.Reason = $"Waiting for {cue.Role} on the correct layer at {cue.Place}.";
			return false;
		}
		if (expressive && cue.Seconds > 0) p.FinishAfterUtc = DateTime.UtcNow.AddSeconds(cue.Seconds);
		if (cue.Verb is PerformanceVerb.Give or PerformanceVerb.Get &&
		    p.PropReturns.FirstOrDefault(r => r.Item == prop?.Id) is { } loan)
		{
			loan.Custodian = loan.TransferTo;
			loan.TransferTo = null;
		}
		return true;
	}

	private void CancelUnauthorised(PerformancePlan p,string reason)
	{
		p.Repeat = null;
		p.ConditionProgId = 0;
		End(p,false,reason,false);
	}

	private void End(PerformancePlan p,bool success,string message,bool announce = true)
	{
		var wasActive = p.Active;
		var wasGathering = p.State == PerformanceState.Gathering;
		var work = Work(p);
		p.State = success ? PerformanceState.Completed : PerformanceState.Cancelled;
		p.LastOutcome = p.Reason = message;
		try
		{
			if (wasActive && announce && !success && work.FailureText.Length > 0)
			{
				var participants = wasGathering
					? p.Registrations.SelectMany(r => r.Value.Select(m => new KeyValuePair<string,CastMember>(r.Key,m)))
						.Concat(p.Cast.Where(x => Actor(x.Value) is { IsPlayerCharacter: false } npc &&
							At(p,work.Roles.First(r => r.Key == x.Key).Mark,npc)))
					: p.LiveCast.AsEnumerable();
				foreach (var group in participants.OrderByDescending(x => x.Key == work.FailureRole).Select(x => Actor(x.Value))
				         .OfType<ICharacter>().Where(x => x.Location is not null).GroupBy(x => (x.Location,x.RoomLayer)))
				{
					var spoken = group.Where(x => !x.IsPlayerCharacter && Able(x)).Any(speaker =>
						PerformanceActions.Try("Speak",speaker,null,null,null,null,work.FailureText,p.RunId,true,out _));
					if (!spoken)
					{
						group.FirstOrDefault(PresentPc)?.OutputHandler.Send(
							$"[Performance {p.Name}] Cancellation cue: {work.FailureText}");
						group.Key.Location.Handle(group.Key.RoomLayer, $"The performance of {p.Name} has been cancelled.");
					}
				}
			}
		}
		finally
		{
			foreach (var id in p.Links.Values) StageLinks.Close(_world,id);
			p.Links.Clear();
			if (p.RunId != Guid.Empty)
			{
				PerformanceActions.Cleanup(_world, p.RunId);
				if (wasActive && (p.PerformerReturns.Count > 0 || p.PropReturns.Count > 0))
					p.CleanupDueUtc = DateTime.UtcNow.AddSeconds(p.CueGraceSeconds);
			}
			p.LiveCast.Clear();
			p.Registrations.Clear();
			p.CurrentMarks.Clear();
			p.PropHolders.Clear();
			p.Sustained.Clear();
			p.Withdrawn.Clear();
			p.FinishAfterUtc = null;
			p.Prompted = p.Yielded = false;
			UnwatchUnused();
			if (p.Repeat is not null && p.ScheduledDate is not null)
			{
				var prior = new MudDateTime(p.ScheduledDate,_world);
				var next = p.Repeat.GetNextDateTime(p.Repeat.GetNextDateTimeAfter(prior));
				p.ScheduledDate = next.GetDateTimeString();
				p.StartInstant = MudInstant.FromMudDateTime(next).Ticks;
				p.State = PerformanceState.Booked;
			}
			else if (p.ConditionProgId != 0) p.State = PerformanceState.Booked;
			if (p.CleanupDueUtc.HasValue) _world.SaveManager.Flush();
			Save(p);
			if (p.CleanupDueUtc.HasValue) ContinueReturns(p);
		}
	}
}

#nullable enable

using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Employment;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.RPG.Performances;

public sealed partial class PerformanceService
{
	public const string Help = """
		A work is a reusable script. A production is one staging of a published revision with a cast, venue and props.

		#5Taking part#0
		  #3performance calls#0 — list parts you qualify for at your present room and layer.
		  #3performance signin "<production>" <role>#0 — register in person for a part assigned to you.
		  #3performance volunteer "<production>" <role>#0 — offer to take a part here. The organiser can assign you before kickoff.
		  #3performance status#0 — inspect your selected production or, without a selection, your active part.
		  #3performance done#0 — yield after a speech, song or emote. For travel and handoffs, carry out the actual action.
		  #3performance withdraw#0 — leave your part. An eligible understudy can take over. Leaving a required mark can also release your part.

		#5Writing and casting#0
		  #3performance work help#0 — write a draft, edit roles and cues, validate it and publish it. Anyone can author a work.
		  #3performance plan#0 — list productions you can manage. #3performance plan "<name>"#0 selects one.
		  #3performance plan new "<production>" "<published work>" ["<host>"]#0 — create and select a production. Quote multiword names.
		  #3performance bind place <mark> here|<direction>#0 — bind this room and layer, or the far end of a visible exit.
		  #3performance bind exit <leg> <direction>#0 — stand at the origin and bind the exit between the two marked places.
		  #3performance bind prop <key> <item>#0 — bind a visible item that you can assign.
		  #3performance cast <role> <person> [fallback <npc>]#0 — target local performers. A PC-only part always needs a player.
		  #3performance assign "<production>" <role> <person> [fallback <npc>]#0 and #3performance remove "<production>" <role>#0 — revise the cast before kickoff.
		For an NPC cast, name a host where you can assign tasks and select its active employees. Stage and guard cues also require authority over their venues.

		#5Booking and staging#0
		  #3performance preview#0 — check the selected revision, cast, room and exit bindings, props and readiness.
		  #3performance call#0 — open registration now. Participants sign in at their starting marks.
		  #3performance start#0 — attempt an early start during the call. The report names any outstanding requirement.
		  #3performance book once "<local date/time>" [every <interval>]#0 — use the venue calendar and clock. For a weekly run, append #3every 1 week#0.
		  #3performance book when <public prog name>#0 — open a call when a public Boolean rule accepting one location becomes true. It must become false before another call.
		  #3performance cancel#0 — cancel the booking or current run. Use #3performance status#0 to inspect the outcome, pending returns and any equipment that needs a manual return.
		""";

	public const string StaffHelp = Help + "\n\n" + """
		#5Staff building tools#0
		  #3performance work list#0 also includes other authors' drafts. Use a work ID to disambiguate revisions.
		  #3performance bind place <mark> <room ID>#0 — bind a room remotely at your current layer.
		  #3performance bind exit <leg> <direction>#0 — resolve an exit from its bound origin. Bind both endpoints first.
		  Staff can stage a rehearsal without a host and cast local NPCs. Staff-published works can use compatible private role eligibility progs when staged under staff authority. Booking conditions must still be public.
		""";

	public void Command(ICharacter a, StringStack args)
	{
		var cmd = args.PopSpeech().ToLowerInvariant();
		if (cmd == "work") { PerformanceAuthoring.Execute(a,args,this); return; }
		a.OutputHandler.Send(Dispatch(a,cmd,args));
	}

	private PerformancePlan? Named(string name) => Plans.FirstOrDefault(x => x.Name.EqualTo(name));
	private PerformancePlan? Selected(ICharacter a) => Plans.FirstOrDefault(x => x.Id == _selection.GetValueOrDefault(Identity(a)) && CanManage(a,x));

	private string Dispatch(ICharacter a,string cmd,StringStack args)
	{
		if (cmd == "plan") return SelectPlan(a,args);
		if (cmd == "calls") return Calls(a);
		if (cmd is "signin" or "volunteer") return Register(a,cmd,args);
		if (cmd is "" or "help" or "?") return (a.IsAdministrator() ? StaffHelp : Help).SubstituteANSIColour();
		if (cmd is not ("withdraw" or "done" or "status" or "preview" or "assign" or "remove" or "cast" or
		    "bind" or "book" or "call" or "start" or "cancel"))
			return $"Unknown performance command {cmd.ColourCommand()}. Use {"performance help".ColourCommand()} for staging or {"performance work help".ColourCommand()} for writing.";
		var member = CastMember.From(a);
		var participating = Plans.FirstOrDefault(x => x.Active && (x.LiveCast.ContainsValue(member) || x.Registrations.Values.Any(y => y.Contains(member))));
		if (cmd is "withdraw" or "done") return PlayerAction(a,participating,cmd);
		var p = cmd is "assign" or "remove" ? Named(args.PopSpeech()) : Selected(a);
		if (cmd == "status" && p is null && participating is not null)
			return $"{participating.Name}: {participating.State.DescribeEnum()}, cue {participating.Cue + 1:N0}. {participating.Reason}";
		if (p is null || !CanManage(a,p)) return cmd is "assign" or "remove"
			? $"Name a production you can manage. Use {$"performance {cmd} \"<production>\" <role>{(cmd == "assign" ? " <person>" : "")}".ColourCommand()}."
			: $"Select a production you can manage with {"performance plan \"<name>\"".ColourCommand()}. Type {"performance plan".ColourCommand()} to list yours.";
		if (p.CleanupDueUtc.HasValue && cmd is "assign" or "remove" or "cast" or "bind")
			return "Wait for post-event returns before changing the production.";
		switch (cmd)
		{
			case "preview": case "status": return Describe(a,p);
			case "assign": case "remove": case "cast": return Cast(a,p,args,cmd);
			case "bind": return Bind(a,p,args);
			case "book": case "call": return Book(a,p,cmd,args);
			case "start":
				if (p.State != PerformanceState.Gathering) return "Open the call first.";
				p.StartInstant = Math.Min(Now(p),p.StartInstant);
				var shortage = Gather(p);
				Save(p);
				return shortage ?? "The performance begins.";
			case "cancel":
				if (p.CleanupDueUtc.HasValue && p.State == PerformanceState.Booked)
				{
					p.Repeat = null;
					p.ConditionProgId = 0;
					p.State = PerformanceState.Cancelled;
					Save(p);
					return "You cancel the next booking. Post-event returns continue.";
				}
				if (!p.Active && p.State != PerformanceState.Booked) return "This production has no booking to cancel.";
				p.Repeat = null;
				p.ConditionProgId = 0;
				End(p,false,"Cancelled by the organiser.");
				return "You cancel the production.";
			default: throw new InvalidOperationException($"No staging command handler for '{cmd}'.");
		}
	}

	private string SelectPlan(ICharacter a,StringStack args)
	{
		var first = args.PopSpeech();
		if (first.Length == 0)
		{
			var plans = Plans.Where(x => CanManage(a,x)).Select(x => x.Name.ColourName()).ToList();
			return plans.Count == 0 ? $"You have no productions to manage. Start with {"performance plan new \"<name>\" \"<published work>\"".ColourCommand()}."
				: "Your productions: " + plans.ListToString() + ".";
		}
		if (!first.EqualTo("new"))
		{
			var existing = Named(first);
			if (!args.IsFinished) return $"Quote a multiword production name, for example {"performance plan \"Sunday service\"".ColourCommand()}.";
			if (existing is null || !CanManage(a,existing)) return $"No production called {first.ColourCommand()} is available to you. Use {"performance plan".ColourCommand()} to list yours.";
			_selection[Identity(a)] = existing.Id;
			return Describe(a,existing);
		}
		var name = args.PopSpeech();
		var workName = args.PopSpeech();
		var hostName = args.SafeRemainingArgument;
		if (name.Length == 0 || workName.Length == 0)
			return $"Supply both a production name and a published work. Use {"performance plan new \"<production>\" \"<work>\" [\"<host>\"]".ColourCommand()}.";
		if (Named(name) is not null) return $"The production name {name.ColourCommand()} is already in use. Choose another name.";
		var work = Works.Where(x => x.Published && (x.Name.EqualTo(workName) || a.IsAdministrator() && x.Id.ToString() == workName))
			.OrderByDescending(x => x.OwnerId == a.Account.Id).ThenByDescending(x => x.Revision).FirstOrDefault();
		if (work is null) return $"No published work matches {workName.ColourCommand()}. Check the name with {"performance work list".ColourCommand()} or publish your draft first.";
		var host = hostName.Length == 0 ? null : EmploymentHostDiscovery.LoadedHosts(_world).FirstOrDefault(x =>
			x.EmploymentHostName.EqualTo(hostName) && (a.IsAdministrator() || x.HasAuthority(a,EmploymentAuthority.AssignTasks)));
		if (hostName.Length > 0 && host is null) return $"No employment host matching {hostName.ColourCommand()} is available to you. Use a host where you have authority to assign tasks.";
		var plan = new PerformancePlan { Name = name, WorkId = work.Id, Owner = Identity(a), ClockRoomId = a.Location.Id,
			HostType = host?.EmploymentHostType, HostId = host?.Id ?? 0 };
		Save(plan);
		_selection[Identity(a)] = plan.Id;
		return $"You create {name.ColourName()} using {work.Name.ColourName()} revision {work.Revision.ToString("N0",a)}.";
	}

	private static string? Match(IEnumerable<string> keys,string value) => keys.FirstOrDefault(x => x.EqualTo(value));
	private static string UnknownKey(string kind, string input, IEnumerable<string> keys)
	{
		var choices = keys.Select(x => x.ColourName()).ToList();
		return choices.Count == 0 ? $"This work declares no {kind} keys." :
			$"{(input.Length == 0 ? $"Supply a {kind} key." : $"{input.ColourCommand()} is not a {kind} in this work.")} Choose from {choices.ListToString(conjunction: "or ")}.";
	}

	private string Bind(ICharacter a,PerformancePlan p,StringStack args)
	{
		if (p.Active || p.State == PerformanceState.Booked) return "Cancel the booking before changing its bindings.";
		var work = Work(p);
		var kind = args.PopSpeech().ToLowerInvariant();
		var key = args.PopSpeech();
		var target = args.SafeRemainingArgument;
		switch (kind)
		{
			case "place":
				var place = Match(work.Places,key);
				if (place is null) return UnknownKey("place", key, work.Places);
				if (target.Length == 0) return $"Where is {place.ColourName()}? Use {"here".ColourCommand()} or a visible exit direction.";
				var layer = a.RoomLayer;
				IRoom? room;
				if (target.EqualTo("here")) room = a.Location;
				else if (long.TryParse(target,out var id))
				{
					if (!a.IsAdministrator()) return $"Use {"here".ColourCommand()} or a visible exit direction to bind {place.ColourName()}.";
					room = _world.Rooms.Get(id);
					if (room is null) return $"There is no room with ID {target.ColourCommand()}.";
				}
				else
				{
					var passage = a.Location.GetExitKeyword(target,a);
					if (passage is null || !a.CanSee(a.Location,passage)) return $"No visible exit here matches {target.ColourCommand()}. Use a direction from the room's exits.";
					var transition = passage.MovementTransition(a);
					if (transition.TransitionType == RoomMovementTransition.NoViableTransition)
						return "That exit does not lead from this layer.";
					room = passage.Destination;
					layer = transition.TargetLayer;
				}
				if (!StageLinks.IsSupported(room)) return "Select an ordinary room here or through a visible exit.";
				p.Places[place] = new PerformanceMark(room!.Id,layer);
				break;
			case "exit":
				var leg = work.Exits.FirstOrDefault(x => x.Key.EqualTo(key));
				if (leg is null) return UnknownKey("exit", key, work.Exits.Select(x => x.Key));
				if (Room(p,leg.From) is null || Room(p,leg.To) is null) return $"Bind both places for {key.ColourName()} first: {leg.From.ColourName()} and {leg.To.ColourName()}.";
				if (!a.IsAdministrator() && !At(p,leg.From,a)) return $"Stand at the bound room and layer for {leg.From.ColourName()} to bind {key.ColourName()}.";
				if (target.Length == 0) return $"Which direction leads from {leg.From.ColourName()} to {leg.To.ColourName()}?";
				var exit = Room(p,leg.From)?.GetExitKeyword(target,a);
				if (exit is null || !a.CanSee(exit.Origin,exit) || exit.Destination != Room(p,leg.To))
					return $"No visible exit matching {target.ColourCommand()} connects {leg.From.ColourName()} to {leg.To.ColourName()}. Check the direction and both place bindings.";
				var movement = exit.MovementTransition(a);
				if (movement.TransitionType == RoomMovementTransition.NoViableTransition || movement.TargetLayer != p.Places[leg.To].Layer)
					return "That exit does not join the bound layers.";
				p.Exits[leg.Key] = exit.Exit.Id;
				break;
			case "prop":
				var prop = Match(work.Props,key);
				if (prop is null) return UnknownKey("prop", key, work.Props);
				if (target.Length == 0) return $"Supply a visible item's keyword to bind {prop.ColourName()}.";
				var item = a.TargetItem(target);
				if (item is null || !a.CanSee(item)) return $"You cannot target a visible item matching {target.ColourCommand()} here.";
				if (!a.IsAdministrator() && (item.HasOwner && item.Owner != a && item.Owner != Host(p) ||
				    item.InInventoryOf?.Actor is { } holder && holder != a && !CanUseNpc(a,p,holder)))
					return $"You cannot assign {item.HowSeen(a)}. Choose an item under your or the host's control that is available to the cast.";
				p.Props[prop] = item.Id;
				break;
			default: return $"Choose {"place".ColourCommand()}, {"exit".ColourCommand()} or {"prop".ColourCommand()}. Use {"performance bind <kind> <key> <target>".ColourCommand()}.";
		}
		Save(p);
		return $"You bind {key.ColourName()} for this production.";
	}

	private string Cast(ICharacter a,PerformancePlan p,StringStack args,string cmd)
	{
		if (p.State == PerformanceState.Performing) return "The performance has already started.";
		var key = args.PopSpeech();
		var role = Work(p).Roles.FirstOrDefault(x => x.Key.EqualTo(key));
		if (role is null) return UnknownKey("role", key, Work(p).Roles.Select(x => x.Key));
		var previous = p.Cast.GetValueOrDefault(role.Key);
		ICharacter? named = null;
		ICharacter? fallback = null;
		if (cmd != "remove")
		{
			var target = args.PopSpeech();
			if (target.Length == 0) return $"Which local performer should play {key.ColourName()}? Supply a character keyword.";
			named = a.TargetActor(target);
			var allowPrivate = a.IsAdministrator() && Work(p).StaffAuthored;
			if (named is null || !a.CanSee(named)) return $"No visible local performer matches {target.ColourCommand()}.";
			if (!named.IsPlayerCharacter && !CanUseNpc(a,p,named)) return $"To cast this NPC, stage the production for their employer with authority to assign tasks.";
			if (!Eligible(role,named,allowPrivate)) return $"{named.HowSeen(a, true)} does not qualify for {role.Key.ColourName()}. Check the role's {role.Mode.DescribeEnum().ColourName()} casting mode and eligibility rule.";
			if (!args.IsFinished)
			{
				if (!args.PopSpeech().EqualTo("fallback")) return "Use fallback <npc> after the performer.";
				fallback = a.TargetActor(args.SafeRemainingArgument);
				if (fallback is null || fallback.IsPlayerCharacter || fallback == named ||
				    !CanUseNpc(a,p,fallback) || !Eligible(role,fallback,allowPrivate)) return "Choose an eligible NPC understudy.";
			}
			if (p.Active && (ClaimedElsewhere(p,CastMember.From(named)) ||
			    fallback is not null && ClaimedElsewhere(p,CastMember.From(fallback)))) return "A performer is already committed.";
		}
		var oldMembers = Candidates(p,role).ToList();
		var next = named is null ? null : CastMember.From(named);
		var cover = fallback is null ? (named is null ? null : p.Understudies.GetValueOrDefault(role.Key)) : CastMember.From(fallback);
		if (next is not null && (next == cover || Work(p).Roles.Where(r => r != role).SelectMany(r => Candidates(p,r))
		    .Any(m => m == next || m == cover))) return "A performer cannot hold two parts in this production.";
		var others = Work(p).Roles.Where(r => r != role).SelectMany(r => Candidates(p,r));
		if ((p.Active || p.State == PerformanceState.Booked && p.ConditionProgId == 0) &&
		    Conflicts(p,p.StartInstant,out var conflict,others.Concat(new[] { next,cover }.OfType<CastMember>()))) return conflict;
		if (previous is not null && previous != next)
		{
			p.Registrations.GetValueOrDefault(role.Key)?.Remove(previous);
			Actor(previous)?.OutputHandler.Send($"You are released from {role.Key} in {p.Name}.");
		}
		if (named is null) p.Cast.Remove(role.Key);
		else
		{
			p.Cast[role.Key] = CastMember.From(named);
			named.OutputHandler.Send($"You are cast as {role.Key} in {p.Name}. Sign in at {role.Mark} when the call opens.");
		}
		if (cover is null) p.Understudies.Remove(role.Key);
		else p.Understudies[role.Key] = cover;
		foreach (var retired in oldMembers.Where(m => m != next && m != cover))
			if (p.Active && Actor(retired) is { IsPlayerCharacter: false } npc) ReleaseEarly(p, retired, npc);
		UnwatchUnused();
		Save(p);
		return $"You update the casting for {role.Key}.";
	}

	private string Calls(ICharacter a)
	{
		var calls = from p in Plans where p.State == PerformanceState.Gathering
			from role in Work(p).Roles where At(p,role.Mark,a) && Eligible(role,a,StaffBooking(p))
			select $"{p.Name} — {role.Key} at {role.Mark}";
		var available = calls.ToList();
		return available.Count == 0 ? "There are no open parts you qualify for at this room and layer." : "Open calls here: " + available.ListToString() + ".";
	}

	private string Register(ICharacter a,string cmd,StringStack args)
	{
		var name = args.PopSpeech();
		var key = args.SafeRemainingArgument;
		if (name.Length == 0 || key.Length == 0) return $"Use {$"performance {cmd} \"<production>\" <role>".ColourCommand()}. Find local openings with {"performance calls".ColourCommand()}.";
		var p = Named(name);
		if (p?.State != PerformanceState.Gathering || !PresentPc(a)) return $"There is no open call available to you for {name.ColourCommand()}. Check {"performance calls".ColourCommand()} at the starting mark.";
		var role = Work(p).Roles.FirstOrDefault(x => x.Key.EqualTo(key));
		if (role is null || !At(p,role.Mark,a) || !Eligible(role,a,StaffBooking(p))) return "There is no eligible part for you here.";
		var member = CastMember.From(a);
		if (cmd == "signin" && p.Cast.GetValueOrDefault(role.Key) != member) return "You may volunteer for this part.";
		if (ClaimedElsewhere(p,member)) return "You already have a conflicting commitment.";
		if (!p.Registrations.TryGetValue(role.Key,out var list)) p.Registrations[role.Key] = list = [];
		if (!list.Contains(member)) list.Add(member);
		WatchBody(a);
		Save(p);
		return cmd == "signin" ? $"You sign in as {role.Key}." : $"You volunteer for {role.Key}. The organiser can now assign you.";
	}

	private string PlayerAction(ICharacter a,PerformancePlan? p,string cmd)
	{
		if (p is null) return "You have no active part.";
		var member = CastMember.From(a);
		if (cmd == "withdraw")
		{
			foreach (var list in p.Registrations.Values) list.Remove(member);
			foreach (var pair in p.LiveCast.Where(x => x.Value == member).ToArray()) LosePart(p,pair.Key,member);
			Save(p);
			UnwatchUnused();
			return "You withdraw from this performance.";
		}
		var cue = Work(p).Cues.ElementAtOrDefault(p.Cue);
		if (p.State != PerformanceState.Performing || cue is null || !p.Prompted || p.LiveCast.GetValueOrDefault(cue.Role) != member ||
		    !At(p,cue.Place,a) || cue.Verb is not (PerformanceVerb.Speak or PerformanceVerb.Sing or PerformanceVerb.Emote))
			return "There is no expressive cue for you to yield.";
		p.Yielded = true;
		Advance(p);
		return "You yield the floor.";
	}
}

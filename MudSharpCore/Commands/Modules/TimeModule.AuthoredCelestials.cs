#nullable enable

using System.Globalization;
using System.Text.Json;
using MudSharp.Accounts;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.TimeAndDate;

namespace MudSharp.Commands.Modules;

internal sealed class AuthoredCelestialDraft(AuthoredCelestial target)
{
	public AuthoredCelestial Target { get; } = target;
	public AuthoredCelestialDefinition Definition { get; set; } = target.Compiled.CopyDefinition();
}

internal partial class TimeModule
{
	public const string AuthoredCelestialHelp = @"Author geography-independent celestials. All times are integer feed-clock minutes from the anchor; bearings/elevations are degrees. A suffix d on a period converts feed-clock days using this clock's dimensions.

#3celestial types|list#0 - types, presets, limits and installed objects
#3celestial new <preset> <clock> <calendar> <name>#0 - create and save a preset
#3celestial clone <object> <name>#0 - clone and save an authored object
#3celestial edit <object>#0 - open a private draft; close discards it
#3celestial show [object]#0 - show active source or the selected draft
#3celestial set name|description <text>#0
#3celestial set anchor <clock-seconds>|year <minutes or days>d#0
#3celestial set eligible <true|false>|longitude <off|degrees-at-epoch>#0
#3celestial set rail <period> <rise> <set> <rise-bearing> <maximum> <tilt-bearing> [set-bearing]#0
#3celestial set circle <period> <rise> <set> <rise-bearing> <via-bearing> <via-elevation> <set-bearing>#0
#3celestial set path <sparse|dense> <period>#0 - starts an empty path draft
#3celestial set key <minute> <bearing> <elevation> <Travel|Hold|Jump>#0
#3celestial set light <Absolute|PhaseScaled>#0
#3celestial set profile <elevation> <lux>#0 - add/replace a clamped elevation knot
#3celestial set lighttrack <period>#0 - starts an empty independent light track
#3celestial set lightkey <minute> <lux> <Travel|Hold|Jump>#0
#3celestial set phase fixed <turns>|regular <period> [full-minute]|authored <period>#0
#3celestial set phasekey <minute> <unwrapped-turns> <Travel|Hold|Jump>#0
#3celestial set milestone <custom:key> <period> <minute>#0
#3celestial set crescent <sun-id> <custom:key> ...#0 - explicit authored markers; crescent off clears them
#3celestial set echo <id> <custom:key> <BodyVisible|SkyVisible> <order> <text>#0
#3celestial set scheduled <id> <period> <minute> <BodyVisible|SkyVisible> <order> <text>#0
#3celestial set threshold <id> <elevation> <Ascending|Descending> <BodyVisible|SkyVisible> <order> <text>#0
#3celestial set remove <key|lightkey|phasekey|profile|milestone|echo> <minute/elevation/key/id>#0
#3celestial validate|save#0 - compile the draft; save activates it silently and persists it
#3celestial preview [minute-from-anchor] [count] [step]#0 - pure sampled numerical preview
#3celestial event <sunrise|sunset|newmoon|fullmoon|solarlongitude|visiblecrescent|custom:key> <reference-minute> [occurrence] [degrees or sun-id]#0
#3celestial export|import#0 - JSON text using the editor; import validates before replacing the draft
#3celestial close#0 - discard the draft

Sparse tracks require a closure key at Period matching offset zero. The last real key's transition defines the seam; explicitly use Jump for teleportation. Phase closure may retain integer winding. Dense tracks require every sample 0..P-1. Independent channels never expand into a combined cycle.
Attach with #3shard set <shard> celestials <ids...>#0. The first eligible object still supplies time of day using existing elevation/direction thresholds. There are no authored time-of-day overrides. Solar longitude is explicitly synthetic; crescent markers are authored narrative, not calculated visibility.";

	[PlayerCommand("Celestial", "celestial")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	[HelpInfo("celestial", AuthoredCelestialHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Celestial(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord());
		var operation = command.PopForSwitch();
		var draft = actor.EffectsOfType<BuilderEditingEffect<AuthoredCelestialDraft>>().FirstOrDefault()?.EditingItem;
		try
		{
			switch (operation)
			{
				case "types":
					var limits = AuthoredCelestial.Limits(actor.Gameworld);
					actor.OutputHandler.Send($"Types: {Enum.GetNames<AuthoredCelestialKind>().ListToCommaSeparatedValues()}.\nPresets: {AuthoredCelestialPresets.Names.ListToCommaSeparatedValues()}.\nSource limit: {limits.SourceBytes.ToString("N0", actor)} UTF-8 bytes (stored in MySQL LONGTEXT); entries {limits.Entries.ToString("N0", actor)}; previews {limits.PreviewSamples.ToString("N0", actor)} samples/{limits.PreviewEvents.ToString("N0", actor)} events. Settings: AuthoredCelestialSourceBytes, AuthoredCelestialEntries, AuthoredCelestialPreviewSamples, AuthoredCelestialPreviewEvents.\nPeriods accept minutes or a d suffix for feed-clock days; source JSON numbers are invariant. Preset lighting is illustrative.");
					return;
				case "list":
					actor.OutputHandler.Send(StringUtilities.GetTextTable(actor.Gameworld.CelestialObjects.Select(x => new[] { x.Id.ToString("N0", actor), x.Name, x is AuthoredCelestial a ? a.Compiled.Kind.ToString() : x.GetType().Name, x.CelestialAngleIsUsedToDetermineTimeOfDay.ToColouredString() }), ["ID", "Name", "Type", "Time of Day"], actor, Telnet.Green));
					return;
				case "new":
				case "create":
					var preset = command.PopSpeech();
					var clock = actor.Gameworld.Clocks.GetByIdOrName(command.PopSpeech()) ?? throw new ArgumentException("No such clock.");
					var calendar = actor.Gameworld.Calendars.GetByIdOrName(command.PopSpeech()) ?? throw new ArgumentException("No such calendar.");
					var definition = AuthoredCelestialPresets.Create(preset, calendar.Id, clock.MinutesPerHour, clock.HoursPerDay);
					definition.Name = RequiredText(command);
					definition.SeederPreset = null;
					CreateAuthoredCelestial(actor, definition, clock);
					return;
				case "clone":
					var original = actor.Gameworld.CelestialObjects.GetByIdOrName(command.PopSpeech()) as AuthoredCelestial ?? throw new ArgumentException("Choose an authored celestial to clone.");
					var clone = original.Compiled.CopyDefinition();
					clone.Name = RequiredText(command);
					clone.SeederPreset = null;
					CreateAuthoredCelestial(actor, clone, original.Clock);
					return;
				case "edit":
					var target = actor.Gameworld.CelestialObjects.GetByIdOrName(command.PopSpeech()) as AuthoredCelestial ?? throw new ArgumentException("Choose one of the four authored celestial types.");
					SelectAuthoredDraft(actor, target);
					actor.OutputHandler.Send($"Editing a private draft of {target.Name.ColourName()}. Use celestial validate, preview and save.");
					return;
				case "close":
					actor.RemoveAllEffects<BuilderEditingEffect<AuthoredCelestialDraft>>();
					actor.OutputHandler.Send("Closed the celestial draft.");
					return;
				case "show" when !command.IsFinished:
					var shown = actor.Gameworld.CelestialObjects.GetByIdOrName(command.PopSpeech()) as AuthoredCelestial ?? throw new ArgumentException("No such authored celestial.");
					actor.OutputHandler.Send(shown.Compiled.Serialize());
					return;
			}
			if (draft is null) throw new ArgumentException("First use celestial edit <object> or celestial new <preset> <clock> <calendar> <name>.");
			switch (operation)
			{
				case "show": case "export": actor.OutputHandler.Send(AuthoredCelestialFormat.Serialize(draft.Definition)); return;
				case "import":
					actor.OutputHandler.Send("Paste a version 1 authored celestial JSON definition. No filesystem paths or XML are accepted.");
					actor.EditorMode((text, handler, arguments) =>
					{
						try
						{
							var imported = AuthoredCelestialFormat.Parse(text, AuthoredCelestial.Limits(actor.Gameworld));
							AuthoredMath.Require(imported.Kind == draft.Target.Compiled.Kind, "Import cannot change the persisted type.");
							_ = draft.Target.CompileCandidate(imported);
							draft.Definition = imported;
							handler.Send("Imported and validated the draft. Use celestial save to activate it.");
						}
						catch (Exception ex) when (ex is ArgumentException or JsonException or OverflowException) { handler.Send(ex.Message.ColourError()); }
					}, (handler, _) => handler.Send("Import cancelled."), 1.0);
					return;
				case "set":
					var copy = AuthoredCelestialFormat.Parse(AuthoredCelestialFormat.Serialize(draft.Definition));
					EditAuthoredDefinition(copy, command, draft.Target.Compiled.MinutesPerDay);
					AuthoredMath.Require(System.Text.Encoding.UTF8.GetByteCount(AuthoredCelestialFormat.Serialize(copy)) <= AuthoredCelestial.Limits(actor.Gameworld).SourceBytes, "Draft exceeds the configured source byte limit.");
					draft.Definition = copy;
					actor.OutputHandler.Send("Draft updated. Validate or preview before saving.");
					return;
				case "validate":
					var validated = draft.Target.CompileCandidate(draft.Definition);
					actor.OutputHandler.Send($"Valid: {validated.Capabilities}. {validated.StoredEntries.ToString("N0", actor)} compiled source entries; maximum elevation {validated.Path.MaximumElevation.ToString("N6", actor)} degrees; via transit minute {validated.Path.ViaMinute?.ToString("N6", actor) ?? "n/a"}.");
					return;
				case "save":
					var prepared = draft.Target.PrepareActivation(draft.Definition);
					using (new FMDB())
					{
						var row = FMDB.Context.Celestials.Find(draft.Target.Id) ?? throw new ArgumentException("This celestial no longer exists.");
						row.Definition = prepared.Compiled.Serialize();
						FMDB.Context.SaveChanges();
					}
					draft.Target.Activate(prepared);
					draft.Target.Changed = false;
					actor.OutputHandler.Send($"Saved and silently activated {draft.Target.Name.ColourName()}.");
					return;
				case "preview": PreviewAuthored(actor, draft, command); return;
				case "event": PreviewAuthoredEvent(actor, draft, command); return;
				default: actor.OutputHandler.Send(AuthoredCelestialHelp.SubstituteANSIColour()); return;
			}
		}
		catch (Exception ex) when (ex is ArgumentException or JsonException or OverflowException or FormatException)
		{
			actor.OutputHandler.Send(ex.Message.ColourError());
		}
	}

	private static void SelectAuthoredDraft(ICharacter actor, AuthoredCelestial target)
	{
		actor.RemoveAllEffects<BuilderEditingEffect<AuthoredCelestialDraft>>();
		actor.AddEffect(new BuilderEditingEffect<AuthoredCelestialDraft>(actor) { EditingItem = new(target) });
	}

	private static void CreateAuthoredCelestial(ICharacter actor, AuthoredCelestialDefinition definition, TimeAndDate.Time.IClock clock)
	{
		using var preflight = AuthoredCelestial.Create(0, definition, clock, actor.Gameworld);
		long id;
		using (new FMDB())
		{
			var row = new Models.Celestial { Definition = preflight.Compiled.Serialize(), CelestialType = definition.Kind.ToString(), FeedClockId = clock.Id };
			FMDB.Context.Celestials.Add(row);
			FMDB.Context.SaveChanges();
			id = row.Id;
		}
		var celestial = AuthoredCelestial.Create(id, definition, clock, actor.Gameworld);
		actor.Gameworld.Add(celestial);
		SelectAuthoredDraft(actor, celestial);
		actor.OutputHandler.Send($"Created {celestial.Name.ColourName()} as {definition.Kind.ToString().ColourName()} #{id.ToString("N0", actor)}. Attach it with shard set <shard> celestials <ids...>.");
	}

	private static string RequiredText(StringStack command)
	{
		var text = command.SafeRemainingArgument;
		AuthoredMath.Require(!string.IsNullOrWhiteSpace(text), "Text is required.");
		return text;
	}
	private static long Integer(StringStack command) => long.TryParse(command.PopSpeech(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : throw new ArgumentException("Expected an integer.");
	private static double Number(StringStack command)
	{
		if (!double.TryParse(command.PopSpeech(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value)) throw new ArgumentException("Expected a finite invariant number (decimal point .).");
		return value;
	}
	private static T Choice<T>(StringStack command) where T : struct, Enum => Enum.TryParse<T>(command.PopSpeech(), true, out var value) && Enum.IsDefined(value) ? value : throw new ArgumentException($"Expected {Enum.GetNames<T>().ListToString()}.");
	private static long Period(StringStack command, long day)
	{
		var input = command.PopSpeech();
		var days = input.EndsWith('d');
		if (!long.TryParse(days ? input[..^1] : input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || value <= 0) throw new ArgumentException("Period must be positive integer minutes, or integer days with a d suffix.");
		return checked(value * (days ? day : 1));
	}
	private static void Put<T>(List<T> list, Func<T, bool> same, T value) { list.RemoveAll(x => same(x)); list.Add(value); }

	internal static void EditAuthoredDefinition(AuthoredCelestialDefinition d, StringStack c, long day)
	{
		var field = c.PopForSwitch();
		switch (field)
		{
			case "name": d.Name = RequiredText(c); return;
			case "description": d.Description = RequiredText(c); return;
			case "anchor": d.AnchorTicks = Integer(c); break;
			case "year": d.AnnualPeriod = Period(c, day); break;
			case "eligible": d.TimeOfDay = bool.Parse(c.PopSpeech()); break;
			case "longitude":
				d.SyntheticLongitude = !c.PeekSpeech().Equals("off", StringComparison.OrdinalIgnoreCase);
				if (d.SyntheticLongitude) d.LongitudeAtEpoch = Number(c); else c.PopSpeech();
				break;
			case "rail": case "circle":
				d.Path = new() { Mode = field == "rail" ? AuthoredPathMode.SimpleRail : AuthoredPathMode.ThreePointRail, Period = Period(c, day), Rise = Integer(c), Set = Integer(c), RiseBearing = Number(c) };
				if (field == "rail") { d.Path.MaximumElevation = Number(c); d.Path.TiltBearing = Number(c); d.Path.SetBearing = c.IsFinished ? d.Path.RiseBearing + 180 : Number(c); }
				else { d.Path.ViaBearing = Number(c); d.Path.ViaElevation = Number(c); d.Path.SetBearing = Number(c); }
				break;
			case "path":
				var mode = Choice<AuthoredPathMode>(c);
				AuthoredMath.Require(mode is AuthoredPathMode.Sparse or AuthoredPathMode.Dense, "Path mode must be Sparse or Dense.");
				d.Path = new() { Mode = mode, Period = Period(c, day) }; break;
			case "key":
				var key = new PositionKey(Integer(c), Number(c), Number(c), Choice<TrackTransition>(c));
				Put(d.Path.Keys, x => x.Minute == key.Minute, key); d.Path.Keys.Sort((a, b) => a.Minute.CompareTo(b.Minute)); break;
			case "light": d.LightMode = Choice<AuthoredLightMode>(c); break;
			case "profile":
				var knot = new LightKnot(Number(c), Number(c)); d.LightTrack = null;
				Put(d.LightProfile, x => x.Elevation == knot.Elevation, knot); break;
			case "lighttrack": d.LightProfile.Clear(); d.LightTrack = new() { Period = Period(c, day) }; break;
			case "lightkey": case "phasekey":
				var track = field == "lightkey" ? d.LightTrack : d.Phase?.Track;
				if (track is null) throw new ArgumentException("First create this track.");
				var scalar = new ScalarKey(Integer(c), Number(c), Choice<TrackTransition>(c));
				Put(track.Keys, x => x.Minute == scalar.Minute, scalar); break;
			case "phase":
				d.Phase = new() { Mode = Choice<AuthoredPhaseMode>(c) };
				if (d.Phase.Mode == AuthoredPhaseMode.Fixed) d.Phase.FixedTurns = Number(c);
				else { d.Phase.Track.Period = Period(c, day); if (d.Phase.Mode == AuthoredPhaseMode.Regular && !c.IsFinished) d.Phase.FullMoonMinute = Integer(c); }
				break;
			case "milestone":
				var milestone = new CelestialMilestone(c.PopSpeech(), Period(c, day), Integer(c));
				Put(d.Milestones, x => x.Key.Equals(milestone.Key, StringComparison.OrdinalIgnoreCase), milestone); break;
			case "crescent":
				if (d.Phase is null) throw new ArgumentException("A lunar phase channel is required.");
				if (c.PeekSpeech().Equals("off", StringComparison.OrdinalIgnoreCase)) { c.PopSpeech(); d.Phase.CrescentSunId = null; d.Phase.CrescentMilestones.Clear(); break; }
				d.Phase.CrescentSunId = Integer(c); d.Phase.CrescentMilestones.Clear();
				while (!c.IsFinished) d.Phase.CrescentMilestones.Add(c.PopSpeech()); break;
			case "echo": case "scheduled": case "threshold":
				var id = c.PopSpeech(); string? reference = null; long period = 0, minute = 0; double? threshold = null; var direction = CelestialMoveDirection.Ascending;
				if (field == "echo") reference = c.PopSpeech();
				else if (field == "scheduled") { period = Period(c, day); minute = Integer(c); }
				else { threshold = Number(c); direction = Choice<CelestialMoveDirection>(c); }
				var audience = Choice<CelestialEchoAudience>(c); var order = checked((int)Integer(c));
				Put(d.Echoes, x => x.Id == id, new(id, RequiredText(c), audience, order, reference, period, minute, threshold, direction)); return;
			case "remove":
				var collection = c.PopForSwitch();
				switch (collection)
				{
					case "key": var m = Integer(c); d.Path.Keys.RemoveAll(x => x.Minute == m); break;
					case "lightkey": var l = Integer(c); d.LightTrack?.Keys.RemoveAll(x => x.Minute == l); break;
					case "phasekey": var p = Integer(c); d.Phase?.Track.Keys.RemoveAll(x => x.Minute == p); break;
					case "profile": var elevation = Number(c); d.LightProfile.RemoveAll(x => x.Elevation == elevation); break;
					case "milestone": var k = c.PopSpeech(); d.Milestones.RemoveAll(x => x.Key.Equals(k, StringComparison.OrdinalIgnoreCase)); break;
					case "echo": var e = c.PopSpeech(); d.Echoes.RemoveAll(x => x.Id == e); break;
					default: throw new ArgumentException("Unknown track or collection.");
				}
				break;
			default: throw new ArgumentException("Unknown authored setting. See help celestial.");
		}
		AuthoredMath.Require(c.IsFinished, "Unexpected trailing arguments.");
	}

	private static void PreviewAuthored(ICharacter actor, AuthoredCelestialDraft draft, StringStack command)
	{
		var compiled = draft.Target.CompileCandidate(draft.Definition);
		var minute = command.IsFinished ? AuthoredMath.FloorDiv(draft.Target.Calendar.CurrentInstant.Ticks - compiled.AnchorTicks, compiled.SecondsPerMinute) : Integer(command);
		var count = command.IsFinished ? 1 : Integer(command);
		var step = command.IsFinished ? 1 : Integer(command);
		AuthoredMath.Require(count > 0 && count <= AuthoredCelestial.Limits(actor.Gameworld).PreviewSamples && command.IsFinished, "Preview count exceeds the configured sample limit or arguments are invalid.");
		var sb = new StringBuilder($"Clock #{draft.Target.Clock.Id}, calendar #{compiled.CalendarId}, anchor {compiled.AnchorTicks} clock-seconds; {compiled.SecondsPerMinute} seconds/minute; {compiled.MinutesPerDay} minutes/day.\nCapabilities: {compiled.Capabilities}.\nMaximum elevation {compiled.Path.MaximumElevation.ToString("N6", actor)} degrees; via transit {compiled.Path.ViaMinute?.ToString("N6", actor) ?? "n/a"} path minutes.\n");
		for (var i = 0L; i < count; i++)
		{
			var sample = checked(minute + i * step);
			var state = compiled.Evaluate(checked(compiled.AnchorTicks + sample * compiled.SecondsPerMinute));
			sb.AppendLine($"Minute {sample}: azimuth {(state.Azimuth / AuthoredMath.Radians).ToString("N6", actor)} deg; elevation {(state.Elevation / AuthoredMath.Radians).ToString("N6", actor)} deg; source {state.SourceLux.ToString("N6", actor)} lux; {state.Direction}/{state.Motion}; phase {state.Phase?.Name.ToString() ?? "none"}; time of day {AuthoredMath.TimeOfDay(state, compiled.Eligible)}; year minute {state.AnnualMinute}.");
		}
		var reference = new MudInstant(MudInstant.CurrentEpoch, checked(compiled.AnchorTicks + minute * compiled.SecondsPerMinute), compiled.CalendarId, draft.Target.Clock.Id);
		var requests = new[] { AstronomicalEventType.Sunrise, AstronomicalEventType.Sunset, AstronomicalEventType.NewMoon,
			AstronomicalEventType.FullMoon, AstronomicalEventType.SolarLongitude, AstronomicalEventType.VisibleCrescent }
			.Select(x => new CelestialEventRequest(x, AssociatedSunId: compiled.CrescentSunId))
			.Concat(compiled.NamedEventKeys.Select(x => new CelestialEventRequest(null, EventKey: x)))
			.Take(AuthoredCelestial.Limits(actor.Gameworld).PreviewEvents);
		foreach (var request in requests)
		{
			var result = compiled.FindNext(reference, request);
			sb.AppendLine($"Next {request.EventKey ?? request.Type.ToString()}: {(result.Found ? $"anchor minute {((result.Instant.Ticks - compiled.AnchorTicks) / compiled.SecondsPerMinute).ToString("N0", actor)}" : $"{result.Status}: {result.Error}")}");
		}
		sb.AppendLine("Use celestial event for a specific nth result or longitude target. Event output is capped by AuthoredCelestialPreviewEvents; previews never deliver echoes.");
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void PreviewAuthoredEvent(ICharacter actor, AuthoredCelestialDraft draft, StringStack command)
	{
		var key = command.PopSpeech();
		var named = key.StartsWith("custom:", StringComparison.OrdinalIgnoreCase);
		AstronomicalEventType? type = named ? null : Enum.TryParse<AstronomicalEventType>(key, true, out var parsed) && Enum.IsDefined(parsed) ? parsed : throw new ArgumentException("Unknown event kind.");
		var minute = Integer(command);
		var occurrence = command.IsFinished ? 1 : Integer(command);
		var parameter = command.IsFinished ? 0 : Number(command);
		AuthoredMath.Require(occurrence > 0 && command.IsFinished, "Occurrence must be a positive integer; see help celestial for event syntax.");
		var compiled = draft.Target.CompileCandidate(draft.Definition);
		var reference = new MudInstant(MudInstant.CurrentEpoch, checked(compiled.AnchorTicks + minute * compiled.SecondsPerMinute), compiled.CalendarId, draft.Target.Clock.Id);
		if (type == AstronomicalEventType.VisibleCrescent)
		{
			AuthoredMath.Require(parameter >= 1 && parameter < long.MaxValue && Math.Truncate(parameter) == parameter,
				"Crescent preview requires a positive integral associated sun ID.");
			var sun = actor.Gameworld.CelestialObjects.Get((long)parameter);
			AuthoredMath.Require(sun is not null && AstronomicalEventService.IsSolar(sun) && sun is ICelestialTimeContext context && context.Clock.Id == draft.Target.Clock.Id,
				"The associated object must be a sun using the same feed clock.");
		}
		var request = new CelestialEventRequest(type, occurrence, (parameter % 360) * AuthoredMath.Radians, named ? key : null, type == AstronomicalEventType.VisibleCrescent ? checked((long)parameter) : null);
		var result = compiled.FindNext(reference, request);
		actor.OutputHandler.Send(result.Found ? $"Found {key.ColourName()} occurrence {occurrence.ToString("N0", actor)} at {result.Instant.GetStorageString().ColourValue()} (anchor minute {((result.Instant.Ticks - compiled.AnchorTicks) / compiled.SecondsPerMinute).ToString("N0", actor)}). This preview delivers no echoes." : $"{result.Status}: {result.Error}");
	}
}

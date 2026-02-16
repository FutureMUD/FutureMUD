using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class AIStorytellerSeeder : IDatabaseSeeder
{
	private const string StorytellerName = "Example Narrative Steward";
	private const string FiveMinuteStatusProgName = "AIStorytellerStatusFiveMinute";
	private const string HourlyStatusProgName = "AIStorytellerStatusHourly";
	private const string SafeCustomToolProgName = "AIStorytellerToolRecordCue";
	private const string PrimerDocumentName = "AI Storyteller Primer";
	private const string FactionDocumentName = "Harbour Ward Factions";
	private const string RoomsSkillDocumentName = "Skill - World Model: Rooms and Exits";
	private const string PathingSkillDocumentName = "Skill - Pathing Characters Between Locations";
	private const string SituationMemorySkillDocumentName = "Skill - Managing Situations and Memories";
	private const string EventTriageSkillDocumentName = "Skill - Event Triage and Attention Bypass";
	private const string ToolProfileSkillDocumentName = "Skill - Tool Profile Awareness (EventFocused vs Full)";
	private const string ReferenceResearchSkillDocumentName = "Skill - Reference Document Research Workflow";
	private const string TimeCalendarSkillDocumentName = "Skill - Time and Calendar Reasoning";
	private const string PlayerIntelligenceSkillDocumentName = "Skill - Player Intelligence and Plan Monitoring";
	private const string CrimeStatePlaybookSkillDocumentName =
		"Skill - Crime and Character-State Incident Playbook";
	private const string InCharacterOutputSkillDocumentName =
		"Skill - In-Character Output and Disclosure Boundaries";

	public bool SafeToRunMoreThanOnce => true;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
	[
		(
			"install",
			"Do you want to install an AI Storyteller starter pack with example prompts, heartbeat progs, a safe custom tool prog, and sample reference documents?\n\nPlease answer #3yes#f or #3no#f: ",
			(_, _) => true,
			(answer, _) =>
			{
				if (answer.EqualToAny("yes", "y", "no", "n"))
				{
					return (true, string.Empty);
				}

				return (false, "Invalid answer");
			}
		)
	];

	public int SortOrder => 215;
	public string Name => "AI Storyteller Starter Pack";
	public string Tagline => "Adds one complete AI storyteller example package";
	public string FullDescription => @"This package installs a complete, operational AI Storyteller example with:

- one sample storyteller configured for heartbeat operation
- sample five-minute and hourly status progs
- one safe sample custom tool prog
- twelve sample reference documents (one global, one storyteller-restricted, and ten reusable skill guides)

This package is safe to run multiple times and updates/reuses existing sample records.";

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any())
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		var hasStoryteller = context.AIStorytellers.Any(x => x.Name == StorytellerName);
		var hasFiveMinuteProg = context.FutureProgs.Any(x => x.FunctionName == FiveMinuteStatusProgName);
		var hasHourlyProg = context.FutureProgs.Any(x => x.FunctionName == HourlyStatusProgName);
		var hasCustomToolProg = context.FutureProgs.Any(x => x.FunctionName == SafeCustomToolProgName);
		var hasPrimerDocument = context.AIStorytellerReferenceDocuments.Any(x => x.Name == PrimerDocumentName);
		var hasFactionDocument = context.AIStorytellerReferenceDocuments.Any(x => x.Name == FactionDocumentName);
		var hasRoomsSkillDocument = context.AIStorytellerReferenceDocuments.Any(x => x.Name == RoomsSkillDocumentName);
		var hasPathingSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == PathingSkillDocumentName);
		var hasSituationMemorySkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == SituationMemorySkillDocumentName);
		var hasEventTriageSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == EventTriageSkillDocumentName);
		var hasToolProfileSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == ToolProfileSkillDocumentName);
		var hasReferenceResearchSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == ReferenceResearchSkillDocumentName);
		var hasTimeCalendarSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == TimeCalendarSkillDocumentName);
		var hasPlayerIntelligenceSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == PlayerIntelligenceSkillDocumentName);
		var hasCrimeStatePlaybookSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == CrimeStatePlaybookSkillDocumentName);
		var hasInCharacterOutputSkillDocument =
			context.AIStorytellerReferenceDocuments.Any(x => x.Name == InCharacterOutputSkillDocumentName);

		if (hasStoryteller && hasFiveMinuteProg && hasHourlyProg && hasCustomToolProg && hasPrimerDocument &&
		    hasFactionDocument && hasRoomsSkillDocument && hasPathingSkillDocument &&
		    hasSituationMemorySkillDocument && hasEventTriageSkillDocument && hasToolProfileSkillDocument &&
		    hasReferenceResearchSkillDocument && hasTimeCalendarSkillDocument &&
		    hasPlayerIntelligenceSkillDocument && hasCrimeStatePlaybookSkillDocument &&
		    hasInCharacterOutputSkillDocument)
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		if (hasStoryteller || hasFiveMinuteProg || hasHourlyProg || hasCustomToolProg || hasPrimerDocument ||
		    hasFactionDocument || hasRoomsSkillDocument || hasPathingSkillDocument ||
		    hasSituationMemorySkillDocument || hasEventTriageSkillDocument || hasToolProfileSkillDocument ||
		    hasReferenceResearchSkillDocument || hasTimeCalendarSkillDocument ||
		    hasPlayerIntelligenceSkillDocument || hasCrimeStatePlaybookSkillDocument ||
		    hasInCharacterOutputSkillDocument)
		{
			return ShouldSeedResult.ExtraPackagesAvailable;
		}

		return ShouldSeedResult.ReadyToInstall;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!questionAnswers.TryGetValue("install", out var answer) || !answer.EqualToAny("yes", "y"))
		{
			return "No changes were applied.";
		}

		context.Database.BeginTransaction();
		try
		{
			var fiveMinuteProg = EnsureProg(
				context,
				FiveMinuteStatusProgName,
				"AI Storyteller",
				"Heartbeat",
				ProgVariableTypes.Text,
				"Sample heartbeat status prog for AI storyteller five minute triggers.",
				"return \"Five-minute heartbeat check complete.\""
			);

			var hourlyProg = EnsureProg(
				context,
				HourlyStatusProgName,
				"AI Storyteller",
				"Heartbeat",
				ProgVariableTypes.Text,
				"Sample heartbeat status prog for AI storyteller hourly triggers.",
				"return \"Hourly heartbeat check complete. Consider reviewing unresolved situations.\""
			);

			var safeCustomToolProg = EnsureProg(
				context,
				SafeCustomToolProgName,
				"AI Storyteller",
				"Tools",
				ProgVariableTypes.Text,
				"A safe sample custom tool that only echoes back a short narrative cue.",
				"""return "Narrative cue captured: " + @Cue""",
				(ProgVariableTypes.Text, "Cue")
			);

			var storyteller = context.AIStorytellers.FirstOrDefault(x => x.Name == StorytellerName);
			if (storyteller is null)
			{
				storyteller = new AIStoryteller();
				context.AIStorytellers.Add(storyteller);
			}

			storyteller.Name = StorytellerName;
			storyteller.Description =
				"An example AI storyteller pack seeded by DatabaseSeeder for heartbeat-driven narrative management.";
			storyteller.Model = "gpt-5";
			storyteller.SystemPrompt = """
You are an AI Storyteller for a roleplay-intensive MUD.
You must be concise, world-aware, and action-oriented.
When uncertain, gather context with tools before taking action.
Keep responses focused on in-world narrative continuity.
""";
			storyteller.AttentionAgentPrompt = """
Reply with "interested" if the event materially affects ongoing narrative situations.
You may append a short reason after the word interested.
Reply with "ignore" otherwise.
""";
			storyteller.SurveillanceStrategyDefinition =
				"""<Definition><Zones /><IncludedCells /><ExcludedCells /></Definition>""";
			storyteller.ReasoningEffort = "2";
			storyteller.CustomToolCallsDefinition = BuildCustomToolDefinition(safeCustomToolProg.Id);
			storyteller.SubscribeToRoomEvents = false;
			storyteller.SubscribeToSpeechEvents = false;
			storyteller.SubscribeToCrimeEvents = false;
			storyteller.SubscribeToStateEvents = false;
			storyteller.SubscribeTo5mHeartbeat = true;
			storyteller.SubscribeTo10mHeartbeat = false;
			storyteller.SubscribeTo30mHeartbeat = false;
			storyteller.SubscribeToHourHeartbeat = true;
			storyteller.HeartbeatStatus5mProgId = fiveMinuteProg.Id;
			storyteller.HeartbeatStatus10mProgId = null;
			storyteller.HeartbeatStatus30mProgId = null;
			storyteller.HeartbeatStatus1hProgId = hourlyProg.Id;
			storyteller.IsPaused = false;

			context.SaveChanges();

			EnsureReferenceDocument(
				context,
				PrimerDocumentName,
				"General guidance for storyteller behaviour and priorities.",
				"Storyteller",
				"Guide",
				"storyteller,policy,operations,narrative",
				"""
This document describes baseline storyteller operations:
- Preserve continuity across unresolved situations.
- Prefer factual retrieval via tools before major actions.
- Keep side effects auditable and intentional.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				FactionDocumentName,
				"Faction notes scoped to the seeded storyteller.",
				"World",
				"FactionBrief",
				"harbour,ward,factions,politics",
				"""
Harbour Ward snapshot:
- Dock Syndicate controls freight disputes and labour contracts.
- Lantern Circle brokers intelligence and rumours.
- Tidewatch maintains formal law but negotiates with both groups.
""",
				storyteller.Id.ToString()
			);

			EnsureReferenceDocument(
				context,
				RoomsSkillDocumentName,
				"How to reason about rooms, exits, and character location in the gameworld.",
				"Skills",
				"Skill",
				"skill,rooms,cells,exits,locations,landmarks,navigation,map,world-model",
				"""
Use this skill when you need to reason about where people and places are in the gameworld.

Core world model:
- In engine terms a location is a cell, but speak and think of it as a room.
- Treat RoomId as the stable identifier. RoomName is useful for narration but may be non-unique.
- Characters and many events are anchored to a room (for example PlayerInformation includes LocationId and LocationName).
- Rooms are connected by exits. Most exits are cardinal (n, s, e, w, u, d); some are non-cardinal action exits (for example "enter gate" or "climb ladder").

Useful tools:
- OnlinePlayers: quick list of active characters and their current LocationId.
- PlayerInformation: full details for one character, including current location and stored memories.
- Landmarks: discover named places and their mapped RoomId.
- ShowLandmark: inspect one landmark's room details and occupants.
- DateTimeForTarget (CharacterId or RoomId): verify local time context when timing matters.

How exits and doors affect movement:
- Closed doors can block traversal depending on path mode.
- Some exits are traversable only if a character can open, unlock, or otherwise pass them.
- Non-cardinal exits return verb+keyword commands; preserve these exactly when presenting directions.

Reasoning checklist:
1. Identify the actor(s) and source room.
2. Identify the destination as a room id (landmark, reference document, or known id).
3. Confirm whether door sensitivity matters for this request.
4. Resolve pathing with an explicit PathSearchFunction.
5. Present the outcome in-world without exposing tool or JSON internals.

Communication rule:
- Never mention "cell", tool names, ids, or JSON to players.
- Use "room", place names, and plain movement instructions instead.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				PathingSkillDocumentName,
				"How to route a character between locations and communicate directions in-character.",
				"Skills",
				"Skill",
				"skill,pathing,directions,landmarks,routing,travel,room,navigation",
				"""
Use this skill when a character asks for directions or you otherwise need a route between locations.

Goal:
Return practical movement steps from an origin to a destination room or character.

Step 1 - Resolve origin:
- If the requester is known, use their CharacterId and prefer PathFromCharacterToRoom.
- If only room ids are known, use PathBetweenRooms.
- For meeting two characters, use PathBetweenCharacters.

Step 2 - Resolve destination:
- Use Landmarks to find candidate place names.
- Use ShowLandmark to confirm the intended place and retrieve its RoomId.
- If a place is not a landmark, use SearchReferenceDocuments plus ShowReferenceDocument to find documented room ids, aliases, or district guidance.

Step 3 - Choose path mode intentionally:
- PathIncludeUnlockableDoors / IncludeUnlockableDoors: best default for practical routes from a character context.
- PathIncludeUnlockedDoors / IncludeUnlockedDoors: route can pass open or unlocked doors, but not locked/unopenable ones.
- PathRespectClosedDoors / RespectClosedDoors: strict route that assumes closed doors block movement.
- IncludeFireableDoors: useful for line-of-sight or fire-through traversal, not typical walking guidance.
- PathIgnoreDoors / IgnorePresenceOfDoors: coarse geography route when you need broad directional guidance regardless of door state.

Step 4 - Execute and interpret:
- Check result.HasPath.
- If Directions is empty and HasPath is true, origin is already at destination.
- Preserve direction tokens exactly, including non-cardinal commands like "enter archway" or "climb ladder".

Step 5 - Deliver in-character:
- Convert the command list into concise travel instructions.
- Do not reveal room ids, function names, or implementation details.
- If no path exists, explain uncertainty in-world and offer nearest known landmark or district-level guidance.

Recovery logic if path fails:
1. Re-check destination identity (wrong landmark or ambiguous place name is common).
2. Retry with a different path mode suited to the situation.
3. If heavy world tools are unavailable in the current trigger context, create or update a situation so the request is revisited in a full tool context.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				SituationMemorySkillDocumentName,
				"How to create, update, resolve and clean up narrative situations and player memories.",
				"Skills",
				"Skill",
				"skill,situations,memories,continuity,lifecycle,story-management",
				"""
Use this skill to maintain narrative continuity without over-storing low-value data.

Difference between the two:
- Situation: an ongoing world thread the storyteller must track until resolved.
- Memory: a persistent fact about one specific character.

Situation lifecycle:
1. CreateSituation when an event introduces an unresolved arc, risk, objective, or conflict.
2. UpdateSituation whenever new evidence changes stakes, actors, locations, or likely outcomes.
3. ResolveSituation when the thread is concluded or no longer needs active monitoring.
4. ShowSituation before major edits if there is any chance your understanding is stale.
5. Resolve stale situations promptly; only unresolved situation titles are injected into future prompts.

Good situation titles:
- Short, specific, and stable.
- Include the core subject and place when relevant.
- Avoid duplicate titles for the same thread.

Memory lifecycle:
1. CreateMemory for durable character facts likely to matter again.
2. UpdateMemory when the fact evolves or is corrected.
3. ForgetMemory when the fact is obsolete, disproven, or harmful noise.
4. Use PlayerInformation to inspect existing memories before adding another.

When to prefer a situation:
- Multi-character problems.
- Anything tied to a location, faction, or unfolding timeline.
- Items that may trigger future action.

When to prefer memory:
- One-character preferences, history, loyalties, fears, leverage, or commitments.
- Facts useful for tone and continuity in future scenes with that character.

Quality rules:
- Write in-world summaries, not implementation notes.
- Keep entries factual, concise, and updateable.
- Do not duplicate the same fact across many memories.
- Update or resolve as soon as an event materially changes the state.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				EventTriageSkillDocumentName,
				"How to classify incoming events and use attention bypass intentionally.",
				"Skills",
				"Skill",
				"skill,event-triage,attention,bypassattention,endbypassattention,situations",
				"""
Use this skill when new events arrive and you must decide whether to act immediately or ignore.

Primary triage question:
- Does this event materially affect an unresolved or likely-to-be-created situation?
- If yes, act. If no, do not force side effects.

Actionable signals:
- Direct impact on safety, law, violence, political stability, faction leverage, or critical relationships.
- Clear change to an active thread already tracked in situations.
- A high-probability trigger for near-term follow-up scenes.

Usually ignorable signals:
- Ambient flavour with no durable consequences.
- Repetition that adds no new facts.
- Private chatter that does not alter world-state or ongoing narrative threads.

When actionable:
1. CreateSituation for genuinely new durable threads.
2. UpdateSituation when the event advances an existing thread.
3. Keep titles concise and descriptions factual so later updates are easy.

Attention bypass lifecycle:
- Use BypassAttention to keep a known hot thread in focus across subsequent events.
- Pass either CharacterId or RoomId, never both in the same call.
- End bypass with EndBypassAttention as soon as focused monitoring is no longer needed.
- Avoid long-lived bypass on broad contexts unless there is active narrative value.

Operational principle:
- Do not call tools just because they exist.
- Prefer clear triage outcomes: act with intent, or explicitly no-op.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				ToolProfileSkillDocumentName,
				"How to reason about available tools in EventFocused vs Full execution contexts.",
				"Skills",
				"Skill",
				"skill,tools,eventfocused,full,capabilities,limitations,defer,continuity",
				"""
Use this skill to avoid planning actions that require tools unavailable in the current execution profile.

Two key profiles:
- EventFocused: reduced tool set for high-frequency event handling.
- Full: full world/tool capability for deeper research and operational actions.

EventFocused limitations:
- Heavy world tools are unavailable (for example Landmarks, ShowLandmark, pathing tools, CharacterPlans, RecentCharacterPlans, CalendarDefinition).
- Core operational tools remain (situations, memories, player information, reference doc search/show, date-time tools).

Full profile contexts:
- Direct storyteller invocations and heartbeat-driven prompts use full tooling.
- Some state/crime handling paths can also run in full profile.

Decision rule:
1. If current tools can answer safely, proceed now.
2. If critical data depends on heavy tools that are unavailable, do not improvise.
3. Preserve continuity by creating/updating a situation describing the pending follow-up.
4. Resolve the deferred work when a full-tool context is available.

Communication rule:
- Never expose internal profile names or tool limitations to players.
- Present any delay as in-world uncertainty or pending confirmation.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				ReferenceResearchSkillDocumentName,
				"How to search and consume reference documents before committing to actions.",
				"Skills",
				"Skill",
				"skill,reference,research,searchreferencedocuments,showreferencedocument,workflow",
				"""
Use this skill whenever memory is uncertain and a decision could have narrative consequences.

Workflow:
1. SearchReferenceDocuments with targeted query terms.
2. Review candidate metadata (Name, Folder, Type, Keywords, Description).
3. Open the best candidate(s) with ShowReferenceDocument.
4. Act only after reading the actual contents relevant to the decision.

Disambiguation guidance:
- Prefer candidates with matching folder and document type for the current task.
- Use keyword overlap to narrow broad topic matches.
- If multiple docs conflict, favour the most specific and context-appropriate source, then note uncertainty in your situation update.

Visibility awareness:
- Some documents are globally visible.
- Some are restricted to specific storytellers and may not be visible in your current context.
- If a needed document is not visible, proceed with explicit uncertainty rather than invented detail.

Safety rule:
- Do not take high-impact side effects based on partial recall.
- Re-open source documents when facts drive consequential updates to situations, memories, or outputs.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				TimeCalendarSkillDocumentName,
				"How to reason about in-game time, calendars, and timezone context correctly.",
				"Skills",
				"Skill",
				"skill,time,calendar,currentdatetime,datetimefortarget,calendardefinition,timezone",
				"""
Use this skill when timing, schedules, or chronology matters for narrative decisions.

Core tools:
- CurrentDateTime: fastest way to get current in-game date/time when one monitored context exists.
- DateTimeForTarget: authoritative date/time for a specific character or room context.
- CalendarDefinition: deep calendar metadata and optional year-specific month expansion.

Known failure/ambiguity pattern:
- CurrentDateTime can fail when multiple calendar/clock/timezone contexts are active.
- In that case, switch to DateTimeForTarget with a specific CharacterId or RoomId.

Parameter discipline:
- For DateTimeForTarget, provide exactly one of CharacterId or RoomId.
- Do not provide both in one call.

Usage guidance:
1. Prefer target-specific date/time when an event is location-bound.
2. Use calendar definitions when interpreting month/day structure, intercalaries, or epoch naming.
3. Store only durable temporal conclusions in situations or memories.

Communication rule:
- Present time to players in-world (named time/date language), not via tool names, ids, or internal payload fields.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				PlayerIntelligenceSkillDocumentName,
				"How to monitor players and plans for narrative relevance without overreacting.",
				"Skills",
				"Skill",
				"skill,players,plans,onlineplayers,playerinformation,characterplans,recentcharacterplans",
				"""
Use this skill when you need to understand who matters right now and what they are trying to do.

Core tools:
- OnlinePlayers: broad live snapshot of active player characters.
- PlayerInformation: deep profile for one player, including existing storyteller memories.
- RecentCharacterPlans: current plan updates within the rolling plan window.
- CharacterPlans: specific short/long plan details for one character.

Recommended workflow:
1. Start broad with OnlinePlayers to identify likely relevant actors.
2. Drill into candidates with PlayerInformation.
3. Correlate intent with RecentCharacterPlans or CharacterPlans where available.
4. Update situations only when player intent intersects with active narrative threads.

When to create or update a situation:
- A player's declared or observed plan changes stakes in an unresolved thread.
- Multiple players' plans converge on the same conflict, location, or resource.
- A plan signals near-term action with likely world consequences.

When not to escalate:
- Plans are routine, static, or disconnected from current narrative priorities.
- Information is duplicative and adds no new consequence.

Memory discipline:
- Store durable player-specific facts (patterns, loyalties, leverage), not every transient plan line.
- Prefer updating an existing memory over creating duplicates.

Context caveat:
- Some execution contexts do not expose heavy plan/world tools. If unavailable, record intent as a pending situation update and revisit in full-tool contexts.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				CrimeStatePlaybookSkillDocumentName,
				"How to handle crime and major character-state incidents with consistent narrative follow-through.",
				"Skills",
				"Skill",
				"skill,crime,state,incident,playbook,situations,memories,followup",
				"""
Use this skill for incidents such as crimes, unconsciousness, pass-outs, and deaths.

Immediate goals:
- Determine narrative significance quickly.
- Preserve continuity through situations.
- Capture only durable character memory when justified.

Incident response playbook:
1. Identify who is involved, where it happened, and what changed in-world.
2. Classify impact: local noise, meaningful escalation, or major turning point.
3. CreateSituation for new incident threads with follow-up potential.
4. UpdateSituation when incidents evolve an existing thread.
5. ResolveSituation when the thread is concluded and no longer active.

Memory guidance:
- CreateMemory for durable character facts revealed by the incident (for example repeated criminal pattern, life-changing injury, irreversible social break).
- UpdateMemory when prior assumptions are corrected.
- ForgetMemory when old incident assumptions become false or irrelevant.

Attention focus guidance:
- If aftermath events are expected in the same room or around the same characters, use BypassAttention for temporary focus.
- Pass either CharacterId or RoomId, never both.
- End bypass when the incident thread stabilizes.

Output discipline:
- Report incidents in-world and consequence-first.
- Avoid speculative conclusions unless marked uncertain in situation text.
""",
				string.Empty
			);

			EnsureReferenceDocument(
				context,
				InCharacterOutputSkillDocumentName,
				"How to communicate in-character while protecting implementation details.",
				"Skills",
				"Skill",
				"skill,in-character,output,disclosure,boundaries,immersion,communication",
				"""
Use this skill whenever producing narrative output for players or public in-world channels.

Hard disclosure boundaries:
- Never expose tool names, JSON, IDs, schema fields, or internal model/profile terminology.
- Never refer to locations as "cells" in player-facing output; use in-world room/place language.
- Do not leak administrative uncertainty phrased as system internals.

In-character communication rules:
- Speak in diegetic terms: people, places, rumours, witnesses, timings, and consequences.
- Prefer concise, actionable wording over technical explanation.
- If confidence is low, communicate uncertainty in-world ("reports are conflicting", "details remain unconfirmed").

Formatting and tone:
- Avoid bracketed meta commentary about decision logic.
- Keep voice consistent with ongoing scene tone and stakes.
- Prioritize continuity with active situations and known character memories.

Before sending:
1. Remove any accidental internal identifiers.
2. Replace implementation jargon with setting-appropriate language.
3. Confirm the message advances scene clarity or consequence understanding.
""",
				string.Empty
			);

			context.SaveChanges();
			context.Database.CommitTransaction();
			return "Installed or updated AI Storyteller starter pack successfully.";
		}
		catch (Exception e)
		{
			context.Database.RollbackTransaction();
			return $"Failed to install AI Storyteller starter pack: {e.Message}";
		}
	}

	private static FutureProg EnsureProg(FuturemudDatabaseContext context, string functionName, string category,
		string subcategory, ProgVariableTypes returnType, string comment, string text,
		params (ProgVariableTypes Type, string Name)[] parameters)
	{
		var prog = context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
		if (prog is null)
		{
			prog = new FutureProg
			{
				FunctionName = functionName,
				FunctionComment = comment,
				FunctionText = text,
				ReturnType = (long)returnType,
				Category = category,
				Subcategory = subcategory,
				Public = false,
				AcceptsAnyParameters = false,
				StaticType = (int)FutureProgStaticType.NotStatic
			};
			context.FutureProgs.Add(prog);
			context.SaveChanges();
		}

		prog.FunctionComment = comment;
		prog.FunctionText = text;
		prog.ReturnType = (long)returnType;
		prog.Category = category;
		prog.Subcategory = subcategory;
		prog.Public = false;
		prog.AcceptsAnyParameters = false;
		prog.StaticType = (int)FutureProgStaticType.NotStatic;

		var existingParameters = prog.FutureProgsParameters.ToList();
		foreach (var parameter in existingParameters)
		{
			context.FutureProgsParameters.Remove(parameter);
		}

		for (var index = 0; index < parameters.Length; index++)
		{
			var (type, name) = parameters[index];
			context.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = index,
				ParameterType = (long)type,
				ParameterName = name
			});
		}

		context.SaveChanges();
		return prog;
	}

	private static void EnsureReferenceDocument(FuturemudDatabaseContext context, string name, string description,
		string folderName, string documentType, string keywords, string contents, string restrictedStorytellerIds)
	{
		var document = context.AIStorytellerReferenceDocuments.FirstOrDefault(x => x.Name == name);
		if (document is null)
		{
			document = new AIStorytellerReferenceDocument();
			context.AIStorytellerReferenceDocuments.Add(document);
		}

		document.Name = name;
		document.Description = description;
		document.FolderName = folderName;
		document.DocumentType = documentType;
		document.Keywords = keywords;
		document.DocumentContents = contents;
		document.RestrictedStorytellerIds = restrictedStorytellerIds;
	}

	private static string BuildCustomToolDefinition(long customToolProgId)
	{
		return new XElement("ToolCalls",
			new XElement("ToolCall",
				new XElement("Name", new XCData("RecordNarrativeCue")),
				new XElement("Description", new XCData("Records a short narrative cue and echoes it back.")),
				new XElement("Prog", customToolProgId),
				new XElement("IncludeWithEcho", false),
				new XElement("ParameterDescriptions",
					new XElement("Description", new XAttribute("name", "Cue"),
						new XCData("A concise narrative cue to preserve for continuity."))))
		).ToString();
	}
}

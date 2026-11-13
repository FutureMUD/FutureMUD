#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Audio;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Movement;

namespace MudSharp.RPG.Performances;

/// <summary>Executes native world actions for staged performances.</summary>
public static class PerformanceActions
{
	public static bool Try(string verb, ICharacter performer, IRoom? place, IRoomExit? exit,
		IGameItem? prop, ICharacter? recipient, string text, Guid runId, bool execute, out string reason)
	{
		reason = string.Empty;
		if (performer is null)
		{
			reason = "There is no performer for this action.";
			return false;
		}

		string action = verb?.Trim().ToLowerInvariant() ?? string.Empty;
		if (!execute)
		{
			return Verify(action, performer, place, exit, prop, recipient, runId, out reason);
		}

		switch (action)
		{
			case "speak":
			case "sing":
			case "emote":
				return DeliverText(action, performer, text, out reason);
			case "travel": return Travel(performer, place, exit, runId, out reason);
			case "get": return Get(performer, prop, out reason);
			case "give": return Give(performer, prop, recipient, out reason);
			case "play": return Play(performer, prop, text, runId, out reason);
			case "stop": return Stop(performer, prop, runId, out reason);
			case "guard": return Guard(performer, exit, runId, out reason);
			default:
				reason = $"The native performance action '{verb}' is not supported.";
				return false;
		}
	}

	public static void Claim(ICharacter npc, Guid runId)
	{
		if (npc is not null && !npc.EffectsOfType<PerformanceClaimEffect>().Any(x => x.RunId == runId))
		{
			npc.AddEffect(new PerformanceClaimEffect(npc, runId));
		}
	}

	public static void Release(ICharacter npc, Guid runId)
	{
		if (npc is null) return;
		npc.RemoveAllEffects<PerformanceClaimEffect>(x => x.RunId == runId, true);
		npc.RemoveAllEffects<GuardingExit>(x => x.PerformanceRunId == runId, true);
		StopOwnedInstruments(npc, runId);
	}

	public static void Cleanup(IFuturemud world, Guid runId)
	{
		foreach (var character in WorldCharacters(world)) Release(character, runId);
	}

	private static bool Verify(string action, ICharacter actor, IRoom? place, IRoomExit? exit,
		IGameItem? prop, ICharacter? recipient, Guid runId, out string reason)
	{
		reason = string.Empty;
		switch (action)
		{
			case "speak": case "sing": case "emote":
				reason = "Waiting for the participant to deliver this direction."; return false;
			case "travel":
				if ((place is not null && ReferenceEquals(actor.Location, place)) ||
				    (exit is not null && ReferenceEquals(actor.Location, exit.Destination))) return true;
				reason = "The performer has not reached the bound mark."; return false;
			case "get":
				if (prop is not null && ReferenceEquals(prop.InInventoryOf, actor.Body) && prop.ContainedIn is null) return true;
				reason = "The performer does not hold the required prop."; return false;
			case "give":
				if (prop is not null && recipient is not null && ReferenceEquals(prop.InInventoryOf, recipient.Body) && prop.ContainedIn is null) return true;
				reason = "The intended recipient has not received the prop."; return false;
			case "play":
				if (prop?.GetItemType<IInstrument>() is { } instrument && instrument.IsBeingPlayed && ReferenceEquals(instrument.CurrentPerformer, actor)) return true;
				reason = "The performer is not playing the bound instrument."; return false;
			case "stop":
				if (prop?.GetItemType<IInstrument>() is { } stopped && !stopped.IsBeingPlayed) return true;
				if (prop?.GetItemType<IInstrument>() is { } active && !ReferenceEquals(active.CurrentPerformer, actor))
				{ reason = "The instrument is being played by another performer."; return false; }
				reason = "The instrument is still being played by this performer."; return false;
			case "guard":
				if (exit is not null && actor.EffectsOfType<IGuardExitEffect>().Any(x => x.Applies() && ReferenceEquals(x.Exit, exit))) return true;
				reason = "The performer is not guarding the bound exit for this run."; return false;
			default:
				reason = $"The native performance action '{action}' is not supported."; return false;
		}
	}

	private static bool DeliverText(string action, ICharacter actor, string text, out string reason)
	{
		reason = string.Empty;
		if (string.IsNullOrWhiteSpace(text)) { reason = $"There is no text to {action}."; return false; }
		string limit = action == "emote" ? "MaximumEmoteLength" : "MaximumSayLength";
		if (text.Length > actor.Gameworld.GetStaticInt(limit)) { reason = "The authored direction exceeds the world's configured message limit."; return false; }
		if (action is "speak" or "sing")
		{
			var body = actor.Body;
			if (body.CurrentLanguage is null)
			{ reason = "The performer has no spoken language selected."; return false; }
			var volume = action == "sing" ? AudioVolume.Loud : AudioVolume.Decent;
			if (!body.Communications.CanVocalise(body, volume))
			{ reason = body.Communications.WhyCannotVocalise(body, volume); return false; }
			if (body.Communications is HumanoidCommunicationStrategy strategy && strategy.IsGagged(body))
			{ reason = "The performer cannot deliver the words through a gag."; return false; }
		}
		switch (action)
		{
			case "speak": actor.Body.Say(null, text, null); break;
			case "sing": actor.Body.Sing(null, text, null); break;
			default: actor.Body.Emote(text, CharacterState.Conscious.HasFlag(actor.State)); break;
		}
		return true;
	}

	private static bool Travel(ICharacter actor, IRoom? place, IRoomExit? exit, Guid runId, out string reason)
	{
		reason = string.Empty;
		if (actor.Movement is not null)
		{
			reason = "The performer is already moving; waiting for arrival at the bound mark.";
			return false;
		}

		if (exit is not null)
		{
			if (ReferenceEquals(actor.Location, exit.Destination)) return true;
			if (!ReferenceEquals(actor.Location, exit.Origin))
			{ reason = "The performer is away from the origin of the route leg."; return false; }
		}
		else
		{
			if (place is null) { reason = "Travel requires a bound destination mark or route leg."; return false; }
			if (ReferenceEquals(actor.Location, place)) return true;
			exit = actor.PathBetween(place, 50, PathSearch.RespectClosedDoors).FirstOrDefault();
			if (exit is null) { reason = "The performer cannot currently path to the bound mark."; return false; }
		}
		if (!actor.CanMove(exit).Result || !actor.CanCross(exit).Success)
		{ reason = "The way is blocked for the performer."; return false; }
		if (!actor.Move(exit)) { reason = "The performer could not start moving."; return false; }
		var claim = actor.EffectsOfType<PerformanceClaimEffect>().FirstOrDefault(x => x.RunId == runId);
		if (claim is not null) claim.StartedMovement = actor.Movement;
		reason = "The performer is travelling toward the bound mark.";
		return false;
	}

	private static bool Get(ICharacter actor, IGameItem? prop, out string reason)
	{
		reason = string.Empty;
		if (prop is null) { reason = "This action has no bound prop."; return false; }
		if (ReferenceEquals(prop.InInventoryOf, actor.Body) && prop.ContainedIn is null) return true;
		if (prop.InInventoryOf is not null && !ReferenceEquals(prop.InInventoryOf, actor.Body))
		{ reason = "The bound prop is held by someone else."; return false; }
		if (!actor.CanSee(prop)) { reason = "The performer cannot see the bound prop."; return false; }
		var reach = actor.CanReachItem(prop);
		if (!reach.Truth) { reason = reach.Message; return false; }
		for (var container = prop.ContainedIn; container is not null; container = container.ContainedIn)
			if (container.InInventoryOf is { } holder && !ReferenceEquals(holder,actor.Body))
			{ reason = "The bound prop is inside someone else's inventory."; return false; }
		bool canGet = prop.ContainedIn is null ? actor.Body.CanGet(prop, 0) : actor.Body.CanGet(prop, prop.ContainedIn, 0);
		if (!canGet)
		{ reason = prop.ContainedIn is null ? actor.Body.WhyCannotGet(prop, 0) : actor.Body.WhyCannotGet(prop, prop.ContainedIn, 0); return false; }
		if (prop.ContainedIn is null) actor.Body.Get(prop); else actor.Body.Get(prop, prop.ContainedIn);
		return ReferenceEquals(prop.InInventoryOf, actor.Body) && prop.ContainedIn is null;
	}

	private static bool Give(ICharacter actor, IGameItem? prop, ICharacter? recipient, out string reason)
	{
		reason = string.Empty;
		if (prop is null || recipient is null) { reason = "Giving requires a bound prop and recipient."; return false; }
		if (ReferenceEquals(prop.InInventoryOf, recipient.Body) && prop.ContainedIn is null) return true;
		if (!actor.ColocatedWith(recipient) || actor.RoomLayer != recipient.RoomLayer)
		{ reason = "The recipient is out of reach."; return false; }
		if (!actor.Body.CanGive(prop, recipient.Body)) { reason = actor.Body.WhyCannotGive(prop, recipient.Body); return false; }
		actor.Body.Give(prop, recipient.Body);
		return ReferenceEquals(prop.InInventoryOf, recipient.Body) && prop.ContainedIn is null;
	}

	private static bool Play(ICharacter actor, IGameItem? prop, string style, Guid runId, out string reason)
	{
		reason = string.Empty;
		if (prop?.GetItemType<IInstrument>() is not InstrumentGameItemComponent instrument)
		{ reason = "The bound prop is not a playable instrument."; return false; }
		if (string.IsNullOrWhiteSpace(style)) style = instrument.Styles.FirstOrDefault() ?? "general";
		if (instrument.IsBeingPlayed && ReferenceEquals(instrument.CurrentPerformer, actor) && instrument.CurrentPerformanceRunId == runId) return true;
		if (instrument.PlayForPerformance(actor, style, runId)) return true;
		reason = instrument.WhyCannotPlay(actor, style);
		if (string.IsNullOrWhiteSpace(reason)) reason = "The performance check did not produce a playable outcome.";
		return false;
	}

	private static bool Stop(ICharacter actor, IGameItem? prop, Guid runId, out string reason)
	{
		reason = string.Empty;
		if (prop?.GetItemType<IInstrument>() is not InstrumentGameItemComponent instrument)
		{ reason = "The bound prop is not a playable instrument."; return false; }
		if (!instrument.IsBeingPlayed) return true;
		if (!ReferenceEquals(instrument.CurrentPerformer, actor)) { reason = "The instrument is being played by another performer."; return false; }
		if (instrument.CurrentPerformanceRunId != runId) { reason = "This run does not own the current instrument performance."; return false; }
		instrument.StopPlaying(actor);
		return !instrument.IsBeingPlayed;
	}

	private static bool Guard(ICharacter actor, IRoomExit? exit, Guid runId, out string reason)
	{
		reason = string.Empty;
		if (exit is null || !ReferenceEquals(actor.Location, exit.Origin)) { reason = "The performer must be at the origin of a bound exit to guard it."; return false; }
		var current = actor.EffectsOfType<IGuardExitEffect>().FirstOrDefault(x => x.Applies());
		if (current is GuardingExit ours && ours.PerformanceRunId == runId && ReferenceEquals(ours.Exit, exit)) return true;
		if (current is not null || actor.EffectsOfType<IAffectedByChangeInGuarding>().Any())
		{ reason = "The performer already has a guard assignment that this run does not own."; return false; }
		var guard = new GuardingExit(actor, exit, false, runId);
		actor.AddEffect(guard);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ begin|begins guarding the exit to {exit.OutboundDirectionDescription}.", actor)));
		return actor.EffectsOfType<GuardingExit>().Any(x => x.PerformanceRunId == runId && ReferenceEquals(x.Exit, exit));
	}

	private static void StopOwnedInstruments(ICharacter actor, Guid runId)
	{
		foreach (var playing in actor.EffectsOfType<PlayingInstrument>().Where(x => x.Instrument.CurrentPerformanceRunId == runId).ToList())
			playing.Instrument.StopPlaying(actor, false);
	}

	private static IEnumerable<ICharacter> WorldCharacters(IFuturemud world) => world.Actors.Concat(world.CachedActors).DistinctPhysicalInstances();
}

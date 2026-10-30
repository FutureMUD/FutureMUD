#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete;

namespace MudSharp.Commands.Modules;

internal partial class RoomBuilderModule
{
	private static void RoomSetPolicy(ICharacter actor, StringStack input)
	{
		var policies = actor.Location.EffectsOfType<RoomPolicyEffect>().Where(x => x.ApplicabilityProg is null).ToArray();
		if (input.IsFinished)
		{
			actor.OutputHandler.Send(policies.Length == 0 ? "This room has no static room policies." :
				policies.Select(x => x.Describe(actor)).ListToString());
			return;
		}
		var option = input.PopSpeech();
		var restrictions = policies.Aggregate(RoomRestrictions.None, (result, policy) => result | policy.Restrictions);
		var capacity = policies.Where(x => x.MaximumOccupants > 0).Select(x => x.MaximumOccupants).DefaultIfEmpty(0).Min();
		var costProg = policies.Select(x => x.OccupancyCostProgId).FirstOrDefault(x => x > 0);
		if (option.EqualTo("clear"))
		{
			restrictions = RoomRestrictions.None;
			capacity = 0;
			costProg = 0;
		}
		else if (option.EqualTo("cost"))
		{
			if (input.SafeRemainingArgument.EqualTo("clear")) costProg = 0;
			else
			{
				var prog = new ProgLookupFromBuilderInput(actor, input.SafeRemainingArgument, ProgVariableTypes.Number, [ProgVariableTypes.Character]).LookupProg();
				if (prog is null) return;
				costProg = prog.Id;
			}
		}
		else if (option.EqualTo("capacity"))
		{
			if (!int.TryParse(input.SafeRemainingArgument, out capacity) || capacity < 0)
			{
				actor.OutputHandler.Send("Specify a maximum occupant count, or 0 for unlimited.");
				return;
			}
		}
		else if (option.TryParseEnum<RoomRestrictions>(out var flag) && flag != RoomRestrictions.None && Enum.IsDefined(flag))
			restrictions ^= flag;
		else
		{
			actor.OutputHandler.Send("Use vehicles, mounts, flight, climbing, npcwandering, magic, lethal, capacity <count>, cost <prog|clear>, or clear.");
			return;
		}
		foreach (var policy in policies) actor.Location.RemoveEffect(policy);
		if (restrictions != RoomRestrictions.None || capacity > 0 || costProg > 0)
			actor.Location.AddEffect(new RoomPolicyEffect(actor.Location, restrictions, capacity, costProg));
		actor.OutputHandler.Send("You update this room's static policies.");
	}

	private static void RoomSetObservation(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			var links = actor.Location.EffectsOfType<RoomObservationEffect>().ToArray();
			actor.OutputHandler.Send(links.Length == 0 ? "No rooms observe communication here." :
				links.Select(x => x.Describe(actor)).ListToString());
			return;
		}
		var observer = LookupRoom(actor, input.PopSpeech());
		if (observer is null) return;
		if (observer == actor.Location || observer.RouteDefinition is not null || actor.Location.RouteDefinition is not null)
		{
			actor.OutputHandler.Send("Choose another ordinary room; coordinate-based route rooms cannot use these links.");
			return;
		}
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Specify the visible context prefix, or clear to remove the link.");
			return;
		}
		var prefix = input.SafeRemainingArgument;
		foreach (var existing in actor.Location.EffectsOfType<RoomObservationEffect>().Where(x => x.ObserverRoomId == observer.Id).ToArray())
			actor.Location.RemoveEffect(existing);
		if (!prefix.EqualTo("clear")) actor.Location.AddEffect(new RoomObservationEffect(actor.Location, observer.Id, prefix));
		actor.OutputHandler.Send(prefix.EqualTo("clear") ? "You remove that visual communication link." : "You configure that visual communication link.");
	}

	private static void RoomSetAccess(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send(actor.Location.EffectsOfType<RoomExitPolicyEffect>().Select(x => x.Describe(actor)).ListToString());
			return;
		}
		if (!input.PopSpeech().TryParseEnum<CardinalDirection>(out var direction) || direction == CardinalDirection.Unknown)
		{
			actor.OutputHandler.Send("Specify a cardinal direction such as north or up.");
			return;
		}
		var controlsOpening = input.Peek().EqualTo("opening");
		if (controlsOpening) input.PopSpeech();
		var mode = input.PopSpeech();
		long progId = 0;
		if (!mode.EqualTo("clear") && !mode.EqualTo("deny") && !mode.EqualTo("flying"))
		{
			var prog = new ProgLookupFromBuilderInput(actor, mode, ProgVariableTypes.Boolean,
				[ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location]).LookupProg();
			if (prog is null) return;
			progId = prog.Id;
		}
		var message = input.SafeRemainingArgument;
		if (!mode.EqualTo("clear") && string.IsNullOrWhiteSpace(message))
		{
			actor.OutputHandler.Send("Supply the denial message after deny, flying, or the access prog.");
			return;
		}
		var attemptProg = actor.Location.EffectsOfType<RoomExitPolicyEffect>().FirstOrDefault(x => x.Direction == direction)?.DeniedAttemptProgId ?? 0;
		foreach (var existing in actor.Location.EffectsOfType<RoomExitPolicyEffect>().Where(x => x.Direction == direction).ToArray())
			actor.Location.RemoveEffect(existing);
		if (!mode.EqualTo("clear")) actor.Location.AddEffect(new RoomExitPolicyEffect(actor.Location, direction, message, progId, mode.EqualTo("flying"), controlsOpening, attemptProg));
		actor.OutputHandler.Send("You update the access policy for that direction.");
	}

	private static void RoomSetAccessAttempt(ICharacter actor, StringStack input)
	{
		if (!input.PopSpeech().TryParseEnum<CardinalDirection>(out var direction) || direction == CardinalDirection.Unknown)
		{ actor.OutputHandler.Send("Specify a cardinal direction with an existing access policy."); return; }
		var existing = actor.Location.EffectsOfType<RoomExitPolicyEffect>().SingleOrDefault(x => x.Direction == direction && x.ApplicabilityProg is null);
		if (existing is null) { actor.OutputHandler.Send("Configure a static access policy for that direction first."); return; }
		long progId = 0;
		if (!input.SafeRemainingArgument.EqualTo("clear"))
		{
			var prog = new ProgLookupFromBuilderInput(actor, input.SafeRemainingArgument, ProgVariableTypes.Boolean,
				[ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location]).LookupProg();
			if (prog is null) return;
			progId = prog.Id;
		}
		actor.Location.RemoveEffect(existing);
		actor.Location.AddEffect(new RoomExitPolicyEffect(actor.Location, direction, existing.Message, existing.AccessProgId,
			existing.RequiresFlight, existing.ControlsOpening, progId));
		actor.OutputHandler.Send("You update the denied movement attempt prog. Feasibility checks do not execute it.");
	}
}

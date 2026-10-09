#nullable enable

using System.Text;
using MudSharp.Character;

namespace MudSharp.Commands.Modules;

internal partial class StaffModule
{
	private static void DebugCensus(ICharacter actor, StringStack ss)
	{
		var world = actor.Gameworld;
		MudSharp.Character.Character? target = null;
		if (!ss.IsFinished)
		{
			var text = ss.PopSpeech();
			if (!ss.IsFinished || !long.TryParse(text, out var id) ||
				world.Actors.FirstOrDefault(x => x.Id == id) is not MudSharp.Character.Character loaded)
			{
				actor.Send("Specify the ID of one currently loaded character.".ColourError());
				return;
			}
			target = loaded;
		}

		var sb = new StringBuilder("Runtime census:\n");
		sb.AppendLine($"Actors: {world.Actors.Count()}");
		sb.AppendLine($"Cached actors: {world.CachedActors.Count()}");
		sb.AppendLine($"NPCs: {world.NPCs.Count()}");
		sb.AppendLine($"Bodies: {world.Bodies.Count()}");
		// Bodies is the detached-body registry. Current actor bodies live on their owners.
		sb.AppendLine($"Attached bodies: {world.Actors.Concat(world.CachedActors).Concat(world.NPCs)
			.Select(x => x.Body).Where(x => x is not null).Distinct(ReferenceEqualityComparer.Instance).Count()}");
		sb.AppendLine($"Main schedules: {world.Scheduler.ScheduleCount}");
		sb.AppendLine($"Effect schedules: {world.EffectScheduler.ScheduleCount}");
		world.HeartbeatManager.AppendPerformanceReport(sb);
		if (target is not null)
		{
			sb.AppendLine($"Character: {target.Id}");
			target.AppendEventSubscriptionReport(sb);
			sb.AppendLine($"Following character: {(target.Following as ICharacter)?.Id ?? 0}");
			sb.AppendLine($"Following instance: {(target.Following as ICharacter)?.InstanceId ?? 0}");
			sb.AppendLine($"Followers: {world.Actors.Count(x => ReferenceEquals(x.Following, target))}");
		}
		actor.Send(sb.ToString());
	}
}

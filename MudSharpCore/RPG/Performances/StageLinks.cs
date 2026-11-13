#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using System.Runtime.CompilerServices;

namespace MudSharp.RPG.Performances;

/// <summary>Creates and releases directed room observation links used by stages.</summary>
public static class StageLinks
{
	private sealed class Counter { public long LastId; }
	private static readonly ConditionalWeakTable<IFuturemud,Counter> Counters = new();
	private static readonly object Sync = new();

	public static long Open(IRoom source, IRoom audience, string label) => OpenOwned(source, audience, label, null);

	/// <summary>Opens a link owned by a durable performance/run identity.</summary>
	public static long OpenOwned(IRoom source, IRoom audience, string label, string? runKey,
		RoomLayer? sourceLayer = null, RoomLayer? audienceLayer = null)
	{
		if (!IsSupported(source) || !IsSupported(audience) || source == audience || source.Gameworld != audience.Gameworld ||
		    string.IsNullOrWhiteSpace(label) || label.Length > 100 || label.Any(char.IsControl) ||
		    runKey is { Length: 0 } || runKey is { Length: > 200 } || runKey?.Any(char.IsControl) == true) return 0;
		lock (Sync)
		{
			var counter = Counters.GetValue(source.Gameworld, world => new Counter { LastId = world.Rooms
					.SelectMany(x => x.EffectsOfType<StageObservationEffect>())
					.SelectMany(x => x.Handles)
					.Select(x => x.Id)
					.DefaultIfEmpty(0)
					.Max() });
			if (counter.LastId == long.MaxValue) return 0;
			var id = ++counter.LastId;
			var effect = source.EffectsOfType<StageObservationEffect>().FirstOrDefault(x => x.AudienceRoomId == audience.Id);
			var handle = new StageObservationEffect.Handle(id, label.Trim(), runKey, sourceLayer, audienceLayer);
			if (effect is null) source.AddEffect(new StageObservationEffect(source, audience, id, label.Trim(), runKey,sourceLayer,audienceLayer));
			else effect.AddHandle(handle);
			return id;
		}
	}

	public static bool Close(IFuturemud world, long id)
	{
		if (world is null || id <= 0) return false;
		lock (Sync)
		{
			foreach (var room in world.Rooms)
			{
				var effect = room.EffectsOfType<StageObservationEffect>().FirstOrDefault(x => x.Handles.Any(y => y.Id == id));
				if (effect is null || !effect.RemoveHandle(id)) continue;
				if (effect.Handles.Count == 0) room.RemoveEffect(effect);
				return true;
			}
		}
		return false;
	}

	/// <summary>Removes all run-owned handles whose run was not recovered as active after boot.</summary>
	public static int ReconcileRunHandles(IFuturemud world, IReadOnlySet<string> activeRunKeys)
	{
		if (world is null || activeRunKeys is null) return 0;
		var removed = 0;
		lock (Sync)
		{
			foreach (var room in world.Rooms)
			{
				foreach (var effect in room.EffectsOfType<StageObservationEffect>().ToArray())
				{
					var before = effect.Handles.Count;
					effect.RemoveRunHandles(activeRunKeys);
					removed += before - effect.Handles.Count;
					if (effect.Handles.Count == 0) room.RemoveEffect(effect);
				}
			}
		}
		return removed;
	}

	internal static bool IsSupported(IRoom? room) => room is not null && room.RouteDefinition is null &&
		!room.EffectsOfType<RoomTemplateInstance>().Any(x => x.IsTemplate);
}

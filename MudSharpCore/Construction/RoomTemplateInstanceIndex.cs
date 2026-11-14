#nullable enable

using System.Runtime.CompilerServices;
using MudSharp.Effects.Concrete;

namespace MudSharp.Construction;

internal sealed class RoomTemplateInstanceIndex
{
	private static readonly ConditionalWeakTable<IFuturemud, RoomTemplateInstanceIndex> Indexes = new();
	private readonly IFuturemud _world;
	private readonly Dictionary<IRoom, long> _order = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IRoom, long[]> _memberships = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<long, SortedDictionary<long, IRoom>> _instances = new();
	private long _nextOrder;
	private bool _initialised;

	private RoomTemplateInstanceIndex(IFuturemud world) => _world = world;
	internal static RoomTemplateInstanceIndex For(IFuturemud world) => Indexes.GetValue(world, x => new(x));

	internal void Register(IRoom room)
	{
		if (!_order.TryAdd(room, _nextOrder)) return;
		_nextOrder++;
		Refresh(room);
	}

	internal void Refresh(IRoom room)
	{
		if (!_order.TryGetValue(room, out var order)) return;
		RemoveMemberships(room, order);
		var templates = room.EffectsOfType<RoomTemplateInstance>().Where(x => !x.IsTemplate)
			.Select(x => x.TemplateRoomId).Distinct().ToArray();
		_memberships[room] = templates;
		foreach (var template in templates)
		{
			if (!_instances.TryGetValue(template, out var rooms))
			{
				rooms = new SortedDictionary<long, IRoom>();
				_instances.Add(template, rooms);
			}
			rooms.Add(order, room);
		}
	}

	internal void Unregister(IRoom room)
	{
		if (!_order.Remove(room, out var order)) return;
		RemoveMemberships(room, order);
	}

	private void RemoveMemberships(IRoom room, long order)
	{
		if (!_memberships.Remove(room, out var templates)) return;
		foreach (var template in templates)
		{
			if (!_instances.TryGetValue(template, out var rooms)) continue;
			rooms.Remove(order);
			if (rooms.Count == 0) _instances.Remove(template);
		}
	}

	internal IReadOnlyList<IRoom> Get(long templateId)
	{
		if (!_initialised)
		{
			_initialised = true;
			foreach (var room in _world.Rooms) Register(room);
		}
		return _instances.TryGetValue(templateId, out var rooms) ? rooms.Values.ToArray() : [];
	}
}

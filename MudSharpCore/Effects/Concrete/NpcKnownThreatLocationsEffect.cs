#nullable enable
using MudSharp.Construction;
using System.Globalization;

namespace MudSharp.Effects.Concrete;

public sealed class NpcKnownThreatLocationsEffect : Effect
{
	private const int MaximumRememberedThreatLocations = 20;

	private readonly List<(long RoomId, DateTime RememberedAtUtc)> _knownThreatRoomIds = new();

	public NpcKnownThreatLocationsEffect(ICharacter owner)
		: base(owner)
	{
	}

	internal NpcKnownThreatLocationsEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect") ??
		                  throw new ArgumentException("Invalid NPC known-threat effect definition.");
		foreach (XElement room in effect.Elements("Cell"))
		{
			long id = long.Parse(room.Attribute("id")?.Value ?? room.Value);
			DateTime remembered = DateTime.TryParse(room.Attribute("utc")?.Value, CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime parsed)
				? parsed
				: RuntimeClock.UtcNow;
			if (_knownThreatRoomIds.All(x => x.RoomId != id))
			{
				_knownThreatRoomIds.Add((id, remembered));
			}
		}
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("NpcKnownThreatLocations", (effect, owner) => new NpcKnownThreatLocationsEffect(effect, owner));
	}

	public static NpcKnownThreatLocationsEffect GetOrCreate(ICharacter owner)
	{
		NpcKnownThreatLocationsEffect? existing = owner.CombinedEffectsOfType<NpcKnownThreatLocationsEffect>()
		                                               .FirstOrDefault();
		if (existing is not null)
		{
			return existing;
		}

		existing = new NpcKnownThreatLocationsEffect(owner);
		owner.AddEffect(existing);
		return existing;
	}

	public static NpcKnownThreatLocationsEffect? Get(ICharacter owner)
	{
		return owner.CombinedEffectsOfType<NpcKnownThreatLocationsEffect>().FirstOrDefault();
	}

	public IEnumerable<IRoom> KnownThreatLocations(TimeSpan memory)
	{
		Prune(memory);
		return _knownThreatRoomIds
		       .Select(x => Gameworld.Rooms.Get(x.RoomId))
		       .WhereNotNull(x => x);
	}

	public bool Knows(IRoom room, TimeSpan memory)
	{
		Prune(memory);
		return _knownThreatRoomIds.Any(x => x.RoomId == room.Id);
	}

	public void Remember(IRoom room)
	{
		_knownThreatRoomIds.RemoveAll(x => x.RoomId == room.Id);
		_knownThreatRoomIds.Insert(0, (room.Id, RuntimeClock.UtcNow));
		if (_knownThreatRoomIds.Count > MaximumRememberedThreatLocations)
		{
			_knownThreatRoomIds.RemoveRange(MaximumRememberedThreatLocations,
				_knownThreatRoomIds.Count - MaximumRememberedThreatLocations);
		}

		Changed = true;
	}

	public void Forget(IRoom room)
	{
		if (_knownThreatRoomIds.RemoveAll(x => x.RoomId == room.Id) == 0)
		{
			return;
		}

		Changed = true;
	}

	public void Prune(TimeSpan memory)
	{
		if (memory <= TimeSpan.Zero)
		{
			if (_knownThreatRoomIds.Count == 0)
			{
				return;
			}

			_knownThreatRoomIds.Clear();
			Changed = true;
			return;
		}

		DateTime cutoff = RuntimeClock.UtcNow.Subtract(memory);
		int removed = _knownThreatRoomIds.RemoveAll(x => x.RememberedAtUtc < cutoff);
		if (removed > 0)
		{
			Changed = true;
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			_knownThreatRoomIds.Select(x =>
				new XElement("Cell",
					new XAttribute("id", x.RoomId),
					new XAttribute("utc", x.RememberedAtUtc.ToString("O")))));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"NPC remembers threat locations in rooms {_knownThreatRoomIds.Select(x => x.RoomId.ToString("N0", voyeur)).ListToCommaSeparatedValues()}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "NpcKnownThreatLocations";
}

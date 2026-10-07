#nullable enable
using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public sealed class NpcKnownWaterLocationsEffect : Effect
{
	private const int MaximumRememberedWaterLocations = 20;

	private readonly List<long> _knownWaterRoomIds = new();

	public NpcKnownWaterLocationsEffect(ICharacter owner)
		: base(owner)
	{
	}

	private NpcKnownWaterLocationsEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect") ??
		                  throw new ArgumentException("Invalid NPC known-water effect definition.");
		_knownWaterRoomIds.AddRange(effect.Elements("Cell").Select(x => long.Parse(x.Value)).Distinct());
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("NpcKnownWaterLocations", (effect, owner) => new NpcKnownWaterLocationsEffect(effect, owner));
	}

	public static NpcKnownWaterLocationsEffect GetOrCreate(ICharacter owner)
	{
		NpcKnownWaterLocationsEffect? existing = owner.CombinedEffectsOfType<NpcKnownWaterLocationsEffect>()
		                                              .FirstOrDefault();
		if (existing is not null)
		{
			return existing;
		}

		existing = new NpcKnownWaterLocationsEffect(owner);
		owner.AddEffect(existing);
		return existing;
	}

	public static NpcKnownWaterLocationsEffect? Get(ICharacter owner)
	{
		return owner.CombinedEffectsOfType<NpcKnownWaterLocationsEffect>().FirstOrDefault();
	}

	public IEnumerable<IRoom> KnownWaterLocations => _knownWaterRoomIds
		.Select(x => Gameworld.Rooms.Get(x))
		.WhereNotNull(x => x);

	public bool Knows(IRoom room)
	{
		return _knownWaterRoomIds.Contains(room.Id);
	}

	public void Remember(IRoom room)
	{
		_knownWaterRoomIds.Remove(room.Id);
		_knownWaterRoomIds.Insert(0, room.Id);
		if (_knownWaterRoomIds.Count > MaximumRememberedWaterLocations)
		{
			_knownWaterRoomIds.RemoveRange(MaximumRememberedWaterLocations,
				_knownWaterRoomIds.Count - MaximumRememberedWaterLocations);
		}

		Changed = true;
	}

	public void Forget(IRoom room)
	{
		if (!_knownWaterRoomIds.Remove(room.Id))
		{
			return;
		}

		Changed = true;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", _knownWaterRoomIds.Select(x => new XElement("Cell", x)));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"NPC remembers water in cells {_knownWaterRoomIds.Select(x => x.ToString("N0", voyeur)).ListToCommaSeparatedValues()}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "NpcKnownWaterLocations";
}

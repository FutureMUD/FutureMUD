#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public sealed class NpcKnownWaterLocationsEffect : Effect
{
	private const int MaximumRememberedWaterLocations = 20;

	private readonly List<long> _knownWaterCellIds = new();

	public NpcKnownWaterLocationsEffect(ICharacter owner)
		: base(owner)
	{
	}

	private NpcKnownWaterLocationsEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect") ??
		                  throw new ArgumentException("Invalid NPC known-water effect definition.");
		_knownWaterCellIds.AddRange(effect.Elements("Cell").Select(x => long.Parse(x.Value)).Distinct());
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

	public IEnumerable<ICell> KnownWaterLocations => _knownWaterCellIds
		.Select(x => Gameworld.Cells.Get(x))
		.WhereNotNull(x => x);

	public bool Knows(ICell cell)
	{
		return _knownWaterCellIds.Contains(cell.Id);
	}

	public void Remember(ICell cell)
	{
		_knownWaterCellIds.Remove(cell.Id);
		_knownWaterCellIds.Insert(0, cell.Id);
		if (_knownWaterCellIds.Count > MaximumRememberedWaterLocations)
		{
			_knownWaterCellIds.RemoveRange(MaximumRememberedWaterLocations,
				_knownWaterCellIds.Count - MaximumRememberedWaterLocations);
		}

		Changed = true;
	}

	public void Forget(ICell cell)
	{
		if (!_knownWaterCellIds.Remove(cell.Id))
		{
			return;
		}

		Changed = true;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", _knownWaterCellIds.Select(x => new XElement("Cell", x)));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"NPC remembers water in cells {_knownWaterCellIds.Select(x => x.ToString("N0", voyeur)).ListToCommaSeparatedValues()}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "NpcKnownWaterLocations";
}

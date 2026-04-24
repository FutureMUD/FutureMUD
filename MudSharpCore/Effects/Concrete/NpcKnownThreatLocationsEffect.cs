#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public sealed class NpcKnownThreatLocationsEffect : Effect
{
	private const int MaximumRememberedThreatLocations = 20;

	private readonly List<(long CellId, DateTime RememberedAtUtc)> _knownThreatCellIds = new();

	public NpcKnownThreatLocationsEffect(ICharacter owner)
		: base(owner)
	{
	}

	internal NpcKnownThreatLocationsEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect") ??
		                  throw new ArgumentException("Invalid NPC known-threat effect definition.");
		foreach (XElement cell in effect.Elements("Cell"))
		{
			long id = long.Parse(cell.Attribute("id")?.Value ?? cell.Value);
			DateTime remembered = DateTime.TryParse(cell.Attribute("utc")?.Value, CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime parsed)
				? parsed
				: DateTime.UtcNow;
			if (_knownThreatCellIds.All(x => x.CellId != id))
			{
				_knownThreatCellIds.Add((id, remembered));
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

	public IEnumerable<ICell> KnownThreatLocations(TimeSpan memory)
	{
		Prune(memory);
		return _knownThreatCellIds
		       .Select(x => Gameworld.Cells.Get(x.CellId))
		       .WhereNotNull(x => x);
	}

	public bool Knows(ICell cell, TimeSpan memory)
	{
		Prune(memory);
		return _knownThreatCellIds.Any(x => x.CellId == cell.Id);
	}

	public void Remember(ICell cell)
	{
		_knownThreatCellIds.RemoveAll(x => x.CellId == cell.Id);
		_knownThreatCellIds.Insert(0, (cell.Id, DateTime.UtcNow));
		if (_knownThreatCellIds.Count > MaximumRememberedThreatLocations)
		{
			_knownThreatCellIds.RemoveRange(MaximumRememberedThreatLocations,
				_knownThreatCellIds.Count - MaximumRememberedThreatLocations);
		}

		Changed = true;
	}

	public void Forget(ICell cell)
	{
		if (_knownThreatCellIds.RemoveAll(x => x.CellId == cell.Id) == 0)
		{
			return;
		}

		Changed = true;
	}

	public void Prune(TimeSpan memory)
	{
		if (memory <= TimeSpan.Zero)
		{
			if (_knownThreatCellIds.Count == 0)
			{
				return;
			}

			_knownThreatCellIds.Clear();
			Changed = true;
			return;
		}

		DateTime cutoff = DateTime.UtcNow.Subtract(memory);
		int removed = _knownThreatCellIds.RemoveAll(x => x.RememberedAtUtc < cutoff);
		if (removed > 0)
		{
			Changed = true;
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			_knownThreatCellIds.Select(x =>
				new XElement("Cell",
					new XAttribute("id", x.CellId),
					new XAttribute("utc", x.RememberedAtUtc.ToString("O")))));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"NPC remembers threat locations in cells {_knownThreatCellIds.Select(x => x.CellId.ToString("N0", voyeur)).ListToCommaSeparatedValues()}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "NpcKnownThreatLocations";
}

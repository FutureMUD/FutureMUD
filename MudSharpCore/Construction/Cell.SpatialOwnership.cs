using MudSharp.GameItems;
using MudSharp.Form.Material;

#nullable enable

namespace MudSharp.Construction;

public partial class Cell
{
	private IZone _owningZone = null!;
	private (int X, int Y, int Z) _storedCoordinates;
	private readonly List<IArea> _areas = [];

	public IZone OwningZone => _owningZone;
	public IEnumerable<IArea> OwningAreas => _areas;
	public (int X, int Y, int Z) StoredCoordinates => _storedCoordinates;

	public void SetCoordinates(int x, int y, int z)
	{
		_storedCoordinates = (x, y, z);
		Changed = true;
	}

	public void SetNewZone(IZone zone)
	{
		ArgumentNullException.ThrowIfNull(zone);
		if (ReferenceEquals(zone, OwningZone)) return;
		if (_isCombatSimulationCell) throw new InvalidOperationException("A simulation cell's stored ownership cannot be rezoned.");
		using var exposureChange = EnvironmentalExposureService.ChangingDefinitions(Gameworld);
		var oldParents = new[] { OwningZone as Location, OwningZone.Shard as Location }
			.OfType<Location>().Distinct().ToArray();
		OwningZone.Unregister(this);
		_owningZone = zone;
		OwningZone.Register(this);
		var newParents = new[] { OwningZone as Location, OwningZone.Shard as Location }
			.OfType<Location>().Distinct().ToArray();
		foreach (var parent in oldParents.Concat(newParents).OfType<IZone>().Distinct())
			parent.Changed = true;

		// Ownership changes are callback-free. Preserve membership belonging to other cells,
		// including a canonical destination within the same shard.
		foreach (var parent in oldParents)
		{
			foreach (var item in _gameItems)
				parent.SetPreparedCurrencyMembership(item, parent.Cells.Any(x => x.GameItems.Any(y => ReferenceEquals(y, item))));
			foreach (var actor in _characters)
				parent.SetNativeCharacterMembership(actor, parent.Cells.Any(x => x.Characters.Any(y => ReferenceEquals(y, actor))));
		}
		foreach (var parent in newParents)
		{
			foreach (var item in _gameItems) parent.SetPreparedCurrencyMembership(item, true);
			foreach (var actor in _characters) parent.SetNativeCharacterMembership(actor, true);
		}
		RefreshWeatherSubscriptions();
		Changed = true;
	}

	public void AddArea(IArea area)
	{
		using var exposureChange = EnvironmentalExposureService.ChangingDefinitions(Gameworld);
		if (_areas.Contains(area)) return;
		_areas.Add(area);
		AreaAdded(area);
	}

	public void RemoveArea(IArea area)
	{
		using var exposureChange = EnvironmentalExposureService.ChangingDefinitions(Gameworld);
		if (_areas.Remove(area)) AreaRemoved(area);
	}
}

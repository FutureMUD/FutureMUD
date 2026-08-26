namespace TerrainPlanner.Contracts;

public sealed class MapHistory
{
	private readonly int _capacity;
	private readonly Stack<MapChangeSet> _undo = [];
	private readonly Stack<MapChangeSet> _redo = [];

	public MapHistory(int capacity = 100)
	{
		_capacity = Math.Max(1, capacity);
	}

	public bool CanUndo => _undo.Count > 0;
	public bool CanRedo => _redo.Count > 0;

	public void Record(MapChangeSet changeSet)
	{
		if (!changeSet.HasChanges)
		{
			return;
		}

		_undo.Push(changeSet);
		_redo.Clear();
		if (_undo.Count <= _capacity)
		{
			return;
		}

		var retained = _undo.Take(_capacity).Reverse().ToArray();
		_undo.Clear();
		foreach (var item in retained)
		{
			_undo.Push(item);
		}
	}

	public bool Undo(PlannerMap map)
	{
		if (!_undo.TryPop(out var changeSet))
		{
			return false;
		}

		changeSet.Undo(map);
		_redo.Push(changeSet);
		return true;
	}

	public bool Redo(PlannerMap map)
	{
		if (!_redo.TryPop(out var changeSet))
		{
			return false;
		}

		changeSet.Redo(map);
		_undo.Push(changeSet);
		return true;
	}

	public void Clear()
	{
		_undo.Clear();
		_redo.Clear();
	}
}

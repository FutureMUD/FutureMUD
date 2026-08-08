namespace MudSharp.Framework.Scheduling;

/// <summary>
/// A compact, stable min-heap for schedules. It deliberately has no tombstones or index map;
/// uncommon bulk mutations compact and heapify in one pass instead.
/// </summary>
internal sealed class StableScheduleHeap<TSchedule> where TSchedule : class
{
	private readonly List<Entry> _entries = [];
	private long _nextSequence;

	public int Count => _entries.Count;

	public void Add(DateTime triggerUtc, TSchedule schedule)
	{
		_entries.Add(new Entry(triggerUtc, _nextSequence++, schedule));
		SiftUp(_entries.Count - 1);
	}

	public bool TryPeek(out Entry entry)
	{
		if (_entries.Count == 0)
		{
			entry = default;
			return false;
		}

		entry = _entries[0];
		return true;
	}

	public bool TryDequeue(out Entry entry)
	{
		if (!TryPeek(out entry))
		{
			return false;
		}

		var lastIndex = _entries.Count - 1;
		if (lastIndex == 0)
		{
			_entries.RemoveAt(0);
			return true;
		}

		_entries[0] = _entries[lastIndex];
		_entries.RemoveAt(lastIndex);
		SiftDown(0);
		return true;
	}

	public TSchedule Find(Func<TSchedule, bool> predicate)
	{
		Entry? earliest = null;
		foreach (var entry in _entries)
		{
			if (predicate(entry.Schedule) &&
			    (earliest is null || Compare(entry, earliest.Value) < 0))
			{
				earliest = entry;
			}
		}

		return earliest?.Schedule;
	}

	public bool Any(Func<TSchedule, bool> predicate)
	{
		return Find(predicate) is not null;
	}

	public int RemoveAll(Func<TSchedule, bool> predicate)
	{
		var writeIndex = 0;
		var removed = 0;
		for (var readIndex = 0; readIndex < _entries.Count; readIndex++)
		{
			var entry = _entries[readIndex];
			if (predicate(entry.Schedule))
			{
				removed++;
				continue;
			}

			if (writeIndex != readIndex)
			{
				_entries[writeIndex] = entry;
			}

			writeIndex++;
		}

		if (removed == 0)
		{
			return 0;
		}

		_entries.RemoveRange(writeIndex, removed);
		Heapify();
		return removed;
	}

	public int UpdateAll(Func<TSchedule, bool> predicate, Action<TSchedule> update,
		Func<TSchedule, DateTime> triggerSelector)
	{
		var updated = 0;
		for (var index = 0; index < _entries.Count; index++)
		{
			var entry = _entries[index];
			if (!predicate(entry.Schedule))
			{
				continue;
			}

			update(entry.Schedule);
			_entries[index] = entry with { TriggerUtc = triggerSelector(entry.Schedule) };
			updated++;
		}

		if (updated > 0)
		{
			Heapify();
		}

		return updated;
	}

	public IReadOnlyList<Entry> SnapshotOrdered()
	{
		return _entries
			.OrderBy(x => x.TriggerUtc)
			.ThenBy(x => x.Sequence)
			.ToList();
	}

	private void Heapify()
	{
		for (var i = (_entries.Count / 2) - 1; i >= 0; i--)
		{
			SiftDown(i);
		}
	}

	private void SiftUp(int index)
	{
		while (index > 0)
		{
			var parent = (index - 1) / 2;
			if (Compare(_entries[parent], _entries[index]) <= 0)
			{
				return;
			}

			(_entries[parent], _entries[index]) = (_entries[index], _entries[parent]);
			index = parent;
		}
	}

	private void SiftDown(int index)
	{
		while (true)
		{
			var left = (index * 2) + 1;
			if (left >= _entries.Count)
			{
				return;
			}

			var right = left + 1;
			var smallest = right < _entries.Count && Compare(_entries[right], _entries[left]) < 0 ? right : left;
			if (Compare(_entries[index], _entries[smallest]) <= 0)
			{
				return;
			}

			(_entries[index], _entries[smallest]) = (_entries[smallest], _entries[index]);
			index = smallest;
		}
	}

	private static int Compare(Entry left, Entry right)
	{
		var triggerComparison = DateTime.Compare(left.TriggerUtc, right.TriggerUtc);
		return triggerComparison != 0 ? triggerComparison : left.Sequence.CompareTo(right.Sequence);
	}

	internal readonly record struct Entry(DateTime TriggerUtc, long Sequence, TSchedule Schedule);
}

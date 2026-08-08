using System.Collections;
using System.Threading;

namespace MudSharp.Network;

/// <summary>
/// Publishes immutable connection snapshots to the game and network loops. Connection churn is
/// uncommon, so copying on mutation removes repeated lock-and-copy work from the hot paths.
/// </summary>
internal sealed class ConnectionSnapshotRegistry : IEnumerable<IPlayerConnection>
{
	private readonly object _gate = new();
	private IPlayerConnection[] _snapshot = [];

	public IReadOnlyList<IPlayerConnection> Snapshot => Volatile.Read(ref _snapshot);

	public void Add(IPlayerConnection connection)
	{
		lock (_gate)
		{
			var snapshot = _snapshot;
			var updated = new IPlayerConnection[snapshot.Length + 1];
			Array.Copy(snapshot, updated, snapshot.Length);
			updated[^1] = connection;
			Volatile.Write(ref _snapshot, updated);
		}
	}

	public bool Remove(IPlayerConnection connection)
	{
		lock (_gate)
		{
			var snapshot = _snapshot;
			var index = Array.IndexOf(snapshot, connection);
			if (index < 0)
			{
				return false;
			}

			PublishWithout(snapshot, index);
			return true;
		}
	}

	public int RemoveClosed()
	{
		lock (_gate)
		{
			var snapshot = _snapshot;
			var remaining = 0;
			for (var i = 0; i < snapshot.Length; i++)
			{
				if (snapshot[i].State != ConnectionState.Closed)
				{
					remaining++;
				}
			}

			if (remaining == snapshot.Length)
			{
				return 0;
			}

			var updated = new IPlayerConnection[remaining];
			var writeIndex = 0;
			foreach (var connection in snapshot)
			{
				if (connection.State != ConnectionState.Closed)
				{
					updated[writeIndex++] = connection;
				}
			}

			Volatile.Write(ref _snapshot, updated);
			return snapshot.Length - remaining;
		}
	}

	public IEnumerator<IPlayerConnection> GetEnumerator()
	{
		return ((IEnumerable<IPlayerConnection>)Snapshot).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private void PublishWithout(IPlayerConnection[] snapshot, int excludedIndex)
	{
		var updated = new IPlayerConnection[snapshot.Length - 1];
		if (excludedIndex > 0)
		{
			Array.Copy(snapshot, updated, excludedIndex);
		}

		if (excludedIndex < snapshot.Length - 1)
		{
			Array.Copy(snapshot, excludedIndex + 1, updated, excludedIndex, snapshot.Length - excludedIndex - 1);
		}

		Volatile.Write(ref _snapshot, updated);
	}
}

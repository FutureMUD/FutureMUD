using System.Diagnostics;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using CharacterItem = MudSharp.Character.Character;
using NpcCharacter = MudSharp.NPC.NPC;

#nullable enable

namespace MudSharp.Combat.Simulation;

/// <summary>
/// Supplies deterministic, in-memory identities and discards ordinary persistence work for one combat simulation.
/// </summary>
/// <remarks>
/// A normal <see cref="SaveManager"/> gives materialised characters and items database identities. Even when the
/// surrounding transaction is rolled back, MySQL auto-increment counters advance, which makes later simulation runs
/// observably different. This manager instead allocates negative IDs in a stable construction order. Explicit
/// database work outside the save queue is still protected by the simulation transaction.
/// </remarks>
internal sealed class CombatSimulationSaveManager : ISaveManager
{
	private readonly List<ILateInitialisingItem> _initialisationQueue = [];
	private readonly Queue<ILazyLoadDuringIdleTime> _lazyLoaders = new();
	private readonly List<ISaveable> _saveQueue = [];
	private bool _flushing;
	private bool _mudBootingMode;
	private long _nextTransientId = -1;

	public bool Flushing => _flushing;

	public bool MudBootingMode
	{
		get => _mudBootingMode;
		set => _mudBootingMode = value;
	}

	public void Add(ISaveable item)
	{
		if (IsNoSave(item) || _saveQueue.Contains(item))
		{
			return;
		}

		_saveQueue.Add(item);
	}

	public void AddInitialisation(ILateInitialisingItem item)
	{
		if (item.GetNoSave() || item.IdHasBeenRegistered || _initialisationQueue.Contains(item))
		{
			return;
		}

		_initialisationQueue.Add(item);
	}

	public void AddLazyLoad(ILazyLoadDuringIdleTime item)
	{
		_lazyLoaders.Enqueue(item);
	}

	public void Abort(ISaveable item)
	{
		_saveQueue.Remove(item);
		if (item is ILateInitialisingItem lateItem)
		{
			_initialisationQueue.Remove(lateItem);
		}

		AbortLazyLoad(item as ILazyLoadDuringIdleTime);
	}

	public void AbortLazyLoad(ILazyLoadDuringIdleTime? item)
	{
		if (item is null || !_lazyLoaders.Contains(item))
		{
			return;
		}

		var remaining = _lazyLoaders.Where(x => !ReferenceEquals(x, item)).ToList();
		_lazyLoaders.Clear();
		foreach (var remainingItem in remaining)
		{
			_lazyLoaders.Enqueue(remainingItem);
		}
	}

	public void DirectInitialise(ILateInitialisingItem item)
	{
		if (item.GetNoSave())
		{
			Abort(item);
			return;
		}

		Initialise(item);
		Abort(item);
	}

	public string DebugInfo(IFuturemud gameworld)
	{
		return $"The combat simulation save manager is {(Flushing ? "initialising" : "idle")}; " +
		       $"{_initialisationQueue.Count:N0} item(s) await transient identities and " +
		       $"{_saveQueue.Count:N0} item(s) await discarded saves.";
	}

	public void Flush()
	{
		if (_flushing)
		{
			return;
		}

		_flushing = true;
		try
		{
			while (_initialisationQueue.Count > 0)
			{
				var queued = _initialisationQueue.ToList();
				_initialisationQueue.Clear();
				foreach (var phase in Enum.GetValues<InitialisationPhase>())
				{
					foreach (var item in queued.Where(x => x.InitialisationPhase == phase))
					{
						if (item.GetNoSave())
						{
							item.Changed = false;
							continue;
						}

						Initialise(item);
					}
				}
			}

			foreach (var item in _saveQueue)
			{
				item.Changed = false;
			}

			_saveQueue.Clear();
		}
		finally
		{
			_flushing = false;
		}
	}

	public void FlushLazyLoad(TimeSpan maximumTime)
	{
		var stopwatch = Stopwatch.StartNew();
		while (_lazyLoaders.TryDequeue(out var loader))
		{
			loader.DoLoad();
			if (stopwatch.Elapsed >= maximumTime)
			{
				return;
			}
		}
	}

	public bool IsQueued(ISaveable saveable)
	{
		return _saveQueue.Contains(saveable) ||
		       (saveable is ILateInitialisingItem lateItem && _initialisationQueue.Contains(lateItem)) ||
		       (saveable is ILazyLoadDuringIdleTime lazyLoad && _lazyLoaders.Contains(lazyLoad));
	}

	private void Initialise(ILateInitialisingItem item)
	{
		if (item.IdHasBeenRegistered)
		{
			return;
		}

		switch (item)
		{
			case NpcCharacter npc:
				npc.InitialiseCombatSimulationIdentity(NextId(), NextId(), NextId(), NextId());
				return;
			case CharacterItem character:
				character.InitialiseCombatSimulationIdentity(NextId(), NextId(), NextId());
				return;
			case LateInitialisingItem lateItem:
				lateItem.InitialiseWithoutPersistence(NextId());
				return;
			case LateKeywordedInitialisingItem lateKeywordedItem:
				lateKeywordedItem.InitialiseWithoutPersistence(NextId());
				return;
			default:
				throw new InvalidOperationException(
					$"{item.FrameworkItemType} cannot be given a transient combat-simulation identity.");
		}
	}

	private long NextId()
	{
		return _nextTransientId--;
	}

	private static bool IsNoSave(ISaveable item)
	{
		return item switch
		{
			SaveableItem saveableItem => saveableItem.GetNoSave(),
			SavableKeywordedItem saveableKeywordedItem => saveableKeywordedItem.GetNoSave(),
			LateInitialisingItem lateInitialisingItem => lateInitialisingItem.GetNoSave(),
			LateKeywordedInitialisingItem lateKeywordedInitialisingItem => lateKeywordedInitialisingItem.GetNoSave(),
			_ => false
		};
	}
}

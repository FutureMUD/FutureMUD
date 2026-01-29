#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;

namespace MudSharp.Arenas;

/// <summary>
/// 	Mirrors arena combat output into observation rooms while enforcing perception gating.
/// </summary>
public sealed class ArenaWatcherEffect : Effect, IRemoteObservationEffect
{
	private const int AdditionalNoticeStages = 2;

	private readonly IArenaEvent _arenaEvent;
	private readonly Dictionary<ICharacter, ICell> _watchers = new();

	public ArenaWatcherEffect(ICell owner, IArenaEvent arenaEvent) : base(owner)
	{
		_arenaEvent = arenaEvent ?? throw new ArgumentNullException(nameof(arenaEvent));
	}

	public IArenaEvent ArenaEvent => _arenaEvent;

	protected override string SpecificEffectType => "ArenaWatcher";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Mirroring arena event {_arenaEvent.Name.ColourName()} to observation rooms.";
	}

	public void AddWatcher(ICharacter watcher, ICell observationCell)
	{
		if (watcher is null)
		{
			return;
		}

		if (observationCell is null)
		{
			throw new ArgumentNullException(nameof(observationCell));
		}

		var effectiveCell = observationCell;
		if (watcher.Location is ICell currentCell &&
				_arenaEvent.Arena.ObservationCells.Contains(currentCell))
		{
			effectiveCell = currentCell;
		}

		if (_watchers.ContainsKey(watcher))
		{
			_watchers[watcher] = effectiveCell;
			return;
		}

		_watchers.Add(watcher, effectiveCell);
		RegisterWatcher(watcher);
	}

	public void RemoveWatcher(ICharacter watcher)
	{
		if (watcher is null)
		{
			return;
		}

		if (!_watchers.Remove(watcher))
		{
			return;
		}

		UnregisterWatcher(watcher);
		TryDeactivate();
	}

	public override void RemovalEffect()
	{
		foreach (var watcher in _watchers.Keys.ToList())
		{
			UnregisterWatcher(watcher);
		}

		_watchers.Clear();
	}

	public void HandleOutput(string text, ILocation location)
	{
		// TODO
	}

	public void HandleOutput(IOutput output, ILocation location)
	{
		if (output is null)
		{
			return;
		}

		if (_arenaEvent.State is ArenaEventState.Completed or ArenaEventState.Aborted)
		{
			Owner.RemoveEffect(this);
			return;
		}

		PruneInvalidWatchers();
		if (_watchers.Count == 0)
		{
			TryDeactivate();
			return;
		}

		var forwarded = PrepareForwardedOutput(output);
		var forwardedEmote = forwarded as EmoteOutput;

		foreach (var watcher in _watchers.Keys.ToList())
		{
			if (!forwarded.ShouldSee(watcher))
			{
				continue;
			}

			if (forwardedEmote is not null)
			{
				watcher.SeeTarget(forwardedEmote.DefaultSource as IMortalPerceiver);
			}

			var prefix = BuildPrefix(watcher, location);
			watcher.OutputHandler.Send(new PrependOutputWrapper(forwarded, prefix));
		}
	}

	private void PruneInvalidWatchers()
	{
		var removed = false;
		foreach (var (watcher, observationCell) in _watchers.ToList())
		{
			if (!IsWatcherValid(watcher, observationCell))
			{
				_watchers.Remove(watcher);
				UnregisterWatcher(watcher);
				removed = true;
			}
		}

		if (removed)
		{
			TryDeactivate();
		}
	}

	private bool IsWatcherValid(ICharacter watcher, ICell observationCell)
	{
		if (!watcher.State.HasFlag(CharacterState.Conscious))
		{
			return false;
		}

		if (watcher.Location is not ICell currentCell)
		{
			return false;
		}

		if (!_arenaEvent.Arena.ObservationCells.Contains(currentCell))
		{
			return false;
		}

		if (!ReferenceEquals(currentCell, observationCell))
		{
			_watchers[watcher] = currentCell;
		}

		return true;
	}

	private void RegisterWatcher(ICharacter watcher)
	{
		watcher.OnQuit -= WatcherInvalidated;
		watcher.OnQuit += WatcherInvalidated;
		watcher.OnDeleted -= WatcherInvalidated;
		watcher.OnDeleted += WatcherInvalidated;
		watcher.OnDeath -= WatcherInvalidated;
		watcher.OnDeath += WatcherInvalidated;
		watcher.OnStateChanged -= WatcherStateChanged;
		watcher.OnStateChanged += WatcherStateChanged;
		watcher.OnMoved -= WatcherMoved;
		watcher.OnMoved += WatcherMoved;
	}

	private void UnregisterWatcher(ICharacter watcher)
	{
		watcher.OnQuit -= WatcherInvalidated;
		watcher.OnDeleted -= WatcherInvalidated;
		watcher.OnDeath -= WatcherInvalidated;
		watcher.OnStateChanged -= WatcherStateChanged;
		watcher.OnMoved -= WatcherMoved;
	}

	private void WatcherInvalidated(IPerceivable owner)
	{
		if (owner is ICharacter watcher)
		{
			RemoveWatcher(watcher);
		}
	}

	private void WatcherStateChanged(IPerceivable owner)
	{
		if (owner is ICharacter watcher && _watchers.ContainsKey(watcher) &&
			!watcher.State.HasFlag(CharacterState.Conscious))
		{
			RemoveWatcher(watcher);
		}
	}

	private void WatcherMoved(object? sender, MoveEventArgs e)
	{
		if (sender is not ICharacter watcher)
		{
			return;
		}

		if (!_watchers.ContainsKey(watcher))
		{
			return;
		}

		if (watcher.Location is ICell currentCell &&
				_arenaEvent.Arena.ObservationCells.Contains(currentCell))
		{
			_watchers[watcher] = currentCell;
			return;
		}

		RemoveWatcher(watcher);
	}

	private IOutput PrepareForwardedOutput(IOutput original)
	{
		if (original is not EmoteOutput emote)
		{
			return original;
		}

		var forwarded = new EmoteOutput(emote);
		if (forwarded.Flags.HasFlag(OutputFlags.NoticeCheckRequired))
		{
			forwarded.NoticeCheckDifficulty = forwarded.NoticeCheckDifficulty.StageUp(AdditionalNoticeStages);
		}
		else if (forwarded.Flags.HasFlag(OutputFlags.PurelyAudible))
		{
			forwarded.Flags |= OutputFlags.NoticeCheckRequired;
			forwarded.NoticeCheckDifficulty = Difficulty.Normal.StageUp(AdditionalNoticeStages);
		}

		return forwarded;
	}

	private string BuildPrefix(ICharacter watcher, ILocation location)
	{
		var arenaName = _arenaEvent.Arena.Name.ColourName();
		var cellName = (location?.HowSeen(watcher) ?? "the arena floor").ColourName();
		return $"[{arenaName} @ {cellName}] ";
	}

	private void TryDeactivate()
	{
		if (_watchers.Count == 0)
		{
			Owner.RemoveEffect(this);
		}
	}
}

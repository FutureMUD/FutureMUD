#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems;

namespace MudSharp.Framework;

/// <summary>
/// Bounds synchronous native evacuation callbacks to the graph whose custody can be restored.
/// Destruction, merging and introducing a new custodian require a separate verified adapter.
/// </summary>
internal static class ForeignCustodyTransferContext
{
	private static readonly AsyncLocal<Scope?> Current = new();

	internal static Scope Enter(IBody body, IEnumerable<IGameItem> items, ICell destination)
	{
		if (Current.Value is not null) throw new InvalidOperationException("Nested foreign custody transfers require a separate adapter.");
		var scope = new Scope(body, new HashSet<IGameItem>(items, ReferenceEqualityComparer.Instance), destination);
		Current.Value = scope;
		return scope;
	}

	internal static void EnsureItem(IGameItem item, bool destructive = false)
	{
		if (Current.Value is not { } scope) return;
		if (destructive || !scope.Items.Contains(item))
			throw new InvalidOperationException("A native transfer callback attempted unsupported destruction or foreign custody outside its captured graph.");
	}

	internal static void EnsureBody(IBody body, IGameItem item)
	{
		EnsureItem(item);
		if (Current.Value is { } scope && !ReferenceEquals(scope.Body, body))
			throw new InvalidOperationException("A native transfer callback attempted an uncaptured body custodian.");
	}

	internal static void EnsurePair(IGameItem parent, IGameItem child)
	{
		EnsureItem(parent); EnsureItem(child);
	}

	internal static void EnsureCell(ICell cell, IGameItem item)
	{
		EnsureItem(item);
		if (Current.Value is { } scope && !ReferenceEquals(scope.Destination, cell))
			throw new InvalidOperationException("A native transfer callback attempted an uncaptured cell destination.");
	}

	internal static void RecordSave(PerceivedItem item, long? persistedItemId = null)
	{
		if (Current.Value is not { } scope) return;
		if (item is IGameItem gameItem)
		{
			EnsureItem(gameItem);
			if (persistedItemId.HasValue && persistedItemId != gameItem.Id)
				throw new InvalidOperationException("A native transfer attempted to save resources to an uncaptured item row.");
		}
		else if (!ReferenceEquals(scope.Body, item))
			throw new InvalidOperationException("A native transfer attempted to save an uncaptured body.");
		scope.SaveRollbacks.Add(item.CaptureCustodySaveRollback());
	}

	internal static void RecordSave(GameItemComponent component)
	{
		if (Current.Value is not { } scope) return;
		EnsureItem(component.Parent);
		// Native components save their whole definition and consume only Changed.
		scope.SaveRollbacks.Add(() =>
		{
			component.Changed = true;
			if (!component.Gameworld.SaveManager.IsQueued(component)) component.Gameworld.SaveManager.Add(component);
		});
	}

	internal static void RecordNeedsSave(MudSharp.Body.Implementations.Body body)
	{
		if (Current.Value is not { } scope) return;
		if (!ReferenceEquals(scope.Body, body))
			throw new InvalidOperationException("A native transfer attempted to save uncaptured needs state.");
		scope.SaveRollbacks.Add(body.CaptureNeedsSaveRollback());
	}

	internal static void EnsureFlushOutsideTransfer()
	{
		if (Current.Value is not null)
			throw new InvalidOperationException("A native custody transaction cannot flush unrelated save queues.");
	}

	internal sealed class Scope(IBody body, HashSet<IGameItem> items, ICell destination) : IDisposable
	{
		internal IBody Body { get; } = body;
		internal HashSet<IGameItem> Items { get; } = items;
		internal ICell Destination { get; } = destination;
		internal List<Action> SaveRollbacks { get; } = [];
		internal void RestorePendingSaves(Action<Action> recover)
		{
			// A callback can save repeatedly. Undo save bookkeeping in reverse order,
			// retaining every consumed flag and the first needs-save counter state.
			for (var i = SaveRollbacks.Count - 1; i >= 0; i--) recover(SaveRollbacks[i]);
			SaveRollbacks.Clear();
		}
		public void Dispose()
		{
			if (ReferenceEquals(Current.Value, this)) Current.Value = null;
			SaveRollbacks.Clear();
		}
	}
}

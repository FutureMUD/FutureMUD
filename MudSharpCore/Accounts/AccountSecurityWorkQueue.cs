#nullable enable
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Menus;

namespace MudSharp.Accounts;

/// <summary>Only CPU work runs off-thread. Database access, menu transitions and output remain on the game thread.</summary>
internal static class AccountSecurityWorkQueue
{
	private sealed class WorldQueue
	{
		internal readonly ConcurrentQueue<Action> Completed = new();
		internal int Pending;
	}
	private static readonly ConditionalWeakTable<IFuturemud, WorldQueue> Queues = new();
	private static readonly SemaphoreSlim Workers = new(2, 2);

	internal static bool TryQueue<T>(Menu owner, Func<T> work, Action<T> complete)
	{
		var queue = Queues.GetOrCreateValue(owner.Gameworld);
		if (Interlocked.Increment(ref queue.Pending) > 32)
		{
			Interlocked.Decrement(ref queue.Pending);
			return false;
		}
		var controller = owner.Controller;
		using (ExecutionContext.SuppressFlow())
		{
		_ = Task.Run(async () =>
		{
			T result = default!;
			var success = false;
			await Workers.WaitAsync().ConfigureAwait(false);
			try { result = work(); success = true; }
			catch (Exception) { /* Never retain a credential-bearing exception or invoke engine APIs here. */ }
			finally { Workers.Release(); }
			queue.Completed.Enqueue(() =>
			{
				try
				{
					if (owner.OutputHandler is null || controller is IPlayerController { Closing: true } ||
					    controller is FuturemudControlContext live && !live.IsControlling(owner)) return;
					try
					{
						if (success) complete(result);
						else owner.SecurityWorkFailed();
					}
					catch (Exception) { owner.SecurityWorkFailed(); }
					(controller as FuturemudControlContext)?.ApplyPendingContext();
				}
				finally { Interlocked.Decrement(ref queue.Pending); }
			});
		});
		}
		return true;
	}

	internal static int PendingCount(IFuturemud world) => Queues.TryGetValue(world, out var queue) ? Volatile.Read(ref queue.Pending) : 0;

	internal static void ProcessCompleted(IFuturemud world)
	{
		if (!Queues.TryGetValue(world, out var queue)) return;
		while (queue.Completed.TryDequeue(out var action)) action();
	}
}

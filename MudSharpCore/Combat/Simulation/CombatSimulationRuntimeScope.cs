using System.Threading;
using ExpressionEngine;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;

#nullable enable

namespace MudSharp.Combat.Simulation;

/// <summary>
/// Replaces only the mutable runtime services used by a combat simulation on the current execution flow.
/// The live game loop and other threads continue to see their ordinary services.
/// </summary>
internal sealed class CombatSimulationRuntimeScope : IDisposable
{
	private static readonly AsyncLocal<CombatSimulationRuntimeScope?> _current = new();
	private readonly IDisposable _clockScope;
	private readonly IDisposable _expressionRandomScope;
	private readonly IDisposable _randomScope;
	private readonly IDisposable _sideEffectScope;
	private readonly AsyncFlowControl? _executionContextFlowControl;
	private bool _disposed;

	public CombatSimulationRuntimeScope(IFuturemud gameworld, TimeProvider timeProvider, int seed,
		CombatSimulationExecutionFingerprint fingerprint)
	{
		if (_current.Value is not null)
		{
			throw new InvalidOperationException("A combat simulation runtime scope is already active.");
		}

		TimeProvider = timeProvider;
		Scheduler = new Scheduler(timeProvider);
		EffectScheduler = new EffectScheduler(gameworld, timeProvider);
		SaveManager = new CombatSimulationSaveManager();
		HeartbeatManager = new HeartbeatManager(gameworld);
		ExecutionFingerprint = fingerprint;
		var random = new CombatSimulationRecordingRandom(new Random(seed), fingerprint);
		// The accelerated simulation runs synchronously on the game loop. Do not let any background
		// task created by an on-load prog, effect or hook inherit this virtual clock/scheduler/random stream.
		if (!ExecutionContext.IsFlowSuppressed())
		{
			_executionContextFlowControl = ExecutionContext.SuppressFlow();
		}

		_clockScope = RuntimeClock.Push(timeProvider);
		_randomScope = Constants.PushRandom(random);
		_expressionRandomScope = Expression.PushRandom(random);
		_sideEffectScope = RuntimeSideEffectContext.SuppressCrimeCreation();
		_current.Value = this;
	}

	public static CombatSimulationRuntimeScope? Current => _current.Value;
	public TimeProvider TimeProvider { get; }
	public IScheduler Scheduler { get; }
	public IEffectScheduler EffectScheduler { get; }
	public ISaveManager SaveManager { get; }
	public IHeartbeatManager HeartbeatManager { get; }
	public CombatSimulationExecutionFingerprint ExecutionFingerprint { get; }

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_current.Value = null;
		_sideEffectScope.Dispose();
		_expressionRandomScope.Dispose();
		_randomScope.Dispose();
		_clockScope.Dispose();
		_executionContextFlowControl?.Dispose();
		_disposed = true;
	}
}

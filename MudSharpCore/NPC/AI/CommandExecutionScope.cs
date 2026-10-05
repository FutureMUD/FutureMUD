#nullable enable

using System.Threading;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Body.Traits;

namespace MudSharp.NPC.AI;

/// <summary>Execution-local continuation checks, separate from accepted-order capture.</summary>
internal sealed class CommandExecutionScope : IDisposable
{
	private static readonly AsyncLocal<CommandExecutionScope?> Current = new();
	private readonly CommandExecutionScope? _previous;
	private readonly IDisposable? _captureSuspension;
	private readonly CommandExecutionAuthority? _authority;
	private readonly ICharacter? _executor;
	private readonly ICombatMove? _move;
	private bool _rejected;
	private bool _committed;
	private bool _disposed;
	private NativeDisplacementReceipt? _displacement;

	private CommandExecutionScope(CommandExecutionAuthority? authority, ICharacter? executor, ICombatMove? move, bool suspendCapture)
	{
		_previous = Current.Value;
		_authority = authority;
		_executor = executor;
		_move = move;
		_captureSuspension = suspendCapture ? CommandExecutionAuthority.SuspendCapture() : null;
		Current.Value = this;
	}

	internal static IDisposable EnterFactory(CommandExecutionAuthority? authority, ICharacter actor) => new CommandExecutionScope(authority, actor, null, authority is null);
	internal static IDisposable EnterDispatch(CommandExecutionAuthority authority, ICharacter actor) => new CommandExecutionScope(authority, actor, null, false);
	internal static IDisposable EnterMove(ICombatMove move)
	{
		if (ReferenceEquals(Current.Value?._move, move)) return NoScope.Instance;
		var bound = CommandExecutionAuthority.ForMove(move);
		return new CommandExecutionScope(bound?.Authority, bound?.Executor ?? move.Assailant, move, bound is null);
	}
	internal static IDisposable EnterIndependent() => new CommandExecutionScope(null, null, null, true);
	internal static object? LearningContext => Current.Value;
	internal static Continuation CaptureContinuation(ICharacter executor) =>
		new(Current.Value?._authority, executor);

	internal sealed class Continuation(CommandExecutionAuthority? authority, ICharacter executor)
	{
		internal IDisposable Enter() => new CommandExecutionScope(authority, executor, null, authority is null);
	}
	internal static IDisposable EnterOwnedOperation(ICharacter executor) =>
		Current.Value is { _authority: not null } scope
			? new CommandExecutionScope(scope._authority, executor, null, false)
			: NoScope.Instance;
	internal static IDisposable EnterBodyOperation(ICharacter actor) =>
		Current.Value is { _authority: not null } scope && !ReferenceEquals(actor, scope._executor)
			? EnterIndependent()
			: NoScope.Instance;

	internal static bool TryContinue(ICharacter? executor = null)
	{
		var scope = Current.Value;
		if (scope?._authority is null || executor is not null && !ReferenceEquals(executor, scope._executor)) return true;
		return scope.Continue();
	}

	// A check on a foreign defender is independent. Match physical references captured
	// at execution entry, never a canonical owner or a body's actor after a callback.
	internal static Func<bool>? CaptureLearningContinuation(IHaveTraits user)
	{
		var scope = Current.Value;
		return scope?._authority?.IsLearningPrincipal(user) == true
			? scope.Continue : null;
	}

	private bool Continue()
	{
		if (_disposed || _rejected) return false;
		var valid = _move is not null
			? CommandExecutionAuthority.MayExecute(_move, _executor)
			: _authority!.MayExecutePrincipal();
		if (valid && !_rejected) return true;
		_rejected = true;
		return false;
	}


	// The receipt is an explicit capability, never an ambient membership exemption.
	internal static NativeDisplacementReceipt? BeginDisplacement(ICharacter actor, ICell destination, RoomLayer layer, double? routePosition = null)
	{
		var scope = Current.Value;
		if (scope?._authority is null || !ReferenceEquals(actor, scope._executor)) return null;
		if (scope._displacement is not null || !TryContinue(actor))
		{
			scope._rejected = true;
			return null;
		}
		return scope._displacement = new NativeDisplacementReceipt(scope, actor, destination, layer, routePosition);
	}

	internal bool ContinueDisplacement(NativeDisplacementReceipt receipt)
	{
		if (!ReferenceEquals(Current.Value, this) || !ReferenceEquals(_displacement, receipt) ||
		    _disposed || _rejected || !receipt.ExecutorsValid()) return false;
		var valid = _move is null
			? _authority?.MayExecutePrincipal(receipt) == true
			: CommandExecutionAuthority.ForMove(_move)?.MayExecute(receipt) == true;
		if (valid && !_rejected && receipt.ExecutorsValid()) return true;
		_rejected = true;
		return false;
	}

	internal void EndDisplacement(NativeDisplacementReceipt receipt, bool completed)
	{
		if (!ReferenceEquals(_displacement, receipt)) return;
		if (!completed) _rejected = true;
		_displacement = null;
	}

	internal static T EvaluateCallback<T>(Func<T> callback, T refused)
	{
		if (!TryContinue()) return refused;
		var result = callback();
		return TryContinue() ? result : refused;
	}

	internal static T InvokeOwned<T>(ICharacter executor, Func<T> operation)
	{
		using var owned = EnterOwnedOperation(executor);
		return operation();
	}

	internal static bool RejectedBeforeCommit => Current.Value is { _rejected: true, _committed: false };
	internal static bool HasCommitted => Current.Value is { _committed: true };
	internal static bool IsOrdered(ICharacter actor) => Current.Value is { _authority: not null } scope && ReferenceEquals(actor, scope._executor);
	internal static void MarkCommitted(ICharacter? executor = null)
	{
		if (Current.Value is { } scope && (executor is null || ReferenceEquals(executor, scope._executor)))
			scope._committed = true;
	}

	internal static CombatMoveResult Resolve(ICombatMove move, ICombatMove? response)
	{
		using var execution = EnterMove(move);
		if (!TryContinue()) return RefusedResolution(move);
		var result = move.ResolveMove(response!);
		if (RejectedBeforeCommit) return RefusedResolution(move);
		if (!ReferenceEquals(result, CombatMoveResult.Irrelevant)) MarkCommitted();
		return result;
	}

	private static CombatMoveResult RefusedResolution(ICombatMove move)
	{
		// CombatBase queries stamina before its outer EnterMove scope is disposed.
		if (RejectedBeforeCommit && move is CombatMoveBase native) native.RejectUnexecutedCommand();
		return CombatMoveResult.Irrelevant;
	}

	internal static CombatMoveResult ResolveIndependent(ICombatMove move, Func<ICombatMove?> response)
	{
		using var independent = EnterIndependent();
		CommandExecutionAuthority.ForgetIndependent(move);
		return Resolve(move, response());
	}

	internal static CombatMoveResult ResolveOwned(ICombatMove parent, ICombatMove child, Func<ICombatMove?> response)
	{
		CommandExecutionAuthority.Inherit(parent, child);
		using var execution = EnterMove(child);
		if (!TryContinue()) return CombatMoveResult.Irrelevant;
		var defense = response();
		if (!TryContinue()) return CombatMoveResult.Irrelevant;
		return Resolve(child, defense);
	}

	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		if (_rejected && !_committed && _move is CombatMoveBase move) move.RejectUnexecutedCommand();
		if (_authority is not null && ReferenceEquals(_previous?._authority, _authority))
		{
			_previous!._rejected |= _rejected;
			_previous._committed |= _committed;
		}
		Current.Value = _previous;
		_captureSuspension?.Dispose();
	}

	private sealed class NoScope : IDisposable
	{
		internal static readonly NoScope Instance = new();
		public void Dispose() { }
	}
}

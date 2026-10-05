#nullable enable
using System.Threading;
using MudSharp.Body.Traits;
using MudSharp.NPC.AI;

namespace MudSharp.RPG.Checks;

/// <summary>Pins the actual check user to the original order while native learning runs.</summary>
internal sealed class CheckLearningScope : IDisposable
{
	private static readonly AsyncLocal<CheckLearningScope?> Current = new();
	private static readonly AsyncLocal<TraitWrite?> CurrentWrite = new();
	private readonly CheckLearningScope? _previous;
	private readonly IHaveTraits _user;
	private readonly Func<bool>? _continue;
	private readonly object? _executionContext;

	private CheckLearningScope(IHaveTraits user)
	{
		_user = user;
		_previous = Current.Value;
		_continue = CommandExecutionScope.CaptureLearningContinuation(user);
		_executionContext = CommandExecutionScope.LearningContext;
		Current.Value = this;
	}

	internal static IDisposable Enter(IHaveTraits user) => new CheckLearningScope(user);
	internal static IDisposable EnterIfNeeded(IHaveTraits user) =>
		ReferenceEquals(Current.Value?._user, user) &&
		ReferenceEquals(Current.Value?._executionContext, CommandExecutionScope.LearningContext)
			? NoScope.Instance : Enter(user);
	internal static bool CanContinue(IHaveTraits user) =>
		!ReferenceEquals(Current.Value?._user, user) || Current.Value!._continue?.Invoke() != false;

	// Only the native TraitUsed write is guarded. Direct trait edits and foreign callback
	// participants retain their own semantics; canonical trait.Owner is not the principal.
	internal static TraitWrite EnterTraitWrite(ITrait trait, IHaveTraits user) => new(trait, user);
	internal static ValueWrite EnterValueWrite(ITrait trait) => new(trait);

	internal sealed class ValueWrite : IDisposable
	{
		private readonly TraitWrite? _write;
		internal ValueWrite(ITrait trait)
		{
			if (CurrentWrite.Value is not { } write || !ReferenceEquals(write.Trait, trait)) return;
			_write = write;
			// Consume the final native assignment's capability. Nested direct setters
			// inside cap callbacks are separate writes, even to this same canonical trait.
			CurrentWrite.Value = null;
		}
		internal bool CanContinue() => _write?.Scope?._continue?.Invoke() != false;
		internal void Resume() { if (_write is not null) CurrentWrite.Value = _write; }
		internal void RecordChange(bool changed) { if (_write is not null) _write.Applied = changed; }
		public void Dispose() { if (_write is not null) CurrentWrite.Value = _write; }
	}

	public void Dispose() => Current.Value = _previous;

	internal sealed class TraitWrite : IDisposable
	{
		private readonly TraitWrite? _previous;
		internal readonly ITrait Trait;
		internal readonly CheckLearningScope? Scope;
		internal bool Applied;
		internal TraitWrite(ITrait trait, IHaveTraits user)
		{
			Trait = trait;
			Scope = ReferenceEquals(Current.Value?._user, user) ? Current.Value : null;
			_previous = CurrentWrite.Value;
			CurrentWrite.Value = this;
		}
		public void Dispose() => CurrentWrite.Value = _previous;
	}

	private sealed class NoScope : IDisposable
	{
		internal static readonly NoScope Instance = new();
		public void Dispose() { }
	}
}

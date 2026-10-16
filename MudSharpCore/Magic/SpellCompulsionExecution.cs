#nullable enable

using System.Threading;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete.SpellEffects;

namespace MudSharp.Magic;

/// <summary>One exact synchronous native IC action. No alias resolution, nested command or queued grant.</summary>
internal sealed class SpellCompulsionExecution : IDisposable
{
	private static readonly AsyncLocal<SpellCompulsionExecution?> Current = new();
	private readonly SpellCompulsionExecution? _previous;
	private readonly ICharacter _controller, _target;
	private readonly SpellCompellingRegardEffect _effect;
	private readonly IExecutable<ICharacter> _command;
	private readonly string _input;
	private bool _consumed, _disposed;
	internal static bool RejectNestedCommand => Current.Value is not null;
	private SpellCompulsionExecution(ICharacter controller, ICharacter target, SpellCompellingRegardEffect effect, IExecutable<ICharacter> command, string input)
	{ _previous = Current.Value; _controller = controller; _target = target; _effect = effect; _command = command; _input = input; Current.Value = this; }
	internal static IExecutable<ICharacter>? PermittedNativeCommand(string word) => word switch
	{
		"look" => PerceptionModule.Instance.Commands.TCommands.GetValueOrDefault("look"),
		"drop" or "give" => InventoryModule.Instance.Commands.TCommands.GetValueOrDefault(word),
		"stand" or "sit" or "kneel" or "prostrate" or "lounge" or "rest" or "sprawl" or "prone" => PositionModule.Instance.Commands.TCommands.GetValueOrDefault(word),
		_ => null
	};
	internal static bool Execute(ICharacter controller, ICharacter target, SpellCompellingRegardEffect effect, string input)
	{
		if (Current.Value is not null || input.Length is < 1 or > 512 || input.Any(char.IsControl)) return false;
		var word = new StringStack(input).PopSpeech();
		var command = PermittedNativeCommand(word);
		if (command is null || target.CommandTree.Commands is not CharacterCommandManager manager ||
			!manager.TCommands.TryGetValue(word, out var resolved) || !ReferenceEquals(resolved, command) || !effect.CanControl(controller)) return false;
		using var scope = new SpellCompulsionExecution(controller, target, effect, command, input);
		using var speech = MagicSpeechContext.Suppress();
		return manager.Execute(target, command, input, target.State, target.PermissionLevel, target.OutputHandler);
	}
	// Command<T> calls this immediately before the native delegate; any nested or stale entry refuses.
	internal static bool Admit(ICharacter actor, object command, string input)
	{
		if (Current.Value is not { } scope) return true;
		if (scope._disposed || scope._consumed || !ReferenceEquals(actor, scope._target) || !ReferenceEquals(command, scope._command) || input != scope._input) return false;
		scope._consumed = true;
		return scope._effect.CanControl(scope._controller) && !scope._disposed;
	}
	public void Dispose() { _disposed = true; Current.Value = _previous; }
}

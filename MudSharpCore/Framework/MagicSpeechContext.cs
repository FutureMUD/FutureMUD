using System.Threading;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.Framework;

/// <summary>Executable authority lives only on the original player command, never on output or speech events.</summary>
internal static class MagicSpeechContext
{
	private static readonly AsyncLocal<Frame?> Current = new();
	private sealed class Frame
	{
		public Frame? Parent;
		public ICharacter? Actor;
		public Guid Id;
		public bool Input;
		public bool Speech;
		public bool Command;
		public bool Generated;
		public bool Closed;
		public int Claimed;
		public string? Method;
		public string? Message;
		public string? FormulaText;
		public bool Emitted;
	}

	internal sealed class Scope : IDisposable
	{
		private readonly Frame _frame;
		private readonly Frame? _previous;
		private int _disposed;
		private Scope(Frame frame) { _previous = Current.Value; _frame = frame; Current.Value = frame; }
		public bool Emitted => _frame.Emitted;
		public void Dispose()
		{
			if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
			_frame.Closed = true;
			Current.Value = _previous;
		}
		internal static Scope Create(ICharacter? actor, Guid id, bool input = false, bool command = false,
			bool generated = false, bool speech = false, string? method = null, string? message = null, string? formulaText = null) =>
			new(new Frame { Parent = Current.Value, Actor = actor, Id = id, Input = input, Command = command,
				Generated = generated, Speech = speech, Method = method, Message = message, FormulaText = formulaText });
	}

	public static IDisposable PlayerInput(ICharacter? actor, Guid? originId = null) => Scope.Create(actor, originId ?? Guid.NewGuid(), input: true);
	public static IDisposable CharacterCommand(ICharacter actor)
	{
		var frame = Current.Value;
		var authorized = frame is { Input: true, Closed: false } && ReferenceEquals(frame.Actor, actor) &&
			Interlocked.CompareExchange(ref frame.Claimed, 1, 0) == 0;
		return Scope.Create(actor, frame?.Id ?? Guid.NewGuid(), command: authorized);
	}
	public static IDisposable Suppress() => Scope.Create(null, Guid.Empty);
	public static IDisposable AuthorizeNativeSpeech(ICharacter actor, string method, string message, string? formulaText = null)
	{
		var frame = Current.Value;
		var authorized = frame is { Command: true, Closed: false } && ReferenceEquals(frame.Actor, actor) &&
			Interlocked.CompareExchange(ref frame.Claimed, 1, 0) == 0;
		return Scope.Create(actor, frame?.Id ?? Guid.NewGuid(), speech: authorized, method: method, message: message, formulaText: formulaText);
	}
	public static Scope Generated(ICharacter actor, Guid id, string method, string message) =>
		Scope.Create(actor, id, generated: true, method: method, message: message);

	public static NativeSpeechEmission? Capture(ICharacter actor, string method, AudioVolume volume,
		ILanguage language, string message)
	{
		var frame = Current.Value;
		if (frame is null || frame.Closed || !ReferenceEquals(frame.Actor, actor) || frame.Method != method ||
			frame.Message != message || (!frame.Speech && !frame.Generated) ||
			Interlocked.CompareExchange(ref frame.Claimed, 1, 0) != 0) return null;
		frame.Emitted = true;
		var provenance = new SpeechProvenance(frame.Id, actor.InstanceId,
			frame.Generated ? SpeechOriginKind.GeneratedCasting : SpeechOriginKind.PlayerInput);
		return new(actor, provenance, frame.Generated ? null : new MagicCastingSpeech(frame.Id, message, method, volume, language.Id, frame.FormulaText));
	}
}

internal sealed class NativeSpeechEmission(ICharacter actor, SpeechProvenance provenance, MagicCastingSpeech? speech)
{
	private int _invoked;
	public SpeechProvenance Provenance { get; } = provenance;
	public void Invoke()
	{
		if (speech is null || Interlocked.CompareExchange(ref _invoked, 1, 0) != 0) return;
		var result = actor.Gameworld.MagicCasting?.CastFormula(actor, speech.FormulaText ?? speech.Message, speech.Method, speech: speech);
		if (result is not null) actor.OutputHandler.Send(result.Message);
	}
}

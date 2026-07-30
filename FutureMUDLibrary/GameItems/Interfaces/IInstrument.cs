#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Form.Audio;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces;

[Flags]
public enum InstrumentUseMode
{
	None = 0,
	Handheld = 1,
	Worn = 2,
	Room = 4
}

public sealed record InstrumentSignalPattern(string Name, string LocalEmote, string DistantEmote,
	string FailureEmote);

public interface IInstrument : IGameItemComponent
{
	string InstrumentFamily { get; }
	ITraitDefinition? PerformanceTrait { get; }
	Difficulty PerformanceDifficulty { get; }
	AudioVolume Volume { get; }
	int RequiredHands { get; }
	InstrumentUseMode UseModes { get; }
	double InitialStaminaCost { get; }
	double StaminaPerTick { get; }
	TimeSpan TickInterval { get; }
	IReadOnlyCollection<string> Styles { get; }
	ICharacter? CurrentPerformer { get; }
	string? CurrentStyle { get; }
	Outcome CurrentOutcome { get; }
	bool IsBeingPlayed { get; }

	bool CanPlay(ICharacter actor, string style);
	string WhyCannotPlay(ICharacter actor, string style);
	bool Play(ICharacter actor, string style, IEmote? playerEmote = null);
	void StopPlaying(ICharacter actor, bool echo = true);
	void PerformTick();
}

public interface ISignalInstrument : IInstrument
{
	IReadOnlyCollection<InstrumentSignalPattern> SignalPatterns { get; }
	double SignalStaminaCost { get; }
	TimeSpan SignalCooldown { get; }

	bool CanSignal(ICharacter actor, string pattern);
	string WhyCannotSignal(ICharacter actor, string pattern);
	bool Signal(ICharacter actor, string pattern, IEmote? playerEmote = null);
}

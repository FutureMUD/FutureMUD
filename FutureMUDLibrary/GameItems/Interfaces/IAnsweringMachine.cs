#nullable enable
using System.Collections.Generic;
using MudSharp.Form.Audio;

namespace MudSharp.GameItems.Interfaces;

public interface IAnsweringMachine : ITelephone
{
	int AutoAnswerRings { get; }
	bool IsRecordingGreeting { get; }
	bool IsRecordingMessage { get; }
	IAudioStorageTape? Tape { get; }
	StoredAudioRecording? GreetingRecording { get; }
	IReadOnlyList<StoredAudioRecording> MessageRecordings { get; }
}

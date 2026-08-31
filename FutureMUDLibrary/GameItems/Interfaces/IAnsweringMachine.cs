#nullable enable
using MudSharp.Form.Audio;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface IAnsweringMachine : ITelephone
{
    int AutoAnswerRings { get; }
    bool IsRecordingGreeting { get; }
    bool IsRecordingMessage { get; }
	IMediaStorageMedium? Medium { get; }
    StoredAudioRecording? GreetingRecording { get; }
    IReadOnlyList<StoredAudioRecording> MessageRecordings { get; }
}

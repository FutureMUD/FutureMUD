#nullable enable
using MudSharp.Form.Audio;
using System;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface IAudioStorageTape : IGameItemComponent
{
    TimeSpan Capacity { get; }
    TimeSpan UsedCapacity { get; }
    TimeSpan RemainingCapacity { get; }
    bool WriteProtected { get; set; }
    IReadOnlyCollection<StoredAudioRecording> Recordings { get; }
    bool HasRecording(string name);
    StoredAudioRecording? GetRecording(string name);
    bool CanStoreRecording(StoredAudioRecording recording, out string error);
    bool StoreRecording(StoredAudioRecording recording, out string error);
    bool DeleteRecording(string name);
    void DeleteAllRecordings();
}

#nullable enable

using MudSharp.Form.Audio;

namespace MudSharp.PerceptionEngine.Outputs;

/// <summary>The visible performance and the instrument's configured acoustic description.</summary>
internal sealed class InstrumentPerformanceOutput(IEmote performance, IEmote sound, AudioVolume volume) : MixedEmoteOutput(performance)
{
	public IEmote Sound { get; } = sound;
	public AudioVolume Volume { get; } = volume;
}

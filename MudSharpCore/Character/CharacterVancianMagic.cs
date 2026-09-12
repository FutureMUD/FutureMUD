using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Character;

public partial class Character
{
	private VancianSleepTracker? _vancianSleepTracker;
	private void UpdateVancianSleepTracker()
	{
		if (!IsPrimaryInstance || !CanRunCharacterOngoingProcesses || !Capabilities.OfType<IVancianMagicCapability>().Any(x => x.RecoveryMode != VancianRecoveryMode.PreparationAction))
		{ _vancianSleepTracker?.Dispose(); _vancianSleepTracker = null; return; }
		_vancianSleepTracker ??= new VancianSleepTracker(this, VancianMagicService.For(Gameworld));
	}
	private void StopVancianSleepTracker() { _vancianSleepTracker?.Dispose(); _vancianSleepTracker = null; }
	internal void InterruptVancianWork()
	{
		_vancianSleepTracker?.Interrupt();
		foreach (var instance in Instances) instance.RemoveAllEffects<VancianTimedAction>(fireRemovalAction: true);
	}
}

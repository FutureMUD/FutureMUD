namespace MudSharp.Effects;

public partial class EffectScheduler : IEffectExpiryObserver
{
	public DateTime? ScheduledExpiry(IEffect effect) => _scheduleMap.TryGetValue(effect,out var schedule) ? schedule.TriggerETA : null;
}

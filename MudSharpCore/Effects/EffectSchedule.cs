using System;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Effects;

public class EffectSchedule : ScheduleBase, IEffectSchedule
{
	public EffectSchedule(IEffect effect, TimeSpan span)
		: base(ScheduleType.Effect, span)
	{
		Effect = effect;
	}

	public IEffect Effect { get; protected set; }

	public override string DebugInfoString => $"EffectSchedule for {Effect.Describe(null)}";

	public override void Fire()
	{
		Effect.ExpireEffect();
	}

	public void ExtendDuration(TimeSpan duration)
	{
		Duration += duration;
		TriggerETA += duration;
	}

	public void Save()
	{
	}

	public string Describe(IPerceiver voyeur)
	{
		return string.Format(voyeur, "{0} {3}[{1:N1}s] (original {2:N1}s){4}",
			Effect.Describe(voyeur),
			(TriggerETA - DateTime.UtcNow).TotalSeconds,
			Duration.TotalSeconds,
			Telnet.Green.Colour,
			Telnet.RESETALL
		);
	}
}
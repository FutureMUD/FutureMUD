using System;
namespace MudSharp.Effects;

/// <summary>Read-only absolute schedule boundary; null means no schedule for that effect.</summary>
public interface IEffectExpiryObserver
{
	DateTime? ScheduledExpiry(IEffect effect);
}

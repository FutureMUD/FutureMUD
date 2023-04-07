using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Effects;

public class EffectHandler : IEffectHandler
{
	private bool _hasSaveableScheduledEffects;

	public EffectHandler(IPerceivable parent)
	{
		Parent = parent;
	}

	public bool EffectsChanged
	{
		get => Parent.EffectsChanged;
		set => Parent.EffectsChanged = value;
	}

	#region IEffectHandler Members

	public IPerceivable Parent { get; set; }

	public IFuturemud Gameworld => Parent.Gameworld;

	#endregion

	#region IHaveEffects Members

	private readonly List<IEffect> _effects = new();
	public IEnumerable<IEffect> Effects => _effects;

	public IEnumerable<T> EffectsOfType<T>(Predicate<T> predicate = null) where T : class, IEffect
	{
		return predicate == null ? _effects.OfType<T>() : _effects.OfType<T>().Where(x => predicate(x));
	}

	public bool AffectedBy<T>() where T : class, IEffect
	{
		return _effects.Any(x => x.IsEffectType<T>());
	}

	public bool AffectedBy<T>(object target) where T : class, IEffect
	{
		return _effects.Any(x => x.IsEffectType<T>(target));
	}

	public bool AffectedBy<T>(object target, object thirdparty) where T : class, IEffect
	{
		return _effects.Any(x => x.IsEffectType<T>(target, thirdparty));
	}

	public void AddEffect(IEffect effect)
	{
		if (!_effects.Contains(effect))
		{
			_effects.Add(effect);
		}
#if DEBUG
		else
		{
			throw new ApplicationException("An effect was added twice.");
		}
#endif
		effect.InitialEffect();
		if (effect.SavingEffect)
		{
			EffectsChanged = true;
		}
	}

	public void AddEffect(IEffect effect, TimeSpan duration)
	{
		AddEffect(effect);
		if (duration >= TimeSpan.Zero && duration != TimeSpan.MaxValue)
		{
			Gameworld.EffectScheduler.AddSchedule(new EffectSchedule(effect, duration));
			if (effect.SavingEffect)
			{
				_hasSaveableScheduledEffects = true;
			}
		}
	}

	public void RemoveEffect(IEffect effect, bool fireRemovalAction = false)
	{
		if (fireRemovalAction)
		{
			effect.RemovalEffect();
		}

		_effects.Remove(effect);
		Gameworld.EffectScheduler.Unschedule(effect);
		if (effect.SavingEffect)
		{
			EffectsChanged = true;
		}

		if (_hasSaveableScheduledEffects &&
		    !_effects.Any(x => x.SavingEffect && Gameworld.EffectScheduler.IsScheduled(x)))
		{
			_hasSaveableScheduledEffects = false;
		}
	}

	public void RemoveAllEffects()
	{
		foreach (var effect in _effects.ToList())
		{
			RemoveEffect(effect);
		}
	}

	public void RemoveAllEffects(Predicate<IEffect> predicate, bool fireRemovalAction = false)
	{
		foreach (var effect in _effects.Where(x => predicate(x)).ToList())
		{
			RemoveEffect(effect, fireRemovalAction);
		}
	}

	public bool RemoveAllEffects<T>(Predicate<T> predicate = null, bool fireRemovalAction = false) where T : IEffect
	{
		var effects = (predicate != null ? _effects.OfType<T>().Where(x => predicate(x)) : _effects.OfType<T>())
			.ToList();
		foreach (var effect in effects)
		{
			RemoveEffect(effect, fireRemovalAction);
		}

		return effects.Any();
	}

	public void Reschedule(IEffect effect, TimeSpan newDuration)
	{
		Gameworld.EffectScheduler.Reschedule(effect, newDuration);
		EffectsChanged = true;
	}

	public void RescheduleIfLonger(IEffect effect, TimeSpan newDuration)
	{
		Gameworld.EffectScheduler.RescheduleIfLonger(effect, newDuration);
		EffectsChanged = true;
	}

	public TimeSpan ScheduledDuration(IEffect effect)
	{
		return Gameworld.EffectScheduler.RemainingDuration(effect);
	}

	public void AddDuration(IEffect effect, TimeSpan addedDuration)
	{
		if (!Gameworld.EffectScheduler.IsScheduled(effect))
		{
			return;
		}

		Gameworld.EffectScheduler.Reschedule(effect,
			Gameworld.EffectScheduler.RemainingDuration(effect) + addedDuration);
		EffectsChanged = true;
	}

	public void RemoveDuration(IEffect effect, TimeSpan removedDuration, bool fireRemovalActionIfRemoved = false)
	{
		if (!Gameworld.EffectScheduler.IsScheduled(effect))
		{
			return;
		}

		var remaining = Gameworld.EffectScheduler.RemainingDuration(effect);
		if (remaining - removedDuration < TimeSpan.Zero)
		{
			RemoveEffect(effect, fireRemovalActionIfRemoved);
			return;
		}

		Gameworld.EffectScheduler.Reschedule(effect, remaining - removedDuration);
		EffectsChanged = true;
	}

	public PerceptionTypes GetPerception(PerceptionTypes type)
	{
		return _effects.Where(x => x.Applies()).Aggregate(type, (prev, effect) => prev & ~effect.PerceptionDenying);
	}

	public bool HiddenFromPerception(PerceptionTypes type, PerceiveIgnoreFlags
		flags = PerceiveIgnoreFlags.None)
	{
		return _effects.Where(x => x.Applies()).Aggregate(type, (prev, effect) => prev & ~effect.Obscuring) !=
		       PerceptionTypes.None;
	}

	public bool HiddenFromPerception(IPerceiver voyeur, PerceptionTypes type,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return _effects.Where(x => x.Applies(voyeur, flags))
		               .Aggregate(type, (prev, effect) => prev & ~effect.Obscuring) !=
		       PerceptionTypes.None;
	}

	#endregion
}
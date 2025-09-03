using System;
using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.Effects {
    public interface IHaveEffects {
        IEnumerable<IEffect> Effects { get; }
        IEnumerable<T> EffectsOfType<T>(Predicate<T> predicate = null) where T : class, IEffect;
        bool AffectedBy<T>() where T : class, IEffect;
        bool AffectedBy<T>(Predicate<T> predicate) where T : class, IEffect;
        bool AffectedBy<T>(object target) where T : class, IEffect;
        bool AffectedBy<T>(object target, object thirdparty) where T : class, IEffect;

        void AddEffect(IEffect effect);
        void AddEffect(IEffect effect, TimeSpan duration);

        void RemoveEffect(IEffect effect, bool fireRemovalEffect = false);
        void RemoveAllEffects();
        void RemoveAllEffects(Predicate<IEffect> predicate, bool fireRemovalAction = false);

        bool RemoveAllEffects<T>(Predicate<T> predicate = null, bool fireRemovalAction = false) where T : IEffect;

        void Reschedule(IEffect effect, TimeSpan newDuration);
        void RescheduleIfLonger(IEffect effect, TimeSpan newDuration);
        void AddDuration(IEffect effect, TimeSpan addedDuration);
        void RemoveDuration(IEffect effect, TimeSpan removedDuration, bool fireRemovalActionIfRemoved = false);

        /// <summary>
        ///     Takes the "natural" perception modes of the owner, and returns which perception types are added/removed because of
        ///     effects
        /// </summary>
        /// <param name="type">The natural perception modes of the owner</param>
        /// <returns>The final perception modes</returns>
        PerceptionTypes GetPerception(PerceptionTypes type);


        bool HiddenFromPerception(PerceptionTypes type, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);

        bool HiddenFromPerception(IPerceiver voyeur, PerceptionTypes type, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
        TimeSpan ScheduledDuration(IEffect effect);
        bool EffectsChanged { get; set; }
    }
}
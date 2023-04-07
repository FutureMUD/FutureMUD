using System;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Effects
{
    public interface IEffectScheduler : IScheduler
    {
        /// <summary>
        ///     Requests the EffectScheduler to remove all effects pertaining to the specified IPerceivable.
        ///     Should be called both when players leave the game world (e.g. quit) and when something is destroyed (e.g. junked
        ///     item)
        /// </summary>
        /// <param name="target">The IPerceivable to remove effects for</param>
        /// <param name="save">Whether or not to invoke the EffectSchedule's Save routine</param>
        void Destroy(IPerceivable target, bool save = false, bool fireRemovalAction = false);

        TimeSpan RemainingDuration(IEffect effect);
        TimeSpan OriginalDuration(IEffect effect);
        void Unschedule(IEffect effect, bool fireExpireAction = false, bool fireRemovalAction = false);
        void AddSchedule(IEffectSchedule schedule);
        bool IsScheduled(IEffect effect);
        void ExtendSchedule(IEffect effect, TimeSpan extension);
        void Reschedule(IEffect effect, TimeSpan newTimespan);
        void RescheduleIfLonger(IEffect effect, TimeSpan newTimespan);
        string Describe(IEffect effect, IPerceiver voyeur);
        void SetupEffectSaver();
        void SaveScheduledEffectDurations();
    }
}
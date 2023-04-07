using System;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Effects
{
    public interface IEffectSchedule : ISchedule
    {
        IEffect Effect { get; }
        void ExtendDuration(TimeSpan duration);
        string Describe(IPerceiver voyeur);
        void Save();
    }
}
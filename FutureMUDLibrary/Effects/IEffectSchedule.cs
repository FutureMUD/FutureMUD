using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using System;

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
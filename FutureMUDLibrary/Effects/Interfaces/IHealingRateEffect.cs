using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IHealingRateEffect : IEffectSubtype
    {
        double HealingRateMultiplier { get; }
        int HealingDifficultyStages { get; }
    }
}

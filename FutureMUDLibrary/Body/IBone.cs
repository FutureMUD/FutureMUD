using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body
{
    public interface IBone : IBodypart
    {
        bool CriticalBone { get; }
        bool CanBeImmobilised { get; }
        double BoneHealingModifier { get; }
        double BoneEffectiveHealthModifier { get; }
        IEnumerable<(IOrganProto Organ, BodypartInternalInfo Info)> CoveredOrgans { get; }
        (double OrdinaryDamage, double BoneDamage) ShouldBeBoneBreak(IDamage damage);
    }
}

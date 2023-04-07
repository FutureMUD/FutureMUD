using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Health {
    public delegate void WoundEvent(IMortalPerceiver wounded, IWound wound);

    public interface IHaveWounds : IPerceivable, IMortal {
        IHealthStrategy HealthStrategy { get; }
        IEnumerable<IWound> Wounds { get; }
        IEnumerable<IWound> VisibleWounds(IPerceiver voyeur, WoundExaminationType examinationType);
        IEnumerable<IWound> SufferDamage(IDamage damage);
        IEnumerable<IWound> PassiveSufferDamage(IDamage damage);
        IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing);
        void ProcessPassiveWound(IWound wound);
        WoundSeverity GetSeverityFor(IWound wound);
        double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false);
        void EvaluateWounds();
        void CureAllWounds();
        void StartHealthTick(bool initial = false);
        void EndHealthTick();

        /// <summary>
        /// Adds wound directly with no side effects
        /// </summary>
        /// <param name="wound">IWound to add to the IHaveWounds class</param>
        void AddWound(IWound wound);
    }

    public interface IMortalPerceiver : IPerceiver, IHaveWounds {
        event WoundEvent OnWounded;
        event WoundEvent OnHeal;
        event WoundEvent OnRemoveWound;
    }
}
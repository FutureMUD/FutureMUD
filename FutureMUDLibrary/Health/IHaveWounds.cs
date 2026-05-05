using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Health
{
    public delegate void WoundEvent(IMortalPerceiver wounded, IWound wound);

    public interface IHaveWounds : IPerceivable, IMortal
    {
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

        void AddWounds(IEnumerable<IWound> wounds);

        /// <summary>
        /// Moves an existing wound to another wound owner while preserving the wound object, its scars, and any wound-specific state.
        /// </summary>
        /// <param name="wound">The wound currently owned by this IHaveWounds.</param>
        /// <param name="newOwner">The new wound owner.</param>
        /// <param name="newBodypart">The mapped bodypart on the new owner.</param>
        /// <param name="newSeveredBodypart">The mapped severed bodypart on the new owner, if any.</param>
        /// <returns>True if the wound was moved.</returns>
        bool TryTransferWoundTo(IWound wound, IHaveWounds newOwner, IBodypart newBodypart,
            IBodypart newSeveredBodypart = null);
    }

    public interface IMortalPerceiver : IPerceiver, IHaveWounds
    {
        event WoundEvent OnWounded;
        event WoundEvent OnHeal;
        event WoundEvent OnRemoveWound;
    }
}

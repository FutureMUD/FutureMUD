using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Effects {
    public interface IEffect : IFrameworkItem, IProgVariable {
        XElement SaveToXml(Dictionary<IEffect, TimeSpan> scheduledEffects);
        bool LoadErrors { get; }
        IPerceivable Owner { get; }
        bool SavingEffect { get; }
        IEnumerable<string> Blocks { get; }
        bool CanBeStoppedByPlayer { get; }

        PerceptionTypes PerceptionDenying { get; }
        PerceptionTypes Obscuring { get; }
        bool Applies();
        bool Applies(object target);
        bool Applies(object target, object thirdparty);
        bool Applies(object target, PerceiveIgnoreFlags flags);
        bool IsEffectType<T>() where T : class, IEffect;
        bool IsEffectType<T>(object target) where T : class, IEffect;
        bool IsEffectType<T>(object target, object thirdparty) where T : class, IEffect;

        bool IsBlockingEffect(string blockingType);
        string BlockingDescription(string blockingType, IPerceiver voyeur);

        T GetSubtype<T>() where T : class, IEffect;

        /// <summary>
        ///     Fires when the scheduled effect "matures"
        /// </summary>
        void ExpireEffect();

        /// <summary>
        ///     Fires when an effect is removed, including a matured scheduled effect
        /// </summary>
        void RemovalEffect();

        /// <summary>
        ///     Fires when a scheduled effect is cancelled before it matures
        /// </summary>
        void CancelEffect();

        /// <summary>
        ///     Fires when an effect is first added to an individual
        /// </summary>
        void InitialEffect();

        /// <summary>
        /// Called when an effect is re-added to a character or item after they are "logged in" to the world, possibly having previously been loaded or quit
        /// </summary>
        void Login();

        string Describe(IPerceiver voyeur);

        IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem);
        bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem);
        bool Changed { get; set; }
    }
}
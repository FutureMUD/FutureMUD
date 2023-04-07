using System;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework.Save;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using System.Collections.Generic;
using MudSharp.Health;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Framework {
    public delegate void PerceivableEvent(IPerceivable owner);

    public class PerceivableRejectionResponse {
        public bool Rejected { get; set; }
        public string Reason { get; set; }
    }

    public delegate void PerceivableResponseEvent(IPerceivable owner, PerceivableRejectionResponse response);

    public interface IPerceivable : IKeywordedItem, IDescribable, ILocateable, IHaveEffects, IPositionable,
        IHandleOutput, IHaveFuturemud, IHandleEvents, ISaveable, IEquatable<IPerceivable>, IFutureProgVariable {
        /// <summary>
        /// True if this perceivable is a single entity as opposed to a group of entities
        /// </summary>
        bool IsSingleEntity { get; }
        
        bool Sentient { get; }

        /// <summary>
        ///     The total contribution of this IPerceivable to the Illumination level in their location
        /// </summary>
        double IlluminationProvided { get; }
        SizeCategory Size { get; }

        XElement SaveEffects();

        string HowSeen(IPerceiver voyeur, bool proper = false, DescriptionType type = DescriptionType.Short, bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);

        Gendering ApparentGender(IPerceiver voyeur);

        bool IdentityIsObscured { get; }
        bool IdentityIsObscuredTo(ICharacter observer);
        /// <summary>
        ///     Tests to see whether the other IPerceivable is considered to be "self" by the perceived item.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsSelf(IPerceivable other);

        void MoveTo(ICell location, RoomLayer layer, ICellExit exit = null, bool noSave = false);

        /// <summary>
        ///     This event fires whenever something happens to the IPerceivable that it believes should invalidate any positional
        ///     references to it
        /// </summary>
        event EventHandler InvalidPositionTargets;

        Proximity GetProximity(IPerceivable thing);

        event PerceivableEvent OnQuit;
        event PerceivableEvent OnDeleted;

        IEnumerable<IWound> ExplosionEmantingFromPerceivable(IExplosiveDamage damage);
    }
}
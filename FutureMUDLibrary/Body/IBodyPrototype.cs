using System.Collections.Generic;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.Movement;
using MudSharp.Strategies.BodyStratagies;

namespace MudSharp.Body {
    public enum BodypartRole
    {
        Core,
        MaleAddition,
        FemaleAddition,
        Extra
    }

    public interface IBodyPrototype : IFrameworkItem, IHaveFuturemud {
        IBodyPrototype Parent { get; }
        IBodyCommunicationStrategy Communications { get; }

        /// <summary>
        ///     Contains an Enumerable of all of the Bodyparts that are part of the core body  bones and organs - e.g. not gender or race specific
        /// </summary>
        IEnumerable<IBodypart> CoreBodyparts { get; }

        /// <summary>
        ///     Contains an Enumerable of all of the Bodyparts that may be used for the prototype including bones and organs - core or not.
        /// </summary>
        IEnumerable<IBodypart> AllBodyparts { get; }

        IEnumerable<IExternalBodypart> AllExternalBodyparts { get; }

        IEnumerable<IOrganProto> Organs { get; }

        IEnumerable<IBone> Bones { get; }

        IEnumerable<IBodypart> AllBodypartsBonesAndOrgans { get; }
            /// <summary>
        ///     Additional Bodyparts to be connected to the IBody only for females
        /// </summary>
        IEnumerable<IBodypart> FemaleOnlyAdditions { get; }

        /// <summary>
        ///     Additional Bodyparts to be connected to the IBody only for males
        /// </summary>
        IEnumerable<IBodypart> MaleOnlyAdditions { get; }

        void UpdateBodypartRole(IBodypart bodypart, BodypartRole role);

        IEnumerable<ILimb> Limbs { get; }

        int MinimumLegsToStand { get; }
        int MinimumWingsToFly { get; }

        IWearableSizeRules WearRulesParameter { get; }

        string WielderDescriptionSingular { get; }

        string WielderDescriptionPlural { get; }

        string LegDescriptionSingular { get; }
        string LegDescriptionPlural { get; }

        string ConsiderString { get; }

        IFutureProg StaminaRecoveryProg { get; }

        IEnumerable<IPositionState> ValidPositions { get; }

        Dictionary<IPositionState, IMoveSpeed> DefaultSpeeds { get; }
        IUneditableAll<IMoveSpeed> Speeds { get; }

        IEnumerable<IBodypart> BodypartsFor(IRace race, Gender gender);

        string DescribeBodypartGroup(IEnumerable<IBodypart> group);

        IBodypart DefaultDoorSmashingPart { get; }
        bool CountsAs(IBodyPrototype bodyPrototype);
        void InvalidateCachedBodyparts();
        IEnumerable<IBodypartGroupDescriber> BodypartGroupDescribers { get; }
        void FinaliseBodyparts(Models.BodyProto proto);
        string Show(ICharacter actor);
    }
}
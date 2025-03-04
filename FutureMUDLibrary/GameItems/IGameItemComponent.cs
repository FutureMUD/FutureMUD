using System;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.GameItems {
    /// <summary>
    ///     An IGameItemComponent is a sub-type of item with some specific function
    /// </summary>
    public interface IGameItemComponent : IFrameworkItem, ISaveable, IHandleEvents {
        IGameItem Parent { get; }
        IGameItemComponentProto Prototype { get; }

        bool DesignedForOffhandUse { get; }

        /// <summary>
        ///     Decorators are applied in Ascending order of DecorationPriority
        /// </summary>
        int DecorationPriority { get; }

        double ComponentWeight { get; }
        double ComponentWeightMultiplier { get; }
        double ComponentBuoyancy(double fluidDensity);
        bool OverridesMaterial { get; }

        ISolid OverridenMaterial { get; }

        bool WrapFullDescription { get; }

        bool AffectsLocationOnDestruction { get; }
        int ComponentDieOrder { get; }

        /// <summary>
        ///     Handles any finalisation that this component needs to perform before being deleted.
        /// </summary>
        void Delete();

        /// <summary>
        ///     Handles any finalisation that this component needs to perform before being removed from the game (e.g. in inventory
        ///     when player quits)
        /// </summary>
        void Quit();

        void Login();

        void PrimeComponentForInsertion(MudSharp.Models.GameItem parent);

        /// <summary>
        ///     Handles any finalisation/removal tasks that need to be performed when an object is destroyed
        /// </summary>
        /// <param name="newItem">If relevant, the "corpse" of the destroyed item</param>
        /// <returns>True if the position of the new item was altered in any way</returns>
        bool HandleDieOrMorph(IGameItem newItem, ICell location);

        bool SwapInPlace(IGameItem existingItem, IGameItem newItem);
        bool Take(IGameItem item);
        IGameItemComponent Copy(IGameItem newParent, bool temporary = false);

        /// <summary>
        ///     This property indicates whether this IGameItemComponent acts as a decorator for the IGameItem's description
        /// </summary>
        bool DescriptionDecorator(DescriptionType type);

        string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
            PerceiveIgnoreFlags flags);
        event EventHandler DescriptionUpdate;

        /// <summary>
        ///     This function gives the IGameItemComponent the opportunity to object to a proposed merge of two IGameItems based on
        ///     a particular IGameItemComponent of the other item
        ///     e.g. Coloured items with different colours, food items with different bites left, etc.
        /// </summary>
        /// <param name="component">A Component from the other IGameItem</param>
        /// <returns>True if this component objects to the merger</returns>
        bool PreventsMerging(IGameItemComponent component);

        /// <summary>
        ///     Indicates whether anything in the current status of this component prevents the item from being repositioned
        /// </summary>
        /// <returns></returns>
        bool PreventsRepositioning();

        /// <summary>
        ///     Indicates whether anything in the current status of this component prevents a person with it in their inventory
        ///     from moving from the room
        /// </summary>
        /// <returns></returns>
        bool PreventsMovement();

        string WhyPreventsMovement(ICharacter mover);

        /// <summary>
        ///     Indicates that the inventory owner has been forcefully moved (admin command, spell, etc) and anything that would be
        ///     affected by that needs to be handled
        /// </summary>
        void ForceMove();

        /// <summary>
        ///     Returns an error message concerning why this component cannot be repositioned
        /// </summary>
        /// <returns></returns>
        string WhyPreventsRepositioning();

        bool CheckPrototypeForUpdate();

        void FinaliseLoad();

        /// <summary>
        /// Invoked when an object has Take called on it from IInventory. Handles removing any special effects.
        /// </summary>
        void Taken();

        bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier);

        IBody HaveABody { get; }

        /// <summary>
        /// Whether or not we should warn the purger before purging this item
        /// </summary>
        bool WarnBeforePurge { get; }

        /// <summary>
        /// Gives a component a chance to override the behaviour of liquid exposure. If true, behaviour was overriden.
        /// </summary>
        /// <param name="mixture">The mixture</param>
        /// <returns>If true, behaviour was intercepted. If false, behaviour should proceed as normal</returns>
        bool ExposeToLiquid(LiquidMixture mixture);

    }
}
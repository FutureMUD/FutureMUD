using System.Collections.Generic;
using MudSharp.Body.Traits.Decorators;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Body.Traits {
    public enum TraitType {
        Skill = 0,
        Attribute = 1,
        DerivedSkill = 2,
        DerivedAttribute = 3,
        TheoreticalSkill = 4
    }

    public interface ITraitDefinition : IEditableItem, IFutureProgVariable {
        /// <summary>
        ///     The group to which this Trait belongs, within its Type. For example, Attributes may have "Physical", "Mental", etc.
        /// </summary>
        string Group { get; }

        /// <summary>
        ///     Defines which sub-group of traits this definition belongs in
        /// </summary>
        TraitType TraitType { get; }

        /// <summary>
        ///     Defines a default Decorator used to display the values of this trait
        /// </summary>
        ITraitValueDecorator Decorator { get; }

        bool Hidden { get; }

        /// <summary>
        ///     Gets the maximum value that this trait definition can have
        /// </summary>
        double MaxValue { get; }

        string MaxValueString { get; }

        /// <summary>
        ///     Load a trait from the database
        /// </summary>
        /// <param name="trait"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        ITrait LoadTrait(MudSharp.Models.Trait trait, IHaveTraits owner);

        /// <summary>
        ///     Having completely loaded all of the trait definition, this function gives the definitions the opportunity to
        ///     perform startup tasks and link dynamic information
        /// </summary>
        /// <param name="definition"></param>
        void Initialise(MudSharp.Models.TraitDefinition definition);

        ITrait NewTrait(IHaveTraits owner, double initial);

        bool ChargenAvailable(ICharacterTemplate template);

        int ResourceCost(IChargenResource resource);

        int ResourceRequirement(IChargenResource resource);

        bool HasResourceCosts { get; }

        IEnumerable<(IChargenResource resource, int cost)> ResourceCosts { get; }

        double BranchMultiplier { get; }
    }
}
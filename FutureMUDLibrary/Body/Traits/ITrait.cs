using System;
using MudSharp.Framework.Save;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits
{
	public enum TraitUseType {
        Theoretical,
        Practical
    }

    public interface ITrait : ISaveable {
        IHaveTraits Owner { get; }

        /// <summary>
        ///     The definition governing this trait
        /// </summary>
        ITraitDefinition Definition { get; }

        /// <summary>
        ///     The value of the Trait
        /// </summary>
        double Value { get; set; }

        double RawValue { get; }

        bool Hidden { get; }

        /// <summary>
        ///     Returns the maximum value this trait can have - for instance, a "Cap"
        /// </summary>
        double MaxValue { get; }

        /// <summary>
        ///     This function informs the trait that it has been used, and invites it to improve itself if it should
        /// </summary>
        /// <param name="user"></param>
        /// <param name="result"></param>
        /// <param name="difficulty"></param>
        /// <param name="usetype"></param>
        /// <returns>true if the trait improved, false if not</returns>
        bool TraitUsed(IHaveTraits user, Outcome result, Difficulty difficulty, TraitUseType usetype);

        /// <summary>
        ///     This event is called whenever the Value changes (through improvement or decay)
        /// </summary>
        event EventHandler<TraitChangedEventArgs> TraitValueChanged;

        /// <summary>
        ///     Gives the trait an opportunity to do post-load initialisation tasks against its owner. In most cases nothing.
        /// </summary>
        /// <param name="owner"></param>
        void Initialise(IHaveTraits owner);
    }
}
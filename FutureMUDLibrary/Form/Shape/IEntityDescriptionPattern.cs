using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Form.Shape {
    public enum DescriptionType
    {
        /// <summary>
        ///     The nominal description of the thing, for example, "a short, blue-eyed man"
        /// </summary>
        Short,

        /// <summary>
        ///     The possessive description of the thing, for example "your" or "a short, blue-eyed man's"
        /// </summary>
        Possessive,

        /// <summary>
        ///     The description of the thing as seen in its scenic context, e.g. "A short, blue-eyed man is here."
        /// </summary>
        Long,

        /// <summary>
        ///     The full, detailed description of a thing, such as when it is LOOKed at
        /// </summary>
        Full,

        /// <summary>
        ///     The description displayed, if any, when something requests to look at the contents of this item, such as when it is
        ///     "look in"'d.
        /// </summary>
        Contents,

        Evaluate,
    }

    public enum EntityDescriptionType {
        ShortDescription = 0,
        FullDescription
    }

    public interface IEntityDescriptionPattern : IFrameworkItem, ISaveable {
        EntityDescriptionType Type { get; }
        IFutureProg ApplicabilityProg { get; }
        string Pattern { get; }
        int RelativeWeight { get; }
        string Show(IPerceiver voyeur);
        bool IsValidSelection(ICharacterTemplate template);
        bool IsValidSelection(ICharacter character);
    }

    public static class EntityDescriptionPatternExtensions {
        public static string Describe(this EntityDescriptionType type) {
            switch (type) {
                case EntityDescriptionType.ShortDescription:
                    return "Short Description";
                case EntityDescriptionType.FullDescription:
                    return "Full Description";
            }

            return "Unknown";
        }
    }
}
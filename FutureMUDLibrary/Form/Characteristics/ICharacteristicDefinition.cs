using System;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Form.Characteristics {
    public enum CharacteristicType {
        RelativeHeight = 0,
        Standard = 1,
        Coloured = 2,
        Multiform = 3,
        Growable = 4
    }

    public static class CharacteristicTypeExtensions {
        public static string Describe(this CharacteristicType type) {
            switch (type) {
                case CharacteristicType.Coloured:
                    return "Coloured";
                case CharacteristicType.Multiform:
                    return "Multiform";
                case CharacteristicType.RelativeHeight:
                    return "Relative Height";
                case CharacteristicType.Standard:
                    return "Standard";
                case CharacteristicType.Growable:
                    return "Growable";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        public static string Describe(this CharacterGenerationDisplayType type) {
            switch (type) {
                case CharacterGenerationDisplayType.DisplayAll:
                    return "Display All";
                case CharacterGenerationDisplayType.GroupByBasic:
                    return "Group By Basic";
                case CharacterGenerationDisplayType.DisplayTable:
                    return "Display Table";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }

    public enum CharacterGenerationDisplayType {
        DisplayAll = 0,
        GroupByBasic = 1,
        DisplayTable = 2
    }
    
    public interface ICharacteristicDefinition : IFrameworkItem, ISaveable {
        /// <summary>
        ///     The Regex that matches this characteristic definition
        /// </summary>
        Regex Pattern { get; }

        /// <summary>
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     The CharacteristicType that describes the behaviour type of this CharacteristicDefinition
        /// </summary>
        CharacteristicType Type { get; }

        /// <summary>
        ///     Controls how the values of this ICharacteristicDefinition should be displayed in the Chargen system
        /// </summary>
        CharacterGenerationDisplayType ChargenDisplayType { get; }

        /// <summary>
        ///     The Default Value, used in certain string-parsing patterns
        /// </summary>
        ICharacteristicValue DefaultValue { get; }

        ICharacteristicDefinition Parent { get; }

        /// <summary>
        ///     Tests whether the supplied CharacteristicValue is a value for this Definition
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsValue(ICharacteristicValue value);

        /// <summary>
        ///     Tests whether a given value is the default value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsDefaultValue(ICharacteristicValue value);

        ICharacteristicValue GetRandomValue();

        /// <summary>
        ///     Sets the default parameter
        /// </summary>
        /// <param name="theDefault"></param>
        void SetDefaultValue(ICharacteristicValue theDefault);

        void BuildingCommand(ICharacter actor, StringStack command);
        string Show(ICharacter actor);
    }
}
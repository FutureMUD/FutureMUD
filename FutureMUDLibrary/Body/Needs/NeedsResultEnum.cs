using System;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Body.Needs {
    [Flags]
    public enum NeedsResult {
        /// <summary>
        ///     No changes to needs resulted
        /// </summary>
        None = 0,

        /// <summary>
        ///     This result occurs when food satiation changes and the character is starving afterwards (hours of satiation 0 or
        ///     less)
        /// </summary>
        Starving = 1 << 0,

        /// <summary>
        ///     This result occurs when food satiation changes and the character is hungry afterwards (hours of satiation 0 - 2)
        /// </summary>
        Hungry = 1 << 1,

        /// <summary>
        ///     This result occurs when food satiation changes and the character is peckish afterwards (hours of satiation 2 - 4)
        /// </summary>
        Peckish = 1 << 2,

        /// <summary>
        ///     This result occurs when food satiation changes and the character is full afterwards (hours of satiation 4 - 8)
        /// </summary>
        Full = 1 << 3,

        /// <summary>
        ///     This result occurs when food satiation changes and the character is stuffed afterwards (hours of satiation 8+)
        /// </summary>
        AbsolutelyStuffed = 1 << 4,

        /// <summary>
        ///     This result occurs when drink satiation changes and the character is parched afterwards (hours of satiation 0 or
        ///     less)
        /// </summary>
        Parched = 1 << 5,

        /// <summary>
        ///     This result occurs when drink satiation changes and the character is thirsty afterwards (hours of satiation 0 - 2)
        /// </summary>
        Thirsty = 1 << 6,

        /// <summary>
        ///     This result occurs when drink satiation changes and the character is not thirsty afterwards (hours of satiation 2 -
        ///     4)
        /// </summary>
        NotThirsty = 1 << 7,

        /// <summary>
        ///     This result occurs when drink satiation changes and the character is not thirsty afterwards (hours of satiation 4+)
        /// </summary>
        Sated = 1 << 8,

        /// <summary>
        ///     This result occurs when alcohol levels change and the character is sober afterwards (BAC less than 0.01)
        /// </summary>
        Sober = 1 << 9,

        /// <summary>
        /// BAC 0.01 - 0.04
        /// </summary>
        Buzzed = 1 << 10,

        /// <summary>
        ///     This result occurs when alcohol levels change and the character is tipsy afterwards (BAC 0.04 - 0.08)
        /// </summary>
        Tipsy = 1 << 11,

        /// <summary>
        ///     This result occurs when alcohol levels change and the character is drunk afterwards (BAC 0.08 - 0.12)
        /// </summary>
        Drunk = 1 << 12,

        /// <summary>
        ///     This result occurs when alcohol levels change and the character is very drunk afterwards (BAC 0.12 - 0.16)
        /// </summary>
        VeryDrunk = 1 << 13,

        /// <summary>
        /// BAC 0.16 - 0.25
        /// </summary>
        BlackoutDrunk = 1 << 14,

        /// <summary>
        ///     This result occurs when alcohol levels change and the character is paralytic afterwards (BAC 0.25+)
        /// </summary>
        Paralytic = 1 << 15,

        HungerOnly = Starving | Hungry | Peckish | Full | AbsolutelyStuffed,

        ThirstOnly = Parched | Thirsty | NotThirsty | Sated,

        DrunkOnly = Sober | Tipsy | Drunk | VeryDrunk | Paralytic | Buzzed | BlackoutDrunk
    }

    public static class NeedsExtensions {
        public static string Describe(this NeedsResult result) {
            switch (result) {
                case NeedsResult.VeryDrunk:
                    return "Very Drunk";
                case NeedsResult.Sated:
                    return "Completely Sated";
                case NeedsResult.AbsolutelyStuffed:
                    return "Absolutely Stuffed";
                case NeedsResult.NotThirsty:
                    return "Not Thirsty";
                case NeedsResult.BlackoutDrunk:
                    return "Blackout Drunk";
            }

            return result.DescribeEnum();
        }

        public static bool IsHungry(this NeedsResult result)
        {
            switch (result & NeedsResult.HungerOnly)
            {
                case NeedsResult.Starving:
                case NeedsResult.Hungry:
                case NeedsResult.Peckish:
                    return true;
            }

            return false;
        }

        public static bool IsThirsty(this NeedsResult result)
        {
            switch (result & NeedsResult.ThirstOnly)
            {
                case NeedsResult.Parched:
                case NeedsResult.Thirsty:
                    return true;
            }

            return false;
        }
    }
}
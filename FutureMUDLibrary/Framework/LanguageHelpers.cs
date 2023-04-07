using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.Framework {
    public static class LanguageHelpers {
        private static readonly Regex StripAAnRegex = new("(?<=\x1B\\[[^m]+m|^)\\s*(?:a|an|the) ",
            RegexOptions.IgnoreCase);

        private static readonly Dictionary<string, string> _pluralOverrides = new();

        private static readonly IFutureMUDPluralizationService _pluralizationService = new FuturemudPluralizationService();

        private static readonly Dictionary<string, bool> _AAnOverrides = new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "hour", true },
            { "hourglass", true },
            { "honest", true },
            { "honorable", true },
            { "honourable", true },
            { "honor", true },
            { "honour", true },
            { "heir", true },
            { "one", false },
            { "once", false },
            { "ewe", false },
            { "unidentified", true },
            { "unidentifiable", true },
            { "unidentifiably", true },
            { "unimportant", true },
            { "used", false },
            { "unintended", true },
            { "unintelligent", true },
            { "user", false },
        };

        public static string The(this string input, bool proper)
        {
            if (input.StartsWith("the ", StringComparison.InvariantCultureIgnoreCase) ||
                input.StartsWith("a ", StringComparison.InvariantCultureIgnoreCase) ||
                input.StartsWith("an ", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                return input;
            }

            return proper ? 
                "The " + input :
                "the " + input;
        }

        private static string GetAorAn(string input, bool proper, ANSIColour colour)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return (input ?? string.Empty).FluentProper(proper).Colour(colour);
            }

            input = input.TrimStart();

            if (
                input.StartsWith("a ", StringComparison.InvariantCultureIgnoreCase) ||
                input.StartsWith("an ", StringComparison.InvariantCultureIgnoreCase) ||
                input.StartsWith("the ", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                return input.FluentProper(proper).Colour(colour);
            }

            // Special english cases
            var firstWord = input.RemoveFirstWord();
            if (_AAnOverrides.ContainsKey(firstWord))
            {
                return $"{(proper ? "A" : "a")}{(_AAnOverrides[firstWord] ? "n" : "")} {input.Colour(colour)}";
            }

            if (
                input.StartsWith("eu", StringComparison.InvariantCultureIgnoreCase) ||
                input.StartsWith("uni", StringComparison.InvariantCultureIgnoreCase) ||
                input.StartsWith("ur", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                return $"{(proper ? "A" : "a")}n {input.Colour(colour)}";
            }

            switch (char.ToLowerInvariant(input[0]))
            {
                case 'a':
                case 'e':
                case 'o':
                case 'i':
                case 'u':
                    return $"{(proper ? "A" : "a")}n {input.Colour(colour)}";
            }

            return $"{(proper ? "A" : "a")} {input.Colour(colour)}";
        }

        /// <summary>
        ///     Analyses a string to work out whether it ought to begin with an "a" or an "an", based on the initial character.
        /// </summary>
        /// <param name="input">The string to compare</param>
        /// <param name="proper">Whether or not to capitalise the a/an</param>
        /// <param name="colour">An optional colour to apply to the original input text</param>
        /// <returns>A or An plus the original string</returns>
        public static string A_An(this string input, bool proper = false, ANSIColour colour = null) {
            return GetAorAn(input, proper, colour);
        }

        public static string A_An_RespectPlurals(this string input, bool proper = false, ANSIColour colour = null) {
            if (input.ContainsPlural()) {
                return input.FluentProper(proper).Colour(colour);
            }

            return GetAorAn(input, proper, colour);
        }

        /// <summary>
        ///     A fluent method to strip a leading a/an/the from a string. Does not respect MXP tags.
        /// </summary>
        /// <param name="input">The text to strip from</param>
        /// <param name="strip">Fluent parameter for whether to strip at all</param>
        /// <returns></returns>
        public static string Strip_A_An(this string input, bool strip = true) {
            return strip ? StripAAnRegex.Replace(input, "", 1) : input;
        }

        /// <summary>
        ///     Analyses a number by looking at its textual representation to work out whether it ought to begin with an "a" or an
        ///     "an", based on the initial character.
        /// </summary>
        /// <param name="input">The string to compare</param>
        /// <param name="proper">Whether or not to capitalise the a/an</param>
        /// <param name="colour">An optional colour to apply to the original input text</param>
        /// <returns>A or An plus the original number</returns>
        public static string A_An(this int input, bool proper = false, ANSIColour colour = null) {
            return input.ToWordyNumber().A_An(proper, colour);
        }

        public static void LoadPluralOverrides() {
            // TODO
        }

        /// <summary>
        ///     Returns a pluralised version of the specified word, according to English rules of pluralisation
        /// </summary>
        /// <param name="input">A string representing a single word to pluralise</param>
        /// <param name="pluralise">Whether or not to pluralise, if using this fluently</param>
        /// <returns>The pluralised string</returns>
        public static string Pluralise(this string input, bool pluralise = true) {
            if (input.Length < 2 || !pluralise) {
                return input;
            }

            var lowerInput = input.ToLowerInvariant();
            return _pluralOverrides.ContainsKey(lowerInput) ? _pluralOverrides[lowerInput] : _pluralizationService.Pluralize(input);
        }

        public static bool ContainsPlural(this string input) {
            return !new ExplodedString(input).Words.All(x => _pluralizationService.IsSingular(x));
        }
    }
}
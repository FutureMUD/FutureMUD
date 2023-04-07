using MudSharp.Construction;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MudSharp.Framework {
    public static partial class Constants {
        public const int PlayerConnectionBufferSize = 40960;

        public const string ValidNameCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public const string ValidRandomCharacters = "abcdefghijklmnopqrstuvwxyz";
        public const string RandomPasswordCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        public const string PunctuationCharacters = ".,!?-:;";
        public const string SentenceEndingCharacters = ".!?:";
        public const string SpaceCharacters = " \t\n";

        public const string EmailRegex = @"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b";

        // Language constants
        public static readonly string[] EnglishConsonants = {
            "b", "c", "d", "f", "g", "h", "j",
            "k", "l", "m", "n", "p", "q", "r",
            "s", "t", "v", "w", "x", "y", "z"
        };

        public static readonly string[] EnglishVowels = {"a", "e", "i", "o", "u", "y"};
        public static readonly string[] EnglishSonorants = {"y", "w", "l", "r", "m", "n", "ng"};

        public static readonly char[] CommandSeparators = {' '};
        public static readonly char[] WordSeparators = {' '};

        public static readonly Random Random = new();
        public static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        public static readonly string[] CardinalDirectionStrings = {
            "n", "e", "s", "w", "u", "d",
            "north", "east", "south", "west",
            "up", "down"
        };

        public static readonly string[] DirectionStrings = {
            "North", "North-East", "East", "South-East", "South",
            "South-West", "West", "North-West", "Up", "Down", "Unknown"
        };

        public static readonly CardinalDirection[] CardinalDirections = {
            CardinalDirection.North,
            CardinalDirection.NorthEast, CardinalDirection.East, CardinalDirection.SouthEast, CardinalDirection.South,
            CardinalDirection.SouthWest, CardinalDirection.West, CardinalDirection.NorthWest, CardinalDirection.Up,
            CardinalDirection.Down, CardinalDirection.Unknown
        };

        public static readonly Dictionary<string, CardinalDirection> CardinalDirectionStringToDirection
            = new(StringComparer.OrdinalIgnoreCase) {
                {"n", CardinalDirection.North},
                {"north", CardinalDirection.North},
                {"ne", CardinalDirection.NorthEast},
                {"northeast", CardinalDirection.NorthEast},
                {"north-east", CardinalDirection.NorthEast},
                {"north east", CardinalDirection.NorthEast},
                {"e", CardinalDirection.East},
                {"east", CardinalDirection.East},
                {"se", CardinalDirection.SouthEast},
                {"southeast", CardinalDirection.SouthEast},
                {"south-east", CardinalDirection.SouthEast},
                {"south east", CardinalDirection.SouthEast},
                {"s", CardinalDirection.South},
                {"south", CardinalDirection.South},
                {"sw", CardinalDirection.SouthWest},
                {"southwest", CardinalDirection.SouthWest},
                {"south-west", CardinalDirection.SouthWest},
                {"south west", CardinalDirection.SouthWest},
                {"w", CardinalDirection.West},
                {"west", CardinalDirection.West},
                {"nw", CardinalDirection.NorthWest},
                {"northwest", CardinalDirection.NorthWest},
                {"north-west", CardinalDirection.NorthWest},
                {"north west", CardinalDirection.NorthWest},
                {"u", CardinalDirection.Up},
                {"up", CardinalDirection.Up},
                {"d", CardinalDirection.Down},
                {"down", CardinalDirection.Down}
            };

        public static readonly int InventoryLocDescPadding = 35;
    }
}
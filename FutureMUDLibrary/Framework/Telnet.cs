using System.Collections;
using System.Collections.Generic;

namespace MudSharp.Framework {
    public class ANSIColour {
        public ANSIColour(string colourname, string colour, string boldcolour, string backgroundcolour,
            string boldbackgroundcolour) {
            Name = colourname;
            Colour = colour;
            Bold = boldcolour;
            BackgroundColour = backgroundcolour;
            BoldBackgroundColour = boldbackgroundcolour;
        }

        public string Name { get; private set; }

        public string Colour { get; set; }

        public string Bold { get; private set; }

        public string BackgroundColour { get; private set; }

        public string BoldBackgroundColour { get; private set; }

        public override string ToString() {
            return Colour;
        }
    }

    public static class Telnet {
        public const string RESETALL = "\x1B[0m";
        public const string RESET = "\x1B[0;39m";
        public const string RESETBACKGROUND = "\x1B[49m";
        public const string RESETNEWLINE = "\x1B[39m\n";
        public const string RESETUNDERLINE = "\x1B[24m";
        public const string UNDERLINE = "\x1B[4m";
        public const string RESETBLINK = "\x1B[25m";
        public const string BLINK = "\x1B[5m";
        private const string BLACK = "\x1B[30m";
        private const string RED = "\x1B[31m";
        private const string GREEN = "\x1B[32m";
        private const string YELLOW = "\x1B[33m";
        private const string BLUE = "\x1B[34m";
        private const string MAGENTA = "\x1B[35m";
        private const string CYAN = "\x1B[36m";
        private const string WHITE = "\x1B[37m";
        private const string BOLDBLACK = "\x1B[1;30m";
        private const string BOLDRED = "\x1B[1;31m";
        private const string BOLDGREEN = "\x1B[1;32m";
        private const string BOLDYELLOW = "\x1B[1;33m";
        private const string BOLDBLUE = "\x1B[1;34m";
        private const string BOLDMAGENTA = "\x1B[1;35m";
        private const string BOLDCYAN = "\x1B[1;36m";
        private const string BOLDWHITE = "\x1B[1;37m";
        private const string ORANGE = "\x1B[38;5;94m";
        private const string BOLDORANGE = "\x1B[38;5;202m";
        private const string PINK = "\x1B[38;5;183m";
        private const string BOLDPINK = "\x1B[38;5;171m";
        private const string KEYWORDBLUE = "\x1b[38;2;86;156;214m";
        private const string KEYWORDPINK = "\x1b[38;2;238;130;238m";
        private const string FUNCTIONYELLOW = "\x1b[38;2;220;220;170m";
        private const string VARIABLECYAN = "\x1b[38;2;156;220;254m";
        private const string TEXTRED = "\x1b[38;2;214;157;133m";
        private const string VARIABLEGREEN = "\x1b[38;2;184;215;163m";

		private const string BLACKBACKGROUND = "\x1B[40m";
        private const string REDBACKGROUND = "\x1B[41m";
        private const string GREENBACKGROUND = "\x1B[42m";
        private const string YELLOWBACKGROUND = "\x1B[43m";
        private const string BLUEBACKGROUND = "\x1B[44m";
        private const string MAGENTABACKGROUND = "\x1B[45m";
        private const string CYANBACKGROUND = "\x1B[46m";
        private const string WHITEBACKGROUND = "\x1B[47m";
        private const string BOLDREDBACKGROUND = "\x1B[1;41m";
        private const string BOLDGREENBACKGROUND = "\x1B[1;42m";
        private const string BOLDYELLOWBACKGROUND = "\x1B[1;43m";
        private const string BOLDBLUEBACKGROUND = "\x1B[1;44m";
        private const string BOLDMAGENTABACKGROUND = "\x1B[1;45m";
        private const string BOLDCYANBACKGROUND = "\x1B[1;46m";
        private const string BOLDWHITEBACKGROUND = "\x1B[1;47m";
        private const string ORANGEBACKGROUND = "\x1B[48;5;94m";
        private const string BOLDORANGEBACKGROUND = "\x1B[48;5;202m";
        private const string BOLDBLACKBACKGROUND = "\x1B[1;40m";
        private const string PINKBACKGROUND = "\x1B[48;5;183m";
        private const string BOLDPINKBACKGROUND = "\x1B[48;5;171m";
        private const string KEYWORDBLUEBACKGROUND = "\x1b[48;2;86;156;214m";
        private const string KEYWORDPINKBACKGROUND = "\x1b[48;2;238;130;238m";
        private const string FUNCTIONYELLOWBACKGROUND = "\x1b[48;2;220;220;170m";
        private const string VARIABLECYANBACKGROUND = "\x1b[48;2;156;220;254m";
        private const string TEXTREDBACKGROUND = "\x1b[48;2;214;157;133m";
        private const string VARIABLEGREENBACKGROUND = "\x1b[48;2;184;215;163m";

		/// <summary>
		///     Interpret as command
		/// </summary>
		public const byte IAC = 0xFF;

        /// <summary>
        ///     Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
        /// </summary>
        public const byte WILL = 251;

        /// <summary>
        ///     Indicates the refusal to perform, or continue performing, the indicated option.
        /// </summary>
        public const byte WONT = 252;

        /// <summary>
        ///     Indicates the request that the other party perform, or confirmation that you are expecting the other party to
        ///     perform, the indicated option.
        /// </summary>
        public const byte DO = 253;

        /// <summary>
        ///     Indicates the demand that the other party stop performing, or confirmation that you are no longer expecting the
        ///     other party to perform, the indicated option.
        /// </summary>
        public const byte DONT = 254;

        /// <summary>
        ///     End of subnegotiation parameters.
        /// </summary>
        public const byte SE = 0xF0;

        /// <summary>
        ///     Subnegotiation of the indicated option follows.
        /// </summary>
        public const byte SB = 250;

        /// <summary>
        ///     Use the MXP Protocol
        /// </summary>
        public const byte TELOPT_MXP = (byte) '\x5B';

        public const byte TELOPT_EOR = 25;

        /// <summary>
        ///     Telnet sub option indicating negotiation of available Charsets
        /// </summary>
        public const byte CHARSET = 42;

        /// <summary>
        ///     End of Interpret as command sequence
        /// </summary>
        public const byte END_IAC = 0;

        public const byte TELOPT_NAWS = 31;

        /// <summary>
        /// Telnet Go Ahead Signal
        /// </summary>
        public const byte GA = 249;

        /// <summary>
        ///     Signals to the Telnet Engine that it should not wrap the text on this line.
        /// </summary>
        public const char NoWordWrapChar = '\x0002';

        /// <summary>
        ///     Signals to the Telnet Engine that it should not wrap the text on this line.
        /// </summary>
        public const string NoWordWrap = "\x0002";

        // ANSI Colours
        public static readonly ANSIColour Red = new("Red", RED, BOLDRED, REDBACKGROUND, BOLDREDBACKGROUND);

        public static readonly ANSIColour Yellow = new("Yellow", YELLOW, BOLDYELLOW, YELLOWBACKGROUND,
            BOLDYELLOWBACKGROUND);

        public static readonly ANSIColour Green = new("Green", GREEN, BOLDGREEN, GREENBACKGROUND,
            BOLDGREENBACKGROUND);

        public static readonly ANSIColour Blue = new("Blue", BLUE, BOLDBLUE, BLUEBACKGROUND,
            BOLDBLUEBACKGROUND);

        public static readonly ANSIColour Cyan = new("Cyan", CYAN, BOLDCYAN, CYANBACKGROUND,
            BOLDCYANBACKGROUND);

        public static readonly ANSIColour Magenta = new("Magenta", MAGENTA, BOLDMAGENTA, MAGENTABACKGROUND,
            BOLDMAGENTABACKGROUND);

        public static readonly ANSIColour White = new("White", WHITE, BOLDWHITE, WHITEBACKGROUND,
            BOLDWHITEBACKGROUND);

        public static readonly ANSIColour Black = new("Black", BLACK, BOLDBLACK, BLACKBACKGROUND, BLACKBACKGROUND);

        public static readonly ANSIColour Orange = new("Orange", ORANGE, BOLDORANGE, ORANGEBACKGROUND, BOLDORANGEBACKGROUND);

        public static readonly ANSIColour Pink = new("Pink", PINK, BOLDPINK, PINKBACKGROUND, BOLDPINKBACKGROUND);

        // Bold Only Colours
        public static readonly ANSIColour BoldRed = new("Bold Red", BOLDRED, BOLDRED, BOLDREDBACKGROUND,
            BOLDREDBACKGROUND);

        public static readonly ANSIColour BoldYellow = new("Bold Yellow", BOLDYELLOW, BOLDYELLOW,
            BOLDYELLOWBACKGROUND, BOLDYELLOWBACKGROUND);

        public static readonly ANSIColour BoldGreen = new("Bold Green", BOLDGREEN, BOLDGREEN, BOLDGREENBACKGROUND,
            BOLDGREENBACKGROUND);

        public static readonly ANSIColour BoldBlue = new("Bold Blue", BOLDBLUE, BOLDBLUE, BOLDBLUEBACKGROUND,
            BOLDBLUEBACKGROUND);

        public static readonly ANSIColour BoldCyan = new("Bold Cyan", BOLDCYAN, BOLDCYAN, BOLDCYANBACKGROUND,
            BOLDCYANBACKGROUND);

        public static readonly ANSIColour BoldMagenta = new("Bold Magenta", BOLDMAGENTA, BOLDMAGENTA,
            BOLDMAGENTABACKGROUND, BOLDMAGENTABACKGROUND);

        public static readonly ANSIColour BoldWhite = new("Bold White", BOLDWHITE, BOLDWHITE, BOLDWHITEBACKGROUND,
            BOLDWHITEBACKGROUND);

        public static readonly ANSIColour BoldOrange = new("Bold Orange", BOLDORANGE, BOLDORANGE, BOLDORANGEBACKGROUND, BOLDORANGEBACKGROUND);

        public static readonly ANSIColour BoldBlack = new("Bold Black", BOLDBLACK, BOLDBLACK, BOLDBLACKBACKGROUND, BOLDBLACKBACKGROUND);

        public static readonly ANSIColour BoldPink = new("Bold Pink", BOLDPINK, BOLDPINK, BOLDPINKBACKGROUND, BOLDPINKBACKGROUND);

        public static readonly ANSIColour KeywordBlue = new("Keyword Blue", KEYWORDBLUE, KEYWORDBLUE,
	        KEYWORDBLUEBACKGROUND, KEYWORDBLUEBACKGROUND);

        public static readonly ANSIColour KeywordPink = new("Keyword Pink", KEYWORDPINK, KEYWORDPINK,
	        KEYWORDPINKBACKGROUND, KEYWORDPINKBACKGROUND);

        public static readonly ANSIColour FunctionYellow = new("Function Yellow", FUNCTIONYELLOW, FUNCTIONYELLOW,
	        FUNCTIONYELLOWBACKGROUND, FUNCTIONYELLOWBACKGROUND);

        public static readonly ANSIColour VariableCyan = new("Variable Cyan", VARIABLECYAN, VARIABLECYAN,
	        VARIABLECYANBACKGROUND, VARIABLECYANBACKGROUND);

        public static readonly ANSIColour TextRed = new("Text Red", TEXTRED, TEXTRED, TEXTREDBACKGROUND,
	        TEXTREDBACKGROUND);

        public static readonly ANSIColour VariableGreen = new("Variable Green", VARIABLEGREEN, VARIABLEGREEN,
	        VARIABLEGREENBACKGROUND, VARIABLECYANBACKGROUND);

        public static string Reset(this ANSIColour colour) {
            return RESET;
        }

        public static string ResetNewLine(this ANSIColour colour) {
            return RESETNEWLINE;
        }

        public static IEnumerable<string> GetColourOptions => new string[]
        {
            "red".Colour(Red),
            "yellow".Colour(Yellow),
            "green".Colour(Green),
            "blue".Colour(Blue),
            "cyan".Colour(Cyan),
            "magenta".Colour(Magenta),
            "white".Colour(White),
            "black".Colour(Black),
            "orange".Colour(Orange),
            "pink".Colour(Pink),
            "bold red".Colour(BoldRed),
            "bold yellow".Colour(BoldYellow),
            "bold green".Colour(BoldGreen),
            "bold blue".Colour(BoldBlue),
            "bold cyan".Colour(BoldCyan),
            "bold magenta".Colour(BoldMagenta),
            "bold white".Colour(BoldWhite),
            "bold black".Colour(BoldBlack),
            "bold pink".Colour(BoldPink),
        };

        public static ANSIColour GetColour(string name) {
            if (name == null) {
                return null;
            }
            switch (name.ToLowerInvariant().CollapseString()) {
                case "red":
                    return Red;
                case "yellow":
                    return Yellow;
                case "green":
                    return Green;
                case "blue":
                    return Blue;
                case "cyan":
                    return Cyan;
                case "magenta":
                    return Magenta;
                case "white":
                    return White;
                case "black":
                    return Black;
                case "orange":
                    return Orange;
                case "pink":
                    return Pink;
                case "boldred":
                    return BoldRed;
                case "boldyellow":
                    return BoldYellow;
                case "boldgreen":
                    return BoldGreen;
                case "boldblue":
                    return BoldBlue;
                case "boldcyan":
                    return BoldCyan;
                case "boldmagenta":
                    return BoldMagenta;
                case "boldwhite":
                    return BoldWhite;
                case "boldorange":
                    return BoldOrange;
                case "boldblack":
                    return BoldBlack;
                case "boldpink":
                    return BoldPink;
                default:
                    return null;
            }
        }
    }
}
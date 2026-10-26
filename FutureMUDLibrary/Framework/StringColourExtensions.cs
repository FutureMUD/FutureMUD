using MudSharp.Form.Colour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework
{
    public static class StringColourExtensions
    {
        /// <summary>
        /// Colours text as a character name when colouring is enabled; otherwise returns the input unchanged.
        /// </summary>
        public static string ColourCharacter(this string input, bool colour = true)
        {
            return colour ? input.ColourIncludingReset(Telnet.Magenta) : input;
        }

        /// <summary>
        /// Colours text as an object name when enabled; otherwise returns it unchanged.
        /// </summary>
        public static string ColourObject(this string input, bool colour = true)
        {
            return colour ? input.ColourIncludingReset(Telnet.Green) : input;
        }

        /// <summary>
        /// Colours text as a value when colouring is enabled; otherwise returns the input unchanged.
        /// </summary>
        public static string ColourValue(this string input, bool colour = true)
        {
            return colour ? Telnet.Green + input + Telnet.RESET : input;
        }

        /// <summary>
        /// Colours text as a room name when enabled; otherwise returns it unchanged.
        /// </summary>
        public static string ColourRoom(this string input, bool colour = true)
        {
            return colour ? Telnet.Cyan + input + Telnet.RESET : input;
        }

        /// <summary>
        /// Colours text as command syntax when colouring is enabled; otherwise returns the input unchanged.
        /// </summary>
        public static string ColourCommand(this string input, bool colour = true)
        {
            return colour ? Telnet.Yellow + input + Telnet.RESET : input;
        }

        /// <summary>
        /// Colours text as a name when enabled; otherwise returns it unchanged.
        /// </summary>
        public static string ColourName(this string input, bool colour = true)
        {
            return colour ? $"{Telnet.Cyan}{input}{Telnet.RESET}" : input;
        }

        /// <summary>
        /// Colours text as an error when colouring is enabled; otherwise returns the input unchanged.
        /// </summary>
        public static string ColourError(this string input, bool colour = true)
        {
            return colour ? $"{Telnet.Red}{input}{Telnet.RESET}" : input;
        }

        /// <summary>
        /// This function applies the specified ANSIColour but also changes any existing RESET in the input to the colour as well.
        /// Use this version if the string you are colouring might have another colour change you want to preserve in the middle
        /// </summary>
        /// <param name="input">The string to colour</param>
        /// <param name="colour">The ANSIColour to apply</param>
        /// <returns>The coloured string</returns>
        public static string ColourIncludingReset(this string input, ANSIColour colour)
        {
            if (colour == null)
            {
                return input;
            }

            return $"{colour}{input.Replace(Telnet.RESET, colour.ToString()).Replace(Telnet.RESETALL, colour.ToString())}{colour.Reset()}";
        }

        /// <summary>
        /// Returns the RGB channel values associated with the basic colour.
        /// </summary>
        public static (int Red, int Green, int Blue) GetRGB(this BasicColour colour)
        {
            switch (colour)
            {
                case BasicColour.Black:
                    return (0, 0, 0);
                case BasicColour.White:
                    return (255, 255, 255);
                case BasicColour.Grey:
                    return (175, 175, 175);
                case BasicColour.Red:
                    return (255, 0, 0);
                case BasicColour.Blue:
                    return (0, 0, 255);
                case BasicColour.Green:
                    return (0, 255, 0);
                case BasicColour.Yellow:
                    return (255, 255, 0);
                case BasicColour.Orange:
                    return (255, 165, 0);
                case BasicColour.Purple:
                    return (128, 0, 128);
                case BasicColour.Pink:
                    return (255, 192, 203);
                case BasicColour.Brown:
                    return (175, 175, 0);
                case BasicColour.Cyan:
                    return (0, 75, 255);
                default:
                    return (0, 0, 0);
            }
        }

        /// <summary>
        /// Applies the specified foreground/background colour sequence to the input text.
        /// </summary>
        public static string Colour(this string input, BasicColour basicColour)
        {
            ANSIColour colour;
            ANSIColour background = Telnet.Black;
            switch (basicColour)
            {
                case BasicColour.Black:
                    colour = Telnet.Black;
                    background = Telnet.White;
                    break;
                case BasicColour.White:
                    colour = Telnet.BoldWhite;
                    break;
                case BasicColour.Grey:
                    colour = Telnet.White;
                    break;
                case BasicColour.Red:
                    colour = Telnet.Red;
                    break;
                case BasicColour.Blue:
                    colour = Telnet.Blue;
                    break;
                case BasicColour.Green:
                    colour = Telnet.Green;
                    break;
                case BasicColour.Yellow:
                    colour = Telnet.BoldYellow;
                    break;
                case BasicColour.Orange:
                    colour = Telnet.Orange;
                    break;
                case BasicColour.Purple:
                    colour = Telnet.Magenta;
                    break;
                case BasicColour.Pink:
                    colour = Telnet.BoldPink;
                    break;
                case BasicColour.Brown:
                    colour = Telnet.Yellow;
                    break;
                case BasicColour.Cyan:
                    colour = Telnet.Cyan;
                    break;
                default:
                    throw new ApplicationException("Unknown basic colour in Colour extension.");
            }

            return $"{background.BackgroundColour}{colour.Colour}{input}{Telnet.RESETALL}";
        }

        /// <summary>
        /// Applies the specified foreground/background colour sequence to the input text.
        /// </summary>
        public static string Colour(this string input, ANSIColour colour)
        {
            return colour == null ? input : colour + input + colour.Reset();
        }

        /// <summary>
        /// Applies the specified colour sequence to the input text.
        /// </summary>
        public static string Colour(this string input, int red, int green, int blue)
        {
            return $"\x1b[38;2;{red};{green};{blue}m{input}\x1B[0m";
        }

        /// <summary>
        /// Applies the specified colour and bold styling to the input text.
        /// </summary>
        public static string ColourBold(this string input, ANSIColour colour)
        {
            return colour == null ? input : colour.Bold + input + Telnet.RESETBOLD + colour.Reset();
        }

        /// <summary>
        /// Applies the specified background colour to the input text.
        /// </summary>
        public static string ColourBackground(this string input, ANSIColour colour)
        {
            return colour == null ? input : colour.BackgroundColour + input + Telnet.RESETALL;
        }

        /// <summary>
        /// Applies the specified bold background colour to the input text.
        /// </summary>
        public static string ColourBoldBackground(this string input, ANSIColour colour)
        {
            return colour == null ? input : colour.BoldBackgroundColour + input + Telnet.RESETALL;
        }

        /// <summary>
        /// Applies the specified colour sequence to the input text.
        /// </summary>
        public static string Colour(this string input, ANSIColour colour, ANSIColour resetcolour)
        {
            return (colour == null) || (resetcolour == null)
                ? input
                : colour.Colour + input + Telnet.RESETBOLD + resetcolour.Colour;
        }

        /// <summary>
        /// Applies the specified colour and bold styling to the input text.
        /// </summary>
        public static string ColourBold(this string input, ANSIColour colour, ANSIColour resetcolour)
        {
            return (colour == null) || (resetcolour == null)
                ? input
                : colour.Bold + input + Telnet.RESETBOLD + resetcolour.Colour;
        }

        /// <summary>
        /// Applies the specified foreground/background colour sequence to the input text.
        /// </summary>
        public static string Colour(this string input, string foreground, string background)
        {
            return background + foreground + input + Telnet.RESETALL;
        }

        /// <summary>
        /// Applies a 256-colour ANSI foreground index to the input text.
        /// </summary>
        public static string ColourForegroundCustom(this string text, string ansiForeground)
        {
            return $"{Telnet.RESETALL}\x1B[38;5;{ansiForeground}m{text}{Telnet.RESETALL}";
        }

        /// <summary>
        /// Applies the colour only when the coloured option is enabled.
        /// </summary>
        public static string FluentColour(this string input, ANSIColour colour, bool coloured)
        {
            return coloured ? input.Colour(colour) : input;
        }

        /// <summary>
        /// Applies the colour, including embedded resets, only when enabled.
        /// </summary>
        public static string FluentColourIncludingReset(this string input, ANSIColour colour, bool coloured)
        {
            return coloured ? input.ColourIncludingReset(colour) : input;
        }

        /// <summary>
        /// Applies the colour only when the input contains no ANSI escape sequence.
        /// </summary>
        public static string ColourIfNotColoured(this string input, ANSIColour colour)
        {
            if (!string.IsNullOrEmpty(input) && !input.Contains("\x1B[", StringComparison.Ordinal))
            {
                return input.Colour(colour);
            }

            return input;
        }

        /// <summary>
        /// Wraps the input text in ANSI underline and reset sequences.
        /// </summary>
        public static string Underline(this string input)
        {
            return Telnet.UNDERLINE + input + Telnet.RESETUNDERLINE;
        }

        /// <summary>
        /// Wraps the input text in ANSI blink and reset sequences.
        /// </summary>
        public static string Blink(this string input)
        {
            return Telnet.BLINK + input + Telnet.RESETBLINK;
        }
    }
}

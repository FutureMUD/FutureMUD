using MudSharp.Framework;

namespace MudSharp.Form.Colour
{
    /// <summary>
    ///     Specifies colours considered to be basic colours in the English Language (with Blue/Cyan distinction)
    /// </summary>
    public enum BasicColour
    {
        Black = 0,
        White = 1,
        Grey = 2,
        Red = 3,
        Blue = 4,
        Green = 5,
        Yellow = 6,
        Orange = 7,
        Purple = 8,
        Pink = 9,
        Brown = 10,
        Cyan = 11
    }

    public static class ColourExtensions
    {
        public static string Describe(this BasicColour colour)
        {
            switch (colour)
            {
                case BasicColour.Black:
                    return "Black";
                case BasicColour.White:
                    return "White";
                case BasicColour.Grey:
                    return "Grey";
                case BasicColour.Red:
                    return "Red";
                case BasicColour.Blue:
                    return "Blue";
                case BasicColour.Green:
                    return "Green";
                case BasicColour.Yellow:
                    return "Yellow";
                case BasicColour.Orange:
                    return "Orange";
                case BasicColour.Purple:
                    return "Purple";
                case BasicColour.Pink:
                    return "Pink";
                case BasicColour.Brown:
                    return "Brown";
                case BasicColour.Cyan:
                    return "Cyan";
                default:
                    return "Unknown";
            }
        }
    }

    public interface IColour : IFrameworkItem
    {
        /// <summary>
        ///     Returns the Red component of the RGB value of this colour
        /// </summary>
        int Red { get; }

        /// <summary>
        ///     Returns the Green component of the RGB value of this colour
        /// </summary>
        int Green { get; }

        /// <summary>
        ///     Returns the Blue component of the RGB value of this colour
        /// </summary>
        int Blue { get; }

        /// <summary>
        ///     Returns the BasicColour equivalent of this colour. For instance, "Navy Blue" would return BasicColour.Blue
        /// </summary>
        BasicColour Basic { get; }

        /// <summary>
        ///     A fancified string version of the colour, for example, "Midnight Black" might have "The colour of a starless
        ///     midnight sky" as its Fancy.
        /// </summary>
        string Fancy { get; }
    }
}
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Form.Material
{
    public enum Temperature
    {
        AbysmallyHot,
        Torrid,
        Sweltering,
        ExtremelyHot,
        VeryHot,
        Hot,
        VeryWarm,
        Warm,
        Temperate,
        Cool,
        Chilly,
        Cold,
        VeryCold,
        ExtremelyCold,
        Frigid,
        Freezing,
        AbysmallyCold
    }

    public static class TemperatureExtensions
    {
        public static string Describe(this Temperature item)
        {
            switch (item)
            {
                case Temperature.AbysmallyHot:
                    return "Abysmally Hot";
                case Temperature.Torrid:
                    return "Torrid";
                case Temperature.Sweltering:
                    return "Sweltering";
                case Temperature.ExtremelyHot:
                    return "Extremely Hot";
                case Temperature.VeryHot:
                    return "Very Hot";
                case Temperature.Hot:
                    return "Hot";
                case Temperature.VeryWarm:
                    return "Very Warm";
                case Temperature.Warm:
                    return "Warm";
                case Temperature.Temperate:
                    return "Temperate";
                case Temperature.Cool:
                    return "Cool";
                case Temperature.Chilly:
                    return "Chilly";
                case Temperature.Cold:
                    return "Cold";
                case Temperature.VeryCold:
                    return "Very Cold";
                case Temperature.ExtremelyCold:
                    return "Extremely Cold";
                case Temperature.Frigid:
                    return "Frigid";
                case Temperature.Freezing:
                    return "Freezing";
                case Temperature.AbysmallyCold:
                    return "Abysmally Cold";
            }

            return "Unknown";
        }

        public static string DescribeColour(this Temperature item)
        {
            switch (item)
            {
                case Temperature.AbysmallyHot:
                    return "Abysmally Hot".Colour(Telnet.BoldWhite);
                case Temperature.Torrid:
                    return "Torrid".Colour(Telnet.BoldRed);
                case Temperature.Sweltering:
                    return "Sweltering".Colour(Telnet.BoldRed);
                case Temperature.ExtremelyHot:
                    return "Extremely Hot".Colour(Telnet.Red);
                case Temperature.VeryHot:
                    return "Very Hot".Colour(Telnet.Red);
                case Temperature.Hot:
                    return "Hot".Colour(Telnet.Yellow);
                case Temperature.VeryWarm:
                    return "Very Warm".Colour(Telnet.Yellow);
                case Temperature.Warm:
                    return "Warm".Colour(Telnet.BoldYellow);
                case Temperature.Temperate:
                    return "Temperate".Colour(Telnet.BoldGreen);
                case Temperature.Cool:
                    return "Cool".Colour(Telnet.Green);
                case Temperature.Chilly:
                    return "Chilly".Colour(Telnet.Cyan);
                case Temperature.Cold:
                    return "Cold".Colour(Telnet.Cyan);
                case Temperature.VeryCold:
                    return "Very Cold".Colour(Telnet.BoldCyan);
                case Temperature.ExtremelyCold:
                    return "Extremely Cold".Colour(Telnet.BoldCyan);
                case Temperature.Frigid:
                    return "Frigid".Colour(Telnet.BoldBlue);
                case Temperature.Freezing:
                    return "Freezing".Colour(Telnet.BoldBlue);
                case Temperature.AbysmallyCold:
                    return "Abysmally Cold".Colour(Telnet.BoldMagenta);
            }

            return "Unknown";
        }

        public static Temperature SubjectiveTemperature(double value, double lower, double upper)
        {
            var range = upper - lower;
            if (value > lower + (range * 0.125 * 13))
            {
                return Temperature.AbysmallyHot;
            }
            if (value > lower + (range * 0.125 * 11.5))
            {
                return Temperature.Torrid;
            }
            if (value > lower + (range * 0.125 * 10))
            {
                return Temperature.Sweltering;
            }
            if (value > lower + (range * 0.125 * 9))
            {
                return Temperature.ExtremelyHot;
            }
            if (value > lower + (range * 0.125 * 8))
            {
                return Temperature.VeryHot;
            }
            if (value > lower + (range * 0.125 * 7))
            {
                return Temperature.Hot;
            }
            if (value > lower + (range * 0.125 * 6))
            {
                return Temperature.VeryWarm;
            }
            if (value > lower + (range * 0.125 * 5))
            {
                return Temperature.Warm;
            }
            if (value > lower + (range * 0.125 * 3)) // The jump here is intentional, so that temperate covers a slightly wider range of temperatures than other descriptors
            {
                return Temperature.Temperate;
            }
            if (value > lower + (range * 0.125 * 2))
            {
                return Temperature.Cool;
            }
            if (value > lower + (range * 0.125 * 1))
            {
                return Temperature.Chilly;
            }
            if (value > lower + (range * 0.125 * 0))
            {
                return Temperature.Cold;
            }
            if (value > lower + (range * 0.125 * -1))
            {
                return Temperature.VeryCold;
            }
            if (value > lower + (range * 0.125 * -2))
            {
                return Temperature.ExtremelyCold;
            }
            if (value > lower + (range * 0.125 * -3.5))
            {
                return Temperature.Frigid;
            }
            if (value > lower + (range * 0.125 * -5))
            {
                return Temperature.Freezing;
            }

            return Temperature.AbysmallyCold;
        }
    }
}

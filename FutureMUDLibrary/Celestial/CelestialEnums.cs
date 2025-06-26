using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Celestial
{
    public enum TimeOfDay
    {
        Night,
        Dawn,
        Morning,
        Afternoon,
        Dusk
    }

    public static class TimeOfDayExtensions
    {
        public static string Describe(this TimeOfDay time)
        {
            switch (time)
            {
                case TimeOfDay.Night:
                    return "Night";
                case TimeOfDay.Dawn:
                    return "Dawn";
                case TimeOfDay.Morning:
                    return "Morning";
                case TimeOfDay.Afternoon:
                    return "Afternoon";
                case TimeOfDay.Dusk:
                    return "Dusk";
            }

            return "Unknown";
        }

        public static string DescribeColour(this TimeOfDay time)
        {
            switch (time)
            {
                case TimeOfDay.Night:
                    return "Night".Colour(Telnet.Magenta);
                case TimeOfDay.Dawn:
                    return "Dawn".Colour(Telnet.BoldWhite);
                case TimeOfDay.Morning:
                    return "Morning".Colour(Telnet.BoldYellow);
                case TimeOfDay.Afternoon:
                    return "Afternoon".Colour(Telnet.Yellow);
                case TimeOfDay.Dusk:
                    return "Dusk".Colour(Telnet.Red);
            }

            return "Unknown";
        }
    }

    public enum MoonPhase
    {
        New,
        WaxingCrescent,
        FirstQuarter,
        WaxingGibbous,
        Full,
        WaningGibbous,
        LastQuarter,
        WaningCrescent
    }

    public static class MoonPhaseExtensions
    {
        public static string Describe(this MoonPhase phase)
        {
            switch (phase)
            {
                case MoonPhase.New:
                    return "New";
                case MoonPhase.WaxingCrescent:
                    return "Waxing Crescent";
                case MoonPhase.FirstQuarter:
                    return "First Quarter";
                case MoonPhase.WaxingGibbous:
                    return "Waxing Gibbous";
                case MoonPhase.Full:
                    return "Full";
                case MoonPhase.WaningGibbous:
                    return "Waning Gibbous";
                case MoonPhase.LastQuarter:
                    return "Last Quarter";
                case MoonPhase.WaningCrescent:
                    return "Waning Crescent";
            }

            return "Unknown";
        }
    }
}

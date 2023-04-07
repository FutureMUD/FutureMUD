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
}

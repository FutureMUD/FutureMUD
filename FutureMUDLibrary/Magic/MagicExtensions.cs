using System;

namespace MudSharp.Magic;

public static class MagicExtensions
{
    public static string LongDescription(this MagicPowerDistance distance)
    {
        switch (distance)
        {
            case MagicPowerDistance.AnyConnectedMind:
                return "any target whose mind you've connected to";
            case MagicPowerDistance.AnyConnectedMindOrConnectedTo:
                return "any target whose mind you've connected to, or who has connected to you";
            case MagicPowerDistance.SameLocationOnly:
                return "any target in the same location as you";
            case MagicPowerDistance.AdjacentLocationsOnly:
                return "any target in the same location as you or adjacent locations";
            case MagicPowerDistance.SameAreaOnly:
                return "any target in the same area as you";
            case MagicPowerDistance.SameZoneOnly:
                return "any target in the same zone as you";
            case MagicPowerDistance.SameShardOnly:
                return "any target in the same planet as you";
            case MagicPowerDistance.SamePlaneOnly:
                return "any target in the same plane as you";
            case MagicPowerDistance.SeenTargetOnly:
                return "any target you have seen or spotted with scan";
            default:
                throw new ArgumentOutOfRangeException(nameof(distance), distance, null);
        }
    }
}
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Construction
{
    public enum RoomLayer
    {
        GroundLevel = 0,
        Underwater = 1,
        DeepUnderwater = 2,
        VeryDeepUnderwater = 3,
        InTrees = 4,
        HighInTrees = 5 ,
        InAir = 6,
        HighInAir = 7,
        OnRooftops = 8
    }

    public enum CellOutdoorsType
    {
        Indoors,
        IndoorsWithWindows,
        Outdoors,
        IndoorsNoLight,
        IndoorsClimateExposed
    }

    public enum Proximity
    {
        /// <summary>
        /// Intimate is the closest proximity - for things that happen in the same location or so close they would otherwise be touching
        /// A usual MUD example would be an entity to itself is intimate. Contents of a container to the container or inventory items to a character would be similarly proximate.
        /// </summary>
        Intimate = 0,

        /// <summary>
        /// Immediate is the second closest proximity; things that are not quite close enough to be touching, but not much farther away
        /// The usual MUD example would be things that are definitely marked as being near - targeting each other in an emote, a character attending to another character etc, character to cover
        /// </summary>
        Immediate,

        /// <summary>
        /// Proximate is for things that are close, but not super close.
        /// The usual MUD example would be things that are probably close by on the balance of probability - people in the same group for example
        /// </summary>
        Proximate,

        /// <summary>
        /// Distant is for things that are in the same location, but not in any other way close to each other.
        /// The usual MUD example is any two items in the same cell that do not otherwise have a relationship
        /// </summary>
        Distant,

        /// <summary>
        /// VeryDistance is for things that are in the same location but not in the same layer
        /// </summary>
        VeryDistant,

        /// <summary>
        /// Unapproximable is things that are not at the same location, or are otherwise too far apart to have a proximity
        /// </summary>
        Unapproximable
    }

    public static class CellOutdoorsTypeExtension
    {
        public static string Describe(this CellOutdoorsType type)
        {
            switch (type)
            {
                case CellOutdoorsType.Indoors:
                    return "Indoors";
                case CellOutdoorsType.IndoorsWithWindows:
                    return "Indoors (With View of Outside)";
                case CellOutdoorsType.Outdoors:
                    return "Outdoors";
                case CellOutdoorsType.IndoorsNoLight:
                    return "Indoors (With No Natural Light)";
                case CellOutdoorsType.IndoorsClimateExposed:
                    return "Indoors (Exposed to Climate)";
                default:
                    throw new NotSupportedException("Invalid CellOutdoorsType in CellOutdoorsTypeExtension.Describe");
            }
        }
    }

    public static class ConstructionExtensions
    {
        public static string Describe(this Proximity proximity)
        {
            switch (proximity)
            {
                case Proximity.Intimate:
                    return "Intimate";
                case Proximity.Immediate:
                    return "Immediate";
                case Proximity.Proximate:
                    return "Proximate";
                case Proximity.Distant:
                    return "Distant";
                case Proximity.Unapproximable:
                    return "Unapproximable";
                case Proximity.VeryDistant:
                    return "Very Distant";
            }

            throw new ApplicationException("Unknown Proximity type in Proximity.Describe");
        }

        public static bool IsLowerThan(this RoomLayer baseLayer, RoomLayer otherLayer)
        {
            return otherLayer.IsHigherThan(baseLayer);
        }

        public static bool IsHigherThan(this RoomLayer baseLayer, RoomLayer otherLayer)
        {
            switch (baseLayer)
            {
                case RoomLayer.GroundLevel:
                    return otherLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater);
                case RoomLayer.Underwater:
                    return otherLayer.In(RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater);
                case RoomLayer.DeepUnderwater:
                    return otherLayer == RoomLayer.VeryDeepUnderwater;
                case RoomLayer.VeryDeepUnderwater:
                    return false;
                case RoomLayer.InTrees:
                    return otherLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater, RoomLayer.GroundLevel);
                case RoomLayer.HighInTrees:
                    return otherLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater, RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.OnRooftops);
                case RoomLayer.InAir:
                    return otherLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater, RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.OnRooftops, RoomLayer.HighInTrees);
                case RoomLayer.HighInAir:
                    return otherLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater, RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.OnRooftops, RoomLayer.HighInTrees, RoomLayer.InAir);
                case RoomLayer.OnRooftops:
                    return otherLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater, RoomLayer.GroundLevel);
            }

            return false;
        }

        public static int LayerHeight(this RoomLayer layer)
        {
            switch (layer)
            {
                case RoomLayer.GroundLevel:
                    return 0;
                case RoomLayer.Underwater:
                    return -1;
                case RoomLayer.DeepUnderwater:
                    return -2;
                case RoomLayer.VeryDeepUnderwater:
                    return -3;
                case RoomLayer.InTrees:
                    return 1;
                case RoomLayer.HighInTrees:
                    return 2;
                case RoomLayer.InAir:
                    return 3;
                case RoomLayer.HighInAir:
                    return 4;
                case RoomLayer.OnRooftops:
                    return 1;
                default:
                    return 0;
            }
        }

        public static RoomLayer ClosestLayer<T>(this T layers, RoomLayer targetLayer) where T : IEnumerable<RoomLayer>
        {
            return layers.WhereMin(x => Math.Abs(x.LayerHeight() - targetLayer.LayerHeight())).FirstOrDefault();
        }

        public static RoomLayer HighestLayer<T>(this T layers) where T : IEnumerable<RoomLayer>
        {
            return layers.FirstMax(x => x.LayerHeight());
        }

        public static RoomLayer LowestLayer<T>(this T layers) where T : IEnumerable<RoomLayer>
        {
            return layers.FirstMin(x => x.LayerHeight());
        }

        public static bool IsUnderwater(this RoomLayer layer)
        {
            return layer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater);
        }

        public static IEnumerable<RoomLayer> IntermediateLayers<T>(this T layers, RoomLayer layer1, RoomLayer layer2) where T : IEnumerable<RoomLayer>
        {
            if (layer1.IsHigherThan(layer2))
            {
                return layers.Where(x => x.IsLowerThan(layer1) && x.IsHigherThan(layer2));
            }

            return layers.Where(x => x.IsLowerThan(layer2) && x.IsHigherThan(layer1));
        }

        public static string PositionalDescription(this RoomLayer layer)
        {
            switch (layer)
            {
                case RoomLayer.GroundLevel:
                    return "on the ground";
                case RoomLayer.Underwater:
                    return "in the water";
                case RoomLayer.DeepUnderwater:
                    return "in the water";
                case RoomLayer.VeryDeepUnderwater:
                    return "in the water";
                case RoomLayer.InTrees:
                    return "in the trees";
                case RoomLayer.HighInTrees:
                    return "in the trees";
                case RoomLayer.InAir:
                    return "in the air";
                case RoomLayer.HighInAir:
                    return "in the air";
                case RoomLayer.OnRooftops:
                    return "on the rooftops";
            }

            throw new ArgumentOutOfRangeException(nameof(layer), "Invalid RoomLayer argument in PositionalDescription extension method");
        }

        public static string OriginDescription(this RoomLayer layer)
        {
	        switch (layer)
	        {
		        case RoomLayer.GroundLevel:
			        return "from the ground";
		        case RoomLayer.Underwater:
		        case RoomLayer.DeepUnderwater:
		        case RoomLayer.VeryDeepUnderwater:
			        return "from under the water";
		        case RoomLayer.InTrees:
		        case RoomLayer.HighInTrees:
                    return "from the trees";
		        case RoomLayer.InAir:
		        case RoomLayer.HighInAir:
                    return "from the air";
		        case RoomLayer.OnRooftops:
			        return "from the rooftops";
	        }

	        throw new ArgumentOutOfRangeException(nameof(layer), "Invalid RoomLayer argument in OriginDescription extension method");
        }

        public static string LocativeDescription(this RoomLayer layer)
        {
            switch (layer)
            {
                case RoomLayer.GroundLevel:
                    return "at ground level";
                case RoomLayer.Underwater:
                    return "underwater";
                case RoomLayer.DeepUnderwater:
                    return "deep underwater";
                case RoomLayer.VeryDeepUnderwater:
                    return "very deep underwater";
                case RoomLayer.InTrees:
                    return "in the trees";
                case RoomLayer.HighInTrees:
                    return "high in the trees";
                case RoomLayer.InAir:
                    return "in the air";
                case RoomLayer.HighInAir:
                    return "high in the air";
                case RoomLayer.OnRooftops:
                    return "on the rooftops";
            }

            throw new ArgumentOutOfRangeException(nameof(layer), "Invalid RoomLayer argument in LocativeDescription extension method");
        }

        public static string DativeDescription(this RoomLayer layer)
        {
            switch (layer)
            {
                case RoomLayer.GroundLevel:
                    return "towards the ground";
                case RoomLayer.Underwater:
                    return "underwater";
                case RoomLayer.DeepUnderwater:
                    return "deep underwater";
                case RoomLayer.VeryDeepUnderwater:
                    return "very deep underwater";
                case RoomLayer.InTrees:
                    return "into the trees";
                case RoomLayer.HighInTrees:
                    return "high into the trees";
                case RoomLayer.InAir:
                    return "into the air";
                case RoomLayer.HighInAir:
                    return "high into the air";
                case RoomLayer.OnRooftops:
                    return "towards the rooftops";
            }

            throw new ArgumentOutOfRangeException(nameof(layer), "Invalid RoomLayer argument in DativeDescription extension method");
        }

        public static string ColouredTag(this RoomLayer layer)
        {
            switch (layer)
            {
                case RoomLayer.GroundLevel:
                    return "[Ground]".Colour(Telnet.Yellow);
                case RoomLayer.Underwater:
                    return "[Underwater]".Colour(Telnet.BoldBlue);
                case RoomLayer.DeepUnderwater:
                    return "[DeepUnderwater]".Colour(Telnet.BoldBlue);
                case RoomLayer.VeryDeepUnderwater:
                    return "[VeryDeepUnderwater]".Colour(Telnet.BoldBlue);
                case RoomLayer.InTrees:
                    return "[Trees]".Colour(Telnet.BoldGreen);
                case RoomLayer.HighInTrees:
                    return "[HighTrees]".Colour(Telnet.BoldGreen);
                case RoomLayer.InAir:
                    return "[Air]".Colour(Telnet.BoldCyan);
                case RoomLayer.HighInAir:
                    return "[HighAir]".Colour(Telnet.BoldCyan);
                case RoomLayer.OnRooftops:
                    return "[Rooftops]".Colour(Telnet.BoldYellow);
            }

            throw new ArgumentOutOfRangeException(nameof(layer), "Invalid RoomLayer argument in ColouredTag extension method");
        }

        /// <summary>
        /// Determines whether a perceiver in the specified layer can see things in this layer in ranged perception such as scan
        /// </summary>
        /// <param name="layer">The layer of the thing being perceived</param>
        /// <param name="perceiverLayer">The layer of the perceiver</param>
        /// <returns>True if the layer should be seen in scan</returns>
        public static bool CanBeSeenFromLayer(this RoomLayer layer, RoomLayer perceiverLayer)
        {
            switch (layer)
            {
                case RoomLayer.GroundLevel:
                case RoomLayer.InTrees:
                case RoomLayer.HighInTrees:
                case RoomLayer.InAir:
                case RoomLayer.HighInAir:
                case RoomLayer.OnRooftops:
                    return !perceiverLayer.IsLowerThan(layer);
                case RoomLayer.Underwater:
                    return perceiverLayer.In(RoomLayer.GroundLevel, RoomLayer.Underwater, RoomLayer.DeepUnderwater);
                case RoomLayer.DeepUnderwater:
                    return perceiverLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater);
                case RoomLayer.VeryDeepUnderwater:
                    return perceiverLayer.In(RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater);
                                    
            }

            throw new ArgumentOutOfRangeException(nameof(layer), "Invalid RoomLayer argument in CanBeSeenFromLayer extension method");
        }

        /// <summary>
        /// Determines whether a perceiver in the specified layer can see things in this layer in ranged perception such as longscan
        /// </summary>
        /// <param name="layer">The layer of the thing being perceived</param>
        /// <param name="perceiverLayer">The layer of the perceiver</param>
        /// <returns>True if the layer should be seen in longscan</returns>
        public static bool CanBeSeenFromLayerForLongscan(this RoomLayer layer, RoomLayer perceiverLayer)
        {
            switch (layer)
            {
                case RoomLayer.GroundLevel:
                case RoomLayer.InTrees:
                case RoomLayer.HighInTrees:
                case RoomLayer.InAir:
                case RoomLayer.HighInAir:
                case RoomLayer.OnRooftops:
                    return !perceiverLayer.IsLowerThan(RoomLayer.GroundLevel);
                case RoomLayer.Underwater:
                    return perceiverLayer.In(RoomLayer.GroundLevel, RoomLayer.Underwater, RoomLayer.DeepUnderwater);
                case RoomLayer.DeepUnderwater:
                    return perceiverLayer.In(RoomLayer.Underwater, RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater);
                case RoomLayer.VeryDeepUnderwater:
                    return perceiverLayer.In(RoomLayer.DeepUnderwater, RoomLayer.VeryDeepUnderwater);

            }

            throw new ArgumentOutOfRangeException(nameof(layer), "Invalid RoomLayer argument in CanBeSeenFromLayer extension method");
        }
    }
}

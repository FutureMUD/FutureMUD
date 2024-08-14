using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Body {
	public enum Alignment {
		// X axis location for this bodypart.
		Irrelevant = 0,
		Left = 1,
		Front = 2,
		Right = 3,
		Rear = 4,
		FrontLeft = 5,
		FrontRight = 6,
		RearLeft = 7,
		RearRight = 8
	}

	public enum Orientation {
		// Y axis location for this bodypart.
		Irrelevant = 0,
		Lowest = 1,
		Low = 2,
		Centre = 3,
		High = 4,
		Highest = 5,
		Appendage = 6
	}

	public enum Facing {
		Front,
		RightFlank,
		Rear,
		LeftFlank
	}

	public static class BodyExtensions {
		public static IBodypart GetBodypartByName<T>(this T parts, string name, bool abbreviationsPermitted = true) where T : IEnumerable<IBodypart>
		{
			if (abbreviationsPermitted)
			{
				return parts.FirstOrDefault(x => x.Name.EqualTo(name)) ??
					   parts.FirstOrDefault(x => x.FullDescription().EqualTo(name)) ??
					   parts.FirstOrDefault(x => x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase) || x.FullDescription().StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
			}

			return parts.FirstOrDefault(x => x.Name.EqualTo(name)) ??
				   parts.FirstOrDefault(x => x.FullDescription().EqualTo(name));
		}

		public static double StaminaMultiplierForEncumbrance(this EncumbranceLevel level)
		{
			switch (level)
			{
				case EncumbranceLevel.Unencumbered:
					return 1.0;
				case EncumbranceLevel.LightlyEncumbered:
					return 1.15;
				case EncumbranceLevel.ModeratelyEncumbered:
					return 1.30;
				case EncumbranceLevel.HeavilyEncumbered:
					return 1.50;
				case EncumbranceLevel.CriticallyEncumbered:
					return 2.0;
			}

			return 1.0;
		}

		public static string DescribeColoured(this EncumbranceLevel level)
		{
			switch (level)
			{
				case EncumbranceLevel.Unencumbered:
					return "Unencumbered".Colour(Telnet.Green);
				case EncumbranceLevel.LightlyEncumbered:
					return "Lightly Encumbered".Colour(Telnet.BoldGreen);
				case EncumbranceLevel.ModeratelyEncumbered:
					return "Moderately Encumbered".Colour(Telnet.BoldYellow);
				case EncumbranceLevel.HeavilyEncumbered:
					return "Heavily Encumbered".Colour(Telnet.BoldRed);
				case EncumbranceLevel.CriticallyEncumbered:
					return "Critically Encumbered".Colour(Telnet.BoldMagenta);
				default:
					throw new ArgumentException("Unknown EncumbranceLevel in DescribeColoured.", nameof(level));
			}
		}
		public static IEnumerable<IBodypart> PruneBodyparts(this IEnumerable<IBodypart> parts,
			IEnumerable<IBodypart> prune) {
			if (!prune.Any()) {
				return parts;
			}

			var result = new List<IBodypart>(parts);
			foreach (var item in prune) {
				result.RemoveAll(x => x.DownstreamOfPart(item));
				result.Remove(item);
			}

			return result;
		}

		private static int ToAlignmentPolar(this Alignment alignment) {
			switch (alignment) {
				case Alignment.Front:
					return 0;
				case Alignment.FrontRight:
					return 1;
				case Alignment.Right:
					return 2;
				case Alignment.RearRight:
					return 3;
				case Alignment.Rear:
					return 4;
				case Alignment.RearLeft:
					return 5;
				case Alignment.Left:
					return 6;
				case Alignment.FrontLeft:
					return 7;
			}

			return 0;
		}

		public static int ToAlignmentPolar(this Alignment alignment, Facing offsetFacing) {
			var offset = 0;
			switch (offsetFacing) {
				case Facing.Front:
					offset = 0;
					break;
				case Facing.RightFlank:
					offset = 2;
					break;
				case Facing.Rear:
					offset = 4;
					break;
				case Facing.LeftFlank:
					offset = 6;
					break;
			}

			switch (alignment) {
				case Alignment.Front:
					return (0 + offset)%8;
				case Alignment.FrontRight:
					return (1 + offset)%8;
				case Alignment.Right:
					return (2 + offset)%8;
				case Alignment.RearRight:
					return (3 + offset)%8;
				case Alignment.Rear:
					return (4 + offset)%8;
				case Alignment.RearLeft:
					return (5 + offset)%8;
				case Alignment.Left:
					return (6 + offset)%8;
				case Alignment.FrontLeft:
					return (7 + offset)%8;
			}

			return offset;
		}

		public static Alignment FromAlignmentPolar(int alignmentPolar) {
			switch (alignmentPolar) {
				case 0:
					return Alignment.Front;
				case 1:
					return Alignment.FrontRight;
				case 2:
					return Alignment.Right;
				case 3:
					return Alignment.RearRight;
				case 4:
					return Alignment.Rear;
				case 5:
					return Alignment.RearLeft;
				case 6:
					return Alignment.Left;
				case 7:
					return Alignment.FrontLeft;
				default:
					return Alignment.Front;
			}
		}

		public static Orientation HeightToOrientation(double HitHeight, double TotalHeight, Orientation appendagesMightBeSubstitutedForThisOrientation)
		{
			if (HitHeight >= TotalHeight)
			{
				return Orientation.Highest;
			}

			if (HitHeight <= 0.0)
			{
				return Orientation.Lowest;
			}

			Orientation NonAppendageHeightToOrientation()
			{
				var ratio = HitHeight / TotalHeight;
				if (ratio < 0.15)
				{
					return Orientation.Lowest;
				}
				else if (ratio < 0.35)
				{
					return Orientation.Low;
				}
				else if (ratio < 0.65)
				{
					return Orientation.Centre;
				}
				else if (ratio < 0.85)
				{
					return Orientation.High;
				}
				else
				{
					return Orientation.Highest;
				}
			}

			var value = NonAppendageHeightToOrientation();
			if (value == appendagesMightBeSubstitutedForThisOrientation && Dice.Roll(1, 3) == 0)
			{
				return Orientation.Appendage;
			}

			return value;
		}

		public static (Orientation,Alignment) GetRandomHitLocation(Orientation attackOrientation,
			Alignment attackAlignment, Facing attackFacing, bool substituteAppendage = false) {
			var orientationRoll = RandomUtilities.Random(1, 100);
			var alignmentRoll = RandomUtilities.Random(1, 100);

			int alignmentOffset;
			if (alignmentRoll < 76) {
				alignmentOffset = 0;
			}
			else if (alignmentRoll < 88) {
				alignmentOffset = 1;
			}
			else {
				alignmentOffset = -1;
			}

			var finalAlignment = FromAlignmentPolar((attackAlignment.ToAlignmentPolar(attackFacing) + alignmentOffset)%8);

			int orientationOffset;
			if (orientationRoll < 76) {
				orientationOffset = 0;
			}
			else if (orientationRoll < 88) {
				orientationOffset = 1;
			}
			else {
				orientationOffset = -1;
			}

			var finalOrientation = attackOrientation.RaiseUp(orientationOffset);
			if (substituteAppendage) {
				// This scenario represents one where an appendage is actively being used to defend, and so may be hit
				if (RandomUtilities.Random(1, 3) == 1) {
					finalOrientation = Orientation.Appendage;
				}
			}
			else {
				//// Apendage may be substituted for a centre hit
				//if ((finalOrientation == Orientation.Centre) && (RandomUtilities.Random(1, 2) == 1)) {
				//    finalOrientation = Orientation.Appendage;
				//}
				//else if ((finalOrientation == Orientation.Low) && (RandomUtilities.Random(1, 4) == 1)) {
				//    finalOrientation = Orientation.Appendage;
				//}
				//else if ((finalOrientation == Orientation.High) && (RandomUtilities.Random(1, 4) == 1)) {
				//    finalOrientation = Orientation.Appendage;
				//}
			}

			return (finalOrientation, finalAlignment);
		}

		public static string Describe(this Orientation orientation) {
			switch (orientation) {
				case Orientation.Irrelevant:
					return "";
				case Orientation.Lowest:
					return "Lowest";
				case Orientation.Low:
					return "Low";
				case Orientation.Centre:
					return "Centre";
				case Orientation.High:
					return "High";
				case Orientation.Highest:
					return "Highest";
				case Orientation.Appendage:
					return "Appendage";
			}

			return "Unknown";
		}

		public static string Describe(this Facing facing) {
			switch (facing) {
				case Facing.Front:
					return "Front";
				case Facing.LeftFlank:
					return "Left Flank";
				case Facing.Rear:
					return "Rear";
				case Facing.RightFlank:
					return "Right Flank";
			}
			return "Unknown";
		}

		public static string DescribeAsHandedness(this Alignment alignment)
		{
			if (alignment == Alignment.Irrelevant)
			{
				return "Ambidextrous";
			}

			return $"{alignment.Describe()}-Handed";
		}

		public static string Describe(this Alignment alignment) {
			switch (alignment) {
				case Alignment.Irrelevant:
					return "";
				case Alignment.Front:
					return "Front";
				case Alignment.Left:
					return "Left";
				case Alignment.Rear:
					return "Rear";
				case Alignment.Right:
					return "Right";
				case Alignment.FrontLeft:
					return "Front Left";
				case Alignment.FrontRight:
					return "Front Right";
				case Alignment.RearLeft:
					return "Rear Left";
				case Alignment.RearRight:
					return "Rear Right";
			}

			return "Unknown";
		}

		public static Alignment LeftRightOnly(this Alignment alignment) {
			switch (alignment) {
				case Alignment.RearRight:
				case Alignment.FrontRight:
				case Alignment.Right:
					return Alignment.Right;
				case Alignment.RearLeft:
				case Alignment.FrontLeft:
				case Alignment.Left:
					return Alignment.Left;
			}

			return Alignment.Irrelevant;
		}

		public static Alignment FrontRearOnly(this Alignment alignment) {
			switch (alignment) {
				case Alignment.RearRight:
				case Alignment.RearLeft:
				case Alignment.Rear:
					return Alignment.Rear;
				case Alignment.FrontRight:
				case Alignment.FrontLeft:
				case Alignment.Front:
					return Alignment.Front;
			}

			return Alignment.Irrelevant;
		}

		public static bool IsCompoundAlignment(this Alignment alignment) {
			switch (alignment) {
				case Alignment.RearLeft:
				case Alignment.RearRight:
				case Alignment.FrontLeft:
				case Alignment.FrontRight:
					return true;
			}

			return false;
		}

		public static Alignment SwitchLeftRight(this Alignment alignment) {
			switch (alignment) {
				case Alignment.Front:
				case Alignment.Rear:
				case Alignment.Irrelevant:
					return alignment;
				case Alignment.FrontLeft:
					return Alignment.FrontRight;
				case Alignment.FrontRight:
					return Alignment.FrontLeft;
				case Alignment.RearLeft:
					return Alignment.RearRight;
				case Alignment.RearRight:
					return Alignment.RearLeft;
				case Alignment.Left:
					return Alignment.Right;
				case Alignment.Right:
					return Alignment.Left;
			}

			return alignment;
		}

		public static Orientation RaiseUp(this Orientation orientation, int degrees) {
			if ((orientation == Orientation.Appendage) || (orientation == Orientation.Irrelevant)) {
				return orientation;
			}

			var orientationAsInt = (int) orientation;
			if (orientationAsInt + degrees >= (int) Orientation.Highest) {
				return Orientation.Highest;
			}
			if (orientationAsInt + degrees <= (int) Orientation.Lowest) {
				return Orientation.Lowest;
			}

			return (Orientation) (orientationAsInt + degrees);
		}

		public static Facing ToFacing(this Alignment alignment) {
			switch (alignment)
			{
				case Alignment.Irrelevant:
				case Alignment.Front:
				case Alignment.FrontLeft:
				case Alignment.FrontRight:
					return Facing.Front;
				case Alignment.Left:
				case Alignment.RearLeft:
					return Facing.LeftFlank;
				case Alignment.Right:
				case Alignment.RearRight:
					return Facing.RightFlank;
				case Alignment.Rear:
					return Facing.Rear;
				default:
					return Facing.Front;
			}
		}

		public static bool WithinOffset(this Alignment alignment, Alignment other, int degrees) {
			return (8 - Math.Abs(alignment.ToAlignmentPolar() - other.ToAlignmentPolar()) % 8 <= degrees) ||
				   (Math.Abs(alignment.ToAlignmentPolar() - other.ToAlignmentPolar()) % 8 <= degrees);
		}

		public static bool WithinOffset(this Orientation orientation, Orientation other, int degrees)
		{
			for (var i = 0; i <= degrees; i++) {
				if (orientation.RaiseUp(i) == other || orientation.RaiseUp(-1 * i) == other) {
					return true;
				}
			}

			return false;
		}
	}
}
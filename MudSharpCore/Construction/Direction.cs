using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

public static partial class DirectionExtensions
{
	/// <summary>
	/// Describes a series of exits in terms of "the East" or "the Shaggy Duck Tavern"
	/// </summary>
	/// <param name="exits">A series of exits, with the order of the enumeration reflecting the distance from the origin</param>
	/// <returns>A string describing the directions</returns>
	public static string DescribeDirection(this IEnumerable<ICellExit> exits)
	{
		var exitFound = false;
		ICellExit firstNonCardinal = null;
		foreach (var exit in exits)
		{
			if (exit.OutboundDirection != CardinalDirection.Unknown)
			{
				exitFound = true;
				break;
			}

			if (firstNonCardinal != null)
			{
				continue;
			}

			firstNonCardinal = exit;
		}

		if (!exitFound && firstNonCardinal != null)
		{
			return firstNonCardinal.OutboundDirectionDescription;
		}

		var counts = exits.CountDirections();
		return
			$"the {CardinalDirectionExtensions.DescribeDirections(counts.Northness, counts.Southness, counts.Westness, counts.Eastness, counts.Upness, counts.Downness)}";
	}

	/// <summary>
	/// Describes a distance/direction combo in the form of "very far to the north-north east" or "from the shaggy duck tavern"
	/// </summary>
	/// <param name="exits">A series of exits, with the order of the enumeration reflecting the distance from the origin</param>
	/// <param name="distance">Distance as the crow flies for the exits</param>
	/// <returns>Text describing the directions</returns>
	public static string DescribeDirectionsToFrom(this IEnumerable<ICellExit> exits)
	{
		var exitFound = false;
		ICellExit firstNonCardinal = null;
		foreach (var exit in exits)
		{
			if (exit.OutboundDirection != CardinalDirection.Unknown)
			{
				exitFound = true;
				break;
			}

			if (firstNonCardinal != null)
			{
				continue;
			}

			firstNonCardinal = exit;
		}

		if (!exitFound && firstNonCardinal != null)
		{
			return firstNonCardinal.OutboundDirectionSuffix;
		}

		var distance = exits.Select(x => x.OutboundDirection).DistanceAsCrowFlies();

		var distanceDescription = "extremely far";
		switch (distance)
		{
			case 1:
				distanceDescription = "immediately";
				break;
			case 2:
				distanceDescription = "close by";
				break;
			case 3:
				distanceDescription = "far";
				break;
			case 4:
				distanceDescription = "very far";
				break;
		}

		var counts = exits.CountDirections();
		return
			$"{distanceDescription.SpaceIfNotEmpty()}to the {CardinalDirectionExtensions.DescribeDirections(counts.Northness, counts.Southness, counts.Westness, counts.Eastness, counts.Upness, counts.Downness)}";
	}

	public static string DescribeDirectionKeywords<T>(this T path, ANSIColour colour = null)
		where T : IEnumerable<ICellExit>
	{
		return path.Select(exit =>
			           (exit is NonCardinalCellExit ncce
				           ? $"{ncce.Verb} {ncce.PrimaryKeyword}".ToLowerInvariant()
				           : exit.OutboundDirection.DescribeBrief()).Colour(colour))
		           .ListToString(conjunction: "", twoItemJoiner: "");
	}

	public static (int Northness, int Southness, int Westness, int Eastness, int Upness, int Downness)
		CountDirections(this IEnumerable<ICellExit> directions)
	{
		int northness = 0, southness = 0, westness = 0, eastness = 0, upness = 0, downness = 0;
		foreach (var direction in directions)
		{
			switch (direction.OutboundDirection)
			{
				case CardinalDirection.North:
					northness += 1;
					southness -= 1;
					break;
				case CardinalDirection.NorthEast:
					northness += 1;
					southness -= 1;
					eastness += 1;
					westness -= 1;
					break;
				case CardinalDirection.East:
					eastness += 1;
					westness -= 1;
					break;
				case CardinalDirection.SouthEast:
					southness += 1;
					northness -= 1;
					eastness += 1;
					westness -= 1;
					break;
				case CardinalDirection.South:
					southness += 1;
					northness -= 1;
					break;
				case CardinalDirection.SouthWest:
					southness += 1;
					northness -= 1;
					westness += 1;
					eastness -= 1;
					break;
				case CardinalDirection.West:
					westness += 1;
					eastness -= 1;
					break;
				case CardinalDirection.NorthWest:
					northness += 1;
					southness -= 1;
					westness += 1;
					eastness -= 1;
					break;
				case CardinalDirection.Up:
					upness += 1;
					downness -= 1;
					break;
				case CardinalDirection.Down:
					downness += 1;
					upness -= 1;
					break;
			}
		}

		return (northness, southness, westness, eastness, upness, downness);
	}

	public static (int Northness, int Southness, int Westness, int Eastness, int Upness, int Downness)
		CountTotalDirections<U, T>(this U directions) where T : IEnumerable<ICellExit> where U : IEnumerable<T>
	{
		int northness = 0, southness = 0, westness = 0, eastness = 0, upness = 0, downness = 0;
		foreach (var item in directions)
		foreach (var direction in item)
		{
			switch (direction.OutboundDirection)
			{
				case CardinalDirection.North:
					northness += 1;
					southness -= 1;
					break;
				case CardinalDirection.NorthEast:
					northness += 1;
					southness -= 1;
					eastness += 1;
					westness -= 1;
					break;
				case CardinalDirection.East:
					eastness += 1;
					westness -= 1;
					break;
				case CardinalDirection.SouthEast:
					southness += 1;
					northness -= 1;
					eastness += 1;
					westness -= 1;
					break;
				case CardinalDirection.South:
					southness += 1;
					northness -= 1;
					break;
				case CardinalDirection.SouthWest:
					southness += 1;
					northness -= 1;
					westness += 1;
					eastness -= 1;
					break;
				case CardinalDirection.West:
					westness += 1;
					eastness -= 1;
					break;
				case CardinalDirection.NorthWest:
					northness += 1;
					southness -= 1;
					westness += 1;
					eastness -= 1;
					break;
				case CardinalDirection.Up:
					upness += 1;
					downness -= 1;
					break;
				case CardinalDirection.Down:
					downness += 1;
					upness -= 1;
					break;
			}
		}

		return (northness, southness, westness, eastness, upness, downness);
	}
}
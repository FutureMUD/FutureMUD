using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Currency;

namespace MudSharp.Construction
{
	public enum CardinalDirection
	{
		North = 0,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest,
		Up,
		Down,
		Unknown
	}

	public static partial class CardinalDirectionExtensions
	{
		public static IReadOnlyDictionary<string, CardinalDirection> CardinalExitStrings { get; } = new Dictionary<string, CardinalDirection>(StringComparer.OrdinalIgnoreCase)
		{
			{ "n", CardinalDirection.North },
			{ "no", CardinalDirection.North },
			{ "nor", CardinalDirection.North },
			{ "nort", CardinalDirection.North },
			{ "north", CardinalDirection.North },
			{ "e", CardinalDirection.East },
			{ "ea", CardinalDirection.East },
			{ "eas", CardinalDirection.East },
			{ "east", CardinalDirection.East },
			{ "s", CardinalDirection.South },
			{ "so", CardinalDirection.South },
			{ "sou", CardinalDirection.South },
			{ "sout", CardinalDirection.South },
			{ "south", CardinalDirection.South },
			{ "w", CardinalDirection.West },
			{ "we", CardinalDirection.West },
			{ "wes", CardinalDirection.West },
			{ "west", CardinalDirection.West },
			{ "ne", CardinalDirection.NorthEast },
			{ "northeast", CardinalDirection.NorthEast },
			{ "northe", CardinalDirection.NorthEast },
			{ "northea", CardinalDirection.NorthEast },
			{ "northes", CardinalDirection.NorthEast },
			{ "nw", CardinalDirection.NorthWest },
			{ "northwest", CardinalDirection.NorthWest },
			{ "northw", CardinalDirection.NorthWest },
			{ "northwe", CardinalDirection.NorthWest },
			{ "northwes", CardinalDirection.NorthWest },
			{ "se", CardinalDirection.SouthEast },
			{ "southeast", CardinalDirection.SouthEast },
			{ "southe", CardinalDirection.SouthEast },
			{ "southea", CardinalDirection.SouthEast },
			{ "southeas", CardinalDirection.SouthEast },
			{ "sw", CardinalDirection.SouthWest },
			{ "southwest", CardinalDirection.SouthWest },
			{ "southw", CardinalDirection.SouthWest },
			{ "southwe", CardinalDirection.SouthWest },
			{ "southwes", CardinalDirection.SouthWest },
			{ "u", CardinalDirection.Up },
			{ "up", CardinalDirection.Up },
			{ "d", CardinalDirection.Down },
			{ "do", CardinalDirection.Down },
			{ "dow", CardinalDirection.Down },
			{ "down", CardinalDirection.Down },
			{ "dn", CardinalDirection.Down },
		};
		public static List<CardinalDirection> ContainedDirections(this (int Northness, int Southness, int Westness, int Eastness, int Upness, int Downness, int Unknownness) directions)
		{
			var list = new List<CardinalDirection>();
			if (directions.Northness > 0)
			{
				list.Add(CardinalDirection.North);
				if (directions.Eastness > 0)
				{
					list.Add(CardinalDirection.NorthEast);
				}
				if (directions.Westness > 0)
				{
					list.Add(CardinalDirection.NorthWest);
				}
			}

			if (directions.Southness > 0)
			{
				list.Add(CardinalDirection.South);
				if (directions.Eastness > 0)
				{
					list.Add(CardinalDirection.SouthEast);
				}
				if (directions.Westness > 0)
				{
					list.Add(CardinalDirection.SouthWest);
				}
			}

			if (directions.Eastness > 0)
			{
				list.Add(CardinalDirection.East);
			}

			if (directions.Westness > 0)
			{
				list.Add(CardinalDirection.West);
			}

			if (directions.Upness > 0)
			{
				list.Add(CardinalDirection.Up);
			}

			if (directions.Downness > 0)
			{
				list.Add(CardinalDirection.Down);
			}

			if (directions.Unknownness == 0)
			{
				list.Add(CardinalDirection.Unknown);
			}
			return list;
		}

		public static List<CardinalDirection> ContainedDirections(this (int Northness, int Southness, int Westness, int Eastness, int Upness, int Downness) directions)
		{
			var list = new List<CardinalDirection>();
			if (directions.Northness > 0)
			{
				list.Add(CardinalDirection.North);
				if (directions.Eastness > 0)
				{
					list.Add(CardinalDirection.NorthEast);
				}
				if (directions.Westness > 0)
				{
					list.Add(CardinalDirection.NorthWest);
				}
			}

			if (directions.Southness > 0)
			{
				list.Add(CardinalDirection.South);
				if (directions.Eastness > 0)
				{
					list.Add(CardinalDirection.SouthEast);
				}
				if (directions.Westness > 0)
				{
					list.Add(CardinalDirection.SouthWest);
				}
			}

			if (directions.Eastness > 0)
			{
				list.Add(CardinalDirection.East);
			}

			if (directions.Westness > 0)
			{
				list.Add(CardinalDirection.West);
			}

			if (directions.Upness > 0)
			{
				list.Add(CardinalDirection.Up);
			}

			if (directions.Downness > 0)
			{
				list.Add(CardinalDirection.Down);
			}

			list.Add(CardinalDirection.Unknown);
			return list;
		}

		public static string Describe(this CardinalDirection direction)
		{
			return Constants.DirectionStrings[(int)direction];
		}

		public static int Northness(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.North:
				case CardinalDirection.NorthWest:
				case CardinalDirection.NorthEast:
					return 1;
				default:
					return 0;
			}
		}

		public static int Southness(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.South:
				case CardinalDirection.SouthWest:
				case CardinalDirection.SouthEast:
					return 1;
				default:
					return 0;
			}
		}

		public static int Eastness(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.East:
				case CardinalDirection.SouthEast:
				case CardinalDirection.NorthEast:
					return 1;
				default:
					return 0;
			}
		}

		public static int Westness(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.West:
				case CardinalDirection.NorthWest:
				case CardinalDirection.SouthWest:
					return 1;
				default:
					return 0;
			}
		}

		public static int Upness(this CardinalDirection direction)
		{
			return direction == CardinalDirection.Up ? 1 : 0;
		}

		public static int Downness(this CardinalDirection direction)
		{
			return direction == CardinalDirection.Down ? 1 : 0;
		}

		public static string DescribeDirections(int northness, int southness, int westness, int eastness, int upness,
			int downness)
		{
			if (northness > 0)
			{
				if (eastness > 0)
				{
					if (northness == eastness)
					{
						return "Northeast";
					}
					return northness > eastness ? "North-Northeast" : "East-Northeast";
				}
				if (westness == 0)
				{
					return "North";
				}
				if (northness == westness)
				{
					return "Northwest";
				}
				return northness > westness ? "North-Northwest" : "West-Northwest";
			}

			if (southness > 0)
			{
				if (eastness > 0)
				{
					if (southness == eastness)
					{
						return "Southeast";
					}
					return southness > eastness ? "South-Southeast" : "East-Southeast";
				}
				if (westness == 0)
				{
					return "South";
				}
				if (southness == westness)
				{
					return "Southwest";
				}
				return southness > westness ? "South-Southwest" : "West-Southwest";
			}

			if (westness > 0)
			{
				return "West";
			}

			if (eastness > 0)
			{
				return "East";
			}

			if (upness > 0)
			{
				return "Up";
			}

			return downness > 0 ? "Down" : "Unknown";
		}



		public static (int Northness, int Southness, int Westness, int Eastness, int Upness, int Downness, int Unknownness)
			CountDirections(this IEnumerable<CardinalDirection> directions)
		{
			int northness = 0, southness = 0, westness = 0, eastness = 0, upness = 0, downness = 0, unknownness = 0;
			foreach (var direction in directions)
			{
				switch (direction)
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
			return (northness, southness, westness, eastness, upness, downness, unknownness);
		}

		public static string DescribeExitDirection<T>(this T directions) where T : IEnumerable<ICellExit>
		{
			if (!directions.Any())
			{
				return "Nowhere";
			}

			if (directions.First() is INonCardinalCellExit nce)
			{
				return nce.OutboundTarget;
			}

			return $"the {directions.Select(x => x.OutboundDirection).DescribeDirection()}";
		}

		public static string DescribeDirection<T>(this T directions) where T : IEnumerable<CardinalDirection>
		{
			var counts = directions.CountDirections();
			return DescribeDirections(counts.Northness, counts.Southness, counts.Westness, counts.Eastness,
									  counts.Upness, counts.Downness);
		}

		public static string DescribeOppositeDirection<T>(this T directions) where T : IEnumerable<CardinalDirection>
		{
			var counts = directions.CountDirections();
			return DescribeDirections(counts.Southness, counts.Northness, counts.Eastness, counts.Westness,
									  counts.Downness, counts.Upness);
		}

		public static int DistanceAsCrowFlies<T>(this T directions) where T : IEnumerable<CardinalDirection>
		{
			var counts = directions.CountDirections();
			return (int)Math.Round(Math.Sqrt(Math.Pow(Math.Sqrt(Math.Pow(counts.Northness - counts.Southness, 2) +
																 Math.Pow(counts.Eastness - counts.Westness, 2)), 2) +
											  Math.Pow(counts.Upness - counts.Downness, 2)), 0) + counts.Unknownness;
		}

		public static int PythagoreanDistance<T>(this T exits, RoundingMode rounding = RoundingMode.Truncate) where T : IEnumerable<ICellExit>
		{
			var directions = exits.Select(x => x.OutboundDirection);
			var counts = directions.CountDirections();
			var distance = Math.Sqrt(Math.Pow(Math.Sqrt(Math.Pow(counts.Northness - counts.Southness, 2) +
														Math.Pow(counts.Eastness - counts.Westness, 2)), 2) +
									 Math.Pow(counts.Upness - counts.Downness, 2)) + counts.Unknownness;
			switch (rounding)
			{
				case RoundingMode.NoRounding:
				case RoundingMode.Truncate:
					return (int)Math.Floor(distance);
				case RoundingMode.Round:
				default:
					return (int)Math.Round(distance, 0);
			}
		}

		public static int MaximumAxialDistance<T>(this T exits) where T : IEnumerable<ICellExit>
		{
			var directions = exits.Select(x => x.OutboundDirection);
			var counts = directions.CountDirections();
			var yaxis = Math.Abs(counts.Northness - counts.Southness);
			var xaxis = Math.Abs(counts.Eastness - counts.Westness);
			var zaxis = Math.Abs(counts.Upness - counts.Downness);
			var values = new List<int> { xaxis, yaxis, zaxis };
			return values.Max() + counts.Unknownness;
		}

		public static string DescribeBrief(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.Down:
					return "d";
				case CardinalDirection.East:
					return "e";
				case CardinalDirection.North:
					return "n";
				case CardinalDirection.NorthEast:
					return "ne";
				case CardinalDirection.NorthWest:
					return "se";
				case CardinalDirection.South:
					return "s";
				case CardinalDirection.SouthEast:
					return "se";
				case CardinalDirection.SouthWest:
					return "sw";
				case CardinalDirection.Unknown:
					return "??";
				case CardinalDirection.Up:
					return "u";
				case CardinalDirection.West:
					return "w";
				default:
					throw new NotSupportedException();
			}
		}

		public static bool IsOpposingDirection(this CardinalDirection direction, CardinalDirection otherDirection)
		{
			switch (direction)
			{
				case CardinalDirection.Unknown:
					return true;
				case CardinalDirection.Up:
					return otherDirection != CardinalDirection.Up;
				case CardinalDirection.Down:
					return otherDirection != CardinalDirection.Down;
				case CardinalDirection.West:
					return (otherDirection == CardinalDirection.East) || (otherDirection == CardinalDirection.SouthEast) ||
						   (otherDirection == CardinalDirection.NorthEast);
				case CardinalDirection.East:
					return (otherDirection == CardinalDirection.West) || (otherDirection == CardinalDirection.NorthWest) ||
						   (otherDirection == CardinalDirection.SouthWest);
				case CardinalDirection.South:
					return (otherDirection == CardinalDirection.North) ||
						   (otherDirection == CardinalDirection.NorthWest) ||
						   (otherDirection == CardinalDirection.NorthEast);
				case CardinalDirection.North:
					return (otherDirection == CardinalDirection.South) ||
						   (otherDirection == CardinalDirection.SouthEast) ||
						   (otherDirection == CardinalDirection.SouthWest);
				case CardinalDirection.NorthWest:
					return (otherDirection == CardinalDirection.SouthEast) ||
						   (otherDirection == CardinalDirection.South) || (otherDirection == CardinalDirection.East);
				case CardinalDirection.NorthEast:
					return (otherDirection == CardinalDirection.SouthWest) ||
						   (otherDirection == CardinalDirection.South) || (otherDirection == CardinalDirection.West);
				case CardinalDirection.SouthEast:
					return (otherDirection == CardinalDirection.NorthWest) ||
						   (otherDirection == CardinalDirection.North) || (otherDirection == CardinalDirection.West);
				case CardinalDirection.SouthWest:
					return (otherDirection == CardinalDirection.NorthEast) ||
						   (otherDirection == CardinalDirection.North) || (otherDirection == CardinalDirection.East);
				default:
					throw new NotImplementedException();
			}
		}

		public static int ExitCommandPriority(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.North:
				case CardinalDirection.Up:
				case CardinalDirection.Down:
				case CardinalDirection.East:
				case CardinalDirection.South:
				case CardinalDirection.West:
					return 1;
				case CardinalDirection.NorthEast:
				case CardinalDirection.SouthEast:
				case CardinalDirection.SouthWest:
				case CardinalDirection.NorthWest:
					return 2;

				case CardinalDirection.Unknown:
					return 3;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public static CardinalDirection Opposite(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.Down:
					return CardinalDirection.Up;
				case CardinalDirection.East:
					return CardinalDirection.West;
				case CardinalDirection.North:
					return CardinalDirection.South;
				case CardinalDirection.NorthEast:
					return CardinalDirection.SouthWest;
				case CardinalDirection.NorthWest:
					return CardinalDirection.SouthEast;
				case CardinalDirection.South:
					return CardinalDirection.North;
				case CardinalDirection.SouthEast:
					return CardinalDirection.NorthWest;
				case CardinalDirection.SouthWest:
					return CardinalDirection.NorthEast;
				case CardinalDirection.Unknown:
					return CardinalDirection.Unknown;
				case CardinalDirection.Up:
					return CardinalDirection.Down;
				case CardinalDirection.West:
					return CardinalDirection.East;
				default:
					throw new NotSupportedException();
			}
		}

		public static CardinalDirection Rotate90Clockwise(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.East:
					return CardinalDirection.South;
				case CardinalDirection.North:
					return CardinalDirection.East;
				case CardinalDirection.NorthEast:
					return CardinalDirection.SouthEast;
				case CardinalDirection.NorthWest:
					return CardinalDirection.NorthEast;
				case CardinalDirection.South:
					return CardinalDirection.West;
				case CardinalDirection.SouthEast:
					return CardinalDirection.SouthWest;
				case CardinalDirection.SouthWest:
					return CardinalDirection.NorthWest;
				case CardinalDirection.West:
					return CardinalDirection.North;
				default:
					throw new NotSupportedException();
			}
		}

		public static CardinalDirection Rotate90CounterClockwise(this CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.East:
					return CardinalDirection.North;
				case CardinalDirection.North:
					return CardinalDirection.West;
				case CardinalDirection.NorthEast:
					return CardinalDirection.NorthWest;
				case CardinalDirection.NorthWest:
					return CardinalDirection.SouthWest;
				case CardinalDirection.South:
					return CardinalDirection.East;
				case CardinalDirection.SouthEast:
					return CardinalDirection.NorthEast;
				case CardinalDirection.SouthWest:
					return CardinalDirection.SouthEast;
				case CardinalDirection.West:
					return CardinalDirection.South;
				default:
					throw new NotSupportedException();
			}
		}
	}
}

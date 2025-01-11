using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests
{
	[TestClass]
	public class CircularRangeTests
	{
		/// <summary>
		/// Basic test to ensure that construction with a 24-hour schedule
		/// and three activities behaves as expected.
		/// </summary>
		[TestMethod]
		public void Constructor_InitializesRangesCorrectly()
		{
			// Arrange
			var items = new[]
			{
				("Sleep", 0.0),  // from hour 0
				("Work", 8.0),   // from hour 8
				("Play", 18.0)   // from hour 18
			};

			// Act
			var circularRange = new CircularRange<string>(24, items);

			// Assert
			// We should end up with 4 or more BoundRange objects as per the internal logic:
			// Sleep => [0..8), Work => [8..18), Play => [18..24) + an extra for bridging 0
			// (the user code adds a wrap-around range for the last item).
			Assert.IsTrue(circularRange.Ranges.Count() >= 3,
				"Expected at least three ranges, one for each item, plus wrap-around range(s).");

			// Basic sanity checks:
			Assert.AreEqual(0, circularRange.Floor, "Floor should be 0 for a 24-hour range.");
			Assert.AreEqual(24, circularRange.Ceiling, "Ceiling should be 24 for a 24-hour range.");
			Assert.AreEqual(24, circularRange.Circumference, "Circumference should be 24 for a 24-hour range.");
		}

		/// <summary>
		/// Checks that calling Get() with various hour inputs yields the correct activity.
		/// </summary>
		[TestMethod]
		public void Get_ReturnsCorrectValuesWithinRange()
		{
			// Arrange
			var items = new[]
			{
				("Sleep", 0.0),
				("Work", 8.0),
				("Play", 18.0)
			};

			var circularRange = new CircularRange<string>(24, items);

			// Act & Assert
			// Hours 0..7 should map to "Sleep"
			for (int hour = 0; hour < 8; hour++)
			{
				Assert.AreEqual("Sleep", circularRange.Get(hour), $"Hour {hour} should return 'Sleep'");
			}

			// Hours 8..17 should map to "Work"
			for (int hour = 8; hour < 18; hour++)
			{
				Assert.AreEqual("Work", circularRange.Get(hour), $"Hour {hour} should return 'Work'");
			}

			// Hours 18..23 should map to "Play"
			for (int hour = 18; hour < 24; hour++)
			{
				Assert.AreEqual("Play", circularRange.Get(hour), $"Hour {hour} should return 'Play'");
			}
		}

		/// <summary>
		/// Ensures that values outside the nominal 0..24 range wrap around correctly.
		/// For example, hour 25 should act like hour 1, hour -1 should act like hour 23.
		/// </summary>
		[TestMethod]
		public void Get_ReturnsCorrectValuesOutsideRange()
		{
			// Arrange
			var items = new[]
			{
				("Sleep", 0.0),
				("Work", 8.0),
				("Play", 18.0)
			};

			var circularRange = new CircularRange<string>(24, items);

			// Act & Assert
			// Hour 25 => effectively hour 1 => "Sleep"
			Assert.AreEqual("Sleep", circularRange.Get(25), "Hour 25 should normalize to hour 1 => Sleep.");

			// Hour -1 => effectively hour 23 => "Play"
			Assert.AreEqual("Play", circularRange.Get(-1), "Hour -1 should normalize to hour 23 => Play.");

			// Hour 24 => effectively hour 0 => "Sleep"
			Assert.AreEqual("Sleep", circularRange.Get(24), "Hour 24 should normalize to hour 0 => Sleep.");
		}

		/// <summary>
		/// Verifies that RangeFraction() calculates the correct fractional offset
		/// within the sub-range for a given hour.
		/// </summary>
		[TestMethod]
		public void RangeFraction_ReturnsCorrectFraction()
		{
			// Arrange
			var items = new[]
			{
				("Sleep", 0.0),  // [0..8)
				("Work", 8.0),   // [8..18)
				("Play", 18.0)   // [18..24)
			};

			var circularRange = new CircularRange<string>(24, items);

			// Act
			// For hour 4 => within "Sleep" range [0..8). The fraction is (4 - 0) / (8 - 0) = 0.5
			var fractionAtHour4 = circularRange.RangeFraction(4);

			// For hour 10 => within "Work" range [8..18). The fraction is (10 - 8) / (18 - 8) = 2 / 10 = 0.2
			var fractionAtHour10 = circularRange.RangeFraction(10);

			// For hour 21 => within "Play" range [18..24). The fraction is (21 - 18) / (24 - 18) = 3 / 6 = 0.5
			var fractionAtHour21 = circularRange.RangeFraction(21);

			// Assert
			Assert.AreEqual(0.5, fractionAtHour4, 1e-6, "Expected fraction 0.5 for hour 4 in [0..8).");
			Assert.AreEqual(0.2, fractionAtHour10, 1e-6, "Expected fraction 0.2 for hour 10 in [8..18).");
			Assert.AreEqual(0.5, fractionAtHour21, 1e-6, "Expected fraction 0.5 for hour 21 in [18..24).");
		}

		/// <summary>
		/// Tests adding extra ranges manually and verifying the sorting logic.
		/// </summary>
		[TestMethod]
		public void AddAndSort_UpdatesRangesCorrectly()
		{
			// Arrange
			var circularRange = new CircularRange<string>();

			// Manually add ranges: 
			// 1. [0..5) => "Low"
			// 2. [10..15) => "Mid"
			// 3. [5..10) => "Medium-Low"
			// 4. [15..24) => "High"
			circularRange.Add(new BoundRange<string>(circularRange, "Low", 0, 5));
			circularRange.Add(new BoundRange<string>(circularRange, "Mid", 10, 15));
			circularRange.Add(new BoundRange<string>(circularRange, "Medium-Low", 5, 10));
			circularRange.Add(new BoundRange<string>(circularRange, "High", 15, 24));

			// Act
			circularRange.Sort();

			// Assert
			// After Sort(), the ranges should be in ascending order of their LowerLimit: 
			// (0..5 => Low), (5..10 => Medium-Low), (10..15 => Mid), (15..24 => High)
			var sorted = circularRange.Ranges.ToList();
			Assert.AreEqual("Low", sorted[0].Value, "First range after sort should be 'Low'.");
			Assert.AreEqual("Medium-Low", sorted[1].Value, "Second range after sort should be 'Medium-Low'.");
			Assert.AreEqual("Mid", sorted[2].Value, "Third range after sort should be 'Mid'.");
			Assert.AreEqual("High", sorted[3].Value, "Fourth range after sort should be 'High'.");

			// Also verify that the CircularRange's Floor, Ceiling, Circumference
			// are updated according to the new sorted set.
			Assert.AreEqual(0, circularRange.Floor, "Floor should be 0 after sorting the new set.");
			Assert.AreEqual(24, circularRange.Ceiling, "Ceiling should be 24 after sorting the new set.");
			Assert.AreEqual(24, circularRange.Circumference, "Circumference should be 24 after sorting the new set.");
		}

		/// <summary>
		/// Checks behavior when encountering NaN or extremely large input values.
		/// </summary>
		[TestMethod]
		public void Get_HandlesNaNAndLargeValuesGracefully()
		{
			// Arrange
			var items = new[]
			{
				("A", 0.0),
				("B", 5.0),
				("C", 10.0),
				("D", 15.0),
				("E", 20.0)
			};
			var circularRange = new CircularRange<string>(25, items); // 0..25

			// Act
			var nanResult = circularRange.Get(double.NaN);
			var largePositiveResult = circularRange.Get(2500.0);  // multiple wraps
			var largeNegativeResult = circularRange.Get(-2500.0); // multiple wraps

			// Assert
			// NaN defaults to 0 => "A"
			Assert.AreEqual("A", nanResult, "NaN should normalize to 0 => 'A'.");

			// 2500 % 25 = 0 => "A"
			Assert.AreEqual("A", largePositiveResult, "Large positive number should wrap around to 0 => 'A'.");

			// -2500 % 25 = 0 => "A" (with the sign adjusted by the Normalise() logic)
			Assert.AreEqual("A", largeNegativeResult, "Large negative number should wrap around to 0 => 'A'.");
		}
	}
}

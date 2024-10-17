using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SystemDateTimeTests
{
	/// <summary>
	/// Tests cultures that use Month/Day ordering based on MonthDayPattern, YearMonthPattern, or ShortDatePattern.
	/// </summary>
	[TestMethod]
	public void GetDateOrder_MonthFirst_Cultures_ReturnsMonthFirst()
	{
		// Arrange
		var cultures = new[]
		{
				new CultureInfo("en-US"),
				new CultureInfo("en-CA"), // Canadian English typically follows MonthFirst
				// Add more MonthFirst cultures as needed
			};

		// Act & Assert
		foreach (var culture in cultures)
		{
			if (IsCultureExpectedToBeMonthFirst(culture.Name))
			{
				var result = DateUtilities.GetDateOrder(culture);
				Assert.AreEqual(DateUtilities.DateOrder.MonthFirst, result, $"Culture {culture.Name} should be MonthFirst.");
			}
		}
	}

	/// <summary>
	/// Tests cultures that use Day/Month ordering based on MonthDayPattern, YearMonthPattern, or ShortDatePattern.
	/// </summary>
	[TestMethod]
	public void GetDateOrder_DayFirst_Cultures_ReturnsDayFirst()
	{
		// Arrange
		var cultures = new[]
		{
				new CultureInfo("en-GB"),
				new CultureInfo("fr-FR"),
				new CultureInfo("de-DE"),
				new CultureInfo("es-ES"),
				new CultureInfo("ru-RU"),
				new CultureInfo("ar-SA"),
				new CultureInfo("it-IT"),
				new CultureInfo("nl-NL"),
				new CultureInfo("sv-SE"),
				new CultureInfo("fi-FI"),
				new CultureInfo("no-NO"),
				new CultureInfo("da-DK"),
				new CultureInfo("pl-PL"),
				new CultureInfo("tr-TR")
				// Add more DayFirst cultures as needed
			};

		// Act & Assert
		foreach (var culture in cultures)
		{
			var result = DateUtilities.GetDateOrder(culture);
			Assert.AreEqual(DateUtilities.DateOrder.DayFirst, result, $"Culture {culture.Name} should be DayFirst.");
		}
	}

	/// <summary>
	/// Tests cultures that primarily use Year/Month/Day ordering but have Month/Day ordering in MonthDayPattern.
	/// </summary>
	[TestMethod]
	public void GetDateOrder_YearFirst_Cultures_ReturnsMonthFirstOrDayFirst()
	{
		// Arrange
		var cultures = new[]
		{
				new CultureInfo("ja-JP"),
				new CultureInfo("zh-CN"),
				new CultureInfo("ko-KR"),
				new CultureInfo("zh-TW")
				// Add more YearFirst cultures as needed
			};

		// Act & Assert
		foreach (var culture in cultures)
		{
			var result = DateUtilities.GetDateOrder(culture);
			Assert.AreEqual(DateUtilities.DateOrder.YearFirst, result, $"Culture {culture.Name} should be YearFirst.");
		}
	}

	/// <summary>
	/// Tests cultures with ambiguous or uncommon date patterns.
	/// </summary>
	[TestMethod]
	public void GetDateOrder_AmbiguousOrUncommonPatterns_ReturnsExpected()
	{
		// Arrange
		// Invariant culture uses "MM/dd/yyyy" and MonthDayPattern "MMMM dd"
		var invariantCulture = CultureInfo.InvariantCulture;
		var resultInvariant = DateUtilities.GetDateOrder(invariantCulture);
		Assert.AreEqual(DateUtilities.DateOrder.YearFirst, resultInvariant, "InvariantCulture should be YearFirst based on MonthDayPattern.");

		// Custom culture with unique pattern
		var customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
		customCulture.DateTimeFormat.ShortDatePattern = "yyyyMMdd"; // YearFirst without separators
		customCulture.DateTimeFormat.MonthDayPattern = "MMdd"; // MonthFirst without separators
		customCulture.DateTimeFormat.YearMonthPattern = "yyMM";
		var resultCustom = DateUtilities.GetDateOrder(customCulture);
		Assert.AreEqual(DateUtilities.DateOrder.YearFirst, resultCustom, "CustomCulture with 'MMdd' in MonthDayPattern should be YearFirst.");

		// Culture with only month and day in MonthDayPattern
		var monthDayOnlyCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
		monthDayOnlyCulture.DateTimeFormat.MonthDayPattern = "MM/dd";
		monthDayOnlyCulture.DateTimeFormat.YearMonthPattern = "MM/yy";
		var resultMonthDayOnly = DateUtilities.GetDateOrder(monthDayOnlyCulture);
		Assert.AreEqual(DateUtilities.DateOrder.MonthFirst, resultMonthDayOnly, "CustomCulture with 'MM/dd' in MonthDayPattern should be MonthFirst.");

		// Culture with only day and month in MonthDayPattern
		var dayMonthOnlyCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
		dayMonthOnlyCulture.DateTimeFormat.MonthDayPattern = "dd/MM";
		dayMonthOnlyCulture.DateTimeFormat.YearMonthPattern = "MM/yy";
		var resultDayMonthOnly = DateUtilities.GetDateOrder(dayMonthOnlyCulture);
		Assert.AreEqual(DateUtilities.DateOrder.DayFirst, resultDayMonthOnly, "CustomCulture with 'dd/MM' in MonthDayPattern should be DayFirst.");
	}

	/// <summary>
	/// Tests cultures with invalid date patterns.
	/// </summary>
	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void GetDateOrder_InvalidPattern_ThrowsException()
	{
		// Arrange
		var invalidCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
		invalidCulture.DateTimeFormat.ShortDatePattern = "HH:mm:ss"; // Time pattern without date components
		invalidCulture.DateTimeFormat.MonthDayPattern = "HH:mm"; // Time pattern without date components
		invalidCulture.DateTimeFormat.YearMonthPattern = "HH:mm"; // Time pattern without date components

		// Act
		DateUtilities.GetDateOrder(invalidCulture);

		// Assert is handled by ExpectedException
	}

	/// <summary>
	/// Tests cultures with mixed specifiers and literals.
	/// </summary>
	[TestMethod]
	public void GetDateOrder_MixedSpecifiersAndLiterals_ReturnsExpected()
	{
		// Arrange
		var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
		culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
		culture.DateTimeFormat.MonthDayPattern = "MM-dd"; // MonthFirst
		culture.DateTimeFormat.YearMonthPattern = "MM-yyyy";
		var result = DateUtilities.GetDateOrder(culture);

		// Act & Assert
		Assert.AreEqual(DateUtilities.DateOrder.MonthFirst, result, "CustomCulture with 'MM-dd' in MonthDayPattern should be MonthFirst.");
	}

	/// <summary>
	/// Helper method to determine if a culture is expected to be MonthFirst.
	/// Adjust this method based on known MonthFirst cultures.
	/// </summary>
	/// <param name="cultureName">Culture name string.</param>
	/// <returns>True if MonthFirst, else False.</returns>
	private bool IsCultureExpectedToBeMonthFirst(string cultureName)
	{
		// List of cultures that use MonthFirst ordering based on MonthDayPattern
		var monthFirstCultures = new[]
		{
				"en-US",
				"en-CA"
				// Add more if necessary
			};

		return Array.Exists(monthFirstCultures, c => c.Equals(cultureName, StringComparison.OrdinalIgnoreCase));
	}
}

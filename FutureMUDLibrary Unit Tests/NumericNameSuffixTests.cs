#nullable enable
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NumericNameSuffixTests
{
	[DataTestMethod]
	[DataRow("item", "item1")]
	[DataRow("item5", "item6")]
	[DataRow("item009", "item10")]
	[DataRow("item000", "item1")]
	[DataRow("0", "1")]
	[DataRow("999", "1000")]
	[DataRow("item2147483646", "item2147483647")]
	[DataRow("item2147483647", "item2147483648")]
	[DataRow("item2147483648", "item2147483649")]
	[DataRow("item999999999999999999999999999999", "item1000000000000000000000000000000")]
	[DataRow("999999999999999999999999999999", "1000000000000000000000000000000")]
	[DataRow("item12 tail", "item12 tail1")]
	public void IncrementNumberOrAddNumber_PreservesPrefixAndIncrementsDecimalSuffix(string name, string expected)
	{
		Assert.AreEqual(expected, name.IncrementNumberOrAddNumber());
	}

	[TestMethod]
	public void IncrementNumberOrAddNumber_ThousandsOfDigits_DoesNotOverflow()
	{
		Assert.AreEqual("item1" + new string('0', 4096),
			("item" + new string('9', 4096)).IncrementNumberOrAddNumber());
		Assert.AreEqual("item2", ("item" + new string('0', 4096) + "1").IncrementNumberOrAddNumber());
	}

	[TestMethod]
	public void IncrementNumberOrAddNumber_NonEnglishCulture_UsesUngroupedAsciiDigits()
	{
		var previous = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-SA");
			Assert.AreEqual("item2147483648", "item2147483647".IncrementNumberOrAddNumber());
		}
		finally
		{
			CultureInfo.CurrentCulture = previous;
		}
	}
}

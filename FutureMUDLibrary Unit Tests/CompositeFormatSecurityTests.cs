#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CompositeFormatSecurityTests
{
	[DataTestMethod]
	[DataRow("{")]
	[DataRow("}")]
	[DataRow("{0")]
	[DataRow("{notanindex}")]
	[DataRow("{0} trailing {")]
	[DataRow("{1,10}")]
	[DataRow("{{{1}}}")]
	public void OptionalPlaceholders_RejectMalformedOrOutOfRangeCompositeFormats(string text)
	{
		Assert.IsFalse(text.IsValidFormatString(new[] { false }));
	}

	[DataTestMethod]
	[DataRow("")]
	[DataRow("No replacement")]
	[DataRow("{{escaped braces}}")]
	[DataRow("{{{0}}}")]
	[DataRow("Value {0}")]
	[DataRow("{0:}")]
	public void OptionalPlaceholders_PreserveValidSyntaxAndOmittedArguments(string text)
	{
		Assert.IsTrue(text.IsValidFormatString(new[] { false }));
		_ = string.Format(text, "text");
	}
}

using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Characteristics;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacteristicPatternSecurityTests
{
	[DataTestMethod]
	[DataRow("^(hair|eyes)$", "HAIR", true)]
	[DataRow(@"^(\w+)\1$", "abcabc", true)]
	[DataRow("^eyes$", "hair", false)]
	[DataRow("^(a+)+$", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa!", false)]
	public void MatchesPattern_AuthoredSyntaxAndBacktracking_ReturnsSafely(string pattern, string input, bool expected)
	{
		var definition = Mock.Of<ICharacteristicDefinition>(x => x.Pattern == new Regex(pattern, RegexOptions.IgnoreCase));
		Assert.AreEqual(expected, definition.MatchesPattern(input));
	}

	[TestMethod]
	public void MatchesPattern_OversizedOrNullInput_DoesNotReadOrExecutePattern()
	{
		var definition = new Mock<ICharacteristicDefinition>(MockBehavior.Strict);
		Assert.IsFalse(definition.Object.MatchesPattern(new string('a', 257)));
		Assert.IsFalse(definition.Object.MatchesPattern(null));
		definition.VerifyGet(x => x.Pattern, Times.Never);
	}
}

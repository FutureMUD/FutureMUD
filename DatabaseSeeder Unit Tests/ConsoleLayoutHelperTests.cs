#nullable enable

using System.Linq;
using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ConsoleLayoutHelperTests
{
	[TestMethod]
	public void FormatMenuEntry_WrapsLongTaglinesWithoutOverflow()
	{
		var lines = ConsoleLayoutHelper
			.FormatMenuEntry(
				8,
				"Celestial Seeder",
				"Current",
				"This is a deliberately long tagline that should wrap onto more than one line without letting the console host decide where the cursor ends up.",
				60)
			.ToList();

		Assert.IsTrue(lines.Count > 1);
		Assert.IsTrue(lines.All(x => x.Length <= 60), "Expected all menu lines to fit within the requested width.");

		var prefixLength = "8) [Celestial Seeder    ] [Current ] ".Length;
		Assert.IsTrue(lines[1].StartsWith(new string(' ', prefixLength)));
	}

	[TestMethod]
	public void ResolveQuestionAnswer_UsesDefaultForBlankInput()
	{
		Assert.AreEqual("01/january/2000", Program.ResolveQuestionAnswer("", "01/january/2000"));
		Assert.AreEqual("typed", Program.ResolveQuestionAnswer("typed", "01/january/2000"));
		Assert.AreEqual("", Program.ResolveQuestionAnswer("", null));
	}
}

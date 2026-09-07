#nullable enable

using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DatabaseSeeder.Seeders;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ConsoleLayoutHelperTests
{
	[TestMethod]
	public void FormatPrompt_CalendarUsesWidthAndPreservesEveryOption()
	{
		var prompt = new TimeSeeder().SeederQuestions.Single(x => x.Id == "mode").Question;
		var narrow = ConsoleLayoutHelper.FormatPrompt(prompt, 80).ToList();
		var wide = ConsoleLayoutHelper.FormatPrompt(prompt, 160).ToList();
		Assert.IsTrue(wide.Count < narrow.Count);
		Assert.IsTrue(wide.Count <= 20, $"Calendar took {wide.Count} lines.");
		foreach (var width in new[] { 30, 80, 120, 160 })
		{
			var lines = ConsoleLayoutHelper.FormatPrompt(prompt, width).ToList();
			Assert.IsTrue(lines.All(x => Regex.Replace(x, @"#[a-fA-F0-9]", "").Length < width));
			CollectionAssert.AreEquivalent(
				Regex.Matches(prompt, @"#B([^#]+)#F:").Select(x => x.Value).ToList(),
				Regex.Matches(string.Join("\n", lines), @"#B([^#]+)#F:").Select(x => x.Value).ToList());
		}
	}

	[TestMethod]
	public void FormatPrompt_WrapsMarkupAndUnbrokenTextWithoutLosingContent()
	{
		var text = "#B" + new string('x', 100) + "#F";
		var lines = ConsoleLayoutHelper.FormatPrompt(text, 30).ToList();
		Assert.AreEqual(text, string.Concat(lines));
		Assert.IsTrue(lines.All(x => Regex.Replace(x, @"#[a-fA-F0-9]", "").Length < 30));
	}

    [TestMethod]
    public void FormatMenuEntry_WrapsLongTaglinesWithoutOverflow()
    {
        List<string> lines = ConsoleLayoutHelper
            .FormatMenuEntry(
                8,
                "Celestial Seeder",
                "Current",
                "This is a deliberately long tagline that should wrap onto more than one line without letting the console host decide where the cursor ends up.",
                60)
            .ToList();

        Assert.IsTrue(lines.Count > 1);
        Assert.IsTrue(lines.All(x => x.Length <= 60), "Expected all menu lines to fit within the requested width.");

        int prefixLength = "8) [Celestial Seeder    ] [Current ] ".Length;
        Assert.IsTrue(lines[1].StartsWith(new string(' ', prefixLength)));
    }

    [TestMethod]
    public void ResolveQuestionAnswer_UsesDefaultForBlankInput()
    {
        Assert.AreEqual("01/january/2000", Program.ResolveQuestionAnswer("", "01/january/2000"));
        Assert.AreEqual("typed", Program.ResolveQuestionAnswer("typed", "01/january/2000"));
        Assert.AreEqual("", Program.ResolveQuestionAnswer("", null));
    }

    [TestMethod]
    public void ShowSeederInMainMenu_HidesBlockedPackagesUnlessRequested()
    {
        Assert.IsFalse(Program.ShowSeederInMainMenu(SeederAssessmentStatus.Blocked, false));
        Assert.IsTrue(Program.ShowSeederInMainMenu(SeederAssessmentStatus.Blocked, true));
        Assert.IsTrue(Program.ShowSeederInMainMenu(SeederAssessmentStatus.ReadyToInstall, false));
    }

    [TestMethod]
    public void GetBlockedSeederSummary_ExplainsInvalidCommand()
    {
        Assert.AreEqual(
            "There are 13 packages reporting prerequisites not met. Type INVALID to see a list of them.",
            Program.GetBlockedSeederSummary(13, false));
        Assert.AreEqual(
            "There is 1 package reporting prerequisites not met. Type INVALID to see it.",
            Program.GetBlockedSeederSummary(1, false));
        Assert.AreEqual(
            "1 blocked package is currently visible. Type VALID to hide it again.",
            Program.GetBlockedSeederSummary(1, true));
    }
}

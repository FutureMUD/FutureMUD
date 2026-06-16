#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.CharacterCreation.Screens;
using MudSharp.RPG.Knowledge;

namespace MudSharp_Unit_Tests.CharacterCreation;

[TestClass]
public class KnowledgePickerBySkillScreenTests
{
	[TestMethod]
	public void FormatExistingKnowledges_NoKnowledges_ReturnsEmptyString()
	{
		var output = KnowledgePickerBySkillScreenStoryboard.KnowledgePickerBySkillScreen
			.FormatExistingKnowledges([], 80);

		Assert.AreEqual(string.Empty, output);
	}

	[TestMethod]
	public void FormatAvailableKnowledges_LongKnowledgeDescriptions_WrapAsSeparateNumberedRows()
	{
		var surgery = Knowledge("Chiurgery",
			"Knowledge of performing surgery and major interventions on humans");
		var anatomy = Knowledge("Physical Medicine",
			"Knowledge of anatomy, diagnosis, and practical medicine for humans");

		var output = KnowledgePickerBySkillScreenStoryboard.KnowledgePickerBySkillScreen
			.FormatAvailableKnowledges([surgery, anatomy], [], 54);

		Assert.IsTrue(output.Contains("1: Knowledge Of Performing Surgery And Major"));
		Assert.IsTrue(output.Contains("\n   Interventions On Humans"));
		Assert.IsTrue(output.Contains("\n2: Knowledge Of Anatomy, Diagnosis, And"));
		Assert.IsFalse(output.Contains("Humans2:"));
	}

	private static IKnowledge Knowledge(string name, string description)
	{
		var knowledge = new Mock<IKnowledge>();
		knowledge.SetupGet(x => x.Name).Returns(name);
		knowledge.SetupGet(x => x.Description).Returns(description);
		return knowledge.Object;
	}
}

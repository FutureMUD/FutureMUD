#nullable enable

using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("post-candidate", false, false, false)]
	[DataRow("post-candidate", true, false, false)]
	[DataRow("post-candidate", true, true, false)]
	[DataRow("skill-setter", false, false, false)]
	[DataRow("skill-setter", true, false, false)]
	[DataRow("skill-setter", true, true, false)]
	[DataRow("trait-setter", false, false, false)]
	[DataRow("trait-setter", true, false, false)]
	[DataRow("trait-setter", true, true, false)]
	[DataRow("post-candidate", true, false, true)]
	[DataRow("trait-setter", true, false, true)]
	[DataRow("none", false, false, false)]
	[DataRow("none", false, false, true)]
	public void NativeLearning_ValidPolicySeparateWriteRefusesStaleGainWithoutChangingEnclosingCommit(
		string gate, bool independent, bool restoreOriginal, bool enclosingCommitted)
	{
		var f = LearningFixture(); var improver = new Mock<IImprovementModel>();
		var definition = LearningDefinition(f, improver.Object); var skill = new Skill(definition, 40, f.Owner.Object);
		f.Actor.Setup(x => x.GetTrait(definition)).Returns(skill); f.Actor.Setup(x => x.HasTrait(definition)).Returns(true);
		improver.Setup(x => x.GetImprovement(It.IsAny<IHaveTraits>(), skill, It.IsAny<Difficulty>(), It.IsAny<Outcome>(), It.IsAny<TraitUseType>())).Returns(5);
		var candidateRead = false;
		var cap = new Mock<ITraitExpression>();
		cap.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>()))
			.Callback(() => candidateRead = true).Returns(100);
		definition.Cap = cap.Object;
		var writes = 0;
		bool Permission()
		{
			if (!candidateRead || writes != 0 || gate == "none") return true;
			var frames = new StackTrace().GetFrames();
			var skillSetter = frames.Any(x => x.GetMethod() is { Name: "set_Value", DeclaringType: var type } && type == typeof(Skill));
			var traitSetter = frames.Any(x => x.GetMethod() is { Name: "set_Value", DeclaringType: var type } && type == typeof(Trait));
			if (gate == "post-candidate" && skillSetter || gate == "skill-setter" && (!skillSetter || traitSetter) || gate == "trait-setter" && !traitSetter) return true;
			++writes;
			using var separate = independent ? CommandExecutionScope.EnterIndependent() : null;
			skill.Value = 60;
			if (restoreOriginal) skill.Value = 40;
			return true; // The original order remains authorized throughout this callback.
		}
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get goods", Permission), f.Actor.Object);
		if (enclosingCommitted) CommandExecutionScope.MarkCommitted(f.Actor.Object);
		var result = LearningCheck(f, new TraitExpression("variable", f.World.Object)).Resolve(f.Actor.Object, definition);
		var unchanged = gate == "none";
		Assert.AreEqual(unchanged ? 0 : 1, writes);
		Assert.AreEqual(unchanged ? 45.0 : restoreOriginal ? 40.0 : 60.0, skill.RawValue);
		Assert.AreEqual(unchanged ? 1 : 0, result.ImprovedTraits.Count(), "Never report a gain whose stale assignment was refused.");
		Assert.AreEqual(Outcome.Pass, result.Outcome, "Learning refusal preserves the rolled result.");
		improver.Verify(x => x.GetImprovement(It.IsAny<IHaveTraits>(), skill, It.IsAny<Difficulty>(), It.IsAny<Outcome>(), It.IsAny<TraitUseType>()), Times.Once);
		Assert.IsTrue(CommandExecutionScope.TryContinue(f.Actor.Object), "A stale learning candidate does not revoke the valid enclosing order.");
		f.Grant = null;
		Assert.IsFalse(CommandExecutionScope.TryContinue(f.Actor.Object));
		Assert.AreEqual(!enclosingCommitted, CommandExecutionScope.RejectedBeforeCommit, "Learning must retain the enclosing operation's prior commit state.");
	}
}

#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MindImpairmentCheckTests
{
	[TestMethod]
	public void CheckEvaluation_ImpairedSkillChecksLoseTwentyFiveButAttributeChecksRetainTheirValue()
	{
		var actor = new Mock<ICharacter>();
		var effect = new Mock<IMindImpairmentEffect>();
		effect.Setup(x => x.Applies()).Returns(true);
		effect.SetupGet(x => x.SkillUseModifier).Returns(-25);
		actor.Setup(x => x.CombinedEffectsOfType<IMindImpairmentEffect>()).Returns([effect.Object]);
		var expression = new Mock<ITraitExpression>();
		expression.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>());
		expression.Setup(x => x.EvaluateWith(actor.Object, It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>(), It.IsAny<(string, object)[]>())).Returns(80);
		var skill = Mock.Of<ITraitDefinition>(x => x.TraitType == TraitType.Skill);
		var attribute = Mock.Of<ITraitDefinition>(x => x.TraitType == TraitType.Attribute);
		Assert.AreEqual(55, CheckEvaluationContext.Evaluate(expression.Object, actor.Object, skill));
		var derived = Mock.Of<ITraitDefinition>(x => x.TraitType == TraitType.DerivedSkill);
		Assert.AreEqual(55, CheckEvaluationContext.Evaluate(expression.Object, actor.Object, derived));
		Assert.AreEqual(80, CheckEvaluationContext.Evaluate(expression.Object, actor.Object, attribute));
		Assert.AreEqual(80, CheckEvaluationContext.Evaluate(expression.Object, actor.Object, null));
		expression.Object.Parameters.Add("fixedSkill", new TraitExpressionParameter { Trait = skill });
		Assert.AreEqual(55, CheckEvaluationContext.Evaluate(expression.Object, actor.Object, null));
		expression.Object.Parameters["fixedSkill"].Trait = derived;
		Assert.AreEqual(55, CheckEvaluationContext.Evaluate(expression.Object, actor.Object, null));
	}
}

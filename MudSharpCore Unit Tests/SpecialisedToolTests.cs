#nullable enable
using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpecialisedToolTests
{
	[TestMethod]
	public void FlatToolBonus_ChangesTargetNumberWithoutRoundingToDifficultySteps()
	{
		var expression = new Mock<ITraitExpression>();
		var owner = Mock.Of<IHaveTraits>();
		expression.Setup(x => x.EvaluateWith(owner, null, It.IsAny<TraitBonusContext>(), It.IsAny<(string, object)[]>())).Returns(40.0);
		var parameters = new (string, object)[] { (CheckEvaluationContext.FlatBonusParameterName, 2.5) };
		Assert.IsNull(CheckEvaluationContext.Find(parameters), "A tool bonus must not disable timebound caching.");
		Assert.AreEqual(42.5, CheckEvaluationContext.Evaluate(expression.Object, owner, null, parameters));
		Assert.AreEqual(40.0, CheckEvaluationContext.Evaluate(expression.Object, owner, null));
		Assert.AreEqual(41.0, CheckEvaluationContext.Evaluate(expression.Object, owner, null, (CheckEvaluationContext.FlatBonusParameterName, 2.5), (CheckEvaluationContext.ParameterName, new CheckEvaluationContext(41, Difficulty.Normal))));
	}

	[TestMethod]
	public void ClimbingAids_UseBestEligibleEquippedAidWithoutStacking()
	{
		IGameItem Aid(double bonus, bool held, bool worn)
		{
			var aid = new Mock<IClimbingAid>();
			aid.SetupGet(x => x.CheckBonus).Returns(bonus);
			aid.SetupGet(x => x.WorksHeld).Returns(held);
			aid.SetupGet(x => x.WorksWorn).Returns(worn);
			var item = new Mock<IGameItem>();
			item.Setup(x => x.GetItemType<IClimbingAid>()).Returns(aid.Object);
			return item.Object;
		}
		var rope = Aid(2, true, false);
		var harness = Aid(5, false, true);
		Assert.AreEqual(5.0, ClimbingAidRules.Bonus([rope], [harness]));
		Assert.AreEqual(0.0, ClimbingAidRules.Bonus([harness], [rope]));
		Assert.AreEqual(2.0, ClimbingAidRules.Bonus([rope], []));
		Assert.AreEqual(5.0, ClimbingAidRules.Bonus([harness], [harness]));
		Assert.AreEqual(0.0, ClimbingAidRules.Bonus([], []));
	}

	[TestMethod]
	public void ToolPrototypes_RoundTripBonusesAndPreserveLegacyLocksmithDefaults()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		T Load<T>(string definition) => (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic,
			null, [new MudSharp.Models.GameItemComponentProto { Id = 1, Name = "tool", Definition = definition,
				EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = 4 } }, world.Object], null)!;
		string Save(object proto) => (string)proto.GetType().GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(proto, null)!;
		var legacy = Load<LocksmithingGameItemComponentProto>("<Definition><DifficultyAdjustment>2</DifficultyAdjustment></Definition>");
		Assert.AreEqual(0.0, legacy.CheckBonus);
		Assert.AreEqual(2, legacy.DifficultyAdjustment);
		legacy.CheckBonus = 3.5;
		Assert.AreEqual(3.5, Load<LocksmithingGameItemComponentProto>(Save(legacy)).CheckBonus);
		var climb = Load<ClimbingAidGameItemComponentProto>("<Definition><CheckBonus>4</CheckBonus><WorksHeld>false</WorksHeld><WorksWorn>true</WorksWorn></Definition>");
		var reloaded = Load<ClimbingAidGameItemComponentProto>(Save(climb));
		Assert.AreEqual(4.0, reloaded.CheckBonus);
		Assert.IsFalse(reloaded.WorksHeld);
		Assert.IsTrue(reloaded.WorksWorn);
	}
}

#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Combat.AuxiliaryEffects;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PressurePointMechanicTests
{
	[TestMethod]
	public void StockLocationsResolveToSeededHumanBodyparts()
	{
		var aliases = PressurePointAnatomy.StockAliases;
		Assert.IsTrue(PressurePointAnatomy.Matches("neck", ["neck", "bneck", "throat"], aliases));
		Assert.IsTrue(PressurePointAnatomy.Matches("throat", ["neck", "bneck", "throat"], aliases));
		Assert.IsTrue(PressurePointAnatomy.Matches("spine", ["abdomen", "uback", "lback", "belly"], aliases));
	}

	[TestMethod]
	public void AnatomyAliasesRejectMissingPartsAndSupportBuilderAuthoredNames()
	{
		Assert.IsFalse(PressurePointAnatomy.Matches("throat", ["head", "abdomen"], PressurePointAnatomy.StockAliases));
		var configured = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.InvariantCultureIgnoreCase)
		{
			["spine"] = ["vertebral column", "back"]
		};
		Assert.IsTrue(PressurePointAnatomy.Matches("spine", ["back"], configured));
		Assert.IsFalse(PressurePointAnatomy.Matches("neck", ["neck"], configured));
	}

	[TestMethod]
	public void AllThreeMundaneStatusPayloadsPersistTheirStatusAndRemainingDuration()
	{
		var owner = Mock.Of<IPerceivable>();
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Id).Returns(123L);
		var statuses = new (IEffect Effect, string Type, int Seconds)[]
		{
			(new MundaneTraitPenaltyEffect(owner, trait.Object, -3.0), "MundaneTraitPenalty", 20),
			(new MundaneParalysisEffect(owner), "MundaneParalysis", 5),
			(new MundaneSilenceEffect(owner), "MundaneSilence", 20)
		};

		foreach (var (effect, expectedType, seconds) in statuses)
		{
			var saved = effect.SaveToXml(new Dictionary<IEffect, TimeSpan> { [effect] = TimeSpan.FromSeconds(seconds) });
			Assert.AreEqual(expectedType, saved.Element("Type")!.Value);
			Assert.AreEqual((seconds * 1000).ToString(), saved.Element("Remaining")!.Value);
			Assert.IsTrue(effect.SavingEffect);
		}
		var affectedTrait = new Mock<ITrait>();
		affectedTrait.SetupGet(x => x.Definition).Returns(trait.Object);
		Assert.AreEqual(-3.0, ((ITraitBonusEffect)statuses[0].Effect).GetBonus(affectedTrait.Object, TraitBonusContext.None));
		Assert.IsTrue(((IForceParalysisEffect)statuses[1].Effect).ShouldParalyse);
		Assert.IsInstanceOfType(statuses[2].Effect, typeof(ISilencedEffect));
	}
}

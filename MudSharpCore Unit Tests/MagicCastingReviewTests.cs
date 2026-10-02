using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingReviewTests
{
	[DataTestMethod]
	[DataRow(59.0)]
	[DataRow(60.0)]
	public void CappedSkill_ExactCapNativeBranch_RemainsAvailableWithoutImplicitSpellAcquisition(double initial)
	{
		var f = new MagicCastingFixture(); Permanent(f);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "branching parent").Allowed);
		f.Skills.Remove(3);
		var improver = new BranchingImprover(new MudSharp.Models.Improver { Id = 99, Name = "Branching",
			Definition = "<Definition Chance='1' Expression='5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='0'><Branches/></Definition>" }, f.World.Object);
		improver.BranchMap.Add((f.Traits[0], 60, 10, f.Traits[2]));
		f.World.SetupGet(x => x.ImprovementModels).Returns(MagicCastingFixture.Collection(() => new IImprovementModel[] { improver }));
		var definition = new SkillDefinition(new MudSharp.Models.TraitDefinition { Id = 1, Name = "Branching parent", ImproverId = 99 }, f.World.Object)
			{ Cap = new TraitExpression("100", f.World.Object) };
		var skill = new Skill(definition, initial, f.Actor.Object);
		f.Actor.Setup(x => x.TraitRawValue(f.Traits[0])).Returns(() => skill.RawValue);
		f.Actor.Setup(x => x.TraitMaxValue(It.IsAny<ITraitDefinition>())).Returns(100);
		if (initial < 60)
		{
			Assert.IsTrue(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
			Assert.AreEqual(60.0, skill.RawValue); Assert.IsFalse(f.Skills.ContainsKey(3));
		}
		Assert.IsFalse(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(60.0, skill.RawValue); Assert.AreEqual(10.0, f.Skills[3]);
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 3), "Native trait branching never grants spell knowledge.");
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CappedTheoreticalBinding_UnsupportedRawBasis_RefusesEnrolmentAndReloadWhileUncappedRemainsCompatible(bool support)
	{
		var f = new MagicCastingFixture(); Permanent(f);
		var improver = new Mock<IImprovementModel>(); improver.SetupGet(x => x.Id).Returns(99);
		improver.Setup(x => x.GetImprovement(It.IsAny<IHaveTraits>(), It.IsAny<ITrait>(), It.IsAny<Difficulty>(), It.IsAny<Outcome>(), It.IsAny<TraitUseType>())).Returns(5);
		f.World.SetupGet(x => x.ImprovementModels).Returns(MagicCastingFixture.Collection(() => new[] { improver.Object }));
		var id = support ? 3L : 1L;
		var definition = new TheoreticalSkillDefinition(new MudSharp.Models.TraitDefinition { Id = id, Name = "Theoretical", ImproverId = 99,
			ValueExpression = "practical", Type = (int)TraitType.Skill }, f.World.Object) { Cap = new TraitExpression("100", f.World.Object) };
		f.Traits[(int)id - 1] = definition;
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack(support ? "casting support add 3 30 60 on" : "casting entry skill 1 30 60 relative")));
		Assert.IsTrue(f.Earth.CastingConfigurationErrors().Any(x => x.Contains("theoretical", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "unsupported theoretical cap").Allowed);
		Assert.IsNull(f.Store.Enrolment(100, f.Earth.CastingPolicy!.Identity)); Assert.AreEqual(0, f.Store.CappedTraits(100).Count);
		var reload = f.NewCapability(91, 1, 11, true, XElement.Parse(f.Earth.SaveToXml()).Element("Casting"));
		Assert.IsTrue(reload.CastingConfigurationErrors().Any(x => x.Contains("theoretical", StringComparison.OrdinalIgnoreCase)));
		f.ActiveCapabilities.Clear(); Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 91, "lost entitlement").Allowed);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack(support ? "casting support remove 3" : "casting entry skill 1 default")));
		Assert.AreEqual(0, f.Earth.CastingConfigurationErrors().Count);
		var legacy = new TheoreticalSkill(definition, new MudSharp.Models.Trait { Value = 59, AdditionalValue = 59 }, f.Actor.Object);
		Assert.IsTrue(legacy.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, [])); Assert.AreEqual(64.0, legacy.PracticalValue);
		legacy.Value = 70; Assert.AreEqual(70.0, legacy.PracticalValue); Assert.AreEqual(70.0, legacy.TheoreticalValue);
	}

	[TestMethod]
	public void TypedPrerequisite_MissingTraitReference_RefusesAndPreservesUnreadableXml()
	{
		var f = new MagicCastingFixture(); var casting = XElement.Parse(f.Earth.SaveToXml()).Element("Casting")!;
		casting.Element("Admission")!.Add(new XElement("Prerequisite", new XAttribute("key", Guid.NewGuid()), new XAttribute("kind", "trait"), new XAttribute("proficiency", 80)));
		var reload = f.NewCapability(91, 1, 11, true, casting);
		Assert.IsTrue(reload.CastingConfigurationErrors().Count > 0); Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 91, "invalid support reference").Allowed);
		Assert.IsTrue(XElement.DeepEquals(casting, XElement.Parse(reload.SaveToXml()).Element("Casting")), "Invalid input must remain safely reviewable on save.");
	}

	[TestMethod]
	public void NativeCharacterOwnedSkill_BodyMaximumLookup_PermitsActualClassicImprovement()
	{
		var f = new MagicCastingFixture();
		var improver = new ClassicImprovement(f.World.Object, new MudSharp.Models.Improver { Id = 99, Name = "Native use",
			Definition = "<Definition Chance='1' Expression='5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='0'/>" });
		f.World.SetupGet(x => x.ImprovementModels).Returns(MagicCastingFixture.Collection(() => new IImprovementModel[] { improver }));
		var definition = new SkillDefinition(new MudSharp.Models.TraitDefinition { Id = 1, Name = "Canonical skill", ImproverId = 99 }, f.World.Object)
			{ Cap = new TraitExpression("100", f.World.Object) };
		var skill = new Skill(definition, 59, f.Actor.Object);
		var body = TestObjectFactory.CreateUninitialized<MudSharp.Body.Implementations.Body>(); body.Actor = f.Actor.Object;
		typeof(MudSharp.Body.Implementations.Body).GetField("_traits", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, new List<ITrait>());
		f.Actor.Setup(x => x.GetTrait(definition)).Returns(skill);
		f.Actor.Setup(x => x.TraitMaxValue(It.IsAny<ITraitDefinition>())).Returns<ITraitDefinition>(body.TraitMaxValue);
		Assert.AreEqual(100.0, body.TraitMaxValue(definition), "Character-owned skills must resolve through canonical GetTrait rather than body-only storage.");
		Assert.IsTrue(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, [])); Assert.AreEqual(64.0, skill.RawValue);
		var missing = new Mock<ITraitDefinition>(); missing.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Body);
		Assert.AreEqual(0.0, body.TraitMaxValue(missing.Object));
	}

	private static void Permanent(MagicCastingFixture f)
	{
		var merit = new Mock<IMagicCapabilityMerit>(); merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		merit.SetupGet(x => x.Capabilities).Returns(() => f.ActiveCapabilities); f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]);
	}
}

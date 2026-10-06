#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExpressionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic.Casting;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

public partial class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("valid", true)]
	[DataRow("expired", false)]
	[DataRow("improver", false)]
	[DataRow("cap", false)]
	[DataRow("body-rebound", false)]
	[DataRow("actor-body-replaced", false)]
	[DataRow("body-user", false)]
	[DataRow("independent-defender", true)]
	[DataRow("autonomous", true)]
	[DataRow("direct", true)]
	public void NativeSkillUse_CapturesPhysicalCheckUserAndGatesAfterCallbacks(string change, bool improved)
	{
		var f = LearningFixture();
		var improver = new Mock<IImprovementModel>();
		var def = LearningDefinition(f, improver.Object);
		// The durable trait belongs to a canonical owner, but the physical user is the NPC.
		var skill = new Skill(def, 40, f.Owner.Object);
		IHaveTraits user = change == "body-user" ? f.Body.Object : f.Actor.Object;
		var foreign = new Mock<ICharacter>();
		if (change == "independent-defender") user = foreign.Object;
		var mutations = 0;
		improver.Setup(x => x.GetImprovement(user, skill, Difficulty.Normal, Outcome.Pass, TraitUseType.Practical))
			.Callback(() =>
			{
				if (change is "improver" or "body-user" or "independent-defender" or "autonomous" or "direct")
					f.Grant = null;
				if (change == "body-rebound") f.Body.SetupGet(x => x.Actor).Returns(f.Commander.Object);
				if (change == "actor-body-replaced") f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
			}).Returns(5);
		if (change == "cap")
		{
			var cap = new Mock<ITraitExpression>();
			cap.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>()))
				.Callback(() => f.Grant = null).Returns(100);
			def.Cap = cap.Object;
		}
		skill.TraitValueChanged += (_, _) => ++mutations;
		var authority = CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get goods", () => true);
		using var execution = change == "direct" ? CommandExecutionScope.EnterIndependent() :
			CommandExecutionScope.EnterDispatch(authority, f.Actor.Object);
		using var autonomous = change == "autonomous" ? CommandExecutionScope.EnterIndependent() : null;
		if (change is "expired" or "independent-defender") f.Grant = null;
		Assert.AreEqual(improved, skill.TraitUsed(user, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(improved ? 45.0 : 40.0, skill.RawValue);
		Assert.AreEqual(improved ? 1 : 0, mutations);
		Assert.IsFalse(CommandExecutionScope.HasCommitted, "Learning must not mark the enclosing action committed.");
	}

	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void NativeClassicImprovement_RevocationPreservesOnlyAlreadyCommittedCooldown(bool theoretical, bool revokeAfterCooldown)
	{
		var f = LearningFixture();
		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Callback(() => f.Grant = null).Returns(5);
		var improver = theoretical ? (IImprovementModel)LearningTheoreticalImprover(f) :
			new ClassicImprovement(f.World.Object, new MudSharp.Models.Improver { Definition = LearningImproverXml });
		if (improver is ClassicImprovement classic) classic.ImprovementProg = prog.Object;
		else ((TheoreticalImprovementModel)improver).ImprovementProg = prog.Object;
		var merit = new Mock<ITraitLearningMerit>();
		merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		merit.Setup(x => x.SkillLearningChanceModifier(f.Actor.Object, It.IsAny<ITraitDefinition>(),
			Outcome.Pass, Difficulty.Normal, TraitUseType.Practical)).Callback(() =>
			{ if (!revokeAfterCooldown) f.Grant = null; }).Returns(1);
		f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]);
		var def = LearningDefinition(f, improver, theoretical);
		ITrait skill = theoretical ? new TheoreticalSkill((TheoreticalSkillDefinition)def,
			new MudSharp.Models.Trait { Value = 40, AdditionalValue = 40 }, f.Owner.Object) : new Skill(def, 40, f.Owner.Object);
		using var execution = LearningExecution(f);
		Assert.IsFalse(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(40.0, theoretical ? ((TheoreticalSkill)skill).PracticalValue : skill.RawValue);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<NoTraitGain>(), It.IsAny<TimeSpan>()),
			revokeAfterCooldown ? Times.Once : Times.Never);
		prog.Verify(x => x.ExecuteDouble(It.IsAny<object[]>()), revokeAfterCooldown ? Times.Once : Times.Never);
	}

	[DataTestMethod]
	[DataRow(TraitUseType.Practical, false)]
	[DataRow(TraitUseType.Theoretical, false)]
	[DataRow(TraitUseType.Practical, true)]
	[DataRow(TraitUseType.Theoretical, true)]
	public void NativeTheoreticalSkill_RefusesUncommittedValueAndPreservesValidTraining(TraitUseType use, bool valid)
	{
		var f = LearningFixture(); var improver = new Mock<IImprovementModel>();
		var def = (TheoreticalSkillDefinition)LearningDefinition(f, improver.Object, true);
		var skill = new TheoreticalSkill(def, new MudSharp.Models.Trait { Value = 40, AdditionalValue = 30 }, f.Owner.Object);
		improver.Setup(x => x.GetImprovement(f.Actor.Object, skill, Difficulty.Normal, Outcome.Pass, use))
			.Callback(() => { if (!valid) f.Grant = null; }).Returns(5);
		using var execution = LearningExecution(f);
		Assert.AreEqual(valid, skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, use, []));
		Assert.AreEqual(40.0 + (valid && use == TraitUseType.Practical ? 5 : 0), skill.PracticalValue);
		Assert.AreEqual(30.0 + (valid && use == TraitUseType.Theoretical ? 5 : 0), skill.TheoreticalValue);
		Assert.AreEqual(valid, skill.Changed);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("learnability")]
	[DataRow("branch-check")]
	[DataRow("opening-value")]
	[DataRow("already-acquired")]
	public void NativeStandardBranching_GatesCallbacksAndReportsOnlySuccessfulAcquisition(string change)
	{
		var f = LearningFixture(); var def = LearningDefinition(f, new NonImproving());
		var learn = new Mock<IFutureProg>();
		learn.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() =>
			{ if (change == "learnability") f.Grant = null; }).Returns(true);
		LearningSet(def, "<LearnableProg>k__BackingField", learn.Object);
		var branch = new Mock<ICheck>();
		branch.Setup(x => x.Check(f.Actor.Object, Difficulty.Normal, def, null, It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Callback(() =>
			{ if (change == "branch-check") f.Grant = null; }).Returns(CheckOutcome.SimpleOutcome(CheckType.TraitBranchCheck, Outcome.Pass));
		f.World.Setup(x => x.GetCheck(CheckType.TraitBranchCheck)).Returns(branch.Object);
		f.World.Setup(x => x.GetStaticDouble("SkillBranchBaseValue")).Callback(() =>
			{ if (change == "opening-value") f.Grant = null; }).Returns(3);
		f.Actor.Setup(x => x.AddTrait(def, 6)).Returns(change != "already-acquired");
		var check = LearningCheck(f, new TraitExpression("variable", f.World.Object));
		using var execution = LearningExecution(f);
		var outcome = check.Resolve(f.Actor.Object, def);
		Assert.AreEqual(Outcome.Pass, outcome.Outcome);
		Assert.AreEqual(change == "valid" ? 1 : 0, outcome.AcquiredTraits.Count());
		f.Actor.Verify(x => x.AddTrait(def, 6), change is "valid" or "already-acquired" ? Times.Once : Times.Never);
		branch.Verify(x => x.Check(f.Actor.Object, Difficulty.Normal, def, null, It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), change == "learnability" ? Times.Never : Times.Once);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NativeBranchingImprover_CappedSkillStillBranchesOnlyUnderValidAuthority(bool valid)
	{
		var f = LearningFixture();
		var improver = new BranchingImprover(new MudSharp.Models.Improver
			{ Definition = LearningImproverXml.Replace("/>", "><Branches/></Definition>") }, f.World.Object);
		var def = LearningDefinition(f, improver);
		var branch = Mock.Of<ITraitDefinition>();
		improver.BranchMap.Add((def, 30, 10, branch));
		f.Actor.Setup(x => x.TraitRawValue(def)).Callback(() => { if (!valid) f.Grant = null; }).Returns(40);
		f.Actor.Setup(x => x.AddTrait(branch, 10)).Returns(true);
		var casting = new Mock<IMagicCastingService>();
		casting.Setup(x => x.RawSkillImprovementCap(f.Owner.Object, def.Id)).Returns(40);
		f.World.SetupGet(x => x.MagicCasting).Returns(casting.Object);
		var skill = new Skill(def, 40, f.Owner.Object);
		using var execution = LearningExecution(f);
		Assert.IsFalse(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.AreEqual(40.0, skill.RawValue);
		f.Actor.Verify(x => x.AddTrait(branch, 10), valid ? Times.Once : Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NativeBranchCheck_RevokedMeritCannotCreateOrIncrementBranchChance(bool existing)
	{
		var f = LearningFixture(); var def = LearningDefinition(f, new NonImproving());
		var merit = new Mock<ITraitLearningMerit>();
		merit.Setup(x => x.Applies(f.Actor.Object)).Callback(() => f.Grant = null).Returns(true);
		merit.Setup(x => x.BranchingChanceModifier(f.Actor.Object, def)).Returns(1);
		f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]);
		var counter = new IncreasedBranchChance(f.Actor.Object);
		counter.UseSkill(def);
		f.Actor.Setup(x => x.EffectsOfType<IncreasedBranchChance>(It.IsAny<Predicate<IncreasedBranchChance>>()))
			.Returns(existing ? [counter] : []);
		var expr = new TraitExpression(new MudSharp.Models.TraitExpression { Id = 500, Expression = "100" }, f.World.Object);
		f.World.SetupGet(x => x.TraitExpressions).Returns(new All<ITraitExpression> { expr });
		var check = new BranchCheck(new MudSharp.Models.Check { TraitExpressionId = 500,
			CheckTemplate = new MudSharp.Models.CheckTemplate { Name = "branch" } }, f.World.Object);
		using var execution = LearningExecution(f);
		check.Check(f.Actor.Object, Difficulty.Normal, def);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<IncreasedBranchChance>()), Times.Never);
		Assert.AreEqual(1, counter.GetAttemptsForSkill(def));
	}

	[DataTestMethod]
	[DataRow("improver-direct")]
	[DataRow("improver-independent")]
	[DataRow("cap-direct")]
	[DataRow("cap-independent")]
	[DataRow("independent-skill-use")]
	public void NativeSkillUse_IndependentCallbackWriteToSameCanonicalTraitSurvivesOuterRefusal(string callback)
	{
		var f = LearningFixture(); var improver = new Mock<IImprovementModel>();
		var def = LearningDefinition(f, improver.Object); var skill = new Skill(def, 40, f.Owner.Object);
		var called = false;
		void SeparateWrite()
		{
			if (called) return;
			called = true; f.Grant = null;
			using var independent = callback.Contains("independent") ? CommandExecutionScope.EnterIndependent() : null;
			if (callback == "independent-skill-use")
				Assert.IsTrue(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
			else skill.Value = 60;
		}
		improver.Setup(x => x.GetImprovement(f.Actor.Object, skill, Difficulty.Normal, Outcome.Pass, TraitUseType.Practical))
			.Callback(() => { if (!callback.StartsWith("cap")) SeparateWrite(); }).Returns(5);
		if (callback.StartsWith("cap"))
		{
			var cap = new Mock<ITraitExpression>();
			cap.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>()))
				.Callback(SeparateWrite).Returns(100);
			def.Cap = cap.Object;
		}
		using var execution = LearningExecution(f);
		Assert.IsFalse(skill.TraitUsed(f.Actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []));
		Assert.IsTrue(called);
		Assert.AreEqual(callback == "independent-skill-use" ? 45.0 : 60.0, skill.RawValue);
	}

	[DataTestMethod]
	[DataRow("single")]
	[DataRow("all")]
	[DataRow("multi")]
	[DataRow("ogl-single")]
	[DataRow("ogl-multi")]
	[DataRow("bonus-single")]
	[DataRow("bonus-multi")]
	[DataRow("bonus-all")]
	public void NativeStandardScoring_RevokedMeritCannotEnterLearning(string entry)
	{
		var f = LearningFixture(); var improver = new Mock<IImprovementModel>();
		var def = LearningDefinition(f, improver.Object); var skill = new Skill(def, 40, f.Owner.Object);
		f.Actor.Setup(x => x.GetTrait(def)).Returns(skill); f.Actor.Setup(x => x.HasTrait(def)).Returns(true);
		var merit = new Mock<ICheckBonusMerit>();
		merit.Setup(x => x.Applies(f.Actor.Object, null)).Callback(() => f.Grant = null).Returns(true);
		f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]);
		var standard = LearningCheck(f, new TraitExpression("variable", f.World.Object));
		StandardCheck check = entry.StartsWith("ogl") ? new OGLCheck(standard.Model, f.World.Object) :
			entry.StartsWith("bonus") ? new BonusAbsentCheck(standard.Model, f.World.Object) : standard;
		if (entry.StartsWith("bonus")) f.Actor.Setup(x => x.TraitValue(def, It.IsAny<TraitBonusContext>()))
			.Callback(() => f.Grant = null).Returns(40);
		typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, 10);
		using var execution = LearningExecution(f);
		if (entry.EndsWith("all")) check.CheckAgainstAllDifficulties(f.Actor.Object, Difficulty.Normal, def);
		else if (entry.EndsWith("multi")) check.MultiDifficultyCheck(f.Actor.Object, Difficulty.Normal, Difficulty.Hard, trait: def);
		else check.Check(f.Actor.Object, Difficulty.Normal, def);
		improver.Verify(x => x.GetImprovement(It.IsAny<IHaveTraits>(), It.IsAny<ITrait>(), It.IsAny<Difficulty>(),
			It.IsAny<Outcome>(), It.IsAny<TraitUseType>()), Times.Never);
		Assert.AreEqual(40.0, skill.RawValue);
	}

	[TestMethod]
	public void NativeLearning_CompletedFirstGainSurvivesRevocationAndStopsLaterTraits()
	{
		var f = LearningFixture(); var improver = new Mock<IImprovementModel>();
		improver.Setup(x => x.GetImprovement(It.IsAny<IHaveTraits>(), It.IsAny<ITrait>(), It.IsAny<Difficulty>(),
			It.IsAny<Outcome>(), It.IsAny<TraitUseType>())).Returns(5);
		var one = LearningDefinition(f, improver.Object); var two = LearningDefinition(f, improver.Object);
		var first = new Skill(one, 40, f.Owner.Object); var second = new Skill(two, 40, f.Owner.Object);
		f.Actor.Setup(x => x.GetTrait(one)).Returns(first); f.Actor.Setup(x => x.GetTrait(two)).Returns(second);
		f.Actor.Setup(x => x.HasTrait(It.IsAny<ITraitDefinition>())).Returns(true);
		first.TraitValueChanged += (_, _) => f.Grant = null;
		var expr = new TraitExpression("first+second", f.World.Object);
		expr.Parameters.Add("first", new TraitExpressionParameter { Trait = one, CanImprove = true });
		expr.Parameters.Add("second", new TraitExpressionParameter { Trait = two, CanImprove = true });
		using var execution = LearningExecution(f);
		var outcome = LearningCheck(f, expr).Resolve(f.Actor.Object, null);
		Assert.AreEqual(45.0, first.RawValue); Assert.AreEqual(40.0, second.RawValue);
		CollectionAssert.AreEqual(new[] { one }, outcome.ImprovedTraits.ToArray());
		Assert.IsTrue(first.Changed); Assert.IsFalse(second.Changed);
	}

	private const string LearningImproverXml = "<Definition Chance='1' Expression='5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='60'/>";
	private static Fixture LearningFixture()
	{
		var f = new Fixture(); f.World.DefaultValue = DefaultValue.Mock;
		f.World.SetupGet(x => x.MagicCasting).Returns((IMagicCastingService)null!);
		f.Actor.SetupGet(x => x.Merits).Returns([]);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(1);
		f.Actor.Setup(x => x.TraitMaxValue(It.IsAny<ITraitDefinition>())).Returns(100);
		f.Actor.Setup(x => x.TraitMaxValue(It.IsAny<ITrait>())).Returns(100);
		return f;
	}
	private static IDisposable LearningExecution(Fixture f) => CommandExecutionScope.EnterDispatch(
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get goods", () => true), f.Actor.Object);
	private static SkillDefinition LearningDefinition(Fixture f, IImprovementModel improver, bool theory = false)
	{
		var def = theory ? (SkillDefinition)TestObjectFactory.CreateUninitialized<TheoreticalSkillDefinition>() : TestObjectFactory.CreateUninitialized<SkillDefinition>();
		LearningSet(def, "_gameworld", f.World.Object);
		LearningSet(def, "<OwnerScope>k__BackingField", TraitOwnerScope.Character);
		LearningSet(def, "<Improver>k__BackingField", improver);
		LearningSet(def, "_id", 99L);
		def.Cap = new TraitExpression("100", f.World.Object);
		if (theory) ((TheoreticalSkillDefinition)def).ValueExpression = new Expression("(practical+theory)/2");
		return def;
	}
	private static TheoreticalImprovementModel LearningTheoreticalImprover(Fixture f)
	{
		// Its XML-load constructor authors an expression in FMDB. Keep this unit fixture
		// entirely in memory; the disposable native lane owns database verification.
		var improver = TestObjectFactory.CreateUninitialized<TheoreticalImprovementModel>();
		LearningSet(improver, "_gameworld", f.World.Object);
		improver.ImprovementChance = 1;
		improver.ImprovementExpression = new TraitExpression("5", f.World.Object);
		improver.ImproveOnSuccess = improver.ImproveOnFail = true;
		improver.NoGainSecondsDiceExpression = "60";
		return improver;
	}
	private static void LearningSet(object target, string field, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is { } member)
			{ member.SetValue(target, value); return; }
		throw new InvalidOperationException(field);
	}
	private static LearningNativeCheck LearningCheck(Fixture f, TraitExpression expr)
	{
		LearningSet(expr, "_id", 500L);
		f.World.SetupGet(x => x.TraitExpressions).Returns(new All<ITraitExpression> { expr });
		var template = new MudSharp.Models.CheckTemplate { Name = "native learning", ImproveTraits = true, CanBranchIfTraitMissing = true };
		foreach (var difficulty in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new() { Difficulty = (int)difficulty });
		return new LearningNativeCheck(new MudSharp.Models.Check { Type = (int)CheckType.MeleeWeaponCheck,
			TraitExpressionId = 500, CheckTemplate = template, MaximumDifficultyForImprovement = (int)Difficulty.Impossible }, f.World.Object);
	}
	private sealed class LearningNativeCheck(MudSharp.Models.Check model, IFuturemud world) : StandardCheck(model, world)
	{
		internal MudSharp.Models.Check Model => model;
		internal CheckOutcome Resolve(IPerceivableHaveTraits user, ITraitDefinition? trait) =>
			HandleStandardCheck(user, null, Outcome.Pass, Difficulty.Normal, trait);
	}
}

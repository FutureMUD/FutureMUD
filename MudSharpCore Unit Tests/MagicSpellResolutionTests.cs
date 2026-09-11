#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicSpellResolutionTests
{
	[TestMethod]
	public void CastSpell_GroupFirstMemberResists_LaterMembersResolveAndInvocationSucceeds()
	{
		var fixture = new SpellFixture();
		var first = fixture.CreateTarget();
		var second = fixture.CreateTarget();
		var third = fixture.CreateTarget();
		var parameter = new SpellAdditionalParameter { ParameterName = "marker" };
		fixture.SetResistanceOutcome(first.Object, Outcome.MajorPass);

		using var invocation = fixture.BeginInvocation();
		fixture.Spell.CastSpell(fixture.Caster.Object, new PerceivableGroup([first.Object, second.Object, third.Object]),
			SpellPower.Strong, parameter);

		Assert.AreEqual(MagicInvocationStatus.Succeeded, invocation.Result.Status);
		fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, first.Object,
			It.IsAny<OpposedOutcomeDegree>(), SpellPower.Strong, It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		foreach (var target in new[] { second, third })
		{
			fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, target.Object,
				It.IsAny<OpposedOutcomeDegree>(), SpellPower.Strong, It.IsAny<IMagicSpellEffectParent>(),
				It.Is<SpellAdditionalParameter[]>(x => x.Length == 1 && ReferenceEquals(x[0], parameter))), Times.Once);
		}

		fixture.CasterEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, fixture.Caster.Object,
			It.IsAny<OpposedOutcomeDegree>(), SpellPower.Strong, It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
		fixture.CastingCheck.Verify(x => x.CheckAgainstAllDifficulties(fixture.Caster.Object,
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Once);
		fixture.ResistanceCheck.Verify(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(),
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), fixture.Caster.Object, It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Exactly(3));
		fixture.Caster.Verify(x => x.UseResource(fixture.Resource.Object, SpellFixture.CastingCost), Times.Once);
		fixture.Plan.Verify(x => x.ExecuteWholePlan(), Times.Once);
		fixture.Caster.Verify(x => x.AddEffect(It.IsAny<MagicSpellLockout>(), TimeSpan.FromSeconds(5)), Times.Once);
	}

	[TestMethod]
	public void CastSpell_GroupFirstMemberIsWardBlocked_LaterMembersResolveWithoutAResistanceCheck()
	{
		var fixture = new SpellFixture();
		var first = fixture.CreateTarget();
		var second = fixture.CreateTarget();
		var third = fixture.CreateTarget();
		fixture.AddBlockingWard(first);

		using var invocation = fixture.BeginInvocation();
		fixture.Spell.CastSpell(fixture.Caster.Object, new PerceivableGroup([first.Object, second.Object, third.Object]),
			SpellPower.Standard);

		Assert.AreEqual(MagicInvocationStatus.Succeeded, invocation.Result.Status);
		fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, first.Object,
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		foreach (var target in new[] { second, third })
		{
			fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, target.Object,
				It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
				It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
		}

		fixture.ResistanceCheck.Verify(x => x.CheckAgainstAllDifficulties(first.Object, It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), fixture.Caster.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string, object)[]>()), Times.Never);
		fixture.ResistanceCheck.Verify(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(),
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), fixture.Caster.Object, It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Exactly(2));
		fixture.CasterEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, fixture.Caster.Object,
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
	}

	[TestMethod]
	public void CastSpell_GroupLastMemberResists_RetainsEarlierResolutionAndCompletesOnce()
	{
		var fixture = new SpellFixture();
		var first = fixture.CreateTarget();
		var second = fixture.CreateTarget();
		var third = fixture.CreateTarget();
		fixture.SetResistanceOutcome(third.Object, Outcome.MajorPass);

		using var invocation = fixture.BeginInvocation();
		fixture.Spell.CastSpell(fixture.Caster.Object, new PerceivableGroup([first.Object, second.Object, third.Object]),
			SpellPower.Standard);

		Assert.AreEqual(MagicInvocationStatus.Succeeded, invocation.Result.Status);
		foreach (var target in new[] { first, second })
		{
			fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, target.Object,
				It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard, It.IsAny<IMagicSpellEffectParent>(),
				It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
		}

		fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, third.Object,
			It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard, It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		fixture.CasterEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, fixture.Caster.Object,
			It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard, It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
	}

	[TestMethod]
	public void CastSpell_GroupAllMembersReject_ProcessesAllAndLeavesCommittedInvocationFailed()
	{
		var fixture = new SpellFixture();
		var first = fixture.CreateTarget();
		var second = fixture.CreateTarget();
		var third = fixture.CreateTarget();
		fixture.SetResistanceOutcome(first.Object, Outcome.MajorPass);
		fixture.AddBlockingWard(second);
		fixture.SetResistanceOutcome(third.Object, Outcome.MajorPass);

		using var invocation = fixture.BeginInvocation();
		fixture.Spell.CastSpell(fixture.Caster.Object, new PerceivableGroup([first.Object, second.Object, third.Object]),
			SpellPower.Standard);

		Assert.AreEqual(MagicInvocationStatus.Failed, invocation.Result.Status);
		fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(),
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		fixture.CasterEffect.Verify(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(),
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		fixture.ResistanceCheck.Verify(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(),
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), fixture.Caster.Object, It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Exactly(2));
		fixture.Caster.Verify(x => x.UseResource(fixture.Resource.Object, SpellFixture.CastingCost), Times.Once);
		fixture.Plan.Verify(x => x.ExecuteWholePlan(), Times.Once);
		fixture.Caster.Verify(x => x.AddEffect(It.IsAny<MagicSpellLockout>(), TimeSpan.FromSeconds(5)), Times.Once);
	}

	[TestMethod]
	public void CastSpell_CastingCheckFails_CommitsCostsMaterialsAndLockoutBeforeFailing()
	{
		var fixture = new SpellFixture();
		var target = fixture.CreateTarget();
		fixture.SetCastingOutcome(Outcome.Fail);

		using var invocation = fixture.BeginInvocation();
		fixture.Spell.CastSpell(fixture.Caster.Object, target.Object, SpellPower.Standard);

		Assert.AreEqual(MagicInvocationStatus.Failed, invocation.Result.Status);
		fixture.Caster.Verify(x => x.UseResource(fixture.Resource.Object, SpellFixture.CastingCost), Times.Once);
		fixture.Plan.Verify(x => x.ExecuteWholePlan(), Times.Once);
		fixture.Caster.Verify(x => x.AddEffect(It.IsAny<MagicSpellLockout>(), TimeSpan.FromSeconds(5)), Times.Once);
		fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(),
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		fixture.CasterEffect.Verify(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(),
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		fixture.ResistanceCheck.Verify(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(),
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Never);
	}

	[TestMethod]
	public void CastSpell_EmptyGroup_PreservesCasterEffectsAndSuccessfulInvocation()
	{
		var fixture = new SpellFixture();

		using var invocation = fixture.BeginInvocation();
		fixture.Spell.CastSpell(fixture.Caster.Object, new PerceivableGroup([]), SpellPower.Standard);

		Assert.AreEqual(MagicInvocationStatus.Succeeded, invocation.Result.Status);
		fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(),
			It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Never);
		fixture.CasterEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, fixture.Caster.Object,
			It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard, It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
	}

	[TestMethod]
	public void CastSpell_InstantaneousTargetEffectWithoutAChild_StillResolvesAndRunsCasterEffects()
	{
		var fixture = new SpellFixture();
		var target = fixture.CreateTarget();

		using var invocation = fixture.BeginInvocation();
		fixture.Spell.CastSpell(fixture.Caster.Object, target.Object, SpellPower.Standard);

		Assert.AreEqual(MagicInvocationStatus.Succeeded, invocation.Result.Status);
		fixture.TargetEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, target.Object,
			It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard, It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
		target.Verify(x => x.AddEffect(It.IsAny<MagicSpellParent>(), It.IsAny<TimeSpan>()), Times.Never);
		fixture.CasterEffect.Verify(x => x.GetOrApplyEffect(fixture.Caster.Object, fixture.Caster.Object,
			It.IsAny<OpposedOutcomeDegree>(), SpellPower.Standard, It.IsAny<IMagicSpellEffectParent>(),
			It.IsAny<SpellAdditionalParameter[]>()), Times.Once);
	}

	private sealed class SpellFixture
	{
		public const double CastingCost = 3.0;
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Caster { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICheck> CastingCheck { get; } = new();
		public Mock<ICheck> ResistanceCheck { get; } = new();
		public Mock<IMagicResource> Resource { get; } = new();
		public Mock<IInventoryPlanTemplate> PlanTemplate { get; } = new();
		public Mock<IInventoryPlan> Plan { get; } = new();
		public Mock<IMagicSpellEffectTemplate> TargetEffect { get; } = new();
		public Mock<IMagicSpellEffectTemplate> CasterEffect { get; } = new();
		public MagicSpell Spell { get; }
		private readonly Mock<IMagicPower> _power = new();

		public SpellFixture()
		{
			var school = new Mock<IMagicSchool>();
			school.SetupGet(x => x.Id).Returns(1);
			var trait = new Mock<ITraitDefinition>();
			trait.SetupGet(x => x.Id).Returns(1);
			var knownProg = new Mock<IFutureProg>();
			knownProg.SetupGet(x => x.Id).Returns(0);
			var costExpression = new Mock<ITraitExpression>();
			costExpression.SetupGet(x => x.Id).Returns(1);
			costExpression.Setup(x => x.EvaluateWith(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(),
				TraitBonusContext.SpellCost, It.IsAny<(string, object)[]>())).Returns(CastingCost);
			Resource.SetupGet(x => x.Id).Returns(1);
			Resource.SetupGet(x => x.Name).Returns("focus");

			World.SetupGet(x => x.MagicSchools).Returns(Collection(school.Object));
			World.SetupGet(x => x.Traits).Returns(Collection(trait.Object));
			World.SetupGet(x => x.FutureProgs).Returns(Collection(knownProg.Object));
			World.SetupGet(x => x.TraitExpressions).Returns(Collection(costExpression.Object));
			World.SetupGet(x => x.MagicResources).Returns(Collection(Resource.Object));
			World.SetupGet(x => x.LegalAuthorities).Returns(Collection<ILegalAuthority>());
			World.Setup(x => x.GetStaticBool(PsychometricRecorder.EnabledSetting)).Returns(false);
			World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(CastingCheck.Object);
			World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(ResistanceCheck.Object);

			Caster.SetupGet(x => x.Gameworld).Returns(World.Object);
			Caster.SetupGet(x => x.Location).Returns((ICell)null!);
			Caster.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
			Caster.Setup(x => x.CombinedEffectsOfType<MagicSpellLockout>()).Returns([]);
			Caster.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
				.Returns([]);
			Caster.Setup(x => x.CanUseResource(Resource.Object, CastingCost)).Returns(true);
			Caster.Setup(x => x.UseResource(Resource.Object, CastingCost)).Returns(true);

			PlanTemplate.Setup(x => x.CreatePlan(Caster.Object)).Returns(Plan.Object);
			Plan.Setup(x => x.PlanIsFeasible()).Returns(InventoryPlanFeasibility.Feasible);
			Plan.Setup(x => x.ExecuteWholePlan()).Returns([]);
			TargetEffect.SetupGet(x => x.IsInstantaneous).Returns(true);
			TargetEffect.Setup(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(),
				It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
				It.IsAny<SpellAdditionalParameter[]>())).Returns((IMagicSpellEffect)null!);
			CasterEffect.SetupGet(x => x.IsInstantaneous).Returns(true);
			CasterEffect.Setup(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(),
				It.IsAny<OpposedOutcomeDegree>(), It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(),
				It.IsAny<SpellAdditionalParameter[]>())).Returns((IMagicSpellEffect)null!);

			SetCastingOutcome(Outcome.Pass);
			SetDefaultResistanceOutcome(Outcome.MajorFail);

			Spell = new MagicSpell(new MudSharp.Models.MagicSpell
			{
				Id = 1,
				Name = "Resolution Test",
				MagicSchoolId = 1,
				SpellKnownProgId = 0,
				CastingTraitDefinitionId = 1,
				ResistingTraitDefinitionId = 1,
				CastingDifficulty = (int)Difficulty.Normal,
				ResistingDifficulty = (int)Difficulty.Normal,
				MinimumSuccessThreshold = (int)Outcome.Pass,
				Definition = "<Spell><NoTrigger /><Costs><Cost resource='1' expression='1' /></Costs><Effects /><CasterEffects /><Plan><Phase /></Plan></Spell>"
			}, World.Object)
			{
				AppliedEffectsAreExclusive = false,
				CastingEmote = "casting",
				FailCastingEmote = "failing",
				ExclusiveDelay = TimeSpan.FromSeconds(5),
				Trigger = new Mock<IMagicTrigger> { DefaultValue = DefaultValue.Mock }.Object
			};
			Mock.Get(Spell.Trigger).SetupGet(x => x.TargetTypes).Returns("perceivables");
			Spell.InventoryPlanTemplate = PlanTemplate.Object;
			((List<IMagicSpellEffectTemplate>)Spell.SpellEffects).Add(TargetEffect.Object);
			((List<IMagicSpellEffectTemplate>)Spell.CasterSpellEffects).Add(CasterEffect.Object);
		}

		public Mock<ICharacter> CreateTarget()
		{
			var target = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
			target.SetupGet(x => x.Location).Returns((ICell)null!);
			target.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
			target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
				.Returns([]);
			return target;
		}

		public void AddBlockingWard(Mock<ICharacter> target)
		{
			var ward = new Mock<IMagicInterdictionEffect>();
			ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
			ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Fail);
			ward.Setup(x => x.ShouldInterdict(Caster.Object, Spell.School)).Returns(true);
			target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
				.Returns([ward.Object]);
		}

		public void SetCastingOutcome(Outcome outcome)
		{
			CastingCheck.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(),
				It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(),
				It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(Outcomes(CheckType.CastSpellCheck, outcome));
		}

		public void SetResistanceOutcome(ICharacter target, Outcome outcome)
		{
			ResistanceCheck.Setup(x => x.CheckAgainstAllDifficulties(target, It.IsAny<Difficulty>(),
				It.IsAny<ITraitDefinition>(), Caster.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(),
				It.IsAny<(string, object)[]>())).Returns(Outcomes(CheckType.ResistMagicSpellCheck, outcome));
		}

		public SpellPowerInvocation BeginInvocation() => new(Caster.Object, Spell, _power.Object);

		private void SetDefaultResistanceOutcome(Outcome outcome)
		{
			ResistanceCheck.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(),
				It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(),
				It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(Outcomes(CheckType.ResistMagicSpellCheck, outcome));
		}

		private static Dictionary<Difficulty, CheckOutcome> Outcomes(CheckType checkType, Outcome outcome)
			=> Enum.GetValues<Difficulty>()
				.ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(checkType, outcome));

		private static IUneditableAll<T> Collection<T>(params T[] values) where T : class, IFrameworkItem
		{
			var collection = new Mock<IUneditableAll<T>>();
			collection.Setup(x => x.Get(It.IsAny<long>()))
				.Returns<long>(id => values.FirstOrDefault(x => x.Id == id)!);
			collection.Setup(x => x.GetEnumerator()).Returns(() => values.AsEnumerable().GetEnumerator());
			return collection.Object;
		}
	}
}

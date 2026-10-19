#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;
using F = MudSharp_Unit_Tests.ChargenSkillSelectionGroupTests.Fixture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoleSkillProgressionRuntimeTests
{
	private static (F Fixture, Mock<IRoleSkillProgressionMerit> Merit, Mock<IChargenRole> Main, Mock<IChargenRole> Sub) Setup()
	{
		var f = new F();
		var merit = new Mock<IRoleSkillProgressionMerit>();
		merit.SetupGet(x => x.Id).Returns(42);
		merit.Setup(x => x.AppliesToTemplate(It.IsAny<ICharacterTemplate>())).Returns(true);
		merit.SetupGet(x => x.Progression).Returns(new RoleSkillProgression([
			new(10, 1, true, 20, 60), new(10, 2, true, 10, 60, 1, 50),
			new(11, 1, false, 20, 45, OverlapBonus: 15), new(11, 2, false, 20, 45, OverlapBonus: 15), new(11, 3, false, 20, 45, OverlapBonus: 15)]));
		var main = new Mock<IChargenRole>(); var sub = new Mock<IChargenRole>();
		foreach (var role in new[] { main, sub })
		{
			role.SetupGet(x => x.AdditionalMerits).Returns(new IMerit[] { merit.Object });
			role.SetupGet(x => x.TraitAdjustments).Returns(new Dictionary<ITraitDefinition, (double amount, bool giveIfMissing)>());
		}
		main.SetupGet(x => x.Id).Returns(10); sub.SetupGet(x => x.Id).Returns(11);
		f.Chargen.SelectedRoles.AddRange([main.Object, sub.Object]);
		return (f, merit, main, sub);
	}

	[TestMethod]
	public void ChargenProjection_SharedChildrenWaitAndRoleResetRemovesDerivedAwards()
	{
		var (f, _, _, sub) = Setup();
		var resolver = new SkillGroupResolver(f.Chargen, null);
		CollectionAssert.AreEquivalent(new long[] { 1, 3 }, f.Chargen.SelectedSkills.Select(x => x.Id).ToArray());
		Assert.AreEqual(35.0, RoleSkillProgressionService.StartingValue(f.Chargen, f.Skills[0], 0));
		Assert.AreEqual(200.0, RoleSkillProgressionService.StartingValue(f.Chargen, f.Skills[0], 200), "Independent and heritage claims max-merge.");
		Assert.AreEqual(0, f.Claims.Independent.Count);
		Assert.AreEqual(0, f.Claims.IndependentValues.Count);
		f.Chargen.SelectedRoles = [sub.Object];
		resolver.Revalidate();
		CollectionAssert.AreEquivalent(new long[] { 1, 2, 3 }, f.Chargen.SelectedSkills.Select(x => x.Id).ToArray());
		Assert.AreEqual(20.0, RoleSkillProgressionService.StartingValue(f.Chargen, f.Skills[0], 0));
		f.Chargen.SelectedRoles.Clear();
		resolver.Revalidate();
		Assert.AreEqual(0, f.Chargen.SelectedSkills.Count);
	}

	[TestMethod]
	public void AcquisitionGate_SharedChildRequiresPossessedParentAtThreshold()
	{
		var (f, merit, main, sub) = Setup();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		character.SetupGet(x => x.Roles).Returns(new[] { main.Object, sub.Object });
		character.SetupGet(x => x.Merits).Returns(new IMerit[] { merit.Object });
		merit.Setup(x => x.Applies(character.Object)).Returns(true);
		character.Setup(x => x.TraitRawValue(f.Skills[0])).Returns(70);
		Assert.IsFalse(RoleSkillProgressionService.CanAcquire(character.Object, f.Skills[1]));
		character.Setup(x => x.HasTrait(f.Skills[0])).Returns(true);
		character.Setup(x => x.TraitRawValue(f.Skills[0])).Returns(49);
		Assert.IsFalse(RoleSkillProgressionService.CanAcquire(character.Object, f.Skills[1]));
		character.Setup(x => x.TraitRawValue(f.Skills[0])).Returns(50);
		Assert.IsTrue(RoleSkillProgressionService.CanAcquire(character.Object, f.Skills[1]));
		// An unrelated skill still uses its ordinary LearnableProg; the profile adds no permission.
		character.SetupGet(x => x.Roles).Returns(new[] { main.Object });
		Assert.IsTrue(RoleSkillProgressionService.CanAcquire(character.Object, f.Skills[2]));
	}

	[TestMethod]
	public void Creation_KnownSharedChildGetsItsFloorWithoutGrantingAbsentChildren()
	{
		var (f, merit, main, sub) = Setup();
		f.Chargen.SelectedSkills.Add(f.Skills[1]);
		Assert.AreEqual(25.0, RoleSkillProgressionService.StartingValue(f.Chargen, f.Skills[1], 20));
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		character.SetupGet(x => x.Roles).Returns(new[] { main.Object, sub.Object });
		character.SetupGet(x => x.Merits).Returns(new IMerit[] { merit.Object });
		merit.Setup(x => x.Applies(character.Object)).Returns(true);
		character.Setup(x => x.EffectsOfType<RoleSkillBonusClaims>(It.IsAny<System.Predicate<RoleSkillBonusClaims>>())).Returns([]);
		character.Setup(x => x.HasTrait(f.Skills[1])).Returns(true);
		character.Setup(x => x.TraitRawValue(f.Skills[1])).Returns(20);
		RoleSkillProgressionService.InitialiseNewCharacter(character.Object);
		character.Verify(x => x.SetTraitValue(f.Skills[1], 25), Times.Once);
		character.Verify(x => x.AddTrait(f.Skills[1], It.IsAny<double>()), Times.Never);
		character.Verify(x => x.AddTrait(f.Skills[0], 35), Times.Once);
		character.Verify(x => x.AddTrait(f.Skills[2], 20), Times.Once);
	}

	[TestMethod]
	public void ChargenProjection_DisabledProfileDoesNotGrantRootsOrOpenings()
	{
		var (f, merit, _, _) = Setup();
		merit.Setup(x => x.AppliesToTemplate(It.IsAny<ICharacterTemplate>())).Returns(false);
		Assert.IsFalse(RoleSkillProgressionService.Applies(f.Chargen));
		var resolver = new SkillGroupResolver(f.Chargen, null);
		Assert.AreEqual(0, f.Chargen.SelectedSkills.Count);
		Assert.AreEqual(0.0, RoleSkillProgressionService.StartingValue(f.Chargen, f.Skills[0], 0));
	}

	[TestMethod]
	public void PersistedProfile_ConstantFalseApplicabilityDisablesCreationGrants()
	{
		var f = new F();
		var no = new Mock<IFutureProg>();
		no.SetupGet(x => x.Id).Returns(2);
		no.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		no.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(false);
		f.Progs.Add(no.Object);
		RoleSkillProgressionMerit.RegisterMeritInitialiser();
		var merit = (IRoleSkillProgressionMerit)MeritFactory.LoadMerit(new MudSharp.Models.Merit
		{
			Id = 42, Name = "Role progression", Type = RoleSkillProgressionMerit.MeritTypeName, MeritScope = 2,
			Definition = "<Definition><ApplicabilityProg>2</ApplicabilityProg><Rules><Rule role='10' trait='1' primary='true' opening='20' cap='60'/></Rules></Definition>"
		}, f.World.Object);
		Assert.IsFalse(merit.AppliesToTemplate(f.Chargen));
		Assert.AreEqual(1, merit.Progression.Rules.Count);
	}

	[TestMethod]
	public void PendingClaims_RestartRetiresAlreadyInsertedChildWithoutAwardingAgain()
	{
		var (f, _, _, _) = Setup();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var effect = new RoleSkillBonusClaims(character.Object, new Dictionary<long, double> { [2] = 25 });
		var persistedBeforeAcquisition = effect.SaveToXml(new Dictionary<IEffect, System.TimeSpan>());
		Assert.AreEqual(25.0, effect.Opening(2, 10));
		Assert.AreEqual(-5.0, effect.Opening(3, -5), "An unrelated trait must retain its requested opening.");
		effect.Consume(2);
		Assert.AreEqual(10.0, effect.Opening(2, 10));
		Assert.AreEqual(-5.0, effect.Opening(2, -5), "An empty initialized marker supplies no floor.");
		RoleSkillBonusClaims.InitialiseEffectType();
		// Model the crash window: final skill insertion committed, old effect XML remained.
		character.Setup(x => x.HasTrait(f.Skills[1])).Returns(true);
		var restored = (RoleSkillBonusClaims)Effect.LoadEffect(persistedBeforeAcquisition, character.Object);
		Assert.AreEqual(10.0, restored.Opening(2, 10));
		Assert.AreEqual(0, restored.SaveToXml(new Dictionary<IEffect, System.TimeSpan>()).Element("Effect")!.Elements().Count());
		character.Verify(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
		character.Verify(x => x.SetTraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
		// A child that was still absent retains its deferred floor across restart.
		character.Setup(x => x.HasTrait(f.Skills[1])).Returns(false);
		var pending = (RoleSkillBonusClaims)Effect.LoadEffect(persistedBeforeAcquisition, character.Object);
		Assert.AreEqual(25.0, pending.Opening(2, 10));
	}
}

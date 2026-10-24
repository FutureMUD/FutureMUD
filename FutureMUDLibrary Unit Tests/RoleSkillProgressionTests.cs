#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Traits;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoleSkillProgressionTests
{
	[TestMethod]
	public void Compose_SharedRootRetainsPrimaryCapAndClampsOneBonus()
	{
		var rules = new RoleSkillProgression([new(1, 10, true, 20, 45), new(2, 10, false, 20, 90, OverlapBonus: 30)]);
		var value = rules.Compose([1, 2])[10];
		Assert.AreEqual(45.0, value.Cap);
		Assert.AreEqual(45.0, value.ComposedOpening);
		Assert.IsTrue(value.InitiallyGranted);
		Assert.AreEqual(value, rules.Compose([1, 2])[10], "Recomposition is a projection, never an increment.");
	}

	[TestMethod]
	public void Compose_SharedChildKeepsPrerequisiteAndStandaloneRemainsImmediate()
	{
		var rules = new RoleSkillProgression([new(1, 10, true, 20, 80), new(1, 11, true, 10, 60, 10, 70), new(2, 11, false, 20, 45, OverlapBonus: 15)]);
		var shared = rules.Compose([1, 2])[11];
		Assert.IsFalse(shared.InitiallyGranted);
		Assert.AreEqual(10L, shared.ParentTraitId);
		Assert.AreEqual(70.0, shared.Threshold);
		Assert.AreEqual(25.0, shared.ComposedOpening);
		Assert.AreEqual(60.0, shared.Cap);
		var standalone = rules.Compose([2])[11];
		Assert.IsTrue(standalone.InitiallyGranted);
		Assert.AreEqual(20.0, standalone.ComposedOpening);
		Assert.AreEqual(45.0, standalone.Cap);
	}

	[TestMethod]
	public void Rules_RejectMissingParentsCyclesAndDuplicateMemberships()
	{
		Assert.ThrowsExactly<ArgumentException>(() => new RoleSkillProgression([new(1, 11, true, 10, 60, 10, 70)]));
		Assert.ThrowsExactly<ArgumentException>(() => new RoleSkillProgression([new(1, 10, true, 10, 60, 11, 50), new(1, 11, true, 10, 60, 10, 50)]));
		Assert.ThrowsExactly<ArgumentException>(() => new RoleSkillProgression([new(1, 10, true, 20, 60), new(1, 10, true, 20, 60)]));
		Assert.ThrowsExactly<ArgumentException>(() => new RoleSkillProgression([new(1, 10, false, 20, double.NaN)]));
	}
}

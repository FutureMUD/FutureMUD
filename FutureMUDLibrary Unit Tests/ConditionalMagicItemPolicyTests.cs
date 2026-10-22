using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

#nullable enable
namespace FutureMUDLibrary_Unit_Tests;

[TestClass]
public class ConditionalMagicItemPolicyTests
{
	[TestMethod]
	public void Shrines_UnrelatedCapabilitiesAndChoices_PersistIndependentPolicies()
	{
		var first = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.SpellSelectionShrine, EligibilityProgId = 11, CapabilityId = 12, SelectionLimit = 3, Choices = [new(100, "Detection"), new(101, "Travel")] };
		var second = first with { EligibilityProgId = 21, CapabilityId = 22, SelectionLimit = 1, AllowReplacement = false, Choices = [new(200, "Healing"), new(201, "Protection")] };
		foreach (var policy in new[] { first, second })
		{
			Assert.IsFalse(policy.Errors().Any());
			var restored = ConditionalMagicItemPolicy.Load(policy.Save());
			Assert.AreEqual(policy.CapabilityId, restored.CapabilityId);
			Assert.AreEqual(policy.SelectionLimit, restored.SelectionLimit);
			Assert.AreEqual(policy.AllowReplacement, restored.AllowReplacement);
			CollectionAssert.AreEqual(policy.Choices.Select(x => x.SpellId).ToArray(), restored.Choices.Select(x => x.SpellId).ToArray());
		}
		Assert.IsTrue(first.AcceptsSelection([101], [100], out _));
		Assert.IsFalse(second.AcceptsSelection([201], [200], out _));
		Assert.IsFalse(second.AcceptsSelection([100], [], out _));
		Assert.IsFalse(second.AcceptsSelection([200, 201], [], out _));
		Assert.IsFalse(first.AcceptsSelection([100, 100], [], out _));
	}

	[TestMethod]
	public void Binding_UnrelatedAuthorityTerritoryAndPayloads_RoundTripWithoutSettingDependencies()
	{
		var first = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ConditionalBinding, EligibilityProgId = 1, ControllerProgId = 2, TerritoryProgId = 3, BindingGroup = "magister", Range = BindingActivationRange.World, Activation = new(20, 40, 40, 10, true) };
		var second = first with { ControllerProgId = 12, TerritoryProgId = 13, BindingGroup = "oath", Range = BindingActivationRange.SameRoom, PunishOutsideTerritory = false, PreventRemoval = false, Activation = new(Stamina: 5), Periodic = new(Stun: 2) };
		foreach (var policy in new[] { first, second })
		{
			Assert.IsFalse(policy.Errors().Any());
			var restored = ConditionalMagicItemPolicy.Load(policy.Save());
			Assert.AreEqual(policy.BindingGroup, restored.BindingGroup);
			Assert.AreEqual(policy.ControllerProgId, restored.ControllerProgId);
			Assert.AreEqual(policy.TerritoryProgId, restored.TerritoryProgId);
			Assert.AreEqual(policy.Activation, restored.Activation);
			Assert.AreEqual(policy.Range, restored.Range);
		}
		Assert.AreEqual(5, first.Activation.HealthLoss(15), 0.00001);
		Assert.AreEqual(0, first.Activation.HealthLoss(5), 0.00001);
		Assert.AreEqual(20, first.Activation.HealthLoss(100), 0.00001);
		Assert.AreEqual(0, second.Activation.HealthLoss(100), 0.00001);
	}

	[TestMethod]
	public void Consequences_InvalidOrFractionalChannels_ValidateAndRespectHealthFloor()
	{
		Assert.IsFalse(new ItemConsequence(Health: double.NaN).IsValid);
		Assert.IsFalse(new ItemConsequence(StunFraction: double.PositiveInfinity).IsValid);
		Assert.IsFalse(new ItemConsequence(HealthFraction: 1.01).IsValid);
		Assert.AreEqual(25, new ItemConsequence(HealthFraction: 0.25).HealthLoss(100, 100), 0.00001);
		Assert.AreEqual(30, new ItemConsequence(HealthFloor: 10, HealthFraction: 1).HealthLoss(40, 100), 0.00001);
		Assert.AreEqual(40, new ItemConsequence(HealthFraction: 1, DestroyItem: true).HealthLoss(40, 100), 0.00001);
	}

	[DataTestMethod]
	[DataRow(8, 1, 1, 0, 11)]
	[DataRow(18, 50, 50, 0, 18)]
	[DataRow(18, 90, 90, 3, 14)]
	[DataRow(14, 80, 60, 0, 12)]
	public void BodilyCost_SourceIntegerRounding_UsesSkillAndWillpower(int basis, double skill, double will, int random, int expected) =>
		Assert.AreEqual(expected, PsionicBodilyCostPolicy.Price(basis, skill, will, random));
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using NativeBody = MudSharp.Body.Implementations.Body;
using NativeCharacter = MudSharp.Character.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonNativeReserveEffectTests
{
	[TestMethod]
	[DataRow(5.0, 40.0, 100.0, 45.0)]
	[DataRow(35.0, 40.0, 100.0, 75.0)]
	[DataRow(35.0, 90.0, 100.0, 100.0)]
	[DataRow(-5.0, 40.0, 100.0, 35.0)]
	[DataRow(-35.0, 40.0, 100.0, 5.0)]
	[DataRow(-35.0, 3.0, 100.0, 0.0)]
	[DataRow(5.0, 100.0, 100.0, 100.0)]
	[DataRow(-35.0, 0.0, 100.0, 0.0)]
	public void ExactStamina_NativeBodySetter_RespectsGradeBudgetBoundsAndBypassesExpenditure(double delta, double before, double maximum, double expected)
	{
		var (f, body, saves) = StaminaFixture(before, maximum);
		var multiplier = new Mock<IStaminaExpenditureEffect>(); multiplier.SetupGet(x => x.Multiplier).Returns(10);
		f.Actor.Setup(x => x.CombinedEffectsOfType<IStaminaExpenditureEffect>()).Returns([multiplier.Object]);
		var secondWind = new Mock<ISecondWindMerit>(); secondWind.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		f.Actor.SetupGet(x => x.Merits).Returns([secondWind.Object]);
		var effect = Stamina(f, delta);
		var result = effect.Apply(f.Actor.Object, f.Actor.Object, default, SpellPower.ExtremelyStrong, null!, []);
		Assert.AreEqual(expected, body.CurrentStamina, 1e-12);
		Assert.AreEqual(before == expected ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(before != expected, body.StaminaChanged);
		Assert.AreEqual(before != expected, body.Changed);
		Assert.AreEqual(Math.Max(0, before - expected), (double)Raw(body, "_staminaSpentInCurrentPeriod")!, 1e-12,
			"Only the actual signed native loss is recorded; a multiplier cannot turn the loss into expenditure.");
		f.Actor.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
		f.Actor.Verify(x => x.GainStamina(It.IsAny<double>()), Times.Never);
		f.Actor.Verify(x => x.CombinedEffectsOfType<IStaminaExpenditureEffect>(), Times.Never);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
		Assert.IsNull(result.Effect);
	}

	[TestMethod]
	public void ExactStamina_PreparedBodyOrInstanceDrift_RejectsWithoutWritingOrReselecting()
	{
		var (f, body, _) = StaminaFixture(40, 100);
		var effect = Stamina(f, -35);
		var selection = effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object)!;
		f.Actor.SetupGet(x => x.Body).Returns(f.Body.Object);
		Assert.IsFalse(effect.TryReusePreparedSelection(selection, f.Actor.Object, f.Actor.Object, out _));
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object, out _));
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []).Status);
		Assert.AreEqual(40.0, body.CurrentStamina);
		f.Actor.SetupGet(x => x.Body).Returns(body); f.Actor.SetupGet(x => x.InstanceId).Returns(999);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []).Status);
		Assert.AreEqual(40.0, body.CurrentStamina);
	}

	[TestMethod]
	public void ExactStamina_FinalBoundsGetterBodyDrift_RejectsConfirmationWithoutWriting()
	{
		var f = new MagicCastingFixture();
		f.Body.SetupGet(x => x.Actor).Returns(f.Actor.Object);
		f.Body.SetupGet(x => x.CurrentStamina).Returns(40);
		var replacement = new Mock<IBody>();
		f.Body.SetupGet(x => x.MaximumStamina).Returns(() =>
		{
			f.Actor.SetupGet(x => x.Body).Returns(replacement.Object); return 100;
		});
		var effect = Stamina(f, -35);
		effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object);
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object, out _));
		f.Body.VerifySet(x => x.CurrentStamina = It.IsAny<double>(), Times.Never);
		replacement.VerifySet(x => x.CurrentStamina = It.IsAny<double>(), Times.Never);
	}

	[TestMethod]
	public void ExactStamina_FormulaCallbackBodyDrift_RejectsBeforeNativeWrite()
	{
		var (f, body, _) = StaminaFixture(40, 100);
		var effect = Stamina(f, -35);
		var expression = new Mock<ITraitExpression>();
		expression.Setup(x => x.EvaluateWith(f.Actor.Object, null!, TraitBonusContext.None, It.IsAny<(string, object)[]>()))
			.Returns(() => { f.Actor.SetupGet(x => x.Body).Returns(f.Body.Object); return -35; });
		effect.AmountExpression = expression.Object;
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []).Status);
		Assert.AreEqual(40.0, body.CurrentStamina);
	}

	[TestMethod]
	public void ExactStamina_NativeDirtyCallbackAfterWrite_RetainsOriginalBodyAndSignalsPaidUncertainty()
	{
		var (f, body, saves) = StaminaFixture(40, 100);
		var effect = Stamina(f, -35);
		effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object);
		saves.Setup(x => x.Add(body)).Callback(() => f.Actor.SetupGet(x => x.Body).Returns(f.Body.Object));
		Assert.ThrowsExactly<InvalidOperationException>(() => effect.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []));
		Assert.AreEqual(5.0, body.CurrentStamina);
		Assert.IsTrue(body.StaminaChanged); Assert.IsTrue(body.Changed); Assert.IsTrue(f.Actor.Object.Changed);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []).Status);
		Assert.AreEqual(5.0, body.CurrentStamina, "An uncertain old-body write must never be replayed against a replacement body.");
	}

	[TestMethod]
	public void ExactStamina_NativeSaveRegistrationThrowsOnce_RearmsStaminaAndOriginalBodyQueue()
	{
		var (f, body, saves) = StaminaFixture(40, 100);
		var nativeQueue = new SaveManager();
		var registrations = 0;
		saves.Setup(x => x.Add(body)).Callback(() =>
		{
			if (++registrations == 1) throw new InvalidOperationException("Injected native queue registration failure.");
			nativeQueue.Add(body);
		});
		var effect = Stamina(f, -35);
		Assert.ThrowsExactly<InvalidOperationException>(() => effect.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []));
		Assert.AreEqual(5.0, body.CurrentStamina, "The uncertain native write is retained for staff reconciliation.");
		Assert.AreEqual(35.0, (double)Raw(body, "_staminaSpentInCurrentPeriod")!, "Repairing dirty state must not replay the signed delta.");
		Assert.IsTrue(body.StaminaChanged, "The native save must still include its changed stamina field.");
		Assert.IsTrue(body.Changed); Assert.IsTrue(f.Actor.Object.Changed);
		Assert.IsTrue(nativeQueue.IsQueued(body), "The actual native save queue must regain the exact original body after the throw.");
		Assert.AreEqual(2, registrations, "The native dirty-flag setter retries the lost registration exactly once.");
	}

	[TestMethod]
	[DataRow(1, 40.0, 35.0)]
	[DataRow(7, 40.0, 5.0)]
	[DataRow(7, 3.0, 0.0)]
	[DataRow(7, 0.0, 0.0)]
	public void ExactDepletion_NativeCharacterDebitAndSavedMagicRows_OnlyTouchConfiguredReserve(int grade, double before, double expected)
	{
		var (f, character, balances, _) = ReserveFixture(before);
		var effect = Reserve(f, -ArmageddonNativeReserveSpellContent.AmountForGrade(grade));
		var sourceBefore = f.Balances.ToDictionary(x => x.Key, x => x.Value);
		var result = effect.Apply(f.Actor.Object, character, default, SpellPower.ExtremelyStrong, null!, []);
		Assert.AreEqual(expected, balances[f.Resources[1]]);
		Assert.AreEqual(80.0, balances[f.Resources[2]], "An unrelated native reserve must remain intact.");
		Assert.IsTrue(sourceBefore.All(x => f.Balances[x.Key] == x.Value), "No reserve is credited to the caster.");
		Assert.AreEqual(before == expected ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(before != expected, character.ResourcesChanged);
		var saved = new MudSharp.Models.Character();
		saved.CharactersMagicResources.Add(new() { MagicResourceId = 11, Amount = before });
		saved.CharactersMagicResources.Add(new() { MagicResourceId = 12, Amount = 80 });
		character.SaveMagic(saved);
		Assert.AreEqual(2, saved.CharactersMagicResources.Count);
		Assert.AreEqual(expected, saved.CharactersMagicResources.Single(x => x.MagicResourceId == 11).Amount);
		Assert.AreEqual(80.0, saved.CharactersMagicResources.Single(x => x.MagicResourceId == 12).Amount);
		var restored = NativeReserveCharacter(f, out var restoredBalances);
		foreach (var row in saved.CharactersMagicResources) restoredBalances[f.Resources.Single(x => x.Id == row.MagicResourceId)] = row.Amount;
		Assert.AreEqual(expected, restored.MagicResourceAmounts[f.Resources[1]]);
		Assert.AreEqual(80.0, restored.MagicResourceAmounts[f.Resources[2]]);
	}

	[TestMethod]
	public void ExactDepletion_AbsentConfiguredReserve_NoChangeWithoutCreatingSubstitute()
	{
		var (f, character, balances, _) = ReserveFixture(40);
		balances.Remove(f.Resources[1]);
		var result = Reserve(f, -35).Apply(f.Actor.Object, character, default, default, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.NoChange, result.Status);
		Assert.IsFalse(balances.ContainsKey(f.Resources[1])); Assert.AreEqual(80.0, balances[f.Resources[2]]);
		Assert.IsFalse(character.ResourcesChanged);
	}

	[TestMethod]
	public void ExactDepletion_CapacityCallbackBodyDrift_RejectsBeforeAnyDebit()
	{
		var (f, character, balances, _) = ReserveFixture(40);
		var originalBody = character.Body;
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(character)).Returns(() =>
		{
			typeof(NativeCharacter).GetProperty(nameof(character.Body))!.SetValue(character, new Mock<IBody>().Object); return 100;
		});
		var result = Reserve(f, -35).Apply(f.Actor.Object, character, default, default, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, result.Status);
		Assert.AreEqual(40.0, balances[f.Resources[1]]); Assert.AreEqual(80.0, balances[f.Resources[2]]);
		Assert.IsFalse(character.ResourcesChanged); Assert.IsNotNull(originalBody);
	}

	[TestMethod]
	public void ExactDepletion_CapacityAdmission_DoesNotReevaluateCallbackInsideNativeDebit()
	{
		var (f, character, balances, _) = ReserveFixture(40);
		var calls = 0;
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(character)).Returns(() => ++calls == 1 ? 100 : throw new InvalidOperationException("Capacity callback was replayed inside the native write."));
		var result = Reserve(f, -35).Apply(f.Actor.Object, character, default, default, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(5.0, balances[f.Resources[1]]); Assert.AreEqual(1, calls);
	}

	[TestMethod]
	public void ExactDepletion_NativeDirtyCallbackReserveDefinitionDrift_PreservesWriteAndSignalsUncertainty()
	{
		var (f, character, balances, saves) = ReserveFixture(40);
		var effect = Reserve(f, -35);
		effect.CapturePreparedSelection(f.Actor.Object, character);
		saves.Setup(x => x.Add(character)).Callback(() => f.Resources.RemoveAll(x => x.Id == 11));
		Assert.ThrowsExactly<InvalidOperationException>(() => effect.Apply(f.Actor.Object, character, default, default, null!, []));
		Assert.AreEqual(5.0, balances[effect.Resource!]); Assert.IsTrue(character.ResourcesChanged);
		Assert.AreEqual(80.0, balances[f.Resources.Single(x => x.Id == 12)]);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, character, default, default, null!, []).Status);
	}

	[TestMethod]
	public void ExactDepletion_ProjectedRecipient_UsesPinnedNativeIdentityHolder()
	{
		var (f, holder, balances, _) = ReserveFixture(40);
		var projected = new Mock<ICharacter>(); projected.SetupGet(x => x.Gameworld).Returns(f.World.Object); projected.SetupGet(x => x.InstanceId).Returns(900);
		var body = new Mock<IBody>(); body.SetupGet(x => x.Id).Returns(901); body.SetupGet(x => x.Actor).Returns(projected.Object);
		projected.SetupGet(x => x.Body).Returns(body.Object);
		var identity = new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.PrimaryInstance).Returns(holder);
		projected.SetupGet(x => x.Identity).Returns(identity.Object);
		var effect = Reserve(f, -35);
		var result = effect.Apply(f.Actor.Object, projected.Object, default, default, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status); Assert.AreEqual(5.0, balances[f.Resources[1]]);
		projected.Verify(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
		projected.Verify(x => x.AddResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
		var changed = NativeReserveCharacter(f, out _); identity.SetupGet(x => x.PrimaryInstance).Returns(changed);
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Actor.Object, projected.Object, out _));
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, projected.Object, default, default, null!, []).Status);
		Assert.AreEqual(5.0, balances[f.Resources[1]]);
	}

	[TestMethod]
	[DataRow("NaN")]
	[DataRow("1e309")]
	[DataRow("0/0")]
	public void InvalidFiniteFormula_FailsCastingQuoteBeforePayment(string formula)
	{
		var f = new MagicCastingFixture();
		var spell = f.NewSpell(1, "Invalid exact stamina", $"<Effect type='staminadelta'><Formula>{formula}</Formula><DirectStaminaAdjustment>true</DirectStaminaAdjustment></Effect>");
		f.Spells.Remove(f.Spell); Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture"))); f.Acquire();
		var before = f.Balances.ToDictionary(x => x.Key, x => x.Value);
		var quoted = f.Service.Quote(f.Intent(1, false));
		Assert.IsFalse(quoted.Allowed); Assert.AreEqual(0, f.Store.Operations.Count);
		Assert.IsTrue(before.All(x => f.Balances[x.Key] == x.Value));
	}

	[TestMethod]
	public void ExactModes_InvalidExplicitPolicyAndTypedReserve_RetainDiagnosticsAndOriginalXmlThroughClone()
	{
		var f = new MagicCastingFixture();
		foreach (var xml in new[]
		{
			XElement.Parse("<Effect type='staminadelta'><Formula>5</Formula><DirectStaminaAdjustment>invalid</DirectStaminaAdjustment></Effect>"),
			XElement.Parse("<Effect type='magicresourcedelta'><Resource>11</Resource><Formula>-5</Formula><ExactResourceDepletion>invalid</ExactResourceDepletion></Effect>"),
			XElement.Parse("<Effect type='magicresourcedelta'><Resource>0</Resource><Formula>-5</Formula><ExactResourceDepletion>true</ExactResourceDepletion></Effect>")
		})
		{
			var effect = SpellEffectFactory.LoadEffect(xml, f.Spell); var clone = effect.Clone();
			var error = effect is StaminaDeltaSpellEffect stamina ? stamina.DefinitionError : ((MagicResourceDeltaEffect)effect).DefinitionError;
			Assert.IsNotNull(error);
			Assert.IsNotNull(clone is StaminaDeltaSpellEffect copied ? copied.DefinitionError : ((MagicResourceDeltaEffect)clone).DefinitionError);
			Assert.IsTrue(XNode.DeepEquals(xml, clone.SaveToXml()));
		}
	}

	[TestMethod]
	public void AbsentExactPolicies_PreserveLegacyStaminaAndResourcePaths()
	{
		var f = new MagicCastingFixture();
		var stamina = (StaminaDeltaSpellEffect)SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='staminadelta'><Formula>-5</Formula></Effect>"), f.Spell);
		Assert.AreEqual(MagicEffectOperationStatus.Unknown, stamina.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []).Status);
		f.Actor.Verify(x => x.SpendStamina(5), Times.Once);
		var resource = (MagicResourceDeltaEffect)SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='magicresourcedelta'><Resource>11</Resource><Formula>-5</Formula></Effect>"), f.Spell);
		Assert.AreEqual(MagicEffectOperationStatus.Unknown, resource.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []).Status);
		f.Actor.Verify(x => x.AddResource(f.Resources[1], -5), Times.Once);
		Assert.IsFalse(((StaminaDeltaSpellEffect)stamina.Clone()).DirectStaminaAdjustment);
		Assert.IsFalse(((MagicResourceDeltaEffect)resource.Clone()).ExactResourceDepletion);
	}

	private static StaminaDeltaSpellEffect Stamina(MagicCastingFixture f, double delta) => (StaminaDeltaSpellEffect)SpellEffectFactory.LoadEffect(
		new XElement("Effect", new XAttribute("type", "staminadelta"), new XElement("Formula", delta), new XElement("DirectStaminaAdjustment", true)), f.Spell);
	private static MagicResourceDeltaEffect Reserve(MagicCastingFixture f, double delta) => (MagicResourceDeltaEffect)SpellEffectFactory.LoadEffect(
		new XElement("Effect", new XAttribute("type", "magicresourcedelta"), new XElement("Resource", 11), new XElement("Formula", delta), new XElement("ExactResourceDepletion", true)), f.Spell);
	private static (MagicCastingFixture, NativeBody, Mock<ISaveManager>) StaminaFixture(double current, double maximum)
	{
		var f = new MagicCastingFixture(); var saves = new Mock<ISaveManager>(); f.World.SetupGet(x => x.SaveManager).Returns(saves.Object);
		f.Actor.SetupProperty(x => x.Changed, false);
		var body = TestObjectFactory.CreateUninitialized<NativeBody>(); body.Actor = f.Actor.Object;
		Raw(body, "_merits", new List<MudSharp.RPG.Merits.IMerit>());
		typeof(NativeBody).GetProperty(nameof(body.Gameworld))!.SetValue(body, f.World.Object);
		Raw(body, "_id", 200L); body.MaximumStamina = maximum; body.CurrentStamina = current; body.StaminaChanged = false; body.Changed = false;
		Raw(body, "_staminaSpentInCurrentPeriod", 0.0); f.Actor.SetupGet(x => x.Body).Returns(body);
		return (f, body, saves);
	}
	private static (MagicCastingFixture, NativeCharacter, DoubleCounter<IMagicResource>, Mock<ISaveManager>) ReserveFixture(double current)
	{
		var f = new MagicCastingFixture(); var saves = new Mock<ISaveManager>(); f.World.SetupGet(x => x.SaveManager).Returns(saves.Object);
		var character = NativeReserveCharacter(f, out var balances); balances[f.Resources[1]] = current; balances[f.Resources[2]] = 80;
		return (f, character, balances, saves);
	}
	private static NativeCharacter NativeReserveCharacter(MagicCastingFixture f, out DoubleCounter<IMagicResource> balances)
	{
		var character = TestObjectFactory.CreateUninitialized<NativeCharacter>();
		Raw(character, "_merits", new List<MudSharp.RPG.Merits.IMerit>());
		Raw(character, "_id", 800L); typeof(NativeCharacter).GetProperty(nameof(character.Gameworld))!.SetValue(character, f.World.Object);
		var body = new Mock<IBody>(); body.SetupGet(x => x.Id).Returns(801); body.SetupGet(x => x.Actor).Returns(character);
		typeof(NativeCharacter).GetProperty(nameof(character.Body))!.SetValue(character, body.Object);
		balances = new DoubleCounter<IMagicResource>(); Raw(character, "_magicResourceAmounts", balances);
		return character;
	}
	private static object? Raw(object item, string field, object? value = null)
	{
		for (var type = item.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is { } found)
			{
				if (value is not null) found.SetValue(item, value); return found.GetValue(item);
			}
		throw new InvalidOperationException("Missing native field " + field);
	}
}

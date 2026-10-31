#nullable enable
using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class EquipmentBonusesTests
{
	private static EquipmentBonusesGameItemComponentProto Load(IFuturemud world, string xml) =>
		(EquipmentBonusesGameItemComponentProto)Activator.CreateInstance(typeof(EquipmentBonusesGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			[new MudSharp.Models.GameItemComponentProto { Id = 1, Name = "equipment", Definition = xml,
				EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = 4 } }, world], null)!;

	[TestMethod]
	public void EquipmentState_ExcludesCarriedStoredAndBackProfile_IncludesPhysicalHands()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = Load(world.Object, "<Definition><Held>true</Held><ExcludedProfiles><Profile>9</Profile></ExcludedProfiles><Bonus kind='Trait' target='12' value='-3'/></Definition>");
		var actor = new Mock<ICharacter>(); var body = new Mock<IBody>(); var item = new Mock<IGameItem>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		item.SetupGet(x => x.Gameworld).Returns(world.Object);
		item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		var component = new EquipmentBonusesGameItemComponent(proto, item.Object, true);
		Assert.IsFalse(component.BonusesFor(actor.Object).Any());
		body.SetupGet(x => x.HeldItems).Returns([item.Object]);
		var hand = new Mock<IGrab>();
		body.Setup(x => x.HoldOrWieldLocFor(item.Object)).Returns(hand.Object);
		Assert.IsFalse(component.BonusesFor(actor.Object).Any(), "Abstract inventory is not equipment.");
		hand.SetupGet(x => x.Unary).Returns(true);
		Assert.AreEqual(-3.0, component.BonusesFor(actor.Object).Single().Bonus);
		body.SetupGet(x => x.HeldItems).Returns([]);
		body.SetupGet(x => x.WornItems).Returns([item.Object]);
		Assert.IsFalse(component.BonusesFor(actor.Object).Any(), "Belt attachments are not directly worn.");
		body.SetupGet(x => x.DirectWornItems).Returns([item.Object]);
		var wearable = new Mock<IWearable>(); var profile = new Mock<IWearProfile>();
		profile.SetupGet(x => x.Id).Returns(9);
		wearable.SetupGet(x => x.CurrentProfile).Returns(profile.Object);
		item.Setup(x => x.GetItemType<IWearable>()).Returns(wearable.Object);
		Assert.IsFalse(component.BonusesFor(actor.Object).Any());
		profile.SetupGet(x => x.Id).Returns(10);
		Assert.IsTrue(component.BonusesFor(actor.Object).Any());
		body.SetupGet(x => x.DirectWornItems).Returns([]);
		body.SetupGet(x => x.WieldedItems).Returns([item.Object]);
		Assert.IsTrue(component.BonusesFor(actor.Object).Any());
		item.SetupGet(x => x.InInventoryOf).Returns(Mock.Of<IBody>());
		Assert.IsFalse(component.BonusesFor(actor.Object).Any());
	}

	[TestMethod]
	public void IntrinsicBonuses_AddAcrossItemsWithoutDuplicatingExternalReferences_AndReload()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = Load(world.Object, "<Definition><Held>false</Held><Bonus kind='Stamina' target='0' value='20'/><Bonus kind='Trait' target='12' value='-3'/></Definition>");
		var saved = (string)typeof(EquipmentBonusesGameItemComponentProto).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(proto, null)!;
		var reloaded = Load(world.Object, saved);
		CollectionAssert.AreEqual(proto.Bonuses.ToArray(), reloaded.Bonuses.ToArray());
		var actor = new Mock<ICharacter>(); var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		IGameItem Item()
		{
			var item = new Mock<IGameItem>(); item.SetupGet(x => x.Gameworld).Returns(world.Object);
			item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
			var component = new EquipmentBonusesGameItemComponent(reloaded, item.Object, true);
			item.Setup(x => x.GetItemTypes<IEquippedItemBonusProvider>()).Returns([component]);
			return item.Object;
		}
		var first = Item(); var second = Item();
		body.SetupGet(x => x.ExternalItems).Returns([first, second, first]);
		body.SetupGet(x => x.WieldedItems).Returns([first, second]);
		Assert.AreEqual(40.0, EquippedItemBonuses.BonusFor(actor.Object, EquippedItemBonusKind.Stamina));
		Assert.AreEqual(-6.0, EquippedItemBonuses.BonusFor(actor.Object, EquippedItemBonusKind.Trait, 12));
		body.SetupGet(x => x.WieldedItems).Returns([first]);
		Assert.AreEqual(20.0, EquippedItemBonuses.BonusFor(actor.Object, EquippedItemBonusKind.Stamina));
	}

	[TestMethod]
	public void CombatAndSpellBonuses_AreFlatScopedAndRespectTargetCaps()
	{
		var actor = new Mock<ICharacter>(); var body = new Mock<IBody>(); var item = new Mock<IGameItem>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.ExternalItems).Returns([item.Object]);
		item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		var provider = new Mock<IEquippedItemBonusProvider>();
		provider.Setup(x => x.BonusesFor(actor.Object)).Returns([
			new(EquippedItemBonusKind.CombatAttack, 0, 2.5), new(EquippedItemBonusKind.CombatDefence, 0, -1.5),
			new(EquippedItemBonusKind.SpellResistance, 0, 4)]);
		item.Setup(x => x.GetItemTypes<IEquippedItemBonusProvider>()).Returns([provider.Object]);
		var expression = new Mock<ITraitExpression>();
		expression.Setup(x => x.EvaluateWith(actor.Object, null, It.IsAny<TraitBonusContext>(), It.IsAny<(string, object)[]>())).Returns(40.0);
		Assert.AreEqual(42.5, CheckEvaluationContext.EvaluateForCheck(expression.Object, actor.Object, null, CheckType.MeleeWeaponCheck));
		Assert.AreEqual(38.5, CheckEvaluationContext.EvaluateForCheck(expression.Object, actor.Object, null, CheckType.DodgeCheck));
		Assert.AreEqual(44.0, CheckEvaluationContext.EvaluateForCheck(expression.Object, actor.Object, null, CheckType.ResistMagicSpellCheck));
		Assert.AreEqual(40.0, CheckEvaluationContext.EvaluateForCheck(expression.Object, actor.Object, null, CheckType.ClimbCheck));
		Assert.AreEqual(41.0, CheckEvaluationContext.EvaluateForCheck(expression.Object, actor.Object, null, CheckType.MeleeWeaponCheck,
			(CheckEvaluationContext.ParameterName, new CheckEvaluationContext(41, Difficulty.Normal))));
		var world = new Mock<IFuturemud>();
		expression.SetupGet(x => x.Id).Returns(1);
		var expressions = new All<ITraitExpression>(); expressions.Add(expression.Object);
		world.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var model = new MudSharp.Models.Check { Type = (int)CheckType.MeleeWeaponCheck, TraitExpressionId = 1,
			CheckTemplate = new MudSharp.Models.CheckTemplate { Name = "Equipment check" } };
		foreach (var difficulty in Enum.GetValues<Difficulty>())
			model.CheckTemplate.CheckTemplateDifficulties.Add(new MudSharp.Models.CheckTemplateDifficulty { Difficulty = (int)difficulty, Modifier = 0 });
		var scaleField = typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldScale = scaleField.GetValue(null);
		try
		{
			scaleField.SetValue(null, 10);
			ICheck[] checks = [new StandardCheck(model, world.Object), new StaticCheck(model, world.Object), new BonusAbsentCheck(model, world.Object), new OGLCheck(model, world.Object)];
			foreach (var check in checks)
				// OGL rolls over its target, so a beneficial bonus lowers the threshold.
				Assert.AreEqual(check is OGLCheck ? -42.5 : 42.5,
					check.TargetNumber(actor.Object, Difficulty.Normal, null), check.GetType().Name);
		}
		finally { scaleField.SetValue(null, oldScale); }
		body.SetupGet(x => x.ExternalItems).Returns([]);
		Assert.AreEqual(40.0, CheckEvaluationContext.EvaluateForCheck(expression.Object, actor.Object, null, CheckType.ResistMagicSpellCheck));
	}

	[TestMethod]
	public void PhysicalAbsorption_ReducesDamageAndStunOnly_ClampsAndPreservesPacketMetadata()
	{
		var guard = new Mock<IDamageMutationGuard>();
		var original = new Damage { DamageAmount = 4, StunAmount = 1, PainAmount = 3, ShockAmount = 2,
			DamageType = DamageType.Crushing, MutationGuard = guard.Object, BypassWornArmour = true };
		var reduced = EquipmentDamageAbsorption.Apply(original, 1.5);
		Assert.AreEqual(2.5, reduced.DamageAmount); Assert.AreEqual(0.0, reduced.StunAmount);
		Assert.AreEqual(3.0, reduced.PainAmount); Assert.AreEqual(2.0, reduced.ShockAmount);
		Assert.AreSame(guard.Object, reduced.MutationGuard); Assert.IsTrue(reduced.BypassWornArmour);
		Assert.AreEqual(4.0, original.DamageAmount);
		Assert.AreEqual(0.0, EquipmentDamageAbsorption.Apply(original, 10).DamageAmount);
		foreach (var type in new[] { DamageType.Burning, DamageType.Freezing, DamageType.Hypoxia, DamageType.Cellular, DamageType.Eldritch })
		{
			var other = new Damage(original) { DamageType = type };
			Assert.AreSame(other, EquipmentDamageAbsorption.Apply(other, 10));
		}
	}
}

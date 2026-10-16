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
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.Magic.Resources;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class KindleItemEnchantmentTests
{
	public static IEnumerable<object[]> SourceBonuses()
	{
		(string Name, int[] Values)[] rows = [
			("strength", [0,0,0,1,2,3,4]), ("agility", [0,0,0,1,2,3,4]), ("wisdom", [0,0,0,1,1,2,3]),
			("endurance", [0,0,1,2,3,4,5]), ("health", [1,1,2,2,3,6,8]), ("stun", [1,1,2,2,3,6,8]),
			("stamina", [1,2,4,8,14,18,20]), ("mana-fire", [1,2,4,6,8,10,12])];
		foreach (var row in rows) for (var grade = 1; grade <= 7; grade++) yield return [row.Name, grade, row.Values[grade-1]];
	}

	[DataTestMethod]
	[DynamicData(nameof(SourceBonuses), DynamicDataSourceType.Method)]
	public void SelectedSourceBonus_AllGradesFreezeAndReloadExactly_ZeroBonusesRefuse(string choice, int grade, int expected)
	{
		var f = new MagicCastingFixture(); var spell = Load(f);
		var baseline = spell.SpellEffects.Single().SaveToXml();
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyStrong, Difficulty.Normal);
		var item = Item(f); var template = (ItemEmpowerEffect)copy.SpellEffects.Single();
		Assert.IsTrue(XNode.DeepEquals(baseline, template.Clone().SaveToXml()));
		var bound = template.TryBindInvocationInputs(f.Actor.Object, item, Inputs(choice), out var error);
		Assert.AreEqual(expected > 0, bound, error);
		if (expected == 0) return;
		Assert.AreEqual(60.0 * grade, copy.EffectDurationExpression.Evaluate(f.Actor.Object));
		var parent = new MagicSpellParent(item, copy, f.Actor.Object);
		var child = (SpellEquippedItemBonusEffect)template.GetOrApplyEffect(f.Actor.Object, item,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, parent, Inputs(choice))!;
		Assert.IsNotNull(child); Assert.AreEqual((double)expected, child.Bonus);
		item.AddEffect(child); parent.AddSpellEffect(child);
		Assert.AreEqual((double)expected, EquippedItemBonuses.BonusFor(f.Actor.Object, child.BonusKind, child.TargetId));
		Assert.AreEqual(0.0, EquippedItemBonuses.BonusFor(f.Actor.Object, EquippedItemBonusKind.Trait, 9999));
		Assert.IsFalse((object)child is IMagicWeaponEnhancementEffect);
		Assert.AreEqual(ItemQuality.Standard, item.RawQuality);
		SpellEquippedItemBonusEffect.InitialiseEffectType();
		var cold = (SpellEquippedItemBonusEffect)Effect.LoadEffect(child.SaveToXml(new()), item);
		Assert.AreEqual(child.BonusKind, cold.BonusKind); Assert.AreEqual(child.TargetId, cold.TargetId); Assert.AreEqual(child.Bonus, cold.Bonus);
		Assert.IsTrue(XNode.DeepEquals(baseline, spell.SpellEffects.Single().SaveToXml()));
	}

	[DataTestMethod]
	[DataRow("casting", 1)] [DataRow("strength", 1)] [DataRow("mana-missing", 7)]
	public void UnsupportedOrZeroChoice_RefusesBeforePaymentRollOrEffect(string choice, int grade)
	{
		var f = new MagicCastingFixture(); f.Acquire(7); f.Skills[1]=100; Load(f); var item = Item(f);
		f.Actor.Setup(x => x.TargetItem("ring")).Returns(item);
		var balances = f.Balances.Values.ToArray(); var writes = f.Store.Writes;
		var result = f.Service.Cast(f.Intent(grade, false) with { Targets = choice + " ring" });
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		CollectionAssert.AreEqual(balances, f.Balances.Values.ToArray());
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(writes, f.Store.Writes); Assert.AreEqual(0, item.Effects.Count());
	}

	[TestMethod]
	public void HeldOnlyAndTransferredItem_StopContributing_WhileWornOrWieldedContributeOnce()
	{
		var f = new MagicCastingFixture(); var spell = Load(f); var item = Item(f);
		var wearable = Mock.Get(item.GetItemType<IWearable>()); var holdable = Mock.Get(item.GetItemType<IHoldable>());
		item.AddEffect(new SpellEquippedItemBonusEffect(item, new MagicSpellParent(item, spell, f.Actor.Object), EquippedItemBonusKind.Trait, 91, 4));
		Assert.AreEqual(4.0, EquippedItemBonuses.BonusFor(f.Actor.Object, EquippedItemBonusKind.Trait, 91));
		f.Body.SetupGet(x => x.WornItems).Returns([]);
		Assert.AreEqual(0.0, EquippedItemBonuses.BonusFor(f.Actor.Object, EquippedItemBonusKind.Trait, 91), "A stale wearable owner does not equip an item absent from the body worn inventory.");
		wearable.SetupGet(x => x.WornBy).Returns((IBody)null!);
		Assert.AreEqual(0.0, EquippedItemBonuses.BonusFor(f.Actor.Object, EquippedItemBonusKind.Trait, 91), "Held is carried but is not equipped.");
		var wield = Mock.Get(item.GetItemType<IWieldable>()); wield.SetupGet(x => x.PrimaryWieldedLocation).Returns(Mock.Of<IWield>());
		f.Body.SetupGet(x => x.WieldedItems).Returns([item]);
		Assert.AreEqual(4.0, EquippedItemBonuses.BonusFor(f.Actor.Object, EquippedItemBonusKind.Trait, 91));
		f.Body.SetupGet(x => x.WieldedItems).Returns([]);
		Assert.AreEqual(0.0, EquippedItemBonuses.BonusFor(f.Actor.Object, EquippedItemBonusKind.Trait, 91), "A retained wield location after native drop/pickup does not equip a held item.");
		f.Body.SetupGet(x => x.WieldedItems).Returns([item]);
		holdable.SetupGet(x => x.HeldBy).Returns(Mock.Of<IBody>());
		Assert.AreEqual(0.0, EquippedItemBonuses.BonusFor(f.Actor.Object, EquippedItemBonusKind.Trait, 91), "Stale external inventory membership cannot give another holder a bonus.");
	}

	[DataTestMethod]
	[DataRow(typeof(SimpleLivingHealthStrategy))] [DataRow(typeof(ComplexLivingHealthStrategy))]
	[DataRow(typeof(RobotHealthStrategy))] [DataRow(typeof(ConstructHealthStrategy))]
	[DataRow(typeof(BrainConstructHealthStrategy))] [DataRow(typeof(BrainHitpointsStrategy))]
	public void NativeHealthCapacity_ChangesThresholdWithoutChangingWounds(Type type)
	{
		var f = new MagicCastingFixture(); var spell = Load(f); var item = Item(f);
		var strategy = (IHealthStrategy)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(type);
		var expression = new Mock<ITraitExpression>();
		expression.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>())).Returns(100);
		type.GetProperty("MaximumHitPointsExpression", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!.SetValue(strategy, expression.Object);
		var wound = new Mock<IWound>(); wound.SetupGet(x => x.CurrentDamage).Returns(103);
		f.Actor.SetupGet(x => x.Wounds).Returns([wound.Object]);
		Assert.AreEqual(100.0, strategy.MaxHP(f.Actor.Object));
		var bonus = new SpellEquippedItemBonusEffect(item, new MagicSpellParent(item, spell, f.Actor.Object), EquippedItemBonusKind.Health, 0, 8);
		item.AddEffect(bonus);
		Assert.AreEqual(108.0, strategy.MaxHP(f.Actor.Object));
		Assert.AreEqual(103.0, wound.Object.CurrentDamage);
		if (type == typeof(ConstructHealthStrategy)) Assert.AreEqual(HealthTickResult.None, strategy.EvaluateStatus(f.Actor.Object), "The actual death threshold must use augmented health capacity.");
		item.RemoveEffect(bonus);
		Assert.AreEqual(100.0, strategy.MaxHP(f.Actor.Object));
		Assert.AreEqual(103.0, wound.Object.CurrentDamage);
		if (type == typeof(ConstructHealthStrategy)) Assert.AreEqual(HealthTickResult.Dead, strategy.EvaluateStatus(f.Actor.Object));
	}

	[DataTestMethod]
	[DataRow(false)] [DataRow(true)]
	public void MagicCapacity_UsesSamePrimaryHolderForBothApis_AndNeverRefillsBalances(bool attributeCapacity)
	{
		var f = new MagicCastingFixture(physicalInstance: true); var spell = Load(f); var item = Item(f);
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(90);
		prog.Setup(x => x.ExecuteDouble(It.IsAny<double>(), It.IsAny<object[]>())).Returns(100);
		var progs=f.World.Object.FutureProgs.Append(prog.Object).ToArray();
		f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => progs));
		var capacityXml=new XElement("Definition",new XElement("ResourceCapProg",90));
		if(attributeCapacity)
		{
			f.Skills[93]=10;
			f.Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression{Id=901,Expression="100"},f.World.Object));
			capacityXml.Add(new XElement("AttributeCapacity",new XAttribute("version",1),new XAttribute("attribute",93),new XAttribute("expression",901),new XAttribute("basis","effective")));
		}
		var resource=new SimpleMagicResource(new MudSharp.Models.MagicResource { Id=11, Name="Fire", ShortName="fire",Definition=capacityXml.ToString() },f.World.Object);
		f.Balances[f.Resources[1]]=3;
		var bonus=new SpellEquippedItemBonusEffect(item,new MagicSpellParent(item,spell,f.Actor.Object),EquippedItemBonusKind.MagicResource,11,12);
		item.AddEffect(bonus);
		var projected=new Mock<ICharacter> { DefaultValue=DefaultValue.Mock };
		var identity=new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.PrimaryInstance).Returns((ICharacterInstance)f.Actor.Object);
		projected.SetupGet(x => x.Identity).Returns(identity.Object); projected.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		Assert.AreEqual(112.0,resource.ResourceCap(f.Actor.Object)); Assert.AreEqual(112.0,resource.ResourceCap(projected.Object));
		Assert.IsTrue(resource.TryGetResourceCap(projected.Object,out var cap,out var error),error); Assert.AreEqual(112.0,cap);
		Assert.AreEqual(3.0,f.Balances[f.Resources[1]],"Raising capacity is not a reserve grant.");
		item.RemoveEffect(bonus);
		Assert.AreEqual(100.0,resource.ResourceCap(projected.Object)); Assert.AreEqual(3.0,f.Balances[f.Resources[1]]);
	}

	[TestMethod]
	public void StaminaCapacity_AttachDoesNotRefill_RemoveClampsWithoutResettingExertion()
	{
		var f=new MagicCastingFixture(); var spell=Load(f); var item=Item(f);
		var body=TestObjectFactory.CreateUninitialized<MudSharp.Body.Implementations.Body>(); body.Actor=f.Actor.Object;
		typeof(MudSharp.Body.Implementations.Body).GetProperty(nameof(body.Gameworld))!.SetValue(body,f.World.Object);
		typeof(PerceivedItem).GetProperty(nameof(body.EffectHandler))!.SetValue(body,new EffectHandler(body));
		typeof(MudSharp.Body.Implementations.Body).GetField("_externalItems",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(body,new List<IGameItem>{item});
		typeof(MudSharp.Body.Implementations.Body).GetField("_wornItemsOnly", BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(body, new List<IGameItem>{item});
		f.Actor.SetupGet(x => x.Body).Returns(body);
		Mock.Get(item.GetItemType<IWearable>()).SetupGet(x => x.WornBy).Returns(body);
		Mock.Get(item.GetItemType<IHoldable>()).SetupGet(x => x.HeldBy).Returns(body);
		var prog=new Mock<IFutureProg>(); prog.Setup(x => x.Execute(It.IsAny<object[]>())).Returns((object)100m);
		var field=typeof(MudSharp.Character.Character).GetField("MaximumStaminaProg",BindingFlags.Static|BindingFlags.NonPublic)!;
		var original=field.GetValue(null); field.SetValue(null,prog.Object);
		try
		{
			body.MaximumStamina=100; body.CurrentStamina=47;
			var exertion=typeof(MudSharp.Body.Implementations.Body).GetField("_currentExertion",BindingFlags.Instance|BindingFlags.NonPublic)!;
			exertion.SetValue(body,ExertionLevel.Heavy);
			var bonus=new SpellEquippedItemBonusEffect(item,new MagicSpellParent(item,spell,f.Actor.Object),EquippedItemBonusKind.Stamina,0,20);
			item.AddEffect(bonus); Assert.AreEqual(120.0,body.MaximumStamina); Assert.AreEqual(47.0,body.CurrentStamina);
			body.CurrentStamina=119; item.RemoveEffect(bonus); Assert.AreEqual(100.0,body.MaximumStamina); Assert.AreEqual(100.0,body.CurrentStamina);
			Assert.AreEqual(ExertionLevel.Heavy,exertion.GetValue(body));
		}
		finally { field.SetValue(null,original); }
	}

	[TestMethod]
	public void SelectedAttribute_FlowsThroughNativeBodyTraitAndExpression_ParentRetainsExactBonusCold()
	{
		var f=new MagicCastingFixture();var spell=Load(f);var item=Item(f);
		var body=TestObjectFactory.CreateUninitialized<MudSharp.Body.Implementations.Body>();body.Actor=f.Actor.Object;
		typeof(MudSharp.Body.Implementations.Body).GetProperty(nameof(body.Gameworld))!.SetValue(body,f.World.Object);
		typeof(PerceivedItem).GetProperty(nameof(body.EffectHandler))!.SetValue(body,new EffectHandler(body));
		foreach(var name in new[]{"_traits","_merits","_implants","_externalItems"})
		{
			var field=typeof(MudSharp.Body.Implementations.Body).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic)!;
			field.SetValue(body,Activator.CreateInstance(field.FieldType));
		}
		typeof(MudSharp.Body.Implementations.Body).GetField("_wornItemsOnly", BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(body, new List<IGameItem>{item});
		f.Actor.SetupGet(x => x.Body).Returns(body);f.Actor.SetupGet(x => x.Merits).Returns([]);
		Mock.Get(item.GetItemType<IWearable>()).SetupGet(x => x.WornBy).Returns(body);
		Mock.Get(item.GetItemType<IHoldable>()).SetupGet(x => x.HeldBy).Returns(body);
		((List<IGameItem>)typeof(MudSharp.Body.Implementations.Body).GetField("_externalItems",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(body)!).Add(item);
		var definition=f.Traits.Single(x => x.Id==91);var trait=new Mock<ITrait>();trait.SetupGet(x => x.Definition).Returns(definition);trait.SetupGet(x => x.Value).Returns(10);
		((List<ITrait>)typeof(MudSharp.Body.Implementations.Body).GetField("_traits",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(body)!).Add(trait.Object);
		var expressionModel=new MudSharp.Models.TraitExpression{Id=901,Expression="strength"};
		expressionModel.TraitExpressionParameters.Add(new(){Parameter="strength",TraitDefinitionId=91});
		var expression=new TraitExpression(expressionModel,f.World.Object);Assert.AreEqual(10.0,expression.Evaluate(body));
		var parent=new MagicSpellParent(item,spell,f.Actor.Object);
		var child=new SpellEquippedItemBonusEffect(item,parent,EquippedItemBonusKind.Trait,91,4);parent.AddSpellEffect(child);
		// The native consumer is tested without attachment callbacks; stamina reconciliation has its separate native-body regression.
		var effects=(List<IEffect>)typeof(EffectHandler).GetField("_effects",BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(item.EffectHandler)!;
		effects.Add(child);Assert.AreEqual(14.0,body.TraitValue(definition));Assert.AreEqual(14.0,expression.Evaluate(body));
		MagicSpellParent.InitialiseEffectType();SpellEquippedItemBonusEffect.InitialiseEffectType();
		var saved=parent.SaveToXml(new(){[parent]=TimeSpan.FromSeconds(420)});
		Assert.AreEqual(420000L,(long)saved.Element("Remaining")!);
		var savedChild=saved.Element("Effect")!.Element("Children")!.Elements().Single();
		var cold=(SpellEquippedItemBonusEffect)Effect.LoadEffect(savedChild,item);
		Assert.AreEqual(EquippedItemBonusKind.Trait,cold.BonusKind);Assert.AreEqual(91L,cold.TargetId);Assert.AreEqual(4.0,cold.Bonus);
		effects.Remove(child);Assert.AreEqual(10.0,body.TraitValue(definition));Assert.AreEqual(10.0,expression.Evaluate(body));
	}

	private static SpellAdditionalParameter[] Inputs(string choice) => [new() { ParameterName = "item-enchantment-choice", Item = choice }];
	private static MagicSpell Load(MagicCastingFixture f)
	{
		ItemEmpowerEffect.RegisterFactory(); CastingTriggerItemChoice.RegisterFactory();
		f.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(),It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		Mock.Get(f.Resources[1]).SetupGet(x => x.ResourceType).Returns(MagicResourceType.PlayerResource);
		for (var id = 91; id <= 94; id++)
		{
			var trait = new Mock<IAttributeDefinition>(); var captured = id;
			trait.SetupGet(x => x.Id).Returns(captured); trait.SetupGet(x => x.TraitType).Returns(TraitType.Attribute);
			trait.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Body); f.Traits.Add(trait.Object);
		}
		var content = ArmageddonKindleContent.Create(91,92,93,94,new Dictionary<string,long> { ["fire"] = 11 }).Content;
		f.Expressions.Clear(); var cost = content.CostRow(1); cost.Id = 1; var duration = content.DurationRow(1); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object)); f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1,1,1); row.Id=1; row.EffectDurationExpressionId=2;
		row.Definition=content.BuildDefinition(10,1,0).ToString(); var spell=new MagicSpell(row,f.World.Object); f.Spells[0]=spell; return spell;
	}
	private static GameItem Item(MagicCastingFixture f)
	{
		var proto = new Mock<IGameItemProto>(); proto.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		proto.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		proto.SetupGet(x => x.Keywords).Returns(["ring"]);
		var wearable = new Mock<IWearable>(); wearable.SetupGet(x => x.WornBy).Returns(f.Body.Object);
		var holdable = new Mock<IHoldable>(); holdable.SetupGet(x => x.HeldBy).Returns(f.Body.Object);
		var wieldable = new Mock<IWieldable>();
		IGameItemComponentProto Component(IGameItemComponent component)
		{
			var model = new Mock<IGameItemComponentProto>();
			model.Setup(x => x.CreateNew(It.IsAny<IGameItem>(), It.IsAny<ICharacter>(), It.IsAny<bool>())).Returns(component); return model.Object;
		}
		proto.SetupGet(x => x.Components).Returns([Component(wearable.Object), Component(holdable.Object), Component(wieldable.Object)]);
		var item = new GameItem(proto.Object, quality: ItemQuality.Standard);
		f.Body.SetupGet(x => x.ExternalItems).Returns([item]);
		f.Body.SetupGet(x => x.WornItems).Returns(() => ReferenceEquals(wearable.Object.WornBy, f.Body.Object) ? new IGameItem[] { item } : Array.Empty<IGameItem>());
		return item;
	}
}

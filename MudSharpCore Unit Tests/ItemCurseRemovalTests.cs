#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemCurseRemovalTests
{
	private static ItemPermissionsGameItemComponentProto Prototype(IFuturemud world, bool cursed, int restrictions, int removable)
	{
		var model = VancianItemTests.Prototype("Curse Permissions");
		model.Definition = new XElement("Definition", new XElement("Restrictions", restrictions), new XElement("Cursed", cursed),
			new XElement("CurseRemovableRestrictions", removable)).ToString();
		return (ItemPermissionsGameItemComponentProto)Activator.CreateInstance(typeof(ItemPermissionsGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic, null, [model, world], null)!;
	}

	[TestMethod]
	[DataRow(true, 0, 0)]
	[DataRow(true, 54, 6)]
	[DataRow(false, 54, 6)]
	public void CurseRemoval_OnlyReleasesDesignatedRestrictionsAndPersists(bool cursed, int mask, int removable)
	{
		var f = new VancianItemTests.ItemFixture();
		var item = f.Item(700);
		var proto = Prototype(f.F.World.Object, cursed, mask, removable);
		var component = (ItemPermissionsGameItemComponent)proto.CreateNew(item.Object, temporary: true);
		Assert.AreEqual(cursed, component.IsCursed);
		Assert.AreEqual(cursed, component.RemoveCurse());
		Assert.AreEqual((ItemInteractionRestriction)(cursed ? mask & ~removable : mask), component.Restrictions);
		Assert.IsFalse(component.IsCursed);
		Assert.IsFalse(component.RemoveCurse());
		var restored = new ItemPermissionsGameItemComponent(new() { Id = 701, Definition = VancianItemTests.Serialize(component) }, proto, item.Object);
		Assert.IsFalse(restored.IsCursed);
		Assert.AreEqual(component.Restrictions, restored.Restrictions);
		Assert.IsFalse(((ICurseRemovableItem)restored.Copy(item.Object, true)).IsCursed);
		component.SetPermission(ItemInteractionRestriction.Drop, false);
		Assert.IsFalse(component.RemoveCurse(), "Later independent restrictions are not another curse.");
		Assert.IsTrue(component.Restrictions.HasFlag(ItemInteractionRestriction.Drop));
	}

	[TestMethod]
	public void RemoveCurse_ItemModeIsOptionalAndDoesNotLiftIndependentComponents()
	{
		var f = new VancianItemTests.ItemFixture();
		var item = f.Item(702);
		var cursed = (ItemPermissionsGameItemComponent)Prototype(f.F.World.Object, true, 54, 6).CreateNew(item.Object, temporary: true);
		var ordinary = (ItemPermissionsGameItemComponent)Prototype(f.F.World.Object, false, 2, 0).CreateNew(item.Object, temporary: true);
		item.Setup(x => x.GetItemTypes<ICurseRemovableItem>()).Returns([cursed, ordinary]);
		item.Setup(x => x.GetItemTypes<IItemInteractionPermissions>()).Returns([cursed, ordinary]);
		var spell = new Mock<IMagicSpell>(); spell.SetupGet(x => x.Gameworld).Returns(f.F.World.Object);
		RemoveCurseEffect Load(bool include) => (RemoveCurseEffect)SpellEffectFactory.LoadEffect(
			new XElement("Effect", new XAttribute("type", "removecurse"), new XElement("IncludeItems", include)), spell.Object);
		Load(false).GetOrApplyEffect(f.F.Actor.Object, item.Object, OpposedOutcomeDegree.None, SpellPower.Standard, null!, []);
		Assert.IsTrue(cursed.IsCursed);
		var effect = Load(true);
		Assert.IsTrue(((RemoveCurseEffect)effect.Clone()).IncludeItems);
		effect.GetOrApplyEffect(f.F.Actor.Object, item.Object, OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, null!, []);
		Assert.IsFalse(cursed.IsCursed);
		Assert.IsFalse(item.Object.Permits(ItemInteractionRestriction.Drop), "An independent component still forbids dropping.");
		Assert.IsTrue(item.Object.Permits(ItemInteractionRestriction.Give));
		Assert.IsFalse(item.Object.Permits(ItemInteractionRestriction.RoomListing));
		Assert.IsFalse(item.Object.Permits(ItemInteractionRestriction.Sheathe));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CharacterTrigger_OptionalItemSelectionUsesNativeCaptureAndSurvivesReload(bool items)
	{
		var f = new MagicCastingFixture();
		var item = Mock.Of<IGameItem>();
		var xml = new XElement("Trigger", new XAttribute("type", "character"), new XElement("MinimumPower", 0),
			new XElement("MaximumPower", 10), new XElement("CanTargetSelf", true), new XElement("TargetFilterProg", 0),
			new XElement("CanTargetItems", items));
		typeof(MagicSpell).GetProperty("Trigger")!.SetValue(f.Spell, SpellTriggerFactory.LoadTrigger(xml, f.Spell));
		f.Actor.Setup(x => x.TargetActorOrCorpse("a garment", It.IsAny<PerceiveIgnoreFlags>())).Returns((ICharacter)null!);
		f.Actor.Setup(x => x.TargetItem("a garment")).Returns(item);
		var result = SpellTargetCapture.Resolve(f.Actor.Object, f.Spell, SpellPower.Standard, new StringStack("a garment"), true);
		Assert.AreEqual(items, result is not null);
		if (items) Assert.AreSame(item, result!.Target);
		var cloned = (CastingTriggerCharacter)f.Spell.Trigger.Clone();
		Assert.AreEqual(items, cloned.CanTargetItems);
		Assert.AreEqual(items ? "perceivable" : "character", cloned.TargetTypes);
	}

	[TestMethod]
	public void CleansedStacks_CannotMergeWithUncleansedStacksInEitherDirection()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var itemProto = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		itemProto.SetupGet(x => x.Gameworld).Returns(world.Object);
		itemProto.SetupGet(x => x.Name).Returns("goods");
		var proto = Prototype(world.Object, true, 6, 6);
		var first = new GameItem(itemProto.Object);
		var second = new GameItem(itemProto.Object);
		ItemPermissionsGameItemComponent Add(GameItem item)
		{
			var components = (List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!;
			components.Add(new StackableGameItemComponent((StackableGameItemComponentProto)null!, item) { Quantity = 3 });
			var permissions = (ItemPermissionsGameItemComponent)proto.CreateNew(item, temporary: true);
			components.Add(permissions);
			return permissions;
		}
		var a = Add(first);
		var b = Add(second);
		Assert.IsTrue(first.CanMerge(second));
		a.RemoveCurse();
		Assert.IsFalse(first.CanMerge(second));
		Assert.IsFalse(second.CanMerge(first));
		b.RemoveCurse();
		Assert.IsTrue(first.CanMerge(second));
	}

	private sealed class TargetCharacter : MudSharp.Character.Character
	{
		private TargetCharacter() : base(null!, null!, true) { }
		public static TargetCharacter Create(IRoom room, IBody body)
		{
			var actor = (TargetCharacter)RuntimeHelpers.GetUninitializedObject(typeof(TargetCharacter));
			actor.Location = room; actor.Body = body;
			return actor;
		}
		public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => thing is not null;
	}

	[TestMethod]
	public void MixedTargeting_ResolvesCarriedItemThroughRealNativeCharacterLookup()
	{
		var f = new MagicCastingFixture();
		var room = new Mock<IRoom>();
		room.Setup(x => x.GetExitKeyword(It.IsAny<string>(), It.IsAny<IPerceiver>())).Returns((IRoomExit)null!);
		var body = new Mock<IBody> { DefaultValue = DefaultValue.Mock };
		var item = new Mock<IGameItem>();
		item.Setup(x => x.HasKeyword("garment", It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		body.SetupGet(x => x.ExternalItems).Returns([item.Object]);
		room.SetupGet(x => x.Perceivables).Returns(Array.Empty<IPerceivable>());
		var actor = TargetCharacter.Create(room.Object, body.Object);
		var trigger = new XElement("Trigger", new XAttribute("type", "character"), new XElement("MinimumPower", 0),
			new XElement("MaximumPower", 10), new XElement("CanTargetSelf", true), new XElement("TargetFilterProg", 0),
			new XElement("CanTargetItems", true));
		typeof(MagicSpell).GetProperty("Trigger")!.SetValue(f.Spell, SpellTriggerFactory.LoadTrigger(trigger, f.Spell));
		Assert.IsNull(actor.TargetLocalItem("garment"), "The item is carried, not on the room floor.");
		Assert.AreSame(item.Object, actor.TargetItem("garment"));
		var result = SpellTargetCapture.Resolve(actor, f.Spell, SpellPower.Standard, new StringStack("garment"), true);
		Assert.IsNotNull(result);
		Assert.AreSame(item.Object, result.Target);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ItemOptIn_CharacterCleansingStillReflectsAndSpendsCapturedWard(bool items)
	{
		var f = new MagicCastingFixture();
		f.ActiveCapabilities.Clear();
		f.Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 3, Name = "Instant", Expression = "0" }, f.World.Object));
		var xml = new XElement("Definition", new XElement("Trigger", new XAttribute("type", "character"),
			new XElement("MinimumPower", 0), new XElement("MaximumPower", 10), new XElement("CanTargetSelf", true),
			new XElement("TargetFilterProg", 0), new XElement("CanTargetItems", items)), new XElement("Costs"),
			new XElement("Effects", new XElement("Effect", new XAttribute("type", "removecurse"), new XElement("IncludeItems", true))),
			new XElement("CasterEffects"), new XElement("Plan"));
		var spell = new MagicSpell(new MudSharp.Models.MagicSpell
		{
			Id = 905, Name = "Lift Hex", MagicSchoolId = 1, CastingTraitDefinitionId = 1, SpellKnownProgId = 1,
			CastingDifficulty = (int)Difficulty.Easy, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			EffectDurationExpressionId = 3, CastingEmote = "casting", FailCastingEmote = "failing", Definition = xml.ToString()
		}, f.World.Object);
		var target = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		target.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		target.SetupGet(x => x.Location).Returns(f.Actor.Object.Location);
		f.Actor.Setup(x => x.CanSee(target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var ward = new Mock<IMagicInterdictionEffect>();
		ward.SetupGet(x => x.Owner).Returns(target.Object);
		ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Reflect);
		ward.Setup(x => x.ShouldInterdict(f.Actor.Object, f.School)).Returns(true);
		var use = new Mock<IMagicInterdictionUse>();
		use.SetupGet(x => x.IsCurrent).Returns(true);
		use.Setup(x => x.TryCommit()).Returns(true);
		var limited = ward.As<IMagicInterdictionUseEffect>();
		limited.SetupGet(x => x.UsesAreLimited).Returns(true);
		limited.Setup(x => x.CaptureUse(It.IsAny<MagicInterdictionContext>())).Returns(use.Object);
		target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		spell.CastSpell(f.Actor.Object, target.Object, SpellPower.Standard);
		use.Verify(x => x.TryCommit(), Times.Once);
		f.Actor.Verify(x => x.RemoveAllEffects<SpellCurseEffect>(It.IsAny<Predicate<SpellCurseEffect>>(), true), Times.Once);
		target.Verify(x => x.RemoveAllEffects<SpellCurseEffect>(It.IsAny<Predicate<SpellCurseEffect>>(), true), Times.Never);
	}
}

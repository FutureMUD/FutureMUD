#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

internal sealed class ItemMagicCastingFixture
{
	internal sealed class RiskRandom(List<string> events) : Random
	{
		public int Draws;
		public double Value = 0.99;
		public override double NextDouble() { ++Draws; events.Add("risk"); return Value; }
	}
	private sealed class Attunement(XElement xml, IMagicSpell spell, Random random) : EtherealItemAttunementEffect(xml, spell)
	{
		protected override Random DestructionRiskRandom => random;
	}
	public static Func<XElement, IMagicSpell, IMagicSpellEffectTemplate> InstallRiskFactory(RiskRandom random)
	{
		var previous = Factories["etherealitemattunement"];
		SpellEffectFactory.RegisterLoadTimeFactory("etherealitemattunement", (xml, spell) => new Attunement(xml, spell, random));
		return previous;
	}
	public static IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>> Factories =>
		(IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>>)typeof(SpellEffectFactory)
			.GetField("_loadTimeFactories", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
	public MagicCastingFixture Cast { get; } = new();
	public List<string> Events { get; } = [];
	public RiskRandom Risk { get; }
	public Mock<IGameItem> Target { get; } = new();
	public Mock<IRoom> Room { get; } = new();
	public List<IEffect> Effects { get; } = [];
	public List<IGameItem> Registry { get; } = [];
	public List<IGameItem> Materials { get; } = [];
	public MagicSpell Spell { get; private set; } = null!;
	public double Weight = 2;
	public bool Deleted;
	public bool Visible = true;
	public int Debits;
	public double Debited;
	public ItemMagicCastingFixture()
	{
		Risk = new(Events);
		Cast.Actor.SetupGet(x => x.Location).Returns(Room.Object);
		Cast.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(() => Visible);
		Room.Setup(x => x.CanGetAccess(It.IsAny<IGameItem>(), Cast.Actor.Object)).Returns(true);
		Target.SetupGet(x => x.Id).Returns(902);
		Target.SetupGet(x => x.Gameworld).Returns(Cast.World.Object);
		Target.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		Target.SetupGet(x => x.Location).Returns(Room.Object);
		Target.SetupGet(x => x.TrueLocations).Returns([Room.Object]);
		Target.SetupGet(x => x.Weight).Returns(() => Weight);
		Target.SetupGet(x => x.Deleted).Returns(() => Deleted);
		Target.SetupGet(x => x.Quantity).Returns(1);
		Target.SetupGet(x => x.Effects).Returns(Effects);
		Target.Setup(x => x.EffectsOfType<IIntrinsicItemMagicEffect>(null)).Returns(() => Effects.OfType<IIntrinsicItemMagicEffect>());
		Target.Setup(x => x.EffectsOfType<IItemPickupAttemptEffect>(null)).Returns(() => Effects.OfType<IItemPickupAttemptEffect>());
		Target.Setup(x => x.EffectsOfType<EtherealItemAffinityEffect>(null)).Returns(() => Effects.OfType<EtherealItemAffinityEffect>());
		Target.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect => { Events.Add("effect"); Effects.Add(effect); });
		Target.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) => Effects.Remove(effect));
		Target.Setup(x => x.Delete()).Callback(() => { Events.Add("destroy"); Deleted = true; });
		Registry.Add(Target.Object);
		Cast.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => Registry));
		Cast.Actor.Setup(x => x.TargetItem("focus")).Returns(Target.Object);
		Cast.Actor.Setup(x => x.TargetActorOrCorpse(It.IsAny<string>(), It.IsAny<PerceiveIgnoreFlags>())).Returns((ICharacter)null!);
		Cast.Body.SetupGet(x => x.HeldItems).Returns(() => Materials);
		Cast.Body.SetupGet(x => x.WieldedItems).Returns([]);
		Cast.Body.SetupGet(x => x.WornItems).Returns([]);
		Cast.Actor.SetupGet(x => x.Inventory).Returns(() => Materials);
		Cast.Actor.Setup(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Returns<IMagicResource, double>((resource, amount) =>
		{
			Events.Add("debit"); ++Debits; Debited += amount;
			if (Cast.Balances[resource] < amount) return false;
			Cast.Balances[resource] -= amount; return true;
		});
		Cast.Skills[1] = 100;
	}
	public void Load(bool glyph, bool material = false)
	{
		var effect = glyph ? "<Effect type='pickuphazard'><DamageSides>4</DamageSides><StunSides>6</StunSides><MinimumDispelGrade>3</MinimumDispelGrade></Effect>" :
			"<Effect type='etherealitemattunement'><PlaneId>9</PlaneId><AutoAdvance>true</AutoAdvance><MassPerGrade>3</MassPerGrade><RiskPercentPerGrade>1</RiskPercentPerGrade></Effect>";
		Spell = Cast.NewSpell(2, glyph ? "Glyph" : "Shadow Passage", effect);
		typeof(MagicSpell).GetProperty(nameof(MagicSpell.Trigger))!.SetValue(Spell,
			SpellTriggerFactory.LoadTrigger(XElement.Parse($"<Trigger type='{(glyph ? "item" : "character")}'><MinimumPower>0</MinimumPower><MaximumPower>10</MaximumPower><CanTargetSelf>true</CanTargetSelf><TargetFilterProg>0</TargetFilterProg><CanTargetItems>true</CanTargetItems></Trigger>"), Spell));
		Assert.IsTrue(Spell.BuildingCommand(Cast.Actor.Object, new StringStack("grades fixture")));
		if (material)
		{
			var tag = Mock.Of<ITag>(x => x.Id == 50 && x.Name == "synthetic component");
			Cast.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => new[] { tag }));
			Spell.InventoryPlanTemplate = new InventoryPlanTemplate(Cast.World.Object,
				InventoryPlanAction.LoadAction(XElement.Parse("<Action state='consumed' tag='50' secondtag='0' quantity='1' carriedonly='true'/>"), Cast.World.Object));
			AddMaterial(50);
		}
		Admit();
	}
	public void Load(MudSharp.Models.MagicSpell model)
	{
		Spell = new MagicSpell(model, Cast.World.Object);
		Cast.Spells.Add(Spell);
		Admit();
	}
	private void Admit()
	{
		Assert.IsTrue(Cast.Earth.BuildingCommand(Cast.Actor.Object, new StringStack($"casting entry add {Spell.Id}")));
		Cast.Store.Write(acquired: new(100, Spell.Id, 7, 1, Cast.Now, "fixture", DateTime.UnixEpoch, 0));
	}
	public Mock<IGameItem> AddMaterial(long tag, int quantity = 1)
	{
		var item = new Mock<IGameItem>();
		var deleted = false;
		item.SetupGet(x => x.Id).Returns(1000 + Registry.Count);
		item.SetupGet(x => x.Gameworld).Returns(Cast.World.Object);
		item.SetupGet(x => x.InInventoryOf).Returns(Cast.Body.Object);
		item.SetupGet(x => x.Quantity).Returns(() => quantity);
		item.SetupGet(x => x.Deleted).Returns(() => deleted);
		item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(candidate => candidate is null || candidate.Id == tag);
		item.Setup(x => x.Delete()).Callback(() => { Events.Add("consume"); deleted = true; Materials.RemoveAll(x => ReferenceEquals(x, item.Object)); });
		if (quantity > 1)
		{
			var stack = new Mock<IStackable>();
			stack.SetupProperty(x => x.Quantity, quantity);
			item.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
			item.SetupGet(x => x.Quantity).Returns(() => stack.Object.Quantity);
		}
		Materials.Add(item.Object); Registry.Add(item.Object); return item;
	}
	public MagicCastingIntent Intent(int grade = 1, string target = "focus", Guid? origin = null) =>
		new(Cast.Actor.Object, Cast.Earth.Id, Spell.Id, grade, false, target, OriginId: origin);
	public MagicCastingResult Invoke(int grade = 1) => Cast.Service.Cast(Intent(grade));
	public void AssertUnpaid(Func<MagicCastingResult> invoke)
	{
		var effects = Effects.ToArray();
		var definitions = effects.Select(x => x.SaveToXml(new Dictionary<IEffect, TimeSpan>()).ToString()).ToArray();
		var materials = Materials.ToArray();
		var quantities = materials.Select(x => x.Quantity).ToArray();
		var balances = Cast.Balances.ToArray();
		var result = invoke();
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId);
		Assert.AreEqual(0, Debits); Assert.AreEqual(0, Cast.Rolls); Assert.AreEqual(0, Risk.Draws);
		Assert.AreEqual(0, Cast.Store.Operations.Count);
		Assert.IsFalse(Events.Contains("consume")); Assert.IsFalse(Deleted);
		CollectionAssert.AreEqual(effects, Effects.ToArray());
		CollectionAssert.AreEqual(definitions, Effects.Select(x => x.SaveToXml(new Dictionary<IEffect, TimeSpan>()).ToString()).ToArray());
		CollectionAssert.AreEqual(materials, Materials.ToArray());
		CollectionAssert.AreEqual(quantities, Materials.Select(x => x.Quantity).ToArray());
		CollectionAssert.AreEquivalent(balances, Cast.Balances.ToArray());
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BackstabArmourRoutingTests
{
	private delegate IDamage PassArmour(IDamage damage, ref List<IWound> wounds);
	private delegate (IDamage, IDamage) PassNaturalArmour(IDamage damage, ItemQuality quality, IMaterial material,
		IHaveWounds owner, ref List<IWound> wounds);

	[TestMethod]
	[DataRow(false, 9.0)]
	[DataRow(true, 11.0)]
	public void BodyDamage_SkipsOnlyWornLayersAndRetainsMagicAndBothNaturalArmours(bool bypass, double expected)
	{
		var world = new Mock<IFuturemud>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.InnerLineFormatLength == 80 && x.LineFormatLength == 80));
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(80);
		actor.SetupGet(x => x.LineFormatLength).Returns(80);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		var race = new Mock<IRace>();
		var part = new Mock<IWear>();
		part.Setup(x => x.CountsAs(part.Object)).Returns(true);
		part.SetupGet(x => x.Bones).Returns([]);
		part.SetupGet(x => x.Organs).Returns([]);
		var material = Mock.Of<ISolid>();
		part.SetupGet(x => x.DefaultMaterial).Returns(material);
		var health = new Mock<IHealthStrategy>();
		race.SetupGet(x => x.DefaultHealthStrategy).Returns(health.Object);
		race.SetupGet(x => x.NaturalArmourQuality).Returns(ItemQuality.Standard);
		race.SetupGet(x => x.NaturalArmourMaterial).Returns(material);
		var body = TestObjectFactory.CreateUninitialized<RuntimeBody>();
		body.Actor = actor.Object;
		actor.SetupGet(x => x.Body).Returns(body);
		typeof(RuntimeBody).GetProperty("Race")!.SetValue(body, race.Object);
		typeof(RuntimeBody).GetProperty("Gameworld")!.SetValue(body, world.Object);
		typeof(PerceivedItem).GetProperty("EffectHandler")!.SetValue(body, new EffectHandler(body));
		SetField(body, "_bodyparts", new HashSet<IBodypart> { part.Object });
		SetField(body, "_wounds", new List<IWound>());
		SetField(body, "_prosthetics", new HashSet<IProsthetic>());
		var stages = new List<string>();
		var worn = new List<(IGameItem Item, IWear Wearloc, IWearlocProfile Profile)>();
		foreach (var label in new[] { "inner", "outer" })
		{
			var item = new Mock<IGameItem>();
			var armour = new Mock<IArmour>();
			armour.SetupGet(x => x.Parent).Returns(item.Object);
			armour.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>(), ref It.Ref<List<IWound>>.IsAny))
				.Returns((PassArmour)((IDamage damage, ref List<IWound> wounds) =>
				{
					stages.Add(label);
					return new Damage(damage) { DamageAmount = damage.DamageAmount - 1.0 };
				}));
			item.Setup(x => x.GetItemType<IArmour>()).Returns(armour.Object);
			worn.Add((item.Object, part.Object, Mock.Of<IWearlocProfile>()));
		}
		SetField(body, "_wornItems", worn);
		var magic = new Mock<IMagicArmour>();
		magic.Setup(x => x.Applies()).Returns(true);
		magic.Setup(x => x.AppliesToPart(part.Object)).Returns(true);
		magic.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>(), ref It.Ref<List<IWound>>.IsAny))
			.Returns((PassArmour)((IDamage damage, ref List<IWound> wounds) =>
			{
				stages.Add("magic");
				return new Damage(damage) { DamageAmount = damage.DamageAmount - 2.0 };
			}));
		actor.Setup(x => x.EffectsOfType<IMagicArmour>(It.IsAny<Predicate<IMagicArmour>>())).Returns([magic.Object]);
		var racialArmour = new Mock<IArmourType>();
		race.SetupGet(x => x.NaturalArmourType).Returns(racialArmour.Object);
		var partArmour = new Mock<IArmourType>();
		part.SetupGet(x => x.NaturalArmourType).Returns(partArmour.Object);
		foreach (var (armour, label, reduction) in new[] { (racialArmour, "racial", 3.0), (partArmour, "bodypart", 4.0) })
		{
			armour.Setup(x => x.AbsorbDamage(It.IsAny<IDamage>(), ItemQuality.Standard, It.IsAny<IMaterial>(),
				actor.Object, ref It.Ref<List<IWound>>.IsAny))
				.Returns((PassNaturalArmour)((IDamage damage, ItemQuality quality, IMaterial mat, IHaveWounds owner, ref List<IWound> wounds) =>
				{
					stages.Add(label);
					var reduced = new Damage(damage) { DamageAmount = damage.DamageAmount - reduction };
					return (reduced, reduced);
				}));
		}
		IDamage? received = null;
		health.Setup(x => x.SufferDamage(actor.Object, It.IsAny<IDamage>(), part.Object))
			.Callback<IHaveWounds, IDamage, IBodypart>((_, damage, _) => received = damage).Returns([]);

		body.PassiveSufferDamage(new Damage { Bodypart = part.Object, DamageAmount = 20.0, BypassWornArmour = bypass }, new StringBuilder()).ToList();

		Assert.IsNotNull(received);
		Assert.AreEqual(expected, received!.DamageAmount);
		CollectionAssert.AreEqual(bypass ? new[] { "magic", "racial", "bodypart" } :
			new[] { "outer", "inner", "magic", "racial", "bodypart" }, stages);
	}

	private static void SetField(object target, string name, object value)
	{
		var type = target.GetType();
		while (type is not null)
		{
			var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (field is not null) { field.SetValue(target, value); return; }
			type = type.BaseType;
		}
		throw new MissingFieldException(name);
	}
}

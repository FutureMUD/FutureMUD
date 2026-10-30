#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Body;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	private static (WeightedFixture Fixture, ChargedMagicDeviceGameItemComponent Device, Mock<IGameItem> Item)
		ReservoirFixture(bool banishment, bool area = false)
	{
		var f = new WeightedFixture(banishment);
		if (area)
		{
			var content = ArmageddonAreaHarmSpellContent.Create(1).Single(x => x.Content.Key == ArmageddonAreaHarmSpellContent.CinderRainKey).Content;
			var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2;
			row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			f.Pair.F.Spells.Clear();
			f.Pair.F.Spells.Add(new MagicSpell(row, f.Pair.F.World.Object));
		}
		var nativeSpell = f.Pair.F.Spells.Single();
		var part = Mock.Of<IBodypart>(x => x.Id == 9001);
		f.Pair.TargetBody.SetupGet(x => x.Bodyparts).Returns([part]);
		f.Pair.TargetBody.SetupGet(x => x.RandomBodypart).Returns(part);
		f.Pair.F.World.SetupGet(x => x.BodypartPrototypes).Returns(MagicCastingFixture.Collection(() => new[] { part }));
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(9500); item.SetupGet(x => x.Name).Returns("test reservoir");
		item.SetupGet(x => x.Gameworld).Returns(f.Pair.F.World.Object); item.SetupGet(x => x.GetObject).Returns(item.Object);
		item.SetupGet(x => x.Deleted).Returns(false); item.SetupGet(x => x.Destroyed).Returns(false);
		item.SetupGet(x => x.InInventoryOf).Returns(f.Pair.F.Body.Object); item.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
		item.SetupGet(x => x.Components).Returns(() => item.Object.GetItemType<IChargedMagicDevice>() is { } component ? [component] : []);
		f.Pair.F.Actor.Setup(x => x.CanSee(item.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		f.Pair.F.Actor.Setup(x => x.CanManipulateItem(item.Object)).Returns((true, string.Empty));
		f.Pair.F.Body.SetupGet(x => x.HeldOrWieldedItems).Returns(() => [item.Object]);
		var model = VancianItemTests.Prototype("ChargedMagicDevice");
		model.Definition = MagicDeviceDefinition.Serialize(MagicDeviceKind.Staff, MagicDeviceRole.Charged,
			MagicDeviceEligibility.Anyone, 1, 60, 0, 0, 0, 0, Difficulty.Normal, Outcome.Pass, [],
			new XElement("Plan", new XElement("Phase")), MagicDeviceMode.EnergyReservoir, nativeSpell.Id, 10, 50, 20,
			false, area ? MagicDeviceDelivery.NativeArea : MagicDeviceDelivery.OtherCharacters,
			new Dictionary<int, int> { [1] = 3 }, area);
		var prototype = new ChargedMagicDeviceGameItemComponentProto(model, f.Pair.F.World.Object);
		var device = new ChargedMagicDeviceGameItemComponent(prototype, item.Object, true);
		item.Setup(x => x.GetItemType<IChargedMagicDevice>()).Returns(device);
		f.Pair.F.Service.DevicePersistence = _ => { };
		return (f, device, item);
	}

	[TestMethod]
	public void ReservoirBanishment_UsesNativeWeightedResistanceAndDebitsOnceWithoutPersonalMana()
	{
		var (f, device, item) = ReservoirFixture(true);
		var balance = f.Pair.F.Balances[f.Pair.F.Resources[0]];
		var result = f.Pair.F.Service.ActivateReservoir(f.Pair.F.Actor.Object, item.Object, "wek");
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreNotSame(f.Pair.Source.Object, f.Pair.TargetLocation);
		Assert.AreSame(f.Pair.Source.Object, f.Pair.CasterLocation);
		Assert.AreEqual(17, device.Energy);
		Assert.AreEqual(balance, f.Pair.F.Balances[f.Pair.F.Resources[0]], "Reservoir activation has no personal spell-resource cost.");
		Assert.AreEqual(0, f.Pair.F.Rolls, "The fixture's native target resistance check is stubbed; the device makes no personal casting check.");
		Assert.AreEqual(0, f.Pair.F.SkillUses);
	}

	[TestMethod]
	public void ReservoirCinderRain_UsesNativeAreaAdmissionsAndDebitsOnceWithoutPersonalMana()
	{
		var (f, device, item) = ReservoirFixture(false, area: true);
		f.Pair.Source.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(RoomOutdoorsType.Outdoors);
		f.Pair.Target.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(f.Pair.TargetEffects.Add);
		f.Pair.Target.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<System.TimeSpan>())).Callback<IEffect, System.TimeSpan>((effect, _) => f.Pair.TargetEffects.Add(effect));
		var balance = f.Pair.F.Balances[f.Pair.F.Resources[0]];
		var result = f.Pair.F.Service.ActivateReservoir(f.Pair.F.Actor.Object, item.Object, "wek");
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(17, device.Energy);
		Assert.AreEqual(balance, f.Pair.F.Balances[f.Pair.F.Resources[0]], "Reservoir activation has no personal spell-resource cost.");
		Assert.AreEqual(0, f.Pair.F.Rolls, "The fixed device outcome does not make a personal casting check.");
		Assert.AreEqual(0, f.Pair.F.SkillUses);
		Assert.IsTrue(f.Pair.TargetEffects.OfType<SpellBurningEffect>().Any(), "The native area payload applies its continuing burn to the selected local target.");
	}
}

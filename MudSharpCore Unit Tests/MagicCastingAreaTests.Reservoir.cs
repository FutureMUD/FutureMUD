#nullable enable

using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicCastingAreaTests
{
	[TestMethod]
	[DataRow(0, false)]
	[DataRow(2, false)]
	[DataRow(2, true)]
	public void ReservoirArea_PaysOnceForAllEligibleTargetsAndNothingForAnEmptyRoom(int count, bool allResist)
	{
		var a = new AreaFixture("roomfireball");
		for (var i = 0; i < count; i++) a.Target(101 + i);
		if (allResist)
		{
			a.Build("resist 3 normal");
			var resistance = new Mock<ICheck>();
			resistance.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
				It.IsAny<MudSharp.Body.Traits.ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(),
				It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorPass)));
			a.F.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(resistance.Object);
		}
		a.F.ActiveCapabilities.Clear();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(400);
		item.SetupGet(x => x.Gameworld).Returns(a.F.World.Object);
		var model = VancianItemTests.Prototype("ChargedMagicDevice");
		model.Definition = MagicDeviceDefinition.Serialize(MagicDeviceKind.Staff, MagicDeviceRole.Charged,
			MagicDeviceEligibility.Anyone, 1, 60, 0, 0, 0, 0, Difficulty.Normal, Outcome.Pass, [a.F.Spell.Id],
			new XElement("Plan", new XElement("Phase")), MagicDeviceMode.EnergyReservoir, a.F.Spell.Id, 10,
			50, 20, false, MagicDeviceDelivery.NativeArea, new Dictionary<int, int> { [3] = 5 });
		var proto = new ChargedMagicDeviceGameItemComponentProto(model, a.F.World.Object);
		var device = new ChargedMagicDeviceGameItemComponent(proto, item.Object, true);
		item.Setup(x => x.GetItemType<IChargedMagicDevice>()).Returns(device);
		a.F.Body.SetupGet(x => x.HeldOrWieldedItems).Returns([item.Object]);
		a.F.Actor.Setup(x => x.CanSee(item.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		a.F.Actor.Setup(x => x.CanManipulateItem(item.Object)).Returns((true, ""));
		a.Service.DevicePersistence = _ => { };
		var balance = a.F.Balances[a.F.Resources[0]];
		var result = a.Service.ActivateReservoir(a.F.Actor.Object, item.Object, "kral");
		Assert.AreEqual(count == 0 ? MagicCastingStatus.Refused : allResist ? MagicCastingStatus.Failed : MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(count == 0 ? 20 : 15, device.Energy);
		Assert.AreEqual(allResist ? 0 : count, a.Damage.Count);
		Assert.IsTrue(a.Damage.All(x => !ReferenceEquals(x.Target, a.F.Actor.Object)));
		Assert.AreEqual(balance, a.F.Balances[a.F.Resources[0]]);
		Assert.AreEqual(0, a.F.Rolls);
	}
}

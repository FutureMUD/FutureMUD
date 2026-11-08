#nullable enable

using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Casting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellMaterialPolicyTests
{
	[TestMethod]
	[DataRow(false, false, false)]
	[DataRow(false, true, true)]
	[DataRow(true, false, false)]
	[DataRow(true, true, true)]
	public void OptionalMaterial_PresenceAndWaiverControlConsumptionWithoutMutatingSharedPlan(bool waived, bool present, bool expectedMaterial)
	{
		var (f, item, material) = Fixture("consumed", present);
		material.OptionalForCasting = true;
		var policy = new Mock<IFutureProg>(); policy.SetupGet(x => x.Id).Returns(99);
		policy.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		policy.Setup(x => x.MatchesParameters(It.IsAny<System.Collections.Generic.IEnumerable<ProgVariableTypes>>())).Returns(true);
		policy.Setup(x => x.ExecuteBool(true, It.IsAny<object[]>())).Returns(!waived);
		var progs = new All<IFutureProg>(); progs.Add(policy.Object); f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		material.CastRequiredProgId = 99;
		var intent = f.Intent(2, false);
		Assert.IsTrue(f.Service.Quote(intent).Allowed);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Service.Cast(intent).Status);
		item.Verify(x => x.Delete(), expectedMaterial && !waived ? Times.Once() : Times.Never());
		Assert.AreEqual(90.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(DesiredItemState.Consumed, material.DesiredState);
		Assert.IsTrue(material.OptionalForCasting);
	}

	[TestMethod]
	public void RetainedMaterial_CastRequiresPresenceButNeverDeletesOrMovesIt()
	{
		var (f, item, material) = Fixture("retained", true);
		Assert.AreEqual(DesiredItemState.Retained, material.DesiredState);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Service.Cast(f.Intent(2, false)).Status);
		item.Verify(x => x.Delete(), Times.Never());
		Assert.AreSame(f.Body.Object, item.Object.InInventoryOf);
		Assert.AreEqual("retained", material.SaveToXml().Attribute("state")!.Value);
		Assert.AreEqual(DesiredItemState.Retained, InventoryPlanAction.LoadAction(material.SaveToXml(), f.World.Object).DesiredState);
	}

	[TestMethod]
	[DataRow("retained", false)]
	[DataRow("consumed", true)]
	public void MissingRetainedMaterialOrMissingPolicy_RefusesBeforePayment(string state, bool present)
	{
		var (f, item, material) = Fixture(state, present);
		if (state == "consumed") { material.CastRequiredProgId = 99; f.World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>()); }
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(2, false)).Status);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Rolls);
		item.Verify(x => x.Delete(), Times.Never());
	}

	private static (MagicCastingFixture F, Mock<IGameItem> Item, InventoryPlanActionConsume Material) Fixture(string state, bool present)
	{
		var f = new MagicCastingFixture();
		var tag = Mock.Of<ITag>(x => x.Id == 1 && x.Name == "test material");
		var tags = new All<ITag>(); tags.Add(tag); f.World.SetupGet(x => x.Tags).Returns(tags);
		var material = (InventoryPlanActionConsume)InventoryPlanAction.LoadAction(XElement.Parse($"<Action state='{state}' tag='1' secondtag='0' quantity='1' carriedonly='true'/>") , f.World.Object);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, material);
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(123);
		item.Setup(x => x.IsA(tag)).Returns(true); item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Body.SetupGet(x => x.HeldItems).Returns(present ? [item.Object] : []);
		f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns(present ? [item.Object] : []);
		f.Skills[1] = 100; f.Acquire(7);
		return (f, item, material);
	}
}

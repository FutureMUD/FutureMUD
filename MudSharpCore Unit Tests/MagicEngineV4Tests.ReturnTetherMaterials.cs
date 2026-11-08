#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	[DataRow(false)] [DataRow(true)]
	public void ReturnTether_OptionalMaterialChangesDeadlineWithoutMakingCastingMandatory(bool present)
	{
		var fixture = new ReturnFixture(); using var clock = RuntimeClock.Push(fixture.Clock);
		var f = fixture.Pair.F; var spell = (MagicSpell)f.Spells.Single();
		var tag = Mock.Of<ITag>(x => x.Id == 888 && x.Name == "duration material");
		var tags = new All<ITag>(); tags.Add(tag); f.World.SetupGet(x => x.Tags).Returns(tags);
		var material = new InventoryPlanActionConsume(XElement.Parse("<Action state='consumed' tag='888' secondtag='0' quantity='1' carriedonly='true' castoptional='true' originalreference='duration'/>") , f.World.Object);
		spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, material);
		var effect = spell.SpellEffects.Single().SaveToXml(); effect.SetElementValue("OptionalDurationComponent", "duration");
		((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(spell)!)[0] = new ReturnTetherEffect(effect, spell);
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(123);
		item.Setup(x => x.IsA(tag)).Returns(true); item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Body.SetupGet(x => x.HeldItems).Returns(present ? [item.Object] : []);
		f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns(present ? [item.Object] : []);
		var result = fixture.Cast(3);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(fixture.Clock.Now.AddSeconds(present ? 270 : 90), fixture.Child.Binding.DeadlineUtc);
		item.Verify(x => x.Delete(), present ? Times.Once() : Times.Never());
	}
}

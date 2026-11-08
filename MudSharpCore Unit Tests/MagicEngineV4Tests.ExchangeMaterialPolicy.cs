#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	public void ExchangePlaces_MaterialExemptionAllowsMissingRetainedFocusWithoutWaivingTravelPayment()
	{
		var pair = new ExchangeFixture(); var f = pair.F; var spell = (MagicSpell)f.Spells.Single();
		var policy = new Mock<IFutureProg>(); policy.SetupGet(x => x.Id).Returns(999);
		policy.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		policy.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		policy.Setup(x => x.ExecuteBool(true, It.IsAny<object[]>())).Returns(false);
		var progs = new All<IFutureProg>(); progs.Add(policy.Object); f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		((InventoryPlanActionConsume)spell.InventoryPlanTemplate.Phases.Single().Actions.Single()).CastRequiredProgId = 999;
		var root = spell.SpellEffects.Single().SaveToXml(); root.SetElementValue("ComponentRequiredProg", 999);
		((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(spell)!)[0] = new ExchangePlacesEffect(root, spell);
		pair.Items.Clear(); f.Body.SetupGet(x => x.DirectItems).Returns([]);
		f.Body.SetupGet(x => x.HeldItems).Returns([]); f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns([]);
		var result = pair.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(2, pair.Moves); Assert.AreEqual(67.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(3, pair.AlterationQuantity); Assert.AreEqual(3, pair.TeleportationQuantity);
	}
}

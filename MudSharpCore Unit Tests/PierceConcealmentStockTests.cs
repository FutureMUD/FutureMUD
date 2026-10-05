#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PierceConcealmentStockTests
{
	[TestMethod]
	public void StockDefinition_ReloadsEditableProfileAndConcreteReportingAdapter()
	{
		var f = new MagicCastingFixture();
		var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(f.Spell, [])!;
		model.Definition = ArmageddonPierceConcealmentStock.Definition(10, 1, 1).ToString();
		var spell = new MagicSpell(model, f.World.Object);
		Assert.AreEqual(ArmageddonPierceConcealmentStock.Key, spell.StockIdentity);
		Assert.AreEqual(30, spell.GradeProfile!.OpeningSkill);
		Assert.AreEqual(7.0, spell.GradeProfile.Efficiency!.MinimumCost);
		Assert.IsTrue(spell.GradeProfile.Practice!.Enabled);
		Assert.IsInstanceOfType(spell.SpellEffects.Single(), typeof(IMagicSpellEffectOperation));
		Assert.AreEqual("detectinvisible", (string)spell.SpellEffects.Single().SaveToXml().Attribute("type")!);
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice difficulty easy")));
		var saved = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(spell, [])!;
		var reload = new MagicSpell(saved, f.World.Object);
		Assert.AreEqual(spell.StockIdentity, reload.StockIdentity);
		Assert.AreEqual(Difficulty.Easy, reload.GradeProfile!.Practice!.Difficulty);
	}

	[DataTestMethod]
	[DataRow(1, 3000.0)]
	[DataRow(7, 21000.0)]
	public void SelectedGrade_BindsSourceFiveHourLifetime(int grade, double seconds)
	{
		var f = new MagicCastingFixture();
		var duration = new MudSharp.Body.Traits.TraitExpression(ArmageddonPierceConcealmentStock.LifetimeSeconds, f.World.Object);
		Assert.AreEqual(seconds, duration.EvaluateWith(f.Actor.Object, values: [("grade", grade)]));
	}
}

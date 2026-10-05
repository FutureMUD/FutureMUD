#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WaterBreathingStockTests
{
	[TestMethod]
	public void StockEligibility_CompilesEditableStandingFilter()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "water_stock_standing_filter", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "target"), Tuple.Create(ProgVariableTypes.Character, "caster")],
			ArmageddonWaterBreathingStock.EligibilitySource);
		Assert.IsTrue(prog.Compile(), "Normal stock construction requires its compiled Standing filter: " + prog.CompileError);
	}

	[TestMethod]
	public void StockDefinition_ReloadsEditableSourceProfileScopeAndLifetime()
	{
		var f = new MagicCastingFixture();
		var water = Mock.Of<ILiquid>(x => x.Id == 51 && x.Name == "Fresh water");
		var salt = Mock.Of<ILiquid>(x => x.Id == 52 && x.Name == "Salt water");
		f.World.SetupGet(x => x.Liquids).Returns(MagicCastingFixture.Collection(() => new[] { water, salt }));
		f.Expressions.RemoveAll(x => x.Id == 2);
		f.Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 2, Name = "Source placeholder", Expression = "0" }, f.World.Object));
		var model = Snapshot(f.Spell);
		model.AppliedEffectsAreExclusive = true;
		model.Definition = ArmageddonWaterBreathingStock.Definition(10, 1, 1, [51, 52]).ToString();
		var spell = new MagicSpell(model, f.World.Object);
		Assert.AreEqual(ArmageddonWaterBreathingStock.Key, spell.StockIdentity);
		Assert.AreEqual(30, spell.GradeProfile!.OpeningSkill);
		Assert.AreEqual(20.0, spell.GradeProfile.Efficiency!.MinimumCost);
		Assert.IsTrue(spell.GradeProfile.Practice!.Enabled);
		Assert.IsFalse(spell.ScrollInscriptionAllowed);
		Assert.AreEqual("0", spell.EffectDurationExpression.OriginalFormulaText);
		Assert.IsInstanceOfType(spell.SpellEffects.Single(), typeof(SourceWaterBreathingEffect));
		Assert.IsInstanceOfType(spell.SpellEffects.Single(), typeof(IMagicSpellEffectOperation));
		Assert.IsNull(spell.LifetimeConfigurationError, spell.LifetimeConfigurationError);
		Assert.AreEqual(new MagicSpellLifetimePolicy(ArmageddonWaterBreathingStock.LifetimeGroup, 600, 36),
			((IMagicSpellEffectLifetimePolicy)spell.SpellEffects.Single()).LifetimePolicy);
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("effect 1 water remove 52")));
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice difficulty easy")));
		var clone = spell.SpellEffects.Single().Clone();
		Assert.IsTrue(XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), clone.SaveToXml()));
		var reload = new MagicSpell(Snapshot(spell), f.World.Object);
		Assert.AreEqual(spell.StockIdentity, reload.StockIdentity);
		Assert.AreEqual(Difficulty.Easy, reload.GradeProfile!.Practice!.Difficulty);
		Assert.IsTrue(XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), reload.SpellEffects.Single().SaveToXml()));
		Assert.AreEqual(51L, (long)reload.SpellEffects.Single().SaveToXml().Element("WaterScope")!.Elements().Single().Attribute("id")!);
	}

	[DataTestMethod, DataRow("empty"), DataRow("duplicate"), DataRow("unavailable"), DataRow("replacement")]
	public void StockConstruction_InvalidExplicitMappingsRefuseBeforeDatabaseWrites(string scenario)
	{
		var f = new MagicCastingFixture();
		var water = Mock.Of<ILiquid>(x => x.Id == 51);
		f.World.SetupGet(x => x.Liquids).Returns(MagicCastingFixture.Collection(() => new[] { water }));
		ILiquid[] selected = scenario switch
		{
			"empty" => [], "duplicate" => [water, water], "unavailable" => [Mock.Of<ILiquid>(x => x.Id == 52)],
			_ => [Mock.Of<ILiquid>(x => x.Id == 51)]
		};
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonWaterBreathingStock.Create(f.World.Object,
			f.School, f.Traits[0], f.Resources[0], selected));
		Assert.AreEqual(1, f.Spells.Count);
		Assert.AreEqual(0, f.Store.Writes);
	}

	private static MudSharp.Models.MagicSpell Snapshot(MagicSpell spell) =>
		(MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(spell, [])!;
}

#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Effects.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeeTheUnbodiedStockTests
{
	[DataTestMethod]
	[DataRow(0L, false), DataRow(1L, true), DataRow(2L, false), DataRow(3L, false), DataRow(4L, false),
	 DataRow(5L, false), DataRow(6L, false), DataRow(7L, false), DataRow(8L, false), DataRow(9L, true),
	 DataRow(10L, true), DataRow(11L, true), DataRow(12L, false), DataRow(13L, true), DataRow(14L, true),
	 DataRow(15L, true), DataRow(16L, true), DataRow(17L, true), DataRow(18L, true), DataRow(19L, true), DataRow(20L, true)]
	public void StockEligibility_SelfMinimumStandingUsesNativePositionIdentities(long id, bool allowed)
	{
		PositionState.SetupPositions();
		var caster = new Mock<ICharacter>();
		caster.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		caster.SetupGet(x => x.GetObject).Returns(caster.Object);
		caster.SetupGet(x => x.PositionState).Returns(PositionState.GetState(id));
		caster.Setup(x => x.GetProperty("incombat")).Returns(new BooleanVariable(false));
		Assert.AreEqual(allowed, Eligibility().ExecuteBool(caster.Object, caster.Object));
	}

	[TestMethod]
	public void StockEligibility_OtherCharacterCombatAndMissingPositionRefuse()
	{
		PositionState.SetupPositions();
		var caster = new Mock<ICharacter>();
		caster.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		caster.SetupGet(x => x.GetObject).Returns(caster.Object);
		caster.SetupGet(x => x.PositionState).Returns(PositionState.GetState(1));
		caster.Setup(x => x.GetProperty("incombat")).Returns(new BooleanVariable(false));
		var other = new Mock<ICharacter>();
		other.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		other.SetupGet(x => x.GetObject).Returns(other.Object);
		var prog = Eligibility();
		Assert.IsTrue(prog.ExecuteBool(caster.Object, caster.Object));
		Assert.IsFalse(prog.ExecuteBool(other.Object, caster.Object));
		caster.Setup(x => x.GetProperty("incombat")).Returns(new BooleanVariable(true));
		Assert.IsFalse(prog.ExecuteBool(caster.Object, caster.Object));
		caster.Setup(x => x.GetProperty("incombat")).Returns(new BooleanVariable(false));
		caster.SetupGet(x => x.PositionState).Returns((IPositionState)null!);
		Assert.IsFalse(prog.ExecuteBool(caster.Object, caster.Object));
	}

	[TestMethod]
	public void StockDefinition_NormalBuildersPersistProfileScopeRanksAndLifetime()
	{
		var f = new MagicCastingFixture(); var (silt, shadow, ranks) = Bindings(f);
		var model = Snapshot(f.Spell); model.AppliedEffectsAreExclusive = true;
		model.Definition = ArmageddonSeeTheUnbodiedStock.Definition(10, 1, 1, silt.Id, shadow.Id, ranks.Select(x => x.Id).ToArray()).ToString();
		var spell = new MagicSpell(model, f.World.Object);
		Assert.AreEqual(ArmageddonSeeTheUnbodiedStock.Key, spell.StockIdentity);
		Assert.AreEqual(30, spell.GradeProfile!.OpeningSkill);
		Assert.AreEqual(7.0, spell.GradeProfile.Efficiency!.MinimumCost);
		Assert.IsTrue(spell.GradeProfile.Practice!.Enabled);
		Assert.IsInstanceOfType(spell.Trigger, typeof(CastingTriggerCharacter));
		Assert.IsInstanceOfType(spell.SpellEffects.Single(), typeof(DetectEtherealEffect));
		Assert.IsInstanceOfType(spell.SpellEffects.Single(), typeof(IMagicSpellEffectOperation));
		Assert.IsNull(spell.LifetimeConfigurationError, spell.LifetimeConfigurationError);
		Assert.AreEqual(new MagicSpellLifetimePolicy(ArmageddonSeeTheUnbodiedStock.LifetimeGroup, 600, 36),
			((IMagicSpellEffectLifetimePolicy)spell.SpellEffects.Single()).LifetimePolicy);
		var action = (InventoryPlanActionConsume)spell.InventoryPlanTemplate.Phases.Single().Actions.Single();
		Assert.AreEqual(1, action.Quantity); Assert.IsTrue(action.CarriedOnly); Assert.IsNull(action.RequiredGrade);
		Assert.AreEqual(ArmageddonSeeTheUnbodiedStock.ComponentReference, action.OriginalReference);
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("effect 1 source component 1 builder.divination")));
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack($"effect 1 source {silt.Id} {shadow.Id} builder.divination")));
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades practice difficulty easy")));
		var clone = spell.SpellEffects.Single().Clone();
		Assert.IsTrue(XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), clone.SaveToXml()));
		var reload = new MagicSpell(Snapshot(spell), f.World.Object);
		Assert.AreEqual(Difficulty.Easy, reload.GradeProfile!.Practice!.Difficulty);
		Assert.IsTrue(XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), reload.SpellEffects.Single().SaveToXml()));
		Assert.IsTrue(XNode.DeepEquals(spell.InventoryPlanTemplate.SaveToXml(), reload.InventoryPlanTemplate.SaveToXml()));
		Assert.AreEqual("builder.divination", reload.InventoryPlanTemplate.Phases.Single().Actions.Single().OriginalReference);
	}

	[DataTestMethod]
	[DataRow(1, 0), DataRow(2, 0), DataRow(3, 0), DataRow(4, 1), DataRow(5, 2), DataRow(6, 3), DataRow(7, 4)]
	public void StockPlan_EachGradeAdmitsExactThresholdAndHigherDescendant(int grade, int minimum)
	{
		var f = new MagicCastingFixture(); var (silt, shadow, ranks) = Bindings(f);
		var xml = ArmageddonSeeTheUnbodiedStock.Definition(10, 1, 1, silt.Id, shadow.Id, ranks.Select(x => x.Id).ToArray());
		var action = new InventoryPlanActionConsume(xml.Element("Plan")!.Element("Phase")!.Element("Action")!, f.World.Object);
		action.BindSelectedGrade(grade);
		for (var rank = 0; rank <= 5; rank++)
		{
			var selected = rank; var item = new Mock<IGameItem>();
			item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
			item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag tag) => tag.Id <= ranks[Math.Min(selected, 4)].Id);
			Assert.AreEqual(rank >= minimum, action.MeetsPersistedSelection(f.Actor.Object, item.Object), $"grade{grade}, rank{rank}");
		}
		Assert.IsFalse(action.MeetsPersistedSelection(f.Actor.Object, Mock.Of<IGameItem>()));
	}

	[DataTestMethod, DataRow(1, 1800.0), DataRow(7, 12600.0)]
	public void StockDuration_SelectedGradeUsesThreeSourceHours(int grade, double seconds)
	{
		var f = new MagicCastingFixture();
		var duration = new MudSharp.Body.Traits.TraitExpression(ArmageddonSeeTheUnbodiedStock.LifetimeSeconds, f.World.Object);
		Assert.AreEqual(seconds, duration.EvaluateWith(f.Actor.Object, values: [("grade", grade)]));
	}

	[DataTestMethod, DataRow("missing terrain"), DataRow("replaced terrain"), DataRow("empty ranks"),
	 DataRow("duplicate ranks"), DataRow("missing rank"), DataRow("replaced rank"), DataRow("broken hierarchy")]
	public void StockConstruction_InvalidNativeBindingsRefuseBeforeDatabaseWrites(string scenario)
	{
		var f = new MagicCastingFixture(); var (silt, shadow, ranks) = Bindings(f);
		if (scenario == "missing terrain") silt = Mock.Of<ITerrain>(x => x.Id == 103);
		if (scenario == "replaced terrain") silt = Mock.Of<ITerrain>(x => x.Id == silt.Id);
		if (scenario == "empty ranks") ranks = [];
		if (scenario == "duplicate ranks") ranks[4] = ranks[3];
		if (scenario == "missing rank") ranks[4] = Mock.Of<ITag>(x => x.Id == 99);
		if (scenario == "replaced rank") ranks[4] = Mock.Of<ITag>(x => x.Id == 5);
		if (scenario == "broken hierarchy") Mock.Get(ranks[4]).Setup(x => x.IsA(ranks[3])).Returns(false);
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonSeeTheUnbodiedStock.Create(f.World.Object,
			f.School, f.Traits[0], f.Resources[0], silt, shadow, ranks));
		Assert.AreEqual(1, f.Spells.Count); Assert.AreEqual(0, f.Store.Writes);
	}

	[TestMethod]
	public void SourceAcquisition_Pierce79RefusesPierce80OpensSee30Cap90WithoutPayment()
	{
		var f = new MagicCastingFixture(); var (silt, shadow, ranks) = Bindings(f);
		var parentModel = Snapshot(f.Spell);
		parentModel.Definition = ArmageddonPierceConcealmentStock.Definition(10, 1, 1).ToString();
		var parent = new MagicSpell(parentModel, f.World.Object);
		var childModel = Snapshot(parent); childModel.Id = 2; childModel.Name = ArmageddonSeeTheUnbodiedStock.Name;
		childModel.CastingTraitDefinitionId = 3; childModel.AppliedEffectsAreExclusive = true;
		childModel.Definition = ArmageddonSeeTheUnbodiedStock.Definition(10, 1, 1, silt.Id, shadow.Id, ranks.Select(x => x.Id).ToArray()).ToString();
		f.Spells.Clear(); f.Spells.Add(parent); f.Spells.Add(new MagicSpell(childModel, f.World.Object));
		f.Skills[1] = 79; f.Skills.Remove(3); f.Capabilities.Clear(); f.ActiveCapabilities.Clear();
		var root = MagicCastingFixture.Admission(1); root.SetAttributeValue("trait", 1);
		var child = MagicCastingFixture.Admission(2); child.SetAttributeValue("trait", 3); child.SetAttributeValue("starting", false);
		child.SetAttributeValue("opening", ArmageddonSeeTheUnbodiedStock.Opening);
		child.SetAttributeValue("rawCap", ArmageddonSeeTheUnbodiedStock.RawCap); child.SetAttributeValue("capRelative", true);
		child.Add(new XElement("Prerequisite", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", 1),
			new XAttribute("grade", 1), new XAttribute("proficiency", ArmageddonSeeTheUnbodiedStock.PrerequisiteRaw)));
		var policy = new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()),
			new XAttribute("enabled", true), new XAttribute("trait", 1), new XAttribute("source", 10),
			new XAttribute("reserve", 11), new XAttribute("passive", true), new XAttribute("startingVersion", 1), root, child);
		var capability = f.NewCapability(90, 1, 11, true, policy); f.ActiveCapabilities.Add(capability); f.Restart();
		var merit = new Mock<IMagicCapabilityMerit>(); merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		merit.SetupGet(x => x.Capabilities).Returns(new[] { capability });
		f.Actor.SetupGet(x => x.Merits).Returns(new IMerit[] { merit.Object });
		Assert.AreEqual(0, capability.CastingConfigurationErrors().Count, string.Join(';', capability.CastingConfigurationErrors()));
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, capability.Id, "source See prerequisite").Allowed);
		Assert.IsNull(f.Store.Acquisition(100, 2)); Assert.IsFalse(f.Skills.ContainsKey(3));
		f.Skills[1] = 80; f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		Assert.AreEqual(1, f.Store.Acquisition(100, 2)!.ControlledGrade); Assert.AreEqual(30.0, f.Skills[3]);
		Assert.AreEqual(90.0, capability.CastingPolicy!.Admissions.Single(x => x.SpellId == 2).RawSkillCap);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses);
		foreach (var balance in f.Balances.Values) Assert.AreEqual(100.0, balance);
		f.Actor.Verify(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
	}

	private static FutureProg Eligibility()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "see_stock_minimum_position", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "target"), Tuple.Create(ProgVariableTypes.Character, "caster")],
			ArmageddonSeeTheUnbodiedStock.EligibilitySource);
		Assert.IsTrue(prog.Compile(), prog.CompileError); return prog;
	}

	private static (ITerrain Silt, ITerrain Shadow, ITag[] Ranks) Bindings(MagicCastingFixture f)
	{
		var silt = Mock.Of<ITerrain>(x => x.Id == 101 && x.Name == "Mapped Silt");
		var shadow = Mock.Of<ITerrain>(x => x.Id == 102 && x.Name == "Mapped Shadow");
		f.World.SetupGet(x => x.Terrains).Returns(MagicCastingFixture.Collection(() => new[] { silt, shadow }));
		var ranks = Enumerable.Range(0, 5).Select(index =>
		{
			var tag = new Mock<ITag>(); tag.SetupGet(x => x.Id).Returns(index + 1); tag.SetupGet(x => x.Name).Returns($"Divination rank{index}");
			tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag other) => other.Id <= index + 1); return tag.Object;
		}).ToArray();
		for (var rank = 1; rank < ranks.Length; rank++) Mock.Get(ranks[rank]).SetupGet(x => x.Parent).Returns(ranks[rank - 1]);
		f.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => ranks));
		return (silt, shadow, ranks);
	}

	private static MudSharp.Models.MagicSpell Snapshot(MagicSpell spell) =>
		(MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(spell, [])!;
}

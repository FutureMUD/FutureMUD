#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class SeeTheUnbodiedSourceSelectionTests
{
	private const string Reference = "see.divination";
	private const string Group = "armageddon.detect_ethereal";

	[DataTestMethod]
	[DataRow(1, 0)] [DataRow(2, 0)] [DataRow(3, 0)] [DataRow(4, 1)]
	[DataRow(5, 2)] [DataRow(6, 3)] [DataRow(7, 4)] [DataRow(7, 5)]
	public void PaidCast_GradeBoundRankConsumesExactlyOneAndPreservesAuthoredPlan(int grade, int rank)
	{
		using var f = new Fixture(rank); var xml = f.Spell.InventoryPlanTemplate.SaveToXml().ToString();
		var plan = f.Spell.InventoryPlanTemplate; var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(2, f.Quantity); Assert.AreEqual(1, f.ConsumptionWrites);
		Assert.AreEqual(100 - 5.0 * grade, f.F.Balances[f.F.Resources[1]]);
		Assert.IsTrue(f.F.Actor.Object.Effects.OfType<SpellDetectEtherealEffect>().Any());
		Assert.AreSame(plan, f.Spell.InventoryPlanTemplate); Assert.AreEqual(xml, plan.SaveToXml().ToString());
		Assert.IsNull(f.Material.ScoutTarget(f.F.Actor.Object), "Authored rank binding must remain unbound.");
	}

	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(7)]
	public void OutsideShadow_MissingOrInsufficientComponentRefusesBeforePayment(int grade)
	{
		using var f = new Fixture(0); if (grade <= 3) f.Items.Clear();
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsNull(result.OperationId);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.ConsumptionWrites);
		Assert.IsFalse(f.F.Store.Operations.Any()); Assert.IsFalse(f.F.Actor.Object.Effects.Any());
	}

	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(7)]
	public void Shadow_ExemptsOnlyDedicatedActionAndDoesNotNeedAComponent(int grade)
	{
		using var f = new Fixture(0); f.CurrentTerrain = f.Shadow; f.Items.Clear();
		var before = f.Spell.InventoryPlanTemplate.SaveToXml().ToString();
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(0, f.ConsumptionWrites); Assert.AreEqual(3, f.Quantity);
		Assert.AreEqual(100 - 5.0 * grade, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(before, f.Spell.InventoryPlanTemplate.SaveToXml().ToString());
	}

	[DataTestMethod]
	[DataRow(false)] [DataRow(true)]
	public void Silt_PrecedesShadowIncludingOverlappingMappings(bool overlap)
	{
		using var f = new Fixture(); f.CurrentTerrain = overlap ? f.Shadow : f.Silt;
		if (overlap) Assert.IsTrue(f.Effect.BuildingCommand(f.F.Actor.Object, new StringStack($"source {f.Shadow.Id} {f.Shadow.Id} {Reference}")));
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsNull(result.OperationId);
		Assert.IsTrue(result.Message.Contains("Silt")); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(0, f.ConsumptionWrites);
	}

	[DataTestMethod]
	[DataRow(false, "BeforePayment")] [DataRow(true, "BeforePayment")]
	[DataRow(false, "PaymentMutated")] [DataRow(true, "PaymentMutated")]
	[DataRow(false, "Committed")] [DataRow(true, "Committed")]
	public void EnvironmentDrift_BothShadowDirectionsRefuseOrQuarantineWithoutGrant(bool startShadow, string stage)
	{
		using var f = new Fixture(); f.CurrentTerrain = startShadow ? f.Shadow : f.Outside;
		f.AddOld(); var old = f.Parents.Single();
		f.F.Checkpoint = checkpoint => { if (checkpoint == stage) f.CurrentTerrain = startShadow ? f.Outside : f.Shadow; };
		var result = f.Cast(1);
		Assert.AreEqual(stage == "BeforePayment" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(stage == "BeforePayment" ? 100.0 : 95.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreSame(old, f.Parents.Single()); Assert.AreEqual(1, old.SpellEffects.Count());
		Assert.AreEqual(!startShadow && stage == "Committed" ? 1 : 0, f.ConsumptionWrites);
		Assert.AreEqual(0, f.F.Samples);
		if (stage == "BeforePayment") Assert.IsNull(result.OperationId);
		else f.AssertColdRetry(result, 95);
	}

	[DataTestMethod]
	[DataRow("custody")] [DataRow("quantity")] [DataRow("rank")]
	[DataRow("plan")] [DataRow("environment")]
	public void PreservedSelector_AfterDebitCannotHideRawMutation(string change)
	{
		using var f = new Fixture(); var armed = false;
		f.Material.PrimaryItemSelector = _ =>
		{
			if (armed)
			{
				armed = false;
				switch (change)
				{
					case "custody": f.Custodian = null; break;
					case "quantity": f.Quantity++; break;
					case "rank": Mock.Get(f.Ranks[4]).SetupGet(x => x.Parent).Returns((ITag?)null); break;
					case "plan": f.Material.Quantity = 2; break;
					case "environment": f.CurrentTerrain = f.Shadow; break;
				}
			}
			return true;
		};
		f.F.Checkpoint = stage => { if (stage == "PaymentMutated") armed = true; };
		var result = f.Cast(7);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(65.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.ConsumptionWrites);
		Assert.IsFalse(f.Parents.Any()); Assert.IsFalse(f.F.Actor.Object.Effects.OfType<SpellDetectEtherealEffect>().Any());
		f.AssertColdRetry(result, 65);
	}

	[DataTestMethod]
	[DataRow("environment")] [DataRow("quantity")]
	public void ScoutingCallbacks_CannotRefreshFrozenEnvironmentOrCandidateQuantity(string change)
	{
		using var f = new Fixture(); var once = true;
		f.Material.PrimaryItemSelector = _ =>
		{
			if (once) { once = false; if (change == "environment") f.CurrentTerrain = f.Shadow; else f.Quantity++; }
			return true;
		};
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsNull(result.OperationId);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.ConsumptionWrites);
	}

	[TestMethod]
	public void NativeRankBindingCallback_CannotRefreshFrozenEnvironmentBranch()
	{
		using var f = new Fixture(); var calls = 0;
		Mock.Get(f.Ranks[1]).Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag tag) =>
		{ if (++calls == 2) f.CurrentTerrain = f.Shadow; return tag.Id <= 2; });
		var result = f.Cast(1);
		Assert.IsTrue(calls >= 2); Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(0, f.ConsumptionWrites); Assert.IsFalse(f.Parents.Any());
	}

	[TestMethod]
	public void NativeConsumedResult_RefillDuringQuantitySetterQuarantinesWithoutGrantOrSecondConsume()
	{
		using var f = new Fixture(); f.RefillOnConsumption = true; f.AddOld(); var old = f.Parents.Single();
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreEqual(1, f.ConsumptionWrites); Assert.AreEqual(3, f.Quantity);
		Assert.AreEqual(95.0, f.F.Balances[f.F.Resources[1]]); Assert.AreSame(old, f.Parents.Single());
		Assert.AreEqual(0, f.F.Samples); f.AssertColdRetry(result, 95);
	}

	[DataTestMethod]
	[DataRow(false)] [DataRow(true)]
	public void WholeItemConsumption_ObservedDisappearanceIsAcceptedButFalseDeletionQuarantines(bool refuseDeletion)
	{
		using var f = new Fixture(); f.Quantity = 1; f.RefuseDeletion = refuseDeletion;
		var result = f.Cast(1);
		Assert.AreEqual(refuseDeletion ? MagicCastingStatus.NeedsReview : MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(95.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(1, f.ConsumptionWrites);
		Assert.AreEqual(!refuseDeletion, f.Deleted);
		Assert.AreEqual(refuseDeletion ? 0 : 1, f.Parents.Count());
		if (refuseDeletion) f.AssertColdRetry(result, 95);
	}

	[TestMethod]
	public void PreservedUnrelatedAction_IsRetainedInShadowAndItsSelectorStillRuns()
	{
		using var f = new Fixture(); f.CurrentTerrain = f.Shadow;
		var calls = 0; var unrelated = new InventoryPlanActionConsume(f.F.World.Object, 1, f.Ranks[0].Id, 0, _ => { calls++; return true; }, null!)
		{ OriginalReference = "unrelated", CarriedOnly = true };
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.F.World.Object, new[] { f.Material, unrelated });
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsTrue(calls > 0); Assert.AreEqual(1, f.ConsumptionWrites); Assert.AreEqual(2, f.Quantity);
		Assert.AreEqual(2, f.Spell.InventoryPlanTemplate.Phases.SelectMany(x => x.Actions).Count());
	}

	[TestMethod]
	public void OtherActionSharingSelectedComponent_RefusesBeforePayment()
	{
		using var f = new Fixture(); var other = new InventoryPlanActionConsume(f.F.World.Object, 1, f.Ranks[0].Id, 0, null!, null!)
		{ OriginalReference = "other", CarriedOnly = true };
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.F.World.Object, new[] { f.Material, other });
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsNull(result.OperationId);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.ConsumptionWrites);
	}

	[TestMethod]
	public void MultipleSourceScopes_ExistingLifetimeReadinessRefusesBeforePayment()
	{
		using var f = new Fixture();
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("effect add detectethereal")));
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("effect 2 lifetime accumulate another.ethereal 600 36")));
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack($"effect 2 source 101 102 {Reference}")));
		Assert.IsFalse(f.Spell.ReadyForGame);
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsNull(result.OperationId);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(0, f.ConsumptionWrites); Assert.IsFalse(f.Parents.Any());
	}

	[TestMethod]
	public void DirectFactory_SourceScopeCannotBypassPreparedMaterialExecution()
	{
		using var f = new Fixture(); var parent = new MagicSpellParent(f.F.Actor.Object, f.Spell, f.F.Actor.Object);
		Assert.ThrowsException<InvalidOperationException>(() => ((IMagicSpellEffectTemplate)f.Effect)
			.GetOrApplyEffect(f.F.Actor.Object, f.F.Actor.Object, default, SpellPower.Weak, parent, []));
		Assert.IsFalse(parent.SpellEffects.Any()); Assert.IsFalse(f.Parents.Any()); Assert.AreEqual(0, f.ConsumptionWrites);
	}

	[TestMethod]
	public void NativeActionOmittedFromPeek_ExplicitlyRefusesInsteadOfDroppingInputReceipt()
	{
		using var f = new Fixture(); var solid = Mock.Of<ISolid>(x => x.Id == 111);
		f.F.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var item = f.Items.Single(); var commodity = Mock.Of<ICommodity>(x => x.Material == solid && x.Parent == item && x.Weight == 10);
		Mock.Get(item).Setup(x => x.GetItemType<ICommodity>()).Returns(commodity);
		var other = new InventoryPlanActionConsumeCommodity(f.F.World.Object, 1, solid, 0, null!, null!) { OriginalReference = "commodity" };
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.F.World.Object, new InventoryPlanAction[] { f.Material, other });
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsTrue(result.Message.Contains("receipts"), result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.ConsumptionWrites);
	}

	[TestMethod]
	public void XmlClone_CannotSilentlyDropAuthoredRuntimeFitnessDelegate()
	{
		using var f = new Fixture();
		var custom = new InventoryPlanActionConsume(f.Material.SaveToXml(), f.F.World.Object) { PrimaryItemFitnessScorer = _ => 100 };
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.F.World.Object, custom);
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsTrue(result.Message.Contains("fitness")); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow("reference")] [DataRow("duplicate")]
	public void DedicatedReference_MissingOrDuplicateRefusesBeforePayment(string change)
	{
		using var f = new Fixture();
		if (change == "reference") Assert.IsTrue(f.Effect.BuildingCommand(f.F.Actor.Object, new StringStack("source 101 102 missing")));
		else f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.F.World.Object, new[] { f.Material, new InventoryPlanActionConsume(f.Material.SaveToXml(), f.F.World.Object) });
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsNull(result.OperationId);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.ConsumptionWrites);
	}

	[TestMethod]
	public void ComponentReferenceBuilder_PreservesPhaseOrderOptionsAndRuntimeSelectors()
	{
		using var f = new Fixture(); Func<IGameItem, bool> primary = _ => true; Func<IGameItem, bool> secondary = _ => false;
		Func<IGameItem, double> fitness = _ => 23;
		var selected = new InventoryPlanActionConsume(f.Material.SaveToXml(), f.F.World.Object)
		{ PrimaryItemSelector = primary, SecondaryItemSelector = secondary, PrimaryItemFitnessScorer = fitness };
		var unrelated = new InventoryPlanActionConsume(f.F.World.Object, 1, f.Ranks[0].Id, 0, secondary, primary)
		{ OriginalReference = "unrelated" };
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.F.World.Object, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[] { selected }), new InventoryPlanPhaseTemplate(2, new[] { unrelated })
		}) { Options = InventoryPlanOptions.DoNotClearHands };
		Assert.IsTrue(f.Effect.BuildingCommand(f.F.Actor.Object, new StringStack("source component 1 updated.reference")));
		var plan = f.Spell.InventoryPlanTemplate; var replacement = (InventoryPlanActionConsume)plan.FirstPhase.Actions.Single();
		Assert.AreEqual("updated.reference", replacement.OriginalReference); Assert.AreNotSame(selected, replacement);
		Assert.AreSame(primary, replacement.PrimaryItemSelector); Assert.AreSame(secondary, replacement.SecondaryItemSelector);
		Assert.AreSame(fitness, replacement.PrimaryItemFitnessScorer); Assert.AreSame(unrelated, plan.Phases.Last().Actions.Single());
		Assert.AreEqual(InventoryPlanOptions.DoNotClearHands, plan.Options); CollectionAssert.AreEqual(new[] { 1, 2 }, plan.Phases.Select(x => x.PhaseNumber).ToArray());
		Assert.IsFalse(f.Effect.BuildingCommand(f.F.Actor.Object, new StringStack("source component 1 unrelated")));
		Assert.AreSame(plan, f.Spell.InventoryPlanTemplate);
	}

	[TestMethod]
	public void ScopeBuilder_CloneReloadAndInvalidXmlPreserveConfiguration()
	{
		using var f = new Fixture(); var original = f.Effect.SaveToXml();
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack($"effect 1 source component 1 {Reference}")));
		Assert.AreEqual(Reference, f.Spell.InventoryPlanTemplate.Phases.Single().Actions.Single().OriginalReference);
		Assert.IsFalse(f.Effect.BuildingCommand(f.F.Actor.Object, new StringStack("source component 1 invalid/reference")));
		Assert.IsTrue(XNode.DeepEquals(original, f.Effect.Clone().SaveToXml()));
		var invalid = new XElement(original); invalid.Element("SourceScope")!.SetAttributeValue("version", 2);
		var loaded = SpellEffectFactory.LoadEffect(invalid, f.Spell);
		Assert.IsNotNull(((IMagicSpellEffectLifetimePolicy)loaded).LifetimePolicyError);
		Assert.IsTrue(XNode.DeepEquals(invalid, loaded.SaveToXml()));
		Assert.IsTrue(f.Effect.BuildingCommand(f.F.Actor.Object, new StringStack("source off")));
		Assert.IsNull(f.Effect.SaveToXml().Element("SourceScope"));
	}

	private sealed class Clock(DateTime now) : TimeProvider
	{
		public override DateTimeOffset GetUtcNow() => new(now);
	}

	private sealed class Fixture : IDisposable
	{
		public MagicCastingFixture F { get; } = new();
		public MagicSpell Spell { get; }
		public DetectEtherealEffect Effect => (DetectEtherealEffect)Spell.SpellEffects.Single();
		public All<ITag> Tags { get; } = new(); public ITag[] Ranks { get; }
		public List<IGameItem> Items { get; } = [];
		public ITerrain Outside { get; } public ITerrain Silt { get; } public ITerrain Shadow { get; }
		public ITerrain CurrentTerrain { get; set; }
		public InventoryPlanActionConsume Material { get; }
		public int Quantity { get; set; } = 3;
		public IBody? Custodian { get; set; }
		public bool Deleted { get; private set; } public bool RefuseDeletion { get; set; }
		public bool RefillOnConsumption { get; set; } public int ConsumptionWrites { get; private set; }
		private readonly IDisposable _time;
		private readonly EffectHandler _handler;
		public IEnumerable<MagicSpellParent> Parents => _handler.Effects.OfType<MagicSpellParent>();
		public Fixture(int rank = 4)
		{
			_time = RuntimeClock.Push(new Clock(F.Now));
			F.World.SetupGet(x => x.EffectScheduler).Returns(new EffectScheduler(F.World.Object));
			_handler = new(F.Actor.Object); F.Actor.SetupGet(x => x.Effects).Returns(() => _handler.Effects);
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect => _handler.AddEffect(effect));
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, duration) => _handler.AddEffect(effect, duration));
			F.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(_handler.RemoveEffect);
			F.Actor.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
				.Returns<Predicate<MagicSpellParent>>(predicate => Parents.Where(x => predicate is null || predicate(x)).ToArray());
			Ranks = Enumerable.Range(0, 6).Select(index =>
			{
				var tag = new Mock<ITag>(); tag.SetupGet(x => x.Id).Returns(index + 1); tag.SetupGet(x => x.Name).Returns($"Divination {index}");
				tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag other) => other.Id <= index + 1); Tags.Add(tag.Object); return tag.Object;
			}).ToArray();
			for (var index = 1; index < Ranks.Length; ++index) Mock.Get(Ranks[index]).SetupGet(x => x.Parent).Returns(Ranks[index - 1]);
			F.World.SetupGet(x => x.Tags).Returns(Tags);
			ITerrain Terrain(long id, string name) => Mock.Of<ITerrain>(x => x.Id == id && x.Name == name &&
				x.TerrainBehaviourString == "land" && x.TerrainLayers == new[] { RoomLayer.GroundLevel } && x.Tags == Array.Empty<ITag>());
			Outside = Terrain(100, "Outside"); Silt = Terrain(101, "Silt"); Shadow = Terrain(102, "Shadow"); CurrentTerrain = Outside;
			F.World.SetupGet(x => x.Terrains).Returns(MagicCastingFixture.Collection(() => new[] { Outside, Silt, Shadow }));
			Mock.Get(F.Actor.Object.Location.CurrentOverlay).SetupGet(x => x.Terrain).Returns(() => CurrentTerrain);
			F.Spells.Remove(F.Spell);
			Spell = F.NewSpell(1, "Source ethereal adapter", $"<Effect type='detectethereal'><LifetimePolicy version='1' mode='accumulate' group='{Group}' unitSeconds='600' maximumUnits='36' retainStrongestGrade='true'/><SourceScope version='1' silt='101' shadow='102' component='{Reference}'/></Effect>");
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("exclusiveeffect")));
			Spell.EffectDurationExpression = new TraitExpression("1800*grade", F.World.Object);
			Material = new InventoryPlanActionConsume(F.World.Object, 1, Ranks[0].Id, 0, null!, null!)
			{ CarriedOnly = true, OriginalReference = Reference }; Material.ConfigureGradeRanks(-3, Ranks.Take(5).ToArray());
			Spell.InventoryPlanTemplate = new InventoryPlanTemplate(F.World.Object, Material);
			var item = new Mock<IGameItem>(); var stack = new Mock<IStackable>(); var prototype = new Mock<IGameItemProto>();
			prototype.SetupGet(x => x.Id).Returns(999); prototype.SetupGet(x => x.Tags).Returns(new[] { Ranks[rank] });
			item.SetupGet(x => x.Id).Returns(123); item.SetupGet(x => x.Prototype).Returns(prototype.Object);
			item.SetupGet(x => x.Tags).Returns(new[] { Ranks[rank] });
			item.SetupGet(x => x.Components).Returns(new[] { stack.Object });
			item.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
			item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag tag) => tag.Id <= rank + 1);
			item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns((IGameItem other) => ReferenceEquals(other, item.Object));
			Custodian = F.Body.Object; item.SetupGet(x => x.InInventoryOf).Returns(() => Custodian);
			item.SetupGet(x => x.Deleted).Returns(() => Deleted);
			item.SetupGet(x => x.Quantity).Returns(() => Quantity);
			item.SetupGet(x => x.DeepItems).Returns(new[] { item.Object });
			stack.SetupGet(x => x.Quantity).Returns(() => Quantity);
			stack.SetupSet(x => x.Quantity = It.IsAny<int>()).Callback<int>(value => { ConsumptionWrites++; Quantity = RefillOnConsumption ? value + 1 : value; });
			item.Setup(x => x.Delete()).Callback(() => { if (!RefuseDeletion) { Deleted = true; Custodian = null; Items.Remove(item.Object); } });
			Items.Add(item.Object); F.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => Items));
			F.Actor.SetupGet(x => x.Inventory).Returns(() => Items); F.Body.SetupGet(x => x.HeldItems).Returns(() => Items);
			F.Body.SetupGet(x => x.HeldOrWieldedItems).Returns(() => Items); F.Body.SetupGet(x => x.WieldedItems).Returns([]); F.Body.SetupGet(x => x.WornItems).Returns([]);
			F.Skills[1] = 100; F.Acquire(7);
		}
		public MagicCastingResult Cast(int grade) => F.Service.Cast(F.Intent(grade, false) with { OriginId = Guid.NewGuid() });
		public void AddOld()
		{
			var parent = new MagicSpellParent(F.Actor.Object, Spell, F.Actor.Object, SpellPower.Weak)
			{ LifetimeState = new(new(Group, 600, 36), 1) };
			var child = new SpellDetectEtherealEffect(F.Actor.Object, parent); parent.AddSpellEffect(child);
			_handler.AddEffect(child); _handler.AddEffect(parent, TimeSpan.FromSeconds(3000));
		}
		public void AssertColdRetry(MagicCastingResult result, double balance)
		{
			var writes = ConsumptionWrites; F.Checkpoint = null; F.Restart();
			var retry = F.Service.Cast(F.Intent(1, false) with { OriginId = result.OperationId });
			Assert.AreEqual(MagicCastingStatus.Refused, retry.Status); Assert.AreEqual(balance, F.Balances[F.Resources[1]]);
			Assert.AreEqual(writes, ConsumptionWrites);
		}
		public void Dispose() => _time.Dispose();
	}
}

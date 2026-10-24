#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4CanonicalUtilityContentTests
{
	[ClassInitialize]
	public static void Initialise(TestContext _) => FutureProgTestBootstrap.EnsureInitialised();
	// These distinct native fixture programs prove exact source/parameter routing. The owning support
	// contribution qualifies the actual Silt, undead/Void/Defiler, Fire and magical-creature predicates.
	private static ArmageddonCanonicalUtilityBindings Bindings() => new(71, 72, 73, 74, [75, 76, 77], 1,
		"return @caster.id > 0\r\n", "return @target.id > 0\r\n", "return @caster.id != 0\r\n", "return @target.id != 0\r\n");
	private sealed class Fixture
	{
		internal readonly MagicCastingFixture F = new();
		internal Fixture()
		{
			var predicate = new Mock<IFutureProg>(); predicate.SetupGet(x => x.Id).Returns(52);
			predicate.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			predicate.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			predicate.Setup(x => x.Compile()).Returns(true);
			F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection<IFutureProg>(() => [predicate.Object]));
			F.World.SetupGet(x => x.SpellOwnedItems).Returns(Mock.Of<ISpellOwnedItemService>());
			F.World.SetupGet(x => x.DefaultHooks).Returns([]);
			var liquids = new All<ILiquid>(); liquids.Add(Mock.Of<ILiquid>(x => x.Id == 71)); liquids.Add(Mock.Of<ILiquid>(x => x.Id == 72));
			F.World.SetupGet(x => x.Liquids).Returns(liquids);
			var planes = new All<IPlane>(); planes.Add(Mock.Of<IPlane>(x => x.Id == 73)); F.World.SetupGet(x => x.Planes).Returns(planes);
			var hover = NativePrototype(74, F.World.Object);
			var wear = TestObjectFactory.CreateUninitialized<WearableGameItemComponentProto>();
			var profile = TestObjectFactory.CreateUninitialized<DirectWearProfile>();
			SetProperty(wear, nameof(wear.DefaultProfile), profile);
			SetField(typeof(WearableGameItemComponentProto), wear, "_profiles", new List<IWearProfile> { profile });
			var light = TestObjectFactory.CreateUninitialized<ProgLightGameItemComponentProto>();
			SetProperty(light, nameof(light.IlluminationProvided), 100.0);
			SetComponents(hover, [TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>(), wear, light]);
			var prototypes = new List<IGameItemProto> { hover };
			foreach (var id in new long[] { 75, 76, 77 })
			{
				var food = TestObjectFactory.CreateUninitialized<FoodGameItemComponentProto>();
				SetProperty(food, nameof(food.Decorator), Mock.Of<IStackDecorator>()); SetProperty(food, nameof(food.Bites), 2.0);
				SetProperty(food, nameof(food.SatiationPoints), 1.0);
				var prototype = NativePrototype(id, F.World.Object); SetComponents(prototype, [TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>(), food]);
				prototypes.Add(prototype);
			}
			var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
			catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => prototypes.SingleOrDefault(x => x.Id == id)!);
			F.World.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
		}
		internal MagicSpell Load(ArmageddonUtilitySpellContent content)
		{
			var cost = content.CostRow(1); cost.Id = 54; var duration = content.DurationRow(1); duration.Id = 55;
			F.Expressions.Add(new TraitExpression(cost, F.World.Object)); F.Expressions.Add(new TraitExpression(duration, F.World.Object));
			var model = content.SpellRow(1, 1, 52); model.Id = 1; model.EffectDurationExpressionId = 55;
			model.Definition = content.BuildDefinition(10, 54, content.EligibilitySource is null ? 0 : 52).ToString(); return new(model, F.World.Object);
		}
	}
	private static void SetProperty(object target, string name, object value) => target.GetType().GetProperty(name)!.SetValue(target, value);
	private static void SetField(Type type, object target, string name, object value) => type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(target, value);
	private static GameItemProto NativePrototype(long id, IFuturemud world)
	{
		var prototype = TestObjectFactory.CreateUninitialized<GameItemProto>(); SetField(typeof(FrameworkItem), prototype, "_id", id);
		SetProperty(prototype, nameof(prototype.Gameworld), world); SetProperty(prototype, nameof(prototype.Status), RevisionStatus.Current);
		prototype.OnLoadProgs = []; return prototype;
	}
	private static void SetComponents(GameItemProto prototype, List<IGameItemComponentProto> components) => SetField(typeof(GameItemProto), prototype, "_components", components);
	[TestMethod]
	public void CanonicalInventory_CostSchoolTargetAndGrades_MatchLockedManifestWithMealRoomException()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Design Documents", "Magic", "Armageddon_Seeder_Completeness_Design_20261011.json"))) directory = directory.Parent;
		Assert.IsNotNull(directory);
		using var inventory = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory!.FullName, "Design Documents", "Magic", "Armageddon_Seeder_Completeness_Design_20261011.json")));
		var entries = inventory.RootElement.GetProperty("entries").EnumerateArray().ToDictionary(x => x.GetProperty("key").GetString()!);
		var contributions = ArmageddonCanonicalUtilityContent.Contributions(Bindings()); Assert.AreEqual(8, contributions.Count);
		foreach (var contribution in contributions)
		{
			var content = contribution.Content; var approved = entries[content.Key];
			Assert.AreEqual(approved.GetProperty("name").GetString(), content.Name);
			Assert.AreEqual(approved.GetProperty("minimum_energy").GetDouble(), content.MinimumEnergy);
			Assert.AreEqual("arm.capability." + approved.GetProperty("native_school").GetString()!.ToLowerInvariant(), contribution.NativeCapabilityKey);
			var definition = content.BuildDefinition(41, 42, content.EligibilitySource is null ? 0 : 43);
			Assert.AreEqual(content.Key, definition.Element("StockIdentity")!.Value);
			Assert.AreEqual(content.Key == "arm.spell.sustain_meal" ? "room" : approved.GetProperty("target_type").GetString(), definition.Element("Trigger")!.Attribute("type")!.Value);
			if (content.Key == "arm.spell.sustain_meal") StringAssert.Contains(content.Description, ArmageddonCanonicalUtilityContent.SustainMealTargetException);
			CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), definition.Descendants("Grade").Select(x => (int)x.Attribute("number")!).ToArray());
			Assert.AreEqual(content.MinimumEnergy + "*grade", content.CostRow(1).Expression);
			Assert.AreEqual("41", definition.Descendants("Cost").Single().Attribute("resource")!.Value);
			Assert.AreEqual("42", definition.Descendants("Cost").Single().Attribute("expression")!.Value);
		}
	}
	[TestMethod]
	[DataRow("sense_enchantment", "detectmagick")] [DataRow("mend_flesh", "heal")]
	[DataRow("draw_water", "createliquid")] [DataRow("hovering_light", "createitem")]
	[DataRow("draw_wine", "createliquid")] [DataRow("sustain_meal", "createitem")]
	[DataRow("pierce_concealment", "detectinvisible")] [DataRow("apex_bane", "damage")]
	public void WholeNativeSpell_ReadyColdRoundtripAndFactories_PreserveCanonicalConfiguration(string suffix, string adapter)
	{
		var f = new Fixture(); var content = ArmageddonCanonicalUtilityContent.Contributions(Bindings()).Single(x => x.Content.Key == "arm.spell." + suffix).Content;
		var spell = f.Load(content); Assert.IsTrue(spell.ReadyForGame, spell.ReadyForGame ? "" : spell.WhyNotReadyForGame(f.F.Actor.Object));
		Assert.AreEqual(adapter, spell.SpellEffects.Single().SaveToXml().Attribute("type")!.Value);
		var saved = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(spell, [])!;
		var cold = new MagicSpell(saved, f.F.World.Object); Assert.IsTrue(cold.ReadyForGame, cold.ReadyForGame ? "" : cold.WhyNotReadyForGame(f.F.Actor.Object));
		Assert.IsTrue(XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), cold.SpellEffects.Single().SaveToXml()));
		Assert.IsTrue(XNode.DeepEquals(cold.SpellEffects.Single().SaveToXml(), cold.SpellEffects.Single().Clone().SaveToXml()));
		Assert.AreEqual(content.DurationFormula, cold.EffectDurationExpression.OriginalFormulaText);
	}
	[TestMethod]
	public void EligibilitySources_ExactOwnedTextAndTypedParameters_DoNotReuseNameBasedLegacySources()
	{
		var bindings = Bindings();
		foreach (var content in ArmageddonCanonicalUtilityContent.Contributions(bindings).Select(x => x.Content))
		{
			var expected = content.Key switch
			{
				"arm.spell.sense_enchantment" or "arm.spell.pierce_concealment" => bindings.PerceptionEligibilitySource,
				"arm.spell.mend_flesh" => bindings.TargetEligibilitySource,
				"arm.spell.draw_water" or "arm.spell.draw_wine" => bindings.LiquidEligibilitySource,
				"arm.spell.apex_bane" => bindings.MagicalEligibilitySource,
				_ => null
			};
			Assert.AreEqual(expected, content.EligibilitySource);
			if (expected is null) { Assert.IsNull(content.EligibilityRow(1)); continue; }
			var row = content.EligibilityRow(1)!; Assert.AreEqual(expected, row.FunctionText);
			Assert.AreEqual(ProgVariableTypes.Boolean.ToStorageString(), row.ReturnTypeDefinition);
			Assert.AreEqual(content.Key is "arm.spell.draw_water" or "arm.spell.draw_wine" ? ProgVariableTypes.Item.ToStorageString() : ProgVariableTypes.Character.ToStorageString(), row.FutureProgsParameters.Single(x => x.ParameterIndex == 0).ParameterTypeDefinition);
			Assert.AreEqual(ProgVariableTypes.Character.ToStorageString(), row.FutureProgsParameters.Single(x => x.ParameterIndex == 1).ParameterTypeDefinition);
			Assert.ThrowsExactly<ArgumentException>(() => content.BuildDefinition(41, 42, 0));
		}
	}
	[TestMethod]
	public void TypedDependencyAndLifetimeDefinitions_PreserveRealFoodLightLiquidAndBinaryDetectionContracts()
	{
		var contents = ArmageddonCanonicalUtilityContent.Contributions(Bindings()).ToDictionary(x => x.Content.Key, x => x.Content);
		foreach (var suffix in new[] { "draw_water", "draw_wine" })
		{
			var xml = contents["arm.spell." + suffix].BuildDefinition(41, 42, 43);
			Assert.AreEqual(suffix == "draw_water" ? 71L : 72L, (long)xml.Descendants("LiquidId").Single());
			Assert.AreEqual("0.5*grade", xml.Descendants("ContainerFill").Single().Element("Litres")!.Value);
			Assert.AreEqual(73L, (long)xml.Descendants("BonusPlane").Single()); Assert.AreEqual(2.0, (double)xml.Descendants("BonusPlane").Single().Attribute("multiplier")!);
			Assert.AreEqual("0", contents["arm.spell." + suffix].DurationFormula);
		}
		var hover = contents["arm.spell.hovering_light"].BuildDefinition(41, 42, 0);
		Assert.AreEqual(74L, (long)hover.Descendants("ItemPrototypeId").Single()); Assert.AreEqual("wornlight", hover.Descendants("Placement").Single().Value);
		Assert.AreEqual("1800*grade", hover.Descendants("Seconds").Single().Value);
		var meal = contents["arm.spell.sustain_meal"].BuildDefinition(41, 42, 0);
		CollectionAssert.AreEqual(new long[] { 75, 76, 77 }, meal.Descendants("Profile").Single().Elements("Prototype").Select(x => (long)x).ToArray());
		Assert.AreEqual("grade", meal.Descendants("Count").Single().Value); Assert.AreEqual("1350*grade", meal.Descendants("Seconds").Single().Value);
		var pierce = contents["arm.spell.pierce_concealment"].BuildDefinition(41, 42, 43);
		Assert.AreEqual("60*grade", contents["arm.spell.pierce_concealment"].DurationFormula); Assert.IsFalse(pierce.Descendants("LifetimePolicy").Any());
		Assert.IsTrue(ArmageddonReviewedPierceContent.PierceConcealment().BuildDefinition(41, 42, 43).Descendants("LifetimePolicy").Any(), "The legacy accumulated profile remains independently available.");
	}
	[TestMethod]
	[DataRow(1)] [DataRow(7)]
	public void NativeSelectedGradeBindings_HealingDamageAndBinaryDuration_UseLockedBounds(int grade)
	{
		foreach (var suffix in new[] { "mend_flesh", "apex_bane", "pierce_concealment" })
		{
			var f = new Fixture(); var content = ArmageddonCanonicalUtilityContent.Contributions(Bindings()).Single(x => x.Content.Key == "arm.spell." + suffix).Content;
			var spell = f.Load(content); var copy = spell.CastingCopy(f.F.Actor.Object, f.F.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Normal, grade);
			if (suffix == "mend_flesh") Assert.AreEqual(2.0 * grade * grade, ((HealEffect)copy.SpellEffects.Single()).HealingAmount.Evaluate(f.F.Actor.Object));
			else if (suffix == "apex_bane")
			{
				var damage = (DamageEffect)copy.SpellEffects.Single(); Assert.AreEqual(DamageType.Arcane, damage.DamageType);
				Assert.AreEqual(8.0 * grade, damage.DamageExpression.Evaluate(f.F.Actor.Object));
				Assert.AreSame(f.F.Traits[0], spell.OpposedTrait); Assert.AreEqual(Difficulty.Normal, spell.OpposedDifficulty);
			}
			else { Assert.AreEqual(60.0 * grade, copy.EffectDurationExpression.Evaluate(f.F.Actor.Object)); Assert.IsNull(((DetectInvisibleEffect)copy.SpellEffects.Single()).LifetimePolicy); }
		}
	}
	[TestMethod]
	public void InvalidOrMutatedBindings_RefuseMissingDependenciesAndFreezeInputFoodPool()
	{
		var bindings = Bindings(); var foods = new List<long> { 75, 76, 77 };
		var meal = ArmageddonCanonicalUtilityContent.Contributions(bindings with { FoodPrototypeIds = foods }).Single(x => x.Content.Key == "arm.spell.sustain_meal").Content;
		foods[0] = 999; Assert.AreEqual(75L, (long)meal.BuildDefinition(41, 42, 0).Descendants("Profile").Single().Elements("Prototype").First());
		foreach (var invalid in new[] { bindings with { WaterLiquidId = 0 }, bindings with { WineLiquidId = 71 }, bindings with { WaterPlaneId = 0 },
			bindings with { HoveringLightPrototypeId = 0 }, bindings with { CharacterResistanceTraitId = 0 }, bindings with { FoodPrototypeIds = [75, 75, 77] },
			bindings with { FoodPrototypeIds = [75, 76] }, bindings with { PerceptionEligibilitySource = " " }, bindings with { TargetEligibilitySource = "" },
			bindings with { LiquidEligibilitySource = "" }, bindings with { MagicalEligibilitySource = "" } })
			Assert.ThrowsExactly<ArgumentException>(() => ArmageddonCanonicalUtilityContent.Contributions(invalid));
		Assert.ThrowsExactly<ArgumentException>(() => meal.BuildDefinition(0, 42, 0));
	}
}

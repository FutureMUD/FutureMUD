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
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4ExistingFactoryContentTests
{
	private sealed class ItemEffect(XElement root, IMagicSpell spell) : CreateItemEffect(root, spell);
	internal static Dictionary<int, long> Items() => Enumerable.Range(1, 7).ToDictionary(x => x, x => 100L + x);
	internal static ArmageddonUtilitySpellContent[] Contents() =>
	[
		ArmageddonProjectionStock.SandEffigyContent(11, 12), ArmageddonProjectionStock.WalkingShadowContent(11),
		ArmageddonRaiseServitorStock.Content(new(13, 14, 15)), ArmageddonRousedFuryStock.Content(new(16, 17)),
		ArmageddonStillAngerStock.Content(new(18, 19, Enumerable.Range(1, 7).ToDictionary(x => x, _ => Difficulty.Normal))),
		ArmageddonFlameKnifeStock.Content(new(Enumerable.Range(1, 6).Select(x => 200L + x).ToArray(),
			Enumerable.Range(1, 8).Select(x => 300L + x).ToArray(), 20, 21, 22)),
		ArmageddonSandKnifeStock.Content(Items()), ArmageddonStormSpearStock.Content(Items())
	];
	[TestMethod]
	public void Contributions_ApprovedInventoryAndEnergy_RetainExactLockedValues()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Design Documents", "Magic", "Armageddon_Seeder_Completeness_Design_20261011.json"))) directory = directory.Parent;
		Assert.IsNotNull(directory, "The source inventory must be available for exact approved-default verification.");
		using var inventory = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory!.FullName, "Design Documents", "Magic", "Armageddon_Seeder_Completeness_Design_20261011.json")));
		var entries = inventory.RootElement.GetProperty("entries").EnumerateArray().ToDictionary(x => x.GetProperty("key").GetString()!);
		CollectionAssert.AreEquivalent(ArmageddonExistingFactorySpellContent.Keys.ToArray(), Contents().Select(x => x.Key).ToArray());
		Assert.AreEqual(0, ArmageddonExistingFactorySpellContent.PendingKeys.Count);
		Assert.AreEqual("item", entries["arm.spell.sand_effigy"].GetProperty("target_type").GetString());
		var effigy = ArmageddonProjectionStock.SandEffigyContent(11, 12).BuildDefinition(31, 32, 0);
		Assert.AreEqual("item", effigy.Element("Trigger")!.Attribute("type")!.Value);
		var effigyPolicy = SpellProjectionAnchor.ReadPolicy(effigy.Descendants("Policy").Single());
		Assert.IsTrue(effigyPolicy.RequiresHeldSourceItem); Assert.AreEqual(12L, effigyPolicy.EffigyPrototypeId);
		Assert.AreEqual(60.0, effigyPolicy.SecondsPerGrade); Assert.AreEqual(0, effigyPolicy.MaximumRoomDistance);
		foreach (var content in Contents())
		{
			Assert.AreEqual(entries[content.Key].GetProperty("minimum_energy").GetDouble(), content.MinimumEnergy, content.Key);
			Assert.AreEqual(entries[content.Key].GetProperty("name").GetString(), content.Name);
			var definition = content.BuildDefinition(31, 32, 0);
			Assert.AreEqual(content.Key, definition.Element("StockIdentity")!.Value);
			Assert.AreEqual("31", definition.Element("Costs")!.Element("Cost")!.Attribute("resource")!.Value);
			Assert.AreEqual("32", definition.Element("Costs")!.Element("Cost")!.Attribute("expression")!.Value);
			var grades = definition.Element("ControlledPower")!.Elements("Grade").ToArray();
			CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), grades.Select(x => (int)x.Attribute("number")!).ToArray());
			Assert.AreEqual((int)SpellPower.ExtremelyWeak, (int)grades[0].Attribute("power")!);
			Assert.AreNotEqual(1, (int)grades[0].Attribute("power")!, "Native power is not the external requested grade.");
			Assert.AreEqual(content.MinimumEnergy + "*grade", content.CostRow(99).Expression);
		}
	}
	[TestMethod]
	public void WeaponDefinitions_ExactTypedOutputsAndMaterialPolicy_RetainSevenGrades()
	{
		foreach (var content in Contents().Where(x => x.Key is "arm.spell.sand_knife" or "arm.spell.storm_spear"))
		{
			var definition = content.BuildDefinition(31, 32, 0);
			var effect = definition.Element("Effects")!.Element("Effect")!;
			Assert.AreEqual("1", effect.Element("Quantity")!.Value);
			var lifecycle = effect.Element("Lifecycle")!;
			Assert.IsNull(lifecycle.Element("PermanentOutput")); Assert.IsFalse(definition.Element("Plan")!.HasElements);
			foreach (var output in lifecycle.Element("GradeOutputs")!.Elements("Output"))
				Assert.AreEqual(Items()[(int)output.Attribute("grade")!], (long)output.Element("Prototype")!);
			Assert.AreEqual(content.Key == "arm.spell.sand_knife" ? "standard" : "primaryhand", lifecycle.Element("Placement")?.Value ?? "standard");
			Assert.AreEqual(content.Key == "arm.spell.sand_knife" ? ArmageddonExistingFactorySpellContent.WeaponLifetime : ArmageddonExistingFactorySpellContent.StormLifetime, lifecycle.Element("Seconds")!.Value);
		}
		var flame = Contents().Single(x => x.Key == "arm.spell.flame_knife").BuildDefinition(31, 32, 0);
		var plan = flame.Descendants("Action").Single();
		Assert.AreEqual(7, (int)plan.Attribute("grade")!); Assert.AreEqual(20L, (long)plan.Attribute("tag")!);
		Assert.IsTrue((bool)plan.Attribute("carriedonly")!);
		var flameLife = flame.Descendants("Lifecycle").Single();
		Assert.AreEqual(21L, (long)flameLife.Element("EligibilityProg")!); Assert.AreEqual(22L, (long)flameLife.Element("LifetimeMultiplierProg")!);
		Assert.AreEqual(8, flameLife.Element("GradeOutputs")!.Elements("Output").Single(x => (int)x.Attribute("grade")! == 7).Elements("Prototype").Count());
	}
	[TestMethod]
	public void ContentInputs_MissingDuplicateOrMutatedBindings_RefuseOrRemainImmutable()
	{
		var items = Items(); var content = ArmageddonSandKnifeStock.Content(items); items[7] = 999;
		Assert.AreEqual(107L, (long)content.BuildDefinition(31, 32, 0).Descendants("Output").Single(x => (int)x.Attribute("grade")! == 7).Element("Prototype")!);
		items.Remove(1); Assert.ThrowsException<ArgumentException>(() => ArmageddonSandKnifeStock.Content(items));
		items = Items(); items[7] = items[1]; Assert.ThrowsException<ArgumentException>(() => ArmageddonStormSpearStock.Content(items));
		Assert.ThrowsException<ArgumentException>(() => ArmageddonRaiseServitorStock.Content(new(1, 2, 2)));
		Assert.ThrowsException<ArgumentException>(() => ArmageddonProjectionStock.WalkingShadowContent(0));
	}
	[TestMethod]
	public void NativeFactoryEffects_SaveAndClone_RetainCanonicalDefinitionsAndTypedReferences()
	{
		var world = new Mock<IFuturemud>();
		var prototypes = Enumerable.Range(101, 300).Select(id => Mock.Of<IGameItemProto>(x => x.Id == id)).ToArray();
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => prototypes.FirstOrDefault(x => x.Id == id)!);
		world.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
		var ais = new All<IArtificialIntelligence>(); ais.Add(Mock.Of<IArtificialIntelligence>(x => x.Id == 14)); ais.Add(Mock.Of<IArtificialIntelligence>(x => x.Id == 15));
		world.SetupGet(x => x.AIs).Returns(ais);
		var spell = Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object);
		foreach (var content in Contents())
		{
			var root = content.BuildDefinition(31, 32, 0).Element("Effects")!.Element("Effect")!;
			IMagicSpellEffectTemplate native = root.Attribute("type")!.Value switch
			{
				"createitem" => new ItemEffect(root, spell), "createprojection" => new CreateProjectionEffect(root, spell),
				"sourcefury" => new SourceFuryEffect(root, spell), "sourcecalm" => new SourceCalmEffect(root, spell),
				"animatecorpse" => (AnimateCorpseSpellEffect)typeof(AnimateCorpseSpellEffect).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
					null, [typeof(XElement), typeof(IMagicSpell)], null)!.Invoke([root, spell]),
				_ => throw new AssertFailedException("Missing factory for " + content.Key)
			};
			Assert.IsTrue(XNode.DeepEquals(native.SaveToXml(), native.Clone().SaveToXml()), content.Key);
			var payload = root.Element("Lifecycle") ?? root.Element("Policy") ?? root.Element("SourceProfile")!;
			Assert.IsTrue(XNode.DeepEquals(payload, native.SaveToXml().Element(payload.Name)), content.Key);
		}
	}
	[TestMethod]
	public void ServitorDefinition_CorpseDerivedControl_UsesExactNativeAiAndLockedDeadlines()
	{
		var root = ArmageddonRaiseServitorStock.Content(new(13, 14, 15)).BuildDefinition(31, 32, 0);
		Assert.AreEqual("item", root.Element("Trigger")!.Attribute("type")!.Value);
		Assert.IsFalse(root.Descendants("NPCPrototypeId").Any());
		CollectionAssert.AreEqual(new long[] { 14, 15 }, root.Descendants("AI").Select(x => (long)x).ToArray());
		Assert.IsTrue((bool)root.Descendants("AllowFinal").Single()); Assert.IsFalse((bool)root.Descendants("AllowSkeletal").Single());
		Assert.AreEqual("60*grade", root.Descendants("Lifecycle").Single().Element("Seconds")!.Value);
		Assert.AreEqual("60*grade", root.Descendants("Control").Single().Element("Seconds")!.Value);
		Assert.AreEqual(13L, (long)root.Descendants("Control").Single().Element("EligibilityProg")!);
	}
}

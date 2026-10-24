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
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4GradedWeaponContentTests
{
	private sealed class ItemEffect(XElement root, IMagicSpell spell) : CreateItemEffect(root, spell);
	internal static long[] Temporary() => Enumerable.Range(101, 6).Select(x => (long)x).ToArray();
	internal static long[] Staffs() => Enumerable.Range(201, 8).Select(x => (long)x).ToArray();
	internal static ArmageddonUtilitySpellContent[] Contents() =>
	[
		ArmageddonGradedWeaponSpellContent.ShadowBlade(new(Temporary(), Staffs(), 31, 32, 33)),
		ArmageddonGradedWeaponSpellContent.ChampionBlade(Enumerable.Range(1, 7).ToDictionary(x => x, x => 300L + x)),
		ArmageddonGradedWeaponSpellContent.SparkLash(new(Temporary(), Staffs(), 31, 32)),
		ArmageddonGradedWeaponSpellContent.GatherPuddle(34)
	];
	[TestMethod]
	public void Contributions_ApprovedInventoryCostsGradesAndTypedReferences_AgreeWithLockedEntries()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Design Documents", "Magic", "Armageddon_Seeder_Completeness_Design_20261011.json"))) directory = directory.Parent;
		Assert.IsNotNull(directory);
		using var inventory = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory!.FullName, "Design Documents", "Magic", "Armageddon_Seeder_Completeness_Design_20261011.json")));
		var entries = inventory.RootElement.GetProperty("entries").EnumerateArray().ToDictionary(x => x.GetProperty("key").GetString()!);
		CollectionAssert.AreEquivalent(ArmageddonGradedWeaponSpellContent.Keys.ToArray(), Contents().Select(x => x.Key).ToArray());
		foreach (var content in Contents())
		{
			var approved = entries[content.Key]; var definition = content.BuildDefinition(41, 42, 0);
			Assert.AreEqual(approved.GetProperty("minimum_energy").GetDouble(), content.MinimumEnergy, content.Key);
			Assert.AreEqual(approved.GetProperty("name").GetString(), content.Name);
			Assert.AreEqual(approved.GetProperty("target_type").GetString(), definition.Element("Trigger")!.Attribute("type")!.Value);
			Assert.AreEqual(content.Key, definition.Element("StockIdentity")!.Value);
			Assert.AreEqual("41", definition.Descendants("Cost").Single().Attribute("resource")!.Value);
			Assert.AreEqual("42", definition.Descendants("Cost").Single().Attribute("expression")!.Value);
			Assert.AreEqual(content.MinimumEnergy + "*grade", content.CostRow(99).Expression);
			var grades = definition.Descendants("Grade").ToArray();
			CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), grades.Select(x => (int)x.Attribute("number")!).ToArray());
			Assert.AreEqual((int)SpellPower.ExtremelyWeak, (int)grades[0].Attribute("power")!);
			if (content.Key == "arm.spell.gather_puddle")
			{
				Assert.AreEqual(34L, (long)definition.Descendants("LiquidId").Single());
				Assert.AreEqual("0.25*grade", definition.Descendants("GatherPuddle").Single().Element("Litres")!.Value);
				Assert.IsFalse(definition.Descendants("ContainerFill").Any()); continue;
			}
			CollectionAssert.AreEqual(Enumerable.Range(1, 7).Select(x => content.Key.Replace("arm.spell.", "arm.item.") + ".g" + x).ToArray(),
				approved.GetProperty("prototype_dependencies").EnumerateArray().Select(x => x.GetString()!).ToArray());
			Assert.AreEqual(1, (int)definition.Descendants("Quantity").Single());
			var lifecycle = definition.Descendants("Lifecycle").Single();
			if (content.Key == "arm.spell.champion_blade")
			{
				Assert.IsNull(lifecycle.Element("PermanentOutput")); Assert.IsFalse(definition.Element("Plan")!.HasElements);
				Assert.AreEqual("300*grade", lifecycle.Element("Seconds")!.Value);
				continue;
			}
			Assert.AreEqual("1125*(grade+1)", lifecycle.Element("Seconds")!.Value);
			Assert.AreEqual(7, (int)lifecycle.Element("PermanentOutput")!.Attribute("grade")!);
			Assert.AreEqual(32L, (long)lifecycle.Element("EligibilityProg")!);
			Assert.AreEqual(content.Key == "arm.spell.shadow_blade" ? 33L : 0, (long?)lifecycle.Element("LifetimeMultiplierProg") ?? 0);
			var action = definition.Descendants("Action").Single();
			Assert.AreEqual(31L, (long)action.Attribute("tag")!); Assert.AreEqual(7, (int)action.Attribute("grade")!);
			Assert.AreEqual(1, (int)action.Attribute("quantity")!); Assert.IsTrue((bool)action.Attribute("carriedonly")!);
			Assert.AreEqual("consumed", action.Attribute("state")!.Value);
			CollectionAssert.AreEqual(Staffs(), lifecycle.Element("GradeOutputs")!.Elements("Output").Single(x => (int)x.Attribute("grade")! == 7).Elements("Prototype").Select(x => (long)x).ToArray());
		}
	}
	[TestMethod]
	[DataRow(1)] [DataRow(6)] [DataRow(7)]
	public void NativeWeaponOutput_GradeBoundariesAndColdReload_RetainChosenPrototype(int grade)
	{
		var world = new Mock<IFuturemud>(); var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		var prototypes = Temporary().Concat(Staffs()).Concat(Enumerable.Range(301, 7).Select(x => (long)x))
			.Select(id => Mock.Of<IGameItemProto>(x => x.Id == id)).ToArray();
		catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => prototypes.SingleOrDefault(x => x.Id == id)!);
		world.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
		var spell = Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object);
		foreach (var content in Contents().Where(x => x.Key != "arm.spell.gather_puddle"))
		{
			var root = content.BuildDefinition(41, 42, 0).Descendants("Effect").Single();
			var effect = new ItemEffect(root, spell); var loaded = new ItemEffect(effect.SaveToXml(), spell);
			Assert.IsTrue(XNode.DeepEquals(effect.SaveToXml(), loaded.Clone().SaveToXml()));
			var prepare = typeof(CreateItemEffect).GetMethod("PreparePrototype", BindingFlags.Instance | BindingFlags.NonPublic)!;
			var selected = (IGameItemProto)prepare.Invoke(loaded, [grade])!;
			Assert.AreSame(selected, prepare.Invoke(loaded, [grade]), "Repeated admission must retain the same sampled output.");
			if (content.Key == "arm.spell.champion_blade") Assert.AreEqual(300L + grade, selected.Id);
			else if (grade < 7) Assert.AreEqual(100L + grade, selected.Id);
			else { CollectionAssert.Contains(Staffs(), selected.Id); Assert.AreEqual(8, loaded.GradeOutputs[7].Length); }
		}
	}
	[TestMethod]
	public void TypedDependencies_InvalidOrMutatedInput_RefuseOrKeepFrozenContent()
	{
		var temporary = Temporary(); var staffs = Staffs();
		var shadow = ArmageddonGradedWeaponSpellContent.ShadowBlade(new(temporary, staffs, 31, 32, 33));
		temporary[0] = 999; staffs[0] = 998;
		var outputs = shadow.BuildDefinition(41, 42, 0).Descendants("Output").ToArray();
		Assert.AreEqual(101L, (long)outputs[0].Element("Prototype")!); Assert.AreEqual(201L, (long)outputs[^1].Elements("Prototype").First());
		Assert.ThrowsExactly<ArgumentException>(() => ArmageddonGradedWeaponSpellContent.ShadowBlade(new(Temporary(), Staffs(), 31, 32, 0)));
		Assert.ThrowsExactly<ArgumentException>(() => ArmageddonGradedWeaponSpellContent.SparkLash(new(Temporary(), Staffs()[..7], 31, 32)));
		Assert.ThrowsExactly<ArgumentException>(() => ArmageddonGradedWeaponSpellContent.SparkLash(new(Temporary(), Enumerable.Repeat(201L, 8).ToArray(), 31, 32)));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonGradedWeaponSpellContent.GatherPuddle(0));
	}
	[TestMethod]
	[DataRow(1)] [DataRow(6)] [DataRow(7)]
	public void NativeLifetimeBinding_ExternalGradeAndPermanentBoundary_PreserveSourceTiming(int grade)
	{
		var f = new MagicCastingFixture();
		foreach (var content in Contents().Where(x => x.Key != "arm.spell.gather_puddle"))
		{
			var effect = new ItemEffect(content.BuildDefinition(41, 42, 0).Descendants("Effect").Single(), f.Spell);
			var bound = CastingNumerics.Bind(effect.LifetimeExpression!, f.Traits[0], grade, SpellPower.ExtremelyWeak,
				"target[0]/LifetimeSeconds", f.World.Object, grade);
			Assert.AreEqual(content.Key == "arm.spell.champion_blade" ? 300.0 * grade : 1125.0 * (grade + 1), bound.Evaluate(f.Actor.Object));
			Assert.AreEqual(SpellLifecycleMode.TemporaryCleanup, effect.LifecycleMode);
			Assert.AreEqual(content.Key == "arm.spell.champion_blade" ? (int?)null : 7, effect.PermanentGrade);
		}
	}
}

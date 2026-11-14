#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class ArmageddonImportedItemMagicCastingTests
{
	private Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>? _previous;
	[TestCleanup]
	public void RestoreFactory()
	{
		if (_previous is not null) ItemMagicCastingFixture.Factories["etherealitemattunement"] = _previous;
	}
	private ItemMagicCastingFixture Fixture(bool glyph, int grade, bool exempt = false, int rankOffset = 0)
	{
		using var db = ArmageddonWorldfileSpellMaterialPlanTests.CreateFixture(out var writer);
		var f = new ItemMagicCastingFixture();
		_previous = ItemMagicCastingFixture.InstallRiskFactory(f.Risk);
		const long shadowPlane = 9;
		var content = glyph ? ArmageddonOwnedItemMarkSpellContent.Create().Single(x => x.Content.Key == ArmageddonOwnedItemMarkSpellContent.CustodianGlyphKey).Content :
			ArmageddonPlanarSpellContent.Create(shadowPlane).Single(x => x.Content.Key == ArmageddonPlanarSpellContent.ShadowPassageKey).Content;
		var row = content.SpellRow(1, 1, 1); row.Id = 2;
		var cost = content.CostRow(2); cost.Id = 91;
		var duration = content.DurationRow(2); duration.Id = 92;
		row.EffectDurationExpressionId = duration.Id;
		var definition = content.BuildDefinition(10, cost.Id, 0);
		if (!glyph)
		{
			var materialPlan = XElement.Parse(db.MagicSpells.Single(x => x.Name == "shadow_passage").Definition).Element("Plan")!;
			definition.Element("Plan")!.ReplaceWith(new XElement(materialPlan));
			var action = definition.Element("Plan")!.Descendants("Action").Single();
			var policyId = (long)action.Attribute("castrequiredprog")!;
			var policyRow = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == policyId);
			MudSharp.FutureProg.FutureProg.Initialise();
			var policy = new MudSharp.FutureProg.FutureProg(f.Cast.World.Object, policyRow.FunctionName, ProgVariableTypes.Boolean,
				policyRow.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => Tuple.Create(ProgVariableTypes.Character, x.ParameterName)), policyRow.FunctionText);
			typeof(FrameworkItem).GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(policy, policyId);
			var role = new Mock<IChargenRole>(); role.SetupGet(x => x.Id).Returns(9992);
			f.Cast.World.SetupGet(x => x.Roles).Returns(MagicCastingFixture.Collection(() => new[] { role.Object }));
			f.Cast.Actor.Setup(x => x.GetProperty("npc")).Returns(new BooleanVariable(false));
			f.Cast.Actor.Setup(x => x.GetProperty("roles")).Returns(new CollectionVariable(Array.Empty<IChargenRole>(), ProgVariableTypes.Role));
			var race = new Mock<IRace>(); race.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(1));
			f.Cast.Actor.Setup(x => x.GetProperty("race")).Returns(race.Object);
			f.Cast.Actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(exempt);
			Assert.IsTrue(policy.Compile(), policy.CompileError);
			Assert.AreEqual(!exempt, policy.ExecuteBool(true, f.Cast.Actor.Object), "Execute the generated ordinary/admin material policy.");
			var progs = f.Cast.World.Object.FutureProgs.Append(policy).ToArray();
			f.Cast.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => progs));
			var tags = db.Tags.ToArray().Select(tag => new Tag(tag, f.Cast.World.Object)).ToArray();
			f.Cast.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection<ITag>(() => tags));
			if (!exempt)
			{
				var rank = Math.Clamp(grade - 1 + rankOffset, 0, 6);
				var tagId = (long)action.Element("GradeRank")!.Elements("Rank").Single(x => (int)x.Attribute("minimum")! == rank).Attribute("tag")!;
				f.AddMaterial(tagId, 3);
			}
		}
		row.Definition = definition.ToString();
		var original = XElement.Parse(row.Definition);
		if (glyph)
		{
			db.MagicSpells.Add(row);
			db.SeederManagedRecords.Add(new() { Seeder = "fixture", StableKey = content.Key, EntityType = nameof(Db.MagicSpell), LogicalId = row.Id });
			db.SeederManagedRecords.Remove(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonPlanarSpellContent.ShadowPassageKey));
		}
		else
		{
			var existing = db.MagicSpells.Single(x => x.Name == "shadow_passage");
			// Preserve the existing owned identity while replacing its deliberately skeletal fixture definition.
			row.Id = existing.Id; existing.Definition = row.Definition;
		}
		db.MagicSpells.Add(new Db.MagicSpell { Id = 3, Name = "fixture veil", Definition = "<Definition/>" });
		db.SeederManagedRecords.Add(new() { Seeder = "fixture", StableKey = "arm.spell.unyielding_veil", EntityType = nameof(Db.MagicSpell), LogicalId = 3 });
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallSourceItemMagic", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, [shadowPlane]);
		var imported = glyph ? row.Definition : db.MagicSpells.Single(x => x.Name == "shadow_passage").Definition;
		var importedXml = XElement.Parse(imported);
		foreach (var element in new[] { "Costs", "ControlledPower", "Plan" })
			Assert.IsTrue(XNode.DeepEquals(original.Element(element), importedXml.Element(element)), $"The item override changed {element}.");
		row.Definition = imported;
		f.Cast.Expressions.Add(new TraitExpression(cost, f.Cast.World.Object));
		f.Cast.Expressions.Add(new TraitExpression(duration, f.Cast.World.Object));
		f.Load(row);
		return f;
	}
	[TestMethod]
	[DataRow(true, 1)][DataRow(true, 4)][DataRow(true, 7)]
	[DataRow(false, 1)][DataRow(false, 4)][DataRow(false, 7)]
	public void ImportedSpell_UsesNativeGradeCostAndMaterialPayment(bool glyph, int grade)
	{
		var f = Fixture(glyph, grade);
		var quote = f.Cast.Service.Quote(f.Intent(grade));
		Assert.IsTrue(quote.Allowed, quote.Reason);
		var quotedCost = quote.Invocation!.Costs.Single().Amount;
		Assert.IsTrue(quotedCost > 0); Assert.AreEqual(0, f.Risk.Draws);
		var result = f.Invoke(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits); Assert.AreEqual(quotedCost, f.Debited);
		Assert.AreEqual(100 - quotedCost, f.Cast.Balances[f.Cast.Resources[1]]);
		if (glyph)
		{
			Assert.AreEqual($"{grade}d4", f.Effects.OfType<ItemPickupHazardEffect>().Single().DamageDice);
			Assert.AreEqual(0, f.Materials.Count);
		}
		else
		{
			Assert.IsTrue(f.Effects.OfType<EtherealItemAffinityEffect>().Single().IsReadyForTransition);
			Assert.AreEqual(2, f.Materials.Single().Quantity);
			Assert.AreEqual(MagicCastingStatus.Succeeded, f.Invoke(grade).Status);
			Assert.IsTrue(f.Effects.OfType<EtherealItemAffinityEffect>().Single().IsVisibleToThatPlane);
			Assert.AreEqual(1, f.Materials.Single().Quantity);
			var before = f.Debited;
			Assert.AreEqual(MagicCastingStatus.Refused, f.Invoke(grade).Status);
			Assert.AreEqual(2, f.Risk.Draws); Assert.AreEqual(before, f.Debited);
			Assert.AreEqual(1, f.Materials.Single().Quantity);
		}
	}
	[TestMethod]
	[DataRow("missing")][DataRow("under-ranked")][DataRow("overweight")]
	public void ImportedShadowPassage_InvalidMaterialOrWeight_RefusesBeforePayment(string invalid)
	{
		var f = Fixture(false, 4, rankOffset: invalid == "under-ranked" ? -1 : 0);
		if (invalid == "missing") f.Materials.Clear();
		if (invalid == "overweight") f.Weight = 12000;
		f.AssertUnpaid(() => f.Invoke(4));
	}
	[TestMethod]
	[DataRow(1)][DataRow(2)]
	public void ImportedShadowPassage_TargetWouldPayItsOwnComponentCost_RefusesWithoutPayment(int quantity)
	{
		var f = Fixture(false, 1);
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		f.Cast.Body.SetupGet(x => x.Actor).Returns(f.Cast.Actor.Object);
		f.Target.SetupGet(x => x.InInventoryOf).Returns(f.Cast.Body.Object);
		f.Target.SetupGet(x => x.Location).Returns((MudSharp.Construction.IRoom)null!);
		f.Target.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		if (quantity > 1)
		{
			var stack = new Mock<IStackable>(); stack.SetupProperty(x => x.Quantity, quantity);
			f.Target.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
			f.Target.SetupGet(x => x.Quantity).Returns(() => stack.Object.Quantity);
			f.Target.SetupGet(x => x.Weight).Returns(() => stack.Object.Quantity * 1000.0);
		}
		f.AssertUnpaid(() => f.Invoke());
	}
	[TestMethod]
	public void ImportedShadowPassage_MaterialExemption_PaysEnergyWithoutRequiringMaterial()
	{
		var f = Fixture(false, 4, exempt: true);
		var result = f.Invoke(4);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits); Assert.IsTrue(f.Debited > 0);
		Assert.AreEqual(0, f.Materials.Count); Assert.IsFalse(f.Events.Contains("consume"));
	}
}

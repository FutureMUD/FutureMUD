#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FlameKnifeGradeTests
{
	private sealed class OutputEffect(XElement root, MudSharp.Magic.IMagicSpell spell) : CreateItemEffect(root, spell);

	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void CastingService_MonOnlyMaterial_PaysOnceAndConsumesOnlyAtSeven(int grade)
	{
		var f = new MagicCastingFixture();
		var tag = Mock.Of<ITag>(x => x.Id == 1 && x.Name == "Conjuration rank six");
		var tags = new All<ITag>(); tags.Add(tag); f.World.SetupGet(x => x.Tags).Returns(tags);
		var action = new InventoryPlanActionConsume(f.World.Object, 1, tag.Id, 0, null!, null!) { CarriedOnly = true };
		action.ConfigureRequiredGrade(7);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, action);
		var component = new Mock<IGameItem>(); component.SetupGet(x => x.Id).Returns(123);
		component.Setup(x => x.IsA(tag)).Returns(true); component.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Body.SetupGet(x => x.HeldItems).Returns([component.Object]); f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns([component.Object]); f.Skills[1] = 100; f.Acquire(7);
		var intent = f.Intent(grade, false); var quote = f.Service.Quote(intent);
		Assert.IsTrue(quote.Allowed, quote.Reason); component.Verify(x => x.Delete(), Times.Never);
		var result = f.Service.Cast(intent); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		component.Verify(x => x.Delete(), grade == 7 ? Times.Once() : Times.Never());
		Assert.AreEqual(100 - 5.0 * grade, f.Balances[f.Resources[1]]);
		Assert.IsNull(action.ScoutTarget(f.Actor.Object), "Shared grade-conditioned material stays unbound.");
	}

	[TestMethod]
	public void CastingService_MissingMonMaterial_RefusesOnlySevenWithoutPayment()
	{
		var f = new MagicCastingFixture(); var tag = Mock.Of<ITag>(x => x.Id == 1);
		var tags = new All<ITag>(); tags.Add(tag); f.World.SetupGet(x => x.Tags).Returns(tags);
		var action = new InventoryPlanActionConsume(f.World.Object, 1, tag.Id, 0, null!, null!) { CarriedOnly = true }; action.ConfigureRequiredGrade(7);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, action);
		f.Body.SetupGet(x => x.HeldItems).Returns([]); f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]); f.Actor.SetupGet(x => x.Inventory).Returns([]);
		f.Skills[1] = 100; f.Acquire(7);
		Assert.IsTrue(f.Service.Quote(f.Intent(6, false)).Allowed);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(7, false)).Status);
		Assert.AreEqual(100, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void MaterialGrade_InvalidReplacement_PreservesPersistedDefinition()
	{
		var world = Mock.Of<IFuturemud>(x => x.Tags == new All<ITag>()); var action = new InventoryPlanActionConsume(world, 1, 0, 0, null!, null!);
		action.ConfigureRequiredGrade(7); var before = action.SaveToXml().ToString();
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => action.ConfigureRequiredGrade(0)); Assert.AreEqual(before, action.SaveToXml().ToString());
		Assert.AreEqual(7, new InventoryPlanActionConsume(action.SaveToXml(), world).RequiredGrade);
		action.ConfigureRequiredGrade(null); Assert.IsNull(action.SaveToXml().Attribute("grade"));
	}

	[TestMethod]
	public void CastingService_UnresolvedMonTag_PreservesIdAndRefusesUnrelatedMaterial()
	{
		var f = new MagicCastingFixture(); f.World.SetupGet(x => x.Tags).Returns(new All<ITag>());
		var xml = XElement.Parse("<Action state='consumed' tag='12345' secondtag='0' quantity='1' carriedonly='true' grade='7'/>");
		var action = new InventoryPlanActionConsume(xml, f.World.Object);
		Assert.AreEqual("12345", action.SaveToXml().Attribute("tag")!.Value);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, action);
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object); item.Setup(x => x.IsA(null!)).Returns(true);
		f.Body.SetupGet(x => x.HeldItems).Returns([item.Object]); f.Actor.SetupGet(x => x.Inventory).Returns([item.Object]); f.Skills[1] = 100; f.Acquire(7);
		Assert.IsFalse(f.Service.Quote(f.Intent(7, false)).Allowed);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(7, false)).Status);
		Assert.AreEqual(100, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); item.Verify(x => x.Delete(), Times.Never);
		Assert.AreEqual("12345", action.SaveToXml().Attribute("tag")!.Value);
	}

	[DataTestMethod]
	[DataRow("<Output grade='0'><Prototype>1</Prototype></Output>")]
	[DataRow("<Output grade='2'><Prototype>1</Prototype><Prototype>1</Prototype></Output>")]
	[DataRow("<Output grade='2'/>")]
	[DataRow("<Output><Prototype>1</Prototype></Output>")]
	[DataRow("<Output grade='2'><Prototype>1</Prototype></Output><Output grade='2'><Prototype>2</Prototype></Output>")]
	public void OutputPools_InvalidPersistedDefinition_RemainsEditableAndRefusesWithoutPayment(string outputs)
	{
		var f = new MagicCastingFixture(); f.Acquire(7);
		var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(f.Spell, [])!;
		var definition = XElement.Parse(model.Definition); var root = Root(outputs); root.SetAttributeValue("type", "createitem");
		definition.Element("Effects")!.ReplaceWith(new XElement("Effects", root)); model.Definition = definition.ToString();
		var loaded = new MagicSpell(model, f.World.Object); var effect = (CreateItemEffect)loaded.SpellEffects.Single();
		Assert.IsTrue(effect.DefinitionError!.StartsWith("Invalid item output policy:"));
		Assert.IsFalse(loaded.ReadyForGame);
		Assert.AreEqual(root.Element("Lifecycle")!.ToString(), effect.SaveToXml().Element("Lifecycle")!.ToString());
		f.Spells.Clear(); f.Spells.Add(loaded);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent(1, false)).Status);
		Assert.AreEqual(100, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
	}

	[DataTestMethod]
	[DataRow("EligibilityProg", "bad")]
	[DataRow("LifetimeMultiplierProg", "999999999999999999999999999")]
	public void OutputPolicies_MalformedProgId_PreservesEditableLifecycle(string field, string value)
	{
		var world = Mock.Of<IFuturemud>(); var spell = Mock.Of<IMagicSpell>(x => x.Gameworld == world);
		var root = Root(""); root.Element("Lifecycle")!.Add(new XElement(field, value));
		var effect = new OutputEffect(root, spell);
		Assert.IsTrue(effect.DefinitionError!.StartsWith("Invalid item output policy:"));
		Assert.AreEqual(root.Element("Lifecycle")!.ToString(), effect.SaveToXml().Element("Lifecycle")!.ToString());
	}

	[TestMethod]
	public void OutputPools_RoundTripAndPreparedSelection_KeepOnePrototypeAcrossRepeatedAdmission()
	{
		var world = new Mock<IFuturemud>(); var protos = new[] { 1, 2, 3 }.Select(id => Mock.Of<IGameItemProto>(x => x.Id == id)).ToArray();
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>(); catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => protos.FirstOrDefault(x => x.Id == id)!);
		world.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
		var spell = Mock.Of<MudSharp.Magic.IMagicSpell>(x => x.Gameworld == world.Object);
		var effect = new OutputEffect(Root("<Output grade='7'><Prototype>2</Prototype><Prototype>3</Prototype></Output>"), spell);
		var reloaded = new OutputEffect(effect.SaveToXml(), spell);
		CollectionAssert.AreEqual(new long[] { 2, 3 }, reloaded.GradeOutputs[7]);
		var exposed = reloaded.GradeOutputs; exposed[7][0] = 999; Assert.AreEqual(2, reloaded.GradeOutputs[7][0]);
		var prepare = typeof(CreateItemEffect).GetMethod("PreparePrototype", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var selected = (IGameItemProto)prepare.Invoke(reloaded, [7])!; Assert.IsTrue(selected.Id is 2 or 3);
		Assert.AreSame(selected, prepare.Invoke(reloaded, [7]));
		var low = new OutputEffect(effect.SaveToXml(), spell); Assert.AreEqual(1, ((IGameItemProto)prepare.Invoke(low, [1])!).Id);
	}

	private static XElement Root(string outputs) => XElement.Parse($"<Effect><ItemQuality>base</ItemQuality><ItemPrototypeId>1</ItemPrototypeId><ItemSkinId>0</ItemSkinId><Quantity>1</Quantity><LoadString/><Lifecycle version='1' mode='TemporaryCleanup'><Family>test</Family><Seconds>20</Seconds><GradeOutputs>{outputs}</GradeOutputs></Lifecycle></Effect>");
}

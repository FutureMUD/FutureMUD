#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.AI;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Magic.Casting;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

public partial class SpellOwnedNpcAdmissionTests
{
	[TestMethod]
	public void NamedChoice_BindsExactTemplateAndRetainedTokenWithoutChangingDefaultOrSharedPlan()
	{
		var (f, spell, templates) = NamedFixture();
		var copy = CopyAtGrade(f, spell, 7);
		var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		Assert.IsTrue(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, [new() { ParameterName = "npc-choice", Item = "named" }], out var error), error);
		Assert.AreSame(templates[101], effect.NPCTemplate);
		var action = copy.InventoryPlanTemplate.Phases.SelectMany(x => x.Actions).Single();
		Assert.AreEqual(DesiredItemState.Retained, action.DesiredState);
		Assert.AreEqual(800, action.DesiredTag.Id);
		Assert.AreEqual(0, spell.InventoryPlanTemplate.Phases.SelectMany(x => x.Actions).Count());
		Assert.AreSame(templates[107], ((CreateNPCEffect)CopyAtGrade(f, spell, 7).SpellEffects.Single()).NPCTemplate);
		Assert.AreEqual(effect.SaveToXml().ToString(), effect.Clone().SaveToXml().ToString());
	}

	[TestMethod]
	public void NamedChoice_LowGradeAndUnknownName_RefuseBeforeCreation()
	{
		var (f, spell, _) = NamedFixture();
		foreach (var (grade, name) in new[] { (5, "named"), (7, "missing") })
		{
			var effect = (CreateNPCEffect)CopyAtGrade(f, spell, grade).SpellEffects.Single();
			Assert.IsFalse(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, [new() { ParameterName = "npc-choice", Item = name }], out var error));
			Assert.IsNotNull(error);
		}
		Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	public void NamedChoice_PermanentCompanionTemplate_RefusesConfiguration()
	{
		var (f, spell, templates) = NamedFixture();
		var ai = (SpellCompanionAI)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(SpellCompanionAI));
		templates[101].ArtificialIntelligences.Add(ai);
		var root = spell.SpellEffects.Single().SaveToXml(); root.Element("Lifecycle")!.SetAttributeValue("mode", "Permanent"); root.Element("GradeTemplates")!.Remove();
		root.SetElementValue("NPCPrototypeId", 107);
		var effect = new CreateNPCEffect(root, spell);
		Assert.IsNotNull(effect.DefinitionError);
		StringAssert.Contains(effect.DefinitionError, "temporary lifecycle");
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	[DataRow(false)] [DataRow(true)]
	public void NamedChoice_MissingTokenOrExistingCreature_RefusesPaidCastingBeforePayment(bool duplicate)
	{
		var (f, spell, templates) = NamedFixture();
		spell.Trigger = new CastingTriggerRoomChoice(spell.Trigger.SaveToXml(), spell);
		f.Store.Write(acquired: f.Store.Acquisition(100, spell.Id)! with { ControlledGrade = 7 });
		f.Body.SetupGet(x => x.HeldItems).Returns([]); f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns([]);
		if (duplicate)
		{
			var existing = Mock.Of<MudSharp.NPC.INPC>(x => x.Template == templates[101]);
			f.World.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => new MudSharp.Character.ICharacter[] { f.Actor.Object, existing }));
		}
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 7, false, "named"));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		StringAssert.Contains(result.Message, duplicate ? "already present" : "component");
		Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	public void NamedChoice_GradeSevenRetainsTokenAndAcceptsItsFixedGradeSixMaterialRequirement()
	{
		var (f, spell, _) = NamedFixture();
		var tags = new All<ITag>(); var tokenTag = f.World.Object.Tags.Get(800); tags.Add(tokenTag);
		for (var rank = 0; rank <= 6; rank++) tags.Add(Mock.Of<ITag>(x => x.Id == 900 + rank));
		f.World.SetupGet(x => x.Tags).Returns(tags);
		var action = new XElement("Action", new XAttribute("state", "consumed"), new XAttribute("tag", 0), new XAttribute("quantity", 1),
			new XAttribute("carriedonly", true), new XAttribute("originalreference", "ritual"),
			new XElement("GradeRank", new XAttribute("selection", "explicit"), new XAttribute("offset", -1),
				Enumerable.Range(0, 7).Select(rank => new XElement("Rank", new XAttribute("minimum", rank), new XAttribute("tag", 900 + rank)))));
		spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, new InventoryPlanActionConsume(action, f.World.Object));
		var root = spell.SpellEffects.Single().SaveToXml(); root.Element("NamedChoices")!.SetAttributeValue("materialReference", "ritual");
		((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(spell)!)[0] = new CreateNPCEffect(root, spell);
		var material = new Mock<IGameItem>(); material.Setup(x => x.IsA(null!)).Returns(true); material.Setup(x => x.IsA(tags.Get(905))).Returns(true);
		material.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Body.SetupGet(x => x.HeldItems).Returns([material.Object]); f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns([material.Object]);
		var copy = CopyAtGrade(f, spell, 7); var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		Assert.IsTrue(effect.TryBindInvocationInputs(f.Actor.Object, f.Actor.Object.Location, [new() { ParameterName = "npc-choice", Item = "named" }], out var error), error);
		var payment = copy.InventoryPlanTemplate.Phases.SelectMany(x => x.Actions).OfType<InventoryPlanActionConsume>().Single(x => x.DesiredState == DesiredItemState.Consumed);
		Assert.AreSame(material.Object, payment.ScoutTarget(f.Actor.Object));
		Assert.AreEqual(1, copy.InventoryPlanTemplate.Phases.SelectMany(x => x.Actions).Count(x => x.DesiredState == DesiredItemState.Retained));
	}

	private static (MagicCastingFixture Fixture, MagicSpell Spell, Dictionary<long, MudSharp.NPC.Templates.SimpleNPCTemplate> Templates) NamedFixture()
	{
		var (f, spell, templates) = GradeTemplateFixture();
		var tags = new All<ITag>(); tags.Add(Mock.Of<ITag>(x => x.Id == 800)); f.World.SetupGet(x => x.Tags).Returns(tags);
		var root = spell.SpellEffects.Single().SaveToXml();
		root.Add(XElement.Parse("<NamedChoices version='1'><Choice name='named' template='101' uniqueTemplate='101' tokenTag='800' minimumGrade='6' materialGrade='6'/></NamedChoices>"));
		((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(spell)!)[0] = new CreateNPCEffect(root, spell);
		return (f, spell, templates);
	}
}

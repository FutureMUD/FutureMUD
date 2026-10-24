#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class SpellOwnedNpcAdmissionTests
{
	[TestMethod]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	[DataRow(4)]
	[DataRow(5)]
	[DataRow(6)]
	[DataRow(7)]
	public void PreparedNpc_RequestedGrade_SelectsExactNativeTemplateAndLifetime(int grade)
	{
		var (f, spell, templates) = GradeTemplateFixture();
		var copy = CopyAtGrade(f, spell, grade);
		var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		var power = spell.GradeProfile!.Grades.Single(x => x.Grade == grade).Power;
		Assert.AreSame(templates[100 + grade], effect.NPCTemplate);
		var token = effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location);
		Assert.IsNotNull(token);
		Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object.Location, OpposedOutcomeDegree.None,
			power, TimeSpan.Zero, out var application, out var error), error);
		Assert.AreSame(templates[100 + grade], application!.GetType().GetProperty("Template")!.GetValue(application));
		Assert.AreEqual(grade, application.GetType().GetProperty("Grade")!.GetValue(application));
		Assert.AreEqual(60.0 * grade, application.GetType().GetProperty("Seconds")!.GetValue(application));
		Assert.AreEqual(7, ((CreateNPCEffect)effect.Clone()).GradeTemplates.Count);
		Assert.AreEqual(effect.SaveToXml().ToString(), effect.Clone().SaveToXml().ToString());
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	public void Cast_UnmappedRequestedGrade_RefusesBeforePaymentWithoutLegacyFallback()
	{
		var (f, spell, _) = GradeTemplateFixture("<GradeTemplates version='1'><Template grade='1'>101</Template></GradeTemplates>");
		var acquisition = f.Store.Acquisition(100, spell.Id);
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		StringAssert.Contains(result.Message, "requested grade 3");
		Assert.IsNull(result.OperationId);
		Assert.AreEqual(acquisition, f.Store.Acquisition(100, spell.Id));
		Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Flushes);
	}

	[TestMethod]
	[DataRow("<GradeTemplates version='2'><Template grade='1'>101</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates xmlns='urn:unsupported' version='1'><Template grade='1'>101</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'/>")]
	[DataRow("<GradeTemplates version='1'><Template grade='1'>101</Template><Template grade='1'>102</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'><Template grade='0'>101</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'><Template grade='8'>101</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'><Template grade='1'>0</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'><Other grade='1'>101</Other></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1' unexpected='true'><Template grade='1'>101</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'><Template grade='1' unexpected='true'>101</Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'><Template grade='1'><Id>101</Id></Template></GradeTemplates>")]
	[DataRow("<GradeTemplates version='1'><Template grade='1'>101</Template></GradeTemplates><GradeTemplates version='1'><Template grade='2'>102</Template></GradeTemplates>")]
	public void Cast_MalformedGradeTemplatePolicy_PreservesXmlAndRefusesWithoutPayment(string mapping)
	{
		var (f, spell, _) = GradeTemplateFixture(mapping);
		var effect = (CreateNPCEffect)spell.SpellEffects.Single();
		Assert.IsNotNull(effect.DefinitionError);
		Assert.AreEqual(effect.SaveToXml().ToString(), effect.Clone().SaveToXml().ToString());
		var expectedSchemas = XElement.Parse("<Schemas>" + mapping + "</Schemas>").Elements().ToArray();
		var savedSchemas = effect.SaveToXml().Elements().Where(x => x.Name.LocalName == "GradeTemplates").ToArray();
		Assert.AreEqual(expectedSchemas.Length, savedSchemas.Length);
		Assert.IsTrue(expectedSchemas.Zip(savedSchemas).All(x => XNode.DeepEquals(x.First, x.Second)));
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Flushes);
	}

	[TestMethod]
	[DataRow("missing")]
	[DataRow("withdrawn")]
	[DataRow("unsupported")]
	public void Cast_UnavailableMappedDependency_RefusesEvenWhenAnotherGradeIsSelected(string reason)
	{
		var (f, spell, templates) = GradeTemplateFixture();
		if (reason == "missing") templates.Remove(107);
		if (reason == "withdrawn") SetProperty(templates[107], "Status", RevisionStatus.UnderDesign);
		if (reason == "unsupported") templates[107].SelectedMerits.Add(Merit<MudSharp.RPG.Merits.Interfaces.IAdditionalBodyFormMerit>(1, MudSharp.RPG.Merits.MeritScope.Character));
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	[DataRow("grade")]
	[DataRow("mapping")]
	[DataRow("registry")]
	public void PreparedNpc_SelectedGradeOrTypedIdentityChange_InvalidatesRawFence(string change)
	{
		var (f, spell, templates) = GradeTemplateFixture();
		var copy = CopyAtGrade(f, spell, 3);
		var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
		Assert.IsTrue(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object.Location, out var error), error);
		if (change == "grade") SetProperty(copy, "InvocationGrade", 4);
		if (change == "mapping") Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("npcgrade 3 107")));
		if (change == "registry")
		{
			var replacement = Template(f.World.Object); SetField(replacement, "_id", 103L); templates[103] = replacement;
		}
		Assert.IsFalse(token.IsCurrent);
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object.Location, out error));
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	[DataRow("npcgrade 3 107")]
	[DataRow("npcgrade clear")]
	public void PreparedNpc_PolicyEditAfterApplicationPreparation_RefusesCreationBeforeMutation(string edit)
	{
		var (f, spell, _) = GradeTemplateFixture();
		var effect = (CreateNPCEffect)CopyAtGrade(f, spell, 3).SpellEffects.Single();
		var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
		Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object.Location, OpposedOutcomeDegree.None,
			spell.GradeProfile!.Grades.Single(x => x.Grade == 3).Power, TimeSpan.Zero, out var application, out var error), error);
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack(edit)));
		Assert.IsFalse(token.IsCurrent);
		var exception = Assert.ThrowsExactly<InvalidOperationException>(() => application!.Create(null!));
		StringAssert.Contains(exception.Message, "changed during group creation");
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[TestMethod]
	public void PreparedNpc_DifferentRequestedGrade_CannotReusePriorTemplateToken()
	{
		var (f, spell, _) = GradeTemplateFixture();
		var first = (CreateNPCEffect)CopyAtGrade(f, spell, 3).SpellEffects.Single();
		var token = first.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
		var same = (CreateNPCEffect)CopyAtGrade(f, spell, 3).SpellEffects.Single();
		var different = (CreateNPCEffect)CopyAtGrade(f, spell, 4).SpellEffects.Single();
		Assert.IsTrue(same.TryReusePreparedSelection(token, f.Actor.Object, f.Actor.Object.Location, out var error), error);
		Assert.IsFalse(different.TryReusePreparedSelection(token, f.Actor.Object, f.Actor.Object.Location, out error));
	}

	private static (MagicCastingFixture Fixture, MagicSpell Spell, Dictionary<long, SimpleNPCTemplate> Templates)
		GradeTemplateFixture(string? schema = null)
	{
		var f = new MagicCastingFixture();
		var legacy = Template(f.World.Object);
		var spell = Configure(f, legacy); PrepareSpatial(f);
		var templates = new Dictionary<long, SimpleNPCTemplate> { [44] = legacy };
		for (var grade = 1; grade <= 7; grade++)
		{
			var template = Template(f.World.Object); SetField(template, "_id", 100L + grade); templates.Add(template.Id, template);
		}
		var registry = new Mock<IUneditableRevisableAll<INPCTemplate>>();
		registry.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => templates.GetValueOrDefault(id));
		registry.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
			.Returns((string name, bool _) => long.TryParse(name, out var id) ? templates.GetValueOrDefault(id) : null);
		f.World.SetupGet(x => x.NpcTemplates).Returns(registry.Object);
		var root = spell.SpellEffects.Single().SaveToXml();
		if (schema is null)
			root.Add(new XElement("GradeTemplates", new XAttribute("version", 1), Enumerable.Range(1, 7)
				.Select(grade => new XElement("Template", new XAttribute("grade", grade), 100 + grade))));
		else root.Add(XElement.Parse("<Schemas>" + schema + "</Schemas>").Elements());
		((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!
			.GetValue(spell)!)[0] = new CreateNPCEffect(root, spell);
		return (f, spell, templates);
	}

	private static MagicSpell CopyAtGrade(MagicCastingFixture f, MagicSpell spell, int grade) => (MagicSpell)typeof(MagicSpell)
		.GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
		.Invoke(spell, [f.Actor.Object, f.Traits[0], grade, spell.GradeProfile!.Grades.Single(x => x.Grade == grade).Power,
			Difficulty.Automatic, grade])!;
}

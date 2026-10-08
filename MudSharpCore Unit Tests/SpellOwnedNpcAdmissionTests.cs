#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Inventory;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public partial class SpellOwnedNpcAdmissionTests
{
	[DataTestMethod]
	[DataRow("direct")]
	[DataRow("role")]
	[DataRow("combo")]
	[DataRow("role-combo")]
	[DataRow("selected-body-combo")]
	public void Cast_UnsupportedEffectiveMerit_RefusesBeforePlanPaymentOrProgress(string source)
	{
		var f = new MagicCastingFixture(); var template = Template(f.World.Object);
		var additional = Merit<IAdditionalBodyFormMerit>(1, MeritScope.Character);
		var combo = Combo(f.World.Object, 2, source == "selected-body-combo" ? MeritScope.Body : MeritScope.Character, additional);
		if (source == "direct") template.SelectedMerits.Add(additional);
		if (source is "combo" or "selected-body-combo") template.SelectedMerits.Add(combo);
		if (source == "role") template.SelectedRoles.Add(Role(additional));
		if (source == "role-combo") template.SelectedRoles.Add(Role(combo));
		var spell = Configure(f, template);
		var acquisition = f.Store.Acquisition(100, spell.Id); var balances = f.Balances.ToArray();
		var plan = new Mock<IInventoryPlanTemplate>(); spell.InventoryPlanTemplate = plan.Object;
		var error = ((CreateNPCEffect)spell.SpellEffects.Single()).DefinitionError;
		StringAssert.Contains(error!, "additional bodies");
		Assert.AreEqual(error, ((CreateNPCEffect)spell.SpellEffects.Single().Clone()).DefinitionError);
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status); Assert.IsNull(result.OperationId);
		StringAssert.Contains(result.Message, "additional bodies");
		Assert.AreEqual(acquisition, f.Store.Acquisition(100, spell.Id));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Store.Opportunities.Count);
		CollectionAssert.AreEqual(balances, f.Balances.ToArray());
		Assert.AreEqual(0, f.Samples); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Flushes);
		plan.Verify(x => x.CreatePlan(It.IsAny<ICharacter>()), Times.Never);
		AssertConstructorGuard(CharacterTemplateMerits.EffectiveCharacterMerits(template.GetCharacterTemplate()), true, error);
		// Repairing the definition restores other casting on the same trait/reserve; there is no receipt quarantine.
		template.SelectedMerits.Clear(); template.SelectedRoles.Clear(); f.Acquire();
		Assert.IsTrue(f.Service.Quote(f.Intent()).Allowed);
	}

	[DataTestMethod]
	[DataRow("withdrawn", "not approved")]
	[DataRow("world", "different gameworld")]
	[DataRow("service", "unavailable")]
	public void Cast_KnownUnavailableNativeTemplate_RefusesBeforePayment(string reason, string diagnostic)
	{
		var f = new MagicCastingFixture(); var template = Template(f.World.Object); var spell = Configure(f, template);
		if (reason == "withdrawn") SetProperty(template, "Status", RevisionStatus.UnderDesign);
		if (reason == "world") SetField(template, "_gameworld", Mock.Of<IFuturemud>());
		if (reason == "service") f.World.SetupGet(x => x.SpellOwnedNpcs).Returns((ISpellOwnedNpcService)null!);
		var acquisition = f.Store.Acquisition(100, spell.Id);
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status); Assert.IsNull(result.OperationId);
		StringAssert.Contains(result.Message, diagnostic);
		Assert.AreEqual(acquisition, f.Store.Acquisition(100, spell.Id));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Store.Opportunities.Count);
		Assert.IsTrue(f.Balances.Values.All(x => x == 100)); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Flushes);
	}

	[TestMethod]
	public void EffectiveMerits_BodyRoleDeduplicationAndOneLevelCombos_PreserveNativeScopeAndOrder()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock }; var template = Template(world.Object);
		var additional = Merit<IAdditionalBodyFormMerit>(1, MeritScope.Character);
		var nested = Combo(world.Object, 2, MeritScope.Character, additional);
		var outer = Combo(world.Object, 3, MeritScope.Character, nested);
		var bodyCombo = Combo(world.Object, 4, MeritScope.Body, additional);
		var ordinary = Merit<ICharacterMerit>(5, MeritScope.Character);
		template.SelectedMerits.Add(outer); template.SelectedMerits.Add(ordinary);
		template.SelectedRoles.Add(Role(bodyCombo, ordinary));
		var effective = CharacterTemplateMerits.EffectiveCharacterMerits(template.GetCharacterTemplate());
		CollectionAssert.AreEqual(new IMerit[] { outer, ordinary, nested }, effective.ToArray());
		Assert.IsNull(NativeNpcCreationEligibility.CharacterTemplateError(template.GetCharacterTemplate()));
		AssertConstructorGuard(effective, true, null);
		// Selecting the nested combo directly is a second, native expansion source and must now refuse.
		template.SelectedMerits.Add(nested);
		StringAssert.Contains(NativeNpcCreationEligibility.CharacterTemplateError(template.GetCharacterTemplate())!, "additional bodies");
	}

	[TestMethod]
	public void ConstructorGuard_ImmediateOrdinaryCreation_DoesNotRejectAdditionalForms()
	{
		AssertConstructorGuard([Merit<IAdditionalBodyFormMerit>(1, MeritScope.Character)], false, null);
	}

	[DataTestMethod]
	[DataRow("account")]
	[DataRow("prosthesis")]
	[DataRow("role-trait")]
	public void Eligibility_KnownUnsupportedCharacterTemplate_RefusesWithoutConstruction(string unsupported)
	{
		var template = new Mock<ICharacterTemplate>();
		template.SetupGet(x => x.SelectedMerits).Returns([]); template.SetupGet(x => x.SelectedRoles).Returns([]);
		template.SetupGet(x => x.SelectedProstheses).Returns([]);
		if (unsupported == "account") template.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.Id == 7));
		if (unsupported == "prosthesis") template.SetupGet(x => x.SelectedProstheses).Returns([Mock.Of<MudSharp.GameItems.IGameItemProto>()]);
		if (unsupported == "role-trait")
		{
			var role = new Mock<IChargenRole>();
			role.SetupGet(x => x.TraitAdjustments).Returns(new Dictionary<MudSharp.Body.Traits.ITraitDefinition, (double amount, bool giveIfMissing)>
				{ [Mock.Of<MudSharp.Body.Traits.ITraitDefinition>()] = (1, true) });
			template.SetupGet(x => x.SelectedRoles).Returns([role.Object]);
		}
		Assert.IsNotNull(NativeNpcCreationEligibility.CharacterTemplateError(template.Object));
	}

	private static MagicSpell Configure(MagicCastingFixture f, SimpleNPCTemplate template)
	{
		var templates = new Mock<IUneditableRevisableAll<INPCTemplate>>(); templates.Setup(x => x.Get(44)).Returns(template);
		f.World.SetupGet(x => x.NpcTemplates).Returns(templates.Object);
		f.World.SetupGet(x => x.SpellOwnedNpcs).Returns(Mock.Of<ISpellOwnedNpcService>());
		var spell = f.NewSpell(2, "Guardian", "<Effect type='createnpc'><NPCPrototypeId>44</NPCPrototypeId><OnLoadProg>0</OnLoadProg><Lifecycle version='1' mode='DeathOnExpiry'><Family>guardian</Family><Seconds>grade*60</Seconds></Lifecycle></Effect>");
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("trigger new room")));
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 2")));
		f.Store.Write(acquired: new(100, 2, 2, spell.GradeProfile!.Version, f.Now, "admission fixture", DateTime.UnixEpoch, 0));
		return spell;
	}

	private static SimpleNPCTemplate Template(IFuturemud world)
	{
		var template = (SimpleNPCTemplate)RuntimeHelpers.GetUninitializedObject(typeof(SimpleNPCTemplate));
		typeof(SimpleNPCTemplate).GetMethod("Initialise", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(template, null);
		foreach (var field in typeof(NPCTemplateBase).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(List<>) && x.GetValue(template) is null))
			field.SetValue(template, Activator.CreateInstance(field.FieldType));
		SetField(template, "_gameworld", world); SetField(template, "_id", 44L);
		SetProperty(template, "Status", RevisionStatus.Current);
		template.SelectedMerits = []; template.SelectedRoles = []; template.SelectedKnowledges = []; template.MissingBodyparts = [];
		return template;
	}

	private static T Merit<T>(long id, MeritScope scope) where T : class, ICharacterMerit =>
		Mock.Of<T>(x => x.Id == id && x.MeritScope == scope);

	private static IChargenRole Role(params IMerit[] merits)
	{
		var role = new Mock<IChargenRole>(); role.SetupGet(x => x.AdditionalMerits).Returns(merits);
		role.SetupGet(x => x.TraitAdjustments).Returns([]); role.SetupGet(x => x.ClanMemberships).Returns([]);
		return role.Object;
	}

	private static ComboMerit Combo(IFuturemud world, long id, MeritScope scope, ICharacterMerit child)
	{
		var combo = new ComboMerit(new MudSharp.Models.Merit { Id = id, Name = "Combo", Type = "Combo", MeritScope = (int)scope,
			Definition = "<Definition/>" }, world);
		SetField(combo, "_characterMerits", new List<ICharacterMerit> { child });
		return combo;
	}

	private static void AssertConstructorGuard(IEnumerable<IMerit> effective, bool deferred, string? expected)
	{
		var actor = (MudSharp.Character.Character)RuntimeHelpers.GetUninitializedObject(typeof(MudSharp.Character.Character));
		SetField(actor, "_merits", effective.ToList()); SetField(actor, "_deferredNativeInitialisation", deferred);
		var guard = typeof(MudSharp.Character.Character).GetMethod("EnsureNativeCreationHasNoAdditionalForms", BindingFlags.NonPublic | BindingFlags.Instance)!;
		if (expected is null) { guard.Invoke(actor, null); return; }
		var exception = Assert.ThrowsException<TargetInvocationException>(() => guard.Invoke(actor, null));
		Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException)); Assert.AreEqual(expected, exception.InnerException!.Message);
	}

	private static void SetProperty(object target, string name, object value) =>
		target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(target, value);

	private static void SetField(object target, string name, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) is { } field)
			{ field.SetValue(target, value); return; }
		throw new MissingFieldException(name);
	}
}

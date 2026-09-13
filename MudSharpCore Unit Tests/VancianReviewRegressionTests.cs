using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.Magic.Vancian;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianReviewRegressionTests
{
	[TestMethod]
	public void WritingCommands_AreAvailableThroughRealPlayerAndNpcCommandTrees()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		items.Book(100); items.Scroll(101);
		var output = new List<string>();
		Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => output.Add(text));
		foreach (var tree in new ActorCommandTree[] { PlayerCommandTree.Instance, NPCCommandTree.Instance, GuideCommandTree.Instance, AdminCommandTree.StandardAdminCommandTree })
		{
			Assert.IsTrue(tree.Commands.TCommands.ContainsKey("spellbook"));
			Assert.IsTrue(tree.Commands.TCommands.ContainsKey("spellscroll"));
		}
		PlayerCommandTree.Instance.Commands.Execute(f.Actor.Object, "spellbook show 100", f.Actor.Object.State, MudSharp.Accounts.PermissionLevel.Player, f.Actor.Object.OutputHandler);
		PlayerCommandTree.Instance.Commands.Execute(f.Actor.Object, "spellscroll show 101", f.Actor.Object.State, MudSharp.Accounts.PermissionLevel.Player, f.Actor.Object.OutputHandler);
		Assert.IsTrue(output.Any(x => x.Contains("Formulae: 0/5")), string.Join("\n", output));
		Assert.IsTrue(output.Any(x => x.Contains("Blank scroll")), string.Join("\n", output));
	}

	internal static (MagicSpell Spell, IMagicResource Resource) PricedSpell(VancianItemTests.ItemFixture items, string formula = "10 + 2 * castinglevel")
	{
		var f = items.F;
		var resource = new Mock<IMagicResource>();
		resource.SetupGet(x => x.Id).Returns(7); resource.SetupGet(x => x.Name).Returns("mana"); resource.SetupGet(x => x.ShortName).Returns("mana");
		var expression = new TraitExpression(new MudSharp.Models.TraitExpression { Id = 9, Name = "cost", Expression = formula }, f.World.Object);
		f.World.SetupGet(x => x.MagicResources).Returns(VancianTestFixture.Collection<IMagicResource>(() => [resource.Object]));
		f.World.SetupGet(x => x.TraitExpressions).Returns(VancianTestFixture.Collection<ITraitExpression>(() => [expression]));
		var model = items.Spell.SnapshotModel(); var definition = XElement.Parse(model.Definition);
		definition.Element("Costs")!.Add(new XElement("Cost", new XAttribute("resource", 7), new XAttribute("expression", 9)));
		model.Definition = definition.ToString();
		var spell = new MagicSpell(model, f.World.Object); f.Spells.RemoveAll(x => x.Id == 2); f.Spells.Add(spell);
		f.Actor.Setup(x => x.CanUseResource(resource.Object, It.IsAny<double>())).Returns(true);
		f.Actor.SetupGet(x => x.InnerLineFormatLength).Returns(100); f.Actor.SetupGet(x => x.LineFormatLength).Returns(120);
		return (spell, resource.Object);
	}

	private static SpellBackedPower Adapter(VancianTestFixture f)
	{
		var permit = f.Prog("candidates", _ => true);
		permit.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		return (SpellBackedPower)MagicPowerFactory.LoadPower(new MudSharp.Models.MagicPower { Id = 9, Name = "Adapter", MagicSchoolId = 1, PowerModel = "spellbacked",
			Definition = $"<Definition><Verb>invoke</Verb><Spell>2</Spell><CanInvokePowerProg>{permit.Object.Id}</CanInvokePowerProg><WhyCantInvokePowerProg>0</WhyCantInvokePowerProg><InvocationCosts/></Definition>" }, f.World.Object);
	}

	[TestMethod]
	public void SpellBackedPower_RemainsIndependentAfterVancianSlotsAreExhausted()
	{
		var items = new VancianItemTests.ItemFixture("<Effect type='staminadelta'><Formula>power</Formula></Effect>"); var f = items.F;
		var (spell, resource) = PricedSpell(items, "5"); f.Count = 1; f.Select(2); f.Plan(2); f.Refresh();
		var adapter = Adapter(f);
		f.Actor.SetupGet(x => x.Powers).Returns([adapter]);
		var check = new Mock<ICheck>(); f.World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(check.Object);
		check.Setup(x => x.CheckAgainstAllDifficulties(f.Actor.Object, It.IsAny<Difficulty>(), f.Trait.Object, f.Actor.Object,
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome.Pass)));

		var ordinary = f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, 1, new StringStack(""));
		Assert.IsTrue(ordinary.Success, ordinary.Message);
		Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots.Single().Status);
		Assert.IsFalse(f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, null, new StringStack("")).Success);
		var ledgerWrites = f.Store.Writes; var ledgerEntries = f.Store.Log.Count;

		MagicModule.MagicGeneric(f.Actor.Object, "arcane invoke recklesslypowerful");

		Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots.Single().Status);
		Assert.AreEqual(ledgerWrites, f.Store.Writes);
		Assert.AreEqual(ledgerEntries, f.Store.Log.Count);
		f.Actor.Verify(x => x.UseResource(resource, 5), Times.Exactly(2));
		f.Actor.Verify(x => x.GainStamina((double)SpellPower.Standard), Times.Once);
		f.Actor.Verify(x => x.GainStamina((double)SpellPower.RecklesslyPowerful), Times.Once);
		check.Verify(x => x.CheckAgainstAllDifficulties(f.Actor.Object, It.IsAny<Difficulty>(), f.Trait.Object, f.Actor.Object,
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Once);
		Assert.AreEqual(1, f.Store.Log.Values.Count(x => x.Kind == "Cast"));
		Assert.IsNull(SpellPowerInvocation.For(f.Actor.Object, spell));
	}

	[TestMethod]
	public void VancianKnowledge_DoesNotGrantSpellBackedPowerOrFreeDirectCasting()
	{
		var items = new VancianItemTests.ItemFixture("<Effect type='staminadelta'><Formula>1</Formula></Effect>"); var f = items.F;
		var (spell, resource) = PricedSpell(items, "5"); f.Count = 1; f.Select(2); f.Plan(2); f.Refresh();
		var output = new List<string>();
		Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => output.Add(text));
		f.Actor.SetupGet(x => x.Powers).Returns([]);
		var ledgerEntries = f.Store.Log.Count;

		Assert.IsTrue(spell.CharacterKnowsSpell(f.Actor.Object));
		MagicModule.MagicGeneric(f.Actor.Object, "arcane invoke standard");
		spell.CastSpell(f.Actor.Object, f.Actor.Object, SpellPower.Standard);

		Assert.IsTrue(output.Any(x => x.Contains("You have no such power.")), string.Join("\n", output));
		Assert.AreEqual(VancianSlotStatus.Prepared, f.State.Slots.Single().Status);
		f.Actor.Verify(x => x.UseResource(resource, It.IsAny<double>()), Times.Never);
		f.Actor.Verify(x => x.GainStamina(It.IsAny<double>()), Times.Never);
		Assert.AreEqual(ledgerEntries, f.Store.Log.Count);
	}

	[TestMethod]
	public void SpellBackedPower_LegitimateLegacyRouteStillUsesItsNormalCheckAndCosts()
	{
		var items = new VancianItemTests.ItemFixture("<Effect type='staminadelta'><Formula>1</Formula></Effect>"); var f = items.F;
		var (spell, resource) = PricedSpell(items, "5");
		var legacy = new Mock<IMagicCapability>(); legacy.SetupGet(x => x.School).Returns(f.School.Object);
		f.Actor.SetupGet(x => x.Capabilities).Returns([legacy.Object, f.Capability.Object]);
		var known = f.Prog("candidates", _ => true); known.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true); spell.SpellKnownProg = known.Object;
		var check = new Mock<ICheck>(); f.World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(check.Object);
		check.Setup(x => x.CheckAgainstAllDifficulties(f.Actor.Object, It.IsAny<Difficulty>(), f.Trait.Object, f.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome.Pass)));
		Adapter(f).UseCommand(f.Actor.Object, "invoke", new StringStack("standard"));
		f.Actor.Verify(x => x.UseResource(resource, 5), Times.Once); f.Actor.Verify(x => x.GainStamina(1), Times.Once);
		Assert.AreEqual(0, f.Store.Log.Count); Assert.AreEqual(0, f.State.Slots.Count);
	}

	[DataTestMethod]
	[DataRow(VancianAllowanceMode.Memorised)]
	[DataRow(VancianAllowanceMode.Spontaneous)]
	[DataRow(VancianAllowanceMode.AtWill)]
	public void Combat_AllOrdinaryAllowanceModesCastButTimedWorkRefuses(VancianAllowanceMode mode)
	{
		var items = new VancianItemTests.ItemFixture("<Effect type='staminadelta'><Formula>1</Formula></Effect>"); var f = items.F;
		var (spell, resource) = PricedSpell(items); f.Count = 1;
		f.Allowances[0] = f.Allowances[0] with { Mode = mode, SlotLevel = mode == VancianAllowanceMode.AtWill ? null : 1 };
		f.Select(2); if (mode == VancianAllowanceMode.Memorised) f.Plan(2); f.Refresh();
		f.Actor.SetupGet(x => x.Combat).Returns(new Mock<ICombat>().Object);
		Assert.IsTrue(f.Service.CanCast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell).Available);
		Assert.IsFalse(f.Service.RequestRefresh(f.Actor.Object, f.Capability.Object).Success);
		Assert.IsFalse(f.Service.BeginInscription(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, null, items.Scroll(100).Item.Object).Success);
		var source = items.Book(101); source.Component.AddFormula(2, f.Clock.Now.UtcDateTime, null);
		Assert.IsFalse(f.Service.BeginTranscription(f.Actor.Object, f.Capability.Object, source.Item.Object, spell, items.Book(102).Item.Object).Success);
		var cast = f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, null, new StringStack(""));
		Assert.IsTrue(cast.Success, cast.Message); f.Actor.Verify(x => x.UseResource(resource, 12), Times.Once); f.Actor.Verify(x => x.GainStamina(1), Times.Once);
		Assert.IsTrue(f.State.Slots.All(x => x.Status == VancianSlotStatus.Spent));
	}

	[TestMethod]
	public void RetainedBookPattern_RemainsKnownAfterLastCastButInvalidReferencesDoNot()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; f.Count = 1;
		f.Rules[0] = f.Rules[0] with { Source = VancianRepertoireSource.Spellbook, SelectionLimitProgId = 0, BookPolicy = VancianBookPolicy.PatternChangesOnly };
		var book = items.Book(100); book.Component.AddFormula(2, f.Clock.Now.UtcDateTime, null); f.Plan(2); f.Refresh(); items.Items.Clear();
		Assert.IsTrue(f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, items.Spell, 1, new StringStack("")).Success);
		Assert.IsTrue(items.Spell.CharacterKnowsSpell(f.Actor.Object)); Assert.IsFalse(items.Spell.CharacterCanCast(f.Actor.Object, f.Actor.Object));
		var original = f.Allowances[0]; f.Allowances[0] = original with { StructuralVersion = original.StructuralVersion + 1 };
		Assert.IsFalse(items.Spell.CharacterKnowsSpell(f.Actor.Object)); f.Allowances[0] = original;
		f.Spells.Remove(items.Spell); Assert.IsFalse(items.Spell.CharacterKnowsSpell(f.Actor.Object)); f.Spells.Add(items.Spell);
		f.Rules[0] = f.Rules[0] with { CandidateProgId = f.Prog("candidates", _ => false).Object.Id };
		Assert.IsFalse(items.Spell.CharacterKnowsSpell(f.Actor.Object));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CombinedMaterials_TwoCostsConsumeTwoUnitsExactlyOnce(bool stack)
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; f.Count = 1; f.Select(2); f.Plan(2); f.Refresh();
		var first = items.Item(110); var second = items.Item(111);
		foreach (var item in new[] { first, second }) item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		var quantity = new Mock<IStackable>(); quantity.SetupProperty(x => x.Quantity, 2);
		if (stack) first.Setup(x => x.GetItemType<IStackable>()).Returns(quantity.Object);
		f.Actor.SetupGet(x => x.Body.HeldItems).Returns(stack ? [first.Object] : [first.Object, second.Object]);
		var scroll = items.Scroll(100);
		items.Spell.InventoryPlanTemplate.Phases.First().AddAction(new InventoryPlanActionConsume(f.World.Object, 1, 0, 0, _ => true, null!));
		((SpellScrollGameItemComponentProto)scroll.Component.Prototype).ProductionPlan.Phases.First().AddAction(new InventoryPlanActionConsume(f.World.Object, 1, 0, 0, _ => true, null!));
		var started = f.Service.BeginInscription(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, items.Spell, 1, scroll.Item.Object);
		Assert.IsTrue(started.Success, started.Message); f.Clock.Advance(TimeSpan.FromSeconds(1));
		var completed = f.Service.CompleteWriting(f.Actor.Object, started.OperationId!.Value); Assert.IsTrue(completed.Success, completed.Message);
		Assert.IsTrue(scroll.Component.IsCharged); first.Verify(x => x.Delete(), Times.Once);
		second.Verify(x => x.Delete(), stack ? Times.Never() : Times.Once()); if (stack) Assert.AreEqual(0, quantity.Object.Quantity);
	}

	[TestMethod]
	[Timeout(10_000)]
	public void CombinedMaterials_HeavilyOverlappingImpossiblePlanRefusesWithoutSpending()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		var reagents = Enumerable.Range(110, 9).Select(x => items.Item(x)).ToArray();
		foreach (var reagent in reagents) reagent.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		f.Actor.SetupGet(x => x.Body.HeldItems).Returns(reagents.Select(x => x.Object).ToArray());
		for (var i = 0; i < 10; i++) items.Spell.InventoryPlanTemplate.Phases.First().AddAction(new InventoryPlanActionConsume(f.World.Object, 1, 0, 0, _ => true, null!));
		using var plan = new VancianProductionPlan(f.Actor.Object, items.Spell.InventoryPlanTemplate);
		StringAssert.Contains(plan.Validate(), "conflicting allocations");
		foreach (var reagent in reagents) reagent.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void CombinedMaterials_RetainedToolIsAllocatedSeparatelyFromConsumedItem()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; var first = items.Item(110); var second = items.Item(111);
		foreach (var item in new[] { first, second }) item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		f.Actor.SetupGet(x => x.Body.HeldItems).Returns([first.Object, second.Object]);
		var tool = new InventoryPlanActionDrop(f.World.Object, 0, 0, item => item.Id == first.Object.Id, null!);
		f.Actor.SetupGet(x => x.Location).Returns(new Mock<MudSharp.Construction.ICell> { DefaultValue = DefaultValue.Mock }.Object);
		var consume = new InventoryPlanActionConsume(f.World.Object, 1, 0, 0, _ => true, null!);
		using var plan = new VancianProductionPlan(f.Actor.Object, new InventoryPlanTemplate(f.World.Object, consume), new InventoryPlanTemplate(f.World.Object, tool));
		Assert.IsNull(plan.Validate()); plan.Execute();
		first.Verify(x => x.Delete(), Times.Never); second.Verify(x => x.Delete(), Times.Once);
	}

	[DataTestMethod]
	[DataRow(VancianAllowanceMode.Memorised, 3, 16)]
	[DataRow(VancianAllowanceMode.AtWill, 1, 12)]
	public void SpellHelp_UsesTheSameRouteContextAsActualExpenditure(VancianAllowanceMode mode, int level, int cost)
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; var (spell, resource) = PricedSpell(items); f.Count = 1;
		f.Allowances[0] = f.Allowances[0] with { Mode = mode, SlotLevel = mode == VancianAllowanceMode.AtWill ? null : level };
		f.Select(2); if (mode == VancianAllowanceMode.Memorised) f.Plan(2); f.Refresh();
		var help = spell.ShowPlayerHelp(f.Actor.Object); StringAssert.Contains(help.StripANSIColour(), $"{cost} mana");
		Assert.IsFalse(help.Contains("Legacy Casting Costs"));
		string? output = null; Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((text, _, _) => output = text);
		typeof(MagicModule).GetMethod("VancianPlayer", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [f.Actor.Object, f.School.Object, new StringStack("20 spell 2")]);
		Assert.IsNotNull(output); StringAssert.Contains(output.StripANSIColour(), $"{cost} mana");
		Assert.IsTrue(f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, null, new StringStack("")).Success);
		f.Actor.Verify(x => x.UseResource(resource, cost), Times.Once);
		if (mode == VancianAllowanceMode.Memorised) StringAssert.Contains(spell.ShowPlayerHelp(f.Actor.Object), "Costs unresolved: 10 + 2 * castinglevel");
	}

	[TestMethod]
	public void SpellHelp_MultipleCapabilitiesShowIndependentCostsAndScopedHelpUsesSelectedCapability()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; var (spell, _) = PricedSpell(items); f.Count = 1;
		f.Select(2); f.Plan(2); f.Refresh();
		var other = new VancianTestFixture();
		other.Capability.SetupGet(x => x.Id).Returns(21); other.Capability.SetupGet(x => x.Name).Returns("Sorcerer");
		other.Capability.SetupGet(x => x.School).Returns(f.School.Object);
		other.Capability.SetupGet(x => x.Repertoires).Returns(f.Rules); other.Capability.SetupGet(x => x.PolicyProgs).Returns(f.Policies);
		var allowance = f.Allowances[0] with { Mode = VancianAllowanceMode.Spontaneous, SlotLevel = 3 };
		other.Capability.SetupGet(x => x.Allowances).Returns([allowance]);
		f.Actor.SetupGet(x => x.Capabilities).Returns([f.Capability.Object, other.Capability.Object]);
		var state = new VancianCapabilityState { OwnerId = 10, CapabilityId = 21 };
		state.Selections[f.Rules[0].Key] = [2];
		state.Slots.Add(new() { AllowanceKey = allowance.Key, AllowanceVersion = allowance.StructuralVersion, Level = 3, Ordinal = 1, Status = VancianSlotStatus.AvailableSpontaneous });
		f.Store.Commit(state, 0);
		var help = spell.ShowPlayerHelp(f.Actor.Object).StripANSIColour();
		StringAssert.Contains(help, "Wizard known/first"); StringAssert.Contains(help, "12 mana");
		StringAssert.Contains(help, "Sorcerer known/first"); StringAssert.Contains(help, "16 mana");
		var scoped = spell.ShowPlayerHelp(f.Actor.Object, other.Capability.Object).StripANSIColour();
		StringAssert.Contains(scoped, "16 mana"); Assert.IsFalse(scoped.Contains("12 mana"));
	}
}

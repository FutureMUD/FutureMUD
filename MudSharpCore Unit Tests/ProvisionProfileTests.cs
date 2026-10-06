#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
namespace MudSharp_Unit_Tests;

[TestClass]
public class ProvisionProfileTests
{
	private sealed class RecipeEffect(XElement root, IMagicSpell spell) : CreateLiquidEffect(root, spell);
	private static (RecipeEffect Effect, Mock<IFuturemud> World, Mock<ICharacter> Actor, Mock<IFutureProg> Policy, List<IFutureProg> Progs) Recipe(string outcome = "true")
	{
		var world = new Mock<IFuturemud>(); var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		var policy = new Mock<IFutureProg>(); policy.SetupGet(x => x.Id).Returns(72); policy.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		policy.SetupGet(x => x.CompileError).Returns(outcome == "compile" ? "Invalid fixture" : ""); policy.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(outcome != "signature");
		object value = outcome == "null" ? null! : outcome != "false";
		policy.Setup(x => x.ExecuteWithStatus(out value, It.IsAny<object[]>())).Returns(outcome != "failed");
		var progs = new List<IFutureProg> { policy.Object }; world.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => progs));
		var liquids = new[] { Mock.Of<ILiquid>(x => x.Id == 1), Mock.Of<ILiquid>(x => x.Id == 2) }; world.SetupGet(x => x.Liquids).Returns(MagicCastingFixture.Collection(() => liquids));
		var xml = XElement.Parse("<Effect type='createliquid'><LiquidId>1</LiquidId><AmountFormula>0.5*grade</AmountFormula><ContainerFill version='1'><Litres>0.5*grade</Litres><Recipes version='1'><Recipe order='32' predicate='0' liquid='1'/></Recipes></ContainerFill></Effect>");
		xml.Element("ContainerFill")!.Element("Recipes")!.AddFirst(new XElement("Recipe", new XAttribute("order", 1), new XAttribute("predicate", 72), new XAttribute("liquid", 2)));
		return (new(xml, Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object)), world, actor, policy, progs);
	}
	[DataTestMethod][DataRow("compile")][DataRow("signature")][DataRow("missing")][DataRow("null")][DataRow("failed")]
	public void ConfiguredRecipe_RefusesInvalidMissingAndFailedPolicy(string failure)
	{
		var f = Recipe(failure); if (failure == "missing") f.Progs.Clear();
		Assert.IsFalse(f.Effect.TryPrepareRecipe(f.Actor.Object, out var error)); Assert.IsNotNull(error);
		Assert.AreEqual(72L, (long)f.Effect.SaveToXml().Element("ContainerFill")!.Element("Recipes")!.Element("Recipe")!.Attribute("predicate")!);
		if (failure is "null" or "failed") f.Policy.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Once());
	}
	[DataTestMethod][DataRow("true", 2L)][DataRow("false", 1L)]
	public void RecipeSelection_OrderedTrueOrFallbackIsFrozenWithLiveRevalidation(string result, long expected)
	{
		var f = Recipe(result); Assert.IsTrue(f.Effect.TryPrepareRecipe(f.Actor.Object, out var error), error);
		Assert.IsTrue(f.Effect.TryPrepareRecipe(f.Actor.Object, out error), error);
		Assert.AreEqual(expected, ((ILiquid)typeof(CreateLiquidEffect).GetField("_preparedRecipe", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(f.Effect)!).Id);
		f.Policy.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Exactly(2));
	}
	[DataTestMethod][DataRow(true)][DataRow(false)]
	public void RecipeToken_AdoptsChoiceOnlyForOriginalRecipientAndRejectsChangedFirstMatch(bool differentRecipient)
	{
		var f = Recipe(); var recipient = Mock.Of<IPerceivable>();
		Assert.IsTrue(f.Effect.TryPrepareRecipe(f.Actor.Object, out _));
		var token = f.Effect.CapturePreparedSelection(f.Actor.Object, recipient)!;
		var copy = new RecipeEffect(f.Effect.SaveToXml(), Mock.Of<IMagicSpell>(x => x.Gameworld == f.World.Object));
		Assert.AreEqual(!differentRecipient, copy.TryReusePreparedSelection(token, f.Actor.Object, differentRecipient ? Mock.Of<IPerceivable>() : recipient, out _));
		if (differentRecipient) return;
		object value = false; f.Policy.Setup(x => x.ExecuteWithStatus(out value, It.IsAny<object[]>())).Returns(true);
		Assert.IsFalse(copy.TryPrepareRecipe(f.Actor.Object, out var error)); StringAssert.Contains(error!, "first matching");
	}
	[DataTestMethod][DataRow(true)][DataRow(false)]
	public void RecipeCallbackOrPreparedContextChange_RefusesAdmission(bool duringCallback)
	{
		var f = Recipe();
		if (duringCallback) {
			object value = true; f.Policy.Setup(x => x.ExecuteWithStatus(out value, It.IsAny<object[]>())).Callback(() => f.Actor.SetupGet(x => x.Location).Returns(Mock.Of<ICell>())).Returns(true);
		} else {
			Assert.IsTrue(f.Effect.TryPrepareRecipe(f.Actor.Object, out _)); f.Actor.SetupGet(x => x.Location).Returns(Mock.Of<ICell>());
		}
		Assert.IsFalse(f.Effect.TryPrepareRecipe(f.Actor.Object, out var error)); StringAssert.Contains(error!, duringCallback ? "callback" : "context");
	}
	[DataTestMethod][DataRow("empty")][DataRow("missing")][DataRow("duplicate")][DataRow("fallback")]
	public void MalformedRecipes_RetainConfiguredXmlAndRefuseInsteadOfFixedFallback(string failure)
	{
		var f = Recipe(); var xml = f.Effect.SaveToXml(); var root = xml.Element("ContainerFill")!.Element("Recipes")!;
		if (failure == "empty") root.RemoveNodes();
		if (failure == "missing") root.Element("Recipe")!.Attribute("predicate")!.Remove();
		if (failure == "duplicate") root.Add(new XElement(root.Element("Recipe")!));
		if (failure == "fallback") root.Elements("Recipe").Last().Attribute("predicate")!.Value = "72";
		var loaded = new RecipeEffect(xml, Mock.Of<IMagicSpell>(x => x.Gameworld == f.World.Object));
		Assert.IsFalse(loaded.TryPrepareRecipe(f.Actor.Object, out _)); Assert.IsTrue(XNode.DeepEquals(xml, loaded.SaveToXml()));
		Assert.IsNotNull(((CreateLiquidEffect)loaded.Clone()).DefinitionError);
	}
}

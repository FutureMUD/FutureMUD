#nullable enable
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ElementalAffinityMeritTests
{
	[TestMethod]
	public void DeclaredAffinities_OnlyExplicitProvidersAndNoProgExecution()
	{
		var first = new Mock<IElementalAffinityMerit>(MockBehavior.Strict); first.SetupGet(x => x.ElementalAffinities).Returns(new[] { "Wind", "Earth" });
		var second = new Mock<IElementalAffinityMerit>(MockBehavior.Strict); second.SetupGet(x => x.ElementalAffinities).Returns(new[] { "earth" });
		var owner = new Mock<IHaveMerits>(); owner.SetupGet(x => x.Merits).Returns(new IMerit[] { first.Object, second.Object, new Mock<IMagicCapabilityMerit>().Object });
		CollectionAssert.AreEqual(new[] { "Earth", "Wind" }, owner.Object.DeclaredElementalAffinities().ToArray());
		first.VerifyGet(x => x.ElementalAffinities, Times.Once); second.VerifyGet(x => x.ElementalAffinities, Times.Once);
		first.VerifyNoOtherCalls(); second.VerifyNoOtherCalls();
		owner.SetupGet(x => x.Merits).Returns(new IMerit[] { new Mock<IMagicCapabilityMerit>().Object });
		Assert.AreEqual(0, owner.Object.DeclaredElementalAffinities().Count);
	}

	[TestMethod]
	public void AffinityMerit_FactoryAndXmlRoundTripPreserveLabels()
	{
		ElementalAffinityMerit.RegisterMeritInitialiser();
		var progs = new All<IFutureProg>(); var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(1); progs.Add(prog.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var row = new Merit { Id = 55, Name = "Stock declared affinity", Type = "Elemental Affinity", MeritScope = (int)MeritScope.Character,
			Definition = new XElement("Merit", new XElement("ChargenAvailableProg", 1), new XElement("ApplicabilityProg", 1),
				new XElement("ChargenBlurb", "Explicit metadata"), new XElement("DescriptionText", "@ have|has an affinity"),
				new XElement("ElementalAffinities", new XElement("Affinity", "Wind"), new XElement("Affinity", "wind"), new XElement("Affinity", "Earth"))).ToString() };
		var loaded = (ElementalAffinityMerit)MeritFactory.LoadMerit(row, world.Object);
		Assert.AreEqual(2, loaded.ElementalAffinities.Count); row.Definition = loaded.SaveDefinition().ToString();
		CollectionAssert.AreEqual(loaded.ElementalAffinities.ToArray(), ((ElementalAffinityMerit)MeritFactory.LoadMerit(row, world.Object)).ElementalAffinities.ToArray());
	}

	[TestMethod]
	public void TraitReferences_BuilderFormulaReplacementOverridesLoadedParameterWithoutWorldLookup()
	{
		var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(25);
		var traits = new All<ITraitDefinition>(); traits.Add(trait.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Traits).Returns(traits);
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		world.SetupGet(x => x.MagicResources).Returns(new All<IMagicResource>());
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var expression = new MudSharp.Body.Traits.TraitExpression("power:25", world.Object); _ = expression.Parameters;
		Assert.IsTrue(expression.BuildingCommand(actor.Object, new StringStack("formula power:26")));
		// Trait 26 is deliberately absent. This metadata query must not resolve it.
		Assert.AreEqual(26L, expression.TraitReferenceIds["power"]); Assert.AreEqual(1, expression.TraitReferenceIds.Count);
		// Renaming the parameter must also discard the stale loaded dependency.
		var renamed = new MudSharp.Body.Traits.TraitExpression("power:25", world.Object); _ = renamed.Parameters;
		Assert.IsTrue(renamed.BuildingCommand(actor.Object, new StringStack("formula other:26")));
		Assert.AreEqual(26L, renamed.TraitReferenceIds["other"]); Assert.AreEqual(1, renamed.TraitReferenceIds.Count);
		Assert.IsFalse(renamed.TraitReferenceIds.ContainsKey("power"));
	}

	[TestMethod]
	public void TraitReferences_NativeGrammarOverridesRowsAndDoesNotLoadOrEvaluate()
	{
		var row = new MudSharp.Models.TraitExpression { Expression = "first:7 + 300 + second:19{noimprove,nobranch}" };
		row.TraitExpressionParameters.Add(new() { Parameter = "first", TraitDefinitionId = 6 });
		var expression = new MudSharp.Body.Traits.TraitExpression(row, new Mock<IFuturemud>(MockBehavior.Strict).Object);
		Assert.AreEqual(7L, expression.TraitReferenceIds["first"]); Assert.AreEqual(19L, expression.TraitReferenceIds["second"]);
		Assert.AreEqual(2, expression.TraitReferenceIds.Count);
		var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(25);
		var world = new Mock<IFuturemud>(); var traits = new All<ITraitDefinition>(); traits.Add(trait.Object); world.SetupGet(x => x.Traits).Returns(traits);
		var lazy = new MudSharp.Body.Traits.TraitExpression("power:25", world.Object);
		Assert.AreEqual(25L, lazy.TraitReferenceIds["power"]); _ = lazy.Parameters;
		Assert.AreEqual(25L, lazy.TraitReferenceIds["power"]);
	}
}

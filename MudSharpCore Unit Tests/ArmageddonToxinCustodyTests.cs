#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using ConcreteBody = MudSharp.Body.Implementations.Body;
using NativeMagicSpell = MudSharp.Magic.MagicSpell;
using NativeTraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonToxinCustodyTests
{
	[TestMethod]
	public void VenomTouch_AllGradesBindPositiveNativeDoseAndRetainExactFormulaProvenance()
	{
		var fixture = new MagicCastingFixture();
		var drug = Mock.Of<IDrug>(x => x.Id == 81 && x.Name == "stock toxin");
		fixture.World.SetupGet(x => x.Drugs).Returns(MagicCastingFixture.Collection(() => new[] { drug }));
		var content = ArmageddonToxinSpellContent.Create(81).Single(x => x.Content.Key ==
			ArmageddonToxinSpellContent.VenomTouchKey).Content;
		fixture.Expressions.Clear();
		var cost = content.CostRow(1); cost.Id = 1;
		var duration = content.DurationRow(1); duration.Id = 2;
		fixture.Expressions.Add(new NativeTraitExpression(cost, fixture.World.Object));
		fixture.Expressions.Add(new NativeTraitExpression(duration, fixture.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new NativeMagicSpell(row, fixture.World.Object);
		for (var grade = 1; grade <= 7; grade++)
		{
			var copy = spell.CastingCopy(fixture.Actor.Object, fixture.Traits[0], grade,
				SpellPower.ExtremelyStrong, Difficulty.Normal);
			var poison = (PoisonEffect)copy.SpellEffects.Single();
			Assert.AreEqual(0.001 * grade, poison.GramsExpression.Evaluate(fixture.Actor.Object), 1e-12);
			Assert.AreEqual(ArmageddonToxinSpellContent.GramsFormula, poison.GramsFormulaText);
			Assert.AreEqual(ArmageddonToxinSpellContent.GramsFormula, (string?)poison.SaveToXml().Element("Formula"));
		}
	}

	[TestMethod]
	public void BodyRows_PreserveMundaneAndSeparateSpellOriginsAcrossSaveAndReload()
	{
		var drug = Mock.Of<IDrug>(x => x.Id == 81);
		var first = Guid.NewGuid();
		var second = Guid.NewGuid();
		var absorbed = ConcreteBody.AbsorbedDrugDose(Dose(drug, 0.001, DrugVector.Touched, first,
			ArmageddonToxinSpellContent.OwnedFamily, "0.001*grade"));
		absorbed.Grams = 0.001;
		var active = new[]
		{
			Dose(drug, 0.008, DrugVector.Ingested),
			absorbed,
			Dose(drug, 0.002, DrugVector.Injected, second, ArmageddonToxinSpellContent.OwnedFamily, "0.002*grade")
		};
		var latent = new[] { Dose(drug, 0.003, DrugVector.Touched, first, ArmageddonToxinSpellContent.OwnedFamily, "0.001*grade") };
		var rows = new List<BodyDrugDose>();

		ConcreteBody.ReconcileDrugDoseRows(rows, active, latent);
		Assert.AreEqual(4, rows.Count);
		var reloaded = rows.Select(row => ConcreteBody.RestoreDrugDoseRow(row, drug)).ToArray();
		Assert.AreEqual(0.008, reloaded.Single(x => x.SpellOriginId is null).Grams, 0.000001);
		Assert.AreEqual(2, reloaded.Count(x => x.SpellOriginId == first));
		Assert.AreEqual(DrugVector.Touched, reloaded.Single(x => x.SpellOriginId == first && x.Grams == 0.001).OriginalVector,
			"Absorption must retain the spell's exposure route for an exact later purge.");
		Assert.AreEqual(1, reloaded.Count(x => x.SpellOriginId == second));
		Assert.IsTrue(reloaded.Where(x => x.SpellOriginId.HasValue)
			.All(x => x.SpellOriginFamily == ArmageddonToxinSpellContent.OwnedFamily));
		Assert.AreEqual("0.002*grade", reloaded.Single(x => x.SpellOriginId == second).SpellOriginFormula);

		ConcreteBody.ReconcileDrugDoseRows(rows, active.Where(x => x.SpellOriginId != first), []);
		Assert.AreEqual(2, rows.Count);
		Assert.IsTrue(rows.Any(x => x.SpellOriginId is null && x.Grams == 0.008));
		Assert.IsTrue(rows.Any(x => x.SpellOriginId == second && x.Grams == 0.002));
	}

	[TestMethod]
	public void ReloadedStockPoison_DoesNotRedose_AndPurgePreservesUnownedAndOtherFamily()
	{
		var fixture = new MagicCastingFixture();
		var drug = Mock.Of<IDrug>(x => x.Id == 81 && x.Name == "stock toxin");
		fixture.World.SetupGet(x => x.Drugs).Returns(MagicCastingFixture.Collection(() => new[] { drug }));
		SpellEffectFactory.SetupFactory();
		SpellPoisonEffect.InitialiseEffectType();
		var parent = new MagicSpellParent(fixture.Actor.Object, fixture.Spell, fixture.Actor.Object);
		var doses = new List<DrugDosage>();
		fixture.Body.SetupGet(x => x.ActiveDrugDosages).Returns(() => doses);
		fixture.Body.SetupGet(x => x.LatentDrugDosages).Returns(Array.Empty<DrugDosage>());
		fixture.Body.Setup(x => x.Dose(drug, DrugVector.Touched, It.IsAny<double>(), It.IsAny<object>()))
			.Callback<IDrug, DrugVector, double, object>((_, vector, grams, origin) =>
				{
					var dose = ConcreteBody.AbsorbedDrugDose(Dose(drug, grams, vector,
						((SpellPoisonEffect)origin).DrugDoseOriginId,
						ArmageddonToxinSpellContent.OwnedFamily, ((SpellPoisonEffect)origin).GramsFormulaText));
					dose.Grams = grams;
					doses.Add(dose);
				});
		fixture.Body.Setup(x => x.RemoveDrugDosages(It.IsAny<Predicate<DrugDosage>>()))
			.Callback<Predicate<DrugDosage>>(predicate => doses.RemoveAll(predicate));
		var original = new SpellPoisonEffect(fixture.Actor.Object, parent, drug, DrugVector.Touched, 0.001,
			ArmageddonToxinSpellContent.GramsFormula, ownedFamily: ArmageddonToxinSpellContent.OwnedFamily);
		original.InitialEffect();
		var saved = original.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = (SpellPoisonEffect)Effect.LoadEffect(saved, fixture.Actor.Object);
		loaded.ParentEffect = parent;
		loaded.InitialEffect();
		Assert.AreEqual(original.DrugDoseOriginId, loaded.DrugDoseOriginId);
		fixture.Body.Verify(x => x.Dose(drug, DrugVector.Touched, 0.001, It.IsAny<object>()), Times.Once);
		doses.Add(Dose(drug, 0.008, DrugVector.Ingested));
		doses.Add(Dose(drug, 0.004, DrugVector.Injected, Guid.NewGuid(), "other.family"));
		doses.Add(Dose(drug, 0.005, DrugVector.Touched, Guid.NewGuid(),
			ArmageddonToxinSpellContent.OwnedFamily, "0.002*grade"));
		doses.Add(Dose(drug, 0.006, DrugVector.Injected, Guid.NewGuid(),
			ArmageddonToxinSpellContent.OwnedFamily, ArmageddonToxinSpellContent.GramsFormula));
		loaded.RemovalEffect();
		Assert.AreEqual(5, doses.Count, "Ordinary expiry leaves the native dose to clear physiologically.");

		var source = ArmageddonToxinSpellContent.Create(81).Single(x => x.Content.Key == ArmageddonToxinSpellContent.PurgeToxinKey)
			.Content.BuildDefinition(10, 1, 0).Element("Effects")!.Elements().Single();
		var cure = (RemovePoisonEffect)SpellEffectFactory.LoadEffect(source, fixture.Spell);
		fixture.Actor.Setup(x => x.EffectsOfType<SpellPoisonEffect>(It.IsAny<Predicate<SpellPoisonEffect>>()))
			.Returns(Array.Empty<SpellPoisonEffect>());
		var result = cure.Apply(fixture.Actor.Object, fixture.Actor.Object, OpposedOutcomeDegree.None,
			SpellPower.Standard, parent, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(4, doses.Count);
		Assert.IsTrue(doses.Any(x => x.SpellOriginId is null));
		Assert.IsTrue(doses.Any(x => x.SpellOriginFamily == "other.family"));
		Assert.IsTrue(doses.Any(x => x.SpellOriginFormula == "0.002*grade"));
		Assert.IsTrue(doses.Any(x => x.SpellOriginFamily == ArmageddonToxinSpellContent.OwnedFamily &&
			x.OriginalVector == DrugVector.Injected));
	}

	private static DrugDosage Dose(IDrug drug, double grams, DrugVector vector, Guid? id = null,
		string? family = null, string? formula = null) => new()
	{
		Drug = drug, Grams = grams, OriginalVector = vector, SpellOriginId = id,
		SpellOriginFamily = family!, SpellOriginFormula = formula!
	};
}

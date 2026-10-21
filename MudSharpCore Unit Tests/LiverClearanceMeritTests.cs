#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LiverClearanceMeritTests
{
	[DataTestMethod]
	[DataRow(1.0, true, 0.2)]
	[DataRow(0.25, true, 0.05)]
	[DataRow(0.0, true, 0.0)]
	[DataRow(0.25, false, 0.025)]
	public void EffectiveClearance_MultipliesAfterInjuryAndAppliesToEveryActiveDrug(double function, bool applies, double expected)
	{
		var actor = new Mock<ICharacter>();
		var merit = new Mock<ILiverClearanceMerit>();
		merit.SetupGet(x => x.ClearanceMultiplier).Returns(2.0);
		merit.Setup(x => x.Applies(actor.Object)).Returns(applies);
		actor.SetupGet(x => x.Merits).Returns(new IMerit[] { merit.Object });
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		body.Actor = actor.Object;
		body.BaseLiverAlcoholRemovalKilogramsPerHour = 0.1;
		typeof(ConcreteBody).GetField("_cachedOrganFunctionsByType", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(body, new DoubleCounter<Type> { [typeof(LiverProto)] = function });
		Assert.AreEqual(expected, body.LiverAlcoholRemovalKilogramsPerHour, 1e-12);
		var harmful = new Mock<IDrug>();
		var beneficial = new Mock<IDrug>();
		harmful.SetupGet(x => x.RelativeMetabolisationRate).Returns(1.0);
		beneficial.SetupGet(x => x.RelativeMetabolisationRate).Returns(0.5);
		var doses = new List<DrugDosage>
		{
			new() { Drug = harmful.Object, Grams = 10 }, new() { Drug = beneficial.Object, Grams = 10 }
		};
		typeof(ConcreteBody).GetField("_activeDrugDosages", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, doses);
		// Avoid save-manager infrastructure in this bounded metabolism test.
		typeof(ConcreteBody).GetField("_drugsChanged", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, true);
		typeof(ConcreteBody).GetMethod("MetaboliseActiveDrugs", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(body, null);
		Assert.AreEqual(10 - expected * 10000 / 3600, doses[0].Grams, 1e-12);
		Assert.AreEqual(10 - expected * 10000 / 3600 * 0.5, doses[1].Grams, 1e-12);
	}

	[TestMethod]
	public void PersistedMerit_ReloadsItsMultiplierWithoutNeedsRateStacking()
	{
		LiverClearanceMerit.RegisterMeritInitialiser();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var merit = MeritFactory.LoadMerit(new MudSharp.Models.Merit
		{
			Id = 42, Name = "Mercenary clearance", Type = LiverClearanceMerit.MeritTypeName,
			MeritScope = 2, Definition = "<Definition multiplier='2'/>"
		}, world.Object);
		Assert.AreEqual(2.0, ((ILiverClearanceMerit)merit).ClearanceMultiplier);
		Assert.IsFalse(merit is INeedRateChangingMerit);
	}
}

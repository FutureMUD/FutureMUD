#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using CultureInfo = System.Globalization.CultureInfo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PhysiologyMeritTypeTests
{
	private static (Mock<IFuturemud> World, All<IFutureProg> Progs) World()
	{
		var world = new Mock<IFuturemud>();
		var progs = new All<IFutureProg>();
		var yes = new Mock<IFutureProg>();
		yes.SetupGet(x => x.Id).Returns(1);
		yes.SetupGet(x => x.FunctionName).Returns("AlwaysTrue");
		yes.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		var no = new Mock<IFutureProg>();
		no.SetupGet(x => x.Id).Returns(2);
		no.SetupGet(x => x.FunctionName).Returns("AlwaysFalse");
		progs.Add(yes.Object);
		progs.Add(no.Object);
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		world.SetupGet(x => x.AlwaysTrueProg).Returns(yes.Object);
		world.SetupGet(x => x.AlwaysFalseProg).Returns(no.Object);
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		return (world, progs);
	}

	private static Merit Row(string type, params object[] content) => new()
	{
		Id = 45,
		Name = "Physiology test",
		Type = type,
		MeritScope = (int)MeritScope.Body,
		Definition = new XElement("Merit", new XElement("ChargenAvailableProg", 2),
			new XElement("ApplicabilityProg", 1), new XElement("ChargenBlurb", "Intrinsic physiology"),
			new XElement("DescriptionText", "@ have|has adapted physiology"), content).ToString()
	};

	private static Mock<IFutureProg> NumericProg(long id, Func<double> value)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(id);
		prog.SetupGet(x => x.FunctionName).Returns("Needs" + id);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		prog.SetupGet(x => x.Public).Returns(true);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
			.Returns<IEnumerable<ProgVariableTypes>>(x => x.SequenceEqual(new[] { ProgVariableTypes.Character }));
		prog.Setup(x => x.ExecuteDouble(double.NaN, It.IsAny<object[]>())).Returns(value);
		return prog;
	}

	[TestMethod]
	public void StaminaCapacity_XmlRoundTripRetainsBodyScopeAndSignedBonus()
	{
		StaminaCapacityMerit.RegisterMeritInitialiser();
		var (world, _) = World();
		var row = Row(StaminaCapacityMerit.MeritTypeName, new XElement("Bonus", -20), new XElement("Multiplier", "1.25"));
		var loaded = (StaminaCapacityMerit)MeritFactory.LoadMerit(row, world.Object);
		Assert.AreEqual(MeritScope.Body, loaded.MeritScope);
		Assert.AreEqual(-20.0, loaded.StaminaBonus);
		Assert.AreEqual(1.25, loaded.StaminaMultiplier);
		row.Definition = loaded.SaveDefinition().ToString();
		var reloaded = (IStaminaCapacityMerit)MeritFactory.LoadMerit(row, world.Object);
		Assert.AreEqual(loaded.StaminaBonus, reloaded.StaminaBonus);
		Assert.AreEqual(loaded.StaminaMultiplier, reloaded.StaminaMultiplier);
		Assert.IsFalse((object)loaded is INeedRateChangingMerit);
	}

	[TestMethod]
	[DataRow("NaN", "1")]
	[DataRow("0", "Infinity")]
	[DataRow("0", "-1")]
	public void StaminaCapacity_InvalidPersistedValuesAreRejected(string bonus, string multiplier)
	{
		StaminaCapacityMerit.RegisterMeritInitialiser();
		var (world, _) = World();
		Assert.ThrowsExactly<InvalidOperationException>(() => MeritFactory.LoadMerit(
			Row(StaminaCapacityMerit.MeritTypeName, new XElement("Bonus", bonus), new XElement("Multiplier", multiplier)), world.Object));
	}

	[TestMethod]
	public void EnvironmentalNeeds_EvaluatesEachConfiguredProgOnceAndReevaluatesContext()
	{
		EnvironmentalNeedsMerit.RegisterMeritInitialiser();
		var (world, progs) = World();
		var value = -2.0;
		var prog = NumericProg(10, () => value);
		progs.Add(prog.Object);
		var row = Row(EnvironmentalNeedsMerit.MeritTypeName, new XElement("HungerProg", 10), new XElement("ThirstProg", 10),
			new XElement("WaterProg", 10), new XElement("MaximumThirstRate", 5));
		var loaded = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(row, world.Object);
		var actor = new Mock<ICharacter>().Object;
		var adjustment = loaded.GetNeedsAdjustment(actor);
		Assert.AreEqual(-2.0, adjustment.HungerRateDelta);
		Assert.AreEqual(-2.0, adjustment.ThirstRateDelta);
		Assert.AreEqual(-2.0, adjustment.WaterLossLitresPerHour);
		Assert.AreEqual(5.0, adjustment.MaximumThirstRate);
		Assert.IsNull(adjustment.MaximumHungerRate);
		prog.Verify(x => x.ExecuteDouble(double.NaN, It.Is<object[]>(args => args.Length == 1 && ReferenceEquals(args[0], actor))), Times.Once);
		value = 3;
		Assert.AreEqual(3.0, loaded.GetNeedsAdjustment(actor).ThirstRateDelta);
		prog.Verify(x => x.ExecuteDouble(double.NaN, It.IsAny<object[]>()), Times.Exactly(2));
		row.Definition = loaded.SaveDefinition().ToString();
		var reloaded = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(row, world.Object);
		Assert.AreEqual(10L, reloaded.ThirstProgId);
		Assert.AreEqual(5.0, reloaded.MaximumThirstRate);
		Assert.AreEqual(MeritScope.Body, reloaded.MeritScope);
	}

	[TestMethod]
	public void EnvironmentalNeeds_MissingWrongSignatureAndNonfiniteProgsAreNeutralAndDiagnosable()
	{
		EnvironmentalNeedsMerit.RegisterMeritInitialiser();
		var (world, progs) = World();
		var invalidResult = NumericProg(10, () => double.NaN);
		var wrongSignature = NumericProg(11, () => 4);
		wrongSignature.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		progs.Add(invalidResult.Object);
		progs.Add(wrongSignature.Object);
		var row = Row(EnvironmentalNeedsMerit.MeritTypeName, new XElement("HungerProg", 777), new XElement("ThirstProg", 10),
			new XElement("WaterProg", 11), new XElement("MaximumThirstRate", "-1"));
		var loaded = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(row, world.Object);
		var adjustment = loaded.GetNeedsAdjustment(new Mock<ICharacter>().Object);
		Assert.AreEqual(new EnvironmentalNeedsAdjustment(), adjustment);
		Assert.AreEqual(4, loaded.ConfigurationWarnings.Count);
		Assert.AreEqual("777", loaded.SaveDefinition().Element("HungerProg")!.Value, "Missing references must remain available for builder repair.");
		wrongSignature.Verify(x => x.ExecuteDouble(It.IsAny<double>(), It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	[DataRow("777")]
	[DataRow("-1")]
	[DataRow("invalid")]
	public void EnvironmentalNeeds_FailedChannelCannotApplyItsCapToBaselineNeeds(string progId)
	{
		EnvironmentalNeedsMerit.RegisterMeritInitialiser();
		var (world, _) = World();
		var loaded = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(Row(EnvironmentalNeedsMerit.MeritTypeName,
			new XElement("ThirstProg", progId), new XElement("MaximumThirstRate", "0.25")), world.Object);
		var adjustment = loaded.GetNeedsAdjustment(new Mock<ICharacter>().Object);
		Assert.AreEqual(0.0, adjustment.ThirstRateDelta);
		Assert.IsNull(adjustment.MaximumThirstRate);
		Assert.AreEqual(0.25, loaded.MaximumThirstRate, "Keep the configured cap available for repair and save/reload.");
		var row = Row(EnvironmentalNeedsMerit.MeritTypeName);
		row.Definition = loaded.SaveDefinition().ToString();
		Assert.AreEqual(progId, XElement.Parse(row.Definition).Element("ThirstProg")!.Value);
		var reloaded = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(row, world.Object);
		Assert.IsNull(reloaded.GetNeedsAdjustment(new Mock<ICharacter>().Object).MaximumThirstRate,
			"An unrelated save must not turn a failed reference into an intentional cap-only channel.");
		Assert.AreEqual(1, reloaded.ConfigurationWarnings.Count);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		Assert.IsTrue(reloaded.BuildingCommand(actor.Object, new StringStack("thirstprog none")));
		row.Definition = reloaded.SaveDefinition().ToString();
		var repaired = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(row, world.Object);
		Assert.AreEqual(0.25, repaired.GetNeedsAdjustment(actor.Object).MaximumThirstRate);
		Assert.AreEqual(0, repaired.ConfigurationWarnings.Count);
	}

	[TestMethod]
	public void EnvironmentalNeeds_ExplicitNoProgCanConfigureACapWithoutADelta()
	{
		EnvironmentalNeedsMerit.RegisterMeritInitialiser();
		var (world, _) = World();
		var loaded = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(Row(EnvironmentalNeedsMerit.MeritTypeName,
			new XElement("ThirstProg", 0), new XElement("MaximumThirstRate", "0.25")), world.Object);
		Assert.AreEqual(0.25, loaded.GetNeedsAdjustment(new Mock<ICharacter>().Object).MaximumThirstRate);
	}

	[TestMethod]
	public void EnvironmentalNeeds_NullExecutionFallbackDisablesTheCapInsteadOfActingAsNumericZero()
	{
		EnvironmentalNeedsMerit.RegisterMeritInitialiser();
		var (world, progs) = World();
		var prog = NumericProg(10, () => 0);
		// Native ExecuteDouble returns the supplied fallback when execution returns null.
		prog.Setup(x => x.ExecuteDouble(It.IsAny<double>(), It.IsAny<object[]>()))
			.Returns<double, object[]>((fallback, _) => fallback);
		progs.Add(prog.Object);
		var loaded = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(Row(EnvironmentalNeedsMerit.MeritTypeName,
			new XElement("ThirstProg", 10), new XElement("MaximumThirstRate", "0.25")), world.Object);
		var adjustment = loaded.GetNeedsAdjustment(new Mock<ICharacter>().Object);
		Assert.AreEqual(0.0, adjustment.ThirstRateDelta);
		Assert.IsNull(adjustment.MaximumThirstRate);
		Assert.AreEqual(1, loaded.ConfigurationWarnings.Count);
	}

	[TestMethod]
	public void PhysiologyBuilders_RejectNonfiniteInputsAndClearOptionalEnvironmentFields()
	{
		StaminaCapacityMerit.RegisterMeritInitialiser();
		EnvironmentalNeedsMerit.RegisterMeritInitialiser();
		var (world, _) = World();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.Culture).Returns(CultureInfo.InvariantCulture);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Account).Returns(account.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var stamina = (StaminaCapacityMerit)MeritFactory.LoadMerit(Row(StaminaCapacityMerit.MeritTypeName), world.Object);
		Assert.IsFalse(stamina.BuildingCommand(actor.Object, new StringStack("bonus NaN")));
		Assert.IsFalse(stamina.BuildingCommand(actor.Object, new StringStack("multiplier -10%")));
		var environment = (EnvironmentalNeedsMerit)MeritFactory.LoadMerit(Row(EnvironmentalNeedsMerit.MeritTypeName,
			new XElement("ThirstProg", 777), new XElement("MaximumThirstRate", 5)), world.Object);
		Assert.IsFalse(environment.BuildingCommand(actor.Object, new StringStack("maxthirst Infinity")));
		Assert.IsTrue(environment.BuildingCommand(actor.Object, new StringStack("thirstprog none")));
		Assert.IsTrue(environment.BuildingCommand(actor.Object, new StringStack("maxthirst none")));
		Assert.AreEqual(0L, environment.ThirstProgId);
		Assert.IsNull(environment.MaximumThirstRate);
		Assert.AreEqual(0, environment.ConfigurationWarnings.Count);
	}
}

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Generators;
using MudSharp.Work.Agriculture;
using MagicGenerator = MudSharp.Models.MagicGenerator;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalMagicGeneratorTests
{
	[TestMethod]
	public void Factory_EnvironmentalDatabaseDefinition_LoadsVersionedProfile()
	{
		var world = World();
		var generator = (EnvironmentalMagicGenerator)BaseMagicResourceGenerator.LoadFromDatabase(Model(), world.Object);
		Assert.AreEqual("Environmental", generator.RegeneratorTypeName);
		Assert.AreEqual(0, generator.ValidationErrors.Count);
		Assert.AreEqual(1, generator.Outputs.Count);
		Assert.AreEqual(100.0, generator.Outputs[0].BaseCapacity);
		Assert.AreEqual(1.0, generator.Outputs[0].BaseRate);
		Assert.AreEqual(3600.0, generator.PressureHalfLifeSeconds);
		Assert.AreEqual(0.0, generator.NaturalRepairPerMinute);
		Assert.IsNull(generator.IdleRecheckSeconds);
		Assert.AreEqual("1", SaveDefinition(generator).Attribute("version")!.Value);
		Assert.IsFalse(generator.HasOrganicConfiguration);
		Assert.AreEqual(0, generator.OrganicSources.Count);
		Assert.AreEqual(NativeOrganicPenaltyEvaluation.Neutral,
			generator.EvaluateOrganicPenalty(NativeOrganicPenaltyChannel.CropYieldRecovery,
				new Dictionary<string, double>()));
	}

	[TestMethod]
	public void Builder_OrganicSourcesAndPenalty_RoundTripWithOuterVersionUnchanged()
	{
		var world = World();
		var actor = Actor(world.Object);
		var generator = Load(world);
		var revision = generator.Revision;

		Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack("organic source add forage Wild Herbs")));
		Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack("organic source add crop")));
		Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack("organic source crop uses crop orchard")));
		Assert.IsTrue(generator.BuildingCommand(actor.Object,
			new StringStack("organic penalty cropyield 1 - scardamage / 10")));

		Assert.AreEqual(revision + 4, generator.Revision);
		var saved = SaveDefinition(generator);
		Assert.AreEqual("1", saved.Attribute("version")!.Value);
		Assert.AreEqual("1", saved.Element("Organic")!.Attribute("version")!.Value);
		var reloaded = Load(world, saved);
		Assert.AreEqual(0, reloaded.ValidationErrors.Count, string.Join("; ", reloaded.ValidationErrors));
		Assert.AreEqual(0, reloaded.OrganicValidationErrors.Count,
			string.Join("; ", reloaded.OrganicValidationErrors));
		Assert.AreEqual(2, reloaded.OrganicSources.Count);
		Assert.AreEqual("forage:wild herbs", reloaded.OrganicSources[0].Selector);
		CollectionAssert.AreEquivalent(new[] { AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard },
			reloaded.OrganicSources[1].AllowedFieldUses.ToArray());
		var evaluation = reloaded.EvaluateOrganicPenalty(NativeOrganicPenaltyChannel.CropYieldRecovery,
			new Dictionary<string, double> { ["scardamage"] = 2.0 });
		Assert.IsTrue(evaluation.IsValid, evaluation.Error);
		Assert.AreEqual(0.8, evaluation.Factor, 1e-12);
		StringAssert.Contains(reloaded.Show(actor.Object), "Ecological Penalties");
	}

	[TestMethod]
	public void Loader_InvalidOrganicFormula_DoesNotDisableLegacyManaDefinition()
	{
		var definition = Definition();
		definition.Add(new XElement("Organic", new XAttribute("version", 1),
			new XElement("Sources"),
			new XElement("Penalties", new XElement("Penalty",
				new XAttribute("channel", NativeOrganicPenaltyChannel.CropYieldRecovery), "2"))));
		var generator = Load(World(), definition);

		Assert.AreEqual(0, generator.ValidationErrors.Count, string.Join("; ", generator.ValidationErrors));
		Assert.IsTrue(generator.OrganicValidationErrors.Count > 0);
		Assert.IsTrue(generator.EvaluateOutput(generator.Outputs[0], new Dictionary<string, double>(), 0.0).IsValid);
		var penalty = generator.EvaluateOrganicPenalty(NativeOrganicPenaltyChannel.CropYieldRecovery,
			new Dictionary<string, double>());
		Assert.IsFalse(penalty.IsValid);
		Assert.AreEqual(0.0, penalty.Factor);
	}

	[TestMethod]
	[TestCategory("Y-T02")]
	[TestCategory("Y-T22")]
	public void Loader_DuplicatePenaltyFailsOnlyThatChannelClosed()
	{
		var definition = Definition();
		definition.Add(new XElement("Organic", new XAttribute("version", 1),
			new XElement("Sources"),
			new XElement("Penalties",
				new XElement("Penalty", new XAttribute("channel", NativeOrganicPenaltyChannel.ForageReplenishment), "0.5"),
				new XElement("Penalty", new XAttribute("channel", NativeOrganicPenaltyChannel.CropYieldRecovery), "0.25"),
				new XElement("Penalty", new XAttribute("channel", NativeOrganicPenaltyChannel.CropYieldRecovery), "0.75"))));
		var generator = Load(World(), definition);

		var duplicate = generator.EvaluateOrganicPenalty(NativeOrganicPenaltyChannel.CropYieldRecovery,
			new Dictionary<string, double>());
		Assert.IsFalse(duplicate.IsValid);
		Assert.AreEqual(0.0, duplicate.Factor);
		Assert.AreEqual(0, generator.RequiredOrganicInputNames(NativeOrganicPenaltyChannel.CropYieldRecovery).Count);
		var independent = generator.EvaluateOrganicPenalty(NativeOrganicPenaltyChannel.ForageReplenishment,
			new Dictionary<string, double>());
		Assert.IsTrue(independent.IsValid, independent.Error);
		Assert.AreEqual(0.5, independent.Factor);
	}

	[TestMethod]
	[TestCategory("Y-T02")]
	[TestCategory("Y-T22")]
	public void Loader_UnsupportedOrganicVersionDoesNotExecuteConfiguredPenalty()
	{
		var definition = Definition();
		definition.Add(new XElement("Organic", new XAttribute("version", 999),
			new XElement("Sources"),
			new XElement("Penalties", new XElement("Penalty",
				new XAttribute("channel", NativeOrganicPenaltyChannel.ForageReplenishment), "0.5"))));
		var generator = Load(World(), definition);

		var evaluation = generator.EvaluateOrganicPenalty(NativeOrganicPenaltyChannel.ForageReplenishment,
			new Dictionary<string, double>());

		Assert.IsFalse(evaluation.IsValid);
		Assert.AreEqual(0.0, evaluation.Factor);
		StringAssert.Contains(evaluation.Error, "Unsupported organic definition version 999");
		Assert.AreEqual(0, generator.RequiredOrganicInputNames(NativeOrganicPenaltyChannel.ForageReplenishment).Count);
	}

	[TestMethod]
	public void Loader_OrganicLimitsDuplicatesAndMalformedSelectorsStaySeparateFromLegacyMana()
	{
		var definition = Definition();
		var sources = new XElement("Sources");
		for (var i = 0; i <= EnvironmentalMagicGenerator.MaximumOrganicSources; i++)
		{
			sources.Add(new XElement("Source", new XAttribute("selector", $"forage:key{i}"),
				new XAttribute("kind", NativeOrganicSourceKind.Forage), new XAttribute("foragekey", $"key{i}"),
				new XElement("Uses"), new XElement("Definitions")));
		}
		sources.Add(new XElement("Source", new XAttribute("selector", "forage:key0"),
			new XAttribute("kind", NativeOrganicSourceKind.Forage), new XAttribute("foragekey", "key0"),
			new XElement("Uses"), new XElement("Definitions")));
		sources.Add(new XElement("Source", new XAttribute("selector", "animals"),
			new XAttribute("kind", "Unknown"), new XElement("Uses"), new XElement("Definitions")));
		definition.Add(new XElement("Organic", new XAttribute("version", 1), sources, new XElement("Penalties")));

		var generator = Load(World(), definition);
		var errors = string.Join("; ", generator.OrganicValidationErrors);
		Assert.AreEqual(0, generator.ValidationErrors.Count);
		StringAssert.Contains(errors, "at most 32");
		StringAssert.Contains(errors, "declared more than once");
		StringAssert.Contains(errors, "kind is unknown");
	}

	[TestMethod]
	public void Builder_OrganicNamedDependencyPreventsRemovingItsInput()
	{
		var world = World();
		var actor = Actor(world.Object);
		var generator = Load(world);

		Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack("input ecology forage herbs 0.01")));
		Assert.IsTrue(generator.BuildingCommand(actor.Object,
			new StringStack("organic penalty forage 1 - ecology")));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("input remove ecology")));
		Assert.AreEqual(1, generator.Inputs.Count);
		Assert.IsTrue(generator.RequiredOrganicInputNames(NativeOrganicPenaltyChannel.ForageReplenishment)
			.Contains("ECOLOGY"));
	}

	[DataTestMethod]
	[DataRow("nativestock")]
	[DataRow("nativehealth")]
	[DataRow("nativeyield")]
	[DataRow("nativecapacity")]
	[DataRow("fieldcondition")]
	[DataRow("baselineincrease")]
	[TestCategory("Y-T02")]
	public void Builder_OrganicBuiltInNamesCannotBeShadowedByNamedInputs(string name)
	{
		var generator = Load(World());
		var actor = Actor(generator.Gameworld);

		Assert.IsFalse(generator.BuildingCommand(actor.Object,
			new StringStack($"input {name} forage herbs 1")));
		Assert.AreEqual(0, generator.Inputs.Count);
	}

	[TestMethod]
	public void Builder_OrganicDeclarationsRejectDuplicatesAndPenaltyRejectsRandomOrOutOfRange()
	{
		var generator = Load(World());
		var actor = Actor(generator.Gameworld);

		Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack("organic source add crop")));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("organic source add crop")));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("organic source add forage")));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("organic penalty forage 1d6")));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("organic penalty forage 1.01")));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("organic penalty forage 1 / 0")));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("organic penalty forage 0 / 0")));
		Assert.AreEqual(1, generator.OrganicSources.Count);
		Assert.AreEqual(0, generator.OrganicPenalties.Count);
	}

	[TestMethod]
	[TestCategory("Y-T14")]
	public void OrganicPenalty_DynamicNonFiniteResultFailsClosed()
	{
		var generator = Load(World());
		var actor = Actor(generator.Gameworld);
		Assert.IsTrue(generator.BuildingCommand(actor.Object,
			new StringStack("organic penalty forage 1 / nativestock")));

		var evaluation = generator.EvaluateOrganicPenalty(
			NativeOrganicPenaltyChannel.ForageReplenishment,
			new Dictionary<string, double> { ["nativestock"] = 0.0 });

		Assert.IsFalse(evaluation.IsValid);
		Assert.AreEqual(0.0, evaluation.Factor);
		StringAssert.Contains(evaluation.Error, "finite");
	}

	[TestMethod]
	public void Builder_OrganicProtectionRequiresExactCompiledSignatureAndIsNeverInvoked()
	{
		var valid = ProtectionProg(20);
		var invalid = ProtectionProg(21);
		invalid.SetupGet(x => x.Parameters).Returns(new[] { ProgVariableTypes.Character });
		var world = World(progs: new[] { valid.Object, invalid.Object });
		var generator = Load(world);
		var actor = Actor(world.Object);

		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("organic protection 21")));
		Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack("organic protection 20")));
		Assert.AreEqual(20L, generator.OrganicProtectionProgId);
		Assert.AreSame(valid.Object, generator.OrganicProtectionProg);
		valid.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
		var reloaded = Load(world, SaveDefinition(generator));
		Assert.AreEqual(0, reloaded.OrganicValidationErrors.Count,
			string.Join("; ", reloaded.OrganicValidationErrors));
		Assert.AreEqual(20L, reloaded.OrganicProtectionProgId);
		valid.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	[TestCategory("Y-T02")]
	public void Loader_MalformedProtectionProgIsRetainedAndCanBeRepairedLive()
	{
		var definition = Definition();
		definition.Add(new XElement("Organic", new XAttribute("version", 1),
			new XElement("ProtectionProg", "not-a-prog-id"), new XElement("Sources"), new XElement("Penalties")));
		var world = World();
		var generator = Load(world, definition);

		StringAssert.Contains(string.Join("; ", generator.OrganicValidationErrors), "malformed");
		var saved = SaveDefinition(generator);
		Assert.AreEqual("not-a-prog-id", saved.Element("Organic")!.Element("ProtectionProg")!.Value);
		var reloaded = Load(world, saved);
		StringAssert.Contains(string.Join("; ", reloaded.OrganicValidationErrors), "malformed");

		Assert.IsTrue(reloaded.BuildingCommand(Actor(world.Object).Object,
			new StringStack("organic protection none")));
		Assert.AreEqual(0, reloaded.OrganicValidationErrors.Count,
			string.Join("; ", reloaded.OrganicValidationErrors));
		Assert.IsNull(SaveDefinition(reloaded).Element("Organic")!.Element("ProtectionProg"));
	}

	[TestMethod]
	public void Builder_EnvironmentalCreationType_ReachesResourceValidation()
	{
		var actor = Actor(World().Object);
		var output = Mock.Get(actor.Object.OutputHandler);
		Assert.IsNull(BaseMagicResourceGenerator.LoadFromBuilderInput(actor.Object,
			new StringStack("environmental")));
		output.Verify(handler => handler.Send("What magic resource should this regenerator be tied to?", true, false),
			Times.Once);
		Assert.IsNull(BaseMagicResourceGenerator.LoadFromBuilderInput(actor.Object,
			new StringStack("environmental 999 Test Profile")));
		output.Verify(handler => handler.Send("There is no such magic resource.", true, false), Times.Once);
	}

	[TestMethod]
	public void Builder_OutputAndInputEdits_RoundTripAndPreserveIndependentDefinitions()
	{
		var world = World();
		var actor = Actor(world.Object);
		var original = Load(world);
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("input herbs forage herbs 0.25")));
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("output 1 maximum basecapacity + herbs")));
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("output 1 rate baserate * (maximum - balance) / maximum")));
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("output 1 basecapacity 200")));
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("output add 2")));
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("output 2 baserate 3")));
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("repair 0.5")));
		Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack("idle 120")));
		var model = Model(SaveDefinition(original));
		var reloaded = (EnvironmentalMagicGenerator)BaseMagicResourceGenerator.LoadFromDatabase(model, world.Object);
		Assert.AreEqual(0, reloaded.ValidationErrors.Count, string.Join("; ", reloaded.ValidationErrors));
		Assert.AreEqual(2, reloaded.Outputs.Count);
		Assert.AreEqual(200.0, reloaded.Outputs[0].BaseCapacity);
		Assert.AreEqual(3.0, reloaded.Outputs[1].BaseRate);
		Assert.AreEqual(0.25, reloaded.Inputs[0].Scale);
		Assert.AreEqual(0.5, reloaded.NaturalRepairPerMinute);
		Assert.AreEqual(120.0, reloaded.IdleRecheckSeconds);
		Assert.IsTrue(reloaded.RequiredInputNames.Contains("HERBS"));
		Assert.IsTrue(reloaded.BuildingCommand(actor.Object, new StringStack("output 1 basecapacity 300")));
		Assert.AreEqual(200.0, original.Outputs[0].BaseCapacity);
		var show = reloaded.Show(actor.Object);
		StringAssert.Contains(show, "Named Input Bindings");
		StringAssert.Contains(show, "Resource Outputs");
		StringAssert.Contains(show, "Validation");
		StringAssert.Contains(show, "herbs");
	}

	[TestMethod]
	public void Builder_HalfLifeTransition_IntegratesOldAndNewIntervalsAndSurvivesReload()
	{
		var now = new DateTimeOffset(2026, 9, 13, 0, 0, 0, TimeSpan.Zero);
		var service = new Mock<IEnvironmentalMagicService>();
		service.SetupGet(value => value.UtcNow).Returns(() => now);
		var world = World();
		world.SetupGet(value => value.EnvironmentalMagic).Returns(service.Object);
		var definition = Definition();
		definition.Add(new XElement("DecayReferenceUtc", now.ToString("O")), new XElement("DecayIntegral", 0.0));
		var generator = Load(world, definition);
		var actor = Actor(world.Object);
		var oldAnchor = generator.PressureDecayIntegralAt(now);
		now += TimeSpan.FromMinutes(30);
		var observedBefore = 0.0;
		service.Setup(value => value.BeforeProfileChange(generator)).Callback(() => observedBefore = generator.PressureHalfLifeSeconds);
		Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack("halflife 1800")));
		Assert.AreEqual(3600.0, observedBefore);
		Assert.AreEqual(0.5, generator.DecayIntegral);
		now += TimeSpan.FromMinutes(30);
		Assert.AreEqual(1.5, generator.PressureDecayIntegralAt(now) - oldAnchor, 1e-12);
		var reloaded = Load(world, SaveDefinition(generator));
		Assert.AreEqual(generator.DecayReferenceUtc, reloaded.DecayReferenceUtc);
		Assert.AreEqual(1.5, reloaded.PressureDecayIntegralAt(now) - oldAnchor, 1e-12);
		service.Verify(value => value.BeforeProfileChange(generator), Times.Once);
		service.Verify(value => value.ProfileChanged(generator), Times.Once);
	}

	[TestMethod]
	public void Builder_EachSuccessfulDefinitionEdit_NotifiesBeforeAndAfterWithANewRevision()
	{
		var world = World();
		var service = new Mock<IEnvironmentalMagicService>();
		service.SetupGet(value => value.UtcNow).Returns(DateTimeOffset.UtcNow);
		world.SetupGet(value => value.EnvironmentalMagic).Returns(service.Object);
		var generator = Load(world);
		var actor = Actor(world.Object);
		var commands = new[] { "name New Name", "repair 1", "idle default", "output 1 rate 2", "input grass forage grass 1" };
		foreach (var command in commands)
		{
			var revision = generator.Revision;
			Assert.IsTrue(generator.BuildingCommand(actor.Object, new StringStack(command)), command);
			Assert.AreEqual(revision + 1, generator.Revision);
		}
		service.Verify(value => value.BeforeProfileChange(generator), Times.Exactly(commands.Length));
		service.Verify(value => value.ProfileChanged(generator), Times.Exactly(commands.Length));
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("repair NaN")));
		service.Verify(value => value.BeforeProfileChange(generator), Times.Exactly(commands.Length));
	}

	[DataTestMethod]
	[DataRow("maximum + 1", "baserate", "rate-only")]
	[DataRow("balance + 1", "baserate", "rate-only")]
	[DataRow("basecapacity", "undeclared", "not been declared")]
	[DataRow("1 +", "baserate", "maximum")]
	[DataRow("0 / 0", "baserate", "maximum")]
	[DataRow("1.0 / 0.0", "baserate", "maximum")]
	[DataRow("-1", "baserate", "negative")]
	[DataRow("basecapacity", "-1", "negative")]
	[DataRow("if(true, 0, missingfunction(1))", "baserate", "Unknown function")]
	[DataRow("1d6", "baserate", "Random function")]
	[DataRow("basecapacity", "drand(0, 1)", "Random function")]
	public void Loader_InvalidFormula_DisablesProfileWithDiagnostics(string maximum, string rate, string diagnostic)
	{
		var definition = Definition(maximum, rate);
		var generator = Load(World(), definition);
		Assert.IsTrue(generator.ValidationErrors.Count > 0);
		StringAssert.Contains(string.Join("; ", generator.ValidationErrors), diagnostic);
		Assert.IsFalse(generator.EvaluateOutput(generator.Outputs[0], new Dictionary<string, double>(), 50.0).IsValid);
	}

	[DataTestMethod]
	[DataRow("-scardamage", "baserate")]
	[DataRow("basecapacity / pressure", "baserate")]
	[DataRow("basecapacity", "sqrt(-scardamage)")]
	[DataRow("basecapacity", "-scardamage")]
	public void EvaluateOutput_InvalidDynamicResult_ReportsFailure(string maximum, string rate)
	{
		var generator = Load(World(), Definition(maximum, rate));
		Assert.AreEqual(0, generator.ValidationErrors.Count);
		var result = generator.EvaluateOutput(generator.Outputs[0], new Dictionary<string, double>
		{
			["scardamage"] = 10.0, ["pressure"] = 0.0
		}, 50.0);
		Assert.IsFalse(result.IsValid);
		Assert.IsFalse(string.IsNullOrWhiteSpace(result.Error));
	}

	[TestMethod]
	public void EvaluateOutput_ComputesMaximumBeforeRateAndDoesNotResolveReferencesOrRunInputProgs()
	{
		var prog = NumericProg(10);
		var world = World(progs: new[] { prog.Object });
		var definition = Definition("basecapacity + policy - scardamage", "(maximum - balance) * baserate");
		definition.Element("Inputs")!.Add(new XElement("Input", new XAttribute("name", "policy"),
			new XAttribute("kind", "Prog"), new XAttribute("source", "10"), new XAttribute("prog", 10), new XAttribute("scale", 2.0)));
		var generator = Load(world, definition);
		var resources = Mock.Get(world.Object.MagicResources);
		var progs = Mock.Get(world.Object.FutureProgs);
		resources.Invocations.Clear();
		progs.Invocations.Clear();
		for (var i = 0; i < 50; i++)
		{
			var result = generator.EvaluateOutput(generator.Outputs[0], new Dictionary<string, double>
			{
				["policy"] = 30.0, ["scardamage"] = 20.0
			}, 10.0);
			Assert.IsTrue(result.IsValid, result.Error);
			Assert.AreEqual(110.0, result.Maximum);
			Assert.AreEqual(100.0, result.Rate);
		}
		resources.Verify(value => value.Get(It.IsAny<long>()), Times.Never);
		progs.Verify(value => value.Get(It.IsAny<long>()), Times.Never);
		prog.Verify(value => value.Execute(It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void EvaluateOutput_ExplicitZeroIsValidButMissingOrNonFiniteInputIsInvalid()
	{
		var zero = Load(World(), Definition("0", "0"));
		Assert.IsTrue(zero.EvaluateOutput(zero.Outputs[0], new Dictionary<string, double>(), 50.0).IsValid);
		var generator = Load(World(), Definition("basecapacity - scardamage", "baserate"));
		Assert.IsFalse(generator.EvaluateOutput(generator.Outputs[0], new Dictionary<string, double>(), 50.0).IsValid);
		Assert.IsFalse(generator.EvaluateOutput(generator.Outputs[0], new Dictionary<string, double>
		{
			["scardamage"] = double.NaN
		}, 50.0).IsValid);
	}

	[TestMethod]
	public void Loader_MissingDuplicateAndUnsupportedResource_ProvidesExplicitDiagnostics()
	{
		var definition = Definition();
		definition.Element("Outputs")!.Add(new XElement(definition.Element("Outputs")!.Element("Output")!));
		definition.Element("Outputs")!.Add(new XElement("Output", new XAttribute("resource", 999),
			new XElement("Maximum", "10"), new XElement("Rate", "1")));
		var invalid = Load(World(), definition);
		StringAssert.Contains(string.Join("; ", invalid.ValidationErrors), "duplicate");
		StringAssert.Contains(string.Join("; ", invalid.ValidationErrors), "#999 does not exist");
		var unsupportedResource = Resource(1, MagicResourceType.PlayerResource);
		invalid = Load(World(resources: new[] { unsupportedResource.Object }));
		StringAssert.Contains(string.Join("; ", invalid.ValidationErrors), "does not support location");
	}

	[TestMethod]
	public void Loader_UnknownVersionAndMalformedXml_DisablesInsteadOfSubstituting()
	{
		var definition = Definition();
		definition.SetAttributeValue("version", 999);
		var invalid = Load(World(), definition);
		StringAssert.Contains(string.Join("; ", invalid.ValidationErrors), "version 999");
		Assert.AreEqual("999", SaveDefinition(invalid).Attribute("version")!.Value);
		var model = Model();
		model.Definition = "not xml";
		invalid = (EnvironmentalMagicGenerator)BaseMagicResourceGenerator.LoadFromDatabase(model, World().Object);
		Assert.IsTrue(invalid.ValidationErrors.Count > 0);
		Assert.AreEqual(0, invalid.Outputs.Count);
	}

	[TestMethod]
	public void Loader_OversizedDefinitionsAndInvalidBindings_AreInactive()
	{
		var definition = Definition();
		for (var i = 0; i < EnvironmentalMagicGenerator.MaximumOutputs; i++)
		{
			definition.Element("Outputs")!.Add(new XElement(definition.Element("Outputs")!.Element("Output")!));
		}
		for (var i = 0; i <= EnvironmentalMagicGenerator.MaximumInputs; i++)
		{
			definition.Element("Inputs")!.Add(new XElement("Input", new XAttribute("name", $"field{i}"),
				new XAttribute("kind", "Agriculture"), new XAttribute("source", "notasource"), new XAttribute("scale", "NaN")));
		}
		var generator = Load(World(), definition);
		StringAssert.Contains(string.Join("; ", generator.ValidationErrors), "1 to 8");
		StringAssert.Contains(string.Join("; ", generator.ValidationErrors), "at most 32");
		StringAssert.Contains(string.Join("; ", generator.ValidationErrors), "finite explicit scale");
	}

	[TestMethod]
	public void Builder_ProgInput_RejectsCachedProgsWithoutChangingTheirStaticSetting()
	{
		var prog = NumericProg(10);
		prog.SetupGet(value => value.StaticType).Returns(FutureProgStaticType.StaticByParameters);
		var world = World(progs: new[] { prog.Object });
		var generator = Load(world);
		var actor = Actor(world.Object);
		Assert.IsFalse(generator.BuildingCommand(actor.Object, new StringStack("input policy prog 10 1")));
		Assert.AreEqual(0, generator.Inputs.Count);
		prog.VerifySet(value => value.StaticType = It.IsAny<FutureProgStaticType>(), Times.Never);
		var definition = Definition();
		definition.Element("Inputs")!.Add(new XElement("Input", new XAttribute("name", "policy"),
			new XAttribute("kind", "Prog"), new XAttribute("source", "10"), new XAttribute("scale", 1.0)));
		var reloaded = Load(world, definition);
		StringAssert.Contains(string.Join("; ", reloaded.ValidationErrors), "NotStatic");
		prog.SetupGet(value => value.StaticType).Returns(FutureProgStaticType.NotStatic);
		reloaded.RefreshReferences();
		Assert.AreEqual(0, reloaded.ValidationErrors.Count);
	}

	[TestMethod]
	public void MinuteDelegate_AllHolderKinds_RejectBeforeAddingCacheEntries()
	{
		var generator = Load(World());
		IHaveMagicResource[] holders = { Mock.Of<ICell>(), Mock.Of<ICharacter>(), Mock.Of<IGameItem>() };
		foreach (var holder in holders)
		{
			var exception = Assert.ThrowsException<InvalidOperationException>(() => generator.GetOnMinuteDelegate(holder));
			StringAssert.Contains(exception.Message, holder is ICell ? "centrally coordinated" : "physical cells only");
		}
		var cache = (IDictionary)typeof(BaseMagicResourceGenerator).GetField("_delegates", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(generator)!;
		Assert.AreEqual(0, cache.Count);
	}

	[TestMethod]
	public void SourceReferenceRefresh_ReusesParsedExpressionsWithoutChangingDefinitionRevision()
	{
		var world = World();
		var generator = Load(world);
		var revision = generator.Revision;
		var outputs = (IDictionary)typeof(EnvironmentalMagicGenerator)
			.GetField("_compiledOutputs", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(generator)!;
		var original = outputs[generator.Outputs[0].ResourceId]!;
		var maximum = original.GetType().GetProperty("Maximum")!.GetValue(original);
		var rate = original.GetType().GetProperty("Rate")!.GetValue(original);
		for (var i = 0; i < 10; i++) generator.RefreshReferences();
		var refreshed = outputs[generator.Outputs[0].ResourceId]!;
		Assert.AreSame(maximum, refreshed.GetType().GetProperty("Maximum")!.GetValue(refreshed));
		Assert.AreSame(rate, refreshed.GetType().GetProperty("Rate")!.GetValue(refreshed));
		Assert.AreEqual(revision, generator.Revision);
		Assert.IsFalse(generator.Changed);
	}

	private static XElement Definition(string maximum = "basecapacity", string rate = "baserate") => new("Definition",
		new XAttribute("version", 1), new XElement("PressureHalfLifeSeconds", 3600.0),
		new XElement("NaturalRepairPerMinute", 0.0),
		new XElement("Outputs", new XElement("Output", new XAttribute("resource", 1),
			new XAttribute("basecapacity", 100.0), new XAttribute("baserate", 1.0),
			new XElement("Maximum", maximum), new XElement("Rate", rate))), new XElement("Inputs"));

	private static MagicGenerator Model(XElement? definition = null) => new()
	{
		Id = 44, Name = "Test Environment", Type = "environmental", Definition = (definition ?? Definition()).ToString()
	};

	private static EnvironmentalMagicGenerator Load(Mock<IFuturemud> world, XElement? definition = null) =>
		(EnvironmentalMagicGenerator)BaseMagicResourceGenerator.LoadFromDatabase(Model(definition), world.Object);

	private static XElement SaveDefinition(EnvironmentalMagicGenerator generator) =>
		(XElement)typeof(EnvironmentalMagicGenerator).GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(generator, null)!;

	private static Mock<IMagicResource> Resource(long id, MagicResourceType type = MagicResourceType.LocationResource)
	{
		var resource = new Mock<IMagicResource>();
		resource.SetupGet(value => value.Id).Returns(id);
		resource.SetupGet(value => value.Name).Returns($"Resource{id}");
		resource.SetupGet(value => value.ResourceType).Returns(type);
		return resource;
	}

	private static Mock<IFutureProg> NumericProg(long id)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(value => value.Id).Returns(id);
		prog.SetupGet(value => value.Name).Returns($"Prog{id}");
		prog.SetupGet(value => value.FunctionName).Returns($"Prog{id}");
		prog.SetupGet(value => value.ReturnType).Returns(ProgVariableTypes.Number);
		prog.SetupGet(value => value.StaticType).Returns(FutureProgStaticType.NotStatic);
		prog.Setup(value => value.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
			.Returns<IEnumerable<ProgVariableTypes>>(parameters => parameters.SequenceEqual(new[] { ProgVariableTypes.Location }));
		return prog;
	}

	private static Mock<IFutureProg> ProtectionProg(long id)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(value => value.Id).Returns(id);
		prog.SetupGet(value => value.Name).Returns($"Protection{id}");
		prog.SetupGet(value => value.FunctionName).Returns($"Protection{id}");
		prog.SetupGet(value => value.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(value => value.Parameters).Returns(new[]
		{
			ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.MagicCapability,
			ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Location
		});
		return prog;
	}

	private static Mock<IFuturemud> World(IMagicResource[]? resources = null, IFutureProg[]? progs = null)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(value => value.MagicResources).Returns(Collection(resources ?? new[] { Resource(1).Object, Resource(2).Object }).Object);
		world.SetupGet(value => value.FutureProgs).Returns(Collection(progs ?? Array.Empty<IFutureProg>()).Object);
		world.SetupGet(value => value.MagicResourceRegenerators).Returns(Collection(Array.Empty<IMagicResourceRegenerator>()).Object);
		world.SetupGet(value => value.SaveManager).Returns(Mock.Of<ISaveManager>());
		return world;
	}

	private static Mock<ICharacter> Actor(IFuturemud world)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(value => value.Gameworld).Returns(world);
		actor.SetupGet(value => value.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(value => value.GetFormat(It.IsAny<Type>())).Returns<Type>(CultureInfo.InvariantCulture.GetFormat);
		var account = new Mock<IAccount>();
		account.SetupGet(value => value.InnerLineFormatLength).Returns(100);
		actor.SetupGet(value => value.Account).Returns(account.Object);
		return actor;
	}

	private static Mock<IUneditableAll<T>> Collection<T>(T[] items) where T : class, IFrameworkItem
	{
		var byId = items.ToDictionary(item => item.Id);
		var collection = new Mock<IUneditableAll<T>>();
		collection.Setup(value => value.Get(It.IsAny<long>())).Returns<long>(id => byId.GetValueOrDefault(id));
		collection.Setup(value => value.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((name, _) =>
			long.TryParse(name, out var id) ? byId.GetValueOrDefault(id) : items.FirstOrDefault(item => item.Name.EqualTo(name)));
		collection.Setup(value => value.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		collection.SetupGet(value => value.Count).Returns(items.Length);
		return collection;
	}
}

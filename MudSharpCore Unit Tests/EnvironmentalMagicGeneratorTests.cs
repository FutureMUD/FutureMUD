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

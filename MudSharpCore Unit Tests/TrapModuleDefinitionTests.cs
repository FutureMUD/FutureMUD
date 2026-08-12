#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Traps;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Movement;
using MudSharp.Traps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TrapModuleDefinitionTests
{
	[TestMethod]
	public void CreateTrapFunction_ExtractSuppliedItems_UnwrapsFutureProgCollections()
	{
		var item = new Mock<IGameItem>();
		var collection = new CollectionVariable(new List<IGameItem> { item.Object }, ProgVariableTypes.Item);

		var result = CreateTrapFunction.ExtractSuppliedItems(collection.GetObject);

		Assert.AreEqual(1, result.Count);
		Assert.AreSame(item.Object, result[0]);
	}

	[TestMethod]
	public void TriggerDefinition_SaveAndLoad_PreservesTypeAndParameters()
	{
		var trigger = new TrapTriggerDefinition(TrapTriggerType.Proximity);
		trigger.SetParameter("chance", "67.5");
		trigger.SetParameter("filterprog", "42");
		trigger.SetParameter("maximumproximity", "Immediate");

		var loaded = TrapTriggerDefinition.LoadFromXml(XElement.Parse(trigger.SaveToXml()));

		Assert.AreEqual(TrapTriggerType.Proximity, loaded.TriggerType);
		Assert.AreEqual("67.5", loaded.Parameters["chance"]);
		Assert.AreEqual("42", loaded.Parameters["filterprog"]);
		Assert.AreEqual("Immediate", loaded.Parameters["maximumproximity"]);
		Assert.IsTrue(loaded.CompatibleSourceKinds.Contains(TrapSourceKind.Natural));
	}

	[TestMethod]
	public void ExitTraversal_AdvertisesAndParsesMovementAndSizeParameters()
	{
		var parameters = TrapTriggerDefinition.ParametersFor(TrapTriggerType.ExitTraversal)
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		CollectionAssert.IsSubsetOf(
			new[] { "chance", "spotdifficulty", "movementtypes", "minimumsize", "maximumsize" },
			parameters.ToList());
		Assert.IsTrue(TrapTemplate.TryParseMovementTypes("upright, flying", out var parsed));
		Assert.IsTrue(parsed.HasFlag(MovementType.Upright));
		Assert.IsTrue(parsed.HasFlag(MovementType.Flying));
		Assert.IsFalse(parsed.HasFlag(MovementType.Floating));
		Assert.IsFalse(TrapTemplate.TryParseMovementTypes("upright, teleporting", out _));
	}

	[TestMethod]
	public void ExitBoundTrap_PersistsExitAndOriginIdentity()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		cell.SetupGet(x => x.Id).Returns(100L);
		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.Id).Returns(200L);
		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Exit).Returns(exit.Object);
		cellExit.SetupGet(x => x.Origin).Returns(cell.Object);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);

		var trap = new TrapEffect(cell.Object, template.Object, null, cellExit.Object);
		var xml = trap.SaveToXml(new Dictionary<IEffect, TimeSpan>());

		Assert.AreEqual(200L, trap.BoundExitId);
		Assert.AreEqual(100L, trap.BoundExitOriginId);
		Assert.AreEqual("200", xml.Descendants("BoundExitId").Single().Value);
		Assert.AreEqual("100", xml.Descendants("BoundExitOriginId").Single().Value);
	}

	[TestMethod]
	public void PayloadDefinition_SaveAndLoad_PreservesDelayTargetAndParameters()
	{
		var payload = new TrapPayloadDefinition(
			TrapPayloadType.Restraint,
			TimeSpan.FromSeconds(12),
			TrapTargetSelector.AnchorOccupants);
		payload.SetParameter("duration", "00:00:30");
		payload.SetParameter("description", "entangled in webbing");

		var loaded = TrapPayloadDefinition.LoadFromXml(XElement.Parse(payload.SaveToXml()));

		Assert.AreEqual(TrapPayloadType.Restraint, loaded.PayloadType);
		Assert.AreEqual(TimeSpan.FromSeconds(12), loaded.Delay);
		Assert.AreEqual(TrapTargetSelector.AnchorOccupants, loaded.TargetSelector);
		Assert.AreEqual("entangled in webbing", loaded.Parameters["description"]);
		Assert.IsTrue(loaded.CompatibleSourceKinds.Contains(TrapSourceKind.Natural));
	}

	[TestMethod]
	public void ComponentRequirement_SaveAndLoad_PreservesTagRolesRecoveryAndQualityWeight()
	{
		var gameworld = new Mock<IFuturemud>();
		var requirement = new TrapComponentRequirementDefinition(gameworld.Object, 42L,
			TrapComponentRole.TriggerAndPayload, 65.0, 1.5);

		var loaded = TrapComponentRequirementDefinition.LoadFromXml(requirement.SaveToXml(), gameworld.Object);

		Assert.AreEqual(42L, loaded.TagId);
		Assert.AreEqual(TrapComponentRole.TriggerAndPayload, loaded.Role);
		Assert.AreEqual(65.0, loaded.SpentRecoveryChance);
		Assert.AreEqual(1.5, loaded.QualityWeight);
	}

	[TestMethod]
	public void DeployedTrap_PersistsPhysicalComponentBindings()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(123L);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		var binding = new TrapComponentBinding(gameworld.Object, item.Object,
			TrapComponentRole.TriggerAndPayload, 80.0, 2.0);

		var trap = new TrapEffect(cell.Object, template.Object, components: [binding]);
		var xml = trap.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var component = xml.Descendants("Component").Single();

		Assert.AreEqual(123L, trap.Components.Single().ItemId);
		Assert.AreEqual("123", component.Attribute("item")?.Value);
		Assert.AreEqual("TriggerAndPayload", component.Attribute("role")?.Value);
		Assert.AreEqual("80", component.Attribute("recovery")?.Value);
	}

	[TestMethod]
	public void ComponentMatcher_AllowsOneItemToServeTriggerAndPayloadButNotDuplicateRoles()
	{
		var gameworld = new Mock<IFuturemud>();
		var triggerTag = new Mock<ITag>();
		var payloadTag = new Mock<ITag>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		item.SetupGet(x => x.Id).Returns(99L);
		item.Setup(x => x.IsA(triggerTag.Object)).Returns(true);
		item.Setup(x => x.IsA(payloadTag.Object)).Returns(true);
		var triggerRequirement = new Mock<ITrapComponentRequirement>();
		triggerRequirement.SetupGet(x => x.Tag).Returns(triggerTag.Object);
		triggerRequirement.SetupGet(x => x.Role).Returns(TrapComponentRole.Trigger);
		triggerRequirement.SetupGet(x => x.SpentRecoveryChance).Returns(85.0);
		triggerRequirement.SetupGet(x => x.QualityWeight).Returns(1.0);
		var payloadRequirement = new Mock<ITrapComponentRequirement>();
		payloadRequirement.SetupGet(x => x.Tag).Returns(payloadTag.Object);
		payloadRequirement.SetupGet(x => x.Role).Returns(TrapComponentRole.Payload);
		payloadRequirement.SetupGet(x => x.SpentRecoveryChance).Returns(0.0);
		payloadRequirement.SetupGet(x => x.QualityWeight).Returns(1.0);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.SourceKind).Returns(TrapSourceKind.Mechanical);
		template.SetupGet(x => x.ComponentRequirements).Returns([triggerRequirement.Object, payloadRequirement.Object]);

		Assert.IsTrue(TrapEffect.TryBindComponents(template.Object, [item.Object], out var bindings));
		Assert.AreEqual(1, bindings.Count);
		Assert.AreEqual(TrapComponentRole.TriggerAndPayload, bindings[0].Role);
		Assert.AreEqual(0.0, bindings[0].SpentRecoveryChance);

		var secondTriggerRequirement = new Mock<ITrapComponentRequirement>();
		secondTriggerRequirement.SetupGet(x => x.Tag).Returns(triggerTag.Object);
		secondTriggerRequirement.SetupGet(x => x.Role).Returns(TrapComponentRole.Trigger);
		secondTriggerRequirement.SetupGet(x => x.SpentRecoveryChance).Returns(85.0);
		secondTriggerRequirement.SetupGet(x => x.QualityWeight).Returns(1.0);
		template.SetupGet(x => x.ComponentRequirements).Returns([triggerRequirement.Object, secondTriggerRequirement.Object]);
		Assert.IsFalse(TrapEffect.TryBindComponents(template.Object, [item.Object], out _));
	}

	[TestMethod]
	public void SignalModules_AreMechanicalOnly()
	{
		var signalTrigger = new TrapTriggerDefinition(TrapTriggerType.Signal);
		var signalPayload = new TrapPayloadDefinition(TrapPayloadType.EmitSignal);

		Assert.IsTrue(signalTrigger.CompatibleSourceKinds.SetEquals([TrapSourceKind.Mechanical]));
		Assert.IsTrue(signalPayload.CompatibleSourceKinds.SetEquals([TrapSourceKind.Mechanical]));
	}

	[TestMethod]
	public void CellArrivalTriggers_UseEntryWitnessInsteadOfOptionalCompletionWitness()
	{
		Assert.IsTrue(TrapEventRouting.IsCellArrivalWitness(EventType.CharacterEnterCellWitness));
		Assert.IsFalse(TrapEventRouting.IsCellArrivalWitness(EventType.CharacterEnterCellFinishWitness));
	}

	[TestMethod]
	public void ProximityTriggers_RequireANonCellAnchorForNewDeployments()
	{
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Triggers).Returns([new TrapTriggerDefinition(TrapTriggerType.Proximity)]);

		Assert.IsFalse(TrapEffect.IsValidAnchor(template.Object, new Mock<ICell>().Object));
		Assert.IsTrue(TrapEffect.IsValidAnchor(template.Object, new Mock<IGameItem>().Object));
	}

	[TestMethod]
	public void GasCloudDose_RequiresAnInhalableDrug()
	{
		var inhalableDrug = new Mock<IDrug>();
		inhalableDrug.SetupGet(x => x.DrugVectors).Returns(DrugVector.Inhaled);
		var inhalableGas = new Mock<IGas>();
		inhalableGas.SetupGet(x => x.Drug).Returns(inhalableDrug.Object);

		var injectedDrug = new Mock<IDrug>();
		injectedDrug.SetupGet(x => x.DrugVectors).Returns(DrugVector.Injected);
		var injectedGas = new Mock<IGas>();
		injectedGas.SetupGet(x => x.Drug).Returns(injectedDrug.Object);

		Assert.IsTrue(TrapGasCloudEffect.CanDose(inhalableGas.Object, 0.1));
		Assert.IsFalse(TrapGasCloudEffect.CanDose(injectedGas.Object, 0.1));
		Assert.IsFalse(TrapGasCloudEffect.CanDose(inhalableGas.Object, 0.0));
	}

	[TestMethod]
	public void TrapFutureProgAndMagicSurfaces_AreRegistered()
	{
		FutureProgTestBootstrap.EnsureInitialised();

		var functions = FutureProg.GetFunctionCompilerInformations()
			.Where(x => x.Category == "Traps")
			.Select(x => x.FunctionName)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(
			new[] { "trapat", "createtrap", "armtrap", "disarmtrap", "triggertrap" },
			functions.ToList());
		Assert.IsTrue(FutureProg.GetFunctionCompilerInformations()
			.Any(x => x.FunctionName == "triggertrap" && x.Parameters.Count() == 1 && x.ParameterNames.Count() == 1));
		Assert.IsTrue(FutureProg.GetFunctionCompilerInformations()
			.Any(x => x.FunctionName == "triggertrap" && x.Parameters.Count() == 2 && x.ParameterNames.Count() == 2));
		Assert.IsTrue(FutureProg.GetFunctionCompilerInformations()
			.Any(x => x.FunctionName == "createtrap" && x.Parameters.Count() == 4 &&
			          x.Parameters.Last() == (ProgVariableTypes.Item | ProgVariableTypes.Collection)));
		Assert.IsTrue(ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Trap]
			.PropertyTypeMap.ContainsKey("charges"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("removetrap"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("dispeltrap"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("createtrap"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("placetrap"));
	}
}

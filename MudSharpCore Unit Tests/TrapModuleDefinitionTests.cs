#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Traps;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TrapModuleDefinitionTests
{
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
		Assert.IsTrue(ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Trap]
			.PropertyTypeMap.ContainsKey("charges"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("removetrap"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("dispeltrap"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("createtrap"));
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("placetrap"));
	}
}

#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
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
using MudSharp.Magic.SpellEffects;
using MudSharp.Movement;
using MudSharp.Traps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

		var trap = new TrapEffect(cell.Object, template.Object, null, cellExit.Object, power: SpellPower.Strong);
		var xml = trap.SaveToXml(new Dictionary<IEffect, TimeSpan>());

		Assert.AreEqual(200L, trap.BoundExitId);
		Assert.AreEqual(100L, trap.BoundExitOriginId);
		Assert.AreEqual(SpellPower.Strong, trap.Power);
		Assert.AreEqual("200", xml.Descendants("BoundExitId").Single().Value);
		Assert.AreEqual("100", xml.Descendants("BoundExitOriginId").Single().Value);
		Assert.AreEqual("Strong", xml.Descendants("Power").Single().Value);
	}

	[TestMethod]
	public void TransientExitBoundTrap_PersistsStableKeyAndMatchesRebuiltExit()
	{
		var gameworld = new Mock<IFuturemud>();
		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Id).Returns(100L);
		origin.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Id).Returns(101L);
		var firstExit = new TransientExit(gameworld.Object, origin.Object, destination.Object, "enter", "portal",
			"portal", "a portal", "a portal", "through", "through", 1.0,
			stableKey: "test-portal:42");
		var rebuiltExit = new TransientExit(gameworld.Object, origin.Object, destination.Object, "enter", "portal",
			"portal", "a portal", "a portal", "through", "through", 1.0,
			stableKey: "test-portal:42");
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);

		var trap = new TrapEffect(origin.Object, template.Object, boundExit: firstExit.CellExitFor(origin.Object));
		var xml = trap.SaveToXml(new Dictionary<IEffect, TimeSpan>());

		Assert.AreNotEqual(firstExit.Id, rebuiltExit.Id);
		Assert.AreEqual("test-portal:42", trap.BoundTransientExitKey);
		Assert.AreEqual("test-portal:42", xml.Descendants("BoundTransientExitKey").Single().Value);
		Assert.IsTrue(trap.MatchesExit(rebuiltExit.CellExitFor(origin.Object)!));
	}

	[TestMethod]
	public void TransientExitReplacement_RebindsTrapAndTrueRemovalDestroysInstalledComponents()
	{
		var gameworld = new Mock<IFuturemud>();
		var manager = new ExitManager(gameworld.Object);
		gameworld.SetupGet(x => x.ExitManager).Returns(manager);
		var templates = new RevisableAll<ITrapTemplate>();
		gameworld.SetupGet(x => x.TrapTemplates).Returns(templates);
		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Id).Returns(100L);
		origin.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		origin.SetupProperty(x => x.EffectsChanged);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Id).Returns(101L);
		var component = new Mock<IGameItem>();
		component.SetupGet(x => x.Id).Returns(500L);
		component.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		component.SetupGet(x => x.Deleted).Returns(false);
		component.Setup(x => x.EffectsOfType<TrapComponentReservationEffect>(
			It.IsAny<Predicate<TrapComponentReservationEffect>>())).Returns([]);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		template.SetupGet(x => x.Triggers).Returns([]);
		templates.Add(template.Object);
		var binding = new TrapComponentBinding(gameworld.Object, component.Object,
			TrapComponentRole.TriggerAndPayload, 50.0, 1.0);
		var firstExit = new TransientExit(gameworld.Object, origin.Object, destination.Object, "enter", "portal",
			"portal", "a portal", "a portal", "through", "through", 1.0,
			stableKey: "test-portal:42");
		manager.RegisterTransientExit(firstExit);
		var trap = new TrapEffect(origin.Object, template.Object, boundExit: firstExit.CellExitFor(origin.Object),
			components: [binding]);
		origin.Setup(x => x.RemoveEffect(trap, true)).Callback(trap.RemovalEffect);
		trap.InitialEffect();
		var replacementExit = new TransientExit(gameworld.Object, origin.Object, destination.Object, "enter", "portal",
			"portal", "a portal", "a portal", "through", "through", 1.0,
			stableKey: "test-portal:42");

		Assert.IsTrue(manager.ReplaceTransientExit(firstExit, replacementExit));
		Assert.AreEqual(replacementExit.Id, trap.BoundExitId);
		origin.Verify(x => x.RemoveEffect(trap, true), Times.Never);

		manager.UnregisterTransientExit(replacementExit);

		origin.Verify(x => x.RemoveEffect(trap, true), Times.Once);
		component.Verify(x => x.Delete(), Times.Once);
		origin.Verify(x => x.RemoveAllEffects<TrapPayloadScheduleEffect>(
			It.IsAny<Predicate<TrapPayloadScheduleEffect>>(), true), Times.Once);
		origin.Verify(x => x.RemoveAllEffects<TrapResetEffect>(
			It.IsAny<Predicate<TrapResetEffect>>(), true), Times.Once);
		origin.Verify(x => x.RemoveAllEffects<TrapSpentCleanupEffect>(
			It.IsAny<Predicate<TrapSpentCleanupEffect>>(), true), Times.Once);
	}

	[TestMethod]
	public void TransientExitReplacement_WithChangedEndpointIsLogicalRemoval()
	{
		var gameworld = new Mock<IFuturemud>();
		var manager = new ExitManager(gameworld.Object);
		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Id).Returns(100L);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Id).Returns(101L);
		var movedDestination = new Mock<ICell>();
		movedDestination.SetupGet(x => x.Id).Returns(102L);
		var firstExit = new TransientExit(gameworld.Object, origin.Object, destination.Object, "enter", "portal",
			"portal", "a portal", "a portal", "through", "through", 1.0,
			stableKey: "test-portal:42");
		var replacementExit = new TransientExit(gameworld.Object, origin.Object, movedDestination.Object, "enter",
			"portal", "portal", "a portal", "a portal", "through", "through", 1.0,
			stableKey: "test-portal:42");
		var removed = 0;
		manager.TransientExitUnregistered += _ => removed++;
		manager.RegisterTransientExit(firstExit);

		Assert.IsFalse(manager.ReplaceTransientExit(firstExit, replacementExit));
		Assert.AreEqual(1, removed);
		Assert.AreSame(replacementExit, manager.GetExitByID(replacementExit.Id));
	}

	[TestMethod]
	public void LegacyNegativeExitBinding_IsDestructivelyRemovedDuringReconciliation()
	{
		var gameworld = new Mock<IFuturemud>();
		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Id).Returns(100L);
		origin.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var legacyExit = new Mock<IExit>();
		legacyExit.SetupGet(x => x.Id).Returns(-1L);
		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Exit).Returns(legacyExit.Object);
		cellExit.SetupGet(x => x.Origin).Returns(origin.Object);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		var trap = new TrapEffect(origin.Object, template.Object, boundExit: cellExit.Object);
		origin.Setup(x => x.RemoveEffect(trap, true)).Callback(trap.RemovalEffect);

		Assert.IsFalse(trap.ReconcileTransientExitBinding());
		origin.Verify(x => x.RemoveEffect(trap, true), Times.Once);
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
	public void PayloadDefinition_LoadFromXml_XmlSchemaZeroDelayLoadsAsZero()
	{
		var loaded = TrapPayloadDefinition.LoadFromXml(
			XElement.Parse("<Payload type=\"DetonateItem\" delay=\"PT0S\" target=\"Triggerer\" />"));

		Assert.AreEqual(TimeSpan.Zero, loaded.Delay);
		Assert.AreEqual(TrapPayloadType.DetonateItem, loaded.PayloadType);
	}

	[TestMethod]
	public void MalformedTrapDefinitions_PreserveValuesAndFailClosed()
	{
		var malformedTrigger = TrapTriggerDefinition.LoadFromXml(
			XElement.Parse("<Trigger type=\"Openable\"><Parameter name=\"chance\">101</Parameter></Trigger>"));
		Assert.AreEqual("101", malformedTrigger.Parameters["chance"]);
		Assert.IsFalse(TrapTriggerDefinition.TryValidateParameters(malformedTrigger.TriggerType,
			malformedTrigger.Parameters, out _));

		var malformedPayload = TrapPayloadDefinition.LoadFromXml(
			XElement.Parse("<Payload type=\"DirectDamage\" delay=\"not-a-timespan\" target=\"NotASelector\"><Parameter name=\"damage\">traitbonus</Parameter></Payload>"));
		Assert.AreEqual("traitbonus", malformedPayload.Parameters["damage"]);
		Assert.IsTrue(malformedPayload.Delay < TimeSpan.Zero);
		Assert.IsFalse(Enum.IsDefined(malformedPayload.TargetSelector));
		Assert.IsFalse(TrapPayloadDefinition.TryValidateParameters(malformedPayload.PayloadType,
			malformedPayload.Parameters, out _));

		Assert.IsFalse(TrapTriggerDefinition.TryValidateParameters((TrapTriggerType)999,
			new Dictionary<string, string>(), out _));
		Assert.IsFalse(TrapPayloadDefinition.TryValidateParameters((TrapPayloadType)999,
			new Dictionary<string, string>(), out _));
		Assert.IsFalse(new TrapPayloadDefinition((TrapPayloadType)999).CompatibleSourceKinds.Any());
	}

	[TestMethod]
	public void PayloadDefinitions_AdvertiseOnlySupportedParametersWithGuidance()
	{
		var damageParameters = TrapPayloadDefinition.ParametersFor(TrapPayloadType.DirectDamage)
			.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		CollectionAssert.AreEquivalent(
			new[] { "echo", "damage", "pain", "stun", "damagetype" },
			damageParameters.Keys.ToList());
		Assert.AreEqual("required", damageParameters["damage"].DefaultValue);
		Assert.AreEqual("<expression>", damageParameters["damage"].Syntax);
		Assert.AreEqual("damage", damageParameters["pain"].DefaultValue);
		Assert.AreEqual("damage", damageParameters["stun"].DefaultValue);
		Assert.AreEqual("<damage type|none>", damageParameters["damagetype"].Syntax);
		StringAssert.Contains(damageParameters["damage"].Description, "quality");
		StringAssert.Contains(damageParameters["pain"].Description, "damage");
		StringAssert.Contains(damageParameters["damagetype"].Description, "damage type");
		Assert.IsTrue(TrapPayloadDefinition.IsSupportedParameter(TrapPayloadType.DirectDamage, "damage"));
		Assert.IsFalse(TrapPayloadDefinition.IsSupportedParameter(TrapPayloadType.DirectDamage, "liquid"));

		var explosiveParameters = TrapPayloadDefinition.ParametersFor(TrapPayloadType.ExplosiveDamage)
			.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		CollectionAssert.AreEquivalent(
			new[] { "echo", "damage", "pain", "stun", "damagetype", "explosionsize", "maximumproximity", "elevation" },
			explosiveParameters.Keys.ToList());
		Assert.AreEqual("Shockwave", explosiveParameters["damagetype"].DefaultValue);
		Assert.AreEqual("Normal", explosiveParameters["explosionsize"].DefaultValue);
		Assert.AreEqual("Proximate", explosiveParameters["maximumproximity"].DefaultValue);
		Assert.AreEqual("0", explosiveParameters["elevation"].DefaultValue);
		StringAssert.Contains(explosiveParameters["elevation"].Description, "height");

		var gasParameters = TrapPayloadDefinition.ParametersFor(TrapPayloadType.GasCloud)
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(
			new[] { "echo", "gas", "dose", "duration", "cloudecho" },
			gasParameters.ToList());
	}

	[TestMethod]
	public void TrapParameterValidation_RejectsInvalidValuesForEveryTypedParameter()
	{
		var invalidTriggerParameters = new[]
		{
			(Type: TrapTriggerType.Openable, Name: "chance", Value: "101"),
			(Type: TrapTriggerType.Openable, Name: "spotdifficulty", Value: "not-a-difficulty"),
			(Type: TrapTriggerType.Openable, Name: "avoiddifficulty", Value: "not-a-difficulty"),
			(Type: TrapTriggerType.Openable, Name: "filterprog", Value: "0"),
			(Type: TrapTriggerType.Openable, Name: "triggerecho", Value: " "),
			(Type: TrapTriggerType.ExitTraversal, Name: "movementtypes", Value: "Teleporting"),
			(Type: TrapTriggerType.ExitTraversal, Name: "minimumsize", Value: "Colossal"),
			(Type: TrapTriggerType.ExitTraversal, Name: "maximumsize", Value: "Colossal"),
			(Type: TrapTriggerType.Proximity, Name: "maximumproximity", Value: "Adjacent"),
			(Type: TrapTriggerType.Signal, Name: "minimumvalue", Value: "NaN"),
			(Type: TrapTriggerType.Signal, Name: "maximumvalue", Value: "Infinity")
		};
		foreach (var parameter in invalidTriggerParameters)
		{
			Assert.IsFalse(TrapTriggerDefinition.TryValidateParameter(parameter.Type, parameter.Name, parameter.Value, out _),
				$"{parameter.Type} {parameter.Name} accepted {parameter.Value}.");
		}

		var invalidPayloadParameters = new[]
		{
			(Type: TrapPayloadType.DirectDamage, Name: "echo", Value: " "),
			(Type: TrapPayloadType.CastSpell, Name: "spell", Value: "0"),
			(Type: TrapPayloadType.CastSpell, Name: "power", Value: "Impossible"),
			(Type: TrapPayloadType.EmitSignal, Name: "targetitem", Value: "0"),
			(Type: TrapPayloadType.EmitSignal, Name: "value", Value: "NaN"),
			(Type: TrapPayloadType.ExecuteProg, Name: "prog", Value: "0"),
			(Type: TrapPayloadType.DirectDamage, Name: "damage", Value: "traitbonus + 1"),
			(Type: TrapPayloadType.DirectDamage, Name: "damage", Value: "-1"),
			(Type: TrapPayloadType.DirectDamage, Name: "pain", Value: "damage - 1"),
			(Type: TrapPayloadType.DirectDamage, Name: "damagetype", Value: "Disintegration"),
			(Type: TrapPayloadType.ExplosiveDamage, Name: "explosionsize", Value: "Unreal"),
			(Type: TrapPayloadType.ExplosiveDamage, Name: "maximumproximity", Value: "Unapproximable"),
			(Type: TrapPayloadType.ExplosiveDamage, Name: "elevation", Value: "NaN"),
			(Type: TrapPayloadType.LiquidDischarge, Name: "liquid", Value: "0"),
			(Type: TrapPayloadType.LiquidDischarge, Name: "amount", Value: "-0.1"),
			(Type: TrapPayloadType.GasCloud, Name: "gas", Value: "0"),
			(Type: TrapPayloadType.GasCloud, Name: "dose", Value: "0"),
			(Type: TrapPayloadType.GasCloud, Name: "duration", Value: "-00:00:01"),
			(Type: TrapPayloadType.GasCloud, Name: "cloudecho", Value: " "),
			(Type: TrapPayloadType.Restraint, Name: "duration", Value: "00:00:00"),
			(Type: TrapPayloadType.Restraint, Name: "description", Value: " ")
		};
		foreach (var parameter in invalidPayloadParameters)
		{
			Assert.IsFalse(TrapPayloadDefinition.TryValidateParameter(parameter.Type, parameter.Name, parameter.Value, out _),
				$"{parameter.Type} {parameter.Name} accepted {parameter.Value}.");
		}
	}

	[TestMethod]
	public void TrapParameterValidation_RejectsInvalidRangesAndRestoresOptionalDefaults()
	{
		Assert.IsFalse(TrapTriggerDefinition.TryValidateParameters(TrapTriggerType.Signal,
			new Dictionary<string, string>
			{
				["minimumvalue"] = "2",
				["maximumvalue"] = "1"
			}, out var signalError));
		StringAssert.Contains(signalError, "minimumvalue");

		Assert.IsFalse(TrapTriggerDefinition.TryValidateParameters(TrapTriggerType.ExitTraversal,
			new Dictionary<string, string>
			{
				["minimumsize"] = "Large",
				["maximumsize"] = "Small"
			}, out var sizeError));
		StringAssert.Contains(sizeError, "minimumsize");

		Assert.IsFalse(TrapPayloadDefinition.TryValidateParameters(TrapPayloadType.DirectDamage,
			new Dictionary<string, string> { ["damagetype"] = "999" }, out var damageError));
		StringAssert.Contains(damageError, "damagetype");

		var payload = new TrapPayloadDefinition(TrapPayloadType.DirectDamage);
		payload.SetParameter("echo", "A sharp crack sounds.");
		payload.SetParameter("echo", "none");
		Assert.IsFalse(payload.Parameters.ContainsKey("echo"));
		Assert.IsTrue(payload.SetParameter("damage", "1d40 + quality"));
		Assert.AreEqual("1d40 + quality", payload.Parameters["damage"]);
		Assert.IsTrue(payload.SetParameter("pain", "damage + power"));
		Assert.AreEqual("damage + power", payload.Parameters["pain"]);
		Assert.IsFalse(payload.SetParameter("stun", "damage / 0"));
		Assert.IsFalse(payload.Parameters.ContainsKey("stun"));

		Assert.IsTrue(TrapParameterValidation.TryEvaluateDamageExpression("damage + quality + power", 7.0,
			SpellPower.Strong, 3.0, out var evaluated, out _));
		Assert.AreEqual(16.0, evaluated);
	}

	[TestMethod]
	public void DirectDamagePayload_EvaluatesDamagePainAndStunFormulasAtResolution()
	{
		var gameworld = new Mock<IFuturemud>();
		var owner = new Mock<ICell>();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		var templates = new RevisableAll<ITrapTemplate>();
		templates.Add(template.Object);
		gameworld.SetupGet(x => x.TrapTemplates).Returns(templates);
		var bodypart = new Mock<IBodypart>();
		var body = new Mock<IBody>();
		body.SetupGet(x => x.RandomBodypart).Returns(bodypart.Object);
		var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Body).Returns(body.Object);
		target.Setup(x => x.SufferDamage(It.IsAny<IDamage>()))
			.Returns(Enumerable.Empty<IWound>());
		var payload = new TrapPayloadDefinition(TrapPayloadType.DirectDamage);
		Assert.IsTrue(payload.SetParameter("damage", "quality + power"));
		Assert.IsTrue(payload.SetParameter("pain", "damage + 2"));
		Assert.IsTrue(payload.SetParameter("stun", "power"));

		var trap = new TrapEffect(owner.Object, template.Object, power: SpellPower.Strong);
		typeof(TrapEffect).GetMethod("ExecuteDamagePayload", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(trap, [payload, target.Object]);

		target.Verify(x => x.SufferDamage(It.Is<IDamage>(damage =>
			damage.DamageAmount == 11.0 && damage.PainAmount == 13.0 && damage.StunAmount == 6.0)), Times.Once);
	}

	[TestMethod]
	public void InvalidPayloadTargetSelector_FailsClosedAtRuntime()
	{
		var gameworld = new Mock<IFuturemud>();
		var owner = new Mock<ICell>();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		var templates = new RevisableAll<ITrapTemplate>();
		templates.Add(template.Object);
		gameworld.SetupGet(x => x.TrapTemplates).Returns(templates);
		var target = new Mock<ICharacter>();
		target.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns(Enumerable.Empty<IWound>());
		var payload = new TrapPayloadDefinition(TrapPayloadType.DirectDamage);
		Assert.IsTrue(payload.SetParameter("damage", "1"));
		payload.SetTargetSelector((TrapTargetSelector)(-1));

		var trap = new TrapEffect(owner.Object, template.Object);
		typeof(TrapEffect).GetMethod("ExecutePayload", BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(ITrapPayload), typeof(ICharacter)], null)!
			.Invoke(trap, [payload, target.Object]);

		target.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void InvalidManualTrigger_FailsClosedBeforeConsumingACharge()
	{
		var gameworld = new Mock<IFuturemud>();
		var owner = new Mock<ICell>();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var invalidManualTrigger = TrapTriggerDefinition.LoadFromXml(
			XElement.Parse("<Trigger type=\"Manual\"><Parameter name=\"chance\">101</Parameter></Trigger>"));
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		template.SetupGet(x => x.Triggers).Returns(new ITrapTrigger[] { invalidManualTrigger });
		template.SetupGet(x => x.Payloads).Returns(Array.Empty<ITrapPayload>());
		var templates = new RevisableAll<ITrapTemplate>();
		templates.Add(template.Object);
		gameworld.SetupGet(x => x.TrapTemplates).Returns(templates);

		var trap = new TrapEffect(owner.Object, template.Object);

		Assert.IsFalse(trap.TriggerManually());
		Assert.AreEqual(TrapState.Armed, trap.State);
		Assert.AreEqual(1, trap.ChargesRemaining);
	}

	[TestMethod]
	public void ExplosiveDamagePayload_UsesStandardExplosionHandlingWithConfiguredPacket()
	{
		var gameworld = new Mock<IFuturemud>();
		var owner = new Mock<ICell>();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		IExplosiveDamage? resolvedExplosion = null;
		owner.Setup(x => x.ExplosionEmantingFromPerceivable(It.IsAny<IExplosiveDamage>()))
			.Callback<IExplosiveDamage>(explosion => resolvedExplosion = explosion)
			.Returns(Enumerable.Empty<IWound>());
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		var templates = new RevisableAll<ITrapTemplate>();
		templates.Add(template.Object);
		gameworld.SetupGet(x => x.TrapTemplates).Returns(templates);
		var payload = new TrapPayloadDefinition(TrapPayloadType.ExplosiveDamage);
		Assert.IsTrue(payload.SetParameter("damage", "quality + power"));
		Assert.IsTrue(payload.SetParameter("pain", "damage + 2"));
		Assert.IsTrue(payload.SetParameter("stun", "power"));
		Assert.IsTrue(payload.SetParameter("damagetype", "Electrical"));
		Assert.IsTrue(payload.SetParameter("explosionsize", "Large"));
		Assert.IsTrue(payload.SetParameter("maximumproximity", "Distant"));
		Assert.IsTrue(payload.SetParameter("elevation", "1.5"));

		var trap = new TrapEffect(owner.Object, template.Object, power: SpellPower.Strong);
		typeof(TrapEffect).GetMethod("ExecuteExplosiveDamagePayload", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(trap, [payload]);

		owner.Verify(x => x.ExplosionEmantingFromPerceivable(It.IsAny<IExplosiveDamage>()), Times.Once);
		Assert.IsNotNull(resolvedExplosion);
		Assert.AreEqual(SizeCategory.Large, resolvedExplosion.ExplosionSize);
		Assert.AreEqual(Proximity.Distant, resolvedExplosion.MaximumProximity);
		Assert.AreEqual(1.5, resolvedExplosion.Elevation);
		var damage = resolvedExplosion.ReferenceDamages.Single();
		Assert.AreEqual(DamageType.Electrical, damage.DamageType);
		Assert.AreEqual(11.0, damage.DamageAmount);
		Assert.AreEqual(13.0, damage.PainAmount);
		Assert.AreEqual(6.0, damage.StunAmount);
	}

	[TestMethod]
	public void CreateTrapSpellEffect_AcceptsExitTriggers()
	{
		var spell = new Mock<IMagicSpell>();
		var effect = (CreateTrapSpellEffect)Activator.CreateInstance(
			typeof(CreateTrapSpellEffect),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[new XElement("Effect"), spell.Object],
			null)!;
		var exitTrigger = new Mock<IMagicTrigger>();
		exitTrigger.SetupGet(x => x.TargetTypes).Returns("exit");

		Assert.IsTrue(effect.IsCompatibleWithTrigger(exitTrigger.Object));
	}

	[TestMethod]
	public void TrapCommandSurface_SeparatesPlayerListingAndAdministratorMaintenance()
	{
		var trapModuleSource = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "TrapModule.cs"));
		StringAssert.Contains(trapModuleSource, "[PlayerCommand(\"Traps\", \"traps\")]");
		StringAssert.Contains(trapModuleSource, "case \"list\" when actor.IsAdministrator():");
		StringAssert.Contains(trapModuleSource, "internal static int DeleteAllTraps");
		StringAssert.Contains(trapModuleSource, "TrapPayloadScheduleEffect");

		var builderSource = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "ActivityBuilderModule.cs"));
		StringAssert.Contains(builderSource, "[PlayerCommand(\"TrapTemplate\", \"traptemplate\", \"trapt\", \"tt\")]");

		var implementorSource = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "ImplementorModule.cs"));
		StringAssert.Contains(implementorSource, "case \"cleartraps\":");
		StringAssert.Contains(implementorSource, "TrapModule.DeleteAllTraps(actor.Gameworld)");
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
	public void ComponentBinding_PersistsInstalledLayerAndRoutePosition()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = new Mock<ICell>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		item.SetupGet(x => x.Id).Returns(123L);
		item.SetupGet(x => x.LocationLevelPerceivable).Returns(item.Object);
		item.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(cell.Object, RoomLayer.InAir, 4_250.5));
		gameworld.Setup(x => x.TryGetItem(123L, true)).Returns(item.Object);
		var binding = new TrapComponentBinding(gameworld.Object, item.Object,
			TrapComponentRole.Payload, 25.0, 1.0);

		var loaded = TrapComponentBinding.LoadFromXml(binding.SaveToXml(), gameworld.Object);

		Assert.AreEqual(RoomLayer.InAir, loaded.InstalledLayer);
		Assert.AreEqual(4_250.5, loaded.InstalledRoutePositionMetres);
	}

	[TestMethod]
	public void CellTrapInitialisation_DefersItemResolutionUntilWorldItemsAreLoaded()
	{
		var templates = new RevisableAll<ITrapTemplate>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.TrapTemplates).Returns(templates);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(123L);
		var template = new Mock<ITrapTemplate>();
		template.SetupGet(x => x.Id).Returns(300L);
		template.SetupGet(x => x.RevisionNumber).Returns(4);
		template.SetupGet(x => x.Charges).Returns(1);
		template.SetupGet(x => x.Triggers).Returns([]);
		var binding = new TrapComponentBinding(gameworld.Object, item.Object,
			TrapComponentRole.TriggerAndPayload, 80.0, 2.0);
		var trap = new TrapEffect(cell.Object, template.Object, components: [binding]);

		trap.InitialEffect();
		trap.Login();

		gameworld.Verify(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);

		templates.Add(template.Object);
		trap.InitialiseAfterWorldItems();

		gameworld.Verify(x => x.TryGetItem(123L, true), Times.Once);
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

	private static string GetSourcePath(params string[] segments)
	{
		for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
		{
			if (!File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
			{
				continue;
			}

			return Path.Combine([directory.FullName, .. segments]);
		}

		throw new DirectoryNotFoundException("Could not locate the FutureMUD repository root.");
	}
}

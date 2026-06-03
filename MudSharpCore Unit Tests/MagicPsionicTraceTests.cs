#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicPsionicTraceTests
{
	[TestInitialize]
	public void TestInitialize()
	{
		PsionicTraceEffect.InitialiseEffectType();
	}

	[TestMethod]
	public void PsionicTraceEffect_SaveLoad_RoundTripsTraceFactsAndSchedule()
	{
		var gameworld = CreateGameworld();
		var power = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var source = CreateCharacter(11, "source", gameworld.Object, cell.Object);
		var target = CreateCharacter(12, "target", gameworld.Object, cell.Object);
		ConfigureCollections(gameworld, [source.Object, target.Object], [cell.Object], [power]);

		var traceId = Guid.NewGuid();
		var created = new DateTime(2026, 6, 1, 3, 0, 0, DateTimeKind.Utc);
		var effect = new PsionicTraceEffect(source.Object, source.Object, target.Object, cell.Object, power,
			PsionicActivityKind.Psychic, "a suggested thought", "a veiled mind", Difficulty.Hard, 2,
			traceId, created, TimeSpan.FromMinutes(45));

		var xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan> { [effect] = TimeSpan.FromMinutes(12) });
		var loaded = (IPsionicTraceEffect)Effect.LoadEffect(xml, source.Object);

		Assert.AreEqual("PsionicTrace", xml.Element("Type")!.Value);
		Assert.AreEqual((long)TimeSpan.FromMinutes(12).TotalMilliseconds, long.Parse(xml.Element("Remaining")!.Value));
		Assert.AreEqual(traceId, loaded.TraceId);
		Assert.AreEqual(source.Object.Id, loaded.SourceCharacterId);
		Assert.AreEqual(target.Object.Id, loaded.TargetCharacterId);
		Assert.AreEqual(cell.Object.Id, loaded.SourceCellId);
		Assert.AreEqual(PsionicActivityKind.Psychic, loaded.ActivityKind);
		Assert.AreEqual("a suggested thought", loaded.ActivityDescription);
		Assert.AreEqual("a veiled mind", loaded.UnknownIdentityDescription);
		Assert.AreEqual(Difficulty.Hard, loaded.ReadDifficulty);
		Assert.AreEqual(2, loaded.ConcealmentDifficultyStages);
		Assert.AreEqual(created, loaded.CreatedUtc);
		Assert.AreEqual(TimeSpan.FromMinutes(45), loaded.TraceDuration);
		Assert.AreSame(source.Object, loaded.SourceCharacter);
		Assert.AreSame(target.Object, loaded.TargetCharacter);
		Assert.AreSame(cell.Object, loaded.SourceCell);
		Assert.IsTrue(loaded.Involves(source.Object));
		Assert.IsTrue(loaded.Involves(target.Object));
	}

	[TestMethod]
	public void MagicPowerTraceXml_LoadsOldDefinitionsDisabledAndRoundTripsExplicitTraceConfig()
	{
		var gameworld = CreateGameworld();
		var oldPower = (MagicPowerBase)MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: false), gameworld.Object);
		var oldSaved = InvokeSaveDefinition(oldPower);

		Assert.IsFalse(oldPower.CreatesPsionicTrace);
		Assert.IsNotNull(oldSaved.Element("PsionicTrace"));
		Assert.AreEqual("false", oldSaved.Element("PsionicTrace")!.Element("Enabled")!.Value);

		var tracedPower = (MagicPowerBase)MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true,
			durationSeconds: 900, readDifficulty: Difficulty.VeryHard, traceDescription: "a sharp residual cut"),
			gameworld.Object);
		var tracedSaved = InvokeSaveDefinition(tracedPower);

		Assert.IsTrue(tracedPower.CreatesPsionicTrace);
		Assert.AreEqual(TimeSpan.FromMinutes(15), tracedPower.PsionicTraceDuration);
		Assert.AreEqual(Difficulty.VeryHard, tracedPower.PsionicTraceReadDifficulty);
		Assert.AreEqual("a sharp residual cut", tracedPower.PsionicTraceDescription);
		Assert.AreEqual("true", tracedSaved.Element("PsionicTrace")!.Element("Enabled")!.Value);
		Assert.AreEqual("900", tracedSaved.Element("PsionicTrace")!.Element("DurationSeconds")!.Value);
		Assert.AreEqual(((int)Difficulty.VeryHard).ToString(), tracedSaved.Element("PsionicTrace")!.Element("ReadDifficulty")!.Value);
		Assert.AreEqual("a sharp residual cut", tracedSaved.Element("PsionicTrace")!.Element("Description")!.Value);
	}

	[TestMethod]
	public void TraceDurationBuilderCommand_RejectedDurationDoesNotMutateExistingValue()
	{
		var gameworld = CreateGameworld();
		var power = (MagicPowerBase)MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true,
			durationSeconds: 900), gameworld.Object);
		var actor = CreateCharacter(11, "builder", gameworld.Object, null);
		var originalDuration = power.PsionicTraceDuration;

		var result = power.BuildingCommand(actor.Object, new StringStack("traceduration 0"));

		Assert.IsFalse(result);
		Assert.AreEqual(originalDuration, power.PsionicTraceDuration);
	}

	[TestMethod]
	public void PsionicActivityNotifier_CreatesSourceTargetAndCellTracesWithOneTraceId()
	{
		var gameworld = CreateGameworld();
		var power = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true,
			durationSeconds: 600, traceDescription: "a residual suggestion"), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var sourceTraces = new List<IPsionicTraceEffect>();
		var targetTraces = new List<IPsionicTraceEffect>();
		var cellTraces = new List<IPsionicTraceEffect>();
		var source = CreateCharacter(11, "source", gameworld.Object, cell.Object, sourceTraces);
		var target = CreateCharacter(12, "target", gameworld.Object, cell.Object, targetTraces);
		ConfigureTraceOwner(cell, cellTraces);
		ConfigureCollections(gameworld, [source.Object, target.Object], [cell.Object], [power]);

		PsionicActivityNotifier.Notify(source.Object, power, "a suggestion", target.Object);

		Assert.AreEqual(1, sourceTraces.Count);
		Assert.AreEqual(1, targetTraces.Count);
		Assert.AreEqual(1, cellTraces.Count);
		Assert.AreEqual(sourceTraces[0].TraceId, targetTraces[0].TraceId);
		Assert.AreEqual(sourceTraces[0].TraceId, cellTraces[0].TraceId);
		Assert.IsTrue(sourceTraces.All(x => x.TraceDuration == TimeSpan.FromMinutes(10)));
		Assert.AreEqual("a residual suggestion", targetTraces[0].ActivityDescription);
		Assert.AreEqual(target.Object.Id, sourceTraces[0].TargetCharacterId);
		Assert.AreEqual(target.Object.Id, targetTraces[0].TargetCharacterId);
		Assert.AreEqual(cell.Object.Id, cellTraces[0].SourceCellId);
		Assert.IsTrue(cellTraces[0].Involves(source.Object));
		Assert.IsTrue(cellTraces[0].Involves(target.Object));
	}

	[TestMethod]
	public void PsionicActivityNotifier_DoesNotDuplicateTraceWhenSourceIsAlsoTarget()
	{
		var gameworld = CreateGameworld();
		var power = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var sourceTraces = new List<IPsionicTraceEffect>();
		var cellTraces = new List<IPsionicTraceEffect>();
		var source = CreateCharacter(11, "source", gameworld.Object, cell.Object, sourceTraces);
		ConfigureTraceOwner(cell, cellTraces);
		ConfigureCollections(gameworld, [source.Object], [cell.Object], [power]);

		PsionicActivityNotifier.Notify(source.Object, power, "self scan", source.Object);

		Assert.AreEqual(1, sourceTraces.Count);
		Assert.AreEqual(1, cellTraces.Count);
		Assert.AreEqual(sourceTraces[0].TraceId, cellTraces[0].TraceId);
	}

	[TestMethod]
	public void PsionicActivityNotifier_PreservesSensitivityNotificationWhileRecordingTrace()
	{
		var listenerOutput = string.Empty;
		var gameworld = CreateGameworld();
		var tracePower = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true), gameworld.Object);
		var sensitivityPower = (SensitivityPower)MagicPowerFactory.LoadPower(CreateSensitivityPowerModel(), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var sourceTraces = new List<IPsionicTraceEffect>();
		var source = CreateCharacter(11, "source", gameworld.Object, cell.Object, sourceTraces);
		var listener = CreateCharacter(12, "listener", gameworld.Object, cell.Object);
		var sensitivity = new PsionicSensitivityEffect(listener.Object, sensitivityPower);
		listener.Setup(x => x.EffectsOfType<PsionicSensitivityEffect>(It.IsAny<Predicate<PsionicSensitivityEffect>?>()))
		        .Returns<Predicate<PsionicSensitivityEffect>?>(predicate => predicate is null || predicate(sensitivity) ? [sensitivity] : []);
		listener.SetupGet(x => x.OutputHandler).Returns(CreateOutputHandler(text => listenerOutput += text).Object);
		ConfigureCollections(gameworld, [source.Object, listener.Object], [cell.Object], [tracePower, sensitivityPower]);

		PsionicActivityNotifier.Notify(source.Object, tracePower, "a psychic disturbance");

		Assert.AreEqual(1, sourceTraces.Count);
		StringAssert.Contains(listenerOutput, "Activity");
	}

	[TestMethod]
	public void PsionicActivityNotifier_RecordsConcealmentFallbackAndRaisedDifficulty()
	{
		var gameworld = CreateGameworld();
		var power = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var sourceTraces = new List<IPsionicTraceEffect>();
		var targetTraces = new List<IPsionicTraceEffect>();
		var source = CreateCharacter(11, "source", gameworld.Object, cell.Object, sourceTraces);
		var target = CreateCharacter(12, "target", gameworld.Object, cell.Object, targetTraces);
		var concealment = new Mock<IMindContactConcealmentEffect>();
		concealment.SetupGet(x => x.UnknownIdentityDescription).Returns("a masked mind");
		concealment.SetupGet(x => x.AuditDifficultyStages).Returns(2);
		concealment.Setup(x => x.ConcealsIdentityFrom(source.Object, target.Object, power.School)).Returns(true);
		source.Setup(x => x.EffectsOfType<IMindContactConcealmentEffect>(It.IsAny<Predicate<IMindContactConcealmentEffect>?>()))
		      .Returns<Predicate<IMindContactConcealmentEffect>?>(predicate =>
		      {
			      var effects = new[] { concealment.Object };
			      return predicate is null ? effects : effects.Where(x => predicate(x));
		      });
		ConfigureCollections(gameworld, [source.Object, target.Object], [cell.Object], [power]);

		PsionicActivityNotifier.Notify(source.Object, power, "a concealed suggestion", target.Object);

		Assert.AreEqual("a masked mind", targetTraces[0].UnknownIdentityDescription);
		Assert.AreEqual(2, targetTraces[0].ConcealmentDifficultyStages);
	}

	[TestMethod]
	public void TracePower_ReportsResidualTraceWhenNoActiveLinkExists()
	{
		var output = string.Empty;
		var gameworld = CreateGameworld();
		var power = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true,
			traceDescription: "a residual touch"), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var targetTraces = new List<IPsionicTraceEffect>();
		var actor = CreateCharacter(11, "actor", gameworld.Object, cell.Object);
		var source = CreateCharacter(12, "source", gameworld.Object, cell.Object);
		var target = CreateCharacter(13, "target", gameworld.Object, cell.Object, targetTraces);
		actor.SetupGet(x => x.OutputHandler).Returns(CreateOutputHandler(text => output += text).Object);
		cell.SetupGet(x => x.Characters).Returns([actor.Object, target.Object]);
		ConfigureCollections(gameworld, [actor.Object, source.Object, target.Object], [cell.Object], [power]);
		targetTraces.Add(new PsionicTraceEffect(target.Object, source.Object, target.Object, cell.Object, power,
			PsionicActivityKind.Psychic, "a residual touch", "an unknown mind", Difficulty.Normal, 0,
			Guid.NewGuid(), DateTime.UtcNow - TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30)));

		power.UseCommand(actor.Object, "trace", new StringStack("target"));

		StringAssert.Contains(output, "Residual psionic traces around");
		StringAssert.Contains(output, "A residual touch");
		StringAssert.Contains(output, "source");
	}

	[TestMethod]
	public void TracePower_ReevaluatesResidualTraceConcealmentForCurrentReader()
	{
		var output = string.Empty;
		var gameworld = CreateGameworld(passDifficulties: difficulty => difficulty <= Difficulty.Normal);
		var power = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true,
			traceDescription: "a hidden touch"), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var targetTraces = new List<IPsionicTraceEffect>();
		var actor = CreateCharacter(11, "actor", gameworld.Object, cell.Object);
		var source = CreateCharacter(12, "source", gameworld.Object, cell.Object);
		var target = CreateCharacter(13, "target", gameworld.Object, cell.Object, targetTraces);
		actor.SetupGet(x => x.OutputHandler).Returns(CreateOutputHandler(text => output += text).Object);
		cell.SetupGet(x => x.Characters).Returns([actor.Object, target.Object]);
		var concealment = new Mock<IMindContactConcealmentEffect>();
		concealment.SetupGet(x => x.UnknownIdentityDescription).Returns("a veiled mind");
		concealment.SetupGet(x => x.AuditDifficultyStages).Returns(2);
		concealment.Setup(x => x.ConcealsIdentityFrom(source.Object, actor.Object, power.School)).Returns(true);
		source.Setup(x => x.EffectsOfType<IMindContactConcealmentEffect>(It.IsAny<Predicate<IMindContactConcealmentEffect>?>()))
		      .Returns<Predicate<IMindContactConcealmentEffect>?>(predicate =>
		      {
			      var effects = new[] { concealment.Object };
			      return predicate is null ? effects : effects.Where(x => predicate(x));
		      });
		ConfigureCollections(gameworld, [actor.Object, source.Object, target.Object], [cell.Object], [power]);
		targetTraces.Add(new PsionicTraceEffect(target.Object, source.Object, target.Object, cell.Object, power,
			PsionicActivityKind.Psychic, "a hidden touch", "an unknown mind", Difficulty.Normal, 0,
			Guid.NewGuid(), DateTime.UtcNow - TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30)));

		power.UseCommand(actor.Object, "trace", new StringStack("target"));

		StringAssert.Contains(output, "a veiled mind");
		Assert.IsFalse(output.Contains("source", StringComparison.InvariantCultureIgnoreCase));
	}

	[TestMethod]
	public void TracePower_IgnoresTargetSpecificStoredConcealmentForUnconcealedReader()
	{
		var output = string.Empty;
		var gameworld = CreateGameworld(passDifficulties: difficulty => difficulty <= Difficulty.Normal);
		var power = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true,
			traceDescription: "a hidden touch"), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var targetTraces = new List<IPsionicTraceEffect>();
		var actor = CreateCharacter(11, "actor", gameworld.Object, cell.Object);
		var source = CreateCharacter(12, "source", gameworld.Object, cell.Object);
		var target = CreateCharacter(13, "target", gameworld.Object, cell.Object, targetTraces);
		actor.SetupGet(x => x.OutputHandler).Returns(CreateOutputHandler(text => output += text).Object);
		cell.SetupGet(x => x.Characters).Returns([actor.Object, target.Object]);
		ConfigureCollections(gameworld, [actor.Object, source.Object, target.Object], [cell.Object], [power]);
		targetTraces.Add(new PsionicTraceEffect(target.Object, source.Object, target.Object, cell.Object, power,
			PsionicActivityKind.Psychic, "a hidden touch", "a masked mind", Difficulty.Normal, 2,
			Guid.NewGuid(), DateTime.UtcNow - TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30)));

		power.UseCommand(actor.Object, "trace", new StringStack("target"));

		StringAssert.Contains(output, "source");
		Assert.IsFalse(output.Contains("a masked mind", StringComparison.InvariantCultureIgnoreCase));
	}

	[TestMethod]
	public void TracePower_ActiveLinkOutputStillIncludesActiveMindLinks()
	{
		var output = string.Empty;
		var gameworld = CreateGameworld();
		var connectPower = (ConnectMindPower)MagicPowerFactory.LoadPower(CreateConnectMindPowerModel(), gameworld.Object);
		var tracePower = MagicPowerFactory.LoadPower(CreateTracePowerModel(includeTrace: true), gameworld.Object);
		var cell = CreateCell(31, gameworld.Object);
		var actor = CreateCharacter(11, "actor", gameworld.Object, cell.Object);
		var target = CreateCharacter(12, "target", gameworld.Object, cell.Object);
		var linked = CreateCharacter(13, "linked", gameworld.Object, cell.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(CreateOutputHandler(text => output += text).Object);
		cell.SetupGet(x => x.Characters).Returns([actor.Object, target.Object]);
		ConfigureCollections(gameworld, [actor.Object, target.Object, linked.Object], [cell.Object], [connectPower, tracePower]);
		var connectEffect = new ConnectMindEffect(target.Object, linked.Object, connectPower);
		target.Setup(x => x.EffectsOfType<ConnectMindEffect>(It.IsAny<Predicate<ConnectMindEffect>?>()))
		      .Returns<Predicate<ConnectMindEffect>?>(predicate => predicate is null || predicate(connectEffect) ? [connectEffect] : []);

		tracePower.UseCommand(actor.Object, "trace", new StringStack("target"));

		StringAssert.Contains(output, "You trace the active mind links around target");
		StringAssert.Contains(output, "outbound");
		StringAssert.Contains(output, "linked");
	}

	private static XElement InvokeSaveDefinition(IMagicPower power)
	{
		return (XElement)power.GetType()
		                      .GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
		                      .Invoke(power, [])!;
	}

	private static MagicPower CreateTracePowerModel(bool includeTrace, double durationSeconds = 1800.0,
		Difficulty readDifficulty = Difficulty.Normal, string traceDescription = "a residual trace")
	{
		return new MagicPower
		{
			Id = 100,
			Name = "Trace",
			Blurb = "Trace",
			ShowHelp = "Trace.",
			MagicSchoolId = 1,
			PowerModel = "trace",
			Definition = TargetedDefinition("trace", MagicPowerDistance.SameLocationOnly, includeTrace, durationSeconds,
				readDifficulty, traceDescription).ToString()
		};
	}

	private static MagicPower CreateSensitivityPowerModel()
	{
		return new MagicPower
		{
			Id = 101,
			Name = "Sensitivity",
			Blurb = "Sensitivity",
			ShowHelp = "Sensitivity.",
			MagicSchoolId = 1,
			PowerModel = "sensitivity",
			Definition = SustainedDefinition("sensitivity", "endsensitivity", includeTrace: false,
				new XElement("ScanVerb", "senscan"),
				new XElement("ScanDistance", MagicPowerDistance.SameLocationOnly),
				new XElement("GrantedPerceptions", PerceptionTypes.SenseMagical | PerceptionTypes.SensePsychic),
				new XElement("ActivityKinds", "Magical,Psychic"),
				new XElement("ActivityRange", 2U),
				new XElement("ActivityDifficulty", (int)Difficulty.Normal),
				new XElement("CapabilityDifficulty", (int)Difficulty.ExtremelyHard),
				new XElement("PermitCapabilityRead", true),
				new XElement("NotifySelf", false),
				new XElement("ActivityEcho", new XCData("Activity."))).ToString()
		};
	}

	private static MagicPower CreateConnectMindPowerModel()
	{
		return new MagicPower
		{
			Id = 102,
			Name = "Connect Mind",
			Blurb = "Connect mind",
			ShowHelp = "Connect mind.",
			MagicSchoolId = 1,
			PowerModel = "connectmind",
			Definition = new XElement("Definition",
				BaseElements(includeTrace: false),
				new XElement("ConnectVerb", "connect"),
				new XElement("DisconnectVerb", "disconnect"),
				new XElement("PowerDistance", MagicPowerDistance.SameLocationOnly),
				new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
				new XElement("SkillCheckTrait", 1L),
				new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass),
				new XElement("TargetCanSeeIdentityProg", 0L),
				new XElement("TargetEligibilityProg", 0L),
				new XElement("ExclusiveConnection", true),
				new XElement("EmoteForConnect", new XCData("Connect.")),
				new XElement("SelfEmoteForConnect", new XCData("Connect self.")),
				new XElement("EmoteForDisconnect", new XCData("Disconnect.")),
				new XElement("SelfEmoteForDisconnect", new XCData("Disconnect self.")),
				new XElement("UnknownIdentityDescription", new XCData("unknown")),
				new XElement("EmoteForFailConnect", new XCData("Fail.")),
				new XElement("SelfEmoteForFailConnect", new XCData("Fail self.")),
				new XElement("OutcomeEchoes"),
				new XElement("ConcentrationPointsToSustain", 1.0),
				new XElement("SustainPenalty", 0.0),
				new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
				new XElement("SustainResourceCosts")
			).ToString()
		};
	}

	private static XElement TargetedDefinition(string verb, MagicPowerDistance distance, bool includeTrace,
		double durationSeconds, Difficulty readDifficulty, string traceDescription)
	{
		return new XElement("Definition",
			BaseElements(includeTrace, durationSeconds, readDifficulty, traceDescription),
			new XElement("Verb", verb),
			new XElement("PowerDistance", distance),
			new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
			new XElement("SkillCheckTrait", 1L),
			new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass),
			new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
			new XElement("FailEcho", new XCData("Fail.")),
			new XElement("SuccessEcho", new XCData(string.Empty))
		);
	}

	private static XElement SustainedDefinition(string begin, string end, bool includeTrace, params object[] additional)
	{
		var definition = new XElement("Definition",
			BaseElements(includeTrace),
			new XElement("BeginVerb", begin),
			new XElement("EndVerb", end),
			new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
			new XElement("SkillCheckTrait", 1L),
			new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass),
			new XElement("BeginEmote", new XCData("Begin.")),
			new XElement("EndEmote", new XCData("End.")),
			new XElement("FailEmote", new XCData("Fail.")),
			new XElement("ConcentrationPointsToSustain", 1.0),
			new XElement("SustainPenalty", 0.0),
			new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
			new XElement("SustainResourceCosts")
		);
		foreach (var item in additional)
		{
			definition.Add(item);
		}

		return definition;
	}

	private static object[] BaseElements(bool includeTrace, double durationSeconds = 1800.0,
		Difficulty readDifficulty = Difficulty.Normal, string traceDescription = "a residual trace")
	{
		var elements = new List<object>
		{
			new XElement("CanInvokePowerProg", 0L),
			new XElement("WhyCantInvokePowerProg", 0L),
			new XElement("IsPsionic", true),
			new XElement("InvocationCosts")
		};
		if (includeTrace)
		{
			elements.Add(new XElement("PsionicTrace",
				new XElement("Enabled", true),
				new XElement("DurationSeconds", durationSeconds),
				new XElement("ReadDifficulty", (int)readDifficulty),
				new XElement("Description", new XCData(traceDescription))));
		}

		return elements.ToArray();
	}

	private static Mock<IFuturemud> CreateGameworld(Func<Difficulty, bool>? passDifficulties = null)
	{
		var trueProg = CreateProg(0, true).Object;
		var falseProg = CreateProg(1, false).Object;
		var heartbeat = new Mock<IHeartbeatManager>();
		var check = CreateCheck(CheckType.MagicTelepathyCheck, passDifficulties);
		var sensitivityCheck = CreateCheck(CheckType.SensitivityPower, _ => true);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(CreateCollectionMock(trueProg, falseProg).Object);
		gameworld.SetupGet(x => x.MagicSchools).Returns(CreateCollectionMock(CreateSchool().Object).Object);
		gameworld.SetupGet(x => x.Traits).Returns(CreateCollectionMock(CreateTrait().Object).Object);
		gameworld.SetupGet(x => x.AlwaysTrueProg).Returns(trueProg);
		gameworld.SetupGet(x => x.AlwaysFalseProg).Returns(falseProg);
		gameworld.SetupGet(x => x.UniversalErrorTextProg).Returns(trueProg);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
		gameworld.SetupGet(x => x.LegalAuthorities).Returns(CreateCollectionMock<ILegalAuthority>().Object);
		gameworld.Setup(x => x.GetCheck(CheckType.MagicTelepathyCheck)).Returns(check.Object);
		gameworld.Setup(x => x.GetCheck(CheckType.SensitivityPower)).Returns(sensitivityCheck.Object);
		ConfigureCollections(gameworld, [], [], []);
		return gameworld;
	}

	private static Mock<ICheck> CreateCheck(CheckType type, Func<Difficulty, bool>? passDifficulties)
	{
		passDifficulties ??= _ => true;
		var check = new Mock<ICheck>();
		check.SetupGet(x => x.Type).Returns(type);
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
				It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable?>(), It.IsAny<double>(),
				It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()))
		     .Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable?, double, TraitUseType,
			     (string Parameter, object value)[]>((_, difficulty, _, _, _, _, _) =>
			     CheckOutcome.SimpleOutcome(type, passDifficulties(difficulty) ? Outcome.Pass : Outcome.Fail));
		check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
				It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable?>(), It.IsAny<double>(),
				It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()))
		     .Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable?, double, TraitUseType,
			     (string Parameter, object value)[]>((_, _, _, _, _, _, _) =>
			     Enum.GetValues<Difficulty>()
			         .ToDictionary(x => x,
				         x => CheckOutcome.SimpleOutcome(type, passDifficulties(x) ? Outcome.Pass : Outcome.Fail)));
		return check;
	}

	private static Mock<IMagicSchool> CreateSchool()
	{
		var school = new Mock<IMagicSchool>();
		school.SetupGet(x => x.Id).Returns(1);
		school.SetupGet(x => x.Name).Returns("Psionics");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.BoldMagenta);
		school.SetupGet(x => x.SchoolVerb).Returns("psi");
		school.SetupGet(x => x.SchoolAdjective).Returns("psychic");
		return school;
	}

	private static Mock<ITraitDefinition> CreateTrait()
	{
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Id).Returns(1);
		trait.SetupGet(x => x.Name).Returns("Psionics");
		return trait;
	}

	private static Mock<IFutureProg> CreateProg(long id, bool truth)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(id);
		prog.SetupGet(x => x.Name).Returns($"Prog {id}");
		prog.SetupGet(x => x.FunctionName).Returns($"Prog{id}");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(truth);
		prog.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(truth);
		prog.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(truth);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.MXPClickableFunctionName()).Returns($"Prog{id}");
		return prog;
	}

	private static Mock<ICharacter> CreateCharacter(long id, string name, IFuturemud gameworld, ICell? location,
		List<IPsionicTraceEffect>? traceEffects = null)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.FrameworkItemType).Returns("Character");
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.Location).Returns(location!);
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns(name);
		character.SetupGet(x => x.PersonalName).Returns(personalName.Object);
		character.As<IHavePersonalName>().SetupGet(x => x.PersonalName).Returns(personalName.Object);
		character.SetupGet(x => x.CurrentName).Returns(personalName.Object);
		character.SetupGet(x => x.Keywords).Returns([name]);
		character.Setup(x => x.HasKeyword(It.IsAny<string>(), It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>()))
		         .Returns<string, IPerceiver, bool, bool>((keyword, _, abbreviated, contains) =>
			         contains
				         ? name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
				         : abbreviated
					         ? name.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase)
					         : name.EqualTo(keyword));
		character.Setup(x => x.HasDubFor(It.IsAny<IKeyworded>(), It.IsAny<string>())).Returns(false);
		character.Setup(x => x.HasDubFor(It.IsAny<IKeyworded>(), It.IsAny<IEnumerable<string>>())).Returns(false);
		character.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<MudSharp.Form.Shape.DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		         .Returns(name);
		character.Setup(x => x.EffectsOfType<IPsionicTraceEffect>(It.IsAny<Predicate<IPsionicTraceEffect>?>()))
		         .Returns<Predicate<IPsionicTraceEffect>?>(predicate => Filter(traceEffects ?? [], predicate));
		character.Setup(x => x.EffectsOfType<IMindContactConcealmentEffect>(It.IsAny<Predicate<IMindContactConcealmentEffect>?>()))
		         .Returns([]);
		character.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>?>()))
		         .Returns([]);
		character.Setup(x => x.EffectsOfType<ConnectMindEffect>(It.IsAny<Predicate<ConnectMindEffect>?>()))
		         .Returns([]);
		character.Setup(x => x.EffectsOfType<MindConnectedToEffect>(It.IsAny<Predicate<MindConnectedToEffect>?>()))
		         .Returns([]);
		character.Setup(x => x.CombinedEffectsOfType<ConnectMindEffect>())
		         .Returns([]);
		character.Setup(x => x.CombinedEffectsOfType<MindConnectedToEffect>())
		         .Returns([]);
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		         .Callback<IEffect, TimeSpan>((effect, _) =>
		         {
			         if (effect is IPsionicTraceEffect trace)
			         {
				         traceEffects?.Add(trace);
			         }
		         });
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>()));
		character.SetupGet(x => x.OutputHandler).Returns(CreateOutputHandler(_ => { }).Object);
		return character;
	}

	private static Mock<ICell> CreateCell(long id, IFuturemud gameworld)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns($"Cell {id}");
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld);
		cell.SetupGet(x => x.Location).Returns(cell.Object);
		cell.SetupGet(x => x.Characters).Returns([]);
		cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns([]);
		cell.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>?>()))
		    .Returns([]);
		ConfigureTraceOwner(cell, []);
		return cell;
	}

	private static void ConfigureTraceOwner<T>(Mock<T> owner, List<IPsionicTraceEffect> traces)
		where T : class, IPerceivable
	{
		owner.Setup(x => x.EffectsOfType<IPsionicTraceEffect>(It.IsAny<Predicate<IPsionicTraceEffect>?>()))
		     .Returns<Predicate<IPsionicTraceEffect>?>(predicate => Filter(traces, predicate));
		owner.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		     .Callback<IEffect, TimeSpan>((effect, _) =>
		     {
			     if (effect is IPsionicTraceEffect trace)
			     {
				     traces.Add(trace);
			     }
		     });
	}

	private static IEnumerable<T> Filter<T>(IEnumerable<T> items, Predicate<T>? predicate)
	{
		return predicate is null ? items : items.Where(x => predicate(x));
	}

	private static Mock<IOutputHandler> CreateOutputHandler(Action<string> onSend)
	{
		var handler = new Mock<IOutputHandler>();
		handler.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
		       .Callback<string, bool, bool>((text, _, _) => onSend(text))
		       .Returns(true);
		return handler;
	}

	private static void ConfigureCollections(Mock<IFuturemud> gameworld, IEnumerable<ICharacter> characters,
		IEnumerable<ICell> cells, IEnumerable<IMagicPower> powers)
	{
		var characterList = characters.ToArray();
		var cellList = cells.ToArray();
		var powerList = powers.ToArray();
		gameworld.SetupGet(x => x.Characters).Returns(CreateCollectionMock(characterList).Object);
		gameworld.SetupGet(x => x.Actors).Returns(CreateCollectionMock(characterList).Object);
		gameworld.SetupGet(x => x.Cells).Returns(CreateCollectionMock(cellList).Object);
		gameworld.SetupGet(x => x.MagicPowers).Returns(CreateCollectionMock(powerList).Object);
	}

	private static Mock<IUneditableAll<T>> CreateCollectionMock<T>(params T[] items) where T : class, IFrameworkItem
	{
		var byId = items.ToDictionary(x => x.Id, x => x);
		var collection = new Mock<IUneditableAll<T>>();
		collection.SetupGet(x => x.Count).Returns(items.Length);
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => byId.GetValueOrDefault(id));
		collection.Setup(x => x.GetByName(It.IsAny<string>())).Returns<string>(name =>
			items.FirstOrDefault(x => x.Name.EqualTo(name)));
		collection.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((text, _) =>
			long.TryParse(text, out var id) ? byId.GetValueOrDefault(id) : items.FirstOrDefault(x => x.Name.EqualTo(text)));
		collection.Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<T>)items).GetEnumerator());
		return collection;
	}
}

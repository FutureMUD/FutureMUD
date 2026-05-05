#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicEngineV4Tests
{
	private static readonly string[] V4PowerTypes =
	[
		"trace",
		"hear",
		"clairaudience",
		"allspeak",
		"babble",
		"magicksense",
		"projectemotion",
		"suggest",
		"coerce",
		"dangersense",
		"empathy",
		"hex",
		"clairvoyance",
		"prescience",
		"sensitivity",
		"psychicbolt"
	];

	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void MagicPowerFactory_RegistersAndLoadsV4PsionicPowerTypes()
	{
		var gameworld = CreateGameworld();

		foreach (var type in V4PowerTypes)
		{
			var power = MagicPowerFactory.LoadPower(CreatePowerModel(type), gameworld.Object);

			Assert.AreEqual(type, ((MagicPowerBase)power).DatabaseType, $"DatabaseType mismatch for {type}.");
			Assert.IsTrue(power.IsPsionic, $"{type} should load as psionic.");
			CollectionAssert.Contains(power.Verbs.ToArray(), PrimaryVerb(type));
		}
	}

	[TestMethod]
	public void V4PowerXml_RoundTripsPsionicAndBasePolicyFields()
	{
		var gameworld = CreateGameworld();
		var power = MagicPowerFactory.LoadPower(CreatePowerModel("suggest"), gameworld.Object);

		var saved = InvokeSaveDefinition(power);

		Assert.IsTrue(bool.Parse(saved.Element("IsPsionic")!.Value));
		Assert.AreEqual("0", saved.Element("CanInvokePowerProg")!.Value);
		Assert.AreEqual("0", saved.Element("WhyCantInvokePowerProg")!.Value);
		Assert.IsNotNull(saved.Element("InvocationCosts"));
		Assert.AreEqual("suggest", saved.Element("Verb")!.Value);
	}

	[TestMethod]
	public void OldSoiPsionicPowerXml_RoundTripsSubtypeFields()
	{
		var gameworld = CreateGameworld();

		foreach (var type in new[]
		         {
			         "dangersense", "empathy", "hex", "clairvoyance", "prescience", "sensitivity", "psychicbolt"
		         })
		{
			var power = MagicPowerFactory.LoadPower(CreatePowerModel(type), gameworld.Object);
			var saved = InvokeSaveDefinition(power);

			Assert.IsTrue(bool.Parse(saved.Element("IsPsionic")!.Value), $"{type} should round-trip psionic.");
			Assert.IsNotNull(saved.Element("InvocationCosts"), $"{type} should save invocation costs.");
			foreach (var elementName in OldSoiSubtypeElements(type))
			{
				Assert.IsNotNull(saved.Element(elementName), $"{type} should save {elementName}.");
			}
		}
	}

	[TestMethod]
	public void OldSoiPsionicCheckTypes_AreClassifiedForPowerEffects()
	{
		Assert.IsTrue(CheckType.DangerSenseDefense.IsDefensiveCombatAction());
		Assert.IsTrue(CheckType.DangerSenseDefense.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.EmpathyPower.IsTargettedFriendlyCheck());
		Assert.IsTrue(CheckType.HexPower.IsTargettedHostileCheck());
		Assert.IsTrue(CheckType.PsychicBoltPower.IsTargettedHostileCheck());
		Assert.IsTrue(CheckType.PsychicBoltPower.IsOffensiveCombatAction());
	}

	[TestMethod]
	public void MagicHexEffect_AppliesConfiguredPenaltyCategories()
	{
		var gameworld = CreateGameworld();
		var power = (HexPower)MagicPowerFactory.LoadPower(CreatePowerModel("hex"), gameworld.Object);
		var owner = CreateCharacter(50, gameworld.Object);
		var effect = new MagicHexEffect(owner.Object, power, 5.5,
			PsionicHexCheckCategory.TargetedHostile);

		Assert.AreEqual(-5.5, effect.CheckBonus);
		Assert.IsTrue(effect.AppliesToCheck(CheckType.HexPower));
		Assert.IsFalse(effect.AppliesToCheck(CheckType.GenericSkillCheck));
		Assert.IsFalse(effect.AppliesToCheck(CheckType.EmpathyPower));

		var generalEffect = new MagicHexEffect(owner.Object, power, 2.0, PsionicHexCheckCategory.General);
		Assert.IsTrue(generalEffect.AppliesToCheck(CheckType.GenericSkillCheck));
	}

	[TestMethod]
	public void DangerSenseDefensiveEdge_AppliesOnlyToDefensiveChecks()
	{
		var gameworld = CreateGameworld();
		var power = (DangerSensePower)MagicPowerFactory.LoadPower(CreatePowerModel("dangersense"), gameworld.Object);
		var owner = CreateCharacter(51, gameworld.Object);
		var effect = new DangerSenseDefensiveEdge(owner.Object, power, 3.0);

		Assert.AreEqual(3.0, effect.CheckBonus);
		Assert.IsTrue(effect.AppliesToCheck(CheckType.DangerSenseDefense));
		Assert.IsTrue(effect.AppliesToCheck(CheckType.DodgeCheck));
		Assert.IsFalse(effect.AppliesToCheck(CheckType.PsychicBoltPower));
	}

	[TestMethod]
	public void ConnectMindPower_LoadsAndAppliesTargetEligibilityProg()
	{
		var trueProg = CreateProg(0, true);
		var falseProg = CreateProg(2, false);
		var gameworld = CreateGameworld(progs: [trueProg.Object, falseProg.Object]);
		var power = (ConnectMindPower)MagicPowerFactory.LoadPower(CreateConnectMindModel(2), gameworld.Object);
		var owner = CreateCharacter(10, gameworld.Object);
		var target = CreateCharacter(11, gameworld.Object);
		var method = typeof(ConnectMindPower).GetMethod("TargetFilterFunction",
			BindingFlags.Instance | BindingFlags.NonPublic)!;

		Assert.AreSame(falseProg.Object, power.TargetEligibilityProg);
		Assert.AreEqual(false, method.Invoke(power, [owner.Object, target.Object]));
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersV4TagWardTypes()
	{
		var spell = CreateSpellMock();

		foreach (var type in new[] { "roomtagward", "personaltagward" })
		{
			var (effect, error) = SpellEffectFactory.LoadEffectFromBuilderInput(type, new StringStack(string.Empty),
				spell.Object);

			Assert.AreEqual(string.Empty, error);
			Assert.IsNotNull(effect);
			Assert.AreEqual(type, effect!.SaveToXml().Attribute("type")?.Value);
			Assert.IsTrue(SpellEffectFactory.BuilderInfoForType(type).MatchingTriggers.Any());
		}
	}

	[TestMethod]
	public void MagicTagEffect_ExposesInterdictionMetadata()
	{
		var spell = CreateSpellMock();
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "magictag"),
			new XElement("Tag", new XCData("discipline")),
			new XElement("Value", new XCData("mind")),
			new XElement("ReplaceExisting", true)), spell.Object);

		var tag = ((IMagicInterdictionTagProvider)effect).MagicInterdictionTags.Single();

		Assert.AreEqual("discipline", tag.Tag);
		Assert.AreEqual("mind", tag.Value);
	}

	[TestMethod]
	public void SubjectiveDescriptionEffect_RoundTripsPriorityAndIllusionKey()
	{
		var spell = CreateSpellMock();
		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "subjectivedesc"),
			new XElement("Description", new XCData("A false face.")),
			new XElement("FixedViewer", true),
			new XElement("Priority", 5),
			new XElement("OverrideKey", new XCData("false-face")),
			new XElement("ApplicabilityProg", 0)), spell.Object);

		var saved = effect.SaveToXml();

		Assert.AreEqual("5", saved.Element("Priority")!.Value);
		Assert.AreEqual("false-face", saved.Element("OverrideKey")!.Value);
	}

	[TestMethod]
	public void RuntimeSubjectiveDescriptionEffect_ExposesPriorityAndKey()
	{
		var gameworld = CreateGameworld();
		var owner = CreatePerceivable(gameworld.Object);
		var parent = CreateParent();
		var effect = new SpellSubjectiveDescriptionEffect(owner.Object, parent.Object,
			MudSharp.Form.Shape.DescriptionType.Full, "A false face.", 0, priority: 7, overrideKey: "mask");

		Assert.AreEqual(7, effect.OverridePriority);
		Assert.AreEqual("mask", effect.OverrideKey);
	}

	[TestMethod]
	public void PsionicTrafficHelper_RespectsIgnoreForceOptOut()
	{
		var target = new Mock<ICharacter>();
		target.Setup(x => x.AffectedBy<IIgnoreForceEffect>()).Returns(true);

		Assert.IsFalse(PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target.Object));
	}

	private static XElement InvokeSaveDefinition(IMagicPower power)
	{
		return (XElement)power.GetType()
		                      .GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
		                      .Invoke(power, [])!;
	}

	private static MagicPower CreatePowerModel(string type)
	{
		return new MagicPower
		{
			Id = Array.IndexOf(V4PowerTypes, type) + 1,
			Name = type,
			Blurb = type,
			ShowHelp = type,
			MagicSchoolId = 1,
			PowerModel = type,
			Definition = type switch
			{
				"hear" => SustainedDefinition("hear", "endhear",
					new XElement("PowerDistance", MagicPowerDistance.AnyConnectedMindOrConnectedTo),
					new XElement("ShowThinks", true),
					new XElement("ShowFeels", true),
					new XElement("ShowName", false),
					new XElement("ShowEmotes", true),
					new XElement("ShowDescriptionProg", 0L)).ToString(),
				"clairaudience" => ClairaudienceDefinition().ToString(),
				"allspeak" => SustainedDefinition("allspeak", "endallspeak").ToString(),
				"magicksense" => SustainedDefinition("magicksense", "endmagicksense").ToString(),
				"dangersense" => SustainedDefinition("dangersense", "enddangersense",
					new XElement("ThreatRange", 1U),
					new XElement("RespectDoors", true),
					new XElement("RespectCorners", false),
					new XElement("IncludeCurrentLocation", true),
					new XElement("OnlyNPCs", true),
					new XElement("ThreatDifficulty", (int)Difficulty.Normal),
					new XElement("DefenseDifficulty", (int)Difficulty.Normal),
					new XElement("DefenseBonus", 10.0),
					new XElement("DefenseDurationSeconds", 15.0),
					new XElement("ThreatWarningIntervalSeconds", 30.0),
					new XElement("ThreatEcho", new XCData("Danger.")),
					new XElement("DefenseEcho", new XCData("Defense."))).ToString(),
				"sensitivity" => SustainedDefinition("sensitivity", "endsensitivity",
					new XElement("ScanVerb", "senscan"),
					new XElement("ScanDistance", MagicPowerDistance.SameLocationOnly),
					new XElement("GrantedPerceptions", PerceptionTypes.SenseMagical | PerceptionTypes.SensePsychic),
					new XElement("ActivityKinds", "Magical,Psychic"),
					new XElement("ActivityRange", 2U),
					new XElement("ActivityDifficulty", (int)Difficulty.Normal),
					new XElement("CapabilityDifficulty", (int)Difficulty.ExtremelyHard),
					new XElement("PermitCapabilityRead", true),
					new XElement("NotifySelf", false),
					new XElement("ActivityEcho", new XCData("Activity."))).ToString(),
				"babble" => TargetedDefinition("babble", new XElement("DurationSeconds", 120.0)).ToString(),
				"coerce" => TargetedDefinition("coerce",
					new XElement("StaminaFractionPerDegree", 0.05),
					new XElement("NeedHoursPerDegree", 2.0)).ToString(),
				"empathy" => TargetedDefinition("empathy",
					new XElement("TransferIntervalSeconds", 10.0),
					new XElement("MaxWounds", 0),
					new XElement("SafetyHealthPercent", 0.75),
					new XElement("StartEcho", new XCData("Start.")),
					new XElement("TransferEcho", new XCData("Transfer.")),
					new XElement("StopEcho", new XCData("Stop.")),
					new XElement("SafetyEcho", new XCData("Safety."))).ToString(),
				"hex" => TargetedDefinition("hex",
					new XElement("DurationSeconds", 600.0),
					new XElement("Penalty", 10.0),
					new XElement("Categories", PsionicHexCheckCategory.All),
					new XElement("ReplaceExisting", true),
					new XElement("TargetEcho", new XCData("Target."))).ToString(),
				"psychicbolt" => TargetedDefinition("psychicbolt",
					new XElement("StunAmount", 20.0),
					new XElement("DamageType", MudSharp.Health.DamageType.Eldritch),
					new XElement("TargetEcho", new XCData("Target."))).ToString(),
				"prescience" => PrescienceDefinition().ToString(),
				_ => TargetedDefinition(type).ToString()
			}
		};
	}

	private static IEnumerable<string> OldSoiSubtypeElements(string type)
	{
		return type switch
		{
			"dangersense" =>
			[
				"ThreatRange",
				"RespectDoors",
				"RespectCorners",
				"IncludeCurrentLocation",
				"OnlyNPCs",
				"ThreatDifficulty",
				"DefenseDifficulty",
				"DefenseBonus",
				"DefenseDurationSeconds",
				"ThreatWarningIntervalSeconds",
				"ThreatEcho",
				"DefenseEcho"
			],
			"empathy" =>
			[
				"TransferIntervalSeconds",
				"MaxWounds",
				"SafetyHealthPercent",
				"StartEcho",
				"TransferEcho",
				"StopEcho",
				"SafetyEcho"
			],
			"hex" => ["DurationSeconds", "Penalty", "Categories", "ReplaceExisting", "TargetEcho"],
			"clairvoyance" =>
			[
				"Verb",
				"PowerDistance",
				"SkillCheckDifficulty",
				"SkillCheckTrait",
				"MinimumSuccessThreshold",
				"DetectableWithDetectMagic",
				"FailEcho",
				"SuccessEcho"
			],
			"prescience" =>
			[
				"Verb",
				"SkillCheckTrait",
				"SkillCheckDifficulty",
				"MinimumSuccessThreshold",
				"BoardIdOrName",
				"SubjectTemplate",
				"AuthorTemplate",
				"PromptText",
				"FailEcho",
				"SuccessEcho"
			],
			"sensitivity" =>
			[
				"ScanVerb",
				"ScanDistance",
				"GrantedPerceptions",
				"ActivityKinds",
				"ActivityRange",
				"ActivityDifficulty",
				"CapabilityDifficulty",
				"PermitCapabilityRead",
				"NotifySelf",
				"ActivityEcho"
			],
			"psychicbolt" => ["StunAmount", "DamageType", "TargetEcho"],
			_ => []
		};
	}

	private static XElement TargetedDefinition(string verb, params object[] additional)
	{
		var definition = new XElement("Definition",
			BaseElements(),
			new XElement("Verb", verb),
			new XElement("PowerDistance", MagicPowerDistance.AnyConnectedMindOrConnectedTo),
			new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
			new XElement("SkillCheckTrait", 1L),
			new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass),
			new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
			new XElement("FailEcho", new XCData("Fail.")),
			new XElement("SuccessEcho", new XCData(string.Empty))
		);
		foreach (var item in additional)
		{
			definition.Add(item);
		}

		return definition;
	}

	private static XElement SustainedDefinition(string begin, string end, params object[] additional)
	{
		var definition = new XElement("Definition",
			BaseElements(),
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

	private static XElement ClairaudienceDefinition()
	{
		var definition = SustainedDefinition("clairaudience", "endclairaudience");
		definition.Add(new XElement("PowerDistance", MagicPowerDistance.AnyConnectedMindOrConnectedTo));
		return definition;
	}

	private static XElement PrescienceDefinition()
	{
		return new XElement("Definition",
			BaseElements(),
			new XElement("Verb", "prescience"),
			new XElement("SkillCheckTrait", 1L),
			new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
			new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass),
			new XElement("BoardIdOrName", "Staff"),
			new XElement("SubjectTemplate", new XCData("Prescience: {character}")),
			new XElement("AuthorTemplate", new XCData("{character}")),
			new XElement("PromptText", new XCData("Prompt.")),
			new XElement("FailEcho", new XCData("Fail.")),
			new XElement("SuccessEcho", new XCData("Success."))
		);
	}

	private static object[] BaseElements()
	{
		return
		[
			new XElement("CanInvokePowerProg", 0L),
			new XElement("WhyCantInvokePowerProg", 0L),
			new XElement("IsPsionic", true),
			new XElement("InvocationCosts")
		];
	}

	private static string PrimaryVerb(string type)
	{
		return type switch
		{
			"hear" => "hear",
			"clairaudience" => "clairaudience",
			"allspeak" => "allspeak",
			"magicksense" => "magicksense",
			_ => type
		};
	}

	private static MagicPower CreateConnectMindModel(long eligibilityProg)
	{
		return new MagicPower
		{
			Id = 99,
			Name = "Connect Mind",
			Blurb = "Connect mind",
			ShowHelp = "Connect mind.",
			MagicSchoolId = 1,
			PowerModel = "connectmind",
			Definition = new XElement("Definition",
				BaseElements(),
				new XElement("ConnectVerb", "connect"),
				new XElement("DisconnectVerb", "disconnect"),
				new XElement("PowerDistance", (int)MagicPowerDistance.AnyConnectedMindOrConnectedTo),
				new XElement("SkillCheckDifficulty", (int)Difficulty.Normal),
				new XElement("SkillCheckTrait", 1L),
				new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass),
				new XElement("TargetCanSeeIdentityProg", 0L),
				new XElement("TargetEligibilityProg", eligibilityProg),
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

	private static Mock<IFuturemud> CreateGameworld(IEnumerable<IFutureProg>? progs = null)
	{
		var progList = (progs ?? [CreateProg(0, true).Object]).ToArray();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(CreateCollectionMock(progList).Object);
		gameworld.SetupGet(x => x.MagicSchools).Returns(CreateCollectionMock(CreateSchool().Object).Object);
		gameworld.SetupGet(x => x.Traits).Returns(CreateCollectionMock(CreateTrait().Object).Object);
		gameworld.SetupGet(x => x.AlwaysTrueProg).Returns(progList.FirstOrDefault(x => x.Id == 0) ?? CreateProg(0, true).Object);
		gameworld.SetupGet(x => x.AlwaysFalseProg).Returns(CreateProg(3, false).Object);
		gameworld.SetupGet(x => x.UniversalErrorTextProg).Returns(progList.FirstOrDefault(x => x.Id == 0) ?? CreateProg(0, true).Object);
		return gameworld;
	}

	private static Mock<IMagicSchool> CreateSchool()
	{
		var school = new Mock<IMagicSchool>();
		school.SetupGet(x => x.Id).Returns(1);
		school.SetupGet(x => x.Name).Returns("Psionics");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.BoldMagenta);
		school.SetupGet(x => x.SchoolVerb).Returns("psi");
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

	private static Mock<ICharacter> CreateCharacter(long id, IFuturemud gameworld)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns($"Character {id}");
		character.SetupGet(x => x.FrameworkItemType).Returns("Character");
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		return character;
	}

	private static Mock<IPerceivable> CreatePerceivable(IFuturemud gameworld)
	{
		var perceivable = new Mock<IPerceivable>();
		perceivable.SetupGet(x => x.Id).Returns(100);
		perceivable.SetupGet(x => x.Name).Returns("Perceivable");
		perceivable.SetupGet(x => x.FrameworkItemType).Returns("Perceivable");
		perceivable.SetupGet(x => x.Gameworld).Returns(gameworld);
		return perceivable;
	}

	private static Mock<IMagicSpellEffectParent> CreateParent()
	{
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Spell).Returns(CreateSpellMock().Object);
		return parent;
	}

	private static Mock<IMagicSpell> CreateSpellMock()
	{
		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Id).Returns(1);
		spell.SetupGet(x => x.Name).Returns("V4 Spell");
		spell.SetupGet(x => x.Gameworld).Returns(CreateGameworld().Object);
		spell.SetupGet(x => x.School).Returns(CreateSchool().Object);
		spell.SetupProperty(x => x.Changed);
		return spell;
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

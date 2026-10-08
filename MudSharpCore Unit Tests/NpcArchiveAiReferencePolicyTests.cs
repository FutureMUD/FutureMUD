#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.NPC.AI;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcArchiveAiReferencePolicyTests
{
	[DataTestMethod]
	[DataRow("Judge")]
	[DataRow("Mount")]
	[DataRow("Animal")]
	[DataRow("Monster")]
	public void Reference_RuntimeCodecRoundTrip_StaticCollisionsClearWithoutExecutingProgs(string type)
	{
		var row = Row(type, Definition(type));
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(row.Definition, 10, 11, 107, 209));
		Assert.IsFalse(Held(row));
		// A Definition string alone cannot select one of the four loaders.
		Assert.IsTrue(NpcArchiveReferencePolicy.HasReferenceOrUncertainty(typeof(Db.ArtificialIntelligence), "Definition",
			row.Definition, 10, 11, 107, 209));
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		var progs = new All<IFutureProg>();
		foreach (var id in new[] { 10L, 11L, 107L, 209L })
		{
			var prog = new Mock<IFutureProg>(MockBehavior.Strict);
			prog.SetupGet(x => x.Id).Returns(id);
			prog.SetupGet(x => x.Name).Returns($"static{id}");
			progs.Add(prog.Object);
		}
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		world.SetupGet(x => x.AlwaysTrueProg).Returns(progs.Get(10));
		world.SetupGet(x => x.AlwaysFalseProg).Returns(progs.Get(11));
		world.SetupGet(x => x.AlwaysOneProg).Returns(progs.Get(107));
		var runtimeType = type switch { "Judge" => typeof(JudgeAI), "Mount" => typeof(MountAI), "Animal" => typeof(AnimalAI), _ => typeof(MonsterAI) };
		var ai = runtimeType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
			[typeof(Db.ArtificialIntelligence), typeof(IFuturemud)], null)!.Invoke([row, world.Object]);
		var saved = (string)typeof(ArtificialIntelligenceBase).GetMethod("DefinitionForSave", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(ai, null)!;
		Assert.IsFalse(Held(Row(type, saved)), $"Actual {type} writer contract was rejected.");
		foreach (var prog in progs) Mock.Get(prog).Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("Animal")]
	[DataRow("Monster")]
	public void Reference_AbsentOptionalCreatureSections_LegacyDefaultsClear(string type) =>
		Assert.IsFalse(Held(Row(type, "<Definition/>")));

	[DataTestMethod]
	[DataRow("Judge")]
	[DataRow("Mount")]
	public void Reference_MissingRequiredCodecFields_Holds(string type)
	{
		Assert.IsTrue(Held(Row(type, "<Definition/>")));
		var root = XElement.Parse(Definition(type));
		foreach (var child in root.Elements().ToArray())
		{
			if (type == "Judge" && child.Name.LocalName.EndsWith("Prog", StringComparison.Ordinal)) continue;
			var mutated = new XElement(root); mutated.Element(child.Name)!.Remove();
			Assert.IsTrue(Held(Row(type, mutated.ToString())), child.Name.ToString());
		}
	}

	[DataTestMethod]
	[DataRow("animal")]
	[DataRow("MonsterFuture")]
	[DataRow("Enforcer")]
	[DataRow("")]
	public void Reference_UnknownOrMismatchedDiscriminator_RetainsGenericScan(string type)
	{
		Assert.IsTrue(Held(Row(type, Definition("Animal"))));
		Assert.IsFalse(Held(Row(type, "<Unknown><Value>99999</Value></Unknown>")));
		Assert.IsTrue(Held(Row(type, "<Unknown>")));
		Assert.IsTrue(Held(Row("Animal", Definition("Monster"))));
		Assert.IsTrue(Held(Row("Monster", Definition("Animal"))));
	}

	[DataTestMethod]
	[DataRow("Judge")]
	[DataRow("Mount")]
	[DataRow("Animal")]
	[DataRow("Monster")]
	public void Reference_UnknownShapeOrHiddenPhysicalPayload_HoldsEvenWithoutCollision(string type)
	{
		var original = XElement.Parse(Definition(type));
		var attribute = new XElement(original); attribute.SetAttributeValue("futureField", "99999");
		Assert.IsTrue(Held(Row(type, attribute.ToString())));
		var unknown = new XElement(original); unknown.Add(new XElement("Character", 99999));
		Assert.IsTrue(Held(Row(type, unknown.ToString())));
		var duplicate = new XElement(original); duplicate.Add(new XElement(original.Elements().First()));
		Assert.IsTrue(Held(Row(type, duplicate.ToString())));
		var namespaced = new XElement(original); namespaced.Name = XName.Get("Definition", "future");
		Assert.IsTrue(Held(Row(type, namespaced.ToString())));
		var mixed = new XElement(original); mixed.Add(new XText("unclassified"));
		Assert.IsTrue(Held(Row(type, mixed.ToString())));
		// Narrative leaves are text only, never a place to nest serialized actors.
		var nested = new XElement(original);
		var leaf = nested.Descendants().First(x => !x.HasElements);
		leaf.Add(new XElement("Body", 99999));
		Assert.IsTrue(Held(Row(type, nested.ToString())));
		Assert.IsTrue(Held(Row(type, "")));
		Assert.IsTrue(Held(Row(type, "<Definition>")));
	}

	[DataTestMethod]
	[DataRow("Animal", "<OpenDoors>1</OpenDoors>")]
	[DataRow("Animal", "<DoorSmashDelayProg>-1</DoorSmashDelayProg>")]
	[DataRow("Animal", "<Movement type='999'/>")]
	[DataRow("Animal", "<Movement><Range>10.5</Range></Movement>")]
	[DataRow("Animal", "<Movement><WanderChancePerMinute>NaN</WanderChancePerMinute></Movement>")]
	[DataRow("Animal", "<Movement><MovementCellProg>1.5</MovementCellProg></Movement>")]
	[DataRow("Animal", "<Home><BurrowCraftId>9223372036854775808</BurrowCraftId></Home>")]
	[DataRow("Animal", "<Awareness><Senses>999</Senses></Awareness>")]
	[DataRow("Animal", "<Refuge><Layer>999</Layer></Refuge>")]
	[DataRow("Animal", "<Feeding><EngageDelayDiceExpression>character(99999)</EngageDelayDiceExpression></Feeding>")]
	[DataRow("Animal", "<Feeding><EngageDelayDiceExpression>9999999999999999999999d1</EngageDelayDiceExpression></Feeding>")]
	[DataRow("Animal", "<Water enabled='perhaps'/>")]
	[DataRow("Animal", "<Threat><OrdinaryResponse>999</OrdinaryResponse></Threat>")]
	[DataRow("Animal", "<Threat><PostureDurationDiceExpression>1d0</PostureDurationDiceExpression></Threat>")]
	[DataRow("Animal", "<Activity><ActiveTime>999</ActiveTime></Activity>")]
	[DataRow("Animal", "<Ecology><SeasonalHabitat seasonGroup=''>10</SeasonalHabitat></Ecology>")]
	[DataRow("Animal", "<Ecology><SeasonalHabitat seasonGroup='wet'>10</SeasonalHabitat><SeasonalHabitat seasonGroup='WET'>11</SeasonalHabitat></Ecology>")]
	[DataRow("Animal", "<Hunting version='2'/>")]
	[DataRow("Animal", "<Hunting><People>999</People></Hunting>")]
	[DataRow("Animal", "<Hunting><TimeoutSeconds>Infinity</TimeoutSeconds></Hunting>")]
	[DataRow("Animal", "<Hunting><Include><Race>0</Race></Include></Hunting>")]
	[DataRow("Animal", "<Hunting><Include><Race>10</Race><Race>010</Race></Include></Hunting>")]
	[DataRow("Animal", "<Hunting><Prefer><Race id='10'>NaN</Race></Prefer></Hunting>")]
	[DataRow("Animal", "<Hunting><Weights><Weight name='character'>99999</Weight></Weights></Hunting>")]
	[DataRow("Animal", "<Hunting><Weights><Weight name='size'>10</Weight><Weight name='SIZE'>11</Weight></Weights></Hunting>")]
	[DataRow("Monster", "<Monster version='2'/>")]
	[DataRow("Monster", "<Monster><Motive>SelfDefence</Motive></Monster>")]
	[DataRow("Monster", "<Monster><Feeding>999</Feeding></Monster>")]
	[DataRow("Monster", "<Monster><LoadError>invalid configured motive</LoadError></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow version='2'/></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow><Phase>999</Phase></ActivityWindow></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow><Time>999</Time></ActivityWindow></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow><Calendar>-1</Calendar></ActivityWindow></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow><FirstDay>0</FirstDay><LastDay>10</LastDay></ActivityWindow></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow><Calendar>10</Calendar><FirstDay>11</FirstDay><LastDay>10</LastDay></ActivityWindow></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow><FirstDay>10</FirstDay></ActivityWindow></Monster>")]
	[DataRow("Monster", "<Monster><ActivityWindow><LoadError>invalid phase</LoadError></ActivityWindow></Monster>")]
	public void Reference_InvalidCreatureValues_Holds(string type, string section) =>
		Assert.IsTrue(Held(Row(type, $"<Definition>{section}</Definition>")));

	[TestMethod]
	public void Reference_JudgeLegacyProgNamesClear_ButInvalidDelaysAndMountValuesHold()
	{
		var judge = XElement.Parse(Definition("Judge"));
		judge.SetElementValue("IdentityProg", "static10");
		Assert.IsFalse(Held(Row("Judge", judge.ToString())));
		judge.SetElementValue("PleaDelay", "NaN");
		Assert.IsTrue(Held(Row("Judge", judge.ToString())));
		var mount = XElement.Parse(Definition("Mount"));
		mount.SetElementValue("MountControlDifficulty", 999);
		Assert.IsTrue(Held(Row("Mount", mount.ToString())));
		mount.SetElementValue("MountControlDifficulty", 1); mount.SetElementValue("MaximumNumberOfRiders", 0);
		Assert.IsTrue(Held(Row("Mount", mount.ToString())));
	}

	[DataTestMethod]
	[DataRow("Animal", false)]
	[DataRow("animal", true)]
	[DataRow("Monster", true)]
	public void SerializedScan_PairsPersistedDiscriminatorWithDefinition(string type, bool expectedHold)
	{
		using var context = Context();
		context.ArtificialIntelligences.Add(Row(type, Definition("Animal"))); context.SaveChanges();
		Assert.AreEqual(expectedHold, !SerializedClear(context, out var diagnostic));
		if (expectedHold) StringAssert.Contains(diagnostic, "ArtificialIntelligence.Definition");
	}

	[TestMethod]
	public void SerializedScan_OversizedKnownPayload_RetainsHold()
	{
		using var context = Context();
		context.ArtificialIntelligences.Add(Row("Animal", $"<Definition><Movement><WanderEmote>{new string('x', 1048576)}</WanderEmote></Movement></Definition>"));
		context.SaveChanges();
		Assert.IsFalse(SerializedClear(context, out var diagnostic));
		StringAssert.Contains(diagnostic, "scan limit");
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	private static bool SerializedClear(FuturemudDatabaseContext context, out string diagnostic)
	{
		object?[] args = [context, 10L, 11L, new long[] { 107, 209 }, null];
		var clear = (bool)typeof(CharacterArchiveService).GetMethod("SerializedReferencesAreClear", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args)!;
		diagnostic = (string)args[4]!; return clear;
	}
	private static Db.ArtificialIntelligence Row(string type, string definition) => new() { Id = 1, Name = "static-test", Type = type, Definition = definition };
	private static bool Held(Db.ArtificialIntelligence row) => NpcArchiveReferencePolicy.HasReferenceOrUncertainty(row, 10, 11, out _, 107, 209);

	private static string Definition(string type)
	{
		if (type == "Judge")
		{
			return new XElement("Definition",
				"IdentityProg WarnEchoProg WarnStartMoveEchoProg FailToComplyEchoProg ThrowInPrisonEchoProg".Split(' ').Select(x => new XElement(x, 107)),
				"IntroductionDelay ChargesDelay PleaDelay CaseDelayPerCrime ClosingArgumentDelay VerdictDelay SentencingDelay".Split(' ').Select(x => new XElement(x, 10)),
				("TrialIntroductionEmote TrialChargesEmote TrialPleaEmote TrialDefaultPleaEnteredEmote TrialCaseEmote TrialClosingArgumentsEmote " +
				 "TrialEndArgumentsEmote TrialVerdictGuiltyEmote TrialVerdictNotGuiltyEmote TrialSentencingEmote TrialEndFreeToGo " +
				 "TrialEndRemandedIntoCustody TrialEndRemandedAwaitingExecution").Split(' ').Select(x => new XElement(x, new XCData("@ say|says {10} {11} 209.")))).ToString();
		}
		if (type == "Mount") return "<Definition><PermitRiderProg>107</PermitRiderProg><PermitControlProg>11</PermitControlProg>" +
			"<WhyCannotPermitRiderProg>209</WhyCannotPermitRiderProg><MountNonConsensualMountDifficulty>10</MountNonConsensualMountDifficulty>" +
			"<MountControlDifficulty>1</MountControlDifficulty><MountResistBuckDifficulty>5</MountResistBuckDifficulty>" +
			"<MaximumNumberOfRiders>1</MaximumNumberOfRiders><RawMountEmote>209</RawMountEmote><RawDismountEmote/>" +
			"<RawControlDeniedEmote/><RawBuckEmote/></Definition>";
		var hunting = new AnimalHuntingSettings { Enabled = true, ClassificationProgId = 10, EligibilityProgId = 107, PreferenceProgId = 209 };
		hunting.IncludedRaces.UnionWith([10, 11]); hunting.ExcludedRaces.Add(107); hunting.PreferredRaces.Add(209, 10);
		var root = XElement.Parse("<Definition><Movement type='Ground'><Range>10</Range><MovementCellProg>107</MovementCellProg>" +
			"<WanderEmote>209</WanderEmote></Movement><Home type='Denning'><BurrowCraftId>11</BurrowCraftId><AnchorItemProg>209</AnchorItemProg>" +
			"</Home><Awareness><MemoryMinutes>10</MemoryMinutes><ThreatProg>11</ThreatProg></Awareness>" +
			"<Refuge><ReturnSeconds>10</ReturnSeconds><CellProg>107</CellProg></Refuge><DoorSmashDelayProg>209</DoorSmashDelayProg></Definition>");
		root.Add(hunting.Save());
		if (type == "Animal") root.Add(XElement.Parse("<Feeding type='Predator'><WillAttackProg>107</WillAttackProg><EngageDelayDiceExpression>10+1d11</EngageDelayDiceExpression></Feeding>"),
			new XElement("Water", new XAttribute("enabled", true)), new XElement("Threat", new XElement("PostureDurationDiceExpression", "10+1d11")),
			new XElement("Activity", new XElement("ActiveTime", TimeOfDay.Night), new XElement("DormantSeasonGroup", "season10")),
			new XElement("Ecology", new XElement("SeasonalHabitat", new XAttribute("seasonGroup", "season11"), 209)));
		else
		{
			var window = new MonsterActivityWindow { CalendarId = 10, MoonId = 11, ConditionProgId = 209, FirstDay = 10, LastDay = 11 };
			window.Times.Add(TimeOfDay.Night); window.Phases.Add(Enum.GetValues<MoonPhase>().First()); window.Months.Add("month107");
			root.Add(new XElement("Monster", new XAttribute("version", 1), new XElement("Motive", "Scheduled"), window.Save(),
				new XElement("AllyProg", 107), new XElement("EngageDelay", "10+1d11"), new XElement("WarningSeconds", 10)));
		}
		return root.ToString();
	}
}

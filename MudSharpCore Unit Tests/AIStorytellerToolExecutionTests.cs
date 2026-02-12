#nullable enable
#pragma warning disable OPENAI001
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Communication.Language;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.AIStorytellers;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using OpenAI.Responses;
using ModelStoryteller = MudSharp.Models.AIStoryteller;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerToolExecutionTests
{
	[TestMethod]
	public void NormalizeFunctionToolSchema_NullSchema_ReturnsClosedEmptyObjectSchema()
	{
		var normalized = AIStoryteller.NormalizeFunctionToolSchema(null!);
		var payload = JsonDocument.Parse(normalized).RootElement;

		Assert.AreEqual("object", payload.GetProperty("type").GetString());
		Assert.IsFalse(payload.GetProperty("additionalProperties").GetBoolean());
		Assert.AreEqual(0, payload.GetProperty("properties").EnumerateObject().Count());
		Assert.AreEqual(0, payload.GetProperty("required").GetArrayLength());
	}

	[TestMethod]
	public void NormalizeFunctionToolSchema_ObjectSchemasAreClosedForStrictMode()
	{
		var normalized = AIStoryteller.NormalizeFunctionToolSchema(
			"""
			{
			  "type": "object",
			  "properties": {
			    "Title": {
			      "type": "string"
			    },
			    "Effect": {
			      "type": "object",
			      "properties": {
			        "EffectId": {
			          "type": "integer"
			        }
			      },
			      "required": ["EffectId"],
			      "additionalProperties": true
			    }
			  },
			  "required": ["Title", "Effect"]
			}
			""");

		var payload = JsonDocument.Parse(normalized).RootElement;
		var effect = payload.GetProperty("properties").GetProperty("Effect");

		Assert.IsFalse(payload.GetProperty("additionalProperties").GetBoolean());
		Assert.IsFalse(effect.GetProperty("additionalProperties").GetBoolean());
	}

	[TestMethod]
	public void NormalizeFunctionToolSchema_OptionalProperty_BecomesNullableAndRequired()
	{
		var normalized = AIStoryteller.NormalizeFunctionToolSchema(
			"""
			{
			  "type": "object",
			  "properties": {
			    "Query": {
			      "type": "string"
			    }
			  },
			  "required": []
			}
			""");

		var payload = JsonDocument.Parse(normalized).RootElement;
		var required = payload.GetProperty("required").EnumerateArray().Select(x => x.GetString()).ToList();
		var queryTypes = payload.GetProperty("properties")
			.GetProperty("Query")
			.GetProperty("type")
			.EnumerateArray()
			.Select(x => x.GetString())
			.ToList();

		Assert.AreEqual(1, required.Count);
		Assert.AreEqual("Query", required[0]);
		CollectionAssert.Contains(queryTypes, "string");
		CollectionAssert.Contains(queryTypes, "null");
	}

	[TestMethod]
	public void ExecuteFunctionCall_MalformedJson_ReturnsMalformedError()
	{
		var storyteller = CreateStoryteller();

		var result = storyteller.ExecuteFunctionCall("Noop", "{", includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(result.MalformedJson);
		Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
		StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty, "Malformed tool-call JSON");
	}

	[TestMethod]
	public void ProcessFunctionCallBatch_MalformedJson_AddsRetryFeedback()
	{
		var storyteller = CreateStoryteller();
		var messages = new List<ResponseItem>();

		var result = storyteller.ProcessFunctionCallBatch(
			[("call-1", "Noop", "{")],
			includeEchoTools: false,
			messages,
			malformedRetries: 0);

		Assert.IsTrue(result.Continue);
		Assert.AreEqual(1, result.MalformedRetries);
		Assert.AreEqual(2, messages.Count);
	}

	[TestMethod]
	public void ProcessFunctionCallBatch_OverRetryBudget_LogsAndStops()
	{
		var storyteller = CreateStoryteller();
		string? loggedMessage = null;
		storyteller.ErrorLoggerOverride = message => loggedMessage = message;

		var retries = 0;
		var messages = new List<ResponseItem>();
		var result = (Continue: true, MalformedRetries: 0);
		for (var i = 0; i < 10; i++)
		{
			result = storyteller.ProcessFunctionCallBatch(
				[("call-1", "Noop", "{")],
				includeEchoTools: false,
				messages,
				retries);
			retries = result.MalformedRetries;
			if (!result.Continue)
			{
				break;
			}
		}

		Assert.IsFalse(result.Continue);
		Assert.IsFalse(string.IsNullOrWhiteSpace(loggedMessage));
		StringAssert.Contains(loggedMessage, "Malformed tool-call JSON retry budget exceeded.");
	}

	[TestMethod]
	public void ExecuteFunctionCall_UpdateSituation_InvokesUpdate()
	{
		var storyteller = CreateStoryteller();
		var title = "Old title";
		var details = "Old details";
		var situation = new Mock<IAIStorytellerSituation>();
		situation.SetupGet(x => x.Id).Returns(25L);
		situation.SetupGet(x => x.Name).Returns(() => title);
		situation.SetupGet(x => x.SituationText).Returns(() => details);
		situation.SetupGet(x => x.IsResolved).Returns(false);
		situation.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
		situation.Setup(x => x.UpdateSituation(It.IsAny<string>(), It.IsAny<string>()))
			.Callback<string, string>((newTitle, newDetails) =>
			{
				title = newTitle;
				details = newDetails;
			});
		storyteller.RegisterLoadedSituation(situation.Object);

		var result = storyteller.ExecuteFunctionCall("UpdateSituation",
			"""{"Id":25,"Title":"New title","Description":"Updated details"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsFalse(result.MalformedJson);
		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		situation.Verify(x => x.UpdateSituation("New title", "Updated details"), Times.Once);
	}

	[TestMethod]
	public void ExecuteFunctionCall_ResolveSituation_InvokesUpdateAndResolve()
	{
		var storyteller = CreateStoryteller();
		var title = "Before";
		var details = "Before details";
		var resolved = false;
		var situation = new Mock<IAIStorytellerSituation>();
		situation.SetupGet(x => x.Id).Returns(26L);
		situation.SetupGet(x => x.Name).Returns(() => title);
		situation.SetupGet(x => x.SituationText).Returns(() => details);
		situation.SetupGet(x => x.IsResolved).Returns(() => resolved);
		situation.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
		situation.Setup(x => x.UpdateSituation(It.IsAny<string>(), It.IsAny<string>()))
			.Callback<string, string>((newTitle, newDetails) =>
			{
				title = newTitle;
				details = newDetails;
			});
		situation.Setup(x => x.Resolve()).Callback(() => resolved = true);
		storyteller.RegisterLoadedSituation(situation.Object);

		var result = storyteller.ExecuteFunctionCall("ResolveSituation",
			"""{"Id":26,"Title":"After","Description":"After details"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsFalse(result.MalformedJson);
		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		situation.Verify(x => x.UpdateSituation("After", "After details"), Times.Once);
		situation.Verify(x => x.Resolve(), Times.Once);
	}

	[TestMethod]
	public void ExecuteFunctionCall_ForgetMemory_RemovesMemoryFromStoryteller()
	{
		var storyteller = CreateStoryteller();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(100L);

		var memory = new Mock<IAIStorytellerCharacterMemory>();
		memory.SetupGet(x => x.Id).Returns(11L);
		memory.SetupGet(x => x.MemoryTitle).Returns("Memory title");
		memory.SetupGet(x => x.MemoryText).Returns("Memory details");
		memory.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
		memory.SetupGet(x => x.Character).Returns(character.Object);
		storyteller.RegisterLoadedMemory(memory.Object);

		var forget = storyteller.ExecuteFunctionCall("ForgetMemory", """{"Id":11}""", includeEchoTools: false);
		var forgetPayload = JsonDocument.Parse(forget.OutputJson).RootElement;

		Assert.IsTrue(forgetPayload.GetProperty("ok").GetBoolean());
		memory.Verify(x => x.Forget(), Times.Once);

		var update = storyteller.ExecuteFunctionCall("UpdateMemory",
			"""{"Id":11,"Title":"After","Details":"After details"}""",
			includeEchoTools: false);
		var updatePayload = JsonDocument.Parse(update.OutputJson).RootElement;
		Assert.IsFalse(updatePayload.GetProperty("ok").GetBoolean());
	}

	[TestMethod]
	public void ExecuteFunctionCall_CustomToolGenderConversion_ParsesAndExecutes()
	{
		var storyteller = CreateStoryteller();
		object[]? capturedArgs = null;
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters)
			.Returns(new List<Tuple<ProgVariableTypes, string>>
			{
				Tuple.Create(ProgVariableTypes.Gender, "SubjectGender")
			});
		prog.SetupGet(x => x.CompileError).Returns(string.Empty);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
		prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
			.Callback((object[] args) => capturedArgs = args)
			.Returns("ok");

		storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomGenderTool", "desc", prog.Object));

		var result = storyteller.ExecuteFunctionCall("CustomGenderTool",
			"""{"SubjectGender":"male"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsNotNull(capturedArgs);
		Assert.AreEqual(1, capturedArgs!.Length);
		Assert.AreEqual(Gender.Male, capturedArgs[0]);
	}

	[TestMethod]
	public void ExecuteFunctionCall_InvalidCustomToolCompileError_ReturnsError()
	{
		var storyteller = CreateStoryteller();
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters)
			.Returns(new List<Tuple<ProgVariableTypes, string>>
			{
				Tuple.Create(ProgVariableTypes.Text, "Cue")
			});
		prog.SetupGet(x => x.CompileError).Returns("Compile failure");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
		storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("BadTool", "desc", prog.Object));

		var result = storyteller.ExecuteFunctionCall("BadTool",
			"""{"Cue":"x"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
		StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty, "does not compile");
	}

	[TestMethod]
	public void ExecuteFunctionCall_CustomToolOutfitConversion_ParsesAndExecutes()
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(200L);

		var outfit = new Mock<IOutfit>();
		outfit.SetupGet(x => x.Name).Returns("Guard Uniform");
		character.SetupGet(x => x.Outfits).Returns([outfit.Object]);

		var storyteller = CreateStoryteller(characters: [character.Object]);
		object[]? capturedArgs = null;
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters)
			.Returns(new List<Tuple<ProgVariableTypes, string>>
			{
				Tuple.Create(ProgVariableTypes.Outfit, "Wardrobe")
			});
		prog.SetupGet(x => x.CompileError).Returns(string.Empty);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
			.Callback((object[] args) => capturedArgs = args)
			.Returns(true);
		storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomOutfitTool", "desc", prog.Object));

		var result = storyteller.ExecuteFunctionCall("CustomOutfitTool",
			"""{"Wardrobe":{"OwnerCharacterId":200,"OutfitName":"Guard Uniform"}}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsNotNull(capturedArgs);
		Assert.AreEqual(1, capturedArgs!.Length);
		Assert.AreSame(outfit.Object, capturedArgs[0]);
	}

	[TestMethod]
	public void ExecuteFunctionCall_CustomToolOutfitItemConversion_ParsesAndExecutes()
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(201L);

		var outfitItem = new Mock<IOutfitItem>();
		outfitItem.SetupGet(x => x.Id).Returns(999L);

		var outfit = new Mock<IOutfit>();
		outfit.SetupGet(x => x.Name).Returns("Harbour Kit");
		outfit.SetupGet(x => x.Items).Returns([outfitItem.Object]);
		character.SetupGet(x => x.Outfits).Returns([outfit.Object]);

		var storyteller = CreateStoryteller(characters: [character.Object]);
		object[]? capturedArgs = null;
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters)
			.Returns(new List<Tuple<ProgVariableTypes, string>>
			{
				Tuple.Create(ProgVariableTypes.OutfitItem, "LoadoutItem")
			});
		prog.SetupGet(x => x.CompileError).Returns(string.Empty);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
			.Callback((object[] args) => capturedArgs = args)
			.Returns(true);
		storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomOutfitItemTool", "desc", prog.Object));

		var result = storyteller.ExecuteFunctionCall("CustomOutfitItemTool",
			"""{"LoadoutItem":{"OwnerCharacterId":201,"OutfitName":"Harbour Kit","ItemId":999}}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsNotNull(capturedArgs);
		Assert.AreEqual(1, capturedArgs!.Length);
		Assert.AreSame(outfitItem.Object, capturedArgs[0]);
	}

	[TestMethod]
	public void ExecuteFunctionCall_CustomToolEffectConversion_ParsesAndExecutes()
	{
		var effect = new Mock<IEffect>();
		effect.SetupGet(x => x.Id).Returns(777L);

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(202L);
		character.SetupGet(x => x.Effects).Returns([effect.Object]);

		var storyteller = CreateStoryteller(characters: [character.Object]);
		object[]? capturedArgs = null;
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters)
			.Returns(new List<Tuple<ProgVariableTypes, string>>
			{
				Tuple.Create(ProgVariableTypes.Effect, "AppliedEffect")
			});
		prog.SetupGet(x => x.CompileError).Returns(string.Empty);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
			.Callback((object[] args) => capturedArgs = args)
			.Returns(true);
		storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomEffectTool", "desc", prog.Object));

		var result = storyteller.ExecuteFunctionCall("CustomEffectTool",
			"""{"AppliedEffect":{"EffectId":777,"OwnerType":"character","OwnerId":202}}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsNotNull(capturedArgs);
		Assert.AreEqual(1, capturedArgs!.Length);
		Assert.AreSame(effect.Object, capturedArgs[0]);
	}

	[TestMethod]
	public void ExecuteFunctionCall_CustomToolExpandedTypeParsers_ParsesAndExecutes()
	{
		var shard = new Mock<IShard>();
		shard.SetupGet(x => x.Id).Returns(11L);
		shard.SetupGet(x => x.Name).Returns("Shard Alpha");

		var zone = new Mock<IZone>();
		zone.SetupGet(x => x.Id).Returns(12L);
		zone.SetupGet(x => x.Name).Returns("Harbour Zone");

		var race = new Mock<IRace>();
		race.SetupGet(x => x.Id).Returns(13L);
		race.SetupGet(x => x.Name).Returns("Human");

		var culture = new Mock<ICulture>();
		culture.SetupGet(x => x.Id).Returns(14L);
		culture.SetupGet(x => x.Name).Returns("Imperial");

		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Id).Returns(15L);
		trait.SetupGet(x => x.Name).Returns("Swordsmanship");

		var ethnicity = new Mock<IEthnicity>();
		ethnicity.SetupGet(x => x.Id).Returns(16L);
		ethnicity.SetupGet(x => x.Name).Returns("Coastal");

		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(17L);
		clan.SetupGet(x => x.Name).Returns("Wardens");
		clan.SetupGet(x => x.Names).Returns(["Wardens"]);

		var rank = new Mock<IRank>();
		rank.SetupGet(x => x.Id).Returns(18L);
		rank.SetupGet(x => x.Name).Returns("Captain");

		var appointment = new Mock<IAppointment>();
		appointment.SetupGet(x => x.Id).Returns(19L);
		appointment.SetupGet(x => x.Name).Returns("Marshal");

		var paygrade = new Mock<IPaygrade>();
		paygrade.SetupGet(x => x.Id).Returns(20L);
		paygrade.SetupGet(x => x.Name).Returns("Grade-3");
		paygrade.SetupGet(x => x.Abbreviation).Returns("G3");

		clan.SetupGet(x => x.Ranks).Returns([rank.Object]);
		clan.SetupGet(x => x.Appointments).Returns([appointment.Object]);
		clan.SetupGet(x => x.Paygrades).Returns([paygrade.Object]);

		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(21L);
		currency.SetupGet(x => x.Name).Returns("Sovereign");

		var exit = new Mock<IExit>();
		var exitManager = new Mock<IExitManager>();
		exitManager.Setup(x => x.GetExitByID(22L)).Returns(exit.Object);

		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(23L);
		language.SetupGet(x => x.Name).Returns("Anglic");

		var accent = new Mock<IAccent>();
		accent.SetupGet(x => x.Id).Returns(24L);
		accent.SetupGet(x => x.Name).Returns("Harbour Drawl");

		var merit = new Mock<IMerit>();
		merit.SetupGet(x => x.Id).Returns(25L);
		merit.SetupGet(x => x.Name).Returns("Quick Learner");

		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.Id).Returns(26L);
		calendar.SetupGet(x => x.Name).Returns("Solar Calendar");
		calendar.SetupGet(x => x.Names).Returns(["Solar Calendar", "solar"]);

		var clock = new Mock<IClock>();
		clock.SetupGet(x => x.Id).Returns(27L);
		clock.SetupGet(x => x.Name).Returns("City Clock");
		clock.SetupGet(x => x.Names).Returns(["City Clock", "city"]);

		var knowledge = new Mock<IKnowledge>();
		knowledge.SetupGet(x => x.Id).Returns(28L);
		knowledge.SetupGet(x => x.Name).Returns("Lore of Ports");

		var role = new Mock<IChargenRole>();
		role.SetupGet(x => x.Id).Returns(29L);
		role.SetupGet(x => x.Name).Returns("Dockmaster");

		var drug = new Mock<IDrug>();
		drug.SetupGet(x => x.Id).Returns(30L);
		drug.SetupGet(x => x.Name).Returns("Blackleaf");

		var shop = new Mock<IShop>();
		shop.SetupGet(x => x.Id).Returns(31L);
		shop.SetupGet(x => x.Name).Returns("Harbour Outfitters");

		var gameworld = CreateGameworld(Array.Empty<IFutureProg>(), Array.Empty<ICharacter>());
		gameworld.SetupGet(x => x.Shards).Returns(BuildRepository([shard.Object]).Object);
		gameworld.SetupGet(x => x.Zones).Returns(BuildRepository([zone.Object]).Object);
		gameworld.SetupGet(x => x.Races).Returns(BuildRepository([race.Object]).Object);
		gameworld.SetupGet(x => x.Cultures).Returns(BuildRepository([culture.Object]).Object);
		gameworld.SetupGet(x => x.Traits).Returns(BuildRepository([trait.Object]).Object);
		gameworld.SetupGet(x => x.Clans).Returns(BuildRepository([clan.Object]).Object);
		gameworld.SetupGet(x => x.Ethnicities).Returns(BuildRepository([ethnicity.Object]).Object);
		gameworld.SetupGet(x => x.Currencies).Returns(BuildRepository([currency.Object]).Object);
		gameworld.SetupGet(x => x.Languages).Returns(BuildRepository([language.Object]).Object);
		gameworld.SetupGet(x => x.Accents).Returns(BuildRepository([accent.Object]).Object);
		gameworld.SetupGet(x => x.Merits).Returns(BuildRepository([merit.Object]).Object);
		gameworld.SetupGet(x => x.Calendars).Returns(BuildRepository([calendar.Object]).Object);
		gameworld.SetupGet(x => x.Clocks).Returns(BuildRepository([clock.Object]).Object);
		gameworld.SetupGet(x => x.Knowledges).Returns(BuildRepository([knowledge.Object]).Object);
		gameworld.SetupGet(x => x.Roles).Returns(BuildRepository([role.Object]).Object);
		gameworld.SetupGet(x => x.Drugs).Returns(BuildRepository([drug.Object]).Object);
		gameworld.SetupGet(x => x.Shops).Returns(BuildRepository([shop.Object]).Object);
		gameworld.SetupGet(x => x.ExitManager).Returns(exitManager.Object);

		var storyteller = new AIStoryteller(CreateModel(), gameworld.Object);
		object[]? capturedArgs = null;
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters)
			.Returns(new List<Tuple<ProgVariableTypes, string>>
			{
				Tuple.Create(ProgVariableTypes.Shard, "ShardArg"),
				Tuple.Create(ProgVariableTypes.Zone, "ZoneArg"),
				Tuple.Create(ProgVariableTypes.Gender, "GenderArg"),
				Tuple.Create(ProgVariableTypes.Race, "RaceArg"),
				Tuple.Create(ProgVariableTypes.Culture, "CultureArg"),
				Tuple.Create(ProgVariableTypes.Trait, "TraitArg"),
				Tuple.Create(ProgVariableTypes.Clan, "ClanArg"),
				Tuple.Create(ProgVariableTypes.Ethnicity, "EthnicityArg"),
				Tuple.Create(ProgVariableTypes.ClanRank, "ClanRankArg"),
				Tuple.Create(ProgVariableTypes.ClanAppointment, "ClanAppointmentArg"),
				Tuple.Create(ProgVariableTypes.ClanPaygrade, "ClanPaygradeArg"),
				Tuple.Create(ProgVariableTypes.Currency, "CurrencyArg"),
				Tuple.Create(ProgVariableTypes.Exit, "ExitArg"),
				Tuple.Create(ProgVariableTypes.DateTime, "DateTimeArg"),
				Tuple.Create(ProgVariableTypes.TimeSpan, "TimeSpanArg"),
				Tuple.Create(ProgVariableTypes.Language, "LanguageArg"),
				Tuple.Create(ProgVariableTypes.Accent, "AccentArg"),
				Tuple.Create(ProgVariableTypes.Merit, "MeritArg"),
				Tuple.Create(ProgVariableTypes.MudDateTime, "MudDateTimeArg"),
				Tuple.Create(ProgVariableTypes.Calendar, "CalendarArg"),
				Tuple.Create(ProgVariableTypes.Clock, "ClockArg"),
				Tuple.Create(ProgVariableTypes.Knowledge, "KnowledgeArg"),
				Tuple.Create(ProgVariableTypes.Role, "RoleArg"),
				Tuple.Create(ProgVariableTypes.Drug, "DrugArg"),
				Tuple.Create(ProgVariableTypes.Shop, "ShopArg")
			});
		prog.SetupGet(x => x.CompileError).Returns(string.Empty);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
			.Callback((object[] args) => capturedArgs = args)
			.Returns(true);
		storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomExpandedTool", "desc", prog.Object));

		var result = storyteller.ExecuteFunctionCall("CustomExpandedTool",
			"""
			{
			  "ShardArg": {"ShardName":"Shard Alpha"},
			  "ZoneArg": "Harbour Zone",
			  "GenderArg": {"Gender":"female"},
			  "RaceArg": {"RaceName":"Human"},
			  "CultureArg": {"CultureName":"Imperial"},
			  "TraitArg": {"TraitName":"Swordsmanship"},
			  "ClanArg": {"ClanName":"Wardens"},
			  "EthnicityArg": {"EthnicityName":"Coastal"},
			  "ClanRankArg": {"Clan":"Wardens","RankName":"Captain"},
			  "ClanAppointmentArg": {"ClanName":"Wardens","AppointmentName":"Marshal"},
			  "ClanPaygradeArg": {"Clan":"Wardens","PaygradeAbbreviation":"G3"},
			  "CurrencyArg": {"CurrencyName":"Sovereign"},
			  "ExitArg": {"ExitId":22},
			  "DateTimeArg": {"Value":"2026-02-11T12:34:56Z"},
			  "TimeSpanArg": {"TotalSeconds":90},
			  "LanguageArg": {"LanguageName":"Anglic"},
			  "AccentArg": {"AccentName":"Harbour Drawl"},
			  "MeritArg": {"MeritName":"Quick Learner"},
			  "MudDateTimeArg": {"Value":"never"},
			  "CalendarArg": {"Alias":"solar"},
			  "ClockArg": {"Alias":"city"},
			  "KnowledgeArg": {"KnowledgeName":"Lore of Ports"},
			  "RoleArg": {"RoleName":"Dockmaster"},
			  "DrugArg": {"DrugName":"Blackleaf"},
			  "ShopArg": {"ShopName":"Harbour Outfitters"}
			}
			""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsNotNull(capturedArgs);
		Assert.AreEqual(25, capturedArgs!.Length);
		Assert.AreSame(shard.Object, capturedArgs[0]);
		Assert.AreSame(zone.Object, capturedArgs[1]);
		Assert.AreEqual(Gender.Female, capturedArgs[2]);
		Assert.AreSame(race.Object, capturedArgs[3]);
		Assert.AreSame(culture.Object, capturedArgs[4]);
		Assert.AreSame(trait.Object, capturedArgs[5]);
		Assert.AreSame(clan.Object, capturedArgs[6]);
		Assert.AreSame(ethnicity.Object, capturedArgs[7]);
		Assert.AreSame(rank.Object, capturedArgs[8]);
		Assert.AreSame(appointment.Object, capturedArgs[9]);
		Assert.AreSame(paygrade.Object, capturedArgs[10]);
		Assert.AreSame(currency.Object, capturedArgs[11]);
		Assert.AreSame(exit.Object, capturedArgs[12]);
		Assert.AreEqual(DateTime.Parse("2026-02-11T12:34:56Z").ToUniversalTime(),
			((DateTime)capturedArgs[13]).ToUniversalTime());
		Assert.AreEqual(TimeSpan.FromSeconds(90), (TimeSpan)capturedArgs[14]);
		Assert.AreSame(language.Object, capturedArgs[15]);
		Assert.AreSame(accent.Object, capturedArgs[16]);
		Assert.AreSame(merit.Object, capturedArgs[17]);
		Assert.IsInstanceOfType(capturedArgs[18], typeof(MudDateTime));
		Assert.IsNull(((MudDateTime)capturedArgs[18]).Date);
		Assert.AreSame(calendar.Object, capturedArgs[19]);
		Assert.AreSame(clock.Object, capturedArgs[20]);
		Assert.AreSame(knowledge.Object, capturedArgs[21]);
		Assert.AreSame(role.Object, capturedArgs[22]);
		Assert.AreSame(drug.Object, capturedArgs[23]);
		Assert.AreSame(shop.Object, capturedArgs[24]);
	}

	[TestMethod]
	public void ExecuteFunctionCall_PathBetweenRooms_ReturnsDirectionsPayload()
	{
		var room = new Mock<ICell>();
		room.SetupGet(x => x.Id).Returns(100L);
		var storyteller = CreateStoryteller(cells: [room.Object]);

		var result = storyteller.ExecuteFunctionCall("PathBetweenRooms",
			"""{"OriginRoomId":100,"DestinationRoomId":100,"PathSearchFunction":"IgnorePresenceOfDoors"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;
		var directions = payload.GetProperty("result").GetProperty("Directions");

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsTrue(payload.GetProperty("result").GetProperty("HasPath").GetBoolean());
		Assert.AreEqual(0, directions.GetArrayLength());
	}

	[TestMethod]
	public void ExecuteFunctionCall_PathFromCharacterToRoom_ReturnsDirectionsPayload()
	{
		var room = new Mock<ICell>();
		room.SetupGet(x => x.Id).Returns(101L);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(1L);
		character.SetupGet(x => x.Location).Returns(room.Object);
		var storyteller = CreateStoryteller(characters: [character.Object], cells: [room.Object]);

		var result = storyteller.ExecuteFunctionCall("PathFromCharacterToRoom",
			"""{"OriginCharacterId":1,"DestinationRoomId":101,"PathSearchFunction":"PathIncludeUnlockableDoors"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;
		var directions = payload.GetProperty("result").GetProperty("Directions");

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsTrue(payload.GetProperty("result").GetProperty("HasPath").GetBoolean());
		Assert.AreEqual(0, directions.GetArrayLength());
	}

	[TestMethod]
	public void ExecuteFunctionCall_PathBetweenCharacters_ReturnsDirectionsPayload()
	{
		var room = new Mock<ICell>();
		room.SetupGet(x => x.Id).Returns(102L);
		var origin = new Mock<ICharacter>();
		origin.SetupGet(x => x.Id).Returns(11L);
		origin.SetupGet(x => x.Location).Returns(room.Object);
		var destination = new Mock<ICharacter>();
		destination.SetupGet(x => x.Id).Returns(12L);
		destination.SetupGet(x => x.Location).Returns(room.Object);
		var storyteller = CreateStoryteller(characters: [origin.Object, destination.Object], cells: [room.Object]);

		var result = storyteller.ExecuteFunctionCall("PathBetweenCharacters",
			"""{"OriginCharacterId":11,"DestinationCharacterId":12,"PathSearchFunction":"PathIgnoreDoors"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;
		var directions = payload.GetProperty("result").GetProperty("Directions");

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsTrue(payload.GetProperty("result").GetProperty("HasPath").GetBoolean());
		Assert.AreEqual(0, directions.GetArrayLength());
	}

	[TestMethod]
	public void ExecuteFunctionCall_RecentCharacterPlans_ReturnsRecentPlans()
	{
		var character = BuildCharacterWithPlans(
			id: 300L,
			name: "Alice Example",
			shortDescription: "a focused planner",
			shortPlan: "Secure dock contracts.",
			longPlan: "Expand guild influence.");
		var recentEffect = new RecentlyUpdatedPlan(character.Object);
		character.Setup(x => x.AffectedBy<RecentlyUpdatedPlan>()).Returns(true);
		character.Setup(x => x.EffectsOfType<RecentlyUpdatedPlan>(It.IsAny<Predicate<RecentlyUpdatedPlan>>()))
			.Returns([recentEffect]);
		character.Setup(x => x.ScheduledDuration(It.IsAny<IEffect>())).Returns(TimeSpan.FromDays(3));
		var storyteller = CreateStoryteller(characters: [character.Object]);

		var result = storyteller.ExecuteFunctionCall("RecentCharacterPlans", "{}", includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;
		var plans = payload.GetProperty("result").GetProperty("Plans");

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.AreEqual(1, plans.GetArrayLength());
		Assert.AreEqual(300L, plans[0].GetProperty("Id").GetInt64());
		Assert.AreEqual("Secure dock contracts.", plans[0].GetProperty("ShortTermPlan").GetString());
		Assert.AreEqual("Expand guild influence.", plans[0].GetProperty("LongTermPlan").GetString());
	}

	[TestMethod]
	public void ExecuteFunctionCall_CharacterPlans_ReturnsSpecificCharacterPlans()
	{
		var character = BuildCharacterWithPlans(
			id: 301L,
			name: "Basil Planner",
			shortDescription: "a thoughtful strategist",
			shortPlan: "Recruit allies.",
			longPlan: "Stabilize trade routes.");
		var recentEffect = new RecentlyUpdatedPlan(character.Object);
		character.Setup(x => x.EffectsOfType<RecentlyUpdatedPlan>(It.IsAny<Predicate<RecentlyUpdatedPlan>>()))
			.Returns([recentEffect]);
		character.Setup(x => x.ScheduledDuration(It.IsAny<IEffect>())).Returns(TimeSpan.FromDays(1));
		var storyteller = CreateStoryteller(characters: [character.Object]);

		var result = storyteller.ExecuteFunctionCall("CharacterPlans", """{"Id":301}""", includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;
		var details = payload.GetProperty("result");

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.AreEqual(301L, details.GetProperty("Id").GetInt64());
		Assert.AreEqual("Recruit allies.", details.GetProperty("ShortTermPlan").GetString());
		Assert.AreEqual("Stabilize trade routes.", details.GetProperty("LongTermPlan").GetString());
		Assert.IsTrue(details.GetProperty("RecentlyUpdated").GetBoolean());
	}


	[TestMethod]
	public void ExecuteFunctionCall_CurrentDateTime_MultipleContexts_ReturnsError()
	{
		var clock = new Mock<IClock>();
		clock.SetupGet(x => x.Id).Returns(51L);
		var timezoneOne = new Mock<IMudTimeZone>();
		timezoneOne.SetupGet(x => x.Id).Returns(1L);
		var timezoneTwo = new Mock<IMudTimeZone>();
		timezoneTwo.SetupGet(x => x.Id).Returns(2L);
		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.Id).Returns(41L);
		calendar.SetupGet(x => x.FeedClock).Returns(clock.Object);

		var roomOne = new Mock<ICell>();
		roomOne.SetupGet(x => x.Id).Returns(401L);
		roomOne.SetupGet(x => x.Calendars).Returns([calendar.Object]);
		roomOne.Setup(x => x.TimeZone(clock.Object)).Returns(timezoneOne.Object);
		var roomTwo = new Mock<ICell>();
		roomTwo.SetupGet(x => x.Id).Returns(402L);
		roomTwo.SetupGet(x => x.Calendars).Returns([calendar.Object]);
		roomTwo.Setup(x => x.TimeZone(clock.Object)).Returns(timezoneTwo.Object);

		var storyteller = CreateStoryteller(cells: [roomOne.Object, roomTwo.Object]);
		var result = storyteller.ExecuteFunctionCall("CurrentDateTime", "{}", includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
		StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty,
			"Multiple calendar/clock/timezone contexts are in use");
	}

	[TestMethod]
	public void ExecuteFunctionCall_DateTimeForTarget_Character_ReturnsDateTimePayload()
	{
		var clock = new Mock<IClock>();
		clock.SetupGet(x => x.Id).Returns(52L);
		clock.SetupGet(x => x.Name).Returns("City Clock");
		clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Long)).Returns("High Sun");
		clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Short)).Returns("Noon");
		clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Vague)).Returns("Around Noon");

		var timezone = new Mock<IMudTimeZone>();
		timezone.SetupGet(x => x.Id).Returns(5L);
		timezone.SetupGet(x => x.Name).Returns("City Standard");

		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.Id).Returns(42L);
		calendar.SetupGet(x => x.FullName).Returns("Civic Calendar");
		calendar.SetupGet(x => x.FeedClock).Returns(clock.Object);
		calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Long)).Returns("3rd Rainfall 1200");
		calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Short)).Returns("3-RF-1200");

		var room = new Mock<ICell>();
		room.SetupGet(x => x.Id).Returns(403L);
		room.SetupGet(x => x.Calendars).Returns([calendar.Object]);
		room.Setup(x => x.TimeZone(clock.Object)).Returns(timezone.Object);
		room.Setup(x => x.Date(calendar.Object)).Returns((MudDate)null!);
		room.Setup(x => x.Time(clock.Object)).Returns((MudTime)null!);
		room.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
			.Returns("Market Square");

		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(NameStyle.FullName)).Returns("Alyx Ward");

		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(501L);
		character.SetupGet(x => x.Location).Returns(room.Object);
		character.SetupGet(x => x.PersonalName).Returns(personalName.Object);

		var storyteller = CreateStoryteller(characters: [character.Object], cells: [room.Object]);
		var result = storyteller.ExecuteFunctionCall("DateTimeForTarget", """{"CharacterId":501}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;
		var resultPayload = payload.GetProperty("result");

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.AreEqual("High Sun on 3rd Rainfall 1200", resultPayload.GetProperty("DateTime").GetString());
		Assert.AreEqual(501L, resultPayload.GetProperty("CharacterId").GetInt64());
		Assert.AreEqual("Market Square", resultPayload.GetProperty("RoomName").GetString());
	}

	[TestMethod]
	public void ExecuteFunctionCall_CalendarDefinition_ReturnsCalendarMetadata()
	{
		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.Id).Returns(77L);
		calendar.SetupGet(x => x.Name).Returns("Harvest Calendar");
		calendar.SetupGet(x => x.FullName).Returns("Harvest Calendar");
		calendar.SetupGet(x => x.ShortName).Returns("Harvest");
		calendar.SetupGet(x => x.Alias).Returns("harvest");
		calendar.SetupGet(x => x.Description).Returns("Tracks agrarian seasons.");
		calendar.SetupGet(x => x.AncientEraLongString).Returns("Before Founding");
		calendar.SetupGet(x => x.ModernEraLongString).Returns("After Founding");
		calendar.SetupGet(x => x.Weekdays).Returns(["Primeday", "Dualday"]);
		calendar.SetupGet(x => x.Months).Returns([]);
		calendar.SetupGet(x => x.Intercalaries).Returns([]);
		calendar.SetupGet(x => x.Names).Returns(["Harvest Calendar", "harvest"]);

		var storyteller = CreateStoryteller(calendars: [calendar.Object]);
		var result = storyteller.ExecuteFunctionCall("CalendarDefinition", """{"Id":"harvest"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;
		var resultPayload = payload.GetProperty("result");

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.AreEqual(77L, resultPayload.GetProperty("Id").GetInt64());
		Assert.AreEqual("Harvest Calendar", resultPayload.GetProperty("Name").GetString());
		Assert.AreEqual("Tracks agrarian seasons.", resultPayload.GetProperty("Description").GetString());
	}

	[TestMethod]
	public void PassHeartbeatEventToAIStorytellerForTesting_MissingApiKey_DoesNotExecuteStatusProg()
	{
		var (runtimeGame, disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
		try
		{
			var heartbeatProg = new Mock<IFutureProg>();
			heartbeatProg.SetupGet(x => x.Id).Returns(123L);
			heartbeatProg.Setup(x => x.ExecuteString(It.IsAny<object[]>()))
				.Throws(new AssertFailedException("Heartbeat status prog should not execute without API key."));

			var gameworld = CreateGameworld(
				new[] { heartbeatProg.Object },
				Array.Empty<ICharacter>());
			var model = CreateModel();
			model.HeartbeatStatus5mProgId = 123L;
			var storyteller = new AIStoryteller(model, gameworld.Object);

			storyteller.PassHeartbeatEventToAIStorytellerForTesting("5m");

			heartbeatProg.Verify(x => x.ExecuteString(It.IsAny<object[]>()), Times.Never);
		}
		finally
		{
			if (disposeRuntimeGame)
			{
				runtimeGame.Dispose();
			}
		}
	}

	[TestMethod]
	public void CellOnRoomEchoForTesting_MissingApiKey_DoesNotParseEcho()
	{
		var (runtimeGame, disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
		try
		{
			var storyteller = CreateStoryteller();
			var cell = new Mock<ICell>();
			var emote = new Mock<IEmoteOutput>();
			emote.Setup(x => x.ParseFor(It.IsAny<IPerceiver>()))
				.Throws(new AssertFailedException("Echo parse should not execute without API key."));

			storyteller.CellOnRoomEchoForTesting(cell.Object, emote.Object);

			emote.Verify(x => x.ParseFor(It.IsAny<IPerceiver>()), Times.Never);
		}
		finally
		{
			if (disposeRuntimeGame)
			{
				runtimeGame.Dispose();
			}
		}
	}

	[TestMethod]
	public void InvokeDirectAttention_MissingApiKey_ReturnsFalse()
	{
		var (runtimeGame, disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
		try
		{
			var storyteller = CreateStoryteller();
			var result = storyteller.InvokeDirectAttention("Direct invocation from FutureProg.");
			Assert.IsFalse(result);
		}
		finally
		{
			if (disposeRuntimeGame)
			{
				runtimeGame.Dispose();
			}
		}
	}

	[TestMethod]
	public void TryParseAttentionClassifierOutput_ValidInterestedJson_ParsesReason()
	{
		var parsed = AIStoryteller.TryParseAttentionClassifierOutput(
			"""{"Decision":"interested","Reason":"world-impacting event"}""",
			out var interested,
			out var reason,
			out var error);

		Assert.IsTrue(parsed);
		Assert.IsTrue(interested);
		Assert.AreEqual("world-impacting event", reason);
		Assert.AreEqual(string.Empty, error);
	}

	[TestMethod]
	public void TryParseAttentionClassifierOutput_InvalidText_ReturnsContractError()
	{
		var parsed = AIStoryteller.TryParseAttentionClassifierOutput(
			"interested because this matters",
			out var interested,
			out var reason,
			out var error);

		Assert.IsFalse(parsed);
		Assert.IsFalse(interested);
		Assert.AreEqual(string.Empty, reason);
		StringAssert.Contains(error, "not valid JSON");
	}

	[TestMethod]
	public void TryInterpretAttentionClassifierOutputForTesting_InvalidText_LogsError()
	{
		var storyteller = CreateStoryteller();
		string? loggedMessage = null;
		storyteller.ErrorLoggerOverride = message => loggedMessage = message;

		var parsed = storyteller.TryInterpretAttentionClassifierOutputForTesting(
			"interested because this matters",
			out var interested,
			out var reason);

		Assert.IsFalse(parsed);
		Assert.IsFalse(interested);
		Assert.AreEqual(string.Empty, reason);
		Assert.IsFalse(string.IsNullOrWhiteSpace(loggedMessage));
		StringAssert.Contains(loggedMessage, "Attention classifier contract violation");
	}

	private static AIStoryteller CreateStoryteller(IEnumerable<IFutureProg>? progs = null,
		IEnumerable<ICharacter>? characters = null, IEnumerable<ICell>? cells = null,
		IEnumerable<ICalendar>? calendars = null)
	{
		var gameworld = CreateGameworld(progs ?? Array.Empty<IFutureProg>(), characters ?? Array.Empty<ICharacter>(),
			cells ?? Array.Empty<ICell>(), calendars ?? Array.Empty<ICalendar>());
		return new AIStoryteller(CreateModel(), gameworld.Object);
	}

	private static ModelStoryteller CreateModel()
	{
		return new ModelStoryteller
		{
			Id = 1L,
			Name = "Test Storyteller",
			Description = "Test",
			Model = "gpt-5",
			SystemPrompt = "System prompt",
			AttentionAgentPrompt = "Attention prompt",
			SurveillanceStrategyDefinition = string.Empty,
			ReasoningEffort = "2",
			CustomToolCallsDefinition = "<ToolCalls />",
			SubscribeToRoomEvents = false,
			SubscribeTo5mHeartbeat = false,
			SubscribeTo10mHeartbeat = false,
			SubscribeTo30mHeartbeat = false,
			SubscribeToHourHeartbeat = false,
			IsPaused = false
		};
	}

	private static Mock<IFuturemud> CreateGameworld(IEnumerable<IFutureProg> progs, IEnumerable<ICharacter> characters,
		IEnumerable<ICell>? cells = null, IEnumerable<ICalendar>? calendars = null)
	{
		var progList = progs.ToList();
		var characterList = characters.ToList();
		var cellList = (cells ?? Array.Empty<ICell>()).ToList();
		var calendarList = (calendars ?? Array.Empty<ICalendar>()).ToList();

		var progRepo = BuildRepository(progList);
		var characterRepo = BuildRepository(characterList);
		var cellRepo = BuildRepository(cellList);
		var calendarRepo = BuildRepository(calendarList);
		var saveManager = new Mock<ISaveManager>();

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progRepo.Object);
		gameworld.SetupGet(x => x.Characters).Returns(characterRepo.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cellRepo.Object);
		gameworld.SetupGet(x => x.Calendars).Returns(calendarRepo.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			.Returns((long id, bool _) => characterList.FirstOrDefault(x => x.Id == id));
		return gameworld;
	}

	private static Mock<ICharacter> BuildCharacterWithPlans(long id, string name, string shortDescription,
		string shortPlan, string longPlan)
	{
		var character = new Mock<ICharacter>();
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(NameStyle.FullName)).Returns(name);

		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.PersonalName).Returns(personalName.Object);
		character.SetupGet(x => x.ShortTermPlan).Returns(shortPlan);
		character.SetupGet(x => x.LongTermPlan).Returns(longPlan);
		character.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
			.Returns(shortDescription);
		return character;
	}

	private static (Futuremud RuntimeGame, bool DisposeRuntimeGame) EnsureRuntimeGameWithMissingApiKey()
	{
		var existing = Futuremud.Games.OfType<Futuremud>().FirstOrDefault();
		if (existing is not null)
		{
			existing.UpdateStaticConfiguration("GPT_Secret_Key", string.Empty);
			return (existing, false);
		}

		var created = new Futuremud(null);
		created.UpdateStaticConfiguration("GPT_Secret_Key", string.Empty);
		return (created, true);
	}

	private static Mock<IUneditableAll<T>> BuildRepository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var byId = list.ToDictionary(x => x.Id, x => x);
		var repo = new Mock<IUneditableAll<T>>();
		repo.Setup(x => x.Get(It.IsAny<long>()))
			.Returns((long id) => byId.TryGetValue(id, out var value) ? value : null!);
		repo.Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		repo.SetupGet(x => x.Count).Returns(list.Count);
		return repo;
	}
}

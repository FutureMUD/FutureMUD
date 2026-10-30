using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EventTypeMetadataTests
{
    [TestMethod]
    public void EventTypes_AllHaveEventInfoMetadata()
    {
        EventType[] missingMetadata = Enum.GetValues<EventType>()
                                          .Where(x => x.GetAttribute<EventInfoAttribute>() is null)
                                          .ToArray();

        Assert.IsFalse(missingMetadata.Any(), $"Missing EventInfoAttribute: {string.Join(", ", missingMetadata)}");
    }

    [TestMethod]
    public void EventInfoMetadata_ParameterNamesAndProgTypesStayAligned()
    {
        foreach (EventType eventType in Enum.GetValues<EventType>())
        {
            EventInfoAttribute info = eventType.GetAttribute<EventInfoAttribute>();
            Assert.IsFalse(string.IsNullOrWhiteSpace(info.Description), $"{eventType} has a blank description.");

            var parameters = info.Parameters.ToArray();
            var progTypes = info.ProgTypes.ToArray();

            Assert.AreEqual(parameters.Length, progTypes.Length,
                $"{eventType} has {parameters.Length} display parameters but {progTypes.Length} FutureProg types.");

            Assert.IsFalse(parameters.Any(x => string.IsNullOrWhiteSpace(x.type)),
                $"{eventType} has a blank parameter type.");
            Assert.IsFalse(parameters.Any(x => string.IsNullOrWhiteSpace(x.name)),
                $"{eventType} has a blank parameter name.");
        }
    }

    [TestMethod]
    public void OfferingEventTypes_DefineExpectedHookPayloads()
    {
        AssertEventMetadata(EventType.OfferingReceived,
            new[] { "item", "character", "item" },
            new[] { "focus", "actor", "offering" },
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Item });
        AssertEventMetadata(EventType.OfferingReceivedWitness,
            new[] { "item", "character", "item", "perceivable" },
            new[] { "focus", "actor", "offering", "witness" },
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Perceivable });
        AssertEventMetadata(EventType.OfferingBurned,
            new[] { "item", "character", "item" },
            new[] { "focus", "actor", "offering" },
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Item });
        AssertEventMetadata(EventType.OfferingBurnedWitness,
            new[] { "item", "character", "item", "perceivable" },
            new[] { "focus", "actor", "offering", "witness" },
            new[] { ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Perceivable });
    }

	[TestMethod]
	public void RouteAndVehicleJourneyEventTypes_AreAppendOnlyAndDefineExpectedHookPayloads()
	{
		Assert.AreEqual(130, (int)EventType.RouteMovementBegin);
		Assert.AreEqual(131, (int)EventType.RouteMovementProgress);
		Assert.AreEqual(132, (int)EventType.RouteMovementComplete);
		Assert.AreEqual(133, (int)EventType.RouteMovementCancelled);
		Assert.AreEqual(134, (int)EventType.RoutePositionChanged);
		Assert.AreEqual(135, (int)EventType.VehicleJourneyDeparted);
		Assert.AreEqual(136, (int)EventType.VehicleJourneyArrived);
		Assert.AreEqual(137, (int)EventType.VehicleJourneyDelayChanged);
		Assert.AreEqual(138, (int)EventType.VehicleJourneyCancelled);
		Assert.AreEqual(139, (int)EventType.VehicleJourneyFaulted);

		AssertEventMetadata(EventType.RouteMovementBegin,
			["perceivable", "location", "number", "number", "text", "number", "text"],
			["mover", "routecell", "originmetres", "destinationmetres", "direction", "speedmetrespersecond", "operationid"],
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Location, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Text]);
		AssertEventMetadata(EventType.RouteMovementProgress,
			["perceivable", "location", "number", "number", "number", "text", "number", "text"],
			["mover", "routecell", "previousmetres", "currentmetres", "destinationmetres", "direction",
				"speedmetrespersecond", "operationid"],
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Location, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Number,
				ProgVariableTypes.Text]);
		AssertEventMetadata(EventType.RouteMovementComplete,
			["perceivable", "location", "number", "number", "text", "text"],
			["mover", "routecell", "originmetres", "destinationmetres", "direction", "operationid"],
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Location, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text]);
		AssertEventMetadata(EventType.RouteMovementCancelled,
			["perceivable", "location", "number", "number", "number", "text", "text", "text"],
			["mover", "routecell", "originmetres", "currentmetres", "destinationmetres", "direction", "reason",
				"operationid"],
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Location, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text,
				ProgVariableTypes.Text]);
		AssertEventMetadata(EventType.RoutePositionChanged,
			["perceivable", "location", "number", "number", "text"],
			["mover", "routecell", "previousmetres", "currentmetres", "operationid"],
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Location, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.Text]);

		var stopEventTypes = new[] { EventType.VehicleJourneyDeparted, EventType.VehicleJourneyArrived };
		foreach (var eventType in stopEventTypes)
		{
			AssertEventMetadata(eventType,
				["item", "number", "number", "number", "number", "location", "number", "text"],
				["vehicle", "journeyid", "routeid", "serviceid", "vehicleid", "stopcell", "stoppositionmetres",
					"message"],
				[ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number,
					ProgVariableTypes.Number, ProgVariableTypes.Location, ProgVariableTypes.Number, ProgVariableTypes.Text]);
		}
		AssertEventMetadata(EventType.VehicleJourneyDelayChanged,
			["item", "number", "number", "number", "number", "timespan", "text"],
			["vehicle", "journeyid", "routeid", "serviceid", "vehicleid", "delay", "message"],
			[ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.TimeSpan, ProgVariableTypes.Text]);

		var terminalEventTypes = new[] { EventType.VehicleJourneyCancelled, EventType.VehicleJourneyFaulted };
		foreach (var eventType in terminalEventTypes)
		{
			AssertEventMetadata(eventType,
				["item", "number", "number", "number", "number", "text"],
				["vehicle", "journeyid", "routeid", "serviceid", "vehicleid", "reason"],
				[ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number,
					ProgVariableTypes.Number, ProgVariableTypes.Text]);
		}
	}

	[TestMethod]
	public void NoiseEmittedEventType_IsAppendOnlyAndDefinesExpectedHookPayload()
	{
		Assert.AreEqual(143, (int)EventType.NoiseEmitted);
		AssertEventMetadata(EventType.NoiseEmitted,
			["location", "perceivable", "number", "text", "text"],
			["origin", "source", "volume", "type", "echo"],
			[ProgVariableTypes.Location, ProgVariableTypes.Perceivable, ProgVariableTypes.Number,
				ProgVariableTypes.Text, ProgVariableTypes.Text]);
	}

	[TestMethod]
	public void SignedCommunicationEventTypes_UseFirstClassLanguagePayloads()
	{
		Assert.AreEqual(147, (int)EventType.CharacterSigns);
		Assert.AreEqual(148, (int)EventType.CharacterSignsWitness);
		Assert.AreEqual(149, (int)EventType.CharacterSignsDirect);
		Assert.AreEqual(150, (int)EventType.CharacterSignsDirectTarget);
		Assert.AreEqual(151, (int)EventType.CharacterSignsDirectWitness);

		AssertEventMetadata(EventType.CharacterSigns,
			["character", "signedlanguage", "signedvariety", "text", "number"],
			["signer", "language", "variety", "message", "outcome"],
			[ProgVariableTypes.Character, ProgVariableTypes.SignedLanguage, ProgVariableTypes.SignedVariety,
				ProgVariableTypes.Text, ProgVariableTypes.Number]);
		AssertEventMetadata(EventType.CharacterSignsWitness,
			["character", "perceivable", "signedlanguage", "signedvariety", "text", "number"],
			["signer", "witness", "language", "variety", "message", "outcome"],
			[ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.SignedLanguage,
				ProgVariableTypes.SignedVariety, ProgVariableTypes.Text, ProgVariableTypes.Number]);
		AssertEventMetadata(EventType.CharacterSignsDirect,
			["character", "perceivable", "signedlanguage", "signedvariety", "text", "number"],
			["signer", "target", "language", "variety", "message", "outcome"],
			[ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.SignedLanguage,
				ProgVariableTypes.SignedVariety, ProgVariableTypes.Text, ProgVariableTypes.Number]);
		AssertEventMetadata(EventType.CharacterSignsDirectTarget,
			["character", "perceivable", "signedlanguage", "signedvariety", "text", "number"],
			["signer", "target", "language", "variety", "message", "outcome"],
			[ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.SignedLanguage,
				ProgVariableTypes.SignedVariety, ProgVariableTypes.Text, ProgVariableTypes.Number]);
		AssertEventMetadata(EventType.CharacterSignsDirectWitness,
			["character", "perceivable", "perceivable", "signedlanguage", "signedvariety", "text", "number"],
			["signer", "target", "witness", "language", "variety", "message", "outcome"],
			[ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
				ProgVariableTypes.SignedLanguage, ProgVariableTypes.SignedVariety, ProgVariableTypes.Text,
				ProgVariableTypes.Number]);
	}

    private static void AssertEventMetadata(EventType eventType, string[] parameterTypes, string[] parameterNames,
        ProgVariableTypes[] progTypes)
    {
        EventInfoAttribute info = eventType.GetAttribute<EventInfoAttribute>();
        CollectionAssert.AreEqual(parameterTypes, info.Parameters.Select(x => x.type).ToArray());
        CollectionAssert.AreEqual(parameterNames, info.Parameters.Select(x => x.name).ToArray());
        CollectionAssert.AreEqual(progTypes, info.ProgTypes.ToArray());
    }

    [TestMethod]
    public void NeedsTransitionEvents_ExposeCapturedLocationAndWitness()
    {
        Assert.AreEqual(157, (int)EventType.CharacterHungry);
        Assert.AreEqual(158, (int)EventType.CharacterInRoomHungry);
        Assert.AreEqual(159, (int)EventType.CharacterStarving);
        Assert.AreEqual(160, (int)EventType.CharacterInRoomStarving);
        Assert.AreEqual(161, (int)EventType.CharacterThirsty);
        Assert.AreEqual(162, (int)EventType.CharacterInRoomThirsty);
        Assert.AreEqual(163, (int)EventType.CharacterParched);
        Assert.AreEqual(164, (int)EventType.CharacterInRoomParched);

        var selfTypes = new[] { ProgVariableTypes.Character, ProgVariableTypes.Location };
        var witnessTypes = new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Perceivable };
        foreach (var type in new[] { EventType.CharacterHungry, EventType.CharacterStarving, EventType.CharacterThirsty, EventType.CharacterParched })
        {
            var info = type.GetAttribute<EventInfoAttribute>();
            CollectionAssert.AreEqual(new[] { "character", "location" }, info.Parameters.Select(x => x.type).ToArray());
            CollectionAssert.AreEqual(new[] { "character", "location" }, info.Parameters.Select(x => x.name).ToArray());
            CollectionAssert.AreEqual(selfTypes, info.ProgTypes.ToArray());
        }

        foreach (var type in new[] { EventType.CharacterInRoomHungry, EventType.CharacterInRoomStarving, EventType.CharacterInRoomThirsty, EventType.CharacterInRoomParched })
        {
            var info = type.GetAttribute<EventInfoAttribute>();
            CollectionAssert.AreEqual(new[] { "character", "location", "perceivable" }, info.Parameters.Select(x => x.type).ToArray());
            CollectionAssert.AreEqual(new[] { "character", "location", "witness" }, info.Parameters.Select(x => x.name).ToArray());
            CollectionAssert.AreEqual(witnessTypes, info.ProgTypes.ToArray());
        }
    }
}

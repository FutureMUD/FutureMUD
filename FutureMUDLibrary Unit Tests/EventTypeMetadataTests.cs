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

    private static void AssertEventMetadata(EventType eventType, string[] parameterTypes, string[] parameterNames,
        ProgVariableTypes[] progTypes)
    {
        EventInfoAttribute info = eventType.GetAttribute<EventInfoAttribute>();
        CollectionAssert.AreEqual(parameterTypes, info.Parameters.Select(x => x.type).ToArray());
        CollectionAssert.AreEqual(parameterNames, info.Parameters.Select(x => x.name).ToArray());
        CollectionAssert.AreEqual(progTypes, info.ProgTypes.ToArray());
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Events;
using MudSharp.Framework;
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
}

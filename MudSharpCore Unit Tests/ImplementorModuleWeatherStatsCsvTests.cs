using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Commands.Modules;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ImplementorModuleWeatherStatsCsvTests
{
    [TestMethod]
    public void FormatWeatherStatisticsMetadataCsv_ValueContainsCommasAndQuotes_QuotesValueColumn()
    {
        string result = ImplementorModule.FormatWeatherStatisticsMetadataCsv(
            "Controller",
            "Planet (#1, \"WeatherController\")");

        Assert.AreEqual("# Controller,\"Planet (#1, \"\"WeatherController\"\")\"", result);
    }
}

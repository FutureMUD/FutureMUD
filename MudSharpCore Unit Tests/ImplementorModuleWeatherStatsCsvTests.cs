using System.IO;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Climate.Analysis;
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

        Assert.AreEqual("\"# Controller\",\"Planet (#1, \"\"WeatherController\"\")\"", result);
    }

	[DataTestMethod]
	[DataRow("=1+1")]
	[DataRow("+SUM(1,1)")]
	[DataRow("-10+20")]
	[DataRow("@SUM(1,1)")]
	[DataRow(" =1+1")]
	[DataRow("\t=1+1")]
	[DataRow("\r=1+1")]
	[DataRow("\n=1+1")]
	[DataRow("=WEBSERVICE(\"https://example.invalid\")")]
	public void WeatherCsv_FormulaNames_AreTextAfterCsvParsing(string name)
	{
		var metadata = Parse(ImplementorModule.FormatWeatherStatisticsMetadataCsv("WeatherEvents", name));
		Assert.AreEqual("# WeatherEvents", metadata[0]);
		Assert.AreEqual($"'{name}", metadata[1]);
		var row = Parse(ImplementorModule.FormatWeatherStatisticsRowCsv(new WeatherStatisticsCsvRow
		{
			Season = name,
			MetricType = name,
			Key = name,
			Hour = 3,
			Statistic = name,
			Value = -12.5,
			Unit = name,
			SampleCount = 10,
			SeasonMinutes = 20
		}));
		foreach (var index in new[] { 0, 1, 2, 4, 6 })
		{
			Assert.AreEqual($"'{name}", row[index]);
		}
		Assert.AreEqual("3", row[3]);
		Assert.AreEqual("-12.5", row[5]);
		Assert.AreEqual("10", row[7]);
		Assert.AreEqual("20", row[8]);
	}

	[TestMethod]
	public void WeatherCsv_OrdinaryText_RetainsItsValueAfterParsing()
	{
		const string value = "Winter, \"wet\" weather";
		Assert.AreEqual(value, Parse(ImplementorModule.FormatWeatherStatisticsMetadataCsv("Season", value))[1]);
	}

	private static string[] Parse(string csv)
	{
		using var parser = new TextFieldParser(new StringReader(csv));
		parser.SetDelimiters(",");
		parser.HasFieldsEnclosedInQuotes = true;
		parser.TrimWhiteSpace = false;
		return parser.ReadFields()!;
	}
}

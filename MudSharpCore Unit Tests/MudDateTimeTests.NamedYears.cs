#nullable enable

using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;

namespace MudSharp_Unit_Tests;

public partial class MudDateTimeTests
{
	[TestMethod]
	public void Calendar_NamedYearCycle_LoadSaveFormatAndLegacyReload()
	{
		var root = XElement.Parse(_testCalendar.SaveToXml().ToString());
		root.Add(new XElement("namedyearcycle", new XAttribute("name", "Test Age"), new XAttribute("epochyear", 34),
			new XElement("year", "First Light"), new XElement("year", "Long Night")));
		var calendar = new Calendar(root, _gameworld);
		calendar.FeedClock = _testClock;
		calendar.SetDate("27/jun/34");
		Assert.AreEqual("First Light|1|Test Age|1|34", calendar.DisplayDate("$yn|$yi|$yc|$ya|$yy"));
		Assert.AreEqual("Long Night|2|0", calendar.DisplayDate(calendar.GetDate("27/jun/33"), "$yn|$yi|$ya"));
		Assert.AreEqual("First Light|1|2", calendar.DisplayDate(calendar.GetDate("27/jun/36"), "$yn|$yi|$ya"));

		var reloaded = new Calendar(XElement.Parse(calendar.SaveToXml().ToString()), _gameworld);
		Assert.AreEqual("Long Night", reloaded.YearCycle!.GetYear(35).Name);
		Assert.AreEqual(34, reloaded.YearCycle.EpochYear);
		calendar.LoadFromXml(XElement.Parse(_testCalendar.SaveToXml().ToString()));
		Assert.IsNull(calendar.YearCycle);
		Assert.AreEqual("34", calendar.DisplayDate(calendar.GetDate("27/jun/34"), "$yn$yi$yc$ya$yy"));
		Assert.IsNull(calendar.SaveToXml().Element("namedyearcycle"));
	}

	[TestMethod]
	[DataRow("<namedyearcycle name='Age' epochyear='1' />")]
	[DataRow("<namedyearcycle name='Age' epochyear='not-a-year'><year>First</year></namedyearcycle>")]
	[DataRow("<namedyearcycle name='Age' epochyear='1'><year>First</year><year>first</year></namedyearcycle>")]
	public void Calendar_NamedYearCycle_RejectsMalformedDefinitions(string xml)
	{
		var root = XElement.Parse(_testCalendar.SaveToXml().ToString());
		root.Add(XElement.Parse(xml));
		Assert.ThrowsExactly<XmlException>(() => new Calendar(root, _gameworld));
	}

	[TestMethod]
	public void CalendarBuilder_NamedYearCycle_CanCreateEditAndClearWithoutChangingDates()
	{
		var actor = CreateBuilder();
		var calendar = new Calendar(XElement.Parse(_testCalendar.SaveToXml().ToString()), _gameworld);
		calendar.FeedClock = _testClock;
		calendar.SetDate("27/jun/34");
		bool Run(string command) => calendar.BuildingCommand(actor.Object, new StringStack("yearcycle " + command));
		Assert.IsTrue(Run("create 34 \"Test Age\" \"First Light\" \"Long Night\""));
		Assert.IsTrue(Run("add Third Year"));
		Assert.IsTrue(Run("set 2 Second Year"));
		Assert.IsTrue(Run("remove 3"));
		Assert.IsTrue(Run("epoch 33"));
		Assert.IsTrue(Run("name Great Age"));
		Assert.AreEqual("Second Year|2|Great Age|1", calendar.DisplayDate("$yn|$yi|$yc|$ya"));
		var saved = calendar.SaveToXml().ToString();
		Assert.IsFalse(Run("set 2 First Light"));
		Assert.IsFalse(Run("remove 9"));
		Assert.IsFalse(Run("epoch 35 trailing"));
		Assert.AreEqual(saved, calendar.SaveToXml().ToString());
		Assert.IsTrue(Run("show"));
		Assert.IsTrue(Run("clear"));
		Assert.IsNull(calendar.YearCycle);
		Assert.AreEqual("27/jung/34", calendar.CurrentDate.GetDateString());
	}
}

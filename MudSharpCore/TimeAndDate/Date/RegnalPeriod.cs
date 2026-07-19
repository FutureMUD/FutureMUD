#nullable enable


namespace MudSharp.TimeAndDate.Date;

public class RegnalPeriod : IRegnalPeriod
{
	public RegnalPeriod(string key, string shortName, string fullName, MudDate startDate, MudDate? endDate)
	{
		Key = key;
		ShortName = shortName;
		FullName = fullName;
		StartDate = new MudDate(startDate);
		EndDate = endDate is null ? null : new MudDate(endDate);
	}

	public RegnalPeriod(ICalendar calendar, XElement element)
	{
		Key = element.Attribute("key")?.Value ?? element.Element("key")?.Value ?? string.Empty;
		ShortName = element.Attribute("short")?.Value ?? element.Element("short")?.Value ?? string.Empty;
		FullName = element.Attribute("full")?.Value ?? element.Element("full")?.Value ?? string.Empty;

		var startText = element.Attribute("start")?.Value ?? element.Element("start")?.Value;
		if (string.IsNullOrWhiteSpace(startText))
		{
			throw new MUDDateException("Regnal period did not have a start date.");
		}

		StartDate = calendar.GetDate(startText);

		var endText = element.Attribute("end")?.Value ?? element.Element("end")?.Value;
		EndDate = string.IsNullOrWhiteSpace(endText) || endText.EqualTo("open")
			? null
			: calendar.GetDate(endText);
	}

	public string Key { get; }

	public string ShortName { get; set; }

	public string FullName { get; set; }

	public MudDate StartDate { get; set; }

	public MudDate? EndDate { get; set; }

	public bool IsOpenEnded => EndDate is null;

	public bool Contains(MudDate date)
	{
		return date >= StartDate && (EndDate is null || date <= EndDate);
	}

	public RegnalPeriod Clone()
	{
		return new RegnalPeriod(Key, ShortName, FullName, StartDate, EndDate);
	}

	public XElement SaveToXml()
	{
		var element = new XElement("regnalperiod",
			new XAttribute("key", Key),
			new XAttribute("short", ShortName),
			new XAttribute("full", FullName),
			new XAttribute("start", StartDate.GetDateString()));
		if (EndDate is not null)
		{
			element.Add(new XAttribute("end", EndDate.GetDateString()));
		}

		return element;
	}
}

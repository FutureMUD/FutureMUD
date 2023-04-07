using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine.Light;

public class LightModel : ILightModel
{
	private readonly RankedRange<string> _descriptions = new();
	private readonly RankedRange<Difficulty> _difficulties = new();
	private readonly RankedRange<string> _roomAddendums = new();

	private LightModel(XElement definition)
	{
		var element = definition.Element("SightDifficulties");
		if (element == null)
		{
			throw new NotSupportedException("There was no SightDifficulties element in the LightModel definition.");
		}

		foreach (var sub in element.Elements("SightDifficulty"))
		{
			_difficulties.Add((Difficulty)(sub.Attribute("Difficulty").Value.GetIntFromOrdinal() ?? 0),
				double.Parse(sub.Attribute("Lower").Value), double.Parse(sub.Attribute("Upper").Value));
		}

		element = definition.Element("Descriptions");
		if (element == null)
		{
			throw new NotSupportedException("There was no Descriptions element in the LightModel definition.");
		}

		foreach (var sub in element.Elements("Description"))
		{
			_descriptions.Add(sub.Value, double.Parse(sub.Attribute("Lower").Value),
				double.Parse(sub.Attribute("Upper").Value));
		}

		element = definition.Element("RoomDescriptions");
		if (element != null)
		{
			foreach (var sub in element.Elements("Description"))
			{
				_roomAddendums.Add(sub.Value.SubstituteANSIColour(), double.Parse(sub.Attribute("Lower").Value),
					double.Parse(sub.Attribute("Upper").Value));
			}
		}
	}

	public Difficulty GetSightDifficulty(double effectiveIllumination)
	{
		return _difficulties.Find(effectiveIllumination);
	}

	public string GetIlluminationDescription(double absoluteIllumination)
	{
		return _descriptions.Find(absoluteIllumination);
	}

	public string GetIlluminationRoomDescription(double absoluteIllumination)
	{
		return _roomAddendums.Find(absoluteIllumination);
	}

	public double GetMinimumIlluminationForDescription(string description)
	{
		return _descriptions.Ranges.FirstOrDefault(x => x.Value.EqualTo(description))?.LowerBound ?? 0.0;
	}

	public IEnumerable<string> LightDescriptions =>
		_descriptions.Ranges.OrderBy(x => x.LowerBound).Select(x => x.Value);

	public static ILightModel LoadLightModel(IFuturemudLoader game)
	{
		var stringValue = game.GetStaticConfiguration("LightModel");
		return new LightModel(XElement.Parse(stringValue));
	}
}
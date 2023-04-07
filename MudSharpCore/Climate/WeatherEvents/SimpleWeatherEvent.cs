using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Framework;

namespace MudSharp.Climate.WeatherEvents;

public class SimpleWeatherEvent : WeatherEventBase
{
	public SimpleWeatherEvent(Models.WeatherEvent weather, IFuturemud gameworld) : base(weather, gameworld)
	{
		var definition = XElement.Parse(weather.AdditionalInfo);
		foreach (var element in definition.Element("Echoes")?.Elements("Echo") ?? Enumerable.Empty<XElement>())
		{
			_randomEchoes.Add((element.Value, double.Parse(element.Attribute("chance")?.Value ?? "1.0")));
		}

		var transition = definition.Element("Transitions");
		_defaultTransitionEcho = transition.Element("Default").Value;
		foreach (var element in transition.Elements("Transition"))
		{
			_transitionEchoOverrides[long.Parse(element.Attribute("other").Value)] = element.Value;
		}
	}

	private readonly List<(string Echo, double Chance)> _randomEchoes = new();
	private string _defaultTransitionEcho;
	private readonly Dictionary<long, string> _transitionEchoOverrides = new();

	public override string RandomFlavourEcho()
	{
		return _randomEchoes.GetWeightedRandom(x => x.Chance).Echo?.SubstituteANSIColour();
	}

	public override string DescribeTransitionTo([CanBeNull] IWeatherEvent oldEvent)
	{
		if (oldEvent == null)
		{
			return _defaultTransitionEcho;
		}

		if (_transitionEchoOverrides.ContainsKey(oldEvent.Id))
		{
			return _transitionEchoOverrides[oldEvent.Id];
		}

		if (oldEvent.CountsAs != null && _transitionEchoOverrides.ContainsKey(oldEvent.CountsAs.Id))
		{
			return _transitionEchoOverrides[oldEvent.CountsAs.Id];
		}

		return _defaultTransitionEcho;
	}
}
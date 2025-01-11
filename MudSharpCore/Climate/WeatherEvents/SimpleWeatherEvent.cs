using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Units;

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
	
	protected virtual XElement SaveToXml()
	{
		return new XElement("Info",
			new XElement("Echoes",
				from item in _randomEchoes
				select new XElement("Echo",
					new XAttribute("chance", item.Chance),
					new XCData(item.Echo)
				)
			),
			new XElement("Transitions",
				new XElement("Default", new XCData(_defaultTransitionEcho)),
				from item in _transitionEchoOverrides
				select new XElement("Transition",
					new XAttribute("other", item.Key),
					new XCData(item.Value)
				)
			)
		);
	}

	protected virtual void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.WeatherEvent
			{
				WeatherEventType = "simple",
				Name = Name,
				CountsAsId = CountsAs?.Id,
				LightLevelMultiplier = LightLevelMultiplier,
				ObscuresViewOfSky = ObscuresViewOfSky,
				PermittedAtAfternoon = PermittedTimesOfDay.Contains(TimeOfDay.Afternoon),
				PermittedAtDawn = PermittedTimesOfDay.Contains(TimeOfDay.Dawn),
				PermittedAtDusk = PermittedTimesOfDay.Contains(TimeOfDay.Dusk),
				PermittedAtMorning = PermittedTimesOfDay.Contains(TimeOfDay.Morning),
				PermittedAtNight = PermittedTimesOfDay.Contains(TimeOfDay.Night),
				Precipitation = (int)Precipitation,
				Wind = (int)Wind,
				TemperatureEffect = TemperatureEffect,
				PrecipitationTemperatureEffect = PrecipitationTemperatureEffect,
				WindTemperatureEffect = WindTemperatureEffect,
				WeatherDescription = WeatherDescription,
				WeatherRoomAddendum = WeatherRoomAddendum,
				AdditionalInfo = SaveToXml().ToString()
			};
			FMDB.Context.WeatherEvents.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public SimpleWeatherEvent(IFuturemud gameworld, string name) : base(gameworld, name)
	{
		Precipitation = PrecipitationLevel.Parched;
		Wind = WindLevel.None;
		WeatherDescription = "An undescribed weather event";
		WeatherRoomAddendum = "";
		TemperatureEffect = 0.0;
		PrecipitationTemperatureEffect = 0.0;
		WindTemperatureEffect = 0.0;
		LightLevelMultiplier = 1.0;
		ObscuresViewOfSky = false;
		PermittedTimesOfDay = [TimeOfDay.Afternoon, TimeOfDay.Dawn, TimeOfDay.Dusk, TimeOfDay.Morning, TimeOfDay.Night];
		DoDatabaseInsert();
	}

	private SimpleWeatherEvent(SimpleWeatherEvent rhs, string name) : base(rhs.Gameworld, name)
	{
		Precipitation = rhs.Precipitation;
		Wind = rhs.Wind;
		WeatherDescription = rhs.WeatherDescription;
		WeatherRoomAddendum = rhs.WeatherRoomAddendum;
		TemperatureEffect = rhs.TemperatureEffect;
		PrecipitationTemperatureEffect = rhs.PrecipitationTemperatureEffect;
		WindTemperatureEffect = rhs.WindTemperatureEffect;
		LightLevelMultiplier = rhs.LightLevelMultiplier;
		ObscuresViewOfSky = rhs.ObscuresViewOfSky;
		PermittedTimesOfDay = rhs.PermittedTimesOfDay.ToList();
		_defaultTransitionEcho = rhs._defaultTransitionEcho;
		_randomEchoes.AddRange(rhs._randomEchoes);
		foreach (var item in rhs._transitionEchoOverrides)
		{
			_transitionEchoOverrides[item.Key] = item.Value;
		}
		DoDatabaseInsert();
	}

	#region Overrides of WeatherEventBase

	/// <inheritdoc />
	public override IWeatherEvent Clone(string name)
	{
		return new SimpleWeatherEvent(this, name);
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Simple Weather Event #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Counts As: {CountsAs?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Description: {WeatherDescription.ColourCommand()}");
		sb.AppendLine($"Room Addendum: {WeatherRoomAddendum?.SubstituteANSIColour()}");
		sb.AppendLine($"Precipitation: {Precipitation.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Wind: {Precipitation.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Temperature Effect: {Gameworld.UnitManager.DescribeExact(TemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine($"Wind Temperature Effect: {Gameworld.UnitManager.DescribeExact(WindTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine($"Precipitation Temperature Effect: {Gameworld.UnitManager.DescribeExact(PrecipitationTemperatureEffect, UnitType.TemperatureDelta, actor).ColourValue()}");
		sb.AppendLine($"Obscure Sky: {ObscuresViewOfSky.ToColouredString()}");
		sb.AppendLine($"Light Level Multiplier: {LightLevelMultiplier.ToStringP2Colour(actor)}");
		sb.AppendLine($"Permitted Times: {PermittedTimesOfDay.ListToColouredString()}");
		sb.AppendLine($"Default Transition Echo: {_defaultTransitionEcho?.SubstituteANSIColour()}");
		sb.AppendLine();
		sb.AppendLine("Special Transition Echoes:");
		sb.AppendLine();
		if (_transitionEchoOverrides.Count > 0)
		{
			foreach (var item in _transitionEchoOverrides)
			{
				var we = Gameworld.WeatherEvents.Get(item.Key);
				if (we is null)
				{
					continue;
				}
				sb.AppendLine($"\t{we.Name.ColourName()} (#{we.Id.ToStringN0(actor)}): {item.Value.SubstituteANSIColour()}");
			}
		}
		else
		{
			sb.AppendLine("\tNone");
		}
		sb.AppendLine();
		sb.AppendLine("Random Flavour Echoes:");
		sb.AppendLine();
		var total = _randomEchoes.Sum(x => x.Chance);
		if (total > 0.0)
		{
			foreach (var echo in _randomEchoes)
			{
				sb.AppendLine($"\t{echo.Chance.ToStringN2Colour(actor)} ({(echo.Chance/total).ToStringP2Colour(actor)}): {echo.Echo.SubstituteANSIColour()}");
			}
		}
		return sb.ToString();
	}

	#endregion

	protected readonly List<(string Echo, double Chance)> _randomEchoes = new();
	protected string _defaultTransitionEcho;
	protected readonly Dictionary<long, string> _transitionEchoOverrides = new();

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

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.WeatherEvents.Find(Id);
		dbitem.Name = Name;
		dbitem.CountsAsId = CountsAs?.Id;
		dbitem.LightLevelMultiplier = LightLevelMultiplier;
		dbitem.ObscuresViewOfSky = ObscuresViewOfSky;
		dbitem.PermittedAtAfternoon = PermittedTimesOfDay.Contains(TimeOfDay.Afternoon);
		dbitem.PermittedAtDawn = PermittedTimesOfDay.Contains(TimeOfDay.Dawn);
		dbitem.PermittedAtDusk = PermittedTimesOfDay.Contains(TimeOfDay.Dusk);
		dbitem.PermittedAtMorning = PermittedTimesOfDay.Contains(TimeOfDay.Morning);
		dbitem.PermittedAtNight = PermittedTimesOfDay.Contains(TimeOfDay.Night);
		dbitem.Precipitation = (int)Precipitation;
		dbitem.Wind = (int)Wind;
		dbitem.TemperatureEffect = TemperatureEffect;
		dbitem.PrecipitationTemperatureEffect = PrecipitationTemperatureEffect;
		dbitem.WindTemperatureEffect = WindTemperatureEffect;
		dbitem.WeatherDescription = WeatherDescription;
		dbitem.WeatherRoomAddendum = WeatherRoomAddendum;
		dbitem.AdditionalInfo = SaveToXml().ToString();
		Changed = false;
	}

	#endregion
}
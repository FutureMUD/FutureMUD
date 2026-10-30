#nullable enable

using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete;

public sealed class RoomWeatherEffect : Effect, IRoomWeatherEffect
{
	public RoomWeatherEffect(IRoom owner, IWeatherEvent weather) : base(owner)
	{
		WeatherEvent = weather;
	}

	private RoomWeatherEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect")!;
		WeatherEvent = Gameworld.WeatherEvents.Get((long)xml.Element("WeatherEvent")!)
			?? throw new ApplicationException("A saved room weather effect refers to missing weather.");
	}

	public IWeatherEvent WeatherEvent { get; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "RoomWeather";
	public override string Describe(IPerceiver voyeur) => $"Local weather: {WeatherEvent.Name.ColourName()}";
	protected override XElement SaveDefinition() => new("Effect", new XElement("WeatherEvent", WeatherEvent.Id));
	public static void InitialiseEffectType() => RegisterFactory("RoomWeather", (xml, owner) => new RoomWeatherEffect(xml, owner));
}

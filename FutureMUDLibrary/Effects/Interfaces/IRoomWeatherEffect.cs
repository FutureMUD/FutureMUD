#nullable enable

using MudSharp.Climate;

namespace MudSharp.Effects.Interfaces;

/// <summary>Local weather supplied by a room effect, without changing its regional controller.</summary>
public interface IRoomWeatherEffect : IEffect
{
	IWeatherEvent WeatherEvent { get; }
}

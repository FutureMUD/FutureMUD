using System.Xml.Linq;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellWeatherFreezeEffect : MagicSpellEffectBase
{
	private readonly IWeatherController _controller;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellWeatherFreeze", (effect, owner) => new SpellWeatherFreezeEffect(effect, owner));
	}

	public SpellWeatherFreezeEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, IWeatherEvent? weatherEvent = null) : base(owner, parent, prog)
	{
		WeatherEvent = weatherEvent;
		_controller = (owner as ICell)?.WeatherController ?? (owner as IZone)?.WeatherController;
		if (_controller != null)
		{
			if (WeatherEvent != null)
			{
				_controller.SetWeather(WeatherEvent);
			}
			_controller.FreezeWeather();
		}
	}

	protected SpellWeatherFreezeEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var tr = root.Element("Effect");
		var id = long.Parse(tr.Element("WeatherEventId")?.Value ?? "0");
		WeatherEvent = id != 0 ? Gameworld.WeatherEvents.Get(id) : null;
		_controller = (owner as ICell)?.WeatherController ?? (owner as IZone)?.WeatherController;
		if (_controller != null)
		{
			if (WeatherEvent != null)
			{
				_controller.SetWeather(WeatherEvent);
			}
			_controller.FreezeWeather();
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("WeatherEventId", WeatherEvent?.Id ?? 0)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return WeatherEvent != null
			? $"Weather Frozen as {WeatherEvent.Name.ColourValue()}"
			: "Weather Frozen";
	}

	protected override string SpecificEffectType => "SpellWeatherFreeze";

	public IWeatherEvent? WeatherEvent { get; set; }

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		_controller?.UnfreezeWeather();
	}
}

using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

#nullable enable
#nullable disable warnings

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellWeatherFreezeEffect : MagicSpellEffectBase
{
    private readonly IWeatherController _controller;
    private bool _weatherIsFrozen;
    private bool _pendingNextTransition;

    public static void InitialiseEffectType()
    {
        RegisterFactory("SpellWeatherFreeze", (effect, owner) => new SpellWeatherFreezeEffect(effect, owner));
    }

    public SpellWeatherFreezeEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg prog, IWeatherEvent? weatherEvent = null, bool nextTransition = false) : base(owner, parent, prog)
    {
        WeatherEvent = weatherEvent;
        _controller = (owner as ICell)?.WeatherController ?? (owner as IZone)?.WeatherController;
        if (_controller == null)
        {
            return;
        }

        if (nextTransition)
        {
            _controller.WeatherChanged += WeatherChanged;
            _pendingNextTransition = true;
            return;
        }

        ApplyFreeze();
    }

    protected SpellWeatherFreezeEffect(XElement root, IPerceivable owner) : base(root, owner)
    {
        XElement tr = root.Element("Effect");
        long id = long.Parse(tr.Element("WeatherEventId")?.Value ?? "0");
        WeatherEvent = id != 0 ? Gameworld.WeatherEvents.Get(id) : null;
        _controller = (owner as ICell)?.WeatherController ?? (owner as IZone)?.WeatherController;
        if (_controller != null)
        {
            ApplyFreeze();
        }
    }

    private void ApplyFreeze()
    {
        if (_controller == null || _weatherIsFrozen)
        {
            return;
        }

        if (WeatherEvent != null)
        {
            _controller.SetWeather(WeatherEvent);
        }

        _controller.FreezeWeather();
        _weatherIsFrozen = true;
    }

    private void WeatherChanged(IWeatherController sender, IWeatherEvent oldWeather, IWeatherEvent newWeather)
    {
        sender.WeatherChanged -= WeatherChanged;
        _pendingNextTransition = false;
        ApplyFreeze();
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
        ReleaseFreeze();
        base.ExpireEffect();
    }

    public override void RemovalEffect()
    {
        ReleaseFreeze();
        base.RemovalEffect();
    }

    private void ReleaseFreeze()
    {
        if (_pendingNextTransition)
        {
            _controller?.WeatherChanged -= WeatherChanged;
            _pendingNextTransition = false;
        }

        if (!_weatherIsFrozen)
        {
            return;
        }

        _controller?.UnfreezeWeather();
        _weatherIsFrozen = false;
    }
}

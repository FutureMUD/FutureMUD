#nullable enable

using System.Text.Json;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public sealed record WeatherOutlookPeriod(string Period, string Conditions, string Wind, string Hazards, double MinimumTemperature, double MaximumTemperature,
	int DayOffset = 0, string? DayPart = null);

/// <summary>A remembered observation, deliberately independent of later changes to the world's schedule.</summary>
public sealed class WeatherForecastReading : Effect
{
	public long ControllerId { get; }
	public long Day { get; }
	public int Horizon { get; }
	public Outcome Outcome { get; }
	public IReadOnlyList<WeatherOutlookPeriod> Periods { get; }
	public WeatherForecastReading(IPerceivable owner, long controllerId, long day, int horizon, Outcome outcome, IReadOnlyList<WeatherOutlookPeriod> periods) : base(owner)
	{
		ControllerId = controllerId; Day = day; Horizon = horizon; Outcome = outcome; Periods = periods;
	}
	private WeatherForecastReading(XElement root, IPerceivable owner) : base(root, owner)
	{
		var data = root.Element("Effect")!;
		ControllerId = (long)data.Attribute("Controller")!; Day = (long)data.Attribute("Day")!;
		Horizon = (int)data.Attribute("Horizon")!; Outcome = (Outcome)(int)data.Attribute("Outcome")!;
		Periods = JsonSerializer.Deserialize<WeatherOutlookPeriod[]>(data.Value) ?? [];
	}
	public static void InitialiseEffectType() => RegisterFactory("WeatherForecastReading", (root, owner) => new WeatherForecastReading(root, owner));
	protected override string SpecificEffectType => "WeatherForecastReading";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => $"Remembered weather forecast for controller #{ControllerId.ToStringN0(voyeur)}.";
	protected override XElement SaveDefinition() => new("Effect", new XAttribute("Controller", ControllerId), new XAttribute("Day", Day),
		new XAttribute("Horizon", Horizon), new XAttribute("Outcome", (int)Outcome), new XCData(JsonSerializer.Serialize(Periods)));
}

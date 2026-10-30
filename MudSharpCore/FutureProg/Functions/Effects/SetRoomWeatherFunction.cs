#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class SetRoomWeatherFunction(IList<IFunction> parameters, IFuturemud gameworld) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var room = ParameterFunctions[0].Result?.GetObject as IRoom;
		var id = (decimal)(ParameterFunctions[1].Result?.GetObject ?? 0m);
		var weather = id > 0 && id <= long.MaxValue && id == decimal.Truncate(id) ? gameworld.WeatherEvents.Get((long)id) : null;
		if (room is null || !ReferenceEquals(room.Gameworld, gameworld) || id < 0 ||
			id != decimal.Truncate(id) || id > long.MaxValue || id > 0 && (weather is null || weather.RequiresRoomFiveSecondTick))
		{
			Result = new BooleanVariable(false); return StatementResult.Normal;
		}
		var existing = room.EffectsOfType<RoomWeatherEffect>().ToArray();
		if (existing.Length == 1 && ReferenceEquals(existing[0].WeatherEvent, weather))
		{
			Result = new BooleanVariable(true); return StatementResult.Normal;
		}
		foreach (var effect in existing) room.RemoveEffect(effect, true);
		if (weather is not null) room.AddEffect(new RoomWeatherEffect(room, weather));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("setroomweather",
		[ProgVariableTypes.Location, ProgVariableTypes.Number],
		(parameters, world) => new SetRoomWeatherFunction(parameters, world),
		["room", "weatherId"],
		["The room.", "A native weather event ID, or zero to clear the local override."],
		"Sets saved local weather through a room effect. Repeating the same setting does not stack effects. Zero restores regional weather. Events requiring five-second work are unsupported. Does not change the regional controller or its forecast or regional temperature baseline. Returns false for invalid inputs.",
		"Effects", ProgVariableTypes.Boolean));
}

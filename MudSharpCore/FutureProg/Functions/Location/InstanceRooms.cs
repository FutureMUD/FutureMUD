#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Location;

internal sealed class InstanceRooms : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	public InstanceRooms(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters) => _gameworld = gameworld;
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Location | ProgVariableTypes.Collection; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var room = ParameterFunctions[0].Result?.GetObject as IRoom;
		var rooms = room is null ? Enumerable.Empty<IRoom>() :
			room.EffectsOfType<RoomTemplateInstance>().Any(x => x.IsTemplate)
				? _gameworld.Rooms.Where(x => x.EffectsOfType<RoomTemplateInstance>().Any(e => !e.IsTemplate && e.TemplateRoomId == room.Id))
				: new[] { room };
		Result = new CollectionVariable(rooms.ToList(), ProgVariableTypes.Location);
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"instancerooms", [ProgVariableTypes.Location], (pars, world) => new InstanceRooms(pars, world),
		["template"], ["A template room, or an ordinary room."],
		"Returns the loaded instances of a template room. Ordinary rooms return themselves; unused templates return an empty collection.",
		"Rooms", ProgVariableTypes.Location | ProgVariableTypes.Collection));
}

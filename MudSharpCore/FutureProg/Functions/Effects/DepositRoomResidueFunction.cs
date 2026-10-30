#nullable enable

using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class DepositRoomResidueFunction(IList<IFunction> parameters, IFuturemud gameworld) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var room = ParameterFunctions[0].Result?.GetObject as IRoom;
		var id = (decimal)(ParameterFunctions[1].Result?.GetObject ?? 0m);
		var amount = (double)(decimal)(ParameterFunctions[2].Result?.GetObject ?? 0m);
		var maximum = (double)(decimal)(ParameterFunctions[3].Result?.GetObject ?? 0m);
		if (room is null || !ReferenceEquals(room.Gameworld, gameworld) || id <= 0 || id > long.MaxValue ||
			id != decimal.Truncate(id) || gameworld.Materials.Get((long)id) is null || amount <= 0 || maximum <= 0 ||
			!WeatherHazardService.AtmosphereExposed(room))
		{
			Result = new BooleanVariable(false); return StatementResult.Normal;
		}
		WeatherResidue.Deposit(room, new WeatherHazardSettings { ResidueMaterialId = (long)id, ResiduePerMinute = amount, ResidueMaximum = maximum });
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("depositroomresidue",
		[ProgVariableTypes.Location, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number],
		(parameters, world) => new DepositRoomResidueFunction(parameters, world),
		["room", "materialId", "amount", "maximum"],
		["The exposed room.", "A native solid ID.", "Base mass units to add per recipient.", "Maximum base mass units of this material per recipient."],
		"Deposits dry material through native surface residue on exposed bodies and belongings. Uses native descriptions, persistence and cleaning. Invalid or sheltered rooms return false. Does not require a regional weather controller.",
		"Effects", ProgVariableTypes.Boolean));
}

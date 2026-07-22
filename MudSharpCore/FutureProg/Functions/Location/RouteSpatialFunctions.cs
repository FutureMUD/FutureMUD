#nullable enable

using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.Location;

/// <summary>
/// Read-only FutureProg surface for RouteCell geometry. All coordinate-sensitive functions
/// resolve lazy movement through <see cref="RouteSpatialService"/> rather than reading only the
/// last durable checkpoint.
/// </summary>
internal sealed class RouteSpatialFunction : BuiltInFunction
{
	private readonly Func<IReadOnlyList<IFunction>, IProgVariable?> _implementation;
	private readonly ProgVariableTypes _returnType;

	private RouteSpatialFunction(
		IList<IFunction> parameters,
		ProgVariableTypes returnType,
		Func<IReadOnlyList<IFunction>, IProgVariable?> implementation)
		: base(parameters)
	{
		_returnType = returnType;
		_implementation = implementation;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _returnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = _implementation(ParameterFunctions.ToArray());
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register(
			"isroutecell",
			[ProgVariableTypes.Location],
			ProgVariableTypes.Boolean,
			["location"],
			["The cell to inspect."],
			"Returns true when the location has linear RouteCell geometry.",
			parameters => new BooleanVariable(CellFrom(parameters[0])?.RouteDefinition is not null));
		Register(
			"routecelllength",
			[ProgVariableTypes.Location],
			ProgVariableTypes.Number,
			["location"],
			["The RouteCell to inspect."],
			"Returns the RouteCell length in metres, or null for an ordinary cell.",
			parameters => NumberOrNull(CellFrom(parameters[0])?.RouteDefinition?.LengthMetres));
		Register(
			"routecelltopologyversion",
			[ProgVariableTypes.Location],
			ProgVariableTypes.Number,
			["location"],
			["The RouteCell to inspect."],
			"Returns the RouteCell topology version, or null for an ordinary cell.",
			parameters => CellFrom(parameters[0])?.RouteDefinition is { } route
				? new NumberVariable(route.TopologyVersion)
				: null);
		Register(
			"routeposition",
			[ProgVariableTypes.Perceivable],
			ProgVariableTypes.Number,
			["perceivable"],
			["The character, item, or other located perceivable."],
			"Returns its effective metres-along-RouteCell coordinate, including lazy movement, or null outside a RouteCell.",
			parameters => NumberOrNull(EffectiveLocation(parameters[0])?.RoutePositionMetres));
		Register(
			"routeexactdistance",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable],
			ProgVariableTypes.Number,
			["first", "second"],
			["The first perceivable.", "The second perceivable."],
			"Returns exact longitudinal separation in metres when both perceivables share a RouteCell layer; otherwise null.",
			parameters =>
			{
				var first = LocateableFrom(parameters[0]);
				var second = LocateableFrom(parameters[1]);
				return first is null || second is null
					? null
					: NumberOrNull(RouteSpatialService.Instance.GetExactSeparation(
						RouteSpatialService.Instance.GetEffectiveLocation(first),
						RouteSpatialService.Instance.GetEffectiveLocation(second)));
			});
		Register(
			"routeroomequivalentdistance",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable],
			ProgVariableTypes.Number,
			["first", "second"],
			["The first perceivable.", "The second perceivable."],
			"Returns the weighted hybrid spatial-path cost in room equivalents, or null when no path exists.",
			parameters =>
			{
				var first = LocateableFrom(parameters[0]);
				var second = LocateableFrom(parameters[1]);
				if (first is null || second is null || first.Gameworld?.ExitManager?.SpatialPathfinder is null ||
					!first.Gameworld.ExitManager.SpatialPathfinder.TryFindPath(
						RouteSpatialService.Instance.GetEffectiveLocation(first),
						RouteSpatialService.Instance.GetEffectiveLocation(second),
						null,
						false,
						double.PositiveInfinity,
						out var path) || path is null)
				{
					return null;
				}

				return new NumberVariable(path.RoomEquivalentCost);
			});
		Register(
			"routerelativedirection",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable],
			ProgVariableTypes.Text,
			["origin", "target"],
			["The perceivable used as the origin.", "The target perceivable."],
			"Returns the RouteCell's authored positive/negative direction label, same, or blank when not comparable.",
			parameters => RelativeDirection(parameters[0], parameters[1]));
		Register(
			"routenearestlandmark",
			[ProgVariableTypes.Perceivable],
			ProgVariableTypes.Text,
			["perceivable"],
			["The located perceivable."],
			"Returns the stable name of the nearest RouteCell landmark, or blank when none applies.",
			parameters => NearestLandmark(parameters[0]));
		Register(
			"routeportalaccessible",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Exit],
			ProgVariableTypes.Boolean,
			["perceivable", "exit"],
			["The located perceivable.", "The portal exit to test."],
			"Returns true when the perceivable is currently inside the RouteCell exit's authored access band.",
			parameters => new BooleanVariable(
				LocateableFrom(parameters[0]) is { } locateable &&
				ExitFrom(parameters[1]) is { } exit &&
				RouteSpatialService.Instance.IsExitAccessible(locateable, exit)));
	}

	private static void Register(
		string name,
		ProgVariableTypes[] parameterTypes,
		ProgVariableTypes returnType,
		string[] parameterNames,
		string[] parameterHelp,
		string description,
		Func<IReadOnlyList<IFunction>, IProgVariable?> implementation)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameterTypes,
			(parameters, _) => new RouteSpatialFunction(parameters, returnType, implementation),
			parameterNames,
			parameterHelp,
			description,
			"RouteCells",
			returnType));
	}

	private static ICell? CellFrom(IFunction function)
	{
		return function.Result as ICell ?? function.Result?.GetObject as ICell;
	}

	private static IPerceivable? LocateableFrom(IFunction function)
	{
		return function.Result as IPerceivable ?? function.Result?.GetObject as IPerceivable;
	}

	private static ICellExit? ExitFrom(IFunction function)
	{
		return function.Result as ICellExit ?? function.Result?.GetObject as ICellExit;
	}

	private static SpatialLocation? EffectiveLocation(IFunction function)
	{
		return LocateableFrom(function) is { } locateable
			? RouteSpatialService.Instance.GetEffectiveLocation(locateable)
			: null;
	}

	private static IProgVariable? NumberOrNull(double? value)
	{
		return value.HasValue && double.IsFinite(value.Value) ? new NumberVariable(value.Value) : null;
	}

	private static IProgVariable RelativeDirection(IFunction originFunction, IFunction targetFunction)
	{
		var origin = EffectiveLocation(originFunction);
		var target = EffectiveLocation(targetFunction);
		if (!origin.HasValue || !target.HasValue || !ReferenceEquals(origin.Value.Cell, target.Value.Cell) ||
			origin.Value.Cell.RouteDefinition is not { } route ||
			!origin.Value.RoutePositionMetres.HasValue || !target.Value.RoutePositionMetres.HasValue)
		{
			return new TextVariable(string.Empty);
		}

		var difference = target.Value.RoutePositionMetres.Value - origin.Value.RoutePositionMetres.Value;
		return new TextVariable(difference switch
		{
			> 0.0005 => route.PositiveDirectionName,
			< -0.0005 => route.NegativeDirectionName,
			_ => "same"
		});
	}

	private static IProgVariable NearestLandmark(IFunction function)
	{
		var location = EffectiveLocation(function);
		if (!location.HasValue || location.Value.Cell.RouteDefinition is not { } route ||
			!location.Value.RoutePositionMetres.HasValue)
		{
			return new TextVariable(string.Empty);
		}

		var landmark = route.Landmarks
			.OrderBy(x => Math.Abs(x.PositionMetres - location.Value.RoutePositionMetres.Value))
			.ThenBy(x => x.DisplayOrder)
			.ThenBy(x => x.Id)
			.FirstOrDefault();
		return new TextVariable(landmark?.Name ?? string.Empty);
	}
}

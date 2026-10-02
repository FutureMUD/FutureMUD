using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.FutureProg.Functions.Magic;

internal sealed class MagicCastingFunction : BuiltInFunction
{
	public sealed record Contract(string Name, ProgVariableTypes Return, ProgVariableTypes[] Parameters, string Help);
	private static readonly ProgVariableTypes C = ProgVariableTypes.Character, K = ProgVariableTypes.MagicCapability,
		S = ProgVariableTypes.MagicSpell, N = ProgVariableTypes.Number, B = ProgVariableTypes.Boolean, T = ProgVariableTypes.Text;
	public static IReadOnlyList<Contract> Contracts { get; } =
	[
		new("hasacquiredspell", B, [C, S], "Pure canonical acquisition query, independent of capability and energy."),
		new("controlledspellgrade", N, [C, S], "Pure canonical controlled grade; zero means unacquired."),
		new("canchannelspell", B, [C, K, S, N, B], "Pure route, grade and acting-body preflight. Does not check target, materials or payment and does not reserve a cast."),
		new("channelspell", T, [C, K, S, N, B, T], "Runs the guarded paid normal casting service with native target syntax. Returns Refused, Failed, Succeeded or NeedsReview plus diagnostic."),
		new("grantchannelspell", B, [C, K, S, T], "Explicit authored acquisition grant with mandatory provenance. Returns true if admitted and acquired/already acquired. Never use this mutation in an inspection policy."),
		new("enrolchannelcasting", B, [C, K, T], "Explicit idempotent authored enrolment with permanent capability merit and provenance. Use in selected chargen-finalisation/NPC workflows, never queries.")
	];
	private readonly IFuturemud _world;
	private readonly Contract _contract;
	public MagicCastingFunction(IList<IFunction> parameters, IFuturemud world, Contract contract) : base(parameters) { _world = world; _contract = contract; }
	public override ProgVariableTypes ReturnType { get => _contract.Return; protected set { } }
	public static void RegisterFunctionCompiler()
	{
		foreach (var c in Contracts) FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(c.Name, c.Parameters,
			(parameters, world) => new MagicCastingFunction(parameters, world, c),
			c.Parameters.Select((_, i) => $"argument{i + 1}").ToArray(), c.Parameters.Select(x => x.Describe()).ToArray(), c.Help, "Magic", c.Return));
	}
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		try
		{
			var service = _world.MagicCasting ?? throw new InvalidOperationException("Configured casting is unavailable.");
			var actor = Required<ICharacter>(0);
			if (_contract.Name is "hasacquiredspell" or "controlledspellgrade")
			{
				var acquired = service.Acquisition(actor, Required<IMagicSpell>(1).Id);
				Result = _contract.Name == "hasacquiredspell" ? new BooleanVariable(acquired is not null) : new NumberVariable(acquired?.ControlledGrade ?? 0);
				return StatementResult.Normal;
			}
			var capability = Required<IMagicCapability>(1);
			if (_contract.Name == "enrolchannelcasting")
			{
				var reason = Value(2)?.ToString() ?? "";
				if (string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("An enrolment reason is required.");
				Result = new BooleanVariable(service is MagicCastingService runtime && runtime.EnrolAuthorised(actor, capability.Id, $"FutureProg: {reason}").Allowed);
				return StatementResult.Normal;
			}
			var spell = Required<IMagicSpell>(2);
			if (_contract.Name == "grantchannelspell")
			{
				var reason = Value(3)?.ToString() ?? "";
				if (string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("A grant reason is required.");
				Result = new BooleanVariable(service is MagicCastingService runtime && runtime.GrantAuthorised(actor, capability.Id, spell.Id, $"FutureProg: {reason}").Allowed);
				return StatementResult.Normal;
			}
			var number = Convert.ToDouble(Value(3));
			if (!double.IsFinite(number) || number != Math.Truncate(number) || number is < 1 or > 7) throw new InvalidOperationException("Grade must be an integer from 1 to 7.");
			var overreach = Value(4) is true;
			if (_contract.Name == "canchannelspell") Result = new BooleanVariable(service.Preflight(actor, capability.Id, spell.Id, (int)number, overreach) is null);
			else
			{
				var result = service.Cast(new(actor, capability.Id, spell.Id, (int)number, overreach, Value(5)?.ToString() ?? ""));
				Result = new TextVariable($"{result.Status}: {result.Message}");
			}
			return StatementResult.Normal;
		}
		catch (Exception ex) { ErrorMessage = ex.Message; return StatementResult.Error; }
	}
	private object? Value(int index) => ParameterFunctions[index].Result?.GetObject;
	private TRequired Required<TRequired>(int index) where TRequired : class => Value(index) as TRequired ?? throw new InvalidOperationException($"Argument {index + 1} requires {typeof(TRequired).Name}.");
}

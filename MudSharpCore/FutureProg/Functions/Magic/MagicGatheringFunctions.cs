using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.Gathering;

#nullable enable
namespace MudSharp.FutureProg.Functions.Magic;

/// <summary>
/// Script entry points intentionally route through the exact same live-action service as the school command.
/// They cannot prepay, forge a completion before elapsed time, select another actor's capability, or inject credit.
/// </summary>
internal sealed class MagicGatheringFunction : BuiltInFunction
{
	public sealed record Contract(string Name, ProgVariableTypes ReturnType, ProgVariableTypes[] Parameters, string Help);
	private readonly IFuturemud _gameworld;
	private readonly Contract _contract;

	private static readonly ProgVariableTypes C = ProgVariableTypes.Character;
	private static readonly ProgVariableTypes K = ProgVariableTypes.MagicCapability;
	private static readonly ProgVariableTypes T = ProgVariableTypes.Text;
	private static readonly ProgVariableTypes N = ProgVariableTypes.Number;
	private static readonly ProgVariableTypes B = ProgVariableTypes.Boolean;

	public MagicGatheringFunction(IList<IFunction> parameters, IFuturemud gameworld, Contract contract) : base(parameters)
	{
		_gameworld = gameworld;
		_contract = contract;
	}

	public override ProgVariableTypes ReturnType { get => _contract.ReturnType; protected set { } }

	public static IReadOnlyList<Contract> Contracts { get; } =
	[
		new("canmagicgather", B, [C, K, T, N], "Pure full-plan validation for one configured gathering method and amount. It creates no receipt, does not advance environmental production, and does not reserve a room source."),
		new("beginmagicgather", T, [C, K, T, N], "Begins the normal timed gathering action and returns its engine-issued token, or empty text on refusal. The actor must possess the exact configured capability."),
		new("completemagicgather", B, [C, T], "Attempts controlled completion for an engine-issued live token. It rechecks elapsed time, body, location, policy, full price and exact source debit; forged or early tokens return false."),
		new("cancelmagicgather", B, [C, T], "Cancels only the caller identity's live precommit token. It never refunds or replays a committed operation."),
		new("magicgatherstatus", T, [C, T], "Returns the caller identity's durable gathering receipt status, or empty text for another owner or an unknown token.")
	];

	public static void RegisterFunctionCompiler()
	{
		foreach (Contract contract in Contracts)
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(contract.Name, contract.Parameters,
				(parameters, game) => new MagicGatheringFunction(parameters, game, contract),
				contract.Parameters.Select((_, index) => $"argument{index + 1}").ToArray(),
				contract.Parameters.Select(x => x.Describe()).ToArray(), contract.Help, "Magic", contract.ReturnType));
		}
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		try
		{
			IMagicGatheringService service = _gameworld.MagicGathering ?? throw new InvalidOperationException("The gathering service is unavailable.");
			ICharacter actor = Required<ICharacter>(0);
			if (_contract.Name is "completemagicgather" or "cancelmagicgather" or "magicgatherstatus")
			{
				if (!Guid.TryParse(Text(1), out Guid token))
				{
					throw new InvalidOperationException("A valid engine-issued gathering token is required.");
				}

				switch (_contract.Name)
				{
					case "completemagicgather":
						Result = new BooleanVariable(service.Complete(actor, token).Success);
						break;
					case "cancelmagicgather":
						Result = new BooleanVariable(service.Cancel(actor, token).Success);
						break;
					default:
						MagicGatheringOperationSummary? operation = service.Operation(token);
						Result = new TextVariable(operation?.OwnerId == MagicGatheringPolicy.Owner(actor).Id ? operation.Status : string.Empty);
						break;
				}

				return StatementResult.Normal;
			}

			IMagicGatheringCapability capability = Required<IMagicGatheringCapability>(1);
			double amount = Number(3);
			MagicGatheringResult result = _contract.Name == "canmagicgather"
				? service.Preview(actor, capability, Text(2), amount)
				: service.Begin(actor, capability, Text(2), amount);
			Result = _contract.Name == "canmagicgather"
				? new BooleanVariable(result.Success)
				: new TextVariable(result.Success ? result.OperationId?.ToString() ?? string.Empty : string.Empty);
			return StatementResult.Normal;
		}
		catch (Exception ex)
		{
			ErrorMessage = ex.Message;
			return StatementResult.Error;
		}
	}

	private object? Value(int index) => ParameterFunctions[index].Result?.GetObject;
	private string Text(int index) => Value(index)?.ToString() ?? string.Empty;
	private T Required<T>(int index) where T : class => Value(index) as T ?? throw new InvalidOperationException($"Argument {index + 1} must be a non-null {typeof(T).Name}.");
	private double Number(int index)
	{
		object? value = Value(index);
		if (value is not (decimal or double or float or int or long))
		{
			throw new InvalidOperationException($"Argument {index + 1} must be a number.");
		}

		double number = Convert.ToDouble(value);
		if (!double.IsFinite(number))
		{
			throw new InvalidOperationException("The gathering amount must be finite.");
		}

		return number;
	}
}

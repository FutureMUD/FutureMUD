#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.Magic.Gathering;

/// <summary>
/// Exact capability-policy contracts and fail-closed evaluation shared by loading, builder validation and
/// the live gathering service. Values are deliberately evaluated per quote; stateful policies are never cached.
/// </summary>
public static class MagicGatheringPolicy
{
	[ThreadStatic] private static HashSet<IFutureProg>? _evaluating;

	private static readonly ProgVariableTypes C = ProgVariableTypes.Character;
	private static readonly ProgVariableTypes K = ProgVariableTypes.MagicCapability;
	private static readonly ProgVariableTypes T = ProgVariableTypes.Text;
	private static readonly ProgVariableTypes N = ProgVariableTypes.Number;
	private static readonly ProgVariableTypes L = ProgVariableTypes.Location;
	private static readonly ProgVariableTypes B = ProgVariableTypes.Boolean;

	public static readonly IReadOnlyDictionary<string, (ProgVariableTypes Return, ProgVariableTypes[] Parameters)> Signatures =
		new Dictionary<string, (ProgVariableTypes, ProgVariableTypes[])>(StringComparer.OrdinalIgnoreCase)
		{
			["permission"] = (B, [C, C, K, T, N, L]),
			["numeric"] = (N, [C, C, K, T, N, L]),
			["onsuccess"] = (ProgVariableTypes.Void, [C, C, K, T, N, L, T])
		};

	public static bool ValidSignature(IFutureProg? prog, string kind) =>
		Signatures.TryGetValue(kind, out var signature) && prog is not null && !prog.AcceptsAnyParameters &&
		string.IsNullOrEmpty(prog.CompileError) && prog.ReturnType == signature.Return &&
		prog.Parameters.SequenceEqual(signature.Parameters);

	public static IReadOnlyList<string> ConfigurationErrors(IFuturemud gameworld, MagicGatheringMethodDefinition method)
	{
		List<string> errors = [];
		string label = $"Gathering method {method.Alias} ({method.Key})";
		if (!Enum.IsDefined(method.Kind) || method.Key == Guid.Empty || string.IsNullOrWhiteSpace(method.Alias) ||
			string.IsNullOrWhiteSpace(method.Name) || method.StructuralVersion < 1)
		{
			errors.Add($"{label}: invalid identity, presentation, kind or structural version.");
		}

		if (!FinitePositive(method.MinimumAmount) || !FinitePositive(method.MaximumAmount) || method.MaximumAmount < method.MinimumAmount ||
			!FinitePositive(method.DurationSeconds))
		{
			errors.Add($"{label}: amount bounds and duration must be finite and positive.");
		}

		if (!FiniteNonNegative(method.StaminaCost) || !FiniteNonNegative(method.MinimumStamina) ||
			!FiniteNonNegative(method.DamageCost) || !FiniteNonNegative(method.PainCost) || !FiniteNonNegative(method.StunCost))
		{
			errors.Add($"{label}: bodily costs must be finite and non-negative.");
		}

		IMagicResource? destination = gameworld.MagicResources.Get(method.DestinationResourceId);
		if (destination is null || !destination.ResourceType.HasFlag(MagicResourceType.PlayerResource))
		{
			errors.Add($"{label}: destination must be an existing character-capable magic resource.");
		}

		if (method.Kind == MagicGatheringMethodKind.Gentle)
		{
			IMagicResource? source = method.SourceResourceId is { } sourceId ? gameworld.MagicResources.Get(sourceId) : null;
			if (source is null || !source.ResourceType.HasFlag(MagicResourceType.LocationResource))
			{
				errors.Add($"{label}: Gentle gathering requires an existing location-capable source resource.");
			}
			if (!FinitePositive(method.SourceUnitsPerDestinationUnit))
			{
				errors.Add($"{label}: source units per destination unit must be finite and positive.");
			}
		}
		else if (method.SourceResourceId.HasValue)
		{
			errors.Add($"{label}: Self gathering has no environmental source resource.");
		}

		bool healthPotential = method.DamageCost > 0.0 || method.PainCost > 0.0 || method.StunCost > 0.0 ||
			method.DamageCostProgId != 0 || method.PainCostProgId != 0 || method.StunCostProgId != 0;
		if (healthPotential && method.MaximumHealthSeverity == WoundSeverity.None)
		{
			errors.Add($"{label}: health-priced gathering requires an explicit maximum health severity.");
		}

		if (method.Kind == MagicGatheringMethodKind.Self && method.StaminaCost <= 0.0 && !healthPotential &&
			method.StaminaCostProgId == 0)
		{
			errors.Add($"{label}: Self gathering needs a genuine configured bodily price.");
		}

		CheckProg(method.PermissionProgId, "permission", "permission");
		CheckProg(method.DurationProgId, "numeric", "duration");
		CheckProg(method.StaminaCostProgId, "numeric", "stamina cost");
		CheckProg(method.DamageCostProgId, "numeric", "damage cost");
		CheckProg(method.PainCostProgId, "numeric", "pain cost");
		CheckProg(method.StunCostProgId, "numeric", "stun cost");
		CheckProg(method.OnGatheredProgId, "onsuccess", "post-gather");
		return errors.AsReadOnly();

		void CheckProg(long id, string kind, string description)
		{
			if (id == 0)
			{
				return;
			}

			if (!ValidSignature(gameworld.FutureProgs.Get(id), kind))
			{
				errors.Add($"{label}: {description} prog #{id} is missing, uncompiled or has the wrong signature.");
			}
		}
	}

	public static ICharacter Owner(ICharacter actor) => actor.Identity?.PrimaryInstance ?? actor;

	public static bool Permits(IFutureProg? prog, params object[] arguments)
	{
		if (prog is null)
		{
			return true;
		}

		return Evaluate(prog, arguments) is true;
	}

	public static double Number(IFutureProg? prog, double fallback, string description, params object[] arguments)
	{
		object? value = prog is null ? fallback : Evaluate(prog, arguments);
		if (value is not (decimal or double or float or int or long))
		{
			throw new InvalidOperationException($"The {description} prog returned no number.");
		}

		double number = Convert.ToDouble(value, CultureInfo.InvariantCulture);
		if (!double.IsFinite(number) || number < 0.0)
		{
			throw new InvalidOperationException($"The {description} must be finite and non-negative.");
		}

		return number;
	}

	private static object? Evaluate(IFutureProg prog, object[] arguments)
	{
		HashSet<IFutureProg> evaluating = _evaluating ??= new(ReferenceEqualityComparer.Instance);
		if (!evaluating.Add(prog))
		{
			throw new InvalidOperationException($"Recursive gathering policy #{prog.Id} is not permitted.");
		}

		try
		{
			if (!prog.ExecuteWithStatus(out object result, arguments))
			{
				throw new InvalidOperationException($"Gathering policy #{prog.Id} reported execution failure.");
			}

			return result;
		}
		finally
		{
			evaluating.Remove(prog);
		}
	}

	private static bool FinitePositive(double value) => double.IsFinite(value) && value > 0.0;
	private static bool FiniteNonNegative(double value) => double.IsFinite(value) && value >= 0.0;
}

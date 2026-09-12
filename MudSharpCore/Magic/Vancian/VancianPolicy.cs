using System.Globalization;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Magic.Vancian;

/// <summary>Exact signatures and fail-closed evaluation shared by editors and runtime.</summary>
public static class VancianPolicy
{
	[ThreadStatic] private static HashSet<IFutureProg>? _evaluating;
	private static object? Evaluate(IFutureProg prog, object[] args)
	{
		var evaluating = _evaluating ??= new(ReferenceEqualityComparer.Instance);
		if (!evaluating.Add(prog)) throw new InvalidOperationException($"Recursive Vancian policy #{prog.Id} is not permitted.");
		try { return prog.Execute(args); }
		finally { evaluating.Remove(prog); }
	}
	private static readonly ProgVariableTypes C = ProgVariableTypes.Character;
	private static readonly ProgVariableTypes K = ProgVariableTypes.MagicCapability;
	private static readonly ProgVariableTypes S = ProgVariableTypes.MagicSpell;
	private static readonly ProgVariableTypes N = ProgVariableTypes.Number;
	private static readonly ProgVariableTypes B = ProgVariableTypes.Boolean;
	private static readonly ProgVariableTypes I = ProgVariableTypes.Item;
	public static readonly IReadOnlyDictionary<string, (ProgVariableTypes Return, ProgVariableTypes[] Parameters)> Signatures =
		new Dictionary<string, (ProgVariableTypes, ProgVariableTypes[])>(StringComparer.OrdinalIgnoreCase)
		{
			["casterlevel"] = (N, [C, K]),
			["candidates"] = (B, [C, K, S]),
			["limit"] = (N, [C, K, N, N]),
			["count"] = (N, [C, K, N, N]),
			["eligibility"] = (B, [C, K, S]),
			["canchangeknown"] = (B, [C, K, S | ProgVariableTypes.Collection, S | ProgVariableTypes.Collection]),
			["onchangeknown"] = (ProgVariableTypes.Void, [C, K, S | ProgVariableTypes.Collection, S | ProgVariableTypes.Collection]),
			["cancast"] = (B, [C, K, S]),
			["canrefresh"] = (B, [C, K]),
			["onrefresh"] = (ProgVariableTypes.Void, [C, K]),
			["bookuse"] = (B, [C, K, I]),
			["transcribe"] = (B, [C, K, S, I, I]),
			["inscribe"] = (B, [C, K, S, I, N]),
			["oninscribe"] = (ProgVariableTypes.Void, [C, K, S, I, N]),
			["scrolluse"] = (B, [C, K, S, I]),
			["scrolldifficulty"] = (N, [C, K, S, N, N])
		};

	public static bool ValidSignature(IFutureProg? prog, string kind) => Signatures.ContainsKey(kind) && prog is not null &&
		!prog.AcceptsAnyParameters && string.IsNullOrEmpty(prog.CompileError) &&
		prog.ReturnType == Signatures[kind].Return && prog.Parameters.SequenceEqual(Signatures[kind].Parameters);

	public static bool Permits(IFutureProg? prog, bool absent, params object[] args)
	{
		if (prog is null) return absent;
		try { return Evaluate(prog, args) is true; }
		catch { return false; }
	}

	public static int Number(IFutureProg? prog, params object[] args)
	{
		if (prog is null) throw new InvalidOperationException("A required numerical prog is missing.");
		var value = Evaluate(prog, args);
		if (value is not (decimal or double or float or int or long))
			throw new InvalidOperationException($"Prog #{prog.Id} returned no valid number.");
		var number = Convert.ToDouble(value, CultureInfo.InvariantCulture);
		if (!double.IsFinite(number) || number < 0 || number > int.MaxValue)
			throw new InvalidOperationException($"Prog #{prog.Id} must return a finite non-negative integer-sized number.");
		return checked((int)Math.Floor(number));
	}

	public static SpellPower Power(IVancianMagicCapability capability, int spellLevel, int castingLevel)
	{
		if (spellLevel < 0 || castingLevel < spellLevel || capability.PowerStepPerSlotLevel < 0 || !Enum.IsDefined(capability.BasePower))
			throw new InvalidOperationException("Invalid spell level, casting level or power configuration.");
		return (SpellPower)Math.Min((long)SpellPower.RecklesslyPowerful,
			(long)capability.BasePower + ((long)castingLevel - spellLevel) * capability.PowerStepPerSlotLevel);
	}

	public static ICharacter Owner(ICharacter actor) => actor.Identity?.PrimaryInstance ?? actor;
}

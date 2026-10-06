#nullable enable
namespace MudSharp.Magic.SpellTriggers;

/// <summary>Validation for the optional single-character and single-item target policies.</summary>
internal static class CastingTargetFilter
{
	internal static string? Error(long id, IFutureProg? prog, ProgVariableTypes targetType)
	{
		if (id == 0) return null;
		if (prog is null) return $"Target filter prog #{id} is missing; replace or explicitly clear the filter.";
		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
			!prog.MatchesParameters([targetType, ProgVariableTypes.Character]) && !prog.MatchesParameters([targetType]))
			return $"Target filter prog #{id} must return boolean and accept target or (target, caster).";
		return string.IsNullOrEmpty(prog.CompileError) ? null : $"Target filter prog #{id} does not compile: {prog.CompileError}";
	}

	internal static bool Allows(long id, IFutureProg? prog, ProgVariableTypes targetType, IPerceivable target, ICharacter caster)
	{
		if (id == 0) return true;
		if (Error(id, prog, targetType) is not null) return false;
		try { return prog!.ExecuteWithStatus(out var result, target, caster) && result is true; }
		catch { return false; } // A policy execution failure cannot authorize a paid target.
	}
}

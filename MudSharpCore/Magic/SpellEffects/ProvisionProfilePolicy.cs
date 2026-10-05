#nullable enable
using MudSharp.FutureProg;
namespace MudSharp.Magic.SpellEffects;

/// <summary>Admission checks shared only by food output and container recipe profiles.</summary>
internal static class ProvisionProfilePolicy
{
	internal static string Stamp(IFuturemud world, IEnumerable<long> ids) => string.Join(";", ids.Select(id => {
		var prog = world.FutureProgs.Get(id);
		return prog is null ? $"{id}:missing" : $"{id}:{System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(prog)}:{prog.FunctionText}:{prog.ReturnType}:{prog.CompileError}";
	}));
	internal static string? Error(IFuturemud world, long id)
	{
		if (id == 0) return null;
		var prog = world.FutureProgs.Get(id);
		return prog is null ? $"Profile predicate #{id} is missing." :
			!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !prog.MatchesParameters([ProgVariableTypes.Character]) ?
			$"Profile predicate #{id} requires boolean(character)." :
			!string.IsNullOrEmpty(prog.CompileError) ? $"Profile predicate #{id} does not compile: {prog.CompileError}" : null;
	}
	internal static bool TryMatch(IFuturemud world, long id, ICharacter actor, out bool matches, out string? error)
	{
		matches = false; error = Error(world, id);
		if (error is not null) return false;
		if (id == 0) { matches = true; return true; }
		try {
			var prog = world.FutureProgs.Get(id);
			if (prog.ExecuteWithStatus(out var result, actor) && result is bool value) { matches = value; return true; }
			error = $"Profile predicate #{id} failed or returned no boolean result.";
		} catch (Exception exception) { error = $"Profile predicate #{id} failed: {exception.Message}"; }
		return false;
	}
	internal sealed record ActorFrame(ICharacter Actor, object Body, object Location, object Layer, object Instance)
	{
		internal static ActorFrame Capture(ICharacter actor) => new(actor, actor.Body, actor.Location, actor.RoomLayer, actor.InstanceId);
		internal bool Matches(ICharacter actor) => ReferenceEquals(Actor, actor) && ReferenceEquals(Body, actor.Body) &&
			ReferenceEquals(Location, actor.Location) && Equals(Layer, actor.RoomLayer) && Equals(Instance, actor.InstanceId);
	}
}

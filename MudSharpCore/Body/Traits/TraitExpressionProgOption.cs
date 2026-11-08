#nullable enable

using MudSharp.Body;

namespace MudSharp.Body.Traits;

/// <summary>A numeric character prog supplies a live value without changing trait-reference syntax.</summary>
public sealed class TraitExpressionProgOption(string reference, IFuturemud gameworld) : TraitExpressionExtendedOption
{
	[ThreadStatic] private static List<(ICharacter Owner, long Prog)>? _evaluating;

	public override double Evaluate(IHaveTraits owner)
	{
		var character = owner as ICharacter ?? (owner as IBody)?.Actor;
		if (character is null) return 0;
		var prog = long.TryParse(reference, out var id) ? gameworld.FutureProgs.Get(id) : gameworld.FutureProgs.GetByName(reference);
		if (prog is null || prog.ReturnType != ProgVariableTypes.Number || !prog.MatchesParameters([ProgVariableTypes.Character])) return 0;
		var key = (character, prog.Id);
		var evaluating = _evaluating ??= [];
		if (evaluating.Any(x => ReferenceEquals(x.Owner, character) && x.Prog == prog.Id)) return 0;
		evaluating.Add(key);
		try
		{
			var value = prog.ExecuteDouble(character);
			return double.IsFinite(value) ? value : 0;
		}
		finally
		{
			evaluating.RemoveAt(evaluating.Count - 1);
		}
	}
}

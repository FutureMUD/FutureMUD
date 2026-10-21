#nullable enable

using MudSharp.Body;

namespace MudSharp.Character;

public partial class Character
{
	/// <summary>Synchronous body-dependent merit/prog evaluation, using the same selection convention as form vital initialization.</summary>
	internal T EvaluateMeritsForBody<T>(IBody selectedBody, Func<T> evaluate)
	{
		var original = Body;
		if (ReferenceEquals(original, selectedBody)) return evaluate();
		Body = selectedBody;
		try { return evaluate(); }
		finally { Body = original; }
	}
}

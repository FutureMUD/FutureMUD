#nullable enable

using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic;

/// <summary>Optional typed input binding on a detached invocation, before effect selection or payment.</summary>
public interface IMagicSpellEffectInvocationInputs
{
	/// <summary>Bind explicit parser inputs without drawing, writing, granting or changing shared definitions.</summary>
	bool TryBindInvocationInputs(ICharacter caster, IPerceivable recipient,
		IReadOnlyList<SpellAdditionalParameter> parameters, out string? error);
}

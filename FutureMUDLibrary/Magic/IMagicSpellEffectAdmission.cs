#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Opt-in target admission, before any child construction or exclusive parent cleanup.</summary>
public interface IMagicSpellEffectAdmission
{
	bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error);
}

public interface IMagicSpellEffectApplication
{
	IMagicSpellEffect Create(IMagicSpellEffectParent parent);
}

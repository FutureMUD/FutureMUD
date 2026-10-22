#nullable enable

using MudSharp.Effects;

namespace MudSharp.Effects.Interfaces;

/// <summary>Source-neutral status contract for effects that impair mental action.</summary>
public interface IMindImpairmentEffect : IEffectSubtype
{
	/// <summary>Modifier applied when the impaired character initiates a psionic action.</summary>
	double PsionicInitiatorModifier { get; }

	/// <summary>Modifier applied when the impaired character resists a psionic action.</summary>
	double PsionicTargetModifier { get; }

	/// <summary>Modifier applied to eligible non-psionic skill use.</summary>
	double SkillUseModifier { get; }

	/// <summary>Whether an impaired character is ineligible to learn from a failed skill attempt.</summary>
	bool PreventsSkillLearning { get; }
}

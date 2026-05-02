using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

internal interface IReadiedRangedWeaponStaminaSource
{
	IGameItem Parent { get; }
	bool IsReadied { get; set; }
	double StaminaPerTick { get; }
	bool ReadiedUseRequiresFreeHand { get; }
	string ReadiedStaminaReleaseEmote { get; }
	string ReadiedStaminaNoFreeHandEmote { get; }
	string ReadiedStaminaExhaustedEmote { get; }
	string ReadiedStaminaEffectDescription { get; }

	bool Unready(ICharacter readier);

	event PerceivableEvent OnFire;
	event PerceivableEvent OnUnready;
}

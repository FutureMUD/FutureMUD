#nullable enable

using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Magic;

public interface ISpellOwnedPocketService
{
	string? AdmissionError(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration, int grade);
	IGameItem Create(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration, SpellLifecycleOrigin origin);
	bool IsActive(IGameItem carrier);
	bool CanWithdraw(IGameItem carrier);
	bool TryPrepareRemoval(IGameItem carrier, out string diagnostic);
}

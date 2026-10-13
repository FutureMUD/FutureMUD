#nullable enable

using System;
using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Magic;

public interface ISpellOwnedPocketService
{
	string? AdmissionError(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration, int grade);
	IGameItem Create(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration, SpellLifecycleOrigin origin);
	IGameItem CreatePrepared(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration,
		SpellLifecycleOrigin origin, Func<bool> selectionCurrent) =>
		throw new InvalidOperationException("The pocket service cannot fence a prepared native invocation.");
	bool IsActive(IGameItem carrier);
	bool CanWithdraw(IGameItem carrier);
	bool TryPrepareRemoval(IGameItem carrier, out string diagnostic);
}

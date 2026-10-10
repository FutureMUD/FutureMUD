#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Magic;

namespace MudSharp.GameItems.Interfaces;

public interface ISpellPocket : IContainer
{
	Guid LifecycleId { get; }
	SpellPocketAnchor? Anchor { get; }
	bool CanAccess(ICharacter actor);
}

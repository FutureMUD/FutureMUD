using System;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Character;

/// <summary>Authority for a single explicit action, restricted to the guard effects it challenged.</summary>
public interface IGuardChallengeContext : IDisposable
{
	ICharacter Actor { get; }
	IRoom Origin { get; }
	bool IsValid { get; }
	bool Bypasses(ICharacter mover, IRoomExit exit, IGuardExitEffect guard);
	bool Bypasses(ICharacter actor, IGameItem item, IGuardItemEffect guard);
}

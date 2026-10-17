#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

/// <summary>A transient covert action: no public suffix, saved action or separate cooldown.</summary>
public sealed class PeekInspectionAction : BlockingDelayedAction, IRemoveOnMovementEffect, IRemoveOnCombatStart
{
	public PeekInspectionAction(ICharacter owner, Action<IPerceivable> complete)
		: base(owner, complete, "covertly inspecting belongings", new[] { "general", "movement", "combat" }, "")
	{
	}

	public override void InitialEffect() => CharacterOwner.OnQuit += OwnerQuit;

	private void OwnerQuit(IPerceivable owner) => owner.RemoveEffect(this, true);

	public override void ExpireEffect()
	{
		// A removed or cancelled schedule callback must never resolve an inspection later.
		if (!Owner.Effects.Contains(this)) return;
		try
		{
			Action(Owner);
		}
		finally
		{
			Owner.RemoveEffect(this, true);
		}
	}

	public override void RemovalEffect()
	{
		CharacterOwner.OnQuit -= OwnerQuit;
		base.RemovalEffect();
	}

	public bool ShouldRemove() => true;
	public override void Login() => Owner.RemoveEffect(this, true);
}

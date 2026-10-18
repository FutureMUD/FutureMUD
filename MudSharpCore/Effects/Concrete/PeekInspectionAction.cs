#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

/// <summary>A transient covert action: no public suffix, saved action or separate cooldown.</summary>
public sealed class PeekInspectionAction : BlockingDelayedAction, IRemoveOnMovementEffect, IRemoveOnCombatStart
{
	private readonly ICharacter _target;

	public PeekInspectionAction(ICharacter owner, ICharacter target, Action<IPerceivable> complete)
		: base(owner, complete, "covertly inspecting belongings", new[] { "general", "movement", "combat" }, "")
	{
		_target = target;
	}

	public override void InitialEffect()
	{
		CharacterOwner.OnQuit += OwnerQuit;
		_target.OnQuit += TargetGone;
		_target.OnDeleted += TargetGone;
	}

	private void OwnerQuit(IPerceivable owner) => owner.RemoveEffect(this, true);
	private void TargetGone(IPerceivable target) => Owner.RemoveEffect(this, true);

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
		_target.OnQuit -= TargetGone;
		_target.OnDeleted -= TargetGone;
		base.RemovalEffect();
	}

	public bool ShouldRemove() => true;
	public override void Login() => Owner.RemoveEffect(this, true);
}

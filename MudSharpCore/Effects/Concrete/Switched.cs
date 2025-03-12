using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class Switched : Effect, INoQuitEffect, IPauseAIEffect, ICountForWho, IIgnoreForceEffect
{
	public ICharacter CharacterOwner { get; set; }
	public ICharacter OriginalCharacter { get; set; }

	public string NoQuitReason => "You cannot quit while you are switched. Return first instead.";

	public Switched(ICharacter owner, ICharacter originalCharacter) : base(owner)
	{
		CharacterOwner = owner;
		OriginalCharacter = originalCharacter;
		owner.OnDeath += OwnerOnOnDeath;
	}

	private void OwnerOnOnDeath(IPerceivable owner)
	{
		DoReturn();
	}

	public void DoReturn()
	{
		CharacterOwner.OnDeath -= OwnerOnOnDeath;
		Owner.Send("You return to your original body.");
		CharacterOwner.Controller.SetContext(OriginalCharacter);
		CharacterOwner.RemoveAllEffects<AdminTelepathy>(fireRemovalAction: true);
		CharacterOwner.RemoveAllEffects<AdminSight>(fireRemovalAction: true);
		CharacterOwner.RemoveAllEffects<AdminSpyMaster>(fireRemovalAction: true);
		Owner.RemoveEffect(this, false);
	}

	#region Overrides of Effect

	public override void RemovalEffect()
	{
		DoReturn();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Being controlled by an admin";
	}

	protected override string SpecificEffectType => "Switched";

	#endregion
}
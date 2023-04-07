using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class MindConnectedToEffect : Effect, IMagicEffect
{
	public ICharacter CharacterOwner { get; protected set; }
	public ICharacter OriginatorCharacter { get; protected set; }
	public ConnectMindEffect OriginatorEffect { get; protected set; }

	public MindConnectedToEffect(ICharacter owner, ICharacter originator, ConnectMindEffect originatorEffect) :
		base(owner)
	{
		CharacterOwner = owner;
		OriginatorCharacter = originator;
		OriginatorEffect = originatorEffect;
	}

	private void CharacterOwner_OnLocationChanged(Form.Shape.ILocateable locatable,
		Construction.Boundary.ICellExit exit)
	{
		OriginatorEffect.CharacterOwner_OnLocationChanged(locatable, exit);
	}

	private void CharacterOwner_OnNoLongerValid(IPerceivable owner)
	{
		OriginatorCharacter.RemoveEffect(OriginatorEffect, true);
	}

	protected void RegisterEvents()
	{
		CharacterOwner.OnQuit += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeleted += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged += CharacterOwner_OnLocationChanged;
	}

	public void ReleaseEvents()
	{
		CharacterOwner.OnQuit -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeleted -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged -= CharacterOwner_OnLocationChanged;
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{OriginatorCharacter.HowSeen(voyeur, true)} has connected to their mind via {School.Name.Colour(School.PowerListColour)}.";
	}

	protected override string SpecificEffectType => "MindConnectedTo";

	public override void RemovalEffect()
	{
		ReleaseEvents();
	}

	#endregion

	#region Implementation of IMagicEffect

	public IMagicSchool School => PowerOrigin.School;
	public IMagicPower PowerOrigin => OriginatorEffect.PowerOrigin;
	public Difficulty DetectMagicDifficulty => OriginatorEffect.DetectMagicDifficulty;

	#endregion
}
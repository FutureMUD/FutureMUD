using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class BeingMagicChoked : Effect, IStopBreathing, INoQuitEffect
{
	public ICharacter CharacterOwner { get; }
	public ICharacter OriginatorCharacter => OriginatorEffect.CharacterOwner;

	public MagicChoking OriginatorEffect;

	public BeingMagicChoked(ICharacter owner, MagicChoking parentEffect) : base(owner)
	{
		CharacterOwner = owner;
		OriginatorEffect = parentEffect;
		CharacterOwner.OnQuit += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeleted += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeath += CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged += CharacterOwner_OnLocationChanged;
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

	public void ReleaseEvents()
	{
		CharacterOwner.OnQuit -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeleted -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnDeath -= CharacterOwner_OnNoLongerValid;
		CharacterOwner.OnLocationChanged -= CharacterOwner_OnLocationChanged;
	}

	protected override string SpecificEffectType => "BeingMagicChoked";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Being magically choked by {OriginatorCharacter.HowSeen(voyeur)} through the {OriginatorEffect.Power.Name.ColourValue()} power.";
	}

	public string NoQuitReason => "You can't quit while you can't breathe!";

	public override bool Applies(object target, object thirdparty)
	{
		if (target is ICharacter ch && thirdparty is ChokePower cp)
		{
			return ch == OriginatorCharacter && OriginatorEffect.Power == cp;
		}

		return base.Applies(target, thirdparty);
	}
}
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Effects.Concrete;

public abstract class ConcentrationConsumingEffect : Effect, IConcentrationConsumingEffect
{
	public ICharacter CharacterOwner { get; protected set; }
	public IMagicSchool School { get; protected set; }

	protected ConcentrationConsumingEffect(ICharacter owner, IMagicSchool school, double concentration) : base(owner)
	{
		School = school;
		CharacterOwner = owner;
		ConcentrationPointsConsumed = concentration;
	}

	protected ConcentrationConsumingEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		CharacterOwner = (ICharacter)owner;
		var root = effect.Element("Effect");
		ConcentrationPointsConsumed = double.Parse(root.Element("ConcentrationPointsConsumed").Value);
		School = Gameworld.MagicSchools.Get(long.Parse(root.Element("School").Value));
	}

	protected XElement SaveToXml(params XElement[] addenda)
	{
		return new XElement("Effect",
			new XElement("ConcentrationPointsConsumed", ConcentrationPointsConsumed),
			new XElement("School", School.Id),
			addenda
		);
	}

	public override void Login()
	{
		base.Login();
		RegisterEvents();
	}

	public double ConcentrationPointsConsumed { get; private set; }

	protected virtual bool EffectCanPersistOnLogout => false;

	protected virtual void RegisterEvents()
	{
		CharacterOwner.OnWounded += CharacterOwner_OnWounded;
		if (EffectCanPersistOnLogout)
		{
			CharacterOwner.OnQuit += CharacterOwner_OnQuit;
		}
	}

	private void CharacterOwner_OnQuit(IPerceivable owner)
	{
		ReleaseEvents();
	}

	private void CharacterOwner_OnWounded(Health.IMortalPerceiver wounded, Health.IWound wound)
	{
		var capability = CharacterOwner.Capabilities.Where(x => x.School == School).FirstMax(x => x.PowerLevel);
		if (capability == null)
		{
			CharacterOwner.RemoveEffect(this, true);
			return;
		}

		var concentrationCapability = capability.ConcentrationAbility(CharacterOwner);
		if (concentrationCapability <= 0.0)
		{
			CharacterOwner.RemoveEffect(this, true);
			return;
		}

		var totalConcentration = CharacterOwner.Effects.OfType<IConcentrationConsumingEffect>()
		                                       .Sum(x => x.ConcentrationPointsConsumed);
		var difficulty = capability
		                 .GetConcentrationDifficulty(CharacterOwner, totalConcentration / concentrationCapability,
			                 ConcentrationPointsConsumed)
		                 .Highest(wound.ConcentrationDifficulty);
		var check = Gameworld.GetCheck(CheckType.MagicConcentrationOnWounded);
		var outcome = check.Check(CharacterOwner, difficulty);
		if (totalConcentration > concentrationCapability && outcome.Outcome.IsFail())
		{
			CharacterOwner.RemoveEffect(this, true);
			return;
		}

		if (outcome.Outcome == Outcome.MajorFail)
		{
			CharacterOwner.RemoveEffect(this, true);
			return;
		}
	}

	public virtual void ReleaseEvents()
	{
		CharacterOwner.OnWounded -= CharacterOwner_OnWounded;
		CharacterOwner.OnQuit -= CharacterOwner_OnQuit;
	}
}
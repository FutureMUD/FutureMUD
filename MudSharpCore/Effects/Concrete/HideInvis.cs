using System;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class HideInvis : Effect, IHideEffect
{
	public HideInvis(IPerceivable owner, double effectiveHideSkill)
		: base(owner)
	{
		EffectiveHideSkill = effectiveHideSkill;
	}

	public HideInvis(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		EffectiveHideSkill = double.Parse(effect.Element("Skill").Value);
	}

	public double EffectiveHideSkill { get; set; }

	protected override string SpecificEffectType => "HideInvis";

	public override bool SavingEffect => true;

	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (target is not IPerceiver voyeur || voyeur == Owner)
		{
			return false;
		}

		if (!base.Applies(target))
		{
			return false;
		}

		if (voyeur.AffectedBy<ISawHiderEffect>(Owner) || flags.HasFlag(PerceiveIgnoreFlags.IgnoreHiding))
		{
			return false;
		}

		if (voyeur is IPerceivableHaveTraits perceiverHasTraits && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreSpotting))
		{
			var outcome = Gameworld.GetCheck(CheckType.SpotStealthCheck)
			                       .Check(perceiverHasTraits, perceiverHasTraits.Location.SpotDifficulty(voyeur),
				                       Owner);
			if (outcome.Outcome.IsPass())
			{
				perceiverHasTraits.AddEffect(new SawHider(perceiverHasTraits, Owner), TimeSpan.FromSeconds(60));
				return false;
			}
		}

		return true;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Hiding at {EffectiveHideSkill.ToString("N2", voyeur).Colour(Telnet.Green)} effective skill.";
	}

	public override PerceptionTypes Obscuring => PerceptionTypes.DirectVisual;

	protected override XElement SaveDefinition()
	{
		return new XElement("Skill", EffectiveHideSkill);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("HideInvis", (effect, owner) => new HideInvis(effect, owner));
	}

	public override string ToString()
	{
		return "Hide Invis Effect";
	}
}
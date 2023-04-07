using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class WellRested : Effect, ICheckBonusEffect, IScoreAddendumEffect
{
	public WellRested(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		RestedMultiplier = 1;
	}

	public WellRested(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		RestedMultiplier = int.Parse(effect.Element("RestedMultiplier").Value);
	}

	public bool ShowInScore => true;
	public bool ShowInHealth => true;

	public string ScoreAddendum => "You feel well rested.".Colour(Telnet.BoldGreen);

	public static void InitialiseEffectType()
	{
		RegisterFactory("WellRested", (effect, owner) => new WellRested(effect, owner));
	}

	public int RestedMultiplier { get; set; }

	public void RenewRest()
	{
		Owner.RescheduleIfLonger(this,
			TimeSpan.FromSeconds(RestedMultiplier * Gameworld.GetStaticDouble("WellRestedDuration")));
		RestedMultiplier = 1;
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"They are well rested, giving them a bonus to all checks.";
	}

	protected override string SpecificEffectType => "WellRested";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("RestedMultiplier", RestedMultiplier);
	}

	public override void ExpireEffect()
	{
		if (Owner is ICharacter character)
		{
			RestedMultiplier += 1;
			if (RestedMultiplier > 8)
			{
				RestedMultiplier = 8;
			}

			if (character.State.HasFlag(CharacterState.Sleeping))
			{
				//renew well rested duration
				character.RescheduleIfLonger(this,
					TimeSpan.FromSeconds(Gameworld.GetStaticDouble("WellRestedDuration")));
				Owner.Send(RestedMultiplier == 8
					? "You feel as well rested as you are going to get."
					: "You feel even more well rested.");
				return;
			}
		}

		base.ExpireEffect();
		Owner.Send("You no longer feel well rested.");
	}

	#endregion

	#region Implementation of ICheckBonusEffect

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsGeneralActivityCheck();
	}

	public double CheckBonus => Gameworld.GetStaticDouble("WellRestedBonus") * ((ICharacter)Owner).Merits
		.OfType<IRestedBonusMerit>().Where(x => x.Applies((ICharacter)Owner))
		.Aggregate(1.0, (x, y) => x * y.Multiplier);

	#endregion
}
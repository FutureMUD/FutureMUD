using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class Anesthesia : Effect, ICheckBonusEffect
{
	private double _intensityPerGramMass;

	public Anesthesia(IBody owner, double intesitypergrammass) : base(owner)
	{
		_intensityPerGramMass = intesitypergrammass;
		IntensityToBonusConversionRate = owner.Gameworld.GetStaticDouble("AnasthesiaIntensityToBonusConversionRate");
	}

	public double IntensityToBonusConversionRate { get; set; }

	protected override string SpecificEffectType => "Anasthesia";

	public double IntensityPerGramMass
	{
		get => _intensityPerGramMass;
		set
		{
			if (value >= 5.0 && _intensityPerGramMass < 5.0)
			{
				Owner?.OutputHandler?.Send("Your autosomatic reflex to breathe has stopped.");
			}
			else if (value >= 1.5 && _intensityPerGramMass < 1.5)
			{
				Owner?.OutputHandler?.Send("You can hardly keep your eyes open.");
			}
			else if (value >= 1.0 && _intensityPerGramMass < 1.0)
			{
				Owner?.OutputHandler?.Send("You feel quite drowsy.");
			}
			else if (value >= 0.5 && _intensityPerGramMass < 0.5)
			{
				Owner?.OutputHandler?.Send("You begin to feel drowsy.");
			}
			else if (value < 5.0 && _intensityPerGramMass >= 5.0)
			{
				Owner?.OutputHandler?.Send("Your body's autosomatic reflex to breathe has kicked back in.");
			}
			else if (value < 1.5 && _intensityPerGramMass >= 1.5)
			{
				Owner?.OutputHandler?.Send("You're having a slightly easier time keeping your eyes open.");
			}
			else if (value < 1.0 && _intensityPerGramMass >= 1.0)
			{
				Owner?.OutputHandler?.Send("You no longer feel quite so drowsy.");
			}
			else if (value < 0.5 && _intensityPerGramMass >= 0.5)
			{
				Owner?.OutputHandler?.Send("You no longer feel drowsy.");
			}

			_intensityPerGramMass = value;
		}
	}

	public bool AppliesToCheck(CheckType type)
	{
		switch (type)
		{
			case CheckType.ExactTimeCheck:
			case CheckType.VagueTimeCheck:
			case CheckType.WoundCloseCheck:
			case CheckType.StunRecoveryCheck:
			case CheckType.PainRecoveryCheck:
			case CheckType.ShockRecoveryCheck:
			case CheckType.HealingCheck:
			case CheckType.InfectionHeartbeat:
			case CheckType.InfectionSpread:
				return false;
		}

		return true;
	}

	public double CheckBonus => -1 * IntensityPerGramMass * IntensityToBonusConversionRate;

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Anesthetic of intensity {IntensityPerGramMass:N2}, giving a penalty of {CheckBonus:N3} to skill checks.";
	}
}
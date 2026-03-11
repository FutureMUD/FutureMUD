#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class AdrenalineRush : Effect, ICheckBonusEffect, IScoreAddendumEffect
{
	private double _intensityPerGramMass;

	public AdrenalineRush(IBody owner, double intensityPerGramMass) : base(owner)
	{
		IntensityPerGramMass = intensityPerGramMass;
		CheckPenaltyPerIntensity = owner.Gameworld.GetStaticDouble("AdrenalineCheckPenaltyPerIntensity");
	}

	public double CheckPenaltyPerIntensity { get; set; }

	public double IntensityPerGramMass
	{
		get => _intensityPerGramMass;
		set
		{
			if (value >= 0.4 && _intensityPerGramMass < 0.4)
			{
				Owner.OutputHandler.Send("Your pulse begins to hammer in your chest.");
			}
			else if (value >= 1.0 && _intensityPerGramMass < 1.0)
			{
				Owner.OutputHandler.Send("A surge of adrenaline leaves your hands trembling and your heart racing.");
			}
			else if (value < 1.0 && _intensityPerGramMass >= 1.0)
			{
				Owner.OutputHandler.Send("Your tremors begin to ease, though your heart still races.");
			}
			else if (value < 0.4 && _intensityPerGramMass >= 0.4)
			{
				Owner.OutputHandler.Send("Your pulse begins to settle.");
			}

			_intensityPerGramMass = value;
		}
	}

	protected override string SpecificEffectType => "AdrenalineRush";

	public bool AppliesToCheck(CheckType type)
	{
		return type.IsGeneralActivityCheck();
	}

	public double CheckBonus => -1.0 * IntensityPerGramMass * CheckPenaltyPerIntensity;

	public bool ShowInScore => IntensityPerGramMass >= 0.2;
	public bool ShowInHealth => true;
	public string ScoreAddendum => "Your heart is racing.".Colour(Telnet.BoldRed);

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Adrenaline rush of intensity {IntensityPerGramMass:N2}, giving a penalty of {CheckBonus:N2} to general activity checks.";
	}
}

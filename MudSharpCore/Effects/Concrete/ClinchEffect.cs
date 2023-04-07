using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class ClinchEffect : CombatEffectBase, IAffectProximity
{
	public ClinchEffect(ICharacter clincher, ICharacter target) : base(clincher, clincher.Combat)
	{
		Clincher = clincher;
		Target = target;
	}

	protected override string SpecificEffectType => "ClinchEffect";
	public ICharacter Clincher { get; set; }
	public ICharacter Target { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"In a clinch with {Target.HowSeen(voyeur)}";
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (Target == thing)
		{
			return (true, Proximity.Intimate);
		}

		return (false, Proximity.Unapproximable);
	}
}
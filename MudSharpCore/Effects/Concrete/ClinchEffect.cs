using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public class ClinchEffect : CombatEffectBase, IAffectProximity
{
    public ClinchEffect(ICharacter clincher, ICharacter target) : base(clincher, clincher.Combat)
    {
        Clincher = clincher;
        Target = target;
		Clincher.OnLeaveCombat += Participant_OnLeaveCombat;
		Target.OnLeaveCombat += Participant_OnLeaveCombat;
    }

    protected override string SpecificEffectType => "ClinchEffect";
    public ICharacter Clincher { get; set; }
    public ICharacter Target { get; set; }

	private void Participant_OnLeaveCombat(IPerceivable participant)
	{
		ExpireEffect();
	}

	public override void RemovalEffect()
	{
		Clincher.OnLeaveCombat -= Participant_OnLeaveCombat;
		Target.OnLeaveCombat -= Participant_OnLeaveCombat;
		base.RemovalEffect();
	}

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

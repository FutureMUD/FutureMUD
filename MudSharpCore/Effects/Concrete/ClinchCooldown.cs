using MudSharp.Combat;

namespace MudSharp.Effects.Concrete;

public class ClinchCooldown : CombatEffectBase
{
    public ClinchCooldown(IPerceivable owner, ICombat combat, IFutureProg applicabilityProg = null)
        : base(owner, combat, applicabilityProg)
    {
    }

    public ClinchCooldown(XElement effect, IPerceivable owner) : base(effect, owner)
    {
    }

    #region Overrides of Effect

    public override string Describe(IPerceiver voyeur)
    {
        return "Unable to begin clinching.";
    }

    protected override string SpecificEffectType => "Clinch Cooldown";

    #endregion
}
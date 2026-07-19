using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Combat.Moves;

namespace MudSharp.Effects.Concrete;

public class BeingStrangled : Effect, IBodypartIneffectiveEffect, INoQuitEffect, ICombatEffect
{
    public BeingStrangled(IPerceivable owner, ICharacter strangler, IFutureProg applicabilityProg = null) : base(owner,
        applicabilityProg)
    {
        Strangler = strangler;
    }

    public ICharacter Strangler { get; set; }

    public IBodypart Bodypart { get; init; }

    public override void RemovalEffect()
    {
        base.RemovalEffect();
        (Owner as IBody)?.CheckHealthStatus();
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"{Bodypart.FullDescription()} is being choked.";
    }

    protected override string SpecificEffectType => "ResidualChoke";

    public string NoQuitReason => "You cannot quit so soon after having been choked.";
}
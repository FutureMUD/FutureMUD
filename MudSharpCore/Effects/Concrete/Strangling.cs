using MudSharp.Combat;
using MudSharp.Combat.Moves;

namespace MudSharp.Effects.Concrete;

public class Strangling : Effect, ICombatEffect, IEndOnCombatMove
{
    public Strangling(IPerceivable owner, ICharacter target, IFutureProg applicabilityProg = null) : base(owner,
        applicabilityProg)
    {
        Target = target;
    }

    public override void RemovalEffect()
    {
        base.RemovalEffect();
        Target.RemoveAllEffects(x => x is BeingStrangled bs && bs.Strangler == Owner, true);
    }

    public ICharacter Target { get; set; }

    public bool CausesToEnd(ICombatMove move)
    {
        return !(move is StrangleAttack);
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Strangling {Target.HowSeen(voyeur)}.";
    }

    protected override string SpecificEffectType => "Strangling";
}
using MudSharp.Body.Position;

namespace MudSharp.Effects.Concrete;

public class Staggered : Effect, IPreventPositionChange
{
    public Staggered(ICharacter owner) : base(owner)
    {
    }

    protected override string SpecificEffectType => "Staggered";

    public override string Describe(IPerceiver voyeur)
    {
        return "Recovering from a staggering blow";
    }

    public bool PreventsChange(IPositionState oldPosition, IPositionState newPosition)
    {
        return true;
    }

    public string WhyPreventsChange(IPositionState oldPosition, IPositionState newPosition)
    {
        return "You are still recovering from the staggering blow you took, you can't reposition.";
    }

    public override void ExpireEffect()
    {
        base.ExpireEffect();
        ((ICharacter)Owner).Send("You feel as if you have recovered from the staggering blow.");
    }
}
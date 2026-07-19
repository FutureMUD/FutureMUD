using MudSharp.Movement;

namespace MudSharp.Effects.Concrete;

public class PreFleeSpeed : Effect, IRemoveOnCombatEnd
{
    /// <inheritdoc />
    public PreFleeSpeed(ICharacter owner, IEnumerable<IMoveSpeed> speeds) : base(owner, null)
    {
        Speeds = speeds;
    }


    /// <inheritdoc />
    public override string Describe(IPerceiver voyeur)
    {
        return $"Remembering to go back to {Speeds.SelectNotNull(x => x?.PresentParticiple).ListToColouredString()} at combat end.";
    }

    /// <inheritdoc />
    protected override string SpecificEffectType => "PreFleeSpeed";

    public IEnumerable<IMoveSpeed> Speeds { get; }

    /// <inheritdoc />
    public override void RemovalEffect()
    {
        ICharacter ch = (ICharacter)Owner;
        foreach (IMoveSpeed speed in Speeds)
        {
            if (speed is null)
            {
                continue;
            }
            ch.CurrentSpeeds[speed.Position] = speed;
        }
    }
}

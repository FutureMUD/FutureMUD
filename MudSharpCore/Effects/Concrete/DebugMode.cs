
namespace MudSharp.Effects.Concrete;

public class DebugMode : Effect
{
    public DebugMode(IPerceivable owner) : base(owner, null)
    {
    }

    protected override string SpecificEffectType => "DebugMode";

    public override string Describe(IPerceiver voyeur)
    {
        return "Tuning in to debug messages.";
    }
}
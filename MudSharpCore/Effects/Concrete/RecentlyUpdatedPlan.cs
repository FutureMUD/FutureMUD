
namespace MudSharp.Effects.Concrete;

public class RecentlyUpdatedPlan : Effect
{
    public RecentlyUpdatedPlan(ICharacter owner) : base(owner, null)
    {
    }

    public RecentlyUpdatedPlan(XElement root, IPerceivable owner) : base(root, owner)
    {
    }

    public override bool SavingEffect => true;

    public static void InitialiseEffectType()
    {
        RegisterFactory("RecentlyUpdatedPlan", (effect, owner) => new RecentlyUpdatedPlan(effect, owner));
    }

    public override string Describe(IPerceiver voyeur)
    {
        return "Recently updated a short or long term plan";
    }

    protected override string SpecificEffectType => "RecentlyUpdatedPlan";
}
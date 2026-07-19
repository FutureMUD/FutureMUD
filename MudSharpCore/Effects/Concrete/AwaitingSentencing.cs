using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;

namespace MudSharp.Effects.Concrete;

public class AwaitingSentencing : Effect, IEffect, IScoreAddendumEffect
{
    #region Static Initialisation

    public static void InitialiseEffectType()
    {
        RegisterFactory("AwaitingSentencing", (effect, owner) => new AwaitingSentencing(effect, owner));
    }

    #endregion

    public ILegalAuthority LegalAuthority { get; set; }
    public MudDateTime ArrestTime { get; set; }

    #region Constructors

    public AwaitingSentencing(ICharacter owner, ILegalAuthority legalAuthority, MudDateTime arrestTime) : base(owner,
        null)
    {
        LegalAuthority = legalAuthority;
        ArrestTime = arrestTime;
    }

    protected AwaitingSentencing(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        XElement root = effect.Element("Effect");
        LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
        ArrestTime = MudDateTime.FromStoredStringOrFallback(root.Element("ArrestTime").Value, Gameworld,
            StoredMudDateTimeFallback.CurrentDateTime, "Effect:AwaitingSentencing", Owner?.Id, Owner?.Name, "ArrestTime");
    }

    #endregion

    #region Saving and Loading

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("LegalAuthority", LegalAuthority.Id),
            new XElement("ArrestTime", new XCData(ArrestTime.GetDateTimeString()))
        );
    }

    #endregion

    #region Overrides of Effect

    protected override string SpecificEffectType => "AwaitingSentencing";

    public override string Describe(IPerceiver voyeur)
    {
        return
            $"Awaiting sentencing in the {LegalAuthority.Name.ColourName()} jurisdiction since {ArrestTime.ToString(TimeAndDate.Date.CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}.";
    }

    public override bool SavingEffect => true;

    public bool ShowInScore => true;

    public bool ShowInHealth => false;

    public string ScoreAddendum =>
        $"You are on remand in the {LegalAuthority.Name.ColourName()} jurisdiction since {ArrestTime.ToString(TimeAndDate.Date.CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}.";

    public override bool Applies(object target)
    {
        if (target is ILegalAuthority authority)
        {
            return base.Applies(target) && authority == LegalAuthority;
        }

        return base.Applies(target);
    }

    #endregion
}

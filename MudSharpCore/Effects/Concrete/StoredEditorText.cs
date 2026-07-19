
namespace MudSharp.Effects.Concrete;

public class StoredEditorText : Effect, IEffectSubtype
{
    public string Text { get; set; }

    public StoredEditorText(IPerceivable owner, string text, IFutureProg applicabilityProg = null) : base(owner,
        applicabilityProg)
    {
        Text = text;
    }

    public override string Describe(IPerceiver voyeur)
    {
        return "Has stored editor text from a recent trip to the editor.";
    }

    protected override string SpecificEffectType => "StoredEditorText";
}
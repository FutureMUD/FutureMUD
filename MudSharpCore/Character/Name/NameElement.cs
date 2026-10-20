
namespace MudSharp.Character.Name;

public class NameElement : FrameworkItem
{
    public NameElement(NameUsage usage, string text, bool confirmed = false, bool? display = null)
    {
        Text = text;
        Usage = usage;
		Confirmed = confirmed;
		Display = display;
    }

    public override string FrameworkItemType => "NameElement";

    public string Text { get; protected set; }
    public NameUsage Usage { get; protected set; }
	public bool Confirmed { get; }
	public bool? Display { get; }
}
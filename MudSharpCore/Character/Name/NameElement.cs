using MudSharp.Framework;

namespace MudSharp.Character.Name;

public class NameElement : FrameworkItem
{
	public NameElement(NameUsage usage, string text)
	{
		Text = text;
		Usage = usage;
	}

	public override string FrameworkItemType => "NameElement";

	public string Text { get; protected set; }
	public NameUsage Usage { get; protected set; }
}
using MudSharp.Framework;

namespace MudSharp.PerceptionEngine.Outputs;

public class RawOutput : Output
{
	public RawOutput(string text,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(visibility, style, flags)
	{
		Text = text;
	}

	public string Text { get; protected set; }

	public override string RawString => Text;

	public override string ParseFor(IPerceiver perceiver)
	{
		return Text;
	}
}
using System.Text;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

public class PriorEmoteOutput : AppendableEmoteOutput
{
	public PriorEmoteOutput(string defaultEmote, IPerceiver defaultSource, bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags)
	{
	}

	public PriorEmoteOutput(IEmote emote, OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal, OutputFlags flags = OutputFlags.Normal)
		: base(emote, visibility, style, flags)
	{
	}

	public override string ParseFor(IPerceiver perceiver)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(EmoteToAppend.RawText))
		{
			sb.Append(EmoteToAppend.ParseFor(perceiver) + ", ");
		}

		if (_assignedEmotes.IsValueCreated && _assignedEmotes.Value.ContainsKey(perceiver))
		{
			sb.Append(_assignedEmotes.Value[perceiver].ParseFor(perceiver));
		}
		else
		{
			sb.Append(DefaultEmote.ParseFor(perceiver));
		}

		return sb.ToString().ProperSentences().Fullstop();
	}
}
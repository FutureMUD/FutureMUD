using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

public class MixedEmoteOutput : AppendableEmoteOutput
{
	public MixedEmoteOutput(string defaultEmote, IPerceiver defaultSource, bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags)
	{
	}

	public MixedEmoteOutput(IEmote emote, OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal, OutputFlags flags = OutputFlags.Normal)
		: base(emote, visibility, style, flags)
	{
	}

	public override string ParseFor(IPerceiver perceiver)
	{
		string returnText;
		var flags = PerceiveIgnoreFlags.None;
		if (Style.HasFlag(OutputStyle.IgnoreLiquidsAndFlags))
		{
			flags = PerceiveIgnoreFlags.IgnoreLiquidsAndFlags;
		}

		if (_assignedEmotes.IsValueCreated && _assignedEmotes.Value.ContainsKey(perceiver))
		{
			returnText = _assignedEmotes.Value[perceiver].ParseFor(perceiver, flags) +
			             (EmoteToAppend != null && EmoteToAppend.RawText.Length > 0
				             ? ", " + EmoteToAppend.ParseFor(perceiver, flags)
				             : "");
		}
		else
		{
			returnText = DefaultEmote.ParseFor(perceiver, flags) +
			             (EmoteToAppend != null && EmoteToAppend.RawText.Length > 0
				             ? ", " + EmoteToAppend.ParseFor(perceiver, flags)
				             : "");
		}

		returnText = returnText.Fullstop().ProperSentences().NormaliseSpacing();
		return Flags.HasFlag(OutputFlags.WideWrap)
			? returnText.Wrap(perceiver.LineFormatLength)
			: returnText.Wrap(perceiver.InnerLineFormatLength);
	}
}
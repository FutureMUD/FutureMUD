using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

public abstract class AppendableEmoteOutput : EmoteOutput
{
	protected IEmote EmoteToAppend;

	protected AppendableEmoteOutput(string defaultEmote, IPerceiver defaultSource, bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags)
	{
	}

	protected AppendableEmoteOutput(IEmote emote, OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal, OutputFlags flags = OutputFlags.Normal)
		: base(emote, visibility, style, flags)
	{
	}

	/// <summary>
	///     Fluent method to append a player's input emote onto this emote output, to be appended at the end.
	/// </summary>
	/// <param name="emote">A player input processed emote.</param>
	public AppendableEmoteOutput Append(IEmote emote)
	{
		EmoteToAppend = emote;
		return this;
	}
}
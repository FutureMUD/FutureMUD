using System;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

/// <summary>
///     A FilteredEmoteOutput is a form of EmoteOutput that includes a Func(IPerceiver,bool) which filters whether someone
///     will see it based on some condition.
/// </summary>
public class FilteredEmoteOutput : EmoteOutput
{
	private readonly Func<IPerceiver, bool> _filterFunction;

	public FilteredEmoteOutput(string defaultEmote, IPerceiver defaultSource, Func<IPerceiver, bool> filterFunction,
		bool forceSourceInclusion = false, OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal, OutputFlags flags = OutputFlags.Normal)
		: base(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags)
	{
		_filterFunction = filterFunction;
	}

	public FilteredEmoteOutput(Emote emote, Func<IPerceiver, bool> filterFunction,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(emote, visibility, style, flags)
	{
		_filterFunction = filterFunction;
	}

	public override bool ShouldSee(IPerceiver perceiver)
	{
		return base.ShouldSee(perceiver) && _filterFunction(perceiver);
	}
}
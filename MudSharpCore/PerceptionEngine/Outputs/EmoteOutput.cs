using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine.Outputs;

public class EmoteOutput : Output, IEmoteOutput
{
	protected Lazy<Dictionary<IPerceiver, IEmote>> _assignedEmotes = new();

	/// <summary>
	///     Handles emote outputs to players and items.
	/// </summary>
	/// <param name="defaultSource">The source of the default emote.</param>
	/// <param name="defaultEmote">The default output shown to all viewers unless assigned to another emote.</param>
	/// <param name="forceSourceInclusion">
	///     If the PersonalDescriptionDelimiter (currently '@') is not given, forces the
	///     inclusion of the source's Short Description at the beginning of the emote.
	/// </param>
	/// <param name="visibility">What kind of output it is, whether IC or OOC or a specific subset of either.</param>
	/// <param name="style">What visual style should be applied to the output.</param>
	/// <param name="flags">Any additional flags to be raised that modify the output and how it should be shown.</param>
	public EmoteOutput(string defaultEmote,
		IPerceiver defaultSource,
		bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(visibility, style, flags)
	{
		AllValid = true;
		DefaultSource = defaultSource;
		DefaultEmote = new Emote(defaultEmote, defaultSource, forceSourceInclusion);
		AllValid &= DefaultEmote.Valid;
	}

	/// <summary>
	///     Handles emote outputs to players and items.
	/// </summary>
	/// <param name="emote">The default emote.</param>
	/// <param name="visibility">What kind of output it is, whether IC or OOC or a specific subset of either.</param>
	/// <param name="style">What visual style should be applied to the output.</param>
	/// <param name="flags">Any additional flags to be raised that modify the output and how it should be shown.</param>
	public EmoteOutput(IEmote emote,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(visibility, style, flags)
	{
		AllValid = true;
		DefaultSource = emote.Source;
		DefaultEmote = emote;
		AllValid &= DefaultEmote.Valid;
	}

	public EmoteOutput(EmoteOutput rhs)
	{
		DefaultEmote = rhs.DefaultEmote;
		DefaultSource = rhs.DefaultSource;
		NoticeCheckDifficulty = rhs.NoticeCheckDifficulty;
		Flags = rhs.Flags;
		Style = rhs.Style;
		Visibility = rhs.Visibility;
		AllValid = rhs.AllValid;
	}

	public IEmote DefaultEmote { get; protected set; }

	public IPerceiver DefaultSource { get; protected set; }

	public bool AllValid { get; protected set; }

	public override string RawString => DefaultEmote.ErrorMessage;

	public void AssignEmote(string emote, bool forceSourceInclusion = false, params IPerceiver[] perceivers)
	{
		var parsedEmote = new Emote(emote, DefaultSource, forceSourceInclusion);
		AllValid &= parsedEmote.Valid;

		foreach (var perceiver in perceivers)
		{
			_assignedEmotes.Value.Add(perceiver, parsedEmote);
		}
	}

	public void AssignEmote(IPerceiver source, string emote, bool forceSourceInclusion = false,
		params IPerceiver[] perceivers)
	{
		var parsedEmote = new Emote(emote, source, forceSourceInclusion);
		AllValid &= parsedEmote.Valid;

		foreach (var perceiver in perceivers)
		{
			_assignedEmotes.Value.Add(perceiver, parsedEmote);
		}
	}

	public override string ParseFor(IPerceiver perceiver)
	{
		string returnText;
		var fixedFormat = false;
		var flags = PerceiveIgnoreFlags.None;
		if (Style.HasFlag(OutputStyle.IgnoreLiquidsAndFlags))
		{
			flags = PerceiveIgnoreFlags.IgnoreLiquidsAndFlags;
		}

		if (perceiver is null)
		{
			flags = PerceiveIgnoreFlags.TrueDescription | PerceiveIgnoreFlags.IgnoreLiquidsAndFlags;
		}

		if (_assignedEmotes.IsValueCreated && _assignedEmotes.Value.ContainsKey(perceiver))
		{
			returnText = _assignedEmotes.Value[perceiver].ParseFor(perceiver, flags);
			fixedFormat = _assignedEmotes.Value[perceiver].FixedFormat;
		}
		else
		{
			returnText = DefaultEmote.ParseFor(perceiver, flags);
			fixedFormat = DefaultEmote.FixedFormat;
		}

		if (Style.HasFlag(OutputStyle.Explosion))
		{
			// TODO
		}

		if (Style == OutputStyle.TextFragment)
		{
			return returnText.NormaliseSpacing();
		}

		if (!fixedFormat)
		{
			returnText = returnText.Fullstop().NormaliseOutputSentences().NormaliseSpacing();
		}

		return Flags.HasFlag(OutputFlags.WideWrap)
			? returnText.Wrap(perceiver?.LineFormatLength ?? 120)
			: returnText.Wrap(perceiver?.InnerLineFormatLength ?? 80);
	}

	public Difficulty NoticeCheckDifficulty { get; set; } = Difficulty.Normal;

	public override bool ShouldSee(IPerceiver perceiver)
	{
		return
			base.ShouldSee(perceiver) &&
			(!perceiver.BriefCombatMode || !Style.HasFlag(OutputStyle.CombatMessage) ||
			 DefaultEmote.Targets.Any(perceiver.IsPersonOfInterest)) &&
			(!Flags.HasFlag(OutputFlags.Insigificant) || !perceiver.BriefCombatMode) &&
			(!Flags.HasFlag(OutputFlags.PurelyAudible) || perceiver.CanHear(DefaultSource)) &&
			(!Flags.HasFlag(OutputFlags.SuppressObscured) || perceiver.CanSee(DefaultSource)) &&
			(!Flags.HasFlag(OutputFlags.SuppressSource) || !perceiver.IsSelf(DefaultSource)) &&
			(!(perceiver is IPerceivableHaveTraits) || !Flags.HasFlag(OutputFlags.NoticeCheckRequired) ||
			 perceiver.IsSelf(DefaultSource) || perceiver.Gameworld.GetCheck(CheckType.NoticeCheck)
			                                             .Check((IPerceivableHaveTraits)perceiver,
				                                             NoticeCheckDifficulty, DefaultSource)
			                                             .IsPass())
			;
	}
}
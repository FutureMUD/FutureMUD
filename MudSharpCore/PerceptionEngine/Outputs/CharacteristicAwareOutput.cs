using System.Text;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

/// <summary>
///     This is used for a special case of output where some characteristic information needs to be left until the emote is
///     actually parsed to be included
/// </summary>
public class CharacteristicAwareOutput : MixedEmoteOutput
{
	private readonly string _characteristicText;
	private readonly bool _ignoreObscurers;
	private readonly IHaveCharacteristics _owner;

	public CharacteristicAwareOutput(string defaultEmote, IPerceiver defaultSource, IHaveCharacteristics owner,
		string characteristicText, bool ignoreObscurers = false, bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags)
	{
		_ignoreObscurers = ignoreObscurers;
		_characteristicText = characteristicText;
		_owner = owner;
	}

	public CharacteristicAwareOutput(IEmote emote, IHaveCharacteristics owner, string characteristicText,
		bool ignoreObscurers = false, OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal, OutputFlags flags = OutputFlags.Normal)
		: base(emote, visibility, style, flags)
	{
		_ignoreObscurers = ignoreObscurers;
		_characteristicText = characteristicText;
		_owner = owner;
	}

	public override string ParseFor(IPerceiver perceiver)
	{
		var sb = new StringBuilder();
		var flags = PerceiveIgnoreFlags.None;
		if (Style.HasFlag(OutputStyle.IgnoreLiquidsAndFlags))
		{
			flags = PerceiveIgnoreFlags.IgnoreLiquidsAndFlags;
		}

		sb.Append(DefaultEmote.ParseFor(perceiver, flags) + ", ");
		sb.Append(
			new Emote(
				(DefaultSource as IHaveCharacteristics).ParseCharacteristics(
					_owner.ParseCharacteristics(_characteristicText, perceiver), perceiver, _ignoreObscurers),
				DefaultSource).ParseFor(perceiver, flags));
		if (!string.IsNullOrWhiteSpace(EmoteToAppend?.RawText))
		{
			sb.Append(", " + EmoteToAppend.ParseFor(perceiver, flags));
		}

		var returnText = sb.ToString().Fullstop().ProperSentences().NormaliseSpacing();
		return Flags.HasFlag(OutputFlags.InnerWrap) ? returnText.Wrap(perceiver.InnerLineFormatLength) : returnText;
	}
}

public class CharacteristicAwarePriorOutput : PriorEmoteOutput
{
	private readonly string _characteristicText;
	private readonly bool _ignoreObscurers;
	private readonly IHaveCharacteristics _owner;

	public CharacteristicAwarePriorOutput(string defaultEmote, IPerceiver defaultSource, IHaveCharacteristics owner,
		string characteristicText, bool ignoreObscurers = false, bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags)
	{
		_ignoreObscurers = ignoreObscurers;
		_characteristicText = characteristicText;
		_owner = owner;
	}

	public CharacteristicAwarePriorOutput(IEmote emote, IHaveCharacteristics owner, string characteristicText,
		bool ignoreObscurers = false, OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal, OutputFlags flags = OutputFlags.Normal)
		: base(emote, visibility, style, flags)
	{
		_ignoreObscurers = ignoreObscurers;
		_characteristicText = characteristicText;
		_owner = owner;
	}

	public override string ParseFor(IPerceiver perceiver)
	{
		var flags = PerceiveIgnoreFlags.None;
		if (Style.HasFlag(OutputStyle.IgnoreLiquidsAndFlags))
		{
			flags = PerceiveIgnoreFlags.IgnoreLiquidsAndFlags;
		}

		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(EmoteToAppend.RawText))
		{
			sb.Append(EmoteToAppend.ParseFor(perceiver, flags) + ", ");
		}

		sb.Append(DefaultEmote.ParseFor(perceiver, flags) + ", ");
		sb.Append(
			new Emote(
				(DefaultSource as IHaveCharacteristics).ParseCharacteristics(
					_owner.ParseCharacteristics(_characteristicText, perceiver), perceiver, _ignoreObscurers),
				DefaultSource).ParseFor(perceiver, flags));

		var returnText = sb.ToString().Fullstop().ProperSentences().NormaliseSpacing();
		return Flags.HasFlag(OutputFlags.WideWrap)
			? returnText.Wrap(perceiver.LineFormatLength)
			: returnText.Wrap(perceiver.InnerLineFormatLength);
	}
}
using System;
using MudSharp.Communication.Language;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

public static class EmoteFactory
{
	public static AppendableEmoteOutput CreateEmote(IFuturemud gameworld, IEmote emote,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
	{
		return gameworld.GetStaticConfiguration("PrependEmotes")
		                .Equals("true", StringComparison.InvariantCultureIgnoreCase)
			? (AppendableEmoteOutput)new PriorEmoteOutput(emote, visibility, style, flags)
			: new MixedEmoteOutput(emote, visibility, style, flags);
	}

	public static AppendableEmoteOutput CreateEmote(IFuturemud gameworld, string defaultEmote,
		IPerceiver defaultSource, bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
	{
		return gameworld.GetStaticConfiguration("PrependEmotes")
		                .Equals("true", StringComparison.InvariantCultureIgnoreCase)
			? (AppendableEmoteOutput)
			new PriorEmoteOutput(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags)
			: new MixedEmoteOutput(defaultEmote, defaultSource, forceSourceInclusion, visibility, style, flags);
	}

	public static AppendableEmoteOutput CreateCharacteristicEmote(IFuturemud gameworld, IEmote emote,
		IHaveCharacteristics owner,
		string characteristicText, bool ignoreObscurers = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
	{
		return gameworld.GetStaticConfiguration("PrependEmotes")
		                .Equals("true", StringComparison.InvariantCultureIgnoreCase)
			? (AppendableEmoteOutput)
			new CharacteristicAwarePriorOutput(emote, owner, characteristicText, ignoreObscurers, visibility,
				style, flags)
			: new CharacteristicAwareOutput(emote, owner, characteristicText, ignoreObscurers, visibility, style,
				flags);
	}

	public static AppendableEmoteOutput CreateCharacteristicEmote(IFuturemud gameworld, string defaultEmote,
		IPerceiver defaultSource, IHaveCharacteristics owner, string characteristicText,
		bool ignoreObscurers = false, bool forceSourceInclusion = false,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
	{
		return gameworld.GetStaticConfiguration("PrependEmotes")
		                .Equals("true", StringComparison.InvariantCultureIgnoreCase)
			? (AppendableEmoteOutput)
			new CharacteristicAwarePriorOutput(defaultEmote, defaultSource, owner, characteristicText,
				ignoreObscurers, forceSourceInclusion, visibility,
				style, flags)
			: new CharacteristicAwareOutput(defaultEmote, defaultSource, owner, characteristicText, ignoreObscurers,
				forceSourceInclusion, visibility, style,
				flags);
	}

	public static Output CreateLanguageEmote(IFuturemud gameworld, IEmote prelanguageEmote,
		LanguageInfo language,
		IEmote optionalOutput,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
	{
		return gameworld.GetStaticConfiguration("PrependEmotes")
		                .Equals("true", StringComparison.InvariantCultureIgnoreCase)
			? (Output)
			new PriorLanguageOutput(prelanguageEmote, language, optionalOutput, visibility, style, flags)
			: new LanguageOutput(prelanguageEmote, language, optionalOutput, visibility, style, flags);
	}
}